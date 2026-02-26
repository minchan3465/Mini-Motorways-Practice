using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;

namespace Server
{
	// Token: 0x02000294 RID: 660
	public class Simulation : ISimulation, IReusable, ICreatedInScopeHandler, IReleasedFromScopeHandler
	{
		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06001029 RID: 4137 RVA: 0x00036231 File Offset: 0x00034431
		// (set) Token: 0x0600102A RID: 4138 RVA: 0x00036239 File Offset: 0x00034439
		[Serialize(true, null)]
		public Fix64 Timestep { get; private set; } = Simulation.DefaultTimestep;

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x0600102B RID: 4139 RVA: 0x00036242 File Offset: 0x00034442
		// (set) Token: 0x0600102C RID: 4140 RVA: 0x0003624A File Offset: 0x0003444A
		[Serialize(true, null)]
		public bool IsPaused { get; set; }

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x0600102D RID: 4141 RVA: 0x00036253 File Offset: 0x00034453
		// (set) Token: 0x0600102E RID: 4142 RVA: 0x0003625B File Offset: 0x0003445B
		[Dependency]
		public IScope Scope { get; private set; }

		// Token: 0x0600102F RID: 4143 RVA: 0x00036264 File Offset: 0x00034464
		public bool Step()
		{
			this.ClearGraveyard();
			int commandIndex = 0;
			while (commandIndex < this._commands.Count && this._commands[commandIndex].FrameIndex <= this._clock.FrameCount)
			{
				Command command = this._commands[commandIndex];
				Simulation.Log.Info("Executing {0} on frame {1}.", new object[]
				{
					command,
					this._clock.FrameCount
				});
				command.FrameIndex = this._clock.FrameCount;
				command.Execute(this);
				if (this._isRecordingSimulationCommands)
				{
					this._journal.Record(command);
				}
				commandIndex++;
			}
			if (commandIndex > 0)
			{
				this._commands.RemoveRange(0, commandIndex);
			}
			Fix64 timestep = (!this.IsPaused) ? this.Timestep : Fix64.Zero;
			this._clock.Step(timestep);
			this.ClearGraveyard();
			foreach (IProcess process in this._processes)
			{
				process.Step(this, timestep);
			}
			return true;
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x00036394 File Offset: 0x00034594
		public bool AddModel(IModel model)
		{
			Type modelType = model.GetType();
			if (!this._models.ContainsKey(modelType))
			{
				this._models[modelType] = new List<IModel>();
			}
			this._models[modelType].Add(model);
			foreach (ISimulationObserver simulationObserver in this._observers)
			{
				simulationObserver.OnModelAdded(this, model, this._clock.Time);
			}
			return true;
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x0003640C File Offset: 0x0003460C
		public bool RemoveModel(IModel model)
		{
			this._graveyard.Add(model);
			foreach (ISimulationObserver simulationObserver in this._observers)
			{
				simulationObserver.OnModelRemoved(this, model, this._clock.Time);
			}
			return true;
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x00036458 File Offset: 0x00034658
		public bool ContainsModel(IModel model)
		{
			List<IModel> models;
			return this._models.TryGetValue(model.GetType(), out models) && models.Contains(model);
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x00036484 File Offset: 0x00034684
		public T GetModel<T>() where T : class, IModel
		{
			ModelListEnumerator<T> modelEnumerator = this.GetModels<T>().GetEnumerator();
			if (modelEnumerator.MoveNext())
			{
				return modelEnumerator.Current;
			}
			return default(T);
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x000364BC File Offset: 0x000346BC
		public ModelList<T> GetModels<T>() where T : class, IModel
		{
			List<IModel> modelsOfType = null;
			Type modelType = typeof(T);
			if (this._models.ContainsKey(modelType))
			{
				modelsOfType = this._models[modelType];
			}
			return new ModelList<T>(modelsOfType);
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x000364F7 File Offset: 0x000346F7
		public bool AddProcess(IProcess process)
		{
			this._processes.Add(process);
			return true;
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x00036508 File Offset: 0x00034708
		public bool ScheduleCommand(Command command)
		{
			if (this._commands.Count == 0)
			{
				this._commands.Add(command);
				return true;
			}
			int commandIndex = this._commands.Count - 1;
			while (commandIndex > 1 && command.FrameIndex < this._commands[commandIndex].FrameIndex)
			{
				commandIndex--;
			}
			this._commands.Insert(commandIndex + 1, command);
			return true;
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x06001037 RID: 4151 RVA: 0x00036571 File Offset: 0x00034771
		public bool HasAnyScheduledCommands
		{
			get
			{
				return this._commands.Count > 0;
			}
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06001038 RID: 4152 RVA: 0x00036581 File Offset: 0x00034781
		public Command NextScheduledCommand
		{
			get
			{
				if (this._commands.Count > 0)
				{
					return this._commands[0];
				}
				return null;
			}
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x000365A0 File Offset: 0x000347A0
		public void Subscribe(ISimulationObserver observer)
		{
			this._observers.Subscribe(observer);
			foreach (List<IModel> list in this._models.Values)
			{
				foreach (IModel model in list)
				{
					observer.OnModelAdded(this, model, this._clock.Time);
				}
			}
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x00036644 File Offset: 0x00034844
		public bool Unsubscribe(ISimulationObserver observer)
		{
			return this._observers.Unsubscribe(observer);
		}

		// Token: 0x0600103B RID: 4155 RVA: 0x00036652 File Offset: 0x00034852
		public void OnCreatedInScope(IScope scope)
		{
			this._isRecordingSimulationCommands = false;
			if (FeatureToggle.IsFeatureEnabled(Feature.RecordSimulationJournal))
			{
				this._isRecordingSimulationCommands = true;
			}
		}

		// Token: 0x0600103C RID: 4156 RVA: 0x0003666C File Offset: 0x0003486C
		public void OnReleasedFromScope(IScope scope)
		{
			this._observers.UnsubscribeAll();
			this.ClearGraveyard();
			if (this._models != null)
			{
				foreach (List<IModel> models in this._models.Values)
				{
					int modelIndex = models.Count - 1;
					while (modelIndex >= 0)
					{
						IModel modelToRelease = models[modelIndex];
						models.RemoveAt(modelIndex);
						modelIndex--;
						scope.Release(modelToRelease);
					}
				}
			}
			if (this._commands != null)
			{
				foreach (Command command in this._commands)
				{
					scope.Release(command);
				}
				this._commands.Clear();
			}
		}

		// Token: 0x0600103D RID: 4157 RVA: 0x0003675C File Offset: 0x0003495C
		public void Reset()
		{
			this.Timestep = Simulation.DefaultTimestep;
			this.IsPaused = false;
			this._models.Clear();
			this._graveyard.Clear();
			this._processes.Clear();
			this._commands.Clear();
		}

		// Token: 0x0600103E RID: 4158 RVA: 0x0003679C File Offset: 0x0003499C
		private void ClearGraveyard()
		{
			if (this._graveyard.Count > 0)
			{
				int lastModelIndex = this._graveyard.Count - 1;
				while (lastModelIndex >= 0)
				{
					IModel deadModel = this._graveyard[lastModelIndex];
					this._graveyard.RemoveAt(lastModelIndex);
					lastModelIndex--;
					List<IModel> models;
					if (Diagnostics.Verify(this._models.TryGetValue(deadModel.GetType(), out models)))
					{
						models.Remove(deadModel);
					}
					this.Scope.Release(deadModel);
				}
			}
		}

		// Token: 0x04000E4A RID: 3658
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Server.Simulation");

		// Token: 0x04000E4D RID: 3661
		public static readonly Fix64 DefaultTimestep = Fix64.One / (Fix64)10L;

		// Token: 0x04000E4E RID: 3662
		private readonly Dictionary<Type, List<IModel>> _models = new Dictionary<Type, List<IModel>>();

		// Token: 0x04000E4F RID: 3663
		[Serialize(false, null)]
		private readonly List<IModel> _graveyard = new List<IModel>();

		// Token: 0x04000E50 RID: 3664
		private readonly List<IProcess> _processes = new List<IProcess>();

		// Token: 0x04000E51 RID: 3665
		private readonly List<Command> _commands = new List<Command>();

		// Token: 0x04000E52 RID: 3666
		[Serialize(false, null)]
		private readonly ObserverList<ISimulationObserver> _observers = new ObserverList<ISimulationObserver>(1);

		// Token: 0x04000E53 RID: 3667
		[Dependency]
		[Serialize(true, null)]
		private CommandJournal _journal;

		// Token: 0x04000E54 RID: 3668
		private bool _isRecordingSimulationCommands;

		// Token: 0x04000E55 RID: 3669
		[Dependency]
		[Serialize(true, null)]
		private Clock _clock;
	}
}
