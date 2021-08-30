using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation.Coroutine
{
    internal interface IAsyncProcessHandleSetter
    {
        void Complete(object result);

        void Error(Exception ex);
    }

    public class AsyncProcessHandle : CustomYieldInstruction, IAsyncProcessHandleSetter
    {
        private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();

        public AsyncProcessHandle(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public object Result { get; private set; }

        public bool IsTerminated { get; private set; }

        public Exception Exception { get; private set; }

        public Task<object> Task => _tcs.Task;

        public bool HasError => Exception != null;

        public override bool keepWaiting => !IsTerminated;

        void IAsyncProcessHandleSetter.Complete(object result)
        {
            Result = result;
            IsTerminated = true;
            OnTerminate?.Invoke();
            _tcs.SetResult(result);
        }

        void IAsyncProcessHandleSetter.Error(Exception ex)
        {
            Exception = ex;
            IsTerminated = true;
            OnTerminate?.Invoke();
            _tcs.SetException(ex);
        }

        public event Action OnTerminate;
    }
}