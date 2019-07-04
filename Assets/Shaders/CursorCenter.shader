Shader "STB/NieR/CursorCenter"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Alpha("Alpha", Range(0,1)) = 1
	}
		SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			Cull Off
			ZTest Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Color;
			fixed _Alpha;

			float4 vert(float4 vertex : POSITION) : SV_POSITION
			{
				float4 pos = UnityObjectToClipPos(vertex);
				return pos;
			}

			fixed4 frag() : SV_Target
			{
				fixed4 col = _Color;
				col.a = _Alpha;
				return col;
			}
			ENDCG
		}
	}
}
