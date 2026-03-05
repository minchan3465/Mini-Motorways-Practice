Shader "Unlit/Colored Grid" {
	Properties {
		_GridSpacing ("Grid Spacing", Float) = 6
		_GridOffset ("Grid Offset", Vector) = (0,0,0,0)
		_GridHalfLineThickness ("Grid Half Line Thickness", Float) = 0.005
		_GridSmoothing ("Grid Smoothing", Float) = 0.001
		_BackgroundColor ("Background Colour", Vector) = (1,0.5,1,1)
		_GridColor ("Grid Colour", Vector) = (0.5,1,1,1)
		_DropShadowColor ("Drop Shadow Colour", Vector) = (0,0,0,1)
		_ShadowTex ("Drop Shadow", 2D) = "white" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
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

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
}