using System;
using System.Collections.Generic;
using Motorways.Constants;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering.RenderFeatures
{
	// Token: 0x0200029E RID: 670
	public class HeadlightRenderPass : ScriptableRenderPass
	{
		// Token: 0x0600109E RID: 4254 RVA: 0x00038908 File Offset: 0x00036B08
		public HeadlightRenderPass(Material headlightMaterial)
		{
			base.profilingSampler = new ProfilingSampler("HeadlightRenderPass");
			this._profilingSampler = new ProfilingSampler("Headlight Pass");
			base.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
			this._material = headlightMaterial;
			this._mFilteringSettings = new FilteringSettings(null, LayerConstants.HeadlightMask, uint.MaxValue, 0);
			this._shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
			this._renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
		}

		// Token: 0x0600109F RID: 4255 RVA: 0x00038994 File Offset: 0x00036B94
		~HeadlightRenderPass()
		{
			this.Dispose();
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Dispose()
		{
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x000389C0 File Offset: 0x00036BC0
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			ScriptableCullingParameters cullingParameters;
			renderingData.cameraData.camera.TryGetCullingParameters(out cullingParameters);
			cullingParameters.cullingMask = (uint)LayerConstants.HeadlightMask;
			CullingResults cullResults = context.Cull(ref cullingParameters);
			SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
			DrawingSettings drawingSettings = base.CreateDrawingSettings(this._shaderTagIdList, ref renderingData, sortingCriteria);
			drawingSettings.overrideMaterial = this._material;
			CommandBuffer cmd = CommandBufferPool.Get();
			using (new ProfilingScope(cmd, this._profilingSampler))
			{
				context.DrawRenderers(cullResults, ref drawingSettings, ref this._mFilteringSettings, ref this._renderStateBlock);
			}
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}

		// Token: 0x04000EAA RID: 3754
		private readonly ProfilingSampler _profilingSampler;

		// Token: 0x04000EAB RID: 3755
		private FilteringSettings _mFilteringSettings;

		// Token: 0x04000EAC RID: 3756
		private RenderStateBlock _renderStateBlock;

		// Token: 0x04000EAD RID: 3757
		private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();

		// Token: 0x04000EAE RID: 3758
		private readonly Material _material;
	}
}
