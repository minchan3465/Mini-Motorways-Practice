using System;
using Factory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000168 RID: 360
public abstract class MenuNavigation
{
	// Token: 0x060007D1 RID: 2001 RVA: 0x00018F51 File Offset: 0x00017151
	public virtual PlayerAction CreateNavigateUpAction(PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		return MenuNavigationAction.CreateMove(playerActionGroup, scope, time, Vector2.up);
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x00018F60 File Offset: 0x00017160
	public virtual PlayerAction CreateNavigateDownAction(PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		return MenuNavigationAction.CreateMove(playerActionGroup, scope, time, Vector2.down);
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x00018F6F File Offset: 0x0001716F
	public virtual PlayerAction CreateNavigateLeftAction(PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		return MenuNavigationAction.CreateMove(playerActionGroup, scope, time, Vector2.left);
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x00018F7E File Offset: 0x0001717E
	public virtual PlayerAction CreateNavigateRightAction(PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		return MenuNavigationAction.CreateMove(playerActionGroup, scope, time, Vector2.right);
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x00018F8D File Offset: 0x0001718D
	public virtual PlayerAction CreateNavigateAccept(PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		return MenuNavigationAction.CreateActivateSelected(playerActionGroup, scope, time);
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x00018F97 File Offset: 0x00017197
	public virtual PlayerAction CreateNavigateBack(PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		return MenuNavigationAction.CreateBackSelected(playerActionGroup, scope, time);
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x00018FA1 File Offset: 0x000171A1
	public virtual PlayerAction CreateNavigatePageLeft(PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		return MenuNavigationAction.CreateChangePageSelected(playerActionGroup, scope, time, Vector2.left);
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x00018FB0 File Offset: 0x000171B0
	public virtual PlayerAction CreateNavigatePageRight(PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		return MenuNavigationAction.CreateChangePageSelected(playerActionGroup, scope, time, Vector2.right);
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x00018FC0 File Offset: 0x000171C0
	public PlayerAction CreateNavigateInDirection(int inputAxisX, int inputAxisY, PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		InputState inputState = scope.Get<InputState>();
		Vector2 direction = new Vector2(inputState.GetAxis(inputAxisX), inputState.GetAxis(inputAxisY));
		return MenuNavigationAction.CreateMove(playerActionGroup, scope, time, direction);
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x00018FF8 File Offset: 0x000171F8
	public virtual void AccumulateMove(Vector2 direction)
	{
		this._accumulatedMovement += direction;
		MenuNavigation.Log.Info("Accumulated movement {0}", new object[]
		{
			this._accumulatedMovement
		});
		if (this._accumulatedMovement.sqrMagnitude > this.menuNavigationSwipeThreshold * this.menuNavigationSwipeThreshold && this.MoveCursor(this._accumulatedMovement.normalized))
		{
			this._accumulatedMovement = Vector2.zero;
		}
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x00019074 File Offset: 0x00017274
	public virtual bool MoveCursor(Vector2 direction)
	{
		if (this._activeFocus == null || !this._activeFocus.gameObject.activeInHierarchy)
		{
			MenuNavigation.Log.Info("No Active Focus to move from!", Array.Empty<object>());
			foreach (MenuNavigation.IObserver observer in this.Observers)
			{
				observer.OnMoveCursorWithNullFocus();
			}
			this._scope.Get<InputState>().CurrentInputTypeRequiresFocus = true;
			return true;
		}
		MoveDirection moveDirection = MenuNavigation.VectorDirectionToMoveDirection(direction);
		foreach (MenuNavigation.IObserver observer2 in this.Observers)
		{
			observer2.OnMoveCursor(this._activeFocus, moveDirection);
		}
		Selectable newFocus;
		if (this._activeFocus.navigation.mode == Navigation.Mode.Explicit)
		{
			newFocus = this.GetSelectableForDirection(this._activeFocus, moveDirection);
			if (newFocus == null)
			{
				newFocus = this._activeFocus;
			}
			else
			{
				int attemptNumber = 0;
				while (attemptNumber < 5 && newFocus != null && !newFocus.gameObject.activeInHierarchy)
				{
					newFocus = this.GetSelectableForDirection(newFocus, moveDirection);
					attemptNumber++;
				}
				if (newFocus == null)
				{
					MenuNavigation.Log.Warn("We tried 5 times to find a new focus going {0} from {1} and didn't find anything active! May need to reorder the explicit navigation on this screen.", new object[]
					{
						moveDirection,
						this._activeFocus
					});
				}
			}
		}
		else
		{
			newFocus = this.GetSelectableForDirection(this._activeFocus, moveDirection);
		}
		bool flag = newFocus != null && newFocus != this._activeFocus;
		if (flag)
		{
			this.SetNewFocus(newFocus);
		}
		return flag;
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x000191E8 File Offset: 0x000173E8
	private Selectable GetSelectableForDirection(Selectable selectable, MoveDirection moveDirection)
	{
		switch (moveDirection)
		{
		case MoveDirection.Left:
			return selectable.FindSelectableOnLeft();
		case MoveDirection.Up:
			return selectable.FindSelectableOnUp();
		case MoveDirection.Right:
			return selectable.FindSelectableOnRight();
		case MoveDirection.Down:
			return selectable.FindSelectableOnDown();
		default:
			return selectable;
		}
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x00019220 File Offset: 0x00017420
	public static MoveDirection VectorDirectionToMoveDirection(Vector2 direction)
	{
		int closestIndex = -1;
		float closestDistance = float.MaxValue;
		for (int directionIndex = 0; directionIndex < MenuNavigation.MoveDirectionToVectorDirection.Length; directionIndex++)
		{
			float currentDistance = Vector2.Distance(MenuNavigation.MoveDirectionToVectorDirection[directionIndex].normalized, direction.normalized);
			if (currentDistance < closestDistance)
			{
				closestIndex = directionIndex;
				closestDistance = currentDistance;
			}
		}
		return (MoveDirection)closestIndex;
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x00019270 File Offset: 0x00017470
	public virtual void SetNewFocus(Selectable newFocus)
	{
		if (newFocus != null)
		{
			MenuNavigation.Log.Info("Setting new focus: {0}", new object[]
			{
				newFocus.name
			});
			this._activeFocus = newFocus;
			EventSystem.current.SetSelectedGameObject(this._activeFocus.gameObject);
			return;
		}
		this.ClearFocus(true);
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x000192C8 File Offset: 0x000174C8
	public virtual void ClearFocus(bool allowAutomaticFocus = true)
	{
		MenuNavigation.Log.Info("Clearing focus.", Array.Empty<object>());
		this._activeFocus = null;
		EventSystem current = EventSystem.current;
		if (current != null)
		{
			current.SetSelectedGameObject(null);
		}
		if (!allowAutomaticFocus)
		{
			this._scope.Get<InputState>().CurrentInputTypeRequiresFocus = false;
		}
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x00019315 File Offset: 0x00017515
	public virtual void ReleaseUIFocus()
	{
		this._activeFocus = null;
		EventSystem current = EventSystem.current;
		if (current == null)
		{
			return;
		}
		current.SetSelectedGameObject(null);
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0001932E File Offset: 0x0001752E
	public virtual Selectable GetCurrentFocus()
	{
		EventSystem current = EventSystem.current;
		if (current == null)
		{
			return null;
		}
		GameObject currentSelectedGameObject = current.currentSelectedGameObject;
		if (currentSelectedGameObject == null)
		{
			return null;
		}
		return currentSelectedGameObject.GetComponent<Selectable>();
	}

	// Token: 0x060007E2 RID: 2018
	public abstract bool ActivateSelected();

	// Token: 0x060007E3 RID: 2019
	public abstract void BackActivated();

	// Token: 0x060007E4 RID: 2020
	public abstract void PageSelected(Vector2 direction);

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x060007E5 RID: 2021 RVA: 0x0001934B File Offset: 0x0001754B
	protected ObserverList<MenuNavigation.IObserver> Observers
	{
		get
		{
			return this._observers;
		}
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x00019353 File Offset: 0x00017553
	public void Subscribe(MenuNavigation.IObserver observer)
	{
		this._observers.Subscribe(observer);
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x00019361 File Offset: 0x00017561
	public bool Unsubscribe(MenuNavigation.IObserver observer)
	{
		return this._observers.Unsubscribe(observer);
	}

	// Token: 0x04000391 RID: 913
	public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("MenuNavigation");

	// Token: 0x04000392 RID: 914
	[Dependency]
	protected IScope _scope;

	// Token: 0x04000393 RID: 915
	protected Selectable _activeFocus;

	// Token: 0x04000394 RID: 916
	protected Vector2 _accumulatedMovement = Vector2.zero;

	// Token: 0x04000395 RID: 917
	public float menuNavigationSwipeThreshold = 0.15f;

	// Token: 0x04000396 RID: 918
	public static readonly Vector2[] MoveDirectionToVectorDirection = new Vector2[]
	{
		Vector2.left,
		Vector2.up,
		Vector2.right,
		Vector2.down
	};

	// Token: 0x04000397 RID: 919
	[Serialize(false, null)]
	private readonly ObserverList<MenuNavigation.IObserver> _observers = new ObserverList<MenuNavigation.IObserver>(1);

	// Token: 0x02000169 RID: 361
	public interface IObserver
	{
		// Token: 0x060007EA RID: 2026
		void OnMoveCursorWithNullFocus();

		// Token: 0x060007EB RID: 2027
		void OnMoveCursor(Selectable currentFocus, MoveDirection direction);
	}
}
