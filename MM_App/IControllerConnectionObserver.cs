using System;

// Token: 0x02000167 RID: 359
public interface IControllerConnectionObserver
{
	// Token: 0x060007CF RID: 1999
	void OnControllerConnected(IController controller);

	// Token: 0x060007D0 RID: 2000
	void OnControllerDisconnected(IController controller);
}
