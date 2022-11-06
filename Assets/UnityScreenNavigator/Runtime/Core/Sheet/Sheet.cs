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

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    [DisallowMultipleComponent]
    public class Sheet : MonoBehaviour, ISheetLifecycleEvent
    {
        [SerializeField] private string _identifier;

        [SerializeField] private int _renderingOrder;

        [SerializeField]
        private SheetTransitionAnimationContainer _animationContainer = new SheetTransitionAnimationContainer();

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

        private readonly PriorityList<ISheetLifecycleEvent> _lifecycleEvents = new PriorityList<ISheetLifecycleEvent>();

        public string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        public SheetTransitionAnimationContainer AnimationContainer => _animationContainer;

        public bool IsTransitioning { get; private set; }

        /// <summary>
        ///     Return the transition animation type currently playing.
        ///     If not in transition, return null.
        /// </summary>
        public SheetTransitionAnimationType? TransitionAnimationType { get; private set; }

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
        public virtual Task WillEnter()
        {
            return Task.CompletedTask;
        }
#else
        public virtual IEnumerator WillEnter()
        {
            yield break;
        }
#endif

        public virtual void DidEnter()
        {
        }

#if USN_USE_ASYNC_METHODS
        public virtual Task WillExit()
        {
            return Task.CompletedTask;
        }
#else
        public virtual IEnumerator WillExit()
        {
            yield break;
        }
#endif

        public virtual void DidExit()
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

        public void AddLifecycleEvent(ISheetLifecycleEvent lifecycleEvent, int priority = 0)
        {
            _lifecycleEvents.Add(lifecycleEvent, priority);
        }

        public void RemoveLifecycleEvent(ISheetLifecycleEvent lifecycleEvent)
        {
            _lifecycleEvents.Remove(lifecycleEvent);
        }

        internal AsyncProcessHandle AfterLoad(RectTransform parentTransform)
        {
            _rectTransform = (RectTransform)transform;
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _lifecycleEvents.Add(this, 0);
            _parentTransform = parentTransform;
            _rectTransform.FillParent(_parentTransform);

            // Set order of rendering.
            var siblingIndex = 0;
            for (var i = 0; i < _parentTransform.childCount; i++)
            {
                var child = _parentTransform.GetChild(i);
                var childPage = child.GetComponent<Sheet>();
                siblingIndex = i;
                if (_renderingOrder >= childPage._renderingOrder)
                    continue;

                break;
            }

            _rectTransform.SetSiblingIndex(siblingIndex);

            gameObject.SetActive(false);

            return CoroutineManager.Instance.Run(CreateCoroutine(_lifecycleEvents.Select(x => x.Initialize())));
        }

        internal AsyncProcessHandle BeforeEnter(Sheet partnerSheet)
        {
            return CoroutineManager.Instance.Run(BeforeEnterRoutine(partnerSheet));
        }

        private IEnumerator BeforeEnterRoutine(Sheet partnerSheet)
        {
            IsTransitioning = true;
            TransitionAnimationType = SheetTransitionAnimationType.Enter;
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            SetTransitionProgress(0.0f);

            _canvasGroup.alpha = 0.0f;

            var handle = CoroutineManager.Instance.Run(CreateCoroutine(_lifecycleEvents.Select(x => x.WillEnter())));
            while (!handle.IsTerminated)
                yield return null;
        }

        internal AsyncProcessHandle Enter(bool playAnimation, Sheet partnerSheet)
        {
            return CoroutineManager.Instance.Run(EnterRoutine(playAnimation, partnerSheet));
        }

        private IEnumerator EnterRoutine(bool playAnimation, Sheet partnerSheet)
        {
            _canvasGroup.alpha = 1.0f;

            if (playAnimation)
            {
                var anim = _animationContainer.GetAnimation(true, partnerSheet?._identifier);
                if (anim == null)
                    anim = UnityScreenNavigatorSettings.Instance.GetDefaultSheetTransitionAnimation(true);

                anim.SetPartner(partnerSheet?.transform as RectTransform);
                anim.Setup(_rectTransform);
                yield return CoroutineManager.Instance.Run(anim.CreatePlayRoutine(TransitionProgressReporter));
            }

            _rectTransform.FillParent(_parentTransform);
            SetTransitionProgress(1.0f);
        }

        internal void AfterEnter(Sheet partnerSheet)
        {
            foreach (var lifecycleEvent in _lifecycleEvents)
                lifecycleEvent.DidEnter();

            IsTransitioning = false;
            TransitionAnimationType = null;
        }

        internal AsyncProcessHandle BeforeExit(Sheet partnerSheet)
        {
            return CoroutineManager.Instance.Run(BeforeExitRoutine(partnerSheet));
        }

        private IEnumerator BeforeExitRoutine(Sheet partnerSheet)
        {
            IsTransitioning = true;
            TransitionAnimationType = SheetTransitionAnimationType.Exit;
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            SetTransitionProgress(0.0f);

            _canvasGroup.alpha = 1.0f;

            var handle = CoroutineManager.Instance.Run(CreateCoroutine(_lifecycleEvents.Select(x => x.WillExit())));
            while (!handle.IsTerminated)
                yield return null;
        }

        internal AsyncProcessHandle Exit(bool playAnimation, Sheet partnerSheet)
        {
            return CoroutineManager.Instance.Run(ExitRoutine(playAnimation, partnerSheet));
        }

        private IEnumerator ExitRoutine(bool playAnimation, Sheet partnerSheet)
        {
            if (playAnimation)
            {
                var anim = _animationContainer.GetAnimation(false, partnerSheet?.Identifier);
                if (anim == null)
                    anim = UnityScreenNavigatorSettings.Instance.GetDefaultSheetTransitionAnimation(false);

                anim.SetPartner(partnerSheet?.transform as RectTransform);
                anim.Setup(_rectTransform);
                yield return CoroutineManager.Instance.Run(anim.CreatePlayRoutine(TransitionProgressReporter));
            }

            _canvasGroup.alpha = 0.0f;
            SetTransitionProgress(1.0f);
        }

        internal void AfterExit(Sheet partnerSheet)
        {
            foreach (var lifecycleEvent in _lifecycleEvents)
                lifecycleEvent.DidExit();

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
