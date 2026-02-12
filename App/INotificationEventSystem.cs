using System;
using System.Collections.Generic;
using NotificationService.Events;

// Token: 0x020001F1 RID: 497
public interface INotificationEventSystem
{
	// Token: 0x06000BA0 RID: 2976
	void RecordEvent(INotificationEventType eventType, bool immediatelyRunScheduler = true);

	// Token: 0x06000BA1 RID: 2977
	void RemoveEvent(int id);

	// Token: 0x06000BA2 RID: 2978
	void RemoveAll();

	// Token: 0x06000BA3 RID: 2979
	List<NotificationEvent> EventsOnDay(DateTime day);

	// Token: 0x17000286 RID: 646
	// (get) Token: 0x06000BA4 RID: 2980
	NotificationEvent? LatestEvent { get; }

	// Token: 0x17000287 RID: 647
	// (get) Token: 0x06000BA5 RID: 2981
	List<NotificationEvent> AllEvents { get; }
}
