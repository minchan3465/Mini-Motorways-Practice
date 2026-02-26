using System;
using Easing;
using TMPro;
using UnityEngine;

namespace Motorways.UI
{
	// Token: 0x02000739 RID: 1849
	public class NumberBubble : MonoBehaviour
	{
		// Token: 0x060033A8 RID: 13224 RVA: 0x000F4687 File Offset: 0x000F2887
		private void Awake()
		{
			if (this._defaultScale == null)
			{
				this._defaultScale = new float?(base.transform.localScale.x);
			}
		}

		// Token: 0x060033A9 RID: 13225 RVA: 0x000F46B1 File Offset: 0x000F28B1
		public void Bounce()
		{
			this._scaleTween.Start(this._isHidden ? 0f : 1.3f, 1f, 0.5f, Easings.Functions.BounceEaseOut, 0f);
			this._isHidden = false;
		}

		// Token: 0x060033AA RID: 13226 RVA: 0x000F46EC File Offset: 0x000F28EC
		public void SetValue(int value, bool doBounce = true)
		{
			if (this.hideWhenZero && value <= 0)
			{
				if (!this._isHidden)
				{
					this.Hide(false);
					return;
				}
			}
			else
			{
				this._optionNumberText.text = value.ToString();
				if (doBounce)
				{
					this.Bounce();
					return;
				}
				if (this._isHidden)
				{
					this._optionNumberTransform.localScale = Vector3.one;
				}
			}
		}

		// Token: 0x060033AB RID: 13227 RVA: 0x000F4749 File Offset: 0x000F2949
		public void SetValueUnlimited()
		{
			this.Bounce();
			this._optionNumberText.text = "<sprite index=1 tint=1>";
		}

		// Token: 0x060033AC RID: 13228 RVA: 0x000F4764 File Offset: 0x000F2964
		public void Hide(bool instantly = false)
		{
			if (this._defaultScale == null)
			{
				this._defaultScale = new float?(base.transform.localScale.x);
			}
			this._isHidden = true;
			if (instantly)
			{
				this._scaleTween.Stop();
				this._optionNumberTransform.localScale = Vector3.zero;
				return;
			}
			this._scaleTween.Start(1f, 0f, 0.1f, Easings.Functions.Linear, 0f);
		}

		// Token: 0x060033AD RID: 13229 RVA: 0x000F47E0 File Offset: 0x000F29E0
		private void Update()
		{
			if (this._scaleTween.IsActive)
			{
				this._scaleTween.Tick(Time.deltaTime);
				this._optionNumberTransform.localScale = Vector3.one * this._scaleTween.Value * this._defaultScale.Value;
			}
		}

		// Token: 0x04002C20 RID: 11296
		public const float BounceScaleAmount = 1.3f;

		// Token: 0x04002C21 RID: 11297
		public const float BounceTweenDuration = 0.5f;

		// Token: 0x04002C22 RID: 11298
		public const float ShrinkTweenDuration = 0.1f;

		// Token: 0x04002C23 RID: 11299
		public bool hideWhenZero;

		// Token: 0x04002C24 RID: 11300
		[SerializeField]
		private RectTransform _optionNumberTransform;

		// Token: 0x04002C25 RID: 11301
		[SerializeField]
		private TMP_Text _optionNumberText;

		// Token: 0x04002C26 RID: 11302
		private TweenFloat _scaleTween = new TweenFloat();

		// Token: 0x04002C27 RID: 11303
		private bool _isHidden;

		// Token: 0x04002C28 RID: 11304
		private float? _defaultScale;

		// Token: 0x04002C29 RID: 11305
		private const string InfiniteSymbol = "<sprite index=1 tint=1>";
	}
}
