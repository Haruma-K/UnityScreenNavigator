using UnityEngine;
using UnityScreenNavigator.Runtime.Foundation.Animation;

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    public interface ITransitionAnimation : IAnimation
    {
        void Setup(RectTransform rectTransform);
    }
}