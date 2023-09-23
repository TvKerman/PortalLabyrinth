using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InfoNode : MonoBehaviour
{
    public Labyrinth.Node node;

    [SerializeField] private Labyrinth.NodeType type;
    [SerializeField] private int id;

    [SerializeField] private int exitCount;
    [SerializeField] private int dimension;

    private void Start()
    {
        id = node.Id;
        exitCount = node.ExitCount;
        dimension = node.Dimension;
        type = node.Type;
    }

}
