using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using JetBrains.Annotations;
using Motorways.Processes;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004EB RID: 1259
	public class GameBehaviourModel : IModel, IReusable, IReleasedFromScopeHandler, TileModel.IObserver
	{
		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x060020E4 RID: 8420 RVA: 0x000824A6 File Offset: 0x000806A6
		public bool AllowSecondDestinationStartUpgraded
		{
			get
			{
				return this._city.Rules.AllowSecondDestinationStartUpgraded;
			}
		}

		// Token: 0x060020E5 RID: 8421 RVA: 0x000824B8 File Offset: 0x000806B8
		public int GetNumberOfUpgradeOptionsPerWeek()
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.UnlimitedUpgrades))
			{
				return 0;
			}
			int baseAmount = this._city.Rules.GetNumberOfUpgradeOptionsPerWeek();
			if (baseAmount > 1)
			{
				if (this._challenges.HasModifierOfType(ChallengeModifierType.ForceWeeklyUpgrade))
				{
					return 1;
				}
				ChallengeModifier setUpgradeChoiceCountModifier;
				if (this._challenges.TryGetModifierOfType(ChallengeModifierType.SetUpgradeChoiceCount, out setUpgradeChoiceCountModifier))
				{
					if (setUpgradeChoiceCountModifier.intParameter > 0)
					{
						return setUpgradeChoiceCountModifier.intParameter;
					}
					ChallengeModifier overrideFreeConcreteModifier;
					if (this._challenges.TryGetModifierOfType(ChallengeModifierType.OverrideFreeConcreteAmount, out overrideFreeConcreteModifier) && overrideFreeConcreteModifier.intParameter == 0)
					{
						return 0;
					}
					return 1;
				}
			}
			return baseAmount;
		}

		// Token: 0x060020E6 RID: 8422 RVA: 0x00082534 File Offset: 0x00080734
		public List<WeeklyUpgradeDefinition> GetWeeklyUpgradeChoiceOptions()
		{
			List<WeeklyUpgradeDefinition> defaultOptions = new List<WeeklyUpgradeDefinition>();
			defaultOptions.AddRange(this._city.Definition.upgradeDefinitions.weeklyChoicePackages);
			ChallengeModifier setUpgradeChoiceCountModifier;
			if (this._challenges.TryGetModifierOfType(ChallengeModifierType.SetUpgradeChoiceCount, out setUpgradeChoiceCountModifier) && setUpgradeChoiceCountModifier.intParameter == 0)
			{
				defaultOptions.Clear();
			}
			ChallengeModifier concreteCountMultiplierModifier;
			if (this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.AwardedUpgradeAmountMultiplier, UpgradeType.Concrete, out concreteCountMultiplierModifier))
			{
				for (int packageIndex = 0; packageIndex < defaultOptions.Count; packageIndex++)
				{
					if (defaultOptions[packageIndex].package.additionalConcrete > 0)
					{
						defaultOptions[packageIndex] = defaultOptions[packageIndex].NewCopy();
						defaultOptions[packageIndex].package.additionalConcrete = (int)((long)Fix64.Max(concreteCountMultiplierModifier.fix64Parameter * (Fix64)((long)defaultOptions[packageIndex].package.additionalConcrete), Fix64.One));
					}
				}
			}
			ChallengeModifier concreteOverrideAmount;
			if (this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.OverrideFreeConcreteAmount, UpgradeType.Concrete, out concreteOverrideAmount))
			{
				for (int packageIndex2 = 0; packageIndex2 < defaultOptions.Count; packageIndex2++)
				{
					defaultOptions[packageIndex2] = defaultOptions[packageIndex2].NewCopy();
					defaultOptions[packageIndex2].package.additionalConcrete = concreteOverrideAmount.intParameter;
				}
			}
			ChallengeModifier exclusiveModifier;
			if (this._challenges.TryGetModifierOfType(ChallengeModifierType.ForceWeeklyUpgrade, out exclusiveModifier))
			{
				ChallengeModifier modifier;
				bool alterAmount = this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.AwardedUpgradeAmountMultiplier, exclusiveModifier.upgradeType, out modifier);
				for (int packageIndex3 = 0; packageIndex3 < defaultOptions.Count; packageIndex3++)
				{
					if (defaultOptions[packageIndex3].package.type == exclusiveModifier.upgradeType)
					{
						WeeklyUpgradeDefinition exclusivePackage = defaultOptions[packageIndex3].NewCopy();
						defaultOptions.Clear();
						if (alterAmount)
						{
							exclusivePackage.package.amount = (int)((long)Fix64.Max(modifier.fix64Parameter * (Fix64)((long)exclusivePackage.package.amount), Fix64.One));
						}
						exclusivePackage.maxPackageCount = 0;
						exclusivePackage.startingWeek = 0;
						exclusivePackage.lastWeek = 0;
						defaultOptions.Add(exclusivePackage);
						return defaultOptions;
					}
				}
				Diagnostics.FailAssert("We failed to find a weekly upgrade choice for upgrade type '{0}', this challenge may be incompatible with city: '{1}'!", new object[]
				{
					exclusiveModifier.upgradeType,
					this._city.Definition.name
				});
				return defaultOptions;
			}
			for (int packageIndex4 = 0; packageIndex4 < defaultOptions.Count; packageIndex4++)
			{
				ChallengeModifier upgradeCountMultiplierModifier;
				if (this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.AwardedUpgradeAmountMultiplier, defaultOptions[packageIndex4].package.type, out upgradeCountMultiplierModifier))
				{
					defaultOptions[packageIndex4] = defaultOptions[packageIndex4].NewCopy();
					defaultOptions[packageIndex4].package.amount = (int)((long)Fix64.Max(upgradeCountMultiplierModifier.fix64Parameter * (Fix64)((long)defaultOptions[packageIndex4].package.amount), Fix64.One));
				}
				ChallengeModifier challengeModifier;
				if (this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.PreventWeeklyUpgrade, defaultOptions[packageIndex4].package.type, out challengeModifier))
				{
					defaultOptions.RemoveAt(packageIndex4);
					packageIndex4--;
				}
				else if (this.HasUnlimitedOfUpgrade(defaultOptions[packageIndex4].package.type))
				{
					defaultOptions.RemoveAt(packageIndex4);
					packageIndex4--;
				}
			}
			if (defaultOptions.Count == 0)
			{
				bool awardAdditionalConcrete = true;
				ChallengeModifier overrideFreeConcreteModifier;
				if (this._challenges.TryGetModifierOfType(ChallengeModifierType.OverrideFreeConcreteAmount, out overrideFreeConcreteModifier) && overrideFreeConcreteModifier.intParameter == 0)
				{
					awardAdditionalConcrete = false;
				}
				if (awardAdditionalConcrete)
				{
					WeeklyUpgradeDefinition defaultConcretePackage = this.GetDefaultConcretePackage();
					defaultOptions.Add(defaultConcretePackage);
				}
			}
			return defaultOptions;
		}

		// Token: 0x060020E7 RID: 8423 RVA: 0x000828A4 File Offset: 0x00080AA4
		public UpgradeChoice GenerateNextUpgradeChoices()
		{
			if (this._city.Rules.GetNumberOfUpgradeOptionsPerWeek() == 9)
			{
				return this.GenerateAllAvailableUpgradeChoices();
			}
			UpgradeChoice choice = this._scope.Get<UpgradeChoice>();
			choice.isFree = false;
			List<UpgradePackageDefinition> validChoices = new List<UpgradePackageDefinition>();
			List<Fix64> choiceWeights = new List<Fix64>();
			Fix64 totalWeight = Fix64.Zero;
			foreach (WeeklyUpgradeDefinition weeklyUpgrade in this.GetWeeklyUpgradeChoiceOptions())
			{
				int upgradeWeekMetric = this._city.Rules.UpgradeWeekMetric;
				bool isOnOrAfterStartingWeek = upgradeWeekMetric >= weeklyUpgrade.startingWeek;
				bool isLastWeekInvalid = weeklyUpgrade.lastWeek <= 0;
				bool isOnOrBeforeLastWeek = upgradeWeekMetric <= weeklyUpgrade.lastWeek;
				bool isMaxPackageCountInvalid = weeklyUpgrade.maxPackageCount <= 0;
				bool hasNotTakenMaxUpgrades = this._upgrades.NumberOfPackagesTakenOf(weeklyUpgrade.package.type) < weeklyUpgrade.maxPackageCount;
				if (isOnOrAfterStartingWeek && (isLastWeekInvalid || isOnOrBeforeLastWeek) && (isMaxPackageCountInvalid || hasNotTakenMaxUpgrades))
				{
					validChoices.Add(weeklyUpgrade.package);
					Fix64 weight = this.CalculateUpgradePackageWeight(weeklyUpgrade);
					choiceWeights.Add(weight);
					totalWeight += weight;
				}
				else
				{
					GameBehaviourModel.Log.Info(string.Format("Rejecting {0}: isOnOrAfterStartingWeek: {1}, isLastWeekInvalid: {2}, isOnOrBeforeLastWeek: {3}, isMaxPackageCountInvalid: {4}, hasNotTakenMaxUpgrades: {5}", new object[]
					{
						weeklyUpgrade.package.type,
						isOnOrAfterStartingWeek,
						isLastWeekInvalid,
						isOnOrBeforeLastWeek,
						isMaxPackageCountInvalid,
						hasNotTakenMaxUpgrades
					}), Array.Empty<object>());
				}
			}
			int numberOfChoicesToPresent = this.GetNumberOfUpgradeOptionsPerWeek();
			int numChoicesMade = 0;
			while (numChoicesMade < numberOfChoicesToPresent && validChoices.Count > 0)
			{
				Fix64 randomWeight = this._pseudorandomGenerator.Fix64() * totalWeight;
				bool didSelectPackage = false;
				for (int validChoiceIndex = 0; validChoiceIndex < validChoices.Count; validChoiceIndex++)
				{
					randomWeight -= choiceWeights[validChoiceIndex];
					if (randomWeight <= Fix64.Zero)
					{
						choice.choices.Add(validChoices[validChoiceIndex]);
						validChoices.RemoveAt(validChoiceIndex);
						didSelectPackage = true;
						break;
					}
				}
				if (!didSelectPackage && validChoices.Count > 0)
				{
					choice.choices.Add(validChoices[validChoices.Count - 1]);
					validChoices.RemoveAt(validChoices.Count - 1);
				}
				numChoicesMade++;
			}
			foreach (UpgradePackageDefinition upgradePackage in validChoices)
			{
				this._upgrades.OnUpgradeNotPresented(upgradePackage.type);
			}
			foreach (UpgradePackageDefinition upgradePackage2 in choice.choices)
			{
				this._upgrades.OnUpgradePresented(upgradePackage2.type);
			}
			choice.ShuffleChoices(this._pseudorandomGenerator);
			return choice;
		}

		// Token: 0x060020E8 RID: 8424 RVA: 0x00082BD8 File Offset: 0x00080DD8
		private UpgradeChoice GenerateAllAvailableUpgradeChoices()
		{
			if (this._upgrades.TotalGrantedUpgradesCount >= this._constants.MaxUpgradeChoicesAwardedInExpertMode)
			{
				return this.GenerateConcreteOnlyUpgradeChoice();
			}
			UpgradeChoice choice = this._scope.Get<UpgradeChoice>();
			choice.isFree = false;
			foreach (WeeklyUpgradeDefinition weeklyUpgrade in this.GetWeeklyUpgradeChoiceOptions())
			{
				bool packageIsValidChoice = true;
				foreach (UpgradePackageDefinition upgrade in choice.choices)
				{
					if (weeklyUpgrade.package.type == upgrade.type)
					{
						packageIsValidChoice = false;
						break;
					}
				}
				if (packageIsValidChoice)
				{
					choice.choices.Add(weeklyUpgrade.package);
					this._upgrades.OnUpgradePresented(weeklyUpgrade.package.type);
				}
				else
				{
					GameBehaviourModel.Log.Info(string.Format("Rejecting {0}: Upgrade type already included in this upgrade choice package", weeklyUpgrade.package.type), Array.Empty<object>());
				}
			}
			for (int upgradeChoiceIndex = 0; upgradeChoiceIndex < choice.choices.Count; upgradeChoiceIndex++)
			{
				if (choice.choices[upgradeChoiceIndex].type == this._upgrades.LastClaimedPackageType)
				{
					choice.disabledOptions |= (DisabledUpgradeOptions)(1 << upgradeChoiceIndex + 1);
					break;
				}
			}
			return choice;
		}

		// Token: 0x060020E9 RID: 8425 RVA: 0x00082D5C File Offset: 0x00080F5C
		private UpgradeChoice GenerateConcreteOnlyUpgradeChoice()
		{
			WeeklyUpgradeDefinition defaultConcretePackage = this.GetDefaultConcretePackage();
			UpgradeChoice upgradeChoice = this._scope.Get<UpgradeChoice>();
			upgradeChoice.isFree = false;
			upgradeChoice.choices.Add(defaultConcretePackage.package);
			this._upgrades.OnUpgradePresented(defaultConcretePackage.package.type);
			return upgradeChoice;
		}

		// Token: 0x060020EA RID: 8426 RVA: 0x00082DAC File Offset: 0x00080FAC
		private Fix64 CalculateUpgradePackageWeight(WeeklyUpgradeDefinition weeklyUpgrade)
		{
			return (weeklyUpgrade.baseWeight + weeklyUpgrade.weightIncreaseWhenNotOffered * (Fix64)((long)this._upgrades.WeeksSinceUpgradePresented(weeklyUpgrade.package.type)) + this.GetExtraWeightFromExpectedTimeline(weeklyUpgrade)) * this.GetMultiplierRelativeToUpgrades(weeklyUpgrade);
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x00082E04 File Offset: 0x00081004
		private Fix64 GetExtraWeightFromExpectedTimeline(WeeklyUpgradeDefinition weeklyUpgrade)
		{
			if (weeklyUpgrade.expectedUpgradeTimeline == null || weeklyUpgrade.expectedUpgradeTimeline.Count == 0)
			{
				return Fix64.Zero;
			}
			int weekToUse = this._city.Rules.UpgradeWeekMetric;
			for (int expectedUpgradeIndex = weeklyUpgrade.expectedUpgradeTimeline.Count - 1; expectedUpgradeIndex >= 0; expectedUpgradeIndex--)
			{
				ExpectedUpgradeTimeline expectedUpgrade = weeklyUpgrade.expectedUpgradeTimeline[expectedUpgradeIndex];
				if (expectedUpgrade.week <= weekToUse && this._upgrades.GetTotalUpgradeCount(weeklyUpgrade.package.type) < expectedUpgrade.expectedUpgradeCount)
				{
					return expectedUpgrade.bonusWeightIfNotMet;
				}
			}
			return Fix64.Zero;
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x00082E98 File Offset: 0x00081098
		private Fix64 GetMultiplierRelativeToUpgrades(WeeklyUpgradeDefinition weeklyUpgrade)
		{
			if (weeklyUpgrade.relativeUpgradeMultiplierCurve.length == 0)
			{
				return Fix64.One;
			}
			int awardedUpgrades = this._upgrades.GetAvailableUpgradeCount(weeklyUpgrade.relativeUpgradeType);
			return (Fix64)weeklyUpgrade.relativeUpgradeMultiplierCurve.Evaluate((float)awardedUpgrades);
		}

		// Token: 0x060020ED RID: 8429 RVA: 0x00082EDC File Offset: 0x000810DC
		private WeeklyUpgradeDefinition GetDefaultConcretePackage()
		{
			int concreteAmount = 0;
			WeeklyUpgradeDefinition[] weeklyChoicePackages = this._city.Definition.upgradeDefinitions.weeklyChoicePackages;
			for (int i = 0; i < weeklyChoicePackages.Length; i++)
			{
				UpgradePackageDefinition package = weeklyChoicePackages[i].package;
				concreteAmount = Mathf.Max(concreteAmount, package.additionalConcrete);
			}
			ChallengeModifier upgradeCountMultiplierModifier;
			if (this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.AwardedUpgradeAmountMultiplier, UpgradeType.Concrete, out upgradeCountMultiplierModifier))
			{
				concreteAmount = (int)((long)Fix64.Max(upgradeCountMultiplierModifier.fix64Parameter * (Fix64)((long)concreteAmount), Fix64.One));
			}
			WeeklyUpgradeDefinition weeklyUpgradeDefinition = new WeeklyUpgradeDefinition();
			weeklyUpgradeDefinition.package.type = UpgradeType.Concrete;
			weeklyUpgradeDefinition.package.amount = concreteAmount;
			weeklyUpgradeDefinition.relativeUpgradeMultiplierCurve = new AnimationCurve();
			return weeklyUpgradeDefinition;
		}

		// Token: 0x060020EE RID: 8430 RVA: 0x00082F83 File Offset: 0x00081183
		public bool HasGotRules()
		{
			City city = this._city;
			return ((city != null) ? city.Rules : null) != null;
		}

		// Token: 0x060020EF RID: 8431 RVA: 0x00082F9C File Offset: 0x0008119C
		public bool HasUnlimitedOfUpgrade(UpgradeType type)
		{
			if (!FeatureToggle.IsFeatureEnabled(Feature.UnlimitedUpgrades))
			{
				GameRules rules = this._city.Rules;
				if (rules == null || !rules.HasUnlimitedUpgrades)
				{
					ChallengeModifier challengeModifier;
					return this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.UnlimitedUpgrade, type, out challengeModifier);
				}
			}
			return true;
		}

		// Token: 0x060020F0 RID: 8432 RVA: 0x00082FDD File Offset: 0x000811DD
		public bool ShouldShowUpgradeCount()
		{
			return this._city == null || this._city.Rules == null || this._city.Rules.ShowUpgradeCounters;
		}

		// Token: 0x060020F1 RID: 8433 RVA: 0x00083008 File Offset: 0x00081208
		public int GetAmountOfStartingUpgradesForType(UpgradeType type)
		{
			if (type == UpgradeType.House || type == UpgradeType.Destination || type == UpgradeType.DoubleDestination)
			{
				if (!(this._city.Rules is CreativeGameRules))
				{
					return 0;
				}
				return 1;
			}
			else
			{
				int amount = 0;
				foreach (UpgradePackageDefinition upgradePackage in this._city.Definition.upgradeDefinitions.startingPackages)
				{
					if (upgradePackage.type == type)
					{
						amount = upgradePackage.amount;
					}
				}
				if (!this._city.Definition.UsesUpgradeType(type))
				{
					return 0;
				}
				ChallengeModifier modifier;
				if (this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.StartWithUpgrade, type, out modifier))
				{
					amount = modifier.intParameter;
				}
				else if (this.HasUnlimitedOfUpgrade(type))
				{
					amount = 1;
				}
				if (FeatureToggle.IsFeatureEnabled(Feature.StartWithTenMotorways) && type == UpgradeType.Motorway)
				{
					amount += 10;
				}
				return amount;
			}
		}

		// Token: 0x060020F2 RID: 8434 RVA: 0x000830C5 File Offset: 0x000812C5
		public BuildingPlacer.WeightEvaluationLevel GetDefaultBuildingWeightEvaluationLevel(CityTileType buildingType)
		{
			if (buildingType == CityTileType.Demand && this._challenges.HasModifierOfType(ChallengeModifierType.DestinationsIgnoreTileWeights))
			{
				return BuildingPlacer.WeightEvaluationLevel.IgnoreWeights;
			}
			if (buildingType == CityTileType.Supply && this._challenges.HasModifierOfType(ChallengeModifierType.HousesIgnoreTileWeights))
			{
				return BuildingPlacer.WeightEvaluationLevel.IgnoreWeights;
			}
			return BuildingPlacer.WeightEvaluationLevel.ExclusivelyUseWeightedTiles;
		}

		// Token: 0x060020F3 RID: 8435 RVA: 0x000830F0 File Offset: 0x000812F0
		public bool DoesBuildingStartUpgraded(int groupIndex)
		{
			return this._challenges.HasModifierOfType(ChallengeModifierType.AllDestinationsStartUpgraded) || this._challenges.HasModifierOfTypeWithIntParameter(ChallengeModifierType.AllDestinationsOfGroupStartUpgraded, groupIndex);
		}

		// Token: 0x060020F4 RID: 8436 RVA: 0x00083116 File Offset: 0x00081316
		public bool TileSupportsCircleDestinations(int groupIndex, Vector2Int position)
		{
			return !this._challenges.HasModifierOfType(ChallengeModifierType.DestinationsNeverUpgrade) && (this._challenges.HasModifierOfType(ChallengeModifierType.DestinationUpgradesIgnoreWeights) || this._city.Definition.TileSupportsCircleDestinations(groupIndex, position));
		}

		// Token: 0x060020F5 RID: 8437 RVA: 0x0008314C File Offset: 0x0008134C
		public Fix64 GetDemandMultiplierForBuilding(DestinationModel destination)
		{
			Fix64 multiplier = destination.demandMultiplier;
			ChallengeModifier demandChangeModifier;
			if (this._challenges.TryGetModifierOfTypeWithIntParameter(ChallengeModifierType.ChangeDemandOfGroupIndex, destination.GroupIndex, out demandChangeModifier))
			{
				multiplier *= demandChangeModifier.fix64Parameter;
			}
			return multiplier * this._city.Rules.GetDemandMultiplierForDestination(destination);
		}

		// Token: 0x060020F6 RID: 8438 RVA: 0x0008319A File Offset: 0x0008139A
		public bool ForceDoubleDestinations()
		{
			return this._challenges.HasModifierOfType(ChallengeModifierType.ForceDoubleDestinations);
		}

		// Token: 0x060020F7 RID: 8439 RVA: 0x000831AC File Offset: 0x000813AC
		public bool UseDestinationDeadzonesFor(CityTileType buildingType)
		{
			if (this._city.Rules.NoDestinationDeadzoneForHouses)
			{
				return false;
			}
			if (buildingType == CityTileType.Demand)
			{
				return !this._challenges.HasModifierOfType(ChallengeModifierType.NoDestinationDeadzoneForDestinations);
			}
			if (buildingType == CityTileType.Supply)
			{
				return !this._challenges.HasModifierOfType(ChallengeModifierType.NoDestinationDeadzoneForHouses);
			}
			GameBehaviourModel.Log.Error("Unhandled building type {0}. Using deadzones.", new object[]
			{
				buildingType
			});
			return true;
		}

		// Token: 0x060020F8 RID: 8440 RVA: 0x00083215 File Offset: 0x00081415
		public bool BuildingSpawnsAreAffectedByOtherBuildings()
		{
			return !this._challenges.HasModifierOfType(ChallengeModifierType.BuildingsIgnoreOtherBuildings) && !this._city.Rules.BuildingsIgnoreOtherBuildings;
		}

		// Token: 0x060020F9 RID: 8441 RVA: 0x00083240 File Offset: 0x00081440
		public Fix64 GetLaneSpeed(LaneModel lane)
		{
			Fix64 speed;
			if (lane.connection.IsMotorway)
			{
				speed = this._constants.maxSpeedOnMotorways;
				ChallengeModifier modifer;
				if (this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.ChangeUpgradeLaneSpeed, UpgradeType.Motorway, out modifer))
				{
					speed *= modifer.fix64Parameter;
				}
				return speed;
			}
			if (lane.connection.IsRoundabout)
			{
				speed = this._constants.roundaboutSpeedMultiplier;
				ChallengeModifier modifer2;
				if (this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.ChangeUpgradeLaneSpeed, UpgradeType.Roundabout, out modifer2))
				{
					speed *= modifer2.fix64Parameter;
				}
			}
			else
			{
				speed = this._constants.defaultLaneSpeed;
			}
			if ((lane.connection.input.type != RoadType.Roundabout || lane.connection.output.type != RoadType.Roundabout) && lane.connection.input.type != RoadType.Motorway)
			{
				TileDirection inputDirection = lane.connection.input.direction;
				TileDirection direction = lane.connection.output.direction;
				TileDirection desiredOutput = TileUtilities.GetOppositeDirection(inputDirection);
				int difference = TileUtilities.GetDistanceBetweenDirections(direction, desiredOutput);
				if (difference == 2)
				{
					speed *= this._constants.rightAngleTurnSpeedMultiplier;
				}
				else if (difference >= 3)
				{
					speed *= this._constants.sharpTurnSpeedMultiplier;
				}
				ChallengeModifier modifier;
				if (difference >= 2 && difference < 4 && this._challenges.TryGetModifierOfType(ChallengeModifierType.SharpTurnSpeedMultiplier, out modifier))
				{
					speed *= modifier.fix64Parameter;
				}
			}
			if (lane.OutboundLanes.Count > 1 && !lane.connection.IsUTurn && !lane.connection.IsRoundabout)
			{
				int validOutboundLaneCount = 0;
				bool applyIntersectionSpeedMultiplier = true;
				foreach (LaneModel outboundLane in lane.OutboundLanes)
				{
					if ((outboundLane.roadChunk != null && outboundLane.roadChunk.IsControlled) || outboundLane.connection.input.type == RoadType.Roundabout || outboundLane.connection.output.type == RoadType.Roundabout)
					{
						applyIntersectionSpeedMultiplier = false;
						break;
					}
					if (outboundLane.state != RoadState.Mothballed || !outboundLane.connection.IsUTurn)
					{
						validOutboundLaneCount++;
					}
				}
				if (applyIntersectionSpeedMultiplier && validOutboundLaneCount > 1)
				{
					speed *= this._constants.intersectionSpeedMultiplier;
				}
			}
			return speed;
		}

		// Token: 0x060020FA RID: 8442 RVA: 0x00083494 File Offset: 0x00081694
		public bool CanDrawRoadOn(TileContentType contentType)
		{
			if (this._challenges.HasModifierOfType(ChallengeModifierType.IndestructibleTrees))
			{
				return contentType == TileContentType.None;
			}
			return contentType == TileContentType.None || contentType == TileContentType.Tree;
		}

		// Token: 0x060020FB RID: 8443 RVA: 0x000834B4 File Offset: 0x000816B4
		public int GetConcreteCostForMotorway(Vector2Int startCoordinates, Vector2Int endCoordinates)
		{
			ChallengeModifier modifier;
			if (this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.UpgradeRoadCostMultiplier, UpgradeType.Motorway, out modifier))
			{
				return GameRules.GetMotorwayLength(startCoordinates, endCoordinates) * modifier.intParameter;
			}
			return 0;
		}

		// Token: 0x060020FC RID: 8444 RVA: 0x000834E3 File Offset: 0x000816E3
		public int GetConcreteCostForConnection([NotNull] Tile origin, [NotNull] Tile destination)
		{
			return this.GetConcreteCostForConnection(origin.Coordinates, origin.ContentType, destination.Coordinates, destination.ContentType);
		}

		// Token: 0x060020FD RID: 8445 RVA: 0x00083504 File Offset: 0x00081704
		public int GetConcreteCostForConnection(ITilemap tilemap, Vector2Int originCoordinates, Vector2Int destinationCoordinates)
		{
			Tile origin = tilemap.GetTile(originCoordinates);
			Tile destination = tilemap.GetTile(destinationCoordinates);
			return this.GetConcreteCostForConnection(originCoordinates, (origin != null) ? origin.ContentType : TileContentType.None, destinationCoordinates, (destination != null) ? destination.ContentType : TileContentType.None);
		}

		// Token: 0x060020FE RID: 8446 RVA: 0x00083544 File Offset: 0x00081744
		public int GetConcreteCostForConnection(Vector2Int originCoordinates, TileContentType originContent, Vector2Int destinationCoordinates, TileContentType destinationContent)
		{
			if (!Diagnostics.Verify(originContent == TileContentType.None || originContent == TileContentType.Tree || originContent == TileContentType.House))
			{
				return 0;
			}
			if (!Diagnostics.Verify(destinationContent == TileContentType.None || destinationContent == TileContentType.Tree || destinationContent == TileContentType.House))
			{
				return 0;
			}
			if (originContent == TileContentType.House || destinationContent == TileContentType.House)
			{
				return 0;
			}
			TileDirection direction = TileUtilities.GetDirectionBetweenAdjacentCoordinates(originCoordinates, destinationCoordinates);
			if (!Diagnostics.Verify(direction != TileDirection.None, "Can't price a connection between non-adjacent coordinates."))
			{
				return 0;
			}
			int cost = 1;
			ChallengeModifier modifier2;
			if (TileUtilities.IsDirectionDiagonal(direction))
			{
				ChallengeModifier modifier;
				if (this._challenges.TryGetModifierOfType(ChallengeModifierType.DiagonalRoadCostMultiplier, out modifier))
				{
					cost *= modifier.intParameter;
				}
			}
			else if (this._challenges.TryGetModifierOfType(ChallengeModifierType.StraightRoadCostMultiplier, out modifier2))
			{
				cost *= modifier2.intParameter;
			}
			ChallengeModifier bridgeModifier2;
			if (this._city.Definition.TileIsOverWater(originCoordinates) || this._city.Definition.TileIsOverWater(destinationCoordinates))
			{
				ChallengeModifier bridgeModifier;
				if (this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.UpgradeRoadCostMultiplier, UpgradeType.Bridge, out bridgeModifier))
				{
					cost *= bridgeModifier.intParameter;
				}
			}
			else if ((this._city.Definition.TileIsUnderAMountain(originCoordinates) || this._city.Definition.TileIsUnderAMountain(destinationCoordinates)) && this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.UpgradeRoadCostMultiplier, UpgradeType.Tunnel, out bridgeModifier2))
			{
				cost *= bridgeModifier2.intParameter;
			}
			return cost;
		}

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x060020FF RID: 8447 RVA: 0x00083671 File Offset: 0x00081871
		public bool MysteryUpgradesActive
		{
			get
			{
				return this._challenges.HasModifierOfType(ChallengeModifierType.MysteryUpgrades);
			}
		}

		// Token: 0x06002100 RID: 8448 RVA: 0x00083680 File Offset: 0x00081880
		public bool OverridesGameModeWithExpert()
		{
			return this._challenges.HasModifierOfType(ChallengeModifierType.OverrideGameModeWithExpert);
		}

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x06002101 RID: 8449 RVA: 0x0008368F File Offset: 0x0008188F
		public bool UsesBonusTrees
		{
			get
			{
				return this._challenges.HasModifierOfType(ChallengeModifierType.BonusTrees);
			}
		}

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06002102 RID: 8450 RVA: 0x000836A0 File Offset: 0x000818A0
		public bool ShouldHideStaticUpgrades
		{
			get
			{
				City city = this._city;
				bool? flag;
				if (city == null)
				{
					flag = null;
				}
				else
				{
					GameRules rules = city.Rules;
					flag = ((rules != null) ? new bool?(rules.ShouldHideStaticUpgrades) : null);
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}
		}

		// Token: 0x06002103 RID: 8451 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Inspect()
		{
		}

		// Token: 0x06002104 RID: 8452 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnReleasedFromScope(IScope scope)
		{
		}

		// Token: 0x06002105 RID: 8453 RVA: 0x000836E8 File Offset: 0x000818E8
		public void Reset()
		{
			this.CanGameOver = true;
		}

		// Token: 0x06002106 RID: 8454 RVA: 0x000836F4 File Offset: 0x000818F4
		public void OnTileModelChanged(TileModel model)
		{
			if (this._city.Definition.TileIsOverWater(model.Coordinates))
			{
				ChallengeModifier modifier;
				if (this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.ChangeUpgradeLaneSpeed, UpgradeType.Bridge, out modifier))
				{
					model.roadChunk.SetLaneSpeedLimitScale(modifier.fix64Parameter);
					return;
				}
			}
			else if (this._city.Definition.TileIsUnderAMountain(model.Coordinates))
			{
				ChallengeModifier modifier2;
				if (this._challenges.TryGetModifierOfTypeWithUpgradeTypeParameter(ChallengeModifierType.ChangeUpgradeLaneSpeed, UpgradeType.Tunnel, out modifier2))
				{
					model.roadChunk.SetLaneSpeedLimitScale(modifier2.fix64Parameter);
					return;
				}
			}
			else if (this._city.Rules.RoadsBecomePermanentOverTime)
			{
				model.roadChunk.SetSpeedLimitScaleOnDirections(~model.Tile.GetPermanentDirections(), this._constants.DryingRoadSpeedMultiplier, true);
				model.roadChunk.UpdateLaneCosts();
			}
		}

		// Token: 0x04001B4F RID: 6991
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("GameBehaviourModel");

		// Token: 0x04001B50 RID: 6992
		[Dependency]
		private IScope _scope;

		// Token: 0x04001B51 RID: 6993
		[Dependency]
		private UpgradeDatabaseModel _upgrades;

		// Token: 0x04001B52 RID: 6994
		[Dependency]
		private ActiveChallengesModel _challenges;

		// Token: 0x04001B53 RID: 6995
		[Dependency]
		private City _city;

		// Token: 0x04001B54 RID: 6996
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001B55 RID: 6997
		[Dependency]
		private PseudorandomGenerator _pseudorandomGenerator;

		// Token: 0x04001B56 RID: 6998
		public bool CanGameOver = true;
	}
}
