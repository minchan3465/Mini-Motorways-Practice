using System;
using Factory;
using FixMath;

namespace Server
{
	// Token: 0x02000283 RID: 643
	[Factory.Serializable(1)]
	public class Clock
	{
		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06000FDA RID: 4058 RVA: 0x00035B08 File Offset: 0x00033D08
		// (set) Token: 0x06000FDB RID: 4059 RVA: 0x00035B10 File Offset: 0x00033D10
		[Serialize(true, null)]
		public int FrameCount { get; private set; }

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06000FDC RID: 4060 RVA: 0x00035B19 File Offset: 0x00033D19
		// (set) Token: 0x06000FDD RID: 4061 RVA: 0x00035B21 File Offset: 0x00033D21
		public int ModelFrameIndex { get; private set; }

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06000FDE RID: 4062 RVA: 0x00035B2A File Offset: 0x00033D2A
		// (set) Token: 0x06000FDF RID: 4063 RVA: 0x00035B32 File Offset: 0x00033D32
		[Serialize(true, null)]
		public Fix64 Time { get; private set; }

		// Token: 0x06000FE0 RID: 4064 RVA: 0x00035B3B File Offset: 0x00033D3B
		public void Step(Fix64 deltaTime)
		{
			this.FrameCount++;
			this.ModelFrameIndex = 1 - this.ModelFrameIndex;
			this.Time += deltaTime;
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x00035B6B File Offset: 0x00033D6B
		public void Rewind()
		{
			this.FrameCount--;
			this.ModelFrameIndex = 1 - this.ModelFrameIndex;
		}
	}
}
