using System;

namespace Motorways.Audio
{
	// Token: 0x020006A9 RID: 1705
	public class Moscow : MusicData
	{
		// Token: 0x06002F18 RID: 12056 RVA: 0x000DB6D0 File Offset: 0x000D98D0
		public override void Injections()
		{
			base.SetRhythms(Liszt.Make<Rhythm>(12, () => Rando.Pick<Rhythm>(new Rhythm[]
			{
				Rhythm.Frag(1f, -1),
				Rhythm.Sine(Rando.Range(3, 8, -1), Rando.Pick<float>(Rhythm.FragRatios(0)), Rando.Range(0.25f, 2f, -1), Rando.Range(0.25f, 0.75f, -1), 0f),
				Rando.Pick<Rhythm>(Rhythm.AllPlets(-1))
			})), Rando.EnumValue<MusicData.RhythmUpdateType>(0, -1));
			base.SetFadeInProgression(this.ZeroOrRandom(), this.ZeroOrRandom(), Rando.FlipCoin(0.5f));
			base.SetFadeInTimes(this.ZeroOrRandom(), this.ZeroOrRandom());
			base.SetNoteSequenceStyles(Liszt.Make<MusicData.NoteSequenceType>(6, () => Rando.EnumValue<MusicData.NoteSequenceType>(0, -1)));
		}

		// Token: 0x06002F19 RID: 12057 RVA: 0x000DB769 File Offset: 0x000D9969
		private double ZeroOrRandom()
		{
			return Rando.Pick<double>(new double[]
			{
				0.0,
				0.0,
				0.0,
				Rando.Range(0.0, 0.25, -1)
			});
		}
	}
}
