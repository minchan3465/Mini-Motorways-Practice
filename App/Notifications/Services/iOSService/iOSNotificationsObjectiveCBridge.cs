using System;

namespace Notifications.Services.iOSService
{
	// Token: 0x020002C9 RID: 713
	public static class iOSNotificationsObjectiveCBridge
	{
		// Token: 0x0600117B RID: 4475 RVA: 0x000022F5 File Offset: 0x000004F5
		internal static void _ScheduleLocalCalendarNotification(IntPtr notificationContent, IntPtr calendarTrigger)
		{
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x000022F5 File Offset: 0x000004F5
		internal static void _ScheduleLocalTimeIntervalNotification(IntPtr notificationContent, IntPtr timeIntervalTrigger)
		{
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x000022F5 File Offset: 0x000004F5
		internal static void _RequestAuthorization()
		{
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x000022F5 File Offset: 0x000004F5
		internal static void _SetAuthorizationRequestCompleteDelegate(iOSNotificationsObjectiveCBridge.AuthorizationRequestComplete callback)
		{
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x00004C50 File Offset: 0x00002E50
		internal static AuthorizationStatus _GetAuthorizationStatus()
		{
			return AuthorizationStatus.Unknown;
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x000022F5 File Offset: 0x000004F5
		internal static void _RemoveScheduledNotification(string identifier)
		{
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x000022F5 File Offset: 0x000004F5
		internal static void _RemoveAllScheduledNotifications()
		{
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x000022F5 File Offset: 0x000004F5
		internal static void _RemoveDeliveredNotification(string identifier)
		{
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x000022F5 File Offset: 0x000004F5
		internal static void _RemoveAllDeliveredNotifications()
		{
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x000022F5 File Offset: 0x000004F5
		internal static void _SetNotificationReceivedDelegate(iOSNotificationsObjectiveCBridge.NotificationReceivedCallback callback)
		{
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x000022F5 File Offset: 0x000004F5
		internal static void _OpenApplicationSettings()
		{
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x000022F5 File Offset: 0x000004F5
		internal static void _SetApplicationBadgeNumber(int number)
		{
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x0000222C File Offset: 0x0000042C
		internal static int _GetApplicationBadgeNumber()
		{
			return 0;
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x000022F5 File Offset: 0x000004F5
		public static void _Log(string text)
		{
		}

		// Token: 0x020002CA RID: 714
		// (Invoke) Token: 0x0600118A RID: 4490
		internal delegate void AuthorizationRequestComplete(bool authorizationGranted);

		// Token: 0x020002CB RID: 715
		// (Invoke) Token: 0x0600118E RID: 4494
		internal delegate void NotificationReceivedCallback(string identifier);
	}
}
