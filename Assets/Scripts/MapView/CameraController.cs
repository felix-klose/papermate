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

        new private Camera camera;

        private Vector2 mapBounds;

        public UnityEvent OnZoomLevelChangedEvent = new UnityEvent();

        private void Awake()
        {
            camera = GetComponent<Camera>();
            PlayerInput.GetInstance().OnDragEvent.AddListener(OnDrag);
            PlayerInput.GetInstance().OnZoomEvent.AddListener(OnZoom);

            MapManager.GetInstance().OnCurrentMapChanged.AddListener(UpdateControlConstraints);

            UpdateControlConstraints();
        }

        public void OnDrag(Vector3 mousePositionDelta)
        {
            transform.position += MouseDeltaToWorldDelta(mousePositionDelta) * movementSpeed;
            FixPosition();
        }

        public void OnZoom(float scrollInput)
        {
            float newOrthoSize = camera.orthographicSize - zoomSpeed * scrollInput;

            newOrthoSize = Mathf.Clamp(newOrthoSize, minOrthographicSize, maxOrthographicSize);

            camera.orthographicSize = newOrthoSize;

            OnZoomLevelChangedEvent.Invoke();

            UpdateMapBounds();
            FixPosition();
        }

        private Vector3 MouseDeltaToWorldDelta(Vector3 mouseDelta)
        {
            Camera mainCamera = Camera.main;
            float viewPortUnits = mainCamera.orthographicSize * 2.0f;
            float pixelsPerUnit = mainCamera.pixelHeight / viewPortUnits;

            return mouseDelta / pixelsPerUnit;
        }

        private void UpdateControlConstraints()
        {
            UpdateMaxOrthographicSize();
            UpdateMapBounds();
        }

        private void UpdateMapBounds()
        {
            Vector2 mapDimensions = MapManager.GetInstance().GetCurrentMapDimensions();

            mapBounds = new Vector2(mapDimensions.x / 2f, mapDimensions.y / 2f);
            float xOrthographicSize = camera.orthographicSize * camera.aspect;
            mapBounds.x = Mathf.Max(mapBounds.x - xOrthographicSize, 0);
            mapBounds.y = Mathf.Max(mapBounds.y - camera.orthographicSize, 0);
        }

        private void UpdateMaxOrthographicSize()
        {
            Vector2 mapDimensions = MapManager.GetInstance().GetCurrentMapDimensions();
            float mapAspect = mapDimensions.x / mapDimensions.y;

            float yOrthographicSize = mapDimensions.y / 2f;
            float xAdjustedOrthographicSize = (mapDimensions.x / mapAspect) / 2f;

            maxOrthographicSize = Mathf.Max(xAdjustedOrthographicSize, yOrthographicSize);
        }

        private void FixPosition()
        {
            Vector3 targetPosition = transform.position;

            targetPosition.x = Mathf.Clamp(targetPosition.x, -mapBounds.x, mapBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, -mapBounds.y, mapBounds.y);
            targetPosition.z = transform.position.z;

            transform.position = targetPosition;
        }
    }

}