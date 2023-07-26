using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class CreaterGraph : MonoBehaviour
{
    private Graph graph;
    private Int32 countNode = 12;
    private float radius = 5f;

    [SerializeField] private GameObject prefabNode;
    [SerializeField] private GameObject prefabLine;

    List<GameObject> nodes = new List<GameObject>();
    List<GameObject> lines = new List<GameObject>();

    private void Awake()
    {
        graph = new Graph();

        for (int i = 0; i < countNode; i++)
        {
            graph.AddNode(new Labyrinth.Node(i, 2, i % 2 == 0 ? Labyrinth.NodeType.Room: 
                                                                Labyrinth.NodeType.Corridor));
        }

        for (int i = 0; i < countNode - 1; i++)
        {
            graph.ConnectNodes(i, i + 1);
        }
        graph.ConnectNodes(countNode - 1, 0);

        for (short i = 0; i < countNode; i++)
        {
            nodes.Add(Instantiate(prefabNode, GetNodePosition(i, countNode), new Quaternion()));
            nodes[i].GetComponent<InfoNode>().node = graph.GetNodeById(i);
        }
    }

    private void Start()
    {
        for (int i = 0; i < nodes.Count; i++) 
        {
            if (nodes[i].GetComponent<InfoNode>().node.Type == Labyrinth.NodeType.Corridor)
            {
                nodes[i].GetComponent<MeshRenderer>().materials[0].color = Color.blue;
            }
            else 
            {
                nodes[i].GetComponent<MeshRenderer>().materials[0].color = Color.red;
            }
        }
    }

    private void Update()
    {
        foreach (var line in lines)
        {
            Destroy(line);
        }
        lines.Clear();
        for (short i = 0; i < countNode; i++)
        {
            for (short j = 0; j < nodes[i].GetComponent<InfoNode>().node.Childs.Count; j++)
            {
                GameObject line = Instantiate(prefabLine, new Vector3(), new Quaternion());
                lines.Add(line);
                line.GetComponent<LineRenderer>().SetPosition(0, nodes[i].transform.position);
                line.GetComponent<LineRenderer>().SetPosition(1, nodes[(int)(nodes[i].GetComponent<InfoNode>().node.Childs[j])].transform.position);
            }
        }

    }

    private Vector3 GetNodePosition(Int32 idNode, Int32 counNode) 
    {
        float sin = (float)(Math.Sin((Math.PI * 2) / counNode * idNode));
        float cos = (float)(Math.Cos((Math.PI * 2) / counNode * idNode));
        return new Vector3(radius * cos, 0, radius * sin);
    }
}
