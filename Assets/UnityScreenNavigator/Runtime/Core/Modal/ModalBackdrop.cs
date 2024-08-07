using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    public class ModalBackdrop : MonoBehaviour
    {
        [SerializeField] private ModalBackdropTransitionAnimationContainer _animationContainer;
        [SerializeField] private bool _closeModalWhenClicked;

        private CanvasGroup _canvasGroup;
        private RectTransform _parentTransform;
        private RectTransform _rectTransform;

        public ModalBackdropTransitionAnimationContainer AnimationContainer => _animationContainer;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            
            if (_closeModalWhenClicked)
            {
                if (!TryGetComponent<Image>(out var image))
                {
                    image = gameObject.AddComponent<Image>();
                    image.color = Color.clear;
                }
                
                if (!TryGetComponent<Button>(out var button))
                {
                    button = gameObject.AddComponent<Button>();
                    button.transition = Selectable.Transition.None;
                }
                button.onClick.AddListener(() =>
                {
                    var modalContainer = ModalContainer.Of(transform);
                    if (modalContainer.IsInTransition)
                        return;
                    modalContainer.Pop(true);
                });
            }
        }

        public void Setup(RectTransform parentTransform, int modalIndex)
        {
            _parentTransform = parentTransform;
            _rectTransform.FillParent(_parentTransform);
            _canvasGroup.interactable = _closeModalWhenClicked;
            OnSetup(parentTransform, modalIndex);
            gameObject.SetActive(false);
        }

        protected virtual void OnSetup(RectTransform parentTransform, int modalIndex)
        {
        }

        internal AsyncProcessHandle Enter(bool playAnimation)
        {
            return CoroutineManager.Instance.Run(EnterRoutine(playAnimation));
        }

        private IEnumerator EnterRoutine(bool playAnimation)
        {
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            _canvasGroup.alpha = 1;

            if (playAnimation)
            {
                var anim = _animationContainer.GetAnimation(true);
                if (anim == null)
                {
                    anim = UnityScreenNavigatorSettings.Instance.ModalBackdropEnterAnimation;
                }

                if (anim.Duration > 0)
                {
                    anim.Setup(_rectTransform);
                    yield return CoroutineManager.Instance.Run(anim.CreatePlayRoutine());
                }
            }

            _rectTransform.FillParent(_parentTransform);
        }

        internal AsyncProcessHandle Exit(bool playAnimation)
        {
            return CoroutineManager.Instance.Run(ExitRoutine(playAnimation));
        }

        private IEnumerator ExitRoutine(bool playAnimation)
        {
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            _canvasGroup.alpha = 1;

            if (playAnimation)
            {
                var anim = _animationContainer.GetAnimation(false);
                if (anim == null)
                {
                    anim = UnityScreenNavigatorSettings.Instance.ModalBackdropExitAnimation;
                }

                if (anim.Duration > 0)
                {
                    anim.Setup(_rectTransform);
                    yield return CoroutineManager.Instance.Run(anim.CreatePlayRoutine());
                }
            }

            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
    }
}
