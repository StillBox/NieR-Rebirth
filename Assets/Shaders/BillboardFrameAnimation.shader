Shader "STB/NieR/BillboardFrameAnimation"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_HorizontalCount ("Horizontal Count", Float) = 1
		_VerticalCount ("Vertical Count", Float) = 1
		_CurrentFrame ("Current Frame", Float) = 0
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "lighting.cginc"

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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _HorizontalCount;
			float _VerticalCount;
			float _CurrentFrame;

			v2f vert (a2v v)
			{
				v2f o;
				float3 center = float3(0, 0, 0);
				float3 upDir = normalize(UNITY_MATRIX_V[1].xyz);
				float3 rightDir = normalize(UNITY_MATRIX_V[0].xyz);
				float3 normalDir = normalize(UNITY_MATRIX_V[2].xyz);
				float3 centerOffs = v.vertex.xyz - center;
				float3 localPos = center + rightDir * centerOffs.x + upDir * centerOffs.y + normalDir * centerOffs.z;
				o.pos = UnityObjectToClipPos(float4(localPos, 1));
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv;
				fixed4 col;
				if (_CurrentFrame < _HorizontalCount *_VerticalCount)
				{
					uv.x = 1 / _HorizontalCount * (_CurrentFrame - floor(_CurrentFrame / _HorizontalCount) * _HorizontalCount + i.uv.x);
					uv.y = 1 / _VerticalCount * (_VerticalCount - 1 - floor(_CurrentFrame / _HorizontalCount) + i.uv.y);
					col = tex2D(_MainTex, uv);
				}
				else
				{
					col = fixed4(0, 0, 0, 0);
				}
				return col;
			}
			ENDCG
		}
	}
}
