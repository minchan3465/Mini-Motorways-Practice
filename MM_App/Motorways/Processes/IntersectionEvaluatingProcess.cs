using System;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x02000490 RID: 1168
	public class IntersectionEvaluatingProcess : IProcess, IReusable
	{
		// Token: 0x06001CF4 RID: 7412 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001CF5 RID: 7413 RVA: 0x0006F328 File Offset: 0x0006D528
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			foreach (RoadChunkModel roadChunkModel in simulation.GetModels<RoadChunkModel>())
			{
				roadChunkModel.SortInboundVehicles();
			}
		}
	}
}
