using System;
using Motorways.Constants;
using Rendering.RenderFeatures;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005FB RID: 1531
	[RequireComponent(typeof(MeshRenderer))]
	public class ShadowFadeoutInfo : MonoBehaviour
	{
		// Token: 0x06002A95 RID: 10901 RVA: 0x000BAB7C File Offset: 0x000B8D7C
		private void Awake()
		{
			if (ShadowFadeoutInfo._materialProperty == null)
			{
				ShadowFadeoutInfo._materialProperty = new MaterialPropertyBlock();
			}
			this._meshRenderer = base.GetComponent<MeshRenderer>();
			this._meshRenderer.GetPropertyBlock(ShadowFadeoutInfo._materialProperty);
			ShadowFadeoutInfo._materialProperty.SetFloat(ShaderConstants.ShadowType, (float)this.shadowType);
			this._meshRenderer.SetPropertyBlock(ShadowFadeoutInfo._materialProperty);
		}

		// Token: 0x040024CA RID: 9418
		public ShadowTypeRenderPass.ShadowType shadowType;

		// Token: 0x040024CB RID: 9419
		private MeshRenderer _meshRenderer;

		// Token: 0x040024CC RID: 9420
		private static MaterialPropertyBlock _materialProperty;
	}
}
