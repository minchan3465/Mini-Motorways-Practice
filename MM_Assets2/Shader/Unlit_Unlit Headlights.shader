Shader "Unlit/Unlit Headlights" {
	Properties {
		[MaterialToggle] _ShouldRenderBeams ("Should Render Beams", Float) = 0
		_HalfBeamWidth ("Half Beam Width", Float) = 0.01
		_BeamLength ("Beam Length", Float) = 0.01
		_CircleOffset ("Circle Offset", Float) = 0.01
		_CircleRadius ("Circle Radius", Float) = 0.01
		_LeftCutPoint ("Left Cut Point", Vector) = (0,0,0,1)
		_RightCutPoint ("Right Cut Point", Vector) = (0,0,0,1)
		_LeftHeadlightPosition ("Left Headlight Position", Vector) = (0,0,0,1)
		_Opacity ("Opacity", Float) = 0.8
		_Intensity ("Intensity", Float) = 1
		_Color ("Main Color", Vector) = (1,1,1,1)
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