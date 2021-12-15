using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace UnityScreenNavigator.Runtime.Foundation.PriorityCollection
{
    /// <summary>
    ///     The list that will sort the items in order of priority.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityList<T> : IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
    {
        private readonly LinkedList<Node> _nodes = new LinkedList<Node>();

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var node in _nodes)
            {
                yield return node.Item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _nodes.Count;

        /// <summary>
        ///     Add objects based on priority.
        ///     If the priority is the same, the object added earlier will be placed in front.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        public void Add(T item, int priority)
        {
            var node = _nodes.First;
            LinkedListNode<Node> beforeNode = null;
            if (node == null)
            {
                _nodes.AddFirst(new Node(item, priority));
                return;
            }
            while (true)
            {
                if (node.Value.Priority > priority)
                {
                    if (beforeNode == null)
                    {
                        _nodes.AddFirst(new Node(item, priority));
                    }
                    else
                    {
                        _nodes.AddAfter(beforeNode, new Node(item, priority));
                    }
                    return;
                }

                beforeNode = node;
                node = node.Next;
                if (node == null)
                {
                    _nodes.AddLast(new Node(item, priority));
                    return;
                }
            }
        }
        
        /// <summary>
        ///     Remove the first appeared one of the specified values.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Remove(T item)
        {
            var linkedListNode = _nodes.First;
            if (linkedListNode == null)
            {
                return false;
            }
            var equalityComparer = EqualityComparer<T>.Default;
            while (true)
            {
                if (equalityComparer.Equals(linkedListNode.Value.Item, item))
                {
                    _nodes.Remove(linkedListNode);
                    return true;
                }

                linkedListNode = linkedListNode.Next;
                if (linkedListNode == null)
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Clear all objects.
        /// </summary>
        public void Clear()
        {
            _nodes.Clear();
        }
        
        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException($"{nameof(array)} has invalid rank.");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException($"{nameof(array)} does not have enough space.");
            }

            if (array is T[] array1)
            {
                CopyTo(array1, index);
                return;
            }
            
            var elementType = array.GetType().GetElementType();
            var c = typeof(T);
            if (!elementType.IsAssignableFrom(c) && !c.IsAssignableFrom(elementType))
            {
                throw new ArgumentException($"Type of the {nameof(array)} element is invalid.");
            }

            if (!(array is object[] objArray))
            {
                throw new ArgumentException($"Type of the {nameof(array)} is invalid.");
            }

            var linkedListNode = _nodes.First;
            try
            {
                if (linkedListNode == null)
                {
                    return;
                }

                while (true)
                {
                    objArray[index++] = linkedListNode.Value.Item;
                    linkedListNode = linkedListNode.Next;
                    if (linkedListNode == null)
                    {
                        return;
                    }
                }
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException($"Type of the {nameof(array)} is invalid.");
            }
        }

        private void CopyTo(T[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException($"{nameof(array)} does not have enough space.");
            }

            var linkedListNode = _nodes.First;
            if (linkedListNode == null)
            {
                return;
            }

            while (true)
            {
                array[index++] = linkedListNode.Value.Item;
                linkedListNode = linkedListNode.Next;
                if (linkedListNode == null)
                {
                    return;
                }
            }
        }

        bool ICollection.IsSynchronized => false;

        private object _syncRoot;
        
        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        private readonly struct Node
        {
            public Node(T item, int priority)
            {
                Item = item;
                Priority = priority;
            }

            public T Item { get; }
            public int Priority { get; }
        }
    }
}