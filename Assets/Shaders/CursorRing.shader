Shader "STB/NieR/CursorRing"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Alpha("Alpha", Range(0,1)) = 1
		_Radius("Radius", Range(0,0.5)) = 0.5
		_Width("Width", Range(0,0.2)) = 0.1
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			Cull Off
			ZWrite Off
			ZTest Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Color;
			fixed _Alpha;
			float _Radius;
			float _Width;

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
						
			fixed4 frag (v2f i) : SV_Target {
				fixed4 c = fixed4(_Color.xyz, 0);
				float radius = pow(pow(i.uv.x - 0.5, 2) + pow(i.uv.y - 0.5, 2), 0.5);
				float dist = abs(radius - _Radius);
				if (dist <= 0.5 * _Width)
				{
					float angle = acos((i.uv.y - 0.5) / radius);
					float range = 3.1416 / 3 * (1 - _Radius);
					if (angle < range || (angle > 2.0944 - range && angle < 2.0944 + range))
						c.w = _Color.w * _Alpha;
					if (dist > 0.3 * _Width)
						c.w *= (0.5 - dist / _Width) / 0.2;
				}
				return c;
			}
			ENDCG
		}
	}
	FallBack "Standard"
}
