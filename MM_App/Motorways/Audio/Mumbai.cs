using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x020006AB RID: 1707
	public class Mumbai : MusicData
	{
		// Token: 0x06002F1F RID: 12063 RVA: 0x000DB820 File Offset: 0x000D9A20
		public override void Injections()
		{
			base.SetKeyDeltas(Liszt.From<int>(new int[]
			{
				-1,
				1
			}), Rando.Pick<int>(new int[]
			{
				11,
				0,
				7
			}));
			base.SetFadeInTimes(2.549999952316284, 2.200000047683716);
			this.GlobalFadeOut = (this.LocalFadeOut = 1.0);
			Quality lb7 = new Quality("Lydian b7", Liszt.From<int>(new int[]
			{
				2,
				2,
				2,
				1,
				2,
				1,
				2
			}), null);
			Quality mix = new Quality("Mixolydian", Liszt.From<int>(new int[]
			{
				2,
				2,
				1,
				2,
				2,
				1,
				2
			}), null);
			Quality lyd = new Quality("Lydian", Liszt.From<int>(new int[]
			{
				2,
				2,
				2,
				1,
				2,
				2,
				1
			}), null);
			base.SetQualities(Liszt.From<Quality>(new Quality[]
			{
				lb7.Chromatic(""),
				mix.Chromatic(""),
				lyd.Chromatic("")
			}), null, true);
			List<Rhythm> rhythms = Liszt.From<Rhythm>(new Rhythm[]
			{
				new Rhythm(0f, new float[]
				{
					0.75f
				}),
				new Rhythm(0f, new float[]
				{
					1f
				}),
				new Rhythm(0f, new float[]
				{
					1.5f
				}),
				new Rhythm(0f, new float[]
				{
					2f
				})
			});
			base.SetRhythms(rhythms, MusicData.RhythmUpdateType.RandomParallel);
		}

		// Token: 0x06002F20 RID: 12064 RVA: 0x000DB9AF File Offset: 0x000D9BAF
		public override void PostLoad()
		{
			base.PostLoad();
			Get.Loadout.Train.PatternLengthOverride = Rando.Pick<int>(new int[]
			{
				5,
				7,
				9,
				11,
				13
			});
		}

		// Token: 0x06002F21 RID: 12065 RVA: 0x000DB9DC File Offset: 0x000D9BDC
		public override void OnTrainArrived()
		{
			Get.Loadout.Train.PatternLengthOverride = Rando.Pick<int>(new int[]
			{
				5,
				7,
				9,
				11,
				13
			});
		}
	}
}
