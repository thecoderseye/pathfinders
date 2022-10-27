using System.Collections;
using System.Collections.Generic;
using System;

using System.Drawing;
using System.IO.Compression;
using UnityEngine;

namespace Astar
{
    public class AstarMap
    {
        private AstarNode[,] matrix;

        public AstarNode this[int row, int col] => matrix[row, col];

        public AstarNode[,] Matrix => matrix;
        public AstarNode StartNode;
        public AstarNode EndNode;

        public readonly int Size;
        private readonly Vector2Int defaultStartPosition;
        private readonly Vector2Int defaultEndPosition;

        public AstarMap(int size = 10)
        {
            this.Size = size;

            defaultStartPosition = new Vector2Int(1, 1);
            defaultEndPosition = new Vector2Int(size - 1, size - 1);
            matrix = new AstarNode[size, size];
        }

        public IEnumerator Build()
        {
            for (var row = 0; row < Size; ++row)
            {
                for (var col = 0; col < Size; ++col)
                {
                    var node = new AstarNode(NodeType.Walkable, new NodeStatus(NodeStatusLiteral.None), new Vector2Int(row, col));
                    matrix[row, col] = node;
                }

                yield return null;
            }

            SetStartPosition(1, 1);
            SetEndPosition(Size - 2, Size - 2);
        }

        public void SetStartPosition(int row, int col)
        {
            StartNode = matrix[row, col];

            StartNode.UpdateScores(0, 0);
            StartNode.Reset(NodeStatusLiteral.Start);//.SetNodeStatus(new NodeStatus(NodeStatusLiteral.Start));
            RecalculateEndNodeStatus();
        }

        public void SetEndPosition(int row, int col)
        {
            EndNode = matrix[row, col];

            //EndNode.UpdateScores(0, 0);
            EndNode.Reset(NodeStatusLiteral.End);//.SetNodeStatus(new NodeStatus(NodeStatusLiteral.End));
            RecalculateEndNodeStatus();
        }

        public void RecalculateEndNodeStatus()
        {
            if (StartNode == null || EndNode == null)
                return;

            var h = 0;
            var g = AstarAlgorithm.GetManhattan(StartNode.Position, EndNode.Position);
            EndNode.UpdateScores(g, h);
        }

        public void Reset()
        {
        }

        public void Clear()
        {
            SetStartPosition(StartNode.Position.x, StartNode.Position.y);
            SetEndPosition(EndNode.Position.x, EndNode.Position.y);

            for (var row = 0; row < Size; ++row)
            {
                for (var col = 0; col < Size; ++col)
                {
                    var node = matrix[row, col];
                    switch (node.Status.Value)
                    {
                        case NodeStatusLiteral.Block:
                            continue;

                        case NodeStatusLiteral.Close:
                        case NodeStatusLiteral.Open:
                        case NodeStatusLiteral.Path:
                            node.UpdateScores(0, 0);
                            node.SetParent(null);
                            node.SetNodeStatus(new NodeStatus(NodeStatusLiteral.None));//.Reset(NodeStatusLiteral.None);
                            break;

                        default:
                            //todo: nothing
                            break;
                    }
                }
            }
        }
    }
}