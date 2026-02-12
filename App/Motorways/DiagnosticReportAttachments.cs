using System;

namespace Motorways
{
	// Token: 0x020003B5 RID: 949
	[Flags]
	public enum DiagnosticReportAttachments
	{
		// Token: 0x04001323 RID: 4899
		AppCommandJournal = 1,
		// Token: 0x04001324 RID: 4900
		SimCommandJournal = 2,
		// Token: 0x04001325 RID: 4901
		SimArchive = 4,
		// Token: 0x04001326 RID: 4902
		Screenshot = 8,
		// Token: 0x04001327 RID: 4903
		Log = 16
	}
}
