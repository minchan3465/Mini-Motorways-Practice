using System;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Processes
{
	// Token: 0x0200049B RID: 1179
	public class ReleaseMotorwaysProcess : IProcess, IReusable
	{
		// Token: 0x06001D2B RID: 7467 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001D2C RID: 7468 RVA: 0x00073064 File Offset: 0x00071264
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			foreach (MotorwayModel motorway in simulation.GetModels<MotorwayModel>())
			{
				if (motorway.State == RoadState.None)
				{
					int concreteCost = motorway.ConcreteCost - motorway.ConcreteGivenToReplacement;
					if (concreteCost > 0)
					{
						this._upgradeDatabase.ReleaseMothballedUpgrade(UpgradeType.Concrete, concreteCost);
					}
					if (motorway.hasConsumedUpgrade)
					{
						ReleaseMotorwaysProcess.Log.Info("Closing motorway {0}, releasing {1} concrete and one upgrade.", new object[]
						{
							motorway.Id,
							concreteCost
						});
						motorway.hasConsumedUpgrade = false;
						MotorwayModel replacementMotorway = this.FindReplacementMotorway(simulation, motorway);
						if (replacementMotorway != null)
						{
							ReleaseMotorwaysProcess.Log.Info("Gifting upgrade to motorway {0} instead of releasing it.", new object[]
							{
								replacementMotorway.Id
							});
							replacementMotorway.hasConsumedUpgrade = true;
						}
						else
						{
							this._upgradeDatabase.MothballUpgrade(UpgradeType.Motorway, 1);
							this._upgradeDatabase.ReleaseMothballedUpgrade(UpgradeType.Motorway, 1);
						}
					}
					else
					{
						ReleaseMotorwaysProcess.Log.Info("Closing motorway {0}, releasing {1} concrete and no upgrade.", new object[]
						{
							motorway.Id,
							concreteCost
						});
					}
					Diagnostics.Verify(this._tilemapModel.RemoveMotorwayModel(motorway), "Failed to remove motorway {0} from the simulation's tilemap.", motorway);
					simulation.RemoveModel(motorway);
				}
			}
		}

		// Token: 0x06001D2D RID: 7469 RVA: 0x000731A8 File Offset: 0x000713A8
		private MotorwayModel FindReplacementMotorway(ISimulation simulation, MotorwayModel oldMotorway)
		{
			int replacementProximity = -1;
			MotorwayModel replacementMotorway = null;
			foreach (MotorwayModel plannedMotorway in simulation.GetModels<MotorwayModel>())
			{
				if (plannedMotorway.State == RoadState.Planned && !plannedMotorway.hasConsumedUpgrade && plannedMotorway.CanSetMotorwayAndNodeState(RoadState.Active))
				{
					int proximity = (plannedMotorway.StartCoordinates - oldMotorway.StartCoordinates).sqrMagnitude;
					proximity = Mathf.Min(proximity, (plannedMotorway.StartCoordinates - oldMotorway.EndCoordinates).sqrMagnitude);
					proximity = Mathf.Min(proximity, (plannedMotorway.EndCoordinates - oldMotorway.StartCoordinates).sqrMagnitude);
					proximity = Mathf.Min(proximity, (plannedMotorway.EndCoordinates - oldMotorway.EndCoordinates).sqrMagnitude);
					if (replacementMotorway == null || (plannedMotorway.isHighBuildPriority && !replacementMotorway.isHighBuildPriority) || proximity < replacementProximity)
					{
						replacementMotorway = plannedMotorway;
						replacementProximity = proximity;
					}
				}
			}
			return replacementMotorway;
		}

		// Token: 0x04001913 RID: 6419
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ReleaseMotorwaysProcess");

		// Token: 0x04001914 RID: 6420
		[Dependency]
		private UpgradeDatabaseModel _upgradeDatabase;

		// Token: 0x04001915 RID: 6421
		[Dependency]
		private TilemapModel _tilemapModel;
	}
}
