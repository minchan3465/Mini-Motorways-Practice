using System;
using System.Collections.Generic;
using Factory;
using Motorways;
using UnityEngine;

// Token: 0x0200000B RID: 11
public class PlayerActionController : IScopeObserver, IReleasedFromScopeHandler
{
	// Token: 0x17000009 RID: 9
	// (get) Token: 0x0600002B RID: 43 RVA: 0x00002458 File Offset: 0x00000658
	public IEnumerable<PlayerActionGroup> ActiveGroups
	{
		get
		{
			return this._activePlayerActionGroups;
		}
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x0600002C RID: 44 RVA: 0x00002460 File Offset: 0x00000660
	public int ActivePlayerActionCount
	{
		get
		{
			return this._activePlayerActionGroups.Count;
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x0600002D RID: 45 RVA: 0x00002470 File Offset: 0x00000670
	public int BlockingPlayerActionCount
	{
		get
		{
			int blockingPlayerActionCount = 0;
			using (List<PlayerActionGroup>.Enumerator enumerator = this._activePlayerActionGroups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.IsInterruptible)
					{
						blockingPlayerActionCount++;
					}
				}
			}
			return blockingPlayerActionCount;
		}
	}

	// Token: 0x0600002E RID: 46 RVA: 0x000024CC File Offset: 0x000006CC
	public void SetGameScope(IScope gameScope)
	{
		if (Diagnostics.Verify(this._gameScope == null))
		{
			this._gameScope = gameScope;
		}
	}

	// Token: 0x0600002F RID: 47 RVA: 0x000024E5 File Offset: 0x000006E5
	public void GameEnded()
	{
		if (Diagnostics.Verify(this._gameScope != null))
		{
			this._gameScope = null;
		}
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00002500 File Offset: 0x00000700
	public void UpdateBlockFlags(InputState.BlockInput blockInputFlags)
	{
		bool appEnabled = true;
		this._gameActionsBlocked = ((blockInputFlags & InputState.BlockInput.BlockGame) > InputState.BlockInput.AllowEverything);
		this.SetScopeActive(this._appScope, appEnabled);
		this.SetScopeActive(this._gameScope, !this._gameActionsBlocked && !this._tutorialActionsBlocked);
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000031 RID: 49 RVA: 0x00002548 File Offset: 0x00000748
	// (set) Token: 0x06000032 RID: 50 RVA: 0x00002550 File Offset: 0x00000750
	public bool TutorialBlockInputFlag
	{
		get
		{
			return this._tutorialActionsBlocked;
		}
		set
		{
			this._tutorialActionsBlocked = value;
			this.SetScopeActive(this._gameScope, !this._gameActionsBlocked && !this._tutorialActionsBlocked);
		}
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00002579 File Offset: 0x00000779
	public void SetScopeActive(IScope scope, bool isActive)
	{
		if (isActive)
		{
			if (!this.activeScopes.Contains(scope))
			{
				this.activeScopes.Add(scope);
				return;
			}
		}
		else if (this.activeScopes.Contains(scope))
		{
			this.activeScopes.Remove(scope);
		}
	}

	// Token: 0x06000034 RID: 52 RVA: 0x000025B4 File Offset: 0x000007B4
	public bool RegisterAction(InputEventFilter inputEventFilter, Func<PlayerActionGroup, IScope, float, PlayerAction> playerActionConstructor, IScope toScope, bool ignorePollingAxis = false)
	{
		if (!this._inputEventFilterToPlayerActionConstructors.ContainsKey(inputEventFilter))
		{
			this._inputEventFilterToPlayerActionConstructors.Add(inputEventFilter, new List<Func<PlayerActionGroup, IScope, float, PlayerAction>>());
		}
		if (!this._owningScopeToConstructors.ContainsKey(toScope))
		{
			this._owningScopeToConstructors.Add(toScope, new List<Func<PlayerActionGroup, IScope, float, PlayerAction>>());
			toScope.Subscribe(this);
		}
		if (Diagnostics.Verify(!this._inputEventFilterToPlayerActionConstructors[inputEventFilter].Contains(playerActionConstructor)))
		{
			this._inputEventFilterToPlayerActionConstructors[inputEventFilter].Add(playerActionConstructor);
			if (!this._owningScopeToConstructors[toScope].Contains(playerActionConstructor))
			{
				this._owningScopeToConstructors[toScope].Add(playerActionConstructor);
			}
			if (!ignorePollingAxis)
			{
				if (inputEventFilter.ExpectedButtonState == InputEventButtonState.Axis)
				{
					this._inputState.EnsurePollingAxis(inputEventFilter.RewiredAction);
				}
				else
				{
					this._inputState.EnsurePollingRewiredAction(inputEventFilter.RewiredAction);
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00002690 File Offset: 0x00000890
	protected void CleanupEmptyDictionaryEntries()
	{
		List<InputEventFilter> inputEventFiltersToRemove = new List<InputEventFilter>();
		foreach (KeyValuePair<InputEventFilter, List<Func<PlayerActionGroup, IScope, float, PlayerAction>>> currentFilter in this._inputEventFilterToPlayerActionConstructors)
		{
			if (currentFilter.Value.Count == 0)
			{
				inputEventFiltersToRemove.Add(currentFilter.Key);
			}
		}
		for (int eventFilterIndex = 0; eventFilterIndex < inputEventFiltersToRemove.Count; eventFilterIndex++)
		{
			this._inputEventFilterToPlayerActionConstructors.Remove(inputEventFiltersToRemove[eventFilterIndex]);
		}
		List<IScope> scopesToRemove = new List<IScope>();
		foreach (KeyValuePair<IScope, List<Func<PlayerActionGroup, IScope, float, PlayerAction>>> currentScope in this._owningScopeToConstructors)
		{
			if (currentScope.Value.Count == 0)
			{
				currentScope.Key.Unsubscribe(this);
				scopesToRemove.Add(currentScope.Key);
			}
		}
		for (int scopeToRemoveIndex = 0; scopeToRemoveIndex < scopesToRemove.Count; scopeToRemoveIndex++)
		{
			this._owningScopeToConstructors.Remove(scopesToRemove[scopeToRemoveIndex]);
		}
	}

	// Token: 0x06000036 RID: 54 RVA: 0x000027BC File Offset: 0x000009BC
	public void UnregisterAction<PlayerActionType>(IScope optionalScopeFilter = null) where PlayerActionType : PlayerAction
	{
		foreach (KeyValuePair<InputEventFilter, List<Func<PlayerActionGroup, IScope, float, PlayerAction>>> entry in this._inputEventFilterToPlayerActionConstructors)
		{
			int constructorIndex = 0;
			while (constructorIndex < entry.Value.Count)
			{
				if (entry.Value[constructorIndex].Method.ReturnType == typeof(PlayerActionType))
				{
					bool removeEntry = optionalScopeFilter == null;
					foreach (KeyValuePair<IScope, List<Func<PlayerActionGroup, IScope, float, PlayerAction>>> currentScope in this._owningScopeToConstructors)
					{
						if (currentScope.Value.Contains(entry.Value[constructorIndex]))
						{
							if (currentScope.Key == optionalScopeFilter)
							{
								removeEntry = true;
							}
							currentScope.Value.Remove(entry.Value[constructorIndex]);
						}
					}
					if (removeEntry)
					{
						entry.Value.RemoveAt(constructorIndex);
					}
				}
				else
				{
					constructorIndex++;
				}
			}
		}
		this.CleanupEmptyDictionaryEntries();
	}

	// Token: 0x06000037 RID: 55 RVA: 0x000028F4 File Offset: 0x00000AF4
	public void OnInputEvent(float timestamp, InputEvent inputEvent)
	{
		bool overUI = this._inputState.IsInputEventOverUI(inputEvent);
		bool newActionsBlocked = false;
		foreach (PlayerActionGroup group in this._activePlayerActionGroups)
		{
			if (group.ObservesInputEvent(inputEvent))
			{
				newActionsBlocked |= group.BlocksNewActionsForInputEvent(inputEvent);
				group.ObserveInput(timestamp, inputEvent, overUI);
			}
		}
		if (!newActionsBlocked && (!overUI || inputEvent is MotorwaysUIInputEvent))
		{
			List<Func<PlayerActionGroup, IScope, float, PlayerAction>> matchingDelegates = null;
			foreach (KeyValuePair<InputEventFilter, List<Func<PlayerActionGroup, IScope, float, PlayerAction>>> pair in this._inputEventFilterToPlayerActionConstructors)
			{
				if (pair.Key.MatchesEvent(inputEvent))
				{
					foreach (Func<PlayerActionGroup, IScope, float, PlayerAction> specificConstructorDelegate in pair.Value)
					{
						foreach (KeyValuePair<IScope, List<Func<PlayerActionGroup, IScope, float, PlayerAction>>> eachScope in this._owningScopeToConstructors)
						{
							if (eachScope.Value.Contains(specificConstructorDelegate) && this.activeScopes.Contains(eachScope.Key))
							{
								if (matchingDelegates == null)
								{
									matchingDelegates = new List<Func<PlayerActionGroup, IScope, float, PlayerAction>>();
								}
								matchingDelegates.Add(specificConstructorDelegate);
								break;
							}
						}
					}
				}
			}
			if (matchingDelegates != null)
			{
				IScope activeScope = this._gameScope ?? this._appScope;
				PlayerActionGroup invokedPlayerActionGroup = this._appScope.Get<PlayerActionGroup>();
				invokedPlayerActionGroup.Initialize(timestamp, inputEvent);
				bool shouldCancelOtherInputTypeActions = false;
				foreach (Func<PlayerActionGroup, IScope, float, PlayerAction> constructor in matchingDelegates)
				{
					if (invokedPlayerActionGroup.CanAddNewActions)
					{
						PlayerAction newAction = constructor(invokedPlayerActionGroup, activeScope, timestamp);
						shouldCancelOtherInputTypeActions |= !newAction.IsInterruptible;
					}
				}
				if (shouldCancelOtherInputTypeActions)
				{
					foreach (PlayerActionGroup playerActionGroup in this._activePlayerActionGroups)
					{
						using (IEnumerator<PlayerAction> enumerator5 = playerActionGroup.Actions.GetEnumerator())
						{
							while (enumerator5.MoveNext())
							{
								if (enumerator5.Current.InputSourceType != inputEvent.Source)
								{
									playerActionGroup.CancelAllActions();
									break;
								}
							}
						}
					}
				}
				this._activePlayerActionGroups.Add(invokedPlayerActionGroup);
			}
		}
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00002BBC File Offset: 0x00000DBC
	public void Tick(float frameTime)
	{
		for (int groupIndex = this._activePlayerActionGroups.Count - 1; groupIndex >= 0; groupIndex--)
		{
			if (this._activePlayerActionGroups[groupIndex].IsSafeToRemove)
			{
				this._appScope.Release(this._activePlayerActionGroups[groupIndex]);
				this._activePlayerActionGroups.RemoveAt(groupIndex);
				PlayerAction.Log.Info("Removing empty PlayerActionGroup.", Array.Empty<object>());
			}
			else
			{
				this._activePlayerActionGroups[groupIndex].Tick(frameTime);
			}
		}
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00002C40 File Offset: 0x00000E40
	public void DebugGUI()
	{
		if (FeatureToggle.IsFeatureDisabled(Feature.PlayerActionView))
		{
			return;
		}
		Rect rect = new Rect(10f, 50f, 1000f, 40f);
		GUI.Label(rect, "PLAYER ACTIONS:");
		rect.y += 50f;
		foreach (PlayerActionGroup playerActionGroup in this._activePlayerActionGroups)
		{
			foreach (PlayerAction action in playerActionGroup.Actions)
			{
				GUI.Label(rect, string.Format("{0}| Instigating Type: {1} | Is Interruptible: {2}", action.GetType(), action.OwningGroup.InstigatingInputEvent.Source, action.IsInterruptible));
				rect.y += 50f;
			}
		}
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00002D50 File Offset: 0x00000F50
	public void InterruptActions<PlayerActionType>() where PlayerActionType : PlayerAction
	{
		for (int groupIndex = this._activePlayerActionGroups.Count - 1; groupIndex >= 0; groupIndex--)
		{
			if (this._activePlayerActionGroups[groupIndex].HasActionType<PlayerActionType>())
			{
				this._activePlayerActionGroups[groupIndex].RemoveActionType<PlayerActionType>();
			}
		}
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00002D9C File Offset: 0x00000F9C
	public void CancelAllActions()
	{
		PlayerAction.Log.Info("Cancelling all actions!", Array.Empty<object>());
		foreach (PlayerActionGroup playerActionGroup in this._activePlayerActionGroups)
		{
			playerActionGroup.CancelAllActions();
		}
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00002E00 File Offset: 0x00001000
	public void OnScopeReleased(IScope scopeBeingReleased)
	{
		if (Diagnostics.Verify(this._owningScopeToConstructors.ContainsKey(scopeBeingReleased), "A scope is reporting being released, but we don't have any actions registered for it!"))
		{
			for (int constructorIndex = 0; constructorIndex < this._owningScopeToConstructors[scopeBeingReleased].Count; constructorIndex++)
			{
				Func<PlayerActionGroup, IScope, float, PlayerAction> specificConstructor = this._owningScopeToConstructors[scopeBeingReleased][constructorIndex];
				foreach (KeyValuePair<InputEventFilter, List<Func<PlayerActionGroup, IScope, float, PlayerAction>>> entry in this._inputEventFilterToPlayerActionConstructors)
				{
					entry.Value.Remove(specificConstructor);
				}
			}
			this._owningScopeToConstructors.Remove(scopeBeingReleased);
		}
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00002EB0 File Offset: 0x000010B0
	public void OnReleasedFromScope(IScope scope)
	{
		foreach (KeyValuePair<IScope, List<Func<PlayerActionGroup, IScope, float, PlayerAction>>> currentScope in this._owningScopeToConstructors)
		{
			currentScope.Key.Unsubscribe(this);
		}
		this._inputEventFilterToPlayerActionConstructors.Clear();
		this._owningScopeToConstructors.Clear();
		foreach (PlayerActionGroup playerActionGroup in this._activePlayerActionGroups)
		{
			this._appScope.Release(playerActionGroup);
		}
		this._activePlayerActionGroups.Clear();
		this._gameScope = null;
	}

	// Token: 0x04000020 RID: 32
	[Dependency]
	private IScope _appScope;

	// Token: 0x04000021 RID: 33
	private IScope _gameScope;

	// Token: 0x04000022 RID: 34
	[Dependency]
	private IInputState _inputState;

	// Token: 0x04000023 RID: 35
	private Dictionary<InputEventFilter, List<Func<PlayerActionGroup, IScope, float, PlayerAction>>> _inputEventFilterToPlayerActionConstructors = new Dictionary<InputEventFilter, List<Func<PlayerActionGroup, IScope, float, PlayerAction>>>();

	// Token: 0x04000024 RID: 36
	private Dictionary<IScope, List<Func<PlayerActionGroup, IScope, float, PlayerAction>>> _owningScopeToConstructors = new Dictionary<IScope, List<Func<PlayerActionGroup, IScope, float, PlayerAction>>>();

	// Token: 0x04000025 RID: 37
	private List<PlayerActionGroup> _activePlayerActionGroups = new List<PlayerActionGroup>();

	// Token: 0x04000026 RID: 38
	private List<IScope> activeScopes = new List<IScope>();

	// Token: 0x04000027 RID: 39
	private bool _gameActionsBlocked;

	// Token: 0x04000028 RID: 40
	private bool _tutorialActionsBlocked;
}
