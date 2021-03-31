using UnityEngine;

namespace Papermate.MapView
{
    [RequireComponent(typeof(MeshRenderer))]
    public class GridDisplay : MonoBehaviour
    {
        [SerializeField]
        private CameraController targetCameraController;
        [SerializeField]
        private float gridLinePixelWidth = 5;
        [SerializeField]
        private float zIndex = -1;

        new private MeshRenderer renderer;
        private Camera targetCamera;

        private void Awake()
        {
            renderer = GetComponent<MeshRenderer>();
            targetCamera = targetCameraController.GetComponent<Camera>();

            targetCameraController.OnZoomLevelChangedEvent.AddListener(OnZoomLevelChanged);

            UpdateScale();
            UpdateGridProperties();
        }

        private void OnZoomLevelChanged()
        {
            UpdateScale();
            UpdateGridProperties();
        }

        private void UpdateScale()
        {
            float h = targetCamera.orthographicSize * 2.0f + 0.5f;
            float w = targetCamera.aspect * h + 0.5f;

            transform.localScale = new Vector3(w, h, 0);
        }

        private void UpdateGridProperties()
        {
            float unitWidth = targetCamera.orthographicSize * 2.0f * targetCamera.rect.width;
            float pixelWidth = targetCamera.scaledPixelWidth;
            float pixelUnitDistance = unitWidth / pixelWidth;

            renderer.material.SetFloat("_LineThickness", pixelUnitDistance * gridLinePixelWidth);
        }
    }
}