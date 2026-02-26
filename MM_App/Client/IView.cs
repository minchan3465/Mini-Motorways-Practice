using System;

namespace Client
{
	// Token: 0x02000796 RID: 1942
	public interface IView
	{
		// Token: 0x0600359B RID: 13723
		TickResult Tick(TimeInterval tickTime, float stepAlpha);

		// Token: 0x0600359C RID: 13724
		void SetGameobjectActive(bool isActive);
	}
}
