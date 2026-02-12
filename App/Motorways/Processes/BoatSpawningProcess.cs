using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x02000484 RID: 1156
	public class BoatSpawningProcess : IProcess, IReusable
	{
		// Token: 0x06001CAE RID: 7342 RVA: 0x0006B418 File Offset: 0x00069618
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			foreach (BoatPathModel boatPathModel in simulation.GetModels<BoatPathModel>())
			{
				List<BoatPathTileModel> boatSpawnTiles = boatPathModel.BoatSpawnTiles;
				while (boatPathModel.BoatCount < boatSpawnTiles.Count && boatPathModel.BoatCount < 1)
				{
					BoatModel boatModel = simulation.Scope.Get<BoatModel>();
					boatModel.state = BoatModel.BehaviorState.Sailing;
					boatModel.CurrentFrame.speed = Fix64.Zero;
					boatModel.NextFrame.speed = Fix64.Zero;
					boatModel.CurrentFrame.tile = boatSpawnTiles[boatPathModel.BoatCount];
					boatModel.CurrentFrame.DistanceAlongPathSegment = Fix64.Zero;
					boatModel.NextFrame.tile = boatSpawnTiles[boatPathModel.BoatCount];
					boatModel.NextFrame.DistanceAlongPathSegment = boatModel.CurrentFrame.DistanceAlongPathSegment;
					boatPathModel.AddBoat(boatModel);
					simulation.AddModel(boatModel);
				}
			}
		}

		// Token: 0x06001CAF RID: 7343 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x040018A6 RID: 6310
		private const int BoatsPerLine = 1;

		// Token: 0x040018A7 RID: 6311
		[Dependency]
		private SimulationConstantsData _constants;
	}
}
