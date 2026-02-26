using System;
using System.Collections.Generic;
using NotificationService.Events;

namespace NotificationService.Persistence
{
	// Token: 0x020002A9 RID: 681
	public class InMemoryNotificationEventPersistence : INotificationEventPersistence
	{
		// Token: 0x060010CD RID: 4301 RVA: 0x00039533 File Offset: 0x00037733
		public void AddEvent(NotificationEvent notificationEvent)
		{
			notificationEvent.Id = this._nextId;
			this._nextId++;
			this._events.Add(notificationEvent);
			this.UpdateLatestEvent(notificationEvent);
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x00039564 File Offset: 0x00037764
		public void RemoveEventWithId(int id)
		{
			this._events.RemoveAt(this.IndexOf(id));
			if (this._latestEvent != null && this._latestEvent.GetValueOrDefault().Id == id)
			{
				if (this._events.Count == 0)
				{
					this._latestEvent = null;
					return;
				}
				this._latestEvent = new NotificationEvent?(this.FindLatestEvent());
			}
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x000395D2 File Offset: 0x000377D2
		public void RemoveAll()
		{
			this._events.Clear();
			this._latestEvent = null;
			this._nextId = 0;
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x000395F2 File Offset: 0x000377F2
		public void UpdateEventWithId(int id, NotificationEvent updatedNotificationEvent)
		{
			updatedNotificationEvent.Id = id;
			this._events[this.IndexOf(id)] = updatedNotificationEvent;
			this.UpdateLatestEvent(updatedNotificationEvent);
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x00039618 File Offset: 0x00037818
		private int IndexOf(int id)
		{
			for (int eventIndex = 0; eventIndex < this._events.Count; eventIndex++)
			{
				if (this._events[eventIndex].Id == id)
				{
					return eventIndex;
				}
			}
			return -1;
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x00039658 File Offset: 0x00037858
		private NotificationEvent FindLatestEvent()
		{
			NotificationEvent latestEvent = this._events[0];
			for (int eventIndex = 1; eventIndex < this._events.Count; eventIndex++)
			{
				NotificationEvent notificationEvent = this._events[eventIndex];
				if (notificationEvent.OccuredAt > latestEvent.OccuredAt)
				{
					latestEvent = notificationEvent;
				}
			}
			return latestEvent;
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x000396B0 File Offset: 0x000378B0
		private void UpdateLatestEvent(NotificationEvent newNotificationEvent)
		{
			if (this._latestEvent == null)
			{
				this._latestEvent = new NotificationEvent?(newNotificationEvent);
				return;
			}
			if (newNotificationEvent.OccuredAt > this._latestEvent.Value.OccuredAt)
			{
				this._latestEvent = new NotificationEvent?(newNotificationEvent);
			}
		}

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x060010D4 RID: 4308 RVA: 0x00039704 File Offset: 0x00037904
		public NotificationEvent? LatestEvent
		{
			get
			{
				return this._latestEvent;
			}
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x060010D5 RID: 4309 RVA: 0x0003970C File Offset: 0x0003790C
		public List<NotificationEvent> Events
		{
			get
			{
				return this._events;
			}
		}

		// Token: 0x04000EDB RID: 3803
		private int _nextId;

		// Token: 0x04000EDC RID: 3804
		private readonly List<NotificationEvent> _events = new List<NotificationEvent>();

		// Token: 0x04000EDD RID: 3805
		private NotificationEvent? _latestEvent;
	}
}
