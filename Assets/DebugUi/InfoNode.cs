using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InfoNode : MonoBehaviour
{
    public Labyrinth.Node node;

    [SerializeField] private Labyrinth.NodeType type;
    [SerializeField] private Int64 id;

    [SerializeField] private Int16 exitCount;
    [SerializeField] private Int16 dimension;

    private void Start()
    {
        id = node.Id;
        exitCount = node.ExitCount;
        dimension = node.Dimension;
        type = node.Type;
    }

}
