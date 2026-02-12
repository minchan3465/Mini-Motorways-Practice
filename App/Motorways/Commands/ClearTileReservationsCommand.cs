using System;
using Factory;
using Motorways.Models;
using Server;

namespace Motorways.Commands
{
	// Token: 0x0200051C RID: 1308
	public class ClearTileReservationsCommand : Command
	{
		// Token: 0x0600229F RID: 8863 RVA: 0x0008C15A File Offset: 0x0008A35A
		public override void Execute(ISimulation simulation)
		{
			this._tilemap.ClearTileReservations();
		}

		// Token: 0x060022A0 RID: 8864 RVA: 0x0008C167 File Offset: 0x0008A367
		public static ClearTileReservationsCommand Create(IScope scope)
		{
			return scope.Get<ClearTileReservationsCommand>();
		}

		// Token: 0x04001CB9 RID: 7353
		[Dependency]
		private TilemapModel _tilemap;
	}
}
