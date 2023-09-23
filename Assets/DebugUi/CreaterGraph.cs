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

    [SerializeField] private int countCrossNode;
    [SerializeField] private int maxCountExits;
    [SerializeField] private int countCorridorsNode;

    [SerializeField] private GameObject prefabNode;
    [SerializeField] private GameObject prefabLine;

    List<GameObject> nodes = new List<GameObject>();
    List<GameObject> lines = new List<GameObject>();


    public int CountCrosses 
    {
        get { return countCrossNode; }
    }

    public int CountCorridors
    {
        get { return countCorridorsNode; }
    }

    public List<int> IdNodes
    {
        get { return graph.IdList; }
    }

    //private void Awake()
    //{
    //    CreateGraph();
    //}

    public Graph CreateGraph()
    {
        graph = GenerateGraphWithTheAdditionOfDimensions(countCrossNode, maxCountExits, countCorridorsNode);

        //int count = graph.CountNode;
        //for (int i = 0; i < count; i++)
        //{
        //    Node tmp = graph.GetNodeById(i);
        //    nodes.Add(Instantiate(prefabNode, GetNodePosition(i, countCrossNode, countCorridorsNode, tmp.Type, tmp.Dimension), new Quaternion()));
        //    nodes[(int)i].transform.SetParent(this.transform);
        //    nodes[(int)i].GetComponent<InfoNode>().node = graph.GetNodeById(i);
        //}

        return graph;
    }

    private void Start()
    {
        //for (int i = 0; i < nodes.Count; i++)
        //{
        //    if (nodes[i].GetComponent<InfoNode>().node.Type == Labyrinth.NodeType.Corridor)
        //    {
        //        nodes[i].GetComponent<MeshRenderer>().materials[0].color = Color.blue;
        //    }
        //    else
        //    {
        //        nodes[i].GetComponent<MeshRenderer>().materials[0].color = Color.red;
        //    }
        //}
    }

    private void Update()
    {
        //foreach (var line in lines)
        //{
        //    Destroy(line);
        //}
        //
        //lines.Clear();
        //for (short i = 0; i < countCrossNode; i++)
        //{
        //    for (short j = 0; j < nodes[i].GetComponent<InfoNode>().node.Childs.Count; j++)
        //    {
        //        GameObject line = Instantiate(prefabLine, new Vector3(), new Quaternion());
        //
        //        lines.Add(line);
        //        line.GetComponent<LineRenderer>().SetPosition(0, nodes[i].transform.position);
        //        line.GetComponent<LineRenderer>().SetPosition(1,
        //        nodes[(int)(nodes[i].GetComponent<InfoNode>().node.Childs[j])].transform.position);
        //    }
        //}
    }

    private Vector3 GetNodePosition(int idNode, int countCrossNode, int countCorridorNode,
                                                NodeType type, int dimension)
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

    public Graph GenerateGraph(int totalCountCrosses)
    {
        List<int> listOfCrossesId = new List<int>();
        Graph resGraph = new Graph();

        for (int i = 0; i < totalCountCrosses; i++)
        {
            Node node = new Node(i, 1, NodeType.Cross);
            resGraph.AddNode(node);
            listOfCrossesId.Add(node.Id);
        }

        List<List<bool>> tmpMatrixOfCrosses = new List<List<bool>>();

        for (int i = 0; i < totalCountCrosses; i++)
        {
            tmpMatrixOfCrosses.Add(new List<bool>());
            for (int j = 0; j < totalCountCrosses; j++)
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

    public Graph GenerateGraph(int totalCountCrosses, List<int> countConnectsList)
    {
        List<int> listOfCrossesId = new List<int>();
        Graph resGraph = new Graph();
        int countAllExits = countConnectsList.Sum();

        AddCrossesToTheGraph(totalCountCrosses, listOfCrossesId, resGraph);

        List<List<int>> tmpMatrixOfCrosses = CreateNonBoolMatrixOfCrosses(totalCountCrosses);

        rand = new Random();

        SetARandomMatrixGraph(totalCountCrosses, rand,
                                countConnectsList, ref countAllExits,
                                                    tmpMatrixOfCrosses);

        AddCorridorsToTheGraphByMatrix(tmpMatrixOfCrosses, totalCountCrosses, 1, listOfCrossesId, resGraph);

        return resGraph;
    }

    public Graph GenerateGraph(int totalCountCrosses, int maxExitsOneCross, int countCorridors)
    {
        List<int> listOfCrossesId = new List<int>();
        Graph resGraph = new Graph();

        AddCrossesToTheGraph(totalCountCrosses, listOfCrossesId, resGraph);

        List<List<bool>> tmpMatrixOfCrosses = CreateMatrixOfCrosses(totalCountCrosses);

        rand = new Random();
        int currentCountAllExits = 0;
        List<int> currentCountExits = new List<int>();

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

    public Graph GenerateGraphWithTheAdditionOfDimensions(int totalCountCrosses, int maxExitsOneCross, int countCorridors)
    {
        List<int> listOfCrossesId = new List<int>();
        Graph resGraph = new Graph();

        AddCrossesToTheGraph(totalCountCrosses, listOfCrossesId, resGraph);

        List<List<bool>> boolMatrixGraph = CreateMatrixOfCrosses(totalCountCrosses);

        rand = new Random();
        int currentCountAllExits = 0;
        List<int> currentCountExits = new List<int>();

        SetARandomConnectedMatrixGraph(totalCountCrosses, rand,
                                        currentCountExits, ref currentCountAllExits,
                                                              boolMatrixGraph);


        AddAdditionalRandomCorridorsToTheGraph(totalCountCrosses, maxExitsOneCross,
                                               countCorridors, ref currentCountAllExits,
                                               currentCountExits, rand, boolMatrixGraph);

        SetCountExitsAtCrosses(totalCountCrosses, listOfCrossesId, currentCountExits, resGraph);

        AddCorridorsToTheGraphByMatrix(boolMatrixGraph, totalCountCrosses, listOfCrossesId, resGraph);
        WriteDataToFile(boolMatrixGraph, 1);

        for (int i = 2; i < 5; i++)
        {
            List<List<int>> intMatrixGraph = CreateNonBoolMatrixOfCrosses(totalCountCrosses);
            int countAllExits = currentCountExits.Sum();

            SetARandomMatrixGraph(totalCountCrosses, rand,
                                  new List<int>(currentCountExits), ref countAllExits,
                                                    intMatrixGraph);
            WriteDataToFile(intMatrixGraph, i);
            AddCorridorsToTheGraphByMatrix(intMatrixGraph, totalCountCrosses, i, listOfCrossesId, resGraph);

        }

        return resGraph;
    }

    private void AddCrossesToTheGraph(int countCrosses, List<int> listOfCrossesId, Graph graph)
    {
        for (int i = 0; i < countCrosses; i++)
        {
            Node node = new Node(i, 1, NodeType.Cross);
            graph.AddNode(node);
            listOfCrossesId.Add(node.Id);
        }
    }

    List<List<bool>> CreateMatrixOfCrosses(int countCrosses)
    {
        List<List<bool>> matrix = new List<List<bool>>();

        for (int i = 0; i < countCrosses; i++)
        {
            matrix.Add(new List<bool>());
            for (int j = 0; j < countCrosses; j++)
            {
                matrix[i].Add(false);
            }
        }

        return matrix;
    }

    List<List<int>> CreateNonBoolMatrixOfCrosses(int countCrosses)
    {
        List<List<int>> matrix = new List<List<int>>();

        for (int i = 0; i < countCrosses; i++)
        {
            matrix.Add(new List<int>());
            for (int j = 0; j < countCrosses; j++)
            {
                matrix[i].Add(0);
            }
        }

        return matrix;
    }

    private void SetARandomConnectedMatrixGraph(int countCrosses, Random random,
                                                List<int> currentCountExits,
                                                ref int currentCountAllExits,
                                                List<List<bool>> matrixGraph)
    {
        bool[] visited = new bool[countCrosses];
        int startVertex = random.Next(0, countCrosses);

        visited[startVertex] = true;
        for (int i = 0; i < countCrosses; i++)
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

    private void AddAdditionalRandomCorridorsToTheGraph(int countCrosses, int maxExitsOneCross, int countCorridors,
                                                        ref int currentCountAllExits, List<int> currentCountExits,
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

    private void SetCountExitsAtCrosses(int countCrosses, List<int> listOfCrossesId,
                                        List<int> currentCountExits, Graph graph)
    {
        for (int i = 0; i < countCrosses; i++)
        {
            Node node = graph.GetNodeById(listOfCrossesId[i]);
            node.ExitCount = currentCountExits[i];
        }
    }

    private void AddCorridorsToTheGraphByMatrix(List<List<bool>> matrix, int countCrosses,
                                                List<int> listOfCrossesId, Graph graph)
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

    private void AddCorridorsToTheGraphByMatrix(List<List<int>> matrix, int countCrosses, int dimension,
                                            List<int> listOfCrossesId, Graph graph)
    {
        int idCorridor = graph.CountNode;
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

    private void SetARandomMatrixGraph(int countCrosses, Random random,
                                                List<int> countConnectsList,
                                                ref int countAllExits,
                                                List<List<int>> matrixGraph)
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
                result += Convert.ToString(value) + " ";
            }
            result += "\n";
        }

        return result;
    }

    private string ConvertMatrixToString(List<List<int>> matrix)
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

    private void WriteDataToFile(List<List<bool>> matrix, int dimension)
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

    private void WriteDataToFile(List<List<int>> matrix, int dimension)
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

    private void addABouquetToATree(List<int> tree, int numberTree, int numberBouquet)
    {
        for (int i = 0; i < tree.Count; i++)
        {
            if (tree[i] == numberBouquet)
            {
                tree[i] = numberTree;
            }
        }
    }
}