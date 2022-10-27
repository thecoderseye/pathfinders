using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Assertions;
using System.Text;
using System.Diagnostics;

namespace Astar
{
    public interface IAstarPriorityQueue
    {
        int Count();
        void Clear();

        bool TryEnqueue(ref AstarNode node);
        bool TryDequeue(out AstarNode node);
        bool TryRemove(int key, ref AstarNode node);
        bool Contains(ref AstarNode node);

        IEnumerable<AstarNode> GetEnumerable();
    }


    /// <summary>
    /// Unity .NetStandard 2.1 does not include PriorityQueue
    /// As such it implements a simple PriorityQueue which is such a wrapper class utilizing SortedDictionary internally.
    /// It is not meant to be high performant queue
    /// Instead, it simply provides a few essential functions of PriorityQueue so that Astar algorithm can use it for OpenSet data structure. 
    /// </summary>
    public class AstarPriorityQueue
    {
        public int Count { get; private set; }
        public bool IsEmpty => queue.Count() > 0 ? false : true;

        private readonly IAstarPriorityQueue queue;

        public AstarPriorityQueue(ref IAstarPriorityQueue queue)
        {
            Count = 0;
            this.queue = queue;
        }

        /// <summary>
        /// TryGetValue offers O(1) operation
        /// </summary>
        /// <param name="node"></param>
        public void Enqueue(AstarNode node)
        {
            if (queue.TryEnqueue(ref node))
            {
                ++Count;
            }
        }

        public AstarNode Dequeue()
        {
            if (queue.TryDequeue(out AstarNode node))
            {
                --Count;
                return node;
            }
            else
            {
                Assert.AreEqual(0, Count, $"it is supposed to be {queue.GetEnumerable().Count()}");
                throw new InvalidOperationException("priority queue is empty");
            }
        }

        public bool Contains(AstarNode node)
        {
            return queue.Contains(ref node);
        }

        public void Relocate(int key, AstarNode node)
        {
            queue.TryRemove(key, ref node);
            queue.TryEnqueue(ref node);
        }

        public override string ToString()
        {
            return queue.ToString();
        }

        public void Clear()
        {
            queue?.Clear();
            Count = 0;
        }

        public IEnumerable<AstarNode> GetEnumerable()
        {
            return queue.GetEnumerable();
        }
    }
}
