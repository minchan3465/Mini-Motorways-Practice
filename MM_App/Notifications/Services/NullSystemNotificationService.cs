using System;
using System.Collections.Generic;

namespace Notifications.Services
{
	// Token: 0x020002C4 RID: 708
	public class NullSystemNotificationService : ISystemNotificationService
	{
		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06001166 RID: 4454 RVA: 0x0003A6A7 File Offset: 0x000388A7
		// (set) Token: 0x06001167 RID: 4455 RVA: 0x0003A6AF File Offset: 0x000388AF
		public int ApplicationBadge { get; set; }

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06001168 RID: 4456 RVA: 0x0003A6B8 File Offset: 0x000388B8
		public List<SystemNotification> ScheduledNotifications { get; }

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06001169 RID: 4457 RVA: 0x0003A6C0 File Offset: 0x000388C0
		public List<SystemNotification> DeliveredNotifications { get; }

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x0600116A RID: 4458 RVA: 0x00004C50 File Offset: 0x00002E50
		public AuthorizationStatus AuthorizationStatus
		{
			get
			{
				return AuthorizationStatus.Unknown;
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x0600116B RID: 4459 RVA: 0x0000222C File Offset: 0x0000042C
		public bool RequiresOptionsPanel
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x0600116C RID: 4460 RVA: 0x0000222C File Offset: 0x0000042C
		public bool IsAvailable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Setup()
		{
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ScheduleNotification(string identifier, SystemNotificationContent content, SystemNotificationTrigger trigger)
		{
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RemoveScheduledNotification(string identifier)
		{
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RemoveAllScheduledNotifications()
		{
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RemoveAllDeliveredNotifications()
		{
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RequestAuthorization(OnAuthorizationRequestComplete authorizationRequestComplete)
		{
		}

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x06001173 RID: 4467 RVA: 0x000022F5 File Offset: 0x000004F5
		// (remove) Token: 0x06001174 RID: 4468 RVA: 0x000022F5 File Offset: 0x000004F5
		public event NotificationReceived OnNotificationReceived
		{
			add
			{
			}
			remove
			{
			}
		}
	}
}
