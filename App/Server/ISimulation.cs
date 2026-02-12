using System;
using Factory;
using FixMath;

namespace Server
{
	// Token: 0x0200028A RID: 650
	[Factory.Serializable(2)]
	public interface ISimulation
	{
		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06000FF9 RID: 4089
		Fix64 Timestep { get; }

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06000FFA RID: 4090
		// (set) Token: 0x06000FFB RID: 4091
		bool IsPaused { get; set; }

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06000FFC RID: 4092
		IScope Scope { get; }

		// Token: 0x06000FFD RID: 4093
		bool Step();

		// Token: 0x06000FFE RID: 4094
		bool AddProcess(IProcess process);

		// Token: 0x06000FFF RID: 4095
		bool AddModel(IModel model);

		// Token: 0x06001000 RID: 4096
		bool RemoveModel(IModel model);

		// Token: 0x06001001 RID: 4097
		bool ContainsModel(IModel model);

		// Token: 0x06001002 RID: 4098
		T GetModel<T>() where T : class, IModel;

		// Token: 0x06001003 RID: 4099
		ModelList<T> GetModels<T>() where T : class, IModel;

		// Token: 0x06001004 RID: 4100
		bool ScheduleCommand(Command command);

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06001005 RID: 4101
		bool HasAnyScheduledCommands { get; }

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06001006 RID: 4102
		Command NextScheduledCommand { get; }

		// Token: 0x06001007 RID: 4103
		void Subscribe(ISimulationObserver observer);

		// Token: 0x06001008 RID: 4104
		bool Unsubscribe(ISimulationObserver observer);
	}
}
