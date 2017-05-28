// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "R&D/Additive Circle" {
	Properties{
		_Strength("Strength", Range(0.0, 4.0)) = 1.0
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Radius("Radius", Range(0.0, 1.0)) = 0.5
		_Width("Width", Range(0.0, 0.5)) = 0.1
		_Crispness("Crispness", Range(0.0, 1.0)) = 0.5
	}

	SubShader{
		Tags{ "Queue" = "Transparent" }
		Pass{
			Blend One One
			ZWrite Off
			ZTest Always

			CGPROGRAM 

			#pragma vertex vert 
			#pragma fragment frag

			uniform float _Strength;
			uniform float4 _Color;
			uniform float _Radius;
			uniform float _Width;
			uniform float _Crispness;

			struct VERTEXOUT {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			VERTEXOUT vert(float4 vertexPos : POSITION, float2 uv : TEXCOORD0)
			{
				VERTEXOUT output;

				output.pos = UnityObjectToClipPos(vertexPos);
				output.uv = uv;

				return output;
			}

			float4 frag(VERTEXOUT input) : COLOR
			{
				float actualCrispness = _Crispness * 4.0;
				actualCrispness *= actualCrispness;

				float ob = 0.5 + actualCrispness * (_Radius + _Width / 2);
				float ib = 0.5 - actualCrispness * (_Radius - _Width / 2);
				float r = length(input.uv - 0.5) * 2;

				float os = -actualCrispness * r + ob;
				float is = actualCrispness * r + ib;

				return saturate(min(os, is) * _Strength) * _Color;
			}

			ENDCG 
		}
	}
}