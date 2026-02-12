using System;
using System.Collections.Generic;

// Token: 0x020001FA RID: 506
public class NotificationScheduleDebugger : INotificationScheduleDebugger
{
	// Token: 0x1700028C RID: 652
	// (get) Token: 0x06000BD3 RID: 3027 RVA: 0x000020AA File Offset: 0x000002AA
	public bool IsAvailable
	{
		get
		{
			return true;
		}
	}

	// Token: 0x14000026 RID: 38
	// (add) Token: 0x06000BD4 RID: 3028 RVA: 0x000284E8 File Offset: 0x000266E8
	// (remove) Token: 0x06000BD5 RID: 3029 RVA: 0x00028520 File Offset: 0x00026720
	public event OnMarkerAdded MarkerAdded;

	// Token: 0x14000027 RID: 39
	// (add) Token: 0x06000BD6 RID: 3030 RVA: 0x00028558 File Offset: 0x00026758
	// (remove) Token: 0x06000BD7 RID: 3031 RVA: 0x00028590 File Offset: 0x00026790
	public event OnMarkerTypeRemoved MarkerTypeRemoved;

	// Token: 0x06000BD8 RID: 3032 RVA: 0x000285C8 File Offset: 0x000267C8
	public void AddDebugMarkersForTruePeriods(List<List<NotificationScheduler.DatePeriod>> truePeriodsForDescriptors, NotificationDescriptorDatabase descriptorDatabase)
	{
		for (int descriptorIndex = 0; descriptorIndex < truePeriodsForDescriptors.Count; descriptorIndex++)
		{
			NotificationDescriptor descriptor = descriptorDatabase.gameNotifications[descriptorIndex];
			foreach (NotificationScheduler.DatePeriod datePeriod in truePeriodsForDescriptors[descriptorIndex])
			{
				this.AddMarker(NotificationScheduleDebuggerMarkerType.DescriptorConditionsTrueStart, datePeriod.startDate, descriptor.Id ?? "");
				if (datePeriod.endDate != null)
				{
					this.AddMarker(NotificationScheduleDebuggerMarkerType.DescriptorConditionsTrueEnd, datePeriod.endDate.Value, descriptor.Id ?? "");
				}
			}
		}
		for (int descriptorIndex2 = 0; descriptorIndex2 < truePeriodsForDescriptors.Count; descriptorIndex2++)
		{
			NotificationDescriptor descriptor2 = descriptorDatabase.gameNotifications[descriptorIndex2];
			foreach (NotificationScheduler.DatePeriod datePeriod2 in truePeriodsForDescriptors[descriptorIndex2])
			{
				this.AddMarker(NotificationScheduleDebuggerMarkerType.ScheduledNotification, datePeriod2.startDate, descriptor2.Id ?? "");
			}
		}
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x00028704 File Offset: 0x00026904
	public void AddMarker(NotificationScheduleDebuggerMarkerType markerType, DateTime dateTime, string text)
	{
		OnMarkerAdded markerAdded = this.MarkerAdded;
		if (markerAdded == null)
		{
			return;
		}
		markerAdded(markerType, dateTime, text);
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x00028719 File Offset: 0x00026919
	public void ClearMarkers()
	{
		this.RemoveEventsWithType(NotificationScheduleDebuggerMarkerType.DescriptorConditionsTrue);
		this.RemoveEventsWithType(NotificationScheduleDebuggerMarkerType.DescriptorConditionsTrueStart);
		this.RemoveEventsWithType(NotificationScheduleDebuggerMarkerType.DescriptorConditionsTrueEnd);
		this.RemoveEventsWithType(NotificationScheduleDebuggerMarkerType.ScheduledNotification);
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x00028737 File Offset: 0x00026937
	private void RemoveEventsWithType(NotificationScheduleDebuggerMarkerType type)
	{
		OnMarkerTypeRemoved markerTypeRemoved = this.MarkerTypeRemoved;
		if (markerTypeRemoved == null)
		{
			return;
		}
		markerTypeRemoved(type);
	}
}
