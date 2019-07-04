Shader "STB/NieR/UnlitTwillCore"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Alpha("Alpha", Range(0,1)) = 1
		_BaseColor("Base Color", Color) = (1,1,1,1)
		_BaseAlpha("Base Alpha", Range(0,1)) = 0.5
		_Count("Count", float) = 8
		_CoreScale("Core Scale", Range(0,1)) = 1
		_ArmorColor("Armor Color", Color) = (1,1,1,1)
		_ArmorAlpha("Armor Alpha", Range(0,1)) = 0
		_ArmorScale("Armor Scale", Range(1,2)) = 1.1
	}
		SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Color;
			fixed _Alpha;
			fixed4 _BaseColor;
			fixed _BaseAlpha;
			float _Count;
			fixed _CoreScale;

			struct a2v {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex * _CoreScale);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float factor = frac((i.uv.y + i.uv.x) * _Count);
				if (abs(factor - 0.5) <= 0.2)
					return fixed4(_Color.xyz, _Color.w * _Alpha);
				else
					return fixed4(_BaseColor.xyz, _BaseColor.w * _BaseAlpha);
			}
			ENDCG
		}

		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster

			fixed _CoreScale;

			struct a2v {
				float4 vertex : POSITION;
			};

			float4 vert(a2v v) : SV_POSITION {
				return UnityObjectToClipPos(v.vertex * _CoreScale);
			}

			half4 frag() : SV_TARGET {
				return 0;
			}

			ENDCG
		}

		Pass
		{
			Name "Armor"
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			CGPROGRAM

			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			float _ArmorScale;
			fixed _ArmorAlpha;
			fixed4 _ArmorColor;

			float4 vert(float4 vertex : POSITION) : SV_POSITION {
				float4 armorPos = vertex * _ArmorScale;
				float4 o = UnityObjectToClipPos(armorPos);
				return o;
			}

			fixed4 frag() : SV_Target{
				return fixed4(_ArmorColor.xyz, _ArmorAlpha);
			}

			ENDCG
		}
	}
}