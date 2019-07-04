Shader "STB/NieR/PostEffect/FadeBorder"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FixedFade("FixedFade", Range(0,1)) = 0.1
		_RatioFade("RatioFade", Range(0,1)) = 0.1
	}
	SubShader
	{
		ZTest Always
		Cull Off
		ZWrite Off

		Pass {

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"

			sampler2D _MainTex;
			float _FixedFade;
			float _RatioFade;

			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			v2f vert(appdata_img v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				float3 col = tex2D(_MainTex, i.uv);
				float dist = pow(i.uv.x - 0.5, 2) + pow(i.uv.y - 0.5, 2);
				if (dist > 0.25)
				{
					float fadeRate = 4 * (dist - 0.25);
					col -= float3(1, 1, 1) * _FixedFade * fadeRate;
					col *= 1 - _RatioFade * fadeRate;
				}
				return float4(col, 1);
			}

			ENDCG
		}
	}
}
