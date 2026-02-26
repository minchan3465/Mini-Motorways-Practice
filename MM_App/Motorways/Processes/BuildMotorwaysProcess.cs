using System;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x02000487 RID: 1159
	public class BuildMotorwaysProcess : IProcess, IReusable
	{
		// Token: 0x06001CC9 RID: 7369 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001CCA RID: 7370 RVA: 0x0006D968 File Offset: 0x0006BB68
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			foreach (MotorwayModel motorway in simulation.GetModels<MotorwayModel>())
			{
				if (motorway.isHighBuildPriority && Diagnostics.Verify(this.TryBuildMotorway(motorway), "Unable to build high-priority motorway! Things are going to get ugly."))
				{
					motorway.isHighBuildPriority = false;
				}
			}
			foreach (MotorwayModel motorway2 in simulation.GetModels<MotorwayModel>())
			{
				if (!motorway2.isHighBuildPriority)
				{
					this.TryBuildMotorway(motorway2);
				}
			}
		}

		// Token: 0x06001CCB RID: 7371 RVA: 0x0006D9EC File Offset: 0x0006BBEC
		private bool TryBuildMotorway(MotorwayModel motorway)
		{
			if (motorway.State != RoadState.Planned)
			{
				return false;
			}
			MotorwayModel mothballedReplacement;
			if (motorway.CanBeReplacedByActivatingMothballedMotorway(out mothballedReplacement))
			{
				BuildMotorwaysProcess.Log.Info("Replacing planned motorway {0} by re-activating mothballed motorway {1}", new object[]
				{
					motorway.Id,
					mothballedReplacement.Id
				});
				motorway.SetState(RoadState.None);
				motorway.ConcreteGivenToReplacement = motorway.ConcreteCost;
				mothballedReplacement.SetState(RoadState.Active);
				return Diagnostics.Verify(motorway.StartTile.Tile.SetNodeState(new RoadTileNode(motorway.StartDirection, RoadType.Motorway, motorway.Id), RoadState.None, Tile.TileChangePermissions.Full) & motorway.EndTile.Tile.SetNodeState(new RoadTileNode(motorway.EndDirection, RoadType.Motorway, motorway.Id), RoadState.None, Tile.TileChangePermissions.Full) & mothballedReplacement.StartTile.Tile.SetNodeState(new RoadTileNode(mothballedReplacement.StartDirection, RoadType.Motorway, mothballedReplacement.Id), RoadState.Active, Tile.TileChangePermissions.Full) & mothballedReplacement.EndTile.Tile.SetNodeState(new RoadTileNode(mothballedReplacement.EndDirection, RoadType.Motorway, mothballedReplacement.Id), RoadState.Active, Tile.TileChangePermissions.Full), "Failed to reactivate mothballed motorway and dispose of planned one!");
			}
			if (!motorway.CanSetMotorwayAndNodeState(RoadState.Active))
			{
				return false;
			}
			if (!motorway.hasConsumedUpgrade)
			{
				if (!this._upgradeDatabase.ConsumeUpgrade(UpgradeType.Motorway, 1))
				{
					BuildMotorwaysProcess.Log.Info("Unable to activate motorway {0}, no motorway asset is available.", new object[]
					{
						motorway.Id
					});
					return true;
				}
				BuildMotorwaysProcess.Log.Info("Consumed upgrade to activate motorway {0}.", new object[]
				{
					motorway.Id
				});
				motorway.hasConsumedUpgrade = true;
			}
			else
			{
				BuildMotorwaysProcess.Log.Info("Activating pre-paid motorway {0}.", new object[]
				{
					motorway.Id
				});
			}
			motorway.SetMotorwayAndNodeState(RoadState.Active);
			return true;
		}

		// Token: 0x040018CC RID: 6348
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("BuildMotorwaysProcess");

		// Token: 0x040018CD RID: 6349
		[Dependency]
		private UpgradeDatabaseModel _upgradeDatabase;
	}
}
