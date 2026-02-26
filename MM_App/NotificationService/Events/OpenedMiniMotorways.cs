using System;
using Factory;

namespace NotificationService.Events
{
	// Token: 0x020002AE RID: 686
	[Factory.Serializable(1)]
	public class OpenedMiniMotorways : INotificationEventType, INotificationEventTypeQuery
	{
		// Token: 0x17000361 RID: 865
		// (get) Token: 0x060010F0 RID: 4336 RVA: 0x0003982C File Offset: 0x00037A2C
		public string QueryName
		{
			get
			{
				return "OpenedMiniMotorways";
			}
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x00039833 File Offset: 0x00037A33
		public bool Matches(INotificationEventType eventType, DateTime onDate)
		{
			return base.GetType() == eventType.GetType();
		}
	}
}
