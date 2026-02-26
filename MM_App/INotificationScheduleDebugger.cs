using System;
using System.Collections.Generic;

// Token: 0x020001F8 RID: 504
public interface INotificationScheduleDebugger
{
	// Token: 0x1700028A RID: 650
	// (get) Token: 0x06000BC2 RID: 3010
	bool IsAvailable { get; }

	// Token: 0x14000022 RID: 34
	// (add) Token: 0x06000BC3 RID: 3011
	// (remove) Token: 0x06000BC4 RID: 3012
	event OnMarkerAdded MarkerAdded;

	// Token: 0x14000023 RID: 35
	// (add) Token: 0x06000BC5 RID: 3013
	// (remove) Token: 0x06000BC6 RID: 3014
	event OnMarkerTypeRemoved MarkerTypeRemoved;

	// Token: 0x06000BC7 RID: 3015
	void AddDebugMarkersForTruePeriods(List<List<NotificationScheduler.DatePeriod>> truePeriodsForDescriptors, NotificationDescriptorDatabase descriptorDatabase);

	// Token: 0x06000BC8 RID: 3016
	void AddMarker(NotificationScheduleDebuggerMarkerType markerType, DateTime dateTime, string text);

	// Token: 0x06000BC9 RID: 3017
	void ClearMarkers();
}
