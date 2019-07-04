Shader "STB/NieR/HackSpace" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Offset ("Offset", Float) = 0.01
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass {
			ZWrite Off
			Blend One One

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Color;
			float _Offset;

			float4 vert(float4 vertex : POSITION) : SV_POSITION {
				float4 pos = UnityObjectToClipPos(vertex);
				pos.x += pos.x * _Offset * (1 + pos.z);
				pos.y += pos.y * _Offset * (1 + pos.z);
				return pos;
			}

			fixed4 frag() : SV_Target {
				return fixed4(_Color.x, 0, _Color.z, 1);
			}

			ENDCG
		}

		Pass {
			ZWrite Off
			Blend One One

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Color;
			float _Offset;

			float4 vert(float4 vertex : POSITION) : SV_POSITION {
				float4 pos = UnityObjectToClipPos(vertex);
				pos.x -= pos.x * _Offset * (1 + pos.z);
				pos.y -= pos.y * _Offset * (1 + pos.z);
				return pos;
			}

			fixed4 frag() : SV_Target {
				return fixed4(0, _Color.y, 0, 1);
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
