using System;
using Client;
using Easing;
using Factory.Pools;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200057E RID: 1406
	public class AlertView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x060026A2 RID: 9890 RVA: 0x000A4856 File Offset: 0x000A2A56
		public void Initialize(Vector3 position, float duration, float scale, Color color, float alpha)
		{
			base.transform.position = position;
			this._timeLeft = duration;
			this._duration = duration;
			this._baseScale = scale;
			this._alpha = alpha;
			this._renderer.color = color;
		}

		// Token: 0x060026A3 RID: 9891 RVA: 0x000A4890 File Offset: 0x000A2A90
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			this._timeLeft -= timeInterval.Delta;
			if (this._timeLeft <= 0f)
			{
				return TickResult.Destroy;
			}
			float t = 1f - this._timeLeft / this._duration;
			float targetScale = 1f + Easings.Interpolate(t, Easings.Functions.ExponentialEaseOut) * this._baseScale;
			float alpha = this._alpha - Easings.Interpolate(t, Easings.Functions.CubicEaseOut) * this._alpha;
			base.transform.localScale = new Vector3(targetScale, targetScale, 1f);
			this.Alpha = alpha;
			return TickResult.ContinueTicking;
		}

		// Token: 0x060026A4 RID: 9892 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x060026A5 RID: 9893 RVA: 0x000A491E File Offset: 0x000A2B1E
		// (set) Token: 0x060026A6 RID: 9894 RVA: 0x000A4930 File Offset: 0x000A2B30
		public float Alpha
		{
			get
			{
				return this._renderer.color.a;
			}
			set
			{
				if (!Mathf.Approximately(this.Alpha, value))
				{
					Color c = this._renderer.color;
					c.a = value;
					this._renderer.color = c;
				}
			}
		}

		// Token: 0x060026A7 RID: 9895 RVA: 0x000A496C File Offset: 0x000A2B6C
		public void Reset()
		{
			this._timeLeft = 0f;
			this._duration = 0f;
			this._baseScale = 0f;
			this._alpha = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localScale = Vector3.one;
		}

		// Token: 0x060026A8 RID: 9896 RVA: 0x000A49C8 File Offset: 0x000A2BC8
		public static AlertView Create(ViewClient client, Vector3 position, Color? color = null, float? scale = null, float? duration = null, float? alpha = null)
		{
			AlertView newAlert = client.Scope.Get<AlertView>();
			float thisScale = scale ?? newAlert._defaultAlertScale;
			float thisDuration = duration ?? newAlert._defaultAlertDuration;
			Color thisColor = color ?? newAlert._defaultAlertColor;
			float thisAlpha = alpha ?? newAlert._defaultAlertAlpha;
			newAlert.Initialize(position, thisDuration, thisScale, thisColor, thisAlpha);
			client.AddView(newAlert);
			return newAlert;
		}

		// Token: 0x0400209C RID: 8348
		[SerializeField]
		private Color _defaultAlertColor = Color.white;

		// Token: 0x0400209D RID: 8349
		[SerializeField]
		private float _defaultAlertScale = 7f;

		// Token: 0x0400209E RID: 8350
		[SerializeField]
		private float _defaultAlertDuration = 2.7f;

		// Token: 0x0400209F RID: 8351
		[SerializeField]
		private float _defaultAlertAlpha = 0.6f;

		// Token: 0x040020A0 RID: 8352
		private float _timeLeft;

		// Token: 0x040020A1 RID: 8353
		private float _duration;

		// Token: 0x040020A2 RID: 8354
		private float _baseScale;

		// Token: 0x040020A3 RID: 8355
		private float _alpha;

		// Token: 0x040020A4 RID: 8356
		[SerializeField]
		private SpriteRenderer _renderer;
	}
}
