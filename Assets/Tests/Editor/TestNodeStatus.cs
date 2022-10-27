using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Astar;
using System;
using System.Security.Policy;

public class TestNodeStatus
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestNodeStatusValue()
    {
        // Use the Assert class to test conditions
        var status = NodeStatusLiteral.Open;

        byte r = (byte)(((int)status & 0xff0000) >> 16);
        byte g = (byte)(((int)status & 0x00ff00) >> 8);
        byte b = (byte)((int)status & 0x0000ff);

        Debug.Log($"{status}, {(int)status}, ({r}, {g}, {b}), {new Color32(r, g, b, 255)}");
    }

    [TestCase(NodeStatusLiteral.None)]
    [TestCase(NodeStatusLiteral.Open)]
    [TestCase(NodeStatusLiteral.Close)]
    [TestCase(NodeStatusLiteral.Path)]
    public void TestNodeStatusStruct(NodeStatusLiteral statusLiteral)
    {
        var status = new NodeStatus(statusLiteral);
        Assert.IsNotNull(status);

        Debug.Log(status);
    }
}
