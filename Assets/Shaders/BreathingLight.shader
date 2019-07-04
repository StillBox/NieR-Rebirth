Shader "STB/NieR/BreathingLight" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Fade ("Fade", Float) = 1
		_Power ("Power", Range(0.01,3)) = 3
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		
		Pass {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Color;
			float _Fade;
			float _Power;

			struct a2v {
				float4 vertex : POSITION;
				float4 texcoord	: TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed4 c = _Color;
				float dist = pow(pow(i.uv.x - 0.5, 2) + pow(i.uv.y - 0.5, 2), 0.5);
				float phase = saturate(dist * 2) * 3.14159;
				c.a = (1 - pow(1 - (cos(phase) + 1) / 2, _Power)) * _Fade;
				return c;
			}

			ENDCG
		}
	}

	FallBack "Diffuse"
}
