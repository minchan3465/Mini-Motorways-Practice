using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Server;
using Unity.Profiling;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004EF RID: 1263
	[Factory.Serializable(1)]
	public class LaneModel : Model<EmptyModelFrame, LaneModel.IObserver>, ICreatedInScopeHandler, IDeserializedHandler
	{
		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x06002124 RID: 8484 RVA: 0x00084108 File Offset: 0x00082308
		// (set) Token: 0x06002125 RID: 8485 RVA: 0x00084110 File Offset: 0x00082310
		public int PathfindingStartNodeId { get; private set; } = -1;

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06002126 RID: 8486 RVA: 0x00084119 File Offset: 0x00082319
		// (set) Token: 0x06002127 RID: 8487 RVA: 0x00084121 File Offset: 0x00082321
		public int PathfindingEndNodeId { get; private set; } = -1;

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06002128 RID: 8488 RVA: 0x0008412A File Offset: 0x0008232A
		public Vector2Fixed StartPosition
		{
			get
			{
				return this.lanePoints[0];
			}
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x06002129 RID: 8489 RVA: 0x00084138 File Offset: 0x00082338
		public Vector2Fixed EndPosition
		{
			get
			{
				return this.lanePoints[this.lanePoints.Count - 1];
			}
		}

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x0600212A RID: 8490 RVA: 0x00084154 File Offset: 0x00082354
		public Fix64 Length
		{
			get
			{
				if (this._length < Fix64.Zero)
				{
					this._length = Fix64.Zero;
					for (int i = 0; i < this.lanePoints.Count - 1; i++)
					{
						Vector2Fixed a = this.lanePoints[i];
						Vector2Fixed end = this.lanePoints[i + 1];
						Fix64 segmentDistance = Vector2Fixed.Distance(a, end);
						this._length += segmentDistance;
					}
				}
				return this._length;
			}
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x0600212B RID: 8491 RVA: 0x000841D0 File Offset: 0x000823D0
		public int PathfindingCost
		{
			get
			{
				Fix64 speedToUse = this.SpeedLimit;
				if (this.connection.IsRoundabout)
				{
					speedToUse = speedToUse / this._constants.roundaboutSpeedMultiplier * this._constants.defaultLaneSpeed;
				}
				Fix64 travelTime = this.Length / speedToUse;
				if (this._isCarparkLane)
				{
					travelTime *= Fix64Consts.Two;
				}
				if (this._state != RoadState.Mothballed)
				{
					return Mathf.Max(1, (int)(10f * (float)travelTime));
				}
				return 100000;
			}
		}

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x0600212C RID: 8492 RVA: 0x00084258 File Offset: 0x00082458
		public Fix64 SpeedLimit
		{
			get
			{
				if (this._speedLimit > Fix64.Zero)
				{
					return this._speedLimit;
				}
				this._speedLimit = this._behaviour.GetLaneSpeed(this) * this._speedLimitScale;
				return this._speedLimit;
			}
		}

		// Token: 0x0600212D RID: 8493 RVA: 0x00084296 File Offset: 0x00082496
		public void SetSpeedLimitScale(Fix64 newSpeedScale)
		{
			if (this._speedLimitScale != newSpeedScale)
			{
				this._speedLimitScale = newSpeedScale;
				this.RecalculateSpeedLimit();
			}
		}

		// Token: 0x0600212E RID: 8494 RVA: 0x000842B3 File Offset: 0x000824B3
		public void RecalculateSpeedLimit()
		{
			this._speedLimit = -Fix64Consts.One;
		}

		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x0600212F RID: 8495 RVA: 0x000842C5 File Offset: 0x000824C5
		public List<Vector2Fixed> lanePoints
		{
			get
			{
				return this._lanePoints;
			}
		}

		// Token: 0x06002130 RID: 8496 RVA: 0x000842CD File Offset: 0x000824CD
		public List<LineSegment> GetLineSegments()
		{
			if (this._lineSegments.Count == 0)
			{
				this.BuildLineSegments();
			}
			return this._lineSegments;
		}

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x06002131 RID: 8497 RVA: 0x000842E8 File Offset: 0x000824E8
		// (set) Token: 0x06002132 RID: 8498 RVA: 0x000842F0 File Offset: 0x000824F0
		public RoadState state
		{
			get
			{
				return this._state;
			}
			set
			{
				if (value != this._state)
				{
					this._state = value;
					if (value != RoadState.Active)
					{
						if (value == RoadState.Mothballed && this.PathfindingStartNodeId != -1 && this.PathfindingEndNodeId != -1)
						{
							this.UpdateLaneCost(100000);
							return;
						}
					}
					else if (this.PathfindingStartNodeId != -1 && this.PathfindingEndNodeId != -1)
					{
						this.UpdateLaneCost(this.PathfindingCost);
					}
				}
			}
		}

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x06002133 RID: 8499 RVA: 0x00084352 File Offset: 0x00082552
		public List<LaneModel> OutboundLanes
		{
			get
			{
				return this._outboundLanes;
			}
		}

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x06002134 RID: 8500 RVA: 0x0008435A File Offset: 0x0008255A
		public List<LaneModel> InboundLanes
		{
			get
			{
				return this._inboundLanes;
			}
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06002135 RID: 8501 RVA: 0x00084362 File Offset: 0x00082562
		public List<VehicleModel> Vehicles
		{
			get
			{
				return this._vehicles;
			}
		}

		// Token: 0x06002136 RID: 8502 RVA: 0x0008436A File Offset: 0x0008256A
		public void RemoveVehicle(VehicleModel vehicle)
		{
			this.Vehicles.Remove(vehicle);
		}

		// Token: 0x06002137 RID: 8503 RVA: 0x0008437C File Offset: 0x0008257C
		public void AddVehicle(VehicleModel vehicle)
		{
			if (Diagnostics.Verify(vehicle.NextFrame.lane == this || vehicle.CurrentFrame.lane == this, "The vehicle doesn't think it is entering this lane!"))
			{
				for (int vehicleIndex = 0; vehicleIndex < this.Vehicles.Count; vehicleIndex++)
				{
					if (vehicle.CurrentFrame.distanceAlongLane > this.Vehicles[vehicleIndex].CurrentFrame.distanceAlongLane)
					{
						this.Vehicles.Insert(vehicleIndex, vehicle);
						return;
					}
				}
				this.Vehicles.Add(vehicle);
			}
		}

		// Token: 0x06002138 RID: 8504 RVA: 0x0008440C File Offset: 0x0008260C
		public VehicleModel GetLastVehicle(bool ignoreWaiting = true)
		{
			VehicleModel lastVehicle = null;
			foreach (VehicleModel vehicle in this.Vehicles)
			{
				if ((!ignoreWaiting || !vehicle.IsWaitingAtHouse) && (lastVehicle == null || vehicle.CurrentFrame.distanceAlongLane < lastVehicle.CurrentFrame.distanceAlongLane))
				{
					lastVehicle = vehicle;
				}
			}
			return lastVehicle;
		}

		// Token: 0x06002139 RID: 8505 RVA: 0x00084488 File Offset: 0x00082688
		public VehicleModel FirstVehicleFromHouse(HouseModel house)
		{
			VehicleModel firstVehicle = null;
			foreach (VehicleModel vehicle in this.Vehicles)
			{
				if (vehicle.house == house && (firstVehicle == null || firstVehicle.CurrentFrame.distanceAlongLane < vehicle.CurrentFrame.distanceAlongLane))
				{
					firstVehicle = vehicle;
				}
			}
			return firstVehicle;
		}

		// Token: 0x0600213A RID: 8506 RVA: 0x00084504 File Offset: 0x00082704
		public bool TryGetNextVehicleAfter(VehicleModel current, out VehicleModel next, out Fix64 distance)
		{
			if (this.connection.input.type == RoadType.Driveway)
			{
				next = null;
				distance = Fix64Consts.Zero;
				return true;
			}
			Fix64 currentVehicleDistanceAlongLane = current.CurrentFrame.distanceAlongLane;
			bool foundCheckingVehicle = false;
			distance = this.Length;
			VehicleModel vehicleAhead = null;
			foreach (VehicleModel newVehicle in this.Vehicles)
			{
				if (newVehicle == current)
				{
					foundCheckingVehicle = true;
				}
				else
				{
					Fix64 newDistance = newVehicle.CurrentFrame.distanceAlongLane - currentVehicleDistanceAlongLane;
					if (newDistance > Fix64.Zero && newDistance < distance)
					{
						distance = newDistance;
						vehicleAhead = newVehicle;
					}
				}
			}
			if (!Diagnostics.Verify(foundCheckingVehicle, "Can't check who is ahead of {0}, because it's not in {1}.", current, this))
			{
				next = null;
				distance = Fix64Consts.Zero;
				return false;
			}
			next = vehicleAhead;
			if (next == null)
			{
				distance = Fix64.MaxValue;
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.CheckForVehicleCollisionsWhenMerging))
			{
				Fix64 currentVehicleDistanceLeft = this.Length - currentVehicleDistanceAlongLane;
				foreach (LaneModel otherLane in this.roadChunk.lanes)
				{
					if (otherLane != this && otherLane.connection.output.direction == this.connection.output.direction)
					{
						foreach (VehicleModel mergingVehicle in otherLane._vehicles)
						{
							Fix64 mergingVehicleDistanceLeft = otherLane.Length - mergingVehicle.CurrentFrame.distanceAlongLane;
							if (mergingVehicleDistanceLeft < currentVehicleDistanceLeft)
							{
								Fix64 distanceToMergingVehicle = currentVehicleDistanceLeft - mergingVehicleDistanceLeft;
								if (distanceToMergingVehicle < distance || next == null)
								{
									distance = distanceToMergingVehicle;
									next = mergingVehicle;
								}
							}
						}
					}
				}
			}
			return true;
		}

		// Token: 0x0600213B RID: 8507 RVA: 0x00084724 File Offset: 0x00082924
		public Vector2Fixed PositionAtDistance(Fix64 distance, bool projectIfOver = false)
		{
			if (distance < Fix64.Zero)
			{
				if (projectIfOver)
				{
					return this.StartPosition + (this.StartPosition - this.lanePoints[1]).normalized * Fix64.Abs(distance);
				}
				return this.StartPosition;
			}
			else
			{
				Fix64 distanceRemaining = distance;
				for (int i = 0; i < this.lanePoints.Count - 1; i++)
				{
					Vector2Fixed start = this.lanePoints[i];
					Vector2Fixed end = this.lanePoints[i + 1];
					Fix64 segmentDistance = Vector2Fixed.Distance(start, end);
					if (segmentDistance > distanceRemaining)
					{
						return start + (end - start).normalized * distanceRemaining;
					}
					distanceRemaining -= segmentDistance;
				}
				if (projectIfOver)
				{
					return this.EndPosition + (this.EndPosition - this.lanePoints[this.lanePoints.Count - 2]).normalized * distanceRemaining;
				}
				return this.EndPosition;
			}
		}

		// Token: 0x0600213C RID: 8508 RVA: 0x00084838 File Offset: 0x00082A38
		public void Initialize(RoadChunkModel owningChunk, RoadTileDefinition owningTileDefinition, RoadTileConnection connectionToReflect, Vector2Fixed worldOffset, bool isEndpointLane)
		{
			this.roadChunk = owningChunk;
			this.connection = connectionToReflect;
			RoadTilePath path = owningTileDefinition.GetPath(this.connection);
			this._lanePoints = path.GetLogicalPoints(worldOffset);
			this._length = path.Length;
			this._tileDefinitionIndex = owningTileDefinition.index;
			this._worldOffset = worldOffset;
			this._isEndpointLane = isEndpointLane;
			this._lineSegments.Clear();
		}

		// Token: 0x0600213D RID: 8509 RVA: 0x000848A1 File Offset: 0x00082AA1
		public void Initialize(RoadChunkModel owningChunk, RoadTileConnection connectionToReflect, List<Vector2Fixed> bespokePath, bool isEndpointLane, bool isCarparkLane)
		{
			this.roadChunk = owningChunk;
			this.connection = connectionToReflect;
			this._bespokeLanePoints = bespokePath;
			this._lanePoints = this._bespokeLanePoints;
			this._isEndpointLane = isEndpointLane;
			this._isCarparkLane = isCarparkLane;
			this._lineSegments.Clear();
		}

		// Token: 0x0600213E RID: 8510 RVA: 0x000848DF File Offset: 0x00082ADF
		public void AddInboundLane(LaneModel inboundLane)
		{
			if (!this._inboundLanes.Contains(inboundLane))
			{
				this._inboundLanes.Add(inboundLane);
			}
		}

		// Token: 0x0600213F RID: 8511 RVA: 0x000848FC File Offset: 0x00082AFC
		public void AddOutboundLane(LaneModel outboundLane)
		{
			if (!this._outboundLanes.Contains(outboundLane))
			{
				this.RecalculateSpeedLimit();
				this._outboundLanes.Add(outboundLane);
				this.SetupPathfindingNodesForOutboundLane(outboundLane);
				this._pathfinder.AddEdge(this, this.PathfindingStartNodeId, this.PathfindingEndNodeId, this.PathfindingCost);
			}
		}

		// Token: 0x06002140 RID: 8512 RVA: 0x0008494E File Offset: 0x00082B4E
		public bool RemoveInboundLane(LaneModel lane)
		{
			return this._inboundLanes.Remove(lane);
		}

		// Token: 0x06002141 RID: 8513 RVA: 0x0008495C File Offset: 0x00082B5C
		public bool RemoveOutboundLane(LaneModel lane)
		{
			this.RecalculateSpeedLimit();
			return this._outboundLanes.Remove(lane);
		}

		// Token: 0x06002142 RID: 8514 RVA: 0x00084970 File Offset: 0x00082B70
		public void RemoveInboundAndOutboundLanes()
		{
			foreach (LaneModel inboundLane in this._inboundLanes)
			{
				Diagnostics.Verify(inboundLane.RemoveOutboundLane(this), "Removing non-reciprocal inbound lane {0} from {1}.", inboundLane, this);
			}
			this._inboundLanes.Clear();
			foreach (LaneModel outboundLane in this._outboundLanes)
			{
				Diagnostics.Verify(outboundLane.RemoveInboundLane(this), "Remove non-reciprocal outbound lane {0} from {1}.", outboundLane, this);
			}
			this._outboundLanes.Clear();
			this.RemovePathfinderEdge();
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x06002143 RID: 8515 RVA: 0x00084A3C File Offset: 0x00082C3C
		public bool HasTraversingOrCommittedVehicles
		{
			get
			{
				return this.Vehicles.Count > 0 || this.roadChunk.DoesLaneHaveAnyCommittedVehicles(this);
			}
		}

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x06002144 RID: 8516 RVA: 0x00084A5A File Offset: 0x00082C5A
		public bool HasInboundVehicles
		{
			get
			{
				return this.roadChunk.DoesLaneHaveAnyInboundVehicles(this);
			}
		}

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x06002145 RID: 8517 RVA: 0x00084A68 File Offset: 0x00082C68
		public bool CanRelease
		{
			get
			{
				return this.Vehicles.Count == 0 && !this.roadChunk.DoesLaneHaveAnyInboundVehicles(this);
			}
		}

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x06002146 RID: 8518 RVA: 0x00084A88 File Offset: 0x00082C88
		public bool CanHotswap
		{
			get
			{
				return this.Vehicles.Count == 0 && !this.roadChunk.DoesLaneHaveAnyCommittedVehicles(this);
			}
		}

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x06002147 RID: 8519 RVA: 0x00084AA8 File Offset: 0x00082CA8
		// (set) Token: 0x06002148 RID: 8520 RVA: 0x00084AB0 File Offset: 0x00082CB0
		public bool IsAboutToHotswap { get; set; }

		// Token: 0x06002149 RID: 8521 RVA: 0x00084ABC File Offset: 0x00082CBC
		public override string ToString()
		{
			if (this._lanePoints == null)
			{
				return string.Format("[LaneModel Id={0}, Connection={1}, State={2}, Start Node={3}, End Node={4}]", new object[]
				{
					this._id,
					this.connection,
					this.state,
					this.PathfindingStartNodeId,
					this.PathfindingEndNodeId
				});
			}
			return string.Format("[LaneModel Id={0}, Connection={1}, State={2}, Start=({3:0.##}, {4:0.##}), End=({5:0.##}, {6:0.##}), Start Node={7}, End Node={8}]", new object[]
			{
				this._id,
				this.connection,
				this.state,
				(float)this.StartPosition.x,
				(float)this.StartPosition.y,
				(float)this.EndPosition.x,
				(float)this.EndPosition.y,
				this.PathfindingStartNodeId,
				this.PathfindingEndNodeId
			});
		}

		// Token: 0x0600214A RID: 8522 RVA: 0x00084BE4 File Offset: 0x00082DE4
		public override void Reset()
		{
			base.Reset();
			this.state = RoadState.None;
			this.isTemporary = false;
			this.connection = default(RoadTileConnection);
			this._worldOffset = default(Vector2Fixed);
			this._speedLimit = -Fix64.One;
			this._speedLimitScale = -Fix64.One;
			this._id = -1;
			this._tileDefinitionIndex = -1;
			this._length = -Fix64Consts.One;
			this._lanePoints = null;
			this._bespokeLanePoints = null;
			this.RemovePathfinderEdge();
			this._isEndpointLane = false;
			this._isCarparkLane = false;
			this._inboundLanes.Clear();
			this._outboundLanes.Clear();
			this.roadChunk = null;
			this._vehicles.Clear();
			this._lineSegments.Clear();
			this.IsAboutToHotswap = false;
			this.hasBeenUsed = false;
		}

		// Token: 0x0600214B RID: 8523 RVA: 0x00084CBE File Offset: 0x00082EBE
		public void OnCreatedInScope(IScope scope)
		{
			this._id = LaneModel.NextId;
			LaneModel.NextId++;
		}

		// Token: 0x0600214C RID: 8524 RVA: 0x00084CD8 File Offset: 0x00082ED8
		public override void OnReleasedFromScope(IScope scope)
		{
			this.RemovePathfinderEdge();
			foreach (LaneModel.IObserver observer in base.Observers)
			{
				observer.OnLaneModelReleased(this);
			}
			base.OnReleasedFromScope(scope);
		}

		// Token: 0x0600214D RID: 8525 RVA: 0x00084D18 File Offset: 0x00082F18
		private void RemovePathfinderEdge()
		{
			if (this.PathfindingStartNodeId != -1 && this.PathfindingEndNodeId != -1 && this._pathfinder.IsActive)
			{
				this._pathfinder.RemoveEdge(this.PathfindingStartNodeId, this.PathfindingEndNodeId);
			}
			this.PathfindingStartNodeId = -1;
			this.PathfindingEndNodeId = -1;
		}

		// Token: 0x0600214E RID: 8526 RVA: 0x00084D69 File Offset: 0x00082F69
		public void UpdateLaneCost(int newCost)
		{
			this._pathfinder.ChangeEdgeCost(this.PathfindingStartNodeId, this.PathfindingEndNodeId, newCost);
		}

		// Token: 0x0600214F RID: 8527 RVA: 0x00084D84 File Offset: 0x00082F84
		public void OnDeserialized(IScope context)
		{
			if (this._tileDefinitionIndex >= 0)
			{
				if (this._bespokeLanePoints != null && this._bespokeLanePoints.Count == 0)
				{
					this._bespokeLanePoints = null;
				}
				RoadTileDefinition tileDefinition = context.Get<RoadTileAtlas>().GetDefinitionForIndex(this._tileDefinitionIndex);
				if (Diagnostics.Verify(tileDefinition != null, "Could not find tile definition for deserialised LaneModel."))
				{
					RoadTilePath path = tileDefinition.GetPath(this.connection);
					if (Diagnostics.Verify(path != null, "Could not find path for connection {0} in tile definition {1}.", this.connection, tileDefinition))
					{
						this._lanePoints = path.GetLogicalPoints(this._worldOffset);
						this._length = path.Length;
					}
				}
			}
			else
			{
				this._lanePoints = this._bespokeLanePoints;
			}
			this._lineSegments.Clear();
			foreach (LaneModel outboundLane in this._outboundLanes)
			{
				outboundLane._inboundLanes.Add(this);
				this.SetupPathfindingNodesForOutboundLane(outboundLane);
			}
			if (Diagnostics.Verify(this.PathfindingStartNodeId != -1 && this.PathfindingEndNodeId != -1, "Nodes not set up! There should always be at least one outbound lane, as it would otherwise be a dead end with no u-turn. Since there are no nodes, we're not creating an edge!"))
			{
				this._pathfinder.AddEdge(this, this.PathfindingStartNodeId, this.PathfindingEndNodeId, this.PathfindingCost);
			}
			int vehicleIndex = 0;
			while (vehicleIndex < this.Vehicles.Count)
			{
				VehicleModel vehicle = this.Vehicles[vehicleIndex];
				if (!Diagnostics.Verify(vehicle.CurrentFrame.lane == this, "Vehicle is on lane {0}, but lane {1} wants it. The vehicle will be removed, but other things may break.", vehicle.CurrentFrame.lane, this))
				{
					this.Vehicles.RemoveAt(vehicleIndex);
				}
				else
				{
					vehicleIndex++;
				}
			}
		}

		// Token: 0x06002150 RID: 8528 RVA: 0x00084F2C File Offset: 0x0008312C
		private void SetupPathfindingNodesForOutboundLane(LaneModel outboundLane)
		{
			if (this.PathfindingStartNodeId == -1)
			{
				this.PathfindingStartNodeId = this._pathfinder.AddNode(this._isEndpointLane);
			}
			if (this.PathfindingEndNodeId == -1)
			{
				if (outboundLane.PathfindingStartNodeId == -1)
				{
					outboundLane.PathfindingStartNodeId = this._pathfinder.AddNode(outboundLane._isEndpointLane);
				}
				this.PathfindingEndNodeId = outboundLane.PathfindingStartNodeId;
				return;
			}
			if (outboundLane.PathfindingStartNodeId == -1)
			{
				outboundLane.PathfindingStartNodeId = this.PathfindingEndNodeId;
				return;
			}
			if (this.PathfindingEndNodeId != outboundLane.PathfindingStartNodeId)
			{
				Diagnostics.Verify(this._pathfinder.MergeNodes(this.PathfindingEndNodeId, outboundLane.PathfindingStartNodeId));
				outboundLane.PathfindingStartNodeId = this.PathfindingEndNodeId;
			}
		}

		// Token: 0x06002151 RID: 8529 RVA: 0x00084FDC File Offset: 0x000831DC
		private void BuildLineSegments()
		{
			this._lineSegments.Clear();
			for (int lineSegmentIndex = 0; lineSegmentIndex < this._lanePoints.Count - 1; lineSegmentIndex++)
			{
				this._lineSegments.Add(new LineSegment((Vector2)this._lanePoints[lineSegmentIndex], (Vector2)this._lanePoints[lineSegmentIndex + 1]));
			}
		}

		// Token: 0x06002152 RID: 8530 RVA: 0x00085040 File Offset: 0x00083240
		public LaneModel() : base(1)
		{
		}

		// Token: 0x04001B66 RID: 7014
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("LaneModel");

		// Token: 0x04001B67 RID: 7015
		[Serialize(false, null)]
		public int _id = -1;

		// Token: 0x04001B68 RID: 7016
		private static int NextId = 1;

		// Token: 0x04001B69 RID: 7017
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001B6A RID: 7018
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x04001B6D RID: 7021
		private bool _isEndpointLane;

		// Token: 0x04001B6E RID: 7022
		private bool _isCarparkLane;

		// Token: 0x04001B6F RID: 7023
		[Dependency]
		private Pathfinder _pathfinder;

		// Token: 0x04001B70 RID: 7024
		[Serialize(false, null)]
		private Fix64 _length = -Fix64Consts.One;

		// Token: 0x04001B71 RID: 7025
		[Serialize(false, null)]
		private Fix64 _speedLimitScale = Fix64Consts.One;

		// Token: 0x04001B72 RID: 7026
		[Serialize(false, null)]
		private Fix64 _speedLimit = -Fix64Consts.One;

		// Token: 0x04001B73 RID: 7027
		public bool hasBeenUsed;

		// Token: 0x04001B74 RID: 7028
		[Serialize(false, null)]
		private List<Vector2Fixed> _lanePoints;

		// Token: 0x04001B75 RID: 7029
		private List<Vector2Fixed> _bespokeLanePoints;

		// Token: 0x04001B76 RID: 7030
		[Serialize(false, null)]
		private readonly List<LineSegment> _lineSegments = new List<LineSegment>();

		// Token: 0x04001B77 RID: 7031
		private int _tileDefinitionIndex = -1;

		// Token: 0x04001B78 RID: 7032
		private Vector2Fixed _worldOffset;

		// Token: 0x04001B79 RID: 7033
		public RoadTileConnection connection;

		// Token: 0x04001B7A RID: 7034
		private RoadState _state;

		// Token: 0x04001B7B RID: 7035
		public bool isTemporary;

		// Token: 0x04001B7C RID: 7036
		private List<LaneModel> _outboundLanes = new List<LaneModel>();

		// Token: 0x04001B7D RID: 7037
		[Serialize(false, null)]
		private List<LaneModel> _inboundLanes = new List<LaneModel>();

		// Token: 0x04001B7E RID: 7038
		public RoadChunkModel roadChunk;

		// Token: 0x04001B7F RID: 7039
		private List<VehicleModel> _vehicles = new List<VehicleModel>();

		// Token: 0x04001B81 RID: 7041
		private static readonly ProfilerMarker Profiler_FirstVehicleAheadOf = new ProfilerMarker(ProfilerUtility.CategoryModel, "LaneModel.FirstVehicleAheadOf");

		// Token: 0x020004F0 RID: 1264
		public interface IObserver
		{
			// Token: 0x06002154 RID: 8532
			void OnLaneModelReleased(LaneModel laneModel);
		}
	}
}
