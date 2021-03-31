using UnityEngine;

using Papermate.MapView;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 1;
    [SerializeField]
    private float zoomSpeed = 1;
    [SerializeField]
    private float minOrthographicSize = 3;
    [SerializeField]
    private float maxOrthographicSize = 10;

    new private Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        MapViewInput.GetInstance().OnMouseDragEvent.AddListener(OnMouseDrag);
    }

    public void OnMouseDrag(Vector3 mousePositionDelta)
    {
        transform.Translate(MouseDeltaToWorldDelta(mousePositionDelta) * movementSpeed);
    }

    public void OnZoom(float zoomInput)
    {
        //float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        float newOrthoSize = camera.orthographicSize - zoomSpeed * zoomInput;

        newOrthoSize = Mathf.Clamp(newOrthoSize, minOrthographicSize, maxOrthographicSize);

        camera.orthographicSize = newOrthoSize;
    }

    private Vector3 MouseDeltaToWorldDelta(Vector3 mouseDelta)
    {
        Camera mainCamera = Camera.main;
        float viewPortUnits = mainCamera.orthographicSize * 2.0f;
        float pixelsPerUnit = mainCamera.pixelHeight / viewPortUnits;

        return mouseDelta / pixelsPerUnit;
    }
}
