Shader "STB/NieR/Distort"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Alpha("Distort Alpha", Range(-0.5,0.5)) = 0
		_Offset0("Node Offset 0", Range(-0.5,0.5)) = 0
		_Offset1("Node Offset 1", Range(-0.5,0.5)) = 0
		_Offset2("Node Offset 2", Range(-0.5,0.5)) = 0
		_Offset3("Node Offset 3", Range(-0.5,0.5)) = 0
		_Offset4("Node Offset 4", Range(-0.5,0.5)) = 0
		_Offset5("Node Offset 5", Range(-0.5,0.5)) = 0
		_Offset6("Node Offset 6", Range(-0.5,0.5)) = 0
		_Offset7("Node Offset 7", Range(-0.5,0.5)) = 0
		_Offset8("Node Offset 8", Range(-0.5,0.5)) = 0
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
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Alpha;
			float _Offset0;
			float _Offset1;
			float _Offset2;
			float _Offset3;
			float _Offset4;
			float _Offset5;
			float _Offset6;
			float _Offset7;
			float _Offset8;

			float getOffset(float v)
			{
				float level = 8 * v;
				float rate = frac(level);
				rate = floor(rate * 6) / 6;
				float offset = 0;
				if (level < 1) offset = lerp(_Offset0, _Offset1, rate) * rate;
				else if (level < 2) offset = lerp(_Offset1, _Offset2, rate) * rate;
				else if (level < 3) offset = lerp(_Offset2, _Offset3, rate) * rate;
				else if (level < 4) offset = lerp(_Offset3, _Offset4, rate) * rate;
				else if (level < 5) offset = lerp(_Offset4, _Offset5, rate) * rate;
				else if (level < 6) offset = lerp(_Offset5, _Offset6, rate) * rate;
				else if (level < 7) offset = lerp(_Offset6, _Offset7, rate) * rate;
				else if (level < 8) offset = lerp(_Offset7, _Offset8, rate) * rate;
				return offset;
			}

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float rate = 16 * i.uv.y;
				col = tex2D(_MainTex, float2(i.uv.x + getOffset(i.uv.y), i.uv.y));
				col.w = col.x * _Alpha;
				return col;
			}
			ENDCG
		}

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
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col.w = col.x;
				return col;
			}
			ENDCG
		}
	}
}
