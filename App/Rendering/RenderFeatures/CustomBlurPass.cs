using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering.RenderFeatures
{
	// Token: 0x0200029A RID: 666
	public class CustomBlurPass : ScriptableRenderPass
	{
		// Token: 0x0600108F RID: 4239 RVA: 0x00038190 File Offset: 0x00036390
		public CustomBlurPass(Material blurMaterial)
		{
			this._blurMaterial = blurMaterial;
			base.renderPassEvent = RenderPassEvent.AfterRendering;
		}

		// Token: 0x06001090 RID: 4240 RVA: 0x000381F7 File Offset: 0x000363F7
		public void Setup(float strength, float levelRange, float levelOffset)
		{
			this._blurMaterial.SetFloat("_Strength", strength);
			this._blurMaterial.SetFloat("_LevelsRange", levelRange);
			this._blurMaterial.SetFloat("_LevelsOffset", levelOffset);
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x0003822C File Offset: 0x0003642C
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			cmd.GetTemporaryRT(this._originalTextureId, cameraTextureDescriptor, FilterMode.Bilinear);
			Vector2Int pingPongTextureSize = Vector2Int.zero;
			if (cameraTextureDescriptor.width > cameraTextureDescriptor.height)
			{
				pingPongTextureSize.x = this._pingPongTextureLongestSide;
				pingPongTextureSize.y = (int)((float)cameraTextureDescriptor.height / (float)cameraTextureDescriptor.width * (float)pingPongTextureSize.x);
			}
			else
			{
				pingPongTextureSize.x = (int)((float)cameraTextureDescriptor.width / (float)cameraTextureDescriptor.height * (float)pingPongTextureSize.y);
				pingPongTextureSize.y = this._pingPongTextureLongestSide;
			}
			cmd.GetTemporaryRT(this._pingTextureId, pingPongTextureSize.x, pingPongTextureSize.y, 0, FilterMode.Bilinear);
			cmd.GetTemporaryRT(this._pongTextureId, pingPongTextureSize.x, pingPongTextureSize.y, 0, FilterMode.Bilinear);
			if (this.TapCount == 5)
			{
				float[] offsets = new float[]
				{
					1.3846154f,
					3.2307692f
				};
				for (int i = 0; i < offsets.Length; i++)
				{
					this._blurMaterial.SetFloat(string.Format("_OffsetX{0}", i + 1), offsets[i] / (float)pingPongTextureSize.x);
					this._blurMaterial.SetFloat(string.Format("_OffsetY{0}", i + 1), offsets[i] / (float)pingPongTextureSize.y);
				}
			}
			else if (this.TapCount == 3)
			{
				float offset = 1.2857143f;
				this._blurMaterial.SetFloat("_OffsetX", offset / (float)pingPongTextureSize.x);
				this._blurMaterial.SetFloat("_OffsetY", offset / (float)pingPongTextureSize.y);
			}
			this._blurMaterial.SetFloat("_Weight0", 0.22702703f);
			this._blurMaterial.SetFloat("_Weight1", 0.31621623f);
			this._blurMaterial.SetFloat("_Weight2", 0.07027027f);
			this._passCount = ((this.TapCount == 3) ? 2 : Mathf.Clamp(cameraTextureDescriptor.width / this._pingPongTextureLongestSide, 1, 4));
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x00038420 File Offset: 0x00036620
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer cmd = CommandBufferPool.Get("Custom Blur Pass");
			cmd.Clear();
			RenderTargetIdentifier sourceTextureId = renderingData.cameraData.renderer.cameraColorTargetHandle;
			cmd.BeginSample("Source to Original");
			cmd.Blit(sourceTextureId, this._originalTextureId);
			cmd.EndSample("Source to Original");
			cmd.BeginSample("Source to Ping");
			cmd.Blit(sourceTextureId, this._pingTextureId);
			cmd.EndSample("Source to Ping");
			for (int passIndex = 0; passIndex < this._passCount; passIndex++)
			{
				string sampleName = string.Format("Pass {0}", passIndex);
				cmd.BeginSample(sampleName);
				cmd.BeginSample("Ping to Pong");
				cmd.Blit(this._pingTextureId, this._pongTextureId, this._blurMaterial, 0);
				cmd.EndSample("Ping to Pong");
				if (passIndex < this._passCount - 1)
				{
					cmd.BeginSample("Pong to Ping");
					cmd.Blit(this._pongTextureId, this._pingTextureId, this._blurMaterial, 1);
					cmd.EndSample("Pong to Ping");
				}
				else
				{
					cmd.BeginSample("Pong to Source");
					cmd.Blit(this._pongTextureId, sourceTextureId, this._blurMaterial, 2);
					cmd.EndSample("Pong to Source");
				}
				cmd.EndSample(sampleName);
			}
			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
			CommandBufferPool.Release(cmd);
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x0003859C File Offset: 0x0003679C
		public override void FrameCleanup(CommandBuffer cmd)
		{
			base.FrameCleanup(cmd);
			cmd.ReleaseTemporaryRT(this._originalTextureId);
			cmd.ReleaseTemporaryRT(this._pingTextureId);
			cmd.ReleaseTemporaryRT(this._pongTextureId);
		}

		// Token: 0x04000E8E RID: 3726
		private readonly Material _blurMaterial;

		// Token: 0x04000E8F RID: 3727
		private int _passCount;

		// Token: 0x04000E90 RID: 3728
		private readonly int _originalTextureId = Shader.PropertyToID("_Original");

		// Token: 0x04000E91 RID: 3729
		private readonly int _pongTextureId = Shader.PropertyToID("_Pong");

		// Token: 0x04000E92 RID: 3730
		private readonly int _pingTextureId = Shader.PropertyToID("_Ping");

		// Token: 0x04000E93 RID: 3731
		private readonly int _pingPongTextureLongestSide = 512;

		// Token: 0x04000E94 RID: 3732
		private int TapCount = 5;
	}
}
