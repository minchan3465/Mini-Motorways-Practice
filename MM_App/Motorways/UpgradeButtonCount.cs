using System;
using Motorways.UI;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000451 RID: 1105
	public class UpgradeButtonCount : UpgradeButtonStack
	{
		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06001B73 RID: 7027 RVA: 0x00064391 File Offset: 0x00062591
		// (set) Token: 0x06001B74 RID: 7028 RVA: 0x00064399 File Offset: 0x00062599
		public override bool IsUnlimited
		{
			get
			{
				return this._isUnlimited;
			}
			set
			{
				if (this._isUnlimited != value)
				{
					this._isUnlimited = value;
					this.SetCountText();
				}
			}
		}

		// Token: 0x06001B75 RID: 7029 RVA: 0x000643B4 File Offset: 0x000625B4
		private void Awake()
		{
			if (this._topIcon != null)
			{
				this._topIcon.iconRenderer.sprite = this.referenceImage.sprite;
				if (this.IsCircle)
				{
					this._topIcon.SetToCircle();
					return;
				}
				this._topIcon.SetToDiamond();
			}
		}

		// Token: 0x06001B76 RID: 7030 RVA: 0x00064409 File Offset: 0x00062609
		private void OnEnable()
		{
			this.SetCountText();
			this.SetCount(this.desiredStackCount);
		}

		// Token: 0x06001B77 RID: 7031 RVA: 0x00064420 File Offset: 0x00062620
		public override void AddToStack(int count = 1, bool fromAnimation = false)
		{
			if (this.IsUnlimited && base.AccountedIconNumber >= 1)
			{
				this.SetCountText();
				return;
			}
			if (fromAnimation)
			{
				if (base.PendingAdditionCount >= count)
				{
					base.PendingAdditionCount -= count;
				}
				else
				{
					base.PendingAdditionCount = 0;
				}
			}
			this.desiredStackCount += count;
			this.SetCountText();
			if (base.AccountedIconNumber >= 1 && this._topIcon != null && !fromAnimation)
			{
				this._topIcon.Bounce();
			}
		}

		// Token: 0x06001B78 RID: 7032 RVA: 0x000644A1 File Offset: 0x000626A1
		public void Bounce()
		{
			if (this._animator != null)
			{
				this._animator.SetTrigger(UpgradeButtonCount.BounceTrigger);
			}
		}

		// Token: 0x06001B79 RID: 7033 RVA: 0x000644C4 File Offset: 0x000626C4
		public override void RemoveFromStack(int count = 1, bool fromAnimation = false)
		{
			if (this.IsUnlimited && base.AccountedIconNumber >= 1)
			{
				this.SetCountText();
				return;
			}
			if (Diagnostics.Verify(this.desiredStackCount - count >= 0, "We tried to remove more icons from a stack than we have! Trying to remove {0} from {1} on {2}", count, this.desiredStackCount, base.name))
			{
				if (fromAnimation)
				{
					base.PendingAdditionCount += count;
				}
				this.desiredStackCount -= count;
			}
			this.SetCountText();
		}

		// Token: 0x06001B7A RID: 7034 RVA: 0x0006453F File Offset: 0x0006273F
		public override void SetCount(int count)
		{
			if (this.desiredStackCount != count)
			{
				this.desiredStackCount = count;
				this.SetCountText();
			}
		}

		// Token: 0x06001B7B RID: 7035 RVA: 0x00064558 File Offset: 0x00062758
		private void SetCountText()
		{
			if (!this.ShowNumberCounter)
			{
				this._numberBubble.Hide(true);
			}
			else if (this.IsUnlimited)
			{
				this._numberBubble.SetValueUnlimited();
			}
			else
			{
				this._numberBubble.SetValue(this.desiredStackCount, true);
			}
			if (this._topIcon != null)
			{
				bool topIconVisible = this.desiredStackCount >= 1;
				this._topIcon.SetVisible(topIconVisible, TransitionStyle.Tween);
				if (this._baseIcon != null)
				{
					this._baseIcon.iconRenderer.gameObject.SetActive(!topIconVisible);
					this._baseIcon.outlineRenderer.gameObject.SetActive(!topIconVisible);
				}
			}
			if (this._upgradeButton != null)
			{
				this._upgradeButton.interactable = (this._upgradeButton.buttonType != GameUIButtonType.None && this.desiredStackCount > 0);
			}
		}

		// Token: 0x06001B7C RID: 7036 RVA: 0x0006463C File Offset: 0x0006283C
		public override UpgradeIcon GetTopIcon()
		{
			return this._topIcon;
		}

		// Token: 0x06001B7D RID: 7037 RVA: 0x000022F5 File Offset: 0x000004F5
		public override void DoStateTransition(ButtonAnimationState state, bool instant)
		{
		}

		// Token: 0x06001B7E RID: 7038 RVA: 0x000022F5 File Offset: 0x000004F5
		private void Update()
		{
		}

		// Token: 0x040016ED RID: 5869
		[SerializeField]
		private UpgradeButton _upgradeButton;

		// Token: 0x040016EE RID: 5870
		[SerializeField]
		private NumberBubble _numberBubble;

		// Token: 0x040016EF RID: 5871
		[SerializeField]
		private UpgradeIcon _baseIcon;

		// Token: 0x040016F0 RID: 5872
		[SerializeField]
		private UpgradeIcon _topIcon;

		// Token: 0x040016F1 RID: 5873
		[SerializeField]
		private Animator _animator;

		// Token: 0x040016F2 RID: 5874
		private static readonly int BounceTrigger = Animator.StringToHash("Bounce");
	}
}
