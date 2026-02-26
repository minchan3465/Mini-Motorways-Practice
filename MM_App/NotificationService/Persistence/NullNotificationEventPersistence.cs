using System;
using System.Collections.Generic;
using NotificationService.Events;

namespace NotificationService.Persistence
{
	// Token: 0x020002AB RID: 683
	public class NullNotificationEventPersistence : INotificationEventPersistence
	{
		// Token: 0x060010DD RID: 4317 RVA: 0x000022F5 File Offset: 0x000004F5
		public void AddEvent(NotificationEvent notificationEvent)
		{
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x000022F5 File Offset: 0x000004F5
		public void UpdateEventWithId(int id, NotificationEvent updatedNotificationEvent)
		{
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RemoveEventWithId(int id)
		{
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RemoveAll()
		{
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x060010E1 RID: 4321 RVA: 0x00039728 File Offset: 0x00037928
		public NotificationEvent? LatestEvent
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x060010E2 RID: 4322 RVA: 0x00004BD9 File Offset: 0x00002DD9
		public List<NotificationEvent> Events
		{
			get
			{
				return null;
			}
		}
	}
}
