Shader "STB/NieR/PostEffect/Snowflake" {
	Properties {
		_MainTex("Base", 2D) = "white" {}
		_Inverse0("Inverse0", Vector) = (0,0,0,0)
		_Inverse1("Inverse1", Vector) = (0,0,0,0)
		_Inverse2("Inverse2", Vector) = (0,0,0,0)
		_Inverse3("Inverse3", Vector) = (0,0,0,0)
		_Offset0("Offset0", Vector) = (0,0,0,0)
		_Offset1("Offset1", Vector) = (0,0,0,0)
		_Offset2("Offset2", Vector) = (0,0,0,0)
		_Offset3("Offset3", Vector) = (0,0,0,0)
		_Gray0("Gray0", Vector) = (0,0,0,0)
		_Gray1("Gray1", Vector) = (0,0,0,0)
		_Gray2("Gray2", Vector) = (0,0,0,0)
		_Gray3("Gray3", Vector) = (0,0,0,0)
		_Gray4("Gray4", Vector) = (0,0,0,0)
		_Gray5("Gray5", Vector) = (0,0,0,0)
		_Gray6("Gray6", Vector) = (0,0,0,0)
		_Gray7("Gray7", Vector) = (0,0,0,0)
	}
	SubShader {
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
			float4 _Inverse0;
			float4 _Inverse1;
			float4 _Inverse2;
			float4 _Inverse3;
			float4 _Offset0;
			float4 _Offset1;
			float4 _Offset2;
			float4 _Offset3;
			float4 _Gray0;
			float4 _Gray1;
			float4 _Gray2;
			float4 _Gray3;
			float4 _Gray4;
			float4 _Gray5;
			float4 _Gray6;
			float4 _Gray7;
			
			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			bool in_range(float2 uv, float4 range)
			{
				if (uv.x < range.x) return false;
				if (uv.x > range.x + range.z) return false;
				if (uv.y < range.y) return false;
				if (uv.y > range.y + range.w) return false;
				return true;
			}

			fixed4 inverse(fixed4 color)
			{
				fixed4 col;
				col.r = 1 - color.r;
				col.g = 1 - color.g;
				col.b = 1 - color.b;
				col.a = color.a;
				return col;
			}

			v2f vert(appdata_img v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				fixed4 col = tex2D(_MainTex, i.uv);
				if (in_range(i.uv, _Inverse0)) col = inverse(col);
				if (in_range(i.uv, _Inverse1)) col = inverse(col);
				if (in_range(i.uv, _Inverse2)) col = inverse(col);
				if (in_range(i.uv, _Inverse3)) col = inverse(col);
				if (in_range(i.uv, _Gray0)) col = fixed4(0.25, 0.25, 0.25, 1);
				if (in_range(i.uv, _Gray1)) col = fixed4(0.25, 0.25, 0.25, 1);
				if (in_range(i.uv, _Gray2)) col = fixed4(0.25, 0.25, 0.25, 1);
				if (in_range(i.uv, _Gray3)) col = fixed4(0.5, 0.5, 0.5, 1);
				if (in_range(i.uv, _Gray4)) col = fixed4(0.5, 0.5, 0.5, 1);
				if (in_range(i.uv, _Gray5)) col = fixed4(0.75, 0.75, 0.75, 1);
				if (in_range(i.uv, _Gray6)) col = fixed4(0.75, 0.75, 0.75, 1);
				if (in_range(i.uv, _Gray7)) col = fixed4(0.75, 0.75, 0.75, 1);
				if (in_range(i.uv, _Offset0)) col = tex2D(_MainTex, float2(i.uv.x - 0.1, i.uv.y - 0.05));
				if (in_range(i.uv, _Offset1)) col = tex2D(_MainTex, float2(i.uv.x - 0.1, i.uv.y + 0.05));
				if (in_range(i.uv, _Offset2)) col = tex2D(_MainTex, float2(i.uv.x + 0.1, i.uv.y - 0.05));
				if (in_range(i.uv, _Offset3)) col = tex2D(_MainTex, float2(i.uv.x + 0.1, i.uv.y + 0.05));
				return col;
			}
			
			ENDCG
		}
	}
	FallBack Off
}
