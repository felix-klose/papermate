// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/GridOverlayShader"
{
	Properties
	{
		_GridColor("Grid Color", Color) = (0.5, 1.0, 1.0)
		_GraduationColor("Graduation Color", Color) = (0.5, 1.0, 1.0)
		_GridScale("Grid Scale", Float) = .02
		_GraduationStep("Graduation Step", Int) = 10
		_GraduationScaleFactor("Graduation Scale Factor", Int) = 2
		_LineThickness("Line Thickness", Float) = 1.0
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		ZWrite On // We need to write in depth to avoid tearing issues
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
			#include "UnityCG.cginc"

			uniform float4 _GridColor;
			uniform float4 _GraduationColor;
			uniform float _GridScale;
			uniform float _LineThickness;
			uniform int _GraduationStep;
			uniform float _GraduationScaleFactor;

			struct appdata
			{
			float4 vertex : POSITION;
			};

			struct v2f
			{
			float4 vertex : SV_POSITION;
			float3 worldPos : TEXCOORD0;
			UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata v)
			{
				v2f o;

				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float xIndex = i.worldPos.x / _GridScale;
				float x = floor(xIndex + 0.5);
				float distX = abs(i.worldPos.x - x * _GridScale);

				// Get the distances to the next horizontal gridlines
				float yIndex = i.worldPos.y / _GridScale;
				float y = floor(yIndex + 0.5);

				float distY = abs(i.worldPos.y - y * _GridScale);

				float minDist = min(distX, distY);

				float GraduationMaxDistance = _LineThickness * _GraduationScaleFactor / 2.0;

				// Check if we are on a graduation line
				bool isGraduationLine = false;

				isGraduationLine = x % _GraduationStep == 0 && distX <= GraduationMaxDistance;
				isGraduationLine = isGraduationLine || (y % _GraduationStep == 0 && distY <= GraduationMaxDistance);

				// Set the color depending on whether we're on a graduation or not
				fixed4 col = isGraduationLine ? _GraduationColor :_GridColor;

				// If the minimum distance is below the threshold, the pixel is visible.
				// We multiply here to enable alpha values in the grid color parameter
				col.a *= minDist < (_LineThickness / 2.0) * (isGraduationLine * (_GraduationScaleFactor - 1) + 1);

				return col;
			}

			ENDCG
		}
	}
}