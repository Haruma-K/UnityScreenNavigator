using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Core.Sheet;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.AssetLoader;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    [RequireComponent(typeof(RectMask2D))]
    public sealed class PageContainer : MonoBehaviour
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
        public bool IsInTransition { get; private set; }

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

            IsInTransition = true;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                if (UnityScreenNavigatorSettings.Instance.ControlInteractionsOfAllContainers)
                {
                    foreach (var pageContainer in Instances)
                        pageContainer.Interactable = false;
                    foreach (var modalContainer in ModalContainer.Instances)
                        modalContainer.Interactable = false;
                    foreach (var sheetContainer in SheetContainer.Instances)
                        sheetContainer.Interactable = false;
                }
                else
                {
                    Interactable = false;
                }
            }

            // Setup
            var assetLoadHandle = loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourceKey)
                : AssetLoader.Load<GameObject>(resourceKey);
            if (!assetLoadHandle.IsDone) yield return new WaitUntil(() => assetLoadHandle.IsDone);

            if (assetLoadHandle.Status == AssetLoadStatus.Failed) throw assetLoadHandle.OperationException;

            var instance = Instantiate(assetLoadHandle.Result);
            if (!instance.TryGetComponent(pageType, out var c))
                c = instance.AddComponent(pageType);
            var enterPage = (Page)c;

            if (pageId == null)
                pageId = Guid.NewGuid().ToString();
            _assetLoadHandles.Add(pageId, assetLoadHandle);
            onLoad?.Invoke((pageId, enterPage));
            var afterLoadHandle = enterPage.AfterLoad((RectTransform)transform);
            while (!afterLoadHandle.IsTerminated)
                yield return null;

            var exitPageId = _orderedPageIds.Count == 0 ? null : _orderedPageIds[_pages.Count - 1];
            var exitPage = exitPageId == null ? null : _pages[exitPageId];

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.BeforePush(enterPage, exitPage);

            var preprocessHandles = new List<AsyncProcessHandle>();
            if (exitPage != null) preprocessHandles.Add(exitPage.BeforeExit(true, enterPage));

            preprocessHandles.Add(enterPage.BeforeEnter(true, exitPage));

            foreach (var coroutineHandle in preprocessHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // Play Animations
            var animationHandles = new List<AsyncProcessHandle>();
            if (exitPage != null)
                animationHandles.Add(exitPage.Exit(true, playAnimation, enterPage));

            animationHandles.Add(enterPage.Enter(true, playAnimation, exitPage));

            foreach (var coroutineHandle in animationHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // End Transition
            if (!_isActivePageStacked && exitPage != null)
            {
                _pages.Remove(exitPageId);
                _orderedPageIds.Remove(exitPageId);
            }

            _pages.Add(pageId, enterPage);
            _orderedPageIds.Add(pageId);
            IsInTransition = false;

            // Postprocess
            if (exitPage != null)
                exitPage.AfterExit(true, enterPage);

            enterPage.AfterEnter(true, exitPage);

            foreach (var callbackReceiver in _callbackReceivers)
                callbackReceiver.AfterPush(enterPage, exitPage);

            // Unload Unused Page
            if (!_isActivePageStacked && exitPage != null)
            {
                var beforeReleaseHandle = exitPage.BeforeRelease();
                while (!beforeReleaseHandle.IsTerminated) yield return null;

                var handle = _assetLoadHandles[exitPageId];
                AssetLoader.Release(handle);

                Destroy(exitPage.gameObject);
                _assetLoadHandles.Remove(exitPageId);
            }

            _isActivePageStacked = stack;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                if (UnityScreenNavigatorSettings.Instance.ControlInteractionsOfAllContainers)
                {
                    // If there's a container in transition, it should restore Interactive to true when the transition is finished.
                    // So, do nothing here if there's a transitioning container.
                    if (Instances.All(x => !x.IsInTransition)
                        && ModalContainer.Instances.All(x => !x.IsInTransition)
                        && SheetContainer.Instances.All(x => !x.IsInTransition))
                    {
                        foreach (var pageContainer in Instances)
                            pageContainer.Interactable = true;
                        foreach (var modalContainer in ModalContainer.Instances)
                            modalContainer.Interactable = true;
                        foreach (var sheetContainer in SheetContainer.Instances)
                            sheetContainer.Interactable = true;
                    }
                }
                else
                {
                    Interactable = true;
                }
            }
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

            IsInTransition = true;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                if (UnityScreenNavigatorSettings.Instance.ControlInteractionsOfAllContainers)
                {
                    foreach (var pageContainer in Instances)
                        pageContainer.Interactable = false;
                    foreach (var modalContainer in ModalContainer.Instances)
                        modalContainer.Interactable = false;
                    foreach (var sheetContainer in SheetContainer.Instances)
                        sheetContainer.Interactable = false;
                }
                else
                {
                    Interactable = false;
                }
            }

            var exitPageId = _orderedPageIds[_orderedPageIds.Count - 1];
            var exitPage = _pages[exitPageId];

            var unusedPageIds = new List<string>();
            var unusedPages = new List<Page>();
            for (var i = _orderedPageIds.Count - 1; i >= _orderedPageIds.Count - popCount; i--)
            {
                var unusedPageId = _orderedPageIds[i];
                unusedPageIds.Add(unusedPageId);
                unusedPages.Add(_pages[unusedPageId]);
            }

            var enterPageIndex = _orderedPageIds.Count - popCount - 1;
            var enterPageId = enterPageIndex < 0 ? null : _orderedPageIds[enterPageIndex];
            var enterPage = enterPageId == null ? null : _pages[enterPageId];

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.BeforePop(enterPage, exitPage);

            var preprocessHandles = new List<AsyncProcessHandle>
            {
                exitPage.BeforeExit(false, enterPage)
            };
            if (enterPage != null) preprocessHandles.Add(enterPage.BeforeEnter(false, exitPage));

            foreach (var coroutineHandle in preprocessHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // Play Animations
            var animationHandles = new List<AsyncProcessHandle>
            {
                exitPage.Exit(false, playAnimation, enterPage)
            };
            if (enterPage != null) animationHandles.Add(enterPage.Enter(false, playAnimation, exitPage));

            foreach (var coroutineHandle in animationHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // End Transition
            for (var i = 0; i < unusedPageIds.Count; i++)
            {
                var unusedPageId = unusedPageIds[i];
                _pages.Remove(unusedPageId);
                _orderedPageIds.RemoveAt(_orderedPageIds.Count - 1);
            }
            IsInTransition = false;

            // Postprocess
            exitPage.AfterExit(false, enterPage);
            if (enterPage != null) enterPage.AfterEnter(false, exitPage);

            foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.AfterPop(enterPage, exitPage);

            // Unload Unused Page
            var beforeReleaseHandle = exitPage.BeforeRelease();
            while (!beforeReleaseHandle.IsTerminated) yield return null;

            for (var i = 0; i < unusedPageIds.Count; i++)
            {
                var unusedPageId = unusedPageIds[i];
                var unusedPage = unusedPages[i];
                var loadHandle = _assetLoadHandles[unusedPageId];
                Destroy(unusedPage.gameObject);
                AssetLoader.Release(loadHandle);
                _assetLoadHandles.Remove(unusedPageId);
            }

            _isActivePageStacked = true;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                if (UnityScreenNavigatorSettings.Instance.ControlInteractionsOfAllContainers)
                {
                    // If there's a container in transition, it should restore Interactive to true when the transition is finished.
                    // So, do nothing here if there's a transitioning container.
                    if (Instances.All(x => !x.IsInTransition)
                        && ModalContainer.Instances.All(x => !x.IsInTransition)
                        && SheetContainer.Instances.All(x => !x.IsInTransition))
                    {
                        foreach (var pageContainer in Instances)
                            pageContainer.Interactable = true;
                        foreach (var modalContainer in ModalContainer.Instances)
                            modalContainer.Interactable = true;
                        foreach (var sheetContainer in SheetContainer.Instances)
                            sheetContainer.Interactable = true;
                    }
                }
                else
                {
                    Interactable = true;
                }
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
    }
}
