using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x0200069A RID: 1690
	public class ChiangMai : MusicData
	{
		// Token: 0x06002ED7 RID: 11991 RVA: 0x000D9F00 File Offset: 0x000D8100
		public override void Injections()
		{
			base.SetRhythms(Rhythm.AllPatterns(-1).Phase(0.0625f), MusicData.RhythmUpdateType.RandomParallel);
			base.SetNoteSequenceStyles(Liszt.Make<MusicData.NoteSequenceType>(6, () => MusicData.NoteSequenceType.AutoReroll));
			base.SetQualities(new List<Quality>
			{
				QualityDatabase.NINE.Chromatic("")
			}, new List<Quality>
			{
				QualityDatabase.INSEN,
				QualityDatabase.PENTA_DOM
			}.Chromodal(), true);
			base.SetVibrato(new Param.Vibrato(new Param.Data(2f, 6f), 12), null);
			base.SetKeyDeltas(Liszt.From<int>(new int[1]), Rando.Range(6, 8, -1));
		}

		// Token: 0x06002ED8 RID: 11992 RVA: 0x000D9FC8 File Offset: 0x000D81C8
		public override void OnRhythmUpdate(int groupIndex)
		{
			float progress = (float)this._rhythmUpdates / 40f;
			if (this._rhythmUpdates % 2 == 0)
			{
				base.UpdateDrumSequencer(Rando.Pick<Rhythm>(Rhythm.Duplet.Patterns(-1)), Rando.FlipCoin(progress), Rando.FlipCoin(0.75f * progress), Rando.FlipCoin(0.5f * progress), Rando.FlipCoin(0.5f));
			}
			if (this._rhythmUpdates < 40)
			{
				this._rhythmUpdates++;
			}
		}

		// Token: 0x06002ED9 RID: 11993 RVA: 0x000DA043 File Offset: 0x000D8243
		public override void OnHouseConnected(int groupIndex)
		{
			base.UpdateNoteWindow(-1, 1f, 0, 0f, false);
		}

		// Token: 0x04002890 RID: 10384
		private int _rhythmUpdates;

		// Token: 0x04002891 RID: 10385
		private const int MaxRhythmUpdates = 40;
	}
}
