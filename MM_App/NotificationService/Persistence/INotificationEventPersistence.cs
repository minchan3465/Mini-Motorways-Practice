using System;
using System.Collections.Generic;
using NotificationService.Events;

namespace NotificationService.Persistence
{
	// Token: 0x020002AA RID: 682
	public interface INotificationEventPersistence
	{
		// Token: 0x060010D7 RID: 4311
		void AddEvent(NotificationEvent notificationEvent);

		// Token: 0x060010D8 RID: 4312
		void UpdateEventWithId(int id, NotificationEvent updatedNotificationEvent);

		// Token: 0x060010D9 RID: 4313
		void RemoveEventWithId(int id);

		// Token: 0x060010DA RID: 4314
		void RemoveAll();

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x060010DB RID: 4315
		NotificationEvent? LatestEvent { get; }

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x060010DC RID: 4316
		List<NotificationEvent> Events { get; }
	}
}
