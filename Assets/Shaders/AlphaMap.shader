Shader "STB/NieR/AlphaMap" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Alpha Map", 2D) = "white" {}
		_Scale ("Scale", Range(0,1)) = 1
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			Cull Off
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

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Scale;

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 mapCol = tex2D(_MainTex, i.uv);
				fixed alpha = mapCol.x * _Scale;
				return fixed4(_Color.xyz, _Color.w * alpha);
			}
			ENDCG
		}
	}
}
