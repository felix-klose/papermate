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
				// Get the distances to the next vertical gridlines
				float xIndex = i.worldPos.x / _GridScale;
				float x1 = floor(xIndex);
				float x2 = ceil(xIndex);
				float distX1 = abs(i.worldPos.x - x1 * _GridScale);
				float distX2 = abs(i.worldPos.x - x2 * _GridScale);

				// Get the distances to the next horizontal gridlines
				float yIndex = i.worldPos.y / _GridScale;
				float y1 = floor(yIndex);
				float y2 = ceil(yIndex);
				float distY1 = abs(i.worldPos.y - y1 * _GridScale);
				float distY2 = abs(i.worldPos.y - y2 * _GridScale);

				// Determine the minimum distance
				float minDistX = min(distX1, distX2);
				float minDistY = min(distY1, distY2);
				float minDist = min(minDistX, minDistY);

				// Check if we are on a graduation line
				bool isGraduationLine = false;
				if (minDistX <= minDistY && distX1 <= distX2)
				{
					isGraduationLine = x1 % _GraduationStep == 0;
				}
				else if (minDistX <= minDistY)
				{
					isGraduationLine = x2 % _GraduationStep == 0;
				}
				else if (minDistY < minDistX && distY1 <= distY2)
				{
					isGraduationLine = y1 % _GraduationStep == 0;
				}
				else 
				{
					isGraduationLine = y2 % _GraduationStep == 0;
				}

				// Set the color depending on whether we're on a graduation or not
				fixed4 col = isGraduationLine ? _GraduationColor :_GridColor;

				// If the minimum distance is below the threshold, the pixel is visible.
				// We multiply here to enable alpha values in the grid color parameter
				col.a *= minDist < _LineThickness * (isGraduationLine * (_GraduationScaleFactor - 1) + 1);

				return col;
			}

			ENDCG
		}
	}
}