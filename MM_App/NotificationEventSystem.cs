using System;
using System.Collections.Generic;
using Factory;
using NotificationService.Events;
using NotificationService.Persistence;

// Token: 0x020001F2 RID: 498
public class NotificationEventSystem : INotificationEventSystem
{
	// Token: 0x06000BA6 RID: 2982 RVA: 0x00027C0C File Offset: 0x00025E0C
	public void RecordEvent(INotificationEventType eventType, bool immediatelyRunScheduler = true)
	{
		if (Diagnostics.Verify(this._activePlayer.HasActivePlayer, "Cannot record events when we don't have a player! Tried to record {0}", eventType))
		{
			NotificationEvent newNotificationEvent = new NotificationEvent(GameDateTime.UtcNow.Date, eventType);
			List<NotificationEvent> list = this.EventsOnDay(newNotificationEvent.OccuredAt);
			bool foundExistingEventOnSameDate = false;
			foreach (NotificationEvent existingEvent in list)
			{
				if (existingEvent.EventType.Matches(eventType))
				{
					NotificationEventSystem.Log.Info("RecordEvent) UpdatingEventWithId - Id: {0}, {1}", new object[]
					{
						existingEvent.Id,
						newNotificationEvent.EventType.GetType()
					});
					this._persistence.UpdateEventWithId(existingEvent.Id, newNotificationEvent);
					foundExistingEventOnSameDate = true;
					break;
				}
			}
			if (!foundExistingEventOnSameDate)
			{
				NotificationEventSystem.Log.Info("RecordEvent) AddEvent - {0}", new object[]
				{
					newNotificationEvent.EventType.GetType()
				});
				this._persistence.AddEvent(newNotificationEvent);
			}
			if (immediatelyRunScheduler)
			{
				this._notificationScheduler.ScheduleNotifications();
			}
		}
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x00027D2C File Offset: 0x00025F2C
	public void RemoveEvent(int id)
	{
		this._persistence.RemoveEventWithId(id);
		this._notificationScheduler.ScheduleNotifications();
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x00027D45 File Offset: 0x00025F45
	public void RemoveAll()
	{
		this._persistence.RemoveAll();
		this._notificationScheduler.ScheduleNotifications();
	}

	// Token: 0x17000288 RID: 648
	// (get) Token: 0x06000BA9 RID: 2985 RVA: 0x00027D5D File Offset: 0x00025F5D
	public NotificationEvent? LatestEvent
	{
		get
		{
			return this._persistence.LatestEvent;
		}
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x00027D6C File Offset: 0x00025F6C
	public List<NotificationEvent> EventsOnDay(DateTime day)
	{
		List<NotificationEvent> eventsOnDay = new List<NotificationEvent>();
		foreach (NotificationEvent gameNotificationEvent in this.AllEvents)
		{
			if (gameNotificationEvent.OccuredAt.Date == day.Date)
			{
				eventsOnDay.Add(gameNotificationEvent);
			}
		}
		return eventsOnDay;
	}

	// Token: 0x17000289 RID: 649
	// (get) Token: 0x06000BAB RID: 2987 RVA: 0x00027DE4 File Offset: 0x00025FE4
	public List<NotificationEvent> AllEvents
	{
		get
		{
			return this._persistence.Events;
		}
	}

	// Token: 0x040006BF RID: 1727
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("NotificationEventSystem");

	// Token: 0x040006C0 RID: 1728
	[Dependency]
	private INotificationEventPersistence _persistence;

	// Token: 0x040006C1 RID: 1729
	[Dependency]
	private NotificationScheduler _notificationScheduler;

	// Token: 0x040006C2 RID: 1730
	[Dependency]
	private IActivePlayer _activePlayer;
}
