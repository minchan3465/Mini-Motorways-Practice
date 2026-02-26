using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x020006C0 RID: 1728
	public class Zurich : MusicData
	{
		// Token: 0x06002F9E RID: 12190 RVA: 0x000DE190 File Offset: 0x000DC390
		public override void Injections()
		{
			base.SetQualities(QualityDatabase.Gather(new string[]
			{
				"Dorian",
				"Aeolian",
				"Penta",
				"Dominant Penta",
				"Lydian Dominant"
			}).Keyless(), null, true);
			base.SetRhythms(Liszt.Flatten<Rhythm>(new List<Rhythm>[]
			{
				Rhythm.Triplet.Pulses(-1),
				Rhythm.Quintuplet.Pulses(-1)
			}), MusicData.RhythmUpdateType.RandomParallel);
			base.SetKeyDeltas(Liszt.From<int>(new int[]
			{
				-4,
				-2,
				1,
				3
			}), this.D20.Range(0, 6));
			base.SetWeekendChances(1f, 0f);
			base.SetNoteSequenceStyles(Liszt.Make<MusicData.NoteSequenceType>(5, delegate(int i)
			{
				MusicData.NoteSequenceType result;
				switch (i)
				{
				case 1:
					result = MusicData.NoteSequenceType.Backward;
					break;
				case 2:
					result = MusicData.NoteSequenceType.PingPong;
					break;
				case 3:
					result = MusicData.NoteSequenceType.Seeded;
					break;
				case 4:
					result = MusicData.NoteSequenceType.AutoReroll;
					break;
				default:
					result = MusicData.NoteSequenceType.Forward;
					break;
				}
				return result;
			}));
			base.SetVoiceLimits(0.1, 3, 0.0, 5);
			base.SetPortamento(new Param.Portamento(-150, 150, 0.0, 0.2), new Param.Portamento(0, 0, 0.0, 0.0));
			base.SetEchoDuratios(Liszt.From<float>(new float[]
			{
				this.Rhythms[this.RhythmPointer].Duration
			}));
			base.SetDrumSequencer(this.Rhythms.Shortest(), true, false, false, true, -1f, -1f);
		}

		// Token: 0x06002F9F RID: 12191 RVA: 0x000DE314 File Offset: 0x000DC514
		public override void OnNewWeek()
		{
			base.OnNewWeek();
			bool bap = false;
			bool hat = false;
			bool boom;
			switch (Get.Clock.Week)
			{
			case 0:
				break;
			case 1:
				goto IL_2D;
			case 2:
				bap = true;
				goto IL_2D;
			default:
				boom = Rando.FlipCoin(0.5f);
				hat = Rando.FlipCoin(0.5f);
				bap = Rando.FlipCoin(0.5f);
				goto IL_56;
			}
			IL_29:
			boom = true;
			goto IL_56;
			IL_2D:
			hat = true;
			goto IL_29;
			IL_56:
			base.UpdateDrumSequencer(Get.Loadout.DestinationGroupRhythms.Shortest(), boom, bap, hat, false);
			this.WeekendKeyChangeChance = ((Get.Clock.Week % 3 == 0) ? 1f : 0f);
		}
	}
}
