using System;
using Client;
using Factory;
using Factory.Pools;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005CE RID: 1486
	public class IndicatorEchoView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x0600299A RID: 10650 RVA: 0x000B2A88 File Offset: 0x000B0C88
		private void Initialize(Vector3 position, Color color, AnimationCurve ringWidthCurve, float scaleMin, float scaleMax, float duration)
		{
			base.transform.position = position;
			this._renderer.material.color = color;
			this._ringWidthCurve = ringWidthCurve;
			this._scaleMin = scaleMin;
			this._scaleMax = scaleMax;
			this._duration = duration;
			this._timeLeft = duration;
		}

		// Token: 0x0600299B RID: 10651 RVA: 0x000B2AD9 File Offset: 0x000B0CD9
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			this._timeLeft -= timeInterval.Delta;
			if (this._timeLeft <= 0f)
			{
				return TickResult.Destroy;
			}
			this.TickAnimation(timeInterval.Delta);
			return TickResult.ContinueTicking;
		}

		// Token: 0x0600299C RID: 10652 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x0600299D RID: 10653 RVA: 0x000B2B0C File Offset: 0x000B0D0C
		private void TickAnimation(float tickTime)
		{
			float progressPercent = 1f - this._timeLeft / this._duration;
			float scale = Mathf.Lerp(this._scaleMin, this._scaleMax, this._normalizedScaleCurve.Evaluate(progressPercent));
			float alpha = this._normalizedAlphaCurve.Evaluate(progressPercent);
			base.transform.localScale = new Vector3(scale, scale, 1f);
			this._renderer.material.SetFloat(IndicatorEchoView.ShaderAlphaId, alpha);
			float ringSize = this._ringWidthCurve.Evaluate(progressPercent) / scale;
			this._renderer.material.SetFloat(IndicatorEchoView.ShaderRingSizeId, ringSize);
		}

		// Token: 0x0600299E RID: 10654 RVA: 0x000B2BAC File Offset: 0x000B0DAC
		public static IndicatorEchoView Create(ViewClient client, Vector3 position, Color color, AnimationCurve ringWidthCurve, float scaleMin, float scaleMax, float duration)
		{
			IndicatorEchoView newAlert = client.Scope.Get<IndicatorEchoView>();
			newAlert.Initialize(position, color, ringWidthCurve, scaleMin, scaleMax, duration);
			client.AddView(newAlert);
			return newAlert;
		}

		// Token: 0x0600299F RID: 10655 RVA: 0x000B2BDC File Offset: 0x000B0DDC
		public void Reset()
		{
			this._scaleMin = 0f;
			this._scaleMax = 0f;
			this._duration = 0f;
			this._timeLeft = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localScale = Vector3.one;
		}

		// Token: 0x04002358 RID: 9048
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x04002359 RID: 9049
		[SerializeField]
		[Header("Shared Settings")]
		private AnimationCurve _normalizedScaleCurve;

		// Token: 0x0400235A RID: 9050
		[SerializeField]
		private AnimationCurve _normalizedAlphaCurve;

		// Token: 0x0400235B RID: 9051
		[Header("Internal References")]
		[SerializeField]
		private Renderer _renderer;

		// Token: 0x0400235C RID: 9052
		private AnimationCurve _ringWidthCurve;

		// Token: 0x0400235D RID: 9053
		private float _scaleMin;

		// Token: 0x0400235E RID: 9054
		private float _scaleMax;

		// Token: 0x0400235F RID: 9055
		private float _duration;

		// Token: 0x04002360 RID: 9056
		private float _timeLeft;

		// Token: 0x04002361 RID: 9057
		private static int ShaderAlphaId = Shader.PropertyToID("_Alpha");

		// Token: 0x04002362 RID: 9058
		private static int ShaderRingSizeId = Shader.PropertyToID("_RingSize");
	}
}
