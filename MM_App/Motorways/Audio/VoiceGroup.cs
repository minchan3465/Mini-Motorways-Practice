using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x02000662 RID: 1634
	public static class VoiceGroup
	{
		// Token: 0x06002D67 RID: 11623 RVA: 0x000D1684 File Offset: 0x000CF884
		public static void AddVoice(this List<AudioSample> sampleList, AudioSample voice)
		{
			if (voice != null)
			{
				sampleList.Add(voice);
			}
		}

		// Token: 0x06002D68 RID: 11624 RVA: 0x000D1690 File Offset: 0x000CF890
		public static void Limit(this List<AudioSample> sampleList, double fadeTime, int voiceLimit)
		{
			if (sampleList.Count == 0)
			{
				return;
			}
			while (sampleList.Count > voiceLimit)
			{
				if (fadeTime < 0.001)
				{
					sampleList[0].ElegantStop();
				}
				else
				{
					sampleList[0].FadeOutAndStop(fadeTime);
				}
				sampleList.RemoveAt(0);
			}
		}
	}
}
