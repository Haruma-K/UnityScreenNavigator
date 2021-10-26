using UnityEngine;

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    /// <summary>
    ///     Base class for transition animation with ScriptableObject.
    /// </summary>
    public abstract class TransitionAnimationObject : ScriptableObject, ITransitionAnimation
    {
        public RectTransform RectTransform { get; private set; }
        public RectTransform PartnerRectTransform { get; private set; }
        public abstract float Duration { get; }

        void ITransitionAnimation.SetPartner(RectTransform partnerRectTransform)
        {
            PartnerRectTransform = partnerRectTransform;
        }

        void ITransitionAnimation.Setup(RectTransform rectTransform)
        {
            RectTransform = rectTransform;
            Setup();
            SetTime(0.0f);
        }

        public abstract void SetTime(float time);

        public abstract void Setup();
    }
}