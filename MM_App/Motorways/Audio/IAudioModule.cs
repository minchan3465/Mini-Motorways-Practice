using System;

namespace Motorways.Audio
{
	// Token: 0x02000690 RID: 1680
	public interface IAudioModule
	{
		// Token: 0x06002E9D RID: 11933
		void Activate(AudioEnvironment environment);

		// Token: 0x06002E9E RID: 11934
		void Deactivate();

		// Token: 0x06002E9F RID: 11935
		void Release();

		// Token: 0x06002EA0 RID: 11936
		void UpdateModule();
	}
}
