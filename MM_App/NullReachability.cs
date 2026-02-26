using System;

// Token: 0x0200011B RID: 283
public class NullReachability : IReachability
{
	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000613 RID: 1555 RVA: 0x0000222C File Offset: 0x0000042C
	public InternetConnectivity Connectivity
	{
		get
		{
			return InternetConnectivity.Unknown;
		}
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x00015C87 File Offset: 0x00013E87
	public void OpenSilentConnection(IReachability.ConnectionOpened connectionOpened)
	{
		connectionOpened(new InternetConnectionHandle(this));
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x00015E29 File Offset: 0x00014029
	public void OpenManualConnection(IReachability.ConnectionOpened connectionOpened)
	{
		this.OpenSilentConnection(connectionOpened);
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x000022F5 File Offset: 0x000004F5
	public void CloseConnection(InternetConnectionHandle handle)
	{
	}

	// Token: 0x1400001A RID: 26
	// (add) Token: 0x06000617 RID: 1559 RVA: 0x000022F5 File Offset: 0x000004F5
	// (remove) Token: 0x06000618 RID: 1560 RVA: 0x000022F5 File Offset: 0x000004F5
	public event Action<InternetConnectivity> ConnectivityChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000619 RID: 1561 RVA: 0x0000222C File Offset: 0x0000042C
	public bool CanConnectManually
	{
		get
		{
			return false;
		}
	}
}
