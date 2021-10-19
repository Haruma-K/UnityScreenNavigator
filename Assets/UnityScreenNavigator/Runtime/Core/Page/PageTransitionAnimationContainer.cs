using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Foundation;
using Object = UnityEngine.Object;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    [Serializable]
    public sealed class PageTransitionAnimationContainer
    {
        [SerializeField] private List<TransitionAnimation> _pushEnterAnimations = new List<TransitionAnimation>();
        [SerializeField] private List<TransitionAnimation> _pushExitAnimations = new List<TransitionAnimation>();
        [SerializeField] private List<TransitionAnimation> _popEnterAnimations = new List<TransitionAnimation>();
        [SerializeField] private List<TransitionAnimation> _popExitAnimations = new List<TransitionAnimation>();

        public List<TransitionAnimation> PushEnterAnimations => _pushEnterAnimations;
        public List<TransitionAnimation> PushExitAnimations => _pushExitAnimations;
        public List<TransitionAnimation> PopEnterAnimations => _popEnterAnimations;
        public List<TransitionAnimation> PopExitAnimations => _popExitAnimations;

        public ITransitionAnimation GetAnimation(bool push, bool enter, string partnerTransitionIdentifier)
        {
            var anims = GetAnimations(push, enter);
            var anim = anims.FirstOrDefault(x => x.IsValid(partnerTransitionIdentifier));
            var result = anim?.GetAnimation();
            return result;
        }

        private IReadOnlyList<TransitionAnimation> GetAnimations(bool push, bool enter)
        {
            if (push)
            {
                return enter ? _pushEnterAnimations : _pushExitAnimations;
            }

            return enter ? _popEnterAnimations : _popExitAnimations;
        }

        [Serializable]
        public sealed class TransitionAnimation
        {
            [SerializeField] private string _partnerPageIdentifierRegex;

            [SerializeField] private AnimationAssetType _assetType;

            [SerializeField] [EnabledIf(nameof(_assetType), (int)AnimationAssetType.MonoBehaviour)]
            private TransitionAnimationBehaviour _animationBehaviour;

            [SerializeField] [EnabledIf(nameof(_assetType), (int)AnimationAssetType.ScriptableObject)]
            private TransitionAnimationObject _animationObject;

            private Regex _partnerSheetIdentifierRegexCache;

            public string PartnerPageIdentifierRegex
            {
                get => _partnerPageIdentifierRegex;
                set => _partnerPageIdentifierRegex = value;
            }

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

            public bool IsValid(string partnerPageIdentifier)
            {
                if (GetAnimation() == null)
                {
                    return false;
                }

                // If the partner identifier is not registered, the animation is always valid.
                if (string.IsNullOrEmpty(_partnerPageIdentifierRegex))
                {
                    return true;
                }
                
                if (string.IsNullOrEmpty(partnerPageIdentifier))
                {
                    return false;
                }
                
                if (_partnerSheetIdentifierRegexCache == null)
                {
                    _partnerSheetIdentifierRegexCache = new Regex(_partnerPageIdentifierRegex);
                }

                return _partnerSheetIdentifierRegexCache.IsMatch(partnerPageIdentifier);
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