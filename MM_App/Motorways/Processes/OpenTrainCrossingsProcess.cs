using System;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x02000492 RID: 1170
	public class OpenTrainCrossingsProcess : IProcess, IReusable
	{
		// Token: 0x06001CFD RID: 7421 RVA: 0x000700F0 File Offset: 0x0006E2F0
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			foreach (TrainCrossingModel trainCrossingModel in simulation.GetModels<TrainCrossingModel>())
			{
				if (trainCrossingModel.HasPendingSignalOpenRequestTimeElapsed())
				{
					trainCrossingModel.CommitPendingSignalOpenRequest();
				}
			}
		}

		// Token: 0x06001CFE RID: 7422 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}
	}
}
