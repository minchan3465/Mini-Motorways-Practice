using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using Motorways.Models;
using Server;

namespace Motorways
{
	// Token: 0x0200040C RID: 1036
	[Factory.Serializable(1)]
	public abstract class TileEdit : IReusable
	{
		// Token: 0x06001959 RID: 6489 RVA: 0x0005A7EF File Offset: 0x000589EF
		public virtual void Reset()
		{
			this.CanApplyToSimulation = true;
		}

		// Token: 0x0600195A RID: 6490 RVA: 0x0005A7F8 File Offset: 0x000589F8
		public virtual IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			yield break;
		}

		// Token: 0x0600195B RID: 6491 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool ApplyToAffectedTile(Tile tile)
		{
			return false;
		}

		// Token: 0x0600195C RID: 6492 RVA: 0x0005A801 File Offset: 0x00058A01
		public virtual IEnumerable<Motorway> GetAffectedMotorways(ITilemap tilemap)
		{
			yield break;
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool ApplyToAffectedMotorway(Motorway motorway)
		{
			return false;
		}

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x0600195E RID: 6494 RVA: 0x0005A80A File Offset: 0x00058A0A
		// (set) Token: 0x0600195F RID: 6495 RVA: 0x0005A812 File Offset: 0x00058A12
		public bool CanApplyToSimulation { get; set; } = true;

		// Token: 0x06001960 RID: 6496 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void ApplyToSimulation(ISimulation simulation)
		{
		}

		// Token: 0x06001961 RID: 6497 RVA: 0x0005A81C File Offset: 0x00058A1C
		public bool ApplyToTilemap(ITilemap tilemap)
		{
			TileEdit.Log.Info("Applying {0} to tilemap.", new object[]
			{
				this
			});
			bool success = true;
			foreach (Motorway affectedMotorway in this.GetAffectedMotorways(tilemap))
			{
				TileEdit.Log.Info("Applying to motorway {0}.", new object[]
				{
					affectedMotorway
				});
				success = (this.ApplyToAffectedMotorway(affectedMotorway) && success);
			}
			foreach (Tile affectedTile in this.GetAffectedTiles(tilemap))
			{
				TileEdit.Log.Info("Applying to tile {0}.", new object[]
				{
					affectedTile
				});
				success = (this.ApplyToAffectedTile(affectedTile) && success);
			}
			return success;
		}

		// Token: 0x06001962 RID: 6498
		public abstract bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap);

		// Token: 0x0400156D RID: 5485
		protected static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("TileEdit");

		// Token: 0x0400156E RID: 5486
		[Dependency]
		protected GameBehaviourModel _behaviour;
	}
}
