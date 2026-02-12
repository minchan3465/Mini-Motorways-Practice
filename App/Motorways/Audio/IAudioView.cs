using System;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000641 RID: 1601
	public interface IAudioView
	{
		// Token: 0x170007A6 RID: 1958
		// (get) Token: 0x06002CD3 RID: 11475
		Vector2 Pan { get; }

		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x06002CD4 RID: 11476
		float Attenuation { get; }

		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x06002CD5 RID: 11477
		Transform transform { get; }

		// Token: 0x06002CD6 RID: 11478
		float GetAttenuation(bool zoom, float falloffFactor = 5f);
	}
}
