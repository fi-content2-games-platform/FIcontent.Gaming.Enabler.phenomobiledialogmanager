Shader "Takomat/ScrollArea"
{
	Properties {
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		_Area ("Area", Vector) = (0, 0, 0, 0)
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
			uniform float _PositionX;
			uniform float _PositionY;
			uniform float4 _Area;

			struct appdata_t
			{
				float4 vertex		: POSITION;
				float2 texcoord		: TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex		: SV_POSITION;
				float2 texcoord		: TEXCOORD0;
				float4 scrPos		: TEXCOORD1;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.scrPos = mul(_Object2World, IN.vertex);
				OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);

				return OUT;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				
				if (IN.scrPos.y < _Area.y && IN.scrPos.y > _Area.w
					&& IN.scrPos.x > _Area.x && IN.scrPos.x < _Area.z)
				{
					return tex2D(_MainTex, IN.texcoord) * _Color;
				}				

				else
				{
					//return fixed4(0,0,0,1);  //for debug only
					return fixed4(0,0,0,0);
				}


			}
			ENDCG
		}
	}
}
