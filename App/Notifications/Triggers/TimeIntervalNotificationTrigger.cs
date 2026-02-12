using System;

namespace Notifications.Triggers
{
	// Token: 0x020002C1 RID: 705
	public class TimeIntervalNotificationTrigger : SystemNotificationTrigger
	{
		// Token: 0x17000377 RID: 887
		// (get) Token: 0x0600113C RID: 4412 RVA: 0x0003A075 File Offset: 0x00038275
		// (set) Token: 0x0600113D RID: 4413 RVA: 0x0003A07D File Offset: 0x0003827D
		public bool Repeats { get; set; }

		// Token: 0x04000F11 RID: 3857
		public TimeSpan TimeInterval;
	}
}
