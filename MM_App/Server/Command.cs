using System;
using Factory;
using Factory.Pools;

namespace Server
{
	// Token: 0x02000284 RID: 644
	[Factory.Serializable(1)]
	public abstract class Command : IReusable
	{
		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06000FE3 RID: 4067 RVA: 0x00035B89 File Offset: 0x00033D89
		// (set) Token: 0x06000FE4 RID: 4068 RVA: 0x00035B91 File Offset: 0x00033D91
		public int FrameIndex
		{
			get
			{
				return this._frameIndex;
			}
			set
			{
				this._frameIndex = value;
			}
		}

		// Token: 0x06000FE5 RID: 4069
		public abstract void Execute(ISimulation simulation);

		// Token: 0x06000FE6 RID: 4070 RVA: 0x00035B9A File Offset: 0x00033D9A
		public virtual void Reset()
		{
			this._frameIndex = -1;
		}

		// Token: 0x04000E36 RID: 3638
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Server.Command");

		// Token: 0x04000E37 RID: 3639
		private int _frameIndex = -1;
	}
}
