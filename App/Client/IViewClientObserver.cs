using System;

namespace Client
{
	// Token: 0x02000798 RID: 1944
	public interface IViewClientObserver
	{
		// Token: 0x0600359E RID: 13726
		void OnViewAdded(IClient client, IView view);

		// Token: 0x0600359F RID: 13727
		void OnViewRemoved(IClient client, IView view);
	}
}
