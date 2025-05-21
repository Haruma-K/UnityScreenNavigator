using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Foundation;
#if USN_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    [DisallowMultipleComponent]
    public class Modal : MonoBehaviour, IModalLifecycleEvent
    {
        [SerializeField] private bool _usePrefabNameAsIdentifier = true;

        [SerializeField] [EnabledIf(nameof(_usePrefabNameAsIdentifier), false)]
        private string _identifier;

        [SerializeField] private ModalTransitionAnimationContainer _animationContainer = new();

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

        private readonly CompositeLifecycleEvent<IModalLifecycleEvent> _lifecycleEvents = new();

        public bool UsePrefabNameAsIdentifier
        {
            get => _usePrefabNameAsIdentifier;
            set => _usePrefabNameAsIdentifier = value;
        }

        public string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        public ModalTransitionAnimationContainer AnimationContainer => _animationContainer;

        public bool IsTransitioning { get; private set; }

        /// <summary>
        ///     Return the transition animation type currently playing.
        ///     If not in transition, return null.
        /// </summary>
        public ModalTransitionAnimationType? TransitionAnimationType { get; private set; }

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

        public void AddLifecycleEvent(IModalLifecycleEvent lifecycleEvent, int priority = 0)
        {
            _lifecycleEvents.AddItem(lifecycleEvent, priority);
        }

        public void RemoveLifecycleEvent(IModalLifecycleEvent lifecycleEvent)
        {
            _lifecycleEvents.RemoveItem(lifecycleEvent);
        }

        internal AsyncProcessHandle AfterLoad(RectTransform parentTransform)
        {
            _rectTransform = (RectTransform)transform;
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _lifecycleEvents.AddItem(this, 0);
            _identifier = _usePrefabNameAsIdentifier ? gameObject.name.Replace("(Clone)", string.Empty) : _identifier;
            _parentTransform = parentTransform;
            _rectTransform.FillParent(_parentTransform);
            _canvasGroup.alpha = 0.0f;

            var processes = _lifecycleEvents
                .GetOrderedItems()
                .Select(x => AsyncProcess.Run(x.Initialize()));
            return AsyncProcess.WhenAll(processes);
        }

        internal AsyncProcessHandle BeforeEnter(bool push, Modal partnerModal)
        {
            IsTransitioning = true;
            if (push)
            {
                TransitionAnimationType = ModalTransitionAnimationType.Enter;
                gameObject.SetActive(true);
                _rectTransform.FillParent(_parentTransform);
                _canvasGroup.alpha = 0.0f;
            }

            SetTransitionProgress(0.0f);

            var orderedEvents = _lifecycleEvents.GetOrderedItems();
            var processes = push
                ? orderedEvents.Select(x => AsyncProcess.Run(x.WillPushEnter()))
                : orderedEvents.Select(x => AsyncProcess.Run(x.WillPopEnter()));
            return AsyncProcess.WhenAll(processes);
        }

        internal AsyncProcessHandle Enter(bool push, bool playAnimation, Modal partnerModal)
        {
            return CoroutineScheduler.Run(EnterRoutine(push, playAnimation, partnerModal));
        }

        private IEnumerator EnterRoutine(bool push, bool playAnimation, Modal partnerModal)
        {
            if (push)
            {
                _canvasGroup.alpha = 1.0f;

                if (playAnimation)
                {
                    var anim = _animationContainer.GetAnimation(true, partnerModal?.Identifier);
                    if (anim == null)
                        anim = UnityScreenNavigatorSettings.Instance.GetDefaultModalTransitionAnimation(true);

                    if (anim.Duration > 0.0f)
                    {
                        anim.SetPartner(partnerModal?.transform as RectTransform);
                        anim.Setup(_rectTransform);
                        yield return anim.CreatePlayRoutine(TransitionProgressReporter);
                    }
                }

                _rectTransform.FillParent(_parentTransform);
            }

            SetTransitionProgress(1.0f);
        }

        internal void AfterEnter(bool push, Modal partnerModal)
        {
            if (push)
                _lifecycleEvents.ExecuteLifecycleEventsSequentially(x => x.DidPushEnter());
            else
                _lifecycleEvents.ExecuteLifecycleEventsSequentially(x => x.DidPopEnter());

            IsTransitioning = false;
            TransitionAnimationType = null;
        }

        internal AsyncProcessHandle BeforeExit(bool push, Modal partnerModal)
        {
            return CoroutineScheduler.Instance.Run(BeforeExitRoutine(push, partnerModal));
        }

        private async Task<AsyncProcessHandle> BeforeExitRoutine(bool push, Modal partnerModal)
        {
            IsTransitioning = true;
            if (!push)
            {
                TransitionAnimationType = ModalTransitionAnimationType.Exit;
                gameObject.SetActive(true);
                _rectTransform.FillParent(_parentTransform);
                _canvasGroup.alpha = 1.0f;
            }

            SetTransitionProgress(0.0f);

            var processes = push
                ? _lifecycleEvents.GetOrderedItems().Select(x => x.WillPushExit())
                : _lifecycleEvents.GetOrderedItems().Select(x => x.WillPopExit());

            var handles = new List<AsyncProcessHandle>();
            foreach (var process in processes)
            {
                var handle = AsyncProcess.Run(process);
                handles.Add(handle);
                await handle.Task;
            }

            return AsyncProcess.WhenAll(handles);
        }

        internal AsyncProcessHandle Exit(bool push, bool playAnimation, Modal partnerModal)
        {
            return CoroutineScheduler.Instance.Run(ExitRoutine(push, playAnimation, partnerModal));
        }

        private IEnumerator ExitRoutine(bool push, bool playAnimation, Modal partnerModal)
        {
            if (!push)
            {
                if (playAnimation)
                {
                    var anim = _animationContainer.GetAnimation(false, partnerModal?._identifier);
                    if (anim == null)
                        anim = UnityScreenNavigatorSettings.Instance.GetDefaultModalTransitionAnimation(false);

                    if (anim.Duration > 0.0f)
                    {
                        anim.SetPartner(partnerModal?.transform as RectTransform);
                        anim.Setup(_rectTransform);
                        yield return CoroutineScheduler.Instance.Run(anim.CreatePlayRoutine(TransitionProgressReporter));
                    }
                }

                _canvasGroup.alpha = 0.0f;
            }

            SetTransitionProgress(1.0f);
        }

        internal void AfterExit(bool push, Modal partnerModal)
        {
            if (push)
                _lifecycleEvents.ExecuteLifecycleEventsSequentially(x => x.DidPushExit());
            else
                _lifecycleEvents.ExecuteLifecycleEventsSequentially(x => x.DidPopExit());

            IsTransitioning = false;
            TransitionAnimationType = null;
        }

        internal void BeforeReleaseAndForget()
        {
            foreach (var item in _lifecycleEvents.GetOrderedItems())
                AsyncProcess.Run(item.Cleanup());
        }

        internal AsyncProcessHandle BeforeRelease()
        {
            var handles = _lifecycleEvents.GetOrderedItems()
                .Select(x => AsyncProcess.Run(x.Cleanup()));
            return AsyncProcess.WhenAll(handles);
        }

        private void SetTransitionProgress(float progress)
        {
            TransitionAnimationProgress = progress;
            TransitionAnimationProgressChanged?.Invoke(progress);
        }
    }
}