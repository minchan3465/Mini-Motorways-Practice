using System;
using Factory;

// Token: 0x02000166 RID: 358
public interface IController
{
	// Token: 0x060007C9 RID: 1993
	void OnControllerConnected();

	// Token: 0x060007CA RID: 1994
	void OnControllerDisconnected();

	// Token: 0x060007CB RID: 1995
	void RegisterInputActionsForApp(IScope appScope);

	// Token: 0x060007CC RID: 1996
	void RegisterInputActionsForGame(IScope gameScope);

	// Token: 0x060007CD RID: 1997
	void EnsureActionsAreRegistered(IScope scope);

	// Token: 0x060007CE RID: 1998
	InputEventSource GetInputSource();
}
