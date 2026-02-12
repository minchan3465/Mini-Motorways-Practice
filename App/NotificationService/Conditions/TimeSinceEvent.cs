using System;
using System.Collections.Generic;
using NotificationService.Events;
using UnityEngine;

namespace NotificationService.Conditions
{
	// Token: 0x020002B8 RID: 696
	[Serializable]
	public class TimeSinceEvent : INotificationCondition
	{
		// Token: 0x06001106 RID: 4358 RVA: 0x00039BD8 File Offset: 0x00037DD8
		public bool Evaluate(DateTime onDate, INotificationEventSystem notificationEventSystem)
		{
			List<NotificationEvent> allEventsWithType = new List<NotificationEvent>();
			foreach (NotificationEvent notificationEvent in notificationEventSystem.AllEvents)
			{
				if (this._notificationEventTypeQuery.Matches(notificationEvent.EventType, onDate) && notificationEvent.OccuredAt <= onDate)
				{
					allEventsWithType.Add(notificationEvent);
				}
			}
			if (allEventsWithType.Count == 0)
			{
				return false;
			}
			NotificationEvent latestEvent = allEventsWithType[0];
			for (int eventIndex = 1; eventIndex < allEventsWithType.Count; eventIndex++)
			{
				NotificationEvent notificationEvent2 = allEventsWithType[eventIndex];
				if (notificationEvent2.OccuredAt > latestEvent.OccuredAt)
				{
					latestEvent = notificationEvent2;
				}
			}
			int timeSinceDays = (int)Math.Floor((onDate.Date - latestEvent.OccuredAt.Date).TotalDays);
			switch (this.comparator)
			{
			case Comparator.Equals:
				return timeSinceDays == this.days;
			case Comparator.LessThan:
				return timeSinceDays < this.days;
			case Comparator.LessThanOrEqual:
				return timeSinceDays <= this.days;
			case Comparator.GreaterThan:
				return timeSinceDays > this.days;
			case Comparator.GreaterThanOrEqual:
				return timeSinceDays >= this.days;
			default:
				Diagnostics.FailAssert("Unknown comparator for notification condition `TimeSinceEvent`", Array.Empty<object>());
				return false;
			}
		}

		// Token: 0x04000EFB RID: 3835
		[SerializeReference]
		public INotificationEventTypeQuery _notificationEventTypeQuery;

		// Token: 0x04000EFC RID: 3836
		public Comparator comparator;

		// Token: 0x04000EFD RID: 3837
		public int days = 1;
	}
}
