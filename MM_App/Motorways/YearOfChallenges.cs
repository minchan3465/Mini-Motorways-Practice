using System;
using System.Linq;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000354 RID: 852
	[CreateAssetMenu(fileName = "New Challenge Years", menuName = "Motorways/Challenges/Year Of Challenges", order = 2)]
	public class YearOfChallenges : ScriptableObject
	{
		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x060014F1 RID: 5361 RVA: 0x00045B28 File Offset: 0x00043D28
		public DateTime MondayOfFirstWeek
		{
			get
			{
				DateTime dateTime = new DateTime(this.year, 1, 1);
				if (dateTime.DayOfWeek == DayOfWeek.Monday)
				{
					return dateTime;
				}
				int dayOffset = dateTime.DayOfWeek - DayOfWeek.Monday;
				dayOffset = (dayOffset % 7 + 7) % 7;
				dateTime = dateTime.AddDays((double)(7 - dayOffset));
				return dateTime;
			}
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x00045B70 File Offset: 0x00043D70
		public PrecalculatedTimedChallengeData GetChallengesOnDay(DateTime dateTime)
		{
			MonthOfDailyChallenges month;
			if (Diagnostics.Verify(this.monthsOfDailyChallenges.Length >= dateTime.Month, "There are only {0} months' of challenges, and current month is {1}", this.monthsOfDailyChallenges.Length, dateTime.Month))
			{
				month = this.monthsOfDailyChallenges[dateTime.Month - 1];
			}
			else if (Diagnostics.Verify(this.monthsOfDailyChallenges.Length != 0, "No challenge months at all!! Returning empty challenge object."))
			{
				month = this.monthsOfDailyChallenges.Last<MonthOfDailyChallenges>();
			}
			else
			{
				month = new MonthOfDailyChallenges();
				month.dailyChallenges = new PrecalculatedTimedChallengeData[]
				{
					new PrecalculatedTimedChallengeData
					{
						name = "Fallback Challenge",
						city = MapDefinition.CityNames.Wellington,
						challenges = Array.Empty<ChallengeData>()
					}
				};
			}
			if (Diagnostics.Verify(month.dailyChallenges.Length >= dateTime.Day, "No challenges found for day {0} of month {1}!", dateTime.Day, dateTime.Month))
			{
				return month.dailyChallenges[dateTime.Day - 1];
			}
			if (Diagnostics.Verify(month.dailyChallenges.Length != 0))
			{
				return month.dailyChallenges.Last<PrecalculatedTimedChallengeData>();
			}
			return new PrecalculatedTimedChallengeData
			{
				name = "Fallback Challenge",
				city = MapDefinition.CityNames.Wellington,
				challenges = Array.Empty<ChallengeData>()
			};
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x00045CB0 File Offset: 0x00043EB0
		public PrecalculatedTimedChallengeData GetChallengesOnWeekOfDay(DateTime dateTime)
		{
			if (!Diagnostics.Verify(dateTime >= this.MondayOfFirstWeek, "Trying to get a weekly challenge of a day before the first monday of the year!"))
			{
				return this.weeklyChallenges[0];
			}
			int daysBetweenDates = dateTime.DayOfYear - this.MondayOfFirstWeek.DayOfYear;
			int weekIndex = daysBetweenDates / 7;
			if (Diagnostics.Verify(weekIndex < this.weeklyChallenges.Length, "Somehow calculated {0} as the week index? Had {1} days between {2} and {3}", weekIndex, daysBetweenDates, dateTime, this.MondayOfFirstWeek))
			{
				return this.weeklyChallenges[weekIndex];
			}
			return this.weeklyChallenges[this.weeklyChallenges.Length - 1];
		}

		// Token: 0x0400116A RID: 4458
		public ulong seed;

		// Token: 0x0400116B RID: 4459
		public int year = 2022;

		// Token: 0x0400116C RID: 4460
		public MonthOfDailyChallenges[] monthsOfDailyChallenges = new MonthOfDailyChallenges[12];

		// Token: 0x0400116D RID: 4461
		public PrecalculatedTimedChallengeData[] weeklyChallenges = new PrecalculatedTimedChallengeData[52];
	}
}
