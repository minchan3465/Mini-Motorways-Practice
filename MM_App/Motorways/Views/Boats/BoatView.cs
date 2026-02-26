using System;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Views.Boats
{
	// Token: 0x0200062F RID: 1583
	public class BoatView : MonoBehaviour, IView, BoatModel.IObserver, IReusable
	{
		// Token: 0x17000771 RID: 1905
		// (get) Token: 0x06002C21 RID: 11297 RVA: 0x000C37D8 File Offset: 0x000C19D8
		public BoatModel Model
		{
			get
			{
				return this._model;
			}
		}

		// Token: 0x06002C22 RID: 11298 RVA: 0x000C37E0 File Offset: 0x000C19E0
		private void Initialize(BoatModel boatModel)
		{
			this._model = boatModel;
			this._model.Subscribe(this);
			this._viewIndex.AddBoatView(this);
			this._laneCursor = this._scope.Get<LaneCursor>();
			this.UpdatePositionFromModel(0f);
			this._boatTrail.SetVisualConstantsData(this._visualConstantsData);
			this._leftRippleEmission = this.LeftRipple.emission;
			this._rightRippleEmission = this.RightRipple.emission;
			this._leftRippleMainModule = this.LeftRipple.main;
			this._rightRippleMainModule = this.RightRipple.main;
			Fix64 distanceToTerminal;
			CarparkModel firstTerminal = this._model.CurrentFrame.tile.GetFirstTerminal(boatModel.CurrentFrame.DistanceAlongPathSegment, this._constants.boatCenterToBowDistance, boatModel.CurrentFrame.direction, out distanceToTerminal);
			this.UpdateDockingGeometry(firstTerminal);
			if (this._motorwaysGame.StartReason == GameStartReason.Resumed && distanceToTerminal < BoatView.RESUME_DOCKING_DISTANCE)
			{
				BoatModel.BehaviorState state = this._model.state;
				this._isDocking = (state == BoatModel.BehaviorState.ApproachingTerminal || state == BoatModel.BehaviorState.Undocking || state == BoatModel.BehaviorState.Stopped || state == BoatModel.BehaviorState.Stopping);
				return;
			}
			this._isDocking = false;
		}

		// Token: 0x17000772 RID: 1906
		// (get) Token: 0x06002C23 RID: 11299 RVA: 0x000C390B File Offset: 0x000C1B0B
		public float Attenuation
		{
			get
			{
				return this._gameCamera.GetAttenuationFromWorld(this._boatTransform.position, true, 5f);
			}
		}

		// Token: 0x17000773 RID: 1907
		// (get) Token: 0x06002C24 RID: 11300 RVA: 0x000C3929 File Offset: 0x000C1B29
		public Vector2 Pan
		{
			get
			{
				return this._gameCamera.GetPanFromWorld(this._boatTransform.position);
			}
		}

		// Token: 0x06002C25 RID: 11301 RVA: 0x000C3944 File Offset: 0x000C1B44
		public void UpdatePositionFromModel(float stepAlpha)
		{
			this._laneCursor.MoveToBoat(this, this._viewIndex, stepAlpha);
			float sineFactor = this._visualConstantsData.boatMovementBobbingPeriod * this._bobbingTimer;
			if ((double)sineFactor >= 6.283185307179586)
			{
				this._bobbingTimer = 0f;
			}
			Vector3 currentPosition = this._laneCursor.Position;
			this._laneCursor.Move(this._visualConstantsData.boatMovementLookAheadDistance);
			if (this._model.state != BoatModel.BehaviorState.Stopped && !Mathf.Approximately(0f, (currentPosition - this._laneCursor.Position).sqrMagnitude))
			{
				Vector2 direction = (currentPosition - this._laneCursor.Position).normalized;
				this._currentDirection = direction;
			}
			currentPosition += this._visualConstantsData.boatMovementBobbingAmplitude * (float)Math.Sin((double)sineFactor) * this._currentDirection.RotateCW2D();
			currentPosition += this._visualConstantsData.boatMovementBobbingForwardsAmplitude * (float)Math.Sin((double)sineFactor + 3.141592653589793) * this._currentDirection;
			this._boatTransform.position = currentPosition - (float)this._constants.boatCenterToPivotDistance * this._currentDirection;
			this._boatTransform.rotation = Quaternion.FromToRotation(Vector3.down, this._currentDirection);
			this._lastPosition = currentPosition;
			BoatModel.BehaviorState state = this._model.state;
			if (state == BoatModel.BehaviorState.Stopping || state == BoatModel.BehaviorState.Stopped || state == BoatModel.BehaviorState.Undocking)
			{
				if (this._isDocking)
				{
					float currentSpeed = Mathf.Lerp((float)this._model.CurrentFrame.speed, (float)this._model.NextFrame.speed, stepAlpha);
					float speedThreshold = (float)((this._model.state == BoatModel.BehaviorState.Undocking) ? this._constants.boatUndockingSpeedThreshold : this._constants.boatDockingSpeedThreshold);
					float speedChangeProgress = Mathf.Clamp01((speedThreshold - currentSpeed) / speedThreshold);
					float speedAdjustmentProgress = 0.5f * (1f - Mathf.Cos(3.1415927f * speedChangeProgress));
					this._boatTransform.rotation = Quaternion.Lerp(this._boatTransform.rotation, this._dockingRotation, speedAdjustmentProgress);
					this._boatTransform.position = Vector3.Lerp(this._boatTransform.position, this._dockingPosition, speedAdjustmentProgress);
					if (this._model.state == BoatModel.BehaviorState.Undocking)
					{
						float offsetAmount = Mathf.Sin(3.1415927f * speedAdjustmentProgress);
						this._boatTransform.position += Vector3.Lerp(Vector3.zero, this._constants.boatUndockingMidpointOffset, offsetAmount);
					}
				}
			}
			else if (this._model.state != BoatModel.BehaviorState.ApproachingTerminal)
			{
				this._isDocking = false;
			}
			if (this._model.state == BoatModel.BehaviorState.Stopped && this._model.GetTargetTerminal() == null)
			{
				Vector3 reverseDirection = -this._currentDirection;
				Quaternion currentRotation = Quaternion.FromToRotation(Vector3.down, this._currentDirection);
				Quaternion reverseRotation = Quaternion.FromToRotation(Vector3.down, reverseDirection);
				Vector3 arrivalOffset = this._lastPosition - (float)this._constants.boatCenterToPivotDistance * this._currentDirection;
				Vector3 departureOffset = this._lastPosition - (float)this._constants.boatCenterToPivotDistance * reverseDirection;
				float dockedProgress = (float)((this._constants.boatTerminalWaitTime - this._model.DelayBeforeStarting) / this._constants.boatTerminalWaitTime);
				this._boatTransform.position = Vector3.Lerp(arrivalOffset, departureOffset, dockedProgress);
				this._boatTransform.rotation = Quaternion.Lerp(currentRotation, reverseRotation, dockedProgress);
			}
		}

		// Token: 0x06002C26 RID: 11302 RVA: 0x000C3D18 File Offset: 0x000C1F18
		public void UpdateRippleEmission(float stepAlpha, float timeDelta)
		{
			float rippleRatio = (float)(Fix64.Lerp(this._model.CurrentFrame.speed, this._model.NextFrame.speed, (Fix64)stepAlpha) / this._constants.boatSpeed);
			this._leftRippleEmission.rateOverTime = rippleRatio * this._visualConstantsData.boatMaximumRippleEmission - this._visualConstantsData.boatRippleEmissionStopFactor;
			this._rightRippleEmission.rateOverTime = rippleRatio * this._visualConstantsData.boatMaximumRippleEmission - this._visualConstantsData.boatRippleEmissionStopFactor;
			float rippleSpeed = timeDelta * this._visualConstantsData.boatRippleSpeed;
			this._leftRippleMainModule.simulationSpeed = rippleSpeed;
			this._rightRippleMainModule.simulationSpeed = rippleSpeed;
			if (this._scope.Get<Simulation>().IsPaused)
			{
				this.LeftRipple.Pause();
				this.RightRipple.Pause();
				return;
			}
			this.LeftRipple.Play();
			this.RightRipple.Play();
		}

		// Token: 0x06002C27 RID: 11303 RVA: 0x000C3E20 File Offset: 0x000C2020
		private void UpdateBoatLight(float deltaTime)
		{
			if (this._scope.Get<ActivePlayer>().IsNightModeEnabled)
			{
				if (this._boatLightTimer > this._visualConstantsData.boatLightBlinkTime)
				{
					this._boatLightTimer = 0f;
					this.BoatLight.enabled = !this.BoatLight.enabled;
				}
				this._boatLightTimer += deltaTime;
				return;
			}
			this.BoatLight.enabled = true;
		}

		// Token: 0x06002C28 RID: 11304 RVA: 0x000C3E94 File Offset: 0x000C2094
		public TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			this._bobbingTimer += tickTime.ScaledDelta;
			this.UpdatePositionFromModel(stepAlpha);
			this.UpdateRippleEmission(stepAlpha, tickTime.ScaledDelta);
			float distanceFromLineEnd = ((float)this._model.DistanceToTarget > 0f) ? ((float)this._model.DistanceToTarget) : ((float)this._model.distanceTraveledSinceLastTarget);
			this._boatTrail.UpdateBoatTrail(tickTime.ScaledDelta, distanceFromLineEnd);
			this.UpdateBoatLight(tickTime.ScaledDelta);
			this._shadowTransform.localPosition = Quaternion.Inverse(this._boatTransform.localRotation) * ((Vector3.right + Vector3.down) * this._visualConstantsData.boatShadowPivotDistance);
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002C29 RID: 11305 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002C2A RID: 11306 RVA: 0x000C3F63 File Offset: 0x000C2163
		public void OnTargetTerminalSet(CarparkModel terminal)
		{
			if (terminal == null)
			{
				return;
			}
			this._isDocking = true;
			this.UpdateDockingGeometry(terminal);
		}

		// Token: 0x06002C2B RID: 11307 RVA: 0x000C3F78 File Offset: 0x000C2178
		private void UpdateDockingGeometry(CarparkModel terminal)
		{
			CarparkView targetCarparkView = this._viewClient.GetCarparkViewFromModel(terminal);
			if (targetCarparkView == null)
			{
				return;
			}
			this._dockingPosition = targetCarparkView.BoatDockingTransform.position;
			this._dockingRotation = targetCarparkView.BoatDockingTransform.rotation;
		}

		// Token: 0x06002C2C RID: 11308 RVA: 0x000C3FC0 File Offset: 0x000C21C0
		public void Reset()
		{
			this._model = null;
			this._lastPosition = default(Vector3);
			this._boatTransform.position = Vector3.zero;
			this._boatTransform.localRotation = Quaternion.identity;
			this._currentDirection = default(Vector3);
			this._boatLightTimer = 0f;
			this._leftRippleEmission = default(ParticleSystem.EmissionModule);
			this._rightRippleEmission = default(ParticleSystem.EmissionModule);
			this._leftRippleMainModule = default(ParticleSystem.MainModule);
			this._rightRippleMainModule = default(ParticleSystem.MainModule);
			this._bobbingTimer = 0f;
			this._isDocking = false;
			this._dockingPosition = Vector3.zero;
			this._dockingRotation = Quaternion.identity;
		}

		// Token: 0x04002656 RID: 9814
		private BoatModel _model;

		// Token: 0x04002657 RID: 9815
		private Vector3 _lastPosition;

		// Token: 0x04002658 RID: 9816
		private static Fix64 RESUME_DOCKING_DISTANCE = (Fix64)4L;

		// Token: 0x04002659 RID: 9817
		[SerializeField]
		private Transform _boatTransform;

		// Token: 0x0400265A RID: 9818
		[SerializeField]
		private ParticleSystem LeftRipple;

		// Token: 0x0400265B RID: 9819
		[SerializeField]
		private ParticleSystem RightRipple;

		// Token: 0x0400265C RID: 9820
		[SerializeField]
		public MeshRenderer BoatLight;

		// Token: 0x0400265D RID: 9821
		[SerializeField]
		private Transform _shadowTransform;

		// Token: 0x0400265E RID: 9822
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x0400265F RID: 9823
		[Dependency]
		private MotorwaysGame _motorwaysGame;

		// Token: 0x04002660 RID: 9824
		[Dependency]
		private MotorwaysThemeDatabase _theme;

		// Token: 0x04002661 RID: 9825
		[Dependency]
		private ViewIndex _viewIndex;

		// Token: 0x04002662 RID: 9826
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x04002663 RID: 9827
		[Dependency]
		private IScope _scope;

		// Token: 0x04002664 RID: 9828
		[Dependency]
		private VisualConstantsData _visualConstantsData;

		// Token: 0x04002665 RID: 9829
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04002666 RID: 9830
		[Dependency]
		private ActivePlayer _activePlayer;

		// Token: 0x04002667 RID: 9831
		private LaneCursor _laneCursor;

		// Token: 0x04002668 RID: 9832
		private bool _isDocking;

		// Token: 0x04002669 RID: 9833
		private Quaternion _dockingRotation;

		// Token: 0x0400266A RID: 9834
		private Vector3 _dockingPosition;

		// Token: 0x0400266B RID: 9835
		private Vector3 _currentDirection;

		// Token: 0x0400266C RID: 9836
		private float _bobbingTimer;

		// Token: 0x0400266D RID: 9837
		[SerializeField]
		private BoatTrail _boatTrail;

		// Token: 0x0400266E RID: 9838
		private float _boatLightTimer;

		// Token: 0x0400266F RID: 9839
		private ParticleSystem.EmissionModule _leftRippleEmission;

		// Token: 0x04002670 RID: 9840
		private ParticleSystem.EmissionModule _rightRippleEmission;

		// Token: 0x04002671 RID: 9841
		private ParticleSystem.MainModule _leftRippleMainModule;

		// Token: 0x04002672 RID: 9842
		private ParticleSystem.MainModule _rightRippleMainModule;

		// Token: 0x02000630 RID: 1584
		public class Builder : IViewBuilder
		{
			// Token: 0x06002C2F RID: 11311 RVA: 0x000C4080 File Offset: 0x000C2280
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				BoatView boatView = client.Scope.Get<BoatView>();
				boatView.Initialize(model as BoatModel);
				client.AddView(boatView);
			}
		}
	}
}
