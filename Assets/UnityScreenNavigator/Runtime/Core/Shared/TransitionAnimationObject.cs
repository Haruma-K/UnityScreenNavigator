using UnityEngine;

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    /// <summary>
    ///     Base class for transition animation with ScriptableObject.
    /// </summary>
    public abstract class TransitionAnimationObject : ScriptableObject, ITransitionAnimation
    {
        public abstract float Duration { get; }

        public abstract void Setup(RectTransform rectTransform);

        public abstract void SetTime(float time);
    }
}