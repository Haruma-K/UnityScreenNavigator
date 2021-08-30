using System;
using UnityEngine;

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    public class MonoBehaviourDestroyedEventDispatcher : MonoBehaviour
    {
        public void OnDestroy()
        {
            OnDispatch?.Invoke();
        }

        public event Action OnDispatch;
    }
}