using System;
using Factory;
using FixMath;
using Motorways.Models;
using Motorways.Processes;
using Motorways.Views;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003D5 RID: 981
	public class GameRules
	{
		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06001758 RID: 5976 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual ScoringMode ScoringMode
		{
			get
			{
				return ScoringMode.Trips;
			}
		}

		// Token: 0x06001759 RID: 5977 RVA: 0x00054028 File Offset: 0x00052228
		public static int GetMotorwayLength(Vector2Int startCoordinates, Vector2Int endCoordinates)
		{
			int a = Mathf.Abs(endCoordinates.x - startCoordinates.x);
			int yDistance = Mathf.Abs(endCoordinates.y - startCoordinates.y);
			return Mathf.Max(a, yDistance);
		}

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x0600175A RID: 5978 RVA: 0x00028DA8 File Offset: 0x00026FA8
		public int MinimumMotorwayLength
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600175B RID: 5979 RVA: 0x00054064 File Offset: 0x00052264
		public virtual bool ShouldShowNewUpgradeIconDescriptionForType(UpgradeType type)
		{
			return !this._player.HasSeenNewContent(GameUpgradeScreen.GetContentIdStringForSelectedUpgradeType(type));
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual StringId GetNoConcreteErrorMessage(DeviceInputType type)
		{
			return StringId.None;
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x0005407C File Offset: 0x0005227C
		public virtual StringId GetUpgradeScreenDescriptionUpgrades(int optionCount = 2)
		{
			if (optionCount == 1)
			{
				return StringId.Tutorial_SecondUpgrade;
			}
			return StringId.WeekTagline_ChooseUpgrade;
		}

		// Token: 0x0600175E RID: 5982 RVA: 0x0005408D File Offset: 0x0005228D
		public virtual StringId GetGameOverLineOne()
		{
			return StringId.GameOver_LineOne;
		}

		// Token: 0x0600175F RID: 5983 RVA: 0x00054094 File Offset: 0x00052294
		public virtual StringId GetGameOverLineTwo()
		{
			return StringId.GameOver_LineTwo;
		}

		// Token: 0x06001760 RID: 5984 RVA: 0x0005409B File Offset: 0x0005229B
		public virtual int GetExpectedUpgradePackageCount(Fix64 upgradeScheduleTime)
		{
			return ClockModel.SecondsToWeeks(upgradeScheduleTime);
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x000540A3 File Offset: 0x000522A3
		public virtual int GetMaximumDemandForDestination(DestinationModel destinationModel)
		{
			return destinationModel.MaximumDemandBeforeTimerStarts + this.GetMaximumOverflowPinsForDestination(destinationModel);
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x000540B3 File Offset: 0x000522B3
		public int GetMaximumOverflowPinsForDestination(DestinationModel model)
		{
			if (!model.IsUpgraded)
			{
				return 4;
			}
			return 6;
		}

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06001763 RID: 5987 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual GenerateDemandProcess.DemandGenerationStyle GetDemandGenerationStyle
		{
			get
			{
				return GenerateDemandProcess.DemandGenerationStyle.Timer;
			}
		}

		// Token: 0x06001764 RID: 5988 RVA: 0x000540C0 File Offset: 0x000522C0
		public virtual Fix64 GetDemandMultiplierForDestination(DestinationModel model)
		{
			if (model.IsBoatTerminal)
			{
				if (!model.IsUpgraded)
				{
					return this._constants.DemandMultiplierForBoatTerminals;
				}
				return this._constants.DemandMultiplierForUpgradedBoatTerminals;
			}
			else
			{
				if (!model.IsUpgraded)
				{
					return this._constants.DemandMultiplierForBuildings;
				}
				return this._constants.DemandMultiplierForUpgradedBuildings;
			}
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x00054114 File Offset: 0x00052314
		public virtual Fix64 GetClockSpeedMultiplier()
		{
			return Fix64.One;
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x00054114 File Offset: 0x00052314
		public virtual Fix64 GetOvercrowdingSpeedMultiplier(Fix64 currentTimerProgress)
		{
			return Fix64.One;
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x00028DA8 File Offset: 0x00026FA8
		public virtual int GetNumberOfUpgradeOptionsPerWeek()
		{
			return 2;
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool CanInteract()
		{
			return true;
		}

		// Token: 0x06001769 RID: 5993 RVA: 0x0005411B File Offset: 0x0005231B
		public virtual float GetCameraPanRange()
		{
			return 8f;
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool ShowsUI()
		{
			return true;
		}

		// Token: 0x0600176B RID: 5995 RVA: 0x00054122 File Offset: 0x00052322
		public virtual bool UseCamera()
		{
			return this.ShowsUI();
		}

		// Token: 0x0600176C RID: 5996 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool DoesIgnorePlayableArea()
		{
			return false;
		}

		// Token: 0x0600176D RID: 5997 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool UIStartVisible()
		{
			return true;
		}

		// Token: 0x0600176E RID: 5998 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool SupportsLeaderboards()
		{
			return true;
		}

		// Token: 0x0600176F RID: 5999 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool RecordsGameStatistics()
		{
			return true;
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool CanSave()
		{
			return true;
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool HasDisabledAutomaticSpawn()
		{
			return false;
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool HasSpawnScheduleVariation()
		{
			return true;
		}

		// Token: 0x06001773 RID: 6003 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool ShowDisconnectedBuildingsUI()
		{
			return true;
		}

		// Token: 0x06001774 RID: 6004 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool ShouldUseUpgradeScreenOffsets()
		{
			return false;
		}

		// Token: 0x06001775 RID: 6005 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool ShowCannotConnectToCarparkErrorNotification()
		{
			return true;
		}

		// Token: 0x06001776 RID: 6006 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool ShowNoConcreteErrorNotification()
		{
			return true;
		}

		// Token: 0x06001777 RID: 6007 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool SupportsChallenges()
		{
			return true;
		}

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06001778 RID: 6008 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool CanBuildingsDemolishUnusedRoads
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06001779 RID: 6009 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool DoRoadsAnimation
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x0600177A RID: 6010 RVA: 0x00054114 File Offset: 0x00052314
		public virtual Fix64 SpawnRampMultiplier
		{
			get
			{
				return Fix64.One;
			}
		}

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x0600177B RID: 6011 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool CanExpansionTimeContinue
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x0600177C RID: 6012 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual int AdditionalHousesPerGroup
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x0600177D RID: 6013 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool UsesPerCityHouseGraph
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x0600177E RID: 6014 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool RoadsBecomePermanentOverTime
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x0600177F RID: 6015 RVA: 0x0005412A File Offset: 0x0005232A
		public virtual int UpgradeWeekMetric
		{
			get
			{
				return this._clock.ExpansionWeek;
			}
		}

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06001780 RID: 6016 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool CanDestinationsOvercrowd
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06001781 RID: 6017 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool CanUpgradeDestinationsAfterFailedSpawns
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06001782 RID: 6018 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool FailedSpawnsIgnoreStoppedExpansionTime
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06001783 RID: 6019 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool ShouldGameStartFullyExpanded
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06001784 RID: 6020 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool HasUnlimitedUpgrades
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06001785 RID: 6021 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool BuildingsIgnoreOtherBuildings
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06001786 RID: 6022 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool NoDestinationDeadzoneForHouses
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06001787 RID: 6023 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool AllowPlacingBuildingsOnUnzoneableTiles
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06001788 RID: 6024 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool AllowSpawningAtMapEdges
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06001789 RID: 6025 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool AllowBlockingSpawns
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x0600178A RID: 6026 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool AllowSpawnsOnRoundaboutDeadzone
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x0600178B RID: 6027 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool AllowConnectingDriveways
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x0600178C RID: 6028 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool ShouldHideStaticUpgrades
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x0600178D RID: 6029 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool ShowColourWidget
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x0600178E RID: 6030 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool AllowSecondDestinationStartUpgraded
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x0600178F RID: 6031 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool ShouldSavePeriodically
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06001790 RID: 6032 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool AllowDemandRelocation
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06001791 RID: 6033 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool ShowUpgradeCounters
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06001792 RID: 6034 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool ShouldBuildingsBulldozeTrees
		{
			get
			{
				return false;
			}
		}

		// Token: 0x040013D9 RID: 5081
		[Dependency]
		protected SimulationConstantsData _constants;

		// Token: 0x040013DA RID: 5082
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x040013DB RID: 5083
		[Dependency]
		private ClockModel _clock;

		// Token: 0x040013DC RID: 5084
		public const int GetAllAvailableUpgradeOptions = 9;
	}
}
