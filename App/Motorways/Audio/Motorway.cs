using System;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006D5 RID: 1749
	public class Motorway : Playback
	{
		// Token: 0x06003008 RID: 12296 RVA: 0x000E1698 File Offset: 0x000DF898
		public Motorway(AudioEventFilter filter) : base(filter)
		{
		}

		// Token: 0x06003009 RID: 12297 RVA: 0x000022F5 File Offset: 0x000004F5
		protected override void OnPulse()
		{
		}

		// Token: 0x0600300A RID: 12298 RVA: 0x000E16AC File Offset: 0x000DF8AC
		public override void AddEventListeners()
		{
			this.EventListener.Add(new Action<AudioEvent>(this.OnMotorwayHandle), AudioEventType.MotorwayHandlePulled | AudioEventType.MotorwayHandleReleased, -1);
		}

		// Token: 0x0600300B RID: 12299 RVA: 0x000E16D0 File Offset: 0x000DF8D0
		private void OnMotorwayHandle(AudioEvent e)
		{
			AudioEventType type = e.Type;
			if (type == AudioEventType.MotorwayHandlePulled)
			{
				this._n = Rando.Pick<string>(Get.Loadout.MusicData.NoteWindow);
				this._n = Note.Transpose(-24, this._n);
				AudioPlayer.Default.PlaySample("StationAdded_" + this._n, e.Pan, 0.75f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
				return;
			}
			if (type != AudioEventType.MotorwayHandleReleased)
			{
				return;
			}
			float a = Maf.Normalize(e.Magnitude, 0f, 10f, true);
			if (Mathf.Approximately(a, 0f))
			{
				return;
			}
			float p = 1f + (float)Mathf.Min(Mathf.FloorToInt(a * 3f), 2) * 0.5f;
			AudioPlayer.Default.PlaySample("StationAdded_" + this._n, e.Pan, Mathf.Lerp(0.1f, 1f, a), 1f, 0.0, -1.0, false, new FX.Modulator(null, new FX.Modulator.Vibrato((double)Mathf.Lerp(10f, 20f, a), Maf.Lerp(0.0, 0.05, (double)a), (double)p, (double)UnityEngine.Random.value), null), false, false, 0f, true);
		}

		// Token: 0x04002977 RID: 10615
		private string _n = "C3";
	}
}
