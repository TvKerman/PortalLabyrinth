using UnityEngine;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    [SerializeField] private List<Portal> _listExitPortal;
    [SerializeField] private Portal _currentExitPortal;
    [SerializeField] private PortalRenderer _renderer;
    
    public void Render(Camera mainCamera)
    {
        if (_currentExitPortal != null)
        {
            _renderer.Render(mainCamera, _currentExitPortal.transform);
        }
    }

    public void SetCurrentExitPortal(Portal portal) 
    { 
        _currentExitPortal = portal;
    }
}
