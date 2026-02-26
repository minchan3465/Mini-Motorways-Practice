using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000374 RID: 884
	[Serializable]
	public class CityStartOffsets
	{
		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06001580 RID: 5504 RVA: 0x00049F0D File Offset: 0x0004810D
		public int Count
		{
			get
			{
				if (this.offsets != null)
				{
					return this.offsets.Count;
				}
				return 0;
			}
		}

		// Token: 0x0400121C RID: 4636
		[Tooltip("The positions that the game can begin in")]
		public List<CityStartOffsetDefinition> offsets;
	}
}
