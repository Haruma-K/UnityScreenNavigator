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
            foreach (var preloadedObject in _preloadedObjects)
                _loader.PreloadedObjects.Add(preloadedObject.Key, preloadedObject.Asset);
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
        public class KeyAssetPair
        {
            [SerializeField] private string _key;
            [SerializeField] private Object _asset;

            public string Key
            {
                get => _key;
                set => _key = value;
            }

            public Object Asset
            {
                get => _asset;
                set => _asset = value;
            }
        }
    }
}
