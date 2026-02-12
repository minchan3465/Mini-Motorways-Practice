using System;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x02000488 RID: 1160
	public class BuildRoundaboutsProcess : IProcess, IReusable
	{
		// Token: 0x06001CCE RID: 7374 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001CCF RID: 7375 RVA: 0x0006DBAC File Offset: 0x0006BDAC
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			foreach (RoundaboutModel roundabout in simulation.GetModels<RoundaboutModel>())
			{
				if (roundabout.State == RoadState.Planned && roundabout.Activate())
				{
					BuildRoundaboutsProcess.Log.Info("Activated roundabout.", Array.Empty<object>());
				}
			}
		}

		// Token: 0x040018CE RID: 6350
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("BuildRoundaboutsProcess");
	}
}
