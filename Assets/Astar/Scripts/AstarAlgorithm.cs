using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;
using System.Globalization;

namespace Astar
{
    using Util;
    public interface IAstarAlgorithmState
    {
        AstarAlgorithm.Status GetStatus();
    }

    /// <summary>
    /// This class implements Astar path finding logic,
    ///     while other classes are more likely supporting features
    ///     such as user interfaces, input handlers, visualizations, etc. 
    /// </summary>
    public class AstarAlgorithm : IAstarAlgorithmState
    {
        #region Lazy Singleton
        private static readonly Lazy<AstarAlgorithm> instance;
        public static AstarAlgorithm GetInstance() => instance.Value;
        #endregion

        public enum Directions
        {
            CardinalOnly, //only
            Intercardinal, //inclusive
        }

        public enum HeuristicMethod
        {
            Manhattan,
            Euclidean,
            Hybrid,
        }

        /// <summary>
        /// after exit, Flags helps to find out the result status: Found or NotFound
        /// </summary>
        [Flags]
        public enum Status
        {
            NotStarted = 0,
            Finding = 1 << 0,
            NotFound = 1 << 1,
            Found = 1 << 2,
            Exit = 1 << 3,
            ExitWithFound = Found | Exit,
            ExitWithNotFound = NotFound | Exit,
        }

        #region static properties
        private static readonly Dictionary<HeuristicMethod, Func<Vector2Int, Vector2Int, int>> heuristicMethods;
        #endregion region

        Status IAstarAlgorithmState.GetStatus() => status;
        private Status status;

        private Func<Vector2Int, Vector2Int, int> activeHeuristicMethod;
        private HeuristicMethod activeHeuristic;
        private Directions direction;

        public IEnumerable<AstarNode> GetOpenSet() => openSet.GetEnumerable();
        public IEnumerable<AstarNode> GetCloseSet()
        {
            var enumerator = closeSet.GetEnumerator();
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        private AstarPriorityQueue openSet;
        private List<AstarNode> closeSet;

        private AstarMap map;
        private AstarNode startNode;
        private AstarNode endNode;

        public int CountOpenSet() => openSet.Count;
        public int CountCloseSet() => closeSet.Count;
        public int CountPath() => pathCount;
        private int pathCount;

        static AstarAlgorithm()
        {
            instance = new Lazy<AstarAlgorithm>(() => new AstarAlgorithm());
            heuristicMethods = new Dictionary<HeuristicMethod, Func<Vector2Int, Vector2Int, int>>()
            {
                { HeuristicMethod.Manhattan, GetManhattan },
                { HeuristicMethod.Euclidean, GetEuclidean },
                { HeuristicMethod.Hybrid, GetHybrid },
            };
        }

        private AstarAlgorithm()
        {
            status = Status.NotStarted;
            closeSet = new List<AstarNode>();
        }

        public void SetDirection(Directions direction)
        {
            this.direction = direction;
        }

        public void SetHeuristic(HeuristicMethod heuristicMethod)
        {
            activeHeuristic = heuristicMethod;
            activeHeuristicMethod = heuristicMethods[heuristicMethod];
        }

        public void SetDataStructure(ref IAstarPriorityQueue queue)
        {
            openSet = new AstarPriorityQueue(ref queue);
        }

        public void SetTerminals(AstarNode startNode, AstarNode endNode)
        {
            this.startNode = startNode;
            this.endNode = endNode;
        }

        /// <summary>
        /// Must be called before the start.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="heuristicMethod"></param>
        public void Setup(AstarMap map, IAstarPriorityQueue queue, HeuristicMethod heuristicMethod = HeuristicMethod.Manhattan, Directions direction = Directions.CardinalOnly) //todo: by default use Manhattan distance 
        {
            this.map = map;
            SetTerminals(map.StartNode, map.EndNode);
            SetHeuristic(heuristicMethod);
            SetDataStructure(ref queue);
            SetDirection(direction);
        }

        #region A* logic
        /// <summary>
        /// Astar pathfinding logic
        /// A series of calls of step forward
        /// </summary>
        /// <returns></returns>
        public IEnumerator StartFindingPath()
        {
            do
            {
                StepForward();
                yield return new WaitForEndOfFrame();

                if ((status & Status.Exit) > 0)
                    break;

            } while (true);
        }

        /// <summary>
        /// Process a single node dequeued from OpenSet
        /// </summary>
        /// <param name="node"></param>
        public Status StepForward()
        {
            switch (status)
            {
                case Status.NotStarted:
                    openSet.Enqueue(startNode);
                    status = Status.Finding;
                    break;

                case Status.Finding:
                    try
                    {
                        var node = openSet.Dequeue();
                        GetNeighbours(ref node);

                        node.Close();
                        closeSet.Add(node);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Debug.LogError(ex.Message);

                        status = Status.NotFound;
                        DoPostprocess();
                    }
                    break;

                case Status.Found:
                    closeSet.Add(endNode);
                    DoPostprocess();
                    break;
            }

            return status;
        }

        private void DoPostprocess()
        {
            switch (status)
            {
                case Status.Found:
                    MakePath();
                    break;

                case Status.NotFound:
                    Debug.Log("Path does not exists.");
                    break;
            }

            status = status | Status.Exit;
        }

        private void MakePath()
        {
            Assert.AreEqual(Status.Found, this.status);

            var path = closeSet.Last();
            pathCount = 0;
            do
            {
                closeSet.Remove(path);
                path.MakePath();
                path = path.Parent;
                ++pathCount;
                if (path == null)
                    break;

            } while (true);
        }

        /// <summary>
        /// Get adjacent nodes from Top by CCW order
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private void GetNeighbours(ref AstarNode parentNode)
        {
            var position = parentNode.Position;
            if (position.y > 0) //todo: topNode; N
            {
                GetNeighbour(ref parentNode, position.x, position.y - 1);

                if (direction == Directions.Intercardinal && position.x > 0) //todo: top-left corner; NW
                    GetNeighbour(ref parentNode, position.x - 1, position.y - 1);

            }

            if (position.x > 0) //todo: leftNode; W
            {
                GetNeighbour(ref parentNode, position.x - 1, position.y);

                if (direction == Directions.Intercardinal && position.y < map.Size - 1) //todo: left-bottom corner; SW
                    GetNeighbour(ref parentNode, position.x - 1, position.y + 1);
            }

            if (position.y < map.Size - 1) //todo: bottomNode; S
            {
                GetNeighbour(ref parentNode, position.x, position.y + 1);

                if (direction == Directions.Intercardinal && position.x < map.Size - 1) //todo: bottom-right cornder; SE
                    GetNeighbour(ref parentNode, position.x + 1, position.y + 1);
            }

            if (position.x < map.Size - 1) //todo: rightNode; E
            {
                GetNeighbour(ref parentNode, position.x + 1, position.y);

                if (direction == Directions.Intercardinal && position.y > 0) //todo: top-right corner; NE
                    GetNeighbour(ref parentNode, position.x + 1, position.y - 1);
            }
        }

        private int tempCount = 0;
        private bool GetNeighbour(ref AstarNode parentNode, int row, int col)
        {
            var neighbour = map[row, col];
            if (neighbour.Status.Value.Equals(NodeStatusLiteral.End))
            {
                endNode.SetParent(parentNode);
                status = Status.Found;
                return true;
            }

            if (neighbour.Type == NodeType.Wall || neighbour.Status.Value.Equals(NodeStatusLiteral.Block))
            {
                return false;
            }

            if (!closeSet.Contains(neighbour)) //todo: pass if the node is already in closeSet
            {
                if (openSet.Contains(neighbour)) //todo: recalculate score F and replace parent only if it has lower G
                {
                    var scoreG = parentNode.ScoreG + AstarNode.UnitDistance;
                    if (neighbour.ScoreG > scoreG)
                    {
                        var previousF = neighbour.ScoreF;
                        neighbour.SetParent(parentNode);
                        openSet.Relocate(previousF, neighbour);
                    }
                }
                else
                {
                    var heuristicValue = GetHeuristic(ref neighbour);
                    neighbour.UpdateHeuristic(heuristicValue);
                    neighbour.SetParent(parentNode);

                    openSet.Enqueue(neighbour); //todo: add node to openSet
                    neighbour.Open();
                }
            }

            return false;
        }

        private int GetHeuristic(ref AstarNode node)
        {
            return activeHeuristicMethod(node.Position, endNode.Position);
        }
        #endregion

        #region static methods
        public static int GetEuclidean(Vector2Int nodePosition, Vector2Int endPosition)
        {
            return (int)Math.Floor(Vector2Int.Distance(nodePosition, endPosition) * AstarNode.UnitDistance);
        }

        public static int GetManhattan(Vector2Int nodePosition, Vector2Int endPosition)
        {
            return AstarNode.UnitDistance * (Math.Abs(nodePosition.x - endPosition.x) + Math.Abs(nodePosition.y - endPosition.y));
        }

        public static int GetHybrid(Vector2Int nodePosition, Vector2Int endPosition)
        {
            return GetEuclidean(nodePosition, endPosition) + GetManhattan(nodePosition, endPosition);
        }
        #endregion

        public void Clear()
        {
            status = Status.NotStarted;
            pathCount = 0;
            tempCount = 0;
            closeSet?.Clear();
            openSet?.Clear();
        }

        public void Reset()
        {
            Clear();
        }
    }
}