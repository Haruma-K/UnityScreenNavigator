using System.Collections;
using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal sealed class CoroutineSystem : MonoBehaviour
    {
        private static CoroutineSystem _instance;
        private CoroutineRunner _runner;

        private void Update()
        {
            _runner?.Tick();
        }

        public static AsyncStatus Run(IEnumerator routine)
        {
            EnsureInitialized();
            return _instance._runner.Run(routine);
        }

        private static void EnsureInitialized()
        {
            if (_instance != null)
                return;

            var go = new GameObject("[Unity Screen Navigator] CoroutineSystem");
            DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;

            _instance = go.AddComponent<CoroutineSystem>();
            _instance._runner = new CoroutineRunner();
        }
    }
}