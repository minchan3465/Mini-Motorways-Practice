using System;

namespace Helpers.GameCenter
{
	// Token: 0x0200078F RID: 1935
	public interface IGameCenterAuthentication
	{
		// Token: 0x0600357A RID: 13690
		void Authenticate();

		// Token: 0x170008DD RID: 2269
		// (get) Token: 0x0600357B RID: 13691
		bool IsAuthenticated { get; }

		// Token: 0x170008DE RID: 2270
		// (get) Token: 0x0600357C RID: 13692
		bool RequiresRetry { get; }
	}
}
