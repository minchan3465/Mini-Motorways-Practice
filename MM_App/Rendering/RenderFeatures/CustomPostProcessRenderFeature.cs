using System;
using Rendering.RenderFeatures.OrderedDither;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils;

namespace Rendering.RenderFeatures
{
	// Token: 0x0200029B RID: 667
	public class CustomPostProcessRenderFeature : ScriptableRendererFeature
	{
		// Token: 0x06001094 RID: 4244 RVA: 0x000385C9 File Offset: 0x000367C9
		public override void Create()
		{
			this._customBlurPass = new CustomBlurPass(this.settings.blurMaterial);
			this._customVignettePass = new CustomVignettePass(this.settings);
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x000385F4 File Offset: 0x000367F4
		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			AuxiliaryGameCamera auxiliaryGameCamera;
			renderingData.cameraData.camera.TryGetComponent<AuxiliaryGameCamera>(out auxiliaryGameCamera);
			if (auxiliaryGameCamera == null || !auxiliaryGameCamera.ShouldBlur)
			{
				return;
			}
			GameCamera gameCamera = auxiliaryGameCamera.mainGameCamera;
			GameCamera mainGameCamera;
			renderingData.cameraData.camera.TryGetComponent<GameCamera>(out mainGameCamera);
			if (!gameCamera.PostProcessingEnabled)
			{
				return;
			}
			CustomBlurData customBlurData = gameCamera.customBlur;
			if ((double)customBlurData.Strength > 0.0)
			{
				this._customBlurPass.Setup(customBlurData.Strength, customBlurData.LevelsRange, customBlurData.LevelsOffset);
				renderer.EnqueuePass(this._customBlurPass);
			}
			renderer.EnqueuePass(this._customVignettePass);
		}

		// Token: 0x04000E95 RID: 3733
		public CustomPostProcessRenderFeature.FeatureSettings settings = new CustomPostProcessRenderFeature.FeatureSettings();

		// Token: 0x04000E96 RID: 3734
		private CustomBlurPass _customBlurPass;

		// Token: 0x04000E97 RID: 3735
		private CustomVignettePass _customVignettePass;

		// Token: 0x0200029C RID: 668
		[Serializable]
		public class FeatureSettings
		{
			// Token: 0x04000E98 RID: 3736
			public Material blurMaterial;

			// Token: 0x04000E99 RID: 3737
			public Material vignetteMaterial;

			// Token: 0x04000E9A RID: 3738
			[Range(0f, 1f)]
			public float tintBlend;

			// Token: 0x04000E9B RID: 3739
			public Texture2D tintTexture;

			// Token: 0x04000E9C RID: 3740
			public Texture2D paperTexture;

			// Token: 0x04000E9D RID: 3741
			[Tooltip("Vignette color.")]
			public Color color;

			// Token: 0x04000E9E RID: 3742
			[Tooltip("Sets the vignette center point (screen center is [0.5, 0.5]).")]
			public Vector2 center;

			// Token: 0x04000E9F RID: 3743
			[Range(0f, 1f)]
			[Tooltip("Amount of vignetting on screen.")]
			public float intensity;

			// Token: 0x04000EA0 RID: 3744
			[Range(0.01f, 1f)]
			[Tooltip("Smoothness of the vignette borders.")]
			public float smoothness;

			// Token: 0x04000EA1 RID: 3745
			[Range(0f, 1f)]
			[Tooltip("Lower values will make a square-ish vignette.")]
			public float roundness;

			// Token: 0x04000EA2 RID: 3746
			[Tooltip("Set to true to mark the vignette to be perfectly round. False will make its shape dependent on the current aspect ratio.")]
			public bool rounded;
		}
	}
}
