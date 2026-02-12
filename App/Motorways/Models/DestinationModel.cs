using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Motorways.Processes;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004E4 RID: 1252
	public class DestinationModel : Model<DestinationModel.Frame, DestinationModel.IObserver>
	{
		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x060020B7 RID: 8375 RVA: 0x00081B23 File Offset: 0x0007FD23
		public bool IsTrainStation
		{
			get
			{
				return this._destinationType == DestinationModel.DestinationType.TrainStation;
			}
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x060020B8 RID: 8376 RVA: 0x00081B2E File Offset: 0x0007FD2E
		public bool IsBoatTerminal
		{
			get
			{
				return this._destinationType == DestinationModel.DestinationType.BoatTerminal;
			}
		}

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x060020B9 RID: 8377 RVA: 0x00081B39 File Offset: 0x0007FD39
		// (set) Token: 0x060020BA RID: 8378 RVA: 0x00081B44 File Offset: 0x0007FD44
		public int GroupIndex
		{
			get
			{
				return this._groupIndex;
			}
			set
			{
				if (this._groupIndex != value)
				{
					int oldGroupIndex = this._groupIndex;
					this._groupIndex = value;
					for (int demandIndex = 0; demandIndex < this.unassignedDemand.Count; demandIndex++)
					{
						this.unassignedDemand[demandIndex] = this._groupIndex;
					}
					foreach (DestinationModel.IObserver observer in base.Observers)
					{
						observer.OnDestinationChangedGroup(this, oldGroupIndex, this._groupIndex);
					}
					this._demandModel.doesSupplyNeedRecalculation = true;
				}
			}
		}

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x060020BB RID: 8379 RVA: 0x00081BC7 File Offset: 0x0007FDC7
		// (set) Token: 0x060020BC RID: 8380 RVA: 0x00081BCF File Offset: 0x0007FDCF
		public Fix64 ActivationTime { get; private set; }

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x060020BD RID: 8381 RVA: 0x00081BD8 File Offset: 0x0007FDD8
		public bool IsSupplySufficient
		{
			get
			{
				if (this.IsTrainStation)
				{
					return this.contributedSupply > Fix64.Zero;
				}
				return this.contributedSupply * this._demandModel.GetSupplyScale(this._groupIndex) >= this.RequiredSupply;
			}
		}

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x060020BE RID: 8382 RVA: 0x00081C28 File Offset: 0x0007FE28
		public Fix64 RequiredSupply
		{
			get
			{
				Fix64 demand = this._constants.AverageCarsPerDay * this._demandModel.spawnScale * this._behaviour.GetDemandMultiplierForBuilding(this);
				Fix64 extraDemand;
				if (this._demandModel.extraDemand.TryGetValue(this._groupIndex, out extraDemand))
				{
					demand += extraDemand;
				}
				return demand;
			}
		}

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x060020BF RID: 8383 RVA: 0x00081C85 File Offset: 0x0007FE85
		// (set) Token: 0x060020C0 RID: 8384 RVA: 0x00081C8D File Offset: 0x0007FE8D
		[Serialize(true, null)]
		public List<TileModel> TileModels { get; private set; }

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x060020C1 RID: 8385 RVA: 0x00081C96 File Offset: 0x0007FE96
		// (set) Token: 0x060020C2 RID: 8386 RVA: 0x00081C9E File Offset: 0x0007FE9E
		[Serialize(true, null)]
		public CarparkModel Carpark { get; private set; }

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x060020C3 RID: 8387 RVA: 0x00081CA7 File Offset: 0x0007FEA7
		public bool IsUpgraded
		{
			get
			{
				return !(this.demandLevelUpTime < Fix64.Zero) && this._clock.ExpansionTime >= this.demandLevelUpTime;
			}
		}

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x060020C4 RID: 8388 RVA: 0x00081CD3 File Offset: 0x0007FED3
		public bool IsScheduledToBeUpgraded
		{
			get
			{
				return this.demandLevelUpTime > this._clock.ExpansionTime;
			}
		}

		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x060020C5 RID: 8389 RVA: 0x00081CEB File Offset: 0x0007FEEB
		public int TotalDemand
		{
			get
			{
				return this.unassignedDemand.Count + this.waitingDemand.Count;
			}
		}

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x060020C6 RID: 8390 RVA: 0x00081D04 File Offset: 0x0007FF04
		public int MaximumDemandBeforeTimerStarts
		{
			get
			{
				if (this.IsTrainStation)
				{
					return 7;
				}
				if (!this.IsUpgraded)
				{
					return 6;
				}
				return 9;
			}
		}

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x060020C7 RID: 8391 RVA: 0x00081D1C File Offset: 0x0007FF1C
		public bool IsOvercrowding
		{
			get
			{
				return this.TotalDemand > this.MaximumDemandBeforeTimerStarts;
			}
		}

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x060020C8 RID: 8392 RVA: 0x00081D2C File Offset: 0x0007FF2C
		// (set) Token: 0x060020C9 RID: 8393 RVA: 0x00081D34 File Offset: 0x0007FF34
		public TutorialIdentifier TutorialIdentifier { get; private set; }

		// Token: 0x060020CA RID: 8394 RVA: 0x00081D40 File Offset: 0x0007FF40
		public float GetMidStepOvercrowdingTime(float stepAlpha)
		{
			if (base.CurrentFrame.OvercrowdingTime == Fix64.Zero && base.NextFrame.OvercrowdingTime == Fix64.Zero)
			{
				return 0f;
			}
			return Mathf.Lerp((float)base.CurrentFrame.OvercrowdingTime, (float)base.NextFrame.OvercrowdingTime, stepAlpha);
		}

		// Token: 0x060020CB RID: 8395 RVA: 0x00081DA9 File Offset: 0x0007FFA9
		public void SetNextFrameOvercrowdingSpeed(Fix64 speed)
		{
			base.NextFrame.OvercrowdingSpeed = Fix64.Clamp(speed, this._constants.MinimumOvercrowdTimerSpeed, this._constants.MaximumOvercrowdTimerSpeed);
		}

		// Token: 0x060020CC RID: 8396 RVA: 0x00081DD4 File Offset: 0x0007FFD4
		public void AcceptVehicleArrival(VehicleModel vehicle)
		{
			this.waitingDemand.Remove(vehicle.house.GroupIndex);
			this._scoreKeeper.AddScore();
			this._scoreKeeper.AddEfficiencyScoreFromTripLength(vehicle.pathLengthAtStartOfJourney);
			this.demandJustCleared++;
			this.totalServicedPins++;
			if (!Application.isEditor && this._player.IsTelemetryEnabled && FeatureToggle.IsFeatureEnabled(Feature.LargeScoreDiagnosticReport) && this._scoreKeeper.Score == 100000)
			{
				MotorwaysGame motorwaysGame = this._motorwaysGame;
				Diagnostics.Report report = (motorwaysGame != null) ? motorwaysGame.GenerateDiagnosticReport("LargeScore", DiagnosticReportAttachments.SimCommandJournal | DiagnosticReportAttachments.SimArchive | DiagnosticReportAttachments.Screenshot) : null;
				if (report != null)
				{
					report.Upload();
				}
			}
			foreach (DestinationModel.IObserver observer in base.Observers)
			{
				observer.OnDestinationReceivedVehicle(this, vehicle);
			}
		}

		// Token: 0x060020CD RID: 8397 RVA: 0x00081EAC File Offset: 0x000800AC
		public void OnOvercrowded()
		{
			if (this._behaviour.CanGameOver)
			{
				foreach (DestinationModel.IObserver observer in base.Observers)
				{
					observer.OnDestinationOvercrowded(this);
				}
			}
		}

		// Token: 0x060020CE RID: 8398 RVA: 0x00081EEC File Offset: 0x000800EC
		public virtual void Initialize(int groupIndex, Fix64 demandMultiplier, Vector2Int footprint, Vector2Int coordinates, CarparkModel carpark, TutorialIdentifier tutorialIdentifier, DestinationModel.DestinationType destinationType)
		{
			Diagnostics.Log.Info("DestinationModel", "Initialising destination of type {0}", new object[]
			{
				destinationType
			});
			this.GroupIndex = groupIndex;
			this.Carpark = carpark;
			this._destinationType = destinationType;
			this.demandMultiplier = demandMultiplier;
			this.demandTimer = this._constants.DelayBeforeFirstPinOfDestination;
			carpark.AddDestination(this);
			if (!carpark.SupportsTwoDestinations && this._destinationType == DestinationModel.DestinationType.TrainStation)
			{
				Diagnostics.FailAssert("Single destination carparks should not have train stations on them!", Array.Empty<object>());
			}
			this.ActivationTime = this._clock.ExpansionTime;
			this.isActive = true;
			this.demandLevelUpTime = -Fix64.One;
			List<TileModel> tiles = new List<TileModel>();
			for (int x = 0; x < footprint.x; x++)
			{
				for (int y = 0; y < footprint.y; y++)
				{
					TileModel tileModel = this._tilemap.GetOrCreateTileModel(coordinates + new Vector2Int(x, y));
					tileModel.Tile.SetContentType(TileContentType.Destination, this);
					tiles.Add(tileModel);
				}
			}
			this.TileModels = tiles;
			this.TutorialIdentifier = tutorialIdentifier;
			if (carpark.destinations.Count <= 1)
			{
				ModelList<RailTileModel> rails = this._simulation.GetModels<RailTileModel>();
				if (rails.Count > 0)
				{
					RailTileModel closestRailTile = rails[0];
					if (this._destinationType == DestinationModel.DestinationType.TrainStation)
					{
						foreach (RailTileModel rail in rails)
						{
							if ((coordinates - rail.Coordinates).magnitude < (coordinates - closestRailTile.Coordinates).magnitude)
							{
								closestRailTile = rail;
							}
						}
						this._closestRailTile = closestRailTile;
						RailTileModel closestRailTile2 = this._closestRailTile;
						if (closestRailTile2 == null)
						{
							return;
						}
						closestRailTile2.SetTrainStation(this);
					}
				}
			}
		}

		// Token: 0x060020CF RID: 8399 RVA: 0x000820A8 File Offset: 0x000802A8
		public void Remove()
		{
			this.isActive = false;
			RailTileModel closestRailTile = this._closestRailTile;
			if (closestRailTile != null)
			{
				closestRailTile.RemoveTrainStation();
			}
			foreach (DestinationModel.IObserver observer in base.Observers)
			{
				observer.OnDestinationRemoved(this);
			}
			foreach (VehicleModel vehicle in this._simulation.GetModels<VehicleModel>())
			{
				if (vehicle.destination == this)
				{
					vehicle.ResetToHouse();
					vehicle.destination = null;
					vehicle.lastVisitedDestination = null;
				}
				if (vehicle.lastVisitedDestination == this)
				{
					vehicle.lastVisitedDestination = null;
					if (vehicle.behaviorState != VehicleModel.BehaviorState.DrivingHome && vehicle.behaviorState != VehicleModel.BehaviorState.WaitingForDestination && vehicle.behaviorState != VehicleModel.BehaviorState.RealigningDriveway)
					{
						vehicle.ResetToHouse();
					}
				}
				if (this.Carpark.entranceLanes.Contains(vehicle.CurrentFrame.lane))
				{
					vehicle.ResetToHouse();
				}
				foreach (LaneModel entranceLane in this.Carpark.entranceLanes)
				{
					if (entranceLane.OutboundLanes.Contains(vehicle.CurrentFrame.lane))
					{
						vehicle.ResetToHouse();
					}
					else if (entranceLane.InboundLanes.Contains(vehicle.CurrentFrame.lane))
					{
						vehicle.ResetToHouse();
					}
				}
			}
			foreach (TileModel tileModel in this.TileModels)
			{
				tileModel.Tile.SetContentType(TileContentType.None, null);
			}
			this._simulation.RemoveModel(this);
			this._demandModel.doesSupplyNeedRecalculation = true;
		}

		// Token: 0x060020D0 RID: 8400 RVA: 0x00082274 File Offset: 0x00080474
		public override void Reset()
		{
			base.Reset();
			this._groupIndex = 0;
			this._closestRailTile = null;
			this.isActive = false;
			this.ActivationTime = Fix64Consts.Zero;
			this.demandMultiplier = Fix64Consts.Zero;
			this.demandTimer = Fix64Consts.Zero;
			this.demandLevelUpTime = -Fix64.One;
			this.unassignedDemand.Clear();
			this.waitingDemand.Clear();
			this.demandJustCleared = 0;
			this.contributedSupply = Fix64.Zero;
			this.TileModels = null;
			this.Carpark = null;
			this.TutorialIdentifier = TutorialIdentifier.None;
			this.totalServicedPins = 0;
			this._destinationType = DestinationModel.DestinationType.Destination;
		}

		// Token: 0x060020D1 RID: 8401 RVA: 0x00082318 File Offset: 0x00080518
		public void RemoveVehicleAssignment()
		{
			if (Diagnostics.Verify(this.waitingDemand.Count > 0))
			{
				this.waitingDemand.Remove(0);
				this.unassignedDemand.Add(this.GroupIndex);
			}
		}

		// Token: 0x060020D2 RID: 8402 RVA: 0x0008234D File Offset: 0x0008054D
		public DestinationModel() : base(1)
		{
		}

		// Token: 0x04001B29 RID: 6953
		private const int LargeScoreForDiagnosticReport = 100000;

		// Token: 0x04001B2A RID: 6954
		[Dependency]
		private MotorwaysGame _motorwaysGame;

		// Token: 0x04001B2B RID: 6955
		[Dependency]
		private Simulation _simulation;

		// Token: 0x04001B2C RID: 6956
		[Dependency]
		private IScope _scope;

		// Token: 0x04001B2D RID: 6957
		[Dependency]
		private TilemapModel _tilemap;

		// Token: 0x04001B2E RID: 6958
		[Dependency]
		private ScoreModel _scoreKeeper;

		// Token: 0x04001B2F RID: 6959
		[Dependency]
		private ClockModel _clock;

		// Token: 0x04001B30 RID: 6960
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001B31 RID: 6961
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x04001B32 RID: 6962
		[Dependency]
		private DemandModel _demandModel;

		// Token: 0x04001B33 RID: 6963
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04001B34 RID: 6964
		private DestinationModel.DestinationType _destinationType;

		// Token: 0x04001B35 RID: 6965
		private int _groupIndex;

		// Token: 0x04001B36 RID: 6966
		private RailTileModel _closestRailTile;

		// Token: 0x04001B37 RID: 6967
		public bool isActive;

		// Token: 0x04001B39 RID: 6969
		public Fix64 demandMultiplier;

		// Token: 0x04001B3A RID: 6970
		public Fix64 demandTimer;

		// Token: 0x04001B3B RID: 6971
		public Fix64 demandLevelUpTime = -Fix64.One;

		// Token: 0x04001B3C RID: 6972
		public readonly List<int> unassignedDemand = new List<int>();

		// Token: 0x04001B3D RID: 6973
		public readonly List<int> waitingDemand = new List<int>();

		// Token: 0x04001B3E RID: 6974
		[Serialize(false, null)]
		public int demandJustCleared;

		// Token: 0x04001B3F RID: 6975
		public Fix64 contributedSupply;

		// Token: 0x04001B40 RID: 6976
		public int totalServicedPins;

		// Token: 0x020004E5 RID: 1253
		public enum DestinationType
		{
			// Token: 0x04001B45 RID: 6981
			Destination,
			// Token: 0x04001B46 RID: 6982
			TrainStation,
			// Token: 0x04001B47 RID: 6983
			BoatTerminal
		}

		// Token: 0x020004E6 RID: 1254
		public class Frame : IFrame
		{
			// Token: 0x170005D5 RID: 1493
			// (get) Token: 0x060020D3 RID: 8403 RVA: 0x0008237C File Offset: 0x0008057C
			// (set) Token: 0x060020D4 RID: 8404 RVA: 0x00082384 File Offset: 0x00080584
			public Fix64 OvercrowdingSpeed
			{
				get
				{
					return this._overcrowdingSpeed;
				}
				set
				{
					this._overcrowdingSpeed = value;
				}
			}

			// Token: 0x170005D6 RID: 1494
			// (get) Token: 0x060020D5 RID: 8405 RVA: 0x0008238D File Offset: 0x0008058D
			// (set) Token: 0x060020D6 RID: 8406 RVA: 0x00082395 File Offset: 0x00080595
			public Fix64 OvercrowdingTime
			{
				get
				{
					return this._overcrowdingTime;
				}
				set
				{
					this._overcrowdingTime = Fix64.Max(value, Fix64.Zero);
				}
			}

			// Token: 0x060020D7 RID: 8407 RVA: 0x000823A8 File Offset: 0x000805A8
			public bool CloneInto(IFrame cloneFrame, IScope scope)
			{
				DestinationModel.Frame frame = (DestinationModel.Frame)cloneFrame;
				frame._overcrowdingTime = this._overcrowdingTime;
				frame._overcrowdingSpeed = this._overcrowdingSpeed;
				return true;
			}

			// Token: 0x060020D8 RID: 8408 RVA: 0x000823C8 File Offset: 0x000805C8
			public void Reset()
			{
				this.OvercrowdingTime = Fix64.Zero;
				this.OvercrowdingSpeed = Fix64.Zero;
			}

			// Token: 0x04001B48 RID: 6984
			private Fix64 _overcrowdingTime;

			// Token: 0x04001B49 RID: 6985
			private Fix64 _overcrowdingSpeed = Fix64.Zero;
		}

		// Token: 0x020004E7 RID: 1255
		public interface IObserver
		{
			// Token: 0x060020DA RID: 8410
			void OnDestinationReceivedVehicle(DestinationModel destination, VehicleModel vehicle);

			// Token: 0x060020DB RID: 8411
			void OnDestinationOvercrowded(DestinationModel destination);

			// Token: 0x060020DC RID: 8412
			void OnDestinationChangedGroup(DestinationModel destination, int oldGroupIndex, int newGroupIndex);

			// Token: 0x060020DD RID: 8413
			void OnDestinationRemoved(DestinationModel destination);
		}
	}
}
