Shader "Takomat/VertexColor"
{
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader {
		Tags { 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;

			struct appdata_t
			{
				float4 vertex		: POSITION;
				fixed4 color		: COLOR;
			};

			struct v2f
			{
				float4 vertex		: SV_POSITION;
				fixed4 color		: COLOR0;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.color = IN.color;

				return OUT;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				return IN.color * _Color;
			}
			ENDCG
		}
	}
}
