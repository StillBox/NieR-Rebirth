Shader "STB/NieR/BlackBox" {
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_CoreScale("Core Scale", Range(0,1)) = 1
		_MainTex("Alpha Map", 2D) = "white" {}
		_EdgeColor("EdgeColor", Color) = (0,0,0,0)
		_DarkColor("DarkColor", Color) = (1,1,1,1)
		_LightColor("LightColor", Color) = (1,1,1,1)
		_LightPos("LightPosition", Range(0,2)) = 0
		_LightWidth("LightWidth", Range(0,1)) = 0
	}
	SubShader
	{
		UsePass "STB/NieR/Core/ForwardBase"
		UsePass "STB/NieR/Core/ForwardAdd"

		Pass
		{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Cull Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "lighting.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			fixed4 _EdgeColor;
			fixed4 _DarkColor;
			fixed4 _LightColor;
			float _LightPos;
			float _LightWidth;
			float totalWeight;

			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			half GetWeight(float x, float y, float rho)
			{
				return exp(-(x * x + y * y) / (2 * rho * rho));
			}

			fixed4 GaussBlur(v2f i, float radius, float sampleDown)
			{
				half rho = radius / 3;

				fixed4 finalCol = 0;
				half totalWeight = 0;
				for (int x = -radius; x <= radius; x += sampleDown)
				{
					for (int y = -radius; y <= radius; y += sampleDown)
					{
						half weight = GetWeight(x, y, rho);

						fixed4 col = tex2D(_MainTex, i.uv + float2(x, y) * _MainTex_TexelSize.xy);
						finalCol += col * weight;
						totalWeight += weight;
					}
				}
				return finalCol / totalWeight;
			}

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 mapCol = tex2D(_MainTex, i.uv);
				fixed alpha = Luminance(mapCol);

				fixed4 col = _DarkColor;
				float factor = i.uv.x - 0.1 * frac(i.uv.y) + 0.05;
				float dist = factor - _LightPos;
				if (dist > 1) dist -= 2;
				if (dist < -1) dist += 2;

				float distRate = dist / _LightWidth * 2;
				if (abs(distRate) <= 1)
				{
					distRate -= 0.7;

					float phase = distRate < 0 ? distRate / 1.7 : distRate / 0.3;
					float highLight = 0.6 * (1 + cos(3.1415 * phase));
					col = _DarkColor + (_LightColor - _DarkColor) * highLight;

					if (phase > -0.6)
					{
						float haloPhase = phase / (phase > 0 ? 1 : 0.6);
						float radius = 12 * (1 + cos(3.1415 * haloPhase));
						fixed4 blurCol = GaussBlur(i, radius, 2);
						alpha = max(alpha, Luminance(blurCol));
					}

					if (phase > 0)
					{
						float radius = 12 * (1 + cos(3.1415 * phase));
					}
					else if (phase > -0.6)
					{
						float radius = 12 * (1 + cos(3.1415 * phase / 0.6));
						fixed4 blurCol = GaussBlur(i, radius, 2);
						alpha = max(alpha, Luminance(blurCol));
					}
				}
				return fixed4(col.xyz, alpha);
			}

			ENDCG
		}
	}
	FallBack "Standard"
}