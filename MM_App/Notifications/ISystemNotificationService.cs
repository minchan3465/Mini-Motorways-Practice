using System;
using System.Collections.Generic;

namespace Notifications
{
	// Token: 0x020002BB RID: 699
	public interface ISystemNotificationService
	{
		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06001110 RID: 4368
		// (set) Token: 0x06001111 RID: 4369
		int ApplicationBadge { get; set; }

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06001112 RID: 4370
		List<SystemNotification> ScheduledNotifications { get; }

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06001113 RID: 4371
		List<SystemNotification> DeliveredNotifications { get; }

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06001114 RID: 4372
		bool IsAvailable { get; }

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06001115 RID: 4373
		AuthorizationStatus AuthorizationStatus { get; }

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06001116 RID: 4374
		bool RequiresOptionsPanel { get; }

		// Token: 0x06001117 RID: 4375
		void Setup();

		// Token: 0x06001118 RID: 4376
		void ScheduleNotification(string identifier, SystemNotificationContent content, SystemNotificationTrigger trigger);

		// Token: 0x06001119 RID: 4377
		void RemoveScheduledNotification(string identifier);

		// Token: 0x0600111A RID: 4378
		void RemoveAllScheduledNotifications();

		// Token: 0x0600111B RID: 4379
		void RemoveAllDeliveredNotifications();

		// Token: 0x0600111C RID: 4380
		void RequestAuthorization(OnAuthorizationRequestComplete authorizationRequestComplete = null);

		// Token: 0x14000033 RID: 51
		// (add) Token: 0x0600111D RID: 4381
		// (remove) Token: 0x0600111E RID: 4382
		event NotificationReceived OnNotificationReceived;
	}
}
