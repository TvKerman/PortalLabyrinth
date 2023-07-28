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
    private float radius = 5f;
    private Random rand;

    [SerializeField] private Int32 countCrossNode;
    [SerializeField] private Int32 maxCountExits;
    [SerializeField] private Int16 countCorridorsNode;

    [SerializeField] private bool useGraphGeneratioòByList = false;

    [SerializeField] private List<Int16> countExits;

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
        if (useGraphGeneratioòByList)
        {
            if (countCrossNode != countExits.Count)
            {
                throw new Exception();
            }

            graph = GenerateGraph(countCrossNode, countExits);
        }
        else 
        {
            graph = GenerateGraph(countCrossNode, maxCountExits, countCorridorsNode);
        }


        Int64 count = graph.CountNode;
        for (Int64 i = 0; i < count; i++)
        {
            Node tmp = graph.GetNodeById(i);
            nodes.Add(Instantiate(prefabNode, GetNodePosition(i, countCrossNode, count - countCrossNode, tmp.Type, tmp.Dimension), new Quaternion()));
            nodes[(Int32)i].GetComponent<InfoNode>().node = graph.GetNodeById(i);
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
        for (short i = 0; i < countCrossNode; i++)
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

    private Vector3 GetNodePosition(Int64 idNode, Int64 countCrossNode, Int64 countCorridorNode,
                                                NodeType type, Int16 dimension)
    {
        float sin = (float)(Math.Sin((Math.PI * 2) / 
            (type == NodeType.Cross ? countCrossNode: countCorridorNode) *
            (type == NodeType.Cross ? idNode : idNode - countCrossNode)));
        float cos = (float)(Math.Cos((Math.PI * 2) /
            (type == NodeType.Cross ? countCrossNode: countCorridorNode) * 
            (type == NodeType.Cross ? idNode: idNode - countCrossNode)));
        if (type == NodeType.Cross)
        {
            return new Vector3(radius * cos, 0, radius * sin);
        }
        return new Vector3(radius * cos, dimension * 5, radius * sin);
    }

    public Graph GenerateGraph(Int32 totalCountCrosses)
    {
        List<Int64> listOfCrossesId = new List<Int64>();
        Graph resGraph = new Graph();

        for (Int32 i = 0; i < totalCountCrosses; i++)
        {
            Node node = new Node(i, 1, NodeType.Cross);
            resGraph.AddNode(node);
            listOfCrossesId.Add(node.Id);
        }
        
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
        int startVertex = rand.Next(0, totalCountCrosses);
        bool[] visited = new bool[totalCountCrosses];

        visited[startVertex] = true;

        while (!AllVerticesVisited(visited))
        {
            int i = GetRandomVisitedVertex(visited, rand);
            int j = GetRandomUnvisitedVertex(visited, rand);

            tmpMatrixOfCrosses[i][j] = true;
            //tmpMatrixOfCrosses[j][i] = true;

            visited[j] = true;
        }

        int countOfCorridors = 0; 
        for (int i = 0; i < totalCountCrosses; i++)
        {
            for (int j = 0; j < totalCountCrosses; j++)
            {
                if (tmpMatrixOfCrosses[i][j])
                {
                    Node corridor = new Node(totalCountCrosses + countOfCorridors, 1, NodeType.Corridor);
                    countOfCorridors++;
                    resGraph.AddNode(corridor);
                    
                    resGraph.ConnectNodes(listOfCrossesId[i], corridor.Id);
                    resGraph.ConnectNodes(corridor.Id, listOfCrossesId[j]);
                }
            }
        }

        return resGraph;
    }

    public Graph GenerateGraph(Int32 totalCountCrosses, List<Int16> countConnectsList)
    {
        List<Int64> listOfCrossesId = new List<Int64>();
        Graph resGraph = new Graph();
        Int64 countAllExits = 0;
        for (Int32 i = 0; i < totalCountCrosses; i++)
        {
            Node node = new Node(i, 1, NodeType.Cross, countConnectsList[i]);
            countAllExits += countConnectsList[i]; 
            resGraph.AddNode(node);
            listOfCrossesId.Add(node.Id);
        }

        List<List<Int16>> tmpMatrixOfCrosses = new List<List<Int16>>();

        for (Int32 i = 0; i < totalCountCrosses; i++)
        {
            tmpMatrixOfCrosses.Add(new List<Int16>());
            for (Int32 j = 0; j < totalCountCrosses; j++)
            {
                tmpMatrixOfCrosses[i].Add(0);
            }
        }

        rand = new Random();
        //int startVertex = rand.Next(0, totalCountCrosses);
        //bool[] visited = new bool[totalCountCrosses];
        //
        //visited[startVertex] = true;
        //
        //while (!AllVerticesVisited(visited))
        //{
        //    int i = GetRandomVisitedVertex(visited, rand);
        //    int j = GetRandomUnvisitedVertex(visited, rand);
        //
        //    tmpMatrixOfCrosses[i][j] = true;
        //    tmpMatrixOfCrosses[j][i] = true;
        //
        //    visited[j] = true;
        //}
        
        while (countAllExits != 0) 
        {
            int firstVertex = rand.Next(0, totalCountCrosses);
            int secondVertex = rand.Next(0, totalCountCrosses);
            
            while (countConnectsList[firstVertex] == 0 ||
                    countConnectsList[secondVertex] == 0 ||
                    (secondVertex == firstVertex && countConnectsList[firstVertex] < 2)) 
            {
                secondVertex = rand.Next(0, totalCountCrosses);
                firstVertex = rand.Next(0, totalCountCrosses);
            }

            tmpMatrixOfCrosses[firstVertex][secondVertex] += 1;
            
            countConnectsList[firstVertex]--;
            countConnectsList[secondVertex]--;
            countAllExits -= 2;
        }

        int countOfCorridors = 0;
        for (int i = 0; i < totalCountCrosses; i++)
        {
            for (int j = 0; j < totalCountCrosses; j++)
            {
                while (tmpMatrixOfCrosses[i][j] != 0)
                {
                    Node corridor = new Node(totalCountCrosses + countOfCorridors, 1, NodeType.Corridor);
                    countOfCorridors++;
                    resGraph.AddNode(corridor);

                    resGraph.ConnectNodes(listOfCrossesId[i], corridor.Id);
                    resGraph.ConnectNodes(corridor.Id, listOfCrossesId[j]);
                    tmpMatrixOfCrosses[i][j] -= 1;
                }
            }
        }

        return resGraph;
    }

    public Graph GenerateGraph(Int32 totalCountCrosses, Int32 maxExitsOneCross, Int32 countCorridors)
    {
        List<Int64> listOfCrossesId = new List<Int64>();
        Graph resGraph = new Graph();

        for (Int32 i = 0; i < totalCountCrosses; i++)
        {
            Node node = new Node(i, 1, NodeType.Cross);
            resGraph.AddNode(node);
            listOfCrossesId.Add(node.Id);
        }

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
        int startVertex = rand.Next(0, totalCountCrosses);
        bool[] visited = new bool[totalCountCrosses];

        visited[startVertex] = true;
        Int32 currentCountAllExits = 0;
        List<Int16> currentCountExits = new List<Int16>();
        for (Int32 i = 0; i < totalCountCrosses; i++)
        {
            currentCountExits.Add(0);
        }
        while (!AllVerticesVisited(visited))
        {
            int i = GetRandomVisitedVertex(visited, rand);
            int j = GetRandomUnvisitedVertex(visited, rand);

            tmpMatrixOfCrosses[i][j] = true;
            currentCountAllExits++;
            currentCountExits[i]++;
            currentCountExits[j]++;
            //tmpMatrixOfCrosses[j][i] = true;

            visited[j] = true;
        }

        

        while (currentCountAllExits != countCorridors) 
        {
            int firstVertex = rand.Next(0, totalCountCrosses);
            int secondVertex = rand.Next(0, totalCountCrosses);
        
            while (tmpMatrixOfCrosses[firstVertex][secondVertex] ||
                tmpMatrixOfCrosses[firstVertex][secondVertex] ||
                firstVertex != secondVertex && (currentCountExits[firstVertex] + 1 >= maxExitsOneCross ||
                currentCountExits[secondVertex] + 1 == maxExitsOneCross) ||
                firstVertex == secondVertex && currentCountExits[firstVertex] + 2 >= maxExitsOneCross)
            {
                secondVertex = rand.Next(0, totalCountCrosses);
                firstVertex = rand.Next(0, totalCountCrosses);
            }
        
            tmpMatrixOfCrosses[firstVertex][secondVertex] = true;
            currentCountAllExits++;
            currentCountExits[firstVertex]++;
            currentCountExits[secondVertex]++;
        }

        for (Int32 i = 0; i < totalCountCrosses; i++)
        {
            Node node = resGraph.GetNodeById(listOfCrossesId[i]);
            node.ExitCount = currentCountExits[i];
        }

        int countOfCorridors = 0;
        for (int i = 0; i < totalCountCrosses; i++)
        {
            for (int j = 0; j < totalCountCrosses; j++)
            {
                if (tmpMatrixOfCrosses[i][j])
                {
                    Node corridor = new Node(totalCountCrosses + countOfCorridors, 1, NodeType.Corridor);
                    countOfCorridors++;
                    resGraph.AddNode(corridor);

                    resGraph.ConnectNodes(listOfCrossesId[i], corridor.Id);
                    resGraph.ConnectNodes(corridor.Id, listOfCrossesId[j]);
                }
            }
        }

        return resGraph;
    }


    private bool AllVerticesVisited(bool[] visited)
    {
        foreach (bool v in visited)
        {
            if (!v)
            {
                return false;
            }
        }
        return true;
    }
    
    static int GetRandomVisitedVertex(bool[] visited, Random random)
    {
        int n = visited.Length;
        int vertex = random.Next(0, n);
        while (!visited[vertex])
        {
            vertex = random.Next(0, n);
        }
        return vertex;
    }

    static int GetRandomUnvisitedVertex(bool[] visited, Random random)
    {
        int n = visited.Length;
        int vertex = random.Next(0, n);
        while (visited[vertex])
        {
            vertex = random.Next(0, n);
        }
        return vertex;
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