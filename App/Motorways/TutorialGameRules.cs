using System;
using Factory;
using FixMath;
using Motorways.Models;
using Motorways.Processes;
using Motorways.Views;

namespace Motorways
{
	// Token: 0x020003D9 RID: 985
	public class TutorialGameRules : GameRules
	{
		// Token: 0x0600179C RID: 6044 RVA: 0x00054854 File Offset: 0x00052A54
		public override int GetMaximumDemandForDestination(DestinationModel destinationModel)
		{
			int demandLimit = this._tutorialProcess.GetGeneratedDemandLimitForDestination(destinationModel.TutorialIdentifier);
			if (demandLimit != -1)
			{
				return demandLimit;
			}
			if (this._tutorialProcess.LastReachedMarker < TutorialProgressionProcess.TutorialMarker.BigPinsAllowed)
			{
				return 5;
			}
			return base.GetMaximumDemandForDestination(destinationModel);
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x00054891 File Offset: 0x00052A91
		public override StringId GetUpgradeScreenDescriptionUpgrades(int optionCount = 2)
		{
			if (this._clock.Day < 23)
			{
				return StringId.Tutorial_SecondUpgrade;
			}
			return base.GetUpgradeScreenDescriptionUpgrades(optionCount);
		}

		// Token: 0x0600179E RID: 6046 RVA: 0x000548B0 File Offset: 0x00052AB0
		public override StringId GetNoConcreteErrorMessage(DeviceInputType type)
		{
			if (this._tutorialProcess.LastReachedMarker < TutorialProgressionProcess.TutorialMarker.BasicsLearnt)
			{
				switch (type)
				{
				case DeviceInputType.Touch:
					return StringId.Tutorial_Error_EarlyDeleteMode_Touch_MouseToggle;
				case DeviceInputType.Mouse:
					if (this._player.IsDrawModeToggleEnabled)
					{
						return StringId.Tutorial_Error_EarlyDeleteMode_Touch_MouseToggle;
					}
					return StringId.Tutorial_Error_EarlyDeleteMode_Mouse;
				case DeviceInputType.Remote:
					return StringId.Tutorial_Error_EarlyDeleteMode_Remote;
				case DeviceInputType.Controller:
					if (this._player.IsTapDrawEnabled)
					{
						return StringId.Tutorial_Error_EarlyDeleteMode_ControllerTap;
					}
					return StringId.Tutorial_Error_EarlyDeleteMode_Controller;
				}
			}
			else
			{
				switch (type)
				{
				case DeviceInputType.Touch:
					return StringId.Tutorial_Error_DeleteRoads_Touch_MouseToggle;
				case DeviceInputType.Mouse:
					if (this._player.IsDrawModeToggleEnabled)
					{
						return StringId.Tutorial_Error_DeleteRoads_Touch_MouseToggle;
					}
					return StringId.Tutorial_Error_DeleteRoads_Mouse;
				case DeviceInputType.Remote:
					return StringId.Tutorial_Error_DeleteRoads_Remote;
				case DeviceInputType.Controller:
					if (this._player.IsTapDrawEnabled)
					{
						return StringId.Tutorial_Error_DeleteRoads_ControllerTap;
					}
					return StringId.Tutorial_Error_EarlyDeleteMode_Controller;
				}
			}
			return base.GetNoConcreteErrorMessage(type);
		}

		// Token: 0x0600179F RID: 6047 RVA: 0x00054981 File Offset: 0x00052B81
		public override StringId GetGameOverLineOne()
		{
			return StringId.GameOver_TutorialLate_LineOne;
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x00054988 File Offset: 0x00052B88
		public override StringId GetGameOverLineTwo()
		{
			return StringId.GameOver_TutorialLate_LineThree;
		}

		// Token: 0x060017A1 RID: 6049 RVA: 0x0005498F File Offset: 0x00052B8F
		public override Fix64 GetClockSpeedMultiplier()
		{
			if (!this._gameUI.IsClockVisible)
			{
				return this._tutorialProcess.ClockSpeedMultiplier;
			}
			return base.GetClockSpeedMultiplier();
		}

		// Token: 0x060017A2 RID: 6050 RVA: 0x000549B0 File Offset: 0x00052BB0
		public override Fix64 GetOvercrowdingSpeedMultiplier(Fix64 currentTimerProgress)
		{
			Fix64 baseOvercrowdingSpeedMultiplier = base.GetOvercrowdingSpeedMultiplier(currentTimerProgress);
			if (this._tutorialProcess.CurrentStageShortName == "E")
			{
				if (currentTimerProgress < TutorialGameRules.PinTimerSlowdownStart)
				{
					return baseOvercrowdingSpeedMultiplier * (Fix64)0.25;
				}
				return baseOvercrowdingSpeedMultiplier;
			}
			else
			{
				if (currentTimerProgress < TutorialGameRules.PinTimerSlowdownStart)
				{
					return baseOvercrowdingSpeedMultiplier;
				}
				if (currentTimerProgress < TutorialGameRules.PinTimerSlowdownFinish)
				{
					return baseOvercrowdingSpeedMultiplier * (TutorialGameRules.PinTimerSlowdownFinish - currentTimerProgress) / TutorialGameRules.PinTimerSlowdownLength;
				}
				return Fix64.Zero;
			}
		}

		// Token: 0x060017A3 RID: 6051 RVA: 0x0000222C File Offset: 0x0000042C
		public override int GetNumberOfUpgradeOptionsPerWeek()
		{
			return 0;
		}

		// Token: 0x060017A4 RID: 6052 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool ShouldShowNewUpgradeIconDescriptionForType(UpgradeType type)
		{
			return false;
		}

		// Token: 0x060017A5 RID: 6053 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool UIStartVisible()
		{
			return false;
		}

		// Token: 0x060017A6 RID: 6054 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool SupportsLeaderboards()
		{
			return false;
		}

		// Token: 0x060017A7 RID: 6055 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool CanSave()
		{
			return false;
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool ShowDisconnectedBuildingsUI()
		{
			return false;
		}

		// Token: 0x060017A9 RID: 6057 RVA: 0x00054A3D File Offset: 0x00052C3D
		public override bool ShouldUseUpgradeScreenOffsets()
		{
			return this._clock.Week <= 5;
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x00054A50 File Offset: 0x00052C50
		public override bool ShowNoConcreteErrorNotification()
		{
			return this._tutorialProcess.ShowNoConcreteErrorMessage;
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x00054A5D File Offset: 0x00052C5D
		public override bool ShowCannotConnectToCarparkErrorNotification()
		{
			return this._tutorialProcess.LastReachedMarker >= TutorialProgressionProcess.TutorialMarker.DemandCollectedFromNewHouseColor;
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool SupportsChallenges()
		{
			return false;
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool RecordsGameStatistics()
		{
			return false;
		}

		// Token: 0x04001463 RID: 5219
		private static readonly Fix64 PinTimerSlowdownStart = (Fix64)0.75f;

		// Token: 0x04001464 RID: 5220
		private static readonly Fix64 PinTimerSlowdownFinish = (Fix64)0.95f;

		// Token: 0x04001465 RID: 5221
		private static readonly Fix64 PinTimerSlowdownLength = TutorialGameRules.PinTimerSlowdownFinish - TutorialGameRules.PinTimerSlowdownStart;

		// Token: 0x04001466 RID: 5222
		[Dependency]
		private ClockModel _clock;

		// Token: 0x04001467 RID: 5223
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04001468 RID: 5224
		[Dependency]
		private TutorialProgressionProcess _tutorialProcess;

		// Token: 0x04001469 RID: 5225
		[Dependency]
		private GameUIScreen _gameUI;
	}
}
