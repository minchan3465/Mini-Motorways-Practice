using System;

namespace Motorways.Audio
{
	// Token: 0x0200069F RID: 1695
	public class Dubai : MusicData
	{
		// Token: 0x06002EED RID: 12013 RVA: 0x000DA81C File Offset: 0x000D8A1C
		public override void Injections()
		{
			base.SetQualities(QualityDatabase.Gather(new string[]
			{
				"Sus2Min7",
				"Min7",
				"Min11",
				"Min9",
				"Sus"
			}), null, true);
			base.SetDrumSequencer(new Rhythm(Rando.Pick<float>(new float[]
			{
				0f,
				0.33333334f,
				0.5f,
				0.6666667f,
				0.75f
			}), new float[]
			{
				0.25f,
				0.25f,
				0.25f,
				0.25f
			}), true, false, false, false, -1f, -1f);
		}

		// Token: 0x06002EEE RID: 12014 RVA: 0x000DA8A4 File Offset: 0x000D8AA4
		public override void OnHour()
		{
			switch (Get.Week % 3)
			{
			case 0:
				this.DrumVolume = Get.WeekProgress;
				break;
			case 1:
				this.DrumVolume = 1f;
				break;
			case 2:
				this.DrumVolume = 1f - Get.WeekProgress;
				this.Bap = true;
				this.UseEuclideanDrumGates = true;
				break;
			default:
				this.Bap = false;
				this.UseEuclideanDrumGates = false;
				break;
			}
			this.Hat = (Get.Week % 2 == 1);
		}

		// Token: 0x06002EEF RID: 12015 RVA: 0x000DA928 File Offset: 0x000D8B28
		public override void OnDrumPulse()
		{
			int num = this.tick;
			this.tick = num + 1;
			this.Boom = (num % 4 == 0);
		}

		// Token: 0x0400289C RID: 10396
		private int tick;
	}
}
