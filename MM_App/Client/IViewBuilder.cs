using System;
using FixMath;
using Server;

namespace Client
{
	// Token: 0x02000797 RID: 1943
	public interface IViewBuilder
	{
		// Token: 0x0600359D RID: 13725
		void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp);
	}
}
