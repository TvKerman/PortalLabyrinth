using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Portal _other;
    [SerializeField] private PortalRenderer _renderer;
    
    public void Render(Camera mainCamera)
    {
        _renderer.Render(mainCamera, _other.transform);
    }
}
