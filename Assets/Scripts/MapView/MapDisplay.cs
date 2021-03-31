using UnityEngine;

namespace Papermate.MapView
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MapDisplay : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Sprite mapSprite;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            Vector2 mapDimensions = MapManager.GetInstance().GetCurrentMapDimensions();
            Texture2D mapTexture = MapManager.GetInstance().GetCurrentMapTexture();

            float pixelsPerUnit = mapTexture.width / mapDimensions.x;

            mapSprite = Sprite.Create(mapTexture, new Rect(0f, 0f, mapTexture.width, mapTexture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            spriteRenderer.sprite = mapSprite;
        }
    }
}