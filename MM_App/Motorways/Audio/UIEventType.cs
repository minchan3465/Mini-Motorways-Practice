using System;

namespace Motorways.Audio
{
	// Token: 0x0200063A RID: 1594
	[Flags]
	public enum UIEventType
	{
		// Token: 0x040026E7 RID: 9959
		None = 0,
		// Token: 0x040026E8 RID: 9960
		MouseOver = 1,
		// Token: 0x040026E9 RID: 9961
		MouseOut = 2,
		// Token: 0x040026EA RID: 9962
		MouseDown = 4,
		// Token: 0x040026EB RID: 9963
		MouseUp = 8,
		// Token: 0x040026EC RID: 9964
		Click = 16,
		// Token: 0x040026ED RID: 9965
		CheckboxChecked = 32,
		// Token: 0x040026EE RID: 9966
		CheckboxUnchecked = 64,
		// Token: 0x040026EF RID: 9967
		Transition = 128,
		// Token: 0x040026F0 RID: 9968
		FocusZoomIn = 256,
		// Token: 0x040026F1 RID: 9969
		FocusZoomOut = 512
	}
}
