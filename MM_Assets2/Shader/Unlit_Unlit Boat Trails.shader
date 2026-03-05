Shader "Unlit/Unlit Boat Trails" {
	Properties {
		[MaterialToggle] _ShouldRenderTrails ("Should Render Trails", Float) = 0
		_OpacityThreshold ("Opacity Threshold", Float) = 0
		_WaveWidth ("Wave Width", Float) = 0
		_WaveLength ("Wave Length", Float) = 0
		_TrailTime ("Trail Time", Float) = 0
		_TrailTimeEnd ("Trail Time End", Float) = 0
		_OverallOpacity ("Overall Opacity", Float) = 1
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