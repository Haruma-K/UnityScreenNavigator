using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.AssetLoader;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    [RequireComponent(typeof(RectMask2D))]
    public sealed class PageContainer : MonoBehaviour, IScreenContainer
    {
        private static readonly Dictionary<int, PageContainer> InstanceCacheByTransform =
            new Dictionary<int, PageContainer>();

        private static readonly Dictionary<string, PageContainer> InstanceCacheByName =
            new Dictionary<string, PageContainer>();

        [SerializeField] private string _name;

        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _assetLoadHandles
            = new Dictionary<string, AssetLoadHandle<GameObject>>();

        private readonly List<IPageContainerCallbackReceiver> _callbackReceivers =
            new List<IPageContainerCallbackReceiver>();

        private readonly List<string> _orderedPageIds = new List<string>();

        private readonly Dictionary<string, Page> _pages = new Dictionary<string, Page>();

        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _preloadedResourceHandles =
            new Dictionary<string, AssetLoadHandle<GameObject>>();

        private IAssetLoader _assetLoader;

        private CanvasGroup _canvasGroup;

        private bool _isActivePageStacked;
        public static List<PageContainer> Instances { get; } = new List<PageContainer>();

        private ScreenContainerTransitionHandler _transitionHandler;

        /// <summary>
        ///     By default, <see cref="IAssetLoader" /> in <see cref="UnityScreenNavigatorSettings" /> is used.
        ///     If this property is set, it is used instead.
        /// </summary>
        public IAssetLoader AssetLoader
        {
            get => _assetLoader ?? UnityScreenNavigatorSettings.Instance.AssetLoader;
            set => _assetLoader = value;
        }

        /// <summary>
        ///     True if in transition.
        /// </summary>
        public bool IsInTransition => _transitionHandler.IsInTransition;

        /// <summary>
        ///     List of PageIds sorted in the order they are stacked. 
        /// </summary>
        public IReadOnlyList<string> OrderedPagesIds => _orderedPageIds;

        /// <summary>
        ///     Map of PageId to Page.
        /// </summary>
        public IReadOnlyDictionary<string, Page> Pages => _pages;

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        private void Awake()
        {
            Instances.Add(this);

            _callbackReceivers.AddRange(GetComponents<IPageContainerCallbackReceiver>());
            if (!string.IsNullOrWhiteSpace(_name)) InstanceCacheByName.Add(_name, this);

            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _transitionHandler = new ScreenContainerTransitionHandler(this);
        }

        private void OnDestroy()
        {
            foreach (var pageId in _orderedPageIds)
            {
                var page = _pages[pageId];
                var assetLoadHandle = _assetLoadHandles[pageId];

                if (UnityScreenNavigatorSettings.Instance.CallCleanupWhenDestroy)
                    page.BeforeReleaseAndForget();
                Destroy(page.gameObject);
                AssetLoader.Release(assetLoadHandle);
            }

            _assetLoadHandles.Clear();
            _pages.Clear();
            _orderedPageIds.Clear();

            InstanceCacheByName.Remove(_name);
            var keysToRemove = new List<int>();
            foreach (var cache in InstanceCacheByTransform)
                if (Equals(cache.Value))
                    keysToRemove.Add(cache.Key);

            foreach (var keyToRemove in keysToRemove) InstanceCacheByTransform.Remove(keyToRemove);

            Instances.Remove(this);
        }

        /// <summary>
        ///     Get the <see cref="PageContainer" /> that manages the page to which <see cref="transform" /> belongs.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="transform" />.</param>
        /// <returns></returns>
        public static PageContainer Of(Transform transform, bool useCache = true)
        {
            return Of((RectTransform)transform, useCache);
        }

        /// <summary>
        ///     Get the <see cref="PageContainer" /> that manages the page to which <see cref="rectTransform" /> belongs.
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="rectTransform" />.</param>
        /// <returns></returns>
        public static PageContainer Of(RectTransform rectTransform, bool useCache = true)
        {
            var id = rectTransform.GetInstanceID();
            if (useCache && InstanceCacheByTransform.TryGetValue(id, out var container)) return container;

            container = rectTransform.GetComponentInParent<PageContainer>();
            if (container != null)
            {
                InstanceCacheByTransform.Add(id, container);
                return container;
            }

            return null;
        }

        /// <summary>
        ///     Find the <see cref="PageContainer" /> of <see cref="containerName" />.
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static PageContainer Find(string containerName)
        {
            if (InstanceCacheByName.TryGetValue(containerName, out var instance)) return instance;

            return null;
        }

        /// <summary>
        ///     Add a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void AddCallbackReceiver(IPageContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Add(callbackReceiver);
        }

        /// <summary>
        ///     Remove a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void RemoveCallbackReceiver(IPageContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Remove(callbackReceiver);
        }

        /// <summary>
        ///     Push new page.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="playAnimation"></param>
        /// <param name="stack"></param>
        /// <param name="pageId"></param>
        /// <param name="loadAsync"></param>
        /// <param name="onLoad"></param>
        /// <returns></returns>
        public AsyncProcessHandle Push(string resourceKey, bool playAnimation, bool stack = true, string pageId = null,
            bool loadAsync = true, Action<(string pageId, Page page)> onLoad = null)
        {
            return CoroutineManager.Instance.Run(PushRoutine(typeof(Page), resourceKey, playAnimation, stack, onLoad,
                loadAsync, pageId));
        }

        /// <summary>
        ///     Push new page.
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="resourceKey"></param>
        /// <param name="playAnimation"></param>
        /// <param name="stack"></param>
        /// <param name="pageId"></param>
        /// <param name="loadAsync"></param>
        /// <param name="onLoad"></param>
        /// <returns></returns>
        public AsyncProcessHandle Push(Type pageType, string resourceKey, bool playAnimation, bool stack = true,
            string pageId = null, bool loadAsync = true, Action<(string pageId, Page page)> onLoad = null)
        {
            return CoroutineManager.Instance.Run(PushRoutine(pageType, resourceKey, playAnimation, stack, onLoad,
                loadAsync, pageId));
        }

        /// <summary>
        ///     Push new page.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="playAnimation"></param>
        /// <param name="stack"></param>
        /// <param name="pageId"></param>
        /// <param name="loadAsync"></param>
        /// <param name="onLoad"></param>
        /// <typeparam name="TPage"></typeparam>
        /// <returns></returns>
        public AsyncProcessHandle Push<TPage>(string resourceKey, bool playAnimation, bool stack = true,
            string pageId = null, bool loadAsync = true, Action<(string pageId, TPage page)> onLoad = null)
            where TPage : Page
        {
            return CoroutineManager.Instance.Run(PushRoutine(typeof(TPage), resourceKey, playAnimation, stack,
                x => onLoad?.Invoke((x.pageId, (TPage)x.page)), loadAsync, pageId));
        }

        /// <summary>
        ///     Pop pages.
        /// </summary>
        /// <param name="playAnimation"></param>
        /// <param name="popCount"></param>
        /// <returns></returns>
        public AsyncProcessHandle Pop(bool playAnimation, int popCount = 1)
        {
            return CoroutineManager.Instance.Run(PopRoutine(playAnimation, popCount));
        }

        /// <summary>
        ///     Pop pages.
        /// </summary>
        /// <param name="playAnimation"></param>
        /// <param name="destinationPageId"></param>
        /// <returns></returns>
        public AsyncProcessHandle Pop(bool playAnimation, string destinationPageId)
        {
            var popCount = 0;
            for (var i = _orderedPageIds.Count - 1; i >= 0; i--)
            {
                var pageId = _orderedPageIds[i];
                if (pageId == destinationPageId)
                    break;
                
                popCount++;
            }

            if (popCount == _orderedPageIds.Count)
                throw new Exception($"The page with id '{destinationPageId}' is not found.");

            return CoroutineManager.Instance.Run(PopRoutine(playAnimation, popCount));
        }

        private IEnumerator PushRoutine(Type pageType, string resourceKey, bool playAnimation, bool stack = true,
            Action<(string pageId, Page page)> onLoad = null, bool loadAsync = true, string pageId = null)
        {
            if (resourceKey == null)
                throw new ArgumentNullException(nameof(resourceKey));

            if (IsInTransition)
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");
            
            _transitionHandler.Begin();

            // Setup
            var assetLoadHandle = loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourceKey)
                : AssetLoader.Load<GameObject>(resourceKey);
            if (!assetLoadHandle.IsDone) yield return new WaitUntil(() => assetLoadHandle.IsDone);

            if (assetLoadHandle.Status == AssetLoadStatus.Failed) throw assetLoadHandle.OperationException;

            var instance = Instantiate(assetLoadHandle.Result);
            if (!instance.TryGetComponent(pageType, out var c))
                c = instance.AddComponent(pageType);

            var context = PagePushContext.Create(pageId, (Page)c, _orderedPageIds, _pages, stack);
            var lifecycleHandler = new PageLifecycleHandler(_callbackReceivers);

            _assetLoadHandles.Add(context.EnterPageId, assetLoadHandle);
            onLoad?.Invoke((context.EnterPageId, context.EnterPage));
            var afterLoadHandle = context.EnterPage.AfterLoad((RectTransform)transform);
            while (!afterLoadHandle.IsTerminated)
                yield return null;

            // Preprocess
            var preprocessHandles = lifecycleHandler.BeforePush(context);
            foreach (var coroutineHandle in preprocessHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // Play Animations
            var animationHandles = new List<AsyncProcessHandle>();
            if (context.ExitPage != null)
                animationHandles.Add(context.ExitPage.Exit(true, playAnimation, context.EnterPage));

            animationHandles.Add(context.EnterPage.Enter(true, playAnimation, context.ExitPage));

            foreach (var coroutineHandle in animationHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // End Transition
            if (!_isActivePageStacked && context.ExitPage != null)
            {
                _pages.Remove(context.ExitPageId);
                _orderedPageIds.Remove(context.ExitPageId);
            }

            _pages.Add(context.EnterPageId, context.EnterPage);
            _orderedPageIds.Add(context.EnterPageId);

            _transitionHandler.End();

            // Postprocess
            lifecycleHandler.AfterPush(context);

            // Unload Unused Page
            if (!_isActivePageStacked && context.ExitPage != null)
            {
                var beforeReleaseHandle = context.ExitPage.BeforeRelease();
                while (!beforeReleaseHandle.IsTerminated) yield return null;

                var handle = _assetLoadHandles[context.ExitPageId];
                AssetLoader.Release(handle);

                Destroy(context.ExitPage.gameObject);
                _assetLoadHandles.Remove(context.ExitPageId);
            }

            _isActivePageStacked = stack;
        }

        private IEnumerator PopRoutine(bool playAnimation, int popCount = 1)
        {
            Assert.IsTrue(popCount >= 1);

            if (_pages.Count < popCount)
                throw new InvalidOperationException(
                    "Cannot transition because the page count is less than the pop count.");

            if (IsInTransition)
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");

            _transitionHandler.Begin();

            var context = PagePopContext.Create(_orderedPageIds, _pages, popCount);
            var lifecycleHandler = new PageLifecycleHandler(_callbackReceivers);

            // Preprocess
            var preprocessHandles = lifecycleHandler.BeforePop(context);
            
            foreach (var coroutineHandle in preprocessHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // Play Animations
            var animationHandles = new List<AsyncProcessHandle>
            {
                context.ExitPage.Exit(false, playAnimation, context.EnterPage)
            };
            if (context.EnterPage != null) animationHandles.Add(context.EnterPage.Enter(false, playAnimation, context.ExitPage));

            foreach (var coroutineHandle in animationHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // End Transition
            for (var i = 0; i < context.ExitPageIds.Count; i++)
            {
                var unusedPageId = context.ExitPageIds[i];
                _pages.Remove(unusedPageId);
                _orderedPageIds.RemoveAt(_orderedPageIds.Count - 1);
            }

            _transitionHandler.End();

            // Postprocess
            lifecycleHandler.AfterPop(context);

            // Unload Unused Page
            var beforeReleaseHandle = context.ExitPage.BeforeRelease();
            while (!beforeReleaseHandle.IsTerminated) yield return null;

            for (var i = 0; i < context.ExitPageIds.Count; i++)
            {
                var unusedPageId = context.ExitPageIds[i];
                var unusedPage = context.ExitPages[i];
                var loadHandle = _assetLoadHandles[unusedPageId];
                Destroy(unusedPage.gameObject);
                AssetLoader.Release(loadHandle);
                _assetLoadHandles.Remove(unusedPageId);
            }

            _isActivePageStacked = true;
        }

        public AsyncProcessHandle Preload(string resourceKey, bool loadAsync = true)
        {
            return CoroutineManager.Instance.Run(PreloadRoutine(resourceKey, loadAsync));
        }

        private IEnumerator PreloadRoutine(string resourceKey, bool loadAsync = true)
        {
            if (_preloadedResourceHandles.ContainsKey(resourceKey))
                throw new InvalidOperationException(
                    $"The resource with key \"${resourceKey}\" has already been preloaded.");

            var assetLoadHandle = loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourceKey)
                : AssetLoader.Load<GameObject>(resourceKey);
            _preloadedResourceHandles.Add(resourceKey, assetLoadHandle);

            if (!assetLoadHandle.IsDone) yield return new WaitUntil(() => assetLoadHandle.IsDone);

            if (assetLoadHandle.Status == AssetLoadStatus.Failed) throw assetLoadHandle.OperationException;
        }

        public bool IsPreloadRequested(string resourceKey)
        {
            return _preloadedResourceHandles.ContainsKey(resourceKey);
        }

        public bool IsPreloaded(string resourceKey)
        {
            if (!_preloadedResourceHandles.TryGetValue(resourceKey, out var handle)) return false;

            return handle.Status == AssetLoadStatus.Success;
        }

        public void ReleasePreloaded(string resourceKey)
        {
            if (!_preloadedResourceHandles.ContainsKey(resourceKey))
                throw new InvalidOperationException($"The resource with key \"${resourceKey}\" is not preloaded.");

            var handle = _preloadedResourceHandles[resourceKey];
            _preloadedResourceHandles.Remove(resourceKey);
            AssetLoader.Release(handle);
        }
    }
}
