using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Astar
{
    public enum ScoreType
    {
        G,
        H,
        F,
    }

    public enum NodeType
    {
        Wall = 0,
        Walkable = 1,
    }

    //little endian
    public enum NodeStatusLiteral
    {
        Block = 0x000000,   //black
        None = 0xffffff,    //white
        Start = 0xffc187,   //blue
        End = 0x3334ca,     //red

        Open = 0x00ffff,    //yellow
        Close = 0x00ff00,   //green
        Path = 0xffff00     //cyan
    }

    //little endian
    [StructLayout(layoutKind: LayoutKind.Explicit, Size = 32, Pack = 1)]
    public class NodeStatus : IEquatable<NodeStatus>
    {
        bool IEquatable<NodeStatus>.Equals(Astar.NodeStatus other)
        {
            return other.Value.Equals(this.Value);
        }

        [FieldOffset(0)]
        public NodeStatusLiteral Value;

        [FieldOffset(0)]
        public byte R;

        [FieldOffset(1)]
        public byte G;

        [FieldOffset(2)]
        public byte B;

        public Color GetColor() => new Color32(R, G, B, 255);

        public NodeStatus(NodeStatusLiteral status) 
        {
            Update(status);
        }

        public NodeStatus(Color32 color) 
        {
            R = color.r;
            G = color.g;
            B = color.b;
        }

        public void Update(NodeStatusLiteral status)
        {
            Value = status;
        }

        public override string ToString()
        {
            return $"{Value}: ({R}, {G}, {B}) {GetColor()}";
        }
    }
}

