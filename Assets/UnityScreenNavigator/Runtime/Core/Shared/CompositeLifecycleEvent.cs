using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    public sealed class CompositeLifecycleEvent<TLifecycleEvent>
    {
        private readonly Dictionary<int, List<TLifecycleEvent>> _priorityToLifecycleEvent = new();

        /// <summary>
        ///     Returns the next priority value after the given priority. If the given priority is the highest, it returns null.
        ///     If the given priority is null, it returns the lowest priority.
        ///     Throws an exception if the given priority does not exist.
        /// </summary>
        /// <param name="priority">The current priority value. If null, the lowest priority is returned.</param>
        /// <returns>The next priority value after the given priority, or null if the given priority is the highest.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the given priority does not exist.</exception>
        public int? FindNextPriority(int? priority)
        {
            var orderedPriorities = _priorityToLifecycleEvent.Keys.OrderBy(x => x).ToList();
            if (!priority.HasValue)
                return orderedPriorities[0];
            var index = orderedPriorities.IndexOf(priority.Value);
            if (index == -1)
                throw new InvalidOperationException($"The priority {priority} does not exist.");
            if (index == orderedPriorities.Count - 1)
                return null;
            return orderedPriorities[index + 1];
        }

        public IEnumerable<TLifecycleEvent> GetItems(int priority)
        {
            if (!_priorityToLifecycleEvent.ContainsKey(priority))
                return Enumerable.Empty<TLifecycleEvent>();
            return _priorityToLifecycleEvent[priority];
        }

        public void AddItem(TLifecycleEvent item, int priority)
        {
            if (!_priorityToLifecycleEvent.ContainsKey(priority))
                _priorityToLifecycleEvent[priority] = new List<TLifecycleEvent>();
            _priorityToLifecycleEvent[priority].Add(item);
        }

        public void RemoveItem(TLifecycleEvent item)
        {
            foreach (var pair in _priorityToLifecycleEvent)
                if (pair.Value.Remove(item) && pair.Value.Count == 0)
                {
                    _priorityToLifecycleEvent.Remove(pair.Key);
                    break;
                }
        }

        public async Task ExecuteLifecycleEventsSequentially(Func<TLifecycleEvent, Task> execute)
        {
            int? currentPriority = null;
            while ((currentPriority = FindNextPriority(currentPriority)) != null)
            {
                // LifecycleEvents with the same Priority are executed in parallel.
                var lifecycleEvents = GetItems(currentPriority.Value);
                var tasks = lifecycleEvents.Select(execute).ToArray();
                await Task.WhenAll(tasks);
            }
        }

        public IEnumerator ExecuteLifecycleEventsSequentially(Func<TLifecycleEvent, IEnumerator> execute)
        {
            int? currentPriority = null;
            while ((currentPriority = FindNextPriority(currentPriority)) != null)
            {
                // LifecycleEvents with the same Priority are executed in parallel.
                var lifecycleEvents = GetItems(currentPriority.Value);
                var handles = lifecycleEvents.Select(x => CoroutineManager.Instance.Run(execute(x))).ToArray();
                foreach (var handle in handles)
                    while (!handle.IsTerminated)
                        yield return null;
            }
        }

        public void ExecuteLifecycleEventsSequentially(Action<TLifecycleEvent> execute)
        {
            foreach (var lifecycleEvent in _priorityToLifecycleEvent.Values)
            foreach (var item in lifecycleEvent)
                execute(item);
        }
    }
}