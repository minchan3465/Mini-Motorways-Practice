using System;

namespace Motorways.Audio
{
	// Token: 0x020006D2 RID: 1746
	public class Experiment : Playback
	{
		// Token: 0x06002FFF RID: 12287 RVA: 0x000E1464 File Offset: 0x000DF664
		public Experiment(AudioEventFilter filter) : base(filter, new string[1], 1f)
		{
		}

		// Token: 0x06003000 RID: 12288 RVA: 0x000022F5 File Offset: 0x000004F5
		protected override void OnPulse()
		{
		}

		// Token: 0x06003001 RID: 12289 RVA: 0x000022F5 File Offset: 0x000004F5
		public override void Update()
		{
		}
	}
}
