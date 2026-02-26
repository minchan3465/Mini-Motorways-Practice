using System;
using System.Collections.Generic;
using Motorways.Constants;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering.RenderFeatures
{
	// Token: 0x020002A4 RID: 676
	public class ShadowTypeRenderPass : ScriptableRenderPass
	{
		// Token: 0x060010B4 RID: 4276 RVA: 0x00038F6C File Offset: 0x0003716C
		public ShadowTypeRenderPass(Shader shadowTypeShader)
		{
			base.profilingSampler = new ProfilingSampler("HighestMotorwayPass");
			this._profilingSampler = new ProfilingSampler("Shadow Type Pass");
			base.renderPassEvent = RenderPassEvent.BeforeRendering;
			if (Diagnostics.Verify(shadowTypeShader != null, "Highest Motorway Shader is null!"))
			{
				this._material = new Material(shadowTypeShader);
			}
			this._mFilteringSettings = new FilteringSettings(null, LayerConstants.ShadowMask, uint.MaxValue, 0);
			this._shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
			this._renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
			this._renderTargetHandle = RTHandles.Alloc(new RenderTargetIdentifier(ShaderConstants.ShadowTypeTexture));
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x00039024 File Offset: 0x00037224
		~ShadowTypeRenderPass()
		{
			this.Dispose();
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x00039050 File Offset: 0x00037250
		public void Dispose()
		{
			RTHandle renderTargetHandle = this._renderTargetHandle;
			if (renderTargetHandle != null)
			{
				renderTargetHandle.Release();
			}
			this._renderTargetHandle = null;
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x0003906C File Offset: 0x0003726C
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			RenderTextureDescriptor blitTargetDescriptor = new RenderTextureDescriptor(renderingData.cameraData.camera.pixelWidth, renderingData.cameraData.camera.pixelHeight, GraphicsFormat.R32_SFloat, 0);
			cmd.GetTemporaryRT(ShaderConstants.ShadowTypeTexture, blitTargetDescriptor);
			base.ConfigureTarget(this._renderTargetHandle);
			base.ConfigureClear(ClearFlag.All, Color.black);
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x000390C8 File Offset: 0x000372C8
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			SortingCriteria sortingCriteria = SortingCriteria.None;
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

		// Token: 0x060010B9 RID: 4281 RVA: 0x00039150 File Offset: 0x00037350
		public override void OnCameraCleanup(CommandBuffer cmd)
		{
			base.OnCameraCleanup(cmd);
			cmd.ReleaseTemporaryRT(ShaderConstants.ShadowTypeTexture);
		}

		// Token: 0x04000EC7 RID: 3783
		private readonly ProfilingSampler _profilingSampler;

		// Token: 0x04000EC8 RID: 3784
		private FilteringSettings _mFilteringSettings;

		// Token: 0x04000EC9 RID: 3785
		private RenderStateBlock _renderStateBlock;

		// Token: 0x04000ECA RID: 3786
		private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();

		// Token: 0x04000ECB RID: 3787
		private readonly Material _material;

		// Token: 0x04000ECC RID: 3788
		private RTHandle _renderTargetHandle;

		// Token: 0x020002A5 RID: 677
		public enum ShadowType
		{
			// Token: 0x04000ECE RID: 3790
			Destination = 1,
			// Token: 0x04000ECF RID: 3791
			House,
			// Token: 0x04000ED0 RID: 3792
			Tree,
			// Token: 0x04000ED1 RID: 3793
			Motorway,
			// Token: 0x04000ED2 RID: 3794
			Mountain
		}
	}
}
