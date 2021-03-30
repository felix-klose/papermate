using UnityEngine;

namespace Papermate.Game
{
    public class GameInput : MonoBehaviour
    {
        private static GameInput instance;

        private Camera mainCamera;
        private bool gameInputEnabled;

        private Vector3 lastMousePosition;

        public GameInput GetInstance()
        {
            return instance;
        }

        private static void SetInstance(GameInput instance)
        {
            GameInput.instance = instance;
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
                }
                else if (Input.GetMouseButton(0))
                {
                    Vector3 mousePositionDelta = curMousePosition - lastMousePosition;

                    // OnMapDragEvent.Invoke(mousePositionDelta);

                    lastMousePosition = curMousePosition;
                }
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

