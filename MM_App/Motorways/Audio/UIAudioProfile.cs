using System;

namespace Motorways.Audio
{
	// Token: 0x0200063B RID: 1595
	[Flags]
	public enum UIAudioProfile
	{
		// Token: 0x040026F3 RID: 9971
		None = 0,
		// Token: 0x040026F4 RID: 9972
		Generic = 1,
		// Token: 0x040026F5 RID: 9973
		Back = 2,
		// Token: 0x040026F6 RID: 9974
		Clock = 4,
		// Token: 0x040026F7 RID: 9975
		Pause = 8,
		// Token: 0x040026F8 RID: 9976
		Play = 16,
		// Token: 0x040026F9 RID: 9977
		Map = 32,
		// Token: 0x040026FA RID: 9978
		Upgrade = 64,
		// Token: 0x040026FB RID: 9979
		ResumeDelete = 128,
		// Token: 0x040026FC RID: 9980
		Theme = 256,
		// Token: 0x040026FD RID: 9981
		NoHover = 512,
		// Token: 0x040026FE RID: 9982
		Picture = 2048,
		// Token: 0x040026FF RID: 9983
		Checkbox = 4096,
		// Token: 0x04002700 RID: 9984
		ArrowLeft = 8192,
		// Token: 0x04002701 RID: 9985
		ArrowRight = 16384,
		// Token: 0x04002702 RID: 9986
		Button = 32768,
		// Token: 0x04002703 RID: 9987
		FastForward = 65536,
		// Token: 0x04002704 RID: 9988
		StartGame = 131072,
		// Token: 0x04002705 RID: 9989
		DrawModeToggle = 262144,
		// Token: 0x04002706 RID: 9990
		Lock = 524288,
		// Token: 0x04002707 RID: 9991
		ElectiveUpgrade = 1048576,
		// Token: 0x04002708 RID: 9992
		CreativeModePaint = 2097152,
		// Token: 0x04002709 RID: 9993
		CreativeModeTrash = 4194304,
		// Token: 0x0400270A RID: 9994
		CreativeModePaintWheel = 8388608
	}
}
