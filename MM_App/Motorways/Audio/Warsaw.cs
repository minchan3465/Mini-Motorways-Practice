using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x020006BC RID: 1724
	public class Warsaw : MusicData
	{
		// Token: 0x06002F89 RID: 12169 RVA: 0x000DDA70 File Offset: 0x000DBC70
		public override void Injections()
		{
			Quality q = Quality.Clone(QualityDatabase.MAJOR, "");
			q.Scales.RemoveAt(6);
			q.Scales.RemoveAt(5);
			q.Scales.RemoveAt(2);
			q.Chromatic("");
			base.SetQualities(new List<Quality>
			{
				q
			}, QualityDatabase.Gather(new string[]
			{
				"Wholetone",
				"Maj7"
			}), true);
			base.SetRhythms(Rhythm.Triplet.Patterns(-1).And(Rhythm.Duplet.Patterns(-1)), MusicData.RhythmUpdateType.RandomParallel);
			base.SetNoteSequenceStyles(Liszt.Make<MusicData.NoteSequenceType>(5, () => Rando.EnumValue<MusicData.NoteSequenceType>(0, -1)));
			base.SetTremolo(new Param.LFO(new Param.Data(0.2f, 0.3f), new Param.Data(0.1f, 0.2f)), new Param.LFO(new Param.Data(2f, 3f), new Param.Data(0.4f, 0.5f)));
		}

		// Token: 0x06002F8A RID: 12170 RVA: 0x000DDB84 File Offset: 0x000DBD84
		public override void OnRhythmUpdate(int groupIndex)
		{
			float progress = (float)this._rhythmUpdates / 40f;
			if (this._rhythmUpdates % 2 == 0)
			{
				base.UpdateDrumSequencer(Rhythm.Duplet.Patterns(-1).Pick(-1), Rando.FlipCoin(0.25f * progress), Rando.FlipCoin(0.75f * progress), Rando.FlipCoin(progress), Rando.FlipCoin(0.5f));
			}
			if (this._rhythmUpdates < 40)
			{
				this._rhythmUpdates++;
			}
		}

		// Token: 0x06002F8B RID: 12171 RVA: 0x000DA043 File Offset: 0x000D8243
		public override void OnHouseConnected(int groupIndex)
		{
			base.UpdateNoteWindow(-1, 1f, 0, 0f, false);
		}

		// Token: 0x04002907 RID: 10503
		private int _rhythmUpdates = 10;

		// Token: 0x04002908 RID: 10504
		private const int MaxRhythmUpdates = 40;
	}
}
