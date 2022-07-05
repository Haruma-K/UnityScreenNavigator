using System;
using System.Collections;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    public static class ModalExtensions
    {
        public static void AddLifecycleEvent(this Modal self, Func<IEnumerator> initialize = null,
            Func<IEnumerator> onWillPushEnter = null, Action onDidPushEnter = null,
            Func<IEnumerator> onWillPushExit = null, Action onDidPushExit = null,
            Func<IEnumerator> onWillPopEnter = null, Action onDidPopEnter = null,
            Func<IEnumerator> onWillPopExit = null, Action onDidPopExit = null, Func<IEnumerator> onCleanup = null,
            int priority = 0)
        {
            var lifecycleEvent = new AnonymousModalLifecycleEvent(initialize, onWillPushEnter, onDidPushEnter,
                onWillPushExit, onDidPushExit, onWillPopEnter, onDidPopEnter, onWillPopExit, onDidPopExit,
                onCleanup);
            self.AddLifecycleEvent(lifecycleEvent, priority);
        }
    }
}
