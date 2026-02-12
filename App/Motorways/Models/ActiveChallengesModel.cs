using System;
using System.Collections.Generic;
using System.Linq;
using Factory;
using Factory.Pools;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004C6 RID: 1222
	public class ActiveChallengesModel : IModel, IReusable, IDeserializedHandler
	{
		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x06001FE3 RID: 8163 RVA: 0x0007DBFB File Offset: 0x0007BDFB
		public bool HasChallenges
		{
			get
			{
				return this.challenges.Count > 0;
			}
		}

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x06001FE4 RID: 8164 RVA: 0x0007DC0B File Offset: 0x0007BE0B
		public bool HasEndTime
		{
			get
			{
				return this.timeEnd != 0;
			}
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x06001FE5 RID: 8165 RVA: 0x0007DC16 File Offset: 0x0007BE16
		public int SecondsLeft
		{
			get
			{
				return this.timeEnd - this._challengeSystem.CurrentTimestamp;
			}
		}

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x06001FE6 RID: 8166 RVA: 0x0007DC2A File Offset: 0x0007BE2A
		public bool IsActive
		{
			get
			{
				return this.HasStarted() && this.SecondsLeft > 0;
			}
		}

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x06001FE7 RID: 8167 RVA: 0x0007DC3F File Offset: 0x0007BE3F
		public int TimeEndWithGracePeriod
		{
			get
			{
				return this.timeEnd + 3600;
			}
		}

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x06001FE8 RID: 8168 RVA: 0x0007DC4D File Offset: 0x0007BE4D
		public bool IsActiveWithGracePeriod
		{
			get
			{
				return this.HasStarted() && this.SecondsLeftWithGracePeriod > 0;
			}
		}

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x06001FE9 RID: 8169 RVA: 0x0007DC62 File Offset: 0x0007BE62
		public int SecondsLeftWithGracePeriod
		{
			get
			{
				return this.TimeEndWithGracePeriod - this._challengeSystem.CurrentTimestamp;
			}
		}

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x06001FEA RID: 8170 RVA: 0x0007DC76 File Offset: 0x0007BE76
		public bool IsCityChallenge
		{
			get
			{
				return this.HasChallenges && this.cityChallengeIndex != -1;
			}
		}

		// Token: 0x06001FEB RID: 8171 RVA: 0x0007DC8E File Offset: 0x0007BE8E
		private bool HasStarted()
		{
			return this._challengeSystem.CurrentTimestamp - this.timeStart > 0;
		}

		// Token: 0x06001FEC RID: 8172 RVA: 0x0007DCA8 File Offset: 0x0007BEA8
		public bool HasModifierOfType(ChallengeModifierType type)
		{
			ChallengeModifier challengeModifier;
			return this.TryGetModifierOfType(type, out challengeModifier);
		}

		// Token: 0x06001FED RID: 8173 RVA: 0x0007DCC0 File Offset: 0x0007BEC0
		public bool TryGetModifierOfType(ChallengeModifierType type, out ChallengeModifier firstModifierFound)
		{
			foreach (ChallengeData challenge in this.challenges)
			{
				if (!(challenge == null))
				{
					foreach (ChallengeModifier modifier in challenge.modifiers)
					{
						if (modifier.type == type)
						{
							firstModifierFound = modifier;
							return true;
						}
					}
				}
			}
			firstModifierFound = null;
			return false;
		}

		// Token: 0x06001FEE RID: 8174 RVA: 0x0007DD68 File Offset: 0x0007BF68
		public bool HasModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType modifierType, UpgradeType upgradeType)
		{
			ChallengeModifier challengeModifier;
			return this.TryGetModifierOfTypeWithUpgradeTypeParameter(modifierType, upgradeType, out challengeModifier);
		}

		// Token: 0x06001FEF RID: 8175 RVA: 0x0007DD80 File Offset: 0x0007BF80
		public bool TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType modifierType, UpgradeType upgradeType, out ChallengeModifier firstModifierFound)
		{
			foreach (ChallengeData challenge in this.challenges)
			{
				if (!(challenge == null))
				{
					foreach (ChallengeModifier modifier in challenge.modifiers)
					{
						if (modifier.type == modifierType && modifier.upgradeType == upgradeType)
						{
							firstModifierFound = modifier;
							return true;
						}
					}
				}
			}
			firstModifierFound = null;
			return false;
		}

		// Token: 0x06001FF0 RID: 8176 RVA: 0x0007DE34 File Offset: 0x0007C034
		public bool HasModifierOfTypeWithIntParameter(ChallengeModifierType modifierType, int intParameter)
		{
			ChallengeModifier challengeModifier;
			return this.TryGetModifierOfTypeWithIntParameter(modifierType, intParameter, out challengeModifier);
		}

		// Token: 0x06001FF1 RID: 8177 RVA: 0x0007DE4C File Offset: 0x0007C04C
		public bool TryGetModifierOfTypeWithIntParameter(ChallengeModifierType modifierType, int intData, out ChallengeModifier firstModifierFound)
		{
			foreach (ChallengeData challenge in this.challenges)
			{
				if (!(challenge == null))
				{
					foreach (ChallengeModifier modifier in challenge.modifiers)
					{
						if (modifier.type == modifierType && modifier.intParameter == intData)
						{
							firstModifierFound = modifier;
							return true;
						}
					}
				}
			}
			firstModifierFound = null;
			return false;
		}

		// Token: 0x06001FF2 RID: 8178 RVA: 0x0007DF00 File Offset: 0x0007C100
		public void RemoveChallengesForEndless()
		{
			bool flag = this.HasModifierOfType(ChallengeModifierType.StraightRoadCostMultiplier) || this.HasModifierOfType(ChallengeModifierType.DiagonalRoadCostMultiplier) || this.HasModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.UpgradeRoadCostMultiplier, UpgradeType.Bridge) || this.HasModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.UpgradeRoadCostMultiplier, UpgradeType.Tunnel);
			bool recalculateMotorwayConcreteAfterRemoval = this.HasModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.UpgradeRoadCostMultiplier, UpgradeType.Motorway);
			bool calculateUnlimitedConcreteAmount = this.HasModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.UnlimitedUpgrade, UpgradeType.Concrete);
			UpgradeDatabaseModel upgradeDatabase = this._simulation.GetModel<UpgradeDatabaseModel>();
			this.challenges.Clear();
			int concreteToRefund = 0;
			if (flag)
			{
				int newTotalSpentConcrete = 0;
				foreach (TileModel originTile in this._simulation.GetModels<TileModel>())
				{
					if (originTile.Tile.ContentType == TileContentType.None)
					{
						foreach (TileDirection direction in originTile.Tile.GetTwoLaneRoads(RoadState.Live, Tile.MotorwayInclusion.Ignore))
						{
							Tile destinationTile = originTile.GetAdjacentTileModelInDirection(direction).Tile;
							if (destinationTile.ContentType == TileContentType.None)
							{
								newTotalSpentConcrete += this._behaviour.GetConcreteCostForConnection(originTile.Tile, destinationTile);
							}
						}
					}
				}
				newTotalSpentConcrete /= 2;
				int previousTotalSpentConcrete = upgradeDatabase.GetUsedUpgradeCount(UpgradeType.Concrete);
				concreteToRefund += previousTotalSpentConcrete - newTotalSpentConcrete;
			}
			if (recalculateMotorwayConcreteAfterRemoval)
			{
				foreach (MotorwayModel motorway in this._simulation.GetModels<MotorwayModel>())
				{
					concreteToRefund += motorway.ConcreteCost;
					motorway.ConcreteCost = 0;
				}
			}
			if (concreteToRefund > 0)
			{
				upgradeDatabase.MothballUpgrade(UpgradeType.Concrete, concreteToRefund);
				upgradeDatabase.ReleaseMothballedUpgrade(UpgradeType.Concrete, concreteToRefund);
			}
			else if (concreteToRefund < 0)
			{
				concreteToRefund = -concreteToRefund;
				upgradeDatabase.AddUpgradeToTotal(UpgradeType.Concrete, concreteToRefund);
			}
			if (calculateUnlimitedConcreteAmount)
			{
				int startingConcrete = this._city.Definition.upgradeDefinitions.startingPackages.First((UpgradePackageDefinition definition) => definition.type == UpgradeType.Concrete).amount;
				int weeklyConcrete = this._city.Definition.upgradeDefinitions.weeklyChoicePackages.Max((WeeklyUpgradeDefinition definition) => definition.package.additionalConcrete);
				int num = this._clock.ExpansionWeek * weeklyConcrete + startingConcrete;
				int currentConcrete = upgradeDatabase.GetUsedUpgradeCount(UpgradeType.Concrete);
				int concreteToAward = Mathf.Max(num - currentConcrete, startingConcrete) - upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Concrete);
				upgradeDatabase.ApplyUpgradePackage(new UpgradePackageDefinition
				{
					amount = concreteToAward,
					type = UpgradeType.Concrete,
					additionalConcrete = 0
				}, false);
			}
		}

		// Token: 0x06001FF3 RID: 8179 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Inspect()
		{
		}

		// Token: 0x06001FF4 RID: 8180 RVA: 0x0007E15C File Offset: 0x0007C35C
		public void Reset()
		{
			this.challenges.Clear();
			this.challengeType = MapChallenge.ChallengeType.None;
			this.cityChallengeIndex = -1;
			this.timeEnd = 0;
			this.timeStart = 0;
			this.initialSeed = 0UL;
		}

		// Token: 0x06001FF5 RID: 8181 RVA: 0x0007E190 File Offset: 0x0007C390
		public void OnDeserialized(IScope context)
		{
			bool foundNullChallenge = false;
			int challengeIndex = 0;
			while (challengeIndex < this.challenges.Count)
			{
				if (this.challenges[challengeIndex] == null)
				{
					this.challenges.RemoveAt(challengeIndex);
					foundNullChallenge = true;
				}
				else
				{
					challengeIndex++;
				}
			}
			if (foundNullChallenge)
			{
				ActiveChallengesModel.Log.Error("Found a null challenge in a deserialized game! Check that the ChallengeDatabase contains all of the available challenge modifiers.", Array.Empty<object>());
			}
		}

		// Token: 0x04001A76 RID: 6774
		public const int GracePeriodInSeconds = 3600;

		// Token: 0x04001A77 RID: 6775
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ActiveChallengesModel");

		// Token: 0x04001A78 RID: 6776
		[Dependency]
		private ChallengeSystem _challengeSystem;

		// Token: 0x04001A79 RID: 6777
		[Dependency]
		private City _city;

		// Token: 0x04001A7A RID: 6778
		[Dependency]
		private ClockModel _clock;

		// Token: 0x04001A7B RID: 6779
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x04001A7C RID: 6780
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x04001A7D RID: 6781
		public readonly List<ChallengeData> challenges = new List<ChallengeData>();

		// Token: 0x04001A7E RID: 6782
		public MapChallenge.ChallengeType challengeType;

		// Token: 0x04001A7F RID: 6783
		public int cityChallengeIndex = -1;

		// Token: 0x04001A80 RID: 6784
		public int timeEnd;

		// Token: 0x04001A81 RID: 6785
		public int timeStart;

		// Token: 0x04001A82 RID: 6786
		public ulong initialSeed;
	}
}
