using UnityEngine;

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    /// <summary>
    ///     Base class for transition animation with MonoBehaviour.
    /// </summary>
    public abstract class TransitionAnimationBehaviour : MonoBehaviour, ITransitionAnimation
    {
        public abstract float Duration { get; }

        public void Setup(RectTransform rectTransform)
        {
            OnSetup();
        }

        public abstract void SetTime(float time);

        protected abstract void OnSetup();
    }
}