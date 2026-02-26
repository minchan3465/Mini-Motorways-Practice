using System;
using Client;

namespace Motorways.Views
{
	// Token: 0x02000610 RID: 1552
	public class UnbuiltMotorwayHandleView : BaseMotorwayHandleView
	{
		// Token: 0x06002B68 RID: 11112 RVA: 0x000BF9DE File Offset: 0x000BDBDE
		public override TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			base.Tick(tickTime, stepAlpha);
			return TickResult.StopTicking;
		}
	}
}
