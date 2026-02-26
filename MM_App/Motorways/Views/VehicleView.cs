using System;
using System.Collections.Generic;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Audio;
using Motorways.Constants;
using Motorways.Models;
using Motorways.Processes;
using Motorways.Themes;
using Server;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000612 RID: 1554
	[SelectionBase]
	public class VehicleView : MonoBehaviour, IView, IAudioView, VehicleModel.IObserver, IReusable, IReleasedFromScopeHandler, IThemeComponent
	{
		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x06002B77 RID: 11127 RVA: 0x000BFC06 File Offset: 0x000BDE06
		public int Id
		{
			get
			{
				return this._id;
			}
		}

		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x06002B78 RID: 11128 RVA: 0x000BFC0E File Offset: 0x000BDE0E
		// (set) Token: 0x06002B79 RID: 11129 RVA: 0x000BFC16 File Offset: 0x000BDE16
		[Dependency]
		public City City { get; private set; }

		// Token: 0x1700074F RID: 1871
		// (set) Token: 0x06002B7A RID: 11130 RVA: 0x000BFC1F File Offset: 0x000BDE1F
		public Renderer CombinedMeshVehicleRenderer
		{
			set
			{
				this._combinedMeshVehicleRenderer = value;
			}
		}

		// Token: 0x17000750 RID: 1872
		// (get) Token: 0x06002B7B RID: 11131 RVA: 0x000BFC28 File Offset: 0x000BDE28
		// (set) Token: 0x06002B7C RID: 11132 RVA: 0x000BFC3A File Offset: 0x000BDE3A
		public bool IsTrailActive
		{
			get
			{
				return this._trail.gameObject.activeInHierarchy;
			}
			set
			{
				this._trail.gameObject.SetActive(value);
			}
		}

		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x06002B7D RID: 11133 RVA: 0x000BFC4D File Offset: 0x000BDE4D
		public bool IsPendingDeletion
		{
			get
			{
				return this._isPendingDeletion;
			}
		}

		// Token: 0x17000752 RID: 1874
		// (get) Token: 0x06002B7E RID: 11134 RVA: 0x000BFC55 File Offset: 0x000BDE55
		public float CenterToWheelDistance
		{
			get
			{
				return (float)VehicleMovementProcess.VehicleLength * 0.3f;
			}
		}

		// Token: 0x06002B7F RID: 11135 RVA: 0x000BFC68 File Offset: 0x000BDE68
		private void Awake()
		{
			VehicleView._carSortingLayerId = SortingLayer.NameToID("Car");
			VehicleView._carHeadlightSortingLayerId = SortingLayer.NameToID("Headlight");
			VehicleView._motorwayCarSortingLayerId = SortingLayer.NameToID("MotorwayCar");
			this._shadowTransform = this._vehicleShadowRenderer.transform;
		}

		// Token: 0x06002B80 RID: 11136 RVA: 0x000BFCA8 File Offset: 0x000BDEA8
		private void Initialize(VehicleModel model)
		{
			this._id = VehicleView._nextId;
			VehicleView._nextId++;
			this._vehicleModel = model;
			this._vehicleModel.Subscribe(this);
			this._viewIndex.AddVehicleView(this);
			this._laneCursor = this._scope.Get<LaneCursor>();
			this._lastMidpoint = new Vector3(-9999f, -9999f, 0f);
			this.groupIndex = this._vehicleModel.house.GroupIndex;
			this._theme.RegisterGameObjectToThemeByGroupIndex(base.gameObject, this.groupIndex);
			this._previousIsNightModeEnabled = this._activePlayer.IsNightModeEnabled;
			this._headlightState = (this._activePlayer.IsNightModeEnabled ? VehicleView.HeadlightState.On : VehicleView.HeadlightState.Off);
			this.SetHeadlightsOn(this._activePlayer.IsNightModeEnabled && !this._vehicleModel.IsWaitingAtHouse);
			Bounds bodyBounds = this._combinedMeshVehicleRenderer.bounds;
			this._bodyAndHeadlightBounds = new Bounds(bodyBounds.center, bodyBounds.size);
			this._bodyAndHeadlightBounds.Encapsulate(this._vehicleHeadlightBeams.bounds);
			MaterialPropertyBlock materialPropertyBlock = this._theme.MaterialPropertyBlock;
			this._combinedMeshVehicleRenderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetInt(ShaderConstants.GroupId, this.groupIndex);
			this._combinedMeshVehicleRenderer.SetPropertyBlock(materialPropertyBlock);
			if (FeatureToggle.IsFeatureDisabled(Feature.VehicleTrails))
			{
				this.IsTrailActive = false;
			}
			RoadTileConnection laneConnection = model.CurrentFrame.lane.connection;
			this._onMotorwayId = ((laneConnection.input.type == RoadType.Motorway && laneConnection.output.type != RoadType.Motorway) ? laneConnection.input.motorwayId : laneConnection.output.motorwayId);
			this._audioSystem.ScheduleEvent(AudioEvent.CreateVehicleEvent((AudioEventType)((ulong)int.MinValue), this, this.House, this.Destination, null));
			if (FeatureToggle.IsFeatureEnabled(Feature.WhatTheCarEasterEgg) && this._scope.Get<EasterEggModel>().ShouldBeEasterEggVehicle(this._vehicleModel))
			{
				this._tribandVehicleEffects = this._scope.Get<TribandVehicleEffects>();
				this._tribandVehicleEffects.transform.SetParent(base.transform, false);
			}
		}

		// Token: 0x06002B81 RID: 11137 RVA: 0x000BFED4 File Offset: 0x000BE0D4
		public void SetNewGroupIndex(int newGroupIndex)
		{
			this._theme.UnregisterGameObjectFromThemeByGroupIndex(base.gameObject, this.groupIndex);
			this.groupIndex = newGroupIndex;
			MaterialPropertyBlock materialPropertyBlock = this._theme.MaterialPropertyBlock;
			this._combinedMeshVehicleRenderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetInt(ShaderConstants.GroupId, this.groupIndex);
			this._combinedMeshVehicleRenderer.SetPropertyBlock(materialPropertyBlock);
			this._theme.RegisterGameObjectToThemeByGroupIndex(base.gameObject, this.groupIndex);
		}

		// Token: 0x17000753 RID: 1875
		// (get) Token: 0x06002B82 RID: 11138 RVA: 0x000BFF4C File Offset: 0x000BE14C
		public VehicleModel Model
		{
			get
			{
				return this._vehicleModel;
			}
		}

		// Token: 0x17000754 RID: 1876
		// (get) Token: 0x06002B83 RID: 11139 RVA: 0x000BFF54 File Offset: 0x000BE154
		public List<LineSegment> PreviousLaneSegments
		{
			get
			{
				return this._previousLaneSegments;
			}
		}

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x06002B84 RID: 11140 RVA: 0x000BFF5C File Offset: 0x000BE15C
		public HouseView House
		{
			get
			{
				return this._viewIndex.GetHouseView(this._vehicleModel.house);
			}
		}

		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x06002B85 RID: 11141 RVA: 0x000BFF74 File Offset: 0x000BE174
		public DestinationView Destination
		{
			get
			{
				if (this._vehicleModel.destination != null)
				{
					return this._viewIndex.GetDestinationView(this._vehicleModel.destination);
				}
				if (this._vehicleModel.lastVisitedDestination == null)
				{
					return null;
				}
				return this._viewIndex.GetDestinationView(this._vehicleModel.lastVisitedDestination);
			}
		}

		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x06002B86 RID: 11142 RVA: 0x000BFFCA File Offset: 0x000BE1CA
		public float Speed
		{
			get
			{
				return this._speed;
			}
		}

		// Token: 0x06002B87 RID: 11143 RVA: 0x000BFFD4 File Offset: 0x000BE1D4
		public void Reset()
		{
			this.groupIndex = int.MinValue;
			this._id = -1;
			this._lastMidpoint = Vector3.zero;
			this._speed = 0f;
			this._previousLaneSegments.Clear();
			this._distanceToGoal = 0f;
			this._isInTunnel = false;
			this._onMotorwayId = -1;
			this._boundSortingLayerId = int.MinValue;
			this._boundMotorwayId = -1;
			this._audioVehicle = null;
			this._pan = Vector2.zero;
			this._attenuation = 0f;
			this._isPendingDeletion = false;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this._bodyAndHeadlightBounds = default(Bounds);
			this._headlightResponseTime = -1f;
			this._nightModeEnabledChangedTime = -1f;
			this._previousIsNightModeEnabled = false;
			this._headlightState = VehicleView.HeadlightState.Off;
			this.SkipHeadlightResponseTime = false;
		}

		// Token: 0x06002B88 RID: 11144 RVA: 0x000C00BC File Offset: 0x000BE2BC
		public void OnReleasedFromScope(IScope scope)
		{
			this._theme.UnregisterGameObjectFromThemeByGroupIndex(base.gameObject, this.groupIndex);
			this._viewIndex.RemoveVehicleView(this);
			if (this._vehicleModel != null)
			{
				this._vehicleModel.Unsubscribe(this);
				this._vehicleModel = null;
			}
			if (this._laneCursor != null)
			{
				scope.Release(this._laneCursor);
				this._laneCursor = null;
			}
			if (this._tribandVehicleEffects != null)
			{
				scope.Release(this._tribandVehicleEffects);
			}
		}

		// Token: 0x06002B89 RID: 11145 RVA: 0x000C0140 File Offset: 0x000BE340
		private void SetSortingInfo(int carSortingLayerId, int headlightSortingLayerId, int motorwayId)
		{
			if (this._boundSortingLayerId != carSortingLayerId)
			{
				this.SetVehicleComponentRenderSorting(this._vehicleFrontLightRight, headlightSortingLayerId, 9);
				this.SetVehicleComponentRenderSorting(this._vehicleFrontLightLeft, headlightSortingLayerId, 9);
				this.SetVehicleComponentRenderSorting(this._vehicleHeadlightBeams, headlightSortingLayerId, 8);
				this.SetVehicleComponentRenderSorting(this._trail.Renderer, carSortingLayerId, 6);
				this.SetVehicleComponentRenderSorting(this._combinedMeshVehicleRenderer, carSortingLayerId, 6);
				this.SetVehicleComponentRenderSorting(this._vehicleShadowRenderer, carSortingLayerId, 5);
				this._boundSortingLayerId = carSortingLayerId;
			}
			if (this._boundMotorwayId != motorwayId)
			{
				MaterialPropertyBlock materialPropertyBlock = this._theme.MaterialPropertyBlock;
				this._combinedMeshVehicleRenderer.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat(ShaderConstants.MotorwayIdShaderId, (float)motorwayId);
				materialPropertyBlock.SetFloat(ShaderConstants.HeadlightOcclusionTypeId, (float)this._vehicleModel.id);
				this._combinedMeshVehicleRenderer.SetPropertyBlock(materialPropertyBlock);
				this._vehicleHeadlightBeams.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat(ShaderConstants.MotorwayIdShaderId, (float)motorwayId);
				materialPropertyBlock.SetFloat(ShaderConstants.HeadlightOcclusionTypeId, (float)this._vehicleModel.id);
				this._vehicleHeadlightBeams.SetPropertyBlock(materialPropertyBlock);
				this._vehicleFrontLightLeft.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat(ShaderConstants.MotorwayIdShaderId, (float)motorwayId);
				materialPropertyBlock.SetFloat(ShaderConstants.HeadlightOcclusionTypeId, (float)this._vehicleModel.id);
				this._vehicleFrontLightLeft.SetPropertyBlock(materialPropertyBlock);
				this._vehicleFrontLightRight.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat(ShaderConstants.MotorwayIdShaderId, (float)motorwayId);
				materialPropertyBlock.SetFloat(ShaderConstants.HeadlightOcclusionTypeId, (float)this._vehicleModel.id);
				this._vehicleFrontLightRight.SetPropertyBlock(materialPropertyBlock);
				this._vehicleShadowRenderer.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat(ShaderConstants.MotorwayIdShaderId, (float)motorwayId);
				this._vehicleShadowRenderer.SetPropertyBlock(materialPropertyBlock);
				this._trail.Renderer.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat(ShaderConstants.MotorwayIdShaderId, (float)motorwayId);
				this._trail.Renderer.SetPropertyBlock(materialPropertyBlock);
				this._boundMotorwayId = motorwayId;
			}
		}

		// Token: 0x06002B8A RID: 11146 RVA: 0x000C031B File Offset: 0x000BE51B
		private void SetVehicleComponentRenderSorting(Renderer vehiclePart, int layerId, int sortingOrder)
		{
			vehiclePart.sortingLayerID = layerId;
			vehiclePart.sortingOrder = sortingOrder;
		}

		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x06002B8B RID: 11147 RVA: 0x000C032B File Offset: 0x000BE52B
		// (set) Token: 0x06002B8C RID: 11148 RVA: 0x000C0333 File Offset: 0x000BE533
		public Vehicle AudioVehicle
		{
			get
			{
				return this._audioVehicle;
			}
			set
			{
				if (this._audioVehicle == value)
				{
					return;
				}
				Vehicle audioVehicle = this._audioVehicle;
				if (audioVehicle != null)
				{
					Vehicle.AudioMotor motor = audioVehicle.Motor;
					if (motor != null)
					{
						motor.FadeOutAndStop(1.0);
					}
				}
				this._audioVehicle = value;
			}
		}

		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x06002B8D RID: 11149 RVA: 0x000C036B File Offset: 0x000BE56B
		public bool IsInTunnel
		{
			get
			{
				return this._isInTunnel;
			}
		}

		// Token: 0x06002B8E RID: 11150 RVA: 0x000C0374 File Offset: 0x000BE574
		private float CalculateHeadlightResponseTime()
		{
			float u = 1f - UnityEngine.Random.Range(0f, 1f);
			float u2 = 1f - UnityEngine.Random.Range(0f, 1f);
			float randStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u)) * Mathf.Sin(6.2831855f * u2);
			float randNormal = this._visualConstants.MeanVehicleHeadlightResponseTime + this._visualConstants.StandardDeviationVehicleHeadlightResponseTime * randStdNormal;
			return Mathf.Max(0f, randNormal);
		}

		// Token: 0x06002B8F RID: 11151 RVA: 0x000C03F1 File Offset: 0x000BE5F1
		private void SetHeadlightsOn(bool isOn)
		{
			this._vehicleHeadlightBeams.gameObject.SetActive(isOn);
			this._vehicleFrontLightLeft.gameObject.SetActive(isOn);
			this._vehicleFrontLightRight.gameObject.SetActive(isOn);
		}

		// Token: 0x06002B90 RID: 11152 RVA: 0x000C0428 File Offset: 0x000BE628
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._tribandVehicleEffects != null)
			{
				this._tribandVehicleEffects.Tick(timeInterval, stepAlpha);
			}
			this.UpdateHeadlightState();
			if (this._isPendingDeletion)
			{
				return TickResult.Destroy;
			}
			this._laneCursor.MoveToVehicle(this, stepAlpha);
			Vector3 midpoint = this._laneCursor.Position;
			if (!Mathf.Approximately(0f, (midpoint - this._lastMidpoint).sqrMagnitude))
			{
				this._lastMidpoint = midpoint;
				this._laneCursor.Move(this.CenterToWheelDistance);
				Vector3 frontWheelPosition = this._laneCursor.Position;
				Vector3 backWheelPosition;
				if (!this._laneCursor.MoveAlongRadius(this.CenterToWheelDistance * -2f, out backWheelPosition))
				{
					backWheelPosition = frontWheelPosition + (midpoint - frontWheelPosition).normalized * (this.CenterToWheelDistance * 2f);
				}
				Vector3 vehiclePosition = Vector3.Lerp(backWheelPosition, frontWheelPosition, 0.5f);
				base.transform.localPosition = new Vector3(vehiclePosition.x, vehiclePosition.y, -6.1f);
				Vector2 direction = (frontWheelPosition - backWheelPosition).normalized;
				Vector2 right = new Vector2(direction.y, -direction.x);
				float qw = Mathf.Sqrt(1f + right.x + direction.y + 1f) * 0.5f;
				float qz;
				if (!Mathf.Approximately(qw, 0f))
				{
					qz = (right.y - direction.x) / (4f * qw);
					float length = Mathf.Sqrt(qz * qz + qw * qw);
					qz /= length;
					qw /= length;
				}
				else
				{
					qz = -1f;
					qw = 0f;
				}
				Quaternion vehicleRotation = new Quaternion(0f, 0f, qz, qw);
				base.transform.localRotation = vehicleRotation;
				this._shadowTransform.localPosition = Quaternion.Inverse(vehicleRotation) * ((Vector3.right + Vector3.down) * this.shadowOffsetScale);
			}
			int previousMotorwayId = this._onMotorwayId;
			int newMotorwayId;
			VehicleView.MotorwayState motorwayState = this.UpdateMotorwayInformation(out newMotorwayId);
			if (newMotorwayId != -1)
			{
				this._onMotorwayId = newMotorwayId;
				this.SetSortingInfo(VehicleView._motorwayCarSortingLayerId, VehicleView._motorwayCarSortingLayerId, this._onMotorwayId);
			}
			else
			{
				this._onMotorwayId = -1;
				this.SetSortingInfo(VehicleView._carSortingLayerId, VehicleView._carHeadlightSortingLayerId, 0);
			}
			if (this._onMotorwayId == -1)
			{
				Vector2Int tileCoordinates = this._tilemapView.GetTileCoordinatesFromWorldPosition(midpoint);
				this._isInTunnel = this.City.Definition.TileIsUnderAMountain(tileCoordinates);
			}
			if (motorwayState == VehicleView.MotorwayState.EnteringMotorway && this._onMotorwayId >= 0)
			{
				Diagnostics.Log.Info("VehicleView", "VehicleEnteredMotorway", Array.Empty<object>());
				this._audioSystem.ScheduleEvent(AudioEvent.CreateVehicleEvent(AudioEventType.VehicleEnteredMotorway, this, null, null, this._tilemapView.GetMotorwayView(this._onMotorwayId)));
			}
			else if (motorwayState == VehicleView.MotorwayState.LeavingMotorway && previousMotorwayId >= 0)
			{
				Diagnostics.Log.Info("VehicleView", "VehicleLeftMotorway", Array.Empty<object>());
				this._audioSystem.ScheduleEvent(AudioEvent.CreateVehicleEvent(AudioEventType.VehicleLeftMotorway, this, null, null, this._tilemapView.GetMotorwayView(previousMotorwayId)));
			}
			this._distanceToGoal = this.CalculateDistanceToGoal(stepAlpha);
			Vector2 screenPos = this._gameCamera.GetScreenFromWorld(base.transform.position);
			this._pan = this._gameCamera.GetPanFromScreen(screenPos);
			this._attenuation = this._gameCamera.GetAttenuationFromScreen(screenPos, true, 5f);
			this._speed = (float)this._vehicleModel.CurrentFrame.speed + (float)(this._vehicleModel.NextFrame.speed - this._vehicleModel.CurrentFrame.speed) * stepAlpha;
			if (this.IsTrailActive)
			{
				this._trail.Tick(timeInterval.UnpausedScaledDelta);
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002B91 RID: 11153 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002B92 RID: 11154 RVA: 0x000C0800 File Offset: 0x000BEA00
		private void UpdateHeadlightState()
		{
			if (this._previousIsNightModeEnabled != this._activePlayer.IsNightModeEnabled)
			{
				this._nightModeEnabledChangedTime = (float)this._clockModel.Time;
				this._headlightResponseTime = this.CalculateHeadlightResponseTime();
				VehicleView.HeadlightState newState = this._activePlayer.IsNightModeEnabled ? VehicleView.HeadlightState.TurningOn : VehicleView.HeadlightState.TurningOff;
				if (newState == VehicleView.HeadlightState.TurningOn && (this._headlightState == VehicleView.HeadlightState.On || this._headlightState == VehicleView.HeadlightState.TurningOff))
				{
					newState = VehicleView.HeadlightState.On;
				}
				else if (newState == VehicleView.HeadlightState.TurningOff && (this._headlightState == VehicleView.HeadlightState.Off || this._headlightState == VehicleView.HeadlightState.TurningOn))
				{
					newState = VehicleView.HeadlightState.Off;
				}
				this._headlightState = newState;
				this._previousIsNightModeEnabled = this._activePlayer.IsNightModeEnabled;
			}
			if (!this._scope.Get<ISimulation>().IsPaused && this._nightModeEnabledChangedTime > 0f && (float)this._clockModel.Time - this._nightModeEnabledChangedTime > this._headlightResponseTime)
			{
				VehicleView.HeadlightState headlightState = this._headlightState;
				VehicleView.HeadlightState headlightState2;
				if (headlightState != VehicleView.HeadlightState.TurningOff)
				{
					if (headlightState == VehicleView.HeadlightState.TurningOn)
					{
						headlightState2 = VehicleView.HeadlightState.On;
					}
					else
					{
						headlightState2 = this._headlightState;
					}
				}
				else
				{
					headlightState2 = VehicleView.HeadlightState.Off;
				}
				this._headlightState = headlightState2;
				this._headlightResponseTime = -1f;
				this._nightModeEnabledChangedTime = -1f;
			}
			switch (this._headlightState)
			{
			case VehicleView.HeadlightState.Off:
			case VehicleView.HeadlightState.TurningOn:
				this.SetHeadlightsOn(false);
				break;
			case VehicleView.HeadlightState.TurningOff:
			case VehicleView.HeadlightState.On:
				this.SetHeadlightsOn(!this._vehicleModel.IsWaitingAtHouse && !this._vehicleModel.IsParkedAtDestination);
				break;
			}
			if (this.SkipHeadlightResponseTime)
			{
				if (this._headlightState == VehicleView.HeadlightState.TurningOff)
				{
					this.SetHeadlightsOn(false);
					return;
				}
				if (this._headlightState == VehicleView.HeadlightState.TurningOn)
				{
					this.SetHeadlightsOn(!this._vehicleModel.IsWaitingAtHouse && !this._vehicleModel.IsParkedAtDestination);
				}
			}
		}

		// Token: 0x1700075A RID: 1882
		// (get) Token: 0x06002B93 RID: 11155 RVA: 0x000C09AA File Offset: 0x000BEBAA
		// (set) Token: 0x06002B94 RID: 11156 RVA: 0x000C09B2 File Offset: 0x000BEBB2
		public bool SkipHeadlightResponseTime { get; set; }

		// Token: 0x06002B95 RID: 11157 RVA: 0x000C09BC File Offset: 0x000BEBBC
		private VehicleView.MotorwayState UpdateMotorwayInformation(out int newMotorwayId)
		{
			RoadTileConnection currentLaneConnection = this._vehicleModel.CurrentFrame.lane.connection;
			VehicleView.MotorwayState motorwayState;
			if (this._onMotorwayId != -1)
			{
				motorwayState = VehicleView.MotorwayState.OnMotorway;
				newMotorwayId = this._onMotorwayId;
				bool flag = currentLaneConnection.input.type != RoadType.Motorway && currentLaneConnection.output.type != RoadType.Motorway;
				bool previousLaneWasMotorwayVehicleWasJustOn = currentLaneConnection.input.motorwayId == this._onMotorwayId && currentLaneConnection.output.motorwayId != this._onMotorwayId;
				if (flag)
				{
					motorwayState = VehicleView.MotorwayState.LeavingMotorway;
					newMotorwayId = -1;
				}
				else if (previousLaneWasMotorwayVehicleWasJustOn)
				{
					MotorwayGeometryInfo.MotorwayEndEdges motorwayEndEdges;
					if (this._tilemapView.MotorwayGeometryInfo.EndEdges.TryGetValue(this._onMotorwayId, out motorwayEndEdges))
					{
						if (!MotorwayIntersectionUtil.EitherEndEdgeIntersectsBoundingBox(motorwayEndEdges, this.VehicleBounds) && !MotorwayIntersectionUtil.EitherEndEdgeIntersectsBoundingBox(motorwayEndEdges, this._vehicleShadowRenderer.bounds))
						{
							motorwayState = VehicleView.MotorwayState.LeavingMotorway;
							newMotorwayId = -1;
						}
					}
					else
					{
						motorwayState = VehicleView.MotorwayState.NotOnMotorway;
						newMotorwayId = -1;
					}
				}
			}
			else
			{
				motorwayState = VehicleView.MotorwayState.NotOnMotorway;
				newMotorwayId = -1;
				if (currentLaneConnection.input.type == RoadType.Motorway && currentLaneConnection.output.type == RoadType.Motorway)
				{
					newMotorwayId = currentLaneConnection.input.motorwayId;
					motorwayState = VehicleView.MotorwayState.EnteringMotorway;
				}
				else if (this._vehicleModel.path.Count >= 1)
				{
					motorwayState = VehicleView.MotorwayState.NotOnMotorway;
					RoadTileConnection laneConnection = this._vehicleModel.path[0].connection;
					if (laneConnection.input.type == RoadType.Motorway)
					{
						int upcomingMotorwayId = laneConnection.input.motorwayId;
						MotorwayGeometryInfo.MotorwayEndEdges motorwayEndEdges2;
						if (this._tilemapView.MotorwayGeometryInfo.EndEdges.TryGetValue(upcomingMotorwayId, out motorwayEndEdges2) && (MotorwayIntersectionUtil.EitherEndEdgeIntersectsBoundingBox(motorwayEndEdges2, this.VehicleBounds) || MotorwayIntersectionUtil.EitherEndEdgeIntersectsBoundingBox(motorwayEndEdges2, this._vehicleShadowRenderer.bounds)))
						{
							motorwayState = VehicleView.MotorwayState.EnteringMotorway;
							newMotorwayId = upcomingMotorwayId;
						}
					}
				}
			}
			return motorwayState;
		}

		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x06002B96 RID: 11158 RVA: 0x000C0B7C File Offset: 0x000BED7C
		public Bounds VehicleBounds
		{
			get
			{
				if (this._vehicleHeadlightBeams.gameObject.activeInHierarchy)
				{
					Bounds bodyBounds = this._combinedMeshVehicleRenderer.bounds;
					this._bodyAndHeadlightBounds = new Bounds(bodyBounds.center, bodyBounds.size);
					this._bodyAndHeadlightBounds.Encapsulate(this._vehicleHeadlightBeams.bounds);
					return this._bodyAndHeadlightBounds;
				}
				return this._combinedMeshVehicleRenderer.bounds;
			}
		}

		// Token: 0x06002B97 RID: 11159 RVA: 0x000C0BE8 File Offset: 0x000BEDE8
		public void OnVehicleMovedToNewLane(LaneModel newLane, LaneModel oldLane)
		{
			this._previousLaneSegments.Clear();
			if (oldLane != null)
			{
				MotorwayView motorway = this._tilemapView.TryGetMotorwayViewForLane(oldLane);
				if (motorway != null)
				{
					List<Vector2> motorwayLanePoints = motorway.GetLanePoints(oldLane);
					if (motorwayLanePoints != null)
					{
						for (int segmentIndex = 0; segmentIndex < motorwayLanePoints.Count - 1; segmentIndex++)
						{
							this._previousLaneSegments.Add(new LineSegment(motorwayLanePoints[segmentIndex], motorwayLanePoints[segmentIndex + 1]));
						}
						return;
					}
				}
				List<Vector2Fixed> lanePoints = oldLane.lanePoints;
				for (int segmentIndex2 = 0; segmentIndex2 < lanePoints.Count - 1; segmentIndex2++)
				{
					this._previousLaneSegments.Add(new LineSegment((Vector2)lanePoints[segmentIndex2], (Vector2)lanePoints[segmentIndex2 + 1]));
				}
			}
		}

		// Token: 0x06002B98 RID: 11160 RVA: 0x000C0CA5 File Offset: 0x000BEEA5
		public void OnVehicleArrivedAtHouse(VehicleModel vehicleModel)
		{
			this._audioSystem.ScheduleEvent(AudioEvent.CreateVehicleEvent(AudioEventType.VehicleArrivedAtHouse, this, this._viewIndex.GetHouseView(this._vehicleModel.house), null, null));
		}

		// Token: 0x06002B99 RID: 11161 RVA: 0x000C0CD4 File Offset: 0x000BEED4
		public void OnVehicleDepartedHouse(VehicleModel vehicle, DestinationModel toDestination)
		{
			this._audioSystem.ScheduleEvent(AudioEvent.CreateVehicleEvent(AudioEventType.VehicleDepartedHouse, this, this._viewIndex.GetHouseView(this._vehicleModel.house), this._viewIndex.GetDestinationView(this._vehicleModel.destination), null));
		}

		// Token: 0x06002B9A RID: 11162 RVA: 0x000C0D22 File Offset: 0x000BEF22
		public void OnVehicleEnteredCarpark(VehicleModel vehicleModel, DestinationModel destinationModel)
		{
			if (destinationModel != null)
			{
				this._audioSystem.ScheduleEvent(AudioEvent.CreateVehicleEvent(AudioEventType.VehicleEnteredCarpark, this, null, this._viewIndex.GetDestinationView(destinationModel), null));
			}
		}

		// Token: 0x06002B9B RID: 11163 RVA: 0x000C0D4C File Offset: 0x000BEF4C
		public void OnVehicleArrivedAtDestination(VehicleModel vehicleModel, DestinationModel destinationModel)
		{
			if (destinationModel != null)
			{
				this._audioSystem.ScheduleEvent(AudioEvent.CreateVehicleEvent(AudioEventType.VehicleArrivedAtDestination, this, null, this._viewIndex.GetDestinationView(destinationModel), null));
			}
		}

		// Token: 0x06002B9C RID: 11164 RVA: 0x000C0D74 File Offset: 0x000BEF74
		public void OnVehicleDepartedDestination(VehicleModel vehicleModel, DestinationModel fromDestination)
		{
			this._audioSystem.ScheduleEvent(AudioEvent.CreateVehicleEvent(AudioEventType.VehicleDepartedDestination, this, this._viewIndex.GetHouseView(this._vehicleModel.house), this._viewIndex.GetDestinationView(this._vehicleModel.lastVisitedDestination), null));
		}

		// Token: 0x06002B9D RID: 11165 RVA: 0x000C0DC4 File Offset: 0x000BEFC4
		private float CalculateDistanceToGoal(float stepAlpha)
		{
			float num = (float)this._vehicleModel.pathLength;
			float nextFrameLaneLength = VehicleView.GetLaneLength(this._vehicleModel.NextFrame.lane);
			float laneLength = VehicleView.GetLaneLength(this._vehicleModel.CurrentFrame.lane);
			float nextStateDistanceToPath = nextFrameLaneLength - (float)this._vehicleModel.NextFrame.distanceAlongLane;
			float currentStateDistanceToPath = laneLength - (float)this._vehicleModel.CurrentFrame.distanceAlongLane;
			if (this._vehicleModel.CurrentFrame.lane != this._vehicleModel.NextFrame.lane)
			{
				currentStateDistanceToPath += nextFrameLaneLength;
			}
			return num + nextStateDistanceToPath * stepAlpha + currentStateDistanceToPath * (1f - stepAlpha);
		}

		// Token: 0x06002B9E RID: 11166 RVA: 0x000C0E74 File Offset: 0x000BF074
		private static float GetLaneLength(LaneModel lane)
		{
			float length = 0f;
			if (Diagnostics.Verify(lane != null))
			{
				length = (float)lane.Length;
			}
			return length;
		}

		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x06002B9F RID: 11167 RVA: 0x000C0EA0 File Offset: 0x000BF0A0
		public float DistanceToGoal
		{
			get
			{
				return Mathf.Max(0f, this._distanceToGoal);
			}
		}

		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x06002BA0 RID: 11168 RVA: 0x000C0EB2 File Offset: 0x000BF0B2
		public Vector2 Pan
		{
			get
			{
				return this._pan;
			}
		}

		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x06002BA1 RID: 11169 RVA: 0x000C0EBA File Offset: 0x000BF0BA
		public float Attenuation
		{
			get
			{
				return this._attenuation;
			}
		}

		// Token: 0x06002BA2 RID: 11170 RVA: 0x000C0EC2 File Offset: 0x000BF0C2
		public float GetAttenuation(bool zoom, float falloffFactor = 5f)
		{
			return this._gameCamera.GetAttenuationFromWorld(base.transform.position, true, falloffFactor);
		}

		// Token: 0x06002BA3 RID: 11171 RVA: 0x000C0EDC File Offset: 0x000BF0DC
		public void OnRemoved()
		{
			this._isPendingDeletion = true;
		}

		// Token: 0x06002BA4 RID: 11172 RVA: 0x000C0EE8 File Offset: 0x000BF0E8
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
			Theme motorwaysTheme = (Theme)themeDatabase.GetTheme();
			this._trail.Color = motorwaysTheme.GetBuildingColor(this.groupIndex, ThemeComponentGroupTarget.CarBase);
		}

		// Token: 0x06002BA5 RID: 11173 RVA: 0x000C0F1C File Offset: 0x000BF11C
		public void ApplyTheme(ITheme theme)
		{
			Theme motorwaysTheme = (Theme)theme;
			this._trail.Color = motorwaysTheme.GetBuildingColor(this.groupIndex, ThemeComponentGroupTarget.CarBase);
		}

		// Token: 0x06002BA6 RID: 11174 RVA: 0x000C0F48 File Offset: 0x000BF148
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			Theme theme = (Theme)oldTheme;
			Theme motorwaysThemeNew = (Theme)newTheme;
			Color buildingColor = theme.GetBuildingColor(this.groupIndex, ThemeComponentGroupTarget.CarBase);
			Color endColor = motorwaysThemeNew.GetBuildingColor(this.groupIndex, ThemeComponentGroupTarget.CarBase);
			Color desiredColor = Color.Lerp(buildingColor, endColor, progress);
			this._trail.Color = desiredColor;
			return ThemeBlendingResult.ContinueBlending;
		}

		// Token: 0x06002BA7 RID: 11175 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x06002BAA RID: 11178 RVA: 0x000AB22A File Offset: 0x000A942A
		Transform IAudioView.get_transform()
		{
			return base.transform;
		}

		// Token: 0x040025A1 RID: 9633
		public const float VehicleZHeight = -6.1f;

		// Token: 0x040025A2 RID: 9634
		private int _id = -1;

		// Token: 0x040025A3 RID: 9635
		private static int _nextId = 1;

		// Token: 0x040025A4 RID: 9636
		private TribandVehicleEffects _tribandVehicleEffects;

		// Token: 0x040025A6 RID: 9638
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x040025A7 RID: 9639
		[Dependency]
		private ViewIndex _viewIndex;

		// Token: 0x040025A8 RID: 9640
		[Dependency]
		private TilemapView _tilemapView;

		// Token: 0x040025A9 RID: 9641
		[Dependency]
		private IScope _scope;

		// Token: 0x040025AA RID: 9642
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x040025AB RID: 9643
		[Dependency]
		private MotorwaysThemeDatabase _theme;

		// Token: 0x040025AC RID: 9644
		[Dependency]
		private ActivePlayer _activePlayer;

		// Token: 0x040025AD RID: 9645
		[Dependency]
		private ClockModel _clockModel;

		// Token: 0x040025AE RID: 9646
		private LaneCursor _laneCursor;

		// Token: 0x040025AF RID: 9647
		private Vector3 _lastMidpoint;

		// Token: 0x040025B0 RID: 9648
		private Transform _shadowTransform;

		// Token: 0x040025B1 RID: 9649
		private VehicleModel _vehicleModel;

		// Token: 0x040025B2 RID: 9650
		private float _speed;

		// Token: 0x040025B3 RID: 9651
		private List<LineSegment> _previousLaneSegments = new List<LineSegment>();

		// Token: 0x040025B4 RID: 9652
		private float _distanceToGoal;

		// Token: 0x040025B5 RID: 9653
		public int groupIndex = int.MinValue;

		// Token: 0x040025B6 RID: 9654
		private bool _isInTunnel;

		// Token: 0x040025B7 RID: 9655
		private int _onMotorwayId = -1;

		// Token: 0x040025B8 RID: 9656
		private Vehicle _audioVehicle;

		// Token: 0x040025B9 RID: 9657
		private Vector2 _pan;

		// Token: 0x040025BA RID: 9658
		private float _attenuation;

		// Token: 0x040025BB RID: 9659
		public Material vehicleMaterial;

		// Token: 0x040025BC RID: 9660
		[HideInInspector]
		[SerializeField]
		private Renderer _combinedMeshVehicleRenderer;

		// Token: 0x040025BD RID: 9661
		[SerializeField]
		private Renderer _vehicleFrontLightLeft;

		// Token: 0x040025BE RID: 9662
		[SerializeField]
		private Renderer _vehicleFrontLightRight;

		// Token: 0x040025BF RID: 9663
		[SerializeField]
		private Renderer _vehicleHeadlightBeams;

		// Token: 0x040025C0 RID: 9664
		[SerializeField]
		private Renderer _vehicleShadowRenderer;

		// Token: 0x040025C1 RID: 9665
		[SerializeField]
		private VehicleTrailRenderer _trail;

		// Token: 0x040025C2 RID: 9666
		private int _boundSortingLayerId = int.MinValue;

		// Token: 0x040025C3 RID: 9667
		private int _boundMotorwayId = -1;

		// Token: 0x040025C4 RID: 9668
		private bool _isPendingDeletion;

		// Token: 0x040025C5 RID: 9669
		public float shadowOffsetScale = 1f;

		// Token: 0x040025C6 RID: 9670
		private const string CarSortingLayerName = "Car";

		// Token: 0x040025C7 RID: 9671
		private static int _carSortingLayerId;

		// Token: 0x040025C8 RID: 9672
		private const string HeadlightSortingLayerName = "Headlight";

		// Token: 0x040025C9 RID: 9673
		private static int _carHeadlightSortingLayerId;

		// Token: 0x040025CA RID: 9674
		private const string MotorwayCarSortingLayerName = "MotorwayCar";

		// Token: 0x040025CB RID: 9675
		private static int _motorwayCarSortingLayerId;

		// Token: 0x040025CC RID: 9676
		private VehicleView.HeadlightState _headlightState;

		// Token: 0x040025CD RID: 9677
		private Bounds _bodyAndHeadlightBounds;

		// Token: 0x040025CE RID: 9678
		private const float InvalidTime = -1f;

		// Token: 0x040025CF RID: 9679
		private float _headlightResponseTime = -1f;

		// Token: 0x040025D0 RID: 9680
		private float _nightModeEnabledChangedTime = -1f;

		// Token: 0x040025D1 RID: 9681
		private bool _previousIsNightModeEnabled;

		// Token: 0x040025D2 RID: 9682
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x02000613 RID: 1555
		private enum MotorwayState
		{
			// Token: 0x040025D5 RID: 9685
			NotOnMotorway,
			// Token: 0x040025D6 RID: 9686
			OnMotorway,
			// Token: 0x040025D7 RID: 9687
			EnteringMotorway,
			// Token: 0x040025D8 RID: 9688
			LeavingMotorway
		}

		// Token: 0x02000614 RID: 1556
		private enum HeadlightState
		{
			// Token: 0x040025DA RID: 9690
			Off,
			// Token: 0x040025DB RID: 9691
			TurningOff,
			// Token: 0x040025DC RID: 9692
			TurningOn,
			// Token: 0x040025DD RID: 9693
			On
		}

		// Token: 0x02000615 RID: 1557
		public class Builder : IViewBuilder
		{
			// Token: 0x06002BAB RID: 11179 RVA: 0x000C1008 File Offset: 0x000BF208
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				VehicleView vehicleView = client.Scope.Get<VehicleView>();
				vehicleView.Initialize(model as VehicleModel);
				client.AddView(vehicleView);
			}
		}
	}
}
