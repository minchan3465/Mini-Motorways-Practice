using System;
using System.Collections.Generic;
using System.Linq;

namespace Motorways.Audio
{
	// Token: 0x02000653 RID: 1619
	public static class QualityExtensions
	{
		// Token: 0x06002D29 RID: 11561 RVA: 0x000D0A0C File Offset: 0x000CEC0C
		public static List<Quality> Chromatic(this List<Quality> list, string addendName = "")
		{
			List<Quality> list2 = list.ToList<Quality>();
			list2.Edit((Quality x) => x.Chromatic(addendName));
			return list2;
		}

		// Token: 0x06002D2A RID: 11562 RVA: 0x000D0A40 File Offset: 0x000CEC40
		public static List<Quality> Transpose(this List<Quality> list, int delta)
		{
			List<Quality> list2 = list.ToList<Quality>();
			list2.Edit((Quality x) => x.Transpose(delta));
			return list2;
		}

		// Token: 0x06002D2B RID: 11563 RVA: 0x000D0A74 File Offset: 0x000CEC74
		public static List<Quality> Modal(this List<Quality> list, string addendName = "")
		{
			List<Quality> list2 = list.ToList<Quality>();
			list2.Edit(delegate(Quality x)
			{
				x.Name += ((addendName.Length > 0) ? (" " + addendName) : "");
				return x.Modal(Array.Empty<string>());
			});
			return list2;
		}

		// Token: 0x06002D2C RID: 11564 RVA: 0x000D0AA7 File Offset: 0x000CECA7
		public static List<Quality> Chromodal(this List<Quality> list)
		{
			return list.Modal("").Chromatic("");
		}

		// Token: 0x06002D2D RID: 11565 RVA: 0x000D0ABE File Offset: 0x000CECBE
		public static List<Quality> Keyless(this List<Quality> list)
		{
			List<Quality> list2 = list.ToList<Quality>();
			list2.Edit((Quality q) => q.Keyless());
			return list2;
		}
	}
}
