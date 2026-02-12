using System;

// Token: 0x020001E9 RID: 489
public static class NotificationEventTypeExtensions
{
	// Token: 0x06000B91 RID: 2961 RVA: 0x00027AFC File Offset: 0x00025CFC
	public static bool Matches(this INotificationEventType eventA, INotificationEventType eventB)
	{
		INotificationEventTypeWithData eventAWithData = eventA as INotificationEventTypeWithData;
		if (eventAWithData != null)
		{
			INotificationEventTypeWithData eventBWithData = eventB as INotificationEventTypeWithData;
			if (eventBWithData != null)
			{
				return eventAWithData.DataMatches(eventBWithData);
			}
		}
		return eventA.GetType() == eventB.GetType();
	}
}
