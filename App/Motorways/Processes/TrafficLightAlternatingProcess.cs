using System;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x0200049D RID: 1181
	public class TrafficLightAlternatingProcess : IProcess, IReusable
	{
		// Token: 0x06001D35 RID: 7477 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001D36 RID: 7478 RVA: 0x0007354C File Offset: 0x0007174C
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			foreach (TrafficLightModel trafficLight in simulation.GetModels<TrafficLightModel>())
			{
				trafficLight.durationOnCurrentPair += timestep;
				if (trafficLight.requiresPairCalculation)
				{
					TileDirectionBitfield cachedActivePair = trafficLight.ActivePair;
					trafficLight.CalculatePairs();
					if (!trafficLight.SetActivePair(cachedActivePair))
					{
						trafficLight.RotateLights();
						trafficLight.durationOnCurrentPair = Fix64.Zero;
					}
				}
				if (trafficLight.amberLightsOn && trafficLight.durationOnCurrentPair > this._constants.amberDelay)
				{
					trafficLight.RotateLights();
					trafficLight.durationOnCurrentPair = Fix64.Zero;
				}
				else if (!trafficLight.amberLightsOn && (trafficLight.durationOnCurrentPair > this._constants.changeDelay || (trafficLight.isInOvertime && trafficLight.durationOnCurrentPair > this._constants.overtimeChangeDelay)))
				{
					if (trafficLight.RequiresRotation())
					{
						trafficLight.isInOvertime = false;
						trafficLight.ChangeGreenToAmber();
					}
					else
					{
						trafficLight.isInOvertime = true;
					}
					trafficLight.durationOnCurrentPair = Fix64.Zero;
				}
			}
		}

		// Token: 0x04001919 RID: 6425
		[Dependency]
		private SimulationConstantsData _constants;
	}
}
