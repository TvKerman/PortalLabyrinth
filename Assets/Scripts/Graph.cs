using System;
using System.Collections;
using System.Collections.Generic;
using Labyrinth;
using UnityEngine;

public class Graph
{
    private Dictionary<Int64, Node> vertices;
    private Int64 countNode;

    public Graph()
    {
        vertices = new Dictionary<Int64, Node>();
        countNode = 0;
    }

    public Node GetNodeById(Int64 id)
    {
        Node tmp;
        vertices.TryGetValue(id,out tmp);
        if (tmp is not null)
        {
            return tmp;
        }
        else
        {
            return null;
        }
    }

    public void AddNode(Node nodeToAdd)
    {
        if (nodeToAdd is null)
        {
            return;
        }
        
        vertices.Add(nodeToAdd.Id, nodeToAdd);
        countNode++;
    }

    public Int64 CountNode { get { return countNode; } }

    public void ConnectNodes(Int64 firstNodeId, Int64 secondNodeId)
    {
        Node firstNode = GetNodeById(firstNodeId);
        Node secondNode = GetNodeById(secondNodeId);
        
        firstNode.Childs.Add(secondNodeId);
        secondNode.Childs.Add(firstNodeId);
    }
}