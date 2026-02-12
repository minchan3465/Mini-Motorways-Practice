using System;
using System.Runtime.InteropServices;
using AOT;
using Factory;

// Token: 0x02000119 RID: 281
public class AppleReachability : IReachability, ICreatedInScopeHandler, IReleasedFromScopeHandler
{
	// Token: 0x060005FE RID: 1534 RVA: 0x00015C59 File Offset: 0x00013E59
	public void OnCreatedInScope(IScope scope)
	{
		AppleReachability.Instance = this;
		AppleReachability.StartCheckingReachability(Marshal.GetFunctionPointerForDelegate<Action<bool>>(new Action<bool>(AppleReachability.OnReachabilityChanged)));
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x00015C77 File Offset: 0x00013E77
	public void OnReleasedFromScope(IScope scope)
	{
		AppleReachability.Instance = null;
	}

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000600 RID: 1536 RVA: 0x00015C7F File Offset: 0x00013E7F
	public InternetConnectivity Connectivity
	{
		get
		{
			return this._connectivity;
		}
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x00015C87 File Offset: 0x00013E87
	public void OpenSilentConnection(IReachability.ConnectionOpened connectionOpened)
	{
		connectionOpened(new InternetConnectionHandle(this));
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x00015C95 File Offset: 0x00013E95
	public void OpenManualConnection(IReachability.ConnectionOpened connectionOpened)
	{
		this.OpenSilentConnection(connectionOpened);
	}

	// Token: 0x14000019 RID: 25
	// (add) Token: 0x06000603 RID: 1539 RVA: 0x00015CA0 File Offset: 0x00013EA0
	// (remove) Token: 0x06000604 RID: 1540 RVA: 0x00015CD8 File Offset: 0x00013ED8
	public event Action<InternetConnectivity> ConnectivityChanged;

	// Token: 0x06000605 RID: 1541 RVA: 0x000022F5 File Offset: 0x000004F5
	public void CloseConnection(InternetConnectionHandle handle)
	{
	}

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000606 RID: 1542 RVA: 0x0000222C File Offset: 0x0000042C
	public bool CanConnectManually
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x00015D10 File Offset: 0x00013F10
	[MonoPInvokeCallback(typeof(Action<bool>))]
	private static void OnReachabilityChanged(bool reachable)
	{
		AppleReachability.Log.Info("Reachability changed to {0}.", new object[]
		{
			reachable
		});
		if (AppleReachability.Instance != null)
		{
			InternetConnectivity newConnectivity = reachable ? InternetConnectivity.Connected : InternetConnectivity.Disconnected;
			if (AppleReachability.Instance._connectivity != newConnectivity)
			{
				AppleReachability.Instance._connectivity = newConnectivity;
				Action<InternetConnectivity> connectivityChanged = AppleReachability.Instance.ConnectivityChanged;
				if (connectivityChanged == null)
				{
					return;
				}
				connectivityChanged(newConnectivity);
			}
		}
	}

	// Token: 0x06000608 RID: 1544
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	private static extern void StartCheckingReachability(IntPtr reachabilityChangedHandler);

	// Token: 0x06000609 RID: 1545
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool IsReachable();

	// Token: 0x0400028B RID: 651
	private InternetConnectivity _connectivity;

	// Token: 0x0400028C RID: 652
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("AppleReachability");

	// Token: 0x0400028D RID: 653
	private static AppleReachability Instance;
}
