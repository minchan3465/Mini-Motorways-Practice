using System;

namespace Motorways.Audio
{
	// Token: 0x020006A0 RID: 1696
	public class HongKong : MusicData
	{
		// Token: 0x06002EF1 RID: 12017 RVA: 0x000DA954 File Offset: 0x000D8B54
		public override void Injections()
		{
			base.SetRhythms(Rhythm.Duplet.Patterns(-1).Crop(2f).Scale(0.75f), MusicData.RhythmUpdateType.RandomParallel);
			base.SetQualities(QualityDatabase.Gather(new string[]
			{
				"Quartal",
				"Overtone",
				"Penta Chromodal"
			}), null, true);
			base.SetFadeInProgression(1.0, 0.0, false);
			base.SetDrumSequencer(this.Rhythms.Shortest(), true, true, true, false, -1f, 50f);
		}

		// Token: 0x06002EF2 RID: 12018 RVA: 0x000DA9EC File Offset: 0x000D8BEC
		public override void OnHour()
		{
			if (Get.Hour % this._bassFreq != 1)
			{
				return;
			}
			this._bassFreq = Rando.Pick<int>(new int[]
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
			AudioPlayer @default = AudioPlayer.Default;
			string str = "bass_";
			string text = note;
			this.Bass = @default.PlaySample(str + text.Substring(0, text.Length - 1), 0.5f, 0.4f, -1f, 0.5, -1.0, false, null, false, false, 0f, false);
			base.UpdateDrumSequencer(Get.Loadout.DestinationGroupRhythms.Shortest().Scale(Get.Pulse.Scale.Scale, false), true, Rando.FlipCoin(0.5f), true, false);
		}

		// Token: 0x0400289D RID: 10397
		private int _bassFreq = 6;
	}
}
