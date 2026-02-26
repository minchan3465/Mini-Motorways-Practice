using System;
using Factory;
using Motorways.Models;
using Server;

namespace Motorways.Commands
{
	// Token: 0x02000520 RID: 1312
	public class RemoveDestinationCommand : Command
	{
		// Token: 0x060022B7 RID: 8887 RVA: 0x0008C900 File Offset: 0x0008AB00
		public override void Execute(ISimulation simulation)
		{
			this._model.Remove();
		}

		// Token: 0x060022B8 RID: 8888 RVA: 0x0008C90D File Offset: 0x0008AB0D
		public override void Reset()
		{
			base.Reset();
			this._model = null;
		}

		// Token: 0x060022B9 RID: 8889 RVA: 0x0008C91C File Offset: 0x0008AB1C
		public static RemoveDestinationCommand Create(IScope scope, DestinationModel model)
		{
			RemoveDestinationCommand removeDestinationCommand = scope.Get<RemoveDestinationCommand>();
			removeDestinationCommand._model = model;
			return removeDestinationCommand;
		}

		// Token: 0x04001CCD RID: 7373
		private DestinationModel _model;
	}
}
