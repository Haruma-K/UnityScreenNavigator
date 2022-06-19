using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityScreenNavigator.Runtime.Foundation.AssetLoader
{
    [CreateAssetMenu(fileName = "PreloadedAssetLoader", menuName = "Resource Loader/Preloaded Asset Loader")]
    public sealed class PreloadedAssetLoaderObject : AssetLoaderObject, IAssetLoader
    {
        [SerializeField] private List<KeyAssetPair> _preloadedObjects = new List<KeyAssetPair>();

        private readonly PreloadedAssetLoader _loader = new PreloadedAssetLoader();

        public List<KeyAssetPair> PreloadedObjects => _preloadedObjects;

        private void OnEnable()
        {
            if (!Application.isPlaying)
                return;

            foreach (var preloadedObject in _preloadedObjects)
            {
                if (string.IsNullOrEmpty(preloadedObject.Key))
                    continue;

                if (_loader.PreloadedObjects.ContainsKey(preloadedObject.Key))
                    continue;

                _loader.PreloadedObjects.Add(preloadedObject.Key, preloadedObject.Asset);
            }
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
                return;

            _loader.PreloadedObjects.Clear();
        }

        public override AssetLoadHandle<T> Load<T>(string key)
        {
            return _loader.Load<T>(key);
        }

        public override AssetLoadHandle<T> LoadAsync<T>(string key)
        {
            return _loader.LoadAsync<T>(key);
        }

        public override void Release(AssetLoadHandle handle)
        {
            _loader.Release(handle);
        }

        [Serializable]
        public sealed class KeyAssetPair
        {
            public enum KeySourceType
            {
                InputField,
                AssetName
            }
            
            [SerializeField] private KeySourceType _keySource;
            [SerializeField] private string _key;
            [SerializeField] private Object _asset;

            public KeySourceType KeySource
            {
                get => _keySource;
                set => _keySource = value;
            }

            public string Key
            {
                get => GetKey();
                set => _key = value;
            }

            public Object Asset
            {
                get => _asset;
                set => _asset = value;
            }

            private string GetKey()
            {
                if (_keySource == KeySourceType.AssetName)
                    return _asset == null ? "" : _asset.name;
                return _key;
            }
        }
    }
}
