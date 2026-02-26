using System;

namespace Motorways.Audio
{
	// Token: 0x020006B7 RID: 1719
	public class Tokyo : MusicData
	{
		// Token: 0x06002F78 RID: 12152 RVA: 0x000DD370 File Offset: 0x000DB570
		public override void Injections()
		{
			base.SetRhythms(Rhythm.AllPulses(-1).Phase(0.1f), MusicData.RhythmUpdateType.LinearParallel);
			base.SetNoteSequenceStyles(Liszt.Make<MusicData.NoteSequenceType>(6, () => MusicData.NoteSequenceType.Forward));
			base.SetQualities(QualityDatabase.Gather(new string[]
			{
				"Penta",
				"Penta Chromodal",
				"Maj7",
				"7",
				"11",
				"13"
			}), QualityDatabase.Gather(new string[]
			{
				"Insen",
				"Insen Chromodal",
				"Ritsu",
				"Quartal"
			}), true);
		}
	}
}
