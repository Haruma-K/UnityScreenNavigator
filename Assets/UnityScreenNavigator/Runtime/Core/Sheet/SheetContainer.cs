using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.AssetLoader;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    [RequireComponent(typeof(RectMask2D))]
    public sealed class SheetContainer : MonoBehaviour
    {
        private static readonly Dictionary<int, SheetContainer> InstanceCacheByTransform =
            new Dictionary<int, SheetContainer>();

        private static readonly Dictionary<string, SheetContainer> InstanceCacheByName =
            new Dictionary<string, SheetContainer>();

        [SerializeField] private string _name;

        private readonly Dictionary<int, AssetLoadHandle<GameObject>> _assetLoadHandles
            = new Dictionary<int, AssetLoadHandle<GameObject>>();

        private readonly List<ISheetContainerCallbackReceiver> _callbackReceivers =
            new List<ISheetContainerCallbackReceiver>();

        private readonly Dictionary<string, int> _sheetNameToId = new Dictionary<string, int>();
        private readonly Dictionary<int, Sheet> _sheets = new Dictionary<int, Sheet>();

        private int? _activeSheetId;
        private CanvasGroup _canvasGroup;

        private IAssetLoader AssetLoader => UnityScreenNavigatorSettings.Instance.AssetLoader;

        public int? ActiveSheetId => _activeSheetId;

        public Sheet ActiveSheet
        {
            get
            {
                if (!_activeSheetId.HasValue)
                {
                    return null;
                }

                return _sheets[_activeSheetId.Value];
            }
        }

        /// <summary>
        ///     True if in transition.
        /// </summary>
        public bool IsInTransition { get; private set; }

        /// <summary>
        ///     Registered sheets.
        /// </summary>
        public IReadOnlyDictionary<int, Sheet> Sheets => _sheets;

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        private void Awake()
        {
            _callbackReceivers.AddRange(GetComponents<ISheetContainerCallbackReceiver>());

            if (!string.IsNullOrWhiteSpace(_name))
            {
                InstanceCacheByName.Add(_name, this);
            }
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            foreach (var sheet in _sheets.Values)
            {
                Destroy(sheet.gameObject);
            }

            foreach (var assetLoadHandle in _assetLoadHandles.Values)
            {
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
        ///     Get the <see cref="SheetContainer" /> that manages the sheet to which <see cref="transform" /> belongs.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="transform" />.</param>
        /// <returns></returns>
        public static SheetContainer Of(Transform transform, bool useCache = true)
        {
            return Of((RectTransform)transform, useCache);
        }

        /// <summary>
        ///     Get the <see cref="SheetContainer" /> that manages the sheet to which <see cref="rectTransform" /> belongs.
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="rectTransform" />.</param>
        /// <returns></returns>
        public static SheetContainer Of(RectTransform rectTransform, bool useCache = true)
        {
            var hashCode = rectTransform.GetInstanceID();

            if (useCache && InstanceCacheByTransform.TryGetValue(hashCode, out var container))
            {
                return container;
            }

            container = rectTransform.GetComponentInParent<SheetContainer>();
            if (container != null)
            {
                InstanceCacheByTransform.Add(hashCode, container);
                return container;
            }

            return null;
        }

        /// <summary>
        ///     Find the <see cref="SheetContainer" /> of <see cref="containerName" />.
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static SheetContainer Find(string containerName)
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
        public void AddCallbackReceiver(ISheetContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Add(callbackReceiver);
        }

        /// <summary>
        ///     Remove a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void RemoveCallbackReceiver(ISheetContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Remove(callbackReceiver);
        }

        /// <summary>
        ///     Show a sheet.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="playAnimation"></param>
        /// <returns></returns>
        public AsyncProcessHandle Show(string resourceKey, bool playAnimation)
        {
            return CoroutineManager.Instance.Run(ShowRoutine(resourceKey, playAnimation));
        }

        /// <summary>
        ///     Show a sheet.
        /// </summary>
        /// <param name="sheetId"></param>
        /// <param name="playAnimation"></param>
        /// <returns></returns>
        public AsyncProcessHandle Show(int sheetId, bool playAnimation)
        {
            return CoroutineManager.Instance.Run(ShowRoutine(sheetId, playAnimation));
        }

        /// <summary>
        ///     Hide a sheet.
        /// </summary>
        /// <param name="playAnimation"></param>
        public AsyncProcessHandle Hide(bool playAnimation)
        {
            return CoroutineManager.Instance.Run(HideRoutine(playAnimation));
        }

        /// <summary>
        ///     Register a sheet.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="onLoad"></param>
        /// <param name="loadAsync"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AsyncProcessHandle Register(string resourceKey,
            Action<(int sheetId, Sheet instance)> onLoad = null, bool loadAsync = true)
        {
            return CoroutineManager.Instance.Run(RegisterRoutine(resourceKey, onLoad, loadAsync));
        }

        private IEnumerator RegisterRoutine(string resourceKey,
            Action<(int sheetId, Sheet instance)> onLoad = null, bool loadAsync = true)
        {
            if (resourceKey == null)
            {
                throw new ArgumentNullException(nameof(resourceKey));
            }

            var assetLoadHandle = loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourceKey)
                : AssetLoader.Load<GameObject>(resourceKey);
            while (!assetLoadHandle.IsDone)
            {
                yield return null;
            }

            if (assetLoadHandle.Status == AssetLoadStatus.Failed)
            {
                throw assetLoadHandle.OperationException;
            }

            var instance = Instantiate(assetLoadHandle.Result);
            var sheet = instance.GetComponent<Sheet>();
            if (sheet == null)
            {
                throw new InvalidOperationException(
                    $"Cannot register because the \"{nameof(Sheet)}\" component is not attached to the specified resource \"{resourceKey}\".");
            }

            var sheetId = sheet.GetInstanceID();
            _sheets.Add(sheetId, sheet);
            _sheetNameToId[resourceKey] = sheetId;
            _assetLoadHandles.Add(sheetId, assetLoadHandle);
            onLoad?.Invoke((sheetId, sheet));
            var afterLoadHandle = sheet.AfterLoad((RectTransform)transform);
            while (!afterLoadHandle.IsTerminated)
            {
                yield return null;
            }

            yield return sheetId;
        }

        private IEnumerator ShowRoutine(string resourceKey, bool playAnimation)
        {
            var sheetId = _sheetNameToId[resourceKey];
            yield return ShowRoutine(sheetId, playAnimation);
        }

        private IEnumerator ShowRoutine(int sheetId, bool playAnimation)
        {
            if (IsInTransition)
            {
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");
            }

            if (_activeSheetId.HasValue && _activeSheetId.Value.Equals(sheetId))
            {
                throw new InvalidOperationException(
                    "Cannot transition because the sheet is already active.");
            }

            IsInTransition = true;

            var enterSheet = _sheets[sheetId];
            var exitSheet = _activeSheetId.HasValue ? _sheets[_activeSheetId.Value] : null;

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.BeforeShow(enterSheet, exitSheet);
            }

            var preprocessHandles = new List<AsyncProcessHandle>();
            if (exitSheet != null)
            {
                preprocessHandles.Add(exitSheet.BeforeExit(enterSheet));
            }

            preprocessHandles.Add(enterSheet.BeforeEnter(exitSheet));
            foreach (var coroutineHandle in preprocessHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return null;
                }
            }

            // Play Animation
            var animationHandles = new List<AsyncProcessHandle>();
            if (exitSheet != null)
            {
                animationHandles.Add(exitSheet.Exit(playAnimation, enterSheet));
            }

            animationHandles.Add(enterSheet.Enter(playAnimation, exitSheet));

            foreach (var handle in animationHandles)
            {
                while (!handle.IsTerminated)
                {
                    yield return null;
                }
            }

            // End Transition
            _activeSheetId = sheetId;
            IsInTransition = false;

            // Postprocess
            if (exitSheet != null)
            {
                exitSheet.AfterExit(enterSheet);
            }

            enterSheet.AfterEnter(exitSheet);

            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.AfterShow(enterSheet, exitSheet);
            }
        }

        private IEnumerator HideRoutine(bool playAnimation)
        {
            if (IsInTransition)
            {
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");
            }

            if (!_activeSheetId.HasValue)
            {
                throw new InvalidOperationException(
                    "Cannot transition because there is no active sheets.");
            }

            IsInTransition = true;

            var exitSheet = _sheets[_activeSheetId.Value];

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.BeforeHide(exitSheet);
            }

            var preprocessHandle = exitSheet.BeforeExit(null);
            while (!preprocessHandle.IsTerminated)
            {
                yield return preprocessHandle;
            }

            // Play Animation
            var animationHandle = exitSheet.Exit(playAnimation, null);
            while (!animationHandle.IsTerminated)
            {
                yield return null;
            }

            // End Transition
            _activeSheetId = null;
            IsInTransition = false;

            // Postprocess
            exitSheet.AfterExit(null);
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.AfterHide(exitSheet);
            }
        }
    }
}