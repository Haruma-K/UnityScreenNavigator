using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace UnityScreenNavigator.Runtime.Foundation.PriorityCollection
{
    /// <summary>
    ///     The queue that will sort the items in order of priority.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T> : IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
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
        public void Enqueue(T item, int priority)
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
        ///     Get an object from the front of the queue.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T Peek()
        {
            if (_nodes.Count == 0)
            {
                throw new InvalidOperationException($"{nameof(Peek)} can not be called when the queue is empty.");
            }

            var node = _nodes.First();
            return node.Item;
        }
        
        /// <summary>
        ///     Get and remove an object from the front of the queue.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T Dequeue()
        {
            if (_nodes.Count == 0)
            {
                throw new InvalidOperationException($"{nameof(Dequeue)} can not be called when the queue is empty.");
            }

            var node = _nodes.First();
            _nodes.Remove(node);
            return node.Item;
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