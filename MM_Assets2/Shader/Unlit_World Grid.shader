Shader "Unlit/World Grid" {
	Properties {
		_GridSpacing ("Grid Spacing", Float) = 10
		_GridHalfLineThickness ("Grid Half Line Thickness", Float) = 0.1
		_GridSmoothing ("Grid Smoothing", Float) = 0.03
		_Color ("Colour", Vector) = (0.5,1,1,1)
		_CityLimitsSpacing ("City Limits Spacing", Float) = 1
		_CityLimitsHalfLineThickness ("City Limits Half Line Thickness", Float) = 0.1
		_CityLimitsSmoothing ("City Limits Smoothing", Float) = 0
		_CityLimitsAlpha ("City Limits Alpha", Float) = 0.8
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 _Color;

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return _Color; // RGBA
			}

			ENDHLSL
		}
	}
}