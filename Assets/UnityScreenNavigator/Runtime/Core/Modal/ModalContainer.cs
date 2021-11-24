using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Shared;
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

        [SerializeField] private ModalBackdrop _overrideBackdropPrefab;

        private readonly Dictionary<int, AssetLoadHandle<GameObject>> _assetLoadHandles
            = new Dictionary<int, AssetLoadHandle<GameObject>>();

        private readonly List<ModalBackdrop> _backdrops = new List<ModalBackdrop>();

        private readonly List<IModalContainerCallbackReceiver> _callbackReceivers =
            new List<IModalContainerCallbackReceiver>();

        private readonly List<Modal> _modals = new List<Modal>();

        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _preloadedResourceHandles =
            new Dictionary<string, AssetLoadHandle<GameObject>>();

        private ModalBackdrop _backdropPrefab;
        private CanvasGroup _canvasGroup;

        private IAssetLoader AssetLoader => UnityScreenNavigatorSettings.Instance.AssetLoader;

        /// <summary>
        ///     True if in transition.
        /// </summary>
        public bool IsInTransition { get; private set; }

        /// <summary>
        ///     Stacked modals.
        /// </summary>
        public IReadOnlyList<Modal> Modals => _modals;

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        private void Awake()
        {
            _callbackReceivers.AddRange(GetComponents<IModalContainerCallbackReceiver>());
            if (!string.IsNullOrWhiteSpace(_name))
            {
                InstanceCacheByName.Add(_name, this);
            }

            _backdropPrefab = _overrideBackdropPrefab
                ? _overrideBackdropPrefab
                : UnityScreenNavigatorSettings.Instance.ModalBackdropPrefab;

            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            foreach (var modal in _modals)
            {
                var modalId = modal.GetInstanceID();
                var assetLoadHandle = _assetLoadHandles[modalId];

                Destroy(modal.gameObject);
                AssetLoader.Release(assetLoadHandle);
            }

            _assetLoadHandles.Clear();

            InstanceCacheByName.Remove(_name);
            var keysToRemove = new List<int>();
            foreach (var cache in InstanceCacheByTransform)
            {
                if (Equals(cache.Value))
                {
                    keysToRemove.Add(cache.Key);
                }
            }

            foreach (var keyToRemove in keysToRemove)
            {
                InstanceCacheByTransform.Remove(keyToRemove);
            }
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
            if (useCache && InstanceCacheByTransform.TryGetValue(id, out var container))
            {
                return container;
            }

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
            if (InstanceCacheByName.TryGetValue(containerName, out var instance))
            {
                return instance;
            }

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
        /// <param name="onLoad"></param>
        /// <param name="loadAsync"></param>
        /// <returns></returns>
        public AsyncProcessHandle Push(string resourceKey, bool playAnimation, Action<Modal> onLoad = null,
            bool loadAsync = true)
        {
            return CoroutineManager.Instance.Run(PushRoutine(resourceKey, playAnimation, onLoad, loadAsync));
        }

        /// <summary>
        ///     Pop current modal.
        /// </summary>
        /// <param name="playAnimation"></param>
        /// <returns></returns>
        public AsyncProcessHandle Pop(bool playAnimation)
        {
            return CoroutineManager.Instance.Run(PopRoutine(playAnimation));
        }

        private IEnumerator PushRoutine(string resourceKey, bool playAnimation, Action<Modal> onLoad = null,
            bool loadAsync = true)
        {
            if (resourceKey == null)
            {
                throw new ArgumentNullException(nameof(resourceKey));
            }

            if (IsInTransition)
            {
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");
            }

            IsInTransition = true;

            var assetLoadHandle = loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourceKey)
                : AssetLoader.Load<GameObject>(resourceKey);
            if (!assetLoadHandle.IsDone)
            {
                yield return new WaitUntil(() => assetLoadHandle.IsDone);
            }

            if (assetLoadHandle.Status == AssetLoadStatus.Failed)
            {
                throw assetLoadHandle.OperationException;
            }

            var backdrop = Instantiate(_backdropPrefab);
            backdrop.Setup((RectTransform)transform);
            _backdrops.Add(backdrop);

            var instance = Instantiate(assetLoadHandle.Result);
            var enterModal = instance.GetComponent<Modal>();
            if (enterModal == null)
            {
                throw new InvalidOperationException(
                    $"Cannot transition because the \"{nameof(Modal)}\" component is not attached to the specified resource \"{resourceKey}\".");
            }

            var modalId = enterModal.GetInstanceID();
            _assetLoadHandles.Add(modalId, assetLoadHandle);
            onLoad?.Invoke(enterModal);
            var afterLoadHandle = enterModal.AfterLoad((RectTransform)transform);
            while (!afterLoadHandle.IsTerminated)
            {
                yield return null;
            }

            var exitModal = _modals.Count == 0 ? null : _modals[_modals.Count - 1];

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.BeforePush(enterModal, exitModal);
            }


            var preprocessHandles = new List<AsyncProcessHandle>();
            if (exitModal != null)
            {
                preprocessHandles.Add(exitModal.BeforeExit(true, enterModal));
            }

            preprocessHandles.Add(enterModal.BeforeEnter(true, exitModal));

            foreach (var coroutineHandle in preprocessHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // Play Animation
            var animationHandles = new List<AsyncProcessHandle>();
            animationHandles.Add(backdrop.Enter(playAnimation));

            if (exitModal != null)
            {
                animationHandles.Add(exitModal.Exit(true, playAnimation, enterModal));
            }

            animationHandles.Add(enterModal.Enter(true, playAnimation, exitModal));

            foreach (var coroutineHandle in animationHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // End Transition
            _modals.Add(enterModal);
            IsInTransition = false;

            // Postprocess
            if (exitModal != null)
            {
                exitModal.AfterExit(true, enterModal);
            }

            enterModal.AfterEnter(true, exitModal);

            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.AfterPush(enterModal, exitModal);
            }
        }

        private IEnumerator PopRoutine(bool playAnimation)
        {
            if (_modals.Count == 0)
            {
                throw new InvalidOperationException(
                    "Cannot transition because there are no modals loaded on the stack.");
            }

            if (IsInTransition)
            {
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");
            }

            IsInTransition = true;

            var exitModal = _modals[_modals.Count - 1];
            var exitModalId = exitModal.GetInstanceID();
            var enterModal = _modals.Count == 1 ? null : _modals[_modals.Count - 2];
            var backdrop = _backdrops[_backdrops.Count - 1];
            _backdrops.RemoveAt(_backdrops.Count - 1);

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.BeforePop(enterModal, exitModal);
            }

            var preprocessHandles = new List<AsyncProcessHandle>
            {
                exitModal.BeforeExit(false, enterModal)
            };
            if (enterModal != null)
            {
                preprocessHandles.Add(enterModal.BeforeEnter(false, exitModal));
            }

            foreach (var coroutineHandle in preprocessHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // Play Animation
            var animationHandles = new List<AsyncProcessHandle>
            {
                exitModal.Exit(false, playAnimation, enterModal)
            };
            if (enterModal != null)
            {
                animationHandles.Add(enterModal.Enter(false, playAnimation, exitModal));
            }

            animationHandles.Add(backdrop.Exit(playAnimation));

            foreach (var coroutineHandle in animationHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // End Transition
            _modals.RemoveAt(_modals.Count - 1);
            IsInTransition = false;

            // Postprocess
            exitModal.AfterExit(false, enterModal);
            if (enterModal != null)
            {
                enterModal.AfterEnter(false, exitModal);
            }

            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.AfterPop(enterModal, exitModal);
            }

            // Unload Unused Page
            var beforeReleaseHandle = exitModal.BeforeRelease();
            while (!beforeReleaseHandle.IsTerminated)
            {
                yield return null;
            }

            var loadHandle = _assetLoadHandles[exitModalId];
            Destroy(exitModal.gameObject);
            Destroy(backdrop.gameObject);
            AssetLoader.Release(loadHandle);
            _assetLoadHandles.Remove(exitModalId);
        }

        public AsyncProcessHandle Preload(string resourceKey, bool loadAsync = true)
        {
            return CoroutineManager.Instance.Run(PreloadRoutine(resourceKey, loadAsync));
        }

        private IEnumerator PreloadRoutine(string resourceKey, bool loadAsync = true)
        {
            if (_preloadedResourceHandles.ContainsKey(resourceKey))
            {
                throw new InvalidOperationException(
                    $"The resource with key \"${resourceKey}\" has already been preloaded.");
            }

            var assetLoadHandle = loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourceKey)
                : AssetLoader.Load<GameObject>(resourceKey);
            _preloadedResourceHandles.Add(resourceKey, assetLoadHandle);

            if (!assetLoadHandle.IsDone)
            {
                yield return new WaitUntil(() => assetLoadHandle.IsDone);
            }

            if (assetLoadHandle.Status == AssetLoadStatus.Failed)
            {
                throw assetLoadHandle.OperationException;
            }
        }

        public bool IsPreloadRequested(string resourceKey)
        {
            return _preloadedResourceHandles.ContainsKey(resourceKey);
        }

        public bool IsPreloaded(string resourceKey)
        {
            if (!_preloadedResourceHandles.TryGetValue(resourceKey, out var handle))
            {
                return false;
            }

            return handle.Status == AssetLoadStatus.Success;
        }

        public void ReleasePreloaded(string resourceKey)
        {
            if (!_preloadedResourceHandles.ContainsKey(resourceKey))
            {
                throw new InvalidOperationException($"The resource with key \"${resourceKey}\" is not preloaded.");
            }

            var handle = _preloadedResourceHandles[resourceKey];
            AssetLoader.Release(handle);
        }
    }
}