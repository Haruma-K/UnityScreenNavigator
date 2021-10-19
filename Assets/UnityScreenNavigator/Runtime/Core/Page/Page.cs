using System.Collections;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.Animation;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    [DisallowMultipleComponent]
    public class Page : MonoBehaviour
    {
        [SerializeField] private bool _usePrefabNameAsIdentifier = true;

        [SerializeField] [EnabledIf(nameof(_usePrefabNameAsIdentifier), false)]
        private string _identifier;

        [SerializeField] private int _renderingOrder;

        [SerializeField]
        private PageTransitionAnimationContainer _animationContainer = new PageTransitionAnimationContainer();

        private CanvasGroup _canvasGroup;
        private bool _isInitialized;
        private RectTransform _parentTransform;
        private RectTransform _rectTransform;

        public string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        public PageTransitionAnimationContainer AnimationContainer => _animationContainer;

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

            // Set order of rendering.
            var siblingIndex = 0;
            for (var i = 0; i < _parentTransform.childCount; i++)
            {
                var child = _parentTransform.GetChild(i);
                var childPage = child.GetComponent<Page>();
                siblingIndex = i;
                if (_renderingOrder >= childPage._renderingOrder)
                {
                    continue;
                }

                break;
            }

            _rectTransform.SetSiblingIndex(siblingIndex);

            _canvasGroup.alpha = 0.0f;

            return CoroutineManager.Instance.Run(Initialize());
        }

        internal AsyncProcessHandle BeforeEnter(bool push, Page partnerPage)
        {
            return CoroutineManager.Instance.Run(BeforeEnterRoutine(push, partnerPage));
        }

        private IEnumerator BeforeEnterRoutine(bool push, Page partnerPage)
        {
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                _canvasGroup.interactable = false;
            }
            _canvasGroup.alpha = 0.0f;

            var routine = push ? WillPushEnter() : WillPopEnter();
            var handle = CoroutineManager.Instance.Run(routine);

            while (!handle.IsTerminated)
            {
                yield return null;
            }
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
                {
                    anim = UnityScreenNavigatorSettings.Instance.GetDefaultPageTransitionAnimation(push, true);
                }

                anim.SetPartner(partnerPage?.transform as RectTransform);
                anim.Setup(_rectTransform);
                yield return CoroutineManager.Instance.Run(anim.CreatePlayRoutine());
            }

            _rectTransform.FillParent(_parentTransform);
        }

        internal void AfterEnter(bool push, Page partnerPage)
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

        internal AsyncProcessHandle BeforeExit(bool push, Page partnerPage)
        {
            return CoroutineManager.Instance.Run(BeforeExitRoutine(push, partnerPage));
        }

        private IEnumerator BeforeExitRoutine(bool push, Page partnerPage)
        {
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                _canvasGroup.interactable = false;
            }

            _canvasGroup.alpha = 1.0f;

            var routine = push ? WillPushExit() : WillPopExit();
            var handle = CoroutineManager.Instance.Run(routine);

            while (!handle.IsTerminated)
            {
                yield return null;
            }
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
                {
                    anim = UnityScreenNavigatorSettings.Instance.GetDefaultPageTransitionAnimation(push, false);
                }

                anim.SetPartner(partnerPage?.transform as RectTransform);
                anim.Setup(_rectTransform);
                yield return CoroutineManager.Instance.Run(anim.CreatePlayRoutine());
            }

            _canvasGroup.alpha = 0.0f;
        }

        internal void AfterExit(bool push, Page partnerPage)
        {
            if (push)
            {
                DidPushExit();
            }
            else
            {
                DidPopExit();
            }

            gameObject.SetActive(false);
        }

        internal AsyncProcessHandle BeforeRelease()
        {
            return CoroutineManager.Instance.Run(Cleanup());
        }
    }
}