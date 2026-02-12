using System;

namespace Motorways.Audio
{
	// Token: 0x020006D8 RID: 1752
	public class TrafficLight : Playback
	{
		// Token: 0x0600301F RID: 12319 RVA: 0x000E1478 File Offset: 0x000DF678
		public TrafficLight(AudioEventFilter filter) : base(filter)
		{
		}

		// Token: 0x06003020 RID: 12320 RVA: 0x000E1E72 File Offset: 0x000E0072
		protected override void OnPulse()
		{
			if (base.GetEvents(0))
			{
				this.audioEvents.ForEach(new Action<AudioEvent>(this.HandleEvent));
				this.audioEvents.Clear();
			}
		}

		// Token: 0x06003021 RID: 12321 RVA: 0x000E1EA0 File Offset: 0x000E00A0
		private void HandleEvent(AudioEvent e)
		{
			double t = AudioSystem.Instance.DspTime;
			AudioEventType type = e.Type;
			if (type != AudioEventType.TrafficLightGreen)
			{
				if (type == AudioEventType.TrafficLightAmber)
				{
					AudioPlayer.UI.PlaySample("PeepAppears_EGG", 0.5f, 0.5f, 1f, 0.0, t + 0.4, false, null, false, false, 0f, false);
					return;
				}
			}
			else
			{
				AudioPlayer.UI.PlaySample("PeepAppears_SQUARE", 0.5f, 0.5f, 1f, 0.0, t + 0.6, false, null, false, false, 0f, false);
			}
		}
	}
}
