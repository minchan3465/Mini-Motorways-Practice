using System;
using System.Linq;

namespace Motorways.Audio
{
	// Token: 0x020006B9 RID: 1721
	public class Tutorial : MusicData
	{
		// Token: 0x06002F7D RID: 12157 RVA: 0x000DD438 File Offset: 0x000DB638
		public override void Injections()
		{
			base.SetEchoDuratios(Liszt.From<float>(new float[]
			{
				0.75f
			}));
			base.SetRhythms(Rhythm.Duplet.Pattern(-1).Uniform(12).Phase(0.0625f), MusicData.RhythmUpdateType.LinearParallel);
			base.SetDrumSequencer(new Rhythm(0f, new float[]
			{
				this.Rhythms[0].Steps.Min()
			}), true, true, true, true, -1f, -1f);
			base.SetNoteSequenceStyles(Liszt.Make<MusicData.NoteSequenceType>(6, () => MusicData.NoteSequenceType.Seeded));
			base.SetQualities(QualityDatabase.Gather(new string[]
			{
				"Major Triad",
				"Sus",
				"Maj7",
				"Sus2Maj7"
			}), null, true);
			base.SetKeyDeltas(Liszt.From<int>(new int[1]), 20);
		}
	}
}
