using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x0200049F RID: 1183
	public class TrainSpawningProcess : IProcess, IReusable
	{
		// Token: 0x06001D3C RID: 7484 RVA: 0x00073E0C File Offset: 0x0007200C
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			foreach (TrainLineModel trainLineModel in simulation.GetModels<TrainLineModel>())
			{
				List<RailTileModel> trainSpawnTiles = trainLineModel.TrainSpawnTiles;
				if (trainLineModel.TrainCount < trainSpawnTiles.Count && trainLineModel.TrainCount < 1)
				{
					foreach (RailTileModel trainSpawnTile in trainSpawnTiles)
					{
						TrainModel trainModel = simulation.Scope.Get<TrainModel>();
						trainModel.state = TrainModel.BehaviorState.Stopped;
						trainModel.CurrentFrame.speed = Fix64.Zero;
						trainModel.NextFrame.speed = Fix64.Zero;
						trainModel.CurrentFrame.tile = trainSpawnTile;
						trainModel.CurrentFrame.distanceAlongTrack = Fix64.Zero;
						trainModel.NextFrame.tile = trainSpawnTile;
						trainModel.NextFrame.distanceAlongTrack = trainModel.CurrentFrame.distanceAlongTrack;
						trainLineModel.AddTrain(trainModel);
						simulation.AddModel(trainModel);
					}
				}
			}
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x0400191C RID: 6428
		private const int TrainsPerLine = 1;

		// Token: 0x0400191D RID: 6429
		[Dependency]
		private SimulationConstantsData _constants;
	}
}
