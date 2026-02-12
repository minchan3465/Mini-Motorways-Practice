using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x020006A2 RID: 1698
	public class London : MusicData
	{
		// Token: 0x06002EFA RID: 12026 RVA: 0x000DAC60 File Offset: 0x000D8E60
		public override void Injections()
		{
			base.SetKeyDeltas(Liszt.From<int>(new int[]
			{
				-1,
				1
			}), Rando.Pick<int>(new int[]
			{
				0,
				1,
				2,
				10,
				11
			}));
			Quality q = Quality.Clone(QualityDatabase.MAJOR, "");
			q.Scales.RemoveAt(6);
			q.Scales.RemoveAt(2);
			q.Chromatic("");
			base.SetQualities(new List<Quality>
			{
				q
			}, null, true);
			base.SetRhythms(Rhythm.AllPulses(-1), MusicData.RhythmUpdateType.RandomParallel);
		}

		// Token: 0x06002EFB RID: 12027 RVA: 0x000DACF1 File Offset: 0x000D8EF1
		public override void PostLoad()
		{
			base.PostLoad();
			Get.Loadout.Train.PatternLengthOverride = Rando.Pick<int>(new int[]
			{
				4,
				6,
				8,
				10,
				12
			});
		}

		// Token: 0x06002EFC RID: 12028 RVA: 0x000DAD1E File Offset: 0x000D8F1E
		public override void OnTrainArrived()
		{
			Get.Loadout.Train.PatternLengthOverride = Rando.Pick<int>(new int[]
			{
				4,
				6,
				8,
				10,
				12
			});
		}
	}
}
