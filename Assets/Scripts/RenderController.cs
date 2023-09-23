using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RenderController : MonoBehaviour
{
    private int currentId;
    [SerializeField] private PlayerInfo info;
    [SerializeField] private CreaterGraph creater;
    [SerializeField] private CreaterLabyrinth labyrinth;
    private bool flag;
    private int ctr;

    void Start()
    {
        currentId = -1;
    }

    void Update()
    {
        if (currentId != info.GetCurrentId())
        {
            currentId = info.GetCurrentId();
            Debug.Log(currentId);

            GameObject rootNode = labyrinth.GetNode(currentId);
            rootNode.SetActive(true);
            List<GameObject> childNodes = new List<GameObject>();
            List<int> tmp = rootNode.GetComponent<Labyrinth.Unit>().Info.Childs;
            foreach (var childId in tmp)
            {
                GameObject tmpChild = labyrinth.GetNode(childId);
                tmpChild.SetActive(true);
                childNodes.Add(tmpChild);
            }

            List<int> allId = creater.IdNodes;
            foreach (var currentId in allId)
            {
                GameObject zalupa = labyrinth.GetNode(currentId);
                Labyrinth.Unit tmpZalupa = zalupa.GetComponent<Labyrinth.Unit>();
                if (rootNode.GetComponent<Labyrinth.Unit>().Info.Id == tmpZalupa.Info.Id)
                {
                    continue;
                }

                bool isRenderChildId = false;
                foreach (var renderNodes in childNodes)
                {
                    if (renderNodes.GetComponent<Labyrinth.Unit>().Info.Id == tmpZalupa.Info.Id)
                    {
                        isRenderChildId = true;
                        break;
                    }
                }

                if (!isRenderChildId)
                {
                    zalupa.SetActive(false);
                }
            }
        }
    }
}
