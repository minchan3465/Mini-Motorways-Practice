using System;
using Motorways;

namespace NotificationService.Conditions
{
	// Token: 0x020002B5 RID: 693
	public class TimeSince : INotificationCondition
	{
		// Token: 0x06001104 RID: 4356 RVA: 0x00039B2C File Offset: 0x00037D2C
		public bool Evaluate(DateTime onDate, INotificationEventSystem notificationEventSystem)
		{
			if (this.otherEvent != TimeSince.OtherEvent.WeeklyChallengeStarted)
			{
				return false;
			}
			int daysSinceWeeklyChallengeStarted = (int)Math.Floor((onDate - ChallengeSystem.StartOfWeek(onDate)).TotalDays);
			switch (this.comparator)
			{
			case Comparator.Equals:
				return daysSinceWeeklyChallengeStarted == this.days;
			case Comparator.LessThan:
				return daysSinceWeeklyChallengeStarted < this.days;
			case Comparator.LessThanOrEqual:
				return daysSinceWeeklyChallengeStarted <= this.days;
			case Comparator.GreaterThan:
				return daysSinceWeeklyChallengeStarted > this.days;
			case Comparator.GreaterThanOrEqual:
				return daysSinceWeeklyChallengeStarted >= this.days;
			default:
				Diagnostics.FailAssert("Unknown comparator for notification condition `TimeSince`", Array.Empty<object>());
				return false;
			}
		}

		// Token: 0x04000EF0 RID: 3824
		public TimeSince.OtherEvent otherEvent;

		// Token: 0x04000EF1 RID: 3825
		public Comparator comparator;

		// Token: 0x04000EF2 RID: 3826
		public int days = 1;

		// Token: 0x020002B6 RID: 694
		public enum OtherEvent
		{
			// Token: 0x04000EF4 RID: 3828
			WeeklyChallengeStarted
		}
	}
}
