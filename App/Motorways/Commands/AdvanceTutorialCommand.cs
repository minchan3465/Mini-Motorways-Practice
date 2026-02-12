using System;
using Factory;
using Motorways.Processes;
using Server;

namespace Motorways.Commands
{
	// Token: 0x0200051B RID: 1307
	public class AdvanceTutorialCommand : Command
	{
		// Token: 0x0600229C RID: 8860 RVA: 0x0008C144 File Offset: 0x0008A344
		public override void Execute(ISimulation simulation)
		{
			this._tutorialProcess.hadInput = true;
		}

		// Token: 0x0600229D RID: 8861 RVA: 0x0008C152 File Offset: 0x0008A352
		public static AdvanceTutorialCommand Create(IScope scope)
		{
			return scope.Get<AdvanceTutorialCommand>();
		}

		// Token: 0x04001CB8 RID: 7352
		[Dependency]
		private TutorialProgressionProcess _tutorialProcess;
	}
}
