using System;
using Server;

namespace Client
{
	// Token: 0x02000792 RID: 1938
	public interface IClient : ISimulationObserver
	{
		// Token: 0x0600358D RID: 13709
		void Start();

		// Token: 0x0600358E RID: 13710
		void Tick(TimeInterval tickTime, float stepAlpha);

		// Token: 0x0600358F RID: 13711
		void ApplyTheme(ITheme theme);

		// Token: 0x06003590 RID: 13712
		void ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress);
	}
}
