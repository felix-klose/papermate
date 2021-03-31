/*
 * This file is part of Papermate which is released under Apache License 2.0.
 * See file LICENSE.txt for full license details.
 */

using UnityEngine;

namespace Papermate.MapView
{
    /// <summary>
    /// Grid renderer component using the GridOverlayShader shader for rendering
    /// an automatically aligned grid. Note that this Component does not automatically
    /// update it's position, positions are expected to be handled via external scripts or
    /// object hierarchy.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class GridDisplay : MonoBehaviour
    {
        /// <summary>
        /// Public accessor for gridColor
        /// </summary>
        public Color GridColor { get => gridColor; set { gridColor = value; UpdateGridProperties(); } }

        /// <summary>
        /// Public accessor for graduationColor
        /// </summary>
        public Color GraduationColor { get => graduationColor; set { graduationColor = value; UpdateGridProperties(); } }

        /// <summary>
        /// Public accessor for gridScale
        /// </summary>
        public float GridScale { get => gridScale; set { gridScale = value; UpdateGridProperties(); } }

        /// <summary>
        /// Public accessor for graduationStep
        /// </summary>
        public int GraduationStep { get => graduationStep; set { graduationStep = value; UpdateGridProperties(); } }

        /// <summary>
        /// Public accessor for graduationScaleFactor
        /// </summary>
        public float GraduationScaleFactor { get => graduationScaleFactor; set { graduationScaleFactor = value; UpdateGridProperties(); } }

        /// <summary>
        /// Public accessor for gridLinePixelWidth
        /// </summary>
        public int GridLinePixelWidth { get => gridLinePixelWidth; set { gridLinePixelWidth = value; UpdateGridProperties(); } }
        /// <summary>
        /// CameraController for the Camera displaying this grid
        /// </summary>
        [SerializeField]
        private CameraController targetCameraController;

        /// <summary>
        /// Color of the grid lines. Passed through to shader.
        /// </summary>
        [SerializeField]
        private Color gridColor = new Color(0.5f, 1f, 1f);

        /// <summary>
        /// Color of the graduation grid lines. Passed through to shader.
        /// </summary>
        [SerializeField]
        private Color graduationColor = new Color(0.5f, 1f, 1f);

        /// <summary>
        /// Distance in meters between each grid line. Passed through to shader.
        /// </summary>
        [SerializeField]
        private float gridScale = 1f;

        /// <summary>
        /// For a graduationStep of n, each nth line is a graduation line.
        /// Passed through to shader.
        /// </summary>
        [SerializeField]
        private int graduationStep = 1;

        /// <summary>
        /// Line thickness factor for graduation lines.
        /// Passed through to shader.
        /// </summary>
        [SerializeField]
        private float graduationScaleFactor = 1f;

        /// <summary>
        /// Pixel width of each grid line. World space width as used in the shader
        /// is computed in this class since this information is not easily available
        /// inside the shader
        /// </summary>
        [SerializeField]
        private int gridLinePixelWidth = 5;

        /// <summary>
        /// MeshRenderer displaying the grid. This renderer's material has to use the GridOverlayShader
        /// to work with this script.
        /// </summary>
        new private MeshRenderer renderer;

        /// <summary>
        /// Camera displaying this grid
        /// </summary>
        private Camera targetCamera;

        /// <summary>
        /// Called after scene loading or object instantiation before first update. Sets up Event listeners and initial
        /// attribute values.
        /// </summary>
        private void Awake()
        {
            renderer = GetComponent<MeshRenderer>();
            targetCamera = targetCameraController.GetComponent<Camera>();

            targetCameraController.OnZoomLevelChangedEvent.AddListener(OnZoomLevelChanged);

            UpdateScale();
            UpdateGridProperties();
        }

        /// <summary>
        /// Update rendering properties of this grid. Usually called after camera zoom
        /// </summary>
        private void OnZoomLevelChanged()
        {
            UpdateScale();
            UpdateGridProperties();
        }

        /// <summary>
        /// Update the scale of this object. This is used to ensure only lines the camera view
        /// (plus a margin of 0.5 meters) are rendered
        /// </summary>
        private void UpdateScale()
        {
            float h = targetCamera.orthographicSize * 2.0f + 0.5f;
            float w = targetCamera.aspect * h + 0.5f;

            transform.localScale = new Vector3(w, h, 0);
        }

        /// <summary>
        /// Update line thickness in world space to ensure the desired pixel width is used
        /// </summary>
        private void UpdateGridProperties()
        {
            float unitWidth = targetCamera.orthographicSize * 2.0f * targetCamera.rect.width;
            float pixelWidth = targetCamera.scaledPixelWidth;
            float pixelUnitDistance = unitWidth / pixelWidth;

            renderer.material.SetColor("_GridColor", gridColor);
            renderer.material.SetColor("_GraduationColor", graduationColor);
            renderer.material.SetFloat("_GridScale", gridScale);
            renderer.material.SetInt("_GraduationStep", graduationStep);
            renderer.material.SetFloat("_GraduationScaleFactor", graduationScaleFactor);
            renderer.material.SetFloat("_LineThickness", pixelUnitDistance * gridLinePixelWidth);
            
        }
    }
}