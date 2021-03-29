using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector3 lastMousePosition;
    new private Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        } else if(Input.GetMouseButton(0))
        {
            Vector3 deltaMousePosition = lastMousePosition - Input.mousePosition;
            lastMousePosition = Input.mousePosition;

            transform.Translate(MouseDeltaToWorldDelta(deltaMousePosition) * movementSpeed);
        }

        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
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
