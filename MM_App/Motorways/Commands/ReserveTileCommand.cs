using System;
using Factory;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Commands
{
	// Token: 0x02000522 RID: 1314
	public class ReserveTileCommand : Command
	{
		// Token: 0x060022BF RID: 8895 RVA: 0x0008C956 File Offset: 0x0008AB56
		public override void Execute(ISimulation simulation)
		{
			this._tilemap.ReserveTile(this._coordinates);
		}

		// Token: 0x060022C0 RID: 8896 RVA: 0x0008C969 File Offset: 0x0008AB69
		public override void Reset()
		{
			base.Reset();
			this._coordinates = default(Vector2Int);
		}

		// Token: 0x060022C1 RID: 8897 RVA: 0x0008C97D File Offset: 0x0008AB7D
		public static ReserveTileCommand Create(IScope scope, Vector2Int coordinates)
		{
			ReserveTileCommand reserveTileCommand = scope.Get<ReserveTileCommand>();
			reserveTileCommand._coordinates = coordinates;
			return reserveTileCommand;
		}

		// Token: 0x04001CCF RID: 7375
		[Dependency]
		private TilemapModel _tilemap;

		// Token: 0x04001CD0 RID: 7376
		private Vector2Int _coordinates;
	}
}
