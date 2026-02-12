using System;

namespace Motorways.Audio
{
	// Token: 0x02000694 RID: 1684
	public class Beijing : MusicData
	{
		// Token: 0x06002EC2 RID: 11970 RVA: 0x000D9368 File Offset: 0x000D7568
		public override void Injections()
		{
			base.SetRhythms(Rhythm.Duplet.Patterns(-1).Crop(2f), MusicData.RhythmUpdateType.RandomParallel);
			base.SetQualities(QualityDatabase.Gather(new string[]
			{
				"Quartal",
				"Overtone",
				"Penta Chromodal"
			}), null, true);
			base.SetFadeInProgression(1.0, 0.0, false);
			base.SetDrumSequencer(this.Rhythms.Shortest(), true, true, true, false, -1f, 50f);
		}

		// Token: 0x06002EC3 RID: 11971 RVA: 0x000D93F4 File Offset: 0x000D75F4
		public override void OnHour()
		{
			if (Get.Hour % this.bassFreq == 1)
			{
				this.bassFreq = Rando.Pick<int>(new int[]
				{
					3,
					6,
					9,
					12,
					15,
					18,
					21,
					24
				});
				string note = Rando.Pick<string>(base.NoteWindow);
				AudioSample bass = this.Bass;
				if (bass != null)
				{
					bass.FadeOutAndStop(0.5);
				}
				this.Bass = AudioPlayer.Default.PlaySample("bass_" + note.Substring(0, note.Length - 1), 0.5f, 0.4f, -1f, 0.5, -1.0, false, null, false, false, 0f, false);
				base.UpdateDrumSequencer(Get.Loadout.DestinationGroupRhythms.Shortest(), true, Rando.FlipCoin(0.5f), true, false);
			}
		}

		// Token: 0x04002886 RID: 10374
		private int bassFreq = 6;
	}
}
