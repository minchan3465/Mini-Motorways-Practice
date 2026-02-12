using System;
using Factory;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Motorways.Views
{
	// Token: 0x020005EE RID: 1518
	public class PermanenceZoneTextureLibrary : MonoBehaviour, ICreatedInScopeHandler
	{
		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x06002A3A RID: 10810 RVA: 0x000B8A23 File Offset: 0x000B6C23
		// (set) Token: 0x06002A3B RID: 10811 RVA: 0x000B8A2B File Offset: 0x000B6C2B
		public RenderTexture PermanenceIndexTexture { get; private set; }

		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x06002A3C RID: 10812 RVA: 0x000B8A34 File Offset: 0x000B6C34
		// (set) Token: 0x06002A3D RID: 10813 RVA: 0x000B8A3C File Offset: 0x000B6C3C
		public RenderTexture PermanenceFadeTexture { get; private set; }

		// Token: 0x14000046 RID: 70
		// (add) Token: 0x06002A3E RID: 10814 RVA: 0x000B8A48 File Offset: 0x000B6C48
		// (remove) Token: 0x06002A3F RID: 10815 RVA: 0x000B8A80 File Offset: 0x000B6C80
		public event Action OnTexturesRecreated;

		// Token: 0x06002A40 RID: 10816 RVA: 0x000B8AB5 File Offset: 0x000B6CB5
		public void OnCreatedInScope(IScope scope)
		{
			this.CreatePermanenceTextures();
			this._visualConstantsData.OnExpertPermanentRoadFadeLengthChanged += this.RecreateTextures;
			this._permanenceTextureMappingDatabase.OnTextureMappingsUpdated += this.RecreateTextures;
		}

		// Token: 0x06002A41 RID: 10817 RVA: 0x000B8AEC File Offset: 0x000B6CEC
		private void CreatePermanenceTextures()
		{
			this.PermanenceIndexTexture = new RenderTexture(512, 512, 1, GraphicsFormat.R32G32B32A32_SFloat, 0)
			{
				useMipMap = false,
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Mirror
			};
			this.PermanenceFadeTexture = new RenderTexture(512, 512, 1, GraphicsFormat.R32G32_SFloat, 0)
			{
				useMipMap = false,
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Mirror
			};
			this.UpdateShaderPropertyReferences();
		}

		// Token: 0x06002A42 RID: 10818 RVA: 0x000B8B5B File Offset: 0x000B6D5B
		private void RecreateTextures()
		{
			Diagnostics.Log.Info("PermanenceTextureLibrary", "Recreating textures...", Array.Empty<object>());
			this.CreatePermanenceTextures();
			Action onTexturesRecreated = this.OnTexturesRecreated;
			if (onTexturesRecreated == null)
			{
				return;
			}
			onTexturesRecreated();
		}

		// Token: 0x06002A43 RID: 10819 RVA: 0x000B8B88 File Offset: 0x000B6D88
		private void UpdateShaderPropertyReferences()
		{
			Material permanenceZoneIndexMaterial = new Material(this._permanenceZoneIndexShader);
			permanenceZoneIndexMaterial.SetFloat(PermanenceZoneTextureLibrary.FadeLength, this._visualConstantsData.ExpertPermanentRoadsFadeLength);
			permanenceZoneIndexMaterial.SetFloatArray(PermanenceZoneTextureLibrary.PermanenceIndexToZoneId, this._permanenceTextureMappingDatabase.shaderIndexToZoneIndex);
			permanenceZoneIndexMaterial.SetVectorArray(PermanenceZoneTextureLibrary.ZoneIdToFadeIds, this._permanenceTextureMappingDatabase.zoneIndexToFadeIndices);
			RenderTexture tempPermanenceIndexTexture = RenderTexture.GetTemporary(this.PermanenceIndexTexture.descriptor);
			Graphics.Blit(this.PermanenceIndexTexture, tempPermanenceIndexTexture, permanenceZoneIndexMaterial, 0);
			Graphics.Blit(tempPermanenceIndexTexture, this.PermanenceIndexTexture);
			RenderTexture.ReleaseTemporary(tempPermanenceIndexTexture);
			Material permanenceZoneFadesMaterial = new Material(this._permanenceZoneFadeShader);
			permanenceZoneFadesMaterial.SetFloat(PermanenceZoneTextureLibrary.FadeLength, this._visualConstantsData.ExpertPermanentRoadsFadeLength);
			permanenceZoneFadesMaterial.SetFloatArray(PermanenceZoneTextureLibrary.PermanenceIndexToZoneId, this._permanenceTextureMappingDatabase.shaderIndexToZoneIndex);
			permanenceZoneFadesMaterial.SetVectorArray(PermanenceZoneTextureLibrary.ZoneIdToFadeIds, this._permanenceTextureMappingDatabase.zoneIndexToFadeIndices);
			RenderTexture tempPermanenceFadeTexture = RenderTexture.GetTemporary(this.PermanenceFadeTexture.descriptor);
			Graphics.Blit(this.PermanenceFadeTexture, tempPermanenceFadeTexture, permanenceZoneFadesMaterial, 0);
			Graphics.Blit(tempPermanenceFadeTexture, this.PermanenceFadeTexture);
			RenderTexture.ReleaseTemporary(tempPermanenceFadeTexture);
		}

		// Token: 0x0400244A RID: 9290
		private const int PermanenceTextureResolution = 512;

		// Token: 0x0400244B RID: 9291
		[SerializeField]
		private Shader _permanenceZoneIndexShader;

		// Token: 0x0400244C RID: 9292
		[SerializeField]
		private Shader _permanenceZoneFadeShader;

		// Token: 0x0400244D RID: 9293
		[Dependency]
		private VisualConstantsData _visualConstantsData;

		// Token: 0x0400244E RID: 9294
		[Dependency]
		private PermanenceTextureMappingDatabase _permanenceTextureMappingDatabase;

		// Token: 0x04002451 RID: 9297
		private static readonly int FadeLength = Shader.PropertyToID("_FadeSize");

		// Token: 0x04002452 RID: 9298
		private static readonly int PermanenceIndexToZoneId = Shader.PropertyToID("_PermanenceIndexToZoneId");

		// Token: 0x04002453 RID: 9299
		private static readonly int ZoneIdToFadeIds = Shader.PropertyToID("_ZoneIdToFadeIds");
	}
}
