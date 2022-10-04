using System;
#if USN_USE_ASYNC_METHODS
using System.Threading.Tasks;
#else
using System.Collections;
#endif

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    public static class SheetExtensions
    {
#if USN_USE_ASYNC_METHODS
        public static void AddLifecycleEvent(this Sheet self, Func<Task> initialize = null,
            Func<Task> onWillEnter = null, Action onDidEnter = null, Func<Task> onWillExit = null,
            Action onDidExit = null, Func<Task> onCleanup = null, int priority = 0)
#else
        public static void AddLifecycleEvent(this Sheet self, Func<IEnumerator> initialize = null,
            Func<IEnumerator> onWillEnter = null, Action onDidEnter = null, Func<IEnumerator> onWillExit = null,
            Action onDidExit = null, Func<IEnumerator> onCleanup = null, int priority = 0)
#endif
        {
            var lifecycleEvent = new AnonymousSheetLifecycleEvent(initialize, onWillEnter, onDidEnter, onWillExit,
                onDidExit, onCleanup);
            self.AddLifecycleEvent(lifecycleEvent, priority);
        }
    }
}
