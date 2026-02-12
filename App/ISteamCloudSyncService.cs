using System;
using JetBrains.Annotations;

// Token: 0x020000D2 RID: 210
public interface ISteamCloudSyncService
{
	// Token: 0x170000DF RID: 223
	// (get) Token: 0x0600045B RID: 1115
	bool IsSupported { get; }

	// Token: 0x0600045C RID: 1116
	AsyncRequestHandle Authenticate([NotNull] SteamCloudAuthenticationCompleted authenticationCompleted);

	// Token: 0x0600045D RID: 1117
	AsyncRequestHandle DownloadProfiles([NotNull] string accessToken, [NotNull] SteamCloudProfileDownloadCompleted downloadCompleted);
}
