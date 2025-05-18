using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal sealed class AsyncStatus
    {
        public bool IsCompleted { get; private set; }
        public bool IsFaulted => AllExceptions.Count > 0;

        /// <summary>
        ///     Exceptions. If multiple exceptions occur, only the first one is recorded.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        ///     List of all exceptions that occurred during the async operation.
        /// </summary>
        public List<Exception> AllExceptions { get; } = new();

        public static AsyncStatus Create(Func<Task> asyncFunc)
        {
            var status = new AsyncStatus();
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

        public void MarkCompleted()
        {
            if (IsCompleted)
                return;

            IsCompleted = true;
        }

        public void MarkFaulted(Exception ex)
        {
            if (IsCompleted)
                return;

            Exception = ex;
            AllExceptions.Add(ex);
            IsCompleted = true;
        }

        public void MarkFaulted(IReadOnlyList<Exception> exceptions)
        {
            if (IsCompleted)
                return;

            Exception = exceptions.FirstOrDefault();
            AllExceptions.AddRange(exceptions);
            IsCompleted = true;
        }


        public IEnumerator Wait()
        {
            while (!IsCompleted)
                yield return null;
        }
    }
}