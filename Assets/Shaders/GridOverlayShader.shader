// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/GridOverlayShader"
{
	Properties
	{
	   _GridColor("Grid Color", Color) = (0.5, 1.0, 1.0)
	   _GridScale("Grid Scale", Float) = 1.0
	   _LineThickness("Line Thickness", Range(0.0001, 0.01)) = 0.005
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
			uniform float _GridScale;
			uniform float _LineThickness;

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
				float x1 = floor(xIndex);
				float x2 = ceil(xIndex);
				float distX1 = abs(i.worldPos.x - x1 * _GridScale);
				float distX2 = abs(i.worldPos.x - x2 * _GridScale);

				float yIndex = i.worldPos.y / _GridScale;
				float y1 = floor(yIndex);
				float y2 = ceil(yIndex);
				float distY1 = abs(i.worldPos.y - y1 * _GridScale);
				float distY2 = abs(i.worldPos.y - y2 * _GridScale);

				float minDist = min(distX1, distX2);
				minDist = min(minDist, distY1);
				minDist = min(minDist, distY2);

				fixed4 col = _GridColor;

				col.a *= minDist < _LineThickness;

				return col;
			}

			ENDCG
		}
	}
}