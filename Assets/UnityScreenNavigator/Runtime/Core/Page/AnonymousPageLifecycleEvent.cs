using System;
using System.Collections.Generic;
#if USN_USE_ASYNC_METHODS
using System.Threading.Tasks;
using System.Linq;

#else
using System.Collections;
#endif

namespace UnityScreenNavigator.Runtime.Core.Page
{
    public sealed class AnonymousPageLifecycleEvent : IPageLifecycleEvent
    {
#if USN_USE_ASYNC_METHODS
        public AnonymousPageLifecycleEvent(Func<Task> initialize = null,
            Func<Task> onWillPushEnter = null, Action onDidPushEnter = null,
            Func<Task> onWillPushExit = null, Action onDidPushExit = null,
            Func<Task> onWillPopEnter = null, Action onDidPopEnter = null,
            Func<Task> onWillPopExit = null, Action onDidPopExit = null,
            Func<Task> onCleanup = null)
#else
        public AnonymousPageLifecycleEvent(Func<IEnumerator> initialize = null,
            Func<IEnumerator> onWillPushEnter = null, Action onDidPushEnter = null,
            Func<IEnumerator> onWillPushExit = null, Action onDidPushExit = null,
            Func<IEnumerator> onWillPopEnter = null, Action onDidPopEnter = null,
            Func<IEnumerator> onWillPopExit = null, Action onDidPopExit = null,
            Func<IEnumerator> onCleanup = null)
#endif
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

#if USN_USE_ASYNC_METHODS
        public List<Func<Task>> OnInitialize { get; } = new List<Func<Task>>();
        public List<Func<Task>> OnWillPushEnter { get; } = new List<Func<Task>>();
        public List<Func<Task>> OnWillPushExit { get; } = new List<Func<Task>>();
        public List<Func<Task>> OnWillPopEnter { get; } = new List<Func<Task>>();
        public List<Func<Task>> OnWillPopExit { get; } = new List<Func<Task>>();
        public List<Func<Task>> OnCleanup { get; } = new List<Func<Task>>();
#else
        public List<Func<IEnumerator>> OnInitialize { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnWillPushEnter { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnWillPushExit { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnWillPopEnter { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnWillPopExit { get; } = new List<Func<IEnumerator>>();
        public List<Func<IEnumerator>> OnCleanup { get; } = new List<Func<IEnumerator>>();
#endif

#if USN_USE_ASYNC_METHODS
        Task IPageLifecycleEvent.Initialize()
        {
            return Task.WhenAll(OnInitialize.Select(x => x.Invoke()));
        }
#else
        IEnumerator IPageLifecycleEvent.Initialize()
        {
            foreach (var onInitialize in OnInitialize)
                yield return onInitialize.Invoke();
        }
#endif

#if USN_USE_ASYNC_METHODS
        Task IPageLifecycleEvent.WillPushEnter()
        {
            return Task.WhenAll(OnWillPushEnter.Select(x => x.Invoke()));
        }
#else
        IEnumerator IPageLifecycleEvent.WillPushEnter()
        {
            foreach (var onWillPushEnter in OnWillPushEnter)
                yield return onWillPushEnter.Invoke();
        }
#endif

        void IPageLifecycleEvent.DidPushEnter()
        {
            OnDidPushEnter?.Invoke();
        }

#if USN_USE_ASYNC_METHODS
        Task IPageLifecycleEvent.WillPushExit()
        {
            return Task.WhenAll(OnWillPushExit.Select(x => x.Invoke()));
        }
#else
        IEnumerator IPageLifecycleEvent.WillPushExit()
        {
            foreach (var onWillPushExit in OnWillPushExit)
                yield return onWillPushExit.Invoke();
        }
#endif

        void IPageLifecycleEvent.DidPushExit()
        {
            OnDidPushExit?.Invoke();
        }

#if USN_USE_ASYNC_METHODS
        Task IPageLifecycleEvent.WillPopEnter()
        {
            return Task.WhenAll(OnWillPopEnter.Select(x => x.Invoke()));
        }
#else
        IEnumerator IPageLifecycleEvent.WillPopEnter()
        {
            foreach (var onWillPopEnter in OnWillPopEnter)
                yield return onWillPopEnter.Invoke();
        }
#endif

        void IPageLifecycleEvent.DidPopEnter()
        {
            OnDidPopEnter?.Invoke();
        }

#if USN_USE_ASYNC_METHODS
        Task IPageLifecycleEvent.WillPopExit()
        {
            return Task.WhenAll(OnWillPopExit.Select(x => x.Invoke()));
        }
#else
        IEnumerator IPageLifecycleEvent.WillPopExit()
        {
            foreach (var onWillPopExit in OnWillPopExit)
                yield return onWillPopExit.Invoke();
        }
#endif

        void IPageLifecycleEvent.DidPopExit()
        {
            OnDidPopExit?.Invoke();
        }

#if USN_USE_ASYNC_METHODS
        Task IPageLifecycleEvent.Cleanup()
        {
            return Task.WhenAll(OnCleanup.Select(x => x.Invoke()));
        }
#else
        IEnumerator IPageLifecycleEvent.Cleanup()
        {
            foreach (var onCleanup in OnCleanup)
                yield return onCleanup.Invoke();
        }
#endif

        public event Action OnDidPushEnter;
        public event Action OnDidPushExit;
        public event Action OnDidPopEnter;
        public event Action OnDidPopExit;
    }
}
