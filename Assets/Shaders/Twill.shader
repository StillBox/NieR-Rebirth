Shader "STB/NieR/Twill"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Alpha("Alpha", Range(0,1)) = 1
		_BaseColor("Base Color", Color) = (1,1,1,1)
		_BaseAlpha("Base Alpha", Range(0,1)) = 0.5
		_Count("Count", float) = 8
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
				o.pos = UnityObjectToClipPos(v.vertex);
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
	}
	Fallback Off
}