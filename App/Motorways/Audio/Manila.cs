using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x020006A5 RID: 1701
	public class Manila : MusicData
	{
		// Token: 0x06002F04 RID: 12036 RVA: 0x000DAF10 File Offset: 0x000D9110
		public override void Injections()
		{
			base.SetQualities(Liszt.From<Quality>(new Quality[]
			{
				QualityDatabase.Find("Penta Chromodal")
			}), null, true);
			base.SetDrumSequencer(Rando.Pick<Rhythm>(Rhythm.Claves), false, true, true, true, -1f, -1f);
			base.SetKeyDeltas(Rando.Numbers(7, 3), Rando.Range(3, 7, -1));
		}

		// Token: 0x06002F05 RID: 12037 RVA: 0x000DAF70 File Offset: 0x000D9170
		public override Rhythm PickInitRhythm(int groupIndex)
		{
			return this.NewRhythm();
		}

		// Token: 0x06002F06 RID: 12038 RVA: 0x000DAF78 File Offset: 0x000D9178
		public override void OnRhythmUpdate(int groupIndex)
		{
			foreach (DestinationGroup destinationGroup in Get.Loadout.DestinationGroups)
			{
				destinationGroup.Module.ChangePulse(this.NewRhythm());
			}
			this.flipFlop = !this.flipFlop;
			base.UpdateDrumSequencer(Rando.Pick<Rhythm>(Rhythm.Claves), Rando.FlipCoin(Get.ZoomOutProgress), Rando.FlipCoin(Get.WeekProgress), Rando.FlipCoin(this.flipFlop ? 1f : 0f), false);
		}

		// Token: 0x06002F07 RID: 12039 RVA: 0x000DB024 File Offset: 0x000D9224
		private Rhythm NewRhythm()
		{
			return Rhythm.Sine(Rando.Range(4, 8, -1), Rando.Pick<float>(this.duratios), Rando.Pick<float>(this.duratios), 0.5f, 0f);
		}

		// Token: 0x040028A1 RID: 10401
		private List<float> duratios = Liszt.From<float>(new float[]
		{
			1f,
			1.25f,
			1.3333334f,
			1.5f,
			1.6666666f,
			1.75f,
			2f
		});

		// Token: 0x040028A2 RID: 10402
		private bool flipFlop;
	}
}
