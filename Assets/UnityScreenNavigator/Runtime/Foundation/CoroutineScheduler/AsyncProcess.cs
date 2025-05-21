using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnityScreenNavigator.Runtime.Foundation
{
    public static class AsyncProcess
    {
#if USN_USE_ASYNC_METHODS
        public static AsyncProcessHandle Run(Task target)
#else
        public static AsyncProcessHandle Run(System.Collections.IEnumerator target)
#endif
        {
#if USN_USE_ASYNC_METHODS
            return TaskScheduler.Run(() => target);
#else
            return CoroutineScheduler.Run(target);
#endif
        }

        public static AsyncProcessHandle WhenAll(IEnumerable<AsyncProcessHandle> handles)
        {
            if (handles == null)
                throw new ArgumentNullException(nameof(handles));

            var handleList = handles.ToArray();
            if (!handleList.Any())
                return AsyncProcessHandle.Completed();

            var combinedHandle = new AsyncProcessHandle();

            var tasks = handleList.Select(h => h.Task);

            Task.WhenAll(tasks).ContinueWith(t =>
            {
                var exceptions = handleList.SelectMany(x => x.AllExceptions).ToArray();
                if (exceptions.Length >= 1)
                    combinedHandle.MarkFaulted(exceptions);
                else
                    combinedHandle.MarkCompleted();
            }, System.Threading.Tasks.TaskScheduler.Default);

            return combinedHandle;
        }

        public static AsyncProcessHandle WhenAll(AsyncProcessHandle[] handles)
        {
            return WhenAll((IEnumerable<AsyncProcessHandle>)handles);
        }
    }
}