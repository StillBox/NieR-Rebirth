Shader "STB/NieR/Halo" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Cutoff ("Cutoff", Range(0,0.9)) = 0.5
		_Power ("Power", Range(0.5,80)) = 3.0
	}
	SubShader {

		Pass {
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"

			fixed4 _Color;
			float _Cutoff;
			float _Power;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed4 col = _Color;
				float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				float dnv = saturate(abs(dot(i.worldNormal, viewDir)));
				col.a = col.a * pow((dnv - _Cutoff) / (1 - _Cutoff), _Power);
				return col;
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
