using System;
using System.Collections.Generic;
using Factory;

namespace NotificationService.Events
{
	// Token: 0x020002B0 RID: 688
	[Factory.Serializable(1)]
	public class PlayedChallenge : INotificationEventTypeWithData, INotificationEventType
	{
		// Token: 0x060010F3 RID: 4339 RVA: 0x00039848 File Offset: 0x00037A48
		public bool InitFromJson(JSON.Dictionary json)
		{
			if (!json.ContainsKey("Type") || !json.ContainsKey("TimeStart"))
			{
				return false;
			}
			this.TimeStart = json.GetInt("TimeStart", 0);
			return Enum.TryParse<ChallengeType>(json.GetString("Type"), true, out this.Type);
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x0003989A File Offset: 0x00037A9A
		public void ToJson(ref Dictionary<string, object> json)
		{
			json["Type"] = this.Type.ToString();
			json["TimeStart"] = this.TimeStart;
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x000398D0 File Offset: 0x00037AD0
		public bool DataMatches(INotificationEventTypeWithData eventTypeWithData)
		{
			PlayedChallenge playedChallenge = eventTypeWithData as PlayedChallenge;
			return playedChallenge != null && this.Type == playedChallenge.Type && this.TimeStart == playedChallenge.TimeStart;
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x00039907 File Offset: 0x00037B07
		public override string ToString()
		{
			return string.Format("PlayedChallenge-{0}", this.Type.ToString());
		}

		// Token: 0x04000EE6 RID: 3814
		public ChallengeType Type;

		// Token: 0x04000EE7 RID: 3815
		public int TimeStart;
	}
}
