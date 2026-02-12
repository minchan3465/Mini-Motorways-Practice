using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x020006B3 RID: 1715
	public class NewYorkCity : MusicData
	{
		// Token: 0x06002F68 RID: 12136 RVA: 0x000DCD38 File Offset: 0x000DAF38
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
				3
			}));
			List<Quality> qualities = QualityDatabase.Gather(new string[]
			{
				"Blues",
				"7#9",
				"11",
				"13"
			});
			qualities.Add(new Quality("Mixolydian", Liszt.From<int>(new int[]
			{
				2,
				2,
				1,
				2,
				2,
				1,
				2
			}), null).Chromatic(""));
			base.SetQualities(qualities, null, true);
			List<Rhythm> rhythms = Liszt.From<Rhythm>(new Rhythm[]
			{
				new Rhythm(0f, new float[]
				{
					0.333f,
					0.333f,
					0.334f
				}),
				new Rhythm(0f, new float[]
				{
					0.666f,
					0.334f
				}),
				new Rhythm(0f, new float[]
				{
					1f
				}),
				new Rhythm(0f, new float[]
				{
					1.666f,
					0.334f
				}),
				new Rhythm(0f, new float[]
				{
					2f
				})
			});
			base.SetRhythms(rhythms, MusicData.RhythmUpdateType.RandomParallel);
		}

		// Token: 0x06002F69 RID: 12137 RVA: 0x000DCE88 File Offset: 0x000DB088
		public override void PostLoad()
		{
			base.PostLoad();
			Get.Loadout.Train.PatternLengthOverride = 4;
		}

		// Token: 0x06002F6A RID: 12138 RVA: 0x000DCEA0 File Offset: 0x000DB0A0
		public override void OnTrainArrived()
		{
			Get.Loadout.Train.PatternLengthOverride = 4;
		}
	}
}
