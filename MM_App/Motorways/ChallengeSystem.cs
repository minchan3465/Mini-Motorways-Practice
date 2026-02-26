using System;
using System.Collections.Generic;
using Factory;

namespace Motorways
{
	// Token: 0x0200034D RID: 845
	public class ChallengeSystem : ICreatedInScopeHandler
	{
		// Token: 0x060014C7 RID: 5319 RVA: 0x00044D10 File Offset: 0x00042F10
		public YearOfChallenges GetYearOfChallengesForYear(int year)
		{
			foreach (YearOfChallenges challenges in this._challengeDatabase.precalculatedChallenges)
			{
				if (challenges.year == year)
				{
					return challenges;
				}
			}
			return null;
		}

		// Token: 0x060014C8 RID: 5320 RVA: 0x00044D74 File Offset: 0x00042F74
		private bool TryGetPrecalculatedDailyChallenge(out MapChallenge result)
		{
			DateTime time = this.DateTimeNow;
			time = new DateTime(time.Year, time.Month, time.Day);
			int startTime = ChallengeSystem.ToTimestamp(time);
			int endTime = ChallengeSystem.ToTimestamp(time.AddDays(1.0));
			if (this._cachedDailyChallenge != null && this._cachedDailyChallenge.TimeStart == startTime)
			{
				result = this._cachedDailyChallenge;
				return true;
			}
			MapChallenge challengeOverride;
			if (this._challengeOverrides.TryGetDailyChallenge(startTime, endTime, out challengeOverride))
			{
				result = challengeOverride;
				this._cachedDailyChallenge = result;
				return true;
			}
			YearOfChallenges yearOfChallenges = this.GetYearOfChallengesForYear(time.Year);
			if (yearOfChallenges == null)
			{
				result = null;
				return false;
			}
			PrecalculatedTimedChallengeData data = yearOfChallenges.GetChallengesOnDay(time);
			MapDefinition mapDefinition = this._mapDatabase.MapLibrary.GetMapByName(data.city.ToString());
			result = MapChallenge.CreateDailyChallenge(this, mapDefinition, data.challenges, startTime, endTime, (ulong)((long)startTime));
			this._cachedDailyChallenge = result;
			return true;
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x00044E6C File Offset: 0x0004306C
		private bool TryGetPrecalculatedWeeklyChallenge(out MapChallenge result)
		{
			DateTime time = ChallengeSystem.StartOfWeek(this.DateTimeNow);
			int startTime = ChallengeSystem.ToTimestamp(time);
			int endTime = ChallengeSystem.ToTimestamp(time.AddDays(7.0));
			if (this._cachedWeeklyChallenge != null && this._cachedWeeklyChallenge.TimeStart == startTime)
			{
				result = this._cachedWeeklyChallenge;
				return true;
			}
			MapChallenge challengeOverride;
			if (this._challengeOverrides.TryGetWeeklyChallenge(startTime, endTime, out challengeOverride))
			{
				result = challengeOverride;
				this._cachedWeeklyChallenge = result;
				return true;
			}
			YearOfChallenges yearOfChallenges = this.GetYearOfChallengesForYear(time.Year);
			if (yearOfChallenges == null)
			{
				result = null;
				return false;
			}
			PrecalculatedTimedChallengeData data = yearOfChallenges.GetChallengesOnWeekOfDay(time);
			MapDefinition mapDefinition = this._mapDatabase.MapLibrary.GetMapByName(data.city.ToString());
			result = MapChallenge.CreateWeeklyChallenge(this, mapDefinition, data.challenges, startTime, endTime, (ulong)((long)startTime));
			this._cachedWeeklyChallenge = result;
			return true;
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x00044F4C File Offset: 0x0004314C
		public bool AreChallengesUnlocked(ActivePlayer player)
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				return false;
			}
			if (FeatureToggle.IsFeatureDisabled(Feature.MapUnlocks))
			{
				return true;
			}
			if (this._challengeDatabase.qualifyingAchievementsToUnlockTimedChallenges == null || this._challengeDatabase.qualifyingAchievementsToUnlockTimedChallenges.Length == 0)
			{
				return true;
			}
			foreach (MotorwaysAchievementData achievement in this._challengeDatabase.qualifyingAchievementsToUnlockTimedChallenges)
			{
				if (player.IsAchievementCompleted(achievement.GetId()))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x060014CB RID: 5323 RVA: 0x00044FBC File Offset: 0x000431BC
		public MapChallenge DailyChallenge
		{
			get
			{
				MapChallenge precalculatedDailyChallenge;
				if (this.TryGetPrecalculatedDailyChallenge(out precalculatedDailyChallenge))
				{
					return precalculatedDailyChallenge;
				}
				int currentTimestamp = this.CurrentTimestamp;
				if (this._weeklyChallengeBlock == null || currentTimestamp >= this._weeklyChallengeBlock.weeklyChallenge.TimeEnd)
				{
					this._weeklyChallengeBlock = this.GenerateWeeklyChallengeBlock();
				}
				MapChallenge mapChallenge = null;
				for (int dayIndex = 0; dayIndex < this._weeklyChallengeBlock.dailyChallenges.Length; dayIndex++)
				{
					if (currentTimestamp < this._weeklyChallengeBlock.dailyChallenges[dayIndex].TimeEnd)
					{
						mapChallenge = this._weeklyChallengeBlock.dailyChallenges[dayIndex];
						break;
					}
				}
				if (!Diagnostics.Verify(mapChallenge != null, "Unable to find a valid MapChallenge. This shouldn't be possible."))
				{
					mapChallenge = this._weeklyChallengeBlock.dailyChallenges[this._weeklyChallengeBlock.dailyChallenges.Length - 1];
				}
				return mapChallenge;
			}
		}

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x060014CC RID: 5324 RVA: 0x00045070 File Offset: 0x00043270
		public MapChallenge WeeklyChallenge
		{
			get
			{
				MapChallenge precalculatedWeeklyChallenge;
				if (this.TryGetPrecalculatedWeeklyChallenge(out precalculatedWeeklyChallenge))
				{
					return precalculatedWeeklyChallenge;
				}
				if (this._weeklyChallengeBlock == null || this._weeklyChallengeBlock.weeklyChallenge.HasExpired())
				{
					this._weeklyChallengeBlock = this.GenerateWeeklyChallengeBlock();
				}
				return this._weeklyChallengeBlock.weeklyChallenge;
			}
		}

		// Token: 0x060014CD RID: 5325 RVA: 0x000450BC File Offset: 0x000432BC
		public List<MotorwaysGameJournalSave> GetActiveDailyChallengeSaves(ActivePlayer player, bool localOnly = false)
		{
			List<MotorwaysGameJournalSave> inProgressDailyChallengeSaves = new List<MotorwaysGameJournalSave>();
			if (this.IsSaveAnInProgressDailyChallenge(player.LocalSavedGame))
			{
				inProgressDailyChallengeSaves.Add((MotorwaysGameJournalSave)player.LocalSavedGame);
			}
			if (!localOnly)
			{
				foreach (IGameJournalSave otherSave in player.ForeignSavedGames)
				{
					if (this.IsSaveAnInProgressDailyChallenge(otherSave))
					{
						inProgressDailyChallengeSaves.Add((MotorwaysGameJournalSave)otherSave);
					}
				}
			}
			return inProgressDailyChallengeSaves;
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x00045140 File Offset: 0x00043340
		public List<MotorwaysGameJournalSave> GetActiveWeeklyChallengeSaves(ActivePlayer player, bool localOnly = false)
		{
			List<MotorwaysGameJournalSave> inProgressWeeklyChallengeSaves = new List<MotorwaysGameJournalSave>();
			if (this.IsSaveAnInProgressWeeklyChallenge(player.LocalSavedGame))
			{
				inProgressWeeklyChallengeSaves.Add((MotorwaysGameJournalSave)player.LocalSavedGame);
			}
			if (!localOnly)
			{
				foreach (IGameJournalSave otherSave in player.ForeignSavedGames)
				{
					if (this.IsSaveAnInProgressWeeklyChallenge(otherSave))
					{
						inProgressWeeklyChallengeSaves.Add((MotorwaysGameJournalSave)otherSave);
					}
				}
			}
			return inProgressWeeklyChallengeSaves;
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x000451C4 File Offset: 0x000433C4
		public bool IsSaveAnInProgressDailyChallenge(IGameJournalSave save)
		{
			MotorwaysGameJournalSave motorwaysSave = save as MotorwaysGameJournalSave;
			return motorwaysSave != null && motorwaysSave.ChallengeType == MapChallenge.ChallengeType.Daily && motorwaysSave.ChallengeEndTime == this.DailyChallenge.TimeEnd;
		}

		// Token: 0x060014D0 RID: 5328 RVA: 0x000451FC File Offset: 0x000433FC
		public bool IsSaveAnInProgressWeeklyChallenge(IGameJournalSave save)
		{
			MotorwaysGameJournalSave motorwaysSave = save as MotorwaysGameJournalSave;
			return motorwaysSave != null && motorwaysSave.ChallengeType == MapChallenge.ChallengeType.Weekly && motorwaysSave.ChallengeEndTime == this.WeeklyChallenge.TimeEnd;
		}

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x060014D1 RID: 5329 RVA: 0x00045231 File Offset: 0x00043431
		public int CurrentTimestamp
		{
			get
			{
				return ChallengeSystem.ToTimestamp(this.DateTimeNow);
			}
		}

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x060014D2 RID: 5330 RVA: 0x0004523E File Offset: 0x0004343E
		private DateTime DateTimeNow
		{
			get
			{
				return GameDateTime.UtcNow + this._debugTimeOffset;
			}
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x00045250 File Offset: 0x00043450
		public void DebugChangeTimeOffset(TimeSpan timespan)
		{
			this._debugTimeOffset += timespan;
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x00045264 File Offset: 0x00043464
		public static int ToTimestamp(DateTime dateTime)
		{
			return (int)((dateTime.Ticks - 621355968000000000L) / 10000000L);
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x00045280 File Offset: 0x00043480
		public static DateTime ToDateTime(int unixTime)
		{
			return DateTimeOffset.FromUnixTimeSeconds((long)unixTime).DateTime;
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x0004529C File Offset: 0x0004349C
		public static ChallengeSystem.LeaderboardWeek GetLeaderboardWeek(int unixTime)
		{
			if (ChallengeSystem.WeeksSinceEpoch(ChallengeSystem.ToDateTime(unixTime)) % 2UL != 0UL)
			{
				return ChallengeSystem.LeaderboardWeek.WeekB;
			}
			return ChallengeSystem.LeaderboardWeek.WeekA;
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x000452B4 File Offset: 0x000434B4
		public static DateTime GetStartOfLastOccurence(DayOfWeek day)
		{
			DateTime lastGivenDay = GameDateTime.UtcToday;
			TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);
			for (int dayIndex = 0; dayIndex < 7; dayIndex++)
			{
				if (lastGivenDay.DayOfWeek == day)
				{
					return lastGivenDay;
				}
				lastGivenDay -= oneDay;
			}
			ChallengeSystem.Log.Error(string.Format("Failed to calculate the last occurence of {0} - Defaulting to today", day), Array.Empty<object>());
			return GameDateTime.UtcToday;
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x00045316 File Offset: 0x00043516
		public void OnCreatedInScope(IScope scope)
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				return;
			}
			this._challengeOverrides.Initialize(this, this._challengeDatabase, this._mapDatabase);
			this.RefreshOverridesFromServer(null);
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x00045344 File Offset: 0x00043544
		public void RefreshOverridesFromServer(Action<ChallengeOverrides.RefreshResult, ChallengeSystem.RefreshOverridesDetails> callback = null)
		{
			this._challengeOverrides.RefreshOverridesFromServer(delegate(ChallengeOverrides.RefreshResult result)
			{
				ChallengeSystem.RefreshOverridesDetails overridesDetails = ChallengeSystem.RefreshOverridesDetails.None;
				if (result == ChallengeOverrides.RefreshResult.Success)
				{
					MapChallenge newDailyChallenge;
					if (this._cachedDailyChallenge != null && this._challengeOverrides.TryGetDailyChallenge(this._cachedDailyChallenge.TimeStart, this._cachedDailyChallenge.TimeEnd, out newDailyChallenge) && !newDailyChallenge.Equals(this._cachedDailyChallenge))
					{
						this._cachedDailyChallenge = newDailyChallenge;
						overridesDetails |= ChallengeSystem.RefreshOverridesDetails.NewDailyChallenge;
					}
					MapChallenge newWeeklyChallenge;
					if (this._cachedWeeklyChallenge != null && this._challengeOverrides.TryGetWeeklyChallenge(this._cachedWeeklyChallenge.TimeStart, this._cachedWeeklyChallenge.TimeEnd, out newWeeklyChallenge) && !newWeeklyChallenge.Equals(this._cachedWeeklyChallenge))
					{
						this._cachedWeeklyChallenge = newWeeklyChallenge;
						overridesDetails |= ChallengeSystem.RefreshOverridesDetails.NewWeeklyChallenge;
					}
				}
				Action<ChallengeOverrides.RefreshResult, ChallengeSystem.RefreshOverridesDetails> callback2 = callback;
				if (callback2 == null)
				{
					return;
				}
				callback2(result, overridesDetails);
			});
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x0004537C File Offset: 0x0004357C
		public static MapDefinition.CityNames GetCityName(MapDefinition mapDefinition)
		{
			MapDefinition.CityNames name;
			if (!Enum.TryParse<MapDefinition.CityNames>(mapDefinition.cityName, out name))
			{
				return MapDefinition.CityNames.None;
			}
			return name;
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x0004539C File Offset: 0x0004359C
		public static DateTime StartOfWeek(DateTime dateTime)
		{
			int numDaysFromStartOfWeek = ChallengeSystem.FloorMod(dateTime.DayOfWeek - DayOfWeek.Monday, 7);
			return dateTime.Date - TimeSpan.FromDays((double)numDaysFromStartOfWeek);
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x000453CC File Offset: 0x000435CC
		public static DateTime GetStartOfLastOccurence(ChallengeSystem.LeaderboardWeek leaderboardWeek)
		{
			DateTime startOfWeek = ChallengeSystem.StartOfWeek(GameDateTime.UtcNow);
			ChallengeSystem.LeaderboardWeek currentWeek = ChallengeSystem.GetLeaderboardWeek(ChallengeSystem.ToTimestamp(startOfWeek));
			if (leaderboardWeek != currentWeek)
			{
				return startOfWeek.Subtract(TimeSpan.FromDays(7.0));
			}
			return startOfWeek;
		}

		// Token: 0x060014DD RID: 5341 RVA: 0x0004540B File Offset: 0x0004360B
		public static int FloorMod(int x, int m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x060014DE RID: 5342 RVA: 0x00045414 File Offset: 0x00043614
		private static TimeSpan TimeSinceWeeklyChallengeEpoch(DateTime dateTime)
		{
			return ChallengeSystem.StartOfWeek(dateTime) - ChallengeSystem.WeeklyChallengeEpoch;
		}

		// Token: 0x060014DF RID: 5343 RVA: 0x00045428 File Offset: 0x00043628
		public static ulong WeeksSinceEpoch(DateTime dateTime)
		{
			return (ulong)((long)(ChallengeSystem.TimeSinceWeeklyChallengeEpoch(ChallengeSystem.StartOfWeek(dateTime)).Days / 7));
		}

		// Token: 0x060014E0 RID: 5344 RVA: 0x0004544C File Offset: 0x0004364C
		private static ulong GetWeeklySeed(DateTime dateTime)
		{
			ulong weeksSinceEpoch = ChallengeSystem.WeeksSinceEpoch(dateTime);
			return weeksSinceEpoch * 7UL * 86400UL * 1354843751UL + weeksSinceEpoch;
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x00045474 File Offset: 0x00043674
		private WeeklyChallengeBlock GenerateWeeklyChallengeBlock()
		{
			ulong seed = ChallengeSystem.GetWeeklySeed(this.DateTimeNow);
			PseudorandomGenerator rand = new PseudorandomGenerator
			{
				Seed = seed
			};
			WeeklyChallengeBlock challengeBlock = new WeeklyChallengeBlock();
			challengeBlock.weeklyChallenge = this.GenerateWeeklyMapChallenge(rand.ULong());
			List<MapDefinition> validDailyChallengeMaps = new List<MapDefinition>();
			for (int dayIndex = 0; dayIndex < challengeBlock.dailyChallenges.Length; dayIndex++)
			{
				if (validDailyChallengeMaps.Count == 0)
				{
					validDailyChallengeMaps.AddRange(this._mapDatabase.MapLibrary.Maps);
					validDailyChallengeMaps.Remove(challengeBlock.weeklyChallenge.mapDefinition);
					validDailyChallengeMaps.Shuffle(rand);
				}
				int mapDefinitionIndex = validDailyChallengeMaps.Count - 1;
				MapDefinition mapDefinition = validDailyChallengeMaps[mapDefinitionIndex];
				validDailyChallengeMaps.RemoveAt(mapDefinitionIndex);
				ChallengeData[] challenges = null;
				int iterationAttempts = 0;
				uint dailyChallengeSeed = 0U;
				while (challenges == null && iterationAttempts < 50)
				{
					dailyChallengeSeed = (uint)rand.Seed;
					ChallengeSystem.TryGenerateChallenges(rand, mapDefinition, this._challengeDatabase, new List<ChallengeData>(), 2, out challenges);
					iterationAttempts++;
				}
				int timeStart = challengeBlock.weeklyChallenge.TimeStart + dayIndex * 86400;
				int timeEnd = timeStart + 86400;
				MapChallenge mapChallenge = MapChallenge.CreateDailyChallenge(this, mapDefinition, challenges, timeStart, timeEnd, (ulong)dailyChallengeSeed);
				challengeBlock.dailyChallenges[dayIndex] = mapChallenge;
			}
			return challengeBlock;
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x000455A4 File Offset: 0x000437A4
		public MapChallenge GenerateDailyMapChallenge(ulong seed, List<ChallengeData> mustHaveChallenges = null)
		{
			PseudorandomGenerator rand = new PseudorandomGenerator
			{
				Seed = seed
			};
			DateTime date = this.DateTimeNow.Date;
			DateTime endTime = date + TimeSpan.FromDays(1.0);
			int timeStart = ChallengeSystem.ToTimestamp(date);
			int timeEnd = ChallengeSystem.ToTimestamp(endTime);
			int iterationCount = 0;
			MapChallenge mapChallenge = null;
			if (mustHaveChallenges == null)
			{
				mustHaveChallenges = new List<ChallengeData>();
			}
			List<MapDefinition> mapDefinitions = new List<MapDefinition>(this._mapDatabase.MapLibrary.Maps);
			while (mapChallenge == null && iterationCount < 50)
			{
				MapDefinition mapDefinition = mapDefinitions[rand.Int(mapDefinitions.Count)];
				this.TryGenerateMapChallenge(rand, mapDefinition, MapChallenge.ChallengeType.Daily, mustHaveChallenges, 2 + mustHaveChallenges.Count, timeStart, timeEnd, (uint)seed, out mapChallenge);
				iterationCount++;
			}
			return mapChallenge;
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x0004565C File Offset: 0x0004385C
		public MapChallenge GenerateWeeklyMapChallenge(ulong seed)
		{
			PseudorandomGenerator rand = new PseudorandomGenerator
			{
				Seed = seed
			};
			DateTime dateTime = ChallengeSystem.StartOfWeek(this.DateTimeNow.Date);
			DateTime endTime = dateTime + TimeSpan.FromDays(7.0);
			int timeStart = ChallengeSystem.ToTimestamp(dateTime);
			int timeEnd = ChallengeSystem.ToTimestamp(endTime);
			int iterationCount = 0;
			MapChallenge mapChallenge = null;
			List<ChallengeData> mustHaveChallenges = new List<ChallengeData>();
			List<MapDefinition> mapDefinitions = new List<MapDefinition>(this._mapDatabase.MapLibrary.Maps);
			while (mapChallenge == null && iterationCount < 50)
			{
				MapDefinition mapDefinition = mapDefinitions[rand.Int(mapDefinitions.Count)];
				List<ChallengeData> validWildcardChallenges = ChallengeSystem.GetValidChallengesForCity(mapDefinition, this._challengeDatabase.wildcardChallenges);
				mustHaveChallenges.Clear();
				mustHaveChallenges.Add(validWildcardChallenges[rand.Int(validWildcardChallenges.Count)]);
				this.TryGenerateMapChallenge(rand, mapDefinition, MapChallenge.ChallengeType.Weekly, mustHaveChallenges, 3, timeStart, timeEnd, (uint)seed, out mapChallenge);
				iterationCount++;
			}
			if (!Diagnostics.Verify(mapChallenge != null, "Unable randomly generate a Weekly challenge after {0} iteration attempts", iterationCount))
			{
				return null;
			}
			ChallengeData temp = mapChallenge.challenges[0];
			mapChallenge.challenges[0] = mapChallenge.challenges[1];
			mapChallenge.challenges[1] = temp;
			return mapChallenge;
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x00045788 File Offset: 0x00043988
		private bool TryGenerateMapChallenge(PseudorandomGenerator rand, MapDefinition mapDefinition, MapChallenge.ChallengeType challengeType, List<ChallengeData> mustHaveChallenges, int numberOfChallenges, int timeStart, int timeEnd, uint seed, out MapChallenge mapChallenge)
		{
			ChallengeData[] challenges;
			if (!ChallengeSystem.TryGenerateChallenges(rand, mapDefinition, this._challengeDatabase, mustHaveChallenges, numberOfChallenges, out challenges))
			{
				mapChallenge = null;
				return false;
			}
			if (challengeType == MapChallenge.ChallengeType.Daily)
			{
				mapChallenge = MapChallenge.CreateDailyChallenge(this, mapDefinition, challenges, timeStart, timeEnd, (ulong)seed);
				return true;
			}
			if (challengeType == MapChallenge.ChallengeType.Weekly)
			{
				mapChallenge = MapChallenge.CreateWeeklyChallenge(this, mapDefinition, challenges, timeStart, timeEnd, (ulong)seed);
				return true;
			}
			Diagnostics.FailAssert(string.Format("Invalid ChallengeType for MapChallenge: {0}, expected Daily Challenge or Weekly Challenge", challengeType), Array.Empty<object>());
			mapChallenge = null;
			return false;
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x00045800 File Offset: 0x00043A00
		public static bool TryGenerateChallenges(PseudorandomGenerator rand, MapDefinition cityName, ChallengeDatabase challengeDatabase, List<ChallengeData> mustHaveChallenges, int numberOfChallenges, out ChallengeData[] result)
		{
			List<ChallengeData> mapChallenges = ChallengeSystem.GetValidChallengesForCity(cityName, challengeDatabase.regularChallenges);
			if (!Diagnostics.Verify(numberOfChallenges <= mustHaveChallenges.Count + mapChallenges.Count, "We do not have enough challenges"))
			{
				result = null;
				return false;
			}
			mapChallenges.Shuffle(rand);
			List<ChallengeData> challenges = new List<ChallengeData>(numberOfChallenges);
			challenges.AddRange(mustHaveChallenges);
			int mapChallengeIndex = 0;
			Predicate<ChallengeData> <>9__0;
			while (challenges.Count < numberOfChallenges && mapChallengeIndex < mapChallenges.Count)
			{
				List<ChallengeData> list = challenges;
				Predicate<ChallengeData> match;
				if ((match = <>9__0) == null)
				{
					match = (<>9__0 = ((ChallengeData challenge) => challenge.IsIncompatibleWith(mapChallenges[mapChallengeIndex])));
				}
				if (!list.Exists(match))
				{
					challenges.Add(mapChallenges[mapChallengeIndex]);
				}
				int mapChallengeIndex2 = mapChallengeIndex + 1;
				mapChallengeIndex = mapChallengeIndex2;
			}
			if (!Diagnostics.Verify(challenges.Count == numberOfChallenges, "Not enough valid results to fill result array"))
			{
				result = null;
				return false;
			}
			result = challenges.ToArray();
			return true;
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x00045908 File Offset: 0x00043B08
		public static List<ChallengeData> GetValidChallengesForCity(MapDefinition map, List<ChallengeData> source)
		{
			List<ChallengeData> result = new List<ChallengeData>();
			foreach (ChallengeData challenge in source)
			{
				if (challenge.IsCompatibleWith(map))
				{
					result.Add(challenge);
				}
			}
			return result;
		}

		// Token: 0x060014E7 RID: 5351 RVA: 0x00045968 File Offset: 0x00043B68
		public bool TryGetChallenge(MapChallenge.ChallengeType type, out MapChallenge result)
		{
			switch (type)
			{
			case MapChallenge.ChallengeType.None:
				result = null;
				return false;
			case MapChallenge.ChallengeType.Daily:
				result = this.DailyChallenge;
				return true;
			case MapChallenge.ChallengeType.Weekly:
				result = this.WeeklyChallenge;
				return true;
			default:
				Diagnostics.FailAssert("Unhandled challenge type: {0}. Dev needs to add an entry in the switch case above.", Array.Empty<object>());
				result = null;
				return false;
			}
		}

		// Token: 0x04001147 RID: 4423
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ChallengeSystem");

		// Token: 0x04001148 RID: 4424
		[Dependency]
		private ChallengeDatabase _challengeDatabase;

		// Token: 0x04001149 RID: 4425
		[Dependency]
		private MapDatabase _mapDatabase;

		// Token: 0x0400114A RID: 4426
		[Dependency]
		private ChallengeOverrides _challengeOverrides;

		// Token: 0x0400114B RID: 4427
		private TimeSpan _debugTimeOffset;

		// Token: 0x0400114C RID: 4428
		private WeeklyChallengeBlock _weeklyChallengeBlock;

		// Token: 0x0400114D RID: 4429
		private MapChallenge _cachedDailyChallenge;

		// Token: 0x0400114E RID: 4430
		private MapChallenge _cachedWeeklyChallenge;

		// Token: 0x0400114F RID: 4431
		public const int DaysPerWeek = 7;

		// Token: 0x04001150 RID: 4432
		public const int SecondsPerDay = 86400;

		// Token: 0x04001151 RID: 4433
		public const int NumberOfChallengesPerWeeklyChallenge = 3;

		// Token: 0x04001152 RID: 4434
		public const int NumberOfChallengesPerDailyChallenge = 2;

		// Token: 0x04001153 RID: 4435
		public const DayOfWeek ExpertModeDailyChallengeDay = DayOfWeek.Saturday;

		// Token: 0x04001154 RID: 4436
		public const int MaxGenerationIterationAttempts = 50;

		// Token: 0x04001155 RID: 4437
		private static readonly DateTime WeeklyChallengeEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		// Token: 0x04001156 RID: 4438
		public const ulong WeeklyChallengeSeedSalt = 1354843751UL;

		// Token: 0x0200034E RID: 846
		public enum LeaderboardWeek
		{
			// Token: 0x04001158 RID: 4440
			WeekA,
			// Token: 0x04001159 RID: 4441
			WeekB
		}

		// Token: 0x0200034F RID: 847
		[Flags]
		public enum RefreshOverridesDetails
		{
			// Token: 0x0400115B RID: 4443
			None = 0,
			// Token: 0x0400115C RID: 4444
			NewDailyChallenge = 1,
			// Token: 0x0400115D RID: 4445
			NewWeeklyChallenge = 2
		}
	}
}
