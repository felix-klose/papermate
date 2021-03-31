/*
 * This file is part of Papermate which is released under <license>.
 * See file <file> for full license details.
 */

using UnityEngine;

namespace Papermate.MapView
{
    /// <summary>
    /// Component to display the map. Creates a map sprite and scales it to the desired
    /// world space size.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class MapDisplay : MonoBehaviour
    {
        /// <summary>
        /// Sprite renderer used to display the map sprite
        /// </summary>
        private SpriteRenderer spriteRenderer;

        /// <summary>
        /// The map sprite. This is not controlled via components or serializable attributes
        /// since it needs to be overwritten if the map changes
        /// </summary>
        private Sprite mapSprite;

        /// <summary>
        /// Called after scene loading or object instantiation before first update. Sets up Event listeners and initial
        /// attribute values.
        /// </summary>
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateMapProperties();

            MapManager.GetInstance().OnCurrentMapChanged.AddListener(UpdateMapProperties);
        }

        /// <summary>
        /// Calculates scaling (as pixels per unit) and updates the displayed sprite object
        /// </summary>
        private void UpdateMapProperties()
        {
            Vector2 mapDimensions = MapManager.GetInstance().GetCurrentMapDimensions();
            Texture2D mapTexture = MapManager.GetInstance().GetCurrentMapTexture();

            float pixelsPerUnit = mapTexture.width / mapDimensions.x;

            mapSprite = Sprite.Create(mapTexture, new Rect(0f, 0f, mapTexture.width, mapTexture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            spriteRenderer.sprite = mapSprite;
        }
    }
}