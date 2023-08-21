using UnityEngine;

public class PortalRenderer : MonoBehaviour
{
    //TODO если необходимо, можно сделать рамки разного цвета для порталов.
    //SerializeField] private Color _outlineColor;
    //[SerializeField] private Renderer _outline;
 
    
    [SerializeField] private Camera _portalCamera;
    
    // number of iterations for recursive render 
    // P.S. для fps БОЛЬНО
    [SerializeField, Range(1, 10)] private int _renderIterations = 3;

    private Material _material;
    private Renderer _renderer;
    private RenderTexture _rTexture;

    
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
        _rTexture = new RenderTexture(Screen.width, Screen.height, 0);
        _material.mainTexture = _rTexture;
        _portalCamera.targetTexture = _rTexture;
        //_outline.material.color = _outlineColor;
        _material.SetInt("_DrawingFlag", 1);
    }

    public void Render(Camera mainCamera, Transform otherPortal)
    {
        // render if the camera sees the render object ("optimization")
        if (!_renderer.isVisible)
        {
            return;
        }
        _material.SetInt("_DrawingFlag", 0);
        for (int i = _renderIterations - 1; i >= 0; i--)
        {
            RenderInternal(mainCamera, otherPortal, i);
            _material.SetInt("_DrawingFlag", 1);
        }
    }
    private void RenderInternal(Camera mainCamera, Transform otherPortal, int iteration)
    {
        // Напишу на русском. 
        // Камера портала, в который должен зайтии игрок синхронизируется и
        // зеркалится отностильно другого портала. 
        Transform enterPoint = transform;
        Transform exitPoint = otherPortal;

        Transform portalCamTransform = _portalCamera.transform;
        portalCamTransform.position = mainCamera.transform.position;
        portalCamTransform.rotation = mainCamera.transform.rotation;

        // После позиционирования происходит итерративная обработка рекурсивных рендеров.
        for (int i = 0; i <= iteration; i++)
        {
            portalCamTransform.MirrorPosition(enterPoint, exitPoint);
            portalCamTransform.MirrorRotation(enterPoint, exitPoint);
        }

        // Здесь мы устанавливаем правильную проекцию,
        // тем самым портальная камера больше не захватывает объекты между ней и порталом.
        SetupProjection(mainCamera, exitPoint);

        // Запускаем рендер камеры 
        _portalCamera.Render();
    }

    private void SetupProjection(Camera mainCamera, Transform exitPoint)
    {
        Plane p = new Plane(-exitPoint.forward, exitPoint.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(
                                           Matrix4x4.Inverse(_portalCamera.worldToCameraMatrix)) * clipPlane;
        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        _portalCamera.projectionMatrix = newMatrix;
    }
}