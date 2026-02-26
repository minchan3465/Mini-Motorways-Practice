using System;

namespace Motorways.Audio
{
	// Token: 0x02000644 RID: 1604
	[Flags]
	public enum StateType
	{
		// Token: 0x0400273B RID: 10043
		None = 0,
		// Token: 0x0400273C RID: 10044
		MenuMain = 1,
		// Token: 0x0400273D RID: 10045
		MenuOptions = 2,
		// Token: 0x0400273E RID: 10046
		MenuMapSelect = 4,
		// Token: 0x0400273F RID: 10047
		MenuMap = 8,
		// Token: 0x04002740 RID: 10048
		GameActive = 16,
		// Token: 0x04002741 RID: 10049
		GamePaused = 32,
		// Token: 0x04002742 RID: 10050
		ModeEdit = 64,
		// Token: 0x04002743 RID: 10051
		ModeDelete = 128,
		// Token: 0x04002744 RID: 10052
		ModeNight = 256,
		// Token: 0x04002745 RID: 10053
		MenuPause = 512,
		// Token: 0x04002746 RID: 10054
		MenuUpgrades = 1024,
		// Token: 0x04002747 RID: 10055
		GameOver = 2048,
		// Token: 0x04002748 RID: 10056
		Credits = 4096,
		// Token: 0x04002749 RID: 10057
		MenuLanguage = 16384,
		// Token: 0x0400274A RID: 10058
		MenuResume = 32768,
		// Token: 0x0400274B RID: 10059
		LateGame = 65536,
		// Token: 0x0400274C RID: 10060
		SkippingMenu = 131072,
		// Token: 0x0400274D RID: 10061
		MenuPhoto = 262144,
		// Token: 0x0400274E RID: 10062
		Minimal = 524288
	}
}
