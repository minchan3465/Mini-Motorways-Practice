using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x020006B4 RID: 1716
	public class Reykjavik : MusicData
	{
		// Token: 0x06002F6C RID: 12140 RVA: 0x000DCEB4 File Offset: 0x000DB0B4
		public override void Injections()
		{
			List<Quality> qualities = new List<Quality>
			{
				new Quality("Aeolian", new List<int>
				{
					2,
					1,
					2,
					2,
					1,
					2,
					2
				}, new List<int>
				{
					0,
					12,
					24
				}),
				new Quality("Ionian", new List<int>
				{
					2,
					2,
					1,
					2,
					2,
					2,
					1
				}, new List<int>
				{
					0,
					12,
					24
				})
			};
			qualities = qualities.Chromatic(" (Chromatic)");
			base.SetQualities(qualities, null, true);
			base.SetRhythms(Reykjavik.Patterns, MusicData.RhythmUpdateType.RandomParallel);
			base.SetDrumSequencer(this.Rhythms.Pick(-1), true, false, false, true, -1f, -1f);
			base.SetKeyDeltas(Liszt.From<int>(new int[]
			{
				-2,
				2
			}), Rando.Pick<int>(new int[]
			{
				0,
				2,
				3,
				5,
				7,
				9,
				11
			}));
		}

		// Token: 0x06002F6D RID: 12141 RVA: 0x000DCFFC File Offset: 0x000DB1FC
		public override void OnNewWeek()
		{
			base.OnNewWeek();
			base.UpdateDrumSequencer(this.Rhythms.Pick(-1), true, false, false, false);
		}

		// Token: 0x040028FC RID: 10492
		public static List<Rhythm> Patterns = Liszt.From<Rhythm>(new Rhythm[]
		{
			new Rhythm(0f, new float[]
			{
				0.75f,
				0.25f
			}),
			new Rhythm(0f, new float[]
			{
				0.5f,
				0.25f,
				0.25f
			}),
			new Rhythm(0f, new float[]
			{
				0.375f,
				0.375f,
				0.25f
			}),
			new Rhythm(0f, new float[]
			{
				0.75f,
				0.75f,
				0.5f
			})
		});
	}
}
