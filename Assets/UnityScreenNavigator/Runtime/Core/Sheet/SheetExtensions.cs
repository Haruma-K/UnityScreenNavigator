using System;
using System.Collections;

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    public static class SheetExtensions
    {
        public static void AddLifecycleEvent(this Sheet self, Func<IEnumerator> initialize = null,
            Func<IEnumerator> onWillEnter = null, Action onDidEnter = null, Func<IEnumerator> onWillExit = null,
            Action onDidExit = null, Func<IEnumerator> onCleanup = null, int priority = 0)
        {
            var lifecycleEvent = new AnonymousSheetLifecycleEvent(initialize, onWillEnter, onDidEnter, onWillExit,
                onDidExit, onCleanup);
            self.AddLifecycleEvent(lifecycleEvent, priority);
        }
    }
}
