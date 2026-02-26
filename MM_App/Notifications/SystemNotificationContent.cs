using System;

namespace Notifications
{
	// Token: 0x020002BE RID: 702
	public class SystemNotificationContent
	{
		// Token: 0x1700036D RID: 877
		// (get) Token: 0x06001123 RID: 4387 RVA: 0x00039D80 File Offset: 0x00037F80
		// (set) Token: 0x06001124 RID: 4388 RVA: 0x00039D88 File Offset: 0x00037F88
		public string Title { get; set; }

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06001125 RID: 4389 RVA: 0x00039D91 File Offset: 0x00037F91
		// (set) Token: 0x06001126 RID: 4390 RVA: 0x00039D99 File Offset: 0x00037F99
		public string Subtitle { get; set; }

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06001127 RID: 4391 RVA: 0x00039DA2 File Offset: 0x00037FA2
		// (set) Token: 0x06001128 RID: 4392 RVA: 0x00039DAA File Offset: 0x00037FAA
		public string Body { get; set; }

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06001129 RID: 4393 RVA: 0x00039DB3 File Offset: 0x00037FB3
		// (set) Token: 0x0600112A RID: 4394 RVA: 0x00039DBB File Offset: 0x00037FBB
		public int Badge { get; set; }
	}
}
