using System;

namespace NotificationService.Events
{
	// Token: 0x020002AD RID: 685
	public struct NotificationEvent
	{
		// Token: 0x1700035E RID: 862
		// (get) Token: 0x060010EB RID: 4331 RVA: 0x000397F4 File Offset: 0x000379F4
		// (set) Token: 0x060010EC RID: 4332 RVA: 0x000397FC File Offset: 0x000379FC
		public int Id { readonly get; set; }

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x060010ED RID: 4333 RVA: 0x00039805 File Offset: 0x00037A05
		public readonly DateTime OccuredAt { get; }

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x060010EE RID: 4334 RVA: 0x0003980D File Offset: 0x00037A0D
		public readonly INotificationEventType EventType { get; }

		// Token: 0x060010EF RID: 4335 RVA: 0x00039815 File Offset: 0x00037A15
		public NotificationEvent(DateTime occuredAt, INotificationEventType eventType)
		{
			this.OccuredAt = occuredAt;
			this.EventType = eventType;
			this.Id = -1;
		}

		// Token: 0x04000EDF RID: 3807
		public const int InvalidId = -1;
	}
}
