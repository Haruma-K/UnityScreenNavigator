using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation.AssetLoader
{
    public interface IAssetLoader
    {
        AssetLoadHandle<T> Load<T>(string key) where T : Object;

        AssetLoadHandle<T> LoadAsync<T>(string key) where T : Object;

        void Release(AssetLoadHandle handle);
    }
}