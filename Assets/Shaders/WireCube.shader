Shader "STB/NieR/WireCube"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_Alpha ("Alpha", Float) = 1
		_Inverse ("Inverse", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag

			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD1;
			};

			fixed4 _Color;
			fixed _Alpha;
			fixed _Inverse;

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = _Color;
				float dist = min(min(i.uv.x, 1 - i.uv.x), min(i.uv.y, 1 - i.uv.y));
				float factor = pow(saturate(1 - dist * 4), 2);
				factor = 0.5 - (factor - 0.5) * (_Inverse - 0.5) * 2;
				col.a = col.a * _Alpha * factor;
				return col;
			}
			ENDCG
		}
		/*
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				SHADOW_COORDS(4)
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				float2 uv : TEXCOORD1;
			};

			fixed4 _Color;
			fixed _Alpha;
			fixed _Inverse;
			
			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color * _LightColor0;
				float dist = min(min(i.uv.x, 1 - i.uv.x), min(i.uv.y, 1 - i.uv.y));
				float factor = pow(saturate(1 - dist * 4), 2);
				factor = 0.5 - (factor - 0.5) * (_Inverse - 0.5) * 2;
				col.a = col.a * _Alpha * factor;
				return col;
			}
			ENDCG
		}

		Pass
		{
			Tags { "LightMode" = "ForwardAdd" }

			CGPROGRAM
			#pragma multi_compile_fwdadd_fullshadows
			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				SHADOW_COORDS(4)
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				float2 uv : TEXCOORD1;
			};

			fixed4 _Color;
			fixed _Alpha;
			fixed _Inverse;

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = _Color * _LightColor0;
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
				col = col * atten;

				float dist = min(min(i.uv.x, 1 - i.uv.x), min(i.uv.y, 1 - i.uv.y));
				float factor = pow(saturate(1 - dist * 4), 2);
				factor = 0.5 - (factor - 0.5) * (_Inverse - 0.5) * 2;
				col.a = col.a * _Alpha * factor;

				return col;
			}
			ENDCG
		}
		*/
	}
}
