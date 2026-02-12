using System;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Models;
using Motorways.Processes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005B8 RID: 1464
	public class TutorialDebugView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x060028BF RID: 10431 RVA: 0x000ADCE6 File Offset: 0x000ABEE6
		private bool ShouldShowView
		{
			get
			{
				return FeatureToggle.IsFeatureEnabled(Feature.TutorialView);
			}
		}

		// Token: 0x060028C0 RID: 10432 RVA: 0x000ADCF4 File Offset: 0x000ABEF4
		private void OnEnable()
		{
			this._style.fontSize = 18;
			this._style.alignment = TextAnchor.MiddleLeft;
			this._style.richText = true;
			this._style.normal.textColor = Color.white;
			this._style.normal.background = DebugViewUtils.DebugWindowBackground;
			this._style.padding = new RectOffset(10, 10, 10, 10);
		}

		// Token: 0x060028C1 RID: 10433 RVA: 0x000ADD68 File Offset: 0x000ABF68
		private void AppendStageList(ref string text)
		{
			int num = this._tutorialProgressionProcess.CurrentStepIndex - 2;
			int endIndex = this._tutorialProgressionProcess.CurrentStepIndex + 2;
			for (int stageIndex = num; stageIndex <= endIndex; stageIndex++)
			{
				if (stageIndex >= 0 && stageIndex < this._tutorialProgressionProcess.StageCount)
				{
					TutorialProgressionProcess.TutorialStep step = this._tutorialProgressionProcess.StageAt(stageIndex);
					string color = (stageIndex == this._tutorialProgressionProcess.CurrentStepIndex) ? "yellow" : "silver";
					text += string.Format("<color={0}>{1}: ({2}) {3}</color>", new object[]
					{
						color,
						stageIndex,
						step.StageShortName,
						step.Id
					});
				}
				else
				{
					text += string.Format("<color=silver>{0}: _______________</color>", stageIndex);
				}
				text += "\n";
			}
		}

		// Token: 0x060028C2 RID: 10434 RVA: 0x000ADE3C File Offset: 0x000AC03C
		private void AppendTutorialInfo(ref string text, TutorialProgressionProcess.TutorialStep currentStep)
		{
			text += "\n<size=20>Info</size><size=18>\n";
			string text2;
			if (currentStep != null)
			{
				Func<string> debugText2 = currentStep.DebugText;
				text2 = ((debugText2 != null) ? debugText2() : null);
			}
			else
			{
				text2 = "No more steps";
			}
			string debugText = text2;
			text += (debugText ?? "Not debug text set for current step.");
			text += string.Format("\nClock Ticking: {0} | h:{1}, d:{2}, w:{3}, t:({4:F1})", new object[]
			{
				this.ColorBoolean(currentStep == null || currentStep.DoesClockTick()),
				this._clockModel.Hour,
				this._clockModel.Day,
				this._clockModel.Week,
				(float)this._clockModel.Time
			});
			text = string.Concat(new string[]
			{
				text,
				"\nGameplay Input Blocked: ",
				this.ColorBoolean(this._tutorialProgressionProcess.IsInputBlocked),
				" | Has Mothballed Road: ",
				this.ColorBoolean(this._tutorialProgressionProcess.HasPlayerMothballedARoad),
				"</size>"
			});
		}

		// Token: 0x060028C3 RID: 10435 RVA: 0x000ADF60 File Offset: 0x000AC160
		private string ColorBoolean(bool value)
		{
			string color = value ? "lime" : "red";
			return string.Format("<color={0}>{1}</color>", color, value);
		}

		// Token: 0x060028C4 RID: 10436 RVA: 0x000ADF90 File Offset: 0x000AC190
		private Rect CalculateRectSize(string text)
		{
			GUIContent content = new GUIContent(text);
			Vector2 contentSize = this._style.CalcSize(content);
			return new Rect(10f, (float)Screen.height - contentSize.y - 10f, contentSize.x, contentSize.y);
		}

		// Token: 0x060028C5 RID: 10437 RVA: 0x000020AA File Offset: 0x000002AA
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			return TickResult.StopTicking;
		}

		// Token: 0x060028C6 RID: 10438 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x060028C7 RID: 10439 RVA: 0x000ADFDA File Offset: 0x000AC1DA
		public void Reset()
		{
			this._style = new GUIStyle();
		}

		// Token: 0x04002274 RID: 8820
		[Dependency]
		private TutorialProgressionProcess _tutorialProgressionProcess;

		// Token: 0x04002275 RID: 8821
		[Dependency]
		private ClockModel _clockModel;

		// Token: 0x04002276 RID: 8822
		[Dependency]
		private City _city;

		// Token: 0x04002277 RID: 8823
		private GUIStyle _style = new GUIStyle();

		// Token: 0x04002278 RID: 8824
		public const string ShouldShowTutorialDebugView = "ShouldShowTutorialDebugView";

		// Token: 0x04002279 RID: 8825
		private const int Padding = 10;

		// Token: 0x0400227A RID: 8826
		private const int Margins = 10;

		// Token: 0x0400227B RID: 8827
		private bool _isCollapsed = true;

		// Token: 0x0400227C RID: 8828
		private const int PreviousStageCount = 2;

		// Token: 0x0400227D RID: 8829
		private const int NextStageCount = 2;
	}
}
