using System;

namespace Notifications
{
	// Token: 0x020002BD RID: 701
	public class SystemNotification
	{
		// Token: 0x0600111F RID: 4383 RVA: 0x00039D4B File Offset: 0x00037F4B
		public SystemNotification(string identifier, SystemNotificationContent content, SystemNotificationTrigger trigger)
		{
			this.Identifier = identifier;
			this.Content = content;
			this.Trigger = trigger;
		}

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06001120 RID: 4384 RVA: 0x00039D68 File Offset: 0x00037F68
		public string Identifier { get; }

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06001121 RID: 4385 RVA: 0x00039D70 File Offset: 0x00037F70
		public SystemNotificationContent Content { get; }

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06001122 RID: 4386 RVA: 0x00039D78 File Offset: 0x00037F78
		public SystemNotificationTrigger Trigger { get; }
	}
}
