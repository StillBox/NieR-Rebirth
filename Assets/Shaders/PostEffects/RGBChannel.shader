Shader "STB/NieR/PostEffect/RGBChannel" {
	Properties 
	{
		_MainTex("Base", 2D) = "white" {}
		_Scale ("Offset Factor", Range(-1,1)) = 0.005
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
			float _Scale;
			
			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv[2] : TEXCOORD0;
			};

			v2f vert(appdata_img v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				half2 uv = v.texcoord;
				o.uv[0] = uv;
				o.uv[1] = uv - (uv - float2(0.5, 0.5)) * _Scale;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				fixed3 origin = tex2D(_MainTex, i.uv[0]).rgb;
				fixed3 offset = tex2D(_MainTex, i.uv[1]).rgb;
				fixed3 col = origin;
				col.x = origin.x * 0.2 + offset.x * 0.8;
				col.y = origin.y * 0.8 + offset.y * 0.2;
				col.z = origin.z * 0.1 + offset.z * 0.9;
				return fixed4(col, 1);
			}
			
			ENDCG
		}
	}
	FallBack Off
}
