using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Assertions;
using System.Text;
using UnityEngine;

namespace Astar
{
    /// <summary>
    /// Unity .NetStandard 2.1 does not include PriorityQueue
    /// As such it implements a simple PriorityQueue which is such a wrapper class utilizing SortedDictionary internally.
    /// It is not meant to be high performant queue
    /// Instead, it simply provides a few essential functions of PriorityQueue so that Astar algorithm can use it for OpenSet data structure. 
    /// </summary>
    public class AstarPriorityQueueFIFO : IAstarPriorityQueue
    {
        int IAstarPriorityQueue.Count() => sorted.Count;

        /// <summary>
        /// Astar path finding frequently insert and remove node to/from the queue
        /// It can take more benenfits from SortedDictionary rather than SortedList as it offers faster insertion and removal
        /// Item retrieval is the same: both SortedList and SortedDictionary offers O(log n) for retrieval
        /// </summary>
        private readonly SortedDictionary<int, LinkedList<AstarNode>> sorted;

        public bool IsEmpty => sorted.Count > 0 ? false : true;

        private readonly IAstarPriorityQueue self;
        public AstarPriorityQueueFIFO()
        {
            self = this;
            sorted = new SortedDictionary<int, LinkedList<AstarNode>>();
        }

        /// <summary>
        /// TryGetValue offers O(1) operation
        /// </summary>
        /// <param name="node"></param>
        bool IAstarPriorityQueue.TryEnqueue(ref Astar.AstarNode node)
        {
            try
            {
                if (sorted.TryGetValue(node.ScoreF, out LinkedList<AstarNode> list))
                {
                    list.AddFirst(node);
                }
                else
                {
                    list = new LinkedList<AstarNode>();
                    list.AddFirst(node);
                    sorted.Add(node.ScoreF, list);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        bool IAstarPriorityQueue.TryDequeue(out Astar.AstarNode node)
        {
            var key = -1;
            var enumerator = sorted.GetEnumerator();
            if (enumerator.MoveNext())
            {
                key = enumerator.Current.Key;
                var list = enumerator.Current.Value;
                var listNode = list.Last;

                Assert.IsNotNull(listNode);
                node = listNode.Value;

                list.RemoveLast();
                if (list.Count == 0)
                    sorted.Remove(key);
                    return true;
            }

            node = null;
            return false;
        }

        bool IAstarPriorityQueue.TryRemove(int key, ref AstarNode node)
        {
            var removed = false;
            try
            {
                if (sorted.TryGetValue(key, out LinkedList<AstarNode> list))
                {
                    var found = list.Find(node);
                    if (found != null)
                    {
                        list.Remove(found);
                        removed = true;

                        if (sorted[key].Count == 0)
                            sorted.Remove(key);
                    }
                }
            }
            catch
            {
                removed = false;
            }

            return removed;
        }

        bool IAstarPriorityQueue.Contains(ref Astar.AstarNode node)
        {
            if (sorted.TryGetValue(node.ScoreF, out var list))
            {
                if (list.Contains(node))
                    return true;
            }

            return false;
        }

        public override string ToString()
        {
            var log = new StringBuilder();

            var enumerator = sorted.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                log.AppendLine($"{pair.Key}: {string.Join(" ", pair.Value)}");
            }

            return log.ToString();
        }

        void IAstarPriorityQueue.Clear()
        {
            sorted?.Clear();
        }

        IEnumerable<AstarNode> IAstarPriorityQueue.GetEnumerable()
        {
            var enumerator = sorted.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                var queueEnumerator = pair.Value.GetEnumerator();
                while (queueEnumerator.MoveNext())
                    yield return queueEnumerator.Current;
            }
        }
    }
}
