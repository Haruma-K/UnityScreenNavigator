using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityScreenNavigator.Runtime.Foundation
{
    //TODO: あとでAsyncProcessHandleにリネーム
    public sealed class AsyncStatusHandle : CustomYieldInstruction
    {
        internal AsyncStatusHandle()
        {
            CoroutineId = -1;
        }

        internal AsyncStatusHandle(int coroutineId)
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

        public static AsyncStatusHandle Completed()
        {
            var status = new AsyncStatusHandle();
            status.MarkCompleted();
            return status;
        }

        internal static AsyncStatusHandle Create(Func<Task> asyncFunc)
        {
            var status = new AsyncStatusHandle();
            status.MonitorAsync(asyncFunc);
            return status;
        }

        private async void MonitorAsync(Func<Task> asyncFunc)
        {
            try
            {
                await asyncFunc();
                MarkCompleted();
            }
            catch (Exception ex)
            {
                MarkFaulted(ex);
            }
        }

        internal void MarkCompleted()
        {
            Assert.IsFalse(IsCompleted);

            IsCompleted = true;
        }

        internal void MarkFaulted(Exception ex)
        {
            Assert.IsFalse(IsCompleted);

            Exception = ex;
            AllExceptions.Add(ex);
            IsCompleted = true;
        }

        internal void MarkFaulted(IReadOnlyList<Exception> exceptions)
        {
            Assert.IsFalse(IsCompleted);

            Exception = exceptions.FirstOrDefault();
            AllExceptions.AddRange(exceptions);
            IsCompleted = true;
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