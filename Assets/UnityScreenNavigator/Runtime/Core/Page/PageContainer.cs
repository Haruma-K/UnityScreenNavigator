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
        private static readonly Dictionary<int, PageContainer> InstanceCacheByTransform = new();

        private static readonly Dictionary<string, PageContainer> InstanceCacheByName = new();

        [SerializeField] private string _name;

        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _assetLoadHandles = new();

        private readonly List<IPageContainerCallbackReceiver> _callbackReceivers = new();

        private readonly List<string> _orderedPageIds = new();

        private readonly Dictionary<string, Page> _pages = new();

        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _preloadedResourceHandles = new();

        private IAssetLoader _assetLoader;

        private CanvasGroup _canvasGroup;

        private bool _isActivePageStacked;
        private PageLifecycleHandler _lifecycleHandler;

        private ScreenContainerTransitionHandler _transitionHandler;
        public static List<PageContainer> Instances { get; } = new();

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
        ///     List of PageIds sorted in the order they are stacked.
        /// </summary>
        public IReadOnlyList<string> OrderedPagesIds => _orderedPageIds;

        /// <summary>
        ///     Map of PageId to Page.
        /// </summary>
        public IReadOnlyDictionary<string, Page> Pages => _pages;

        private void Awake()
        {
            Instances.Add(this);

            _callbackReceivers.AddRange(GetComponents<IPageContainerCallbackReceiver>());
            if (!string.IsNullOrWhiteSpace(_name)) InstanceCacheByName.Add(_name, this);

            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _transitionHandler = new ScreenContainerTransitionHandler(this);
            _lifecycleHandler = new PageLifecycleHandler((RectTransform)transform, _callbackReceivers);
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
        ///     True if in transition.
        /// </summary>
        public bool IsInTransition => _transitionHandler.IsInTransition;

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
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
        public AsyncProcessHandle Push(
            string resourceKey,
            bool playAnimation,
            bool stack = true,
            string pageId = null,
            bool loadAsync = true,
            Action<(string pageId, Page page)> onLoad = null
        )
        {
            return CoroutineManager.Instance.Run(PushRoutine(typeof(Page),
                resourceKey,
                playAnimation,
                stack,
                onLoad,
                loadAsync,
                pageId));
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
        public AsyncProcessHandle Push(
            Type pageType,
            string resourceKey,
            bool playAnimation,
            bool stack = true,
            string pageId = null,
            bool loadAsync = true,
            Action<(string pageId, Page page)> onLoad = null
        )
        {
            return CoroutineManager.Instance.Run(PushRoutine(pageType,
                resourceKey,
                playAnimation,
                stack,
                onLoad,
                loadAsync,
                pageId));
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
        public AsyncProcessHandle Push<TPage>(
            string resourceKey,
            bool playAnimation,
            bool stack = true,
            string pageId = null,
            bool loadAsync = true,
            Action<(string pageId, TPage page)> onLoad = null
        )
            where TPage : Page
        {
            return CoroutineManager.Instance.Run(PushRoutine(typeof(TPage),
                resourceKey,
                playAnimation,
                stack,
                x => onLoad?.Invoke((x.pageId, (TPage)x.page)),
                loadAsync,
                pageId));
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

        private IEnumerator PushRoutine(
            Type pageType,
            string resourceKey,
            bool playAnimation,
            bool stack = true,
            Action<(string pageId, Page page)> onLoad = null,
            bool loadAsync = true,
            string pageId = null
        )
        {
            Assert.IsNotNull(resourceKey);
            Assert.IsFalse(IsInTransition,
                "Cannot transition because the screen is already in transition.");

            _transitionHandler.Begin();

            pageId ??= Guid.NewGuid().ToString();

            Page page = null;
            yield return LoadPage(pageType,
                resourceKey,
                loadAsync,
                (p, lh) =>
                {
                    page = p;
                    _assetLoadHandles.Add(pageId, lh);
                    onLoad?.Invoke((pageId, p));
                });

            var context = PagePushContext.Create(pageId, page, _orderedPageIds, _pages, stack, _isActivePageStacked);

            yield return _lifecycleHandler.AfterLoad(context);

            yield return _lifecycleHandler.BeforePush(context);

            yield return _lifecycleHandler.Push(context, playAnimation);

            _lifecycleHandler.AfterPush(context);

            if (context.ShouldRemoveExitPage)
            {
                context.ExitPage.gameObject.SetActive(false);
                _pages.Remove(context.ExitPageId);
                _orderedPageIds.Remove(context.ExitPageId);
            }

            _pages.Add(context.EnterPageId, context.EnterPage);
            _orderedPageIds.Add(context.EnterPageId);
            _isActivePageStacked = stack;

            _transitionHandler.End();

            // Resource cleanup isn't tied to the transition itself, so it should be handled asynchronously in the background.
            StartCoroutine(AfterPushRoutine(context));
        }

        private IEnumerator AfterPushRoutine(
            PagePushContext context
        )
        {
            yield return _lifecycleHandler.AfterPushRoutine(context);
            if (context.ShouldRemoveExitPage)
            {
                Destroy(context.ExitPage.gameObject);
                var handle = _assetLoadHandles[context.ExitPageId];
                AssetLoader.Release(handle);
                _assetLoadHandles.Remove(context.ExitPageId);
            }
        }

        private IEnumerator PopRoutine(bool playAnimation, int popCount = 1)
        {
            Assert.IsTrue(popCount >= 1);
            Assert.IsTrue(_pages.Count >= popCount,
                "Cannot transition because the page count is less than the pop count.");
            Assert.IsFalse(IsInTransition,
                "Cannot transition because the screen is already in transition.");

            _transitionHandler.Begin();

            var context = PagePopContext.Create(_orderedPageIds, _pages, popCount);

            yield return _lifecycleHandler.BeforePop(context);

            yield return _lifecycleHandler.Pop(context, playAnimation);

            _lifecycleHandler.AfterPop(context);

            for (var i = 0; i < context.ExitPageIds.Count; i++)
            {
                var exitPage = context.ExitPages[i];
                var exitPageId = context.ExitPageIds[i];
                exitPage.gameObject.SetActive(false);
                _pages.Remove(exitPageId);
                _orderedPageIds.RemoveAt(_orderedPageIds.Count - 1);
            }

            _isActivePageStacked = true;

            _transitionHandler.End();

            // Resource cleanup isn't tied to the transition itself, so it should be handled asynchronously in the background.
            StartCoroutine(AfterPopRoutine(context));
        }

        private IEnumerator AfterPopRoutine(PagePopContext context)
        {
            yield return _lifecycleHandler.AfterPopRoutine(context);
            for (var i = 0; i < context.ExitPageIds.Count; i++)
            {
                var unusedPageId = context.ExitPageIds[i];
                var unusedPage = context.ExitPages[i];
                Destroy(unusedPage.gameObject);
                var loadHandle = _assetLoadHandles[unusedPageId];
                AssetLoader.Release(loadHandle);
                _assetLoadHandles.Remove(unusedPageId);
            }
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

        private IEnumerator LoadPage(
            Type pageType,
            string resourceKey,
            bool loadAsync,
            Action<Page, AssetLoadHandle<GameObject>> onLoaded
        )
        {
            var assetLoadHandle = loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourceKey)
                : AssetLoader.Load<GameObject>(resourceKey);

            if (!assetLoadHandle.IsDone)
                yield return new WaitUntil(() => assetLoadHandle.IsDone);

            if (assetLoadHandle.Status == AssetLoadStatus.Failed)
                throw assetLoadHandle.OperationException;

            var instance = Instantiate(assetLoadHandle.Result);
            if (!instance.TryGetComponent(pageType, out var c))
                c = instance.AddComponent(pageType);

            var page = (Page)c;
            onLoaded?.Invoke(page, assetLoadHandle);
        }
    }
}