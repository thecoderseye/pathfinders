using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Astar
{
    public class AstarNode : IEquatable<AstarNode>
    {
        bool IEquatable<AstarNode>.Equals(Astar.AstarNode other)
        {
            return this.Position.x == other.Position.x &&
                   this.Position.y == other.Position.y;//.Equals(other.Position);
        }

        public static readonly int UnitDistance = 10;
        public static readonly int UnitDistanceIntercardinal = 14;

        public AstarNode Parent { get; private set; }
        public Vector2Int Position;

        public NodeType Type;
        public NodeStatus Status;

        public int ScoreG { get; private set; }
        public int ScoreH { get; private set; }
        public int ScoreF => ScoreG + ScoreH;

        private event Action onValueChanged;
        public event Action OnValueChanged
        {
            add { onValueChanged += value; }
            remove { onValueChanged -= value; }
        }

        public AstarNode(ref AstarNode node)
        {
            SetNodeType(node.Type);
            SetNodeStatus(new NodeStatus(node.Status.Value));
            Position = node.Position;
        }

        public AstarNode(NodeType type, NodeStatus status, Vector2Int position)
        {
            SetNodeType(type);
            SetNodeStatus(status);

            this.Position = position;
        }

        public void Reset()
        {
        }

        public void SetNodeType(NodeType type)
        {
            Type = type;
            switch (type)
            {
                case NodeType.Walkable:
                    break;

                case NodeType.Wall:
                    break;
            }
        }

        public void Reset(NodeStatusLiteral status)
        {
            Status.Value = status;
            Notify();
        }

        public void Close()
        {
            Status.Value = NodeStatusLiteral.Close;
            Notify();
        }

        public void Open()
        {
            Status.Value = NodeStatusLiteral.Open;
            Notify();
        }

        public void MakePath()
        {
            Status.Value = NodeStatusLiteral.Path;
            Notify();
        }

        public void SetNodeStatus(NodeStatus status)
        {
            if (status.Equals(Status))
                return;

            Status = status;
            Notify();
        }

        public void SetParent(AstarNode parent)
        {
            Parent = parent;
            if (Parent != null)
            {
                var distance = Math.Abs(Parent.Position.x - Position.x) + Math.Abs(Parent.Position.y - Position.y);//manhattan distance
                if (distance == 1)
                    ScoreG = Parent.ScoreG + UnitDistance;
                else if (distance == 2) //intercardinal direction
                    ScoreG = Parent.ScoreG + UnitDistanceIntercardinal;
                Notify();
            }
        }

        public void UpdateHeuristic(int h)
        {
            ScoreH = h;
            Notify();
        }

        public void UpdateScores(int g, int h)
        {
            ScoreG = g;
            ScoreH = h;

            Notify();
        }

        private void Notify()
        {
            var callback = onValueChanged;
            callback?.Invoke();
        }

        public override string ToString()
        {
            return $"[{Position.x}, {Position.y}]";
        }
    }
}
