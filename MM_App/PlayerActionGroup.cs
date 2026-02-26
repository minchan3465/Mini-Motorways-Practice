using System;
using System.Collections.Generic;
using Factory.Pools;

// Token: 0x0200000C RID: 12
public class PlayerActionGroup : IReusable
{
	// Token: 0x1700000D RID: 13
	// (get) Token: 0x0600003F RID: 63 RVA: 0x00002FB0 File Offset: 0x000011B0
	public IEnumerable<PlayerAction> Actions
	{
		get
		{
			return this._activePlayerActions;
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x06000040 RID: 64 RVA: 0x00002FB8 File Offset: 0x000011B8
	public bool HasExclusiveAction
	{
		get
		{
			return this._activePlayerActions.Count == 1 || this._actionResolvedAsExclusive;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x06000041 RID: 65 RVA: 0x00002FD0 File Offset: 0x000011D0
	public bool IsSafeToRemove
	{
		get
		{
			return this._activePlayerActions.Count == 0;
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x06000042 RID: 66 RVA: 0x00002FE0 File Offset: 0x000011E0
	public bool IsInterruptible
	{
		get
		{
			using (List<PlayerAction>.Enumerator enumerator = this._activePlayerActions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.IsInterruptible)
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x06000043 RID: 67 RVA: 0x0000303C File Offset: 0x0000123C
	public bool CanAddNewActions
	{
		get
		{
			return !this._actionResolvedAsExclusive;
		}
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00003048 File Offset: 0x00001248
	public void MakeActionExclusive(PlayerAction action)
	{
		if (!this._actionResolvedAsExclusive)
		{
			this._actionResolvedAsExclusive = true;
			foreach (PlayerAction activeAction in this._activePlayerActions)
			{
				if (activeAction != action)
				{
					activeAction.OnActionCancel();
					PlayerActionGroup.Log.Info("Cancelling action due to exclusivity change: {0}", new object[]
					{
						activeAction.GetType().ToString()
					});
				}
			}
		}
	}

	// Token: 0x06000045 RID: 69 RVA: 0x000030D0 File Offset: 0x000012D0
	public bool IsActionExclusive(PlayerAction action)
	{
		return this.HasExclusiveAction && this._activePlayerActions[0] == action;
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x06000046 RID: 70 RVA: 0x000030EB File Offset: 0x000012EB
	// (set) Token: 0x06000047 RID: 71 RVA: 0x000030F3 File Offset: 0x000012F3
	public InputEvent InstigatingInputEvent { get; private set; }

	// Token: 0x06000048 RID: 72 RVA: 0x000030FC File Offset: 0x000012FC
	public void Initialize(float timestamp, InputEvent instigatingEvent)
	{
		this._creationTimestamp = timestamp;
		this.InstigatingInputEvent = instigatingEvent;
		this._actionResolvedAsExclusive = false;
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00003113 File Offset: 0x00001313
	public bool AddAction(PlayerAction newAction)
	{
		if (this.CanAddNewActions)
		{
			this._activePlayerActions.Add(newAction);
			newAction.OwningGroup = this;
			return true;
		}
		return false;
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00003133 File Offset: 0x00001333
	public void RemoveAction(PlayerAction action)
	{
		this._pendingRemovalPlayerActions.Add(action);
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00003144 File Offset: 0x00001344
	public void CancelAllActions()
	{
		foreach (PlayerAction playerAction in this._activePlayerActions)
		{
			playerAction.OnActionCancel();
		}
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00003194 File Offset: 0x00001394
	public void RemoveActionType<PlayerActionType>() where PlayerActionType : PlayerAction
	{
		foreach (PlayerAction playerAction in this._activePlayerActions)
		{
			if (playerAction.GetType() == typeof(PlayerActionType))
			{
				playerAction.OnActionComplete();
				this.RemoveAction(playerAction);
			}
		}
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00003204 File Offset: 0x00001404
	public bool HasAction(PlayerAction action)
	{
		return this._activePlayerActions.Contains(action);
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00003214 File Offset: 0x00001414
	public bool HasActionType<PlayerActionType>() where PlayerActionType : PlayerAction
	{
		using (List<PlayerAction>.Enumerator enumerator = this._activePlayerActions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.GetType() == typeof(PlayerActionType))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600004F RID: 79 RVA: 0x0000327C File Offset: 0x0000147C
	public bool ObservesInputEvent(InputEvent inputEvent)
	{
		bool observesEvent = false;
		foreach (PlayerAction activeAction in this._activePlayerActions)
		{
			observesEvent |= activeAction.ObservesInputEvent(inputEvent);
		}
		return observesEvent;
	}

	// Token: 0x06000050 RID: 80 RVA: 0x000032D8 File Offset: 0x000014D8
	public bool BlocksNewActionsForInputEvent(InputEvent inputEvent)
	{
		bool actionsBlocksNewActions = false;
		foreach (PlayerAction activeAction in this._activePlayerActions)
		{
			actionsBlocksNewActions |= activeAction.BlocksNewActionsForInputEvent(inputEvent);
		}
		return actionsBlocksNewActions;
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00003334 File Offset: 0x00001534
	public void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
	{
		foreach (PlayerAction activeAction in this._activePlayerActions)
		{
			if (activeAction.ObservesInputEvent(inputEvent))
			{
				activeAction.ObserveInput(timestamp, inputEvent, overUI);
			}
		}
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00003394 File Offset: 0x00001594
	public void Tick(float frameTime)
	{
		foreach (PlayerAction action in this._pendingRemovalPlayerActions)
		{
			action.Scope.Release(action);
			this._activePlayerActions.Remove(action);
		}
		this._pendingRemovalPlayerActions.Clear();
		foreach (PlayerAction action2 in this._activePlayerActions)
		{
			if (!this._pendingRemovalPlayerActions.Contains(action2))
			{
				action2.Tick(frameTime);
			}
		}
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00003458 File Offset: 0x00001658
	public void Reset()
	{
		this._creationTimestamp = 0f;
		this._activePlayerActions.Clear();
		this._pendingRemovalPlayerActions.Clear();
		this._actionResolvedAsExclusive = false;
	}

	// Token: 0x04000029 RID: 41
	private float _creationTimestamp;

	// Token: 0x0400002A RID: 42
	private List<PlayerAction> _activePlayerActions = new List<PlayerAction>();

	// Token: 0x0400002B RID: 43
	private List<PlayerAction> _pendingRemovalPlayerActions = new List<PlayerAction>();

	// Token: 0x0400002C RID: 44
	private bool _actionResolvedAsExclusive;

	// Token: 0x0400002D RID: 45
	public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("PlayerActionGroup");
}
