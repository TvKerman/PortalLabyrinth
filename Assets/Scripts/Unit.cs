using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Labyrinth
{
    public class Unit : MonoBehaviour
    {
        public int id;
        public List<int> childs;
        public Node infoNode;
        [SerializeField] private List<Teleporter> portals;
        [SerializeField] private List<GameObject> doors;

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

        public void OpenTheDoor(bool isOpen) 
        {
            foreach (var door in doors)
            {
                door.SetActive(isOpen);
            }
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
                    childs = infoNode.Childs;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerInfo>().SetCurrentUnit(this);
                OpenTheDoor(false);
            }
        }
    }
}