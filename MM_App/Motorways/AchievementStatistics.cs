using System;
using Motorways.Models;

namespace Motorways
{
	// Token: 0x020003DA RID: 986
	public class AchievementStatistics : JsonSerializable
	{
		// Token: 0x1400003B RID: 59
		// (add) Token: 0x060017B0 RID: 6064 RVA: 0x00054AA4 File Offset: 0x00052CA4
		// (remove) Token: 0x060017B1 RID: 6065 RVA: 0x00054ADC File Offset: 0x00052CDC
		public event Action DataChanged;

		// Token: 0x060017B2 RID: 6066 RVA: 0x00054B11 File Offset: 0x00052D11
		public void ConfirmDataChanged()
		{
			Action dataChanged = this.DataChanged;
			if (dataChanged == null)
			{
				return;
			}
			dataChanged();
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x00054B23 File Offset: 0x00052D23
		private static string GetJsonSerializableNameOfProperty(string propertyName)
		{
			return JsonSerializable.GetJsonSerializableName(typeof(AchievementStatistics).GetProperty(propertyName));
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x00054B3C File Offset: 0x00052D3C
		public void OnTreeBulldozed(IAchievementHandler achievementHandler)
		{
			int treesBulldozed = this.TreesBulldozed;
			this.TreesBulldozed = treesBulldozed + 1;
			this.ConfirmDataChanged();
			achievementHandler.IncrementStatistic(AchievementStatistics.GetJsonSerializableNameOfProperty("TreesBulldozed"), 1);
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x00054B74 File Offset: 0x00052D74
		public void OnEndlessMilestoneAchieved(IAchievementHandler achievementHandler)
		{
			int totalEndlessMilestonesAchieved = this.TotalEndlessMilestonesAchieved;
			this.TotalEndlessMilestonesAchieved = totalEndlessMilestonesAchieved + 1;
			this.ConfirmDataChanged();
			achievementHandler.IncrementStatistic(AchievementStatistics.GetJsonSerializableNameOfProperty("TotalEndlessMilestonesAchieved"), 1);
		}

		// Token: 0x060017B6 RID: 6070 RVA: 0x00054BAC File Offset: 0x00052DAC
		public int GetTotalUpgradesUsed(UpgradeType type)
		{
			switch (type)
			{
			case UpgradeType.Concrete:
				return this.TotalConcreteUsed;
			case UpgradeType.Bridge:
				return this.TotalBridgesUsed;
			case UpgradeType.Motorway:
				return this.TotalMotorwaysUsed;
			case UpgradeType.TrafficLight:
				return this.TotalTrafficLightsUsed;
			case UpgradeType.Roundabout:
				return this.TotalRoundaboutsUsed;
			case UpgradeType.Tunnel:
				return this.TotalTunnelsUsed;
			default:
				Diagnostics.FailAssert("Unknown upgrade type {0}.", new object[]
				{
					type
				});
				return 0;
			}
		}

		// Token: 0x060017B7 RID: 6071 RVA: 0x00054C20 File Offset: 0x00052E20
		private void LogUsedUpgrade(UpgradeType type, int amount, IAchievementHandler achievementHandler)
		{
			string statisticId;
			switch (type)
			{
			case UpgradeType.Concrete:
				statisticId = AchievementStatistics.GetJsonSerializableNameOfProperty("TotalConcreteUsed");
				this.TotalConcreteUsed += amount;
				break;
			case UpgradeType.Bridge:
				statisticId = AchievementStatistics.GetJsonSerializableNameOfProperty("TotalBridgesUsed");
				this.TotalBridgesUsed += amount;
				break;
			case UpgradeType.Motorway:
				statisticId = AchievementStatistics.GetJsonSerializableNameOfProperty("TotalMotorwaysUsed");
				this.TotalMotorwaysUsed += amount;
				break;
			case UpgradeType.TrafficLight:
				statisticId = AchievementStatistics.GetJsonSerializableNameOfProperty("TotalTrafficLightsUsed");
				this.TotalTrafficLightsUsed += amount;
				break;
			case UpgradeType.Roundabout:
				statisticId = AchievementStatistics.GetJsonSerializableNameOfProperty("TotalRoundaboutsUsed");
				this.TotalRoundaboutsUsed += amount;
				break;
			case UpgradeType.Tunnel:
				statisticId = AchievementStatistics.GetJsonSerializableNameOfProperty("TotalTunnelsUsed");
				this.TotalTunnelsUsed += amount;
				break;
			default:
				Diagnostics.FailAssert("Unknown upgrade type {0}.", new object[]
				{
					type
				});
				return;
			}
			achievementHandler.IncrementStatistic(statisticId, amount);
		}

		// Token: 0x060017B8 RID: 6072 RVA: 0x00054D1C File Offset: 0x00052F1C
		public void LogDeletedUpgrade(UpgradeType type, int amount, IAchievementHandler achievementHandler)
		{
			if (type == UpgradeType.Concrete)
			{
				string statisticId = AchievementStatistics.GetJsonSerializableNameOfProperty("TotalConcreteDeleted");
				this.TotalConcreteDeleted += amount;
				achievementHandler.IncrementStatistic(statisticId, amount);
				return;
			}
			Diagnostics.FailAssert("Unsupported upgrade type {0}.", new object[]
			{
				type
			});
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x00054D6A File Offset: 0x00052F6A
		public int GetTotalUpgradesDeleted(UpgradeType type)
		{
			if (type == UpgradeType.Concrete)
			{
				return this.TotalConcreteDeleted;
			}
			Diagnostics.FailAssert("Unsupported upgrade type {0}.", new object[]
			{
				type
			});
			return 0;
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x00054D90 File Offset: 0x00052F90
		public void LogScoreStatistics(MotorwaysGameStatistics incrementalStats, IAchievementHandler achievementHandler)
		{
			bool flag = false;
			this.TotalPointsScored += incrementalStats.NewTrips;
			bool flag2 = flag | incrementalStats.NewTrips > 0;
			achievementHandler.IncrementStatistic(AchievementStatistics.GetJsonSerializableNameOfProperty("TotalPointsScored"), incrementalStats.NewTrips);
			if (flag2)
			{
				this.ConfirmDataChanged();
			}
		}

		// Token: 0x060017BB RID: 6075 RVA: 0x00054DD0 File Offset: 0x00052FD0
		public void LogGameOverStatistics(MotorwaysGame game, IAchievementHandler achievementHandler)
		{
			if (!Diagnostics.Verify(game.HasGameEnded, "Can't log game statistics if the game isn't over!"))
			{
				return;
			}
			bool hasMadeChanges = false;
			MapChallenge.ChallengeType challengeType = game.Simulation.GetModel<ActiveChallengesModel>().challengeType;
			if (challengeType == MapChallenge.ChallengeType.Daily)
			{
				int num = this.DailyChallengesPlayed;
				this.DailyChallengesPlayed = num + 1;
				hasMadeChanges = true;
				achievementHandler.IncrementStatistic(AchievementStatistics.GetJsonSerializableNameOfProperty("DailyChallengesPlayed"), 1);
			}
			else if (challengeType == MapChallenge.ChallengeType.Weekly)
			{
				int num = this.WeeklyChallengesPlayed;
				this.WeeklyChallengesPlayed = num + 1;
				hasMadeChanges = true;
				achievementHandler.IncrementStatistic(AchievementStatistics.GetJsonSerializableNameOfProperty("WeeklyChallengesPlayed"), 1);
			}
			if (hasMadeChanges)
			{
				this.ConfirmDataChanged();
			}
		}

		// Token: 0x060017BC RID: 6076 RVA: 0x00054E60 File Offset: 0x00053060
		public void LogUpgradeStatistics(MotorwaysGame game, IAchievementHandler achievementHandler, AchievementStatistics statsAtStart = null)
		{
			bool hasMadeChanges = false;
			UpgradeDatabaseModel upgradeDatabase = game.Simulation.GetModel<UpgradeDatabaseModel>();
			foreach (UpgradeType type in UpgradeDatabase.UpgradeTypes)
			{
				int usedUpgrades = upgradeDatabase.GetTotalUpgradeCount(type);
				usedUpgrades -= upgradeDatabase.GetAvailableUpgradeCount(type);
				if (statsAtStart != null)
				{
					usedUpgrades -= statsAtStart.GetTotalUpgradesUsed(type);
				}
				this.LogUsedUpgrade(type, usedUpgrades, achievementHandler);
				if (usedUpgrades > 0)
				{
					hasMadeChanges = true;
				}
			}
			if (hasMadeChanges)
			{
				this.ConfirmDataChanged();
			}
		}

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x060017BD RID: 6077 RVA: 0x00054ED6 File Offset: 0x000530D6
		// (set) Token: 0x060017BE RID: 6078 RVA: 0x00054EDE File Offset: 0x000530DE
		[JsonSerializable("DCPlayed", JsonSerializableAttribute.MergeStrategy.Max)]
		public int DailyChallengesPlayed { get; private set; }

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x060017BF RID: 6079 RVA: 0x00054EE7 File Offset: 0x000530E7
		// (set) Token: 0x060017C0 RID: 6080 RVA: 0x00054EEF File Offset: 0x000530EF
		[JsonSerializable("WCPlayed", JsonSerializableAttribute.MergeStrategy.Max)]
		public int WeeklyChallengesPlayed { get; private set; }

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x060017C1 RID: 6081 RVA: 0x00054EF8 File Offset: 0x000530F8
		// (set) Token: 0x060017C2 RID: 6082 RVA: 0x00054F00 File Offset: 0x00053100
		[JsonSerializable("TreesBulldozed", JsonSerializableAttribute.MergeStrategy.Max)]
		public int TreesBulldozed { get; private set; }

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x060017C3 RID: 6083 RVA: 0x00054F09 File Offset: 0x00053109
		// (set) Token: 0x060017C4 RID: 6084 RVA: 0x00054F11 File Offset: 0x00053111
		[JsonSerializable("TotalScore", JsonSerializableAttribute.MergeStrategy.Max)]
		public int TotalPointsScored { get; private set; }

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x060017C5 RID: 6085 RVA: 0x00054F1A File Offset: 0x0005311A
		// (set) Token: 0x060017C6 RID: 6086 RVA: 0x00054F22 File Offset: 0x00053122
		[JsonSerializable("TotalConcrete", JsonSerializableAttribute.MergeStrategy.Max)]
		public int TotalConcreteUsed { get; private set; }

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x060017C7 RID: 6087 RVA: 0x00054F2B File Offset: 0x0005312B
		// (set) Token: 0x060017C8 RID: 6088 RVA: 0x00054F33 File Offset: 0x00053133
		[JsonSerializable("TotalBridges", JsonSerializableAttribute.MergeStrategy.Max)]
		public int TotalBridgesUsed { get; private set; }

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x060017C9 RID: 6089 RVA: 0x00054F3C File Offset: 0x0005313C
		// (set) Token: 0x060017CA RID: 6090 RVA: 0x00054F44 File Offset: 0x00053144
		[JsonSerializable("TotalTunnels", JsonSerializableAttribute.MergeStrategy.Max)]
		public int TotalTunnelsUsed { get; private set; }

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x060017CB RID: 6091 RVA: 0x00054F4D File Offset: 0x0005314D
		// (set) Token: 0x060017CC RID: 6092 RVA: 0x00054F55 File Offset: 0x00053155
		[JsonSerializable("TotalMotorways", JsonSerializableAttribute.MergeStrategy.Max)]
		public int TotalMotorwaysUsed { get; private set; }

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x060017CD RID: 6093 RVA: 0x00054F5E File Offset: 0x0005315E
		// (set) Token: 0x060017CE RID: 6094 RVA: 0x00054F66 File Offset: 0x00053166
		[JsonSerializable("TotalTrafficLights", JsonSerializableAttribute.MergeStrategy.Max)]
		public int TotalTrafficLightsUsed { get; private set; }

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x060017CF RID: 6095 RVA: 0x00054F6F File Offset: 0x0005316F
		// (set) Token: 0x060017D0 RID: 6096 RVA: 0x00054F77 File Offset: 0x00053177
		[JsonSerializable("TotalRoundabouts", JsonSerializableAttribute.MergeStrategy.Max)]
		public int TotalRoundaboutsUsed { get; private set; }

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x060017D1 RID: 6097 RVA: 0x00054F80 File Offset: 0x00053180
		// (set) Token: 0x060017D2 RID: 6098 RVA: 0x00054F88 File Offset: 0x00053188
		[JsonSerializable("TotalConcreteDeleted", JsonSerializableAttribute.MergeStrategy.Max)]
		public int TotalConcreteDeleted { get; private set; }

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x060017D3 RID: 6099 RVA: 0x00054F91 File Offset: 0x00053191
		// (set) Token: 0x060017D4 RID: 6100 RVA: 0x00054F99 File Offset: 0x00053199
		[JsonSerializable("TotalEndlessMilestonesAchieved", JsonSerializableAttribute.MergeStrategy.Max)]
		public int TotalEndlessMilestonesAchieved { get; private set; }

		// Token: 0x04001476 RID: 5238
		private const string TotalEndlessMilestonesAchievedKey = "TotalEndlessMilestonesAchieved";
	}
}
