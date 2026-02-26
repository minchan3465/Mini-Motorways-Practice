using System;
using Factory;
using Motorways.Models;
using Server;

namespace Motorways.Commands
{
	// Token: 0x02000521 RID: 1313
	public class RemoveHouseCommand : Command
	{
		// Token: 0x060022BB RID: 8891 RVA: 0x0008C92B File Offset: 0x0008AB2B
		public override void Execute(ISimulation simulation)
		{
			this._model.Remove();
		}

		// Token: 0x060022BC RID: 8892 RVA: 0x0008C938 File Offset: 0x0008AB38
		public override void Reset()
		{
			base.Reset();
			this._model = null;
		}

		// Token: 0x060022BD RID: 8893 RVA: 0x0008C947 File Offset: 0x0008AB47
		public static RemoveHouseCommand Create(IScope scope, HouseModel model)
		{
			RemoveHouseCommand removeHouseCommand = scope.Get<RemoveHouseCommand>();
			removeHouseCommand._model = model;
			return removeHouseCommand;
		}

		// Token: 0x04001CCE RID: 7374
		private HouseModel _model;
	}
}
