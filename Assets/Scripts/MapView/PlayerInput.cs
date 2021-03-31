using UnityEngine;

using Papermate.Events;

namespace Papermate.MapView
{
    public class PlayerInput : MonoBehaviour
    {
        private static PlayerInput instance;

        private Camera mainCamera;
        private bool gameInputEnabled;
        private bool mouseDown;

        private Vector3 lastMousePosition;

        public Vector3Event OnMouseDragEvent = new Vector3Event();
        public FloatEvent OnMouseScrollEvent = new FloatEvent();

        public static PlayerInput GetInstance()
        {
            return instance;
        }

        private static void SetInstance(PlayerInput instance)
        {
            PlayerInput.instance = instance;
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
            }
            else
            {
                mainCamera = Camera.main;
                gameInputEnabled = true;
                mouseDown = false;
                SetInstance(this);
            }
        }

        private void Update()
        {
            Vector3 curMousePosition = Input.mousePosition;

            if (gameInputEnabled && isPointInViewport(curMousePosition))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    lastMousePosition = curMousePosition;
                    mouseDown = true;
                }
                else if (Input.GetMouseButton(0) && mouseDown)
                {
                    Vector3 mousePositionDelta = lastMousePosition - curMousePosition;

                    OnMouseDragEvent.Invoke(mousePositionDelta);

                    lastMousePosition = curMousePosition;
                } else if(Input.GetMouseButtonUp(0))
                {
                    mouseDown = false;
                }

                if(!mouseDown)
                {
                    float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                    if (scrollInput != 0.0f)
                        OnMouseScrollEvent.Invoke(scrollInput);
                }
            } 
            else
            {
                mouseDown = false;
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                SetInstance(null);
            }
        }

        private bool isPointInViewport(Vector3 point)
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
            this.gameInputEnabled = gameInputEnabled;
        }

        public bool IsGameInputEnabled()
        {
            return gameInputEnabled;
        }
    }
}

