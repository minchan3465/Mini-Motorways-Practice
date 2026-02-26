using System;
using System.Runtime.InteropServices;

// Token: 0x020000CE RID: 206
public class GameCenterShared
{
	// Token: 0x06000435 RID: 1077
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCStart(IntPtr logDelegate, IntPtr gameCenterFocusChangedDelegate, IntPtr gameCenterAuthAttemptedDelegate);

	// Token: 0x06000436 RID: 1078
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCIsAuthenticated();

	// Token: 0x06000437 RID: 1079
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCAreAchievementsReady();

	// Token: 0x06000438 RID: 1080
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCSetAchievement(string achievementId, bool showBanner);

	// Token: 0x06000439 RID: 1081
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCIsAchievementComplete(string achievementId);

	// Token: 0x0600043A RID: 1082
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCSupportsRecurringLeaderboards();

	// Token: 0x0600043B RID: 1083
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCSetLeaderboardScore(string leaderboardId, int score, int scoreContext);

	// Token: 0x0600043C RID: 1084
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCRequestTopLeaderboardEntries(string leaderboardId);

	// Token: 0x0600043D RID: 1085
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCRequestPlayerCenteredLeaderboardEntries(string leaderboardId);

	// Token: 0x0600043E RID: 1086
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCRequestLocalLeaderboardEntry(string leaderboardId);

	// Token: 0x0600043F RID: 1087
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCRequestFriendLeaderboardEntries(string leaderboardId);

	// Token: 0x06000440 RID: 1088
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCIsLeaderboardRequestFinished();

	// Token: 0x06000441 RID: 1089
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	public static extern int GCGetDownloadedLeaderboardEntryCount();

	// Token: 0x06000442 RID: 1090
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	public static extern long GCGetTotalLeaderboardEntryCount();

	// Token: 0x06000443 RID: 1091
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCGetRetrievedLeaderboardEntry(int entryIndex, ref IntPtr id, ref IntPtr name, ref int score, ref long rank, ref int context, ref bool isLocal, ref bool isFriend);

	// Token: 0x06000444 RID: 1092
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	public static extern void GCResetAchievements();

	// Token: 0x06000445 RID: 1093
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCIsAccessPointAvailable();

	// Token: 0x06000446 RID: 1094
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	public static extern void GCShowAccessPoint();

	// Token: 0x06000447 RID: 1095
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	public static extern void GCHideAccessPoint();

	// Token: 0x06000448 RID: 1096
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	public static extern void GCSelectAccessPoint();

	// Token: 0x06000449 RID: 1097
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	public static extern float GCGetAccessPointOriginX();

	// Token: 0x0600044A RID: 1098
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	public static extern float GCGetAccessPointOriginY();

	// Token: 0x0600044B RID: 1099
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	public static extern float GCGetAccessPointSizeWidth();

	// Token: 0x0600044C RID: 1100
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	public static extern float GCGetAccessPointSizeHeight();

	// Token: 0x0600044D RID: 1101
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool GCOpenLeaderboardView(string leaderboardId);

	// Token: 0x020000CF RID: 207
	// (Invoke) Token: 0x06000450 RID: 1104
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void LogDelegate(string logMessage);

	// Token: 0x020000D0 RID: 208
	// (Invoke) Token: 0x06000454 RID: 1108
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void GameCenterFocusChangedDelegate(bool gameCenterHasFocus);

	// Token: 0x020000D1 RID: 209
	// (Invoke) Token: 0x06000458 RID: 1112
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void GameCenterAuthAttemptedDelegate(int status);
}
