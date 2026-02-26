using System;
using Factory;

// Token: 0x02000158 RID: 344
public interface IApp
{
	// Token: 0x06000787 RID: 1927
	void Start();

	// Token: 0x06000788 RID: 1928
	void Tick(float absoluteTime, float deltaTime);

	// Token: 0x06000789 RID: 1929
	void GameOpenedNotificationSetup();

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x0600078A RID: 1930
	IScope Scope { get; }

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x0600078B RID: 1931
	Game Game { get; }

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x0600078C RID: 1932
	IInputState InputState { get; }

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x0600078D RID: 1933
	PlayerActionController PlayerActionController { get; }
}
