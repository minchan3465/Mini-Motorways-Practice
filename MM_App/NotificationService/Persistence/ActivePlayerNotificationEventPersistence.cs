using System;
using System.Collections.Generic;
using Factory;
using Motorways;
using NotificationService.Events;

namespace NotificationService.Persistence
{
	// Token: 0x020002A8 RID: 680
	public class ActivePlayerNotificationEventPersistence : INotificationEventPersistence
	{
		// Token: 0x060010C6 RID: 4294 RVA: 0x000394DE File Offset: 0x000376DE
		public void AddEvent(NotificationEvent notificationEvent)
		{
			this._activePlayer.AddGameNotificationEvent(notificationEvent);
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x000394EC File Offset: 0x000376EC
		public void UpdateEventWithId(int id, NotificationEvent updatedNotificationEvent)
		{
			this._activePlayer.UpdateGameNotificationEventWithId(id, updatedNotificationEvent);
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x000394FB File Offset: 0x000376FB
		public void RemoveEventWithId(int id)
		{
			Diagnostics.FailAssert("UserProfileNotificationEventPersistence does not implement RemoveEvent", Array.Empty<object>());
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x0003950C File Offset: 0x0003770C
		public void RemoveAll()
		{
			this._activePlayer.RemoveAllNotificationEvents();
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x060010CA RID: 4298 RVA: 0x00039519 File Offset: 0x00037719
		public NotificationEvent? LatestEvent
		{
			get
			{
				return this._activePlayer.LatestNotificationEvent;
			}
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x060010CB RID: 4299 RVA: 0x00039526 File Offset: 0x00037726
		public List<NotificationEvent> Events
		{
			get
			{
				return this._activePlayer.NotificationEvents;
			}
		}

		// Token: 0x04000EDA RID: 3802
		[Dependency]
		protected ActivePlayer _activePlayer;
	}
}
