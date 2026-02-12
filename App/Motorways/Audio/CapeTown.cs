using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000699 RID: 1689
	public class CapeTown : MusicData
	{
		// Token: 0x06002ED4 RID: 11988 RVA: 0x000D9B4C File Offset: 0x000D7D4C
		public override void Injections()
		{
			this._rhythms = Liszt.From<Rhythm>(new Rhythm[]
			{
				new Rhythm(0f, new float[]
				{
					0.75f,
					1.25f
				}),
				new Rhythm(0f, new float[]
				{
					0.25f,
					0.25f,
					0.25f,
					1.25f
				}),
				new Rhythm(0f, new float[]
				{
					0.75f,
					0.25f,
					0.5f,
					0.5f
				}),
				new Rhythm(0f, new float[]
				{
					0.75f,
					0.75f,
					0.5f
				})
			});
			Quality q = new Quality("IMaj6/9", Liszt.From<int>(new int[]
			{
				2,
				2,
				3,
				2,
				3
			}), Liszt.From<int>(new int[]
			{
				0,
				12
			}));
			q.Scales.Add(new Scale(2, "II7", Liszt.From<int>(new int[]
			{
				4,
				3,
				3,
				2
			}), Liszt.From<int>(new int[]
			{
				0,
				12
			})));
			q.Scales.Add(new Scale(4, "III-7", Liszt.From<int>(new int[]
			{
				3,
				4,
				3,
				2
			}), Liszt.From<int>(new int[]
			{
				0,
				12
			})));
			q.Scales.Add(new Scale(5, "IVMaj9", Liszt.From<int>(new int[]
			{
				2,
				2,
				3,
				4,
				1
			}), Liszt.From<int>(new int[]
			{
				0,
				12
			})));
			q.Scales.Add(new Scale(7, "V7", Liszt.From<int>(new int[]
			{
				4,
				3,
				3,
				2
			}), Liszt.From<int>(new int[]
			{
				0,
				12
			})));
			q.Scales.Add(new Scale(9, "VI-7", Liszt.From<int>(new int[]
			{
				3,
				4,
				3,
				2
			}), Liszt.From<int>(new int[]
			{
				0,
				12
			})));
			base.SetKeyDeltas(Liszt.From<int>(new int[]
			{
				-4,
				-2,
				1,
				3
			}), this.D20.Range(0, 6));
			base.SetVoiceLimits(0.1, 4, 0.1, 1);
			base.SetTremolo(new Param.LFO(new Param.Data(2.4f, -1f), new Param.Data(0.25f, 0.5f)), null);
			base.SetRhythms(this._rhythms, MusicData.RhythmUpdateType.RandomParallel);
			base.SetDrumSequencer(this._rhythms.Pick(-1), true, true, true, true, -1f, -1f);
			base.SetQualities(Liszt.From<Quality>(new Quality[]
			{
				q
			}), null, true);
		}

		// Token: 0x06002ED5 RID: 11989 RVA: 0x000D9DF8 File Offset: 0x000D7FF8
		public override void OnHour()
		{
			if (Get.Hour % 4 != 0)
			{
				return;
			}
			base.UpdateNoteWindow(Rando.Range(2, Get.MaxGroups - 1, -1), 0.5f, 0, 0f, false);
			float timeElapsed = Time.time - this.timeAtStart;
			if (Rando.FlipCoin(0.5f) && timeElapsed > 3f)
			{
				AudioSample bass = this.Bass;
				if (bass != null)
				{
					bass.FadeOutAndStop(0.05);
				}
				this.Bass = AudioPlayer.Default.PlaySample("bass_" + Note.SCALE[this.CurrentScale.Key], 0.5f, 0.2f, 1f, 0.0, Get.Pulse.Master.Next, false, null, false, false, 0f, false);
			}
			if (!Rando.FlipCoin(0.5f))
			{
				return;
			}
			Rhythm rhythm = this._rhythms.Pick(-1).Scale(Get.Pulse.Scale.Scale, false);
			base.UpdateDrumSequencer(rhythm, true, true, true, false);
		}

		// Token: 0x0400288F RID: 10383
		private List<Rhythm> _rhythms;
	}
}
