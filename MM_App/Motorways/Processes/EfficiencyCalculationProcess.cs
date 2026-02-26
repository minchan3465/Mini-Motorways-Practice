using System;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using Unity.Profiling;

namespace Motorways.Processes
{
	// Token: 0x0200048B RID: 1163
	public class EfficiencyCalculationProcess : IProcess, IReusable
	{
		// Token: 0x06001CDA RID: 7386 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001CDB RID: 7387 RVA: 0x0006E118 File Offset: 0x0006C318
		public void Step(ISimulation simulation, Fix64 deltaTime)
		{
			foreach (ScoreModel scoreModel in simulation.GetModels<ScoreModel>())
			{
				if (scoreModel.HasAchievedCurrentMilestone())
				{
					scoreModel.ProgressToNextMilestone();
				}
				scoreModel.DeductEfficiencyScore(deltaTime);
			}
		}

		// Token: 0x040018D4 RID: 6356
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("EfficiencyCalculationProcess");

		// Token: 0x040018D5 RID: 6357
		private static readonly ProfilerMarker Profiler_Step = new ProfilerMarker(ProfilerUtility.CategoryProcess, "EfficiencyCalculationProcess.Step");
	}
}
