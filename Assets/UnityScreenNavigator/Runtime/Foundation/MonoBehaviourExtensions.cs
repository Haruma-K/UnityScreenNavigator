using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal static class MonoBehaviourExtensions
    {
        public static IEnumerator StartCoroutines(this MonoBehaviour self, IReadOnlyList<IEnumerator> routines)
        {
            yield return self.StartCoroutinesInternal(routines);
        }

        public static void StartCoroutinesWithCallback(this MonoBehaviour self,
            IReadOnlyList<IEnumerator> routines, Action onComplete)
        {
            var _ = self.StartCoroutinesInternal(routines, onComplete);
        }

        public static void StartCoroutineWithCallback(this MonoBehaviour self, IEnumerator source,
            Action onComplete)
        {
            self.StartCoroutine(RoutineWithCallback(source, onComplete));
        }

        private static IEnumerator RoutineWithCallback(IEnumerator source, Action onComplete)
        {
            yield return source;
            onComplete?.Invoke();
        }

        private static IEnumerator StartCoroutinesInternal(this MonoBehaviour self, IReadOnlyList<IEnumerator> routines,
            Action onComplete = null)
        {
            var completeCount = 0;
            foreach (var routine in routines)
            {
                self.StartCoroutine(RoutineWithCallback(routine, () => completeCount++));
            }

            while (completeCount < routines.Count)
            {
                yield return null;
            }

            onComplete?.Invoke();
        }
    }
}