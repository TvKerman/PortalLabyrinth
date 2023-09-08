using UnityEngine;

public class Portal : MonoBehaviour
{
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
