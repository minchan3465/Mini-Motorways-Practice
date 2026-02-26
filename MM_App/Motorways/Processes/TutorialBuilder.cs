using System;
using System.Collections.Generic;

namespace Motorways.Processes
{
	// Token: 0x020004B3 RID: 1203
	public class TutorialBuilder
	{
		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x06001F77 RID: 8055 RVA: 0x0007B216 File Offset: 0x00079416
		public IReadOnlyList<TutorialProgressionProcess.TutorialStep> Steps
		{
			get
			{
				return this._steps;
			}
		}

		// Token: 0x06001F78 RID: 8056 RVA: 0x0007B21E File Offset: 0x0007941E
		public TutorialBuilder(TutorialProgressionProcess progressionProcess)
		{
			this._progressionProcess = progressionProcess;
		}

		// Token: 0x06001F79 RID: 8057 RVA: 0x0007B238 File Offset: 0x00079438
		public void StartStage(string name, string shortName)
		{
			this._progressionProcess.SetCurrentStage(name, shortName);
			this.AddStep(new TutorialProgressionProcess.TutorialStep("Start Stage: " + name, null).ClockTicksWhile(() => false).WhenStepStarts(delegate()
			{
				this._progressionProcess.SetCurrentStage(name, shortName);
			}).StepOverWhen(() => true));
		}

		// Token: 0x06001F7A RID: 8058 RVA: 0x0007B2EC File Offset: 0x000794EC
		public void AddStep(TutorialProgressionProcess.TutorialStep tutorialStep)
		{
			tutorialStep.StageShortName = this._progressionProcess.CurrentStageShortName;
			this._steps.Add(tutorialStep);
		}

		// Token: 0x06001F7B RID: 8059 RVA: 0x0007B30C File Offset: 0x0007950C
		public void AddMarker(TutorialProgressionProcess.TutorialMarker marker)
		{
			this.AddStep(new TutorialProgressionProcess.TutorialStep("Marker : " + marker.ToString(), null).WhenStepStarts(delegate()
			{
				this._progressionProcess.SetLastReachedMarker(marker);
			}).StepOverWhen(() => true));
		}

		// Token: 0x06001F7C RID: 8060 RVA: 0x0007B38C File Offset: 0x0007958C
		public void AddRealtimeDelay(float delay, bool clockTicks)
		{
			this.AddStep(new TutorialProgressionProcess.TutorialStep(delay.ToString() + " second delay", null).WhenStepStarts(delegate()
			{
				this._progressionProcess.StartRealtimeTimer(delay);
			}).StepOverWhen(new Func<bool>(this._progressionProcess.RealtimeTimerFinished)).ClockTicksWhile(() => clockTicks));
		}

		// Token: 0x04001A05 RID: 6661
		private readonly List<TutorialProgressionProcess.TutorialStep> _steps = new List<TutorialProgressionProcess.TutorialStep>();

		// Token: 0x04001A06 RID: 6662
		private readonly TutorialProgressionProcess _progressionProcess;
	}
}
