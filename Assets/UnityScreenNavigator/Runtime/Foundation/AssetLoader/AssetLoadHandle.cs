using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace UnityScreenNavigator.Runtime.Foundation.AssetLoader
{
    public abstract class AssetLoadHandle
    {
        protected Func<float> PercentCompleteFunc;

        protected AssetLoadHandle(int controlId)
        {
            ControlId = controlId;
        }

        public int ControlId { get; }

        public bool IsDone => Status != AssetLoadStatus.None;

        public AssetLoadStatus Status { get; protected set; }

        public float PercentComplete => PercentCompleteFunc.Invoke();

        public Exception OperationException { get; protected set; }
    }

    public sealed class AssetLoadHandle<T> : AssetLoadHandle, IAssetLoadHandleSetter<T> where T : Object
    {
        public AssetLoadHandle(int controlId) : base(controlId)
        {
        }

        public T Result { get; private set; }

        public Task<T> Task { get; private set; }

        void IAssetLoadHandleSetter<T>.SetStatus(AssetLoadStatus status)
        {
            Status = status;
        }

        void IAssetLoadHandleSetter<T>.SetResult(T result)
        {
            Result = result;
        }

        void IAssetLoadHandleSetter<T>.SetPercentCompleteFunc(Func<float> percentComplete)
        {
            PercentCompleteFunc = percentComplete;
        }

        void IAssetLoadHandleSetter<T>.SetTask(Task<T> task)
        {
            Task = task;
        }

        void IAssetLoadHandleSetter<T>.SetOperationException(Exception ex)
        {
            OperationException = ex;
        }
    }
}