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
    public sealed class SheetContainer : MonoBehaviour, IScreenContainer
    {
        private static readonly Dictionary<int, SheetContainer> InstanceCacheByTransform =
            new Dictionary<int, SheetContainer>();

        private static readonly Dictionary<string, SheetContainer> InstanceCacheByName =
            new Dictionary<string, SheetContainer>();

        [SerializeField] private string _name;

        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _assetLoadHandles
            = new Dictionary<string, AssetLoadHandle<GameObject>>();

        private readonly List<ISheetContainerCallbackReceiver> _callbackReceivers =
            new List<ISheetContainerCallbackReceiver>();

        private readonly Dictionary<string, string> _sheetNameToId = new Dictionary<string, string>();
        private readonly Dictionary<string, Sheet> _sheets = new Dictionary<string, Sheet>();

        private IAssetLoader _assetLoader;
        private CanvasGroup _canvasGroup;
        public static List<SheetContainer> Instances { get; } = new List<SheetContainer>();

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

        public string ActiveSheetId { get; private set; }

        public Sheet ActiveSheet
        {
            get
            {
                if (ActiveSheetId == null)
                    return null;

                return _sheets[ActiveSheetId];
            }
        }

        /// <summary>
        ///     True if in transition.
        /// </summary>
        public bool IsInTransition => _transitionHandler.IsInTransition;

        /// <summary>
        ///     Registered sheets.
        /// </summary>
        public IReadOnlyDictionary<string, Sheet> Sheets => _sheets;

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        private void Awake()
        {
            Instances.Add(this);

            _callbackReceivers.AddRange(GetComponents<ISheetContainerCallbackReceiver>());

            if (!string.IsNullOrWhiteSpace(_name)) InstanceCacheByName.Add(_name, this);
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _transitionHandler = new ScreenContainerTransitionHandler(this);
        }

        private void OnDestroy()
        {
            UnregisterAll();

            InstanceCacheByName.Remove(_name);
            var keysToRemove = new List<int>();
            foreach (var cache in InstanceCacheByTransform)
                if (Equals(cache.Value))
                    keysToRemove.Add(cache.Key);

            foreach (var keyToRemove in keysToRemove) InstanceCacheByTransform.Remove(keyToRemove);

            Instances.Remove(this);
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

            if (useCache && InstanceCacheByTransform.TryGetValue(hashCode, out var container)) return container;

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
            if (InstanceCacheByName.TryGetValue(containerName, out var instance)) return instance;

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
        public AsyncProcessHandle ShowByResourceKey(string resourceKey, bool playAnimation)
        {
            return CoroutineManager.Instance.Run(ShowByResourceKeyRoutine(resourceKey, playAnimation));
        }

        /// <summary>
        ///     Show a sheet.
        /// </summary>
        /// <param name="sheetId"></param>
        /// <param name="playAnimation"></param>
        /// <returns></returns>
        public AsyncProcessHandle Show(string sheetId, bool playAnimation)
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
        /// <param name="sheetId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AsyncProcessHandle Register(string resourceKey,
            Action<(string sheetId, Sheet sheet)> onLoad = null, bool loadAsync = true, string sheetId = null)
        {
            return CoroutineManager.Instance.Run(
                RegisterRoutine(typeof(Sheet), resourceKey, onLoad, loadAsync, sheetId));
        }

        /// <summary>
        ///     Register a sheet.
        /// </summary>
        /// <param name="sheetType"></param>
        /// <param name="resourceKey"></param>
        /// <param name="onLoad"></param>
        /// <param name="loadAsync"></param>
        /// <param name="sheetId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AsyncProcessHandle Register(Type sheetType, string resourceKey,
            Action<(string sheetId, Sheet sheet)> onLoad = null, bool loadAsync = true, string sheetId = null)
        {
            return CoroutineManager.Instance.Run(RegisterRoutine(sheetType, resourceKey, onLoad, loadAsync, sheetId));
        }

        /// <summary>
        ///     Register a sheet.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="onLoad"></param>
        /// <param name="loadAsync"></param>
        /// <param name="sheetId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AsyncProcessHandle Register<TSheet>(string resourceKey,
            Action<(string sheetId, TSheet sheet)> onLoad = null, bool loadAsync = true, string sheetId = null)
            where TSheet : Sheet
        {
            return CoroutineManager.Instance.Run(RegisterRoutine(typeof(TSheet), resourceKey,
                x => onLoad?.Invoke((x.sheetId, (TSheet)x.sheet)), loadAsync, sheetId));
        }

        private IEnumerator RegisterRoutine(Type sheetType, string resourceKey,
            Action<(string sheetId, Sheet sheet)> onLoad = null, bool loadAsync = true, string sheetId = null)
        {
            if (resourceKey == null) throw new ArgumentNullException(nameof(resourceKey));

            var context = new SheetRegisterContext(sheetType, resourceKey, sheetId);

            // アセットのロード
            var assetLoadHandle = loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourceKey)
                : AssetLoader.Load<GameObject>(resourceKey);
            context.SetAssetLoadHandle(assetLoadHandle);
            while (!assetLoadHandle.IsDone) yield return null;

            if (assetLoadHandle.Status == AssetLoadStatus.Failed) throw assetLoadHandle.OperationException;

            // シートのインスタンス化と初期化
            var instance = Instantiate(assetLoadHandle.Result);
            if (!instance.TryGetComponent(sheetType, out var c))
                c = instance.AddComponent(sheetType);
            var sheet = (Sheet)c;
            context.SetSheet(sheet);

            // シートの登録
            _sheets.Add(context.SheetId, sheet);
            _sheetNameToId[resourceKey] = context.SheetId;
            _assetLoadHandles.Add(context.SheetId, assetLoadHandle);
            onLoad?.Invoke((context.SheetId, sheet));

            // シートの後処理
            var afterLoadHandle = sheet.AfterLoad((RectTransform)transform);
            while (!afterLoadHandle.IsTerminated) yield return null;

            yield return context.SheetId;
        }

        private IEnumerator ShowByResourceKeyRoutine(string resourceKey, bool playAnimation)
        {
            var sheetId = _sheetNameToId[resourceKey];
            yield return ShowRoutine(sheetId, playAnimation);
        }

        private IEnumerator ShowRoutine(string sheetId, bool playAnimation)
        {
            if (IsInTransition)
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");

            if (ActiveSheetId != null && ActiveSheetId == sheetId)
                throw new InvalidOperationException(
                    "Cannot transition because the sheet is already active.");

            var context = SheetShowContext.Create(sheetId, ActiveSheetId, _sheets);

            _transitionHandler.Begin();


            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
                callbackReceiver.BeforeShow(context.EnterSheet, context.ExitSheet);

            var preprocessHandles = new List<AsyncProcessHandle>();
            if (context.ExitSheet != null) preprocessHandles.Add(context.ExitSheet.BeforeExit(context.EnterSheet));

            preprocessHandles.Add(context.EnterSheet.BeforeEnter(context.ExitSheet));
            foreach (var coroutineHandle in preprocessHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return null;

            // Play Animation
            var animationHandles = new List<AsyncProcessHandle>();
            if (context.ExitSheet != null)
                animationHandles.Add(context.ExitSheet.Exit(playAnimation, context.EnterSheet));

            animationHandles.Add(context.EnterSheet.Enter(playAnimation, context.ExitSheet));

            foreach (var handle in animationHandles)
                while (!handle.IsTerminated)
                    yield return null;

            // End Transition
            ActiveSheetId = sheetId;
            _transitionHandler.End();

            // Postprocess
            if (context.ExitSheet != null) context.ExitSheet.AfterExit(context.EnterSheet);

            context.EnterSheet.AfterEnter(context.ExitSheet);

            foreach (var callbackReceiver in _callbackReceivers)
                callbackReceiver.AfterShow(context.EnterSheet, context.ExitSheet);
        }

        private IEnumerator HideRoutine(bool playAnimation)
        {
            if (IsInTransition)
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");

            if (ActiveSheetId == null)
                throw new InvalidOperationException(
                    "Cannot transition because there is no active sheets.");

            _transitionHandler.Begin();

            var context = SheetHideContext.Create(_sheets[ActiveSheetId], playAnimation);

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers) 
                callbackReceiver.BeforeHide(context.ExitSheet);

            var preprocessHandle = context.ExitSheet.BeforeExit(null);
            while (!preprocessHandle.IsTerminated) 
                yield return preprocessHandle;

            // Play Animation
            var animationHandle = context.ExitSheet.Exit(context.PlayAnimation, null);
            while (!animationHandle.IsTerminated) 
                yield return null;

            // End Transition
            ActiveSheetId = null;
            _transitionHandler.End();

            // Postprocess
            context.ExitSheet.AfterExit(null);
            foreach (var callbackReceiver in _callbackReceivers) 
                callbackReceiver.AfterHide(context.ExitSheet);
        }

        /// <summary>
        ///     Destroy and release all sheets.
        /// </summary>
        public void UnregisterAll()
        {
            foreach (var sheet in _sheets.Values)
            {
                if (UnityScreenNavigatorSettings.Instance.CallCleanupWhenDestroy)
                    sheet.BeforeReleaseAndForget();
                Destroy(sheet.gameObject);
            }

            foreach (var assetLoadHandle in _assetLoadHandles.Values)
                AssetLoader.Release(assetLoadHandle);

            _assetLoadHandles.Clear();
        }
    }
}
