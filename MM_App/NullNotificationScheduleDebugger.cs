using System;
using System.Collections.Generic;

// Token: 0x020001F9 RID: 505
public class NullNotificationScheduleDebugger : INotificationScheduleDebugger
{
	// Token: 0x1700028B RID: 651
	// (get) Token: 0x06000BCA RID: 3018 RVA: 0x0000222C File Offset: 0x0000042C
	public bool IsAvailable
	{
		get
		{
			return false;
		}
	}

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x06000BCB RID: 3019 RVA: 0x000022F5 File Offset: 0x000004F5
	// (remove) Token: 0x06000BCC RID: 3020 RVA: 0x000022F5 File Offset: 0x000004F5
	public event OnMarkerAdded MarkerAdded
	{
		add
		{
		}
		remove
		{
		}
	}

	// Token: 0x14000025 RID: 37
	// (add) Token: 0x06000BCD RID: 3021 RVA: 0x000022F5 File Offset: 0x000004F5
	// (remove) Token: 0x06000BCE RID: 3022 RVA: 0x000022F5 File Offset: 0x000004F5
	public event OnMarkerTypeRemoved MarkerTypeRemoved
	{
		add
		{
		}
		remove
		{
		}
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x000022F5 File Offset: 0x000004F5
	public void AddDebugMarkersForTruePeriods(List<List<NotificationScheduler.DatePeriod>> truePeriodsForDescriptors, NotificationDescriptorDatabase descriptorDatabase)
	{
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x000022F5 File Offset: 0x000004F5
	public void AddMarker(NotificationScheduleDebuggerMarkerType markerType, DateTime dateTime, string text)
	{
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x000022F5 File Offset: 0x000004F5
	public void ClearMarkers()
	{
	}
}
