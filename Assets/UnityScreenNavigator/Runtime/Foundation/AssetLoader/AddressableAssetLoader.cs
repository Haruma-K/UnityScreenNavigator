#if USN_USE_ADDRESSABLES
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace UnityScreenNavigator.Runtime.Foundation.AssetLoader
{
    public sealed class AddressableAssetLoader : IAssetLoader
    {
        private readonly Dictionary<int, AsyncOperationHandle> _controlIdToHandles =
            new Dictionary<int, AsyncOperationHandle>();

        private int _nextControlId;

        public AssetLoadHandle<T> Load<T>(string key) where T : Object
        {
#if ADDRESSABLES_1_17_4_OR_NEWER
            var addressableHandle = Addressables.LoadAssetAsync<T>(key);
            addressableHandle.WaitForCompletion();
            var controlId = _nextControlId++;
            _controlIdToHandles.Add(controlId, addressableHandle);
            var handle = new AssetLoadHandle<T>(controlId);
            var setter = (IAssetLoadHandleSetter<T>)handle;
            setter.SetPercentCompleteFunc(() => addressableHandle.PercentComplete);
            setter.SetTask(Task.FromResult(addressableHandle.Result));
            setter.SetResult(addressableHandle.Result);
            var status = addressableHandle.Status == AsyncOperationStatus.Succeeded
                ? AssetLoadStatus.Success
                : AssetLoadStatus.Failed;
            setter.SetStatus(status);
            setter.SetOperationException(addressableHandle.OperationException);
            return handle;
#else
            throw new NotSupportedException();
#endif
        }

        public AssetLoadHandle<T> LoadAsync<T>(string key) where T : Object
        {
            var addressableHandle = Addressables.LoadAssetAsync<T>(key);
            var controlId = _nextControlId++;
            _controlIdToHandles.Add(controlId, addressableHandle);
            var handle = new AssetLoadHandle<T>(controlId);
            var setter = (IAssetLoadHandleSetter<T>)handle;
            var tcs = new TaskCompletionSource<T>();
            addressableHandle.Completed += x =>
            {
                setter.SetResult(x.Result);
                var status = x.Status == AsyncOperationStatus.Succeeded
                    ? AssetLoadStatus.Success
                    : AssetLoadStatus.Failed;
                setter.SetStatus(status);
                setter.SetOperationException(addressableHandle.OperationException);
                tcs.SetResult(x.Result);
            };

            setter.SetPercentCompleteFunc(() => addressableHandle.PercentComplete);
            setter.SetTask(tcs.Task);
            return handle;
        }

        public void Release(AssetLoadHandle handle)
        {
            if (!_controlIdToHandles.ContainsKey(handle.ControlId))
            {
                throw new InvalidOperationException(
                    $"There is no asset that has been requested for release (ControlId: {handle.ControlId}).");
            }

            var addressableHandle = _controlIdToHandles[handle.ControlId];
            _controlIdToHandles.Remove(handle.ControlId);
            Addressables.Release(addressableHandle);
        }
    }
}
#endif