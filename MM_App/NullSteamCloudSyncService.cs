using System;

// Token: 0x020000D5 RID: 213
public class NullSteamCloudSyncService : ISteamCloudSyncService
{
	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000466 RID: 1126 RVA: 0x0000222C File Offset: 0x0000042C
	public bool IsSupported
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x0000F624 File Offset: 0x0000D824
	public AsyncRequestHandle Authenticate(SteamCloudAuthenticationCompleted authenticationCompleted)
	{
		authenticationCompleted(null, SteamCloudSyncError.NotSupported);
		return AsyncRequestHandle.CompletedRequestHandle;
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x0000F633 File Offset: 0x0000D833
	public AsyncRequestHandle DownloadProfiles(string accessToken, SteamCloudProfileDownloadCompleted downloadCompleted)
	{
		downloadCompleted(null, null, SteamCloudSyncError.NotSupported);
		return AsyncRequestHandle.CompletedRequestHandle;
	}
}
