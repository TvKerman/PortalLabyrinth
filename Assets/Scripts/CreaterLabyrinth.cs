using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaterLabyrinth : MonoBehaviour
{
    [SerializeField] private CreaterGraph creater;
    [SerializeField] private List<GameObject> corridorsPrefab;
    [SerializeField] private List<GameObject> crossesPrefab;
    [SerializeField] private MainCamera mainCamera;

    private List<GameObject> corridors;
    private List<GameObject> crosses;

    private Vector3 step = new Vector3(40f, 0f, 0f);

    private void Awake()
    {
        GenerateLabyrinth();
    }

    private void GenerateLabyrinth() 
    {
        Graph graph = creater.CreateGraph();
        Int32 countCrossNode = creater.CountCrosses;
        Int32 countCorridorsNode = creater.CountCorridors;
        
        crosses = new List<GameObject>();
        for (Int32 id = 0; id < countCrossNode; id++) 
        {
            GameObject cross = Instantiate(crossesPrefab[0], GetPosition(id, countCrossNode, countCorridorsNode), new Quaternion());
            cross.GetComponent<Labyrinth.Unit>().Info = graph.GetNodeById(id);
            crosses.Add(cross);
            for (int i = 0; i < 4; i++) 
            {
                mainCamera.AddPortal(cross.GetComponent<Labyrinth.Unit>().GetTeleporter(i).gameObject.GetComponent<Portal>());
            }
        }
        
        corridors = new List<GameObject>();
        for (Int32 id = countCrossNode; id < countCrossNode + countCorridorsNode * 4; id++)
        {
            GameObject corridor = Instantiate(corridorsPrefab[0], GetPosition(id, countCrossNode, countCorridorsNode), new Quaternion());
            corridor.GetComponent<Labyrinth.Unit>().Info = graph.GetNodeById(id);
            corridors.Add(corridor);
            for (int i = 0; i < 2; i++)
            {
                mainCamera.AddPortal(corridor.GetComponent<Labyrinth.Unit>().GetTeleporter(i).gameObject.GetComponent<Portal>());
            }
        }
        
        for (Int32 id = 0; id < countCrossNode; id++)
        {
            Labyrinth.Unit cross = crosses[id].GetComponent<Labyrinth.Unit>();
            for (Int32 indexChild = 0; indexChild < cross.Info.Childs.Count; indexChild++)
            { 
                Labyrinth.Unit corridor = corridors[(Int32)(cross.Info.Childs[indexChild] - (Int64)(countCrossNode))].GetComponent<Labyrinth.Unit>();
                for (Int32 portalIndex = 0; portalIndex < cross.GetCountPortals(); portalIndex++) 
                {
                    if (!cross.GetTeleporter(portalIndex).IsCreateListDimension()) 
                    {
                        cross.GetTeleporter(portalIndex).CreateListExitPointForDimension();
                    }

                    if (!cross.GetTeleporter(portalIndex).IsExitPointSetInList(corridor.Info.Dimension - 1)) 
                    {
                        if (!corridor.GetTeleporter(0).IsCurrentExitPointSet())
                        {
                            cross.GetTeleporter(portalIndex).AddExitPoint(corridor.Info.Dimension - 1, 
                                                                            corridor.GetTeleporter(0));
                            corridor.GetTeleporter(0).SetCurrentExitPoint(cross.GetTeleporter(portalIndex));
                            break;
                            //cross.GetPortal(portalIndex).SetLink(corridor.Info.Dimension - 1, corridor.GetPortal(0));
                            //corridor.GetPortal(0).SetLink(corridor.Info.Dimension - 1, cross.GetPortal(portalIndex));
                            //break;
                        }
                        else 
                        {
                            cross.GetTeleporter(portalIndex).AddExitPoint(corridor.Info.Dimension - 1,
                                                                            corridor.GetTeleporter(1));
                            corridor.GetTeleporter(1).SetCurrentExitPoint(cross.GetTeleporter(portalIndex));
                            break;
                            //cross.GetPortal(portalIndex).SetLink(corridor.Info.Dimension - 1, corridor.GetPortal(1));
                            //corridor.GetPortal(1).SetLink(corridor.Info.Dimension - 1, cross.GetPortal(portalIndex));
                            //break;
                        }
                    }
                }
            }
        }

        for (Int32 id = 0; id < countCrossNode; id++) 
        {
            Labyrinth.Unit cross = crosses[id].GetComponent<Labyrinth.Unit>();
            for (Int32 portalIndex = 0; portalIndex < cross.GetCountPortals(); portalIndex++) 
            {
                if (cross.GetTeleporter(portalIndex).IsCreateListDimension())
                {
                    cross.GetTeleporter(portalIndex).SetCurrentExitPoint(0);
                }
            }
        }
    }

    private Vector3 GetPosition(Int64 idNode, Int64 countCrossNode, Int64 countCorridorNode) 
    {
        return new Vector3(step.x * idNode, step.y * idNode, step.z * idNode);
    }
}
