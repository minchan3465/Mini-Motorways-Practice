using System;
using System.Collections.Generic;
using Motorways.Constants;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering.RenderFeatures
{
	// Token: 0x0200029F RID: 671
	public class HighestMotorwayPass : ScriptableRenderPass
	{
		// Token: 0x060010A2 RID: 4258 RVA: 0x00038A74 File Offset: 0x00036C74
		public HighestMotorwayPass(Shader highestMotorwayShader)
		{
			base.profilingSampler = new ProfilingSampler("HighestMotorwayPass");
			this._profilingSampler = new ProfilingSampler("Highest Motorway Pass");
			base.renderPassEvent = RenderPassEvent.BeforeRendering;
			if (Diagnostics.Verify(highestMotorwayShader != null, "Highest Motorway Shader is null!"))
			{
				this._material = new Material(highestMotorwayShader);
			}
			this._mFilteringSettings = new FilteringSettings(null, LayerConstants.MotorwayMask, uint.MaxValue, 0);
			this._shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
			this._renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
			this._renderTargetHandle = RTHandles.Alloc(new RenderTargetIdentifier(ShaderConstants.HighestMotorwayTexture));
		}

		// Token: 0x060010A3 RID: 4259 RVA: 0x00038B2C File Offset: 0x00036D2C
		~HighestMotorwayPass()
		{
			this.Dispose();
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x00038B58 File Offset: 0x00036D58
		public void Dispose()
		{
			RTHandle renderTargetHandle = this._renderTargetHandle;
			if (renderTargetHandle != null)
			{
				renderTargetHandle.Release();
			}
			this._renderTargetHandle = null;
		}

		// Token: 0x060010A5 RID: 4261 RVA: 0x00038B74 File Offset: 0x00036D74
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			RenderTextureDescriptor blitTargetDescriptor = new RenderTextureDescriptor(renderingData.cameraData.camera.pixelWidth, renderingData.cameraData.camera.pixelHeight, GraphicsFormat.R32_SFloat, 16);
			cmd.GetTemporaryRT(ShaderConstants.HighestMotorwayTexture, blitTargetDescriptor);
			base.ConfigureTarget(this._renderTargetHandle);
			base.ConfigureClear(ClearFlag.All, Color.black);
		}

		// Token: 0x060010A6 RID: 4262 RVA: 0x00038BD0 File Offset: 0x00036DD0
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
			DrawingSettings drawingSettings = base.CreateDrawingSettings(this._shaderTagIdList, ref renderingData, sortingCriteria);
			drawingSettings.overrideMaterial = this._material;
			CommandBuffer cmd = CommandBufferPool.Get();
			using (new ProfilingScope(cmd, this._profilingSampler))
			{
				context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref this._mFilteringSettings, ref this._renderStateBlock);
			}
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}

		// Token: 0x060010A7 RID: 4263 RVA: 0x00038C5C File Offset: 0x00036E5C
		public override void OnCameraCleanup(CommandBuffer cmd)
		{
			base.OnCameraCleanup(cmd);
			cmd.ReleaseTemporaryRT(ShaderConstants.HighestMotorwayTexture);
		}

		// Token: 0x04000EAF RID: 3759
		private readonly ProfilingSampler _profilingSampler;

		// Token: 0x04000EB0 RID: 3760
		private FilteringSettings _mFilteringSettings;

		// Token: 0x04000EB1 RID: 3761
		private RenderStateBlock _renderStateBlock;

		// Token: 0x04000EB2 RID: 3762
		private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();

		// Token: 0x04000EB3 RID: 3763
		private readonly Material _material;

		// Token: 0x04000EB4 RID: 3764
		private RTHandle _renderTargetHandle;
	}
}
