using System;
using System.Collections;
using System.Collections.Generic;

namespace Labyrinth
{
    enum NodeType
    {
        Cross,
        Corridor,
    }
    public class Node
    {
        private NodeType type;
        private Int64 id;
        private List<Int64> neighbors;
        private Int16 exitCount;
<<<<<<< HEAD
<<<<<<< HEAD
        private Int16 dimension;

        private Vector3 coordinates;

  

=======
        private Int16 dimesion;
>>>>>>> parent of 89fab3a (Merge remote-tracking branch 'remotes/origin/Roman')
=======
        private Int16 dimesion;
>>>>>>> parent of 89fab3a (Merge remote-tracking branch 'remotes/origin/Roman')
        public Node(Int64 id, Int16 dimension)
        {
            this.id = id;
            this.dimesion = dimension;
            this.neighbors = new List<Int64>();
            this.exitCount = 0;
<<<<<<< HEAD
        }

        public Node(Int64 id, Int16 dimension, NodeType type) : this(id, dimension) 
        {
            this.type = type;
        }

        public Node(Int64 id, Int16 dimension, NodeType type, Int16 exits) : this(id, dimension, type) 
        {
            this.exitCount = exits;
=======
>>>>>>> parent of 89fab3a (Merge remote-tracking branch 'remotes/origin/Roman')
        }

        public NodeType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public Int64 Id
        {
            get { return this.id; }
        }

        public List<Int64> Neighbors
        {
            get { return this.neighbors; }
        }

        public Int16 ExitCount
        {
            get { return this.exitCount; }
            set { this.exitCount = value; }
        }

        public Int16 Dimension
        {
            get { return this.dimesion; }
        }


    }
}
