using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000695 RID: 1685
	public class Busan : MusicData
	{
		// Token: 0x06002EC5 RID: 11973 RVA: 0x000D94DC File Offset: 0x000D76DC
		public override void Injections()
		{
			List<Quality> qualities = new List<Quality>
			{
				new Quality("Dorian Pentatonic", new List<int>
				{
					3,
					2,
					2,
					2
				}, null),
				new Quality("Minor Penta 5", new List<int>
				{
					2,
					3,
					2,
					2
				}, null),
				new Quality("Minor Penta 1", new List<int>
				{
					3,
					2,
					2,
					3
				}, null),
				new Quality("Aeolian no6no7", new List<int>
				{
					2,
					1,
					2,
					2
				}, null),
				new Quality("Dorian no3", new List<int>
				{
					2,
					3,
					2,
					2,
					1
				}, null)
			};
			qualities = qualities.Chromatic(" (Chromatic)");
			base.SetQualities(qualities, null, true);
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

		// Token: 0x06002EC6 RID: 11974 RVA: 0x000D96FC File Offset: 0x000D78FC
		public override int ChordSize()
		{
			if (Get.Clock.Hour >= 6)
			{
				return Mathf.Min(Get.Week + 1, base.NoteWindow.Count);
			}
			return base.ChordSize();
		}

		// Token: 0x06002EC7 RID: 11975 RVA: 0x000D9729 File Offset: 0x000D7929
		public override float ChordSpread()
		{
			return Mathf.Lerp(0.5f, 0.05f, (float)(this.ChordSize() / base.NoteWindow.Count));
		}

		// Token: 0x06002EC8 RID: 11976 RVA: 0x000D9750 File Offset: 0x000D7950
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

		// Token: 0x04002887 RID: 10375
		private List<Rhythm> ClavesLong = Rhythm.Claves.Scale(1.5f);
	}
}
