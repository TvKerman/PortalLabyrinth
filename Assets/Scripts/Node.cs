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
        private List<Int64> childs;

        private NodeType type;
        private Int64 id;
        
        private Int16 exitCount;
        private Int16 dimension;

        private Vector3 coordinates;

  

        public Node(Int64 id, Int16 dimension)
        {
            childs = new List<Int64>();

            exitCount = 0;
            
            this.id = id;
            this.dimension = dimension;
        }

        public Node(Int64 id, Int16 dimension, NodeType type) : this(id, dimension) 
        {
            this.type = type;
        }

        public Node(Int64 id, Int16 dimension, NodeType type, Int16 exits) : this(id, dimension, type) 
        {
            this.exitCount = exits;
        }

        public NodeType Type
        {
            get { return type; }
            set { type = value; }
        }

        public Int64 Id
        {
            get { return id; }
        }

        public List<Int64> Childs
        {
            get { return childs; }
        }

        public Int16 ExitCount
        {
            get { return exitCount; }
            set { exitCount = value; }
        }

        public Int16 Dimension
        {
            get { return dimension; }
        }
    }
}