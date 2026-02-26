using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006A1 RID: 1697
	public class Lisbon : MusicData
	{
		// Token: 0x06002EF4 RID: 12020 RVA: 0x000DAAE4 File Offset: 0x000D8CE4
		public override void Injections()
		{
			Quality q = Quality.Clone(QualityDatabase.MAJOR, "");
			q.Scales.RemoveAt(6);
			q.Chromatic("");
			base.SetQualities(Liszt.From<Quality>(new Quality[]
			{
				q
			}), null, true);
			base.SetKeyDeltas(Liszt.From<int>(new int[]
			{
				-1
			}), Rando.Range(4, 7, -1));
			base.SetRhythms(Rando.Pick<List<Rhythm>>(new List<Rhythm>[]
			{
				Rhythm.Claves,
				this.ClavesLong
			}), MusicData.RhythmUpdateType.RandomSingle);
			base.SetTremolo(new Param.LFO(new Param.Data(5f, 5f), new Param.Data(0f, 0f)), new Param.LFO(new Param.Data(0.18f, 0.22f), new Param.Data(0.33f, 0.45f)));
			base.SetVibrato(new Param.Vibrato(new Param.Data(0.72f, 0.88f), 25), new Param.Vibrato(new Param.Data(5f, 5f), 0));
			base.SetVoiceLimits(0.1, 5, 0.0, 2);
		}

		// Token: 0x06002EF5 RID: 12021 RVA: 0x000D96FC File Offset: 0x000D78FC
		public override int ChordSize()
		{
			if (Get.Clock.Hour >= 6)
			{
				return Mathf.Min(Get.Week + 1, base.NoteWindow.Count);
			}
			return base.ChordSize();
		}

		// Token: 0x06002EF6 RID: 12022 RVA: 0x000D9729 File Offset: 0x000D7929
		public override float ChordSpread()
		{
			return Mathf.Lerp(0.5f, 0.05f, (float)(this.ChordSize() / base.NoteWindow.Count));
		}

		// Token: 0x06002EF7 RID: 12023 RVA: 0x000DAC07 File Offset: 0x000D8E07
		public override void OnHour()
		{
			if (Get.Clock.Hour < 6)
			{
				return;
			}
			if (Get.Hour % 18 == 0 && Rando.FlipCoin(0.5f))
			{
				base.UpdateNoteWindow(-1, 1f, 0, 0f, false);
			}
		}

		// Token: 0x06002EF8 RID: 12024 RVA: 0x000DA043 File Offset: 0x000D8243
		public override void OnHouseConnected(int groupIndex)
		{
			base.UpdateNoteWindow(-1, 1f, 0, 0f, false);
		}

		// Token: 0x0400289E RID: 10398
		private List<Rhythm> ClavesLong = Rhythm.Claves.Scale(1.5f);
	}
}
