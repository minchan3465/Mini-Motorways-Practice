using System;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x0200048C RID: 1164
	public class FailureStateProcess : IProcess, IReusable
	{
		// Token: 0x06001CDE RID: 7390 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001CDF RID: 7391 RVA: 0x0006E184 File Offset: 0x0006C384
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			foreach (DestinationModel destination in simulation.GetModels<DestinationModel>())
			{
				Fix64 timerDelta = -this._constants.OvercrowdTimerReturnSpeed;
				Fix64 overcrowdingTime = destination.CurrentFrame.OvercrowdingTime;
				if (destination.IsOvercrowding && this._city.Rules.CanDestinationsOvercrowd)
				{
					Fix64 overcrowdingProgress = destination.CurrentFrame.OvercrowdingTime / this._constants.MaxOvercrowdTime;
					Fix64 overcrowdingSpeedMultiplier = this._city.Rules.GetOvercrowdingSpeedMultiplier(overcrowdingProgress);
					Fix64 extraDemand;
					if (this._demand.extraDemand.TryGetValue(destination.GroupIndex, out extraDemand))
					{
						overcrowdingSpeedMultiplier *= this._constants.GetOvercrowdTimerSpeedMultiplierForExtraDemand(extraDemand);
					}
					timerDelta = destination.CurrentFrame.OvercrowdingSpeed * overcrowdingSpeedMultiplier;
					Fix64 overcrowdingSpeed = destination.CurrentFrame.OvercrowdingSpeed;
					if (destination.demandJustCleared > 0)
					{
						overcrowdingTime = Fix64.Min(overcrowdingTime, this._constants.MaxOvercrowdTime - this._constants.GracePeriodTime);
						for (int justClearedDemand = 0; justClearedDemand < destination.demandJustCleared; justClearedDemand++)
						{
							overcrowdingSpeed *= this._constants.OvercrowdTimerCarArrivalDeceleration;
							Fix64 timerReductionForArrivingVehicle = this._constants.PercentageToReduceTimerOnCarArrival / (Fix64)100L * overcrowdingTime;
							timerReductionForArrivingVehicle *= this._constants.GetCarArrivalPinReductionMultiplierOverTime(this._clock.Time);
							timerReductionForArrivingVehicle = Fix64.Clamp(timerReductionForArrivingVehicle, this._constants.MinimumAmountToReduceTimerOnCarArrival, this._constants.MaximumAmountToReduceTimerOnCarArrival);
							overcrowdingTime -= timerReductionForArrivingVehicle;
							overcrowdingTime = Fix64.Max(overcrowdingTime, Fix64.Zero);
						}
						destination.demandJustCleared = 0;
					}
					overcrowdingSpeed += this._constants.OvercrowdTimerAcceleration * timestep;
					destination.SetNextFrameOvercrowdingSpeed(overcrowdingSpeed);
				}
				else
				{
					destination.SetNextFrameOvercrowdingSpeed(this._constants.MinimumOvercrowdTimerSpeed);
					overcrowdingTime = Fix64.Min(overcrowdingTime, this._constants.MaxOvercrowdTime - this._constants.GracePeriodTime);
				}
				destination.NextFrame.OvercrowdingTime = overcrowdingTime + timerDelta * timestep;
				if (destination.CurrentFrame.OvercrowdingTime > this._constants.MaxOvercrowdTime && !simulation.IsPaused)
				{
					destination.OnOvercrowded();
				}
			}
		}

		// Token: 0x040018D6 RID: 6358
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x040018D7 RID: 6359
		[Dependency]
		private DemandModel _demand;

		// Token: 0x040018D8 RID: 6360
		[Dependency]
		private City _city;

		// Token: 0x040018D9 RID: 6361
		[Dependency]
		private ClockModel _clock;
	}
}
