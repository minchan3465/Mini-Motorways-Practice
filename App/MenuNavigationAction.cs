using System;
using Factory;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class MenuNavigationAction : PlayerAction
{
	// Token: 0x17000002 RID: 2
	// (get) Token: 0x06000009 RID: 9 RVA: 0x000020AA File Offset: 0x000002AA
	public override bool IsInterruptible
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600000A RID: 10 RVA: 0x000020B8 File Offset: 0x000002B8
	public override void OnActionBegin(float timestamp)
	{
		base.OnActionBegin(timestamp);
		switch (this._action)
		{
		case MenuNavigationAction.NavigationAction.AccumulateMove:
			this._menuNavigation.AccumulateMove(this._direction);
			return;
		case MenuNavigationAction.NavigationAction.ResetAccumulated:
			break;
		case MenuNavigationAction.NavigationAction.MoveCursor:
			this._menuNavigation.MoveCursor(this._direction);
			return;
		case MenuNavigationAction.NavigationAction.ActivateSelected:
			if (this._menuNavigation.ActivateSelected())
			{
				base.MakeExclusive();
				return;
			}
			break;
		case MenuNavigationAction.NavigationAction.BackSelected:
			this._menuNavigation.BackActivated();
			return;
		case MenuNavigationAction.NavigationAction.PageSelected:
			this._menuNavigation.PageSelected(this._direction);
			break;
		default:
			return;
		}
	}

	// Token: 0x0600000B RID: 11 RVA: 0x000020A2 File Offset: 0x000002A2
	public override void Tick(float frameTime)
	{
		this.OnActionComplete();
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00002148 File Offset: 0x00000348
	public override void Reset()
	{
		base.Reset();
		this._action = MenuNavigationAction.NavigationAction.AccumulateMove;
		this._direction = default(Vector2);
	}

	// Token: 0x0600000D RID: 13 RVA: 0x00002163 File Offset: 0x00000363
	public static MenuNavigationAction CreateMove(PlayerActionGroup owningGroup, IScope scope, float timestamp, Vector2 direction)
	{
		MenuNavigationAction menuNavigationAction = scope.Get<MenuNavigationAction>();
		menuNavigationAction._action = MenuNavigationAction.NavigationAction.MoveCursor;
		menuNavigationAction._direction = direction;
		menuNavigationAction.InitializeAction(owningGroup, timestamp);
		menuNavigationAction.OnActionBegin(timestamp);
		return menuNavigationAction;
	}

	// Token: 0x0600000E RID: 14 RVA: 0x00002188 File Offset: 0x00000388
	public static MenuNavigationAction CreateAccumulateMove(PlayerActionGroup owningGroup, IScope scope, float timestamp, Vector2 direction)
	{
		MenuNavigationAction menuNavigationAction = scope.Get<MenuNavigationAction>();
		menuNavigationAction._action = MenuNavigationAction.NavigationAction.AccumulateMove;
		menuNavigationAction._direction = direction;
		menuNavigationAction.InitializeAction(owningGroup, timestamp);
		menuNavigationAction.OnActionBegin(timestamp);
		return menuNavigationAction;
	}

	// Token: 0x0600000F RID: 15 RVA: 0x000021AD File Offset: 0x000003AD
	public static MenuNavigationAction CreateResetAccumulated(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		MenuNavigationAction menuNavigationAction = scope.Get<MenuNavigationAction>();
		menuNavigationAction._action = MenuNavigationAction.NavigationAction.ResetAccumulated;
		menuNavigationAction.InitializeAction(owningGroup, timestamp);
		menuNavigationAction.OnActionBegin(timestamp);
		return menuNavigationAction;
	}

	// Token: 0x06000010 RID: 16 RVA: 0x000021CB File Offset: 0x000003CB
	public static MenuNavigationAction CreateActivateSelected(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		MenuNavigationAction menuNavigationAction = scope.Get<MenuNavigationAction>();
		menuNavigationAction._action = MenuNavigationAction.NavigationAction.ActivateSelected;
		menuNavigationAction.InitializeAction(owningGroup, timestamp);
		menuNavigationAction.OnActionBegin(timestamp);
		return menuNavigationAction;
	}

	// Token: 0x06000011 RID: 17 RVA: 0x000021E9 File Offset: 0x000003E9
	public static MenuNavigationAction CreateBackSelected(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		MenuNavigationAction menuNavigationAction = scope.Get<MenuNavigationAction>();
		menuNavigationAction._action = MenuNavigationAction.NavigationAction.BackSelected;
		menuNavigationAction.InitializeAction(owningGroup, timestamp);
		menuNavigationAction.OnActionBegin(timestamp);
		return menuNavigationAction;
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00002207 File Offset: 0x00000407
	public static MenuNavigationAction CreateChangePageSelected(PlayerActionGroup owningGroup, IScope scope, float timestamp, Vector2 direction)
	{
		MenuNavigationAction menuNavigationAction = scope.Get<MenuNavigationAction>();
		menuNavigationAction._action = MenuNavigationAction.NavigationAction.PageSelected;
		menuNavigationAction._direction = direction;
		menuNavigationAction.InitializeAction(owningGroup, timestamp);
		menuNavigationAction.OnActionBegin(timestamp);
		return menuNavigationAction;
	}

	// Token: 0x04000004 RID: 4
	[Dependency]
	protected MenuNavigation _menuNavigation;

	// Token: 0x04000005 RID: 5
	protected MenuNavigationAction.NavigationAction _action;

	// Token: 0x04000006 RID: 6
	protected Vector2 _direction;

	// Token: 0x02000007 RID: 7
	protected enum NavigationAction
	{
		// Token: 0x04000008 RID: 8
		AccumulateMove,
		// Token: 0x04000009 RID: 9
		ResetAccumulated,
		// Token: 0x0400000A RID: 10
		MoveCursor,
		// Token: 0x0400000B RID: 11
		ActivateSelected,
		// Token: 0x0400000C RID: 12
		BackSelected,
		// Token: 0x0400000D RID: 13
		PageSelected
	}
}
