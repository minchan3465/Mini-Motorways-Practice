using System;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000633 RID: 1587
	public class AudioAnimEvents : MonoBehaviour
	{
		// Token: 0x06002C3B RID: 11323 RVA: 0x000C45D0 File Offset: 0x000C27D0
		public void FireAudioEvent(int type)
		{
			AudioSystem.Instance.ScheduleEvent(AudioEvent.CreateEvent(AudioSystem.Instance.DspTime, (AudioEventType)(1L << type), 0.5f, -1f, true, null));
		}
	}
}
