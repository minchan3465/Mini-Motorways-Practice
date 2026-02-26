using System;
using Easing;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways
{
	// Token: 0x02000445 RID: 1093
	[RequireComponent(typeof(RectTransform))]
	public class FocusPoint : MonoBehaviour
	{
		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06001B21 RID: 6945 RVA: 0x000636B4 File Offset: 0x000618B4
		public Vector2 Position
		{
			get
			{
				return this._rectTransform.anchoredPosition;
			}
		}

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06001B22 RID: 6946 RVA: 0x000636C1 File Offset: 0x000618C1
		public bool IsVisible
		{
			get
			{
				return this._animationState == FocusPoint.AnimationState.TransitionIn || this._animationState == FocusPoint.AnimationState.Visible;
			}
		}

		// Token: 0x06001B23 RID: 6947 RVA: 0x000636D7 File Offset: 0x000618D7
		private void Awake()
		{
			this._image = base.GetComponent<Image>();
			this._rectTransform = base.GetComponent<RectTransform>();
			this.SetFocusPointActive(false, true);
			this._targetPosition = this._rectTransform.anchoredPosition;
		}

		// Token: 0x06001B24 RID: 6948 RVA: 0x0006370A File Offset: 0x0006190A
		public void Update()
		{
			this.ProcessTransitions();
			this.ProcessFade();
			this._rectTransform.anchoredPosition = Vector2.Lerp(this._rectTransform.anchoredPosition, this._targetPosition, 0.8f);
		}

		// Token: 0x06001B25 RID: 6949 RVA: 0x00063740 File Offset: 0x00061940
		private void ProcessTransitions()
		{
			FocusPoint.AnimationState animationState = this._animationState;
			if (animationState == FocusPoint.AnimationState.TransitionIn)
			{
				this._transitionProgress += Time.deltaTime;
				if (this._transitionProgress >= this._transitionDuration)
				{
					this._transitionProgress = this._transitionDuration;
					this._animationState = FocusPoint.AnimationState.Visible;
				}
				float lerpTime = Easings.BackEaseOut(1f / this._transitionDuration * this._transitionProgress);
				base.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, lerpTime);
				return;
			}
			if (animationState != FocusPoint.AnimationState.TransitionOut)
			{
				return;
			}
			this._transitionProgress += Time.deltaTime;
			if (this._transitionProgress >= this._transitionDuration)
			{
				this._transitionProgress = this._transitionDuration;
				this._animationState = FocusPoint.AnimationState.Hidden;
			}
			float lerpTime2 = Easings.BackEaseIn(1f / this._transitionDuration * this._transitionProgress);
			base.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, lerpTime2);
		}

		// Token: 0x06001B26 RID: 6950 RVA: 0x0006382C File Offset: 0x00061A2C
		private void ProcessFade()
		{
			if (FeatureToggle.IsFeatureDisabled(Feature.CursorFade))
			{
				return;
			}
			FocusPoint.FadeState fadeState = this._fadeState;
			if (fadeState != FocusPoint.FadeState.FadeDelay)
			{
				if (fadeState != FocusPoint.FadeState.Fading)
				{
					return;
				}
				this._fadeProgress += Time.deltaTime;
				float alpha = 1f - 1f / this._fadeDuration * this._fadeProgress;
				this.SetAlpha(alpha);
				if (this._fadeProgress >= this._fadeDuration)
				{
					this._fadeState = FocusPoint.FadeState.Hidden;
					this._fadeProgress = 0f;
				}
			}
			else
			{
				this._fadeDelayProgress += Time.deltaTime;
				if (this._fadeDelayProgress >= this._fadeDelayDuration)
				{
					this._fadeState = FocusPoint.FadeState.Fading;
					this._fadeDelayProgress = 0f;
					return;
				}
			}
		}

		// Token: 0x06001B27 RID: 6951 RVA: 0x000638D9 File Offset: 0x00061AD9
		private void RemoveFade()
		{
			if (FeatureToggle.IsFeatureDisabled(Feature.CursorFade))
			{
				return;
			}
			this._fadeState = FocusPoint.FadeState.Visible;
			this.SetAlpha(1f);
		}

		// Token: 0x06001B28 RID: 6952 RVA: 0x000638F8 File Offset: 0x00061AF8
		private void SetAlpha(float alpha)
		{
			Color color = this._image.color;
			color.a = alpha;
			this._image.color = color;
			Color additionalColor = this._additionalImage.color;
			additionalColor.a = alpha;
			this._additionalImage.color = additionalColor;
		}

		// Token: 0x06001B29 RID: 6953 RVA: 0x00063945 File Offset: 0x00061B45
		public void SetCursorPosition(Vector2 position)
		{
			this._targetPosition = position;
			if (FeatureToggle.IsFeatureEnabled(Feature.CursorFade) && this.IsVisible)
			{
				this._fadeState = FocusPoint.FadeState.FadeDelay;
				this._fadeDelayProgress = 0f;
				this._fadeProgress = 0f;
			}
		}

		// Token: 0x06001B2A RID: 6954 RVA: 0x0006397C File Offset: 0x00061B7C
		public void OffsetCursorPosition(Vector2 offset)
		{
			this._targetPosition += offset * this._sensitivity;
		}

		// Token: 0x06001B2B RID: 6955 RVA: 0x0006399C File Offset: 0x00061B9C
		public void SetFocusPointActive(bool active, bool instant = false)
		{
			this.RemoveFade();
			if (active)
			{
				if (instant)
				{
					base.transform.localScale = Vector3.one;
					this._animationState = FocusPoint.AnimationState.Visible;
					return;
				}
				this.BeginShowCursor();
				return;
			}
			else
			{
				if (instant)
				{
					base.transform.localScale = Vector3.zero;
					this._animationState = FocusPoint.AnimationState.Hidden;
					return;
				}
				this.BeginHideCursor();
				return;
			}
		}

		// Token: 0x06001B2C RID: 6956 RVA: 0x000639F5 File Offset: 0x00061BF5
		private void BeginShowCursor()
		{
			this._animationState = FocusPoint.AnimationState.TransitionIn;
			this._transitionProgress = this._transitionDuration - this._transitionProgress;
		}

		// Token: 0x06001B2D RID: 6957 RVA: 0x00063A11 File Offset: 0x00061C11
		private void BeginHideCursor()
		{
			this._animationState = FocusPoint.AnimationState.TransitionOut;
			this._transitionProgress = this._transitionDuration - this._transitionProgress;
		}

		// Token: 0x0400168C RID: 5772
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("FocusPoint");

		// Token: 0x0400168D RID: 5773
		private RectTransform _rectTransform;

		// Token: 0x0400168E RID: 5774
		private Image _image;

		// Token: 0x0400168F RID: 5775
		[SerializeField]
		private Image _additionalImage;

		// Token: 0x04001690 RID: 5776
		[SerializeField]
		private FocusPoint.AnimationState _animationState;

		// Token: 0x04001691 RID: 5777
		private float _transitionProgress;

		// Token: 0x04001692 RID: 5778
		[SerializeField]
		private float _transitionDuration = 0.2f;

		// Token: 0x04001693 RID: 5779
		[SerializeField]
		private float _sensitivity = 5f;

		// Token: 0x04001694 RID: 5780
		private FocusPoint.FadeState _fadeState;

		// Token: 0x04001695 RID: 5781
		[SerializeField]
		private float _fadeDelayDuration = 2.5f;

		// Token: 0x04001696 RID: 5782
		private float _fadeDelayProgress;

		// Token: 0x04001697 RID: 5783
		[SerializeField]
		private float _fadeDuration = 0.2f;

		// Token: 0x04001698 RID: 5784
		private float _fadeProgress;

		// Token: 0x04001699 RID: 5785
		private Vector2 _targetPosition;

		// Token: 0x0400169A RID: 5786
		private const float AnchorPositionOffsetFactor = 0.8f;

		// Token: 0x02000446 RID: 1094
		private enum AnimationState
		{
			// Token: 0x0400169C RID: 5788
			Hidden,
			// Token: 0x0400169D RID: 5789
			TransitionIn,
			// Token: 0x0400169E RID: 5790
			Visible,
			// Token: 0x0400169F RID: 5791
			TransitionOut
		}

		// Token: 0x02000447 RID: 1095
		private enum FadeState
		{
			// Token: 0x040016A1 RID: 5793
			Visible,
			// Token: 0x040016A2 RID: 5794
			FadeDelay,
			// Token: 0x040016A3 RID: 5795
			Fading,
			// Token: 0x040016A4 RID: 5796
			Hidden
		}
	}
}
