using System;

namespace Helpers.GameCenter
{
	// Token: 0x02000790 RID: 1936
	public class NullGameCenterAuthentication : IGameCenterAuthentication
	{
		// Token: 0x0600357D RID: 13693 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Authenticate()
		{
		}

		// Token: 0x170008DF RID: 2271
		// (get) Token: 0x0600357E RID: 13694 RVA: 0x0000222C File Offset: 0x0000042C
		public bool IsAuthenticated
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x0600357F RID: 13695 RVA: 0x0000222C File Offset: 0x0000042C
		public bool RequiresRetry
		{
			get
			{
				return false;
			}
		}
	}
}
