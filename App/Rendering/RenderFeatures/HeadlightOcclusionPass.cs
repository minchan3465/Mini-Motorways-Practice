using System;
using System.Collections.Generic;
using Motorways.Constants;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering.RenderFeatures
{
	// Token: 0x0200029D RID: 669
	public class HeadlightOcclusionPass : ScriptableRenderPass
	{
		// Token: 0x06001098 RID: 4248 RVA: 0x000386AC File Offset: 0x000368AC
		public HeadlightOcclusionPass(Shader headlightOcclusionShader)
		{
			base.profilingSampler = new ProfilingSampler("HighestMotorwayPass");
			this._headlightOcclusionProfilingSampler = new ProfilingSampler("Headlight Occlusion Pass");
			base.renderPassEvent = RenderPassEvent.BeforeRendering;
			if (Diagnostics.Verify(headlightOcclusionShader != null))
			{
				this._headlightOcclusionMaterial = new Material(headlightOcclusionShader)
				{
					enableInstancing = true
				};
			}
			this._headlightOcclusionFilteringSettings = new FilteringSettings(null, LayerConstants.HeadlightOcclusionLayerMask, uint.MaxValue, 0);
			this._mountainFilteringSettings = new FilteringSettings(null, LayerConstants.HeadlightOcclusionMountainLayerNameLayerMask, uint.MaxValue, 0);
			this._shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
			this._renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
			this._renderTargetHandle = RTHandles.Alloc(new RenderTargetIdentifier(ShaderConstants.HeadlightOcclusionTexture));
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x00038780 File Offset: 0x00036980
		~HeadlightOcclusionPass()
		{
			this.Dispose();
		}

		// Token: 0x0600109A RID: 4250 RVA: 0x000387AC File Offset: 0x000369AC
		public void Dispose()
		{
			RTHandle renderTargetHandle = this._renderTargetHandle;
			if (renderTargetHandle != null)
			{
				renderTargetHandle.Release();
			}
			this._renderTargetHandle = null;
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x000387C8 File Offset: 0x000369C8
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			RenderTextureDescriptor blitTargetDescriptor = new RenderTextureDescriptor(renderingData.cameraData.camera.pixelWidth, renderingData.cameraData.camera.pixelHeight, GraphicsFormat.R32_SFloat, 0);
			cmd.GetTemporaryRT(ShaderConstants.HeadlightOcclusionTexture, blitTargetDescriptor);
			base.ConfigureTarget(this._renderTargetHandle);
			base.ConfigureClear(ClearFlag.All, Color.black);
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x00038824 File Offset: 0x00036A24
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
			DrawingSettings drawingSettings = base.CreateDrawingSettings(this._shaderTagIdList, ref renderingData, sortingCriteria);
			drawingSettings.overrideMaterial = this._headlightOcclusionMaterial;
			CommandBuffer cmd = CommandBufferPool.Get();
			using (new ProfilingScope(cmd, this._headlightOcclusionProfilingSampler))
			{
				context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref this._headlightOcclusionFilteringSettings, ref this._renderStateBlock);
				this._headlightOcclusionMaterial.SetFloat(ShaderConstants.MotorwayIdShaderId, 0f);
				this._headlightOcclusionMaterial.SetFloat(ShaderConstants.HeadlightOcclusionTypeId, ShaderConstants.HeadlightNonVehicleOcclusionTypeId);
				context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref this._mountainFilteringSettings, ref this._renderStateBlock);
			}
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x000388F4 File Offset: 0x00036AF4
		public override void OnCameraCleanup(CommandBuffer cmd)
		{
			base.OnCameraCleanup(cmd);
			cmd.ReleaseTemporaryRT(ShaderConstants.HeadlightOcclusionTexture);
		}

		// Token: 0x04000EA3 RID: 3747
		private readonly ProfilingSampler _headlightOcclusionProfilingSampler;

		// Token: 0x04000EA4 RID: 3748
		private FilteringSettings _headlightOcclusionFilteringSettings;

		// Token: 0x04000EA5 RID: 3749
		private FilteringSettings _mountainFilteringSettings;

		// Token: 0x04000EA6 RID: 3750
		private RenderStateBlock _renderStateBlock;

		// Token: 0x04000EA7 RID: 3751
		private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();

		// Token: 0x04000EA8 RID: 3752
		private readonly Material _headlightOcclusionMaterial;

		// Token: 0x04000EA9 RID: 3753
		private RTHandle _renderTargetHandle;
	}
}
