using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Map : MonoBehaviour
{
    [SerializeField]
    private Texture2D mapTexture;
    [SerializeField]
    private float mapWidth;

    private SpriteRenderer spriteRenderer;
    private Sprite sprite;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        float pixelsPerUnit = mapTexture.width / mapWidth;

        sprite = Sprite.Create(mapTexture, new Rect(0, 0, mapTexture.width, mapTexture.height),
              new Vector2(0.5f, 0.5f), pixelsPerUnit);
        spriteRenderer.sprite = sprite;
    }
}
