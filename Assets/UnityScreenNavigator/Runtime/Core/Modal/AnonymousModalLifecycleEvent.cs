using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    public sealed class AnonymousModalLifecycleEvent : IModalLifecycleEvent
    {
        public AnonymousModalLifecycleEvent(Func<IEnumerator> initialize = null,
            Func<IEnumerator> onWillPushEnter = null, Action onDidPushEnter = null,
            Func<IEnumerator> onWillPushExit = null, Action onDidPushExit = null,
            Func<IEnumerator> onWillPopEnter = null, Action onDidPopEnter = null,
            Func<IEnumerator> onWillPopExit = null, Action onDidPopExit = null,
            Func<IEnumerator> onCleanup = null)
        {
            if (initialize != null)
                OnInitialize.Add(initialize);

            if (onWillPushEnter != null)
                OnWillPushEnter.Add(onWillPushEnter);

            OnDidPushEnter = onDidPushEnter;

            if (onWillPushExit != null)
                OnWillPushExit.Add(onWillPushExit);

            OnDidPushExit = onDidPushExit;

            if (onWillPopEnter != null)
                OnWillPopEnter.Add(onWillPopEnter);

            OnDidPopEnter = onDidPopEnter;

            if (onWillPopExit != null)
                OnWillPopExit.Add(onWillPopExit);

            OnDidPopExit = onDidPopExit;

            if (onCleanup != null)
                OnCleanup.Add(onCleanup);
        }

        public List<Func<IEnumerator>> OnInitialize { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnWillPushEnter { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnWillPushExit { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnWillPopEnter { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnWillPopExit { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnCleanup { get; } = new List<Func<IEnumerator>>();

        IEnumerator IModalLifecycleEvent.Initialize()
        {
            foreach (var onInitialize in OnInitialize)
                yield return onInitialize.Invoke();
        }

        IEnumerator IModalLifecycleEvent.WillPushEnter()
        {
            foreach (var onWillPushEnter in OnWillPushEnter)
                yield return onWillPushEnter.Invoke();
        }

        void IModalLifecycleEvent.DidPushEnter()
        {
            OnDidPushEnter?.Invoke();
        }

        IEnumerator IModalLifecycleEvent.WillPushExit()
        {
            foreach (var onWillPushExit in OnWillPushExit)
                yield return onWillPushExit.Invoke();
        }

        void IModalLifecycleEvent.DidPushExit()
        {
            OnDidPushExit?.Invoke();
        }

        IEnumerator IModalLifecycleEvent.WillPopEnter()
        {
            foreach (var onWillPopEnter in OnWillPopEnter)
                yield return onWillPopEnter.Invoke();
        }

        void IModalLifecycleEvent.DidPopEnter()
        {
            OnDidPopEnter?.Invoke();
        }

        IEnumerator IModalLifecycleEvent.WillPopExit()
        {
            foreach (var onWillPopExit in OnWillPopExit)
                yield return onWillPopExit.Invoke();
        }

        void IModalLifecycleEvent.DidPopExit()
        {
            OnDidPopExit?.Invoke();
        }

        IEnumerator IModalLifecycleEvent.Cleanup()
        {
            foreach (var onCleanup in OnCleanup)
                yield return onCleanup.Invoke();
        }

        public event Action OnDidPushEnter;
        public event Action OnDidPushExit;
        public event Action OnDidPopEnter;
        public event Action OnDidPopExit;
    }
}
