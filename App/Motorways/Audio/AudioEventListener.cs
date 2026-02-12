using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x0200063E RID: 1598
	public class AudioEventListener
	{
		// Token: 0x06002CCA RID: 11466 RVA: 0x000CF675 File Offset: 0x000CD875
		public virtual void Start(Action AddEventListeners)
		{
			AddEventListeners();
		}

		// Token: 0x06002CCB RID: 11467 RVA: 0x000CF680 File Offset: 0x000CD880
		public virtual void Stop()
		{
			for (int i = 0; i < this.eventListenerIds.Count; i++)
			{
				AudioSystem.Instance.RemoveAudioEventListener(this.eventListenerIds[i]);
			}
			this.eventListenerIds.Clear();
		}

		// Token: 0x06002CCC RID: 11468 RVA: 0x000CF6C4 File Offset: 0x000CD8C4
		public void Add(Action<AudioEvent> function, AudioEventType eventTypes, int groupIndex = -1)
		{
			AudioEventFilter f = new AudioEventFilter(eventTypes);
			f.GroupIndex = groupIndex;
			this.eventListenerIds.Add(AudioSystem.Instance.AddAudioEventListener(new AudioSystem.SignalAudioEventScheduled(function.Invoke), f));
		}

		// Token: 0x06002CCD RID: 11469 RVA: 0x000CF703 File Offset: 0x000CD903
		public void Add(Action<AudioEvent> function, UIEventType uiEventTypes, UIAudioProfile uiAudioProfile = UIAudioProfile.None)
		{
			this.eventListenerIds.Add(AudioSystem.Instance.AddAudioEventListener(new AudioSystem.SignalAudioEventScheduled(function.Invoke), new AudioEventFilter(uiEventTypes, uiAudioProfile)));
		}

		// Token: 0x06002CCE RID: 11470 RVA: 0x000CF72D File Offset: 0x000CD92D
		public void Add(Action<AudioEvent> function, AudioEventFilter filter)
		{
			this.eventListenerIds.Add(AudioSystem.Instance.AddAudioEventListener(new AudioSystem.SignalAudioEventScheduled(function.Invoke), filter));
		}

		// Token: 0x0400272B RID: 10027
		private List<int> eventListenerIds = new List<int>();
	}
}
