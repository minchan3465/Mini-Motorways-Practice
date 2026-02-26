using System;
using Easing;
using Factory;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x0200071A RID: 1818
	public abstract class AnimatedCard : TouchButton
	{
		// Token: 0x17000836 RID: 2102
		// (get) Token: 0x060031F2 RID: 12786 RVA: 0x000EC646 File Offset: 0x000EA846
		public DelegateCanvasGroup CanvasGroup
		{
			get
			{
				return this._canvasGroup;
			}
		}

		// Token: 0x17000837 RID: 2103
		// (get) Token: 0x060031F3 RID: 12787 RVA: 0x000EC64E File Offset: 0x000EA84E
		public bool IsHidden
		{
			get
			{
				return this._isHiddenLeft || this._isHiddenRight;
			}
		}

		// Token: 0x1400004B RID: 75
		// (add) Token: 0x060031F4 RID: 12788 RVA: 0x000EC660 File Offset: 0x000EA860
		// (remove) Token: 0x060031F5 RID: 12789 RVA: 0x000EC698 File Offset: 0x000EA898
		public event Action onAnimationMidFlip;

		// Token: 0x1400004C RID: 76
		// (add) Token: 0x060031F6 RID: 12790 RVA: 0x000EC6D0 File Offset: 0x000EA8D0
		// (remove) Token: 0x060031F7 RID: 12791 RVA: 0x000EC708 File Offset: 0x000EA908
		public event Action onFlipAnimationComplete;

		// Token: 0x060031F8 RID: 12792 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void RegisterThemeComponents()
		{
		}

		// Token: 0x060031F9 RID: 12793 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void UnregisterThemeComponents()
		{
		}

		// Token: 0x060031FA RID: 12794 RVA: 0x000EC740 File Offset: 0x000EA940
		protected virtual void Update()
		{
			if (this._pushOffset.IsActive)
			{
				this._pushOffset.Tick(Time.deltaTime);
				this._offsetRect.localPosition = Vector3.right * this._pushOffset.Value;
			}
			if (this._delayBeforePush >= 0f)
			{
				this._delayBeforePush -= Time.deltaTime;
				if (this._delayBeforePush < 0f)
				{
					if (this._pushLeft)
					{
						this._animator.SetBool(AnimatedCard.PushedLeft, true);
						return;
					}
					this._animator.SetBool(AnimatedCard.PushedRight, true);
				}
			}
		}

		// Token: 0x060031FB RID: 12795 RVA: 0x000EC7E4 File Offset: 0x000EA9E4
		public static void SetNavigationOnRight(Selectable selectable, Selectable selectOnRight)
		{
			Navigation nav = selectable.navigation;
			nav.selectOnRight = selectOnRight;
			selectable.navigation = nav;
		}

		// Token: 0x060031FC RID: 12796 RVA: 0x000EC808 File Offset: 0x000EAA08
		public static void SetNavigationOnLeft(Selectable selectable, Selectable selectOnLeft)
		{
			Navigation nav = selectable.navigation;
			nav.selectOnLeft = selectOnLeft;
			selectable.navigation = nav;
		}

		// Token: 0x060031FD RID: 12797 RVA: 0x000EC82C File Offset: 0x000EAA2C
		public static void SetNavigationOnUp(Selectable selectable, Selectable selectOnUp)
		{
			Navigation nav = selectable.navigation;
			nav.selectOnUp = selectOnUp;
			selectable.navigation = nav;
		}

		// Token: 0x060031FE RID: 12798 RVA: 0x000EC850 File Offset: 0x000EAA50
		public static void SetNavigationOnDown(Selectable selectable, Selectable selectOnDown)
		{
			Navigation nav = selectable.navigation;
			nav.selectOnDown = selectOnDown;
			selectable.navigation = nav;
		}

		// Token: 0x060031FF RID: 12799 RVA: 0x000EC873 File Offset: 0x000EAA73
		public void SetHighlightAnimation(float transitionAmount)
		{
			this.SetHeightOffGrid(transitionAmount);
		}

		// Token: 0x06003200 RID: 12800 RVA: 0x000EC87C File Offset: 0x000EAA7C
		public void SetHeightOffGrid(float relativeHeight)
		{
			this._shadowRect.anchoredPosition = this.minimumShadowOffset + new Vector2(this.selectedZoomLevel * (this.minimumShadowOffset.x / 2f) * relativeHeight, this.selectedZoomLevel * (this.minimumShadowOffset.y / 2f) * relativeHeight);
			this._shadowRect.localScale = Vector3.one + new Vector3(relativeHeight * 0.1f * this.selectedZoomLevel, relativeHeight * 0.1f * this.selectedZoomLevel);
			this._mainPanelRect.localScale = Vector3.one + new Vector3(relativeHeight * 0.1f * this.selectedZoomLevel, relativeHeight * 0.1f * this.selectedZoomLevel);
			this.SetSelectedValue(relativeHeight);
		}

		// Token: 0x06003201 RID: 12801 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void SetSelectedValue(float distance)
		{
		}

		// Token: 0x06003202 RID: 12802 RVA: 0x000EC94C File Offset: 0x000EAB4C
		public void SetOffset(AnimatedCard.ExpansionLevel offsetLevel, bool isPushedLeft = false)
		{
			float num;
			switch (offsetLevel)
			{
			case AnimatedCard.ExpansionLevel.Narrow:
				num = 0f;
				break;
			case AnimatedCard.ExpansionLevel.Medium:
				num = 100f;
				break;
			case AnimatedCard.ExpansionLevel.Wide:
				num = 200f;
				break;
			default:
				num = 0f;
				break;
			}
			float offsetAmount = num;
			if (isPushedLeft)
			{
				offsetAmount *= -1f;
			}
			this._pushOffset.Start(this._offsetRect.localPosition.x, offsetAmount, this._expandedPushOutDuration, this.expandedPushOutEaseType, 0f);
		}

		// Token: 0x06003203 RID: 12803 RVA: 0x000EC9C5 File Offset: 0x000EABC5
		public virtual void OnOtherCardConfirmed(bool pushLeft, float delay)
		{
			this._delayBeforePush = delay;
			this._pushLeft = pushLeft;
			this._canvasGroup.SetInteractable(false);
		}

		// Token: 0x06003204 RID: 12804 RVA: 0x000EC9E1 File Offset: 0x000EABE1
		public virtual void OnCardConfirmed()
		{
			this._animator.SetBool(AnimatedCard.Confirmed, true);
			this._canvasGroup.SetInteractable(false);
		}

		// Token: 0x06003205 RID: 12805 RVA: 0x000ECA00 File Offset: 0x000EAC00
		protected virtual void SetExpanded(AnimatedCard.ExpansionLevel expansionLevel)
		{
			float num;
			switch (expansionLevel)
			{
			case AnimatedCard.ExpansionLevel.Narrow:
				num = 432f;
				break;
			case AnimatedCard.ExpansionLevel.Medium:
				num = 550f;
				break;
			case AnimatedCard.ExpansionLevel.Wide:
				num = 770f;
				break;
			default:
				num = 432f;
				break;
			}
			float width = num;
			base.GetComponent<RectTransform>().sizeDelta = new Vector2(width, base.GetComponent<RectTransform>().sizeDelta.y);
		}

		// Token: 0x06003206 RID: 12806 RVA: 0x000ECA64 File Offset: 0x000EAC64
		public void ResetAnimations()
		{
			this._animator.SetBool(AnimatedCard.PushedLeft, false);
			this._animator.SetBool(AnimatedCard.PushedRight, false);
			this._animator.SetBool(AnimatedCard.Confirmed, false);
			this._animator.Update(1f);
		}

		// Token: 0x06003207 RID: 12807 RVA: 0x000ECAB4 File Offset: 0x000EACB4
		[UsedImplicitly]
		public void OnAnimationMidFlip()
		{
			Action action = this.onAnimationMidFlip;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06003208 RID: 12808 RVA: 0x000ECAC6 File Offset: 0x000EACC6
		[UsedImplicitly]
		public void OnFlipAnimationComplete()
		{
			Action action = this.onFlipAnimationComplete;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06003209 RID: 12809 RVA: 0x000ECAD8 File Offset: 0x000EACD8
		public virtual void OnTabSelectMidFlip()
		{
			this.onAnimationMidFlip -= this.OnTabSelectMidFlip;
		}

		// Token: 0x0600320A RID: 12810 RVA: 0x000ECAED File Offset: 0x000EACED
		public void TweenToNextCard()
		{
			this._animator.SetTrigger(AnimatedCard.Flip);
		}

		// Token: 0x0600320B RID: 12811 RVA: 0x000ECAFF File Offset: 0x000EACFF
		public void SetHideLeft()
		{
			this._animator.SetBool(AnimatedCard.HiddenLeft, true);
			this._canvasGroup.Alpha = 0f;
			this._isHiddenLeft = true;
		}

		// Token: 0x0600320C RID: 12812 RVA: 0x000ECB2C File Offset: 0x000EAD2C
		public void EnterFromHidden(Action onComplete = null)
		{
			this._canvasGroup.Alpha = 1f;
			this.OnAppear = onComplete;
			if (this._isHiddenLeft)
			{
				this._animator.SetBool(AnimatedCard.HiddenLeft, false);
				return;
			}
			if (this._isHiddenRight)
			{
				this._animator.SetBool(AnimatedCard.HiddenRight, false);
			}
		}

		// Token: 0x0600320D RID: 12813 RVA: 0x000ECB83 File Offset: 0x000EAD83
		public void OnEnteredFromLeft()
		{
			this._isHiddenLeft = false;
			Action onAppear = this.OnAppear;
			if (onAppear == null)
			{
				return;
			}
			onAppear();
		}

		// Token: 0x0600320E RID: 12814 RVA: 0x000ECB9C File Offset: 0x000EAD9C
		public void SetHideRight()
		{
			this._animator.SetBool(AnimatedCard.HiddenRight, true);
			this._canvasGroup.Alpha = 0f;
			this._isHiddenRight = true;
		}

		// Token: 0x0600320F RID: 12815 RVA: 0x000ECBC6 File Offset: 0x000EADC6
		public void OnEnteredFromRight()
		{
			this._isHiddenRight = false;
			Action onAppear = this.OnAppear;
			if (onAppear == null)
			{
				return;
			}
			onAppear();
		}

		// Token: 0x04002ACB RID: 10955
		[SerializeField]
		private Vector2 minimumShadowOffset = new Vector2(15f, -15f);

		// Token: 0x04002ACC RID: 10956
		[SerializeField]
		private float selectedZoomLevel = 2.5f;

		// Token: 0x04002ACD RID: 10957
		[SerializeField]
		private RectTransform _shadowRect;

		// Token: 0x04002ACE RID: 10958
		[SerializeField]
		private RectTransform _mainPanelRect;

		// Token: 0x04002ACF RID: 10959
		[SerializeField]
		private RectTransform _offsetRect;

		// Token: 0x04002AD0 RID: 10960
		[SerializeField]
		private DelegateCanvasGroup _canvasGroup;

		// Token: 0x04002AD1 RID: 10961
		[SerializeField]
		private Easings.Functions expandedPushOutEaseType = Easings.Functions.CubicEaseIn;

		// Token: 0x04002AD2 RID: 10962
		[SerializeField]
		private Easings.Functions expandedReturnEaseType = Easings.Functions.CubicEaseOut;

		// Token: 0x04002AD3 RID: 10963
		[SerializeField]
		private float _expandedPushOutDuration = 0.335f;

		// Token: 0x04002AD4 RID: 10964
		[SerializeField]
		protected Animator _animator;

		// Token: 0x04002AD5 RID: 10965
		private const float DefaultWidth = 432f;

		// Token: 0x04002AD6 RID: 10966
		private const float MediumWidth = 550f;

		// Token: 0x04002AD7 RID: 10967
		private const float ExpandedWidth = 770f;

		// Token: 0x04002AD8 RID: 10968
		private const float MediumNeighbourOffset = 100f;

		// Token: 0x04002AD9 RID: 10969
		private const float WideNeighbourOffset = 200f;

		// Token: 0x04002ADA RID: 10970
		private bool _isHiddenLeft;

		// Token: 0x04002ADB RID: 10971
		private bool _isHiddenRight;

		// Token: 0x04002ADC RID: 10972
		private bool _pushLeft;

		// Token: 0x04002ADD RID: 10973
		private float _delayBeforePush = -1f;

		// Token: 0x04002ADE RID: 10974
		private IScope _scope;

		// Token: 0x04002AE1 RID: 10977
		private Action OnAppear;

		// Token: 0x04002AE2 RID: 10978
		private readonly TweenFloat _pushOffset = new TweenFloat();

		// Token: 0x04002AE3 RID: 10979
		private static readonly int Flip = Animator.StringToHash("Flip");

		// Token: 0x04002AE4 RID: 10980
		private static readonly int PushedLeft = Animator.StringToHash("PushedLeft");

		// Token: 0x04002AE5 RID: 10981
		private static readonly int PushedRight = Animator.StringToHash("PushedRight");

		// Token: 0x04002AE6 RID: 10982
		private static readonly int Confirmed = Animator.StringToHash("Confirmed");

		// Token: 0x04002AE7 RID: 10983
		private static readonly int HiddenLeft = Animator.StringToHash("HiddenLeft");

		// Token: 0x04002AE8 RID: 10984
		private static readonly int HiddenRight = Animator.StringToHash("HiddenRight");

		// Token: 0x0200071B RID: 1819
		public enum ExpansionLevel
		{
			// Token: 0x04002AEA RID: 10986
			Narrow,
			// Token: 0x04002AEB RID: 10987
			Medium,
			// Token: 0x04002AEC RID: 10988
			Wide
		}
	}
}
