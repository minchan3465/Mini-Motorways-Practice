using System;
using System.Collections.Generic;
using Motorways.Constants;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering.RenderFeatures
{
	// Token: 0x020002A3 RID: 675
	public class ShadowFadeoutPass : ScriptableRenderPass
	{
		// Token: 0x060010B0 RID: 4272 RVA: 0x00038E10 File Offset: 0x00037010
		public ShadowFadeoutPass(Shader shadowFadeoutShader)
		{
			base.profilingSampler = new ProfilingSampler("HighestMotorwayPass");
			this._profilingSampler = new ProfilingSampler("Shadow Type Pass");
			base.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
			if (Diagnostics.Verify(shadowFadeoutShader != null, "Shadow Fadeout Shader is null!"))
			{
				this._material = new Material(shadowFadeoutShader);
			}
			this._mFilteringSettings = new FilteringSettings(null, LayerConstants.MotorwayMask, uint.MaxValue, 0);
			this._shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
			this._renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x00038EB4 File Offset: 0x000370B4
		~ShadowFadeoutPass()
		{
			this.Dispose();
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Dispose()
		{
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x00038EE0 File Offset: 0x000370E0
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

		// Token: 0x04000EC2 RID: 3778
		private readonly ProfilingSampler _profilingSampler;

		// Token: 0x04000EC3 RID: 3779
		private FilteringSettings _mFilteringSettings;

		// Token: 0x04000EC4 RID: 3780
		private RenderStateBlock _renderStateBlock;

		// Token: 0x04000EC5 RID: 3781
		private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();

		// Token: 0x04000EC6 RID: 3782
		private readonly Material _material;
	}
}
