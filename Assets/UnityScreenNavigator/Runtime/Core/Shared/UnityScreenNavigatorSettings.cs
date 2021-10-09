using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Foundation.AssetLoader;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    internal sealed class UnityScreenNavigatorSettings : ScriptableObject
    {
        private const string DefaultModalBackdropPrefabKey = "DefaultModalBackdrop";
        private static UnityScreenNavigatorSettings _instance;

        [SerializeField] private TransitionAnimationObject _defaultSheetEnterAnimation;

        [SerializeField] private TransitionAnimationObject _defaultSheetExitAnimation;

        [SerializeField] private TransitionAnimationObject _defaultPagePushEnterAnimation;

        [SerializeField] private TransitionAnimationObject _defaultPagePushExitAnimation;

        [SerializeField] private TransitionAnimationObject _defaultPagePopEnterAnimation;

        [SerializeField] private TransitionAnimationObject _defaultPagePopExitAnimation;

        [SerializeField] private TransitionAnimationObject _defaultModalEnterAnimation;

        [SerializeField] private TransitionAnimationObject _defaultModalExitAnimation;

        [SerializeField] private TransitionAnimationObject _defaultModalBackdropEnterAnimation;

        [SerializeField] private TransitionAnimationObject _defaultModalBackdropExitAnimation;

        [SerializeField] private ModalBackdrop _defaultModalBackdropPrefab;

        [SerializeField] private AssetLoaderObject _assetLoader;

        public ITransitionAnimation DefaultSheetEnterAnimation
        {
            get
            {
                if (_defaultSheetEnterAnimation == null)
                {
                    _defaultSheetEnterAnimation =
                        SimpleTransitionAnimationObject.CreateInstance(beforeAlpha: 0.0f, easeType: EaseType.Linear);
                }

                return Instantiate(_defaultSheetEnterAnimation);
            }
        }

        public ITransitionAnimation DefaultSheetExitAnimation
        {
            get
            {
                if (_defaultSheetExitAnimation == null)
                {
                    _defaultSheetExitAnimation =
                        SimpleTransitionAnimationObject.CreateInstance(afterAlpha: 0.0f, easeType: EaseType.Linear);
                }

                return Instantiate(_defaultSheetExitAnimation);
            }
        }

        public ITransitionAnimation DefaultPagePushEnterAnimation
        {
            get
            {
                if (_defaultPagePushEnterAnimation == null)
                {
                    _defaultPagePushEnterAnimation =
                        SimpleTransitionAnimationObject.CreateInstance(beforeAlignment: SheetAlignment.Right,
                            afterAlignment: SheetAlignment.Center);
                }

                return Instantiate(_defaultPagePushEnterAnimation);
            }
        }

        public ITransitionAnimation DefaultPagePushExitAnimation
        {
            get
            {
                if (_defaultPagePushExitAnimation == null)
                {
                    _defaultPagePushExitAnimation =
                        SimpleTransitionAnimationObject.CreateInstance(beforeAlignment: SheetAlignment.Center,
                            afterAlignment: SheetAlignment.Left);
                }

                return Instantiate(_defaultPagePushExitAnimation);
            }
        }

        public ITransitionAnimation DefaultPagePopEnterAnimation
        {
            get
            {
                if (_defaultPagePopEnterAnimation == null)
                {
                    _defaultPagePopEnterAnimation =
                        SimpleTransitionAnimationObject.CreateInstance(beforeAlignment: SheetAlignment.Left,
                            afterAlignment: SheetAlignment.Center);
                }

                return Instantiate(_defaultPagePopEnterAnimation);
            }
        }

        public ITransitionAnimation DefaultPagePopExitAnimation
        {
            get
            {
                if (_defaultPagePopExitAnimation == null)
                {
                    _defaultPagePopExitAnimation =
                        SimpleTransitionAnimationObject.CreateInstance(beforeAlignment: SheetAlignment.Center,
                            afterAlignment: SheetAlignment.Right);
                }

                return Instantiate(_defaultPagePopExitAnimation);
            }
        }

        public ITransitionAnimation DefaultModalEnterAnimation
        {
            get
            {
                if (_defaultModalEnterAnimation == null)
                {
                    _defaultModalEnterAnimation =
                        SimpleTransitionAnimationObject.CreateInstance(beforeScale: Vector3.one * 0.3f,
                            beforeAlpha: 0.0f);
                }

                return Instantiate(_defaultModalEnterAnimation);
            }
        }

        public ITransitionAnimation DefaultModalExitAnimation
        {
            get
            {
                if (_defaultModalExitAnimation == null)
                {
                    _defaultModalExitAnimation =
                        SimpleTransitionAnimationObject.CreateInstance(afterScale: Vector3.one * 0.3f,
                            afterAlpha: 0.0f);
                }

                return Instantiate(_defaultModalExitAnimation);
            }
        }

        public ITransitionAnimation DefaultModalBackdropEnterAnimation
        {
            get
            {
                if (_defaultModalBackdropEnterAnimation == null)
                {
                    _defaultModalBackdropEnterAnimation =
                        SimpleTransitionAnimationObject.CreateInstance(beforeAlpha: 0.0f, easeType: EaseType.Linear);
                }

                return Instantiate(_defaultModalBackdropEnterAnimation);
            }
        }

        public ITransitionAnimation DefaultModalBackdropExitAnimation
        {
            get
            {
                if (_defaultModalBackdropExitAnimation == null)
                {
                    _defaultModalBackdropExitAnimation =
                        SimpleTransitionAnimationObject.CreateInstance(afterAlpha: 0.0f, easeType: EaseType.Linear);
                }

                return Instantiate(_defaultModalBackdropExitAnimation);
            }
        }

        public ModalBackdrop DefaultModalBackdropPrefab
        {
            get
            {
                if (_defaultModalBackdropPrefab == null)
                {
                    _defaultModalBackdropPrefab = Resources.Load<ModalBackdrop>(DefaultModalBackdropPrefabKey);
                }

                return _defaultModalBackdropPrefab;
            }
        }

        public IAssetLoader AssetLoader => _assetLoader != null
            ? _assetLoader
            : _assetLoader = CreateInstance<ResourcesAssetLoaderObject>();

        public static UnityScreenNavigatorSettings Instance
        {
            get
            {
#if UNITY_EDITOR
                if (_instance == null)
                {
                    var asset = PlayerSettings.GetPreloadedAssets().OfType<UnityScreenNavigatorSettings>().FirstOrDefault();
                    _instance = asset != null ? asset : CreateInstance<UnityScreenNavigatorSettings>();
                }

                return _instance;

#else
                if (_instance == null)
                {
                    _instance = CreateInstance<ScreenNavigatorSettings>();
                }

                return _instance;
#endif
            }
            private set => _instance = value;
        }

        private void OnEnable()
        {
            _instance = this;
        }

        public ITransitionAnimation GetDefaultPageTransitionAnimation(bool push, bool enter)
        {
            if (push)
            {
                return enter ? DefaultPagePushEnterAnimation : DefaultPagePushExitAnimation;
            }

            return enter ? DefaultPagePopEnterAnimation : DefaultPagePopExitAnimation;
        }

        public ITransitionAnimation GetDefaultModalTransitionAnimation(bool enter)
        {
            return enter ? DefaultModalEnterAnimation : DefaultModalExitAnimation;
        }

        public ITransitionAnimation GetDefaultSheetTransitionAnimation(bool enter)
        {
            return enter ? DefaultSheetEnterAnimation : DefaultSheetExitAnimation;
        }

#if UNITY_EDITOR
        
        [MenuItem("Assets/Create/Screen Navigator Settings", priority = -1)]
        private static void Create()
        {
            var asset = PlayerSettings.GetPreloadedAssets().OfType<UnityScreenNavigatorSettings>().FirstOrDefault();
            if (asset != null)
            {
                throw new InvalidOperationException(
                    $"{nameof(UnityScreenNavigatorSettings)} already exists in preloaded assets");
            }

            var assetPath = EditorUtility.SaveFilePanelInProject($"Save {nameof(UnityScreenNavigatorSettings)}",
                nameof(UnityScreenNavigatorSettings),
                "asset", "", "Assets");

            if (string.IsNullOrEmpty(assetPath))
            {
                // Return if canceled.
                return;
            }

            // Create folders if needed.
            var folderPath = Path.GetDirectoryName(assetPath);
            if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var instance = CreateInstance<UnityScreenNavigatorSettings>();
            AssetDatabase.CreateAsset(instance, assetPath);
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.Add(instance);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            AssetDatabase.SaveAssets();
        }
#endif
    }
}