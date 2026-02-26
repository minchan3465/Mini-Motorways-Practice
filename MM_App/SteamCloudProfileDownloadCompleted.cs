using System;
using JetBrains.Annotations;

// Token: 0x020000D4 RID: 212
// (Invoke) Token: 0x06000463 RID: 1123
public delegate void SteamCloudProfileDownloadCompleted([CanBeNull] ILegacyUserProfile steamUserProfile, [CanBeNull] IExtendedUserProfile steamExtendedUserProfile, SteamCloudSyncError error);
