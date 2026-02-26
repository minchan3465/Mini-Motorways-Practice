using System;
using System.Collections.Generic;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Audio;
using Motorways.Models;
using Motorways.Themes;
using Motorways.Views.MeshGeneration;
using Server;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000597 RID: 1431
	[SelectionBase]
	public class DestinationView : MonoBehaviour, IView, DestinationModel.IObserver, IAudioView, ICreatedInScopeHandler, IReleasedFromScopeHandler, IReusable, IThemeComponent
	{
		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x060027E7 RID: 10215 RVA: 0x000AA24A File Offset: 0x000A844A
		// (set) Token: 0x060027E8 RID: 10216 RVA: 0x000AA252 File Offset: 0x000A8452
		[Dependency]
		public City City { get; private set; }

		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x060027E9 RID: 10217 RVA: 0x000AA25B File Offset: 0x000A845B
		public bool IsShowingPins
		{
			get
			{
				return this._isShowingPins;
			}
		}

		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x060027EA RID: 10218 RVA: 0x000AA263 File Offset: 0x000A8463
		public int PinCount
		{
			get
			{
				return this._pinAnimatorToUse.VisiblePinCount;
			}
		}

		// Token: 0x060027EB RID: 10219 RVA: 0x000AA270 File Offset: 0x000A8470
		public void Subscribe(DestinationView.IObserver observer)
		{
			this._observers.Subscribe(observer);
		}

		// Token: 0x060027EC RID: 10220 RVA: 0x000AA27E File Offset: 0x000A847E
		public bool Unsubscribe(DestinationView.IObserver observer)
		{
			return this._observers.Unsubscribe(observer);
		}

		// Token: 0x060027ED RID: 10221 RVA: 0x000AA28C File Offset: 0x000A848C
		public void Initialize(DestinationModel model)
		{
			this.SetVisualVariantActive();
			this._visualVariant.StationPinAnimator.gameObject.SetActive(false);
			this._visualVariant.StationPinAnimatorVertical.gameObject.SetActive(false);
			this._visualVariant.PinAnimator.gameObject.SetActive(false);
			bool tweenIn = !this._viewClient.OnFirstFrame;
			this._connectivity = NetworkConnectivity.Disconnected;
			this._lastNotifiedConnectivity = NetworkConnectivity.Disconnected;
			this._destinationModel = model;
			this._destinationModel.Subscribe(this);
			this._destinationModel.Subscribe(this._gameplayEventHandler);
			this._viewIndex.AddDestinationView(this);
			this.UpdateDestinationMeshes();
			if (this._destinationModel.isActive)
			{
				this._visibility = (model.IsUpgraded ? DestinationView.DestinationVisibility.Circle : DestinationView.DestinationVisibility.Square);
				if (this._visibility == DestinationView.DestinationVisibility.Square)
				{
					this._level0.Show(tweenIn ? TransitionStyle.Tween : TransitionStyle.Snap);
					this._visualVariant.Level1.Hide(TransitionStyle.Snap);
				}
				else
				{
					this._level0.Hide(TransitionStyle.Snap);
					this._visualVariant.Level1.Show(tweenIn ? TransitionStyle.Tween : TransitionStyle.Snap);
					this._pinAnimatorToUse.Upgrade();
				}
			}
			else
			{
				this._visibility = DestinationView.DestinationVisibility.NotShown;
				this._level0.Hide(TransitionStyle.Snap);
				this._visualVariant.Level1.Hide(TransitionStyle.Snap);
			}
			this.groupIndex = this._destinationModel.GroupIndex;
			this._theme.RegisterGameObjectToThemeByGroupIndex(this._level0.gameObject, this.groupIndex);
			this._theme.RegisterGameObjectToThemeByGroupIndex(this._visualVariant.Level1.gameObject, this.groupIndex);
			Vector2 middlePosition = new Vector2(0f, 0f);
			foreach (TileModel tileModel in this._destinationModel.TileModels)
			{
				middlePosition += tileModel.Coordinates;
			}
			middlePosition /= (float)this._destinationModel.TileModels.Count;
			Vector3 destinationPosition = new Vector3(middlePosition.x * (float)TilemapModel.TileWidth, middlePosition.y * (float)TilemapModel.TileWidth, 0f);
			if (model.IsTrainStation)
			{
				if (model.Carpark.carparkSide == TileDirection.North)
				{
					destinationPosition.y += DestinationView.StationOffset;
				}
				if (model.Carpark.carparkSide == TileDirection.West)
				{
					destinationPosition.x -= DestinationView.StationOffset;
				}
			}
			base.transform.position = destinationPosition;
			this._pinAnimatorToUse.Initialize(this, this._scope);
			base.GetComponent<CreativeModeEditableDestination>().Initialize(this._scope, this._destinationModel.Carpark.SupportsTwoDestinations);
			AudioEventType aet = this._destinationModel.isActive ? AudioEventType.DestinationActivated : AudioEventType.DestinationSpawned;
			this._audioSystem.ScheduleEvent(AudioEvent.CreateDestinationEvent(aet, this, true));
			this._spawnTime = Time.time;
		}

		// Token: 0x060027EE RID: 10222 RVA: 0x000AA594 File Offset: 0x000A8794
		private void UpdateDestinationMeshes()
		{
			DestinationMesh.Type level0Type = DestinationMesh.Type.Square;
			this._level0 = this._visualVariant.Level0Square;
			this._pinAnimatorToUse = this._visualVariant.PinAnimator;
			if (this._destinationModel.IsTrainStation)
			{
				bool shouldUseVerticalStation = this._destinationModel.Carpark.Alignment == TileAlignment.Vertical;
				level0Type = (shouldUseVerticalStation ? DestinationMesh.Type.StationVertical : DestinationMesh.Type.StationHorizontal);
				this._level0 = (shouldUseVerticalStation ? this._visualVariant.Level0StationVertical : this._visualVariant.Level0StationHorizontal);
				this._pinAnimatorToUse = (shouldUseVerticalStation ? this._visualVariant.StationPinAnimatorVertical : this._visualVariant.StationPinAnimator);
				this._visualVariant.DisconnectedParticles_TrainStation.main.startRotationZ = (shouldUseVerticalStation ? 1.5707964f : 0f);
			}
			TileDirection doorDirection = this._destinationModel.Carpark.carparkSide;
			this._level0.SetDestinationMesh(this._meshCombiner.GetCombinedMesh(level0Type, doorDirection, this._destinationModel.GroupIndex, this.VisualVariantIndex()));
			if (!this._destinationModel.IsTrainStation)
			{
				this._visualVariant.Level1.SetDestinationMesh(this._meshCombiner.GetCombinedMesh(DestinationMesh.Type.Circle, doorDirection, this._destinationModel.GroupIndex, this.VisualVariantIndex()));
			}
			this._pinAnimatorToUse.gameObject.SetActive(true);
			if (this._destinationModel.IsTrainStation)
			{
				Bounds meshBounds = this._level0.GetComponent<Renderer>().bounds;
				Transform transform = this._visualVariant.DisconnectedParticles_TrainStation.transform;
				Vector3 particlesPosition = transform.position;
				particlesPosition = new Vector3(meshBounds.center.x, meshBounds.center.y, particlesPosition.z);
				transform.position = particlesPosition;
			}
		}

		// Token: 0x060027EF RID: 10223 RVA: 0x000AA74C File Offset: 0x000A894C
		public void OnReleasedFromScope(IScope scope)
		{
			this._theme.UnregisterGameObjectFromThemeByGroupIndex(this._level0.gameObject, this.groupIndex);
			this._theme.UnregisterGameObjectFromThemeByGroupIndex(this._visualVariant.Level1.gameObject, this.groupIndex);
			if (this._destinationModel != null)
			{
				this._viewIndex.RemoveDestinationView(this);
				this._destinationModel.Unsubscribe(this);
				this._destinationModel.Unsubscribe(this._gameplayEventHandler);
				this._destinationModel = null;
			}
		}

		// Token: 0x060027F0 RID: 10224 RVA: 0x000AA7D4 File Offset: 0x000A89D4
		public void SetPinViewVisible(bool isVisible)
		{
			Renderer[] componentsInChildren = this._pinAnimatorToUse.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = isVisible;
			}
			this._isShowingPins = isVisible;
			if (isVisible)
			{
				this._pinAnimatorToUse.SetPinColors((this._theme.GetTheme() as Theme).GetBuildingColor(this.groupIndex, ThemeComponentGroupTarget.BuildingTop));
			}
		}

		// Token: 0x060027F1 RID: 10225 RVA: 0x000AA836 File Offset: 0x000A8A36
		public Vector3 GetPositionOfPin(int pinIndex)
		{
			if (Diagnostics.Verify(pinIndex < this._destinationModel.MaximumDemandBeforeTimerStarts, "Can't get a pin greater than the count we have!"))
			{
				return this._pinAnimatorToUse.pins[pinIndex].PinCenterPosition;
			}
			return Vector3.zero;
		}

		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x060027F2 RID: 10226 RVA: 0x000AA86E File Offset: 0x000A8A6E
		public float MaxOvercrowdingTime
		{
			get
			{
				return (float)this._constants.MaxOvercrowdTime;
			}
		}

		// Token: 0x060027F3 RID: 10227 RVA: 0x000AA884 File Offset: 0x000A8A84
		public void Reset()
		{
			this._destinationVisualVariants = base.GetComponentsInChildren<DestinationVisualVariant>(true);
			this.SetPinViewVisible(true);
			this._isPendingDeletion = false;
			this.groupIndex = -1;
			this._visibility = DestinationView.DestinationVisibility.NotShown;
			this._connectivity = NetworkConnectivity.Unknown;
			this._lastNotifiedConnectivity = NetworkConnectivity.Unknown;
			this._spawnTime = 0f;
			this._observers.UnsubscribeAll();
			this._visualVariant.PinAnimator.Reset();
			this._visualVariant.StationPinAnimator.Reset();
			this._visualVariant.StationPinAnimatorVertical.Reset();
			this._visualVariant.PinAnimator.gameObject.SetActive(false);
			this._visualVariant.StationPinAnimator.gameObject.SetActive(false);
			this._visualVariant.StationPinAnimatorVertical.gameObject.SetActive(false);
			this._isShowingPins = true;
			this._vehiclesWaitingForPin.Clear();
			base.transform.localPosition = Vector3.zero;
			this._level0 = null;
			this._visualVariant.Level0Square.Hide(TransitionStyle.Snap);
			this._visualVariant.Level0StationHorizontal.Hide(TransitionStyle.Snap);
			this._visualVariant.Level0StationVertical.Hide(TransitionStyle.Snap);
			this._visualVariant.Level1.Hide(TransitionStyle.Snap);
			this._visualVariant.DisconnectedParticles_TrainStation.main.startRotationZ = 0f;
			this._visualVariant.DisconnectedParticles_TrainStation.transform.localPosition = Vector3.zero;
			this._visualVariant = this._defaultVisualVariant;
		}

		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x060027F4 RID: 10228 RVA: 0x000AAA05 File Offset: 0x000A8C05
		public DestinationModel Model
		{
			get
			{
				return this._destinationModel;
			}
		}

		// Token: 0x060027F5 RID: 10229 RVA: 0x000AAA10 File Offset: 0x000A8C10
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			this._level0.Tick(timeInterval.Delta);
			this._visualVariant.Level1.Tick(timeInterval.Delta);
			this._pinAnimatorToUse.Tick(timeInterval, stepAlpha, (float)this._constants.GracePeriodTime);
			if (this._isPendingDeletion)
			{
				return TickResult.Destroy;
			}
			if (this._destinationModel.isActive)
			{
				DestinationView.DestinationVisibility newVisibility = this._destinationModel.IsUpgraded ? DestinationView.DestinationVisibility.Circle : DestinationView.DestinationVisibility.Square;
				if (newVisibility != this._visibility)
				{
					AudioEventType aet = AudioEventType.None;
					if (this._visibility == DestinationView.DestinationVisibility.NotShown)
					{
						if (newVisibility == DestinationView.DestinationVisibility.Square)
						{
							this._level0.Show(TransitionStyle.Tween);
						}
						else
						{
							this._visualVariant.Level1.Show(TransitionStyle.Tween);
							this._pinAnimatorToUse.Upgrade();
							aet = AudioEventType.DestinationMutated;
						}
					}
					else
					{
						this._level0.Hide(TransitionStyle.Tween);
						this._visualVariant.Level1.Show(TransitionStyle.Tween);
						this._pinAnimatorToUse.Upgrade();
						aet = AudioEventType.DestinationMutated;
					}
					if (aet != AudioEventType.None)
					{
						this._audioSystem.ScheduleEvent(AudioEvent.CreateDestinationEvent(aet, this, true));
					}
					this._visibility = newVisibility;
					foreach (DestinationView.IObserver observer in this._observers)
					{
						observer.OnDemandLevelChanged(this);
					}
				}
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x060027F6 RID: 10230 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x060027F7 RID: 10231 RVA: 0x000AAB54 File Offset: 0x000A8D54
		private int VisualVariantIndex()
		{
			int variantIndex = (int)this.City.Definition.destinationVisualVariantType;
			if (variantIndex < this._destinationVisualVariants.Length)
			{
				return variantIndex;
			}
			Debug.LogWarning("Visual variant type index for City " + this.City.Definition.name + " is out of bounds.Please set the visual variant type index in the CityDefinition to a valid index, or add a new visual variant in the Destination prefab to match your variant type.");
			return 0;
		}

		// Token: 0x060027F8 RID: 10232 RVA: 0x000AABA4 File Offset: 0x000A8DA4
		private void SetVisualVariantActive()
		{
			this._visualVariant = this._destinationVisualVariants[this.VisualVariantIndex()];
			for (int i = 0; i < this._destinationVisualVariants.Length; i++)
			{
				this._destinationVisualVariants[i].gameObject.SetActive(i == this.VisualVariantIndex());
			}
		}

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x060027F9 RID: 10233 RVA: 0x000AABF2 File Offset: 0x000A8DF2
		public Vector2 Pan
		{
			get
			{
				return this._gameCamera.GetPanFromWorld(base.transform.position);
			}
		}

		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x060027FA RID: 10234 RVA: 0x000AAC0A File Offset: 0x000A8E0A
		public float Attenuation
		{
			get
			{
				return this._gameCamera.GetAttenuationFromWorld(base.transform.position, true, 5f);
			}
		}

		// Token: 0x060027FB RID: 10235 RVA: 0x000AAC28 File Offset: 0x000A8E28
		public float GetAttenuation(bool zoom, float falloffFactor = 5f)
		{
			return this._gameCamera.GetAttenuationFromWorld(base.transform.position, zoom, falloffFactor);
		}

		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x060027FC RID: 10236 RVA: 0x000AAC44 File Offset: 0x000A8E44
		public DestinationView NeighboringDestination
		{
			get
			{
				DestinationModel neighboringDestinationModel = this._destinationModel.Carpark.GetNeighboringDestination(this._destinationModel);
				if (neighboringDestinationModel != null)
				{
					return this._viewIndex.GetDestinationView(neighboringDestinationModel);
				}
				return null;
			}
		}

		// Token: 0x060027FD RID: 10237 RVA: 0x000AAC79 File Offset: 0x000A8E79
		public Color GetBuildingColor(ThemeComponentGroupTarget groupTarget)
		{
			return (this._theme.GetTheme() as Theme).GetBuildingColor(this.groupIndex, groupTarget);
		}

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x060027FE RID: 10238 RVA: 0x000AAC97 File Offset: 0x000A8E97
		// (set) Token: 0x060027FF RID: 10239 RVA: 0x000AACA0 File Offset: 0x000A8EA0
		public NetworkConnectivity NetworkConnectivity
		{
			get
			{
				return this._connectivity;
			}
			set
			{
				this._connectivity = value;
				if (this._connectivity != NetworkConnectivity.Unknown && this._lastNotifiedConnectivity != this._connectivity)
				{
					this._lastNotifiedConnectivity = this._connectivity;
					this._audioSystem.ScheduleEvent(AudioEvent.CreateDestinationEvent(AudioEventType.DestinationConnectedToNetwork, this, this._connectivity == NetworkConnectivity.Connected));
				}
			}
		}

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x06002800 RID: 10240 RVA: 0x000AACFD File Offset: 0x000A8EFD
		public float SpawnTime
		{
			get
			{
				return this._spawnTime;
			}
		}

		// Token: 0x06002801 RID: 10241 RVA: 0x000AAD08 File Offset: 0x000A8F08
		public void OnDestinationReceivedVehicle(DestinationModel destination, VehicleModel vehicle)
		{
			bool isPinDisappearingFromDestination = this._pinAnimatorToUse.RemovePinForVehicleArrival();
			VehicleView vehicleView = this._viewIndex.GetVehicleView(vehicle);
			if (vehicleView != null)
			{
				if (isPinDisappearingFromDestination)
				{
					this._vehiclesWaitingForPin.Add(vehicleView);
					return;
				}
				this.ShowPinOnVehicle(vehicleView);
			}
		}

		// Token: 0x06002802 RID: 10242 RVA: 0x000AAD50 File Offset: 0x000A8F50
		private void ShowPinOnVehicle(VehicleView vehicleView)
		{
			if (!this._isShowingPins || vehicleView.IsPendingDeletion)
			{
				return;
			}
			PinView newPinView = this._scope.Get<PinView>();
			this._viewClient.AddView(newPinView);
			newPinView.AppearAtVehicle(vehicleView, this);
		}

		// Token: 0x06002803 RID: 10243 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnDestinationOvercrowded(DestinationModel destination)
		{
		}

		// Token: 0x06002804 RID: 10244 RVA: 0x000AAD90 File Offset: 0x000A8F90
		public void OnPinHidden()
		{
			while (this._vehiclesWaitingForPin.Count > 0)
			{
				VehicleView vehicle = this._vehiclesWaitingForPin[0];
				this._vehiclesWaitingForPin.RemoveAt(0);
				if (vehicle.Model != null)
				{
					RoadType roadTypeVehicleIsDrivingTo = vehicle.Model.CurrentFrame.lane.connection.output.type;
					if (roadTypeVehicleIsDrivingTo == RoadType.Carpark || roadTypeVehicleIsDrivingTo == RoadType.ParkingSpace)
					{
						this.ShowPinOnVehicle(vehicle);
						return;
					}
				}
			}
		}

		// Token: 0x06002805 RID: 10245 RVA: 0x000AAE00 File Offset: 0x000A9000
		public Bounds GetBounds()
		{
			float tileWidth = (float)TilemapModel.TileWidth;
			Vector3 halfTileDimensions = new Vector3(tileWidth, tileWidth, 0f) * 0.5f;
			Vector2Int firstTileCoordinates = this.Model.TileModels[0].Coordinates;
			Vector3 firstPosition = new Vector3((float)firstTileCoordinates.x * tileWidth, (float)firstTileCoordinates.y * tileWidth, base.transform.position.z);
			Bounds result = new Bounds(firstPosition, Vector3.zero);
			foreach (TileModel tileModel in this.Model.TileModels)
			{
				Vector2Int tileCoordinates = tileModel.Coordinates;
				Vector3 a = new Vector3((float)tileCoordinates.x * tileWidth, (float)tileCoordinates.y * tileWidth, firstPosition.z);
				Vector3 tileMinPos = a - halfTileDimensions;
				result.Encapsulate(tileMinPos);
				Vector3 tilMaxPos = a + halfTileDimensions;
				result.Encapsulate(tilMaxPos);
			}
			return result;
		}

		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x06002806 RID: 10246 RVA: 0x000AAF10 File Offset: 0x000A9110
		private bool ShouldShowDemandCounters
		{
			get
			{
				return FeatureToggle.IsFeatureEnabled(Feature.DemandCounters);
			}
		}

		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x06002807 RID: 10247 RVA: 0x0000222C File Offset: 0x0000042C
		private bool ShouldShowHouseRequirements
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002808 RID: 10248 RVA: 0x000AAF20 File Offset: 0x000A9120
		public void DoDisconnectedPulse()
		{
			ParticleSystem disconnectedParticles = this._visualVariant.DisconnectedParticles_Square;
			if (this._destinationModel.IsTrainStation)
			{
				disconnectedParticles = this._visualVariant.DisconnectedParticles_TrainStation;
			}
			else if (this._visibility == DestinationView.DestinationVisibility.Circle)
			{
				disconnectedParticles = this._visualVariant.DisconnectedParticles_Circle;
			}
			disconnectedParticles.Play(false);
		}

		// Token: 0x06002809 RID: 10249 RVA: 0x000AAF70 File Offset: 0x000A9170
		public void CreateImminentFailAlert(bool isInitialAlert)
		{
			if (this._isShowingPins)
			{
				foreach (DestinationView.IObserver observer in this._observers)
				{
					observer.OnImminentFailAlert(this, isInitialAlert);
				}
			}
		}

		// Token: 0x0600280A RID: 10250 RVA: 0x000AAFAC File Offset: 0x000A91AC
		public void CreateBigPinAlert()
		{
			if (this._isShowingPins)
			{
				foreach (DestinationView.IObserver observer in this._observers)
				{
					observer.OnBigPinAppeared(this);
				}
			}
		}

		// Token: 0x0600280B RID: 10251 RVA: 0x000AAFE8 File Offset: 0x000A91E8
		public void OnDestinationChangedGroup(DestinationModel destinationModel, int oldGroupIndex, int newGroupIndex)
		{
			this._theme.UnregisterGameObjectFromThemeByGroupIndex(this._level0.gameObject, this.groupIndex);
			this._theme.UnregisterGameObjectFromThemeByGroupIndex(this._visualVariant.Level1.gameObject, this.groupIndex);
			this.groupIndex = newGroupIndex;
			this._theme.RegisterGameObjectToThemeByGroupIndex(this._level0.gameObject, this.groupIndex);
			this._theme.RegisterGameObjectToThemeByGroupIndex(this._visualVariant.Level1.gameObject, this.groupIndex);
			Color color = (this._theme.GetTheme() as Theme).GetBuildingColor(newGroupIndex, ThemeComponentGroupTarget.BuildingTop);
			this._pinAnimatorToUse.SetPinColors(color);
			this.SetDisconnectedParticleColour(color);
			this.UpdateDestinationMeshes();
		}

		// Token: 0x0600280C RID: 10252 RVA: 0x000AB0AC File Offset: 0x000A92AC
		private void SetDisconnectedParticleColour(Color color)
		{
			this._visualVariant.DisconnectedParticles_Circle.main.startColor = color;
			this._visualVariant.DisconnectedParticles_Square.main.startColor = color;
			this._visualVariant.DisconnectedParticles_TrainStation.main.startColor = color;
		}

		// Token: 0x0600280D RID: 10253 RVA: 0x000AB113 File Offset: 0x000A9313
		public void OnDestinationRemoved(DestinationModel destinationModel)
		{
			this._isPendingDeletion = true;
		}

		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x0600280E RID: 10254 RVA: 0x000AB11C File Offset: 0x000A931C
		public Vector2 BigPinAlertPosition
		{
			get
			{
				return this._pinAnimatorToUse.BigPinAlertPosition;
			}
		}

		// Token: 0x0600280F RID: 10255 RVA: 0x000022F5 File Offset: 0x000004F5
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x06002810 RID: 10256 RVA: 0x000AB12C File Offset: 0x000A932C
		public void ApplyTheme(ITheme theme)
		{
			Color color = (theme as Theme).GetBuildingColor(this.groupIndex, ThemeComponentGroupTarget.BuildingTop);
			this._pinAnimatorToUse.SetPinColors(color);
			this.SetDisconnectedParticleColour(color);
		}

		// Token: 0x06002811 RID: 10257 RVA: 0x000AB160 File Offset: 0x000A9360
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			Color buildingColor = (oldTheme as Theme).GetBuildingColor(this.groupIndex, ThemeComponentGroupTarget.BuildingTop);
			Color newColor = (newTheme as Theme).GetBuildingColor(this.groupIndex, ThemeComponentGroupTarget.BuildingTop);
			Color color = Color.Lerp(buildingColor, newColor, progress);
			this._pinAnimatorToUse.SetPinColors(color);
			this.SetDisconnectedParticleColour(color);
			if (!(buildingColor == newColor))
			{
				return ThemeBlendingResult.ContinueBlending;
			}
			return ThemeBlendingResult.StopBlending;
		}

		// Token: 0x06002812 RID: 10258 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x06002813 RID: 10259 RVA: 0x000AB1B9 File Offset: 0x000A93B9
		public void OnCreatedInScope(IScope scope)
		{
			this._visualVariant = this._defaultVisualVariant;
			this._pinAnimatorToUse = this._visualVariant.PinAnimator;
			this._destinationVisualVariants = base.GetComponentsInChildren<DestinationVisualVariant>(true);
		}

		// Token: 0x06002816 RID: 10262 RVA: 0x000AB22A File Offset: 0x000A942A
		Transform IAudioView.get_transform()
		{
			return base.transform;
		}

		// Token: 0x040021BA RID: 8634
		[Dependency]
		private IScope _gameScope;

		// Token: 0x040021BC RID: 8636
		[Dependency]
		private IScope _scope;

		// Token: 0x040021BD RID: 8637
		[Dependency]
		private ViewIndex _viewIndex;

		// Token: 0x040021BE RID: 8638
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x040021BF RID: 8639
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x040021C0 RID: 8640
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x040021C1 RID: 8641
		[Dependency]
		private MotorwaysThemeDatabase _theme;

		// Token: 0x040021C2 RID: 8642
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x040021C3 RID: 8643
		[Dependency]
		private DestinationMeshCombiner _meshCombiner;

		// Token: 0x040021C4 RID: 8644
		[Dependency]
		private GameplayEventHandler _gameplayEventHandler;

		// Token: 0x040021C5 RID: 8645
		private DestinationModel _destinationModel;

		// Token: 0x040021C6 RID: 8646
		public int groupIndex = -1;

		// Token: 0x040021C7 RID: 8647
		[SerializeField]
		private DestinationVisualVariant _defaultVisualVariant;

		// Token: 0x040021C8 RID: 8648
		private DestinationVisualVariant _visualVariant;

		// Token: 0x040021C9 RID: 8649
		private DestinationVisualVariant[] _destinationVisualVariants;

		// Token: 0x040021CA RID: 8650
		private bool _isPendingDeletion;

		// Token: 0x040021CB RID: 8651
		private DestinationLevel _level0;

		// Token: 0x040021CC RID: 8652
		private DestinationView.DestinationVisibility _visibility;

		// Token: 0x040021CD RID: 8653
		private DestinationPinAnimatorView _pinAnimatorToUse;

		// Token: 0x040021CE RID: 8654
		private NetworkConnectivity _connectivity;

		// Token: 0x040021CF RID: 8655
		private NetworkConnectivity _lastNotifiedConnectivity;

		// Token: 0x040021D0 RID: 8656
		private float _spawnTime;

		// Token: 0x040021D1 RID: 8657
		private bool _isShowingPins = true;

		// Token: 0x040021D2 RID: 8658
		private readonly List<VehicleView> _vehiclesWaitingForPin = new List<VehicleView>();

		// Token: 0x040021D3 RID: 8659
		private readonly ObserverList<DestinationView.IObserver> _observers = new ObserverList<DestinationView.IObserver>(1);

		// Token: 0x040021D4 RID: 8660
		private static readonly float StationOffset = 0.75f * (float)TilemapModel.TileWidth;

		// Token: 0x02000598 RID: 1432
		public enum DestinationVisibility
		{
			// Token: 0x040021D6 RID: 8662
			NotShown,
			// Token: 0x040021D7 RID: 8663
			Square,
			// Token: 0x040021D8 RID: 8664
			Circle
		}

		// Token: 0x02000599 RID: 1433
		public interface IObserver
		{
			// Token: 0x06002817 RID: 10263
			void OnDemandLevelChanged(DestinationView owner);

			// Token: 0x06002818 RID: 10264
			void OnImminentFailAlert(DestinationView owner, bool isInitialAlert);

			// Token: 0x06002819 RID: 10265
			void OnBigPinAppeared(DestinationView owner);
		}

		// Token: 0x0200059A RID: 1434
		public class Builder : IViewBuilder
		{
			// Token: 0x0600281A RID: 10266 RVA: 0x000AB234 File Offset: 0x000A9434
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				DestinationView buildingView = client.Scope.Get<DestinationView>();
				buildingView.Initialize(model as DestinationModel);
				client.AddView(buildingView);
			}
		}
	}
}
