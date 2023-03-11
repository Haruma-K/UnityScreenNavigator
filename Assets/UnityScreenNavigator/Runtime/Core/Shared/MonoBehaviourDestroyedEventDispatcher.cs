using System;
using UnityEngine;

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    internal sealed class MonoBehaviourDestroyedEventDispatcher : MonoBehaviour
    {
        private void OnDestroy()
        {
            OnDispatch?.Invoke();
        }

        public event Action OnDispatch;
    }
}
