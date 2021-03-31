using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MapGrid : MonoBehaviour
{
    [SerializeField]
    private float gridLinePixelWidth = 5;
    [SerializeField]
    private float zIndex = -1;

    new private MeshRenderer renderer;

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();

        UpdateScale();
        UpdateGridProperties();
    }

    void Update()
    {
        Vector3 targetPosition = Camera.main.transform.position;
        targetPosition.z = zIndex;

        transform.position = targetPosition;

        UpdateScale();
        UpdateGridProperties();
    }

    void UpdateScale()
    {
        float h = Camera.main.orthographicSize * 2.0f + 0.5f;
        float w = Camera.main.aspect * h + 0.5f;

        transform.localScale = new Vector3(w, h, 0);
    }

    void UpdateGridProperties()
    {
        Camera mainCamera = Camera.main;
        float unitWidth = mainCamera.orthographicSize * 2.0f * mainCamera.rect.width;
        float pixelWidth = mainCamera.scaledPixelWidth;
        float pixelUnitDistance = unitWidth / pixelWidth;

        renderer.material.SetFloat("_LineThickness", pixelUnitDistance * gridLinePixelWidth);
    }
}
