using System;
using System.Collections.Generic;
using NotificationService.Events;

namespace NotificationService
{
	// Token: 0x020002A7 RID: 679
	public class NullNotificationEventSystem : INotificationEventSystem
	{
		// Token: 0x060010BF RID: 4287 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RecordEvent(INotificationEventType eventType, bool immediatelyRunScheduler = true)
		{
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RemoveEvent(int id)
		{
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RemoveAll()
		{
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x00004BD9 File Offset: 0x00002DD9
		public List<NotificationEvent> EventsOnDay(DateTime day)
		{
			return null;
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x060010C3 RID: 4291 RVA: 0x000394C8 File Offset: 0x000376C8
		public NotificationEvent? LatestEvent
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x060010C4 RID: 4292 RVA: 0x00004BD9 File Offset: 0x00002DD9
		public List<NotificationEvent> AllEvents
		{
			get
			{
				return null;
			}
		}
	}
}
