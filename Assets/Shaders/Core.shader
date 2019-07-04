Shader "STB/NieR/Core" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_CoreScale("Core Scale", Range(0,1)) = 1
		_ArmorColor ("Armor Color", Color) = (1,1,1,1)
		_ArmorAlpha ("Armor Alpha", Range(0,1)) = 0	
		_ArmorScale ("Armor Scale", Range(1,2)) = 1.1
	}
	SubShader {
		Pass {
			Name "ForwardBase"
			Tags { "LightMode"="ForwardBase" "IgnoreProjector" = "True" "RenderType" = "Opaque" }

			CGPROGRAM

			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			fixed4 _Color;
			fixed _CoreScale;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
	
			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				SHADOW_COORDS(2)
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex * _CoreScale);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				TRANSFER_SHADOW(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				fixed3 albedo = _Color.rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

				float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				float3 worldNormal = normalize(i.worldNormal);
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, lightDir));

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				return fixed4(ambient + diffuse * atten, 1);
			}

			ENDCG
		}

		Pass {
			Name "ForwardAdd"
			Tags { "LightMode" = "ForwardAdd" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
			Blend One One
			ZWrite Off

			CGPROGRAM

			#pragma multi_compile_fwdadd_fullshadows
			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			fixed4 _Color;
			fixed _CoreScale;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				SHADOW_COORDS(2)
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex * _CoreScale);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				TRANSFER_SHADOW(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				fixed3 albedo = _Color.rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

				float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				float3 worldNormal = normalize(i.worldNormal);
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, lightDir));

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				return fixed4(diffuse * atten, 1);
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

		Pass{
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
