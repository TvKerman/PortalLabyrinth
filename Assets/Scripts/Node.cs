using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Labyrinth
{
    public enum NodeType
    {
        Cross,
        Corridor,
    }
    
    public class Node
    {
        private List<int> childs;

        private NodeType type;
        private int id;
        
        private int exitCount;
        private int dimension;

        private Vector3 coordinates;

        public Node(int id, int dimension)
        {
            childs = new List<int>();

            exitCount = 0;
            
            this.id = id;
            this.dimension = dimension;
        }

        public Node(int id, int dimension, NodeType type) : this(id, dimension) 
        {
            this.type = type;
        }

        public Node(int id, int dimension, NodeType type, int exits) : this(id, dimension, type) 
        {
            this.exitCount = exits;
        }

        public NodeType Type
        {
            get { return type; }
            set { type = value; }
        }

        public int Id
        {
            get { return id; }
        }

        public List<int> Childs
        {
            get { return childs; }
        }

        public int ExitCount
        {
            get { return exitCount; }
            set { exitCount = value; }
        }

        public int Dimension
        {
            get { return dimension; }
        }
    }
}