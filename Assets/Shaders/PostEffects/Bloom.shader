Shader "STB/NieR/PostEffect/Bloom"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Bloom("Bloom", 2D) = "black" {}
		_LuminanceThreshold("Luminance Threshold", Float) = 0.5
		_BlurRadius("BlurRadius", Float) = 10
		_SampleDown("SampleDown", Float) = 2
	}
	SubShader
	{
		CGINCLUDE

		#include "Lighting.cginc"

		sampler2D _MainTex;
		half4 _MainTex_TexelSize;
		sampler2D _Bloom;
		float _LuminanceThreshold;

		struct v2f {
			float4 pos :SV_POSITION;
			half2 uv : TEXCOORD0;
		};

		v2f vertExtractBright(appdata_img v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			return o;
		}

		fixed4 fragExtractBright(v2f i) : SV_Target{
			fixed4 c = tex2D(_MainTex, i.uv);
			fixed val = clamp(Luminance(c) - _LuminanceThreshold, 0.0, 1.0);
			return c * val;
		}

		struct v2fBloom {
			float4 pos : SV_POSITION;
			half4 uv : TEXCOORD0;
		};

		v2fBloom vertBloom(appdata_img v) {
			v2fBloom o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv.xy = v.texcoord;
			o.uv.zw = v.texcoord;

			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0.0)
				o.uv.w = 1.0 - o.uv.w;
			#endif

			return o;
		}

		fixed4 fragBloom(v2fBloom i) : SV_Target{
			return tex2D(_MainTex, i.uv.xy) + tex2D(_Bloom, i.uv.zw);
		}

		ENDCG

		ZTest Always
		Cull Off
		ZWrite Off

		Pass {
			CGPROGRAM
			#pragma vertex vertExtractBright
			#pragma fragment fragExtractBright
			ENDCG
		}

		UsePass "STB/NieR/PostEffect/GaussBlur/GAUSS_BLUR"

		Pass {
			CGPROGRAM
			#pragma vertex vertBloom
			#pragma fragment fragBloom
			ENDCG
		}
	}
	FallBack Off
}
