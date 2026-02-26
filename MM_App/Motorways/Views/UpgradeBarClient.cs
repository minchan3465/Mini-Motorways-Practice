using System;
using Client;
using Factory;
using Motorways.Models;
using Motorways.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x0200057A RID: 1402
	public class UpgradeBarClient : MonoBehaviour, IView, ICreatedInScopeHandler, IReleasedFromScopeHandler
	{
		// Token: 0x06002661 RID: 9825 RVA: 0x000A2F58 File Offset: 0x000A1158
		public void DeselectButtons()
		{
			UpgradeButton[] upgradeButtons = this._upgradeButtons;
			for (int i = 0; i < upgradeButtons.Length; i++)
			{
				upgradeButtons[i].ClearSelectionState();
			}
			UpgradeButtonStack[] upgradeButtonStacks = this._upgradeButtonStacks;
			for (int i = 0; i < upgradeButtonStacks.Length; i++)
			{
				upgradeButtonStacks[i].DoStateTransition(ButtonAnimationState.Normal, false);
			}
		}

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x06002662 RID: 9826 RVA: 0x000A2FA1 File Offset: 0x000A11A1
		// (set) Token: 0x06002663 RID: 9827 RVA: 0x000A2FA9 File Offset: 0x000A11A9
		public bool IsVisible { get; protected set; }

		// Token: 0x06002664 RID: 9828 RVA: 0x000A2FB4 File Offset: 0x000A11B4
		public virtual TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (!this._hasDoneRuleBasedInitialization && this._behaviour.HasGotRules())
			{
				for (int upgradeTypeIndex = 0; upgradeTypeIndex < 9; upgradeTypeIndex++)
				{
					UpgradeType upgradeType = (UpgradeType)upgradeTypeIndex;
					if (this._behaviour.HasUnlimitedOfUpgrade(upgradeType))
					{
						this._upgradeHasBeenAwarded[upgradeTypeIndex] = true;
					}
					int upgradeStacksCount = this._upgradeButtonStacks[upgradeTypeIndex].AccountedIconNumber;
					int upgradeDelta = this._clientUpgrades.GetAvailableOrDraftUpgradeCount((UpgradeType)upgradeTypeIndex) - upgradeStacksCount;
					this.OnUpgradeChanged(upgradeType, upgradeDelta);
				}
				this._hasDoneRuleBasedInitialization = true;
			}
			for (int upgradeTypeIndex2 = 0; upgradeTypeIndex2 < 9; upgradeTypeIndex2++)
			{
				int upgradeStacksCount2 = this._upgradeButtonStacks[upgradeTypeIndex2].AccountedIconNumber;
				int upgradeDelta2 = this._clientUpgrades.GetAvailableOrDraftUpgradeCount((UpgradeType)upgradeTypeIndex2) - upgradeStacksCount2;
				this.OnUpgradeChanged((UpgradeType)upgradeTypeIndex2, upgradeDelta2);
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002665 RID: 9829 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002666 RID: 9830 RVA: 0x000A3068 File Offset: 0x000A1268
		public void RefreshAllAvailableUpgradeStacks()
		{
			for (int upgradeTypeIndex = 0; upgradeTypeIndex < 9; upgradeTypeIndex++)
			{
				UpgradeType type = (UpgradeType)upgradeTypeIndex;
				this._upgradeButtonStacks[upgradeTypeIndex].IsUnlimited = this._behaviour.HasUnlimitedOfUpgrade(type);
			}
		}

		// Token: 0x06002667 RID: 9831 RVA: 0x000A30A0 File Offset: 0x000A12A0
		protected virtual void OnUpgradeChanged(UpgradeType type, int delta)
		{
			if (this._behaviour.ShouldHideStaticUpgrades && (type == UpgradeType.Concrete || type == UpgradeType.Bridge || type == UpgradeType.Tunnel))
			{
				return;
			}
			this._upgradeButtonStacks[(int)type].IsUnlimited = this._behaviour.HasUnlimitedOfUpgrade(type);
			this._upgradeButtonStacks[(int)type].ShowNumberCounter = this._behaviour.ShouldShowUpgradeCount();
			this._upgradeHasBeenAwarded[(int)type] |= (this._clientUpgrades.GetTotalUpgradeCount(type) > 0);
			if (this._upgradeHasBeenAwarded[(int)type] && !this._floatingUpgradeButtons[(int)type].BaseElementActive)
			{
				this.SetUpgradeButtonVisible(type, this.IsVisible);
				this.SetCreativeModeColourWidgetVisible(this.IsVisible);
				if (this._upgradeButtons[(int)type] != null)
				{
					this._upgradeButtons[(int)type].enabled = true;
				}
				if (this._dividerObject != null)
				{
					this._dividerObject.SetActive(this.IsVisible && this.HasBeenAwardedPlaceableAsset && !this._behaviour.ShouldHideStaticUpgrades);
				}
			}
			if (delta > 0)
			{
				this._upgradeButtonStacks[(int)type].AddToStack(delta, false);
				return;
			}
			if (delta < 0)
			{
				if (this._upgradeButtonStacks[(int)type].PendingAdditionCount > 0)
				{
					this._upgradeButtonStacks[(int)type].PendingAdditionCount -= Math.Abs(delta);
					return;
				}
				this._upgradeButtonStacks[(int)type].RemoveFromStack(Math.Abs(delta), false);
			}
		}

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x06002668 RID: 9832 RVA: 0x000A31FC File Offset: 0x000A13FC
		private bool HasBeenAwardedPlaceableAsset
		{
			get
			{
				return this._upgradeHasBeenAwarded[4] || this._upgradeHasBeenAwarded[2] || this._upgradeHasBeenAwarded[3];
			}
		}

		// Token: 0x06002669 RID: 9833 RVA: 0x000A321C File Offset: 0x000A141C
		public void OnAssetButtonPressed(float pressTime, GameUIButtonType upgradeType, int pointerIndex, IController onController)
		{
			UpgradeBarClient.Log.Info("OnAssetButtonPressed, from pointerIndex {0}", new object[]
			{
				pointerIndex
			});
			if (Diagnostics.Verify(this._scope != null))
			{
				float inputTime = pressTime;
				InputEvent assetButtonPressEvent;
				if (onController != null)
				{
					assetButtonPressEvent = MotorwaysUIInputEvent.CreateGenericUIEvent(this._scope, 2, onController.GetInputSource(), InputEventButtonState.JustDown, upgradeType, 0);
					inputTime = (float)this._clockModel.Time;
				}
				else if (pointerIndex < 0)
				{
					assetButtonPressEvent = MotorwaysUIInputEvent.CreateMouseUIEvent(this._scope, (InputEventMouseButtonType)(-pointerIndex - 1), InputEventButtonState.JustDown, upgradeType, 0);
				}
				else
				{
					assetButtonPressEvent = MotorwaysUIInputEvent.CreateTouchUIEvent(this._scope, pointerIndex, InputEventButtonState.JustDown, upgradeType, 0);
				}
				this._playerActionController.OnInputEvent(inputTime, assetButtonPressEvent);
			}
		}

		// Token: 0x0600266A RID: 9834 RVA: 0x000A32C0 File Offset: 0x000A14C0
		public void CreateAlertOnUpgradeButton(UpgradeType upgradeButtonType)
		{
			AlertView.Create(this._viewClient, this._floatingUpgradeButtons[(int)upgradeButtonType].baseElement.transform.position, new Color?(this._theme.GetGlobalColor(this._constants.UpgradeAlertColor)), new float?(this.UpgradeAlertSize), new float?(1f), new float?(this.AlertAlpha));
		}

		// Token: 0x0600266B RID: 9835 RVA: 0x000A332C File Offset: 0x000A152C
		public virtual void OnCreatedInScope(IScope scope)
		{
			this._hasDoneRuleBasedInitialization = false;
			foreach (UpgradeButton button in this._upgradeButtons)
			{
				if (button != null)
				{
					UpgradeBarClient.Log.Info("Binding press event for {0}", new object[]
					{
						button.buttonType
					});
					UpgradeButton upgradeButton = button;
					upgradeButton.onPressed = (UpgradeButton.OnAssetButtonPressed)Delegate.Combine(upgradeButton.onPressed, new UpgradeButton.OnAssetButtonPressed(this.OnAssetButtonPressed));
				}
			}
			this.HideUpgradeButtons();
			this.SetVisibility(false, true);
		}

		// Token: 0x0600266C RID: 9836 RVA: 0x000A33B8 File Offset: 0x000A15B8
		public virtual void OnReleasedFromScope(IScope scope)
		{
			foreach (UpgradeButton button in this._upgradeButtons)
			{
				if (button != null)
				{
					UpgradeBarClient.Log.Info("Unbinding press event for {0}", new object[]
					{
						button.buttonType
					});
					UpgradeButton upgradeButton = button;
					upgradeButton.onPressed = (UpgradeButton.OnAssetButtonPressed)Delegate.Remove(upgradeButton.onPressed, new UpgradeButton.OnAssetButtonPressed(this.OnAssetButtonPressed));
				}
			}
			this.IsVisible = false;
			this.HideUpgradeButtons();
			this.SetVisibility(false, true);
			for (int stackIndex = 0; stackIndex < this._upgradeButtonStacks.Length; stackIndex++)
			{
				this._upgradeButtonStacks[stackIndex].SetCount(0);
				this._upgradeButtonStacks[stackIndex].IsUnlimited = false;
			}
			Array.Clear(this._upgradeHasBeenAwarded, 0, this._upgradeHasBeenAwarded.Length);
		}

		// Token: 0x0600266D RID: 9837 RVA: 0x000A3484 File Offset: 0x000A1684
		public virtual void SetVisibility(bool isVisible, bool instantly = false)
		{
			this.IsVisible = isVisible;
			if (instantly)
			{
				for (int baseUpgradeIconIndex = 0; baseUpgradeIconIndex < this._floatingUpgradeButtons.Length; baseUpgradeIconIndex++)
				{
					bool isNowVisible = this.IsVisible && this._upgradeHasBeenAwarded[baseUpgradeIconIndex];
					this.SetUpgradeButtonVisible((UpgradeType)baseUpgradeIconIndex, isNowVisible);
				}
			}
			this.SetCreativeModeColourWidgetVisible(isVisible);
			if (this._dividerObject != null)
			{
				this._dividerObject.SetActive(!this._behaviour.ShouldHideStaticUpgrades && this.IsVisible && this.HasBeenAwardedPlaceableAsset);
				this._dividerObjectInactive.SetActive(!this._behaviour.ShouldHideStaticUpgrades);
				this._concreteSpacer.SetActive(!this._behaviour.ShouldHideStaticUpgrades);
				this._concreteSpacerInactive.SetActive(!this._behaviour.ShouldHideStaticUpgrades);
				this._bridgeSpacer.SetActive(!this._behaviour.ShouldHideStaticUpgrades);
				this._bridgeSpacerInactive.SetActive(!this._behaviour.ShouldHideStaticUpgrades);
			}
		}

		// Token: 0x0600266E RID: 9838 RVA: 0x000A3588 File Offset: 0x000A1788
		public void SetCreativeModeColourWidgetVisible(bool visible)
		{
			if (this._city.Rules != null && this._city.Rules.ShowColourWidget)
			{
				this._gameUI.ColourWidget.FloatingElement.baseElement.SetActive(visible);
				this._gameUI.ColourWidget.FloatingElement.InactiveAnchor.SetActive(visible);
			}
		}

		// Token: 0x0600266F RID: 9839 RVA: 0x000A35EC File Offset: 0x000A17EC
		protected virtual void HideUpgradeButtons()
		{
			for (int baseUpgradeIconIndex = 0; baseUpgradeIconIndex < this._floatingUpgradeButtons.Length; baseUpgradeIconIndex++)
			{
				if (this._floatingUpgradeButtons[baseUpgradeIconIndex] != null)
				{
					this.SetUpgradeButtonVisible((UpgradeType)baseUpgradeIconIndex, false);
					if (this._upgradeButtons[baseUpgradeIconIndex] != null)
					{
						this._upgradeButtons[baseUpgradeIconIndex].enabled = false;
					}
				}
			}
			this.SetCreativeModeColourWidgetVisible(false);
			if (this._dividerObject != null)
			{
				this._dividerObject.SetActive(false);
			}
		}

		// Token: 0x06002670 RID: 9840 RVA: 0x000A3664 File Offset: 0x000A1864
		public virtual void SetUpgradeButtonVisible(UpgradeType type, bool visible)
		{
			if ((type == UpgradeType.Concrete || type == UpgradeType.Bridge || type == UpgradeType.Tunnel) && this._behaviour.ShouldHideStaticUpgrades)
			{
				visible = false;
			}
			this._floatingUpgradeButtons[(int)type].baseElement.SetActive(visible);
			this._floatingUpgradeButtons[(int)type].InactiveAnchor.SetActive(visible);
		}

		// Token: 0x06002671 RID: 9841 RVA: 0x000A36B2 File Offset: 0x000A18B2
		protected bool IsUpgradeButtonVisible(UpgradeType type)
		{
			return this._floatingUpgradeButtons[(int)type].IsActive;
		}

		// Token: 0x06002672 RID: 9842 RVA: 0x000A36C1 File Offset: 0x000A18C1
		public virtual void AddToUpgradeButtonStack(UpgradeType type, bool fromAnimation = false, int count = 1)
		{
			this._upgradeButtonStacks[(int)type].AddToStack(count, fromAnimation);
			this._upgradeHasBeenAwarded[(int)type] = true;
		}

		// Token: 0x06002673 RID: 9843 RVA: 0x000A36DB File Offset: 0x000A18DB
		public virtual void AddPendingToUpgradeButtonStack(UpgradeType type, int count = 1)
		{
			this._upgradeButtonStacks[(int)type].PendingAdditionCount += count;
		}

		// Token: 0x06002674 RID: 9844 RVA: 0x000A36F2 File Offset: 0x000A18F2
		public virtual void RemoveFromUpgradeButtonStack(UpgradeType type, bool fromAnimation = false)
		{
			this._upgradeButtonStacks[(int)type].RemoveFromStack(1, fromAnimation);
		}

		// Token: 0x06002675 RID: 9845 RVA: 0x000A3704 File Offset: 0x000A1904
		public Selectable GetFirstUpgradeIconSelectable()
		{
			int indexInto = -1;
			for (int upgradeIndex = 0; upgradeIndex < 9; upgradeIndex++)
			{
				if (this._upgradeButtons[upgradeIndex] != null && this._floatingUpgradeButtons[upgradeIndex].BaseElementActive)
				{
					indexInto = upgradeIndex;
					break;
				}
			}
			if (indexInto == -1)
			{
				return null;
			}
			return this._upgradeButtons[indexInto].GetComponent<TouchButton>();
		}

		// Token: 0x06002676 RID: 9846 RVA: 0x000A3756 File Offset: 0x000A1956
		public virtual void PulseUpgradeIcon(UpgradeType type)
		{
			this._upgradeButtonStacks[(int)type].GetTopIcon().Pulse();
		}

		// Token: 0x06002677 RID: 9847 RVA: 0x000A376C File Offset: 0x000A196C
		public virtual void BounceUpgrade(UpgradeType type)
		{
			if (!this._floatingUpgradeButtons[(int)type].IsActive)
			{
				this.SetUpgradeButtonVisible(type, true);
			}
			UpgradeButtonCount count = this._upgradeButtonStacks[(int)type] as UpgradeButtonCount;
			if (count != null)
			{
				count.Bounce();
			}
		}

		// Token: 0x06002678 RID: 9848 RVA: 0x000A37A7 File Offset: 0x000A19A7
		public RectTransform GetRectTransformForUpgrade(UpgradeType type)
		{
			return this._floatingUpgradeButtons[(int)type].GetComponent<RectTransform>();
		}

		// Token: 0x06002679 RID: 9849 RVA: 0x000A37B6 File Offset: 0x000A19B6
		public Sprite GetSpriteForUpgradeType(UpgradeType type)
		{
			return this._upgradeButtonStacks[(int)type].referenceImage.sprite;
		}

		// Token: 0x0600267A RID: 9850 RVA: 0x000A37CA File Offset: 0x000A19CA
		public bool IsSpriteForUpgradeACircle(UpgradeType type)
		{
			return this._upgradeButtonStacks[(int)type].IsCircle;
		}

		// Token: 0x0400205A RID: 8282
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("AssetBarClient");

		// Token: 0x0400205B RID: 8283
		[Dependency]
		private IScope _scope;

		// Token: 0x0400205C RID: 8284
		[Dependency]
		private City _city;

		// Token: 0x0400205D RID: 8285
		[Dependency]
		protected GameUIScreen _gameUI;

		// Token: 0x0400205E RID: 8286
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x0400205F RID: 8287
		[Dependency]
		private ClockModel _clockModel;

		// Token: 0x04002060 RID: 8288
		[Dependency]
		protected ClientUpgradeDatabase _clientUpgrades;

		// Token: 0x04002061 RID: 8289
		[Dependency]
		private PlayerActionController _playerActionController;

		// Token: 0x04002062 RID: 8290
		[Dependency]
		private MotorwaysThemeDatabase _theme;

		// Token: 0x04002063 RID: 8291
		[Dependency]
		private VisualConstantsData _constants;

		// Token: 0x04002064 RID: 8292
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x04002065 RID: 8293
		[Dependency]
		protected ScreenStack _screenStack;

		// Token: 0x04002066 RID: 8294
		[EnumTypedArray(typeof(UpgradeType))]
		[SerializeField]
		[FormerlySerializedAs("upgradeButtons")]
		[NonReorderable]
		protected UpgradeButton[] _upgradeButtons;

		// Token: 0x04002067 RID: 8295
		[NonReorderable]
		[FormerlySerializedAs("upgradeButtonStacks")]
		[EnumTypedArray(typeof(UpgradeType))]
		[SerializeField]
		protected UpgradeButtonStack[] _upgradeButtonStacks = new UpgradeButtonStack[9];

		// Token: 0x04002068 RID: 8296
		[NonReorderable]
		[SerializeField]
		[EnumTypedArray(typeof(UpgradeType))]
		protected FloatingElement[] _floatingUpgradeButtons = new FloatingElement[9];

		// Token: 0x04002069 RID: 8297
		[EnumTypedArray(typeof(UpgradeType))]
		[NonSerialized]
		protected bool[] _upgradeHasBeenAwarded = new bool[9];

		// Token: 0x0400206A RID: 8298
		[SerializeField]
		private GameObject _dividerObject;

		// Token: 0x0400206B RID: 8299
		[SerializeField]
		private GameObject _dividerObjectInactive;

		// Token: 0x0400206C RID: 8300
		[SerializeField]
		private GameObject _concreteSpacer;

		// Token: 0x0400206D RID: 8301
		[SerializeField]
		private GameObject _concreteSpacerInactive;

		// Token: 0x0400206E RID: 8302
		[SerializeField]
		private GameObject _bridgeSpacer;

		// Token: 0x0400206F RID: 8303
		[SerializeField]
		private GameObject _bridgeSpacerInactive;

		// Token: 0x04002070 RID: 8304
		public float UpgradeAlertSize = 4f;

		// Token: 0x04002071 RID: 8305
		public float AlertAlpha = 0.6f;

		// Token: 0x04002072 RID: 8306
		private bool _hasDoneRuleBasedInitialization;
	}
}
