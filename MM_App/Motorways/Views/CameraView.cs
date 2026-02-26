using System;
using System.Collections.Generic;
using Client;
using Easing;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Motorways.Utility;
using Motorways.Views.Trains;
using Server;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000587 RID: 1415
	public class CameraView : IView, InputState.IObserver, ICreatedInScopeHandler, IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x14000041 RID: 65
		// (add) Token: 0x060026FF RID: 9983 RVA: 0x000A5F04 File Offset: 0x000A4104
		// (remove) Token: 0x06002700 RID: 9984 RVA: 0x000A5F3C File Offset: 0x000A413C
		public event Action OnCameraZoomLevelChanged;

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x06002701 RID: 9985 RVA: 0x000A5F71 File Offset: 0x000A4171
		// (set) Token: 0x06002702 RID: 9986 RVA: 0x000A5F79 File Offset: 0x000A4179
		public Vector3 DesiredPosition { get; private set; }

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x06002703 RID: 9987 RVA: 0x000A5F82 File Offset: 0x000A4182
		// (set) Token: 0x06002704 RID: 9988 RVA: 0x000A5F8A File Offset: 0x000A418A
		public Vector3 CurrentUnfocusedPosition { get; private set; }

		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x06002705 RID: 9989 RVA: 0x000A5F93 File Offset: 0x000A4193
		// (set) Token: 0x06002706 RID: 9990 RVA: 0x000A5F9B File Offset: 0x000A419B
		public float MaxZoom { get; private set; }

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x06002707 RID: 9991 RVA: 0x000A5FA4 File Offset: 0x000A41A4
		// (set) Token: 0x06002708 RID: 9992 RVA: 0x000A5FAC File Offset: 0x000A41AC
		public float DesiredZoom { get; private set; }

		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x06002709 RID: 9993 RVA: 0x000A5FB5 File Offset: 0x000A41B5
		// (set) Token: 0x0600270A RID: 9994 RVA: 0x000A5FBD File Offset: 0x000A41BD
		public bool IsFocussedIn
		{
			get
			{
				return this._isFocussedIn;
			}
			private set
			{
				this._isFocussedIn = value;
			}
		}

		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x0600270B RID: 9995 RVA: 0x000A5FC6 File Offset: 0x000A41C6
		// (set) Token: 0x0600270C RID: 9996 RVA: 0x000A5FCE File Offset: 0x000A41CE
		public bool CanChangeFocus { get; set; }

		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x0600270D RID: 9997 RVA: 0x000A5FD7 File Offset: 0x000A41D7
		// (set) Token: 0x0600270E RID: 9998 RVA: 0x000A5FDF File Offset: 0x000A41DF
		public float FixedZoom { get; private set; }

		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x0600270F RID: 9999 RVA: 0x000A5FE8 File Offset: 0x000A41E8
		public GameCamera GameCamera
		{
			get
			{
				return this._camera;
			}
		}

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x06002710 RID: 10000 RVA: 0x000A5FF0 File Offset: 0x000A41F0
		public int CinematicZoomIndex
		{
			get
			{
				return this._cinematicZoomIndex;
			}
		}

		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x06002711 RID: 10001 RVA: 0x000A5FF8 File Offset: 0x000A41F8
		public int ZoomLevelCount
		{
			get
			{
				return this._visualConstantsData.ZoomLevelsCameraMin.Count;
			}
		}

		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x06002712 RID: 10002 RVA: 0x000A600A File Offset: 0x000A420A
		private bool _editMenuFocusPointApplied
		{
			get
			{
				return this._editMenuFocusPoint.sqrMagnitude > 0f;
			}
		}

		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x06002713 RID: 10003 RVA: 0x000A601E File Offset: 0x000A421E
		public bool IsPlayerPanning
		{
			get
			{
				return this._isPlayerPanning;
			}
		}

		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x06002714 RID: 10004 RVA: 0x000A6028 File Offset: 0x000A4228
		public float MinZoom
		{
			get
			{
				if (CameraView.potentialTapZoomIndex != 0)
				{
					return CameraView.potentialTapZoomLevels[CameraView.potentialTapZoomIndex];
				}
				if (this.IsInCinematicMode)
				{
					return this._visualConstantsData.ZoomLevelsCameraMin[this._cinematicZoomIndex];
				}
				return this._visualConstantsData.ZoomLevelsCameraMin[this._player.ZoomLevel];
			}
		}

		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x06002715 RID: 10005 RVA: 0x000A6086 File Offset: 0x000A4286
		// (set) Token: 0x06002716 RID: 10006 RVA: 0x000A608E File Offset: 0x000A428E
		public bool HasControlOverriden
		{
			get
			{
				return this._hasControlOverriden;
			}
			set
			{
				if (this._hasControlOverriden && !value)
				{
					this._camera.transform.rotation = Quaternion.identity;
				}
				this._hasControlOverriden = value;
			}
		}

		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x06002717 RID: 10007 RVA: 0x000A60B7 File Offset: 0x000A42B7
		public bool IsInCinematicMode
		{
			get
			{
				return this._cinematicModeState > CameraView.CinematicModeState.Off;
			}
		}

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x06002718 RID: 10008 RVA: 0x000A60C4 File Offset: 0x000A42C4
		public bool HasControl
		{
			get
			{
				if (this.HasControlOverriden || this.IsInCinematicMode)
				{
					return false;
				}
				if (this.IsInGame)
				{
					ScreenStack.MotorwaysScreen topScreenType = this._screenStack.GetTopActiveScreenType();
					return topScreenType == ScreenStack.MotorwaysScreen.InGame || topScreenType == ScreenStack.MotorwaysScreen.Upgrade || topScreenType == ScreenStack.MotorwaysScreen.CinematicMode;
				}
				return false;
			}
		}

		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x06002719 RID: 10009 RVA: 0x000A6108 File Offset: 0x000A4308
		private bool IsInGame
		{
			get
			{
				return this._screenStack.IsScreenVisible(ScreenStack.MotorwaysScreen.InGame);
			}
		}

		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x0600271A RID: 10010 RVA: 0x000A6118 File Offset: 0x000A4318
		private float DebugRotation
		{
			get
			{
				if (!this.HasControlOverriden)
				{
					return 0f;
				}
				if (Input.GetKeyDown(KeyCode.LeftBracket))
				{
					this._debugRotation -= 15f;
				}
				if (Input.GetKeyDown(KeyCode.RightBracket))
				{
					this._debugRotation += 15f;
				}
				return this._debugRotation;
			}
		}

		// Token: 0x0600271B RID: 10011 RVA: 0x000A616F File Offset: 0x000A436F
		public void OnReleasedFromScope(IScope scope)
		{
			this._inputState.Unsubscribe(this);
		}

		// Token: 0x0600271C RID: 10012 RVA: 0x000A6180 File Offset: 0x000A4380
		public void Reset()
		{
			this.DesiredPosition = default(Vector3);
			this.CurrentUnfocusedPosition = default(Vector3);
			this.MaxZoom = 0f;
			this.DesiredZoom = 0f;
			this._cinematicZoomIndex = 0;
			this.IsFocussedIn = false;
			this.CanChangeFocus = false;
			this.FixedZoom = 0f;
			this._interpolationType = CameraView.CameraInterpType.Default;
			this.HasControlOverriden = false;
			InertialFloat panX = this._panX;
			if (panX != null)
			{
				panX.Reset();
			}
			InertialFloat panY = this._panY;
			if (panY != null)
			{
				panY.Reset();
			}
			this.playerOrthoZoom = -1f;
			this.playerZoomedIn = false;
			this._isPlayerPanning = false;
			this._setPanToCenter = false;
			this._includeSafeAreaOffsetInZoom = false;
			this._debugControlsPanOffset = default(Vector2);
			this._debugControlsZoomOffset = 0f;
			this._debugControlsLastMouseWorldPosition = default(Vector3);
			this._cinematicModeVehicleToFollow = null;
			this._cinematicModeState = CameraView.CinematicModeState.Off;
			this._cinematicModeSpeed = 1f;
			this._durationSpentOnCurrentMode = 0f;
		}

		// Token: 0x0600271D RID: 10013 RVA: 0x000A627F File Offset: 0x000A447F
		public void OnCreatedInScope(IScope scope)
		{
			this._inputState.Subscribe(this);
		}

		// Token: 0x0600271E RID: 10014 RVA: 0x000A6290 File Offset: 0x000A4490
		public void Initialize(GameRules rules)
		{
			CameraView.Log.Info("CameraView initialized.", Array.Empty<object>());
			this.playerOrthoZoom = this._visualConstantsData.ZoomLevelsCameraMin[this._player.ZoomLevel];
			this.CanChangeFocus = true;
			this._panX = new InertialFloat(this._visualConstantsData.CinematicCameraSpringDuration, this._visualConstantsData.CinematicCameraEasingFunction);
			this._panY = new InertialFloat(this._visualConstantsData.CinematicCameraSpringDuration, this._visualConstantsData.CinematicCameraEasingFunction);
			this._panX.Range = rules.GetCameraPanRange();
			this._panY.Range = rules.GetCameraPanRange();
		}

		// Token: 0x0600271F RID: 10015 RVA: 0x000A6340 File Offset: 0x000A4540
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._city == null || !this._city.Rules.UseCamera())
			{
				return TickResult.ContinueTicking;
			}
			Fix64 fixedZoom = this._city.GetCameraSizeAtTime(this._clock.GetInterpolatedExpansionTime(stepAlpha));
			this.FixedZoom = (float)fixedZoom;
			RectFixed playableArea = this._city.GetClientPlayableAreaAtZoom(fixedZoom, City.PlayableAreaRoundingType.AllowPartialTiles);
			RectFixed playableTiles = this._city.GetClientPlayableAreaAtZoom(fixedZoom, City.PlayableAreaRoundingType.ForceWholeTiles);
			this._camera.debugPlayableArea = playableTiles;
			this._camera.debugPlayerOffset = new Vector2(this._panX.RawValue, this._panY.RawValue);
			Vector2 cameraSafeAreaOffset = Vector2.zero;
			this._gameUI.playableArea.GetWorldCorners(CameraView.PlayableCorners);
			this.UpdateMaxZoom(playableArea);
			if (!this.playerZoomedIn)
			{
				this.playerOrthoZoom = this.MaxZoom;
			}
			if (!this.playerZoomedIn || this._includeSafeAreaOffsetInZoom)
			{
				cameraSafeAreaOffset = this.GetCameraOffsetToFitPlayableArea(this.playerOrthoZoom, CameraView.PlayableCorners);
			}
			if (this._interpolationType == CameraView.CameraInterpType.Resetting && Mathf.Approximately(this._camera.OrthographicSize, this.playerOrthoZoom))
			{
				this._interpolationType = CameraView.CameraInterpType.Default;
			}
			if (this._viewClient.OnFirstFrame)
			{
				this.DesiredZoom = this.playerOrthoZoom;
			}
			else
			{
				this.DesiredZoom = Mathf.Lerp(this._camera.OrthographicSize, this.playerOrthoZoom, this.GetInterpolationSpeed());
			}
			Vector3 newOrigin = new Vector3((float)playableArea.Center.x + cameraSafeAreaOffset.x, (float)playableArea.Center.y + cameraSafeAreaOffset.y, this._camera.transform.position.z);
			this.CurrentUnfocusedPosition = newOrigin;
			if (this.playerZoomedIn || this._isPlayerPanning || this._editMenuFocusPointApplied)
			{
				float zoomRatio = this.DesiredZoom / this.MaxZoom;
				float excessWidth = ((float)playableArea.width - (float)playableArea.width * zoomRatio) * 0.6f;
				float excessHeight = ((float)playableArea.height - (float)playableArea.height * zoomRatio) * 0.5f;
				this._panX.Min = newOrigin.x - excessWidth;
				this._panX.Max = newOrigin.x + excessWidth;
				this._panY.Min = newOrigin.y - excessHeight;
				this._panY.Max = newOrigin.y + excessHeight;
				if (this._setPanToCenter)
				{
					this._panX.RawValue = newOrigin.x;
					this._panY.RawValue = newOrigin.y;
				}
				this._panX.Tick(timeInterval.Delta);
				this._panY.Tick(timeInterval.Delta);
				newOrigin = new Vector3(this._panX.ConstrainedValue, this._panY.ConstrainedValue, this._camera.transform.position.z);
			}
			if (this._viewClient.OnFirstFrame || !this.HasControl)
			{
				this.DesiredPosition = newOrigin;
			}
			else
			{
				this.DesiredPosition = Vector3.Lerp(this.DesiredPosition, newOrigin, this.GetInterpolationSpeed());
			}
			Shader.SetGlobalVector("_PLAYABLE_AREA", new Vector4((float)playableTiles.xMin - 1f, (float)playableTiles.yMin - 1f, (float)playableTiles.xMax + 1f, (float)playableTiles.yMax + 1f));
			if (this.HasControl)
			{
				this._camera.SetPosition(this.DesiredPosition);
				this._camera.OrthographicSize = this.DesiredZoom;
			}
			else if (this.IsInCinematicMode)
			{
				this.HandleCinematicCamera(timeInterval.ScaledDelta);
			}
			else if (this.HasControlOverriden && this.IsInGame)
			{
				this.ApplyDebugPanAndZoom();
			}
			this._setPanToCenter = false;
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002720 RID: 10016 RVA: 0x000022F5 File Offset: 0x000004F5
		public void SetGameobjectActive(bool isActive)
		{
		}

		// Token: 0x06002721 RID: 10017 RVA: 0x000A6728 File Offset: 0x000A4928
		public void EnterCinematicMode()
		{
			this._cinematicModeState = CameraView.CinematicModeState.AwaitingJourney;
			this._cinematicModeSpeed = 0f;
			this._cinematicZoomIndex = this._player.ZoomLevel;
			this.GoToNextAgentInCinematicMode();
		}

		// Token: 0x06002722 RID: 10018 RVA: 0x000A6754 File Offset: 0x000A4954
		public void GoToNextAgentInCinematicMode()
		{
			this.TryAssignNextAvailableVehicle();
			this._durationSpentOnCurrentMode = 0f;
			if (!this._player.IsSkipTransitionsEnabled)
			{
				this._cinematicModeSpeed = 0f;
				return;
			}
			if (this._cinematicModeSpeed > 0f)
			{
				this._cinematicModeSpeed = 1f;
			}
		}

		// Token: 0x06002723 RID: 10019 RVA: 0x000A67A4 File Offset: 0x000A49A4
		public void ExitCinematicMode()
		{
			if (this._cinematicModeNoCarsMessage != null)
			{
				this._simulation.RemoveModel(this._cinematicModeNoCarsMessage);
				this._cinematicModeNoCarsMessage = null;
			}
			if (this.IsInCinematicMode && this._cinematicModeState != CameraView.CinematicModeState.ExitingMode)
			{
				this._cinematicModeState = CameraView.CinematicModeState.ExitingMode;
				this._cinematicModeSpeed = 0f;
			}
		}

		// Token: 0x06002724 RID: 10020 RVA: 0x000A67F8 File Offset: 0x000A49F8
		public void CinematicModeDebugGUI()
		{
			if (!this.IsInCinematicMode)
			{
				return;
			}
			Transform transformToFollow;
			if (this._cinematicModeState == CameraView.CinematicModeState.ExitingMode || this._cinematicModeState == CameraView.CinematicModeState.CompletedJourney)
			{
				transformToFollow = null;
			}
			else if (this._cinematicModeState == CameraView.CinematicModeState.FollowingTrain)
			{
				transformToFollow = this._cinematicModeTrainToFollow.CenterTransform;
			}
			else
			{
				transformToFollow = this._cinematicModeVehicleToFollow.transform;
			}
			string text = "Cinematic mode:\n" + string.Format("Current State: {0}\n", this._cinematicModeState) + string.Format("Current Speed: {0:F1}\n", this._cinematicModeSpeed) + string.Format("Duration In state: {0:F1}\n", this._durationSpentOnCurrentMode);
			if (transformToFollow != null)
			{
				text = text + "Current Agent: " + transformToFollow.gameObject.name;
			}
			GUI.Label(new Rect(10f, 50f, 1000f, 400f), text, new GUIStyle
			{
				fontSize = 50
			});
		}

		// Token: 0x06002725 RID: 10021 RVA: 0x000A68E0 File Offset: 0x000A4AE0
		private void HandleCinematicCamera(float deltaTime)
		{
			if ((this._cinematicModeState == CameraView.CinematicModeState.FollowingTrain && this._durationSpentOnCurrentMode > this._visualConstantsData.MaximumDurationOnTrain) || (this._cinematicModeState == CameraView.CinematicModeState.AwaitingJourney && this._durationSpentOnCurrentMode > this._visualConstantsData.MaximumDurationOnIdleCar) || (this._cinematicModeState == CameraView.CinematicModeState.CompletedJourney && this._durationSpentOnCurrentMode > this._visualConstantsData.WaitDurationBetweenCompletedJourneyAndNewAgent))
			{
				this.GoToNextAgentInCinematicMode();
			}
			else if (this._cinematicModeState == CameraView.CinematicModeState.FollowingTrain)
			{
				if (this._durationSpentOnCurrentMode > this._visualConstantsData.MinimumDurationOnTrain && this._cinematicModeTrainToFollow.Model.distanceTraveledSinceLastStation < Fix64.One && this._cinematicModeTrainToFollow.Model.state == TrainModel.BehaviorState.Stopped && this._cinematicModeTrainToFollow.Model.DelayBeforeStarting < (Fix64)0.5)
				{
					this._cinematicModeState = CameraView.CinematicModeState.CompletedJourney;
					this._durationSpentOnCurrentMode = 0f;
				}
			}
			else if (this._cinematicModeState == CameraView.CinematicModeState.AwaitingJourney)
			{
				if (this._cinematicModeVehicleToFollow.Model.IsDrivingToDestination)
				{
					this._cinematicModeState = CameraView.CinematicModeState.OnJourney;
					this._durationSpentOnCurrentMode = 0f;
				}
			}
			else if (this._cinematicModeState == CameraView.CinematicModeState.OnJourney && this._cinematicModeVehicleToFollow.Model.IsWaitingAtHouse)
			{
				this._cinematicModeState = CameraView.CinematicModeState.CompletedJourney;
				this._durationSpentOnCurrentMode = 0f;
				this._cinematicModeSpeed = 0.05f;
			}
			Vector3 desiredPosition = this.DesiredPosition;
			if (this._cinematicModeState == CameraView.CinematicModeState.CompletedJourney)
			{
				if (this._cinematicModeVehicleToFollow != null)
				{
					desiredPosition = this._cinematicModeVehicleToFollow.Model.house.tileModel.Coordinates.ToVector3() * 2f;
				}
				else
				{
					desiredPosition = this._camera.transform.position;
				}
			}
			else if (this._cinematicModeState != CameraView.CinematicModeState.ExitingMode)
			{
				desiredPosition = ((this._cinematicModeState == CameraView.CinematicModeState.FollowingTrain) ? this._cinematicModeTrainToFollow.CenterTransform : this._cinematicModeVehicleToFollow.transform).position;
			}
			Vector3 movementDelta = desiredPosition - this._camera.transform.position;
			movementDelta.z = 0f;
			float cameraSpeed = 1f;
			if (cameraSpeed > this._cinematicModeSpeed)
			{
				this._cinematicModeSpeed += deltaTime * this._visualConstantsData.CinematicCameraAccelerationWhenChangingAgent;
				if (this._cinematicModeSpeed > cameraSpeed)
				{
					this._cinematicModeSpeed = cameraSpeed;
				}
			}
			this._durationSpentOnCurrentMode += deltaTime;
			this._camera.transform.position += movementDelta * Mathf.Clamp(this._cinematicModeSpeed * Time.deltaTime * this._visualConstantsData.CinematicCameraMoveSpeed, 0f, 1f);
			float zoomDelta = ((this._cinematicModeState == CameraView.CinematicModeState.ExitingMode) ? this.DesiredZoom : this._visualConstantsData.ZoomLevelsCameraMin[this._cinematicZoomIndex]) - this._camera.OrthographicSize;
			this._camera.OrthographicSize += zoomDelta * Time.deltaTime * this._visualConstantsData.CinematicCameraZoomSpeed * this._cinematicModeSpeed;
			if (this._cinematicModeState == CameraView.CinematicModeState.ExitingMode && zoomDelta <= 0.01f && movementDelta.sqrMagnitude <= 0.01f)
			{
				this._cinematicModeState = CameraView.CinematicModeState.Off;
			}
		}

		// Token: 0x06002726 RID: 10022 RVA: 0x000A6C10 File Offset: 0x000A4E10
		private void TryAssignNextAvailableVehicle()
		{
			List<TrainView> trains = this._viewClient.GetViews<TrainView>();
			if (this._cinematicModeTrainToFollow == null && trains.Count > 0)
			{
				Vector3 deltaToTrain = trains[0].CenterTransform.position - this._camera.transform.position;
				deltaToTrain.z = 0f;
				float distanceToTrainFromCurrentPosition = deltaToTrain.magnitude;
				distanceToTrainFromCurrentPosition -= this._visualConstantsData.DistanceAtWhichToLowerChanceToSelectTrain;
				float distanceMultiplier = Mathf.Lerp(1f, 0.25f, distanceToTrainFromCurrentPosition / this._visualConstantsData.DistanceAtWhichToLowerChanceToSelectTrain);
				float num = UnityEngine.Random.Range(0f, 1f);
				float chanceToSelectTrain = this._visualConstantsData.ChanceToSelectTrain * distanceMultiplier;
				if (num < chanceToSelectTrain)
				{
					this._cinematicModeTrainToFollow = trains[0];
					this._cinematicModeState = CameraView.CinematicModeState.FollowingTrain;
					this._cinematicModeVehicleToFollow = null;
					return;
				}
			}
			else
			{
				this._cinematicModeTrainToFollow = null;
			}
			List<VehicleView> validVehicles = new List<VehicleView>();
			List<VehicleView> secondaryVehicles = new List<VehicleView>();
			foreach (VehicleView vehicle in this._viewClient.GetViews<VehicleView>())
			{
				if (vehicle.Model.IsDrivingToDestination && vehicle != this._cinematicModeVehicleToFollow)
				{
					validVehicles.Add(vehicle);
				}
				else if (vehicle.Model.IsAvailableAtHouse && vehicle.Model.house.FirstWaitingVehicle == vehicle.Model)
				{
					secondaryVehicles.Add(vehicle);
				}
			}
			if (validVehicles.Count > 0)
			{
				this._cinematicModeVehicleToFollow = validVehicles[UnityEngine.Random.Range(0, validVehicles.Count)];
				this._cinematicModeState = CameraView.CinematicModeState.AwaitingJourney;
				return;
			}
			if (secondaryVehicles.Count > 0)
			{
				this._cinematicModeVehicleToFollow = secondaryVehicles[UnityEngine.Random.Range(0, secondaryVehicles.Count)];
				this._cinematicModeState = CameraView.CinematicModeState.AwaitingJourney;
				return;
			}
			if (trains.Count > 0)
			{
				this._cinematicModeTrainToFollow = trains[0];
				this._cinematicModeState = CameraView.CinematicModeState.FollowingTrain;
				return;
			}
			List<VehicleView> vehicleViews = this._viewClient.GetViews<VehicleView>();
			if (vehicleViews.Count > 0)
			{
				this._cinematicModeVehicleToFollow = vehicleViews[0];
				this._cinematicModeState = CameraView.CinematicModeState.AwaitingJourney;
			}
			else
			{
				if (this._cinematicModeNoCarsMessage != null)
				{
					this._simulation.RemoveModel(this._cinematicModeNoCarsMessage);
				}
				this._cinematicModeNoCarsMessage = this._city.Scope.Get<AnchoredMessageModel>();
				this._cinematicModeNoCarsMessage.InitializeWithScreenAnchor(StringId.CinematicMode_ErrorMessage_NoCarsToFollow, CameraView.MessageAnchorOffset, CameraLayer.Overlay, null);
				this._simulation.AddModel(this._cinematicModeNoCarsMessage);
				this._cinematicModeState = CameraView.CinematicModeState.Off;
			}
			Diagnostics.Log.Warn("CameraView", "We couldn't find a valid car or train to cinematic mode select! Maybe there's no trains and nothing is connected.", Array.Empty<object>());
		}

		// Token: 0x06002727 RID: 10023 RVA: 0x000A6EBC File Offset: 0x000A50BC
		private void ApplyDebugPanAndZoom()
		{
			Transform cameraTransform = this._camera.transform;
			float zoomSpeed = 5f;
			if (this._camera.OrthographicSize < 10f)
			{
				zoomSpeed = Mathf.Lerp(5f, 0.5f, Easings.QuadraticEaseOut(1f - Mathf.Clamp01(this._camera.OrthographicSize / 10f)));
			}
			Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			Vector3 mouseWorldPositionBeforeZoom = this._camera.GetWorldFromScreen(mousePosition);
			float zoomOffset = -(Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime);
			float debugZoomOrthographicSize = this.DesiredZoom + this._debugControlsZoomOffset;
			if (debugZoomOrthographicSize >= 0.1f || (debugZoomOrthographicSize < 0.1f && zoomOffset >= 0f))
			{
				debugZoomOrthographicSize += zoomOffset;
				this._debugControlsZoomOffset += zoomOffset;
			}
			this._camera.OrthographicSize = ((debugZoomOrthographicSize >= 0.1f) ? debugZoomOrthographicSize : 0.1f);
			Vector2 mouseWorldPositionDelta = this._camera.GetWorldFromScreen(mousePosition) - mouseWorldPositionBeforeZoom;
			cameraTransform.position -= new Vector3(mouseWorldPositionDelta.x, mouseWorldPositionDelta.y, 0f);
			Vector3 mouseWorldPosition = this._camera.GetWorldFromScreen(mousePosition);
			if (Input.GetMouseButtonDown(2))
			{
				this._debugControlsLastMouseWorldPosition = mouseWorldPosition;
			}
			if (Input.GetMouseButton(2))
			{
				Vector3 changeInPosition = this._debugControlsLastMouseWorldPosition - mouseWorldPosition;
				cameraTransform.position += changeInPosition;
				this._debugControlsLastMouseWorldPosition = mouseWorldPosition + changeInPosition;
			}
			cameraTransform.rotation = Quaternion.Euler(0f, 0f, this.DebugRotation);
		}

		// Token: 0x06002728 RID: 10024 RVA: 0x000A706C File Offset: 0x000A526C
		public void SetDebugZoom(float zoom)
		{
			this._camera.OrthographicSize = zoom;
			this._debugControlsZoomOffset = zoom - this.DesiredZoom;
		}

		// Token: 0x06002729 RID: 10025 RVA: 0x000A7088 File Offset: 0x000A5288
		public void UpdateMaxZoom()
		{
			RectFixed playableArea = this._city.GetClientPlayableAreaAtZoom((Fix64)this.FixedZoom, City.PlayableAreaRoundingType.AllowPartialTiles);
			this.UpdateMaxZoom(playableArea);
		}

		// Token: 0x0600272A RID: 10026 RVA: 0x000A70B4 File Offset: 0x000A52B4
		private void UpdateMaxZoom(RectFixed playableArea)
		{
			Vector2 screenSize = new Vector2(0f, 0f);
			new Vector2(0f, 0f);
			GameObject transformObject = this._gameUI.playableArea.gameObject;
			while (transformObject != null)
			{
				CanvasScaler canvasScaler;
				if (transformObject.TryGetComponent<CanvasScaler>(out canvasScaler))
				{
					screenSize = transformObject.GetComponent<RectTransform>().rect.size;
					break;
				}
				transformObject = transformObject.transform.parent.gameObject;
			}
			Vector2 playableAreaSize = this._gameUI.playableArea.rect.size;
			Vector2 playableToScreenScale = screenSize / playableAreaSize;
			float orthographicHeight = (float)playableArea.height * playableToScreenScale.y;
			float orthographicHeightScaledFromWidth = (float)playableArea.width * playableToScreenScale.x * (screenSize.y / screenSize.x);
			this.MaxZoom = Mathf.Max(orthographicHeight, orthographicHeightScaledFromWidth) * 0.5f;
		}

		// Token: 0x0600272B RID: 10027 RVA: 0x000A71A0 File Offset: 0x000A53A0
		public float GetInterpolationSpeed()
		{
			switch (this._interpolationType)
			{
			case CameraView.CameraInterpType.Default:
				return 0.98f;
			case CameraView.CameraInterpType.PlayerPanning:
				return 0.98f;
			case CameraView.CameraInterpType.PlayerZooming:
				return 0.98f;
			case CameraView.CameraInterpType.TileFocus:
				return 0.35f;
			case CameraView.CameraInterpType.PanToAlign:
				return 0.1f;
			case CameraView.CameraInterpType.Resetting:
				return 0.35f;
			default:
				return 0.98f;
			}
		}

		// Token: 0x0600272C RID: 10028 RVA: 0x000A7200 File Offset: 0x000A5400
		public Vector2 GetCameraOffsetToFitPlayableArea(float zoom, Vector3[] uiPlayableAreaCorners)
		{
			Vector2 playablePosition = uiPlayableAreaCorners[0];
			Vector2 playableScale = new Vector2(uiPlayableAreaCorners[2].x - uiPlayableAreaCorners[0].x, uiPlayableAreaCorners[2].y - uiPlayableAreaCorners[0].y);
			Vector2 playableCenter = new Vector2(playablePosition.x + playableScale.x * 0.5f, playablePosition.y + playableScale.y * 0.5f);
			this._gameUI.GetRectTransform().GetWorldCorners(CameraView.ParentCorners);
			Vector2 parentPosition = CameraView.ParentCorners[0];
			Vector2 parentScale = new Vector2(CameraView.ParentCorners[2].x - CameraView.ParentCorners[0].x, CameraView.ParentCorners[2].y - CameraView.ParentCorners[0].y);
			Vector2 a = new Vector2(parentPosition.x + parentScale.x * 0.5f, parentPosition.y + parentScale.y * 0.5f);
			float offsetScale = this.playerOrthoZoom * 2f / parentScale.x;
			return (a - playableCenter) * offsetScale;
		}

		// Token: 0x0600272D RID: 10029 RVA: 0x000A7340 File Offset: 0x000A5540
		public void ResetPlayerViewport()
		{
			this._interpolationType = CameraView.CameraInterpType.Resetting;
			this.playerOrthoZoom = this.MaxZoom;
			this.playerZoomedIn = false;
			this._panX.RawValue = 0f;
			this._panY.RawValue = 0f;
			this.IsFocussedIn = false;
			this._editMenuFocusPoint = Vector3.zero;
			this._isPlayerPanning = false;
			this._setPanToCenter = false;
			this._includeSafeAreaOffsetInZoom = false;
			Action onCameraZoomLevelChanged = this.OnCameraZoomLevelChanged;
			if (onCameraZoomLevelChanged == null)
			{
				return;
			}
			onCameraZoomLevelChanged();
		}

		// Token: 0x0600272E RID: 10030 RVA: 0x000A73C0 File Offset: 0x000A55C0
		public void ApplyPlayerPanPosition(Vector2 worldPosition, Vector2 screenPosition)
		{
			this._interpolationType = CameraView.CameraInterpType.PlayerPanning;
			Vector2 screenSize = new Vector2(this._camera.Width, this._camera.Height);
			Vector2 offsetFromCentre = screenPosition - screenSize * 0.5f;
			Vector2 rawPan = worldPosition - offsetFromCentre * (this._camera.OrthographicSize / (screenSize.y * 0.5f));
			this._panX.RawValue = rawPan.x;
			this._panX.Hold();
			this._panY.RawValue = rawPan.y;
			this._panY.Hold();
			this._isPlayerPanning = true;
		}

		// Token: 0x0600272F RID: 10031 RVA: 0x000A7467 File Offset: 0x000A5667
		public void ReleasePlayerPan()
		{
			if (this._editMenuFocusPointApplied)
			{
				this.FocusOnWorldPositionWithoutZoom(this._editMenuFocusPoint, CameraView.CameraFocusOffsetType.FocusOnMiddle);
				return;
			}
			this._panX.SpringBackToExtents();
			this._panY.SpringBackToExtents();
		}

		// Token: 0x06002730 RID: 10032 RVA: 0x000A7498 File Offset: 0x000A5698
		public void SetCinematicZoomLevel(int newZoomLevel)
		{
			if (!this.IsInCinematicMode)
			{
				return;
			}
			if (newZoomLevel < 0 || newZoomLevel > this.ZoomLevelCount - 1)
			{
				this._cinematicZoomIndex = Mathf.Clamp(newZoomLevel, 0, this.ZoomLevelCount - 1);
				return;
			}
			this._cinematicZoomIndex = newZoomLevel;
			this.playerOrthoZoom = this._visualConstantsData.ZoomLevelsCameraMin[this._cinematicZoomIndex];
			this.playerOrthoZoom = Mathf.Min(this.playerOrthoZoom, this.MinZoom);
		}

		// Token: 0x06002731 RID: 10033 RVA: 0x000A7510 File Offset: 0x000A5710
		public void FocusOnWorldPosition(Vector3 focusPosition, CameraView.CameraFocusOffsetType offsetType = CameraView.CameraFocusOffsetType.FocusOnMiddle)
		{
			this._interpolationType = CameraView.CameraInterpType.TileFocus;
			CameraView.Log.Info("Focusing on world position {0}, on tile {1}", new object[]
			{
				focusPosition,
				this._tilemapView.GetTileCoordinatesFromWorldPosition(focusPosition)
			});
			this.playerOrthoZoom = Mathf.Min(this.playerOrthoZoom, this.MinZoom);
			if (!this.playerZoomedIn)
			{
				Action onCameraZoomLevelChanged = this.OnCameraZoomLevelChanged;
				if (onCameraZoomLevelChanged != null)
				{
					onCameraZoomLevelChanged();
				}
			}
			this.playerZoomedIn = true;
			this.IsFocussedIn = true;
			if (this.playerOrthoZoom < this.MinZoom)
			{
				this._setPanToCenter = true;
				this._includeSafeAreaOffsetInZoom = true;
			}
			else if (offsetType == CameraView.CameraFocusOffsetType.FocusOnMiddle)
			{
				this._panX.RawValue = focusPosition.x;
				this._panY.RawValue = focusPosition.y;
				this._setPanToCenter = false;
				this._includeSafeAreaOffsetInZoom = false;
			}
			else if (offsetType == CameraView.CameraFocusOffsetType.MaintainScreenPosition)
			{
				Vector2 focusPointScreenPosition = this._camera.GetScreenFromWorld(focusPosition);
				float prevOrthoSize = this._camera.OrthographicSize;
				Vector3 prevCameraPosition = this._camera.transform.position;
				this._camera.OrthographicSize = this.MinZoom;
				this._camera.SetPosition(focusPosition);
				Vector3 focusPointZoomedOffset = this._camera.GetWorldFromScreen(focusPointScreenPosition) - focusPosition;
				this._camera.OrthographicSize = prevOrthoSize;
				this._camera.SetPosition(prevCameraPosition);
				Vector2 rawPan = focusPosition - focusPointZoomedOffset;
				this._panX.RawValue = rawPan.x;
				this._panY.RawValue = rawPan.y;
				this._setPanToCenter = false;
				this._includeSafeAreaOffsetInZoom = false;
			}
			this._panX.Hold();
			this._panY.Hold();
		}

		// Token: 0x06002732 RID: 10034 RVA: 0x000A76C4 File Offset: 0x000A58C4
		public void SetEditMenuFocusPoint(Vector3 focusPosition)
		{
			this._editMenuFocusPoint = focusPosition;
			if (this._editMenuFocusPointApplied)
			{
				this.FocusOnWorldPositionWithoutZoom(focusPosition, CameraView.CameraFocusOffsetType.FocusOnMiddle);
				return;
			}
			this._interpolationType = CameraView.CameraInterpType.TileFocus;
			this.IsFocussedIn = false;
			this._panX.RawValue = 0f;
			this._panY.RawValue = 0f;
		}

		// Token: 0x06002733 RID: 10035 RVA: 0x000A7718 File Offset: 0x000A5918
		public void FocusOnWorldPositionWithoutZoom(Vector3 focusPosition, CameraView.CameraFocusOffsetType offsetType = CameraView.CameraFocusOffsetType.FocusOnMiddle)
		{
			this._interpolationType = CameraView.CameraInterpType.TileFocus;
			CameraView.Log.Info("Focusing on world position {0}, on tile {1}", new object[]
			{
				focusPosition,
				this._tilemapView.GetTileCoordinatesFromWorldPosition(focusPosition)
			});
			this.playerZoomedIn = false;
			this.IsFocussedIn = true;
			this._includeSafeAreaOffsetInZoom = false;
			if (offsetType == CameraView.CameraFocusOffsetType.FocusOnMiddle)
			{
				this._panX.RawValue = focusPosition.x;
				this._panY.RawValue = focusPosition.y;
			}
			else if (offsetType == CameraView.CameraFocusOffsetType.MaintainScreenPosition)
			{
				Vector2 focusPointScreenPosition = this._camera.GetScreenFromWorld(focusPosition);
				Vector3 prevCameraPosition = this._camera.transform.position;
				this._camera.SetPosition(focusPosition);
				Vector3 focusPointZoomedOffset = this._camera.GetWorldFromScreen(focusPointScreenPosition) - focusPosition;
				this._camera.SetPosition(prevCameraPosition);
				Vector2 rawPan = focusPosition - focusPointZoomedOffset;
				this._panX.RawValue = rawPan.x;
				this._panY.RawValue = rawPan.y;
			}
			this._panX.Hold();
			this._panY.Hold();
		}

		// Token: 0x06002734 RID: 10036 RVA: 0x000A7831 File Offset: 0x000A5A31
		public void OnCurrentDeviceInputTypeChanged(DeviceInputType newInputType)
		{
			if (newInputType != DeviceInputType.Touch && this.IsFocussedIn)
			{
				this.ResetPlayerViewport();
			}
		}

		// Token: 0x040020FC RID: 8444
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x040020FD RID: 8445
		[Dependency]
		private City _city;

		// Token: 0x040020FE RID: 8446
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x040020FF RID: 8447
		[Dependency]
		private TilemapView _tilemapView;

		// Token: 0x04002100 RID: 8448
		[Dependency]
		private ClockModel _clock;

		// Token: 0x04002101 RID: 8449
		[Dependency]
		private GameCamera _camera;

		// Token: 0x04002102 RID: 8450
		[Dependency]
		private GameUIScreen _gameUI;

		// Token: 0x04002103 RID: 8451
		[Dependency]
		private InputState _inputState;

		// Token: 0x04002104 RID: 8452
		[Dependency]
		private ScreenStack _screenStack;

		// Token: 0x04002105 RID: 8453
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04002106 RID: 8454
		[Dependency]
		private VisualConstantsData _visualConstantsData;

		// Token: 0x0400210C RID: 8460
		private bool _isFocussedIn;

		// Token: 0x0400210F RID: 8463
		public static int potentialTapZoomIndex = 0;

		// Token: 0x04002110 RID: 8464
		public static List<float> potentialTapZoomLevels = new List<float>
		{
			-1f,
			13f,
			12f,
			11f,
			10f,
			9f,
			8f,
			7f,
			6f
		};

		// Token: 0x04002111 RID: 8465
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("CameraView");

		// Token: 0x04002112 RID: 8466
		private CameraView.CameraInterpType _interpolationType;

		// Token: 0x04002113 RID: 8467
		private InertialFloat _panX;

		// Token: 0x04002114 RID: 8468
		private InertialFloat _panY;

		// Token: 0x04002115 RID: 8469
		private Vector3 _editMenuFocusPoint;

		// Token: 0x04002116 RID: 8470
		public float playerOrthoZoom = -1f;

		// Token: 0x04002117 RID: 8471
		public bool playerZoomedIn;

		// Token: 0x04002118 RID: 8472
		private bool _isPlayerPanning;

		// Token: 0x04002119 RID: 8473
		private bool _setPanToCenter;

		// Token: 0x0400211A RID: 8474
		private bool _includeSafeAreaOffsetInZoom;

		// Token: 0x0400211B RID: 8475
		private const float _defaultCameraMoveSpeed = 0.98f;

		// Token: 0x0400211C RID: 8476
		private const float _defaultCameraZoomSpeed = 0.98f;

		// Token: 0x0400211D RID: 8477
		private const float _tileFocusCameraMoveSpeed = 0.35f;

		// Token: 0x0400211E RID: 8478
		private const float _panToAlignCameraMoveSpeed = 0.1f;

		// Token: 0x0400211F RID: 8479
		private const float _playerPanCameraMoveSpeed = 0.98f;

		// Token: 0x04002120 RID: 8480
		private const float _playableAreaWidthScaleToConstrainCamera = 0.6f;

		// Token: 0x04002121 RID: 8481
		private const float _playableAreaHeightScaleToConstrainCamera = 0.5f;

		// Token: 0x04002122 RID: 8482
		private bool _hasControlOverriden;

		// Token: 0x04002123 RID: 8483
		private int _cinematicZoomIndex;

		// Token: 0x04002124 RID: 8484
		private VehicleView _cinematicModeVehicleToFollow;

		// Token: 0x04002125 RID: 8485
		private TrainView _cinematicModeTrainToFollow;

		// Token: 0x04002126 RID: 8486
		private CameraView.CinematicModeState _cinematicModeState;

		// Token: 0x04002127 RID: 8487
		private float _cinematicModeSpeed = 1f;

		// Token: 0x04002128 RID: 8488
		private float _durationSpentOnCurrentMode;

		// Token: 0x04002129 RID: 8489
		private float _debugControlsZoomOffset;

		// Token: 0x0400212A RID: 8490
		private Vector2 _debugControlsPanOffset = Vector2.zero;

		// Token: 0x0400212B RID: 8491
		private Vector3 _debugControlsLastMouseWorldPosition = Vector3.zero;

		// Token: 0x0400212C RID: 8492
		private const float RotationStepDegrees = 15f;

		// Token: 0x0400212D RID: 8493
		private float _debugRotation;

		// Token: 0x0400212E RID: 8494
		private AnchoredMessageModel _cinematicModeNoCarsMessage;

		// Token: 0x0400212F RID: 8495
		private static readonly Vector3[] PlayableCorners = new Vector3[4];

		// Token: 0x04002130 RID: 8496
		private static readonly Vector2 MessageAnchorOffset = new Vector2(0f, 0.8f);

		// Token: 0x04002131 RID: 8497
		private static readonly Vector3[] ParentCorners = new Vector3[4];

		// Token: 0x02000588 RID: 1416
		public enum CameraInterpType
		{
			// Token: 0x04002133 RID: 8499
			Default,
			// Token: 0x04002134 RID: 8500
			PlayerPanning,
			// Token: 0x04002135 RID: 8501
			PlayerZooming,
			// Token: 0x04002136 RID: 8502
			TileFocus,
			// Token: 0x04002137 RID: 8503
			PanToAlign,
			// Token: 0x04002138 RID: 8504
			Resetting
		}

		// Token: 0x02000589 RID: 1417
		public enum CinematicModeState
		{
			// Token: 0x0400213A RID: 8506
			Off,
			// Token: 0x0400213B RID: 8507
			AwaitingJourney,
			// Token: 0x0400213C RID: 8508
			OnJourney,
			// Token: 0x0400213D RID: 8509
			CompletedJourney,
			// Token: 0x0400213E RID: 8510
			FollowingTrain,
			// Token: 0x0400213F RID: 8511
			ExitingMode
		}

		// Token: 0x0200058A RID: 1418
		public enum CameraFocusOffsetType
		{
			// Token: 0x04002141 RID: 8513
			FocusOnMiddle,
			// Token: 0x04002142 RID: 8514
			MaintainScreenPosition
		}
	}
}
