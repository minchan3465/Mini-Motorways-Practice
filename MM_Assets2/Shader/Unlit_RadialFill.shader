Shader "Unlit/RadialFill" {
	Properties {
		_InnerColor ("Inner Color", Vector) = (1,1,1,1)
		_OuterColor ("Outer Color", Vector) = (1,1,1,1)
		_Alpha ("Alpha", Range(0, 1)) = 1
		_CircleSize ("Circle Size", Range(0, 2)) = 2
		_InnerCircleSize ("Inner Circle Size", Range(0, 2)) = 0
		_CircleFade ("Circle Fade", Range(0, 0.5)) = 0
		_InnerProgress ("Inner Progress", Range(0, 1.1)) = 0
		_OuterProgress ("Outer Progress", Range(0, 1.1)) = 0
		_FillOffset ("Fill Offset", Range(0, 1)) = 0
		_MainTex ("Texture", 2D) = "white" {}
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
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy);
			}

			ENDHLSL
		}
	}
}