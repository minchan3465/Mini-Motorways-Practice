using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;

// Token: 0x02000008 RID: 8
public abstract class PlayerAction : IReusable
{
	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000014 RID: 20 RVA: 0x0000222C File Offset: 0x0000042C
	public virtual bool IsInterruptible
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000015 RID: 21 RVA: 0x0000222F File Offset: 0x0000042F
	// (set) Token: 0x06000016 RID: 22 RVA: 0x00002237 File Offset: 0x00000437
	[Dependency]
	public IScope Scope { get; protected set; }

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000017 RID: 23 RVA: 0x00002240 File Offset: 0x00000440
	// (set) Token: 0x06000018 RID: 24 RVA: 0x00002248 File Offset: 0x00000448
	public PlayerActionGroup OwningGroup
	{
		get
		{
			return this._owningGroup;
		}
		set
		{
			this._owningGroup = value;
		}
	}

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000019 RID: 25 RVA: 0x00002251 File Offset: 0x00000451
	// (set) Token: 0x0600001A RID: 26 RVA: 0x00002259 File Offset: 0x00000459
	public PlayerAction.State ActionState { get; protected set; }

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x0600001B RID: 27 RVA: 0x00002262 File Offset: 0x00000462
	public InputEventSource InputSourceType
	{
		get
		{
			return this._owningGroup.InstigatingInputEvent.Source;
		}
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00002274 File Offset: 0x00000474
	public virtual void InitializeAction(PlayerActionGroup owningGroup, float timestamp)
	{
		this.OwningGroup = owningGroup;
		Diagnostics.Verify(owningGroup.AddAction(this), "Action {0} was not added to its owning group.", this);
		this.ActionState = PlayerAction.State.Initialized;
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00002297 File Offset: 0x00000497
	public virtual void OnActionBegin(float timestamp)
	{
		this.timeCreated = timestamp;
		this.ActionState = PlayerAction.State.Begun;
	}

	// Token: 0x0600001E RID: 30 RVA: 0x000022A7 File Offset: 0x000004A7
	public virtual void OnActionComplete()
	{
		this.OwningGroup.RemoveAction(this);
		this.ActionState = PlayerAction.State.Complete;
		this.ClearInputObserveFilters();
	}

	// Token: 0x0600001F RID: 31 RVA: 0x000022C2 File Offset: 0x000004C2
	public virtual void OnActionCancel()
	{
		this.OwningGroup.RemoveAction(this);
		this.ActionState = PlayerAction.State.Cancelled;
		this.ClearInputObserveFilters();
	}

	// Token: 0x06000020 RID: 32 RVA: 0x000022DD File Offset: 0x000004DD
	protected void ClearInputObserveFilters()
	{
		this._observedInputEventFilters.Clear();
		this._inputFiltersToBlockNewActions.Clear();
	}

	// Token: 0x06000021 RID: 33 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void Tick(float frameTime)
	{
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000022 RID: 34 RVA: 0x000022F7 File Offset: 0x000004F7
	public bool IsExclusive
	{
		get
		{
			return this.OwningGroup.IsActionExclusive(this);
		}
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00002305 File Offset: 0x00000505
	public void MakeExclusive()
	{
		this.OwningGroup.MakeActionExclusive(this);
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002314 File Offset: 0x00000514
	public bool ObservesInputEvent(InputEvent inputEvent)
	{
		using (List<InputEventFilter>.Enumerator enumerator = this._observedInputEventFilters.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.MatchesEvent(inputEvent))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000025 RID: 37 RVA: 0x00002370 File Offset: 0x00000570
	public bool BlocksNewActionsForInputEvent(InputEvent inputEvent)
	{
		using (List<InputEventFilter>.Enumerator enumerator = this._inputFiltersToBlockNewActions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.MatchesEvent(inputEvent))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000026 RID: 38 RVA: 0x000023CC File Offset: 0x000005CC
	protected void RegisterObserveInputEvent(InputEventFilter eventToObserve, PlayerAction.ObserverGreediness inputGreediness)
	{
		if (!this._observedInputEventFilters.Contains(eventToObserve))
		{
			this._observedInputEventFilters.Add(eventToObserve);
			if (inputGreediness == PlayerAction.ObserverGreediness.BlocksNewActions)
			{
				this._inputFiltersToBlockNewActions.Add(eventToObserve);
			}
		}
	}

	// Token: 0x06000027 RID: 39 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
	{
	}

	// Token: 0x06000028 RID: 40 RVA: 0x000023F8 File Offset: 0x000005F8
	public virtual void Reset()
	{
		this._owningGroup = null;
		this.timeCreated = 0f;
		this._inputFiltersToBlockNewActions.Clear();
		this._observedInputEventFilters.Clear();
		this.ActionState = PlayerAction.State.None;
	}

	// Token: 0x0400000E RID: 14
	public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Action");

	// Token: 0x04000010 RID: 16
	[Dependency]
	protected InputState _inputState;

	// Token: 0x04000011 RID: 17
	[Dependency]
	protected PlayerActionController _actionController;

	// Token: 0x04000012 RID: 18
	private List<InputEventFilter> _inputFiltersToBlockNewActions = new List<InputEventFilter>();

	// Token: 0x04000013 RID: 19
	private List<InputEventFilter> _observedInputEventFilters = new List<InputEventFilter>();

	// Token: 0x04000014 RID: 20
	protected PlayerActionGroup _owningGroup;

	// Token: 0x04000016 RID: 22
	public float timeCreated;

	// Token: 0x02000009 RID: 9
	public enum ObserverGreediness
	{
		// Token: 0x04000018 RID: 24
		AllowsNewActions,
		// Token: 0x04000019 RID: 25
		BlocksNewActions
	}

	// Token: 0x0200000A RID: 10
	public enum State
	{
		// Token: 0x0400001B RID: 27
		None,
		// Token: 0x0400001C RID: 28
		Initialized,
		// Token: 0x0400001D RID: 29
		Begun,
		// Token: 0x0400001E RID: 30
		Cancelled,
		// Token: 0x0400001F RID: 31
		Complete
	}
}
