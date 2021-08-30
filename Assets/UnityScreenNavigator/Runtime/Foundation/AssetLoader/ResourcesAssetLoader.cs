using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityScreenNavigator.Runtime.Foundation.AssetLoader
{
    public sealed class ResourcesAssetLoader : IAssetLoader
    {
        private int _nextControlId;

        public AssetLoadHandle<T> Load<T>(string key) where T : Object
        {
            var controlId = _nextControlId++;

            var handle = new AssetLoadHandle<T>(controlId);
            var setter = (IAssetLoadHandleSetter<T>)handle;
            var result = Resources.Load<T>(key);

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
            var controlId = _nextControlId++;

            var handle = new AssetLoadHandle<T>(controlId);
            var setter = (IAssetLoadHandleSetter<T>)handle;
            var tcs = new TaskCompletionSource<T>();

            var req = Resources.LoadAsync<T>(key);

            req.completed += _ =>
            {
                var result = req.asset as T;
                setter.SetResult(result);
                var status = result != null ? AssetLoadStatus.Success : AssetLoadStatus.Failed;
                setter.SetStatus(status);
                if (result == null)
                {
                    var exception = new InvalidOperationException($"Requested asset（Key: {key}）was not found.");
                    setter.SetOperationException(exception);
                }

                tcs.SetResult(result);
            };

            setter.SetPercentCompleteFunc(() => req.progress);
            setter.SetTask(tcs.Task);
            return handle;
        }

        public void Release(AssetLoadHandle handle)
        {
            // Resources.UnloadUnusedAssets() is responsible for releasing assets loaded by Resources.Load(), so nothing is done here.
        }
    }
}