using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Page;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Core.Sheet;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.AssetLoader;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    [RequireComponent(typeof(RectMask2D))]
    public sealed class ModalContainer : MonoBehaviour
    {
        private static readonly Dictionary<int, ModalContainer> InstanceCacheByTransform =
            new Dictionary<int, ModalContainer>();

        private static readonly Dictionary<string, ModalContainer> InstanceCacheByName =
            new Dictionary<string, ModalContainer>();

        [SerializeField] private string _name;

        [SerializeField] private ModalBackdropStrategy _backdropStrategy = ModalBackdropStrategy.GeneratePerModal;
        
        [SerializeField] private ModalBackdrop _overrideBackdropPrefab;

        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _assetLoadHandles
            = new Dictionary<string, AssetLoadHandle<GameObject>>();

        private readonly List<IModalContainerCallbackReceiver> _callbackReceivers =
            new List<IModalContainerCallbackReceiver>();

        private readonly Dictionary<string, Modal> _modals = new Dictionary<string, Modal>();

        private readonly List<string> _orderedModalIds = new List<string>();

        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _preloadedResourceHandles =
            new Dictionary<string, AssetLoadHandle<GameObject>>();

        private IAssetLoader _assetLoader;

        private ModalBackdrop _backdropPrefab;
        private CanvasGroup _canvasGroup;
        public static List<ModalContainer> Instances { get; } = new List<ModalContainer>();

        private IModalBackdropHandler _backdropHandler;

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
        ///     List of ModalIds sorted in the order they are stacked.
        /// </summary>
        public IReadOnlyList<string> OrderedModalIds => _orderedModalIds;

        /// <summary>
        ///     Map of ModalId to Modal.
        /// </summary>
        public IReadOnlyDictionary<string, Modal> Modals => _modals;

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        private void Awake()
        {
            Instances.Add(this);

            _callbackReceivers.AddRange(GetComponents<IModalContainerCallbackReceiver>());
            if (!string.IsNullOrWhiteSpace(_name)) InstanceCacheByName.Add(_name, this);

            _backdropPrefab = _overrideBackdropPrefab
                ? _overrideBackdropPrefab
                : UnityScreenNavigatorSettings.Instance.ModalBackdropPrefab;

            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _backdropHandler = ModalBackdropHandlerFactory.Create(_backdropStrategy, _backdropPrefab);
        }

        private void OnDestroy()
        {
            foreach (var modalId in _orderedModalIds)
            {
                var modal = _modals[modalId];
                var assetLoadHandle = _assetLoadHandles[modalId];

                if (UnityScreenNavigatorSettings.Instance.CallCleanupWhenDestroy)
                    modal.BeforeReleaseAndForget();
                Destroy(modal.gameObject);
                AssetLoader.Release(assetLoadHandle);
            }

            _assetLoadHandles.Clear();
            _orderedModalIds.Clear();

            InstanceCacheByName.Remove(_name);
            var keysToRemove = new List<int>();
            foreach (var cache in InstanceCacheByTransform)
                if (Equals(cache.Value))
                    keysToRemove.Add(cache.Key);

            foreach (var keyToRemove in keysToRemove) InstanceCacheByTransform.Remove(keyToRemove);

            Instances.Remove(this);
        }

        /// <summary>
        ///     Get the <see cref="ModalContainer" /> that manages the modal to which <see cref="transform" /> belongs.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="transform" />.</param>
        /// <returns></returns>
        public static ModalContainer Of(Transform transform, bool useCache = true)
        {
            return Of((RectTransform)transform, useCache);
        }

        /// <summary>
        ///     Get the <see cref="ModalContainer" /> that manages the modal to which <see cref="rectTransform" /> belongs.
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="rectTransform" />.</param>
        /// <returns></returns>
        public static ModalContainer Of(RectTransform rectTransform, bool useCache = true)
        {
            var id = rectTransform.GetInstanceID();
            if (useCache && InstanceCacheByTransform.TryGetValue(id, out var container)) return container;

            container = rectTransform.GetComponentInParent<ModalContainer>();
            if (container != null)
            {
                InstanceCacheByTransform.Add(id, container);
                return container;
            }

            return null;
        }

        /// <summary>
        ///     Find the <see cref="ModalContainer" /> of <see cref="containerName" />.
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static ModalContainer Find(string containerName)
        {
            if (InstanceCacheByName.TryGetValue(containerName, out var instance)) return instance;

            return null;
        }

        /// <summary>
        ///     Add a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void AddCallbackReceiver(IModalContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Add(callbackReceiver);
        }

        /// <summary>
        ///     Remove a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void RemoveCallbackReceiver(IModalContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Remove(callbackReceiver);
        }

        /// <summary>
        ///     Push new modal.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="playAnimation"></param>
        /// <param name="modalId"></param>
        /// <param name="loadAsync"></param>
        /// <param name="onLoad"></param>
        /// <returns></returns>
        public AsyncProcessHandle Push(string resourceKey, bool playAnimation, string modalId = null,
            bool loadAsync = true, Action<(string modalId, Modal modal)> onLoad = null)
        {
            return CoroutineManager.Instance.Run(PushRoutine(typeof(Modal), resourceKey, playAnimation, onLoad,
                loadAsync, modalId));
        }

        /// <summary>
        ///     Push new modal.
        /// </summary>
        /// <param name="modalType"></param>
        /// <param name="resourceKey"></param>
        /// <param name="playAnimation"></param>
        /// <param name="modalId"></param>
        /// <param name="loadAsync"></param>
        /// <param name="onLoad"></param>
        /// <returns></returns>
        public AsyncProcessHandle Push(Type modalType, string resourceKey, bool playAnimation, string modalId = null,
            bool loadAsync = true, Action<(string modalId, Modal modal)> onLoad = null)
        {
            return CoroutineManager.Instance.Run(PushRoutine(modalType, resourceKey, playAnimation, onLoad, loadAsync,
                modalId));
        }

        /// <summary>
        ///     Push new modal.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="playAnimation"></param>
        /// <param name="modalId"></param>
        /// <param name="loadAsync"></param>
        /// <param name="onLoad"></param>
        /// <typeparam name="TModal"></typeparam>
        /// <returns></returns>
        public AsyncProcessHandle Push<TModal>(string resourceKey, bool playAnimation, string modalId = null,
            bool loadAsync = true, Action<(string modalId, TModal modal)> onLoad = null)
            where TModal : Modal
        {
            return CoroutineManager.Instance.Run(PushRoutine(typeof(TModal), resourceKey, playAnimation,
                x => onLoad?.Invoke((x.modalId, (TModal)x.modal)), loadAsync, modalId));
        }

        /// <summary>
        ///     Pop modals.
        /// </summary>
        /// <param name="playAnimation"></param>
        /// <param name="popCount"></param>
        /// <returns></returns>
        public AsyncProcessHandle Pop(bool playAnimation, int popCount = 1)
        {
            return CoroutineManager.Instance.Run(PopRoutine(playAnimation, popCount));
        }

        /// <summary>
        ///     Pop modals.
        /// </summary>
        /// <param name="playAnimation"></param>
        /// <param name="destinationModalId"></param>
        /// <returns></returns>
        public AsyncProcessHandle Pop(bool playAnimation, string destinationModalId)
        {
            var popCount = 0;
            for (var i = _orderedModalIds.Count - 1; i >= 0; i--)
            {
                var modalId = _orderedModalIds[i];
                if (modalId == destinationModalId)
                    break;
                
                popCount++;
            }

            if (popCount == _orderedModalIds.Count)
                throw new Exception($"The modal with id '{destinationModalId}' is not found.");

            return CoroutineManager.Instance.Run(PopRoutine(playAnimation, popCount));
        }

        private IEnumerator PushRoutine(Type modalType, string resourceKey, bool playAnimation,
            Action<(string modalId, Modal modal)> onLoad = null, bool loadAsync = true, string modalId = null)
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
                    foreach (var pageContainer in PageContainer.Instances)
                        pageContainer.Interactable = false;
                    foreach (var modalContainer in Instances)
                        modalContainer.Interactable = false;
                    foreach (var sheetContainer in SheetContainer.Instances)
                        sheetContainer.Interactable = false;
                }
                else
                {
                    Interactable = false;
                }
            }

            var assetLoadHandle = loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourceKey)
                : AssetLoader.Load<GameObject>(resourceKey);
            if (!assetLoadHandle.IsDone) yield return new WaitUntil(() => assetLoadHandle.IsDone);

            if (assetLoadHandle.Status == AssetLoadStatus.Failed) throw assetLoadHandle.OperationException;

            var instance = Instantiate(assetLoadHandle.Result);
            if (!instance.TryGetComponent(modalType, out var c))
                c = instance.AddComponent(modalType);
            var enterModal = (Modal)c;

            if (modalId == null)
                modalId = Guid.NewGuid().ToString();
            _assetLoadHandles.Add(modalId, assetLoadHandle);
            onLoad?.Invoke((modalId, enterModal));
            var afterLoadHandle = enterModal.AfterLoad((RectTransform)transform);
            while (!afterLoadHandle.IsTerminated)
                yield return null;

            var exitModalId = _orderedModalIds.Count == 0 ? null : _orderedModalIds[_orderedModalIds.Count - 1];
            var exitModal = exitModalId == null ? null : _modals[exitModalId];

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.BeforePush(enterModal, exitModal);

            var preprocessHandles = new List<AsyncProcessHandle>();
            if (exitModal != null) preprocessHandles.Add(exitModal.BeforeExit(true, enterModal));

            preprocessHandles.Add(enterModal.BeforeEnter(true, exitModal));

            foreach (var coroutineHandle in preprocessHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // Play Animation
            var enterModalIndex = _modals.Count;
            var animationHandles = new List<AsyncProcessHandle>();
            animationHandles.Add(_backdropHandler.BeforeModalEnter(enterModal, enterModalIndex, playAnimation));

            if (exitModal != null) animationHandles.Add(exitModal.Exit(true, playAnimation, enterModal));

            animationHandles.Add(enterModal.Enter(true, playAnimation, exitModal));

            foreach (var coroutineHandle in animationHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            _backdropHandler.AfterModalEnter(enterModal, enterModalIndex, true);

            // End Transition
            _modals.Add(modalId, enterModal);
            _orderedModalIds.Add(modalId);
            IsInTransition = false;

            // Postprocess
            if (exitModal != null) exitModal.AfterExit(true, enterModal);

            enterModal.AfterEnter(true, exitModal);

            foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.AfterPush(enterModal, exitModal);

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                if (UnityScreenNavigatorSettings.Instance.ControlInteractionsOfAllContainers)
                {
                    // If there's a container in transition, it should restore Interactive to true when the transition is finished.
                    // So, do nothing here if there's a transitioning container.
                    if (PageContainer.Instances.All(x => !x.IsInTransition)
                        && Instances.All(x => !x.IsInTransition)
                        && SheetContainer.Instances.All(x => !x.IsInTransition))
                    {
                        foreach (var pageContainer in PageContainer.Instances)
                            pageContainer.Interactable = true;
                        foreach (var modalContainer in Instances)
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

            if (_orderedModalIds.Count < popCount)
                throw new InvalidOperationException(
                    "Cannot transition because the modal count is less than the pop count.");

            if (IsInTransition)
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");

            IsInTransition = true;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                if (UnityScreenNavigatorSettings.Instance.ControlInteractionsOfAllContainers)
                {
                    foreach (var pageContainer in PageContainer.Instances)
                        pageContainer.Interactable = false;
                    foreach (var modalContainer in Instances)
                        modalContainer.Interactable = false;
                    foreach (var sheetContainer in SheetContainer.Instances)
                        sheetContainer.Interactable = false;
                }
                else
                {
                    Interactable = false;
                }
            }

            var exitModalId = _orderedModalIds[_orderedModalIds.Count - 1];
            var exitModal = _modals[exitModalId];

            var unusedModalIds = new List<string>();
            var unusedModals = new List<Modal>();
            var unusedModalIndices = new List<int>();
            for (var i = _orderedModalIds.Count - 1; i >= _orderedModalIds.Count - popCount; i--)
            {
                var unusedModalId = _orderedModalIds[i];
                unusedModalIds.Add(unusedModalId);
                unusedModals.Add(_modals[unusedModalId]);
                unusedModalIndices.Add(i);
            }

            var enterModalIndex = _orderedModalIds.Count - popCount - 1;
            var enterModalId = enterModalIndex < 0 ? null : _orderedModalIds[enterModalIndex];
            var enterModal = enterModalId == null ? null : _modals[enterModalId];

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.BeforePop(enterModal, exitModal);

            var preprocessHandles = new List<AsyncProcessHandle>
            {
                exitModal.BeforeExit(false, enterModal)
            };
            if (enterModal != null) preprocessHandles.Add(enterModal.BeforeEnter(false, exitModal));

            foreach (var coroutineHandle in preprocessHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // Play Animation
            var animationHandles = new List<AsyncProcessHandle>();
            for (var i = unusedModalIds.Count - 1; i >= 0; i--)
            {
                var unusedModalId = unusedModalIds[i];
                var unusedModal = _modals[unusedModalId];
                var unusedModalIndex = unusedModalIndices[i];
                var partnerModalId = i == 0 ? enterModalId : unusedModalIds[i - 1];
                var partnerModal = partnerModalId == null ? null : _modals[partnerModalId];
                animationHandles.Add(_backdropHandler.BeforeModalExit(unusedModal, unusedModalIndex, playAnimation));
                animationHandles.Add(unusedModal.Exit(false, playAnimation, partnerModal));
            }

            if (enterModal != null) animationHandles.Add(enterModal.Enter(false, playAnimation, exitModal));

            foreach (var coroutineHandle in animationHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // End Transition
            for (var i = 0; i < unusedModalIds.Count; i++)
            {
                var unusedModalId = unusedModalIds[i];
                _modals.Remove(unusedModalId);
                _orderedModalIds.RemoveAt(_orderedModalIds.Count - 1);
            }
            IsInTransition = false;

            // Postprocess
            exitModal.AfterExit(false, enterModal);
            if (enterModal != null) enterModal.AfterEnter(false, exitModal);

            foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.AfterPop(enterModal, exitModal);

            // Unload Unused Page
            var beforeReleaseHandle = exitModal.BeforeRelease();
            while (!beforeReleaseHandle.IsTerminated) yield return null;

            for (var i = 0; i < unusedModalIds.Count; i++)
            {
                var unusedModalId = unusedModalIds[i];
                var unusedModal = unusedModals[i];
                var unusedModalIndex = unusedModalIndices[i];
                var loadHandle = _assetLoadHandles[unusedModalId];
                Destroy(unusedModal.gameObject);
                AssetLoader.Release(loadHandle);
                _assetLoadHandles.Remove(unusedModalId);
                _backdropHandler.AfterModalExit(exitModal, unusedModalIndex, playAnimation);
            }

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                if (UnityScreenNavigatorSettings.Instance.ControlInteractionsOfAllContainers)
                {
                    // If there's a container in transition, it should restore Interactive to true when the transition is finished.
                    // So, do nothing here if there's a transitioning container.
                    if (PageContainer.Instances.All(x => !x.IsInTransition)
                        && Instances.All(x => !x.IsInTransition)
                        && SheetContainer.Instances.All(x => !x.IsInTransition))
                    {
                        foreach (var pageContainer in PageContainer.Instances)
                            pageContainer.Interactable = true;
                        foreach (var modalContainer in Instances)
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
            AssetLoader.Release(handle);
        }
    }
}
