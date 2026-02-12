using System;

// Token: 0x02000226 RID: 550
[Flags]
public enum PersistentStorageServiceIssues
{
	// Token: 0x04000753 RID: 1875
	None = 0,
	// Token: 0x04000754 RID: 1876
	NotAuthenticated = 2,
	// Token: 0x04000755 RID: 1877
	NotAvailable = 4,
	// Token: 0x04000756 RID: 1878
	RecentUnauthenticatedData = 8,
	// Token: 0x04000757 RID: 1879
	AuthenticatedButOtherUsersiCloudData = 16,
	// Token: 0x04000758 RID: 1880
	QuotaExceeded = 32
}
