Shader "STB/NieR/FadeCircle" {
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,0)
	}
	SubShader 
	{
		Pass {
			Tags { "Queue" = "Transparent" "LightMode" = "ForwardBase" "IgnoreProjector" = "True" "RenderType" = "Tranparent" }
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			fixed4 _Color;

			struct a2v {
				float4 vertex : POSITION;
				float4 texcoord	: TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				SHADOW_COORDS(3)
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
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

				float dist = pow(pow(i.uv.x - 0.5, 2) + pow(i.uv.y - 0.5, 2), 0.5);
				float phase = saturate(dist * 2) * 3.14159;
				float a = (cos(phase) + 1) / 2;

				return fixed4(ambient + diffuse * atten, a);
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
