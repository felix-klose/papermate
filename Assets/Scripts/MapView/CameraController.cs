using UnityEngine;
using UnityEngine.Events;

namespace Papermate.MapView
{
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

        public UnityEvent OnZoomLevelChangedEvent = new UnityEvent();

        private void Awake()
        {
            camera = GetComponent<Camera>();
            PlayerInput.GetInstance().OnMouseDragEvent.AddListener(OnMouseDrag);
            PlayerInput.GetInstance().OnMouseScrollEvent.AddListener(OnMouseScroll);
        }

        public void OnMouseDrag(Vector3 mousePositionDelta)
        {
            transform.Translate(MouseDeltaToWorldDelta(mousePositionDelta) * movementSpeed);
        }

        public void OnMouseScroll(float scrollInput)
        {
            float newOrthoSize = camera.orthographicSize - zoomSpeed * scrollInput;

            newOrthoSize = Mathf.Clamp(newOrthoSize, minOrthographicSize, maxOrthographicSize);

            camera.orthographicSize = newOrthoSize;

            OnZoomLevelChangedEvent.Invoke();
        }

        private Vector3 MouseDeltaToWorldDelta(Vector3 mouseDelta)
        {
            Camera mainCamera = Camera.main;
            float viewPortUnits = mainCamera.orthographicSize * 2.0f;
            float pixelsPerUnit = mainCamera.pixelHeight / viewPortUnits;

            return mouseDelta / pixelsPerUnit;
        }
    }

}