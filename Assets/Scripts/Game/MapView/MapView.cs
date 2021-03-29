using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MapView : MonoBehaviour
{
    [SerializeField]
    private float gridLinePixelWidth = 5;

    private MeshRenderer renderer;

    private float pixelUnitDistance;

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();

        Camera mainCamera = Camera.main;
        float unitWidth = mainCamera.orthographicSize * 2.0f * mainCamera.rect.width;
        float pixelWidth = mainCamera.scaledPixelWidth;
        pixelUnitDistance = unitWidth / pixelWidth;

        renderer.material.SetFloat("_LineThickness", pixelUnitDistance * gridLinePixelWidth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
