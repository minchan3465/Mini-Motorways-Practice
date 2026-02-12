using System;
using Factory;
using Motorways.Audio;
using Motorways.Views;

namespace Motorways.Actions
{
	// Token: 0x020006F3 RID: 1779
	public class ChangeGameSpeedAction : MotorwaysPlayerAction
	{
		// Token: 0x060030B1 RID: 12465 RVA: 0x000E5180 File Offset: 0x000E3380
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this.SetColourWidgetRadialVisible(false);
			if (Diagnostics.Verify(this._gameUI != null, "GameUI is null on ChangeGameSpeedAction"))
			{
				switch (this._mode)
				{
				case GameUIScreen.TimeScaleMode.Paused:
					this._gameUI.OnPausePressed();
					AudioSystem.Instance.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.Click, UIAudioProfile.Pause, -1f, false, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
					return;
				case GameUIScreen.TimeScaleMode.Play:
					this._gameUI.OnPlayPressed();
					AudioSystem.Instance.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.Click, UIAudioProfile.Play, -1f, false, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
					return;
				case GameUIScreen.TimeScaleMode.FastForward:
					this._gameUI.OnFastForwardPressed();
					AudioSystem.Instance.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.Click, UIAudioProfile.FastForward, -1f, false, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
					return;
				case GameUIScreen.TimeScaleMode.ExtraFastForward:
					this._gameUI.OnExtraFastForwardPressed();
					AudioSystem.Instance.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.Click, UIAudioProfile.FastForward, -1f, false, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x060030B2 RID: 12466 RVA: 0x000020A2 File Offset: 0x000002A2
		public override void Tick(float frameTime)
		{
			this.OnActionComplete();
		}

		// Token: 0x060030B3 RID: 12467 RVA: 0x000E5278 File Offset: 0x000E3478
		public override void Reset()
		{
			base.Reset();
			this._mode = GameUIScreen.TimeScaleMode.Paused;
		}

		// Token: 0x060030B4 RID: 12468 RVA: 0x000E5288 File Offset: 0x000E3488
		public static ChangeGameSpeedAction CreateSpeedUp(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ChangeGameSpeedAction newAction = ChangeGameSpeedAction.Create(owningGroup, scope, timestamp);
			GameUIScreen.TimeScaleMode oldTimeScaleMode = newAction._gameUI.GetTimeScaleMode();
			GameUIScreen.TimeScaleMode newTimeScaleMode;
			switch (oldTimeScaleMode)
			{
			case GameUIScreen.TimeScaleMode.Paused:
				newTimeScaleMode = GameUIScreen.TimeScaleMode.Play;
				break;
			case GameUIScreen.TimeScaleMode.Play:
				newTimeScaleMode = GameUIScreen.TimeScaleMode.FastForward;
				break;
			case GameUIScreen.TimeScaleMode.FastForward:
				if (FeatureToggle.IsFeatureEnabled(Feature.ExtraFastForward))
				{
					newTimeScaleMode = GameUIScreen.TimeScaleMode.ExtraFastForward;
				}
				else
				{
					newTimeScaleMode = oldTimeScaleMode;
				}
				break;
			default:
				newTimeScaleMode = oldTimeScaleMode;
				break;
			}
			newAction._mode = newTimeScaleMode;
			newAction.OnActionBegin(timestamp);
			return newAction;
		}

		// Token: 0x060030B5 RID: 12469 RVA: 0x000E52E8 File Offset: 0x000E34E8
		public static ChangeGameSpeedAction CreateSlowDown(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ChangeGameSpeedAction newAction = ChangeGameSpeedAction.Create(owningGroup, scope, timestamp);
			GameUIScreen.TimeScaleMode oldTimeScaleMode = newAction._gameUI.GetTimeScaleMode();
			GameUIScreen.TimeScaleMode newTimeScaleMode;
			switch (oldTimeScaleMode)
			{
			case GameUIScreen.TimeScaleMode.Play:
				newTimeScaleMode = GameUIScreen.TimeScaleMode.Paused;
				break;
			case GameUIScreen.TimeScaleMode.FastForward:
				newTimeScaleMode = GameUIScreen.TimeScaleMode.Play;
				break;
			case GameUIScreen.TimeScaleMode.ExtraFastForward:
				newTimeScaleMode = GameUIScreen.TimeScaleMode.FastForward;
				break;
			default:
				newTimeScaleMode = oldTimeScaleMode;
				break;
			}
			newAction._mode = newTimeScaleMode;
			newAction.OnActionBegin(timestamp);
			return newAction;
		}

		// Token: 0x060030B6 RID: 12470 RVA: 0x000E5340 File Offset: 0x000E3540
		public static ChangeGameSpeedAction CreateToggleSpeed(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ChangeGameSpeedAction newAction = ChangeGameSpeedAction.Create(owningGroup, scope, timestamp);
			GameUIScreen.TimeScaleMode newTimeScaleMode;
			if (newAction._gameUI.GetTimeScaleMode() == GameUIScreen.TimeScaleMode.Paused)
			{
				newTimeScaleMode = newAction._gameUI.GetUnpausedTimeScaleMode();
			}
			else
			{
				newTimeScaleMode = GameUIScreen.TimeScaleMode.Paused;
			}
			newAction._mode = newTimeScaleMode;
			newAction.OnActionBegin(timestamp);
			return newAction;
		}

		// Token: 0x060030B7 RID: 12471 RVA: 0x000E5382 File Offset: 0x000E3582
		public static ChangeGameSpeedAction CreatePauseSpeed(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ChangeGameSpeedAction changeGameSpeedAction = ChangeGameSpeedAction.Create(owningGroup, scope, timestamp);
			changeGameSpeedAction._mode = GameUIScreen.TimeScaleMode.Paused;
			changeGameSpeedAction.OnActionBegin(timestamp);
			return changeGameSpeedAction;
		}

		// Token: 0x060030B8 RID: 12472 RVA: 0x000E539A File Offset: 0x000E359A
		public static ChangeGameSpeedAction CreatePlaySpeed(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ChangeGameSpeedAction changeGameSpeedAction = ChangeGameSpeedAction.Create(owningGroup, scope, timestamp);
			changeGameSpeedAction._mode = GameUIScreen.TimeScaleMode.Play;
			changeGameSpeedAction.OnActionBegin(timestamp);
			return changeGameSpeedAction;
		}

		// Token: 0x060030B9 RID: 12473 RVA: 0x000E53B2 File Offset: 0x000E35B2
		public static ChangeGameSpeedAction CreateFastForwardSpeed(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ChangeGameSpeedAction changeGameSpeedAction = ChangeGameSpeedAction.Create(owningGroup, scope, timestamp);
			changeGameSpeedAction._mode = GameUIScreen.TimeScaleMode.FastForward;
			changeGameSpeedAction.OnActionBegin(timestamp);
			return changeGameSpeedAction;
		}

		// Token: 0x060030BA RID: 12474 RVA: 0x000E53CA File Offset: 0x000E35CA
		public static ChangeGameSpeedAction CreateExtraFastForwardSpeed(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ChangeGameSpeedAction changeGameSpeedAction = ChangeGameSpeedAction.Create(owningGroup, scope, timestamp);
			changeGameSpeedAction._mode = GameUIScreen.TimeScaleMode.ExtraFastForward;
			changeGameSpeedAction.OnActionBegin(timestamp);
			return changeGameSpeedAction;
		}

		// Token: 0x060030BB RID: 12475 RVA: 0x000E53E2 File Offset: 0x000E35E2
		private static ChangeGameSpeedAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ChangeGameSpeedAction changeGameSpeedAction = scope.Get<ChangeGameSpeedAction>();
			changeGameSpeedAction.InitializeAction(owningGroup, timestamp);
			return changeGameSpeedAction;
		}

		// Token: 0x040029F6 RID: 10742
		private GameUIScreen.TimeScaleMode _mode;
	}
}
