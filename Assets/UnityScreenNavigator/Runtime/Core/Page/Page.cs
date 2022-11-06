using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;
using UnityScreenNavigator.Runtime.Foundation.PriorityCollection;
#if USN_USE_ASYNC_METHODS
using System;
using System.Threading.Tasks;
#endif

namespace UnityScreenNavigator.Runtime.Core.Page
{
    [DisallowMultipleComponent]
    public class Page : MonoBehaviour, IPageLifecycleEvent
    {
        [SerializeField] private bool _usePrefabNameAsIdentifier = true;

        [SerializeField] [EnabledIf(nameof(_usePrefabNameAsIdentifier), false)]
        private string _identifier;

        [SerializeField] private int _renderingOrder;

        [SerializeField]
        private PageTransitionAnimationContainer _animationContainer = new PageTransitionAnimationContainer();

        private CanvasGroup _canvasGroup;
        private RectTransform _parentTransform;
        private RectTransform _rectTransform;
        private Progress<float> _transitionProgressReporter;

        private Progress<float> TransitionProgressReporter
        {
            get
            {
                if (_transitionProgressReporter == null)
                    _transitionProgressReporter = new Progress<float>(SetTransitionProgress);
                return _transitionProgressReporter;
            }
        }

        private readonly PriorityList<IPageLifecycleEvent> _lifecycleEvents = new PriorityList<IPageLifecycleEvent>();

        public string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        public PageTransitionAnimationContainer AnimationContainer => _animationContainer;

        public bool IsTransitioning { get; private set; }

        /// <summary>
        ///     Return the transition animation type currently playing.
        ///     If not in transition, return null.
        /// </summary>
        public PageTransitionAnimationType? TransitionAnimationType { get; private set; }

        /// <summary>
        ///     Progress of the transition animation.
        /// </summary>
        public float TransitionAnimationProgress { get; private set; }

        /// <summary>
        ///     Event when the transition animation progress changes.
        /// </summary>
        public event Action<float> TransitionAnimationProgressChanged;

#if USN_USE_ASYNC_METHODS
        public virtual Task Initialize()
        {
            return Task.CompletedTask;
        }
#else
        public virtual IEnumerator Initialize()
        {
            yield break;
        }
#endif

#if USN_USE_ASYNC_METHODS
        public virtual Task WillPushEnter()
        {
            return Task.CompletedTask;
        }
#else
        public virtual IEnumerator WillPushEnter()
        {
            yield break;
        }
#endif

        public virtual void DidPushEnter()
        {
        }

#if USN_USE_ASYNC_METHODS
        public virtual Task WillPushExit()
        {
            return Task.CompletedTask;
        }
#else
        public virtual IEnumerator WillPushExit()
        {
            yield break;
        }
#endif

        public virtual void DidPushExit()
        {
        }

#if USN_USE_ASYNC_METHODS
        public virtual Task WillPopEnter()
        {
            return Task.CompletedTask;
        }
#else
        public virtual IEnumerator WillPopEnter()
        {
            yield break;
        }
#endif

        public virtual void DidPopEnter()
        {
        }

#if USN_USE_ASYNC_METHODS
        public virtual Task WillPopExit()
        {
            return Task.CompletedTask;
        }
#else
        public virtual IEnumerator WillPopExit()
        {
            yield break;
        }
#endif

        public virtual void DidPopExit()
        {
        }

#if USN_USE_ASYNC_METHODS
        public virtual Task Cleanup()
        {
            return Task.CompletedTask;
        }
#else
        public virtual IEnumerator Cleanup()
        {
            yield break;
        }
#endif

        public void AddLifecycleEvent(IPageLifecycleEvent lifecycleEvent, int priority = 0)
        {
            _lifecycleEvents.Add(lifecycleEvent, priority);
        }

        public void RemoveLifecycleEvent(IPageLifecycleEvent lifecycleEvent)
        {
            _lifecycleEvents.Remove(lifecycleEvent);
        }

        internal AsyncProcessHandle AfterLoad(RectTransform parentTransform)
        {
            _rectTransform = (RectTransform)transform;
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _lifecycleEvents.Add(this, 0);
            _identifier = _usePrefabNameAsIdentifier ? gameObject.name.Replace("(Clone)", string.Empty) : _identifier;
            _parentTransform = parentTransform;
            _rectTransform.FillParent(_parentTransform);

            // Set order of rendering.
            var siblingIndex = 0;
            for (var i = 0; i < _parentTransform.childCount; i++)
            {
                var child = _parentTransform.GetChild(i);
                var childPage = child.GetComponent<Page>();
                siblingIndex = i;
                if (_renderingOrder >= childPage._renderingOrder)
                    continue;

                break;
            }

            _rectTransform.SetSiblingIndex(siblingIndex);

            _canvasGroup.alpha = 0.0f;

            return CoroutineManager.Instance.Run(CreateCoroutine(_lifecycleEvents.Select(x => x.Initialize())));
        }


        internal AsyncProcessHandle BeforeEnter(bool push, Page partnerPage)
        {
            return CoroutineManager.Instance.Run(BeforeEnterRoutine(push, partnerPage));
        }

        private IEnumerator BeforeEnterRoutine(bool push, Page partnerPage)
        {
            IsTransitioning = true;
            TransitionAnimationType =
                push ? PageTransitionAnimationType.PushEnter : PageTransitionAnimationType.PopEnter;
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            SetTransitionProgress(0.0f);
            _canvasGroup.alpha = 0.0f;

            var routines = push
                ? _lifecycleEvents.Select(x => x.WillPushEnter())
                : _lifecycleEvents.Select(x => x.WillPopEnter());
            var handle = CoroutineManager.Instance.Run(CreateCoroutine(routines));

            while (!handle.IsTerminated)
                yield return null;
        }

        internal AsyncProcessHandle Enter(bool push, bool playAnimation, Page partnerPage)
        {
            return CoroutineManager.Instance.Run(EnterRoutine(push, playAnimation, partnerPage));
        }

        private IEnumerator EnterRoutine(bool push, bool playAnimation, Page partnerPage)
        {
            _canvasGroup.alpha = 1.0f;

            if (playAnimation)
            {
                var anim = _animationContainer.GetAnimation(push, true, partnerPage?.Identifier);
                if (anim == null)
                    anim = UnityScreenNavigatorSettings.Instance.GetDefaultPageTransitionAnimation(push, true);

                anim.SetPartner(partnerPage?.transform as RectTransform);
                anim.Setup(_rectTransform);
                yield return CoroutineManager.Instance.Run(anim.CreatePlayRoutine(TransitionProgressReporter));
            }

            _rectTransform.FillParent(_parentTransform);
            SetTransitionProgress(1.0f);
        }

        internal void AfterEnter(bool push, Page partnerPage)
        {
            if (push)
                foreach (var lifecycleEvent in _lifecycleEvents)
                    lifecycleEvent.DidPushEnter();
            else
                foreach (var lifecycleEvent in _lifecycleEvents)
                    lifecycleEvent.DidPopEnter();

            IsTransitioning = false;
            TransitionAnimationType = null;
        }

        internal AsyncProcessHandle BeforeExit(bool push, Page partnerPage)
        {
            return CoroutineManager.Instance.Run(BeforeExitRoutine(push, partnerPage));
        }

        private IEnumerator BeforeExitRoutine(bool push, Page partnerPage)
        {
            IsTransitioning = true;
            TransitionAnimationType = push ? PageTransitionAnimationType.PushExit : PageTransitionAnimationType.PopExit;
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            SetTransitionProgress(0.0f);
            _canvasGroup.alpha = 1.0f;

            var routines = push
                ? _lifecycleEvents.Select(x => x.WillPushExit())
                : _lifecycleEvents.Select(x => x.WillPopExit());
            var handle = CoroutineManager.Instance.Run(CreateCoroutine(routines));

            while (!handle.IsTerminated)
                yield return null;
        }

        internal AsyncProcessHandle Exit(bool push, bool playAnimation, Page partnerPage)
        {
            return CoroutineManager.Instance.Run(ExitRoutine(push, playAnimation, partnerPage));
        }

        private IEnumerator ExitRoutine(bool push, bool playAnimation, Page partnerPage)
        {
            if (playAnimation)
            {
                var anim = _animationContainer.GetAnimation(push, false, partnerPage?.Identifier);
                if (anim == null)
                    anim = UnityScreenNavigatorSettings.Instance.GetDefaultPageTransitionAnimation(push, false);

                anim.SetPartner(partnerPage?.transform as RectTransform);
                anim.Setup(_rectTransform);
                yield return CoroutineManager.Instance.Run(anim.CreatePlayRoutine(TransitionProgressReporter));
            }

            _canvasGroup.alpha = 0.0f;
            SetTransitionProgress(1.0f);
        }

        internal void AfterExit(bool push, Page partnerPage)
        {
            if (push)
                foreach (var lifecycleEvent in _lifecycleEvents)
                    lifecycleEvent.DidPushExit();
            else
                foreach (var lifecycleEvent in _lifecycleEvents)
                    lifecycleEvent.DidPopExit();

            gameObject.SetActive(false);
            IsTransitioning = false;
            TransitionAnimationType = null;
        }

        internal AsyncProcessHandle BeforeRelease()
        {
            return CoroutineManager.Instance.Run(CreateCoroutine(_lifecycleEvents.Select(x => x.Cleanup())));
        }

#if USN_USE_ASYNC_METHODS
        private IEnumerator CreateCoroutine(IEnumerable<Task> targets)
#else
        private IEnumerator CreateCoroutine(IEnumerable<IEnumerator> targets)
#endif
        {
            foreach (var target in targets)
            {
                var handle = CoroutineManager.Instance.Run(CreateCoroutine(target));
                if (!handle.IsTerminated)
                    yield return handle;
            }
        }

#if USN_USE_ASYNC_METHODS
        private IEnumerator CreateCoroutine(Task target)
#else
        private IEnumerator CreateCoroutine(IEnumerator target)
#endif
        {
#if USN_USE_ASYNC_METHODS
            async void WaitTaskAndCallback(Task task, Action callback)
            {
                await task;
                callback?.Invoke();
            }
            
            var isCompleted = false;
            WaitTaskAndCallback(target, () =>
            {
                isCompleted = true;
            });
            return new WaitUntil(() => isCompleted);
#else
            return target;
#endif
        }

        private void SetTransitionProgress(float progress)
        {
            TransitionAnimationProgress = progress;
            TransitionAnimationProgressChanged?.Invoke(progress);
        }
    }
}
