using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Labyrinth
{
    public class Unit : MonoBehaviour
    {
        private int id;
        private Node infoNode;
        [SerializeField] private List<Teleporter> portals;

        public Teleporter GetTeleporter(int index)  
        {
            if (index < 0 || index >= portals.Count) 
            { 
                throw new ArgumentOutOfRangeException("index");
            }
            return portals[index];
        }
        
        public int GetCountPortals() 
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerInfo>().SetCurrentUnit(this);
            }
        }
    }
}