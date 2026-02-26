using System;
using System.Collections.Generic;
using Client;
using Easing;
using Motorways.Themes;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways
{
	// Token: 0x02000452 RID: 1106
	public class UpgradeButtonStack : MonoBehaviour, IThemeComponent
	{
		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06001B81 RID: 7041 RVA: 0x0006465D File Offset: 0x0006285D
		public int AccountedIconNumber
		{
			get
			{
				return this.desiredStackCount + this.hiddenUpgradeCount + this.PendingAdditionCount;
			}
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06001B82 RID: 7042 RVA: 0x00064673 File Offset: 0x00062873
		// (set) Token: 0x06001B83 RID: 7043 RVA: 0x0006467B File Offset: 0x0006287B
		public int PendingAdditionCount { get; set; }

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x06001B84 RID: 7044 RVA: 0x00064391 File Offset: 0x00062591
		// (set) Token: 0x06001B85 RID: 7045 RVA: 0x00064684 File Offset: 0x00062884
		public virtual bool IsUnlimited
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
					this.SetCount(this.desiredStackCount);
				}
			}
		}

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x06001B86 RID: 7046 RVA: 0x000646A2 File Offset: 0x000628A2
		private int VisibleStackCount
		{
			get
			{
				return this._stackedIcons.Count;
			}
		}

		// Token: 0x06001B87 RID: 7047 RVA: 0x000646AF File Offset: 0x000628AF
		public virtual UpgradeIcon GetTopIcon()
		{
			if (this._stackedIcons.Count > 0)
			{
				return this._stackedIcons[this._stackedIcons.Count - 1];
			}
			return this._baseStackIcon;
		}

		// Token: 0x06001B88 RID: 7048 RVA: 0x000646E0 File Offset: 0x000628E0
		public virtual void SetCount(int count)
		{
			this.PendingAdditionCount = 0;
			if (this.desiredStackCount < count)
			{
				int newAmount = Math.Min(UpgradeButtonStack.MaxVisibleIcons, count);
				this.hiddenUpgradeCount = Math.Max(count - UpgradeButtonStack.MaxVisibleIcons, 0);
				for (int iteration = 0; iteration < newAmount - this.desiredStackCount; iteration++)
				{
					this.AddNewIcon().Rect.localScale = Vector3.one;
				}
				this.desiredStackCount = newAmount;
			}
			else if (this.desiredStackCount > count)
			{
				int iconsToRemove = this.desiredStackCount - count;
				for (int iteration2 = 0; iteration2 < iconsToRemove; iteration2++)
				{
					if (this._stackedIcons.Count > 0)
					{
						this.RemoveIcon();
					}
				}
				this.desiredStackCount = Math.Min(UpgradeButtonStack.MaxVisibleIcons, count);
				this.hiddenUpgradeCount = Math.Max(count - UpgradeButtonStack.MaxVisibleIcons, 0);
			}
			this._animatingIconAddition = true;
			this.SetStackPositions(1f);
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x000647B8 File Offset: 0x000629B8
		public virtual void AddToStack(int count = 1, bool fromAnimation = false)
		{
			if (this.IsUnlimited && this.AccountedIconNumber >= 1)
			{
				return;
			}
			if (fromAnimation)
			{
				if (this.PendingAdditionCount >= count)
				{
					this.PendingAdditionCount -= count;
				}
				else
				{
					this.PendingAdditionCount = 0;
				}
			}
			if (this.desiredStackCount >= UpgradeButtonStack.MaxVisibleIcons)
			{
				this.hiddenUpgradeCount += count;
			}
			else if (count + this.desiredStackCount >= UpgradeButtonStack.MaxVisibleIcons)
			{
				count -= UpgradeButtonStack.MaxVisibleIcons - this.desiredStackCount;
				this.desiredStackCount = UpgradeButtonStack.MaxVisibleIcons;
				this.hiddenUpgradeCount = count;
			}
			else
			{
				this.desiredStackCount += count;
			}
			this._baseStackIcon.SetVisible(this.desiredStackCount <= 1, TransitionStyle.Snap);
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x00064870 File Offset: 0x00062A70
		public virtual void RemoveFromStack(int count = 1, bool fromAnimation = false)
		{
			if (this.IsUnlimited && this.AccountedIconNumber >= 1)
			{
				return;
			}
			if (this.hiddenUpgradeCount > 0)
			{
				if (this.hiddenUpgradeCount <= count)
				{
					count -= this.hiddenUpgradeCount;
					this.hiddenUpgradeCount = 0;
				}
				else
				{
					this.hiddenUpgradeCount -= count;
					count = 0;
				}
			}
			if (count > 0 && Diagnostics.Verify(this.desiredStackCount - count >= 0, "We tried to remove more icons from a stack than we have! Trying to remove {0} from {1} on {2}", count, this.desiredStackCount, base.name))
			{
				if (fromAnimation)
				{
					this.PendingAdditionCount += count;
				}
				this.desiredStackCount -= count;
			}
		}

		// Token: 0x06001B8B RID: 7051 RVA: 0x0006491C File Offset: 0x00062B1C
		public virtual void DoStateTransition(ButtonAnimationState state, bool instant)
		{
			this.internalSelectionState = state;
			for (int iconIndex = 0; iconIndex < this._stackedIcons.Count; iconIndex++)
			{
				this._stackedIcons[iconIndex].IsHighlighted = (state == ButtonAnimationState.Hover);
			}
		}

		// Token: 0x06001B8C RID: 7052 RVA: 0x0006495B File Offset: 0x00062B5B
		private void Awake()
		{
			this.desiredStackCount = 0;
			this._baseStackIcon = base.GetComponent<UpgradeIcon>();
		}

		// Token: 0x06001B8D RID: 7053 RVA: 0x00064970 File Offset: 0x00062B70
		private void Update()
		{
			if (this._animating)
			{
				this.SetStackPositions(Easings.ElasticEaseOut(Mathf.Clamp01(this._animationTime)));
				if (this._animationTime > 0.5f)
				{
					if (!this._animatingIconAddition)
					{
						this.RemoveIcon();
					}
					this._animating = false;
					this._animationTime = 0f;
				}
				this._animationTime += 2.5f * Time.deltaTime;
			}
			if (this.desiredStackCount != this.VisibleStackCount && !this._animating)
			{
				if (this.VisibleStackCount < this.desiredStackCount)
				{
					this.AddNewIcon().transform.localScale = Vector3.zero;
					this._animating = true;
					this._animatingIconAddition = true;
				}
				else
				{
					this._animating = true;
					this._animatingIconAddition = false;
				}
				this._baseStackIcon.SetVisible(this.desiredStackCount <= 0, TransitionStyle.Snap);
			}
		}

		// Token: 0x06001B8E RID: 7054 RVA: 0x00064A50 File Offset: 0x00062C50
		private void RemoveIcon()
		{
			UpgradeIcon lastIcon = this._stackedIcons[this._stackedIcons.Count - 1];
			this._stackedIcons.RemoveAt(this._stackedIcons.Count - 1);
			if (this._passiveUpgradeStack != null)
			{
				this._passiveUpgradeStack.RemoveIcon(lastIcon);
			}
			UnityEngine.Object.Destroy(lastIcon.gameObject);
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x00064AB4 File Offset: 0x00062CB4
		private UpgradeIcon AddNewIcon()
		{
			UpgradeIcon newStackIcon = UnityEngine.Object.Instantiate<UpgradeIcon>(this.stackPrefab, base.transform);
			newStackIcon.transform.SetSiblingIndex(1);
			newStackIcon.iconRenderer.sprite = this.referenceImage.sprite;
			newStackIcon.name = "Icon " + this._stackedIcons.Count.ToString();
			if (this.IsCircle)
			{
				newStackIcon.SetToCircle();
			}
			else
			{
				newStackIcon.SetToDiamond();
			}
			this._stackedIcons.Add(newStackIcon);
			newStackIcon.Rect.anchoredPosition = Vector3.right * (float)this._stackedIcons.Count * this.offset;
			newStackIcon.Rect.sizeDelta = base.GetComponent<RectTransform>().sizeDelta;
			newStackIcon.ApplyTheme(this._currentTheme);
			newStackIcon.SetOutlineIndex(this._stackedIcons.Count - 1);
			if (this._passiveUpgradeStack != null)
			{
				this._passiveUpgradeStack.AddIcon(newStackIcon);
			}
			return newStackIcon;
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x00064BBC File Offset: 0x00062DBC
		private void SetStackPositions(float lerpTime)
		{
			if (this._stackedIcons.Count == 0)
			{
				return;
			}
			for (int iconIndex = 0; iconIndex < this._stackedIcons.Count - 1; iconIndex++)
			{
				int previousIndex = this._animatingIconAddition ? (this._stackedIcons.Count - 2 - iconIndex) : (this._stackedIcons.Count - 1 - iconIndex);
				float num = (float)(this._animatingIconAddition ? (this._stackedIcons.Count - 1 - iconIndex) : (this._stackedIcons.Count - 2 - iconIndex));
				float previousPosition = (float)previousIndex * this.offset;
				float desiredPosition = num * this.offset;
				Vector3 newPosition = Vector3.zero;
				newPosition.x = Mathf.Lerp(previousPosition, desiredPosition, lerpTime);
				this._stackedIcons[iconIndex].Rect.anchoredPosition = newPosition;
				this._stackedIcons[iconIndex].SetOutlineIndex(iconIndex);
			}
			Vector3 startScale = this._animatingIconAddition ? Vector3.zero : Vector3.one;
			Vector3 endScale = this._animatingIconAddition ? Vector3.one : Vector3.zero;
			this._stackedIcons[this._stackedIcons.Count - 1].Rect.localScale = Vector3.Lerp(startScale, endScale, lerpTime);
			this._stackedIcons[this._stackedIcons.Count - 1].Rect.anchoredPosition = Vector3.zero;
		}

		// Token: 0x06001B91 RID: 7057 RVA: 0x000022F5 File Offset: 0x000004F5
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x06001B92 RID: 7058 RVA: 0x00064D24 File Offset: 0x00062F24
		public void ApplyTheme(ITheme theme)
		{
			this._currentTheme = (theme as Theme);
			for (int iconIndex = 0; iconIndex < this._stackedIcons.Count; iconIndex++)
			{
				this._stackedIcons[iconIndex].ApplyTheme(theme);
				this._stackedIcons[iconIndex].SetOutlineIndex(iconIndex);
			}
		}

		// Token: 0x06001B93 RID: 7059 RVA: 0x00064D78 File Offset: 0x00062F78
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			ThemeBlendingResult blendingResult = ThemeBlendingResult.StopBlending;
			this._currentTheme = (newTheme as Theme);
			for (int iconIndex = 0; iconIndex < this._stackedIcons.Count; iconIndex++)
			{
				if (this._stackedIcons[iconIndex].ApplyBlendedTheme(oldTheme, newTheme, progress) == ThemeBlendingResult.ContinueBlending)
				{
					blendingResult = ThemeBlendingResult.ContinueBlending;
				}
				this._stackedIcons[iconIndex].SetOutlineIndex(iconIndex);
			}
			return blendingResult;
		}

		// Token: 0x06001B94 RID: 7060 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x040016F3 RID: 5875
		protected int desiredStackCount;

		// Token: 0x040016F4 RID: 5876
		protected int hiddenUpgradeCount;

		// Token: 0x040016F6 RID: 5878
		private List<UpgradeIcon> _stackedIcons = new List<UpgradeIcon>();

		// Token: 0x040016F7 RID: 5879
		public UpgradeIcon stackPrefab;

		// Token: 0x040016F8 RID: 5880
		private float _animationTime;

		// Token: 0x040016F9 RID: 5881
		private const float AnimationDuration = 0.4f;

		// Token: 0x040016FA RID: 5882
		private bool _animating;

		// Token: 0x040016FB RID: 5883
		private bool _animatingIconAddition;

		// Token: 0x040016FC RID: 5884
		private UpgradeIcon _baseStackIcon;

		// Token: 0x040016FD RID: 5885
		public float offset = -10f;

		// Token: 0x040016FE RID: 5886
		[Tooltip("The image that we will copy to put on the stack.")]
		public Image referenceImage;

		// Token: 0x040016FF RID: 5887
		public static int MaxVisibleIcons = 5;

		// Token: 0x04001700 RID: 5888
		public bool IsCircle;

		// Token: 0x04001701 RID: 5889
		public bool ShowNumberCounter = true;

		// Token: 0x04001702 RID: 5890
		protected bool _isUnlimited;

		// Token: 0x04001703 RID: 5891
		protected Theme _currentTheme;

		// Token: 0x04001704 RID: 5892
		private ButtonAnimationState internalSelectionState;

		// Token: 0x04001705 RID: 5893
		[SerializeField]
		private PassiveUpgradeStackIcon _passiveUpgradeStack;
	}
}
