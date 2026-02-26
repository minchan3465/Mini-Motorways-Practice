using System;
using FixMath;

namespace Server
{
	// Token: 0x0200028B RID: 651
	public interface ISimulationObserver
	{
		// Token: 0x06001009 RID: 4105
		void OnModelAdded(ISimulation simulation, IModel model, Fix64 timestamp);

		// Token: 0x0600100A RID: 4106
		void OnModelRemoved(ISimulation simulation, IModel model, Fix64 timestamp);
	}
}
