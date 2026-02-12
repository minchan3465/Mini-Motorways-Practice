using System;
using System.Collections.Generic;
using System.Linq;
using Factory;
using Notifications.Triggers;

namespace Notifications.Services
{
	// Token: 0x020002C2 RID: 706
	public class ConsoleSystemNotificationService : ISystemNotificationService
	{
		// Token: 0x17000378 RID: 888
		// (get) Token: 0x0600113F RID: 4415 RVA: 0x0003A086 File Offset: 0x00038286
		// (set) Token: 0x06001140 RID: 4416 RVA: 0x0003A08E File Offset: 0x0003828E
		public int ApplicationBadge { get; set; }

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06001141 RID: 4417 RVA: 0x0003A097 File Offset: 0x00038297
		public List<SystemNotification> ScheduledNotifications
		{
			get
			{
				return this._deliveredNotifications.Values.ToList<SystemNotification>();
			}
		}

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06001142 RID: 4418 RVA: 0x0003A0A9 File Offset: 0x000382A9
		public List<SystemNotification> DeliveredNotifications
		{
			get
			{
				return this._scheduledNotifications.Values.ToList<SystemNotification>();
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06001143 RID: 4419 RVA: 0x000020AA File Offset: 0x000002AA
		public bool IsAvailable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06001144 RID: 4420 RVA: 0x0003A0BB File Offset: 0x000382BB
		// (set) Token: 0x06001145 RID: 4421 RVA: 0x0003A0C3 File Offset: 0x000382C3
		public AuthorizationStatus AuthorizationStatus
		{
			get
			{
				return this._authorizationStatus;
			}
			private set
			{
				this._authorizationStatus = value;
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06001146 RID: 4422 RVA: 0x0000222C File Offset: 0x0000042C
		public bool RequiresOptionsPanel
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x0003A0CC File Offset: 0x000382CC
		public void RemoveAllDeliveredNotifications()
		{
			this._deliveredNotifications.Clear();
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x0003A0DC File Offset: 0x000382DC
		public void RequestAuthorization(OnAuthorizationRequestComplete authorizationRequestComplete)
		{
			this.AuthorizationStatus = (true ? AuthorizationStatus.Authorized : AuthorizationStatus.Denied);
			if (authorizationRequestComplete != null)
			{
				authorizationRequestComplete(this.AuthorizationStatus == AuthorizationStatus.Authorized);
			}
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x0003A10A File Offset: 0x0003830A
		public void Setup()
		{
			if (this.AuthorizationStatus == AuthorizationStatus.Authorized)
			{
				this._tickRegistry.AppTicking += this.Tick;
			}
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x0003A12C File Offset: 0x0003832C
		public void ScheduleNotification(string identifier, SystemNotificationContent content, SystemNotificationTrigger trigger)
		{
			this._scheduledNotifications.Add(identifier, new SystemNotification(identifier, content, trigger));
			CalendarNotificationTrigger calendarNotificationTrigger = trigger as CalendarNotificationTrigger;
			if (calendarNotificationTrigger != null)
			{
				ConsoleSystemNotificationService.Log.Info(string.Format("ScheduleNotification({0}, {1}) for {2} {3}/{4}/{5}", new object[]
				{
					identifier,
					content.Title,
					calendarNotificationTrigger.Hour,
					calendarNotificationTrigger.Day,
					calendarNotificationTrigger.Month,
					calendarNotificationTrigger.Year
				}), Array.Empty<object>());
			}
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x0003A1BB File Offset: 0x000383BB
		public void RemoveScheduledNotification(string identifier)
		{
			this._scheduledNotifications.Remove(identifier);
			ConsoleSystemNotificationService.Log.Info("RemoveScheduledNotification(" + identifier + ")", Array.Empty<object>());
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x0003A1E9 File Offset: 0x000383E9
		public void RemoveAllScheduledNotifications()
		{
			this._scheduledNotifications.Clear();
		}

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x0600114D RID: 4429 RVA: 0x0003A1F8 File Offset: 0x000383F8
		// (remove) Token: 0x0600114E RID: 4430 RVA: 0x0003A230 File Offset: 0x00038430
		public event NotificationReceived OnNotificationReceived;

		// Token: 0x0600114F RID: 4431 RVA: 0x0003A268 File Offset: 0x00038468
		private void Tick(float deltaTime)
		{
			if (this.AuthorizationStatus != AuthorizationStatus.Authorized)
			{
				this._tickRegistry.AppTicking -= this.Tick;
				return;
			}
			DateTime currentTime = GameDateTime.UtcNow;
			List<KeyValuePair<string, SystemNotification>> notificationsToSend = new List<KeyValuePair<string, SystemNotification>>();
			foreach (KeyValuePair<string, SystemNotification> scheduledNotification in this._scheduledNotifications)
			{
				CalendarNotificationTrigger calendarNotificationTrigger = scheduledNotification.Value.Trigger as CalendarNotificationTrigger;
				if (calendarNotificationTrigger != null && !this._deliveredNotifications.ContainsKey(scheduledNotification.Key))
				{
					DateTime deliveryTime = this.DeliveryTime(calendarNotificationTrigger);
					if (currentTime >= deliveryTime && currentTime < deliveryTime + TimeSpan.FromSeconds(30.0))
					{
						notificationsToSend.Add(scheduledNotification);
					}
				}
			}
			foreach (KeyValuePair<string, SystemNotification> notification in notificationsToSend)
			{
				ConsoleSystemNotificationService.Log.Info("[Notification] " + notification.Key, Array.Empty<object>());
				NotificationReceived onNotificationReceived = this.OnNotificationReceived;
				if (onNotificationReceived != null)
				{
					onNotificationReceived(notification.Key, notification.Value.Content);
				}
				if (notification.Value.Content.Badge >= 0)
				{
					this.ApplicationBadge = notification.Value.Content.Badge;
				}
				this.RemoveScheduledNotification(notification.Key);
				this._deliveredNotifications.Add(notification.Key, notification.Value);
			}
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x0003A418 File Offset: 0x00038618
		private DateTime DeliveryTime(CalendarNotificationTrigger calendarTrigger)
		{
			DateTime now = GameDateTime.UtcNow;
			return new DateTime(calendarTrigger.Year ?? now.Year, calendarTrigger.Month ?? now.Month, calendarTrigger.Day ?? now.Day, calendarTrigger.Hour ?? now.Hour, calendarTrigger.Minute ?? now.Minute, calendarTrigger.Second ?? now.Second, DateTimeKind.Local);
		}

		// Token: 0x04000F12 RID: 3858
		public const string DebugSystemNotificationsEditorPrefKey = "DebugSystemNotificationsEditorPrefKey";

		// Token: 0x04000F13 RID: 3859
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ConsoleSystemNotificationService");

		// Token: 0x04000F14 RID: 3860
		private readonly Dictionary<string, SystemNotification> _scheduledNotifications = new Dictionary<string, SystemNotification>();

		// Token: 0x04000F15 RID: 3861
		private readonly Dictionary<string, SystemNotification> _deliveredNotifications = new Dictionary<string, SystemNotification>();

		// Token: 0x04000F16 RID: 3862
		private AuthorizationStatus _authorizationStatus;

		// Token: 0x04000F17 RID: 3863
		[Dependency]
		private TickRegistry _tickRegistry;
	}
}
