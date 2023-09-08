using System;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private List<Teleporter> teleporters;
    [SerializeField] private Teleporter Other;

    private void OnTriggerStay(Collider other)
    {
        // convert the player's global coordinates to local relative to the portal
        float zPos = transform.worldToLocalMatrix.MultiplyPoint3x4(other.transform.position).z;

        // teleport if most of the player went behind the portal
        if (zPos > 0) Teleport(other.transform);
    }

    private void Teleport(Transform obj)
    {
        // Player position after teleporting
        
        Vector3 localPos = transform.worldToLocalMatrix.MultiplyPoint3x4(obj.position);
        
        // invert and set the new position of the player
        localPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
        obj.position = Other.transform.localToWorldMatrix.MultiplyPoint3x4(localPos);

        // Player rotation after teleporting
        Quaternion difference = Other.transform.rotation * Quaternion.Inverse(transform.rotation * Quaternion.Euler(0, 180, 0));
        obj.rotation = difference * obj.rotation;
    }

    public void CreateListExitPointForDimension() 
    { 
        teleporters = new List<Teleporter>(4);
        teleporters.Add(null);
        teleporters.Add(null);
        teleporters.Add(null);
        teleporters.Add(null);
    }

    public void SetCurrentExitPoint(Teleporter teleporter) 
    { 
        Other = teleporter;
        gameObject.GetComponent<Portal>().SetCurrentExitPortal(teleporter.gameObject.GetComponent<Portal>());
    }

    public void SetCurrentExitPoint(int dimensionIndex) 
    {
        if (teleporters == null) 
        {
            throw new ArgumentNullException("The current teleport has a connection in only one dimension");
        }
        if (dimensionIndex < 0 || dimensionIndex >= teleporters.Count) 
        {
            throw new ArgumentOutOfRangeException("Index teleport exit point out of range");
        }
        //if (teleporters[dimensionIndex] == null) 
        //{
        //    throw new ArgumentNullException("The exit point in this dimension is not set");
        //}

        SetCurrentExitPoint(teleporters[dimensionIndex]);
    }

    public void AddExitPoint(int index, Teleporter teleporter) 
    {
        if (index < 0 || index >= teleporters.Count) 
        { 
            throw new ArgumentOutOfRangeException("Can't add new teleport");
        }
        teleporters[index] = teleporter;
    }

    public bool IsCreateListDimension() 
    {
        return teleporters != null;
    }

    public bool IsExitPointSetInList(int index) 
    {
        if (index < 0 || index >= teleporters.Count)
        {
            throw new ArgumentOutOfRangeException("Index teleport exit point out of range");
        }
        return teleporters[index] != null;
    }

    public bool IsCurrentExitPointSet() 
    {
        return Other != null;
    }
}
