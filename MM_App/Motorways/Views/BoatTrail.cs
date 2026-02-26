using System;
using Motorways.Constants;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000581 RID: 1409
	[ExecuteAlways]
	public class BoatTrail : MonoBehaviour
	{
		// Token: 0x060026C3 RID: 9923 RVA: 0x000A4F8D File Offset: 0x000A318D
		public void SetVisualConstantsData(VisualConstantsData data)
		{
			this._visualConstantsData = data;
		}

		// Token: 0x060026C4 RID: 9924 RVA: 0x000A4F98 File Offset: 0x000A3198
		public void UpdateBoatTrail(float scaledDelta, float distanceToTarget)
		{
			this._currentOverallOpacity = (Mathf.Clamp(distanceToTarget, this._visualConstantsData.boatTrailDistanceFromTargetVisible, this._visualConstantsData.boatTrailDistanceFromTargetFadeIn) - this._visualConstantsData.boatTrailDistanceFromTargetVisible) / (this._visualConstantsData.boatTrailDistanceFromTargetFadeIn - this._visualConstantsData.boatTrailDistanceFromTargetVisible);
			this._trailRendererTime = this._visualConstantsData.boatNormalTrailRendererTime;
			if (this._trailRendererTime >= 0f)
			{
				this._boatTrailRenderer.SetLifetime(this._trailRendererTime);
			}
			this._boatTrailRenderer.Tick(scaledDelta);
		}

		// Token: 0x060026C5 RID: 9925 RVA: 0x000A5028 File Offset: 0x000A3228
		private void UpdatePosition()
		{
			this._boatTrail.sharedMaterial.SetFloat(ShaderConstants.TrailTimeEnd, this._boatTrailRenderer.GetTimeForPoint(this._boatTrailRenderer.GetTailIndex()));
			this._boatTrail.sharedMaterial.SetFloat(ShaderConstants.TrailTime, this._boatTrailRenderer.GetTimeForPoint(this._boatTrailRenderer.GetHeadIndex()));
			this._boatTrail.sharedMaterial.SetFloat(ShaderConstants.OverallOpacity, this._currentOverallOpacity);
			this._boatTrail.sharedMaterial.SetFloat(ShaderConstants.WaveWidth, this.waveWidth);
			this._boatTrail.sharedMaterial.SetFloat(ShaderConstants.WaveLength, this.waveLength);
			this._boatTrail.sharedMaterial.SetFloat(ShaderConstants.OpacityThreshold, this.opacityThreshold);
		}

		// Token: 0x060026C6 RID: 9926 RVA: 0x000A50F7 File Offset: 0x000A32F7
		private void OnEnable()
		{
			this.UpdatePosition();
			this._currentOverallOpacity = 1f;
			this._trailRendererTime = ((this._visualConstantsData != null) ? this._visualConstantsData.boatNormalTrailRendererTime : 0f);
		}

		// Token: 0x060026C7 RID: 9927 RVA: 0x000A5130 File Offset: 0x000A3330
		private void Update()
		{
			this.UpdatePosition();
		}

		// Token: 0x040020B1 RID: 8369
		[Range(0f, 0.5f)]
		public float waveWidth;

		// Token: 0x040020B2 RID: 8370
		[Range(0f, 0.5f)]
		public float waveLength;

		// Token: 0x040020B3 RID: 8371
		[Range(0f, 1f)]
		public float opacityThreshold = 0.5f;

		// Token: 0x040020B4 RID: 8372
		[SerializeField]
		private VehicleTrailRenderer _boatTrailRenderer;

		// Token: 0x040020B5 RID: 8373
		[SerializeField]
		private Renderer _boatTrail;

		// Token: 0x040020B6 RID: 8374
		private float _currentOverallOpacity = 1f;

		// Token: 0x040020B7 RID: 8375
		private VisualConstantsData _visualConstantsData;

		// Token: 0x040020B8 RID: 8376
		private float _trailRendererTime;
	}
}
