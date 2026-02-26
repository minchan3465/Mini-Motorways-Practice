using System;
using System.Collections.Generic;

// Token: 0x020001EA RID: 490
public interface INotificationEventTypeWithData : INotificationEventType
{
	// Token: 0x06000B92 RID: 2962
	bool InitFromJson(JSON.Dictionary json);

	// Token: 0x06000B93 RID: 2963
	void ToJson(ref Dictionary<string, object> json);

	// Token: 0x06000B94 RID: 2964
	bool DataMatches(INotificationEventTypeWithData eventTypeWithData);
}
