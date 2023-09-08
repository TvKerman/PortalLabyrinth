using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private List<Portal> _portals = new List<Portal>();

    private Camera _myCamera;

    private void Awake()
    {
        _myCamera = GetComponent<Camera>();
    }

    private void OnPreRender()
    {
        for (int i = 0; i < _portals.Count; i++)
        {
            _portals[i].Render(_myCamera);
        }
    }

    public void AddPortal(Portal portal) 
    { 
        _portals.Add(portal);
    }
}
