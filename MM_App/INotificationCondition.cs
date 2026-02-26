using System;

// Token: 0x020001E6 RID: 486
public interface INotificationCondition
{
	// Token: 0x06000B8E RID: 2958
	bool Evaluate(DateTime onDate, INotificationEventSystem notificationEventSystem);
}
