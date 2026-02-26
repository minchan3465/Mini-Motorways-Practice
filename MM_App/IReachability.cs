using System;
using JetBrains.Annotations;

// Token: 0x020000ED RID: 237
public interface IReachability
{
	// Token: 0x170000FD RID: 253
	// (get) Token: 0x060004DF RID: 1247
	InternetConnectivity Connectivity { get; }

	// Token: 0x060004E0 RID: 1248
	void OpenSilentConnection([NotNull] IReachability.ConnectionOpened connectionOpened);

	// Token: 0x060004E1 RID: 1249
	void OpenManualConnection([NotNull] IReachability.ConnectionOpened connectionOpened);

	// Token: 0x060004E2 RID: 1250
	void CloseConnection([NotNull] InternetConnectionHandle handle);

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x060004E3 RID: 1251
	bool CanConnectManually { get; }

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x060004E4 RID: 1252
	// (remove) Token: 0x060004E5 RID: 1253
	event Action<InternetConnectivity> ConnectivityChanged;

	// Token: 0x020000EE RID: 238
	// (Invoke) Token: 0x060004E7 RID: 1255
	public delegate void ConnectionOpened(InternetConnectionHandle handle);
}
