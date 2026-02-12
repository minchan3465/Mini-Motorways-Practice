using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Utils;

namespace Rendering.RenderFeatures
{
	// Token: 0x020002A0 RID: 672
	public class MiniMotorwaysRenderFeature : ScriptableRendererFeature
	{
		// Token: 0x060010A8 RID: 4264 RVA: 0x00038C70 File Offset: 0x00036E70
		public MiniMotorwaysRenderFeature()
		{
			RTHandles.Initialize(Screen.width, Screen.height);
		}

		// Token: 0x060010A9 RID: 4265 RVA: 0x00038C94 File Offset: 0x00036E94
		public override void Create()
		{
			this._highestMotorwayPass = new HighestMotorwayPass(this.settings.highestMotorwayShader);
			this._shadowTypeRenderPass = new ShadowTypeRenderPass(this.settings.shadowTypeShader);
			this._shadowFadeoutPass = new ShadowFadeoutPass(this.settings.shadowFadeoutShader);
			this._headlightOcclusionPass = new HeadlightOcclusionPass(this.settings.headlightOcclusionShader);
			this._headlightRenderPass = new HeadlightRenderPass(this.settings.headlightMaterial);
			RenderPipelineManager.beginCameraRendering += delegate(ScriptableRenderContext context, Camera camera)
			{
				RTHandles.SetReferenceSize(Screen.width, Screen.height);
				context.SetupCameraProperties(camera, false);
			};
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x00038D34 File Offset: 0x00036F34
		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (renderingData.cameraData.camera.gameObject.GetComponent<MiniMotorwaysRenderFeatureCameraMarker>() != null)
			{
				renderer.EnqueuePass(this._highestMotorwayPass);
				renderer.EnqueuePass(this._shadowTypeRenderPass);
				renderer.EnqueuePass(this._headlightOcclusionPass);
				renderer.EnqueuePass(this._shadowFadeoutPass);
				renderer.EnqueuePass(this._headlightRenderPass);
			}
		}

		// Token: 0x060010AB RID: 4267 RVA: 0x00038D9C File Offset: 0x00036F9C
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this._highestMotorwayPass.Dispose();
				this._shadowTypeRenderPass.Dispose();
				this._headlightOcclusionPass.Dispose();
				this._shadowFadeoutPass.Dispose();
				this._headlightRenderPass.Dispose();
			}
		}

		// Token: 0x04000EB5 RID: 3765
		public MiniMotorwaysRenderFeature.FeatureSettings settings = new MiniMotorwaysRenderFeature.FeatureSettings();

		// Token: 0x04000EB6 RID: 3766
		private HighestMotorwayPass _highestMotorwayPass;

		// Token: 0x04000EB7 RID: 3767
		private ShadowTypeRenderPass _shadowTypeRenderPass;

		// Token: 0x04000EB8 RID: 3768
		private ShadowFadeoutPass _shadowFadeoutPass;

		// Token: 0x04000EB9 RID: 3769
		private HeadlightOcclusionPass _headlightOcclusionPass;

		// Token: 0x04000EBA RID: 3770
		private HeadlightRenderPass _headlightRenderPass;

		// Token: 0x020002A1 RID: 673
		[Serializable]
		public class FeatureSettings
		{
			// Token: 0x04000EBB RID: 3771
			public Shader highestMotorwayShader;

			// Token: 0x04000EBC RID: 3772
			public Shader shadowTypeShader;

			// Token: 0x04000EBD RID: 3773
			public Shader shadowFadeoutShader;

			// Token: 0x04000EBE RID: 3774
			public Shader headlightOcclusionShader;

			// Token: 0x04000EBF RID: 3775
			public Material headlightMaterial;
		}
	}
}
