using System;

namespace Notifications.Triggers
{
	// Token: 0x020002C0 RID: 704
	public class CalendarNotificationTrigger : SystemNotificationTrigger
	{
		// Token: 0x17000371 RID: 881
		// (get) Token: 0x0600112C RID: 4396 RVA: 0x00039DC4 File Offset: 0x00037FC4
		// (set) Token: 0x0600112D RID: 4397 RVA: 0x00039DCC File Offset: 0x00037FCC
		public int? Year { get; set; }

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x0600112E RID: 4398 RVA: 0x00039DD5 File Offset: 0x00037FD5
		// (set) Token: 0x0600112F RID: 4399 RVA: 0x00039DDD File Offset: 0x00037FDD
		public int? Month { get; set; }

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06001130 RID: 4400 RVA: 0x00039DE6 File Offset: 0x00037FE6
		// (set) Token: 0x06001131 RID: 4401 RVA: 0x00039DEE File Offset: 0x00037FEE
		public int? Day { get; set; }

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06001132 RID: 4402 RVA: 0x00039DF7 File Offset: 0x00037FF7
		// (set) Token: 0x06001133 RID: 4403 RVA: 0x00039DFF File Offset: 0x00037FFF
		public int? Hour { get; set; }

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06001134 RID: 4404 RVA: 0x00039E08 File Offset: 0x00038008
		// (set) Token: 0x06001135 RID: 4405 RVA: 0x00039E10 File Offset: 0x00038010
		public int? Minute { get; set; }

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06001136 RID: 4406 RVA: 0x00039E19 File Offset: 0x00038019
		// (set) Token: 0x06001137 RID: 4407 RVA: 0x00039E21 File Offset: 0x00038021
		public int? Second { get; set; }

		// Token: 0x06001138 RID: 4408 RVA: 0x00039E2C File Offset: 0x0003802C
		public bool MatchesDateTime(DateTime dateTime)
		{
			return (this.Year == null || this.Year.Value == dateTime.Year) && (this.Month == null || this.Month.Value == dateTime.Month) && (this.Day == null || this.Day.Value == dateTime.Day) && (this.Hour == null || this.Hour.Value == dateTime.Hour) && (this.Minute == null || this.Minute.Value == dateTime.Minute) && (this.Second == null || this.Second.Value == dateTime.Second);
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x00039F30 File Offset: 0x00038130
		public DateTime AsDateTime(DateTime now)
		{
			return new DateTime(this.Year ?? now.Year, this.Month ?? now.Month, this.Day ?? now.Day, this.Hour ?? now.Hour, this.Minute ?? now.Minute, this.Second ?? now.Second);
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x0003A004 File Offset: 0x00038204
		public override string ToString()
		{
			return string.Format("{0}:{1}:{2} {3}/{4}/{5}", new object[]
			{
				this.Hour,
				this.Minute,
				this.Second,
				this.Day,
				this.Month,
				this.Year
			});
		}
	}
}
