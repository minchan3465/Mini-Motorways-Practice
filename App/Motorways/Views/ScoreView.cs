using System;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Motorways.Themes;
using Motorways.UI;
using Server;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x020005F9 RID: 1529
	public class ScoreView : MonoBehaviour, IView, IReusable, IThemeComponent
	{
		// Token: 0x14000047 RID: 71
		// (add) Token: 0x06002A7C RID: 10876 RVA: 0x000BA50C File Offset: 0x000B870C
		// (remove) Token: 0x06002A7D RID: 10877 RVA: 0x000BA544 File Offset: 0x000B8744
		public event Action OnElectiveUpgradeButtonPressed;

		// Token: 0x14000048 RID: 72
		// (add) Token: 0x06002A7E RID: 10878 RVA: 0x000BA57C File Offset: 0x000B877C
		// (remove) Token: 0x06002A7F RID: 10879 RVA: 0x000BA5B4 File Offset: 0x000B87B4
		public event Action OnScoreButtonPressed;

		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x06002A80 RID: 10880 RVA: 0x000BA5E9 File Offset: 0x000B87E9
		// (set) Token: 0x06002A81 RID: 10881 RVA: 0x000BA5F1 File Offset: 0x000B87F1
		public ScoreModel ScoreModel
		{
			get
			{
				return this._scoreModel;
			}
			set
			{
				this._scoreModel = value;
			}
		}

		// Token: 0x06002A82 RID: 10882 RVA: 0x000BA5FA File Offset: 0x000B87FA
		private void Initialize(ScoreModel scoreModel)
		{
			this._scoreModel = scoreModel;
			this.SetupView();
		}

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x06002A83 RID: 10883 RVA: 0x000BA609 File Offset: 0x000B8809
		public FloatingElement FloatingElement
		{
			get
			{
				return this._floatingElement;
			}
		}

		// Token: 0x06002A84 RID: 10884 RVA: 0x000BA614 File Offset: 0x000B8814
		public void SetupView()
		{
			this.electiveUpgradeTicker.SetActive(this._city.Rules.ScoringMode == ScoringMode.EfficiencyMilestones);
			this.scoreButton.gameObject.SetActive(this._city.Rules.ScoringMode == ScoringMode.Trips);
			if (this._city.Rules.ScoringMode == ScoringMode.EfficiencyMilestones || this._city.Rules.ScoringMode == ScoringMode.None)
			{
				this.scoreText.TextField.text = "";
			}
		}

		// Token: 0x06002A85 RID: 10885 RVA: 0x000BA6A0 File Offset: 0x000B88A0
		public void Reset()
		{
			this._scoreModel = null;
			this._displayedScore = -1;
			this._innerDesiredEndlessProgress = 0f;
			this._outerDesiredEndlessProgress = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
		}

		// Token: 0x06002A86 RID: 10886 RVA: 0x000BA704 File Offset: 0x000B8904
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._city.Rules.ScoringMode == ScoringMode.Trips && this._scoreModel.Score != this._displayedScore)
			{
				this._displayedScore = this._scoreModel.Score;
				this.scoreText.LocString = StandaloneLocString.CreateLocalizedNumberString(this._scope, this._displayedScore);
			}
			if (this._city.Rules.ScoringMode == ScoringMode.EfficiencyMilestones)
			{
				float score = (float)this._scoreModel.EfficiencyScore;
				float goal = (float)this._city.Definition.GetEfficiencyMilestone(this._scoreModel.CurrentEfficiencyMilestone, this._simulationConstantsData.MilestoneIncreaseAfterPrecalculatedIntervals);
				float tickerProgress = score / goal;
				if (this._upgradeDatabaseModel.HasPendingUpgrades)
				{
					tickerProgress = 1f;
				}
				if (tickerProgress - this._innerDesiredEndlessProgress > 0f)
				{
					float innerIncrease = this._innerTickerSpeed * timeInterval.ScaledDelta;
					if (this._outerDesiredEndlessProgress >= 1f)
					{
						innerIncrease *= this._tickerCompleteSpeedMultiplier;
					}
					this._innerDesiredEndlessProgress += innerIncrease;
				}
				else
				{
					this._innerDesiredEndlessProgress = tickerProgress;
				}
				if (tickerProgress - this._outerDesiredEndlessProgress > 0f)
				{
					this._outerDesiredEndlessProgress += this._outerTickerSpeed * timeInterval.ScaledDelta;
				}
				else
				{
					this._outerDesiredEndlessProgress = tickerProgress;
				}
				float innerProgress = (this._innerDesiredEndlessProgress >= 1f) ? 1.1f : this._innerDesiredEndlessProgress;
				float outerProgress = (this._outerDesiredEndlessProgress >= 1f) ? 1.1f : this._outerDesiredEndlessProgress;
				this.tickerMeshRenderer.material.SetFloat(ScoreView.InnerProgress, innerProgress);
				this.tickerMeshRenderer.material.SetFloat(ScoreView.OuterProgress, outerProgress);
				if (this.IsEfficiencyTickerVisuallyComplete && !this._player.HasSeenNewContent("EndlessMilestoneFTUXMessage"))
				{
					this._notificationView.AddNotification(StringId.FTUX_Endless, 0f, () => this._player.HasSeenNewContent("EndlessMilestoneFTUXMessage") || this._scope.Get<ScreenStack>().GetTopActiveScreenType() != ScreenStack.MotorwaysScreen.InGame);
				}
				if (FeatureToggle.IsFeatureEnabled(Feature.EndlessEfficiencyText))
				{
					this.scoreText.TextField.text = string.Format("{0:F2} / {1}", score, goal);
				}
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002A87 RID: 10887 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x06002A88 RID: 10888 RVA: 0x000BA924 File Offset: 0x000B8B24
		public bool IsEfficiencyTickerVisuallyComplete
		{
			get
			{
				return this._innerDesiredEndlessProgress >= 1f;
			}
		}

		// Token: 0x06002A89 RID: 10889 RVA: 0x000BA936 File Offset: 0x000B8B36
		public void SetEfficiencyTickerAnimationsPaused(bool isPaused)
		{
			this.electiveUpgradeAnimator.speed = (float)(isPaused ? 0 : 1);
		}

		// Token: 0x06002A8A RID: 10890 RVA: 0x000022F5 File Offset: 0x000004F5
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x06002A8B RID: 10891 RVA: 0x000BA94C File Offset: 0x000B8B4C
		public void ApplyTheme(ITheme theme)
		{
			Theme motorwaysTheme = theme as Theme;
			if (motorwaysTheme != null)
			{
				this.tickerMeshRenderer.material.SetColor(ScoreView.OuterColor, motorwaysTheme.GetColor(this._tickerOuterColorType, "_Color"));
				this.tickerMeshRenderer.material.SetColor(ScoreView.InnerColor, motorwaysTheme.GetColor(this._tickerInnerColorType, "_Color"));
			}
		}

		// Token: 0x06002A8C RID: 10892 RVA: 0x000BA9B0 File Offset: 0x000B8BB0
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			Theme theme = oldTheme as Theme;
			Theme newMotorwaysTheme = oldTheme as Theme;
			Color oldColor = theme.GetColor(this._tickerOuterColorType, "_Color");
			Color newColor = newMotorwaysTheme.GetColor(this._tickerOuterColorType, "_Color");
			this.tickerMeshRenderer.material.SetColor(ScoreView.OuterColor, Color.Lerp(oldColor, newColor, progress));
			oldColor = theme.GetColor(this._tickerInnerColorType, "_Color");
			newColor = newMotorwaysTheme.GetColor(this._tickerInnerColorType, "_Color");
			this.tickerMeshRenderer.material.SetColor(ScoreView.InnerColor, Color.Lerp(oldColor, newColor, progress));
			if (!(oldColor == newColor))
			{
				return ThemeBlendingResult.ContinueBlending;
			}
			return ThemeBlendingResult.StopBlending;
		}

		// Token: 0x06002A8D RID: 10893 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x06002A8E RID: 10894 RVA: 0x000BAA57 File Offset: 0x000B8C57
		public void ElectiveUpgradeButtonPressed()
		{
			Action onElectiveUpgradeButtonPressed = this.OnElectiveUpgradeButtonPressed;
			if (onElectiveUpgradeButtonPressed == null)
			{
				return;
			}
			onElectiveUpgradeButtonPressed();
		}

		// Token: 0x06002A8F RID: 10895 RVA: 0x000BAA69 File Offset: 0x000B8C69
		public void ScoreButtonPressed()
		{
			Action onScoreButtonPressed = this.OnScoreButtonPressed;
			if (onScoreButtonPressed == null)
			{
				return;
			}
			onScoreButtonPressed();
		}

		// Token: 0x040024AB RID: 9387
		[Dependency]
		private IScope _scope;

		// Token: 0x040024AC RID: 9388
		[Dependency]
		private City _city;

		// Token: 0x040024AD RID: 9389
		[Dependency]
		private SimulationConstantsData _simulationConstantsData;

		// Token: 0x040024AE RID: 9390
		[Dependency]
		private GameUIScreen _gameUIScreen;

		// Token: 0x040024AF RID: 9391
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x040024B0 RID: 9392
		[Dependency]
		private NotificationView _notificationView;

		// Token: 0x040024B1 RID: 9393
		[Dependency]
		private UpgradeDatabaseModel _upgradeDatabaseModel;

		// Token: 0x040024B2 RID: 9394
		public LocalizedTextUI scoreText;

		// Token: 0x040024B3 RID: 9395
		private ScoreModel _scoreModel;

		// Token: 0x040024B4 RID: 9396
		public const string EndlessMilestoneNci = "EndlessMilestoneFTUXMessage";

		// Token: 0x040024B7 RID: 9399
		private int _displayedScore = -1;

		// Token: 0x040024B8 RID: 9400
		private float _innerDesiredEndlessProgress;

		// Token: 0x040024B9 RID: 9401
		private float _outerDesiredEndlessProgress;

		// Token: 0x040024BA RID: 9402
		public TouchButton scoreButton;

		// Token: 0x040024BB RID: 9403
		public GameObject electiveUpgradeTicker;

		// Token: 0x040024BC RID: 9404
		public Animator electiveUpgradeAnimator;

		// Token: 0x040024BD RID: 9405
		public Image tickerMeshRenderer;

		// Token: 0x040024BE RID: 9406
		private static readonly int InnerProgress = Shader.PropertyToID("_InnerProgress");

		// Token: 0x040024BF RID: 9407
		private static readonly int OuterProgress = Shader.PropertyToID("_OuterProgress");

		// Token: 0x040024C0 RID: 9408
		private static readonly int OuterColor = Shader.PropertyToID("_OuterColor");

		// Token: 0x040024C1 RID: 9409
		private static readonly int InnerColor = Shader.PropertyToID("_InnerColor");

		// Token: 0x040024C2 RID: 9410
		public static readonly int UpgradeAvailableId = Animator.StringToHash("UpgradeAvailable");

		// Token: 0x040024C3 RID: 9411
		public static readonly int PlayerInterruptedId = Animator.StringToHash("PlayerInterrupted");

		// Token: 0x040024C4 RID: 9412
		[SerializeField]
		private ThemedMaterialType _tickerInnerColorType = ThemedMaterialType.DarkSecondary;

		// Token: 0x040024C5 RID: 9413
		[SerializeField]
		private ThemedMaterialType _tickerOuterColorType = ThemedMaterialType.Grey;

		// Token: 0x040024C6 RID: 9414
		[SerializeField]
		private float _innerTickerSpeed = 1f;

		// Token: 0x040024C7 RID: 9415
		[Tooltip("How much faster does the inner ticker go when the outer is complete?")]
		[SerializeField]
		private float _tickerCompleteSpeedMultiplier = 30f;

		// Token: 0x040024C8 RID: 9416
		[SerializeField]
		private float _outerTickerSpeed = 1f;

		// Token: 0x040024C9 RID: 9417
		[SerializeField]
		private FloatingElement _floatingElement;

		// Token: 0x020005FA RID: 1530
		public class Builder : IViewBuilder
		{
			// Token: 0x06002A93 RID: 10899 RVA: 0x000BAB50 File Offset: 0x000B8D50
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				ScoreView scoreView = client.Scope.Get<ScoreView>();
				scoreView.Initialize(model as ScoreModel);
				client.AddView(scoreView);
			}
		}
	}
}
