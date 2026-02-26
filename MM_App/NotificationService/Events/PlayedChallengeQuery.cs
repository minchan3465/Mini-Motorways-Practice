using System;
using Factory;
using Motorways;

namespace NotificationService.Events
{
	// Token: 0x020002B2 RID: 690
	[Factory.Serializable(1)]
	public class PlayedChallengeQuery : INotificationEventTypeQuery
	{
		// Token: 0x17000362 RID: 866
		// (get) Token: 0x060010F8 RID: 4344 RVA: 0x00039924 File Offset: 0x00037B24
		public string QueryName
		{
			get
			{
				return "PlayedChallenge";
			}
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x0003992C File Offset: 0x00037B2C
		public bool Matches(INotificationEventType eventType, DateTime onDate)
		{
			PlayedChallenge playedChallenge = eventType as PlayedChallenge;
			if (playedChallenge == null)
			{
				return false;
			}
			if (this.Type != playedChallenge.Type)
			{
				return false;
			}
			if (this.Time == ChallengeTime.LastPlayed)
			{
				return true;
			}
			ChallengeType type = playedChallenge.Type;
			if (type == ChallengeType.Daily)
			{
				DateTime currentChallengeStartDate = onDate.Date;
				DateTime challengeStartDate = currentChallengeStartDate;
				if (this.Time == ChallengeTime.Previous)
				{
					challengeStartDate = currentChallengeStartDate - TimeSpan.FromDays(1.0);
				}
				return playedChallenge.TimeStart == ChallengeSystem.ToTimestamp(challengeStartDate.Date);
			}
			if (type == ChallengeType.Weekly)
			{
				DateTime currentChallengeStartDate2 = ChallengeSystem.StartOfWeek(onDate);
				DateTime challengeStartDate2 = currentChallengeStartDate2;
				if (this.Time == ChallengeTime.Previous)
				{
					challengeStartDate2 = currentChallengeStartDate2 - TimeSpan.FromDays(7.0);
				}
				return playedChallenge.TimeStart == ChallengeSystem.ToTimestamp(challengeStartDate2.Date);
			}
			return false;
		}

		// Token: 0x04000EEC RID: 3820
		public ChallengeType Type;

		// Token: 0x04000EED RID: 3821
		public ChallengeTime Time;
	}
}
