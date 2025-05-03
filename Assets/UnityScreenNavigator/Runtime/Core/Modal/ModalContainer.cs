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

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    [RequireComponent(typeof(RectMask2D))]
    public sealed class ModalContainer : MonoBehaviour, IScreenContainer
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
            _transitionHandler = new ScreenContainerTransitionHandler(this);
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

            _transitionHandler.Begin();

            var assetLoadHandle = loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourceKey)
                : AssetLoader.Load<GameObject>(resourceKey);
            if (!assetLoadHandle.IsDone) yield return new WaitUntil(() => assetLoadHandle.IsDone);

            if (assetLoadHandle.Status == AssetLoadStatus.Failed) throw assetLoadHandle.OperationException;

            var instance = Instantiate(assetLoadHandle.Result);
            if (!instance.TryGetComponent(modalType, out var c))
                c = instance.AddComponent(modalType);

            var context = ModalPushContext.Create(modalId, (Modal)c, _orderedModalIds, _modals);

            _assetLoadHandles.Add(context.ModalId, assetLoadHandle);
            onLoad?.Invoke((context.ModalId, context.EnterModal));
            var afterLoadHandle = context.EnterModal.AfterLoad((RectTransform)transform);
            while (!afterLoadHandle.IsTerminated)
                yield return null;

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
                callbackReceiver.BeforePush(context.EnterModal, context.ExitModal);

            var preprocessHandles = new List<AsyncProcessHandle>();
            if (context.ExitModal != null)
                preprocessHandles.Add(context.ExitModal.BeforeExit(true, context.EnterModal));

            preprocessHandles.Add(context.EnterModal.BeforeEnter(true, context.ExitModal));

            foreach (var coroutineHandle in preprocessHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // Play Animation
            var animationHandles = new List<AsyncProcessHandle>();
            animationHandles.Add(
                _backdropHandler.BeforeModalEnter(context.EnterModal, context.EnterModalIndex, playAnimation));

            if (context.ExitModal != null)
                animationHandles.Add(context.ExitModal.Exit(true, playAnimation, context.EnterModal));

            animationHandles.Add(context.EnterModal.Enter(true, playAnimation, context.ExitModal));

            foreach (var coroutineHandle in animationHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            _backdropHandler.AfterModalEnter(context.EnterModal, context.EnterModalIndex, true);

            // End Transition
            _modals.Add(context.ModalId, context.EnterModal);
            _orderedModalIds.Add(context.ModalId);
            _transitionHandler.End();

            // Postprocess
            if (context.ExitModal != null) context.ExitModal.AfterExit(true, context.EnterModal);

            context.EnterModal.AfterEnter(true, context.ExitModal);

            foreach (var callbackReceiver in _callbackReceivers)
                callbackReceiver.AfterPush(context.EnterModal, context.ExitModal);
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

            var context = ModalPopContext.Create(_orderedModalIds, _modals, popCount);
            
            _transitionHandler.Begin();

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
                callbackReceiver.BeforePop(context.EnterModal, context.FirstExitModal);

            var preprocessHandles = new List<AsyncProcessHandle>
            {
                context.FirstExitModal.BeforeExit(false, context.EnterModal)
            };
            if (context.EnterModal != null)
                preprocessHandles.Add(context.EnterModal.BeforeEnter(false, context.FirstExitModal));

            foreach (var coroutineHandle in preprocessHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // Play Animation
            var animationHandles = new List<AsyncProcessHandle>();
            for (var i = context.ExitModalIds.Count - 1; i >= 0; i--)
            {
                var exitModalId = context.ExitModalIds[i];
                var exitModal = _modals[exitModalId];
                var exitModalIndex = context.ExitModalIndices[i];
                var partnerModalId = i == 0 ? context.EnterModalId : context.ExitModalIds[i - 1];
                var partnerModal = partnerModalId == null ? null : _modals[partnerModalId];
                animationHandles.Add(_backdropHandler.BeforeModalExit(exitModal, exitModalIndex, playAnimation));
                animationHandles.Add(exitModal.Exit(false, playAnimation, partnerModal));
            }

            if (context.EnterModal != null)
                animationHandles.Add(context.EnterModal.Enter(false, playAnimation, context.FirstExitModal));

            foreach (var coroutineHandle in animationHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // End Transition
            for (var i = 0; i < context.ExitModalIds.Count; i++)
            {
                var unusedModalId = context.ExitModalIds[i];
                _modals.Remove(unusedModalId);
                _orderedModalIds.RemoveAt(_orderedModalIds.Count - 1);
            }
            _transitionHandler.End();

            // Postprocess
            context.FirstExitModal.AfterExit(false, context.EnterModal);
            if (context.EnterModal != null) context.EnterModal.AfterEnter(false, context.FirstExitModal);

            foreach (var callbackReceiver in _callbackReceivers)
                callbackReceiver.AfterPop(context.EnterModal, context.FirstExitModal);

            // Unload Unused Page
            var beforeReleaseHandle = context.FirstExitModal.BeforeRelease();
            while (!beforeReleaseHandle.IsTerminated) yield return null;

            for (var i = 0; i < context.ExitModalIds.Count; i++)
            {
                var unusedModalId = context.ExitModalIds[i];
                var unusedModal = context.ExitModals[i];
                var unusedModalIndex = context.ExitModalIndices[i];
                var loadHandle = _assetLoadHandles[unusedModalId];
                Destroy(unusedModal.gameObject);
                AssetLoader.Release(loadHandle);
                _assetLoadHandles.Remove(unusedModalId);
                _backdropHandler.AfterModalExit(context.FirstExitModal, unusedModalIndex, playAnimation);
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
