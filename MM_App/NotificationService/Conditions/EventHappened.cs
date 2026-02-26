using System;
using NotificationService.Events;
using UnityEngine;

namespace NotificationService.Conditions
{
	// Token: 0x020002B4 RID: 692
	[Serializable]
	public class EventHappened : INotificationCondition
	{
		// Token: 0x06001102 RID: 4354 RVA: 0x00039AA4 File Offset: 0x00037CA4
		public bool Evaluate(DateTime onDate, INotificationEventSystem notificationEventSystem)
		{
			foreach (NotificationEvent notificationEvent in notificationEventSystem.AllEvents)
			{
				if (this._notificationEventTypeQuery.Matches(notificationEvent.EventType, onDate) && notificationEvent.OccuredAt.Date <= onDate.Date)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000EEF RID: 3823
		[SerializeReference]
		public INotificationEventTypeQuery _notificationEventTypeQuery;
	}
}
