using System.Collections;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.Animation;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    [DisallowMultipleComponent]
    public class Modal : MonoBehaviour
    {
        [SerializeField] private bool _usePrefabNameAsIdentifier = true;

        [SerializeField] [EnabledIf(nameof(_usePrefabNameAsIdentifier), false)]
        private string _identifier;

        [SerializeField]
        private ModalTransitionAnimationContainer _animationContainer = new ModalTransitionAnimationContainer();

        private CanvasGroup _canvasGroup;
        private bool _isInitialized;
        private RectTransform _parentTransform;
        private RectTransform _rectTransform;

        public string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        public ModalTransitionAnimationContainer AnimationContainer => _animationContainer;

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        public virtual IEnumerator Initialize()
        {
            yield break;
        }

        public virtual IEnumerator WillPushEnter()
        {
            yield break;
        }

        public virtual void DidPushEnter()
        {
        }

        public virtual IEnumerator WillPushExit()
        {
            yield break;
        }

        public virtual void DidPushExit()
        {
        }

        public virtual IEnumerator WillPopEnter()
        {
            yield break;
        }

        public virtual void DidPopEnter()
        {
        }

        public virtual IEnumerator WillPopExit()
        {
            yield break;
        }

        public virtual void DidPopExit()
        {
        }

        public virtual IEnumerator Cleanup()
        {
            yield break;
        }

        internal AsyncProcessHandle AfterLoad(RectTransform parentTransform)
        {
            if (!_isInitialized)
            {
                _rectTransform = (RectTransform)transform;
                _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
                _isInitialized = true;
            }

            _identifier = _usePrefabNameAsIdentifier ? gameObject.name.Replace("(Clone)", string.Empty) : _identifier;
            _parentTransform = parentTransform;
            _rectTransform.FillParent(_parentTransform);
            _canvasGroup.alpha = 0.0f;

            return CoroutineManager.Instance.Run(Initialize());
        }

        internal AsyncProcessHandle BeforeEnter(bool push, Modal partnerModal)
        {
            return CoroutineManager.Instance.Run(BeforeEnterRoutine(push, partnerModal));
        }

        private IEnumerator BeforeEnterRoutine(bool push, Modal partnerModal)
        {
            if (push)
            {
                gameObject.SetActive(true);
                _rectTransform.FillParent(_parentTransform);
                _canvasGroup.alpha = 0.0f;
            }

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                _canvasGroup.interactable = false;
            }

            var routine = push ? WillPushEnter() : WillPopEnter();
            var handle = CoroutineManager.Instance.Run(routine);

            while (!handle.IsTerminated)
            {
                yield return null;
            }
        }

        internal AsyncProcessHandle Enter(bool push, bool playAnimation, Modal partnerModal)
        {
            return CoroutineManager.Instance.Run(EnterRoutine(push, playAnimation, partnerModal));
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
                    {
                        anim = UnityScreenNavigatorSettings.Instance.GetDefaultModalTransitionAnimation(true);
                    }

                    anim.SetPartner(partnerModal?.transform as RectTransform);
                    anim.Setup(_rectTransform);
                    yield return CoroutineManager.Instance.Run(anim.CreatePlayRoutine());
                }

                _rectTransform.FillParent(_parentTransform);
            }
        }

        internal void AfterEnter(bool push, Modal partnerModal)
        {
            if (push)
            {
                DidPushEnter();
            }
            else
            {
                DidPopEnter();
            }

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                _canvasGroup.interactable = true;
            }
        }

        internal AsyncProcessHandle BeforeExit(bool push, Modal partnerModal)
        {
            return CoroutineManager.Instance.Run(BeforeExitRoutine(push, partnerModal));
        }

        private IEnumerator BeforeExitRoutine(bool push, Modal partnerModal)
        {
            if (!push)
            {
                gameObject.SetActive(true);
                _rectTransform.FillParent(_parentTransform);
                _canvasGroup.alpha = 1.0f;
            }

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                _canvasGroup.interactable = false;
            }

            var routine = push ? WillPushExit() : WillPopExit();
            var handle = CoroutineManager.Instance.Run(routine);

            while (!handle.IsTerminated)
            {
                yield return null;
            }
        }

        internal AsyncProcessHandle Exit(bool push, bool playAnimation, Modal partnerModal)
        {
            return CoroutineManager.Instance.Run(ExitRoutine(push, playAnimation, partnerModal));
        }

        private IEnumerator ExitRoutine(bool push, bool playAnimation, Modal partnerModal)
        {
            if (!push)
            {
                if (playAnimation)
                {
                    var anim = _animationContainer.GetAnimation(false, partnerModal?._identifier);
                    if (anim == null)
                    {
                        anim = UnityScreenNavigatorSettings.Instance.GetDefaultModalTransitionAnimation(false);
                    }

                    anim.SetPartner(partnerModal?.transform as RectTransform);
                    anim.Setup(_rectTransform);
                    yield return CoroutineManager.Instance.Run(anim.CreatePlayRoutine());
                }

                _canvasGroup.alpha = 0.0f;
            }
        }

        internal void AfterExit(bool push, Modal partnerModal)
        {
            if (push)
            {
                DidPushExit();
            }
            else
            {
                DidPopExit();
            }
        }

        internal AsyncProcessHandle BeforeRelease()
        {
            return CoroutineManager.Instance.Run(Cleanup());
        }
    }
}