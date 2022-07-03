using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    public sealed class AnonymousSheetLifecycleEvent : ISheetLifecycleEvent
    {
        public AnonymousSheetLifecycleEvent(Func<IEnumerator> initialize = null,
            Func<IEnumerator> onWillEnter = null, Action onDidEnter = null,
            Func<IEnumerator> onWillExit = null, Action onDidExit = null,
            Func<IEnumerator> onCleanup = null)
        {
            if (initialize != null)
                OnInitialize.Add(initialize);

            if (onWillEnter != null)
                OnWillEnter.Add(onWillEnter);

            OnDidEnter = onDidEnter;

            if (onWillExit != null)
                OnWillExit.Add(onWillExit);

            OnDidExit = onDidExit;

            if (onCleanup != null)
                OnCleanup.Add(onCleanup);
        }

        public List<Func<IEnumerator>> OnInitialize { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnWillEnter { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnWillExit { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnCleanup { get; } = new List<Func<IEnumerator>>();

        IEnumerator ISheetLifecycleEvent.Initialize()
        {
            foreach (var onInitialize in OnInitialize)
                yield return onInitialize.Invoke();
        }

        IEnumerator ISheetLifecycleEvent.WillEnter()
        {
            foreach (var onWillEnter in OnWillEnter)
                yield return onWillEnter.Invoke();
        }

        void ISheetLifecycleEvent.DidEnter()
        {
            OnDidEnter?.Invoke();
        }

        IEnumerator ISheetLifecycleEvent.WillExit()
        {
            foreach (var onWillExit in OnWillExit)
                yield return onWillExit.Invoke();
        }

        void ISheetLifecycleEvent.DidExit()
        {
            OnDidExit?.Invoke();
        }

        IEnumerator ISheetLifecycleEvent.Cleanup()
        {
            foreach (var onCleanup in OnCleanup)
                yield return onCleanup.Invoke();
        }

        public event Action OnDidEnter;
        public event Action OnDidExit;
    }
}
