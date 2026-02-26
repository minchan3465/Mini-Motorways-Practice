using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Server;
using Unity.Profiling;

namespace Motorways.Models
{
	// Token: 0x02000513 RID: 1299
	public class VehicleModel : Model<VehicleModel.Frame, VehicleModel.IObserver>, ICreatedInScopeHandler
	{
		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06002270 RID: 8816 RVA: 0x0008AD58 File Offset: 0x00088F58
		public bool IsParkedAtDestination
		{
			get
			{
				return this.behaviorState == VehicleModel.BehaviorState.ParkedAtDestination;
			}
		}

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x06002271 RID: 8817 RVA: 0x0008AD63 File Offset: 0x00088F63
		public bool IsWaitingAtHouse
		{
			get
			{
				return this.behaviorState == VehicleModel.BehaviorState.WaitingForDestination;
			}
		}

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x06002272 RID: 8818 RVA: 0x0008AD6E File Offset: 0x00088F6E
		public bool IsRealigningOnDriveway
		{
			get
			{
				return this.behaviorState == VehicleModel.BehaviorState.RealigningDriveway;
			}
		}

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x06002273 RID: 8819 RVA: 0x0008AD79 File Offset: 0x00088F79
		public bool IsAvailableAtHouse
		{
			get
			{
				return this.behaviorState == VehicleModel.BehaviorState.WaitingForDestination && this._clock.Time - this._timeWhenArrivedAtHouse > this._constants.TimeAtHouseBeforeCarIsAvailable;
			}
		}

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x06002274 RID: 8820 RVA: 0x0008ADAB File Offset: 0x00088FAB
		public bool IsDrivingToDestination
		{
			get
			{
				return this.behaviorState == VehicleModel.BehaviorState.DrivingToDestination;
			}
		}

		// Token: 0x06002275 RID: 8821 RVA: 0x0008ADB8 File Offset: 0x00088FB8
		public void Remove()
		{
			foreach (VehicleModel.IObserver observer in base.Observers)
			{
				observer.OnRemoved();
			}
			if (this.destination != null)
			{
				this.destination.unassignedDemand.Add(this.destination.waitingDemand[0]);
				this.destination.waitingDemand.RemoveAt(0);
				this.destination.Carpark.vehiclesEntering.Remove(this);
			}
			if (this.lastVisitedDestination != null)
			{
				foreach (CarparkModel.ParkingSpace space in this.lastVisitedDestination.Carpark.spaces)
				{
					if (space.vehicle == this)
					{
						space.vehicle = null;
					}
				}
				this.lastVisitedDestination.Carpark.vehiclesDrivingThrough.Remove(this);
			}
			this.ClearReturnPath();
			this.ClearLeadingVehicles();
			this.ClearForwardPath();
			foreach (LaneModel lane in this.returnPath)
			{
				lane.roadChunk.RemoveInboundVehicle(this, lane, false);
				lane.RemoveVehicle(this);
			}
			this._simulation.RemoveModel(this);
		}

		// Token: 0x06002276 RID: 8822 RVA: 0x0008AF28 File Offset: 0x00089128
		public void MoveToLane(LaneModel newLane, Fix64 distanceAlongNewLane)
		{
			LaneModel lane = base.CurrentFrame.lane;
			if (lane != null)
			{
				lane.RemoveVehicle(this);
			}
			base.CurrentFrame.lane = newLane;
			base.NextFrame.lane = newLane;
			base.CurrentFrame.distanceAlongLane = distanceAlongNewLane;
			base.NextFrame.distanceAlongLane = distanceAlongNewLane;
			this.targetDistanceAlongLastLane = distanceAlongNewLane;
			newLane.AddVehicle(this);
			this.OnMovedToNewLane(newLane, null);
		}

		// Token: 0x06002277 RID: 8823 RVA: 0x0008AF92 File Offset: 0x00089192
		public void OnCreatedInScope(IScope scope)
		{
			this.id = VehicleModel.NextId;
			VehicleModel.NextId++;
		}

		// Token: 0x06002278 RID: 8824 RVA: 0x0008AFAC File Offset: 0x000891AC
		public void OnMovedToNewLane(LaneModel newLane, LaneModel oldLane)
		{
			foreach (VehicleModel.IObserver observer in base.Observers)
			{
				observer.OnVehicleMovedToNewLane(newLane, oldLane);
			}
		}

		// Token: 0x06002279 RID: 8825 RVA: 0x0008AFE0 File Offset: 0x000891E0
		public void ResetToHouse()
		{
			this.ClearReturnPath();
			this.ClearForwardPath();
			this.ClearLeadingVehicles();
			this.repathUrgency = VehicleModel.PathfindUrgency.NotRequired;
			this.returnRepathUrgency = VehicleModel.PathfindUrgency.NotRequired;
			base.NextFrame.lane = this.house.DrivewayLane;
			base.CurrentFrame.lane = this.house.DrivewayLane;
			this.house.DrivewayLane.AddVehicle(this);
			this.house.waitingVehicles.Add(this);
			Fix64 distanceAlongLane = (this.house.waitingVehicles.Count == 0) ? this.house.GetLaneDistanceAtFrontOfDriveway(this.house.DrivewayLane) : this.house.GetLaneDistanceAtBackOfDriveway(this.house.DrivewayLane);
			base.NextFrame.distanceAlongLane = distanceAlongLane;
			base.CurrentFrame.distanceAlongLane = distanceAlongLane;
			if (this.destination != null && (this.behaviorState == VehicleModel.BehaviorState.DrivingToDestination || this.behaviorState == VehicleModel.BehaviorState.ParkingAtDestination))
			{
				this.destination.RemoveVehicleAssignment();
			}
			if (this.destination != null)
			{
				foreach (CarparkModel.ParkingSpace space in this.destination.Carpark.spaces)
				{
					if (space.vehicle == this)
					{
						space.vehicle = null;
					}
				}
				this.destination.Carpark.vehiclesDrivingThrough.Remove(this);
			}
			this.destination = null;
			this.lastVisitedDestination = null;
			this.behaviorState = VehicleModel.BehaviorState.WaitingForDestination;
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x0008B168 File Offset: 0x00089368
		public void AssignPath(IReadOnlyList<LaneModel> newPath, Fix64 newTargetDistanceAlongLastLane)
		{
			VehicleModel.Log.Info("Assigning new path of length {0} to vehicle {1}", new object[]
			{
				newPath.Count,
				this
			});
			int newPathCount = newPath.Count;
			int existingPathCount = this.path.Count;
			int firstNewLaneIndex = 0;
			while (firstNewLaneIndex < newPathCount && firstNewLaneIndex + 2 < existingPathCount && newPath[firstNewLaneIndex] == this.path[firstNewLaneIndex + 2])
			{
				firstNewLaneIndex++;
			}
			if (firstNewLaneIndex >= newPathCount && existingPathCount == newPathCount + 2)
			{
				return;
			}
			int existingLanesToPrune = existingPathCount - (firstNewLaneIndex + 2);
			if (existingLanesToPrune > 0)
			{
				for (int prunedLaneIndex = firstNewLaneIndex + 2; prunedLaneIndex < existingPathCount; prunedLaneIndex++)
				{
					LaneModel prunedLane = this.path[prunedLaneIndex];
					this.pathLength -= prunedLane.Length;
					prunedLane.roadChunk.RemoveInboundVehicle(this, prunedLane, false);
				}
				this.pathLength += this.path[existingPathCount - 1].Length - this.targetDistanceAlongLastLane;
				this.path.RemoveRange(firstNewLaneIndex + 2, existingLanesToPrune);
			}
			else if (existingPathCount > 0)
			{
				this.pathLength += this.path[existingPathCount - 1].Length - this.targetDistanceAlongLastLane;
			}
			else
			{
				this.pathLength = base.CurrentFrame.lane.Length - base.CurrentFrame.distanceAlongLane;
			}
			for (int newLaneIndex = firstNewLaneIndex; newLaneIndex < newPath.Count; newLaneIndex++)
			{
				LaneModel nextLane = newPath[newLaneIndex];
				this.path.Add(nextLane);
				nextLane.roadChunk.AddInboundVehicle(this, nextLane, this.path.Count, false);
				this.pathLength += nextLane.Length;
			}
			if (newTargetDistanceAlongLastLane < Fix64.Zero)
			{
				this.targetDistanceAlongLastLane = ((this.path.Count > 0) ? this.path[this.path.Count - 1].Length : Fix64.Zero);
			}
			else
			{
				this.targetDistanceAlongLastLane = newTargetDistanceAlongLastLane;
				if (this.path.Count > 0)
				{
					this.pathLength -= this.path[this.path.Count - 1].Length - newTargetDistanceAlongLastLane;
				}
			}
			if (this.behaviorState == VehicleModel.BehaviorState.DrivingHome)
			{
				this.ClearReturnPath();
			}
			this.repathUrgency = VehicleModel.PathfindUrgency.NotRequired;
			this.latestAttemptedPathfindFrame = 0;
		}

		// Token: 0x0600227B RID: 8827 RVA: 0x0008B3E4 File Offset: 0x000895E4
		public void AssignReturnPath(IReadOnlyList<LaneModel> newReturnPath)
		{
			int newReturnPathCount = newReturnPath.Count;
			int existingReturnPathCount = this.returnPath.Count;
			int firstNewLaneIndex = 0;
			while (firstNewLaneIndex < newReturnPathCount && firstNewLaneIndex < existingReturnPathCount && newReturnPath[firstNewLaneIndex] == this.returnPath[firstNewLaneIndex])
			{
				firstNewLaneIndex++;
			}
			if (firstNewLaneIndex >= newReturnPathCount && newReturnPathCount == existingReturnPathCount)
			{
				return;
			}
			int existingLanesToPrune = existingReturnPathCount - firstNewLaneIndex;
			if (existingLanesToPrune > 0)
			{
				for (int prunedLaneIndex = firstNewLaneIndex; prunedLaneIndex < existingReturnPathCount; prunedLaneIndex++)
				{
					LaneModel prunedLane = this.returnPath[prunedLaneIndex];
					this.returnPath[prunedLaneIndex].roadChunk.RemoveInboundVehicle(this, prunedLane, true);
				}
				this.returnPath.RemoveRange(firstNewLaneIndex, existingLanesToPrune);
			}
			for (int newLaneIndex = firstNewLaneIndex; newLaneIndex < newReturnPathCount; newLaneIndex++)
			{
				LaneModel returnLane = newReturnPath[newLaneIndex];
				if (Diagnostics.Verify(returnLane.roadChunk != null, "Tried to add a return lane with no road chunk; might be a carpark. ({0})", returnLane))
				{
					returnLane.roadChunk.AddInboundVehicle(this, returnLane, 0, true);
					this.returnPath.Add(returnLane);
				}
			}
			this.returnRepathUrgency = VehicleModel.PathfindUrgency.NotRequired;
		}

		// Token: 0x0600227C RID: 8828 RVA: 0x0008B4DA File Offset: 0x000896DA
		public void RequestPathfind(VehicleModel.PathfindUrgency urgency = VehicleModel.PathfindUrgency.WhenPossible)
		{
			if (urgency > this.repathUrgency)
			{
				this.repathUrgency = urgency;
			}
		}

		// Token: 0x0600227D RID: 8829 RVA: 0x0008B4EC File Offset: 0x000896EC
		public void RequestReturnPathfind(VehicleModel.PathfindUrgency urgency = VehicleModel.PathfindUrgency.WhenPossible)
		{
			if (urgency > this.returnRepathUrgency)
			{
				this.returnRepathUrgency = urgency;
			}
		}

		// Token: 0x0600227E RID: 8830 RVA: 0x0008B500 File Offset: 0x00089700
		public void NotifyBehaviorChange()
		{
			if (this.lastNotifiedBehaviorState != this.behaviorState && this.path.Count > 0)
			{
				this.lastNotifiedBehaviorState = this.behaviorState;
				if (this.behaviorState == VehicleModel.BehaviorState.DrivingToDestination)
				{
					foreach (VehicleModel.IObserver observer in base.Observers)
					{
						observer.OnVehicleDepartedHouse(this, this.destination);
					}
					return;
				}
				if (this.behaviorState == VehicleModel.BehaviorState.DrivingHome)
				{
					foreach (VehicleModel.IObserver observer2 in base.Observers)
					{
						observer2.OnVehicleDepartedDestination(this, this.lastVisitedDestination);
					}
				}
			}
		}

		// Token: 0x0600227F RID: 8831 RVA: 0x0008B5A0 File Offset: 0x000897A0
		public void OnArrivedAtHouse()
		{
			foreach (VehicleModel.IObserver observer in base.Observers)
			{
				observer.OnVehicleArrivedAtHouse(this);
			}
			this._timeWhenArrivedAtHouse = this._clock.Time;
		}

		// Token: 0x06002280 RID: 8832 RVA: 0x0008B5E4 File Offset: 0x000897E4
		public void OnArrivedAtDestination()
		{
			foreach (VehicleModel.IObserver observer in base.Observers)
			{
				observer.OnVehicleArrivedAtDestination(this, this.destination);
			}
			if (Diagnostics.Verify(this.destination != null, "Vehicle arrived at null destination."))
			{
				this.destination.AcceptVehicleArrival(this);
				this.lastVisitedDestination = this.destination;
				this.destination = null;
			}
		}

		// Token: 0x06002281 RID: 8833 RVA: 0x0008B650 File Offset: 0x00089850
		public void OnEnteredCarpark()
		{
			foreach (VehicleModel.IObserver observer in base.Observers)
			{
				observer.OnVehicleEnteredCarpark(this, this.destination);
			}
			this.lastVisitedDestination = this.destination;
		}

		// Token: 0x06002282 RID: 8834 RVA: 0x0008B693 File Offset: 0x00089893
		public void OnDepartedHouse()
		{
			this.lastVisitedDestination = null;
		}

		// Token: 0x06002283 RID: 8835 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnDepartedDestination()
		{
		}

		// Token: 0x06002284 RID: 8836 RVA: 0x0008B69C File Offset: 0x0008989C
		public Fix64 DistanceToLane(LaneModel laneOnPath)
		{
			return this.DistanceToLane(laneOnPath, Fix64.Zero);
		}

		// Token: 0x06002285 RID: 8837 RVA: 0x0008B6AC File Offset: 0x000898AC
		public Fix64 DistanceToLane(LaneModel laneOnPath, Fix64 distanceAlongLane)
		{
			if (base.CurrentFrame.lane == laneOnPath && distanceAlongLane >= base.CurrentFrame.distanceAlongLane)
			{
				return distanceAlongLane - base.CurrentFrame.distanceAlongLane;
			}
			Fix64 distance = base.CurrentFrame.lane.Length - base.CurrentFrame.distanceAlongLane;
			for (int lookaheadIndex = 0; lookaheadIndex < this.path.Count; lookaheadIndex++)
			{
				if (this.path[lookaheadIndex] == laneOnPath)
				{
					return distance + distanceAlongLane;
				}
				distance += this.path[lookaheadIndex].Length;
			}
			return -Fix64.One;
		}

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x06002286 RID: 8838 RVA: 0x0008B75C File Offset: 0x0008995C
		public LaneModel LastCommittedLane
		{
			get
			{
				if (this.path.Count > 0)
				{
					return this.path[Math.Min(1, this.path.Count - 1)];
				}
				return base.CurrentFrame.lane;
			}
		}

		// Token: 0x06002287 RID: 8839 RVA: 0x0008B798 File Offset: 0x00089998
		public bool IsCommittedToLane(LaneModel lane)
		{
			int numLanesToCheck = Math.Min(this.path.Count, 2);
			for (int committedPathIndex = 0; committedPathIndex < numLanesToCheck; committedPathIndex++)
			{
				if (lane == this.path[committedPathIndex])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002288 RID: 8840 RVA: 0x0008B7D8 File Offset: 0x000899D8
		public void ClearNonCommittedLanes()
		{
			if (this.path.Count > 2)
			{
				for (int i = this.path.Count - 1; i >= 2; i--)
				{
					this.path[i].roadChunk.RemoveInboundVehicle(this, this.path[i], false);
					this.path.RemoveAt(i);
				}
			}
		}

		// Token: 0x06002289 RID: 8841 RVA: 0x0008B83C File Offset: 0x00089A3C
		public override void Reset()
		{
			base.Reset();
			this.id = -1;
			this.house = null;
			this.destination = null;
			this.lastVisitedDestination = null;
			this.behaviorState = VehicleModel.BehaviorState.WaitingForDestination;
			this.lastNotifiedBehaviorState = VehicleModel.BehaviorState.WaitingForDestination;
			this.latestAttemptedPathfindFrame = 0;
			this.path.Clear();
			this.repathUrgency = VehicleModel.PathfindUrgency.NotRequired;
			this.returnPath.Clear();
			this.returnRepathUrgency = VehicleModel.PathfindUrgency.NotRequired;
			this.targetDistanceAlongLastLane = Fix64.Zero;
			this.pathLength = Fix64.Zero;
			this.pathLengthAtStartOfJourney = Fix64.Zero;
			this._timeWhenArrivedAtHouse = Fix64.Zero;
			this.isShovingIntoNextIntersection = false;
			this.vehiclePushingInto = null;
			this.blockingVehicle = null;
			this.frameBlockingChainLastChecked = 0;
		}

		// Token: 0x0600228A RID: 8842 RVA: 0x0008B8EC File Offset: 0x00089AEC
		public override string ToString()
		{
			return string.Format("[Vehicle Id={0}, BehaviorState={1}]", this.id, this.behaviorState);
		}

		// Token: 0x0600228B RID: 8843 RVA: 0x0008B910 File Offset: 0x00089B10
		public void ClearReturnPath()
		{
			foreach (LaneModel oldReturnLane in this.returnPath)
			{
				if (Diagnostics.Verify(oldReturnLane.roadChunk != null, "Tried to remove ourselves from a return lane with no road chunk. ({0})", oldReturnLane))
				{
					oldReturnLane.roadChunk.RemoveInboundVehicle(this, oldReturnLane, true);
				}
			}
			this.returnPath.Clear();
		}

		// Token: 0x0600228C RID: 8844 RVA: 0x0008B98C File Offset: 0x00089B8C
		private void ClearForwardPath()
		{
			foreach (LaneModel lane in this.path)
			{
				lane.roadChunk.RemoveInboundVehicle(this, lane, false);
				lane.RemoveVehicle(this);
			}
			base.CurrentFrame.lane.RemoveVehicle(this);
			this.path.Clear();
			foreach (LaneModel lane2 in base.CurrentFrame.lane.OutboundLanes)
			{
				if (lane2.Vehicles.Contains(this))
				{
					lane2.RemoveVehicle(this);
				}
			}
		}

		// Token: 0x0600228D RID: 8845 RVA: 0x0008BA64 File Offset: 0x00089C64
		private void ClearLeadingVehicles()
		{
			foreach (VehicleModel vehicle in this._simulation.GetModels<VehicleModel>())
			{
				if (vehicle.blockingVehicle == this)
				{
					vehicle.blockingVehicle = null;
					vehicle.CurrentFrame.nearestObstacle = VehicleModel.ObstacleType.None;
					vehicle.CurrentFrame.leadingVehicle = null;
					vehicle.NextFrame.nearestObstacle = VehicleModel.ObstacleType.None;
					vehicle.NextFrame.leadingVehicle = null;
				}
			}
			this.blockingVehicle = null;
			base.CurrentFrame.nearestObstacle = VehicleModel.ObstacleType.None;
			base.NextFrame.nearestObstacle = VehicleModel.ObstacleType.None;
			base.CurrentFrame.leadingVehicle = null;
			base.NextFrame.leadingVehicle = null;
		}

		// Token: 0x0600228E RID: 8846 RVA: 0x0008BB0F File Offset: 0x00089D0F
		public VehicleModel() : base(1)
		{
		}

		// Token: 0x04001C37 RID: 7223
		[Dependency]
		private Simulation _simulation;

		// Token: 0x04001C38 RID: 7224
		[Dependency]
		private ClockModel _clock;

		// Token: 0x04001C39 RID: 7225
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001C3A RID: 7226
		[Serialize(false, null)]
		public int id = -1;

		// Token: 0x04001C3B RID: 7227
		private static int NextId = 1;

		// Token: 0x04001C3C RID: 7228
		public const int NumberOfLanesToCommitTo = 2;

		// Token: 0x04001C3D RID: 7229
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("VehicleModel");

		// Token: 0x04001C3E RID: 7230
		public int latestAttemptedPathfindFrame;

		// Token: 0x04001C3F RID: 7231
		public HouseModel house;

		// Token: 0x04001C40 RID: 7232
		public DestinationModel destination;

		// Token: 0x04001C41 RID: 7233
		public DestinationModel lastVisitedDestination;

		// Token: 0x04001C42 RID: 7234
		public VehicleModel.BehaviorState behaviorState;

		// Token: 0x04001C43 RID: 7235
		public VehicleModel.BehaviorState lastNotifiedBehaviorState;

		// Token: 0x04001C44 RID: 7236
		public readonly List<LaneModel> path = new List<LaneModel>();

		// Token: 0x04001C45 RID: 7237
		public VehicleModel.PathfindUrgency repathUrgency;

		// Token: 0x04001C46 RID: 7238
		public Fix64 targetDistanceAlongLastLane;

		// Token: 0x04001C47 RID: 7239
		public Fix64 pathLength;

		// Token: 0x04001C48 RID: 7240
		public Fix64 pathLengthAtStartOfJourney;

		// Token: 0x04001C49 RID: 7241
		public bool isShovingIntoNextIntersection;

		// Token: 0x04001C4A RID: 7242
		public VehicleModel vehiclePushingInto;

		// Token: 0x04001C4B RID: 7243
		[Serialize(false, null)]
		public VehicleModel blockingVehicle;

		// Token: 0x04001C4C RID: 7244
		[Serialize(false, null)]
		public int frameBlockingChainLastChecked;

		// Token: 0x04001C4D RID: 7245
		private Fix64 _timeWhenArrivedAtHouse;

		// Token: 0x04001C4E RID: 7246
		public readonly List<LaneModel> returnPath = new List<LaneModel>();

		// Token: 0x04001C4F RID: 7247
		public VehicleModel.PathfindUrgency returnRepathUrgency;

		// Token: 0x04001C50 RID: 7248
		private static readonly ProfilerMarker Profiler_AssignPath = new ProfilerMarker(ProfilerUtility.CategoryModel, "VehicleModel.AssignPath");

		// Token: 0x04001C51 RID: 7249
		private static readonly ProfilerMarker Profiler_AssignReturnPath = new ProfilerMarker(ProfilerUtility.CategoryModel, "VehicleModel.AssignReturnPath");

		// Token: 0x04001C52 RID: 7250
		private static readonly ProfilerMarker Profiler_DistanceToLane = new ProfilerMarker(ProfilerUtility.CategoryModel, "VehicleModel.DistanceToLane");

		// Token: 0x04001C53 RID: 7251
		private static readonly ProfilerMarker Profiler_ClearNonCommittedLanes = new ProfilerMarker(ProfilerUtility.CategoryModel, "VehicleModel.ClearNonCommittedLanes");

		// Token: 0x04001C54 RID: 7252
		private static readonly ProfilerMarker Profiler_ClearReturnPath = new ProfilerMarker(ProfilerUtility.CategoryModel, "VehicleModel.ClearReturnPath");

		// Token: 0x02000514 RID: 1300
		public enum ObstacleType
		{
			// Token: 0x04001C56 RID: 7254
			None,
			// Token: 0x04001C57 RID: 7255
			Target,
			// Token: 0x04001C58 RID: 7256
			LeadingVehicle,
			// Token: 0x04001C59 RID: 7257
			BlockingIntersection,
			// Token: 0x04001C5A RID: 7258
			HotswappingLane
		}

		// Token: 0x02000515 RID: 1301
		public enum PathfindUrgency
		{
			// Token: 0x04001C5C RID: 7260
			NotRequired,
			// Token: 0x04001C5D RID: 7261
			WhenPossible,
			// Token: 0x04001C5E RID: 7262
			AsSoonAsPossible
		}

		// Token: 0x02000516 RID: 1302
		public enum BehaviorState
		{
			// Token: 0x04001C60 RID: 7264
			WaitingForDestination,
			// Token: 0x04001C61 RID: 7265
			DrivingToDestination,
			// Token: 0x04001C62 RID: 7266
			ParkingAtDestination,
			// Token: 0x04001C63 RID: 7267
			ParkedAtDestination,
			// Token: 0x04001C64 RID: 7268
			DrivingHome,
			// Token: 0x04001C65 RID: 7269
			RealigningDriveway
		}

		// Token: 0x02000517 RID: 1303
		public class Frame : IFrame
		{
			// Token: 0x06002290 RID: 8848 RVA: 0x0008BBC0 File Offset: 0x00089DC0
			public bool CloneInto(IFrame cloneFrame, IScope scope)
			{
				VehicleModel.Frame frame = (VehicleModel.Frame)cloneFrame;
				frame.lane = this.lane;
				frame.distanceAlongLane = this.distanceAlongLane;
				frame.acceleration = this.acceleration;
				frame.speed = this.speed;
				frame.nearestObstacle = this.nearestObstacle;
				frame.leadingVehicle = this.leadingVehicle;
				frame.distanceToLeadingVehicle = this.distanceToLeadingVehicle;
				frame.blockingLane = this.blockingLane;
				frame.distanceToBlockingLane = this.distanceToBlockingLane;
				return true;
			}

			// Token: 0x06002291 RID: 8849 RVA: 0x0008BC40 File Offset: 0x00089E40
			public void Reset()
			{
				this.lane = null;
				this.distanceAlongLane = Fix64.Zero;
				this.speed = Fix64.Zero;
				this.acceleration = Fix64.Zero;
				this.nearestObstacle = VehicleModel.ObstacleType.None;
				this.leadingVehicle = null;
				this.distanceToLeadingVehicle = Fix64.MaxValue;
				this.blockingLane = null;
				this.distanceToBlockingLane = Fix64.MaxValue;
			}

			// Token: 0x04001C66 RID: 7270
			public LaneModel lane;

			// Token: 0x04001C67 RID: 7271
			public Fix64 distanceAlongLane;

			// Token: 0x04001C68 RID: 7272
			public Fix64 speed;

			// Token: 0x04001C69 RID: 7273
			public Fix64 acceleration;

			// Token: 0x04001C6A RID: 7274
			public VehicleModel.ObstacleType nearestObstacle;

			// Token: 0x04001C6B RID: 7275
			public VehicleModel leadingVehicle;

			// Token: 0x04001C6C RID: 7276
			public Fix64 distanceToLeadingVehicle = Fix64.MaxValue;

			// Token: 0x04001C6D RID: 7277
			public LaneModel blockingLane;

			// Token: 0x04001C6E RID: 7278
			public Fix64 distanceToBlockingLane = Fix64.MaxValue;
		}

		// Token: 0x02000518 RID: 1304
		public interface IObserver
		{
			// Token: 0x06002293 RID: 8851
			void OnVehicleMovedToNewLane(LaneModel newLane, LaneModel oldLane);

			// Token: 0x06002294 RID: 8852
			void OnVehicleEnteredCarpark(VehicleModel vehicle, DestinationModel destination);

			// Token: 0x06002295 RID: 8853
			void OnVehicleArrivedAtDestination(VehicleModel vehicle, DestinationModel destination);

			// Token: 0x06002296 RID: 8854
			void OnVehicleDepartedDestination(VehicleModel vehicle, DestinationModel fromDestination);

			// Token: 0x06002297 RID: 8855
			void OnVehicleArrivedAtHouse(VehicleModel vehicle);

			// Token: 0x06002298 RID: 8856
			void OnVehicleDepartedHouse(VehicleModel vehicle, DestinationModel toDestination);

			// Token: 0x06002299 RID: 8857
			void OnRemoved();
		}
	}
}
