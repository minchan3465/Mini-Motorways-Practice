using System;
using UnityEngine;

// Token: 0x020001E7 RID: 487
[Serializable]
public class NotificationBooleanExpression
{
	// Token: 0x06000B8F RID: 2959 RVA: 0x00027AB8 File Offset: 0x00025CB8
	public bool IsTrue(DateTime onDate, INotificationEventSystem notificationEventSystem)
	{
		if (this.condition == null)
		{
			Diagnostics.FailAssert("condition is null in NotificationBooleanExpression", Array.Empty<object>());
			return false;
		}
		bool result = this.condition.Evaluate(onDate, notificationEventSystem);
		if (!this.not)
		{
			return result;
		}
		return !result;
	}

	// Token: 0x040006B0 RID: 1712
	public bool not;

	// Token: 0x040006B1 RID: 1713
	[SerializeReference]
	public INotificationCondition condition;
}
