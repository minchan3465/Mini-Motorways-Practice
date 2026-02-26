using System;
using FixMath;
using UnityEngine.Serialization;

namespace Motorways
{
	// Token: 0x02000341 RID: 833
	[Serializable]
	public class ChallengeModifier
	{
		// Token: 0x06001490 RID: 5264 RVA: 0x000432F0 File Offset: 0x000414F0
		public override string ToString()
		{
			switch (this.type)
			{
			case ChallengeModifierType.StartWithUpgrade:
				return string.Format("Start with {0} {1}{2}", this.intParameter, this.upgradeType, this.PluralS);
			case ChallengeModifierType.PreventWeeklyUpgrade:
				return string.Format("Prevent Weekly Upgrade ({0})", this.upgradeType);
			case ChallengeModifierType.ForceWeeklyUpgrade:
				return string.Format("Exclusive Weekly Upgrade {0}", this.upgradeType);
			case ChallengeModifierType.AwardedUpgradeAmountMultiplier:
				return string.Format("{0} {1} awarded", this.fix64Parameter, this.upgradeType);
			case ChallengeModifierType.SetUpgradeChoiceCount:
				return string.Format("Offer {0} weekly upgrade choice{1}", this.intParameter, this.PluralS);
			case ChallengeModifierType.OverrideFreeConcreteAmount:
				return string.Format("Give {0} free concrete", this.intParameter);
			case ChallengeModifierType.DestinationsIgnoreTileWeights:
				return "Destinations Ignore Tile Weights";
			case ChallengeModifierType.ChangeDemandOfGroupIndex:
				return string.Format("Multiply demand group {0} by {1}", this.intParameter, this.fix64Parameter);
			case ChallengeModifierType.DestinationUpgradesIgnoreWeights:
				return "Destination Upgrades Ignore Weights";
			case ChallengeModifierType.DestinationsNeverUpgrade:
				return "Destinations Never Upgrade";
			case ChallengeModifierType.AllDestinationsStartUpgraded:
				return "All Destinations Start Upgraded";
			case ChallengeModifierType.AllDestinationsOfGroupStartUpgraded:
				return string.Format("All Destinations of group {0} start upgraded", this.intParameter);
			case ChallengeModifierType.HousesIgnoreTileWeights:
				return "Houses Ignore Weights";
			case ChallengeModifierType.ForceDoubleDestinations:
				return "All Destinations Are Doubles";
			case ChallengeModifierType.NoDestinationDeadzoneForDestinations:
				return "Destinations Ignore Deadzone for Destinations";
			case ChallengeModifierType.NoDestinationDeadzoneForHouses:
				return "Destinations Ignore Deadzone for Houses";
			case ChallengeModifierType.BuildingsIgnoreOtherBuildings:
				return "Building Spawns Ignore Other Buildings";
			case ChallengeModifierType.UnlimitedUpgrade:
				return string.Format("Unlimited {0}", this.upgradeType);
			case ChallengeModifierType.IndestructibleTrees:
				return "Indestructible Trees";
			case ChallengeModifierType.BonusTrees:
				return "Bonus Trees";
			case ChallengeModifierType.MysteryUpgrades:
				return "Mystery Upgrades";
			case ChallengeModifierType.ChangeUpgradeLaneSpeed:
				return string.Format("Change {0} Lane Speed by {1}", this.upgradeType, this.fix64Parameter);
			case ChallengeModifierType.StraightRoadCostMultiplier:
				return string.Format("Multiply Straight Road Cost By {0}", this.intParameter);
			case ChallengeModifierType.DiagonalRoadCostMultiplier:
				return string.Format("Multiply Diagonal Road Cost By {0}", this.intParameter);
			case ChallengeModifierType.UpgradeRoadCostMultiplier:
				return string.Format("Multiply {0} Road Cost By {1}", this.upgradeType, this.intParameter);
			case ChallengeModifierType.SharpTurnSpeedMultiplier:
				return string.Format("Multiply 90 and 45 turn speed by {0}", this.fix64Parameter);
			case ChallengeModifierType.OverrideGameModeWithExpert:
				return "Go to Expert mode";
			default:
				Diagnostics.FailAssert("Please fill out the ChallengeModifier `ToString` for {0}", new object[]
				{
					this.type
				});
				return base.ToString();
			}
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x00043564 File Offset: 0x00041764
		public string ToFilenameString()
		{
			string result;
			switch (this.type)
			{
			case ChallengeModifierType.StartWithUpgrade:
				result = string.Format("startwith{0}{1}{2}", this.intParameter, "_", this.upgradeType);
				break;
			case ChallengeModifierType.PreventWeeklyUpgrade:
				result = string.Format("remove{0}{1}", "_", this.upgradeType);
				break;
			case ChallengeModifierType.ForceWeeklyUpgrade:
				result = string.Format("exclusive{0}{1}", "_", this.upgradeType);
				break;
			case ChallengeModifierType.AwardedUpgradeAmountMultiplier:
				result = string.Format("{0}{1}{2}{3}awarded", new object[]
				{
					this.fix64Parameter,
					"_",
					this.upgradeType,
					"_"
				});
				break;
			case ChallengeModifierType.SetUpgradeChoiceCount:
				result = string.Format("{0}{1}upgradechoice", this.intParameter, "_");
				break;
			case ChallengeModifierType.OverrideFreeConcreteAmount:
				result = string.Format("freeconcrete{0}{1}", "_", this.intParameter);
				break;
			case ChallengeModifierType.DestinationsIgnoreTileWeights:
				result = "destinationsignoreweights";
				break;
			case ChallengeModifierType.ChangeDemandOfGroupIndex:
				result = string.Format("group{0}{1}{2}demandmultiplier", "_", this.intParameter, "_");
				break;
			case ChallengeModifierType.DestinationUpgradesIgnoreWeights:
				result = "circlesignoreweights";
				break;
			case ChallengeModifierType.DestinationsNeverUpgrade:
				result = "nocircles";
				break;
			case ChallengeModifierType.AllDestinationsStartUpgraded:
				result = "allcircles";
				break;
			case ChallengeModifierType.AllDestinationsOfGroupStartUpgraded:
				result = string.Format("group{0}{1}{2}allcircles", "_", this.intParameter, "_");
				break;
			case ChallengeModifierType.HousesIgnoreTileWeights:
				result = "housesignoreweights";
				break;
			case ChallengeModifierType.ForceDoubleDestinations:
				result = "forcedoubles";
				break;
			case ChallengeModifierType.NoDestinationDeadzoneForDestinations:
				result = "nodestinationdeadzone";
				break;
			case ChallengeModifierType.NoDestinationDeadzoneForHouses:
				result = "nohousedeadzone";
				break;
			case ChallengeModifierType.BuildingsIgnoreOtherBuildings:
				result = "spawnsignorebuildings";
				break;
			case ChallengeModifierType.UnlimitedUpgrade:
				result = string.Format("unlimited{0}{1}", "_", this.upgradeType);
				break;
			case ChallengeModifierType.IndestructibleTrees:
				result = "indestructibletrees";
				break;
			case ChallengeModifierType.BonusTrees:
				result = "bonustrees";
				break;
			case ChallengeModifierType.MysteryUpgrades:
				result = "mysteryupgrades";
				break;
			case ChallengeModifierType.ChangeUpgradeLaneSpeed:
				result = string.Format("{0}lanespeed{1}{2}", this.upgradeType, "_", this.fix64Parameter);
				break;
			case ChallengeModifierType.StraightRoadCostMultiplier:
				result = string.Format("straightroadcost{0}{1}", "_", this.intParameter);
				break;
			case ChallengeModifierType.DiagonalRoadCostMultiplier:
				result = string.Format("diagonalroadcost{0}{1}", "_", this.intParameter);
				break;
			case ChallengeModifierType.UpgradeRoadCostMultiplier:
				result = string.Format("{0}{1}roadcost{2}{3}", new object[]
				{
					this.upgradeType,
					"_",
					"_",
					this.intParameter
				});
				break;
			case ChallengeModifierType.SharpTurnSpeedMultiplier:
				result = string.Format("sharpturnspeed{0}{1}", "_", this.fix64Parameter);
				break;
			case ChallengeModifierType.OverrideGameModeWithExpert:
				result = "expert";
				break;
			default:
				result = string.Format("TYPE{0}{1}{2}INVALID", "_", this.type, "_");
				break;
			}
			return result;
		}

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06001492 RID: 5266 RVA: 0x000438BA File Offset: 0x00041ABA
		private string PluralS
		{
			get
			{
				if (this.intParameter != 1)
				{
					return "s";
				}
				return "";
			}
		}

		// Token: 0x06001493 RID: 5267 RVA: 0x000438D0 File Offset: 0x00041AD0
		public float GetLocalizationParameter()
		{
			switch (this.type)
			{
			case ChallengeModifierType.StartWithUpgrade:
				return (float)this.intParameter;
			case ChallengeModifierType.SetUpgradeChoiceCount:
				return (float)this.intParameter;
			case ChallengeModifierType.OverrideFreeConcreteAmount:
				return (float)this.intParameter;
			case ChallengeModifierType.ChangeDemandOfGroupIndex:
				return (float)this.intParameter;
			case ChallengeModifierType.AllDestinationsOfGroupStartUpgraded:
				return (float)this.intParameter;
			case ChallengeModifierType.StraightRoadCostMultiplier:
				return (float)this.intParameter;
			case ChallengeModifierType.DiagonalRoadCostMultiplier:
				return (float)this.intParameter;
			case ChallengeModifierType.UpgradeRoadCostMultiplier:
				return (float)this.intParameter;
			case ChallengeModifierType.SharpTurnSpeedMultiplier:
				return (float)this.fix64Parameter;
			}
			return -1f;
		}

		// Token: 0x06001494 RID: 5268 RVA: 0x000439AA File Offset: 0x00041BAA
		public bool IsCompatibleWithMap(MapDefinition city)
		{
			return this.type != ChallengeModifierType.ForceWeeklyUpgrade || this.CityHasUpgradeType(city);
		}

		// Token: 0x06001495 RID: 5269 RVA: 0x000439BE File Offset: 0x00041BBE
		private bool CityHasUpgradeType(MapDefinition city)
		{
			return city.HasUpgradeType(this.upgradeType);
		}

		// Token: 0x06001496 RID: 5270 RVA: 0x000439CC File Offset: 0x00041BCC
		public bool IsCompatibleWith(ChallengeModifier otherModifier)
		{
			switch (this.type)
			{
			case ChallengeModifierType.StartWithUpgrade:
				return this.OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(otherModifier, ChallengeModifierType.StartWithUpgrade) && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(otherModifier, ChallengeModifierType.UnlimitedUpgrade);
			case ChallengeModifierType.PreventWeeklyUpgrade:
				return this.OtherModifierAllowsWeeklyUpgrades(otherModifier) && otherModifier.type != ChallengeModifierType.ForceWeeklyUpgrade && otherModifier.type != ChallengeModifierType.PreventWeeklyUpgrade && otherModifier.type != ChallengeModifierType.OverrideGameModeWithExpert && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(otherModifier, ChallengeModifierType.AwardedUpgradeAmountMultiplier) && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(otherModifier, ChallengeModifierType.ChangeUpgradeLaneSpeed) && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(otherModifier, ChallengeModifierType.UpgradeRoadCostMultiplier) && otherModifier.type != ChallengeModifierType.UnlimitedUpgrade;
			case ChallengeModifierType.ForceWeeklyUpgrade:
				return this.OtherModifierAllowsWeeklyUpgradeChoices(otherModifier) && otherModifier.type != ChallengeModifierType.PreventWeeklyUpgrade && otherModifier.type != ChallengeModifierType.OverrideGameModeWithExpert && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsSame(otherModifier, ChallengeModifierType.AwardedUpgradeAmountMultiplier) && otherModifier.type != ChallengeModifierType.MysteryUpgrades && otherModifier.type != ChallengeModifierType.UnlimitedUpgrade && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsSame(otherModifier, ChallengeModifierType.UpgradeRoadCostMultiplier) && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsSame(otherModifier, ChallengeModifierType.ChangeUpgradeLaneSpeed);
			case ChallengeModifierType.AwardedUpgradeAmountMultiplier:
				return this.OtherModifierTypeIsDifferentOrUpgradeTypeIsSame(otherModifier, ChallengeModifierType.ForceWeeklyUpgrade) && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(otherModifier, ChallengeModifierType.PreventWeeklyUpgrade) && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(otherModifier, ChallengeModifierType.AwardedUpgradeAmountMultiplier) && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(otherModifier, ChallengeModifierType.UnlimitedUpgrade) && this.OtherModifierAllowsWeeklyUpgrades(otherModifier);
			case ChallengeModifierType.SetUpgradeChoiceCount:
				return otherModifier.type != ChallengeModifierType.SetUpgradeChoiceCount && otherModifier.type != ChallengeModifierType.ForceWeeklyUpgrade && otherModifier.type != ChallengeModifierType.OverrideGameModeWithExpert && ((this.intParameter == 0 && otherModifier.type != ChallengeModifierType.ChangeUpgradeLaneSpeed && otherModifier.type != ChallengeModifierType.UpgradeRoadCostMultiplier && otherModifier.type != ChallengeModifierType.AwardedUpgradeAmountMultiplier && otherModifier.type != ChallengeModifierType.PreventWeeklyUpgrade && otherModifier.type != ChallengeModifierType.MysteryUpgrades) || (this.intParameter == 1 && otherModifier.type != ChallengeModifierType.MysteryUpgrades));
			case ChallengeModifierType.OverrideFreeConcreteAmount:
				return otherModifier.type != this.type;
			case ChallengeModifierType.DestinationsIgnoreTileWeights:
				return otherModifier.type != this.type;
			case ChallengeModifierType.ChangeDemandOfGroupIndex:
				return otherModifier.type != this.type || otherModifier.intParameter != this.intParameter;
			case ChallengeModifierType.DestinationUpgradesIgnoreWeights:
				return otherModifier.type != this.type && otherModifier.type != ChallengeModifierType.DestinationsNeverUpgrade;
			case ChallengeModifierType.DestinationsNeverUpgrade:
				return otherModifier.type != this.type && otherModifier.type != ChallengeModifierType.AllDestinationsStartUpgraded && otherModifier.type != ChallengeModifierType.DestinationUpgradesIgnoreWeights && otherModifier.type != ChallengeModifierType.AllDestinationsOfGroupStartUpgraded;
			case ChallengeModifierType.AllDestinationsStartUpgraded:
				return otherModifier.type != this.type && otherModifier.type != ChallengeModifierType.DestinationsNeverUpgrade && otherModifier.type != ChallengeModifierType.AllDestinationsOfGroupStartUpgraded;
			case ChallengeModifierType.AllDestinationsOfGroupStartUpgraded:
				return (otherModifier.type != this.type || otherModifier.intParameter != this.intParameter) && otherModifier.type != ChallengeModifierType.DestinationsNeverUpgrade && otherModifier.type != ChallengeModifierType.AllDestinationsStartUpgraded;
			case ChallengeModifierType.HousesIgnoreTileWeights:
			case ChallengeModifierType.ForceDoubleDestinations:
			case ChallengeModifierType.NoDestinationDeadzoneForDestinations:
			case ChallengeModifierType.NoDestinationDeadzoneForHouses:
			case ChallengeModifierType.BuildingsIgnoreOtherBuildings:
			case ChallengeModifierType.IndestructibleTrees:
			case ChallengeModifierType.BonusTrees:
			case ChallengeModifierType.SharpTurnSpeedMultiplier:
				return otherModifier.type != this.type;
			case ChallengeModifierType.UnlimitedUpgrade:
				return this.OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(otherModifier, ChallengeModifierType.StartWithUpgrade) && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(otherModifier, ChallengeModifierType.AwardedUpgradeAmountMultiplier) && otherModifier.type != ChallengeModifierType.ForceWeeklyUpgrade && otherModifier.type != ChallengeModifierType.OverrideGameModeWithExpert && otherModifier.type != ChallengeModifierType.UnlimitedUpgrade && otherModifier.type != ChallengeModifierType.PreventWeeklyUpgrade && (otherModifier.type != ChallengeModifierType.UpgradeRoadCostMultiplier || this.upgradeType != UpgradeType.Concrete) && (otherModifier.type != ChallengeModifierType.StraightRoadCostMultiplier || this.upgradeType != UpgradeType.Concrete) && (otherModifier.type != ChallengeModifierType.DiagonalRoadCostMultiplier || this.upgradeType > UpgradeType.Concrete);
			case ChallengeModifierType.MysteryUpgrades:
				return otherModifier.type != this.type && this.OtherModifierAllowsWeeklyUpgradeChoices(otherModifier);
			case ChallengeModifierType.ChangeUpgradeLaneSpeed:
				return otherModifier.type != this.type && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(otherModifier, ChallengeModifierType.PreventWeeklyUpgrade) && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsSame(otherModifier, ChallengeModifierType.ForceWeeklyUpgrade) && this.OtherModifierAllowsWeeklyUpgrades(otherModifier);
			case ChallengeModifierType.StraightRoadCostMultiplier:
			case ChallengeModifierType.DiagonalRoadCostMultiplier:
				return otherModifier.type != this.type && (otherModifier.type != ChallengeModifierType.UnlimitedUpgrade || otherModifier.upgradeType > UpgradeType.Concrete);
			case ChallengeModifierType.UpgradeRoadCostMultiplier:
				return otherModifier.type != this.type && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(otherModifier, ChallengeModifierType.PreventWeeklyUpgrade) && this.OtherModifierTypeIsDifferentOrUpgradeTypeIsSame(otherModifier, ChallengeModifierType.ForceWeeklyUpgrade) && (otherModifier.type != ChallengeModifierType.UnlimitedUpgrade || otherModifier.upgradeType != UpgradeType.Concrete) && this.OtherModifierAllowsWeeklyUpgrades(otherModifier);
			default:
				return true;
			}
		}

		// Token: 0x06001497 RID: 5271 RVA: 0x00043DD7 File Offset: 0x00041FD7
		private bool OtherModifierTypeIsDifferentOrUpgradeTypeIsDifferent(ChallengeModifier otherModifier, ChallengeModifierType bannedType)
		{
			return otherModifier.type != bannedType || this.upgradeType != otherModifier.upgradeType;
		}

		// Token: 0x06001498 RID: 5272 RVA: 0x00043DF5 File Offset: 0x00041FF5
		private bool OtherModifierTypeIsDifferentOrUpgradeTypeIsSame(ChallengeModifier otherModifier, ChallengeModifierType bannedType)
		{
			return otherModifier.type != bannedType || this.upgradeType == otherModifier.upgradeType;
		}

		// Token: 0x06001499 RID: 5273 RVA: 0x00043E10 File Offset: 0x00042010
		private bool OtherModifierAllowsWeeklyUpgrades(ChallengeModifier otherModifier)
		{
			return otherModifier.type != ChallengeModifierType.SetUpgradeChoiceCount || otherModifier.intParameter != 0;
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x00043E26 File Offset: 0x00042026
		private bool OtherModifierAllowsWeeklyUpgradeChoices(ChallengeModifier otherModifier)
		{
			return (otherModifier.type != ChallengeModifierType.SetUpgradeChoiceCount || otherModifier.intParameter > 1) && otherModifier.type != ChallengeModifierType.ForceWeeklyUpgrade && otherModifier.type != ChallengeModifierType.OverrideGameModeWithExpert;
		}

		// Token: 0x0600149B RID: 5275 RVA: 0x00043E54 File Offset: 0x00042054
		public bool UsesUpgradeType()
		{
			switch (this.type)
			{
			case ChallengeModifierType.StartWithUpgrade:
			case ChallengeModifierType.PreventWeeklyUpgrade:
			case ChallengeModifierType.ForceWeeklyUpgrade:
			case ChallengeModifierType.AwardedUpgradeAmountMultiplier:
			case ChallengeModifierType.UnlimitedUpgrade:
			case ChallengeModifierType.UpgradeRoadCostMultiplier:
				return true;
			case ChallengeModifierType.OverrideFreeConcreteAmount:
				this.upgradeType = UpgradeType.Concrete;
				return true;
			}
			return false;
		}

		// Token: 0x04001111 RID: 4369
		public ChallengeModifierType type;

		// Token: 0x04001112 RID: 4370
		public UpgradeType upgradeType;

		// Token: 0x04001113 RID: 4371
		public int intParameter;

		// Token: 0x04001114 RID: 4372
		[FormerlySerializedAs("fix64Paramter")]
		public Fix64 fix64Parameter;
	}
}
