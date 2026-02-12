using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Motorways.Models;
using Unity.Profiling;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000335 RID: 821
	public class MotorwaysAchievementDefinition : AchievementDefinition
	{
		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x060013D7 RID: 5079 RVA: 0x00041150 File Offset: 0x0003F350
		// (set) Token: 0x060013D8 RID: 5080 RVA: 0x00041158 File Offset: 0x0003F358
		public int IntValue { get; private set; }

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x060013D9 RID: 5081 RVA: 0x00041161 File Offset: 0x0003F361
		// (set) Token: 0x060013DA RID: 5082 RVA: 0x00041169 File Offset: 0x0003F369
		public string CityName { get; private set; }

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x060013DB RID: 5083 RVA: 0x00041172 File Offset: 0x0003F372
		// (set) Token: 0x060013DC RID: 5084 RVA: 0x0004117A File Offset: 0x0003F37A
		public int ChallengeIndex { get; private set; } = -1;

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x060013DD RID: 5085 RVA: 0x00041183 File Offset: 0x0003F383
		// (set) Token: 0x060013DE RID: 5086 RVA: 0x0004118B File Offset: 0x0003F38B
		public AchievementType Type { get; private set; }

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x060013DF RID: 5087 RVA: 0x00041194 File Offset: 0x0003F394
		// (set) Token: 0x060013E0 RID: 5088 RVA: 0x0004119C File Offset: 0x0003F39C
		public AchievementScale Scale { get; private set; }

		// Token: 0x060013E1 RID: 5089 RVA: 0x000411A8 File Offset: 0x0003F3A8
		public bool DoesGameModeMatch(GameMode otherMode)
		{
			switch (otherMode)
			{
			case GameMode.Normal:
				return this.RequiredGameMode.HasFlag(MotorwaysAchievementDefinition.AchievementGameMode.Normal);
			case GameMode.Tutorial:
			case GameMode.Background:
				break;
			case GameMode.Endless:
				return this.RequiredGameMode.HasFlag(MotorwaysAchievementDefinition.AchievementGameMode.Endless);
			case GameMode.Expert:
				return this.RequiredGameMode.HasFlag(MotorwaysAchievementDefinition.AchievementGameMode.Expert);
			default:
				if (otherMode == GameMode.Creative)
				{
					return this.RequiredGameMode.HasFlag(MotorwaysAchievementDefinition.AchievementGameMode.Creative);
				}
				break;
			}
			return false;
		}

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x060013E2 RID: 5090 RVA: 0x00041232 File Offset: 0x0003F432
		// (set) Token: 0x060013E3 RID: 5091 RVA: 0x0004123A File Offset: 0x0003F43A
		public MotorwaysAchievementDefinition.AchievementGameMode RequiredGameMode { get; private set; } = MotorwaysAchievementDefinition.AchievementGameMode.Everything;

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x060013E4 RID: 5092 RVA: 0x00041243 File Offset: 0x0003F443
		// (set) Token: 0x060013E5 RID: 5093 RVA: 0x0004124B File Offset: 0x0003F44B
		public UpgradeType UpgradeType { get; private set; }

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x060013E6 RID: 5094 RVA: 0x00041254 File Offset: 0x0003F454
		// (set) Token: 0x060013E7 RID: 5095 RVA: 0x0004125C File Offset: 0x0003F45C
		public StringId Description { get; protected set; }

		// Token: 0x060013E8 RID: 5096 RVA: 0x00041268 File Offset: 0x0003F468
		public bool IsRetroactivelySatisfied(ActivePlayer player)
		{
			switch (this.Scale)
			{
			case AchievementScale.City:
				return this.Type == AchievementType.Score && this.IsCityScoreAchievementCompleted(player, this.ChallengeIndex);
			case AchievementScale.Game:
				return false;
			case AchievementScale.Lifetime:
				return this.IsLifetimeAchievementSatisfied(player);
			default:
				return false;
			}
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x000412B3 File Offset: 0x0003F4B3
		private IEnumerable<GameMode> GetSupportedGameModes()
		{
			if (this.DoesGameModeMatch(GameMode.Normal))
			{
				yield return GameMode.Normal;
			}
			if (this.DoesGameModeMatch(GameMode.Endless))
			{
				yield return GameMode.Endless;
			}
			if (this.DoesGameModeMatch(GameMode.Expert))
			{
				yield return GameMode.Expert;
			}
			if (this.DoesGameModeMatch(GameMode.Creative))
			{
				yield return GameMode.Creative;
			}
			yield break;
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x000412C4 File Offset: 0x0003F4C4
		private bool IsCityScoreAchievementCompleted(ActivePlayer player, int challengeIndex)
		{
			if (!Diagnostics.Verify(this.Scale == AchievementScale.City && this.Type == AchievementType.Score, "IsCityScoreAchievementCompleted called with achievement of scale {0} and type {1}. Only valid for scale City and type Score", this.Scale, this.Type))
			{
				return false;
			}
			int bestScoreForCity = 0;
			foreach (GameMode gameMode in this.GetSupportedGameModes())
			{
				if (challengeIndex == -1 || challengeIndex == -2)
				{
					if (challengeIndex == -1)
					{
						MotorwaysCityStatistics statisticsForCity = player.GetCityStatisticsForCity(this.CityName, gameMode, false);
						if (statisticsForCity != null)
						{
							bestScoreForCity = Mathf.Max(statisticsForCity.MaxTrips, bestScoreForCity);
						}
					}
					using (IEnumerator<CityChallengeStatistics> enumerator2 = player.GetCityChallengeScores(this.CityName, gameMode).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							CityChallengeStatistics cityChallengeStatistics = enumerator2.Current;
							if (cityChallengeStatistics != null)
							{
								bestScoreForCity = Mathf.Max(cityChallengeStatistics.BestScore, bestScoreForCity);
							}
						}
						continue;
					}
				}
				CityChallengeStatistics cityChallengeStatistics2 = player.GetCityChallengeScore(this.CityName, gameMode, challengeIndex, true);
				if (cityChallengeStatistics2 != null)
				{
					bestScoreForCity = Mathf.Max(cityChallengeStatistics2.BestScore, bestScoreForCity);
				}
			}
			return bestScoreForCity >= this.IntValue;
		}

		// Token: 0x060013EB RID: 5099 RVA: 0x000413FC File Offset: 0x0003F5FC
		public bool IsGameAchievementSatisfied(MotorwaysGame game)
		{
			if (!Diagnostics.Verify(this.Scale == AchievementScale.City || this.Scale == AchievementScale.Game, "Can't check if the achievement is satisfied when of scale {0}", this.Scale))
			{
				return false;
			}
			if (!this.DoesGameModeMatch(game.StartedWithGameMode))
			{
				return false;
			}
			if (Diagnostics.Verify(this.Scale == AchievementScale.Game || (this.Scale == AchievementScale.City && this.CityName == game.MapDefinition.cityName)))
			{
				switch (this.Type)
				{
				case AchievementType.Score:
					return game.Simulation.GetModel<ScoreModel>().Score >= this.IntValue;
				case AchievementType.UpgradesUsed:
					return this.CheckGameUpgradesUsedAchievement(game);
				case AchievementType.UpgradeLength:
					return this.CheckUpgradeLengthAchievement(game);
				case AchievementType.ClearBigPin:
					foreach (DestinationModel destinationModel in game.Simulation.GetModels<DestinationModel>())
					{
						if (destinationModel.CurrentFrame.OvercrowdingTime > Fix64.Zero && destinationModel.NextFrame.OvercrowdingTime <= Fix64.Zero && !destinationModel.IsOvercrowding)
						{
							return true;
						}
					}
					return false;
				case AchievementType.UseAllUpgrades:
					return this.CheckUsedAllUpgradesAchievement(game);
				case AchievementType.EndlessMilestones:
					return game.Simulation.GetModel<ScoreModel>().CurrentEfficiencyMilestone >= this.IntValue;
				}
			}
			Diagnostics.FailAssert("We failed to find a game/city condition that meets achievement: {0} ({1}, {2}, {3}, {4})", new object[]
			{
				base.Id,
				this.Scale,
				this.Type,
				this.IntValue,
				this.UpgradeType
			});
			return false;
		}

		// Token: 0x060013EC RID: 5100 RVA: 0x000415C0 File Offset: 0x0003F7C0
		public bool IsLifetimeAchievementSatisfied(ActivePlayer player)
		{
			if (!Diagnostics.Verify(this.Scale == AchievementScale.Lifetime, "Can't check non-lifetime achievements using this method!"))
			{
				return false;
			}
			switch (this.Type)
			{
			case AchievementType.Score:
				return player.AchievementStatistics.TotalPointsScored >= this.IntValue;
			case AchievementType.Tutorial:
				return player.IsAnyTutorialCompleted;
			case AchievementType.DailyChallenge:
				return player.AchievementStatistics.DailyChallengesPlayed >= this.IntValue;
			case AchievementType.WeeklyChallenge:
				return player.AchievementStatistics.WeeklyChallengesPlayed >= this.IntValue;
			case AchievementType.TreesBulldozed:
				return player.AchievementStatistics.TreesBulldozed >= this.IntValue;
			case AchievementType.UpgradesUsed:
				return player.AchievementStatistics.GetTotalUpgradesUsed(this.UpgradeType) >= this.IntValue;
			case AchievementType.DeletedUpgrades:
				return player.AchievementStatistics.GetTotalUpgradesDeleted(this.UpgradeType) >= this.IntValue;
			case AchievementType.EndlessMilestones:
				return player.AchievementStatistics.TotalEndlessMilestonesAchieved >= this.IntValue;
			}
			Diagnostics.FailAssert("We failed to find a lifetime condition that meets achievement: {0} ({1}, {2}, {3})", new object[]
			{
				base.Id,
				this.Scale,
				this.Type,
				this.IntValue
			});
			return false;
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x0004171C File Offset: 0x0003F91C
		public override bool InitFromAchievementData(AchievementData achievementData, IScope scope)
		{
			if (base.InitFromAchievementData(achievementData, scope))
			{
				MotorwaysAchievementData motorwaysData = achievementData as MotorwaysAchievementData;
				if (motorwaysData != null)
				{
					this.IntValue = motorwaysData.intValue;
					this.CityName = motorwaysData.cityName;
					this.ChallengeIndex = motorwaysData.challengeIndex;
					this.Type = motorwaysData.type;
					this.Scale = motorwaysData.scale;
					this.UpgradeType = motorwaysData.upgradeType;
					this.RequiredGameMode = motorwaysData.gameMode;
					base.Id = motorwaysData.name;
					StringId description;
					if (Enum.TryParse<StringId>(motorwaysData.DescriptionId, out description))
					{
						this.Description = description;
					}
					else
					{
						this.Description = StringId.None;
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x060013EE RID: 5102 RVA: 0x000417C8 File Offset: 0x0003F9C8
		private bool CheckUsedAllUpgradesAchievement(MotorwaysGame game)
		{
			UpgradeDatabaseModel upgradeDatabase = game.Simulation.GetModel<UpgradeDatabaseModel>();
			for (int upgradeIndex = 0; upgradeIndex < 9; upgradeIndex++)
			{
				UpgradeType upgradeType = (UpgradeType)upgradeIndex;
				if (upgradeType != UpgradeType.House && upgradeType != UpgradeType.Destination && upgradeType != UpgradeType.DoubleDestination && upgradeDatabase.GetUsedUpgradeCount(upgradeType) == 0)
				{
					return false;
				}
			}
			MotorwayModel motorway = game.Simulation.GetModel<MotorwayModel>();
			return this.MotorwayIsValid(motorway);
		}

		// Token: 0x060013EF RID: 5103 RVA: 0x0004181A File Offset: 0x0003FA1A
		private bool MotorwayIsValid(MotorwayModel motorway)
		{
			return motorway != null && motorway.EndCoordinates != motorway.StartCoordinates && motorway.State == RoadState.Active;
		}

		// Token: 0x060013F0 RID: 5104 RVA: 0x00041840 File Offset: 0x0003FA40
		private bool CheckGameUpgradesUsedAchievement(MotorwaysGame game)
		{
			if (this.UpgradeType != UpgradeType.Motorway)
			{
				return game.Simulation.GetModel<UpgradeDatabaseModel>().GetUsedUpgradeCount(this.UpgradeType) >= this.IntValue;
			}
			int validMotorwayCount = 0;
			foreach (MotorwayModel motorway in game.Simulation.GetModels<MotorwayModel>())
			{
				if (this.MotorwayIsValid(motorway))
				{
					validMotorwayCount++;
				}
			}
			return validMotorwayCount >= this.IntValue;
		}

		// Token: 0x060013F1 RID: 5105 RVA: 0x000418BC File Offset: 0x0003FABC
		private bool CheckUpgradeLengthAchievement(MotorwaysGame game)
		{
			if (!Diagnostics.Verify(this.UpgradeType == UpgradeType.Bridge || this.UpgradeType == UpgradeType.Tunnel || this.UpgradeType == UpgradeType.Motorway, "Can't use the UpgradeLength achievement stat with upgrade type {0} ({1})", this.UpgradeType, base.Id))
			{
				return false;
			}
			if (this.UpgradeType == UpgradeType.Motorway)
			{
				foreach (MotorwayModel motorway in game.Simulation.GetModels<MotorwayModel>())
				{
					if (Mathf.CeilToInt(Vector2Int.Distance(motorway.StartTile.Coordinates, motorway.EndTile.Coordinates)) >= this.IntValue)
					{
						return true;
					}
				}
				return false;
			}
			foreach (PassageModel passageModel in game.Simulation.GetModels<PassageModel>())
			{
				Passage passage = passageModel.Passage;
				if (passage.UpgradeType == this.UpgradeType && passage.IsComplete && passage.Length >= this.IntValue)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400109B RID: 4251
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("MotorwaysAchievementDefinition");

		// Token: 0x040010A4 RID: 4260
		private static readonly ProfilerMarker Profiler_UpgradeLengthMotorway = new ProfilerMarker(ProfilerCategory.Scripts, "MotorwaysAchievementDefinition.UpgradeLength(Motorway)");

		// Token: 0x040010A5 RID: 4261
		private static readonly ProfilerMarker Profiler_UpgradeLengthPassage = new ProfilerMarker(ProfilerCategory.Scripts, "MotorwaysAchievementDefinition.UpgradeLength(Passage)");

		// Token: 0x02000336 RID: 822
		[Flags]
		public enum AchievementGameMode
		{
			// Token: 0x040010A7 RID: 4263
			Normal = 1,
			// Token: 0x040010A8 RID: 4264
			Endless = 2,
			// Token: 0x040010A9 RID: 4265
			Expert = 4,
			// Token: 0x040010AA RID: 4266
			Creative = 8,
			// Token: 0x040010AB RID: 4267
			Everything = 15
		}
	}
}
