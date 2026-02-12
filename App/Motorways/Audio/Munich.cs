using System;

namespace Motorways.Audio
{
	// Token: 0x020006AC RID: 1708
	public class Munich : MusicData
	{
		// Token: 0x06002F23 RID: 12067 RVA: 0x000DBA03 File Offset: 0x000D9C03
		public override void Injections()
		{
			base.SetRhythms(Rhythm.Triplet.Pulses(-1).And(Rhythm.Duplet.Patterns(-1)), MusicData.RhythmUpdateType.RandomSingle);
			base.SetQualities(QualityDatabase.Gather(new string[]
			{
				"Maj7"
			}), null, true);
		}
	}
}
