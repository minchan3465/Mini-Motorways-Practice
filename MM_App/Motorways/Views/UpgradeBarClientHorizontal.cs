using System;
using Client;
using Factory;
using Motorways.Audio;
using Motorways.Models;
using Motorways.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x0200057B RID: 1403
	public class UpgradeBarClientHorizontal : UpgradeBarClient, InputState.IObserver
	{
		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x0600267D RID: 9853 RVA: 0x000A383D File Offset: 0x000A1A3D
		public bool IsLocked
		{
			get
			{
				return this._isLocked;
			}
		}

		// Token: 0x0600267E RID: 9854 RVA: 0x000A3845 File Offset: 0x000A1A45
		private float GetTimeUpgradeButtonAppeared(UpgradeType upgradeType)
		{
			if (upgradeType == UpgradeType.Concrete)
			{
				return this._timeConcreteButtonAppeared;
			}
			return this._timeNonConcreteButtonAppeared;
		}

		// Token: 0x0600267F RID: 9855 RVA: 0x000A3857 File Offset: 0x000A1A57
		private void SetTimeUpgradeButtonAppeared(UpgradeType upgradeType, float time)
		{
			if (upgradeType == UpgradeType.Concrete)
			{
				this._timeConcreteButtonAppeared = time;
				return;
			}
			this._timeNonConcreteButtonAppeared = time;
		}

		// Token: 0x06002680 RID: 9856 RVA: 0x000A386C File Offset: 0x000A1A6C
		protected override void OnUpgradeChanged(UpgradeType type, int delta)
		{
			if (delta != 0 && this._hudAnimationsEnabled)
			{
				if (!this._upgradeHasBeenAwarded[(int)type])
				{
					this.MakeNewUpgradeAppear(type);
				}
				if (!this._behaviour.HasUnlimitedOfUpgrade(type))
				{
					this.SetUpgradeButtonVisible(type, true);
				}
			}
			base.OnUpgradeChanged(type, delta);
			this._anchorSizer.UpdateSizing();
		}

		// Token: 0x06002681 RID: 9857 RVA: 0x000A38C0 File Offset: 0x000A1AC0
		public override TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._screenStack.IsScreenInStack(ScreenStack.MotorwaysScreen.Upgrade))
			{
				for (int upgradeTypeIndex = 0; upgradeTypeIndex < 9; upgradeTypeIndex++)
				{
					if (this._upgradeHasBeenAwarded[upgradeTypeIndex])
					{
						this.SetUpgradeButtonVisible((UpgradeType)upgradeTypeIndex, true);
					}
				}
				base.SetCreativeModeColourWidgetVisible(true);
			}
			else
			{
				this.TickUpgradesHud();
			}
			return base.Tick(timeInterval, stepAlpha);
		}

		// Token: 0x06002682 RID: 9858 RVA: 0x000A3914 File Offset: 0x000A1B14
		private void TickUpgradesHud()
		{
			bool isHudUp = this.AreUpgradesShowing();
			if (!this._handleAnchor.IsAnimating)
			{
				this.TickUpgradeHudVisibility(isHudUp);
			}
			this.TickUpgradeHudHitboxes(isHudUp);
			this._lockLineCanvasGroup.alpha = Mathf.Clamp01(this._lockLineCanvasGroup.alpha + Time.deltaTime * (this._pointerOverAppearHitbox ? this.LockLineAlphaSpeed : (-this.LockLineAlphaSpeed)));
		}

		// Token: 0x06002683 RID: 9859 RVA: 0x000A397C File Offset: 0x000A1B7C
		private void TickUpgradeHudVisibility(bool isHudUp)
		{
			if (isHudUp)
			{
				if (!this._isLocked)
				{
					if (!this.PointerInRectTransform(this._rectTransform) && (!this._city.Rules.ShowColourWidget || !this.PointerInRectTransform(this._gameUI.ColourWidget.RectTransform)))
					{
						for (UpgradeType upgradeType = UpgradeType.Concrete; upgradeType < UpgradeType.Count; upgradeType++)
						{
							if (base.IsUpgradeButtonVisible(upgradeType) && Time.time - this.GetTimeUpgradeButtonAppeared(upgradeType) > this.DurationToKeepUpgradeElementsOnScreenAfterUse)
							{
								if (this._upgradeButtonStacks[(int)upgradeType].PendingAdditionCount == 0)
								{
									this.SetUpgradeButtonVisible(upgradeType, false);
								}
								base.SetCreativeModeColourWidgetVisible(false);
							}
						}
						return;
					}
					bool iconPromoted = false;
					for (UpgradeType upgradeType2 = UpgradeType.Concrete; upgradeType2 < UpgradeType.Count; upgradeType2++)
					{
						if (this._upgradeHasBeenAwarded[(int)upgradeType2] && !base.IsUpgradeButtonVisible(upgradeType2))
						{
							this.SetUpgradeButtonVisible(upgradeType2, true);
							iconPromoted = true;
						}
					}
					if (iconPromoted)
					{
						for (UpgradeType upgradeType3 = UpgradeType.Concrete; upgradeType3 < UpgradeType.Count; upgradeType3++)
						{
							if (base.IsUpgradeButtonVisible(upgradeType3))
							{
								this.SetTimeUpgradeButtonAppeared(upgradeType3, Time.time);
							}
						}
						return;
					}
				}
			}
			else if (this._pointerOverAppearHitbox)
			{
				if (Time.time - this._lastTimePointerEnteredAppearHitbox > this.AppearDelayAfterPointerEnter && this._appearHitboxTimerEnabled)
				{
					this.ShowAllAvailableUpgrades(true);
					return;
				}
			}
			else if (!this._appearHitboxTimerEnabled && this._hudAnimationsEnabled && !this.PointerInRectTransform(this._rectTransform) && !this.PointerInRectTransform(this._gameUI.ColourWidget.RectTransform))
			{
				this._appearHitboxTimerEnabled = true;
			}
		}

		// Token: 0x06002684 RID: 9860 RVA: 0x000A3ADC File Offset: 0x000A1CDC
		private void TickUpgradeHudHitboxes(bool isHudUp)
		{
			RectTransform hitboxRect;
			if (isHudUp)
			{
				hitboxRect = this._deactivateHitboxRectTransform;
				this._activateHitboxRectTransform.gameObject.SetActive(false);
				this._deactivateHitboxRectTransform.gameObject.SetActive(true);
			}
			else
			{
				hitboxRect = this._activateHitboxRectTransform;
				this._deactivateHitboxRectTransform.gameObject.SetActive(false);
				this._activateHitboxRectTransform.gameObject.SetActive(true);
			}
			bool pointerOverAppearHitbox = this._pointerOverAppearHitbox;
			this._pointerOverAppearHitbox = (this.PointerInRectTransform(hitboxRect) || (this._city.Rules.ShowColourWidget && this.PointerInRectTransform(this._gameUI.ColourWidget.HitboxRect)));
			if (pointerOverAppearHitbox != this._pointerOverAppearHitbox)
			{
				this._lockButton.interactable = this._pointerOverAppearHitbox;
				if (this._pointerOverAppearHitbox)
				{
					this._lastTimePointerEnteredAppearHitbox = Time.time;
				}
			}
			if (this._pointerOverAppearHitbox)
			{
				this._hudDotButton.animator.SetTrigger(this._hudDotButton.animationTriggers.highlightedTrigger);
				return;
			}
			this._hudDotButton.animator.ResetTrigger(this._hudDotButton.animationTriggers.highlightedTrigger);
			this._hudDotButton.animator.SetTrigger(this._hudDotButton.animationTriggers.normalTrigger);
		}

		// Token: 0x06002685 RID: 9861 RVA: 0x000A3C18 File Offset: 0x000A1E18
		private bool PointerInRectTransform(RectTransform rectTransform)
		{
			Vector2 pointerToUse = this._gameUI.IsFocusPointActive ? this._gameUI.FocusPointPosition : this._inputState.Mouse.Position;
			Vector2 localPoint;
			return RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pointerToUse, Camera.main, out localPoint) && rectTransform.rect.Contains(localPoint);
		}

		// Token: 0x06002686 RID: 9862 RVA: 0x000A3C74 File Offset: 0x000A1E74
		public override void SetUpgradeButtonVisible(UpgradeType type, bool visible)
		{
			if (visible)
			{
				if (type == UpgradeType.Concrete)
				{
					bool showConcrete = !this._behaviour.ShouldHideStaticUpgrades;
					if (showConcrete)
					{
						this.SetTimeUpgradeButtonAppeared(UpgradeType.Concrete, Time.time);
					}
					this._upgradeButtons[(int)type].enabled = showConcrete;
					base.SetUpgradeButtonVisible(type, showConcrete);
					this._floatingUpgradeButtons[(int)type].IsActive = showConcrete;
				}
				else
				{
					for (UpgradeType upgradeType = UpgradeType.Concrete; upgradeType < UpgradeType.Count; upgradeType++)
					{
						int upgradeIndex = (int)upgradeType;
						bool showUpgrade = this._upgradeHasBeenAwarded[upgradeIndex] || type == upgradeType;
						if ((upgradeType == UpgradeType.Concrete || upgradeType == UpgradeType.Bridge || upgradeType == UpgradeType.Tunnel) && this._behaviour.ShouldHideStaticUpgrades)
						{
							showUpgrade = false;
						}
						if (showUpgrade)
						{
							this.SetTimeUpgradeButtonAppeared(upgradeType, Time.time);
							this._upgradeButtons[upgradeIndex].enabled = true;
							base.SetUpgradeButtonVisible(upgradeType, true);
							this._floatingUpgradeButtons[upgradeIndex].IsActive = true;
						}
					}
				}
			}
			else if (type == UpgradeType.Concrete)
			{
				for (UpgradeType upgradeType2 = UpgradeType.Concrete; upgradeType2 < UpgradeType.Count; upgradeType2++)
				{
					this._floatingUpgradeButtons[(int)upgradeType2].IsActive = false;
				}
			}
			else
			{
				for (UpgradeType upgradeType3 = UpgradeType.Bridge; upgradeType3 < UpgradeType.Count; upgradeType3++)
				{
					this._floatingUpgradeButtons[(int)upgradeType3].IsActive = false;
				}
			}
			this.CheckHandlePosition();
		}

		// Token: 0x06002687 RID: 9863 RVA: 0x000A3D8C File Offset: 0x000A1F8C
		public override void AddToUpgradeButtonStack(UpgradeType type, bool fromAnimation = false, int count = 1)
		{
			base.AddToUpgradeButtonStack(type, fromAnimation, count);
			if (fromAnimation)
			{
				this.SetUpgradeButtonVisible(type, true);
			}
			this._anchorSizer.UpdateSizing();
		}

		// Token: 0x06002688 RID: 9864 RVA: 0x000A3DB0 File Offset: 0x000A1FB0
		private void MakeNewUpgradeAppear(UpgradeType type)
		{
			this._upgradeButtonStacks[(int)type].SetCount(0);
			this._upgradeButtons[(int)type].enabled = true;
			this._upgradeButtons[(int)type]._upgradeIcon.SetVisible(false, TransitionStyle.Snap);
			this._upgradeButtons[(int)type]._upgradeIcon.SetVisible(true, TransitionStyle.Tween);
			this.SetUpgradeButtonVisible(type, true);
			Canvas.ForceUpdateCanvases();
			this._floatingUpgradeButtons[(int)type].Snap();
			this._upgradeHasBeenAwarded[(int)type] = true;
		}

		// Token: 0x06002689 RID: 9865 RVA: 0x000A3E24 File Offset: 0x000A2024
		protected override void HideUpgradeButtons()
		{
			base.HideUpgradeButtons();
			for (int baseUpgradeIconIndex = 0; baseUpgradeIconIndex < this._floatingUpgradeButtons.Length; baseUpgradeIconIndex++)
			{
				base.SetUpgradeButtonVisible((UpgradeType)baseUpgradeIconIndex, false);
			}
		}

		// Token: 0x0600268A RID: 9866 RVA: 0x000A3E54 File Offset: 0x000A2054
		public override void SetVisibility(bool isVisible, bool instantly = false)
		{
			if (this._behaviour.HasGotRules())
			{
				bool enableLeftUpgrades = true;
				bool enableCenterUpgrades = !this._behaviour.ShouldHideStaticUpgrades;
				bool enableRightUpgrades = !this._behaviour.ShouldHideStaticUpgrades;
				this._anchorSizer.ToggleUpgradeGroups(enableLeftUpgrades, enableCenterUpgrades, enableRightUpgrades);
			}
			this._entireBar.IsActive = isVisible;
			if (instantly)
			{
				this._entireBar.transform.position = (isVisible ? this._entireBar.baseElement.transform.position : this._entireBar.InactiveAnchor.transform.position);
			}
			if (!this._hudAnimationsEnabled && isVisible)
			{
				this._hudAnimationsEnabled = true;
				if (this._player.DoesHudStartLocked)
				{
					this.ShowAllAvailableUpgrades(false);
					this.OnLockToggled(true, false);
				}
			}
			else if (this._hudAnimationsEnabled && this._isLocked && isVisible)
			{
				this.ShowAllAvailableUpgrades(false);
			}
			base.SetVisibility(isVisible, instantly);
		}

		// Token: 0x0600268B RID: 9867 RVA: 0x000A3F40 File Offset: 0x000A2140
		public override void AddPendingToUpgradeButtonStack(UpgradeType type, int count = 1)
		{
			base.AddPendingToUpgradeButtonStack(type, count);
			if (!this._upgradeHasBeenAwarded[(int)type])
			{
				this.MakeNewUpgradeAppear(type);
			}
			this._anchorSizer.UpdateSizing();
		}

		// Token: 0x0600268C RID: 9868 RVA: 0x000A3F66 File Offset: 0x000A2166
		public override void PulseUpgradeIcon(UpgradeType type)
		{
			base.PulseUpgradeIcon(type);
			this.SetUpgradeButtonVisible(type, true);
		}

		// Token: 0x0600268D RID: 9869 RVA: 0x000A3F78 File Offset: 0x000A2178
		public void CheckHandlePosition()
		{
			for (int upgradeTypeIndex = 0; upgradeTypeIndex < 9; upgradeTypeIndex++)
			{
				if (base.IsUpgradeButtonVisible((UpgradeType)upgradeTypeIndex))
				{
					this._handleAnchor.IsActive = true;
					return;
				}
			}
			this._handleAnchor.IsActive = false;
		}

		// Token: 0x0600268E RID: 9870 RVA: 0x000A3FB4 File Offset: 0x000A21B4
		public void OnLockClicked()
		{
			if (this._handleAnchor.IsAnimating)
			{
				return;
			}
			this.OnLockToggled(!this._isLocked, true);
		}

		// Token: 0x0600268F RID: 9871 RVA: 0x000A3FD4 File Offset: 0x000A21D4
		public void OnLockToggled(bool locked, bool saveLockedStateToProfile = false)
		{
			if (saveLockedStateToProfile)
			{
				this._player.DoesHudStartLocked = locked;
			}
			this._isLocked = locked;
			if (this._isLocked)
			{
				((Image)this._lockButton.targetGraphic).sprite = this._lockButtonLockedSprite;
				this.ShowAllAvailableUpgrades(false);
				return;
			}
			((Image)this._lockButton.targetGraphic).sprite = this._lockButtonUnlockedSprite;
		}

		// Token: 0x06002690 RID: 9872 RVA: 0x000A403D File Offset: 0x000A223D
		public void OnHandleClicked()
		{
			if (this._handleAnchor.IsAnimating)
			{
				return;
			}
			if (this.AreUpgradesShowing())
			{
				this.HideAllUpgrades();
				this.OnLockToggled(false, true);
			}
			else
			{
				this.ShowAllAvailableUpgrades(true);
			}
			this._lastTimePointerEnteredAppearHitbox = Time.time;
		}

		// Token: 0x06002691 RID: 9873 RVA: 0x000A4077 File Offset: 0x000A2277
		public void ShowHud(bool locked)
		{
			this.ShowAllAvailableUpgrades(false);
			if (locked)
			{
				this.OnLockToggled(true, true);
				this._lockLineCanvasGroup.alpha = 1f;
			}
		}

		// Token: 0x06002692 RID: 9874 RVA: 0x000A409B File Offset: 0x000A229B
		public void HideHud()
		{
			this.HideHud(false);
		}

		// Token: 0x06002693 RID: 9875 RVA: 0x000A40A4 File Offset: 0x000A22A4
		public void HideHud(bool saveLockedStateToProfile)
		{
			this.OnLockToggled(false, saveLockedStateToProfile);
			this.HideAllUpgrades();
		}

		// Token: 0x06002694 RID: 9876 RVA: 0x000A40B4 File Offset: 0x000A22B4
		private void ShowAllAvailableUpgrades(bool playSound = false)
		{
			for (int upgradeTypeIndex = 0; upgradeTypeIndex < 9; upgradeTypeIndex++)
			{
				if (this._upgradeHasBeenAwarded[upgradeTypeIndex] && !this._floatingUpgradeButtons[upgradeTypeIndex].IsActive)
				{
					this.SetUpgradeButtonVisible((UpgradeType)upgradeTypeIndex, true);
				}
			}
			base.SetCreativeModeColourWidgetVisible(true);
			if (playSound)
			{
				AudioPlayer ui = AudioPlayer.UI;
				if (ui == null)
				{
					return;
				}
				ui.PlaySample("iso-ui-show-controls", 0.5f, 0.5f, 1f, 0.0, -1.0, false, null, false, false, 0f, false);
			}
		}

		// Token: 0x06002695 RID: 9877 RVA: 0x000A413C File Offset: 0x000A233C
		private void HideAllUpgrades()
		{
			base.SetCreativeModeColourWidgetVisible(false);
			for (int upgradeTypeIndex = 0; upgradeTypeIndex < 9; upgradeTypeIndex++)
			{
				this.SetUpgradeButtonVisible((UpgradeType)upgradeTypeIndex, false);
			}
			AudioPlayer ui = AudioPlayer.UI;
			if (ui == null)
			{
				return;
			}
			ui.PlaySample("iso-ui-hide-controls", 0.5f, 0.5f, 1f, 0.0, -1.0, false, null, false, false, 0f, false);
		}

		// Token: 0x06002696 RID: 9878 RVA: 0x000A41A8 File Offset: 0x000A23A8
		public bool AreUpgradesShowing()
		{
			for (UpgradeType upgradeType = UpgradeType.Concrete; upgradeType < UpgradeType.Count; upgradeType++)
			{
				if (base.IsUpgradeButtonVisible(upgradeType))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002697 RID: 9879 RVA: 0x000A41D0 File Offset: 0x000A23D0
		public override void OnCreatedInScope(IScope scope)
		{
			base.OnCreatedInScope(scope);
			scope.Get<ColourWidget>().FloatingElement.IsActive = false;
			scope.Get<ColourWidget>().FloatingElement.baseElement.SetActive(false);
			scope.Get<ColourWidget>().FloatingElement.InactiveAnchor.SetActive(false);
			FloatingElement[] floatingUpgradeButtons = this._floatingUpgradeButtons;
			for (int i = 0; i < floatingUpgradeButtons.Length; i++)
			{
				floatingUpgradeButtons[i].IsActive = false;
			}
			this.OnLockToggled(false, false);
			this._lockLineCanvasGroup.alpha = 0f;
			this._hudAnimationsEnabled = false;
			this._lastTimePointerEnteredAppearHitbox = 0f;
			this._appearHitboxTimerEnabled = false;
			this._inputState.Subscribe(this);
			this.RefreshHudAnchor();
			this._anchorSizer.Initialize(scope);
		}

		// Token: 0x06002698 RID: 9880 RVA: 0x000A4290 File Offset: 0x000A2490
		public override void OnReleasedFromScope(IScope scope)
		{
			base.OnReleasedFromScope(scope);
			FloatingElement[] floatingUpgradeButtons = this._floatingUpgradeButtons;
			for (int i = 0; i < floatingUpgradeButtons.Length; i++)
			{
				floatingUpgradeButtons[i].IsActive = false;
			}
			this.OnLockToggled(false, false);
			this._lockLineCanvasGroup.alpha = 0f;
			this._timeConcreteButtonAppeared = 0f;
			this._timeNonConcreteButtonAppeared = 0f;
			this._hudAnimationsEnabled = false;
			this._lastTimePointerEnteredAppearHitbox = 0f;
			this._appearHitboxTimerEnabled = false;
			this._inputState.Unsubscribe(this);
		}

		// Token: 0x06002699 RID: 9881 RVA: 0x000A4316 File Offset: 0x000A2516
		public void OnCurrentDeviceInputTypeChanged(DeviceInputType newInputType)
		{
			this.RefreshHudAnchor();
		}

		// Token: 0x0600269A RID: 9882 RVA: 0x000A431E File Offset: 0x000A251E
		private void RefreshHudAnchor()
		{
			if (this._inputState.CurrentDeviceInputType == DeviceInputType.Remote)
			{
				this._handleAnchor.gameObject.SetActive(false);
				this.OnLockToggled(true, false);
				return;
			}
			this._handleAnchor.gameObject.SetActive(true);
		}

		// Token: 0x04002074 RID: 8308
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04002075 RID: 8309
		[Dependency]
		protected InputState _inputState;

		// Token: 0x04002076 RID: 8310
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x04002077 RID: 8311
		[Dependency]
		private City _city;

		// Token: 0x04002078 RID: 8312
		private float _timeConcreteButtonAppeared;

		// Token: 0x04002079 RID: 8313
		private float _timeNonConcreteButtonAppeared;

		// Token: 0x0400207A RID: 8314
		[SerializeField]
		private FloatingElement _entireBar;

		// Token: 0x0400207B RID: 8315
		[SerializeField]
		private FloatingElement _handleAnchor;

		// Token: 0x0400207C RID: 8316
		[SerializeField]
		private UpgradeBarHorizontalAnchorSizer _anchorSizer;

		// Token: 0x0400207D RID: 8317
		[SerializeField]
		private TouchToggle _lockButton;

		// Token: 0x0400207E RID: 8318
		[SerializeField]
		private Sprite _lockButtonLockedSprite;

		// Token: 0x0400207F RID: 8319
		[SerializeField]
		private Sprite _lockButtonUnlockedSprite;

		// Token: 0x04002080 RID: 8320
		[SerializeField]
		private CanvasGroup _lockLineCanvasGroup;

		// Token: 0x04002081 RID: 8321
		[SerializeField]
		private TouchButton _hudDotButton;

		// Token: 0x04002082 RID: 8322
		private bool _isLocked;

		// Token: 0x04002083 RID: 8323
		[SerializeField]
		private float DurationToKeepUpgradeElementsOnScreenAfterUse = 3f;

		// Token: 0x04002084 RID: 8324
		[SerializeField]
		private float LockLineAlphaSpeed = 2f;

		// Token: 0x04002085 RID: 8325
		[SerializeField]
		private float AppearDelayAfterPointerEnter = 1f;

		// Token: 0x04002086 RID: 8326
		[SerializeField]
		private RectTransform _rectTransform;

		// Token: 0x04002087 RID: 8327
		[SerializeField]
		private RectTransform _activateHitboxRectTransform;

		// Token: 0x04002088 RID: 8328
		[SerializeField]
		private RectTransform _deactivateHitboxRectTransform;

		// Token: 0x04002089 RID: 8329
		private float _lastTimePointerEnteredAppearHitbox;

		// Token: 0x0400208A RID: 8330
		private bool _appearHitboxTimerEnabled;

		// Token: 0x0400208B RID: 8331
		private bool _pointerOverAppearHitbox;

		// Token: 0x0400208C RID: 8332
		private bool _hudAnimationsEnabled;
	}
}
