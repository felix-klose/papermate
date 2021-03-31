using UnityEngine;
using UnityEngine.Events;

namespace Papermate.MapView
{
    public class MapManager : MonoBehaviour
    {
        private static MapManager instance;

        [SerializeField]
        private Texture2D defaultMapTexture;
        [SerializeField]
        private Vector2 defaultMapDimensions;

        private Texture2D currentMapTexture;
        private Vector2 currentMapDimensions;

        public UnityEvent OnCurrentMapChanged = new UnityEvent();

        public static MapManager GetInstance()
        {
            return instance;
        }

        private static void SetInstance(MapManager instance)
        {
            MapManager.instance = instance;
        }

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(this);
            } 
            else
            {
                currentMapTexture = defaultMapTexture;
                currentMapDimensions = defaultMapDimensions;
                CalculateMapSize();

                MapManager.SetInstance(this);
            }
        }

        private void OnDestroy()
        {
            if (MapManager.GetInstance() == this)
                MapManager.SetInstance(null);
        }

        private void CalculateMapSize()
        {
            float aspect = (float)currentMapTexture.width / (float)currentMapTexture.height;
            if (currentMapDimensions.x < 0)
            {
                currentMapDimensions.x = aspect * currentMapDimensions.y;
            }
            else
            {
                currentMapDimensions.y = currentMapDimensions.x / aspect;
            }
        }

        public Texture2D GetCurrentMapTexture()
        {
            return currentMapTexture;
        }

        public void GetCurrentMapTexture(Texture2D currentMapTexture)
        {
            this.currentMapTexture = currentMapTexture;
        }

        public Vector2 GetCurrentMapDimensions()
        {
            return currentMapDimensions;
        }

        public void SetCurrentMapDimensions(Vector2 currentMapDimensions)
        {
            this.currentMapDimensions = currentMapDimensions;
        }
    }
}
