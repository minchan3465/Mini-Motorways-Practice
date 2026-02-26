using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006B5 RID: 1717
	public class RioDeJaneiro : MusicData
	{
		// Token: 0x06002F70 RID: 12144 RVA: 0x000DD0B8 File Offset: 0x000DB2B8
		public override void Injections()
		{
			base.SetQualities(Liszt.Flatten<Quality>(new List<Quality>[]
			{
				QualityDatabase.Gather(new string[]
				{
					"Min7",
					"Min7b5",
					"Maj6",
					"Maj7",
					"7",
					"Min9",
					"b7sus",
					"7#9",
					"11",
					"13",
					"Min11"
				}),
				Liszt.From<Quality>(new Quality[]
				{
					QualityDatabase.PENTA.Chromatic(""),
					QualityDatabase.PENTA_DOM.Chromatic("")
				})
			}), null, true);
			base.SetKeyDeltas(Liszt.From<int>(new int[]
			{
				-1
			}), Rando.Range(4, 7, -1));
			base.SetRhythms(Rando.Pick<List<Rhythm>>(new List<Rhythm>[]
			{
				Rhythm.Claves,
				this.ClavesLong
			}), MusicData.RhythmUpdateType.RandomParallel);
			base.SetDrumSequencer(this.Rhythms.Pick(-1), false, true, true, true, -1f, -1f);
			base.SetTremolo(new Param.LFO(new Param.Data(0.18f, 0.22f), new Param.Data(0.33f, 0.45f)), new Param.LFO(new Param.Data(5f, 5f), new Param.Data(0f, 0f)));
			base.SetVibrato(new Param.Vibrato(new Param.Data(0.72f, 0.88f), 25), new Param.Vibrato(new Param.Data(5f, 5f), 0));
			base.SetVoiceLimits(0.1, 4, 0.0, 2);
		}

		// Token: 0x06002F71 RID: 12145 RVA: 0x000D96FC File Offset: 0x000D78FC
		public override int ChordSize()
		{
			if (Get.Clock.Hour >= 6)
			{
				return Mathf.Min(Get.Week + 1, base.NoteWindow.Count);
			}
			return base.ChordSize();
		}

		// Token: 0x06002F72 RID: 12146 RVA: 0x000D9729 File Offset: 0x000D7929
		public override float ChordSpread()
		{
			return Mathf.Lerp(0.5f, 0.05f, (float)(this.ChordSize() / base.NoteWindow.Count));
		}

		// Token: 0x06002F73 RID: 12147 RVA: 0x000DD26C File Offset: 0x000DB46C
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
			float changePatternChance = Mathf.Max(Get.WeekProgress, Get.ZoomOutProgress);
			if (Get.Hour % 12 == 0 && Rando.FlipCoin(changePatternChance))
			{
				base.UpdateDrumSequencer(this.Rhythms.Pick(-1), this.Boom, this.Bap, this.Hat, false);
			}
			if (Get.Hour % Rando.Pick<int>(new int[]
			{
				6,
				9
			}) == 0)
			{
				Get.Loadout.DrumSequencer.Parts.ForEach(delegate(DrumSequencer.Part x)
				{
					x.Toggle(0.25f);
				});
			}
		}

		// Token: 0x040028FD RID: 10493
		private List<Rhythm> ClavesLong = Rhythm.Claves.Scale(1.5f);
	}
}
