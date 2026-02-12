using System;
using Unity.Profiling;

// Token: 0x02000268 RID: 616
public static class ProfilerUtility
{
	// Token: 0x040008A8 RID: 2216
	public static readonly ProfilerCategory CategoryProcess = new ProfilerCategory("Simulation.Processes");

	// Token: 0x040008A9 RID: 2217
	public static readonly ProfilerCategory CategoryModel = new ProfilerCategory("Simulation.Model");
}
