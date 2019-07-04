Shader "STB/NieR/PostEffect/ScreenGrid" {
	Properties 
	{
		_MainTex ("Base", 2D) = "white" {}
		_CountW ("Count in Width", Float) = 240
		_BorderW ("Border Size in Witdth", Float) = 0.016
		_CountH ("Count in Height", Float) = 135
		_BorderH ("Border Size in Height", Float) = 0.016
		_ShadeScale ("Grid Shade Scale", Range(0,1)) = 1
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
			float _CountW;
			float _BorderW;
			float _CountH;
			float _BorderH;
			float _ShadeScale;
			
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
				float w = frac(i.uv.x * _CountW);
				float h = frac(i.uv.y * _CountH);
				if (w >= _BorderW && w <= 1 - _BorderW && h >= _BorderH && h < 1 - _BorderW)
					col += fixed3(0.01, 0.01, 0.01) * _ShadeScale;
				else
					col -= fixed3(0.017, 0.017, 0.017) * _ShadeScale;
				return fixed4(col, 1);
			}
			
			ENDCG
		}
	}
	FallBack Off
}