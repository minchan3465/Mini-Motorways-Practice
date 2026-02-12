using System;
using Factory;
using Factory.Pools;
using FixMath;

namespace Server
{
	// Token: 0x02000289 RID: 649
	[Factory.Serializable(1)]
	public interface IProcess : IReusable
	{
		// Token: 0x06000FF8 RID: 4088
		void Step(ISimulation simulation, Fix64 timestep);
	}
}
