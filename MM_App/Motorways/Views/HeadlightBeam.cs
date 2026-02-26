using System;
using Motorways.Constants;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005C4 RID: 1476
	[ExecuteAlways]
	public class HeadlightBeam : MonoBehaviour
	{
		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x0600294B RID: 10571 RVA: 0x000B193A File Offset: 0x000AFB3A
		public Vector2 RightCutPoint
		{
			get
			{
				return this.leftCutPoint + new Vector2(Mathf.Cos(this.cutAngle * 0.017453292f), Mathf.Sin(this.cutAngle * 0.017453292f)) * this.cutLength;
			}
		}

		// Token: 0x0600294C RID: 10572 RVA: 0x000B197C File Offset: 0x000AFB7C
		private void UpdatePosition()
		{
			if (this._materialPropertyBlock == null)
			{
				this._materialPropertyBlock = new MaterialPropertyBlock();
			}
			this._headlightBeams.GetPropertyBlock(this._materialPropertyBlock);
			Transform headlightTransform = base.transform;
			this._materialPropertyBlock.SetMatrix(ShaderConstants.ObjectToLocalMatrix, headlightTransform.parent.worldToLocalMatrix * headlightTransform.localToWorldMatrix);
			this._headlightBeams.SetPropertyBlock(this._materialPropertyBlock);
		}

		// Token: 0x0600294D RID: 10573 RVA: 0x000B19EB File Offset: 0x000AFBEB
		private void OnEnable()
		{
			this._headlightBeams = base.GetComponent<Renderer>();
			this.UpdatePosition();
		}

		// Token: 0x0600294E RID: 10574 RVA: 0x000B19FF File Offset: 0x000AFBFF
		private void Update()
		{
			this.UpdatePosition();
		}

		// Token: 0x04002302 RID: 8962
		[Range(0f, 20f)]
		public float beamLength;

		// Token: 0x04002303 RID: 8963
		[Range(0f, 179.99f)]
		public float beamAngle;

		// Token: 0x04002304 RID: 8964
		public Vector2 leftCutPoint;

		// Token: 0x04002305 RID: 8965
		public float cutAngle;

		// Token: 0x04002306 RID: 8966
		public float cutLength;

		// Token: 0x04002307 RID: 8967
		private Renderer _headlightBeams;

		// Token: 0x04002308 RID: 8968
		private MaterialPropertyBlock _materialPropertyBlock;

		// Token: 0x04002309 RID: 8969
		public Vector2 beamOrigin;
	}
}
