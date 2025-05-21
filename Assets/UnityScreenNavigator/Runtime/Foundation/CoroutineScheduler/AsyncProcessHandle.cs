using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityScreenNavigator.Runtime.Foundation
{
    public sealed class AsyncProcessHandle : CustomYieldInstruction
    {
        private readonly TaskCompletionSource<object> _tcs = new();
        
        internal AsyncProcessHandle()
        {
            CoroutineId = -1;
        }

        internal AsyncProcessHandle(int coroutineId)
        {
            CoroutineId = coroutineId;
        }

        public int CoroutineId { get; }

        public bool IsCompleted { get; private set; }
        public bool IsFaulted => AllExceptions.Count > 0;
        public override bool keepWaiting => !IsCompleted;

        /// <summary>
        ///     Exceptions. If multiple exceptions occur, only the first one is recorded.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        ///     List of all exceptions that occurred during the async operation.
        /// </summary>
        public List<Exception> AllExceptions { get; } = new();
        
        public Task Task => _tcs.Task;

        public static AsyncProcessHandle Completed()
        {
            var status = new AsyncProcessHandle();
            status.MarkCompleted();
            return status;
        }

        internal void MarkCompleted()
        {
            Assert.IsFalse(IsCompleted);

            IsCompleted = true;
            _tcs.TrySetResult(null);
        }

        internal void MarkFaulted(Exception ex)
        {
            Assert.IsFalse(IsCompleted);

            Exception = ex;
            AllExceptions.Add(ex);
            IsCompleted = true;
            _tcs.TrySetException(ex); 
        }

        internal void MarkFaulted(IReadOnlyList<Exception> exceptions)
        {
            Assert.IsFalse(IsCompleted);

            Exception = exceptions.FirstOrDefault();
            AllExceptions.AddRange(exceptions);
            IsCompleted = true;
            _tcs.TrySetException(new AggregateException(exceptions));
        }

        internal void ThrowIfFaulted()
        {
            if (IsFaulted)
            {
                if (AllExceptions.Count == 1)
                    throw Exception;
                throw new AggregateException("Multiple exceptions occurred during the async operation.", AllExceptions);
            }
        }
    }
}