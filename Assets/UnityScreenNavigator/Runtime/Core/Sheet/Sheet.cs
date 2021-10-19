using System.Collections;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.Animation;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    [DisallowMultipleComponent]
    public class Sheet : MonoBehaviour
    {
        [SerializeField] private string _identifier;

        [SerializeField] private int _renderingOrder;

        [SerializeField]
        private SheetTransitionAnimationContainer _animationContainer = new SheetTransitionAnimationContainer();

        private CanvasGroup _canvasGroup;
        private bool _isInitialized;
        private RectTransform _parentTransform;
        private RectTransform _rectTransform;

        public string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        public SheetTransitionAnimationContainer AnimationContainer => _animationContainer;

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        public virtual IEnumerator Initialize()
        {
            yield break;
        }

        public virtual IEnumerator WillEnter()
        {
            yield break;
        }

        public virtual void DidEnter()
        {
        }

        public virtual IEnumerator WillExit()
        {
            yield break;
        }

        public virtual void DidExit()
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
                {
                    continue;
                }

                break;
            }

            _rectTransform.SetSiblingIndex(siblingIndex);

            gameObject.SetActive(false);
            return CoroutineManager.Instance.Run(Initialize());
        }

        internal AsyncProcessHandle BeforeEnter(Sheet partnerSheet)
        {
            return CoroutineManager.Instance.Run(BeforeEnterRoutine(partnerSheet));
        }

        private IEnumerator BeforeEnterRoutine(Sheet partnerSheet)
        {
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                _canvasGroup.interactable = false;
            }

            _canvasGroup.alpha = 0.0f;

            var handle = CoroutineManager.Instance.Run(WillEnter());
            while (!handle.IsTerminated)
            {
                yield return null;
            }
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
                {
                    anim = UnityScreenNavigatorSettings.Instance.GetDefaultSheetTransitionAnimation(true);
                }

                anim.SetPartner(partnerSheet?.transform as RectTransform);
                anim.Setup(_rectTransform);
                yield return CoroutineManager.Instance.Run(anim.CreatePlayRoutine());
            }

            _rectTransform.FillParent(_parentTransform);
        }

        internal void AfterEnter(Sheet partnerSheet)
        {
            DidEnter();
            
            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                _canvasGroup.interactable = true;
            }
        }

        internal AsyncProcessHandle BeforeExit(Sheet partnerSheet)
        {
            return CoroutineManager.Instance.Run(BeforeExitRoutine(partnerSheet));
        }

        private IEnumerator BeforeExitRoutine(Sheet partnerSheet)
        {
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                _canvasGroup.interactable = false;
            }

            _canvasGroup.alpha = 1.0f;

            var handle = CoroutineManager.Instance.Run(WillExit());
            while (!handle.IsTerminated)
            {
                yield return null;
            }
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
                {
                    anim = UnityScreenNavigatorSettings.Instance.GetDefaultSheetTransitionAnimation(false);
                }

                anim.SetPartner(partnerSheet?.transform as RectTransform);
                anim.Setup(_rectTransform);
                yield return CoroutineManager.Instance.Run(anim.CreatePlayRoutine());
            }

            _canvasGroup.alpha = 0.0f;
        }

        internal void AfterExit(Sheet partnerSheet)
        {
            DidExit();
            gameObject.SetActive(false);
        }

        internal AsyncProcessHandle BeforeRelease()
        {
            return CoroutineManager.Instance.Run(Cleanup());
        }
    }
}