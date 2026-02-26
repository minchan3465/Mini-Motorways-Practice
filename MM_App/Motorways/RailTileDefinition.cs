using System;
using Factory;
using Factory.Pools;

namespace Motorways
{
	// Token: 0x0200041B RID: 1051
	public class RailTileDefinition : IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x060019ED RID: 6637 RVA: 0x0005D369 File Offset: 0x0005B569
		public RailTileDefinition CreateRotatedDefinition(IScope scope, RoadTileRotation newRotation)
		{
			RailTileDefinition railTileDefinition = scope.Get<RailTileDefinition>();
			railTileDefinition.rotation = TileUtilities.AddRotation(newRotation, this.rotation);
			railTileDefinition.path = this.path.CreateRotatedPath(newRotation);
			return railTileDefinition;
		}

		// Token: 0x060019EE RID: 6638 RVA: 0x0005D395 File Offset: 0x0005B595
		public void Reset()
		{
			this.index = -1;
			this.path = null;
			this.rotation = RoadTileRotation.None;
		}

		// Token: 0x060019EF RID: 6639 RVA: 0x0005D3AC File Offset: 0x0005B5AC
		public void OnReleasedFromScope(IScope scope)
		{
			if (this.path != null)
			{
				scope.Release(this.path);
				this.path = null;
			}
		}

		// Token: 0x040015C0 RID: 5568
		public int index = -1;

		// Token: 0x040015C1 RID: 5569
		public RoadTilePath path;

		// Token: 0x040015C2 RID: 5570
		public RoadTileRotation rotation;
	}
}
