using System;

namespace Motorways.Views
{
	// Token: 0x020005C3 RID: 1475
	[Flags]
	public enum EditMenuButtonType
	{
		// Token: 0x040022FA RID: 8954
		Flip = 1,
		// Token: 0x040022FB RID: 8955
		Rotate = 2,
		// Token: 0x040022FC RID: 8956
		UpgradeDowngrade = 4,
		// Token: 0x040022FD RID: 8957
		Colour = 8,
		// Token: 0x040022FE RID: 8958
		Confirm = 16,
		// Token: 0x040022FF RID: 8959
		Decline = 32,
		// Token: 0x04002300 RID: 8960
		Delete = 64,
		// Token: 0x04002301 RID: 8961
		Move = 128
	}
}
