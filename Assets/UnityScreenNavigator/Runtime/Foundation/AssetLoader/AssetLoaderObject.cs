using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation.AssetLoader
{
    public abstract class AssetLoaderObject : ScriptableObject, IAssetLoader
    {
        public abstract AssetLoadHandle<T> Load<T>(string key) where T : Object;

        public abstract AssetLoadHandle<T> LoadAsync<T>(string key) where T : Object;

        public abstract void Release(AssetLoadHandle handle);
    }
}