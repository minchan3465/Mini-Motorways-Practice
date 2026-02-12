using System;

namespace Motorways.Audio
{
	// Token: 0x020006A3 RID: 1699
	public class LosAngeles : MusicData
	{
		// Token: 0x06002EFE RID: 12030 RVA: 0x000DAD48 File Offset: 0x000D8F48
		public override void Injections()
		{
			base.SetEasterEggHorn("Special");
			base.SetRhythms(Rhythm.Frag(1f, -1).Uniform(12), MusicData.RhythmUpdateType.RandomParallel);
			base.SetKeyDeltas(Rando.Numbers(3, -1), 20);
			base.SetNoteSequenceStyles(Liszt.Make<MusicData.NoteSequenceType>(6, () => MusicData.NoteSequenceType.Backward));
			base.SetVoiceLimits(0.1, 5, 0.0, 5);
			Quality q = Quality.Clone(QualityDatabase.MAJOR, "");
			q.Scales.RemoveAt(6);
			base.SetQualities(Liszt.From<Quality>(new Quality[]
			{
				q
			}), null, true);
			base.SetPortamento(new Param.Portamento(0, 0, 0.0, 0.0), new Param.Portamento(-100, 100, 0.0, 0.5));
			base.SetVibrato(this.Vibrato, new Param.Vibrato(new Param.Data(10f, 20f), 20));
		}

		// Token: 0x06002EFF RID: 12031 RVA: 0x000DAE5C File Offset: 0x000D905C
		public override void OnRhythmUpdate(int groupIndex)
		{
			float duration = this.Rhythms[0].Duration;
			int steps = this.Rhythms[0].Steps.Length;
			Rhythm r = new Rhythm(0f, new D20(-1).Frag(steps, duration, 1f, -1f, -1f));
			foreach (DestinationGroup destinationGroup in Get.Loadout.DestinationGroups)
			{
				destinationGroup.Module.ChangePulse(r);
			}
		}
	}
}
