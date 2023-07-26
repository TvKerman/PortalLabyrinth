using System;
using System.Collections;
using System.Collections.Generic;
using Labyrinth;
using UnityEngine;

public class Graph
{
    private Dictionary<Int64, Node> vertices;

    public Graph()
    {
        vertices = new Dictionary<Int64, Node>();
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
    }

    public void ConnectNodes(Int64 firstNodeId, Int64 secondNodeId)
    {
        Node firstNode = GetNodeById(firstNodeId);
        Node secondNode = GetNodeById(secondNodeId);
        
        firstNode.Childs.Add(secondNodeId);
        secondNode.Childs.Add(firstNodeId);
    }
}