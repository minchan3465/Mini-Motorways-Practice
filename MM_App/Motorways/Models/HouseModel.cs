using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Motorways.Processes;
using Server;

namespace Motorways.Models
{
	// Token: 0x020004EC RID: 1260
	public class HouseModel : Model<EmptyModelFrame, HouseModel.IObserver>
	{
		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x06002109 RID: 8457 RVA: 0x000837E0 File Offset: 0x000819E0
		// (set) Token: 0x0600210A RID: 8458 RVA: 0x000837E8 File Offset: 0x000819E8
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
					foreach (HouseModel.IObserver observer in base.Observers)
					{
						observer.OnHouseChangedGroup(this, oldGroupIndex, this._groupIndex);
					}
					this._cityPlanModel.groupHouseCounts[oldGroupIndex]--;
					this._cityPlanModel.groupHouseCounts[this._groupIndex]++;
					this._simulation.GetModel<DemandModel>().doesSupplyNeedRecalculation = true;
				}
			}
		}

		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x0600210B RID: 8459 RVA: 0x00083879 File Offset: 0x00081A79
		// (set) Token: 0x0600210C RID: 8460 RVA: 0x00083881 File Offset: 0x00081A81
		[Serialize(true, null)]
		public TileModel tileModel { get; private set; }

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x0600210D RID: 8461 RVA: 0x0008388C File Offset: 0x00081A8C
		public TileDirection DrivewayDirection
		{
			get
			{
				LaneModel drivewayLane = this.DrivewayLane;
				foreach (object obj in Enum.GetValues(typeof(TileDirection)))
				{
					TileDirection potentialDrivewayDirection = (TileDirection)obj;
					if (drivewayLane.roadChunk.GetLanesConnectedToDirection(RoadState.Active, potentialDrivewayDirection).Count > 0)
					{
						return potentialDrivewayDirection;
					}
				}
				return TileDirection.None;
			}
		}

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x0600210E RID: 8462 RVA: 0x00083910 File Offset: 0x00081B10
		public LaneModel DrivewayLane
		{
			get
			{
				if (this.tileModel != null && this.tileModel.roadChunk != null)
				{
					for (int laneIndex = 0; laneIndex < this.tileModel.roadChunk.lanes.Count; laneIndex++)
					{
						if (this.tileModel.roadChunk.lanes[laneIndex].state == RoadState.Active)
						{
							return this.tileModel.roadChunk.lanes[laneIndex];
						}
					}
					Diagnostics.FailAssert("House at {0} has {1} lanes but no active lane.", new object[]
					{
						this.tileModel.Coordinates,
						this.tileModel.roadChunk.lanes.Count
					});
				}
				return null;
			}
		}

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x0600210F RID: 8463 RVA: 0x000839CE File Offset: 0x00081BCE
		public bool HasWaitingVehicle
		{
			get
			{
				return this.waitingVehicles.Count > 0;
			}
		}

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x06002110 RID: 8464 RVA: 0x000839E0 File Offset: 0x00081BE0
		public VehicleModel FirstWaitingVehicle
		{
			get
			{
				if (this.waitingVehicles.Count == 0)
				{
					return null;
				}
				VehicleModel firstWaitingVehicle = this.waitingVehicles[0];
				for (int waitingVehicleIndex = 1; waitingVehicleIndex < this.waitingVehicles.Count; waitingVehicleIndex++)
				{
					if (this.waitingVehicles[waitingVehicleIndex].CurrentFrame.distanceAlongLane > firstWaitingVehicle.CurrentFrame.distanceAlongLane)
					{
						firstWaitingVehicle = this.waitingVehicles[waitingVehicleIndex];
					}
				}
				return firstWaitingVehicle;
			}
		}

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x06002111 RID: 8465 RVA: 0x00083A55 File Offset: 0x00081C55
		// (set) Token: 0x06002112 RID: 8466 RVA: 0x00083A5D File Offset: 0x00081C5D
		public TutorialIdentifier TutorialIdentifier { get; private set; }

		// Token: 0x06002113 RID: 8467 RVA: 0x00083A68 File Offset: 0x00081C68
		public void Initialize(int buildingGroupIndex, TileModel hostTile, TutorialIdentifier tutorialIdentifier)
		{
			this._groupIndex = buildingGroupIndex;
			this.tileModel = hostTile;
			this.TutorialIdentifier = tutorialIdentifier;
			if (Diagnostics.Verify(this.tileModel.Tile.CanSetContentType(TileContentType.House), "Unable to build a house on {0}.", this.tileModel.Tile))
			{
				this.tileModel.Tile.SetContentType(TileContentType.House, this);
			}
		}

		// Token: 0x06002114 RID: 8468 RVA: 0x00083AC4 File Offset: 0x00081CC4
		public void Remove()
		{
			this._simulation.GetModel<DemandModel>().doesSupplyNeedRecalculation = true;
			foreach (VehicleModel vehicleModel in this.ownedVehicles)
			{
				vehicleModel.Remove();
			}
			foreach (TileDirection drivewayDirection in this.tileModel.Tile.GetTwoLaneRoads(RoadState.Live, Tile.MotorwayInclusion.Ignore))
			{
				TileModel drivewayTile = this._tilemap.GetTileModel(TileUtilities.GetAdjacentCoordinates(this.tileModel.Coordinates, drivewayDirection));
				this.tileModel.Tile.SetNodeState(new RoadTileNode(drivewayDirection, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
				drivewayTile.Tile.SetNodeState(new RoadTileNode(TileUtilities.GetOppositeDirection(drivewayDirection), RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
				this.tileModel.Tile.SetNodeState(new RoadTileNode(drivewayDirection, RoadType.TwoLane, -1), RoadState.None, Tile.TileChangePermissions.Full);
				drivewayTile.Tile.SetNodeState(new RoadTileNode(TileUtilities.GetOppositeDirection(drivewayDirection), RoadType.TwoLane, -1), RoadState.None, Tile.TileChangePermissions.Full);
				List<LaneModel> drivewayLanes = drivewayTile.roadChunk.GetLanesConnectedToDirection(RoadState.Pending | RoadState.Active | RoadState.Mothballed, TileUtilities.GetOppositeDirection(drivewayDirection));
				List<RoadChunkModel.InboundVehicle> drivewayInboundVehicles = drivewayTile.roadChunk.inboundVehicles;
				for (int drivewayLaneIndex = drivewayLanes.Count - 1; drivewayLaneIndex >= 0; drivewayLaneIndex--)
				{
					LaneModel drivewayLane = drivewayLanes[drivewayLaneIndex];
					for (int vehicleIndex = drivewayLane.Vehicles.Count - 1; vehicleIndex >= 0; vehicleIndex--)
					{
						drivewayLane.Vehicles[vehicleIndex].ResetToHouse();
					}
					int inboundVehicleIndex = 0;
					while (inboundVehicleIndex < drivewayInboundVehicles.Count)
					{
						RoadChunkModel.InboundVehicle drivewayInboundVehicle = drivewayInboundVehicles[inboundVehicleIndex];
						if (drivewayInboundVehicle.vehicle.path.Contains(drivewayLane))
						{
							HouseModel.Log.Info("Resetting car {0} to it's home as it's inbound on deleted house lane.", new object[]
							{
								drivewayInboundVehicle
							});
							drivewayInboundVehicle.vehicle.ResetToHouse();
						}
						else
						{
							inboundVehicleIndex++;
						}
					}
					drivewayTile.roadChunk.RemoveLane(drivewayLane);
				}
			}
			foreach (LaneModel houseTileLane in this.tileModel.roadChunk.lanes)
			{
				if (houseTileLane.Vehicles.Count > 0)
				{
					for (int vehicleIndex2 = houseTileLane.Vehicles.Count - 1; vehicleIndex2 >= 0; vehicleIndex2--)
					{
						VehicleModel houseLaneVehicle = houseTileLane.Vehicles[vehicleIndex2];
						HouseModel.Log.Info("Resetting car {0} to it's home as it's currently on a deleted house lane {1}.", new object[]
						{
							houseLaneVehicle,
							houseTileLane
						});
						houseLaneVehicle.ResetToHouse();
					}
				}
			}
			for (int inboundVehicleIndex2 = this.tileModel.roadChunk.inboundVehicles.Count - 1; inboundVehicleIndex2 >= 0; inboundVehicleIndex2--)
			{
				RoadChunkModel.InboundVehicle houseInboundVehicle = this.tileModel.roadChunk.inboundVehicles[inboundVehicleIndex2];
				HouseModel.Log.Info("Resetting car {0} to it's home as it's inbound on deleted house lane.", new object[]
				{
					houseInboundVehicle
				});
				houseInboundVehicle.vehicle.ResetToHouse();
			}
			this.tileModel.roadChunk.RemoveAllLanes();
			foreach (HouseModel.IObserver observer in base.Observers)
			{
				observer.OnHouseRemoved(this);
			}
			this.tileModel.Tile.SetContentType(TileContentType.None, null);
			this._simulation.RemoveModel(this);
		}

		// Token: 0x06002115 RID: 8469 RVA: 0x00083E34 File Offset: 0x00082034
		public Fix64 GetLaneDistanceAtFrontOfDriveway(LaneModel drivewayLane)
		{
			return drivewayLane.Length - HouseModel.LaneDistanceFromDrivewayEnds;
		}

		// Token: 0x06002116 RID: 8470 RVA: 0x00083E46 File Offset: 0x00082046
		public Fix64 GetLaneDistanceAtBackOfDriveway(LaneModel drivewayLane)
		{
			return HouseModel.LaneDistanceFromDrivewayEnds;
		}

		// Token: 0x06002117 RID: 8471 RVA: 0x00083E4D File Offset: 0x0008204D
		public Fix64 GetLaneDistanceAtCenterOfDriveway(LaneModel drivewayLane)
		{
			return drivewayLane.Length * Fix64Consts.OneHalf;
		}

		// Token: 0x06002118 RID: 8472 RVA: 0x00083E5F File Offset: 0x0008205F
		public override void Reset()
		{
			base.Reset();
			this._groupIndex = 0;
			this.tileModel = null;
			this.ownedVehicles.Clear();
			this.waitingVehicles.Clear();
			this.realigningVehicles.Clear();
			this.TutorialIdentifier = TutorialIdentifier.None;
		}

		// Token: 0x06002119 RID: 8473 RVA: 0x00083E9D File Offset: 0x0008209D
		public HouseModel() : base(1)
		{
		}

		// Token: 0x04001B57 RID: 6999
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x04001B58 RID: 7000
		[Dependency]
		private TilemapModel _tilemap;

		// Token: 0x04001B59 RID: 7001
		[Dependency]
		private CityPlanModel _cityPlanModel;

		// Token: 0x04001B5A RID: 7002
		private static readonly Fix64 LaneDistanceFromDrivewayEnds = Fix64.FromRaw(1288490240L);

		// Token: 0x04001B5B RID: 7003
		private int _groupIndex;

		// Token: 0x04001B5C RID: 7004
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("HouseModel");

		// Token: 0x04001B5D RID: 7005
		public List<VehicleModel> waitingVehicles = new List<VehicleModel>();

		// Token: 0x04001B5E RID: 7006
		public List<VehicleModel> realigningVehicles = new List<VehicleModel>();

		// Token: 0x04001B60 RID: 7008
		public List<VehicleModel> ownedVehicles = new List<VehicleModel>();

		// Token: 0x020004ED RID: 1261
		public interface IObserver
		{
			// Token: 0x0600211B RID: 8475
			void OnHouseChangedGroup(HouseModel house, int oldGroupIndex, int newGroupIndex);

			// Token: 0x0600211C RID: 8476
			void OnHouseRemoved(HouseModel house);
		}
	}
}
