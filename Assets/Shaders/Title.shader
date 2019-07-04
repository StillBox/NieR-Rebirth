Shader "STB/NieR/Title"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
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
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "lighting.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

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
				if (color.a < 0.1)
				{
					return fixed4(1, 1, 1, 1);
				}
				else
				{
					col.r = 1 - color.r;
					col.g = 1 - color.g;
					col.b = 1 - color.b;
					col.a = color.a;
					return col;
				}
			}

			v2f vert(a2v v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				if (in_range(i.uv, _Inverse0)) col = inverse(col);
				if (in_range(i.uv, _Inverse1)) col = inverse(col);
				if (in_range(i.uv, _Inverse2)) col = inverse(col);
				if (in_range(i.uv, _Inverse3)) col = inverse(col);
				if (in_range(i.uv, _Offset0)) col = tex2D(_MainTex, float2(i.uv.x - 0.008, i.uv.y));
				if (in_range(i.uv, _Offset1)) col = tex2D(_MainTex, float2(i.uv.x - 0.008, i.uv.y));
				if (in_range(i.uv, _Offset2)) col = tex2D(_MainTex, float2(i.uv.x - 0.008, i.uv.y));
				if (in_range(i.uv, _Offset3)) col = tex2D(_MainTex, float2(i.uv.x - 0.008, i.uv.y));
				if (in_range(i.uv, _Gray0)) col = fixed4(0.5, 0.5, 0.5, 0.5);
				if (in_range(i.uv, _Gray1)) col = fixed4(0.5, 0.5, 0.5, 0.5);
				if (in_range(i.uv, _Gray2)) col = fixed4(0.5, 0.5, 0.5, 0.5);
				if (in_range(i.uv, _Gray3)) col = fixed4(0.5, 0.5, 0.5, 0.5);
				return col;
			}
			ENDCG
		}
	}
}