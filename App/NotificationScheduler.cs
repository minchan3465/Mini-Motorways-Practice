using System;
using System.Collections.Generic;
using Factory;
using Notifications;
using Notifications.Triggers;

// Token: 0x020001F3 RID: 499
public class NotificationScheduler
{
	// Token: 0x06000BAE RID: 2990 RVA: 0x00027E02 File Offset: 0x00026002
	public void OnPlayerChanged(Player oldPlayer, Player newPlayer)
	{
		this._scheduledLocale = LocaleDatabase.LocaleId.Unknown;
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x00027E0C File Offset: 0x0002600C
	public void OnPlayerDataChanged()
	{
		LocaleDatabase.LocaleId newLocale = this._player.LocaleId;
		if (newLocale != this._scheduledLocale)
		{
			this._scheduledLocale = newLocale;
			this.ScheduleNotifications();
		}
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x00027E3C File Offset: 0x0002603C
	public void ScheduleTestNotification()
	{
		if (this._notificationDescriptorDatabase.gameNotifications.Count > 0)
		{
			NotificationDescriptor descriptor = this._notificationDescriptorDatabase.gameNotifications[this._testNotificationIndex];
			SystemNotificationContent systemNotificationContent = this.CreateSystemNotificationContentFromDescriptor(descriptor);
			int second = GameDateTime.UtcNow.Second + 15;
			if (second >= 60)
			{
				second -= 60;
			}
			CalendarNotificationTrigger notificationTrigger = new CalendarNotificationTrigger
			{
				Second = new int?(second)
			};
			this._systemNotificationService.ScheduleNotification(string.Format("{0}_test", descriptor.Id), systemNotificationContent, notificationTrigger);
			this._testNotificationIndex++;
			if (this._testNotificationIndex >= this._notificationDescriptorDatabase.gameNotifications.Count)
			{
				this._testNotificationIndex = 0;
			}
		}
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x00027EF8 File Offset: 0x000260F8
	public void ScheduleNotifications()
	{
		if (!this._systemNotificationService.IsAvailable && !this._scheduleDebugger.IsAvailable)
		{
			return;
		}
		DateTime now = GameDateTime.UtcNow;
		DateTime startDate = now.Date;
		DateTime endDate = startDate + TimeSpan.FromDays(30.0);
		this._scheduleDebugger.ClearMarkers();
		if (this._scheduleDebugger.IsAvailable)
		{
			startDate = new DateTime(now.Year, now.Month, 1);
			endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
		}
		Dictionary<DateTime, List<NotificationDescriptor>> conditionsTrueOnDates = this.FindConditionsTrueOnDates(startDate, endDate);
		List<List<NotificationScheduler.DatePeriod>> truePeriodsForDescriptors = this.CalculateTruePeriodsForDescriptors(startDate, endDate, conditionsTrueOnDates);
		this.ScheduleNotificationsWithSystem(truePeriodsForDescriptors);
		this._scheduleDebugger.AddDebugMarkersForTruePeriods(truePeriodsForDescriptors, this._notificationDescriptorDatabase);
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x00027FC8 File Offset: 0x000261C8
	private bool IsNotificationCategoryAllowedByPlayer(NotificationDescriptor descriptor)
	{
		NotificationDescriptor.MessageCategory category = descriptor.category;
		if (category != NotificationDescriptor.MessageCategory.Content)
		{
			return category != NotificationDescriptor.MessageCategory.Challenge || this._player.IsChallengeRemindersEnabledSetting;
		}
		return this._player.IsContentRemindersEnabledSetting;
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x00028000 File Offset: 0x00026200
	private void ScheduleNotificationsWithSystem(List<List<NotificationScheduler.DatePeriod>> truePeriodsForDescriptors)
	{
		this._systemNotificationService.RemoveAllScheduledNotifications();
		HashSet<DateTime> daysWithScheduledNotifications = new HashSet<DateTime>();
		for (int descriptorIndex = 0; descriptorIndex < truePeriodsForDescriptors.Count; descriptorIndex++)
		{
			NotificationDescriptor descriptor = this._notificationDescriptorDatabase.gameNotifications[descriptorIndex];
			SystemNotificationContent systemNotificationContent = this.CreateSystemNotificationContentFromDescriptor(descriptor);
			List<NotificationScheduler.DatePeriod> truePeriodsForDescriptor = truePeriodsForDescriptors[descriptorIndex];
			for (int truePeriodIndex = 0; truePeriodIndex < truePeriodsForDescriptor.Count; truePeriodIndex++)
			{
				DateTime notificationDateUTC = truePeriodsForDescriptor[truePeriodIndex].startDate;
				if (!daysWithScheduledNotifications.Contains(notificationDateUTC))
				{
					DateTime notificationDateLocal = TimeZoneInfo.ConvertTimeFromUtc(notificationDateUTC, TimeZoneInfo.Local);
					DateTime notificationDateTimeUTC = TimeZoneInfo.ConvertTimeToUtc(new DateTime(notificationDateLocal.Year, notificationDateLocal.Month, notificationDateLocal.Day, 9, 30, 0));
					CalendarNotificationTrigger notificationTrigger = new CalendarNotificationTrigger
					{
						Year = new int?(notificationDateTimeUTC.Year),
						Month = new int?(notificationDateTimeUTC.Month),
						Day = new int?(notificationDateTimeUTC.Day),
						Hour = new int?(notificationDateTimeUTC.Hour),
						Minute = new int?(notificationDateTimeUTC.Minute)
					};
					this._systemNotificationService.ScheduleNotification(string.Format("{0}_{1}", descriptor.Id, truePeriodIndex), systemNotificationContent, notificationTrigger);
					daysWithScheduledNotifications.Add(notificationDateUTC);
				}
				else
				{
					NotificationScheduler.Log.Info("{0} was not scheduled on {1} as a notification is already present on that day.", new object[]
					{
						descriptor.Id,
						notificationDateUTC
					});
				}
			}
		}
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x00028180 File Offset: 0x00026380
	private SystemNotificationContent CreateSystemNotificationContentFromDescriptor(NotificationDescriptor descriptor)
	{
		SystemNotificationContent systemNotificationContent = new SystemNotificationContent();
		if (descriptor.messages.Count > 0)
		{
			NotificationDescriptor.GameNotificationMessage gameNotificationMessage = descriptor.messages[0];
			systemNotificationContent.Title = StandaloneLocString.CreateString(this._scope, gameNotificationMessage.Title).ToString();
			systemNotificationContent.Body = StandaloneLocString.CreateString(this._scope, gameNotificationMessage.Body).ToString();
		}
		else
		{
			Diagnostics.FailAssert("{0} would have been scheduled but had no messages set.", new object[]
			{
				descriptor
			});
		}
		systemNotificationContent.Badge = 1;
		return systemNotificationContent;
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x00028208 File Offset: 0x00026408
	private List<List<NotificationScheduler.DatePeriod>> CalculateTruePeriodsForDescriptors(DateTime startDate, DateTime endDate, Dictionary<DateTime, List<NotificationDescriptor>> conditionsTrueOnDates)
	{
		List<List<DateTime>> truePeriods = new List<List<DateTime>>();
		for (int descriptorIndex = 0; descriptorIndex < this._notificationDescriptorDatabase.gameNotifications.Count; descriptorIndex++)
		{
			truePeriods.Add(new List<DateTime>());
		}
		for (int descriptorIndex2 = 0; descriptorIndex2 < this._notificationDescriptorDatabase.gameNotifications.Count; descriptorIndex2++)
		{
			NotificationDescriptor descriptor = this._notificationDescriptorDatabase.gameNotifications[descriptorIndex2];
			DateTime dateTime = startDate;
			while (dateTime < endDate)
			{
				List<DateTime> truePeriodsForDescriptor = truePeriods[descriptorIndex2];
				bool isTrueOnDay = conditionsTrueOnDates.ContainsKey(dateTime) && conditionsTrueOnDates[dateTime].Contains(descriptor);
				if (truePeriodsForDescriptor.Count % 2 == 0)
				{
					if (isTrueOnDay)
					{
						truePeriodsForDescriptor.Add(dateTime);
					}
				}
				else if (!isTrueOnDay)
				{
					truePeriodsForDescriptor.Add(dateTime - TimeSpan.FromDays(1.0));
				}
				dateTime += TimeSpan.FromDays(1.0);
			}
		}
		List<List<NotificationScheduler.DatePeriod>> truePeriodsForDescriptors = new List<List<NotificationScheduler.DatePeriod>>();
		for (int descriptorIndex3 = 0; descriptorIndex3 < this._notificationDescriptorDatabase.gameNotifications.Count; descriptorIndex3++)
		{
			List<DateTime> truePeriodsForDescriptor2 = truePeriods[descriptorIndex3];
			truePeriodsForDescriptors.Add(new List<NotificationScheduler.DatePeriod>());
			for (int periodIndex = 0; periodIndex < truePeriodsForDescriptor2.Count; periodIndex += 2)
			{
				DateTime periodStartDate = truePeriodsForDescriptor2[periodIndex];
				DateTime? periodEndDate = null;
				if (periodIndex + 1 < truePeriodsForDescriptor2.Count)
				{
					periodEndDate = new DateTime?(truePeriodsForDescriptor2[periodIndex + 1]);
				}
				truePeriodsForDescriptors[truePeriodsForDescriptors.Count - 1].Add(new NotificationScheduler.DatePeriod
				{
					startDate = periodStartDate,
					endDate = periodEndDate
				});
			}
		}
		return truePeriodsForDescriptors;
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x000283B0 File Offset: 0x000265B0
	private Dictionary<DateTime, List<NotificationDescriptor>> FindConditionsTrueOnDates(DateTime startDate, DateTime endDate)
	{
		Dictionary<DateTime, List<NotificationDescriptor>> conditionsTrueOnDate = new Dictionary<DateTime, List<NotificationDescriptor>>();
		DateTime currentDate = startDate;
		while (currentDate <= endDate)
		{
			foreach (NotificationDescriptor descriptor in this._notificationDescriptorDatabase.gameNotifications)
			{
				if (descriptor == null)
				{
					Diagnostics.FailAssert("Found null descriptor in database while scheduling notifications. Is an active notification descriptor set to 'None'?", Array.Empty<object>());
				}
				else if (this.IsNotificationCategoryAllowedByPlayer(descriptor))
				{
					bool conditionsAreTrueOnCurrentDate = true;
					using (List<NotificationBooleanExpression>.Enumerator enumerator2 = descriptor.conditions.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (!enumerator2.Current.IsTrue(currentDate, this._notificationEventSystem))
							{
								conditionsAreTrueOnCurrentDate = false;
								break;
							}
						}
					}
					if (conditionsAreTrueOnCurrentDate)
					{
						List<NotificationDescriptor> trueOnDateDescriptors;
						if (!conditionsTrueOnDate.TryGetValue(currentDate, out trueOnDateDescriptors))
						{
							trueOnDateDescriptors = new List<NotificationDescriptor>();
						}
						trueOnDateDescriptors.Add(descriptor);
						conditionsTrueOnDate[currentDate] = trueOnDateDescriptors;
					}
				}
			}
			currentDate += TimeSpan.FromDays(1.0);
		}
		return conditionsTrueOnDate;
	}

	// Token: 0x040006C3 RID: 1731
	private const int MaxNumberOfDaysToScheduleFor = 30;

	// Token: 0x040006C4 RID: 1732
	private const int LocalNotificationTimeHour = 9;

	// Token: 0x040006C5 RID: 1733
	private const int LocalNotificationTimeMinute = 30;

	// Token: 0x040006C6 RID: 1734
	[Dependency]
	private IScope _scope;

	// Token: 0x040006C7 RID: 1735
	[Dependency]
	private ISystemNotificationService _systemNotificationService;

	// Token: 0x040006C8 RID: 1736
	[Dependency]
	private NotificationDescriptorDatabase _notificationDescriptorDatabase;

	// Token: 0x040006C9 RID: 1737
	[Dependency]
	private INotificationEventSystem _notificationEventSystem;

	// Token: 0x040006CA RID: 1738
	[Dependency]
	private INotificationScheduleDebugger _scheduleDebugger;

	// Token: 0x040006CB RID: 1739
	[Dependency]
	private IActivePlayer _player;

	// Token: 0x040006CC RID: 1740
	private LocaleDatabase.LocaleId _scheduledLocale;

	// Token: 0x040006CD RID: 1741
	private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("NotificationScheduler");

	// Token: 0x040006CE RID: 1742
	private int _testNotificationIndex;

	// Token: 0x040006CF RID: 1743
	public const int TestNotificationSeconds = 15;

	// Token: 0x020001F4 RID: 500
	public class DatePeriod
	{
		// Token: 0x040006D0 RID: 1744
		public DateTime startDate;

		// Token: 0x040006D1 RID: 1745
		public DateTime? endDate;
	}
}
