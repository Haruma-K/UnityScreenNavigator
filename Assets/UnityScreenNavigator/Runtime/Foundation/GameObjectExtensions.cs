using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            if (!self.TryGetComponent<T>(out var component))
            {
                component = self.AddComponent<T>();
            }

            return component;
        }
    }
}