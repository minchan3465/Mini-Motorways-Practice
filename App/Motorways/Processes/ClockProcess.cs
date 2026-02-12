using System;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x02000489 RID: 1161
	public class ClockProcess : IProcess, IReusable
	{
		// Token: 0x06001CD2 RID: 7378 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001CD3 RID: 7379 RVA: 0x0006DC14 File Offset: 0x0006BE14
		public void Step(ISimulation simulation, Fix64 deltaTime)
		{
			foreach (ClockModel clock in simulation.GetModels<ClockModel>())
			{
				if (!clock.isPaused)
				{
					clock.NextFrame.time = clock.CurrentFrame.time + deltaTime * this._city.Rules.GetClockSpeedMultiplier();
					if (!this._city.Rules.CanExpansionTimeContinue || clock.expansionTimeManuallyPaused)
					{
						deltaTime = Fix64.Zero;
					}
					clock.NextFrame.expansionTime = clock.CurrentFrame.expansionTime + deltaTime * this._city.Rules.GetClockSpeedMultiplier();
				}
			}
		}

		// Token: 0x040018CF RID: 6351
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ClockProcess");

		// Token: 0x040018D0 RID: 6352
		[Dependency]
		private City _city;
	}
}
