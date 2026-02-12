using System;
using System.Collections.Generic;
using Motorways.Leaderboards;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200034A RID: 842
	public class MapChallenge : IEquatable<MapChallenge>
	{
		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x060014B8 RID: 5304 RVA: 0x00044A2C File Offset: 0x00042C2C
		public int TimeStart
		{
			get
			{
				return this._timeStart;
			}
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x060014B9 RID: 5305 RVA: 0x00044A34 File Offset: 0x00042C34
		public int TimeEnd
		{
			get
			{
				return this._timeEnd;
			}
		}

		// Token: 0x060014BA RID: 5306 RVA: 0x00044A3C File Offset: 0x00042C3C
		public static MapChallenge CreateCityChallenge(ChallengeSystem challengeSystem, int cityChallengeIndex, MapDefinition mapDefinition, ChallengeData[] challenges, ulong seed = 0UL)
		{
			return new MapChallenge(challengeSystem, MapChallenge.ChallengeType.City, cityChallengeIndex, mapDefinition, challenges, 0, 0, seed);
		}

		// Token: 0x060014BB RID: 5307 RVA: 0x00044A4C File Offset: 0x00042C4C
		public static MapChallenge CreateDailyChallenge(ChallengeSystem challengeSystem, MapDefinition mapDefinition, ChallengeData[] challenges, int timeStart, int timeEnd, ulong seed)
		{
			return new MapChallenge(challengeSystem, MapChallenge.ChallengeType.Daily, -1, mapDefinition, challenges, timeStart, timeEnd, seed);
		}

		// Token: 0x060014BC RID: 5308 RVA: 0x00044A5D File Offset: 0x00042C5D
		public static MapChallenge CreateWeeklyChallenge(ChallengeSystem challengeSystem, MapDefinition mapDefinition, ChallengeData[] challenges, int timeStart, int timeEnd, ulong seed)
		{
			return new MapChallenge(challengeSystem, MapChallenge.ChallengeType.Weekly, -1, mapDefinition, challenges, timeStart, timeEnd, seed);
		}

		// Token: 0x060014BD RID: 5309 RVA: 0x00044A70 File Offset: 0x00042C70
		public static MapChallenge CreateMysteryChallenge(ChallengeSystem challengeSystem, ChallengeDatabase challengeDatabase)
		{
			MapChallenge result;
			if (FeatureToggle.IsFeatureEnabled(Feature.RandomChallengesAreExpert))
			{
				List<ChallengeData> requiredChallenges = new List<ChallengeData>();
				requiredChallenges.Add(challengeDatabase.expertModeChallenge);
				result = challengeSystem.GenerateDailyMapChallenge((ulong)UnityEngine.Random.Range(0, 100000), requiredChallenges);
			}
			else if (global::Random.Bool())
			{
				result = challengeSystem.GenerateWeeklyMapChallenge((ulong)UnityEngine.Random.Range(0, 100000));
			}
			else
			{
				result = challengeSystem.GenerateDailyMapChallenge((ulong)UnityEngine.Random.Range(0, 100000), null);
			}
			result.type = MapChallenge.ChallengeType.Mystery;
			result._timeStart = 0;
			result._timeEnd = 0;
			return result;
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x00044AF6 File Offset: 0x00042CF6
		public static MapChallenge RebuildMysteryChallenge(ChallengeSystem challengeSystem, MapDefinition mapDefinition, ChallengeData[] challenges, ulong seed)
		{
			return new MapChallenge(challengeSystem, MapChallenge.ChallengeType.Mystery, -1, mapDefinition, challenges, 0, 0, seed);
		}

		// Token: 0x060014BF RID: 5311 RVA: 0x00044B08 File Offset: 0x00042D08
		private MapChallenge(ChallengeSystem challengeSystem, MapChallenge.ChallengeType type, int cityChallengeIndex, MapDefinition mapDefinition, ChallengeData[] challenges, int timeStart, int timeEnd, ulong seed)
		{
			this._challengeSystem = challengeSystem;
			this.type = type;
			this.cityChallengeIndex = cityChallengeIndex;
			this.mapDefinition = mapDefinition;
			this.challenges = challenges;
			this.seed = seed;
			this._timeStart = timeStart;
			this._timeEnd = timeEnd;
		}

		// Token: 0x060014C0 RID: 5312 RVA: 0x00044B5F File Offset: 0x00042D5F
		public bool HasExpired()
		{
			return this._challengeSystem.CurrentTimestamp - this._timeStart <= 0 || this.SecondsLeft < 0;
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x00044B83 File Offset: 0x00042D83
		public bool HasExpiredWithGracePeriod()
		{
			return this._challengeSystem.CurrentTimestamp - this._timeStart <= 0 || this.SecondsLeft + 3600 < 0;
		}

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x060014C2 RID: 5314 RVA: 0x00044BAD File Offset: 0x00042DAD
		public int SecondsLeft
		{
			get
			{
				return this._timeEnd - this._challengeSystem.CurrentTimestamp;
			}
		}

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x060014C3 RID: 5315 RVA: 0x00044BC4 File Offset: 0x00042DC4
		public DateTimeOffset StartOfChallenge
		{
			get
			{
				DateTimeOffset startOfChallenge;
				if (this.type == MapChallenge.ChallengeType.Daily || this.type == MapChallenge.ChallengeType.Weekly)
				{
					startOfChallenge = DateTimeOffset.FromUnixTimeSeconds((long)this.TimeStart);
				}
				else
				{
					startOfChallenge = DateTimeOffset.UtcNow;
				}
				return startOfChallenge;
			}
		}

		// Token: 0x060014C4 RID: 5316 RVA: 0x00044BF9 File Offset: 0x00042DF9
		public static LeaderboardId GetLeaderboardIdForTimedChallenge(MapChallenge.ChallengeType challengeType, int challengeStartTime)
		{
			if (challengeType == MapChallenge.ChallengeType.Daily)
			{
				return new DailyLeaderboardId(challengeStartTime);
			}
			if (challengeType == MapChallenge.ChallengeType.Weekly)
			{
				return new WeeklyLeaderboardId(challengeStartTime);
			}
			Diagnostics.FailAssert("Invalid challenge type for leaderboard: {0}", new object[]
			{
				challengeType
			});
			return null;
		}

		// Token: 0x060014C5 RID: 5317 RVA: 0x00044C2C File Offset: 0x00042E2C
		public bool Equals(MapChallenge mapChallenge)
		{
			if (mapChallenge == null)
			{
				return false;
			}
			if (this == mapChallenge)
			{
				return true;
			}
			if (base.GetType() != mapChallenge.GetType())
			{
				return false;
			}
			bool challengeDataEquals = this.challenges.Length == mapChallenge.challenges.Length;
			for (int challengeDataIndex = 0; challengeDataIndex < this.challenges.Length; challengeDataIndex++)
			{
				if (this.challenges[challengeDataIndex] != mapChallenge.challenges[challengeDataIndex])
				{
					challengeDataEquals = false;
					break;
				}
			}
			return this.type == mapChallenge.type && this.cityChallengeIndex == mapChallenge.cityChallengeIndex && this.mapDefinition == mapChallenge.mapDefinition && challengeDataEquals && this.seed == mapChallenge.seed && this._timeStart == mapChallenge._timeStart && this._timeEnd == mapChallenge._timeEnd;
		}

		// Token: 0x04001135 RID: 4405
		public const int NoChallengeIndex = -1;

		// Token: 0x04001136 RID: 4406
		public const int AnyChallengeIndex = -2;

		// Token: 0x04001137 RID: 4407
		private readonly ChallengeSystem _challengeSystem;

		// Token: 0x04001138 RID: 4408
		public MapChallenge.ChallengeType type;

		// Token: 0x04001139 RID: 4409
		public int cityChallengeIndex = -1;

		// Token: 0x0400113A RID: 4410
		public readonly MapDefinition mapDefinition;

		// Token: 0x0400113B RID: 4411
		public readonly ChallengeData[] challenges;

		// Token: 0x0400113C RID: 4412
		public ulong seed;

		// Token: 0x0400113D RID: 4413
		private int _timeStart;

		// Token: 0x0400113E RID: 4414
		private int _timeEnd;

		// Token: 0x0200034B RID: 843
		public enum ChallengeType
		{
			// Token: 0x04001140 RID: 4416
			None,
			// Token: 0x04001141 RID: 4417
			Daily,
			// Token: 0x04001142 RID: 4418
			Weekly,
			// Token: 0x04001143 RID: 4419
			Mystery,
			// Token: 0x04001144 RID: 4420
			City
		}
	}
}
