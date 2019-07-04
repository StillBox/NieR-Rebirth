Shader "STB/NieR/PostEffect/CutOff"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Offset0("Offset 0", Vector) = (0,0,0)
		_Offset1("Offset 0", Vector) = (0,0,0)
		_Offset2("Offset 0", Vector) = (0,0,0)
		_Offset3("Offset 0", Vector) = (0,0,0)
	}
	SubShader
	{
		Pass {
			ZTest Always
			Cull Off
			ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Offset0;
			float4 _Offset1;
			float4 _Offset2;
			float4 _Offset3;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			bool in_range(float2 uv, float2 range)
			{
				if (uv.y < range.x) return false;
				if (uv.y > range.y) return false;
				return true;
			}

			v2f vert(appdata_img v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				float2 uv = i.uv;
				if (in_range(uv, _Offset0)) uv.x += _Offset0.z;
				if (in_range(uv, _Offset1)) uv.x += _Offset1.z;
				if (in_range(uv, _Offset2)) uv.x += _Offset2.z;
				if (in_range(uv, _Offset3)) uv.x += _Offset3.z;
				return tex2D(_MainTex, uv);
			}

			ENDCG
		}
	}
}
