using System;
using Factory;
using Motorways.Models;
using Server;

namespace Motorways.Commands
{
	// Token: 0x0200051F RID: 1311
	public class RemoveCarparkCommand : Command
	{
		// Token: 0x060022B3 RID: 8883 RVA: 0x0008C8D5 File Offset: 0x0008AAD5
		public override void Execute(ISimulation simulation)
		{
			this._model.Remove();
		}

		// Token: 0x060022B4 RID: 8884 RVA: 0x0008C8E2 File Offset: 0x0008AAE2
		public override void Reset()
		{
			base.Reset();
			this._model = null;
		}

		// Token: 0x060022B5 RID: 8885 RVA: 0x0008C8F1 File Offset: 0x0008AAF1
		public static RemoveCarparkCommand Create(IScope scope, CarparkModel model)
		{
			RemoveCarparkCommand removeCarparkCommand = scope.Get<RemoveCarparkCommand>();
			removeCarparkCommand._model = model;
			return removeCarparkCommand;
		}

		// Token: 0x04001CCC RID: 7372
		private CarparkModel _model;
	}
}
