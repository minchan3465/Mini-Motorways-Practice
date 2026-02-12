using System;
using Factory;
using Motorways.Actions;
using Motorways.Views;

// Token: 0x0200018D RID: 397
public class ChangeUpgradeBarAction : MotorwaysPlayerAction
{
	// Token: 0x06000903 RID: 2307 RVA: 0x0001D7C0 File Offset: 0x0001B9C0
	public override void OnActionBegin(float timestamp)
	{
		base.OnActionBegin(timestamp);
		UpgradeBarClientHorizontal upgradeBarHorizontal = this._gameUI.UpgradeBar as UpgradeBarClientHorizontal;
		if (upgradeBarHorizontal != null)
		{
			switch (this._visibilityState)
			{
			case ChangeUpgradeBarAction.VisibilityState.Down:
				upgradeBarHorizontal.HideHud(true);
				this.SetColourWidgetRadialVisible(false);
				return;
			case ChangeUpgradeBarAction.VisibilityState.Up:
				upgradeBarHorizontal.ShowHud(false);
				this.SetColourWidgetRadialVisible(true);
				return;
			case ChangeUpgradeBarAction.VisibilityState.UpLocked:
				upgradeBarHorizontal.ShowHud(true);
				this.SetColourWidgetRadialVisible(true);
				return;
			default:
				Diagnostics.FailAssert("Unexpected ChangeUpgradeBarAction.State: {0}. Has someone forgotten to update this switch statement?", new object[]
				{
					this._visibilityState
				});
				break;
			}
		}
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x000020A2 File Offset: 0x000002A2
	public override void Tick(float frameTime)
	{
		this.OnActionComplete();
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x0001D84E File Offset: 0x0001BA4E
	public override void Reset()
	{
		base.Reset();
		this._visibilityState = ChangeUpgradeBarAction.VisibilityState.Down;
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x0001D85D File Offset: 0x0001BA5D
	private static ChangeUpgradeBarAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		ChangeUpgradeBarAction changeUpgradeBarAction = scope.Get<ChangeUpgradeBarAction>();
		changeUpgradeBarAction.InitializeAction(owningGroup, timestamp);
		return changeUpgradeBarAction;
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x0001D86D File Offset: 0x0001BA6D
	private static ChangeUpgradeBarAction.VisibilityState CurrentUpgradeBarState(UpgradeBarClientHorizontal upgradeBar)
	{
		if (!upgradeBar.AreUpgradesShowing())
		{
			return ChangeUpgradeBarAction.VisibilityState.Down;
		}
		if (upgradeBar.IsLocked)
		{
			return ChangeUpgradeBarAction.VisibilityState.UpLocked;
		}
		return ChangeUpgradeBarAction.VisibilityState.Up;
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x0001D884 File Offset: 0x0001BA84
	public static ChangeUpgradeBarAction CreateShowOrLockUpgradeBar(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		ChangeUpgradeBarAction newAction = ChangeUpgradeBarAction.Create(owningGroup, scope, timestamp);
		UpgradeBarClientHorizontal upgradeBarHorizontal = newAction._gameUI.UpgradeBar as UpgradeBarClientHorizontal;
		if (upgradeBarHorizontal != null)
		{
			ChangeUpgradeBarAction.VisibilityState currentVisibilityState = ChangeUpgradeBarAction.CurrentUpgradeBarState(upgradeBarHorizontal);
			switch (currentVisibilityState)
			{
			case ChangeUpgradeBarAction.VisibilityState.Down:
				newAction._visibilityState = ChangeUpgradeBarAction.VisibilityState.Up;
				break;
			case ChangeUpgradeBarAction.VisibilityState.Up:
				newAction._visibilityState = ChangeUpgradeBarAction.VisibilityState.UpLocked;
				break;
			case ChangeUpgradeBarAction.VisibilityState.UpLocked:
				newAction._visibilityState = ChangeUpgradeBarAction.VisibilityState.UpLocked;
				break;
			default:
				Diagnostics.FailAssert("Unexpected ChangeUpgradeBarAction.VisibilityState: {0}. Has someone forgotten to update this switch statement?", new object[]
				{
					currentVisibilityState
				});
				break;
			}
		}
		newAction.OnActionBegin(timestamp);
		return newAction;
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x0001D905 File Offset: 0x0001BB05
	public static ChangeUpgradeBarAction CreateHideUpgradeBar(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		ChangeUpgradeBarAction changeUpgradeBarAction = ChangeUpgradeBarAction.Create(owningGroup, scope, timestamp);
		changeUpgradeBarAction._visibilityState = ChangeUpgradeBarAction.VisibilityState.Down;
		changeUpgradeBarAction.OnActionBegin(timestamp);
		return changeUpgradeBarAction;
	}

	// Token: 0x0400047F RID: 1151
	private ChangeUpgradeBarAction.VisibilityState _visibilityState;

	// Token: 0x0200018E RID: 398
	private enum VisibilityState
	{
		// Token: 0x04000481 RID: 1153
		Down,
		// Token: 0x04000482 RID: 1154
		Up,
		// Token: 0x04000483 RID: 1155
		UpLocked
	}
}
