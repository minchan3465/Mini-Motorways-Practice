using System;
using FixMath;
using UnityEngine.Serialization;

namespace Motorways
{
	// Token: 0x0200036F RID: 879
	[Serializable]
	public class GroupDemandOscillation
	{
		// Token: 0x0600157A RID: 5498 RVA: 0x00049C68 File Offset: 0x00047E68
		public Fix64 GetDemandAtDay(Fix64 timeInDays)
		{
			timeInDays %= (Fix64)((long)this.periodInDays);
			return this.baseDemand + Fix64.Pow(Fix64.Abs(Fix64.Sin(timeInDays * Fix64.Pi / (Fix64)((long)this.periodInDays))), (Fix64)((long)(2 + this.eccentricity))) * (this.burstDemand - this.baseDemand);
		}

		// Token: 0x040011FA RID: 4602
		public int index;

		// Token: 0x040011FB RID: 4603
		[FormerlySerializedAs("minimumDemand")]
		public Fix64 baseDemand = Fix64Consts.OneHalf;

		// Token: 0x040011FC RID: 4604
		[FormerlySerializedAs("maximumDemand")]
		public Fix64 burstDemand = Fix64Consts.Two;

		// Token: 0x040011FD RID: 4605
		public int eccentricity;

		// Token: 0x040011FE RID: 4606
		public int periodInDays = 7;
	}
}
