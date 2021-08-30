using System;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Foundation;
using Object = UnityEngine.Object;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    [Serializable]
    public sealed class ModalBackdropTransitionAnimationContainer
    {
        [SerializeField] private TransitionAnimation _enterAnimation;
        [SerializeField] private TransitionAnimation _exitAnimation;

        public TransitionAnimation EnterAnimation => _enterAnimation;
        public TransitionAnimation ExitAnimation => _exitAnimation;

        public ITransitionAnimation GetAnimation(bool enter)
        {
            var transitionAnimation = enter ? _enterAnimation : _exitAnimation;
            return transitionAnimation.GetAnimation();
        }

        [Serializable]
        public sealed class TransitionAnimation
        {
            [SerializeField] private AnimationAssetType _assetType;

            [SerializeField] [EnabledIf(nameof(_assetType), (int)AnimationAssetType.MonoBehaviour)]
            private TransitionAnimationBehaviour _animationBehaviour;

            [SerializeField] [EnabledIf(nameof(_assetType), (int)AnimationAssetType.ScriptableObject)]
            private TransitionAnimationObject _animationObject;

            public AnimationAssetType AssetType
            {
                get => _assetType;
                set => _assetType = value;
            }

            public TransitionAnimationBehaviour AnimationBehaviour
            {
                get => _animationBehaviour;
                set => _animationBehaviour = value;
            }

            public TransitionAnimationObject AnimationObject
            {
                get => _animationObject;
                set => _animationObject = value;
            }

            public ITransitionAnimation GetAnimation()
            {
                switch (_assetType)
                {
                    case AnimationAssetType.MonoBehaviour:
                        return _animationBehaviour;
                    case AnimationAssetType.ScriptableObject:
                        return Object.Instantiate(_animationObject);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}