using System;
using Factory;
using Factory.Pools;

namespace Motorways
{
	// Token: 0x020003E7 RID: 999
	public class BoatPathTileDefinition : IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x06001834 RID: 6196 RVA: 0x000566F9 File Offset: 0x000548F9
		public BoatPathTileDefinition CreateRotatedDefinition(IScope scope, RoadTileRotation newRotation)
		{
			BoatPathTileDefinition boatPathTileDefinition = scope.Get<BoatPathTileDefinition>();
			boatPathTileDefinition.rotation = TileUtilities.AddRotation(newRotation, this.rotation);
			boatPathTileDefinition.path = this.path.CreateRotatedPath(newRotation);
			return boatPathTileDefinition;
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x00056725 File Offset: 0x00054925
		public void Reset()
		{
			this.index = -1;
			this.path = null;
			this.rotation = RoadTileRotation.None;
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x0005673C File Offset: 0x0005493C
		public void OnReleasedFromScope(IScope scope)
		{
			if (this.path != null)
			{
				scope.Release(this.path);
				this.path = null;
			}
		}

		// Token: 0x040014B2 RID: 5298
		public int index = -1;

		// Token: 0x040014B3 RID: 5299
		public RoadTilePath path;

		// Token: 0x040014B4 RID: 5300
		public RoadTileRotation rotation;
	}
}
