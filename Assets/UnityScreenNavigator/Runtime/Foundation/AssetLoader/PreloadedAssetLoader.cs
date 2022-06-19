using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace UnityScreenNavigator.Runtime.Foundation.AssetLoader
{
    /// <summary>
    ///     <see cref="IAssetLoader" /> that allows you to register preloaded assets.
    /// </summary>
    public sealed class PreloadedAssetLoader : IAssetLoader
    {
        private int _nextControlId;

        public Dictionary<string, Object> PreloadedObjects { get; } = new Dictionary<string, Object>();

        public AssetLoadHandle<T> Load<T>(string key) where T : Object
        {
            var controlId = _nextControlId++;

            var handle = new AssetLoadHandle<T>(controlId);
            var setter = (IAssetLoadHandleSetter<T>)handle;
            T result = null;
            if (PreloadedObjects.TryGetValue(key, out var obj))
                result = obj as T;

            setter.SetResult(result);
            var status = result != null ? AssetLoadStatus.Success : AssetLoadStatus.Failed;
            setter.SetStatus(status);
            if (result == null)
            {
                var exception = new InvalidOperationException($"Requested asset（Key: {key}）was not found.");
                setter.SetOperationException(exception);
            }

            setter.SetPercentCompleteFunc(() => 1.0f);
            setter.SetTask(Task.FromResult(result));
            return handle;
        }

        public AssetLoadHandle<T> LoadAsync<T>(string key) where T : Object
        {
            return Load<T>(key);
        }

        public void Release(AssetLoadHandle handle)
        {
            // This class does not release any objects.
        }

        /// <summary>
        ///     Add a object to <see cref="PreloadedObjects" />. The asset name is used as the key.
        ///     If you want to set your own key, add item to <see cref="PreloadedObjects" /> directly.
        /// </summary>
        /// <param name="obj"></param>
        public void AddObject(Object obj)
        {
            PreloadedObjects.Add(obj.name, obj);
        }
    }
}
