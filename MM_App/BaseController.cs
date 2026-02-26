using System;
using Factory;

// Token: 0x0200015F RID: 351
public abstract class BaseController : IController
{
	// Token: 0x060007B8 RID: 1976 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void OnControllerConnected()
	{
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void OnControllerDisconnected()
	{
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void RegisterInputActionsForApp(IScope appScope)
	{
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void RegisterInputActionsForGame(IScope gameScope)
	{
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void EnsureActionsAreRegistered(IScope scope)
	{
	}

	// Token: 0x170001BA RID: 442
	// (get) Token: 0x060007BD RID: 1981
	public abstract string DeviceName { get; }

	// Token: 0x060007BE RID: 1982 RVA: 0x0000EFC6 File Offset: 0x0000D1C6
	public virtual InputEventSource GetInputSource()
	{
		return InputEventSource.Generic;
	}

	// Token: 0x0400038B RID: 907
	public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("RemappableController");

	// Token: 0x0400038C RID: 908
	[Dependency]
	protected PlayerActionController _playerActionController;

	// Token: 0x0400038D RID: 909
	[Dependency]
	protected IScope _scope;

	// Token: 0x0400038E RID: 910
	[Dependency]
	protected InputState _inputState;
}
