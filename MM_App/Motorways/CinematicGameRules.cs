using System;

namespace Motorways
{
	// Token: 0x020003D0 RID: 976
	public class CinematicGameRules : BackgroundGameRules
	{
		// Token: 0x06001725 RID: 5925 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool UseCamera()
		{
			return true;
		}

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06001726 RID: 5926 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool CanDestinationsOvercrowd
		{
			get
			{
				return false;
			}
		}
	}
}
