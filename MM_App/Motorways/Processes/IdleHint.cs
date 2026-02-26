using System;
using System.Collections.Generic;
using FixMath;

namespace Motorways.Processes
{
	// Token: 0x020004B9 RID: 1209
	public class IdleHint
	{
		// Token: 0x06001F89 RID: 8073 RVA: 0x0007B46F File Offset: 0x0007966F
		public IdleHint SetDelayBeforeShowing(float delay)
		{
			this.DelayBeforeShowing = (Fix64)delay;
			return this;
		}

		// Token: 0x06001F8A RID: 8074 RVA: 0x0007B480 File Offset: 0x00079680
		public IdleHint SetShowHintHandler(Action handler)
		{
			this.ShowHintHandler = delegate(Fix64 timestep)
			{
				handler();
			};
			return this;
		}

		// Token: 0x06001F8B RID: 8075 RVA: 0x0007B4AD File Offset: 0x000796AD
		public IdleHint SetShowHintHandler(Action<Fix64> handler)
		{
			this.ShowHintHandler = handler;
			return this;
		}

		// Token: 0x06001F8C RID: 8076 RVA: 0x0007B4B7 File Offset: 0x000796B7
		public IdleHint SetHideHintHandler(Action handler)
		{
			this.HideHintHandler = handler;
			return this;
		}

		// Token: 0x06001F8D RID: 8077 RVA: 0x0007B4C1 File Offset: 0x000796C1
		public IdleHint SetProgressionHandler(Action handler)
		{
			this.StepProgressedHandler = handler;
			return this;
		}

		// Token: 0x06001F8E RID: 8078 RVA: 0x0007B4CB File Offset: 0x000796CB
		public IdleHint AddCondition(Func<bool> condition)
		{
			this.ShowConditions.Add(condition);
			return this;
		}

		// Token: 0x04001A2B RID: 6699
		private const float DefaultDelayBeforeIdleHint = 1f;

		// Token: 0x04001A2C RID: 6700
		public Fix64 idleTime = Fix64.Zero;

		// Token: 0x04001A2D RID: 6701
		public Fix64 DelayBeforeShowing = (Fix64)1f;

		// Token: 0x04001A2E RID: 6702
		public Action<Fix64> ShowHintHandler;

		// Token: 0x04001A2F RID: 6703
		public Action HideHintHandler;

		// Token: 0x04001A30 RID: 6704
		public List<Func<bool>> ShowConditions = new List<Func<bool>>();

		// Token: 0x04001A31 RID: 6705
		public Action StepProgressedHandler;
	}
}
