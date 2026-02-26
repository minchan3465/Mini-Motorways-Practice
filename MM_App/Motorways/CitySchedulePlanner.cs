using System;
using System.Collections.Generic;
using FixMath;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200036A RID: 874
	public class CitySchedulePlanner : MonoBehaviour
	{
		// Token: 0x06001571 RID: 5489 RVA: 0x00049A98 File Offset: 0x00047C98
		public ScheduleGroup GetScheduleGroup(int groupIndex)
		{
			return this.scheduleGroups.Find((ScheduleGroup group) => group.index == groupIndex);
		}

		// Token: 0x06001572 RID: 5490 RVA: 0x00049ACC File Offset: 0x00047CCC
		public Fix64 GetOscillationForDemand(int groupIndex, Fix64 timeInDays)
		{
			if (!Diagnostics.Verify(this.demandOscillationData != null && this.demandOscillationData.Count > 0, "We have no demand oscillation for this city {0}! Defaulting to none.", base.name))
			{
				return Fix64.One;
			}
			GroupDemandOscillation demandOscillation = null;
			foreach (GroupDemandOscillation entry in this.demandOscillationData)
			{
				if (entry.index == groupIndex)
				{
					demandOscillation = entry;
				}
			}
			if (Diagnostics.Verify(demandOscillation != null, "Missing demand oscillation for group {0}. Defaulting to none", groupIndex))
			{
				return demandOscillation.GetDemandAtDay(timeInDays);
			}
			return Fix64.One;
		}

		// Token: 0x06001573 RID: 5491 RVA: 0x00049B7C File Offset: 0x00047D7C
		public int GetDemandOscillationPeriod(int groupIndex)
		{
			if (!Diagnostics.Verify(this.demandOscillationData != null && this.demandOscillationData.Count > 0, "We have no demand oscillation for this city {0}! Defaulting to no oscillation", base.name))
			{
				return 1;
			}
			GroupDemandOscillation demandOscillation = this.demandOscillationData.Find((GroupDemandOscillation group) => group.index == groupIndex);
			if (Diagnostics.Verify(demandOscillation != null, "We have no demand oscillation for group: {0}! Defaulting to none", groupIndex))
			{
				return demandOscillation.periodInDays;
			}
			return 1;
		}

		// Token: 0x040011EC RID: 4588
		public const int DaysInSchedule = 70;

		// Token: 0x040011ED RID: 4589
		public List<ScheduleGroup> scheduleGroups = new List<ScheduleGroup>();

		// Token: 0x040011EE RID: 4590
		public List<GroupDemandOscillation> demandOscillationData = new List<GroupDemandOscillation>();

		// Token: 0x040011EF RID: 4591
		public List<ScheduleChunk> scheduleChunks = new List<ScheduleChunk>
		{
			new ScheduleChunk()
		};
	}
}
