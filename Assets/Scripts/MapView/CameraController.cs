/*
 * This file is part of Papermate which is released under Apache License 2.0.
 * See file LICENSE.txt for full license details.
 */

using UnityEngine;
using UnityEngine.Events;

namespace Papermate.MapView
{
    /// <summary>
    /// Controller for player-controlled camera-movement and zoom.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// Relative zoom speed. For each unit of zoomSpeed, the change rate of the orthographic size of the
        /// camera is changed by 2
        /// </summary>
        [SerializeField]
        private float zoomSpeed = 1;

        /// <summary>
        /// Minimum orthographic size of the camera. Note that
        ///  a) orthographic size is measured vertically and
        ///  b) for each unit of orthographic size, 2 meters of the world are visible
        /// </summary>
        [SerializeField]
        private float minOrthographicSize = 3;

        /// <summary>
        /// Camera controlled by this controller. Used to get viewport information, aspect ratios and update 
        /// orthographic size / zoom level
        /// </summary>
        new private Camera camera;

        /// <summary>
        /// Maximum camera translation from the origin in both x and y directions
        /// </summary>
        private Vector2 mapBounds;

        /// <summary>
        /// Max orthographic size of the camera, i.e. max zoom level. This is controlled by the map's dimensions
        /// in meters
        /// </summary>
        private float maxOrthographicSize;

        /// <summary>
        /// Event fired whenever the zoom level changes. Since orthographic size and aspect ratio are both required
        /// for most calculations, listening classes will retrieve the required information directly from the controller
        /// instead of an event payload
        /// </summary>
        public UnityEvent OnZoomLevelChangedEvent = new UnityEvent();

        /// <summary>
        /// Called after scene loading or object instantiation before first update. Sets up Event listeners and initial
        /// attribute values.
        /// </summary>
        private void Awake()
        {
            camera = GetComponent<Camera>();
            PlayerInput.GetInstance().OnDragEvent.AddListener(OnDrag);
            PlayerInput.GetInstance().OnZoomEvent.AddListener(OnZoom);

            MapManager.GetInstance().OnCurrentMapChanged.AddListener(UpdateControlConstraints);

            UpdateControlConstraints();
        }

        /// <summary>
        /// Moves the camera. Called whenever the player drags the map.
        /// </summary>
        /// <param name="pointerPositionDelta">positional change of the pointer in pixels</param>
        public void OnDrag(Vector3 pointerPositionDelta)
        {
            transform.position += ScreenSpaceToWorldSpace(pointerPositionDelta);
            FixPosition();
        }

        /// <summary>
        /// Zooms in and out with the camera. Called whenever the player scrolls in the map view. 
        /// </summary>
        /// <param name="scrollInput">amount of scrolling registered this frame</param>
        public void OnZoom(float scrollInput)
        {
            // Update current zoom and invoke events
            float newOrthoSize = camera.orthographicSize - zoomSpeed * scrollInput;
            newOrthoSize = Mathf.Clamp(newOrthoSize, minOrthographicSize, maxOrthographicSize);
            camera.orthographicSize = newOrthoSize;
            OnZoomLevelChangedEvent.Invoke();

            // Update movement constraints and position
            UpdateMapBounds();
            FixPosition();
        }

        /// <summary>
        /// Converts a screen space vector to a world space vector. 
        /// 
        /// Note: Since this calculation depends on camera zoom, this shouldn't be moved to a 
        /// utility class or static method
        /// </summary>
        /// <param name="screenSpaceDelta">screen space vector</param>
        /// <returns>corresponding world space coordinate</returns>
        private Vector3 ScreenSpaceToWorldSpace(Vector3 screenSpaceDelta)
        {
            Camera mainCamera = Camera.main;
            float viewPortUnits = mainCamera.orthographicSize * 2.0f;
            float pixelsPerUnit = mainCamera.pixelHeight / viewPortUnits;

            return screenSpaceDelta / pixelsPerUnit;
        }

        /// <summary>
        /// Update minimum zoom level (furthest away) and movement bounds
        /// </summary>
        private void UpdateControlConstraints()
        {
            UpdateMaxOrthographicSize();
            UpdateMapBounds();
        }

        /// <summary>
        /// Update movement bounds
        /// </summary>
        private void UpdateMapBounds()
        {
            Vector2 mapDimensions = MapManager.GetInstance().GetCurrentMapDimensions();

            mapBounds = new Vector2(mapDimensions.x / 2f, mapDimensions.y / 2f);
            float xOrthographicSize = camera.orthographicSize * camera.aspect;
            mapBounds.x = Mathf.Max(mapBounds.x - xOrthographicSize, 0);
            mapBounds.y = Mathf.Max(mapBounds.y - camera.orthographicSize, 0);
        }

        /// <summary>
        /// Update minimum zoom level (furthest away)
        /// </summary>
        private void UpdateMaxOrthographicSize()
        {
            Vector2 mapDimensions = MapManager.GetInstance().GetCurrentMapDimensions();
            float mapAspect = mapDimensions.x / mapDimensions.y;

            float yOrthographicSize = mapDimensions.y / 2f;
            float xAdjustedOrthographicSize = (mapDimensions.x / mapAspect) / 2f;

            maxOrthographicSize = Mathf.Max(xAdjustedOrthographicSize, yOrthographicSize);
        }

        /// <summary>
        /// Ensure that this object is inside it's movement bounds. Should be called after every zoom
        /// or movement
        /// </summary>
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