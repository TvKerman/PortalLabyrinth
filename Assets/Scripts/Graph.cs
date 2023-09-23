using System;
using System.Collections;
using System.Collections.Generic;
using Labyrinth;
using UnityEngine;

public class Graph
{
    private Dictionary<int, Node> vertices;
    private List<int> idList; 
    private int countNode;

    public Graph()
    {
        vertices = new Dictionary<int, Node>();
        idList = new List<int>();
        countNode = 0;
    }

    public List<int> IdList
    {
        get
        {
            return idList;
        }
    }

    public Node GetNodeById(int id)
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
        idList.Add(nodeToAdd.Id);
        countNode++;
    }

    public int CountNode { get { return countNode; } }

    public void ConnectNodes(int firstNodeId, int secondNodeId)
    {
        Node firstNode = GetNodeById(firstNodeId);
        Node secondNode = GetNodeById(secondNodeId);
        
        firstNode.Childs.Add(secondNodeId);
        secondNode.Childs.Add(firstNodeId);
    }
}