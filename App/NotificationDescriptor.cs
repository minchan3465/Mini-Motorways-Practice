using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001EC RID: 492
[CreateAssetMenu(fileName = "New Notification Descriptor", menuName = "Motorways/Notifications/Notification Descriptor", order = 1)]
public class NotificationDescriptor : ScriptableObject
{
	// Token: 0x17000283 RID: 643
	// (get) Token: 0x06000B97 RID: 2967 RVA: 0x0000E3A3 File Offset: 0x0000C5A3
	public string Id
	{
		get
		{
			return base.name;
		}
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x00027B38 File Offset: 0x00025D38
	public override bool Equals(object other)
	{
		NotificationDescriptor otherDescriptor = other as NotificationDescriptor;
		return otherDescriptor != null && base.name == otherDescriptor.name;
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x00027B62 File Offset: 0x00025D62
	public override int GetHashCode()
	{
		return this.Id.GetHashCode();
	}

	// Token: 0x040006B2 RID: 1714
	public NotificationDescriptor.MessageCategory category;

	// Token: 0x040006B3 RID: 1715
	public List<NotificationDescriptor.GameNotificationMessage> messages = new List<NotificationDescriptor.GameNotificationMessage>();

	// Token: 0x040006B4 RID: 1716
	public NotificationDescriptor.MessageDeliveryMethod messageDeliveryMethod;

	// Token: 0x040006B5 RID: 1717
	public List<NotificationBooleanExpression> conditions = new List<NotificationBooleanExpression>();

	// Token: 0x020001ED RID: 493
	[Serializable]
	public enum MessageDeliveryMethod
	{
		// Token: 0x040006B7 RID: 1719
		Consecutive,
		// Token: 0x040006B8 RID: 1720
		Random
	}

	// Token: 0x020001EE RID: 494
	[Serializable]
	public enum MessageCategory
	{
		// Token: 0x040006BA RID: 1722
		Content = 1,
		// Token: 0x040006BB RID: 1723
		Challenge
	}

	// Token: 0x020001EF RID: 495
	[Serializable]
	public struct GameNotificationMessage
	{
		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000B9B RID: 2971 RVA: 0x00027B90 File Offset: 0x00025D90
		// (set) Token: 0x06000B9C RID: 2972 RVA: 0x00027BAF File Offset: 0x00025DAF
		public StringId Title
		{
			get
			{
				StringId stringId;
				if (Enum.TryParse<StringId>(this._title, out stringId))
				{
					return stringId;
				}
				return StringId.None;
			}
			set
			{
				this._title = value.ToString();
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000B9D RID: 2973 RVA: 0x00027BC4 File Offset: 0x00025DC4
		// (set) Token: 0x06000B9E RID: 2974 RVA: 0x00027BE3 File Offset: 0x00025DE3
		public StringId Body
		{
			get
			{
				StringId stringId;
				if (Enum.TryParse<StringId>(this._body, out stringId))
				{
					return stringId;
				}
				return StringId.None;
			}
			set
			{
				this._body = value.ToString();
			}
		}

		// Token: 0x040006BC RID: 1724
		[StringEnumSearch(typeof(StringId))]
		[SerializeField]
		private string _title;

		// Token: 0x040006BD RID: 1725
		[SerializeField]
		[StringEnumSearch(typeof(StringId))]
		private string _body;
	}
}
