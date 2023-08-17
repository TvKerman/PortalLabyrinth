using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Labyrinth;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;


public class CreaterGraph : MonoBehaviour
{
    private Graph graph;
    private float radius = 10f;
    private Random rand;

    [SerializeField] private Int32 countCrossNode;
    [SerializeField] private Int32 maxCountExits;
    [SerializeField] private Int32 countCorridorsNode;

    [SerializeField] private GameObject prefabNode;
    [SerializeField] private GameObject prefabLine;

    List<GameObject> nodes = new List<GameObject>();
    List<GameObject> lines = new List<GameObject>();


    public Int32 CountCrosses 
    {
        get { return countCrossNode; }
    }

    public Int32 CountCorridors
    {
        get { return countCorridorsNode; }
    }

    //private void Awake()
    //{
    //    CreateGraph();
    //}

    public Graph CreateGraph()
    {
        graph = GenerateGraphWithTheAdditionOfDimensions(countCrossNode, maxCountExits, countCorridorsNode);

        Int64 count = graph.CountNode;
        for (Int64 i = 0; i < count; i++)
        {
            Node tmp = graph.GetNodeById(i);
            nodes.Add(Instantiate(prefabNode, GetNodePosition(i, countCrossNode, countCorridorsNode, tmp.Type, tmp.Dimension), new Quaternion()));
            nodes[(Int32)i].GetComponent<InfoNode>().node = graph.GetNodeById(i);
        }

        return graph;
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
            (type == NodeType.Cross ? countCrossNode : countCorridorNode) *
            (type == NodeType.Cross ? idNode : idNode - countCrossNode)));
        float cos = (float)(Math.Cos((Math.PI * 2) /
            (type == NodeType.Cross ? countCrossNode : countCorridorNode) *
            (type == NodeType.Cross ? idNode : idNode - countCrossNode)));
        if (type == NodeType.Cross)
        {
            return new Vector3(radius * cos, 0, radius * sin);
        }
        return new Vector3(dimension * dimension * radius * cos, dimension * 5, dimension * dimension * radius * sin);
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
        Int64 countAllExits = countConnectsList.Sum(x => Convert.ToInt64(x));

        AddCrossesToTheGraph(totalCountCrosses, listOfCrossesId, resGraph);

        List<List<Int16>> tmpMatrixOfCrosses = CreateNonBoolMatrixOfCrosses(totalCountCrosses);

        rand = new Random();

        SetARandomMatrixGraph(totalCountCrosses, rand,
                                countConnectsList, ref countAllExits,
                                                    tmpMatrixOfCrosses);

        AddCorridorsToTheGraphByMatrix(tmpMatrixOfCrosses, totalCountCrosses, 1, listOfCrossesId, resGraph);

        return resGraph;
    }

    public Graph GenerateGraph(Int32 totalCountCrosses, Int32 maxExitsOneCross, Int32 countCorridors)
    {
        List<Int64> listOfCrossesId = new List<Int64>();
        Graph resGraph = new Graph();

        AddCrossesToTheGraph(totalCountCrosses, listOfCrossesId, resGraph);

        List<List<bool>> tmpMatrixOfCrosses = CreateMatrixOfCrosses(totalCountCrosses);

        rand = new Random();
        Int32 currentCountAllExits = 0;
        List<Int16> currentCountExits = new List<Int16>();

        SetARandomConnectedMatrixGraph(totalCountCrosses, rand,
                                        currentCountExits, ref currentCountAllExits,
                                                              tmpMatrixOfCrosses);


        AddAdditionalRandomCorridorsToTheGraph(totalCountCrosses, maxExitsOneCross,
                                               countCorridors, ref currentCountAllExits,
                                               currentCountExits, rand, tmpMatrixOfCrosses);


        SetCountExitsAtCrosses(totalCountCrosses, listOfCrossesId, currentCountExits, resGraph);

        AddCorridorsToTheGraphByMatrix(tmpMatrixOfCrosses, totalCountCrosses, listOfCrossesId, resGraph);

        return resGraph;
    }

    public Graph GenerateGraphWithTheAdditionOfDimensions(Int32 totalCountCrosses, Int32 maxExitsOneCross, Int32 countCorridors)
    {
        List<Int64> listOfCrossesId = new List<Int64>();
        Graph resGraph = new Graph();

        AddCrossesToTheGraph(totalCountCrosses, listOfCrossesId, resGraph);

        List<List<bool>> boolMatrixGraph = CreateMatrixOfCrosses(totalCountCrosses);

        rand = new Random();
        Int32 currentCountAllExits = 0;
        List<Int16> currentCountExits = new List<Int16>();

        SetARandomConnectedMatrixGraph(totalCountCrosses, rand,
                                        currentCountExits, ref currentCountAllExits,
                                                              boolMatrixGraph);


        AddAdditionalRandomCorridorsToTheGraph(totalCountCrosses, maxExitsOneCross,
                                               countCorridors, ref currentCountAllExits,
                                               currentCountExits, rand, boolMatrixGraph);

        SetCountExitsAtCrosses(totalCountCrosses, listOfCrossesId, currentCountExits, resGraph);

        AddCorridorsToTheGraphByMatrix(boolMatrixGraph, totalCountCrosses, listOfCrossesId, resGraph);
        WriteDataToFile(boolMatrixGraph, 1);

        for (Int16 i = 2; i < 5; i++)
        {
            List<List<Int16>> intMatrixGraph = CreateNonBoolMatrixOfCrosses(totalCountCrosses);
            Int64 countAllExits = currentCountExits.Sum(x => Convert.ToInt64(x));

            SetARandomMatrixGraph(totalCountCrosses, rand,
                                  new List<Int16>(currentCountExits), ref countAllExits,
                                                    intMatrixGraph);
            WriteDataToFile(intMatrixGraph, i);
            AddCorridorsToTheGraphByMatrix(intMatrixGraph, totalCountCrosses, i, listOfCrossesId, resGraph);

        }

        return resGraph;
    }

    private void AddCrossesToTheGraph(Int32 countCrosses, List<Int64> listOfCrossesId, Graph graph)
    {
        for (Int32 i = 0; i < countCrosses; i++)
        {
            Node node = new Node(i, 1, NodeType.Cross);
            graph.AddNode(node);
            listOfCrossesId.Add(node.Id);
        }
    }

    List<List<bool>> CreateMatrixOfCrosses(Int32 countCrosses)
    {
        List<List<bool>> matrix = new List<List<bool>>();

        for (Int32 i = 0; i < countCrosses; i++)
        {
            matrix.Add(new List<bool>());
            for (Int32 j = 0; j < countCrosses; j++)
            {
                matrix[i].Add(false);
            }
        }

        return matrix;
    }

    List<List<Int16>> CreateNonBoolMatrixOfCrosses(Int32 countCrosses)
    {
        List<List<Int16>> matrix = new List<List<Int16>>();

        for (Int32 i = 0; i < countCrosses; i++)
        {
            matrix.Add(new List<Int16>());
            for (Int32 j = 0; j < countCrosses; j++)
            {
                matrix[i].Add(0);
            }
        }

        return matrix;
    }

    private void SetARandomConnectedMatrixGraph(Int32 countCrosses, Random random,
                                                List<Int16> currentCountExits,
                                                ref Int32 currentCountAllExits,
                                                List<List<bool>> matrixGraph)
    {
        bool[] visited = new bool[countCrosses];
        int startVertex = random.Next(0, countCrosses);

        visited[startVertex] = true;
        for (Int32 i = 0; i < countCrosses; i++)
        {
            currentCountExits.Add(0);
        }

        while (!AllVerticesVisited(visited))
        {
            int i = GetRandomVisitedVertex(visited, random);
            int j = GetRandomUnvisitedVertex(visited, random);

            matrixGraph[i][j] = true;
            currentCountAllExits++;
            currentCountExits[i]++;
            currentCountExits[j]++;

            visited[j] = true;
        }
    }

    private void AddAdditionalRandomCorridorsToTheGraph(Int32 countCrosses, Int32 maxExitsOneCross, Int32 countCorridors,
                                                        ref Int32 currentCountAllExits, List<Int16> currentCountExits,
                                                        Random random, List<List<bool>> matrixGraph)
    {
        while (currentCountAllExits != countCorridors)
        {
            int firstVertex = random.Next(0, countCrosses);
            int secondVertex = random.Next(0, countCrosses);

            while (matrixGraph[firstVertex][secondVertex] ||
                matrixGraph[firstVertex][secondVertex] ||
                firstVertex != secondVertex && (currentCountExits[firstVertex] + 1 == maxExitsOneCross ||
                currentCountExits[secondVertex] + 1 == maxExitsOneCross) ||
                firstVertex == secondVertex && currentCountExits[firstVertex] + 2 >= maxExitsOneCross)
            {
                secondVertex = random.Next(0, countCrosses);
                firstVertex = random.Next(0, countCrosses);
            }

            matrixGraph[firstVertex][secondVertex] = true;
            currentCountAllExits++;
            currentCountExits[firstVertex]++;
            currentCountExits[secondVertex]++;
        }
    }

    private void SetCountExitsAtCrosses(Int32 countCrosses, List<Int64> listOfCrossesId,
                                        List<Int16> currentCountExits, Graph graph)
    {
        for (Int32 i = 0; i < countCrosses; i++)
        {
            Node node = graph.GetNodeById(listOfCrossesId[i]);
            node.ExitCount = currentCountExits[i];
        }
    }

    private void AddCorridorsToTheGraphByMatrix(List<List<bool>> matrix, Int32 countCrosses,
                                                List<Int64> listOfCrossesId, Graph graph)
    {
        int countOfCorridors = 0;
        for (int i = 0; i < countCrosses; i++)
        {
            for (int j = 0; j < countCrosses; j++)
            {
                if (matrix[i][j])
                {
                    Node corridor = new Node(countCrosses + countOfCorridors, 1, NodeType.Corridor);
                    countOfCorridors++;
                    graph.AddNode(corridor);

                    graph.ConnectNodes(listOfCrossesId[i], corridor.Id);
                    graph.ConnectNodes(corridor.Id, listOfCrossesId[j]);
                }
            }
        }
    }

    private void AddCorridorsToTheGraphByMatrix(List<List<Int16>> matrix, Int32 countCrosses, Int16 dimension,
                                            List<Int64> listOfCrossesId, Graph graph)
    {
        Int64 idCorridor = graph.CountNode;
        for (int i = 0; i < countCrosses; i++)
        {
            for (int j = 0; j < countCrosses; j++)
            {
                while (matrix[i][j] != 0)
                {
                    Node corridor = new Node(idCorridor, dimension, NodeType.Corridor);
                    idCorridor++;
                    graph.AddNode(corridor);

                    graph.ConnectNodes(listOfCrossesId[i], corridor.Id);
                    graph.ConnectNodes(corridor.Id, listOfCrossesId[j]);
                    matrix[i][j] -= 1;
                }
            }
        }
    }

    private void SetARandomMatrixGraph(Int32 countCrosses, Random random,
                                                List<Int16> countConnectsList,
                                                ref Int64 countAllExits,
                                                List<List<Int16>> matrixGraph)
    {
        while (countAllExits != 0)
        {
            int firstVertex = random.Next(0, countCrosses);
            int secondVertex = random.Next(0, countCrosses);

            while (countConnectsList[firstVertex] == 0 ||
                    countConnectsList[secondVertex] == 0 ||
                    (secondVertex == firstVertex && countConnectsList[firstVertex] < 2))
            {
                secondVertex = random.Next(0, countCrosses);
                firstVertex = random.Next(0, countCrosses);
            }

            matrixGraph[firstVertex][secondVertex] += 1;

            countConnectsList[firstVertex]--;
            countConnectsList[secondVertex]--;
            countAllExits -= 2;
        }
    }

    private string ConvertMatrixToString(List<List<bool>> matrix)
    {
        string result = "";
        foreach (var row in matrix)
        {
            foreach (var value in row)
            {
                result += Convert.ToString(Convert.ToInt16(value)) + " ";
            }
            result += "\n";
        }

        return result;
    }

    private string ConvertMatrixToString(List<List<Int16>> matrix)
    {
        string result = "";
        foreach (var row in matrix)
        {
            foreach (var value in row)
            {
                result += Convert.ToString(value) + " ";
            }
            result += "\n";
        }

        return result;
    }

    private void WriteDataToFile(List<List<bool>> matrix, Int16 dimension)
    {
        FileStream? fstream = null;
        try
        {
            string fileName = "dimension" + Convert.ToString(dimension) + ".txt";
            fstream = new FileStream(fileName, FileMode.Create);
            byte[] buffer = Encoding.Default.GetBytes(ConvertMatrixToString(matrix));
            fstream.Write(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        { }
        finally
        {
            fstream?.Close();
        }
    }

    private void WriteDataToFile(List<List<Int16>> matrix, Int16 dimension)
    {
        FileStream? fstream = null;
        try
        {
            string fileName = "dimension" + Convert.ToString(dimension) + ".txt";
            fstream = new FileStream(fileName, FileMode.Create);
            byte[] buffer = Encoding.Default.GetBytes(ConvertMatrixToString(matrix));
            fstream.Write(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        { }
        finally
        {
            fstream?.Close();
        }
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