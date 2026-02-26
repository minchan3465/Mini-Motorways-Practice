using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x020006A8 RID: 1704
	public class MexicoCity : MusicData
	{
		// Token: 0x06002F12 RID: 12050 RVA: 0x000DB3D3 File Offset: 0x000D95D3
		private List<Rhythm> GenRhythms()
		{
			return Rhythm.Frag(0.25f, -1).Uniform(12).Scatter(-1);
		}

		// Token: 0x06002F13 RID: 12051 RVA: 0x000DB3F0 File Offset: 0x000D95F0
		public override void Injections()
		{
			base.SetEasterEggHorn("Special");
			base.SetWeekendChances(0f, 1f);
			base.SetRhythms(this.GenRhythms(), MusicData.RhythmUpdateType.RandomAll);
			base.SetQualities(Liszt.From<Quality>(new Quality[]
			{
				QualityDatabase.MAJOR.GetMode(0, "Ionian"),
				QualityDatabase.MAJOR.GetMode(5, "Aeolian"),
				QualityDatabase.Find("7"),
				QualityDatabase.Find("7b9")
			}), null, true);
			base.SetVoiceLimits(0.05, 2, 0.0, 5);
			base.SetDrumSequencer(this.Rhythms.Shortest(), false, false, false, true, -1f, -1f);
		}

		// Token: 0x06002F14 RID: 12052 RVA: 0x000DB4B4 File Offset: 0x000D96B4
		public override void OnRhythmUpdate(int groupIndex)
		{
			this.Rhythms = this.GenRhythms();
			base.UpdateDrumSequencer(this.Rhythms.Shortest(), this.Boom, this.Bap, this.Hat, false);
			foreach (DestinationGroup destinationGroup in Get.Loadout.DestinationGroups)
			{
				destinationGroup.Module.ChangePulse(Rando.Pick<Rhythm>(this.Rhythms));
			}
		}

		// Token: 0x06002F15 RID: 12053 RVA: 0x000DB548 File Offset: 0x000D9748
		public override void OnConnection()
		{
			this.Boom = Rando.FlipCoin(0.5f);
			this.Bap = Rando.FlipCoin(0.5f);
			this.Hat = Rando.FlipCoin(0.5f);
			this.UseEuclideanDrumGates = Rando.FlipCoin(0.5f);
			Get.Loadout.DrumSequencer.Hat.PseudoUpbeatChance = Rando.m(-1);
			if (!this.Boom && !this.Bap && !this.Hat)
			{
				Rando.Pick<bool>(new bool[]
				{
					this.Boom,
					this.Bap,
					this.Hat
				}).Flip();
			}
		}

		// Token: 0x06002F16 RID: 12054 RVA: 0x000DB5F4 File Offset: 0x000D97F4
		public override float SamplePitchSign()
		{
			float result;
			switch (Get.Day)
			{
			case 0:
				result = 1f;
				break;
			case 1:
				result = (Rando.FlipCoin(0.8333333f) ? 1f : -1f);
				break;
			case 2:
				result = (Rando.FlipCoin(0.6666667f) ? 1f : -1f);
				break;
			case 3:
				result = (Rando.FlipCoin(0.5f) ? 1f : -1f);
				break;
			case 4:
				result = (Rando.FlipCoin(0.33333334f) ? 1f : -1f);
				break;
			case 5:
				result = (Rando.FlipCoin(0.16666667f) ? 1f : -1f);
				break;
			case 6:
				result = -1f;
				break;
			default:
				result = 1f;
				break;
			}
			return result;
		}
	}
}
