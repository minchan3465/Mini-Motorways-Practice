using System;
using Motorways.Constants;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering.RenderFeatures.OrderedDither
{
	// Token: 0x020002A6 RID: 678
	public class CustomVignettePass : ScriptableRenderPass
	{
		// Token: 0x060010BA RID: 4282 RVA: 0x00039164 File Offset: 0x00037364
		public CustomVignettePass(CustomPostProcessRenderFeature.FeatureSettings settings)
		{
			this._settings = settings;
			this._profilingSampler = new ProfilingSampler("Vignette Pass");
			base.renderPassEvent = RenderPassEvent.AfterRendering;
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x0003919E File Offset: 0x0003739E
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			base.Configure(cmd, cameraTextureDescriptor);
			cmd.GetTemporaryRT(this._temporaryVignetteTextureId, cameraTextureDescriptor, FilterMode.Bilinear);
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x000391B8 File Offset: 0x000373B8
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer cmd = CommandBufferPool.Get();
			using (new ProfilingScope(cmd, this._profilingSampler))
			{
				if (CustomVignettePass.Quad != null)
				{
					context.ExecuteCommandBuffer(cmd);
					cmd.Clear();
					cmd.SetRenderTarget(this._temporaryVignetteTextureId);
					this._settings.vignetteMaterial.SetFloat(ShaderConstants.Blend, this._settings.tintBlend);
					this._settings.vignetteMaterial.SetTexture(ShaderConstants.TintTex, this._settings.tintTexture);
					this._settings.vignetteMaterial.SetTexture(ShaderConstants.PaperTex, this._settings.paperTexture);
					this._settings.vignetteMaterial.SetColor(ShaderConstants.VignetteColor, this._settings.color);
					this._settings.vignetteMaterial.SetVector(ShaderConstants.VignetteCenter, this._settings.center);
					float roundness = (1f - this._settings.roundness) * 6f + this._settings.roundness;
					this._settings.vignetteMaterial.SetVector(ShaderConstants.VignetteSettings, new Vector4(3f * this._settings.intensity, 5f * this._settings.smoothness, roundness, this._settings.rounded ? 1f : 0f));
					cmd.DrawMesh(CustomVignettePass.Quad, Matrix4x4.identity, this._settings.vignetteMaterial);
					cmd.Blit(this._temporaryVignetteTextureId, renderingData.cameraData.renderer.cameraColorTargetHandle.nameID);
				}
			}
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x000393A4 File Offset: 0x000375A4
		public override void FrameCleanup(CommandBuffer cmd)
		{
			base.OnCameraCleanup(cmd);
			cmd.ReleaseTemporaryRT(this._temporaryVignetteTextureId);
		}

		// Token: 0x04000ED3 RID: 3795
		private const float IntensityMultiplier = 3f;

		// Token: 0x04000ED4 RID: 3796
		private const float SmoothnessMultiplier = 5f;

		// Token: 0x04000ED5 RID: 3797
		private const float RoundnessMultiplier = 6f;

		// Token: 0x04000ED6 RID: 3798
		private readonly int _temporaryVignetteTextureId = Shader.PropertyToID("_TemporaryVignetteTexture");

		// Token: 0x04000ED7 RID: 3799
		private static readonly Mesh Quad = new Mesh
		{
			vertices = new Vector3[]
			{
				new Vector3(1f, 1f, 0f),
				new Vector3(-1f, -1f, 0f),
				new Vector3(-1f, 1f, 0f),
				new Vector3(1f, -1f, 0f)
			},
			triangles = new int[]
			{
				1,
				0,
				2,
				1,
				3,
				0
			},
			uv = new Vector2[]
			{
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(0f, 0f),
				new Vector2(1f, 1f)
			}
		};

		// Token: 0x04000ED8 RID: 3800
		private readonly CustomPostProcessRenderFeature.FeatureSettings _settings;

		// Token: 0x04000ED9 RID: 3801
		private readonly ProfilingSampler _profilingSampler;
	}
}
