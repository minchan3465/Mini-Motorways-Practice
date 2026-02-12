using System;
using Factory;
using JetBrains.Annotations;
using Motorways;
using UnityEngine;
using UnityEngine.UI;

namespace Popups
{
	// Token: 0x020002CF RID: 719
	public class DebugOverlayScreen : BasePopup
	{
		// Token: 0x060011AD RID: 4525 RVA: 0x0003AC9C File Offset: 0x00038E9C
		private void Start()
		{
			this._currentYear = GameDateTime.UtcNow.Year;
			this._currentMonth = GameDateTime.UtcNow.Month;
			this._yearSelector.SetOption(this._currentYear - 2022);
			this._monthSelector.SetOption(this._currentMonth - 1);
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x0003ACF9 File Offset: 0x00038EF9
		public void OnYearChanged()
		{
			this._currentYear = 2022 + this._yearSelector.SelectedOptionIndex;
			this.UpdateCalendar();
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x0003AD18 File Offset: 0x00038F18
		public void OnMonthChanged()
		{
			this._currentMonth = this._monthSelector.SelectedOptionIndex + 1;
			this.UpdateCalendar();
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x0003AD34 File Offset: 0x00038F34
		private void UpdateCalendar()
		{
			DateTime time = new DateTime(this._currentYear, this._currentMonth, 1);
			time = new DateTime(time.Year, time.Month, time.Day);
			int daysInMonth = DateTime.DaysInMonth(time.Year, time.Month);
			YearOfChallenges yearOfChallenges = this._challengeSystem.GetYearOfChallengesForYear(this._currentYear);
			if (yearOfChallenges == null)
			{
				for (int buttonIndex = this._calendarDayButtons.Length - 1; buttonIndex >= 0; buttonIndex--)
				{
					this._calendarDayButtons[buttonIndex].gameObject.SetActive(false);
				}
				return;
			}
			MonthOfDailyChallenges monthOfDailyChallenges = yearOfChallenges.monthsOfDailyChallenges[this._currentMonth - 1];
			for (int buttonIndex2 = this._calendarDayButtons.Length - 1; buttonIndex2 >= 0; buttonIndex2--)
			{
				this._calendarDayButtons[buttonIndex2].gameObject.SetActive(buttonIndex2 < daysInMonth);
				if (monthOfDailyChallenges.dailyChallenges.Length > buttonIndex2)
				{
					string challenges = "";
					challenges = challenges + "<color=\"purple\">" + monthOfDailyChallenges.dailyChallenges[buttonIndex2].city.ToString() + "</color> \n";
					foreach (ChallengeData challengeData in monthOfDailyChallenges.dailyChallenges[buttonIndex2].challenges)
					{
						MotorwaysStringKey titleKey = this._scope.Get<MotorwaysStringKey>();
						StringId challengeTitleStringId;
						if (Enum.TryParse<StringId>(challengeData.challengeName, out challengeTitleStringId))
						{
							titleKey.InitWithStringId(challengeTitleStringId);
							string str = challenges;
							StandaloneLocString standaloneLocString = StandaloneLocString.CreateString(this._scope, titleKey);
							challenges = str + ((standaloneLocString != null) ? standaloneLocString.ToString() : null) + ",";
						}
					}
					this._calendarDayButtons[buttonIndex2].GetComponentInChildren<Text>().text = (buttonIndex2 + 1).ToString() + "th\n" + challenges;
				}
			}
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x0003AEF8 File Offset: 0x000390F8
		public void OnDayButtonPressed(int day)
		{
			DateTime dateTime = new DateTime(this._currentYear, this._currentMonth, day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second);
			TimeSpan timeSpan = dateTime - DateTime.UtcNow;
			if (dateTime > DateTime.UtcNow)
			{
				AdjustableGameDateTime adjustableGameDateTime = GameDateTime.Backend as AdjustableGameDateTime;
				if (adjustableGameDateTime != null)
				{
					adjustableGameDateTime.UtcOffset = default(TimeSpan);
					adjustableGameDateTime.UtcOffset += TimeSpan.FromDays((double)(timeSpan.Days + 1));
					return;
				}
			}
			else
			{
				AdjustableGameDateTime adjustableGameDateTime2 = GameDateTime.Backend as AdjustableGameDateTime;
				if (adjustableGameDateTime2 != null)
				{
					adjustableGameDateTime2.UtcOffset = default(TimeSpan);
					adjustableGameDateTime2.UtcOffset += TimeSpan.FromDays((double)timeSpan.Days);
				}
			}
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x0003AFD3 File Offset: 0x000391D3
		[UsedImplicitly]
		public void ClosePressed()
		{
			this._popupStack.PopPopup(false);
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x0003AFE4 File Offset: 0x000391E4
		public override void Reset()
		{
			base.Reset();
			this._currentYear = GameDateTime.UtcNow.Year;
			this._currentMonth = GameDateTime.UtcNow.Month;
		}

		// Token: 0x04000F3E RID: 3902
		[SerializeField]
		private TouchOptionButton _yearSelector;

		// Token: 0x04000F3F RID: 3903
		[SerializeField]
		private TouchOptionButton _monthSelector;

		// Token: 0x04000F40 RID: 3904
		[SerializeField]
		private Button[] _calendarDayButtons = new Button[31];

		// Token: 0x04000F41 RID: 3905
		[Dependency]
		private ChallengeSystem _challengeSystem;

		// Token: 0x04000F42 RID: 3906
		[Dependency]
		private IScope _scope;

		// Token: 0x04000F43 RID: 3907
		[Dependency]
		private PopupStack _popupStack;

		// Token: 0x04000F44 RID: 3908
		private const int _startingYear = 2022;

		// Token: 0x04000F45 RID: 3909
		private int _currentYear = 2023;

		// Token: 0x04000F46 RID: 3910
		private int _currentMonth;
	}
}
