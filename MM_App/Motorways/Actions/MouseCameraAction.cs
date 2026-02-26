using System;
using Factory;
using Motorways.Audio;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x020006F1 RID: 1777
	public class MouseCameraAction : MotorwaysPlayerAction
	{
		// Token: 0x060030A5 RID: 12453 RVA: 0x000E4AA8 File Offset: 0x000E2CA8
		public override void OnActionBegin(float timestamp)
		{
			if (!this._cameraView.IsFocussedIn)
			{
				this.OnActionCancel();
				return;
			}
			base.OnActionBegin(timestamp);
			this._initialScreenPosition = base.GetPointerScreenPosition();
			this._isPanning = false;
			PlayerAction.Log.Info("Beginning MouseCameraAction from {0}.", new object[]
			{
				this._initialScreenPosition
			});
		}

		// Token: 0x060030A6 RID: 12454 RVA: 0x000E4B08 File Offset: 0x000E2D08
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			Vector2 newPosition = base.GetPointerScreenPosition();
			if (this._inputState.Mouse.GetButtonState(3).IsUp)
			{
				if (!this._isPanning)
				{
					this._panOriginWorldPosition = this._tilemapView.GetWorldPositionFromScreenPosition(newPosition);
					this._isPanning = true;
					PlayerAction.Log.Info("Beginning pan from a screen position of {0} holding a world position of {1}.", new object[]
					{
						this._initialScreenPosition,
						this._panOriginWorldPosition
					});
					if (!base.IsExclusive)
					{
						base.MakeExclusive();
						return;
					}
				}
				else
				{
					this._cameraView.ApplyPlayerPanPosition(this._panOriginWorldPosition, newPosition);
				}
			}
		}

		// Token: 0x060030A7 RID: 12455 RVA: 0x000E4BB0 File Offset: 0x000E2DB0
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.ButtonState == InputEventButtonState.JustUp)
			{
				Vector2 newScreenPosition = inputEvent.PointerPosition;
				if (!this._isPanning && Vector2.Distance(newScreenPosition, this._initialScreenPosition) < this._tilemapView.ScreenDistanceBetweenTiles * 0.5f && timestamp - this.timeCreated <= 1f)
				{
					if (this._inputState.Mouse.GetButtonState(3).IsUp && this._cameraView.CanChangeFocus)
					{
						if (!this._cameraView.IsFocussedIn)
						{
							this.SetWorldGridVisible(true);
							if (this._player.IsZoomEnabled)
							{
								this._cameraView.FocusOnWorldPosition(this._tilemapView.GetWorldPositionFromScreenPosition(newScreenPosition), CameraView.CameraFocusOffsetType.MaintainScreenPosition);
							}
							else
							{
								this._cameraView.FocusOnWorldPositionWithoutZoom(this._tilemapView.GetWorldPositionFromScreenPosition(newScreenPosition), CameraView.CameraFocusOffsetType.MaintainScreenPosition);
							}
							this._gameUI.SetDrawButtonsVisible(true);
							this._tilemapView.viewMode = TilemapView.ViewMode.Edit;
							this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.FocusZoomIn, UIAudioProfile.None, this._cameraView.GetInterpolationSpeed(), true, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
						}
						else
						{
							this.SetWorldGridVisible(false);
							this.SetMotorwayGridVisible(false);
							this._tilemapView.viewMode = TilemapView.ViewMode.Normal;
							this._cameraView.ResetPlayerViewport();
							this._gameUI.SetDrawButtonsVisible(false);
							if (this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Remove)
							{
								this._gameUI.ToggleDrawMode();
							}
							this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.FocusZoomOut, UIAudioProfile.None, this._cameraView.GetInterpolationSpeed(), true, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
						}
					}
					this.OnActionComplete();
					return;
				}
				this._cameraView.ReleasePlayerPan();
				this.OnActionComplete();
			}
		}

		// Token: 0x060030A8 RID: 12456 RVA: 0x000E4D60 File Offset: 0x000E2F60
		public static MouseCameraAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			MouseCameraAction newAction = scope.Get<MouseCameraAction>();
			newAction.InitializeAction(owningGroup, timestamp);
			PlayerAction.Log.Info("[MouseCameraAction] Creating new instance of action: {0}", new object[]
			{
				timestamp
			});
			if (owningGroup.InstigatingInputEvent.Source == InputEventSource.Mouse)
			{
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateMouseEventFilter(30, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
			}
			newAction.OnActionBegin(timestamp);
			return newAction;
		}

		// Token: 0x060030A9 RID: 12457 RVA: 0x000E4DBE File Offset: 0x000E2FBE
		public override void Reset()
		{
			base.Reset();
			this._isPanning = false;
			this._initialScreenPosition = default(Vector2);
			this._panOriginWorldPosition = default(Vector2);
		}

		// Token: 0x040029E3 RID: 10723
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x040029E4 RID: 10724
		[Dependency]
		protected IAudioSystem _audioSystem;

		// Token: 0x040029E5 RID: 10725
		[Dependency]
		protected ActivePlayer _player;

		// Token: 0x040029E6 RID: 10726
		private Vector2 _initialScreenPosition;

		// Token: 0x040029E7 RID: 10727
		private Vector2 _panOriginWorldPosition;

		// Token: 0x040029E8 RID: 10728
		private bool _isPanning;

		// Token: 0x040029E9 RID: 10729
		private const float _tapTimeThreshold = 1f;

		// Token: 0x040029EA RID: 10730
		private const float _tapDistanceCoefficient = 0.5f;
	}
}
