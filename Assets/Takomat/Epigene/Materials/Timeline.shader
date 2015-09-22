Shader "Takomat/Timeline"
{
	Properties {
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		_Percent ("Percent", float) = 0.5
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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			uniform float _Percent;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex        : POSITION;
				float2 texcoord      : TEXCOORD0;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);

				return OUT;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				if (IN.texcoord.x > _Percent)
					return fixed4(0, 0, 0, 0);
				else
					return tex2D( _MainTex, IN.texcoord) * _Color;
			}
			ENDCG
		}
	}
}
