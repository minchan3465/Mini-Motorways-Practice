using System;
using System.IO;
using Factory;
using Motorways.Models;
using Server;

namespace Motorways.Actions
{
	// Token: 0x020006FE RID: 1790
	public class DebugTestSerialization : MotorwaysPlayerAction
	{
		// Token: 0x060030FF RID: 12543 RVA: 0x000E6424 File Offset: 0x000E4624
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			MemoryStream simulationDataStream = new MemoryStream();
			using (BinaryWriter writer = new BinaryWriter(simulationDataStream))
			{
				base.Scope.Export(this._simulation, writer);
			}
			byte[] buffer = simulationDataStream.ToArray();
			IScope appScope = base.Scope.ParentScope;
			Scope newScope = new Scope(base.Scope.Assembler, null);
			newScope.ParentScope = appScope;
			simulationDataStream = new MemoryStream(buffer);
			Simulation newSimulation;
			using (BinaryReader reader = new BinaryReader(simulationDataStream))
			{
				newSimulation = newScope.Import<Simulation>(reader);
			}
			City oldCity = this._simulation.Scope.Get<City>();
			newScope.Get<City>().Initialize(oldCity.Definition, oldCity.Rules);
			this.CompareSimulations((Simulation)this._simulation, newSimulation);
			for (int stepIndex = 0; stepIndex < 10; stepIndex++)
			{
				this._simulation.Step();
				newSimulation.Step();
				this.CompareSimulations((Simulation)this._simulation, newSimulation);
			}
			newScope.Release();
		}

		// Token: 0x06003100 RID: 12544 RVA: 0x000020A2 File Offset: 0x000002A2
		public override void Tick(float frameTime)
		{
			this.OnActionComplete();
		}

		// Token: 0x06003101 RID: 12545 RVA: 0x000E6550 File Offset: 0x000E4750
		private void CompareSimulations(Simulation oldSimulation, Simulation newSimulation)
		{
			oldSimulation.Scope.Get<Clock>();
			newSimulation.Scope.Get<Clock>();
			ModelList<LaneModel> oldLanes = oldSimulation.GetModels<LaneModel>();
			newSimulation.GetModels<LaneModel>();
			DebugTestSerialization.Log.Info("Matching {0} lanes.", new object[]
			{
				oldLanes.Count
			});
			for (int laneIndex = 0; laneIndex < oldLanes.Count; laneIndex++)
			{
			}
			ModelList<RoadChunkModel> oldChunks = oldSimulation.GetModels<RoadChunkModel>();
			ModelList<RoadChunkModel> newChunks = newSimulation.GetModels<RoadChunkModel>();
			DebugTestSerialization.Log.Info("Matching {0} road chunks.", new object[]
			{
				oldChunks.Count
			});
			for (int chunkIndex = 0; chunkIndex < oldChunks.Count; chunkIndex++)
			{
				RoadChunkModel oldChunk = oldChunks[chunkIndex];
				RoadChunkModel newChunk = newChunks[chunkIndex];
				oldChunk.SortInboundVehicles();
				newChunk.SortInboundVehicles();
				if (Diagnostics.Verify(oldChunk.traversingVehicles.Count == newChunk.traversingVehicles.Count))
				{
					for (int traversingVehicleIndex = 0; traversingVehicleIndex < oldChunk.traversingVehicles.Count; traversingVehicleIndex++)
					{
						VehicleModel vehicleModel = oldChunk.traversingVehicles[traversingVehicleIndex];
						VehicleModel vehicleModel2 = newChunk.traversingVehicles[traversingVehicleIndex];
					}
				}
			}
			ModelList<VehicleModel> oldVehicles = oldSimulation.GetModels<VehicleModel>();
			ModelList<VehicleModel> newVehicles = newSimulation.GetModels<VehicleModel>();
			DebugTestSerialization.Log.Info("Matching {0} vehicles.", new object[]
			{
				oldVehicles.Count
			});
			for (int vehicleIndex = 0; vehicleIndex < oldVehicles.Count; vehicleIndex++)
			{
				VehicleModel oldVehicle = oldVehicles[vehicleIndex];
				VehicleModel newVehicle = newVehicles[vehicleIndex];
				DebugTestSerialization.Log.Info("Matching path of length {0}.", new object[]
				{
					oldVehicle.path.Count
				});
				for (int pathIndex = 0; pathIndex < oldVehicle.path.Count; pathIndex++)
				{
				}
				DebugTestSerialization.Log.Info("Matching return path of length {0}.", new object[]
				{
					oldVehicle.returnPath.Count
				});
				for (int returnPathIndex = 0; returnPathIndex < oldVehicle.returnPath.Count; returnPathIndex++)
				{
				}
				for (int pathIndex2 = 0; pathIndex2 < oldVehicle.path.Count; pathIndex2++)
				{
					foreach (RoadChunkModel.InboundVehicle inboundVehicle in newVehicle.path[pathIndex2].roadChunk.inboundVehicles)
					{
						if (inboundVehicle.vehicle == newVehicle)
						{
							LaneModel chosenLane = inboundVehicle.chosenLane;
							LaneModel laneModel = newVehicle.path[pathIndex2];
						}
					}
				}
				for (int pathIndex3 = 0; pathIndex3 < oldVehicle.returnPath.Count; pathIndex3++)
				{
					foreach (RoadChunkModel.InboundVehicle inboundVehicle2 in newVehicle.returnPath[pathIndex3].roadChunk.returningInboundVehicles)
					{
						if (inboundVehicle2.vehicle == newVehicle)
						{
							LaneModel chosenLane2 = inboundVehicle2.chosenLane;
							LaneModel laneModel2 = newVehicle.returnPath[pathIndex3];
						}
					}
				}
			}
		}

		// Token: 0x06003102 RID: 12546 RVA: 0x000E6894 File Offset: 0x000E4A94
		public static DebugTestSerialization Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			DebugTestSerialization debugTestSerialization = scope.Get<DebugTestSerialization>();
			debugTestSerialization.InitializeAction(owningGroup, timestamp);
			debugTestSerialization.OnActionBegin(timestamp);
			return debugTestSerialization;
		}

		// Token: 0x04002A12 RID: 10770
		private new static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("DebugTestSerialization");
	}
}
