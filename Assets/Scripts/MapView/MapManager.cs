/*
 * This file is part of Papermate which is released under Apache License 2.0.
 * See file LICENSE.txt for full license details.
 */

using UnityEngine;
using UnityEngine.Events;

namespace Papermate.MapView
{
    /// <summary>
    /// Singleton manager class to control map properties and display
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static MapManager instance;

        /// <summary>
        /// Default map texture
        /// </summary>
        [SerializeField]
        private Texture2D defaultMapTexture;

        /// <summary>
        /// Default map dimensions in meters. If one Vector-component is given as a negative number,
        /// the other component will be calculated from the texture's aspect ratio.
        /// 
        /// Warning: If both coordinates are negative, this will cause issues
        /// </summary>
        [SerializeField]
        private Vector2 defaultMapDimensions;

        private Texture2D currentMapTexture;
        private Vector2 currentMapDimensions;

        /// <summary>
        /// Event invoked whenever map dimensions or texture change
        /// </summary>
        public UnityEvent OnCurrentMapChanged = new UnityEvent();

        /// <summary>
        /// Singleton implementation - Getter for the singleton instance
        /// </summary>
        /// <returns>the current singleton instance</returns>
        public static MapManager GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// Singleton implementation - Setter for the singleton instance
        /// </summary>
        /// <param name="instance">the new singleton instance</param>
        private static void SetInstance(MapManager instance)
        {
            MapManager.instance = instance;
        }

        /// <summary>
        /// Called after scene loading or object instantiation before first update. Sets up initial attribute values.
        /// Ensures this classes singleton property.
        /// </summary>
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

        /// <summary>
        /// Clears the singleton instance on object destruction if necessary.
        /// Ensures this classes singleton property.
        /// </summary>
        private void OnDestroy()
        {
            if (MapManager.GetInstance() == this)
                MapManager.SetInstance(null);
        }

        /// <summary>
        /// Calculates missing components of the map dimensions in meters
        /// </summary>
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
