using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Labyrinth
{
    public class Unit : MonoBehaviour
    {
        private Int64 id;
        private Node infoNode;
        [SerializeField] private List<Teleporter> portals;

        public Teleporter GetTeleporter(Int32 index)  
        {
            if (index < 0 || index >= portals.Count) 
            { 
                throw new ArgumentOutOfRangeException("index");
            }
            return portals[index];
        }

        

        public Int32 GetCountPortals() 
        { 
            return portals.Count;
        }

        public Node Info 
        { 
            get { return infoNode; }
            
            set 
            { 
                infoNode = value;
                if (infoNode != null) 
                {
                    id = infoNode.Id;
                }
            }
        }
    }
}