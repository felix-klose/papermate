/*
 * This file is part of Papermate which is released under Apache License 2.0.
 * See file LICENSE.txt for full license details.
 */

using UnityEngine;

using Papermate.Events;

namespace Papermate.MapView
{

    /// <summary>
    /// Central input component for MapView player input. Implemented as a singleton. This will
    /// not be synchronized over the network.
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static PlayerInput instance;

        /// <summary>
        /// Camera used to get current viewport, aspect ratio etc.
        /// </summary>
        private Camera mainCamera;

        /// <summary>
        /// Toggle to en- or disable player input in the map view
        /// </summary>
        private bool mapViewInputEnabled;

        /// <summary>
        /// Control attribute to store if the mouse was pressed last frame.
        /// This is used to control input behavior where the mouse enters or
        /// leaves the viewport pressed.
        /// </summary>
        private bool mouseDown;

        /// <summary>
        /// Mouse position last frame. Used to calculate movement distances
        /// </summary>
        private Vector3 lastMousePosition;

        /// <summary>
        /// Drag event called when the player drags along the map (so far only with the mouse)
        /// Payload is the drag movement distance in pixels / viewport space
        /// </summary>
        public Vector3Event OnDragEvent = new Vector3Event();

        /// <summary>
        /// Zoom event called when the player zooms in or out of the map. Payload is the relative zoom
        /// change.
        /// </summary>
        public FloatEvent OnZoomEvent = new FloatEvent();

        /// <summary>
        /// Singleton implementation - Getter for the singleton instance
        /// </summary>
        /// <returns>the current singleton instance</returns>
        public static PlayerInput GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// Singleton implementation - Setter for the singleton instance
        /// </summary>
        /// <param name="instance">the new singleton instance</param>
        private static void SetInstance(PlayerInput instance)
        {
            PlayerInput.instance = instance;
        }

        /// <summary>
        /// Called after scene loading or object instantiation before first update. Sets up initial attribute values.
        /// Ensures this classes singleton property.
        /// </summary>
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
            }
            else
            {
                mainCamera = Camera.main;
                mapViewInputEnabled = true;
                mouseDown = false;
                SetInstance(this);
            }
        }

        /// <summary>
        /// Called once per frame. Registers input and invokes input events.
        /// </summary>
        private void Update()
        {
            Vector3 curMousePosition = Input.mousePosition;

            // We only want to react to input inside the camera viewport and
            // only if input isn't disabled for the map view
            if (mapViewInputEnabled && IsPointInViewport(curMousePosition))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    lastMousePosition = curMousePosition;
                    mouseDown = true;
                }
                else if (Input.GetMouseButton(0) && mouseDown)
                {
                    Vector3 mousePositionDelta = lastMousePosition - curMousePosition;

                    OnDragEvent.Invoke(mousePositionDelta);

                    lastMousePosition = curMousePosition;
                } else if(Input.GetMouseButtonUp(0))
                {
                    mouseDown = false;
                }

                if(!mouseDown)
                {
                    float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                    if (scrollInput != 0.0f)
                        OnZoomEvent.Invoke(scrollInput);
                }
            } 
            else
            {
                mouseDown = false;
            }
        }

        /// <summary>
        /// Clears the singleton instance on object destruction if necessary.
        /// Ensures this classes singleton property.
        /// </summary>
        private void OnDestroy()
        {
            if (instance == this)
            {
                SetInstance(null);
            }
        }

        /// <summary>
        /// Checks if a given point is inside the camera's viewport
        /// </summary>
        /// <param name="point">an arbitrary world space point</param>
        /// <returns>true, iff the point is inside the camera's viewport</returns>
        private bool IsPointInViewport(Vector3 point)
        {
            // We can't store these because the window and thus the viewport might be resized
            // Maybe move this update to a method called by resize events
            float viewportWidth = mainCamera.pixelWidth;
            float viewportHeight = mainCamera.pixelHeight;

            bool result = point.x >= 0 && point.x <= viewportWidth
                && point.y >= 0 && point.y <= viewportHeight;

            return result;
        }

        public void SetGameInputEnabled(bool gameInputEnabled)
        {
            this.mapViewInputEnabled = gameInputEnabled;
        }

        public bool IsGameInputEnabled()
        {
            return mapViewInputEnabled;
        }
    }
}

