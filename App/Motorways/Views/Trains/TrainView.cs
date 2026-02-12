using System;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Audio;
using Motorways.Constants;
using Motorways.Models;
using Motorways.Themes;
using Server;
using UnityEngine;

namespace Motorways.Views.Trains
{
	// Token: 0x0200061A RID: 1562
	public class TrainView : MonoBehaviour, IView, TrainModel.IObserver, IReusable, ICreatedInScopeHandler, IReleasedFromScopeHandler, IThemeComponent
	{
		// Token: 0x17000764 RID: 1892
		// (get) Token: 0x06002BC6 RID: 11206 RVA: 0x000C15A0 File Offset: 0x000BF7A0
		public TrainModel Model
		{
			get
			{
				return this._model;
			}
		}

		// Token: 0x17000765 RID: 1893
		// (get) Token: 0x06002BC7 RID: 11207 RVA: 0x000C15A8 File Offset: 0x000BF7A8
		public Transform CenterTransform
		{
			get
			{
				return this._firstCarriageTransform;
			}
		}

		// Token: 0x17000766 RID: 1894
		// (get) Token: 0x06002BC8 RID: 11208 RVA: 0x000C15B0 File Offset: 0x000BF7B0
		// (set) Token: 0x06002BC9 RID: 11209 RVA: 0x000C15E8 File Offset: 0x000BF7E8
		public bool IsTrailActive
		{
			get
			{
				bool isActive = false;
				foreach (VehicleTrailRenderer trail in this._trails)
				{
					isActive |= trail.gameObject.activeInHierarchy;
				}
				return isActive;
			}
			set
			{
				VehicleTrailRenderer[] trails = this._trails;
				for (int i = 0; i < trails.Length; i++)
				{
					trails[i].gameObject.SetActive(value);
				}
			}
		}

		// Token: 0x06002BCA RID: 11210 RVA: 0x000C1618 File Offset: 0x000BF818
		public void OnCreatedInScope(IScope scope)
		{
			TrainShadowView[] array = this.trainShadowViews;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnCreatedInScope(this._visualConstantsData);
			}
		}

		// Token: 0x06002BCB RID: 11211 RVA: 0x000C1648 File Offset: 0x000BF848
		private void Initialize(TrainModel trainModel)
		{
			this._model = trainModel;
			this._model.Subscribe(this);
			this._model.Subscribe(this);
			this._laneCursor = this._scope.Get<LaneCursor>();
			this._viewIndex.AddTrainView(this);
			this.UpdatePositionFromModel(0f);
			this._headlightDirectionState = TrainView.HeadlightDirectionState.FrontOn;
			this._frontHeadlightIntensity.Set(1f, 0f);
			this._frontHeadlightBeam.gameObject.SetActive(true);
			this._backHeadlightIntensity.Set(0f, 0f);
			this._backHeadlightBeam.gameObject.SetActive(false);
			this.SetHeadlightPowerState(this._activePlayer.IsNightModeEnabled ? TrainView.HeadlightPowerState.On : TrainView.HeadlightPowerState.Off);
			this.UpdateHeadlights(0f);
			if (FeatureToggle.IsFeatureDisabled(Feature.VehicleTrails))
			{
				this.IsTrailActive = false;
			}
		}

		// Token: 0x06002BCC RID: 11212 RVA: 0x000C1724 File Offset: 0x000BF924
		private void UpdatePositionFromModel(float stepAlpha)
		{
			float centerToWheelDistance = (float)this._constants.trainCenterToWheelDistance;
			float carriageSeparationDistance = (float)this._constants.trainCarriageSeparationDistance;
			float additionalLength = 3f * centerToWheelDistance * 2f + 2f * carriageSeparationDistance;
			this._speed = (float)this._model.CurrentFrame.speed;
			if (this._model.state != this._state)
			{
				TrainModel.BehaviorState state = this._model.state;
				if (state != TrainModel.BehaviorState.Driving)
				{
					if (state == TrainModel.BehaviorState.Stopped)
					{
						this._audioSystem.ScheduleEvent(AudioEvent.CreateTrainEvent(-1.0, AudioEventType.TrainArrives, this));
					}
				}
				else
				{
					this._audioSystem.ScheduleEvent(AudioEvent.CreateTrainEvent(-1.0, AudioEventType.TrainDeparts, this));
				}
				this._state = this._model.state;
			}
			this._laneCursor.MoveToTrain(this, stepAlpha, this._viewIndex, additionalLength);
			Vector3 midpoint = this._laneCursor.Position;
			if (!Mathf.Approximately(0f, (midpoint - this._lastMidpoint).sqrMagnitude))
			{
				this._lastMidpoint = midpoint;
				this._laneCursor.Move(centerToWheelDistance);
				Vector3 frontWheelPosition = this._laneCursor.Position;
				Vector3 backWheelPosition;
				if (!this._laneCursor.MoveAlongRadius(centerToWheelDistance * -2f, out backWheelPosition))
				{
					backWheelPosition = frontWheelPosition + (midpoint - frontWheelPosition).normalized * (centerToWheelDistance * 2f);
				}
				TrainView.PositionTransform(this._trainTransform, frontWheelPosition, backWheelPosition);
				this._laneCursor.Move(-carriageSeparationDistance);
				frontWheelPosition = this._laneCursor.Position;
				if (Diagnostics.Verify(this._laneCursor.MoveAlongRadius(centerToWheelDistance * -2f, out backWheelPosition)))
				{
					TrainView.PositionTransform(this._firstCarriageTransform, frontWheelPosition, backWheelPosition);
				}
				this._laneCursor.Move(-carriageSeparationDistance);
				frontWheelPosition = this._laneCursor.Position;
				if (Diagnostics.Verify(this._laneCursor.MoveAlongRadius(centerToWheelDistance * -2f, out backWheelPosition)))
				{
					TrainView.PositionTransform(this._secondCarriageTransform, frontWheelPosition, backWheelPosition);
				}
			}
		}

		// Token: 0x06002BCD RID: 11213 RVA: 0x000C1940 File Offset: 0x000BFB40
		private static void PositionTransform(Transform body, Vector3 frontWheelPosition, Vector3 backWheelPosition)
		{
			Vector3 bodyPosition = Vector3.Lerp(backWheelPosition, frontWheelPosition, 0.5f);
			body.localPosition = new Vector3(bodyPosition.x, bodyPosition.y, 0f);
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
			Quaternion bodyRotation = new Quaternion(0f, 0f, qz, qw);
			body.localRotation = bodyRotation;
		}

		// Token: 0x06002BCE RID: 11214 RVA: 0x000C1A30 File Offset: 0x000BFC30
		private void UpdateHeadlights(float deltaTime)
		{
			this.UpdateHeadlightPowerState();
			this.UpdateHeadlightDirectionState();
			if (this._globalHeadlightIntensity.IsActive)
			{
				this._globalHeadlightIntensity.Tick(deltaTime);
			}
			if (this._frontHeadlightIntensity.IsActive)
			{
				this._frontHeadlightIntensity.Tick(deltaTime);
			}
			if (this._backHeadlightIntensity.IsActive)
			{
				this._backHeadlightIntensity.Tick(deltaTime);
			}
			MaterialPropertyBlock materialPropertyBlock = this._theme.MaterialPropertyBlock;
			this._frontHeadlightBeam.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat(ShaderConstants.Intensity, this._frontHeadlightIntensity.Value * this._globalHeadlightIntensity.Value);
			this._frontHeadlightBeam.SetPropertyBlock(materialPropertyBlock);
			this._backHeadlightBeam.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat(ShaderConstants.Intensity, this._backHeadlightIntensity.Value * this._globalHeadlightIntensity.Value);
			this._backHeadlightBeam.SetPropertyBlock(materialPropertyBlock);
		}

		// Token: 0x06002BCF RID: 11215 RVA: 0x000C1B17 File Offset: 0x000BFD17
		private void SetHeadlightPowerState(TrainView.HeadlightPowerState newHeadlightPowerState)
		{
			this._headlightPowerState = newHeadlightPowerState;
			this._globalHeadlightIntensity.Set((newHeadlightPowerState == TrainView.HeadlightPowerState.On) ? 1f : 0f, 0f);
		}

		// Token: 0x06002BD0 RID: 11216 RVA: 0x000C1B40 File Offset: 0x000BFD40
		private void UpdateHeadlightPowerState()
		{
			TrainView.HeadlightPowerState newHeadlightPowerState = this._headlightPowerState;
			if (this._activePlayer.IsNightModeEnabled)
			{
				TrainView.HeadlightPowerState headlightPowerState = this._headlightPowerState;
				if (headlightPowerState != TrainView.HeadlightPowerState.TurningOn)
				{
					if (headlightPowerState - TrainView.HeadlightPowerState.Off <= 1)
					{
						newHeadlightPowerState = TrainView.HeadlightPowerState.TurningOn;
					}
				}
				else if (!this._globalHeadlightIntensity.IsActive)
				{
					newHeadlightPowerState = TrainView.HeadlightPowerState.On;
				}
			}
			else
			{
				TrainView.HeadlightPowerState headlightPowerState = this._headlightPowerState;
				if (headlightPowerState > TrainView.HeadlightPowerState.TurningOn)
				{
					if (headlightPowerState == TrainView.HeadlightPowerState.TurningOff)
					{
						if (!this._globalHeadlightIntensity.IsActive)
						{
							newHeadlightPowerState = TrainView.HeadlightPowerState.Off;
						}
					}
				}
				else
				{
					newHeadlightPowerState = TrainView.HeadlightPowerState.TurningOff;
				}
			}
			if (newHeadlightPowerState != this._headlightPowerState)
			{
				if (newHeadlightPowerState == TrainView.HeadlightPowerState.TurningOn)
				{
					this._globalHeadlightIntensity.Start(this._globalHeadlightIntensity.Value, 1f, this._visualConstantsData.TrainHeadlightDayNightTransitionTime, this._visualConstantsData.TrainHeadlightDayNightTransitionEasingFunction, 0f);
				}
				else if (newHeadlightPowerState == TrainView.HeadlightPowerState.TurningOff)
				{
					this._globalHeadlightIntensity.Start(this._globalHeadlightIntensity.Value, 0f, this._visualConstantsData.TrainHeadlightDayNightTransitionTime, this._visualConstantsData.TrainHeadlightDayNightTransitionEasingFunction, 0f);
				}
				this._headlightPowerState = newHeadlightPowerState;
			}
		}

		// Token: 0x06002BD1 RID: 11217 RVA: 0x000C1C34 File Offset: 0x000BFE34
		private void UpdateHeadlightDirectionState()
		{
			TrainView.HeadlightDirectionState newHeadlightDirectionState = this._headlightDirectionState;
			if (this._activePlayer.IsNightModeEnabled)
			{
				RailDirection direction = this._model.CurrentFrame.direction;
				if (direction != RailDirection.Forwards)
				{
					if (direction == RailDirection.Backwards)
					{
						if (this._headlightDirectionState == TrainView.HeadlightDirectionState.FrontOn)
						{
							newHeadlightDirectionState = TrainView.HeadlightDirectionState.FrontToBackTransition;
						}
						if (this._headlightDirectionState == TrainView.HeadlightDirectionState.FrontToBackTransition && !this._backHeadlightIntensity.IsActive)
						{
							newHeadlightDirectionState = TrainView.HeadlightDirectionState.BackOn;
						}
					}
				}
				else
				{
					if (this._headlightDirectionState == TrainView.HeadlightDirectionState.BackOn)
					{
						newHeadlightDirectionState = TrainView.HeadlightDirectionState.BackToFrontTransition;
					}
					if (this._headlightDirectionState == TrainView.HeadlightDirectionState.BackToFrontTransition && !this._frontHeadlightIntensity.IsActive)
					{
						newHeadlightDirectionState = TrainView.HeadlightDirectionState.FrontOn;
					}
				}
			}
			if (newHeadlightDirectionState != this._headlightDirectionState)
			{
				if (newHeadlightDirectionState == TrainView.HeadlightDirectionState.FrontToBackTransition || newHeadlightDirectionState == TrainView.HeadlightDirectionState.BackToFrontTransition)
				{
					float frontStartIntensity = (newHeadlightDirectionState == TrainView.HeadlightDirectionState.FrontToBackTransition) ? 1f : 0f;
					this._frontHeadlightIntensity.Start(frontStartIntensity, 1f - frontStartIntensity, this._visualConstantsData.TrainHeadlightSwitchTransitionTime, this._visualConstantsData.TrainHeadlightSwitchTransitionEasingFunction, 0f);
					this._backHeadlightIntensity.Start(1f - frontStartIntensity, frontStartIntensity, this._visualConstantsData.TrainHeadlightSwitchTransitionTime, this._visualConstantsData.TrainHeadlightSwitchTransitionEasingFunction, 0f);
					this._frontHeadlightBeam.gameObject.SetActive(true);
					this._backHeadlightBeam.gameObject.SetActive(true);
				}
				if (newHeadlightDirectionState == TrainView.HeadlightDirectionState.FrontOn)
				{
					this._frontHeadlightIntensity.Set(1f, 0f);
					this._backHeadlightIntensity.Set(0f, 0f);
					this._frontHeadlightBeam.gameObject.SetActive(true);
					this._backHeadlightBeam.gameObject.SetActive(false);
				}
				else if (newHeadlightDirectionState == TrainView.HeadlightDirectionState.BackOn)
				{
					this._frontHeadlightIntensity.Set(0f, 0f);
					this._backHeadlightIntensity.Set(1f, 0f);
					this._frontHeadlightBeam.gameObject.SetActive(false);
					this._backHeadlightBeam.gameObject.SetActive(true);
				}
				this._headlightDirectionState = newHeadlightDirectionState;
			}
		}

		// Token: 0x06002BD2 RID: 11218 RVA: 0x000C1E04 File Offset: 0x000C0004
		public TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			this.UpdatePositionFromModel(stepAlpha);
			this.UpdateHeadlights(tickTime.ScaledDelta);
			if (this.IsTrailActive)
			{
				VehicleTrailRenderer[] trails = this._trails;
				for (int i = 0; i < trails.Length; i++)
				{
					trails[i].Tick(tickTime.UnpausedScaledDelta);
				}
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002BD3 RID: 11219 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002BD4 RID: 11220 RVA: 0x000C1E50 File Offset: 0x000C0050
		public void Reset()
		{
			this._model = null;
			this._trainTransform.position = Vector3.zero;
			this._trainTransform.localRotation = Quaternion.identity;
			this._firstCarriageTransform.position = Vector3.zero;
			this._firstCarriageTransform.localRotation = Quaternion.identity;
			this._secondCarriageTransform.position = Vector3.zero;
			this._secondCarriageTransform.localRotation = Quaternion.identity;
			this._lastMidpoint = default(Vector3);
			this._state = TrainModel.BehaviorState.Stopped;
			this._speed = 0f;
			this._headlightDirectionState = TrainView.HeadlightDirectionState.FrontOn;
			this._headlightPowerState = TrainView.HeadlightPowerState.Off;
			this._frontHeadlightIntensity.Reset();
			this._backHeadlightIntensity.Reset();
			this._globalHeadlightIntensity.Reset();
		}

		// Token: 0x06002BD5 RID: 11221 RVA: 0x000C1F11 File Offset: 0x000C0111
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._laneCursor != null)
			{
				scope.Release(this._laneCursor);
				this._laneCursor = null;
			}
		}

		// Token: 0x17000767 RID: 1895
		// (get) Token: 0x06002BD6 RID: 11222 RVA: 0x000C1F2F File Offset: 0x000C012F
		public float Attenuation
		{
			get
			{
				return this._gameCamera.GetAttenuationFromWorld(this._trainTransform.position, true, 5f);
			}
		}

		// Token: 0x17000768 RID: 1896
		// (get) Token: 0x06002BD7 RID: 11223 RVA: 0x000C1F4D File Offset: 0x000C014D
		public Vector2 Pan
		{
			get
			{
				return this._gameCamera.GetPanFromWorld(this._trainTransform.position);
			}
		}

		// Token: 0x06002BD8 RID: 11224 RVA: 0x000C1F68 File Offset: 0x000C0168
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
			Theme motorwaysTheme = themeDatabase.GetTheme() as Theme;
			if (motorwaysTheme != null)
			{
				Color color = motorwaysTheme.GetColor(ThemedMaterialType.Train, "_Color");
				VehicleTrailRenderer[] trails = this._trails;
				for (int i = 0; i < trails.Length; i++)
				{
					trails[i].Color = color;
				}
			}
		}

		// Token: 0x06002BD9 RID: 11225 RVA: 0x000C1FB0 File Offset: 0x000C01B0
		public void ApplyTheme(ITheme theme)
		{
			Theme motorwaysTheme = theme as Theme;
			if (motorwaysTheme != null)
			{
				Color color = motorwaysTheme.GetColor(ThemedMaterialType.Train, "_Color");
				VehicleTrailRenderer[] trails = this._trails;
				for (int i = 0; i < trails.Length; i++)
				{
					trails[i].Color = color;
				}
			}
		}

		// Token: 0x06002BDA RID: 11226 RVA: 0x000C1FF4 File Offset: 0x000C01F4
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			Theme oldMotorwaysTheme = oldTheme as Theme;
			if (oldMotorwaysTheme != null)
			{
				Theme newMotorwaysTheme = newTheme as Theme;
				if (newMotorwaysTheme != null)
				{
					Color color2 = oldMotorwaysTheme.GetColor(ThemedMaterialType.Train, "_Color");
					Color newColor = newMotorwaysTheme.GetColor(ThemedMaterialType.Train, "_Color");
					Color color = Color.Lerp(color2, newColor, progress);
					VehicleTrailRenderer[] trails = this._trails;
					for (int i = 0; i < trails.Length; i++)
					{
						trails[i].Color = color;
					}
				}
			}
			return ThemeBlendingResult.ContinueBlending;
		}

		// Token: 0x06002BDB RID: 11227 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x040025E9 RID: 9705
		private TrainModel _model;

		// Token: 0x040025EA RID: 9706
		[SerializeField]
		private Transform _trainTransform;

		// Token: 0x040025EB RID: 9707
		[SerializeField]
		private Transform _firstCarriageTransform;

		// Token: 0x040025EC RID: 9708
		[SerializeField]
		private Transform _secondCarriageTransform;

		// Token: 0x040025ED RID: 9709
		[SerializeField]
		private VehicleTrailRenderer[] _trails;

		// Token: 0x040025EE RID: 9710
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x040025EF RID: 9711
		[Dependency]
		private MotorwaysThemeDatabase _theme;

		// Token: 0x040025F0 RID: 9712
		[Dependency]
		private ViewIndex _viewIndex;

		// Token: 0x040025F1 RID: 9713
		[Dependency]
		private IScope _scope;

		// Token: 0x040025F2 RID: 9714
		[Dependency]
		private VisualConstantsData _visualConstantsData;

		// Token: 0x040025F3 RID: 9715
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x040025F4 RID: 9716
		[Dependency]
		private ActivePlayer _activePlayer;

		// Token: 0x040025F5 RID: 9717
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x040025F6 RID: 9718
		private LaneCursor _laneCursor;

		// Token: 0x040025F7 RID: 9719
		private Vector3 _lastMidpoint;

		// Token: 0x040025F8 RID: 9720
		[SerializeField]
		private Renderer _frontHeadlightBeam;

		// Token: 0x040025F9 RID: 9721
		[SerializeField]
		private Renderer _backHeadlightBeam;

		// Token: 0x040025FA RID: 9722
		public TrainShadowView[] trainShadowViews;

		// Token: 0x040025FB RID: 9723
		private TrainView.HeadlightDirectionState _headlightDirectionState;

		// Token: 0x040025FC RID: 9724
		private TrainView.HeadlightPowerState _headlightPowerState = TrainView.HeadlightPowerState.Off;

		// Token: 0x040025FD RID: 9725
		private readonly TweenFloat _frontHeadlightIntensity = new TweenFloat();

		// Token: 0x040025FE RID: 9726
		private readonly TweenFloat _backHeadlightIntensity = new TweenFloat();

		// Token: 0x040025FF RID: 9727
		private readonly TweenFloat _globalHeadlightIntensity = new TweenFloat();

		// Token: 0x04002600 RID: 9728
		private const float MaxHeadlightIntensity = 1f;

		// Token: 0x04002601 RID: 9729
		private const float MinHeadlightIntensity = 0f;

		// Token: 0x04002602 RID: 9730
		public TrainModel.BehaviorState _state = TrainModel.BehaviorState.Stopped;

		// Token: 0x04002603 RID: 9731
		public float _speed;

		// Token: 0x0200061B RID: 1563
		private enum HeadlightDirectionState
		{
			// Token: 0x04002605 RID: 9733
			FrontOn,
			// Token: 0x04002606 RID: 9734
			BackOn,
			// Token: 0x04002607 RID: 9735
			FrontToBackTransition,
			// Token: 0x04002608 RID: 9736
			BackToFrontTransition
		}

		// Token: 0x0200061C RID: 1564
		private enum HeadlightPowerState
		{
			// Token: 0x0400260A RID: 9738
			On,
			// Token: 0x0400260B RID: 9739
			TurningOn,
			// Token: 0x0400260C RID: 9740
			Off,
			// Token: 0x0400260D RID: 9741
			TurningOff
		}

		// Token: 0x0200061D RID: 1565
		public class Builder : IViewBuilder
		{
			// Token: 0x06002BDD RID: 11229 RVA: 0x000C2098 File Offset: 0x000C0298
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				TrainView trainView = client.Scope.Get<TrainView>();
				trainView.Initialize(model as TrainModel);
				client.AddView(trainView);
			}
		}
	}
}
