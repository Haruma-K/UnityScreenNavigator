using System;
using System.Threading.Tasks;

namespace UnityScreenNavigator.Runtime.Foundation
{
    public sealed class TaskScheduler
    {
        internal static AsyncProcessHandle Run(Func<Task> asyncFunc)
        {
            var handle = new AsyncProcessHandle();
            MonitorAsync(handle, asyncFunc);
            return handle;
        }

        private static async void MonitorAsync(AsyncProcessHandle handle, Func<Task> asyncFunc)
        {
            try
            {
                await asyncFunc();
                handle.MarkCompleted();
            }
            catch (Exception ex)
            {
                handle.MarkFaulted(ex);
            }
        }
    }
}