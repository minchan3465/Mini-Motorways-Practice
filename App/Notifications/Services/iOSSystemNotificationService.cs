using System;
using System.Collections.Generic;
using AOT;
using JetBrains.Annotations;
using Notifications.Services.iOSService;
using Notifications.Triggers;

namespace Notifications.Services
{
	// Token: 0x020002C3 RID: 707
	public class iOSSystemNotificationService : ISystemNotificationService
	{
		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06001153 RID: 4435 RVA: 0x0003A520 File Offset: 0x00038720
		// (set) Token: 0x06001154 RID: 4436 RVA: 0x0003A527 File Offset: 0x00038727
		public int ApplicationBadge
		{
			get
			{
				return iOSNotificationsObjectiveCBridge._GetApplicationBadgeNumber();
			}
			set
			{
				iOSNotificationsObjectiveCBridge._SetApplicationBadgeNumber(value);
			}
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06001155 RID: 4437 RVA: 0x0003A52F File Offset: 0x0003872F
		public List<SystemNotification> ScheduledNotifications { get; }

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06001156 RID: 4438 RVA: 0x0003A537 File Offset: 0x00038737
		public List<SystemNotification> DeliveredNotifications { get; }

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06001157 RID: 4439 RVA: 0x000020AA File Offset: 0x000002AA
		public bool IsAvailable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x06001158 RID: 4440 RVA: 0x000020AA File Offset: 0x000002AA
		public bool RequiresOptionsPanel
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x0003A53F File Offset: 0x0003873F
		public void Setup()
		{
			iOSNotificationsObjectiveCBridge._SetNotificationReceivedDelegate(new iOSNotificationsObjectiveCBridge.NotificationReceivedCallback(iOSSystemNotificationService.NotificationReceived));
			iOSNotificationsObjectiveCBridge._SetAuthorizationRequestCompleteDelegate(new iOSNotificationsObjectiveCBridge.AuthorizationRequestComplete(iOSSystemNotificationService.OnAuthorizationRequestComplete));
		}

		// Token: 0x0600115A RID: 4442 RVA: 0x0003A563 File Offset: 0x00038763
		public void RemoveAllDeliveredNotifications()
		{
			iOSNotificationsObjectiveCBridge._RemoveAllDeliveredNotifications();
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x0003A56A File Offset: 0x0003876A
		public void RequestAuthorization(OnAuthorizationRequestComplete authorizationRequestComplete = null)
		{
			if (iOSSystemNotificationService._authorizationRequestComplete != null)
			{
				Diagnostics.FailAssert("Cannot handle multiple notification authorization requests.", Array.Empty<object>());
				return;
			}
			iOSNotificationsObjectiveCBridge._Log("RequestAuthorization called");
			iOSSystemNotificationService._authorizationRequestComplete = authorizationRequestComplete;
			iOSNotificationsObjectiveCBridge._RequestAuthorization();
		}

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x0600115C RID: 4444 RVA: 0x0003A598 File Offset: 0x00038798
		public AuthorizationStatus AuthorizationStatus
		{
			get
			{
				return iOSNotificationsObjectiveCBridge._GetAuthorizationStatus();
			}
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x0003A59F File Offset: 0x0003879F
		[MonoPInvokeCallback(typeof(iOSNotificationsObjectiveCBridge.AuthorizationRequestComplete))]
		public static void OnAuthorizationRequestComplete(bool authorizationGranted)
		{
			iOSNotificationsObjectiveCBridge._Log(string.Format("Authorization Request Complete! Granted: {0}", authorizationGranted));
			OnAuthorizationRequestComplete authorizationRequestComplete = iOSSystemNotificationService._authorizationRequestComplete;
			if (authorizationRequestComplete != null)
			{
				authorizationRequestComplete(authorizationGranted);
			}
			iOSSystemNotificationService._authorizationRequestComplete = null;
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x0003A5CD File Offset: 0x000387CD
		[MonoPInvokeCallback(typeof(iOSNotificationsObjectiveCBridge.NotificationReceivedCallback))]
		public static void NotificationReceived(string identifier)
		{
			iOSNotificationsObjectiveCBridge._Log("Notification Received! " + identifier);
			Diagnostics.Log.Info("iOSNotificationBackend", "Notification Received! {0}", new object[]
			{
				identifier
			});
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x0003A5F8 File Offset: 0x000387F8
		public void ScheduleNotification(string identifier, SystemNotificationContent content, SystemNotificationTrigger trigger)
		{
			IntPtr contentDataPointer = iOSContentData.ToIntPtr<iOSContentData.NotificationContentData>(iOSContentData.ToContentData(identifier, content));
			CalendarNotificationTrigger calendarNotificationTrigger = trigger as CalendarNotificationTrigger;
			if (calendarNotificationTrigger != null)
			{
				IntPtr calendarTriggerDataPointer = iOSContentData.ToIntPtr<iOSContentData.CalendarTriggerData>(iOSContentData.ToContentData(calendarNotificationTrigger));
				iOSNotificationsObjectiveCBridge._ScheduleLocalCalendarNotification(contentDataPointer, calendarTriggerDataPointer);
				iOSNotificationsObjectiveCBridge._Log(string.Format("Scheduling calendar notification {0} for {1}", identifier, calendarNotificationTrigger));
				return;
			}
			TimeIntervalNotificationTrigger timeIntervalNotificationTrigger = trigger as TimeIntervalNotificationTrigger;
			if (timeIntervalNotificationTrigger == null)
			{
				Diagnostics.FailAssert("No implementation available for {0} in iOSNotificationBackend", new object[]
				{
					trigger.GetType().ToString()
				});
				return;
			}
			IntPtr timeIntervalTriggerPointer = iOSContentData.ToIntPtr<iOSContentData.TimeIntervalTriggerData>(iOSContentData.ToContentData(timeIntervalNotificationTrigger));
			iOSNotificationsObjectiveCBridge._ScheduleLocalTimeIntervalNotification(contentDataPointer, timeIntervalTriggerPointer);
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x0003A681 File Offset: 0x00038881
		public void RemoveScheduledNotification(string identifier)
		{
			iOSNotificationsObjectiveCBridge._Log("Removing scheduled notification " + identifier);
			iOSNotificationsObjectiveCBridge._RemoveScheduledNotification(identifier);
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x0003A699 File Offset: 0x00038899
		public void RemoveAllScheduledNotifications()
		{
			iOSNotificationsObjectiveCBridge._RemoveAllScheduledNotifications();
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x0003A6A0 File Offset: 0x000388A0
		public void OpenApplicationSettings()
		{
			iOSNotificationsObjectiveCBridge._OpenApplicationSettings();
		}

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x06001163 RID: 4451 RVA: 0x000022F5 File Offset: 0x000004F5
		// (remove) Token: 0x06001164 RID: 4452 RVA: 0x000022F5 File Offset: 0x000004F5
		public event NotificationReceived OnNotificationReceived
		{
			add
			{
			}
			remove
			{
			}
		}

		// Token: 0x04000F1C RID: 3868
		[CanBeNull]
		private static OnAuthorizationRequestComplete _authorizationRequestComplete;
	}
}
