using System;
using Factory;
using Motorways.Audio;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x020006F2 RID: 1778
	public class TouchCameraAction : MotorwaysPlayerAction
	{
		// Token: 0x060030AB RID: 12459 RVA: 0x000E4DE8 File Offset: 0x000E2FE8
		public override void OnActionBegin(float timestamp)
		{
			this._playerActionController.CancelAllActions();
			base.OnActionBegin(timestamp);
			this._initialScreenPosition = base.GetPointerScreenPosition();
			this._currentScreenPosition = this._initialScreenPosition;
			this._isPanning = false;
			PlayerAction.Log.Info("Beginning TouchCameraAction from {0}.", new object[]
			{
				this._initialScreenPosition
			});
		}

		// Token: 0x060030AC RID: 12460 RVA: 0x000E4E4C File Offset: 0x000E304C
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			Vector2 newPosition = base.GetPointerScreenPosition();
			if (this._inputState.TouchCount == 2)
			{
				if (!this._isPanning)
				{
					this._panOriginCameraPosition = this._cameraView.DesiredPosition;
					this._panOriginWorldPosition = this._tilemapView.GetWorldPositionFromScreenPosition(newPosition);
					this._isPanning = true;
					PlayerAction.Log.Info("Beginning pan from a screen position of {0} touching a world position of {1}.", new object[]
					{
						this._initialScreenPosition,
						this._panOriginWorldPosition
					});
					if (!base.IsExclusive)
					{
						base.MakeExclusive();
					}
				}
				else
				{
					this._cameraView.ApplyPlayerPanPosition(this._panOriginWorldPosition, newPosition);
				}
			}
			this._currentScreenPosition = newPosition;
		}

		// Token: 0x060030AD RID: 12461 RVA: 0x000E4F0C File Offset: 0x000E310C
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.ButtonState == InputEventButtonState.JustUp)
			{
				Vector2 newScreenPosition = inputEvent.PointerPosition;
				if (!this._isPanning && Vector2.Distance(newScreenPosition, this._initialScreenPosition) < this._tilemapView.ScreenDistanceBetweenTiles * this._tapDistanceCoefficient && timestamp - this.timeCreated <= this._tapTimeThreshold)
				{
					if (this._inputState.TouchCount == 1 && this._cameraView.CanChangeFocus)
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

		// Token: 0x060030AE RID: 12462 RVA: 0x000E50B4 File Offset: 0x000E32B4
		public static TouchCameraAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			TouchCameraAction newAction = scope.Get<TouchCameraAction>();
			newAction.InitializeAction(owningGroup, timestamp);
			PlayerAction.Log.Info("[TouchCameraAction] Creating new instance of action: {0}", new object[]
			{
				timestamp
			});
			if (owningGroup.InstigatingInputEvent.Source == InputEventSource.Touch)
			{
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateTouchEventFilter(0, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateTouchEventFilter(1, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
			}
			newAction.OnActionBegin(timestamp);
			return newAction;
		}

		// Token: 0x060030AF RID: 12463 RVA: 0x000E5120 File Offset: 0x000E3320
		public override void Reset()
		{
			base.Reset();
			this._isPanning = false;
			this._initialScreenPosition = default(Vector2);
			this._currentScreenPosition = default(Vector2);
			this._panOriginCameraPosition = default(Vector2);
			this._panOriginWorldPosition = default(Vector2);
		}

		// Token: 0x040029EB RID: 10731
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x040029EC RID: 10732
		[Dependency]
		protected IAudioSystem _audioSystem;

		// Token: 0x040029ED RID: 10733
		[Dependency]
		protected ActivePlayer _player;

		// Token: 0x040029EE RID: 10734
		private Vector2 _initialScreenPosition;

		// Token: 0x040029EF RID: 10735
		private Vector2 _currentScreenPosition;

		// Token: 0x040029F0 RID: 10736
		private Vector2 _panOriginCameraPosition;

		// Token: 0x040029F1 RID: 10737
		private Vector2 _panOriginWorldPosition;

		// Token: 0x040029F2 RID: 10738
		private bool _isPanning;

		// Token: 0x040029F3 RID: 10739
		private float _tapTimeThreshold = 1f;

		// Token: 0x040029F4 RID: 10740
		private float _tapDistanceCoefficient = 0.5f;

		// Token: 0x040029F5 RID: 10741
		[Dependency]
		private PlayerActionController _playerActionController;
	}
}
