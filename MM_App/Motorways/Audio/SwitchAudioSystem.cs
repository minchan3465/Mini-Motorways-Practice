using System;

namespace Motorways.Audio
{
	// Token: 0x020006EC RID: 1772
	public class SwitchAudioSystem : AudioSystem
	{
		// Token: 0x17000818 RID: 2072
		// (get) Token: 0x06003087 RID: 12423 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool RequiresVolumeControl
		{
			get
			{
				return false;
			}
		}
	}
}
