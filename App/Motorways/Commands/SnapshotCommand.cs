using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Commands
{
	// Token: 0x02000524 RID: 1316
	public class SnapshotCommand : Command, IReleasedFromScopeHandler
	{
		// Token: 0x060022C7 RID: 8903 RVA: 0x0008CAFC File Offset: 0x0008ACFC
		public override void Execute(ISimulation simulation)
		{
			ulong prngSeed = simulation.GetModel<CityModel>().pseudorandomGenerator.Seed;
			int scheduledBuildingCount = simulation.GetModel<CityPlanModel>().scheduledBuildings.Count;
			List<Vector2Int> houseCoordinates = new List<Vector2Int>();
			foreach (HouseModel house in simulation.GetModels<HouseModel>())
			{
				houseCoordinates.Add(house.tileModel.Coordinates);
			}
			List<Vector2Int> destinationCoordinates = new List<Vector2Int>();
			List<Fix64> destinationDemandTimers = new List<Fix64>();
			foreach (DestinationModel destination in simulation.GetModels<DestinationModel>())
			{
				destinationCoordinates.Add(destination.TileModels[0].Coordinates);
				destinationDemandTimers.Add(destination.demandTimer);
			}
			List<Fix64> vehicleLaneDistances = new List<Fix64>();
			foreach (VehicleModel vehicle in simulation.GetModels<VehicleModel>())
			{
				vehicleLaneDistances.Add(vehicle.CurrentFrame.distanceAlongLane);
			}
			SnapshotModel snapshotModel = simulation.GetModel<SnapshotModel>();
			List<VehicleDispatchRecord> vehicleDispatches = null;
			if (snapshotModel != null)
			{
				vehicleDispatches = snapshotModel.vehicleDispatches;
			}
			if (this._prngSeed == 0UL)
			{
				this._prngSeed = prngSeed;
				this._scheduledBuildingCount = scheduledBuildingCount;
				this._vehicleLaneDistances.AddRange(vehicleLaneDistances);
				this._destinationCoordinates.AddRange(destinationCoordinates);
				this._destinationDemandTimers.AddRange(destinationDemandTimers);
				this._houseCoordinates.AddRange(houseCoordinates);
				if (vehicleDispatches != null)
				{
					this._vehicleDispatches.AddRange(vehicleDispatches);
					snapshotModel.vehicleDispatches.Clear();
					return;
				}
			}
			else
			{
				if (Diagnostics.Verify(this._vehicleLaneDistances.Count == vehicleLaneDistances.Count, "Detected divergence in vehicle count on frame {0}.", this._clock.FrameCount))
				{
					for (int vehicleIndex = 0; vehicleIndex < this._vehicleLaneDistances.Count; vehicleIndex++)
					{
					}
				}
				if (Diagnostics.Verify(this._houseCoordinates.Count == houseCoordinates.Count, "Detected divergence in house count on frame {0}.", this._clock.FrameCount))
				{
					for (int houseIndex = 0; houseIndex < this._houseCoordinates.Count; houseIndex++)
					{
					}
				}
				if (Diagnostics.Verify(this._destinationCoordinates.Count == destinationCoordinates.Count, "Detected divergence in destination count on frame {0}.", this._clock.FrameCount))
				{
					for (int destinationIndex = 0; destinationIndex < this._destinationCoordinates.Count; destinationIndex++)
					{
					}
				}
				if (vehicleDispatches != null)
				{
					if (Diagnostics.Verify(this._vehicleDispatches.Count == vehicleDispatches.Count, "Detected divergence in vehicle dispatches on frame {0}.", this._clock.FrameCount))
					{
						for (int vehicleDispatchIndex = 0; vehicleDispatchIndex < this._vehicleDispatches.Count; vehicleDispatchIndex++)
						{
						}
					}
					foreach (VehicleDispatchRecord vehicleDispatchRecord in snapshotModel.vehicleDispatches)
					{
						simulation.Scope.Release(vehicleDispatchRecord);
					}
					snapshotModel.vehicleDispatches.Clear();
				}
			}
		}

		// Token: 0x060022C8 RID: 8904 RVA: 0x0008CE0C File Offset: 0x0008B00C
		public override void Reset()
		{
			base.Reset();
			this._prngSeed = 0UL;
			this._scheduledBuildingCount = 0;
			this._vehicleLaneDistances.Clear();
			this._vehicleDispatches.Clear();
			this._houseCoordinates.Clear();
			this._destinationCoordinates.Clear();
			this._destinationDemandTimers.Clear();
		}

		// Token: 0x060022C9 RID: 8905 RVA: 0x0008CE68 File Offset: 0x0008B068
		public void OnReleasedFromScope(IScope scope)
		{
			foreach (VehicleDispatchRecord record in this._vehicleDispatches)
			{
				scope.Release(record);
			}
			this._vehicleDispatches.Clear();
		}

		// Token: 0x04001CD4 RID: 7380
		private ulong _prngSeed;

		// Token: 0x04001CD5 RID: 7381
		private int _scheduledBuildingCount;

		// Token: 0x04001CD6 RID: 7382
		private readonly List<Fix64> _vehicleLaneDistances = new List<Fix64>();

		// Token: 0x04001CD7 RID: 7383
		private readonly List<VehicleDispatchRecord> _vehicleDispatches = new List<VehicleDispatchRecord>();

		// Token: 0x04001CD8 RID: 7384
		private readonly List<Vector2Int> _houseCoordinates = new List<Vector2Int>();

		// Token: 0x04001CD9 RID: 7385
		private readonly List<Vector2Int> _destinationCoordinates = new List<Vector2Int>();

		// Token: 0x04001CDA RID: 7386
		private readonly List<Fix64> _destinationDemandTimers = new List<Fix64>();

		// Token: 0x04001CDB RID: 7387
		[Dependency]
		private Clock _clock;
	}
}
