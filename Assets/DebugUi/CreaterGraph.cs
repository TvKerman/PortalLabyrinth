using System;
using System.Collections;
using System.Collections.Generic;
using Labyrinth;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;


public class CreaterGraph : MonoBehaviour
{
    private Graph graph;
    private Int32 countNode = 12;
    private float radius = 5f;
    private Random rand;
    
    
    [SerializeField] private GameObject prefabNode;
    [SerializeField] private GameObject prefabLine;
    
    

    List<GameObject> nodes = new List<GameObject>();
    List<GameObject> lines = new List<GameObject>();

    private void Awake()
    {
        // graph = new Graph();
        //
        // for (int i = 0; i < countNode; i++)
        // {
        //     graph.AddNode(new Labyrinth.Node(i, 2, i % 2 == 0 ? Labyrinth.NodeType.Cross : Labyrinth.NodeType.Corridor));
        // }
        //
        // for (int i = 0; i < countNode - 1; i++)
        // {
        //     graph.ConnectNodes(i, i + 1);
        // }
        //
        // graph.ConnectNodes(countNode - 1, 0);

        graph = GenerateGraph(countNode,5);
        
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
                line.GetComponent<LineRenderer>().SetPosition(1,
                    nodes[(int)(nodes[i].GetComponent<InfoNode>().node.Childs[j])].transform.position);
            }
        }
    }

    private Vector3 GetNodePosition(Int32 idNode, Int32 counNode)
    {
        float sin = (float)(Math.Sin((Math.PI * 2) / counNode * idNode));
        float cos = (float)(Math.Cos((Math.PI * 2) / counNode * idNode));
        return new Vector3(radius * cos, 0, radius * sin);
    }

    public Graph GenerateGraph(Int32 totalCountCrosses, int countOfConnects)
    {
        List<Int64> listOfCrossesId = new List<Int64>();
        Graph resGraph = new Graph();

        for (Int32 i = 0; i < totalCountCrosses; i++)
        {
            Labyrinth.Node node = new Node(i, 1, NodeType.Cross);
            resGraph.AddNode(node);
            listOfCrossesId.Add(node.Id);
        }
        
        Int32 exit;
        List<List<bool>> tmpMatrixOfCrosses = new List<List<bool>>();

        for (Int32 i = 0; i < totalCountCrosses; i++)
        {
            tmpMatrixOfCrosses.Add(new List<bool>());
            for (Int32 j = 0; j < totalCountCrosses; j++)
            {
                tmpMatrixOfCrosses[i].Add(false);
            }
        }

        rand = new Random();
        
        for (int i = 0; i < countOfConnects; i++)
        {
            rand.Next(0, countOfConnects);
            
        }
        // tmpMatrixOfCrosses = KraskalsAlgorithm(out exit, totalCountCrosses);

        int countOfCorridors = 0; 
        for (int i = 0; i < totalCountCrosses; i++)
        {
            for (int j = 0; j < totalCountCrosses; j++)
            {
                if (tmpMatrixOfCrosses[i][j])
                {
                    Node corridor = new Node(totalCountCrosses + countOfCorridors, 1, NodeType.Corridor);
                    resGraph.AddNode(corridor);
                    
                    resGraph.ConnectNodes(listOfCrossesId[i], corridor.Id);
                    resGraph.ConnectNodes(corridor.Id, listOfCrossesId[j]);
                }
            }
        }

        return resGraph;
    }


    private void addABouquetToATree(List<Int32> tree, Int32 numberTree, Int32 numberBouquet)
    {
        for (int i = 0; i < tree.Count; i++)
        {
            if (tree[i] == numberBouquet)
            {
                tree[i] = numberTree;
            }
        }
    }

    private List<List<bool>> KraskalsAlgorithm(out Int32 exit, Int32 size)
    {
        List<Int32> tmpList = new List<Int32>();
        for (Int16 x = 0; x < size; x++)
        {
            tmpList.Add(x + 1);
        }


        List<List<bool>> tmpMatrixOfCrosses = new List<List<bool>>();

        for (Int32 i = 0; i < size; i++)
        {
            tmpMatrixOfCrosses.Add(new List<bool>());
            for (Int32 j = 0; j < size; j++)
            {
                tmpMatrixOfCrosses[i].Add(false);
            }
        }

        exit = size;
        for (Int32 x = 0; x < size; x++)
        {
            for (Int32 y = 0; y < size; y++)
            {
                if (tmpMatrixOfCrosses[x][y] && tmpList[x] != tmpList[y])
                {
                    addABouquetToATree(tmpList, tmpList[x], tmpList[y]);
                    tmpMatrixOfCrosses[x][y] = true;
                    tmpMatrixOfCrosses[y][x] = true;
                    exit--;
                }
            }
        }

        return tmpMatrixOfCrosses;
    }
}