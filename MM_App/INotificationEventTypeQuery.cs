using System;

// Token: 0x020001EB RID: 491
public interface INotificationEventTypeQuery
{
	// Token: 0x17000282 RID: 642
	// (get) Token: 0x06000B95 RID: 2965
	string QueryName { get; }

	// Token: 0x06000B96 RID: 2966
	bool Matches(INotificationEventType eventType, DateTime onDate);
}
