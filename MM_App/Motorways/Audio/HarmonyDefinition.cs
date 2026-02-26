using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x02000689 RID: 1673
	public class HarmonyDefinition
	{
		// Token: 0x170007EB RID: 2027
		// (get) Token: 0x06002E63 RID: 11875 RVA: 0x000D6E7B File Offset: 0x000D507B
		// (set) Token: 0x06002E64 RID: 11876 RVA: 0x000D6E83 File Offset: 0x000D5083
		public int WeekIndex { get; private set; }

		// Token: 0x06002E65 RID: 11877 RVA: 0x000D6E8C File Offset: 0x000D508C
		public MusicData CreateHarmony(AudioLoadout loadout)
		{
			return new MusicData();
		}

		// Token: 0x04002853 RID: 10323
		private AudioLoadout _parentLoadout;

		// Token: 0x04002855 RID: 10325
		private List<Attribute> _sequenceAttribute;

		// Token: 0x04002856 RID: 10326
		private Attribute _bassAttribute;
	}
}
