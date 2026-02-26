using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Actions;
using Motorways.Models;
using Motorways.Views;
using Popups;
using Server;
using UnityEngine;

namespace Motorways.Processes
{
	// Token: 0x020004A0 RID: 1184
	public class TutorialProgressionProcess : IProcess, IReusable, InputState.IObserver, IReleasedFromScopeHandler
	{
		// Token: 0x06001D3F RID: 7487 RVA: 0x00073F34 File Offset: 0x00072134
		private void AddBigPinStage()
		{
			this._tutorial.StartStage("Upgrade Big Pin", "UBP");
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("EnsureZoomedOut", null).ClockTicksWhile(() => false).StepOverWhen(() => !this._camera.IsFocussedIn));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("Small delay", null).ClockTicksWhile(() => true).StepOverWhen(() => this.RequireTimePassed(1f)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("FirstOvercrowdingMessage", null).ClockTicksWhile(() => false).WhenStepStarts(delegate()
			{
				this.PrepareForDismissibleMessage();
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_OvercrowdingTwo_02, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
			}).StepOverWhen(new Func<bool>(this.HadInputAndMessageSpentMinimumTime)).WhenStepEnds(new Action(this.RestorePlayerControl)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("Small delay", null).ClockTicksWhile(() => true).StepOverWhen(() => this.RequireTimePassed(1f)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("SecondOvercrowdingMessage", null).ClockTicksWhile(() => false).WhenStepStarts(delegate()
			{
				this.PrepareForDismissibleMessage();
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_OvercrowdingThree_02, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
			}).StepOverWhen(new Func<bool>(this.HadInputAndMessageSpentMinimumTime)).WhenStepEnds(new Action(this.RestorePlayerControl)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("EnsureZoomedOut_2", null).ClockTicksWhile(() => false).StepOverWhen(() => !this._camera.IsFocussedIn));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("AddDemandToBigPinDestination", null).ClockTicksWhile(() => true).WhenStepStarts(delegate()
			{
				this.LimitGeneratedDemandForDestination(TutorialIdentifier.BigPinDestination, 10);
				this.SetTotalDemandOnDestination(TutorialIdentifier.BigPinDestination, 10);
			}).StepOverWhen(() => this.RequireTimePassed(3f)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("EnsureZoomedOut_3", null).ClockTicksWhile(() => false).StepOverWhen(() => !this._camera.IsFocussedIn));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("SpeedUpOvercrowdTimer", "Advance clock so player can respond to surge.").ClockTicksWhile(() => this._clock.Day <= 30).WhenStepStarts(delegate()
			{
				DestinationModel destination = this.GetDestinationById(TutorialIdentifier.BigPinDestination);
				Fix64 overcrowdTimer = Fix64.Max(Fix64.Max(destination.CurrentFrame.OvercrowdingTime, destination.NextFrame.OvercrowdingTime), Fix64Consts.OneHalf);
				destination.CurrentFrame.OvercrowdingTime = overcrowdTimer;
				destination.NextFrame.OvercrowdingTime = overcrowdTimer;
			}).StepOverWhen(() => true));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("OvercrowdingMessage", null).ClockTicksWhile(() => this._clock.Day <= 30).WhenStepStarts(delegate(bool stepAlreadyComplete)
			{
				if (!stepAlreadyComplete)
				{
					DestinationModel destination = this.GetDestinationById(TutorialIdentifier.BigPinDestination);
					Fix64 overcrowdTimer = Fix64.Max(Fix64.Max(destination.CurrentFrame.OvercrowdingTime, destination.NextFrame.OvercrowdingTime), Fix64Consts.OneHalf);
					destination.CurrentFrame.OvercrowdingTime = overcrowdTimer;
					destination.NextFrame.OvercrowdingTime = overcrowdTimer;
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_OvercrowdingFour, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).StepOverWhen(() => this.RequireAllHousesAndDestinationsInGroupToBeConnected(2) || !this.GetDestinationById(TutorialIdentifier.BigPinDestination).IsOvercrowding).WhenStepEnds(delegate
			{
				this.RemoveAllPerDestinationDemandLimits();
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitToClearBigPin", null).ClockTicksWhile(() => this._clock.Day <= 30).StepOverWhen(() => !this.GetDestinationById(TutorialIdentifier.BigPinDestination).IsOvercrowding && this.RequireTimePassed(15f)));
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x000742F8 File Offset: 0x000724F8
		private void DrawRoadHintAnimationHandler(Fix64 timestep, Vector2 from, Vector2 to, float delayBeforeReplay)
		{
			if (this._drawRoadHintAnimationTimer <= Fix64.Zero)
			{
				IndicatorAnimationView animation = this.AddDragIndicator(from, to);
				this._drawRoadHintAnimationTimer = animation.Duration + (Fix64)delayBeforeReplay;
				return;
			}
			this._drawRoadHintAnimationTimer -= timestep;
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x00074358 File Offset: 0x00072558
		private void AddDrawDeleteStage()
		{
			this.SetDrawModeToggleVisibility(false);
			this._tutorial.StartStage("Draw/Delete", "DD");
			this._tutorial.AddRealtimeDelay(0.5f, false);
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WelcomeMessage", null).ClockTicksWhile(() => false).WhenStepStarts(delegate()
			{
				this.PrepareForDismissibleMessage();
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_Welcome, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
			}).StepOverWhen(new Func<bool>(this.HadInputAndMessageSpentMinimumTime)));
			this._tutorial.AddRealtimeDelay(2.5f, false);
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WelcomeMessage2", null).ClockTicksWhile(() => false).WhenStepStarts(delegate()
			{
				this.PrepareForDismissibleMessage();
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_Welcome_02, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
			}).StepOverWhen(new Func<bool>(this.HadInputAndMessageSpentMinimumTime)).WhenStepEnds(delegate
			{
				this._concreteCountAtStartOfTutorial = this._upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Concrete);
				this.RestorePlayerControl();
			}));
			this._tutorial.AddRealtimeDelay(2f, false);
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("Setup", null).ClockTicksWhile(() => false).WhenStepStarts(delegate()
			{
				this.SetDrawModeToggleVisibility(false);
				this._gameUI.SetUpgradeBarVisibility(true, false);
				this.ShowNoConcreteErrorMessage = false;
			}).StepOverWhen(() => true));
			switch (this._inputState.CurrentDeviceInputType)
			{
			case DeviceInputType.Touch:
				this.AddTouchSteps_DrawDelete();
				break;
			case DeviceInputType.Mouse:
				this.AddMouseSteps_DrawDelete();
				break;
			case DeviceInputType.Remote:
				this.AddRemoteSteps_DrawDelete();
				break;
			case DeviceInputType.Controller:
				this.AddControllerSteps_DrawDelete();
				break;
			}
			this._tutorial.AddMarker(TutorialProgressionProcess.TutorialMarker.InputControlsTaught);
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x0007453C File Offset: 0x0007273C
		private void AddControllerSteps_DrawDelete()
		{
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToDraw_OneRoad", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RoadCountGreaterThanOrEqualTo((int)((float)this._concreteCountAtStartOfTutorial * 0.6f))).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					StringId drawString = this._player.IsTapDrawEnabled ? StringId.Tutorial_PromptToStartDrawRoad_ControllerTap : StringId.Tutorial_PromptToStartDrawRoad_Controller;
					this.SetNextMessageAnchoredToScreen(drawString, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).AddIdleHint(new IdleHint().SetDelayBeforeShowing(0.5f).SetShowHintHandler(delegate(Fix64 timestep)
			{
				this.DrawRoadHintAnimationHandler(timestep, this._tutorialConstants.DrawRoadIdleHintStartPosition, this._tutorialConstants.DrawRoadIdleHintEndPosition, 0f);
			}).AddCondition(() => this.RoadCountIs(0))).WhenStepEnds(delegate
			{
				this._drawRoadHintAnimationTimer = Fix64.Zero;
				this._roadCountAfterDrawStep = this.GetRoadCount();
			}).SetDebugText(() => string.Format("Road Count: {0}/{1}", this.GetRoadCount(), this._concreteCountAtStartOfTutorial)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitBeforeDeletePrompt", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireTimePassed(0.5f)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToDelete_Controller", "Tell the player how to delete roads.").ClockTicksWhile(() => false).StepOverWhen(() => this._roadCountAfterDrawStep - this.GetRoadCount() >= 1 && this._enteredDeleteMode && this._exitedDeleteMode).SetDebugText(delegate
			{
				int deletedRoadCount = this._roadCountAfterDrawStep - this.GetRoadCount();
				return string.Format("Deleted Roads {0}, current roads {1}", deletedRoadCount, this.GetRoadCount());
			}).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					StringId drawString = this._player.IsTapDrawEnabled ? StringId.Tutorial_PromptToDeleteRoad_ControllerTap : StringId.Tutorial_PromptToDeleteRoad_Controller;
					this.SetNextMessageAnchoredToScreen(drawString, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
				this._roadCountAfterDrawStep = this.GetRoadCount();
			}).WhenStepEnds(delegate
			{
				this._simulation.IsPaused = false;
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToDeleteAllRoads_Controller", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RoadCountIs(0)).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_DeleteAllRoads, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitBeforeTapDrawFTUX", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireTimePassed(0.5f)));
			this.ShowControllerTapDrawFtux();
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x00074778 File Offset: 0x00072978
		private void AddRemoteSteps_DrawDelete()
		{
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToDraw_OneRoad", null).ClockTicksWhile(() => false).StepOverWhen(() => this._controllerIsDrawingRoads).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToWorld(StringId.Tutorial_PromptToStartDrawRoad_Remote, this._tutorialConstants.DrawRoadIdleHintStartPosition, TileDirection.North, true);
					this.AddHighlightPositionIndicator(this._tutorialConstants.DrawRoadIdleHintStartPosition);
				}
			}).WhenStepEnds(delegate
			{
				this._drawRoadHintAnimationTimer = Fix64.Zero;
				this._roadCountAfterDrawStep = this.GetRoadCount();
			}).SetDebugText(() => string.Format("Road Count: {0}/{1}", this.GetRoadCount(), this._concreteCountAtStartOfTutorial)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToDraw_DragCursor", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RoadCountGreaterThanOrEqualTo(1)).WhenStepStarts(delegate()
			{
				this._dragIndicatorTimer = Fix64.Zero;
				Vector2 positionToShowMessage = this._tutorialConstants.DrawRoadIdleHintEndPosition;
				if (Mathf.Abs(this.CurrentControllerWorldPosition.x - this._tutorialConstants.DrawRoadIdleHintStartPosition.x) > 4f)
				{
					positionToShowMessage = this._tutorialConstants.DrawRoadIdleHintStartPosition;
				}
				this.SetNextMessageAnchoredToWorld(StringId.Tutorial_PromptToFinishDrawRoad_Remote, positionToShowMessage, TileDirection.North, true);
				this.AddHighlightPositionIndicator(positionToShowMessage);
			}).AddIdleHint(new IdleHint().AddCondition(() => this._controllerIsDrawingRoads).SetDelayBeforeShowing(2f).SetShowHintHandler(delegate(Fix64 timestep)
			{
				Vector2 positionToShowAnimation = this._tutorialConstants.DrawRoadIdleHintEndPosition;
				if (Mathf.Abs(this.CurrentControllerWorldPosition.x - this._tutorialConstants.DrawRoadIdleHintStartPosition.x) > 4f)
				{
					positionToShowAnimation = this._tutorialConstants.DrawRoadIdleHintStartPosition;
				}
				this.DragIndicatorBetween(this.CurrentControllerWorldPosition, positionToShowAnimation, timestep);
			})).WhenStepEnds(delegate
			{
				this._dragIndicatorTimer = Fix64.Zero;
				this._drawRoadHintAnimationTimer = Fix64.Zero;
				this._roadCountAfterDrawStep = this.GetRoadCount();
			}).StepRegressesWhen(() => !this._controllerIsDrawingRoads && !this.RoadCountGreaterThanOrEqualTo(this._concreteCountAtStartOfTutorial)).SetDebugText(() => string.Format("Road Count: {0}/{1}", this.GetRoadCount(), this._concreteCountAtStartOfTutorial)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitBeforeDeletePrompt", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireTimePassed(0.5f)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToDelete_Remote", "Tell the player how to delete roads.").ClockTicksWhile(() => false).StepOverWhen(() => this._roadCountAfterDrawStep > this.GetRoadCount()).WhenStepStarts(delegate(bool isStepOver)
			{
				this.SetDrawModeToggleVisibility(true);
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_PromptToDeleteRoad_Remote, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).SetDebugText(() => string.Format("Deleted Road Count: {0}, Entered Delete Mode: {1}, Exited Delete Mode: {2}", this._roadCountAfterDrawStep - this.GetRoadCount(), this._enteredDeleteMode, this._exitedDeleteMode)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToDeleteAllRoads_Remote", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RoadCountIs(0)).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_DeleteAllRoads, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitBeforeExitDeletePrompt", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireTimePassed(0.5f)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToExitDeleteMode_Remote", null).ClockTicksWhile(() => false).StepOverWhen(() => this._exitedDeleteMode).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_TapExitBuildMode_Remote, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}));
		}

		// Token: 0x06001D44 RID: 7492 RVA: 0x00074A88 File Offset: 0x00072C88
		private void AddMouseSteps_DrawDelete()
		{
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToDraw_OneRoad", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RoadCountGreaterThanOrEqualTo((int)((float)this._concreteCountAtStartOfTutorial * 0.6f))).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_PromptToDrawRoad_Mouse, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).AddIdleHint(new IdleHint().SetDelayBeforeShowing(0.5f).SetShowHintHandler(delegate(Fix64 timestep)
			{
				this.DrawRoadHintAnimationHandler(timestep, this._tutorialConstants.DrawRoadIdleHintStartPosition, this._tutorialConstants.DrawRoadIdleHintEndPosition, 0f);
			}).AddCondition(() => this.RoadCountIs(0))).WhenStepEnds(delegate
			{
				this._drawRoadHintAnimationTimer = Fix64.Zero;
				this._roadCountAfterDrawStep = this.GetRoadCount();
			}).SetDebugText(() => string.Format("Road Count: {0}/{1}", this.GetRoadCount(), this._concreteCountAtStartOfTutorial)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitBeforeDeletePrompt", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireTimePassed(0.5f)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToDelete_Mouse", "Tell the player how to delete roads.").ClockTicksWhile(() => false).StepOverWhen(() => this._player.IsDrawModeToggleEnabled || (this._roadCountAfterDrawStep - this.GetRoadCount() >= 1 && this._enteredDeleteMode && this._exitedDeleteMode)).SetDebugText(delegate
			{
				int deletedRoadCount = this._roadCountAfterDrawStep - this.GetRoadCount();
				return string.Format("Deleted Roads {0}, current roads {1}", deletedRoadCount, this.GetRoadCount());
			}).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_PromptToDeleteRoad_Mouse, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
				this._roadCountAfterDrawStep = this.GetRoadCount();
			}).WhenStepEnds(delegate
			{
				this._simulation.IsPaused = false;
			}).AddIdleHint(new IdleHint().SetDelayBeforeShowing(15f).SetShowHintHandler(delegate()
			{
				if (this._currentPopup == null && !this._hasShownAlternateDrawModeTogglePopup && !this._player.IsDrawModeToggleEnabled)
				{
					this._hasShownAlternateDrawModeTogglePopup = true;
					this._simulation.IsPaused = true;
					this._currentPopup = this._popups.PushConfirmationPopup<ConfirmationPopup>(StringId.Options_Game_DrawDeleteToggle, delegate()
					{
						this._currentPopup = null;
					}, delegate()
					{
						this._currentPopup = null;
						this._player.IsDrawModeToggleEnabled = true;
						this.SetDrawModeToggleVisibility(true);
					}, StringId.FTUX_Accessibility_DrawModeToggleDescription);
				}
			})));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitBeforeDrawDeleteFTUX", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireTimePassed(0.5f)));
			this.ShowDrawModeFtuxPopup();
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("RecordRoadCountBeforeWaitUntilDeleteModeEnabled", null).ClockTicksWhile(() => false).StepOverWhen(() => true).WhenStepEnds(delegate
			{
				this._roadCountBeforeWaitUntilDeleteModeEnabled = this.GetRoadCount();
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitUntilDeleteModeEnabled_DrawModeToggleModeEnabled", null).ClockTicksWhile(() => false).StepOverWhen(() => !this._player.IsDrawModeToggleEnabled || ((this.GetRoadCount() < this._roadCountBeforeWaitUntilDeleteModeEnabled || this._roadCountBeforeWaitUntilDeleteModeEnabled == 0) && this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Remove)).WhenStepStarts(delegate(bool isStepOver)
			{
				if (this._player.IsDrawModeToggleEnabled)
				{
					this.SetDrawModeToggleVisibility(true);
				}
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_PromptToDeleteRoad_MouseToggle, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).AddIdleHint(new IdleHint().SetShowHintHandler(delegate()
			{
				this._gameUI.drawModeToggle.Pulse();
			})));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToDeleteAllRoads_Mouse", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RoadCountIs(0)).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_DeleteAllRoads, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("ExitDeleteMode_DrawDeleteMouse", null).ClockTicksWhile(() => false).StepOverWhen(() => !this._player.IsDrawModeToggleEnabled || this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Add).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_TapExitBuildMode_MouseToggle, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).AddIdleHint(new IdleHint().SetShowHintHandler(delegate()
			{
				this._gameUI.drawModeToggle.Pulse();
			})));
		}

		// Token: 0x06001D45 RID: 7493 RVA: 0x00074E48 File Offset: 0x00073048
		private void ShowControllerTapDrawFtux()
		{
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("TapDrawFTUX", "Ask if player wants to use tap draw on controller.").ClockTicksWhile(() => false).WhenStepStarts(delegate()
			{
				StringId headerID = StringId.TapDrawToggle;
				if (!this._player.IsTapDrawEnabled && !this._hasShownAlternateDrawModeTogglePopup)
				{
					this._hasShownAlternateDrawModeTogglePopup = true;
					this._simulation.IsPaused = true;
					this._currentPopup = this._popups.PushConfirmationPopup<ConfirmationPopup>(headerID, delegate()
					{
						this._currentPopup = null;
					}, delegate()
					{
						this._currentPopup = null;
						this._player.IsTapDrawEnabled = true;
					}, StringId.FTUX_Accessibility_DrawDeleteHoldOrTapDescription);
				}
			}).WhenStepEnds(delegate
			{
				this._simulation.IsPaused = false;
			}).StepOverWhen(() => this._currentPopup == null));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("Wait_AfterDrawModeFTUX", "Wait a little so next delete prompt doesn't pop in immediately").ClockTicksWhile(() => false).StepOverWhen(() => this.RequireTimePassed(1f)));
		}

		// Token: 0x06001D46 RID: 7494 RVA: 0x00074F18 File Offset: 0x00073118
		private void ShowDrawModeFtuxPopup()
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.FTUX_Accessibility))
			{
				this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("DrawModeFTUX", "Ask if player wants to use draw mode toggle.").ClockTicksWhile(() => false).WhenStepStarts(delegate()
				{
					if (!this._player.IsDrawModeToggleEnabled && !this._hasShownAlternateDrawModeTogglePopup)
					{
						this._hasShownAlternateDrawModeTogglePopup = true;
						this._simulation.IsPaused = true;
						this._currentPopup = this._popups.PushConfirmationPopup<ConfirmationPopup>(StringId.DrawModeToggle, delegate()
						{
							this._currentPopup = null;
						}, delegate()
						{
							this._currentPopup = null;
							this._player.IsDrawModeToggleEnabled = true;
							this.SetDrawModeToggleVisibility(true);
						}, StringId.FTUX_Accessibility_DrawModeToggleDescription);
					}
				}).WhenStepEnds(delegate
				{
					this._simulation.IsPaused = false;
				}).StepOverWhen(() => this._currentPopup == null));
				this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("Wait_AfterDrawModeFTUX", "Wait a little so next delete prompt doesn't pop in immediately").ClockTicksWhile(() => false).StepOverWhen(() => this.RequireTimePassed(1f)));
			}
		}

		// Token: 0x06001D47 RID: 7495 RVA: 0x00074FF4 File Offset: 0x000731F4
		private void AddTouchSteps_DrawDelete()
		{
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("EditMode", "Enter edit mode.").ClockTicksWhile(() => false).StepOverWhen(() => this._camera.IsFocussedIn).WhenStepStarts(delegate()
			{
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_TapEnterBuildMode_Touch, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToDrawRoad", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RoadCountGreaterThanOrEqualTo(this._concreteCountAtStartOfTutorial)).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_PromptToDrawRoad_Touch, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).StepRegressesWhen(() => !this._camera.IsFocussedIn).AddIdleHint(new IdleHint().SetDelayBeforeShowing(0.5f).SetShowHintHandler(delegate(Fix64 timestep)
			{
				this.DrawRoadHintAnimationHandler(timestep, this._tutorialConstants.DrawRoadIdleHintStartPosition, this._tutorialConstants.DrawRoadIdleHintEndPosition, 0f);
			}).AddCondition(() => this.RoadCountIs(0))).WhenStepEnds(delegate
			{
				this._drawRoadHintAnimationTimer = Fix64.Zero;
				this._roadCountAfterDrawStep = this.GetRoadCount();
			}).SetDebugText(() => string.Format("Road Count: {0}/{1}", this.GetRoadCount(), this._concreteCountAtStartOfTutorial)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitBeforeDeletePrompt", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireTimePassed(0.5f)).StepRegressesWhen(() => !this._camera.IsFocussedIn));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitUntilDeleteModeEnabled_DrawModeToggleModeEnabled", null).ClockTicksWhile(() => false).StepOverWhen(() => this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Remove && this.GetRoadCount() < this._roadCountAfterDrawStep).WhenStepStarts(delegate(bool isStepOver)
			{
				this.SetDrawModeToggleVisibility(true);
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_PromptToDeleteRoad_Touch, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).AddIdleHint(new IdleHint().SetShowHintHandler(delegate()
			{
				this._gameUI.drawModeToggle.Pulse();
			})).StepRegressesWhen(() => !this._camera.IsFocussedIn));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("PromptToDeleteAllRoads", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RoadCountIs(0)).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_DeleteAllRoads, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).StepRegressesWhen(() => this._gameUI.CurrentRoadDrawMode != RoadDrawMode.Remove));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("ShowHowToExitDrawMode", null).ClockTicksWhile(() => false).StepOverWhen(() => !this._camera.IsFocussedIn).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_TapExitBuildMode_Touch, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).StepRegressesWhen(() => this.RoadCountGreaterThanOrEqualTo(1)));
		}

		// Token: 0x06001D48 RID: 7496 RVA: 0x000752E4 File Offset: 0x000734E4
		private void AddEndStage()
		{
			this._tutorial.AddRealtimeDelay(2f, false);
			this._tutorial.StartStage("End", "E");
			this._tutorial.AddMarker(TutorialProgressionProcess.TutorialMarker.BigPinsAllowed);
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("ScoreRequirementMessage", null).ClockTicksWhile(() => false).StepOverWhen(new Func<bool>(this.HadInputAndMessageSpentMinimumTime)).WhenStepStarts(delegate()
			{
				this._scoreToFinishTutorial = Mathf.RoundToInt((float)(this._score.Score + this._tutorialConstants.AdditionalScoreToGet) / (float)this._tutorialConstants.AdditionalScoreToGetRounding) * this._tutorialConstants.AdditionalScoreToGetRounding;
				this.PrepareForDismissibleMessage();
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_ScoretoComplete, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, new int?(this._scoreToFinishTutorial));
			}).WhenStepEnds(new Action(this.RestorePlayerControl)));
			this.AddGameEndWeeklyUpgradeScreen(6, UpgradeType.TrafficLight, 20, UpgradeType.Roundabout, 20);
			this.AddGameEndWeeklyUpgradeScreen(7, UpgradeType.Bridge, 20, UpgradeType.Roundabout, 20);
			this.AddGameEndWeeklyUpgradeScreen(8, UpgradeType.TrafficLight, 20, UpgradeType.Motorway, 10);
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("GameOverScreen_TutorialEnd", null).StepOverWhen(() => true).WhenStepStarts(delegate()
			{
				this.UnregisterActions();
				this._inputState.Unsubscribe(this);
				this._screenStack.PushScreen<GameOverScreen>(ScreenStack.MotorwaysScreen.GameOver, delegate(GameOverScreen gameOverScreen)
				{
					DestinationView firstDestinationView = this._scope.Get<ViewIndex>().GetDestinationView(this.GetDestinationById(TutorialIdentifier.FirstDestination));
					gameOverScreen.focusPoint = firstDestinationView.transform.position;
				}, true, this._scope, true, null);
			}).WhenStepEnds(delegate
			{
				this._analytics.TrackTutorialFinished();
			}));
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x00075414 File Offset: 0x00073614
		private void AddGameEndWeeklyUpgradeScreen(int week, UpgradeType mainUpgrade, int mainConcrete, UpgradeType alternateUpgrade, int alternateConcrete)
		{
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("GameOverWeeekUpgrade_Week" + week.ToString(), null).StepOverWhen(() => this._clock.Week >= week || this._score.Score >= this._scoreToFinishTutorial).WhenStepEnds(delegate
			{
				if (this._score.Score < this._scoreToFinishTutorial)
				{
					this.SetNextUpgrades(mainUpgrade, mainConcrete, alternateUpgrade, alternateConcrete, false);
				}
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("AddFixedOrderPendingUpgrades", null).ClockTicksWhile(() => true).StepOverWhen(() => this.UpgradeScreenIsVisible() || this._score.Score >= this._scoreToFinishTutorial));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForPlayerToChooseUpgrade", null).ClockTicksWhile(() => false).StepOverWhen(() => this._upgradeDatabase.pendingUpgradeChoices.Count <= 0).WhenStepStarts(delegate()
			{
				if (this._score.Score < this._scoreToFinishTutorial)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_SecondUpgrade, this._tutorialConstants.UpgradeScreenMessageOffset, CameraLayer.Overlay, true, null);
				}
			}).WhenStepEnds(delegate
			{
				if (this._score.Score < this._scoreToFinishTutorial)
				{
					this.ClearCurrentMessage();
				}
			}));
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x00075554 File Offset: 0x00073754
		private void AddIntroduceClockStage()
		{
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitBeforeMakingClockVisible", null).StepOverWhen(() => this.RequireTimePassed(2f)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("MakeClockVisible", null).ClockTicksWhile(() => true).WhenStepStarts(delegate()
			{
				this._gameUI.SetClockVisibility(true);
			}).StepOverWhen(() => this.RequireTimePassed(2f)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("ClockIntroMessage", null).ClockTicksWhile(() => true).WhenStepStarts(delegate()
			{
				this.SetNextMessageAnchoredToScreen(this.ClockStringId, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
			}).StepOverWhen(() => this._gameUI.TimeButtonsVisible).AddIdleHint(new IdleHint().SetShowHintHandler(delegate()
			{
				this._scope.Get<GameUIScreen>().PulseClock();
			})));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("DelayAfterClockMessage", null).StepOverWhen(() => this.RequireTimePassed(5f)).ClockTicksWhile(() => true));
		}

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06001D4B RID: 7499 RVA: 0x000756A8 File Offset: 0x000738A8
		private StringId ClockStringId
		{
			get
			{
				switch (this._inputState.CurrentDeviceInputType)
				{
				case DeviceInputType.Mouse:
					return StringId.Tutorial_ClockIntroduction_Mouse;
				case DeviceInputType.Remote:
					return StringId.Tutorial_ClockIntroduction_Remote;
				case DeviceInputType.Controller:
					return StringId.Tutorial_ClockIntroduction_Controller;
				}
				return StringId.Tutorial_ClockIntroduction;
			}
		}

		// Token: 0x06001D4C RID: 7500 RVA: 0x000756F0 File Offset: 0x000738F0
		private void AddLearnBasicsStage()
		{
			this._tutorial.StartStage("Learn Basics", "LB");
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("FirstHouse", null).ClockTicksWhile(() => true).StepOverWhen(() => this.HouseHasSpawned(TutorialIdentifier.FirstHouse)).WhenStepStarts(delegate()
			{
				this.SetAllInputBlocked(true);
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("FirstDestination", null).ClockTicksWhile(() => true).StepOverWhen(() => this.DestinationHasSpawned(TutorialIdentifier.FirstDestination)).WhenStepEnds(delegate
			{
				this.ShowNoConcreteErrorMessage = true;
				this.LimitGeneratedDemandForDestination(TutorialIdentifier.FirstDestination, 0);
			}));
			switch (this._inputState.CurrentDeviceInputType)
			{
			case DeviceInputType.Touch:
				this.AddTouchSteps_LearnBasics();
				goto IL_10F;
			case DeviceInputType.Remote:
				this.AddRemoteSteps_LearnBasics();
				goto IL_10F;
			case DeviceInputType.Controller:
				this.AddControllerSteps_LearnBasics();
				goto IL_10F;
			}
			this.AddMouseSteps_LearnBasics();
			IL_10F:
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("DeleteReminder_Wait", "Wait for a bit before showing delete reminder.").ClockTicksWhile(() => this._clock.Hour < 23).StepOverWhen(() => this.RequireHouseConnectedToDestination(TutorialIdentifier.FirstHouse, TutorialIdentifier.FirstDestination) || this.RequireTimePassed(2f)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("DeleteReminder", "Remind player how to delete if they have no concrete remaining for a time.").ClockTicksWhile(() => this._clock.Hour < 23).StepOverWhen(() => this.RequireHouseConnectedToDestination(TutorialIdentifier.FirstHouse, TutorialIdentifier.FirstDestination)).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_EarlyDeleteMode, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).AddIdleHint(this._connectHousesIdleMessage));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("AddDemand", "Add single demand and delay for an hour").ClockTicksWhile(() => false).StepOverWhen(new Func<bool>(this.RealtimeTimerFinished)).WhenStepStarts(delegate()
			{
				this.StartRealtimeTimer(4f);
				this.AddDemandToDestination(TutorialIdentifier.FirstDestination, 1);
				this._simulation.IsPaused = true;
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("ExplainPin", null).ClockTicksWhile(() => false).StepOverWhen(new Func<bool>(this.HadInputAndMessageSpentMinimumTime)).WhenStepStarts(delegate()
			{
				this.PrepareForDismissibleMessage();
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_DemandIntroduction_02, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				this.AddHighlightPositionIndicator(this.GetFirstDestinationPinPosition());
			}).WhenStepEnds(delegate
			{
				this._simulation.IsPaused = false;
				this.RestorePlayerControl();
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForCar", "Waiting for car to reach destination.").ClockTicksWhile(() => false).StepOverWhen(() => this._simulation.GetModels<DestinationModel>()[0].TotalDemand == 0).AddIdleHint(this._connectHousesIdleMessage));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("Score", null).ClockTicksWhile(() => false).StepOverWhen(new Func<bool>(this.HadInputAndMessageSpentMinimumTime)).WhenStepStarts(delegate()
			{
				this.PrepareForDismissibleMessage();
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_ScoreIntroduction, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
			}).WhenStepEnds(new Action(this.RestorePlayerControl)).AddIdleHint(new IdleHint().SetShowHintHandler(delegate()
			{
				this._gameUI.ScoreButton.animator.SetTrigger(GameUIScreen.ScorePulseAnimatorTrigger);
			})));
			this._tutorial.AddMarker(TutorialProgressionProcess.TutorialMarker.BasicsLearnt);
		}

		// Token: 0x06001D4D RID: 7501 RVA: 0x00075A5C File Offset: 0x00073C5C
		private void AddMouseSteps_LearnBasics()
		{
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("Connecting_Mouse", "Show how to connect house to destination.").ClockTicksWhile(() => this._clock.Hour < 8).WhenStepStarts(delegate()
			{
				this.SetAllInputBlocked(false);
				this._gameUI.SetUpgradeBarVisibility(true, false);
			}).StepOverWhen(() => this.RequireHouseConnectedToDestination(TutorialIdentifier.FirstHouse, TutorialIdentifier.FirstDestination)).AddIdleHint(new IdleHint().SetDelayBeforeShowing(5f).SetShowHintHandler(delegate(Fix64 timestep)
			{
				this.DragIndicatorBetween(this.GetHouseById(TutorialIdentifier.FirstHouse), this.GetDestinationById(TutorialIdentifier.FirstDestination), timestep);
			})).AddIdleHint(new IdleHint().AddCondition(() => this._upgradeDatabase.HasUpgradeAvailable(UpgradeType.Concrete, 1)).SetDelayBeforeShowing(10f).SetShowHintHandler(delegate()
			{
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_ConnectRoad_Mouse, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, true, null);
			}).SetHideHintHandler(delegate
			{
				this.ClearCurrentMessageIf(StringId.Tutorial_ConnectRoad_Mouse);
			})));
		}

		// Token: 0x06001D4E RID: 7502 RVA: 0x00075B24 File Offset: 0x00073D24
		private void AddControllerSteps_LearnBasics()
		{
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("Connecting_Controller", "Show how to connect house to destination.").ClockTicksWhile(() => this._clock.Hour < 8).WhenStepStarts(delegate()
			{
				this.SetAllInputBlocked(false);
				this._gameUI.SetUpgradeBarVisibility(true, false);
			}).StepOverWhen(() => this.RequireHouseConnectedToDestination(TutorialIdentifier.FirstHouse, TutorialIdentifier.FirstDestination)).AddIdleHint(new IdleHint().SetDelayBeforeShowing(5f).SetShowHintHandler(delegate(Fix64 timestep)
			{
				this.DragIndicatorBetween(this.GetHouseById(TutorialIdentifier.FirstHouse), this.GetDestinationById(TutorialIdentifier.FirstDestination), timestep);
			})).AddIdleHint(new IdleHint().AddCondition(() => this._upgradeDatabase.HasUpgradeAvailable(UpgradeType.Concrete, 1)).SetDelayBeforeShowing(10f).SetShowHintHandler(delegate()
			{
				this.SetNextMessageAnchoredToScreen(this.<AddControllerSteps_LearnBasics>g__GetDrawString|21_0(), this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, true, null);
			}).SetHideHintHandler(delegate
			{
				this.ClearCurrentMessageIf(this.<AddControllerSteps_LearnBasics>g__GetDrawString|21_0());
			})));
		}

		// Token: 0x06001D4F RID: 7503 RVA: 0x00075BEC File Offset: 0x00073DEC
		private void AddRemoteSteps_LearnBasics()
		{
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("Connecting_Controller", "Show how to connect house to destination.").ClockTicksWhile(() => this._clock.Hour < 8).WhenStepStarts(delegate()
			{
				this.SetAllInputBlocked(false);
				this._gameUI.SetUpgradeBarVisibility(true, false);
			}).StepOverWhen(() => this.RequireHouseConnectedToDestination(TutorialIdentifier.FirstHouse, TutorialIdentifier.FirstDestination)).AddIdleHint(new IdleHint().SetDelayBeforeShowing(5f).SetShowHintHandler(delegate(Fix64 timestep)
			{
				this.DragIndicatorBetween(this.GetHouseById(TutorialIdentifier.FirstHouse), this.GetDestinationById(TutorialIdentifier.FirstDestination), timestep);
			})).AddIdleHint(new IdleHint().AddCondition(() => this._upgradeDatabase.HasUpgradeAvailable(UpgradeType.Concrete, 1)).SetDelayBeforeShowing(10f).SetShowHintHandler(delegate()
			{
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_ConnectRoad_Remote, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, true, null);
			}).SetHideHintHandler(delegate
			{
				this.ClearCurrentMessageIf(StringId.Tutorial_ConnectRoad_Remote);
			})));
		}

		// Token: 0x06001D50 RID: 7504 RVA: 0x00075CB4 File Offset: 0x00073EB4
		private void AddTouchSteps_LearnBasics()
		{
			this._gameUI.SetDrawButtonsHiddenByTutorial(false);
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("Connecting_Touch", "Show how to connect house to destination.").ClockTicksWhile(() => this._clock.Hour < 8).WhenStepStarts(delegate()
			{
				this.SetAllInputBlocked(false);
				this._gameUI.SetUpgradeBarVisibility(true, false);
			}).StepOverWhen(() => this.RequireHouseConnectedToDestination(TutorialIdentifier.FirstHouse, TutorialIdentifier.FirstDestination)).AddIdleHint(new IdleHint().AddCondition(() => !this._camera.IsFocussedIn).SetDelayBeforeShowing(10f).SetShowHintHandler(delegate(Fix64 timestep)
			{
				this.TouchEnterDrawModeIndicator(timestep);
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_TapEnterBuildMode_Touch, new Vector2(0f, 0.7f), CameraLayer.Default, false, null);
			}).SetHideHintHandler(delegate
			{
				this.ClearCurrentMessageIf(StringId.Tutorial_TapEnterBuildMode_Touch);
			})).AddIdleHint(new IdleHint().AddCondition(() => this._camera.IsFocussedIn).SetDelayBeforeShowing(5f).SetShowHintHandler(delegate(Fix64 timestep)
			{
				this.DragIndicatorBetween(this.GetHouseById(TutorialIdentifier.FirstHouse), this.GetDestinationById(TutorialIdentifier.FirstDestination), timestep);
			})).AddIdleHint(new IdleHint().AddCondition(() => this._camera.IsFocussedIn).AddCondition(() => this._upgradeDatabase.HasUpgradeAvailable(UpgradeType.Concrete, 1)).SetDelayBeforeShowing(15f).SetShowHintHandler(delegate()
			{
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_ConnectRoad_Touch, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, true, null);
			}).SetHideHintHandler(delegate
			{
				this.ClearCurrentMessageIf(StringId.Tutorial_ConnectRoad_Touch);
			})));
		}

		// Token: 0x06001D51 RID: 7505 RVA: 0x00075DF0 File Offset: 0x00073FF0
		private void AddLearnBasicsPracticeStage()
		{
			this._tutorial.StartStage("LearnBasics_Practice", "LBP");
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForSecondHouseSpawn", null).ClockTicksWhile(() => true).StepOverWhen(() => this.HouseHasSpawned(TutorialIdentifier.SecondHouse)).WhenStepStarts(delegate()
			{
				UpgradePackageDefinition upgradePackage = new UpgradePackageDefinition
				{
					amount = 12,
					type = UpgradeType.Concrete
				};
				this._upgradeDatabase.ApplyUpgradePackage(upgradePackage, true);
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitTillSecondHouseConnected", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireAllHousesAndDestinationsInGroupToBeConnected(0)).WhenStepEnds(delegate
			{
				this.SetTotalDemandOnDestination(TutorialIdentifier.FirstDestination, 3);
				this.LimitGeneratedDemandForDestination(TutorialIdentifier.FirstDestination, 3);
			}).AddIdleHint(this._connectHousesIdleMessage));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForAwkwardDrivewayHouseSpawn", null).ClockTicksWhile(() => true).StepOverWhen(() => this.HouseHasSpawned(TutorialIdentifier.AwkwardDrivewayHouse)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitTillAwkwardDrivewayHouseRealigned", null).ClockTicksWhile(() => false).StepOverWhen(delegate
			{
				HouseModel awkwardHouse = this.GetHouseById(TutorialIdentifier.AwkwardDrivewayHouse);
				return awkwardHouse != null && !awkwardHouse.tileModel.Tile.HasTwoLaneRoadInDirection(TileDirection.East, RoadState.Active);
			}).WhenStepStarts(delegate()
			{
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_ReorientHouse, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
			}).WhenStepEnds(delegate
			{
				this.SetTotalDemandOnDestination(TutorialIdentifier.FirstDestination, 3);
				this.LimitGeneratedDemandForDestination(TutorialIdentifier.FirstDestination, 3);
			}).AddIdleHint(new IdleHint().SetShowHintHandler(delegate(Fix64 timestep)
			{
				Vector3 position = this.GetHousePosition(TutorialIdentifier.AwkwardDrivewayHouse);
				this.DragIndicatorBetween(position, position + new Vector3(0f, 4f), timestep);
			})));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForAwkwardHouseConnected", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireAllHousesAndDestinationsInGroupToBeConnected(0)).AddIdleHint(this._connectHousesIdleMessage));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForDiagonalHouseSpawn", null).ClockTicksWhile(() => true).StepOverWhen(() => this.HouseHasSpawned(TutorialIdentifier.DiagonalHouse)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitTillDiagonalHouseConnected", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireAllHousesAndDestinationsInGroupToBeConnected(0)).WhenStepEnds(delegate
			{
				this.SetTotalDemandOnDestination(TutorialIdentifier.FirstDestination, 4);
				this.RemoveMaximumGeneratedDemandLimitForDestination(TutorialIdentifier.FirstDestination);
			}).AddIdleHint(this._connectHousesIdleMessage));
		}

		// Token: 0x06001D52 RID: 7506 RVA: 0x000760B0 File Offset: 0x000742B0
		private void AddSecondColorStage()
		{
			this._tutorial.StartStage("SecondColor", "SC");
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForNewColor", null).ClockTicksWhile(() => true).StepOverWhen(() => this.DestinationHasSpawned(TutorialIdentifier.SecondColorDestination) && this.HouseHasSpawned(TutorialIdentifier.SecondColorHouse)).WhenStepEnds(delegate
			{
				this.LimitGeneratedDemandForDestination(TutorialIdentifier.SecondColorDestination, 1);
				this.AddDemandToDestination(TutorialIdentifier.SecondColorDestination, 1);
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForVehicleToCollectDemand", null).ClockTicksWhile(() => false).StepOverWhen(() => this.DestinationDemandEquals(TutorialIdentifier.SecondColorDestination, 0)).WhenStepEnds(delegate
			{
				this.SetTotalDemandOnDestination(TutorialIdentifier.SecondColorDestination, 1);
			}).AddIdleHint(this._connectHousesIdleMessage));
			this._tutorial.AddMarker(TutorialProgressionProcess.TutorialMarker.DemandCollectedFromNewHouseColor);
			if (this._inputState.CurrentDeviceInputType == DeviceInputType.Touch)
			{
				this._tutorial.AddRealtimeDelay(15f, true);
				this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("TeachPan_Touch", null).ClockTicksWhile(() => false).StepOverWhen(() => this._cameraView.IsPlayerPanning).WhenStepStarts(delegate()
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_TouchTwoFingerPan_02, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}));
			}
			Fix64 justBeforeWeek = (Fix64)139.16666666666669;
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForLastHouseToBeConnected", null).ClockTicksWhile(() => this._clock.Time < justBeforeWeek).StepOverWhen(() => this.RequireHouseConnectedToDestinationWithSameGroup(TutorialIdentifier.LastHouseBeforeBridgeUpgrade)).WhenStepEnds(delegate
			{
				this.SetTotalDemandOnDestination(TutorialIdentifier.SecondColorDestination, 3);
			}).AddIdleHint(this._connectHousesIdleMessage));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitBeforeUpgradeScreen", null).ClockTicksWhile(() => this._clock.Time < justBeforeWeek).StepOverWhen(() => this.RequireTimePassed(20f)).StepRegressesWhen(() => !this.RequireHouseConnectedToDestinationWithSameGroup(TutorialIdentifier.LastHouseBeforeBridgeUpgrade)).AddIdleHint(this._connectHousesIdleMessage));
		}

		// Token: 0x06001D53 RID: 7507 RVA: 0x000762E4 File Offset: 0x000744E4
		private void AddSetupBigPinStage()
		{
			this._tutorial.StartStage("Setup Big Pin", "SBP");
			this._tutorial.AddMarker(TutorialProgressionProcess.TutorialMarker.BeganBigPinStage);
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForLastSpawnBeforeBigPin", null).StepOverWhen(() => this.HouseHasSpawned(TutorialIdentifier.LastHouseBeforeBigPin)));
		}

		// Token: 0x06001D54 RID: 7508 RVA: 0x0007633C File Offset: 0x0007453C
		private void AddSetupMotorwayStage()
		{
			this._tutorial.StartStage("Setup Motorway", "SM");
			this._tutorial.AddMarker(TutorialProgressionProcess.TutorialMarker.BeganMotorwayStage);
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForFirstHouseBeforeMotorwayToBeConnected", null).ClockTicksWhile(() => !this.HouseHasSpawnedAndHasDestinationToTravelTo(TutorialIdentifier.SetupMotorway_FirstHouse)).StepOverWhen(() => this.RequireHouseConnectedToDestinationWithSameGroup(TutorialIdentifier.SetupMotorway_FirstHouse)));
			Fix64 justBeforeWeek = (Fix64)419.1666666666667;
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForLastHouseToSpawn", null).ClockTicksWhile(() => this._clock.Time < justBeforeWeek).StepOverWhen(() => this.HouseHasSpawned(TutorialIdentifier.SetupMotorway_LastHouse)).StepRegressesWhen(() => !this.RequireHouseConnectedToDestinationWithSameGroup(TutorialIdentifier.SetupMotorway_FirstHouse)).AddIdleHint(this._connectHousesIdleMessage));
			(Fix64)420.0;
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("GiveTimeToConnectHouses", null).ClockTicksWhile(() => this._clock.Time < justBeforeWeek).StepOverWhen(() => this.RequireTimePassed(10f)).StepRegressesWhen(() => !this.RequireHouseConnectedToDestinationWithSameGroup(TutorialIdentifier.SetupMotorway_FirstHouse)).AddIdleHint(this._connectHousesIdleMessage));
		}

		// Token: 0x06001D55 RID: 7509 RVA: 0x0007647C File Offset: 0x0007467C
		private void AddSetupRoundaboutStage()
		{
			this._tutorial.StartStage("Setup Roundabout", "SR");
			this._tutorial.AddMarker(TutorialProgressionProcess.TutorialMarker.BeganRoundaboutStage);
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForFirstHouseToBeConnected", null).ClockTicksWhile(() => !this.HouseHasSpawnedAndHasDestinationToTravelTo(TutorialIdentifier.SetupRoundabout_FirstHouse)).StepOverWhen(() => this.RequireHouseConnectedToDestinationWithSameGroup(TutorialIdentifier.SetupRoundabout_FirstHouse)).AddIdleHint(this._connectHousesIdleMessage));
			Fix64 justBeforeWeek = (Fix64)559.1666666666667;
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForLastHouseToSpawn", null).ClockTicksWhile(() => this._clock.Time < justBeforeWeek).StepOverWhen(() => this.HouseHasSpawned(TutorialIdentifier.SetupRoundabout_LastHouse)).StepRegressesWhen(() => !this.RequireHouseConnectedToDestinationWithSameGroup(TutorialIdentifier.SetupRoundabout_FirstHouse)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("GiveTimeToConnectHouses", null).ClockTicksWhile(() => this._clock.Time < justBeforeWeek).StepOverWhen(() => this.RequireTimePassed(10f)).StepRegressesWhen(() => !this.RequireHouseConnectedToDestinationWithSameGroup(TutorialIdentifier.SetupRoundabout_FirstHouse)));
		}

		// Token: 0x06001D56 RID: 7510 RVA: 0x000765A0 File Offset: 0x000747A0
		public Fix64 TimeAtStartOfWeek(int week)
		{
			return (Fix64)((long)(week * 189));
		}

		// Token: 0x06001D57 RID: 7511 RVA: 0x000765B0 File Offset: 0x000747B0
		private void AddSetupTrafficLightStage()
		{
			this._tutorial.StartStage("Setup Traffic Light", "STL");
			this._tutorial.AddMarker(TutorialProgressionProcess.TutorialMarker.BeganTrafficLightStage);
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForFirstHouseToBeConnected", null).ClockTicksWhile(() => !this.HouseHasSpawnedAndHasDestinationToTravelTo(TutorialIdentifier.SetupTrafficLight_FirstHouse)).StepOverWhen(() => this.RequireHouseConnectedToDestinationWithSameGroup(TutorialIdentifier.SetupTrafficLight_FirstHouse)).AddIdleHint(this._connectHousesIdleMessage));
			if (FeatureToggle.IsFeatureEnabled(Feature.FTUX_Accessibility))
			{
				this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("DelayBeforeColorblindPrompt", null).ClockTicksWhile(() => true).StepOverWhen(() => this.RequireTimePassed(1f)));
				Action <>9__12;
				Action <>9__13;
				this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("ColorblindPrompt", null).ClockTicksWhile(() => false).WhenStepStarts(delegate()
				{
					if (!this._player.IsColorblindModeEnabled)
					{
						this._simulation.IsPaused = true;
						TutorialProgressionProcess <>4__this = this;
						PopupStack popups = this._popups;
						StringId mainPromptStringId = StringId.ColorblindMode;
						Action onNoPressed;
						if ((onNoPressed = <>9__12) == null)
						{
							onNoPressed = (<>9__12 = delegate()
							{
								this._currentPopup = null;
							});
						}
						Action onYesPressed;
						if ((onYesPressed = <>9__13) == null)
						{
							onYesPressed = (<>9__13 = delegate()
							{
								this._currentPopup = null;
								this._player.IsColorblindModeEnabled = true;
							});
						}
						<>4__this._currentPopup = popups.PushConfirmationPopup<ConfirmationPopup>(mainPromptStringId, onNoPressed, onYesPressed, StringId.FTUX_Accessibility_EnableColorblindModeDescription);
					}
				}).WhenStepEnds(delegate
				{
					this._simulation.IsPaused = false;
				}).StepOverWhen(() => this._currentPopup == null));
			}
			Fix64 justBeforeWeek = (Fix64)279.1666666666667;
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForLastHouseToSpawn", null).ClockTicksWhile(() => this._clock.Time < justBeforeWeek).StepOverWhen(() => this.HouseHasSpawned(TutorialIdentifier.SetupTrafficLight_LastHouse)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("GiveTimeToConnectHouses", null).ClockTicksWhile(() => this._clock.Time < justBeforeWeek).StepOverWhen(() => this.RequireTimePassed(10f)));
		}

		// Token: 0x06001D58 RID: 7512 RVA: 0x00076778 File Offset: 0x00074978
		private void AddUpgradeBridgeStage()
		{
			this._tutorial.StartStage("Upgrade Bridge (First Upgrade)", "BU");
			Fix64 startOfSecondWeek = (Fix64)140.0;
			this._tutorial.AddMarker(TutorialProgressionProcess.TutorialMarker.BeganBridgeStage);
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("ExplainEndOfWeekScreen", null).ClockTicksWhile(() => false).StepOverWhen(() => this.HadInputAndMessageSpentMinimumTime()).WhenStepStarts(delegate()
			{
				this.PrepareForDismissibleMessage();
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_ExplainEndOfWeek, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, true, null);
			}).WhenStepEnds(new Action(this.RestorePlayerControl)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("DelayBeforeUpgradeScreen", null).ClockTicksWhile(() => true).WhenStepStarts(delegate()
			{
				this.StartRealtimeTimer(3f);
			}).StepOverWhen(new Func<bool>(this.RealtimeTimerFinished)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("AddFixedOrderPendingUpgrades", null).ClockTicksWhile(() => false).WhenStepStarts(delegate()
			{
				this.SetNextUpgrades(UpgradeType.Bridge, this._tutorialConstants.DefaultConcreteForUpgradePair, UpgradeType.TrafficLight, this._tutorialConstants.DefaultConcreteForUpgradePair, true);
			}).StepOverWhen(new Func<bool>(this.UpgradeScreenIsVisible)).WhenStepEnds(delegate
			{
				this.SkipClockTo(startOfSecondWeek);
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("Realtime1SecondDelay", null).ClockTicksWhile(() => true).WhenStepStarts(delegate()
			{
				this.StartRealtimeTimer(1f);
			}).StepOverWhen(new Func<bool>(this.RealtimeTimerFinished)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("AskPlayerToChooseBridge", null).ClockTicksWhile(() => true).StepOverWhen(() => this._upgradeDatabase.pendingUpgradeChoices.Count <= 0).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_ChooseTheBridge, this._tutorialConstants.UpgradeScreenMessageOffset, CameraLayer.Overlay, true, null);
				}
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForHouseAcrossRiver", null).ClockTicksWhile(() => true).StepOverWhen(() => this.HouseHasSpawned(TutorialIdentifier.HouseAcrossRiver)));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("ActionConnectHouseAcrossRiver", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireExactUpgradeCount(UpgradeType.Bridge, 0) && this.RequireHouseConnectedToDestination(TutorialIdentifier.HouseAcrossRiver, TutorialIdentifier.SecondColorDestination)).WhenStepStarts(delegate()
			{
				this._dragIndicatorTimer = Fix64.Zero;
			}).WhenStepEnds(delegate
			{
				this.SetTotalDemandOnDestination(TutorialIdentifier.SecondColorDestination, 5);
				this.RemoveMaximumGeneratedDemandLimitForDestination(TutorialIdentifier.SecondColorDestination);
			}).AddIdleHint(new IdleHint().SetDelayBeforeShowing(1f).SetShowHintHandler(delegate(Fix64 timestep)
			{
				this.DragIndicatorBetween(this.GetHouseById(TutorialIdentifier.HouseAcrossRiver), this.GetDestinationById(TutorialIdentifier.SecondColorDestination), timestep);
			})).AddIdleHint(new IdleHint().SetDelayBeforeShowing(1f).SetShowHintHandler(delegate()
			{
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_DrawRoadAcrossWater, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, true, null);
			})));
		}

		// Token: 0x06001D59 RID: 7513 RVA: 0x00076AB8 File Offset: 0x00074CB8
		private void AddUpgradeChoiceStage()
		{
			Fix64 startOfFifthWeek = (Fix64)700.0;
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("DelayBeforeUpgradeScreen", null).StepOverWhen(() => this.RequireTimePassed(20f)));
			this._tutorial.AddMarker(TutorialProgressionProcess.TutorialMarker.BeganUpgradeChoiceStage);
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("ShowEndOfWeekScreen", null).ClockTicksWhile(() => false).StepOverWhen(new Func<bool>(this.UpgradeScreenIsVisible)).WhenStepStarts(delegate()
			{
				this.SetNextUpgrades(UpgradeType.Bridge, 20, UpgradeType.Motorway, 10, false);
			}).WhenStepEnds(delegate
			{
				this.SkipClockTo(startOfFifthWeek);
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("ShowMessageAndWaitForPlayerToChooseUpgrade", null).ClockTicksWhile(() => false).StepOverWhen(() => this._upgradeDatabase.pendingUpgradeChoices.Count <= 0).WhenStepStarts(delegate()
			{
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_SecondUpgrade, this._tutorialConstants.UpgradeScreenMessageOffset, CameraLayer.Overlay, true, null);
			}).WhenStepEnds(new Action(this.ClearCurrentMessage)));
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x00076BF4 File Offset: 0x00074DF4
		private void AddUpgradeMotorwayStage()
		{
			this._tutorial.StartStage("Upgrade Motorway", "UM");
			Fix64 startOfFourthWeek = (Fix64)420.0;
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("AddFixedOrderPendingUpgrades", null).ClockTicksWhile(() => false).WhenStepStarts(delegate()
			{
				this.SetNextUpgrades(UpgradeType.Motorway, this._tutorialConstants.DefaultConcreteForUpgradePair, UpgradeType.Bridge, this._tutorialConstants.DefaultConcreteForUpgradePair, true);
			}).StepOverWhen(new Func<bool>(this.UpgradeScreenIsVisible)).WhenStepEnds(delegate
			{
				this.SkipClockTo(startOfFourthWeek);
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("AskPlayerToChooseMotorway", null).ClockTicksWhile(() => true).StepOverWhen(() => this._upgradeDatabase.pendingUpgradeChoices.Count <= 0).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_ChooseTheMotorway, this._tutorialConstants.UpgradeScreenMessageOffset, CameraLayer.Overlay, true, null);
				}
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitToTakeMotorway", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireExactUpgradeCount(UpgradeType.Motorway, 0) || this.HasActiveAssetDragAction(GameUIButtonType.Motorway)).WhenStepStarts(delegate(bool isStepOver)
			{
				this._scope.Get<NotificationView>().NotificationsEnabled = false;
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_Motorway_PlaceStart, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).AddIdleHint(new IdleHint().SetShowHintHandler(delegate()
			{
				this._gameUI.UpgradeBar.PulseUpgradeIcon(UpgradeType.Motorway);
			})));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("TellPlayerHowToDragOutMotorway", null).ClockTicksWhile(() => false).StepOverWhen(new Func<bool>(this.HasPlacedMotorway)).StepRegressesWhen(() => this.RequireExactUpgradeCount(UpgradeType.Motorway, 1)).WhenStepStarts(delegate(bool isStepOver)
			{
				if (isStepOver)
				{
					return;
				}
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_Motorway_PlaceEnd, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("GiveSomeTimeToEnsureConnected", null).ClockTicksWhile(() => true).WhenStepStarts(delegate()
			{
				this.StartRealtimeTimer(5f);
			}).StepOverWhen(() => this.RealtimeTimerFinished() || this.IsMotorwayConnectedToRoads()));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("RequireTheMotorwayConnectedToEdges", null).ClockTicksWhile(() => false).StepOverWhen(new Func<bool>(this.IsMotorwayConnectedToRoads)).StepRegressesWhen(() => this.RequireExactUpgradeCount(UpgradeType.Motorway, 1)).WhenStepStarts(delegate(bool isStepOver)
			{
				if (isStepOver)
				{
					return;
				}
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_Motorway_Roads, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
			}).WhenStepEnds(delegate
			{
				this.AddDemandToAllDestinations(1);
				this.SetTotalDemandOnDestination(TutorialIdentifier.UpgradeMotorway_Destination, 4);
				this._scope.Get<NotificationView>().NotificationsEnabled = true;
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitToSomeCarsToDriveOverMotorway", null).ClockTicksWhile(() => false).StepOverWhen(() => this.TripsOnMotorwaysGreaterThanOrEqualTo(3)).SetDebugText(() => string.Format("# vehicles exited motorway: {0}/{1}", this._numberOfVehiclesThatHaveLeftAMotorway, 3)));
		}

		// Token: 0x06001D5B RID: 7515 RVA: 0x00076F1C File Offset: 0x0007511C
		private void AddUpgradeRoundaboutStage()
		{
			this._tutorial.StartStage("Upgrade Roundabout", "UR");
			Fix64 startOfWeekFive = (Fix64)560.0;
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("AddFixedOrderPendingUpgrades", null).ClockTicksWhile(() => false).WhenStepStarts(delegate()
			{
				this.SetNextUpgrades(UpgradeType.Roundabout, this._tutorialConstants.DefaultConcreteForUpgradePair, UpgradeType.Bridge, this._tutorialConstants.DefaultConcreteForUpgradePair, true);
			}).StepOverWhen(new Func<bool>(this.UpgradeScreenIsVisible)).WhenStepEnds(delegate
			{
				this.SkipClockTo(startOfWeekFive);
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("AskPlayerToChooseRoundabout", null).ClockTicksWhile(() => false).StepOverWhen(() => this._upgradeDatabase.pendingUpgradeChoices.Count <= 0).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_ChooseTheRoundabout, this._tutorialConstants.UpgradeScreenMessageOffset, CameraLayer.Overlay, true, null);
				}
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitTillPlacedRoundabout", null).ClockTicksWhile(() => false).StepOverWhen(() => this.HasPlacedUpgrade(UpgradeType.Roundabout)).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_DragRoundabout, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).AddIdleHint(new IdleHint().SetShowHintHandler(delegate()
			{
				this._gameUI.UpgradeBar.PulseUpgradeIcon(UpgradeType.Roundabout);
			})));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitToSomeCarsToDriveThroughRoundabout", null).ClockTicksWhile(() => false).StepOverWhen(() => this.TripsOnRoundaboutGreaterThanOrEqualTo(1)).SetDebugText(() => string.Format("# vehicles exited roundabout: {0}/{1}", this._numberOfVehiclesThatHaveLeftARoundabout, 1)).StepRegressesWhen(() => this.RequireExactUpgradeCount(UpgradeType.Roundabout, 1)).AddIdleHint(new IdleHint().SetDelayBeforeShowing(40f).SetShowHintHandler(delegate()
			{
				this.SetNextMessageAnchoredToScreen(StringId.Tutorial_RoundaboutNoTripsHint, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
			}).AddCondition(() => this.TripsOnRoundaboutGreaterThanOrEqualTo(0))));
		}

		// Token: 0x06001D5C RID: 7516 RVA: 0x00077144 File Offset: 0x00075344
		private void AddUpgradeTrafficLightStage()
		{
			this._tutorial.StartStage("Upgrade Traffic Light", "UTL");
			Fix64 startOfThirdWeek = (Fix64)280.0;
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("AddFixedOrderPendingUpgrades", null).ClockTicksWhile(() => false).WhenStepStarts(delegate()
			{
				this.SetNextUpgrades(UpgradeType.TrafficLight, this._tutorialConstants.DefaultConcreteForUpgradePair, UpgradeType.Bridge, this._tutorialConstants.DefaultConcreteForUpgradePair, true);
			}).StepOverWhen(new Func<bool>(this.UpgradeScreenIsVisible)).WhenStepEnds(delegate
			{
				this.SkipClockTo(startOfThirdWeek);
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("AskPlayerToChooseTrafficLight", null).ClockTicksWhile(() => false).StepOverWhen(() => this._upgradeDatabase.pendingUpgradeChoices.Count <= 0).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_ChooseTheTrafficLight, this._tutorialConstants.UpgradeScreenMessageOffset, CameraLayer.Overlay, true, null);
				}
			}));
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("Wait for traffic light to be taken.", null).ClockTicksWhile(() => false).StepOverWhen(() => this.RequireExactUpgradeCount(UpgradeType.TrafficLight, 0) || this.HasActiveAssetDragAction(GameUIButtonType.TrafficLight)).WhenStepStarts(delegate(bool isStepOver)
			{
				if (!isStepOver)
				{
					this.SetNextMessageAnchoredToScreen(StringId.Tutorial_TrafficLight_02, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, false, null);
				}
			}).AddIdleHint(new IdleHint().SetShowHintHandler(delegate()
			{
				this._gameUI.UpgradeBar.PulseUpgradeIcon(UpgradeType.TrafficLight);
			})));
			Fix64 timeToPauseClock = (Fix64)285.0;
			this._tutorial.AddStep(new TutorialProgressionProcess.TutorialStep("WaitForTrafficLightToBePlaced", null).ClockTicksWhile(() => this._clock.Time < timeToPauseClock).StepOverWhen(new Func<bool>(this.HasPlacedTrafficLight)).AddIdleHint(new IdleHint().SetShowHintHandler(delegate()
			{
				this._gameUI.UpgradeBar.PulseUpgradeIcon(UpgradeType.TrafficLight);
			})).WhenStepEnds(delegate
			{
				this.AddDemandToAllDestinations(2);
			}));
		}

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06001D5D RID: 7517 RVA: 0x0007733F File Offset: 0x0007553F
		// (set) Token: 0x06001D5E RID: 7518 RVA: 0x00077347 File Offset: 0x00075547
		public string CurrentStage { get; private set; }

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x06001D5F RID: 7519 RVA: 0x00077350 File Offset: 0x00075550
		// (set) Token: 0x06001D60 RID: 7520 RVA: 0x00077358 File Offset: 0x00075558
		public string CurrentStageShortName { get; private set; }

		// Token: 0x06001D61 RID: 7521 RVA: 0x00077361 File Offset: 0x00075561
		public void SetCurrentStage(string name, string shortName)
		{
			this.CurrentStage = name;
			this.CurrentStageShortName = shortName;
		}

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06001D62 RID: 7522 RVA: 0x00077371 File Offset: 0x00075571
		// (set) Token: 0x06001D63 RID: 7523 RVA: 0x00077379 File Offset: 0x00075579
		public TutorialProgressionProcess.TutorialMarker LastReachedMarker { get; private set; }

		// Token: 0x06001D64 RID: 7524 RVA: 0x00077382 File Offset: 0x00075582
		public void SetLastReachedMarker(TutorialProgressionProcess.TutorialMarker tutorialMarker)
		{
			this._analytics.TrackTutorialStage((int)tutorialMarker);
			this.LastReachedMarker = tutorialMarker;
		}

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06001D65 RID: 7525 RVA: 0x00077397 File Offset: 0x00075597
		public Fix64 ClockSpeedMultiplier
		{
			get
			{
				return this._clockSpeedMultiplier;
			}
		}

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x06001D66 RID: 7526 RVA: 0x0007739F File Offset: 0x0007559F
		private Vector3 CurrentControllerWorldPosition
		{
			get
			{
				return this._currentControllerPosition.ToVector3() * 2f;
			}
		}

		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x06001D67 RID: 7527 RVA: 0x000773B6 File Offset: 0x000755B6
		// (set) Token: 0x06001D68 RID: 7528 RVA: 0x000773BE File Offset: 0x000755BE
		public bool HasPlayerMothballedARoad { get; private set; }

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x06001D69 RID: 7529 RVA: 0x000773C7 File Offset: 0x000755C7
		// (set) Token: 0x06001D6A RID: 7530 RVA: 0x000773CF File Offset: 0x000755CF
		public bool ShowNoConcreteErrorMessage { get; private set; } = true;

		// Token: 0x06001D6B RID: 7531 RVA: 0x000773D8 File Offset: 0x000755D8
		private void Initialize()
		{
			this._gameUI = this._scope.Get<GameUIScreen>();
			this._camera = this._scope.Get<CameraView>();
			this._gameUI.SetDrawButtonsHiddenByTutorial(true);
			this._gameUI.SetDrawButtonsVisible(false);
			this._gameUI.SetTileHighlightsAllowed(false);
			this._clockSpeedMultiplier = Fix64.Zero;
			this._connectHousesIdleMessage = new IdleHint().SetDelayBeforeShowing(40f).SetShowHintHandler(delegate()
			{
				if (!this.HasVisibleMessage && !this._connectHouseIdleMessageHasBeenDismissed)
				{
					this.PrepareForDismissibleMessage();
					this.AddMessageAnchoredToScreen(StringId.Tutorial_Error_UnconnectedHouses, this._tutorialConstants.UnanchoredMessageOffset, CameraLayer.Default, null);
					return;
				}
				if (!this._connectHouseIdleMessageHasBeenDismissed && this.HasVisibleMessage && this.HadInputAndMessageSpentMinimumTime())
				{
					this._connectHouseIdleMessageHasBeenDismissed = true;
					this.RestorePlayerControl();
					this.ClearCurrentMessageIf(StringId.Tutorial_Error_UnconnectedHouses);
				}
			}).SetProgressionHandler(delegate
			{
				this._connectHouseIdleMessageHasBeenDismissed = false;
			});
			this.CreateStages();
			this._inputState.Subscribe(this);
			this.RegisterActions();
			this._player.SetNewContentSeen("NewControllerSchemePopup");
			this._player.SetNewContentSeen("NewColorblindPopup");
		}

		// Token: 0x06001D6C RID: 7532 RVA: 0x000774A8 File Offset: 0x000756A8
		public void Reset()
		{
			this._gameUI = null;
			this._camera = null;
			this.hadInput = false;
			this._isInTutorial = false;
			this._dragIndicatorTimer = Fix64.Zero;
			this._rules = null;
			this._clockSpeedMultiplier = Fix64Consts.Zero;
			this._isProgressing = false;
			this._currentStepIndex = 0;
			this.LastReachedMarker = TutorialProgressionProcess.TutorialMarker.InitialMarker;
			this._timeSpentInStep = Fix64.Zero;
			this._timeSpentNotProgressing = Fix64.Zero;
			this._tutorial = null;
			this._animatorViews.Clear();
			this.currentMessage = null;
			this._nextMessage = null;
			this._currentControllerPosition = default(Vector2Int);
			this._controllerIsDrawingRoads = false;
			this._unscaledMessageTimer = 0f;
			this._skipTimeForDismissibleMessages = false;
			this.HasPlayerMothballedARoad = false;
			this._tapIndexTimer = default(Fix64);
			this._demandLimits.Clear();
			this._numberOfVehiclesThatHaveLeftAMotorway = 0;
			this._vehiclesOnMotorway.Clear();
			this._numberOfVehiclesThatHaveLeftARoundabout = 0;
			this._vehiclesOnRoundabout.Clear();
			this._enteredDeleteMode = true;
			this._exitedDeleteMode = true;
			this._drawRoadHintAnimationTimer = Fix64.Zero;
			this._hasShownAlternateDrawModeTogglePopup = false;
			this._connectHousesIdleMessage = null;
			this._scoreToFinishTutorial = 0;
			this._roadCountAfterDrawStep = 0;
			this._roadCountBeforeWaitUntilDeleteModeEnabled = 0;
			this._concreteCountAtStartOfTutorial = 0;
			this._connectHouseIdleMessageHasBeenDismissed = false;
			this.ShowNoConcreteErrorMessage = true;
		}

		// Token: 0x06001D6D RID: 7533 RVA: 0x000775F8 File Offset: 0x000757F8
		private void CreateStages()
		{
			this._tutorial = new TutorialBuilder(this);
			this.AddDrawDeleteStage();
			this.AddLearnBasicsStage();
			this.AddLearnBasicsPracticeStage();
			this.AddSecondColorStage();
			this.AddUpgradeBridgeStage();
			this.AddSetupTrafficLightStage();
			this.AddUpgradeTrafficLightStage();
			this.AddSetupMotorwayStage();
			this.AddUpgradeMotorwayStage();
			this.AddSetupRoundaboutStage();
			this.AddUpgradeRoundaboutStage();
			this.AddSetupBigPinStage();
			this.AddBigPinStage();
			this.AddUpgradeChoiceStage();
			this.AddIntroduceClockStage();
			this.AddEndStage();
		}

		// Token: 0x06001D6E RID: 7534 RVA: 0x00077674 File Offset: 0x00075874
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			if (this._unscaledMessageTimer > 0f)
			{
				if (timestep == Fix64.Zero)
				{
					this._unscaledMessageTimer -= Time.unscaledDeltaTime * (1f / (float)Simulation.DefaultTimestep);
				}
				else
				{
					this._unscaledMessageTimer -= Time.unscaledDeltaTime * (1f / (float)timestep);
				}
			}
			if (!this._isInTutorial && this._rules != null)
			{
				return;
			}
			if (!this._isInTutorial && this._rules == null)
			{
				this._rules = this._city.Rules;
				this._isInTutorial = (this._rules is TutorialGameRules);
				if (!this._isInTutorial)
				{
					return;
				}
				this.Initialize();
				if (this._currentStepIndex < this._tutorial.Steps.Count)
				{
					TutorialProgressionProcess.TutorialStep currentStep = this._tutorial.Steps[this._currentStepIndex];
					Action<bool> onStepStart = currentStep.OnStepStart;
					if (onStepStart != null)
					{
						Func<bool> isStepOver = currentStep.IsStepOver;
						onStepStart(isStepOver != null && isStepOver());
					}
				}
			}
			this.CheckForEnteringAndExitingDeleteMode();
			this.CheckIfVehicleLeftMotorway();
			this.CheckIfVehicleLeftRoundabout();
			this._timeSpentInStep += timestep;
			if (this._isProgressing && this.ClockSpeedMultiplier <= Fix64.One)
			{
				this._clockSpeedMultiplier = Fix64.Clamp01(this._clockSpeedMultiplier + timestep * TutorialProgressionProcess.ClockAccelerationMultiplier);
			}
			else if (!this._isProgressing && this.ClockSpeedMultiplier >= Fix64.Zero)
			{
				this._clockSpeedMultiplier = Fix64.Clamp01(this._clockSpeedMultiplier - timestep * TutorialProgressionProcess.ClockDecelerationMultiplier);
			}
			if (this._currentStepIndex < this._tutorial.Steps.Count)
			{
				TutorialProgressionProcess.TutorialStep currentStep2 = this._tutorial.Steps[this._currentStepIndex];
				if (!this._isProgressing)
				{
					this._timeSpentNotProgressing += timestep;
				}
				this._isProgressing = currentStep2.DoesClockTick();
				if (this._timeSpentNotProgressing > TutorialProgressionProcess.DelayBeforeIdleMessage && currentStep2.IdleMessageAnimationHandler != null)
				{
					currentStep2.IdleMessageAnimationHandler(timestep);
				}
				else if (this._timeSpentNotProgressing > TutorialProgressionProcess.DelayBeforeIdleAnimation)
				{
					Action<Fix64> idlePromptAnimationHandler = currentStep2.IdlePromptAnimationHandler;
					if (idlePromptAnimationHandler != null)
					{
						idlePromptAnimationHandler(timestep);
					}
				}
				foreach (IdleHint idleHint in currentStep2.IdleHints)
				{
					bool areShowConditionsMet = true;
					if (idleHint.ShowConditions != null)
					{
						foreach (Func<bool> idleHintCondition in idleHint.ShowConditions)
						{
							areShowConditionsMet &= idleHintCondition();
						}
					}
					if (areShowConditionsMet)
					{
						if (idleHint.idleTime <= idleHint.DelayBeforeShowing)
						{
							idleHint.idleTime += timestep;
						}
						else
						{
							Action<Fix64> showHintHandler = idleHint.ShowHintHandler;
							if (showHintHandler != null)
							{
								showHintHandler(timestep);
							}
						}
					}
					else
					{
						Action hideHintHandler = idleHint.HideHintHandler;
						if (hideHintHandler != null)
						{
							hideHintHandler();
						}
						idleHint.idleTime = Fix64.Zero;
					}
				}
				if (Diagnostics.Verify(currentStep2.IsStepOver != null, "'{0}' must have a IsStepOver action", currentStep2.Id))
				{
					if (currentStep2.IsStepOver())
					{
						Action onStepComplete = currentStep2.OnStepComplete;
						if (onStepComplete != null)
						{
							onStepComplete();
						}
						this.TransitionToStep(this._currentStepIndex + 1);
					}
					else if (currentStep2.ShouldRegressStep != null && currentStep2.ShouldRegressStep())
					{
						this.TransitionToStep(this._currentStepIndex - 1);
					}
				}
			}
			else
			{
				this._isProgressing = true;
			}
			if (this._nextMessage != null)
			{
				TutorialProgressionProcess.MessageData message = this._nextMessage.Value;
				if (message.force && this.HasVisibleMessage && !this.currentMessage.Message.Equals(message.messageString))
				{
					this.ClearCurrentMessage();
				}
				if (!this.HasVisibleMessage)
				{
					if (message.IsWorldAnchored)
					{
						this.AddMessageAnchoredToWorld(message.messageString, message.position, message.direction);
					}
					else if (message.IsUIAnchored)
					{
						this.AddMessageAnchoredToUI(message.messageString, message.uiAnchor, new Vector2?(message.position));
					}
					else if (message.IsScreenAnchored)
					{
						this.AddMessageAnchoredToScreen(message.messageString, message.position, message.cameraLayer, message.intParameter);
					}
				}
			}
			this.CheckIfPlayerHasMothballedARoad(simulation);
			this.hadInput = false;
		}

		// Token: 0x06001D6F RID: 7535 RVA: 0x00077B3C File Offset: 0x00075D3C
		private void CheckForEnteringAndExitingDeleteMode()
		{
			if (this._enteredDeleteMode && this._exitedDeleteMode)
			{
				this._enteredDeleteMode = (this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Remove);
				this._exitedDeleteMode = false;
			}
			RoadDrawMode previousRoadDrawMode = this._previousRoadDrawMode;
			if (previousRoadDrawMode != RoadDrawMode.Add)
			{
				if (previousRoadDrawMode == RoadDrawMode.Remove)
				{
					if (this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Add)
					{
						this._exitedDeleteMode = true;
					}
				}
			}
			else if (this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Remove)
			{
				this._enteredDeleteMode = true;
			}
			this._previousRoadDrawMode = this._gameUI.CurrentRoadDrawMode;
		}

		// Token: 0x06001D70 RID: 7536 RVA: 0x00077BC0 File Offset: 0x00075DC0
		private void TransitionToStep(int newStepIndex)
		{
			foreach (IdleHint idleHint in this._tutorial.Steps[this._currentStepIndex].IdleHints)
			{
				idleHint.idleTime = Fix64.Zero;
				Action stepProgressedHandler = idleHint.StepProgressedHandler;
				if (stepProgressedHandler != null)
				{
					stepProgressedHandler();
				}
			}
			this._timeSpentInStep = Fix64.Zero;
			this._timeSpentNotProgressing = Fix64.Zero;
			this._currentStepIndex = newStepIndex;
			this.ClearCurrentMessage();
			if (this._currentStepIndex >= this._tutorial.Steps.Count)
			{
				return;
			}
			TutorialProgressionProcess.TutorialStep currentStep = this._tutorial.Steps[this._currentStepIndex];
			if (currentStep.ShouldRegressStep != null && currentStep.ShouldRegressStep())
			{
				this.TransitionToStep(this._currentStepIndex - 1);
				return;
			}
			Action<bool> onStepStart = currentStep.OnStepStart;
			if (onStepStart == null)
			{
				return;
			}
			Func<bool> isStepOver = currentStep.IsStepOver;
			onStepStart(isStepOver != null && isStepOver());
		}

		// Token: 0x06001D71 RID: 7537 RVA: 0x00077CD4 File Offset: 0x00075ED4
		private void CheckIfPlayerHasMothballedARoad(ISimulation simulation)
		{
			if (this.HasPlayerMothballedARoad)
			{
				return;
			}
			TilemapModel tilemapModel = simulation.GetModel<TilemapModel>();
			foreach (TileModel tileModel in simulation.GetModels<TileModel>())
			{
				TileDirectionBitfield directions = tileModel.Tile.GetTwoLaneRoads(RoadState.Mothballed, Tile.MotorwayInclusion.Ignore);
				if (directions.Count > 0 && tileModel.Tile.ContentType != TileContentType.House)
				{
					if (directions.Count == 1)
					{
						TileDirection direction = directions[0];
						Vector2Int coordinate = TileUtilities.GetAdjacentCoordinates(tileModel.Tile.Coordinates, direction);
						if (tilemapModel.GetTileModel(coordinate).Tile.ContentType == TileContentType.House)
						{
							continue;
						}
					}
					this.HasPlayerMothballedARoad = true;
					return;
				}
			}
		}

		// Token: 0x06001D72 RID: 7538 RVA: 0x00077D80 File Offset: 0x00075F80
		private bool HasPlacedUpgrade(UpgradeType upgradeType)
		{
			switch (upgradeType)
			{
			case UpgradeType.Bridge:
				return this._upgradeDatabase.GetUsedUpgradeCount(UpgradeType.Bridge) >= 1;
			case UpgradeType.Motorway:
				return this._simulation.GetModel<MotorwayModel>() != null;
			case UpgradeType.TrafficLight:
				return this._simulation.GetModel<TrafficLightModel>() != null;
			case UpgradeType.Roundabout:
				return this._upgradeDatabase.GetUsedUpgradeCount(UpgradeType.Roundabout) >= 1;
			case UpgradeType.Tunnel:
				return this._upgradeDatabase.GetUsedUpgradeCount(UpgradeType.Tunnel) >= 1;
			default:
				return false;
			}
		}

		// Token: 0x06001D73 RID: 7539 RVA: 0x00077E03 File Offset: 0x00076003
		private bool HasPlacedMotorway()
		{
			return this._simulation.GetModel<MotorwayModel>() != null;
		}

		// Token: 0x06001D74 RID: 7540 RVA: 0x00077E14 File Offset: 0x00076014
		private bool IsMotorwayConnectedToRoads()
		{
			MotorwayModel motorway = this._simulation.GetModel<MotorwayModel>();
			return motorway != null && motorway.StartTile.Tile.GetTwoLaneRoadCount(RoadState.Active, Tile.MotorwayInclusion.Ignore) >= 1 && motorway.EndTile.Tile.GetTwoLaneRoadCount(RoadState.Active, Tile.MotorwayInclusion.Ignore) >= 1;
		}

		// Token: 0x06001D75 RID: 7541 RVA: 0x00077E5F File Offset: 0x0007605F
		private bool HasPlacedTrafficLight()
		{
			return this._simulation.GetModel<TrafficLightModel>() != null;
		}

		// Token: 0x06001D76 RID: 7542 RVA: 0x00077E6F File Offset: 0x0007606F
		public void SetControllerIsDrawingRoads(bool isDrawingRoad)
		{
			this._controllerIsDrawingRoads = isDrawingRoad;
		}

		// Token: 0x06001D77 RID: 7543 RVA: 0x00077E78 File Offset: 0x00076078
		public void SetCurrentControllerCursor(Vector2Int position)
		{
			this._currentControllerPosition = position;
		}

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x06001D78 RID: 7544 RVA: 0x00077E81 File Offset: 0x00076081
		public bool HasVisibleMessage
		{
			get
			{
				return this.currentMessage != null;
			}
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x00077E8C File Offset: 0x0007608C
		public void ClearCurrentMessageIf(StringId stringId)
		{
			if (this.HasVisibleMessage && this.currentMessage.Message == stringId)
			{
				this.ClearCurrentMessage();
			}
		}

		// Token: 0x06001D7A RID: 7546 RVA: 0x00077EAC File Offset: 0x000760AC
		public void ClearCurrentMessage()
		{
			if (this.HasVisibleMessage)
			{
				this._simulation.RemoveModel(this.currentMessage);
				this.currentMessage = null;
				for (int animatorIndex = 0; animatorIndex < this._animatorViews.Count; animatorIndex++)
				{
					IndicatorAnimationView indicator = this._animatorViews[animatorIndex];
					IndicatorAnimationView.AnimationType animation = indicator.Animation;
					if (animation == IndicatorAnimationView.AnimationType.Highlight || animation == IndicatorAnimationView.AnimationType.Tap || animation == IndicatorAnimationView.AnimationType.Drag)
					{
						indicator.OnAnimationRelease();
						this._viewClient.MarkViewForRemoval(indicator);
						this._animatorViews.RemoveAt(animatorIndex);
						animatorIndex--;
					}
				}
			}
			this._nextMessage = null;
		}

		// Token: 0x06001D7B RID: 7547 RVA: 0x00077F3D File Offset: 0x0007613D
		public void TemporarilyHideMessage()
		{
			if (this.HasVisibleMessage)
			{
				this._simulation.RemoveModel(this.currentMessage);
				this.currentMessage = null;
			}
		}

		// Token: 0x06001D7C RID: 7548 RVA: 0x00077F60 File Offset: 0x00076160
		private void SetNextMessageAnchoredToScreen(StringId messageString, Vector2 screenOffset, CameraLayer cameraLayer = CameraLayer.Default, bool force = false, int? intParameter = null)
		{
			this._nextMessage = new TutorialProgressionProcess.MessageData?(new TutorialProgressionProcess.MessageData(messageString, screenOffset, cameraLayer, force, intParameter));
		}

		// Token: 0x06001D7D RID: 7549 RVA: 0x00077F80 File Offset: 0x00076180
		private void AddMessageAnchoredToScreen(StringId messageString, Vector2 screenOffset, CameraLayer cameraLayer, int? intParameter)
		{
			if (!this.HasVisibleMessage)
			{
				AnchoredMessageModel message = this._simulation.Scope.Get<AnchoredMessageModel>();
				message.InitializeWithScreenAnchor(messageString, screenOffset, cameraLayer, intParameter);
				if (this._playerActionController.TutorialBlockInputFlag)
				{
					message.ShowDismissArrow = true;
				}
				this._simulation.AddModel(message);
				this.currentMessage = message;
			}
		}

		// Token: 0x06001D7E RID: 7550 RVA: 0x00077FD9 File Offset: 0x000761D9
		private void SetNextMessageAnchoredToWorld(StringId messageString, Vector3 position, TileDirection direction = TileDirection.North, bool force = false)
		{
			this._nextMessage = new TutorialProgressionProcess.MessageData?(new TutorialProgressionProcess.MessageData(messageString, position, direction, force));
		}

		// Token: 0x06001D7F RID: 7551 RVA: 0x00077FF0 File Offset: 0x000761F0
		private void AddMessageAnchoredToWorld(StringId messageString, Vector3 position, TileDirection direction = TileDirection.North)
		{
			if (!this.HasVisibleMessage)
			{
				AnchoredMessageModel message = this._simulation.Scope.Get<AnchoredMessageModel>();
				message.InitializeWithWorldAnchor(messageString, position, direction);
				if (this._playerActionController.TutorialBlockInputFlag)
				{
					message.ShowDismissArrow = true;
				}
				this._simulation.AddModel(message);
				this.currentMessage = message;
			}
		}

		// Token: 0x06001D80 RID: 7552 RVA: 0x00078048 File Offset: 0x00076248
		private void SetNextMessageAnchoredToUI(StringId messageString, UIMessageAnchor uiMessageAnchor, Vector2? offsetParam = null)
		{
			Vector2 offset = offsetParam ?? new Vector2(0.5f, 0.5f);
			this._nextMessage = new TutorialProgressionProcess.MessageData?(new TutorialProgressionProcess.MessageData(messageString, uiMessageAnchor, offset));
		}

		// Token: 0x06001D81 RID: 7553 RVA: 0x0007808C File Offset: 0x0007628C
		private void AddMessageAnchoredToUI(StringId messageString, UIMessageAnchor uiMessageAnchor, Vector2? offsetParam = null)
		{
			Vector2 offset = offsetParam ?? new Vector2(0.5f, 0.5f);
			if (!this.HasVisibleMessage)
			{
				AnchoredMessageModel message = this._simulation.Scope.Get<AnchoredMessageModel>();
				message.InitializeWithUIAnchor(messageString, uiMessageAnchor, offset);
				if (this._playerActionController.TutorialBlockInputFlag)
				{
					message.ShowDismissArrow = true;
				}
				this._simulation.AddModel(message);
				this.currentMessage = message;
			}
		}

		// Token: 0x06001D82 RID: 7554 RVA: 0x00078108 File Offset: 0x00076308
		private void AddDemandToDestination(TutorialIdentifier identifier, int amount)
		{
			DestinationModel destination = this.GetDestinationById(identifier);
			if (destination != null)
			{
				for (int count = 0; count < amount; count++)
				{
					destination.unassignedDemand.Add(destination.GroupIndex);
				}
			}
		}

		// Token: 0x06001D83 RID: 7555 RVA: 0x00078140 File Offset: 0x00076340
		private void SetTotalDemandOnDestination(TutorialIdentifier identifier, int amount)
		{
			DestinationModel destination = this.GetDestinationById(identifier);
			if (destination != null)
			{
				int demandToAdd = Math.Max(amount - destination.TotalDemand, 0);
				for (int count = 0; count < demandToAdd; count++)
				{
					destination.unassignedDemand.Add(destination.GroupIndex);
				}
			}
		}

		// Token: 0x06001D84 RID: 7556 RVA: 0x00078184 File Offset: 0x00076384
		private void AddDemandToAllDestinations(int amount)
		{
			foreach (DestinationModel destination in this._simulation.GetModels<DestinationModel>())
			{
				for (int count = 0; count < amount; count++)
				{
					destination.unassignedDemand.Add(destination.GroupIndex);
				}
			}
		}

		// Token: 0x06001D85 RID: 7557 RVA: 0x000781D5 File Offset: 0x000763D5
		private void TouchEnterDrawModeIndicator(Fix64 timestep)
		{
			this.DoTapIndicator(new Vector3(-22f, -4f), timestep);
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x000781F0 File Offset: 0x000763F0
		private void SetNextUpgrades(UpgradeType mainOption, int mainConcrete, UpgradeType alternateOption, int alternateConcrete, bool alternateOptionDisabled = false)
		{
			this._upgradeDatabase.AddPendingUpgradeChoice(new UpgradeChoice
			{
				choices = 
				{
					new UpgradePackageDefinition
					{
						type = mainOption,
						amount = 1,
						additionalConcrete = mainConcrete
					},
					new UpgradePackageDefinition
					{
						type = alternateOption,
						amount = 1,
						additionalConcrete = alternateConcrete
					}
				},
				disabledOptions = (alternateOptionDisabled ? DisabledUpgradeOptions.Option2 : DisabledUpgradeOptions.None)
			});
		}

		// Token: 0x06001D87 RID: 7559 RVA: 0x00078274 File Offset: 0x00076474
		private IndicatorAnimationView AddDragIndicator(Vector3 start, Vector3 end)
		{
			IndicatorAnimationView animation = this._scope.Get<IndicatorAnimationView>();
			animation.Initialize(IndicatorAnimationView.AnimationType.Drag, start, new Vector3?(end));
			this._viewClient.AddView(animation);
			this._animatorViews.Add(animation);
			return animation;
		}

		// Token: 0x06001D88 RID: 7560 RVA: 0x000782B4 File Offset: 0x000764B4
		private void DoTapIndicator(Vector3 position, Fix64 timestep)
		{
			if (this._tapIndexTimer <= Fix64.Zero)
			{
				if (this._idleTapAnimationView != null)
				{
					this._idleTapAnimationView.OnAnimationRelease();
					this._viewClient.MarkViewForRemoval(this._idleTapAnimationView);
					this._animatorViews.Remove(this._idleTapAnimationView);
				}
				this._idleTapAnimationView = this._simulation.Scope.Get<IndicatorAnimationView>();
				this._idleTapAnimationView.Initialize(IndicatorAnimationView.AnimationType.Tap, position, null);
				this._viewClient.AddView(this._idleTapAnimationView);
				this._animatorViews.Add(this._idleTapAnimationView);
				this._tapIndexTimer = Fix64Consts.One;
				return;
			}
			this._tapIndexTimer -= timestep;
		}

		// Token: 0x06001D89 RID: 7561 RVA: 0x00078380 File Offset: 0x00076580
		private IndicatorAnimationView AddHighlightPositionIndicator(Vector3 position)
		{
			IndicatorAnimationView animation = this._simulation.Scope.Get<IndicatorAnimationView>();
			animation.Initialize(IndicatorAnimationView.AnimationType.Highlight, position, null);
			this._viewClient.AddView(animation);
			this._animatorViews.Add(animation);
			return animation;
		}

		// Token: 0x06001D8A RID: 7562 RVA: 0x000783C8 File Offset: 0x000765C8
		private void DragIndicatorBetween(HouseModel houseModel, DestinationModel destinationModel, Fix64 timestep)
		{
			this.DragIndicatorBetween(this.GetHouseDrivewayPosition(houseModel), this.GetDestinationDrivewayPosition(destinationModel), timestep);
		}

		// Token: 0x06001D8B RID: 7563 RVA: 0x000783E0 File Offset: 0x000765E0
		private void DragIndicatorBetween(Vector3 start, Vector3 end, Fix64 timestep)
		{
			if (this._dragIndicatorTimer <= Fix64.Zero)
			{
				IndicatorAnimationView animation = this.AddDragIndicator(start, end);
				this._dragIndicatorTimer = animation.Duration;
				return;
			}
			this._dragIndicatorTimer -= timestep;
		}

		// Token: 0x06001D8C RID: 7564 RVA: 0x00078427 File Offset: 0x00076627
		private Vector3 GetHouseDrivewayPosition(HouseModel houseModel)
		{
			return (houseModel.tileModel.Coordinates + TileUtilities.GetVectorForDirection(houseModel.DrivewayLane.connection.output.direction)) * 2f;
		}

		// Token: 0x06001D8D RID: 7565 RVA: 0x00078468 File Offset: 0x00076668
		private Vector3 GetFirstDestinationPinPosition()
		{
			DestinationModel destination = this.GetDestinationById(TutorialIdentifier.FirstDestination);
			DestinationView destinationView = this._simulation.Scope.Get<ViewIndex>().GetDestinationView(destination);
			if (Diagnostics.Verify(destinationView != null))
			{
				return destinationView.GetPositionOfPin(0);
			}
			return Vector3.zero;
		}

		// Token: 0x06001D8E RID: 7566 RVA: 0x000784B0 File Offset: 0x000766B0
		private Vector3 GetHousePosition(TutorialIdentifier identifier)
		{
			ModelList<HouseModel> houses = this._simulation.GetModels<HouseModel>();
			for (int houseIndex = 0; houseIndex < houses.Count; houseIndex++)
			{
				if (houses[houseIndex].TutorialIdentifier == identifier)
				{
					return this.GetHousePosition(houseIndex);
				}
			}
			return Vector3.zero;
		}

		// Token: 0x06001D8F RID: 7567 RVA: 0x000784F8 File Offset: 0x000766F8
		private Vector3 GetHousePosition(int houseIndex)
		{
			ModelList<HouseModel> houses = this._simulation.GetModels<HouseModel>();
			houseIndex = Math.Min(houseIndex, houses.Count - 1);
			if (houseIndex < 0)
			{
				return new Vector3(0f, 0f);
			}
			return new Vector3((float)houses[houseIndex].tileModel.Coordinates.x, (float)houses[houseIndex].tileModel.Coordinates.y) * 2f + new Vector3(0.05f, 0.05f, 0f);
		}

		// Token: 0x06001D90 RID: 7568 RVA: 0x00078594 File Offset: 0x00076794
		private Vector3 GetDestinationDrivewayPosition(DestinationModel destination)
		{
			if (destination.Carpark.entranceAtBottomRight)
			{
				return (destination.Carpark.BottomRightDrivewayTileCoordinates * 2).ToVector3() + new Vector3(0.5f, 0.5f);
			}
			return (destination.Carpark.TopLeftDrivewayTileCoordinates * 2).ToVector3() + new Vector3(0.05f, 0.05f);
		}

		// Token: 0x06001D91 RID: 7569 RVA: 0x00078604 File Offset: 0x00076804
		private bool RequireHouseConnectedToDestinationWithSameGroup(TutorialIdentifier houseId)
		{
			HouseModel houseModel = this.GetHouseById(houseId);
			if (houseModel == null)
			{
				return false;
			}
			foreach (DestinationModel destination in this._simulation.GetModels<DestinationModel>())
			{
				if (destination.GroupIndex == houseModel.GroupIndex && this._pathfinder.CreatePath(houseModel.DrivewayLane, destination.Carpark.entranceLanes, true) != null)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D92 RID: 7570 RVA: 0x00078678 File Offset: 0x00076878
		private bool RequireHouseConnectedToDestination(TutorialIdentifier houseId, TutorialIdentifier destinationId)
		{
			HouseModel houseModel = this.GetHouseById(houseId);
			DestinationModel destinationModel = this.GetDestinationById(destinationId);
			return destinationModel != null && destinationModel.isActive && destinationModel.Carpark != null && destinationModel.Carpark.entranceLanes.Count != 0 && Diagnostics.Verify(houseModel.DrivewayLane != null, "HouseModel should always have a driveway.") && this._pathfinder.AreLanesConnected(houseModel.DrivewayLane, destinationModel.Carpark.entranceLanes, true);
		}

		// Token: 0x06001D93 RID: 7571 RVA: 0x000786F0 File Offset: 0x000768F0
		private bool RequireAllHousesAndDestinationsInGroupToBeConnected(int groupIndex)
		{
			bool foundAtLeastOneDestinationInGroup = false;
			bool foundAtLeastOneHouseInGroup = false;
			foreach (DestinationModel destinationModel in this._simulation.GetModels<DestinationModel>())
			{
				if (destinationModel.isActive && destinationModel.Carpark != null && destinationModel.Carpark.entranceLanes.Count != 0 && destinationModel.GroupIndex == groupIndex)
				{
					foundAtLeastOneDestinationInGroup = true;
					foreach (HouseModel houseModel in this._simulation.GetModels<HouseModel>())
					{
						if (houseModel.GroupIndex == groupIndex)
						{
							foundAtLeastOneHouseInGroup = true;
							if (Diagnostics.Verify(houseModel.DrivewayLane != null, "HouseModel should always have a driveway.") && !this._pathfinder.AreLanesConnected(houseModel.DrivewayLane, destinationModel.Carpark.entranceLanes, true))
							{
								return false;
							}
						}
					}
				}
			}
			return foundAtLeastOneDestinationInGroup && foundAtLeastOneHouseInGroup;
		}

		// Token: 0x06001D94 RID: 7572 RVA: 0x000787D8 File Offset: 0x000769D8
		private bool RoadCountIs(int requiredRoadCount)
		{
			return this.GetRoadCount() == requiredRoadCount;
		}

		// Token: 0x06001D95 RID: 7573 RVA: 0x000787E3 File Offset: 0x000769E3
		private bool RoadCountGreaterThanOrEqualTo(int requiredRoadCount)
		{
			return this.GetRoadCount() >= requiredRoadCount;
		}

		// Token: 0x06001D96 RID: 7574 RVA: 0x000787F4 File Offset: 0x000769F4
		private bool HouseHasSpawned(TutorialIdentifier identifier)
		{
			ModelListEnumerator<HouseModel> enumerator = this._simulation.GetModels<HouseModel>().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.TutorialIdentifier == identifier)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D97 RID: 7575 RVA: 0x00078834 File Offset: 0x00076A34
		private bool HouseHasSpawnedAndHasDestinationToTravelTo(TutorialIdentifier tutorialIdentifier)
		{
			HouseModel houseModel = this.GetHouseById(tutorialIdentifier);
			if (houseModel == null)
			{
				return false;
			}
			ModelListEnumerator<DestinationModel> enumerator = this._simulation.GetModels<DestinationModel>().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.GroupIndex == houseModel.GroupIndex)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D98 RID: 7576 RVA: 0x00078884 File Offset: 0x00076A84
		private bool DestinationHasSpawned(TutorialIdentifier identifier)
		{
			ModelListEnumerator<DestinationModel> enumerator = this._simulation.GetModels<DestinationModel>().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.TutorialIdentifier == identifier)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D99 RID: 7577 RVA: 0x000788C2 File Offset: 0x00076AC2
		public int GetGeneratedDemandLimitForDestination(TutorialIdentifier identifier)
		{
			if (!this._demandLimits.ContainsKey(identifier))
			{
				return -1;
			}
			return this._demandLimits[identifier];
		}

		// Token: 0x06001D9A RID: 7578 RVA: 0x000788E0 File Offset: 0x00076AE0
		private void RemoveAllPerDestinationDemandLimits()
		{
			this._demandLimits.Clear();
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x000788ED File Offset: 0x00076AED
		private void LimitGeneratedDemandForDestination(TutorialIdentifier identifier, int maxDemand)
		{
			if (Diagnostics.Verify(maxDemand >= 0, "Demand limit for destination must be >= 0"))
			{
				if (this._demandLimits.ContainsKey(identifier))
				{
					this._demandLimits[identifier] = maxDemand;
					return;
				}
				this._demandLimits.Add(identifier, maxDemand);
			}
		}

		// Token: 0x06001D9C RID: 7580 RVA: 0x0007892B File Offset: 0x00076B2B
		private void RemoveMaximumGeneratedDemandLimitForDestination(TutorialIdentifier identifier)
		{
			if (this._demandLimits.ContainsKey(identifier))
			{
				this._demandLimits.Remove(identifier);
			}
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x00078948 File Offset: 0x00076B48
		private DestinationModel GetDestinationById(TutorialIdentifier identifier)
		{
			foreach (DestinationModel destination in this._simulation.GetModels<DestinationModel>())
			{
				if (destination.TutorialIdentifier == identifier)
				{
					return destination;
				}
			}
			Diagnostics.FailAssert(string.Format("Could not find destination with tutorial identifier: {0}", identifier), Array.Empty<object>());
			return null;
		}

		// Token: 0x06001D9E RID: 7582 RVA: 0x000789A2 File Offset: 0x00076BA2
		private bool DestinationDemandEquals(TutorialIdentifier identifier, int demand)
		{
			DestinationModel destinationById = this.GetDestinationById(identifier);
			return destinationById != null && destinationById.TotalDemand == demand;
		}

		// Token: 0x06001D9F RID: 7583 RVA: 0x000789C0 File Offset: 0x00076BC0
		private HouseModel GetHouseById(TutorialIdentifier identifier)
		{
			foreach (HouseModel house in this._simulation.GetModels<HouseModel>())
			{
				if (house.TutorialIdentifier == identifier)
				{
					return house;
				}
			}
			return null;
		}

		// Token: 0x06001DA0 RID: 7584 RVA: 0x00078A00 File Offset: 0x00076C00
		public bool RequireTimePassed(float seconds)
		{
			return this._timeSpentInStep >= (Fix64)seconds;
		}

		// Token: 0x06001DA1 RID: 7585 RVA: 0x00078A13 File Offset: 0x00076C13
		public void StartRealtimeTimer(float seconds)
		{
			this._unscaledMessageTimer = seconds;
		}

		// Token: 0x06001DA2 RID: 7586 RVA: 0x00078A1C File Offset: 0x00076C1C
		public bool RealtimeTimerFinished()
		{
			return this._unscaledMessageTimer <= 0f;
		}

		// Token: 0x06001DA3 RID: 7587 RVA: 0x00078A2E File Offset: 0x00076C2E
		private bool RequireExactUpgradeCount(UpgradeType upgrade, int numRequired)
		{
			return this._upgradeDatabase.GetAvailableUpgradeCount(upgrade) == numRequired;
		}

		// Token: 0x06001DA4 RID: 7588 RVA: 0x00078A40 File Offset: 0x00076C40
		private bool HasActiveAssetDragAction(GameUIButtonType upgrade)
		{
			foreach (PlayerActionGroup playerActionGroup in this._playerActionController.ActiveGroups)
			{
				MotorwaysUIInputEvent uiInputEvent = playerActionGroup.InstigatingInputEvent as MotorwaysUIInputEvent;
				if (uiInputEvent != null && uiInputEvent.UIButtonType == upgrade)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001DA5 RID: 7589 RVA: 0x00078AA8 File Offset: 0x00076CA8
		private bool HadInputAndMessageSpentMinimumTime()
		{
			return this.hadInput && this._unscaledMessageTimer <= 0f;
		}

		// Token: 0x06001DA6 RID: 7590 RVA: 0x00078AC4 File Offset: 0x00076CC4
		private void RestorePlayerControl()
		{
			this._playerActionController.TutorialBlockInputFlag = false;
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x00078AD2 File Offset: 0x00076CD2
		private void PrepareForDismissibleMessage()
		{
			this._playerActionController.TutorialBlockInputFlag = true;
			if (this._skipTimeForDismissibleMessages)
			{
				this._unscaledMessageTimer = 0f;
				return;
			}
			this._unscaledMessageTimer = 2f;
		}

		// Token: 0x06001DA8 RID: 7592 RVA: 0x00078AFF File Offset: 0x00076CFF
		public static TutorialProgressionProcess.TutorialType TutorialTypeForInputType(DeviceInputType inputType)
		{
			switch (inputType)
			{
			case DeviceInputType.Touch:
				return TutorialProgressionProcess.TutorialType.Mobile;
			case DeviceInputType.Mouse:
				return TutorialProgressionProcess.TutorialType.Desktop;
			case DeviceInputType.Remote:
			case DeviceInputType.Controller:
				return TutorialProgressionProcess.TutorialType.TV;
			default:
				return TutorialProgressionProcess.TutorialType.None;
			}
		}

		// Token: 0x06001DA9 RID: 7593 RVA: 0x00078B20 File Offset: 0x00076D20
		public void OnReleasedFromScope(IScope scope)
		{
			this._playerActionController.TutorialBlockInputFlag = false;
			this.UnregisterActions();
			if (this._gameUI != null)
			{
				this._gameUI.SetDrawButtonsHiddenByTutorial(false);
				this._gameUI.SetTileHighlightsAllowed(true);
			}
			this._inputState.Unsubscribe(this);
		}

		// Token: 0x06001DAA RID: 7594 RVA: 0x00078B74 File Offset: 0x00076D74
		private void RegisterActions()
		{
			IScope scopeToRegisterActions = this._scope.ParentScope ?? this._scope;
			this._playerActionController.RegisterAction(InputEventFilter.CreateMouseEventFilter(19, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(AdvanceTutorialAction.Create), scopeToRegisterActions, true);
			this._playerActionController.RegisterAction(InputEventFilter.CreateMouseEventFilter(20, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(AdvanceTutorialAction.Create), scopeToRegisterActions, true);
			this._playerActionController.RegisterAction(InputEventFilter.CreateTouchEventFilter(0, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(AdvanceTutorialAction.Create), scopeToRegisterActions, true);
			this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(2, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(AdvanceTutorialAction.Create), scopeToRegisterActions, true);
			this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(16, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(AdvanceTutorialAction.Create), scopeToRegisterActions, true);
			this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(17, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(AdvanceTutorialAction.Create), scopeToRegisterActions, true);
			this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(18, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(AdvanceTutorialAction.Create), scopeToRegisterActions, true);
			this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(2, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(AdvanceTutorialAction.Create), scopeToRegisterActions, true);
			this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(16, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(AdvanceTutorialAction.Create), scopeToRegisterActions, true);
			this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(2, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(AdvanceTutorialAction.Create), scopeToRegisterActions, true);
		}

		// Token: 0x06001DAB RID: 7595 RVA: 0x00078CE7 File Offset: 0x00076EE7
		public void UnregisterActions()
		{
			this._playerActionController.UnregisterAction<AdvanceTutorialAction>(null);
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x00078CF5 File Offset: 0x00076EF5
		public void SkipTutorial()
		{
			this._currentStepIndex = this._tutorial.Steps.Count;
			this.UnregisterActions();
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x00078D14 File Offset: 0x00076F14
		public void OnCurrentDeviceInputTypeChanged(DeviceInputType newInputType)
		{
			if (this._currentStepIndex >= this._tutorial.Steps.Count)
			{
				return;
			}
			this.UnregisterActions();
			int oldAmountOfStages = this._tutorial.Steps.Count;
			this.Initialize();
			if (this.LastReachedMarker >= TutorialProgressionProcess.TutorialMarker.InputControlsTaught)
			{
				this._currentStepIndex += this._tutorial.Steps.Count - oldAmountOfStages;
			}
			else
			{
				this.ClearCurrentMessage();
				this._currentStepIndex = 0;
			}
			this._simulation.IsPaused = false;
			this.SetAllInputBlocked(false);
		}

		// Token: 0x06001DAE RID: 7598 RVA: 0x00078DA4 File Offset: 0x00076FA4
		public TutorialProgressionProcess.TutorialStep StageAt(int index)
		{
			if (this._tutorial.Steps == null || this._tutorial.Steps.Count <= 0 || index >= this._tutorial.Steps.Count)
			{
				return null;
			}
			return this._tutorial.Steps[index];
		}

		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x06001DAF RID: 7599 RVA: 0x00078DF8 File Offset: 0x00076FF8
		public TutorialProgressionProcess.TutorialStep CurrentStep
		{
			get
			{
				if (this._tutorial.Steps == null || this._tutorial.Steps.Count <= 0 || this._currentStepIndex >= this._tutorial.Steps.Count)
				{
					return null;
				}
				return this._tutorial.Steps[this._currentStepIndex];
			}
		}

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x06001DB0 RID: 7600 RVA: 0x00078E55 File Offset: 0x00077055
		public int CurrentStepIndex
		{
			get
			{
				return this._currentStepIndex;
			}
		}

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x06001DB1 RID: 7601 RVA: 0x00078E5D File Offset: 0x0007705D
		public int StageCount
		{
			get
			{
				return this._tutorial.Steps.Count;
			}
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x06001DB2 RID: 7602 RVA: 0x00078E6F File Offset: 0x0007706F
		public bool IsInputBlocked
		{
			get
			{
				return this._playerActionController.TutorialBlockInputFlag;
			}
		}

		// Token: 0x06001DB3 RID: 7603 RVA: 0x00078E7C File Offset: 0x0007707C
		private void SkipClockTo(Fix64 justBeforeFirstWeek)
		{
			this._clock.NextFrame.time = justBeforeFirstWeek;
			this._clock.NextFrame.expansionTime = justBeforeFirstWeek;
		}

		// Token: 0x06001DB4 RID: 7604 RVA: 0x00078EA0 File Offset: 0x000770A0
		private void CheckIfVehicleLeftRoundabout()
		{
			if (!this.HasPlacedUpgrade(UpgradeType.Roundabout))
			{
				return;
			}
			foreach (VehicleModel vehicleModel in this._simulation.GetModels<VehicleModel>())
			{
				if (!this._vehiclesOnRoundabout.Contains(vehicleModel))
				{
					RoadTileConnection currentConnection = vehicleModel.CurrentFrame.lane.connection;
					if (currentConnection.input.type == RoadType.Roundabout && currentConnection.output.type == RoadType.Roundabout)
					{
						this._vehiclesOnRoundabout.Add(vehicleModel);
					}
				}
			}
			for (int vehicleIndex = this._vehiclesOnRoundabout.Count - 1; vehicleIndex >= 0; vehicleIndex--)
			{
				if (this._vehiclesOnRoundabout[vehicleIndex].CurrentFrame.lane.connection.input.type != RoadType.Roundabout)
				{
					this._numberOfVehiclesThatHaveLeftARoundabout++;
					this._vehiclesOnRoundabout.RemoveAt(vehicleIndex);
				}
			}
		}

		// Token: 0x06001DB5 RID: 7605 RVA: 0x00078F88 File Offset: 0x00077188
		private void CheckIfVehicleLeftMotorway()
		{
			if (!this.HasPlacedMotorway())
			{
				return;
			}
			foreach (VehicleModel vehicleModel in this._simulation.GetModels<VehicleModel>())
			{
				if (!this._vehiclesOnMotorway.Contains(vehicleModel))
				{
					RoadTileConnection currentConnection = vehicleModel.CurrentFrame.lane.connection;
					if (currentConnection.input.type == RoadType.Motorway && currentConnection.output.type == RoadType.Motorway)
					{
						this._vehiclesOnMotorway.Add(vehicleModel);
					}
				}
			}
			for (int vehicleIndex = this._vehiclesOnMotorway.Count - 1; vehicleIndex >= 0; vehicleIndex--)
			{
				if (this._vehiclesOnMotorway[vehicleIndex].CurrentFrame.lane.connection.input.type != RoadType.Motorway)
				{
					this._numberOfVehiclesThatHaveLeftAMotorway++;
					this._vehiclesOnMotorway.RemoveAt(vehicleIndex);
				}
			}
		}

		// Token: 0x06001DB6 RID: 7606 RVA: 0x0007906C File Offset: 0x0007726C
		private bool TripsOnMotorwaysGreaterThanOrEqualTo(int tripCount)
		{
			return this._numberOfVehiclesThatHaveLeftAMotorway >= tripCount;
		}

		// Token: 0x06001DB7 RID: 7607 RVA: 0x0007907A File Offset: 0x0007727A
		private bool TripsOnRoundaboutGreaterThanOrEqualTo(int tripCount)
		{
			return this._numberOfVehiclesThatHaveLeftARoundabout >= tripCount;
		}

		// Token: 0x06001DB8 RID: 7608 RVA: 0x00079088 File Offset: 0x00077288
		public void SetDrawModeToggleVisibility(bool isVisible)
		{
			this._gameUI.SetDrawButtonsHiddenByTutorial(!isVisible);
			this._gameUI.SetDrawButtonsVisible(isVisible);
		}

		// Token: 0x06001DB9 RID: 7609 RVA: 0x000790A5 File Offset: 0x000772A5
		public void SetAllInputBlocked(bool blocked)
		{
			this._playerActionController.TutorialBlockInputFlag = blocked;
		}

		// Token: 0x06001DBA RID: 7610 RVA: 0x000790B4 File Offset: 0x000772B4
		public int GetRoadCount()
		{
			int twoLaneRoadCount = 0;
			foreach (TileModel tileModel in this._simulation.GetModels<TileModel>())
			{
				twoLaneRoadCount += tileModel.Tile.GetTwoLaneRoadCount(RoadState.Active, Tile.MotorwayInclusion.Ignore);
			}
			if (twoLaneRoadCount > 0)
			{
				return twoLaneRoadCount / 2;
			}
			return 0;
		}

		// Token: 0x06001DBB RID: 7611 RVA: 0x00079103 File Offset: 0x00077303
		public bool UpgradeScreenIsVisible()
		{
			return this._screenStack.GetTopActiveScreenType() == ScreenStack.MotorwaysScreen.Upgrade;
		}

		// Token: 0x06001E4F RID: 7759 RVA: 0x0007A0AF File Offset: 0x000782AF
		[CompilerGenerated]
		private StringId <AddControllerSteps_LearnBasics>g__GetDrawString|21_0()
		{
			if (!this._player.IsTapDrawEnabled)
			{
				return StringId.Tutorial_ConnectRoad_Controller;
			}
			return StringId.Tutorial_ConnectRoad_ControllerTap;
		}

		// Token: 0x0400191E RID: 6430
		private int _roadCountAfterDrawStep;

		// Token: 0x0400191F RID: 6431
		private int _roadCountBeforeWaitUntilDeleteModeEnabled;

		// Token: 0x04001920 RID: 6432
		private int _concreteCountAtStartOfTutorial;

		// Token: 0x04001921 RID: 6433
		private Fix64 _drawRoadHintAnimationTimer = Fix64.Zero;

		// Token: 0x04001922 RID: 6434
		public const int TutorialEndWeek = 6;

		// Token: 0x04001923 RID: 6435
		private const float MinimumTimeForDismissibleMessages = 2f;

		// Token: 0x04001924 RID: 6436
		private int _currentStepIndex;

		// Token: 0x04001928 RID: 6440
		[Dependency]
		private IScope _scope;

		// Token: 0x04001929 RID: 6441
		[Dependency]
		private TutorialConstantsData _tutorialConstants;

		// Token: 0x0400192A RID: 6442
		[Dependency]
		private City _city;

		// Token: 0x0400192B RID: 6443
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x0400192C RID: 6444
		[Dependency]
		private ClockModel _clock;

		// Token: 0x0400192D RID: 6445
		[Dependency]
		private ScoreModel _score;

		// Token: 0x0400192E RID: 6446
		[Dependency]
		private Pathfinder _pathfinder;

		// Token: 0x0400192F RID: 6447
		[Dependency]
		private InputState _inputState;

		// Token: 0x04001930 RID: 6448
		[Dependency]
		private UpgradeDatabaseModel _upgradeDatabase;

		// Token: 0x04001931 RID: 6449
		[Dependency]
		private PopupStack _popups;

		// Token: 0x04001932 RID: 6450
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04001933 RID: 6451
		[Dependency]
		private PlayerActionController _playerActionController;

		// Token: 0x04001934 RID: 6452
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04001935 RID: 6453
		[Dependency]
		private ScreenStack _screenStack;

		// Token: 0x04001936 RID: 6454
		[Dependency]
		private AnalyticsTracker _analytics;

		// Token: 0x04001937 RID: 6455
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x04001938 RID: 6456
		[Serialize(false, null)]
		private GameUIScreen _gameUI;

		// Token: 0x04001939 RID: 6457
		[Serialize(false, null)]
		private CameraView _camera;

		// Token: 0x0400193A RID: 6458
		public bool hadInput;

		// Token: 0x0400193B RID: 6459
		private IndicatorAnimationView _idleTapAnimationView;

		// Token: 0x0400193C RID: 6460
		private bool _isInTutorial;

		// Token: 0x0400193D RID: 6461
		private int _scoreToFinishTutorial;

		// Token: 0x0400193E RID: 6462
		[Serialize(false, null)]
		private GameRules _rules;

		// Token: 0x0400193F RID: 6463
		private Fix64 _clockSpeedMultiplier;

		// Token: 0x04001940 RID: 6464
		private bool _isProgressing;

		// Token: 0x04001941 RID: 6465
		private ConfirmationPopup _currentPopup;

		// Token: 0x04001942 RID: 6466
		private bool _hasShownAlternateDrawModeTogglePopup;

		// Token: 0x04001943 RID: 6467
		private Fix64 _timeSpentInStep = Fix64.Zero;

		// Token: 0x04001944 RID: 6468
		private Fix64 _timeSpentNotProgressing = Fix64.Zero;

		// Token: 0x04001945 RID: 6469
		private float _unscaledMessageTimer;

		// Token: 0x04001946 RID: 6470
		private Vector2Int _currentControllerPosition;

		// Token: 0x04001947 RID: 6471
		private bool _controllerIsDrawingRoads;

		// Token: 0x04001948 RID: 6472
		private TutorialBuilder _tutorial;

		// Token: 0x04001949 RID: 6473
		[Serialize(false, null)]
		private readonly List<IndicatorAnimationView> _animatorViews = new List<IndicatorAnimationView>();

		// Token: 0x0400194A RID: 6474
		[Serialize(false, null)]
		private AnchoredMessageModel currentMessage;

		// Token: 0x0400194B RID: 6475
		[Serialize(false, null)]
		private TutorialProgressionProcess.MessageData? _nextMessage;

		// Token: 0x0400194C RID: 6476
		private static readonly Fix64 ClockDecelerationMultiplier = (Fix64)5L;

		// Token: 0x0400194D RID: 6477
		private static readonly Fix64 ClockAccelerationMultiplier = (Fix64)1L;

		// Token: 0x0400194E RID: 6478
		private static readonly Fix64 DelayBeforeIdleAnimation = (Fix64)10f;

		// Token: 0x0400194F RID: 6479
		private static readonly Fix64 DelayBeforeIdleMessage = (Fix64)0.5f;

		// Token: 0x04001950 RID: 6480
		private bool _skipTimeForDismissibleMessages;

		// Token: 0x04001952 RID: 6482
		private int _numberOfVehiclesThatHaveLeftAMotorway;

		// Token: 0x04001953 RID: 6483
		private readonly List<VehicleModel> _vehiclesOnMotorway = new List<VehicleModel>();

		// Token: 0x04001954 RID: 6484
		private int _numberOfVehiclesThatHaveLeftARoundabout;

		// Token: 0x04001955 RID: 6485
		private readonly List<VehicleModel> _vehiclesOnRoundabout = new List<VehicleModel>();

		// Token: 0x04001956 RID: 6486
		public const int NoDemandLimit = -1;

		// Token: 0x04001957 RID: 6487
		private readonly Dictionary<TutorialIdentifier, int> _demandLimits = new Dictionary<TutorialIdentifier, int>();

		// Token: 0x04001959 RID: 6489
		private bool _enteredDeleteMode = true;

		// Token: 0x0400195A RID: 6490
		private bool _exitedDeleteMode = true;

		// Token: 0x0400195B RID: 6491
		private RoadDrawMode _previousRoadDrawMode;

		// Token: 0x0400195C RID: 6492
		private IdleHint _connectHousesIdleMessage;

		// Token: 0x0400195D RID: 6493
		private bool _connectHouseIdleMessageHasBeenDismissed;

		// Token: 0x0400195E RID: 6494
		private Fix64 _tapIndexTimer = Fix64.Zero;

		// Token: 0x0400195F RID: 6495
		private Fix64 _dragIndicatorTimer = Fix64.Zero;

		// Token: 0x020004A1 RID: 1185
		[Flags]
		public enum TutorialType
		{
			// Token: 0x04001961 RID: 6497
			None = 0,
			// Token: 0x04001962 RID: 6498
			Mobile = 1,
			// Token: 0x04001963 RID: 6499
			Desktop = 4,
			// Token: 0x04001964 RID: 6500
			TV = 8
		}

		// Token: 0x020004A2 RID: 1186
		private struct MessageData
		{
			// Token: 0x06001E7A RID: 7802 RVA: 0x0007A365 File Offset: 0x00078565
			public MessageData(StringId messageString, Vector3 offset, CameraLayer cameraLayer = CameraLayer.Default, bool force = false, int? intParameter = null)
			{
				this.messageString = messageString;
				this.position = offset;
				this.direction = TileDirection.None;
				this.force = force;
				this._anchorType = TutorialProgressionProcess.MessageData.AnchorType.Screen;
				this.uiAnchor = UIMessageAnchor.None;
				this.cameraLayer = cameraLayer;
				this.intParameter = intParameter;
			}

			// Token: 0x06001E7B RID: 7803 RVA: 0x0007A3A1 File Offset: 0x000785A1
			public MessageData(StringId messageString, Vector3 position, TileDirection direction, bool force = false)
			{
				this.messageString = messageString;
				this.position = position;
				this.direction = direction;
				this.force = force;
				this._anchorType = TutorialProgressionProcess.MessageData.AnchorType.World;
				this.uiAnchor = UIMessageAnchor.None;
				this.cameraLayer = CameraLayer.Default;
				this.intParameter = null;
			}

			// Token: 0x06001E7C RID: 7804 RVA: 0x0007A3E4 File Offset: 0x000785E4
			public MessageData(StringId messageString, UIMessageAnchor uiMessageAnchor, Vector2 offset)
			{
				this.messageString = messageString;
				this.uiAnchor = uiMessageAnchor;
				this.position = offset;
				this.direction = TileDirection.None;
				this.force = false;
				this._anchorType = TutorialProgressionProcess.MessageData.AnchorType.UI;
				this.cameraLayer = CameraLayer.Default;
				this.intParameter = null;
			}

			// Token: 0x1700057D RID: 1405
			// (get) Token: 0x06001E7D RID: 7805 RVA: 0x0007A433 File Offset: 0x00078633
			public bool IsScreenAnchored
			{
				get
				{
					return this._anchorType == TutorialProgressionProcess.MessageData.AnchorType.Screen;
				}
			}

			// Token: 0x1700057E RID: 1406
			// (get) Token: 0x06001E7E RID: 7806 RVA: 0x0007A43E File Offset: 0x0007863E
			public bool IsWorldAnchored
			{
				get
				{
					return this._anchorType == TutorialProgressionProcess.MessageData.AnchorType.World;
				}
			}

			// Token: 0x1700057F RID: 1407
			// (get) Token: 0x06001E7F RID: 7807 RVA: 0x0007A449 File Offset: 0x00078649
			public bool IsUIAnchored
			{
				get
				{
					return this._anchorType == TutorialProgressionProcess.MessageData.AnchorType.UI;
				}
			}

			// Token: 0x04001965 RID: 6501
			private TutorialProgressionProcess.MessageData.AnchorType _anchorType;

			// Token: 0x04001966 RID: 6502
			public StringId messageString;

			// Token: 0x04001967 RID: 6503
			public int? intParameter;

			// Token: 0x04001968 RID: 6504
			public Vector3 position;

			// Token: 0x04001969 RID: 6505
			public TileDirection direction;

			// Token: 0x0400196A RID: 6506
			public bool force;

			// Token: 0x0400196B RID: 6507
			public UIMessageAnchor uiAnchor;

			// Token: 0x0400196C RID: 6508
			public CameraLayer cameraLayer;

			// Token: 0x020004A3 RID: 1187
			private enum AnchorType
			{
				// Token: 0x0400196E RID: 6510
				Screen,
				// Token: 0x0400196F RID: 6511
				World,
				// Token: 0x04001970 RID: 6512
				UI
			}
		}

		// Token: 0x020004A4 RID: 1188
		public enum TutorialMarker
		{
			// Token: 0x04001972 RID: 6514
			InitialMarker,
			// Token: 0x04001973 RID: 6515
			InputControlsTaught,
			// Token: 0x04001974 RID: 6516
			BasicsLearnt,
			// Token: 0x04001975 RID: 6517
			DemandCollectedFromNewHouseColor,
			// Token: 0x04001976 RID: 6518
			BeganBridgeStage,
			// Token: 0x04001977 RID: 6519
			BeganTrafficLightStage,
			// Token: 0x04001978 RID: 6520
			BeganRoundaboutStage,
			// Token: 0x04001979 RID: 6521
			BeganMotorwayStage,
			// Token: 0x0400197A RID: 6522
			BeganBigPinStage,
			// Token: 0x0400197B RID: 6523
			BeganUpgradeChoiceStage,
			// Token: 0x0400197C RID: 6524
			BigPinsAllowed
		}

		// Token: 0x020004A5 RID: 1189
		public class TutorialStep
		{
			// Token: 0x17000580 RID: 1408
			// (get) Token: 0x06001E80 RID: 7808 RVA: 0x0007A454 File Offset: 0x00078654
			public string Id { get; }

			// Token: 0x17000581 RID: 1409
			// (get) Token: 0x06001E81 RID: 7809 RVA: 0x0007A45C File Offset: 0x0007865C
			public string Description { get; }

			// Token: 0x17000582 RID: 1410
			// (get) Token: 0x06001E82 RID: 7810 RVA: 0x0007A464 File Offset: 0x00078664
			// (set) Token: 0x06001E83 RID: 7811 RVA: 0x0007A46C File Offset: 0x0007866C
			public string StageShortName { get; set; }

			// Token: 0x17000583 RID: 1411
			// (get) Token: 0x06001E84 RID: 7812 RVA: 0x0007A475 File Offset: 0x00078675
			// (set) Token: 0x06001E85 RID: 7813 RVA: 0x0007A47D File Offset: 0x0007867D
			public Func<string> DebugText { get; private set; }

			// Token: 0x06001E86 RID: 7814 RVA: 0x0007A488 File Offset: 0x00078688
			public TutorialStep(string id, string description = null)
			{
				this.Id = id;
				this.Description = description;
			}

			// Token: 0x17000584 RID: 1412
			// (get) Token: 0x06001E87 RID: 7815 RVA: 0x0007A4D9 File Offset: 0x000786D9
			// (set) Token: 0x06001E88 RID: 7816 RVA: 0x0007A4E1 File Offset: 0x000786E1
			public Func<bool> DoesClockTick { get; private set; } = () => true;

			// Token: 0x17000585 RID: 1413
			// (get) Token: 0x06001E89 RID: 7817 RVA: 0x0007A4EA File Offset: 0x000786EA
			// (set) Token: 0x06001E8A RID: 7818 RVA: 0x0007A4F2 File Offset: 0x000786F2
			public Func<bool> IsStepOver { get; private set; }

			// Token: 0x17000586 RID: 1414
			// (get) Token: 0x06001E8B RID: 7819 RVA: 0x0007A4FB File Offset: 0x000786FB
			// (set) Token: 0x06001E8C RID: 7820 RVA: 0x0007A503 File Offset: 0x00078703
			public Action<Fix64> IdlePromptAnimationHandler { get; private set; }

			// Token: 0x17000587 RID: 1415
			// (get) Token: 0x06001E8D RID: 7821 RVA: 0x0007A50C File Offset: 0x0007870C
			public List<IdleHint> IdleHints { get; } = new List<IdleHint>();

			// Token: 0x17000588 RID: 1416
			// (get) Token: 0x06001E8E RID: 7822 RVA: 0x0007A514 File Offset: 0x00078714
			// (set) Token: 0x06001E8F RID: 7823 RVA: 0x0007A51C File Offset: 0x0007871C
			public Action<Fix64> IdleMessageAnimationHandler { get; private set; }

			// Token: 0x17000589 RID: 1417
			// (get) Token: 0x06001E90 RID: 7824 RVA: 0x0007A525 File Offset: 0x00078725
			// (set) Token: 0x06001E91 RID: 7825 RVA: 0x0007A52D File Offset: 0x0007872D
			public Func<bool> ShouldRegressStep { get; private set; }

			// Token: 0x1700058A RID: 1418
			// (get) Token: 0x06001E92 RID: 7826 RVA: 0x0007A536 File Offset: 0x00078736
			// (set) Token: 0x06001E93 RID: 7827 RVA: 0x0007A53E File Offset: 0x0007873E
			public Action<bool> OnStepStart { get; private set; }

			// Token: 0x1700058B RID: 1419
			// (get) Token: 0x06001E94 RID: 7828 RVA: 0x0007A547 File Offset: 0x00078747
			// (set) Token: 0x06001E95 RID: 7829 RVA: 0x0007A54F File Offset: 0x0007874F
			public Action OnStepComplete { get; private set; }

			// Token: 0x1700058C RID: 1420
			// (get) Token: 0x06001E96 RID: 7830 RVA: 0x0007A558 File Offset: 0x00078758
			// (set) Token: 0x06001E97 RID: 7831 RVA: 0x0007A560 File Offset: 0x00078760
			public Action DesignerConstantsUpdateHandler { get; private set; }

			// Token: 0x06001E98 RID: 7832 RVA: 0x0007A569 File Offset: 0x00078769
			public TutorialProgressionProcess.TutorialStep ClockTicksWhile(Func<bool> clockTickCheck)
			{
				this.DoesClockTick = clockTickCheck;
				return this;
			}

			// Token: 0x06001E99 RID: 7833 RVA: 0x0007A573 File Offset: 0x00078773
			public TutorialProgressionProcess.TutorialStep StepOverWhen(Func<bool> stepOverCheck)
			{
				this.IsStepOver = stepOverCheck;
				return this;
			}

			// Token: 0x06001E9A RID: 7834 RVA: 0x0007A57D File Offset: 0x0007877D
			public TutorialProgressionProcess.TutorialStep WhenStepStarts(Action<bool> onStartHandler)
			{
				this.OnStepStart = onStartHandler;
				return this;
			}

			// Token: 0x06001E9B RID: 7835 RVA: 0x0007A588 File Offset: 0x00078788
			public TutorialProgressionProcess.TutorialStep WhenStepStarts(Action onStartHandler)
			{
				this.OnStepStart = delegate(bool isStepOver)
				{
					onStartHandler();
				};
				return this;
			}

			// Token: 0x06001E9C RID: 7836 RVA: 0x0007A5B5 File Offset: 0x000787B5
			public TutorialProgressionProcess.TutorialStep StepRegressesWhen(Func<bool> stepRegressCheck)
			{
				this.ShouldRegressStep = stepRegressCheck;
				return this;
			}

			// Token: 0x06001E9D RID: 7837 RVA: 0x0007A5BF File Offset: 0x000787BF
			public TutorialProgressionProcess.TutorialStep WhenStepEnds(Action onCompleteHandler)
			{
				this.OnStepComplete = onCompleteHandler;
				return this;
			}

			// Token: 0x06001E9E RID: 7838 RVA: 0x0007A5C9 File Offset: 0x000787C9
			public TutorialProgressionProcess.TutorialStep AddIdleHint(IdleHint idleHint)
			{
				this.IdleHints.Add(idleHint);
				return this;
			}

			// Token: 0x06001E9F RID: 7839 RVA: 0x0007A5D8 File Offset: 0x000787D8
			public TutorialProgressionProcess.TutorialStep SetIdlePromptHandler(Action<Fix64> idleAnimation)
			{
				this.IdlePromptAnimationHandler = idleAnimation;
				return this;
			}

			// Token: 0x06001EA0 RID: 7840 RVA: 0x0007A5E2 File Offset: 0x000787E2
			public TutorialProgressionProcess.TutorialStep SetIdleMessageHandler(Action<Fix64> idleAnimation)
			{
				this.IdleMessageAnimationHandler = idleAnimation;
				return this;
			}

			// Token: 0x06001EA1 RID: 7841 RVA: 0x0007A5EC File Offset: 0x000787EC
			public TutorialProgressionProcess.TutorialStep SetDesignerConstantsUpdateHandler(Action constantsUpdateHandler)
			{
				this.DesignerConstantsUpdateHandler = constantsUpdateHandler;
				return this;
			}

			// Token: 0x06001EA2 RID: 7842 RVA: 0x0007A5F6 File Offset: 0x000787F6
			public TutorialProgressionProcess.TutorialStep SetDebugText(Func<string> debugText)
			{
				this.DebugText = debugText;
				return this;
			}
		}
	}
}
