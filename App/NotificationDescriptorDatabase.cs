using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001F0 RID: 496
[CreateAssetMenu(fileName = "New GameNotificationDatabase", menuName = "Motorways/Notifications/Game Notification Database", order = 2)]
public class NotificationDescriptorDatabase : ScriptableObject
{
	// Token: 0x040006BE RID: 1726
	[NonReorderable]
	public List<NotificationDescriptor> gameNotifications = new List<NotificationDescriptor>();
}
