using System;
using Factory;

namespace Server
{
	// Token: 0x02000293 RID: 659
	public class SetPausedCommand : Command
	{
		// Token: 0x06001025 RID: 4133 RVA: 0x000361D1 File Offset: 0x000343D1
		public override void Execute(ISimulation simulation)
		{
			Command.Log.Info("Executing SetPauseCommand to {0}.", new object[]
			{
				this._pause ? "pause" : "resume"
			});
			simulation.IsPaused = this._pause;
		}

		// Token: 0x06001026 RID: 4134 RVA: 0x0003620B File Offset: 0x0003440B
		public override void Reset()
		{
			base.Reset();
			this._pause = false;
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x0003621A File Offset: 0x0003441A
		public static SetPausedCommand Create(IScope scope, bool pause)
		{
			SetPausedCommand setPausedCommand = scope.Get<SetPausedCommand>();
			setPausedCommand._pause = pause;
			return setPausedCommand;
		}

		// Token: 0x04000E49 RID: 3657
		private bool _pause;
	}
}
