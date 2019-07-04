Shader "STB/NieR/PostEffect/GaussBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BlurRadius("BlurRadius", Float) = 10
		_SampleDown("SampleDown", Float) = 2
	}
	SubShader
	{
		ZTest Always
		Cull Off
		ZWrite Off

		Pass
		{
			NAME "GAUSS_BLUR"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			float _BlurRadius;
			float _SampleDown;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			half GetWeight(float x, float y, float rho)
			{
				return exp(-(x * x + y * y) / (2 * rho * rho));
			}

			fixed4 GaussBlur(v2f i, float radius)
			{
				half rho = radius / 3;

				fixed4 finalCol = 0;
				half totalWeight = 0;
				for (int x = -radius; x <= radius; x += _SampleDown)
				{
					for (int y = -radius; y <= radius; y += _SampleDown)
					{
						half weight = GetWeight(x, y, rho);

						fixed4 col = tex2D(_MainTex, i.uv + float2(x, y) * _MainTex_TexelSize.xy);
						finalCol += col * weight;
						totalWeight += weight;
					}
				}
				return finalCol / totalWeight;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				if (_BlurRadius == 0) return tex2D(_MainTex, i.uv);
				else return GaussBlur(i, _BlurRadius);
			}
			ENDCG
		}
	}
}
