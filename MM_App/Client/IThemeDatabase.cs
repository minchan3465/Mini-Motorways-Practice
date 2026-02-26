using System;

namespace Client
{
	// Token: 0x02000795 RID: 1941
	public interface IThemeDatabase
	{
		// Token: 0x06003595 RID: 13717
		void Start();

		// Token: 0x06003596 RID: 13718
		void Tick(float deltaTime);

		// Token: 0x06003597 RID: 13719
		ITheme GetTheme();

		// Token: 0x06003598 RID: 13720
		void AddView(IClient view);

		// Token: 0x06003599 RID: 13721
		void RemoveView(IClient view);

		// Token: 0x0600359A RID: 13722
		void DisableDeleteModeOverrides();
	}
}
