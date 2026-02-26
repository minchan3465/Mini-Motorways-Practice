using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x0200069C RID: 1692
	public class Copenhagen : MusicData
	{
		// Token: 0x06002EDE RID: 11998 RVA: 0x000DA064 File Offset: 0x000D8264
		public override void Injections()
		{
			this.rhythms = Liszt.From<Rhythm>(new Rhythm[]
			{
				new Rhythm(0f, new float[]
				{
					0.5f,
					0.25f,
					0.25f,
					0.25f,
					0.25f
				}),
				new Rhythm(0f, new float[]
				{
					0.5f,
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
					0.75f,
					0.25f,
					0.5f,
					0.5f
				}),
				new Rhythm(0f, new float[]
				{
					0.5f,
					0.5f,
					1f
				}),
				new Rhythm(0f, new float[]
				{
					0.75f,
					0.75f,
					0.5f
				}),
				new Rhythm(0f, new float[]
				{
					0.5f,
					0.25f,
					0.25f,
					0.5f,
					0.5f
				})
			});
			for (int i = 0; i < 7; i++)
			{
				this.rhythms.Add(this.rhythms[i].Scale(2f, true));
				this.rhythms.Add(this.rhythms[i].Scale(3f, true));
			}
			base.SetRhythms(this.rhythms, MusicData.RhythmUpdateType.RandomParallel);
			List<int> baseStack = Liszt.From<int>(new int[]
			{
				12,
				19,
				24
			});
			List<Quality> qualities = Liszt.From<Quality>(new Quality[]
			{
				new Quality("Ionian", Liszt.From<int>(new int[]
				{
					2,
					2,
					1,
					2,
					2,
					2,
					1
				}), baseStack),
				new Quality("Mixolydian b7", Liszt.From<int>(new int[]
				{
					2,
					2,
					2,
					1,
					2,
					1,
					2
				}), baseStack),
				new Quality("Dorian", Liszt.From<int>(new int[]
				{
					2,
					1,
					2,
					2,
					2,
					1,
					2
				}), baseStack),
				new Quality("Aeolian", Liszt.From<int>(new int[]
				{
					2,
					1,
					2,
					2,
					1,
					2,
					2
				}), baseStack),
				new Quality("Mixolydian", Liszt.From<int>(new int[]
				{
					2,
					2,
					1,
					2,
					2,
					1,
					2
				}), baseStack),
				new Quality("Traditional Minor", Liszt.From<int>(new int[]
				{
					2,
					1,
					2,
					2,
					2,
					2,
					1
				}), baseStack)
			});
			base.SetQualities(qualities, null, true);
			base.SetDrumSequencer(this.quarterNote, false, false, false, true, 0f, 0f);
		}

		// Token: 0x06002EDF RID: 11999 RVA: 0x000DA2D4 File Offset: 0x000D84D4
		public override void PostLoad()
		{
			base.PostLoad();
			base.UpdateTrain(Rando.Pick<int>(new int[]
			{
				3,
				4
			}), 0.5f, 0, new string[]
			{
				"STAR",
				"CROSS",
				"TRIANGLE"
			});
		}

		// Token: 0x06002EE0 RID: 12000 RVA: 0x000DA324 File Offset: 0x000D8524
		public override void OnHour()
		{
			this.DrumVolume = 1f - Maf.Map(Get.Loadout.Train.SpeedAlpha, 0.1f, 1f, 0f, 1f) * Maf.Map(Get.Loadout.Train.Attenuation, 0f, 0.25f, 0f, 1f);
		}

		// Token: 0x06002EE1 RID: 12001 RVA: 0x000DA38E File Offset: 0x000D858E
		public override void OnTrainArrived()
		{
			base.UpdateTrain(Rando.Pick<int>(new int[]
			{
				3,
				4
			}), -1f, -1, Array.Empty<string>());
		}

		// Token: 0x06002EE2 RID: 12002 RVA: 0x000DA3B4 File Offset: 0x000D85B4
		public override void OnNewWeek()
		{
			base.UpdateTrain(-1, -1f, -1, Array.Empty<string>());
			if (Get.Week > 0)
			{
				base.UpdateDrumSequencer(this.quarterNote, Rando.FlipCoin(0.5f), Rando.FlipCoin(0.5f), Rando.FlipCoin(0.5f), false);
			}
		}

		// Token: 0x04002894 RID: 10388
		private List<Rhythm> rhythms;

		// Token: 0x04002895 RID: 10389
		private Rhythm quarterNote = new Rhythm(0f, new float[]
		{
			0.25f
		});
	}
}
