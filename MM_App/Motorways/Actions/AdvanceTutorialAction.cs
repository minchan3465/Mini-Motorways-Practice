using System;
using Factory;
using Motorways.Commands;
using Server;

namespace Motorways.Actions
{
	// Token: 0x020006EF RID: 1775
	public class AdvanceTutorialAction : MotorwaysPlayerAction
	{
		// Token: 0x0600309B RID: 12443 RVA: 0x000E47BD File Offset: 0x000E29BD
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this.targetSimulation = base.Scope.Get<ISimulation>();
			this.targetSimulation.ScheduleCommand(AdvanceTutorialCommand.Create(base.Scope));
		}

		// Token: 0x0600309C RID: 12444 RVA: 0x000020A2 File Offset: 0x000002A2
		public override void Tick(float frameTime)
		{
			this.OnActionComplete();
		}

		// Token: 0x0600309D RID: 12445 RVA: 0x000E47EE File Offset: 0x000E29EE
		public static AdvanceTutorialAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			AdvanceTutorialAction advanceTutorialAction = scope.Get<AdvanceTutorialAction>();
			advanceTutorialAction.InitializeAction(owningGroup, timestamp);
			advanceTutorialAction.OnActionBegin(timestamp);
			return advanceTutorialAction;
		}

		// Token: 0x040029DB RID: 10715
		public ISimulation targetSimulation;
	}
}
