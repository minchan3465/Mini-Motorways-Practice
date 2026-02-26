using System;
using System.Runtime.InteropServices;
using Notifications.Triggers;

namespace Notifications.Services.iOSService
{
	// Token: 0x020002C5 RID: 709
	public class iOSContentData
	{
		// Token: 0x06001176 RID: 4470 RVA: 0x0003A6C8 File Offset: 0x000388C8
		internal static iOSContentData.NotificationContentData ToContentData(string identifier, SystemNotificationContent systemNotificationContent)
		{
			return new iOSContentData.NotificationContentData
			{
				identifier = identifier,
				title = systemNotificationContent.Title,
				body = systemNotificationContent.Body,
				badge = systemNotificationContent.Badge,
				subtitle = systemNotificationContent.Subtitle
			};
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x0003A71C File Offset: 0x0003891C
		internal static iOSContentData.CalendarTriggerData ToContentData(CalendarNotificationTrigger calendarNotificationTrigger)
		{
			return new iOSContentData.CalendarTriggerData
			{
				year = calendarNotificationTrigger.Year.GetValueOrDefault(-1),
				month = calendarNotificationTrigger.Month.GetValueOrDefault(-1),
				day = calendarNotificationTrigger.Day.GetValueOrDefault(-1),
				hour = calendarNotificationTrigger.Hour.GetValueOrDefault(-1),
				minute = calendarNotificationTrigger.Minute.GetValueOrDefault(-1),
				second = calendarNotificationTrigger.Second.GetValueOrDefault(-1)
			};
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x0003A7B8 File Offset: 0x000389B8
		internal static iOSContentData.TimeIntervalTriggerData ToContentData(TimeIntervalNotificationTrigger calendarNotificationTrigger)
		{
			return new iOSContentData.TimeIntervalTriggerData
			{
				timeIntervalSeconds = (int)calendarNotificationTrigger.TimeInterval.TotalSeconds,
				repeats = calendarNotificationTrigger.Repeats
			};
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x0003A7F0 File Offset: 0x000389F0
		internal static IntPtr ToIntPtr<T>(T data)
		{
			IntPtr dataPointer = Marshal.AllocHGlobal(Marshal.SizeOf<T>(data));
			Marshal.StructureToPtr<T>(data, dataPointer, false);
			return dataPointer;
		}

		// Token: 0x04000F20 RID: 3872
		public const int InvalidCalendarDate = -1;

		// Token: 0x020002C6 RID: 710
		public struct NotificationContentData
		{
			// Token: 0x04000F21 RID: 3873
			public string identifier;

			// Token: 0x04000F22 RID: 3874
			public string title;

			// Token: 0x04000F23 RID: 3875
			public string body;

			// Token: 0x04000F24 RID: 3876
			public int badge;

			// Token: 0x04000F25 RID: 3877
			public string subtitle;

			// Token: 0x04000F26 RID: 3878
			public string categoryIdentifier;

			// Token: 0x04000F27 RID: 3879
			public string threadIdentifier;

			// Token: 0x04000F28 RID: 3880
			public string data;

			// Token: 0x04000F29 RID: 3881
			public bool showInForeground;

			// Token: 0x04000F2A RID: 3882
			public int showInForegroundPresentationOptions;
		}

		// Token: 0x020002C7 RID: 711
		public struct TimeIntervalTriggerData
		{
			// Token: 0x04000F2B RID: 3883
			public int timeIntervalSeconds;

			// Token: 0x04000F2C RID: 3884
			public bool repeats;
		}

		// Token: 0x020002C8 RID: 712
		public struct CalendarTriggerData
		{
			// Token: 0x04000F2D RID: 3885
			public int year;

			// Token: 0x04000F2E RID: 3886
			public int month;

			// Token: 0x04000F2F RID: 3887
			public int day;

			// Token: 0x04000F30 RID: 3888
			public int hour;

			// Token: 0x04000F31 RID: 3889
			public int minute;

			// Token: 0x04000F32 RID: 3890
			public int second;
		}
	}
}
