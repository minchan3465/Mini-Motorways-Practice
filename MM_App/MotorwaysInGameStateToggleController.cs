using System;
using Factory;
using Motorways.Views;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001A4 RID: 420
public class MotorwaysInGameStateToggleController : MotorwaysMenuNavigation
{
	// Token: 0x1700020F RID: 527
	// (get) Token: 0x06000967 RID: 2407 RVA: 0x0001EFD5 File Offset: 0x0001D1D5
	// (set) Token: 0x06000968 RID: 2408 RVA: 0x0001EFDD File Offset: 0x0001D1DD
	public MotorwaysInGameStateToggleController.InGameControllerState ControllerState { get; protected set; }

	// Token: 0x06000969 RID: 2409 RVA: 0x0001EFE8 File Offset: 0x0001D1E8
	public static void SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState newState, IScope scope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour actionBehaviour = MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions)
	{
		MotorwaysMenuNavigation menuNavigation = scope.Get<MenuNavigation>() as MotorwaysMenuNavigation;
		if (menuNavigation != null && typeof(MotorwaysInGameStateToggleController).IsAssignableFrom(menuNavigation.GetType()))
		{
			((MotorwaysInGameStateToggleController)menuNavigation).SwitchToState(newState, scope, actionBehaviour);
		}
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x0001F02C File Offset: 0x0001D22C
	public virtual void SwitchToState(MotorwaysInGameStateToggleController.InGameControllerState newState, IScope scope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour actionBehaviour = MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions)
	{
		if (newState != MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles)
		{
			if (newState == MotorwaysInGameStateToggleController.InGameControllerState.SelectingUpgrades)
			{
				GameUIScreen gameUIScreen = scope.Get<GameUIScreen>();
				if (Diagnostics.Verify(gameUIScreen != null))
				{
					Selectable selectable = gameUIScreen.GetFirstUpgradeIconSelectable();
					if (selectable != null)
					{
						this.SetNewFocus(selectable);
					}
					else
					{
						newState = this.ControllerState;
					}
				}
			}
		}
		else
		{
			this.ReleaseUIFocus();
		}
		if (this.ControllerState != newState)
		{
			this.ControllerState = newState;
			if (actionBehaviour == MotorwaysInGameStateToggleController.StateSwapActionBehaviour.CancelActions)
			{
				this._actionController.CancelAllActions();
			}
		}
		this._inputState.MaxRecognizedTouchCount = ((newState == MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles) ? 2 : 1);
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x0001F0B4 File Offset: 0x0001D2B4
	public override bool MoveCursor(Vector2 direction)
	{
		bool newFocusFound = base.MoveCursor(direction);
		if (!newFocusFound && direction.x > this.menuNavigationSwipeThreshold && this.ControllerState == MotorwaysInGameStateToggleController.InGameControllerState.SelectingUpgrades)
		{
			this.SwitchToState(MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles, this._scope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions);
			return true;
		}
		return newFocusFound;
	}

	// Token: 0x040004F0 RID: 1264
	[Dependency]
	private PlayerActionController _actionController;

	// Token: 0x040004F1 RID: 1265
	[Dependency]
	private InputState _inputState;

	// Token: 0x020001A5 RID: 421
	public enum InGameControllerState
	{
		// Token: 0x040004F4 RID: 1268
		OutOfGame,
		// Token: 0x040004F5 RID: 1269
		EditingTiles,
		// Token: 0x040004F6 RID: 1270
		SelectingUpgrades,
		// Token: 0x040004F7 RID: 1271
		InGameOverlayScreen,
		// Token: 0x040004F8 RID: 1272
		PauseScreen,
		// Token: 0x040004F9 RID: 1273
		EditMenu
	}

	// Token: 0x020001A6 RID: 422
	public enum StateSwapActionBehaviour
	{
		// Token: 0x040004FB RID: 1275
		MaintainActions,
		// Token: 0x040004FC RID: 1276
		CancelActions
	}
}
