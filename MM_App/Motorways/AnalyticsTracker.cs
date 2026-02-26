using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Factory;
using Motorways.Models;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200033A RID: 826
	public class AnalyticsTracker : MonoBehaviour, ICreatedInScopeHandler
	{
		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x0600146B RID: 5227 RVA: 0x0004245A File Offset: 0x0004065A
		private bool Initialized
		{
			get
			{
				return this._currentState == AnalyticsTracker.State.Initialized;
			}
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x00042468 File Offset: 0x00040668
		private void StartAnalytics()
		{
			AnalyticsTracker.<StartAnalytics>d__29 <StartAnalytics>d__;
			<StartAnalytics>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<StartAnalytics>d__.<>4__this = this;
			<StartAnalytics>d__.<>1__state = -1;
			<StartAnalytics>d__.<>t__builder.Start<AnalyticsTracker.<StartAnalytics>d__29>(ref <StartAnalytics>d__);
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x000424A0 File Offset: 0x000406A0
		public void TrackTutorialSkipped(int currentTutorialStage)
		{
			if (!this.Initialized)
			{
				return;
			}
			this._parameters.Clear();
			this._parameters.Add("tutorialStage", currentTutorialStage);
			UnityAnalyticsBridge.CustomEvent("tutorialSkipped", this._parameters);
			AnalyticsTracker.Log.Info("Sent tutorial skipped message. Payload Entries: {0}", new object[]
			{
				this._parameters.Keys.Count
			});
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x00042514 File Offset: 0x00040714
		public void TrackTutorialStarted(bool voluntaryStart)
		{
			if (!this.Initialized)
			{
				return;
			}
			this._parameters.Clear();
			this._parameters.Add("voluntaryStart", voluntaryStart);
			UnityAnalyticsBridge.CustomEvent("tutorialStarted", this._parameters);
			AnalyticsTracker.Log.Info("Sent tutorial start message. Payload Entries: {0}", new object[]
			{
				this._parameters.Keys.Count
			});
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x00042588 File Offset: 0x00040788
		public void TrackTutorialStage(int stageIndex)
		{
			if (!this.Initialized)
			{
				return;
			}
			this._parameters.Clear();
			this._parameters.Add("tutorialStage", stageIndex);
			this._parameters.Add("gameVersion", AnalyticsTracker.GetVersionNumber());
			UnityAnalyticsBridge.CustomEvent("tutorialStage", this._parameters);
			AnalyticsTracker.Log.Info("Sent tutorial stage message. Payload Entries: {0}", new object[]
			{
				this._parameters.Keys.Count
			});
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x00042614 File Offset: 0x00040814
		public void TrackTutorialFinished()
		{
			if (!this.Initialized)
			{
				return;
			}
			this._parameters.Clear();
			this._parameters.Add("gameVersion", AnalyticsTracker.GetVersionNumber());
			UnityAnalyticsBridge.CustomEvent("tutorialFinished", this._parameters);
			AnalyticsTracker.Log.Info("Sent tutorial complete message. Payload Entries: {0}", new object[]
			{
				this._parameters.Keys.Count
			});
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x00042688 File Offset: 0x00040888
		public void TrackScreenEntered(ScreenStack.MotorwaysScreen screenType)
		{
			if (!this.Initialized)
			{
				return;
			}
			if (screenType == ScreenStack.MotorwaysScreen.None || screenType == ScreenStack.MotorwaysScreen.Startup || screenType == ScreenStack.MotorwaysScreen.InGame || screenType == ScreenStack.MotorwaysScreen.Upgrade)
			{
				return;
			}
			this._parameters.Clear();
			this._parameters.Add("gameVersion", AnalyticsTracker.GetVersionNumber());
			this._parameters.Add("screenName", screenType.ToString());
			UnityAnalyticsBridge.CustomEvent("screenVisited", this._parameters);
			AnalyticsTracker.Log.Info("Sent screen visit {1}. Payload Entries: {0}", new object[]
			{
				this._parameters.Keys.Count,
				screenType
			});
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x00042734 File Offset: 0x00040934
		public void TrackUpgradeChoice(UpgradeType chosenType, List<UpgradeType> otherTypes, MotorwaysGame game)
		{
			if (!this.Initialized)
			{
				return;
			}
			this._parameters.Clear();
			this._parameters.Add("gameVersion", AnalyticsTracker.GetVersionNumber());
			this._parameters.Add("cityName", game.MapDefinition.cityName);
			this._parameters.Add("cityTime", AnalyticsTracker.GetTimeParameter(game));
			this._parameters.Add("cityMode", AnalyticsTracker.GetGameModeParameter(game));
			this._parameters.Add("selectedUpgrade", chosenType.ToString());
			if (otherTypes.Count == 1)
			{
				this._parameters.Add("otherUpgrade", otherTypes[0]);
			}
			UnityAnalyticsBridge.CustomEvent("upgradeChoice", this._parameters);
			AnalyticsTracker.Log.Info("Sent upgrade choice message. Payload Entries: {0}", new object[]
			{
				this._parameters.Keys.Count
			});
		}

		// Token: 0x06001473 RID: 5235 RVA: 0x00042838 File Offset: 0x00040A38
		public void TrackGameStarted(MapDefinition mapDefinition, MapChallenge.ChallengeType challengeType, int challengeIndex, GameMode mode, MotorwaysThemePreference theme)
		{
			if (!this.Initialized)
			{
				return;
			}
			this._parameters.Clear();
			this._parameters.Add("gameVersion", AnalyticsTracker.GetVersionNumber());
			this._parameters.Add("cityName", mapDefinition.cityName);
			this._parameters.Add("cityMode", AnalyticsTracker.GetGameModeParameter(challengeIndex, challengeType, mode));
			this._parameters.Add("gameTheme", theme.ToString());
			UnityAnalyticsBridge.CustomEvent("cityStarted", this._parameters);
			AnalyticsTracker.Log.Info("Sent game started message. Payload Entries: {0}", new object[]
			{
				this._parameters.Keys.Count
			});
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x000428F8 File Offset: 0x00040AF8
		public void TrackGameResumed(MotorwaysGame game, MapDefinition mapDefinition)
		{
			if (!this.Initialized)
			{
				return;
			}
			this._parameters.Clear();
			this._parameters.Add("gameVersion", AnalyticsTracker.GetVersionNumber());
			this._parameters.Add("cityName", mapDefinition.cityName);
			this._parameters.Add("cityMode", AnalyticsTracker.GetGameModeParameter(game));
			this._parameters.Add("cityScore", game.Scope.Get<ScoreModel>().Score);
			this._parameters.Add("cityTime", AnalyticsTracker.GetTimeParameter(game));
			UnityAnalyticsBridge.CustomEvent("cityResumed", this._parameters);
			AnalyticsTracker.Log.Info("Sent game resumed message. Payload Entries: {0}", new object[]
			{
				this._parameters.Keys.Count
			});
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x000429D8 File Offset: 0x00040BD8
		public void TrackGameContinued(MotorwaysGame game, MapDefinition mapDefinition)
		{
			if (!this.Initialized)
			{
				return;
			}
			this._parameters.Clear();
			this._parameters.Add("gameVersion", AnalyticsTracker.GetVersionNumber());
			this._parameters.Add("cityName", mapDefinition.cityName);
			this._parameters.Add("cityMode", AnalyticsTracker.GetGameModeParameter(game));
			this._parameters.Add("cityScore", game.Scope.Get<ScoreModel>().CurrentEfficiencyMilestone);
			this._parameters.Add("cityTime", AnalyticsTracker.GetTimeParameter(game));
			UnityAnalyticsBridge.CustomEvent("cityContinued", this._parameters);
			AnalyticsTracker.Log.Info("Sent game continued message. Payload Entries: {0}", new object[]
			{
				this._parameters.Keys.Count
			});
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x00042AB8 File Offset: 0x00040CB8
		public void TrackGameComplete(MotorwaysGame game)
		{
			if (!this.Initialized)
			{
				return;
			}
			this._parameters.Clear();
			this._parameters.Add("gameVersion", AnalyticsTracker.GetVersionNumber());
			this._parameters.Add("cityName", game.MapDefinition.cityName);
			this._parameters.Add("cityMode", AnalyticsTracker.GetGameModeParameter(game));
			this._parameters.Add("cityScore", game.Scope.Get<ScoreModel>().Score);
			this._parameters.Add("cityTime", AnalyticsTracker.GetTimeParameter(game));
			this._parameters.Add("destinationCount", game.Simulation.GetModels<DestinationModel>().Count);
			UpgradeDatabaseModel upgrades = game.Simulation.GetModel<UpgradeDatabaseModel>();
			this.AddPercentageOfUpgradeRemainingIfApplicable(upgrades, UpgradeType.Concrete);
			this.AddPercentageOfUpgradeRemainingIfApplicable(upgrades, UpgradeType.Bridge);
			this.AddPercentageOfUpgradeRemainingIfApplicable(upgrades, UpgradeType.Tunnel);
			this.AddPercentageOfUpgradeRemainingIfApplicable(upgrades, UpgradeType.TrafficLight);
			this.AddPercentageOfUpgradeRemainingIfApplicable(upgrades, UpgradeType.Roundabout);
			this.AddPercentageOfUpgradeRemainingIfApplicable(upgrades, UpgradeType.Motorway);
			UnityAnalyticsBridge.CustomEvent("cityOver", this._parameters);
			AnalyticsTracker.Log.Info("Sent game over message. Payload Entries: {0}", new object[]
			{
				this._parameters.Keys.Count
			});
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x00042C00 File Offset: 0x00040E00
		private void AddPercentageOfUpgradeRemainingIfApplicable(UpgradeDatabaseModel upgrades, UpgradeType type)
		{
			int totalUpgrades = upgrades.GetTotalUpgradeCount(type);
			if (totalUpgrades == 0)
			{
				return;
			}
			string percentageString = ((float)upgrades.GetAvailableUpgradeCount(type) / (float)totalUpgrades).ToString("F3", CultureInfo.InvariantCulture);
			this._parameters.Add(AnalyticsTracker.GetUpgradePercentageParameterName(type), percentageString);
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x00042C49 File Offset: 0x00040E49
		private static string GetUpgradePercentageParameterName(UpgradeType type)
		{
			return type.ToString().ToLower() + "Remaining";
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x00042C67 File Offset: 0x00040E67
		private static int GetTimeParameter(MotorwaysGame game)
		{
			return (int)((long)game.Scope.Get<ClockModel>().Time);
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x00042C80 File Offset: 0x00040E80
		private static string GetGameModeParameter(int challengeIndex, MapChallenge.ChallengeType challengeType, GameMode mode)
		{
			if (challengeIndex != -1)
			{
				return string.Format("{0}_challenge_{1}", mode.ToString().ToLower(), challengeIndex);
			}
			if (challengeType != MapChallenge.ChallengeType.None)
			{
				return challengeType.ToString().ToLower();
			}
			return mode.ToString().ToLower();
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x00042CDC File Offset: 0x00040EDC
		private static string GetGameModeParameter(MotorwaysGame game)
		{
			ActiveChallengesModel challenges = game.Simulation.GetModel<ActiveChallengesModel>();
			CityModel cityModel = game.Simulation.GetModel<CityModel>();
			return AnalyticsTracker.GetGameModeParameter(challenges.cityChallengeIndex, challenges.challengeType, cityModel.Mode);
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x00042D18 File Offset: 0x00040F18
		private static string GetVersionNumber()
		{
			return global::Version.Name;
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x00042D1F File Offset: 0x00040F1F
		private void OnPlayerDataChanged()
		{
			if (this._player.IsTelemetryEnabled)
			{
				this.StartAnalytics();
			}
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x00042D34 File Offset: 0x00040F34
		public void OnCreatedInScope(IScope scope)
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.Analytics))
			{
				this._player.DataChanged += this.OnPlayerDataChanged;
			}
		}

		// Token: 0x040010BF RID: 4287
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x040010C0 RID: 4288
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Analytics");

		// Token: 0x040010C1 RID: 4289
		private const string UpgradeChoiceEvent = "upgradeChoice";

		// Token: 0x040010C2 RID: 4290
		private const string GameResumedEvent = "cityResumed";

		// Token: 0x040010C3 RID: 4291
		private const string GameContinuedEvent = "cityContinued";

		// Token: 0x040010C4 RID: 4292
		private const string GameOverEvent = "cityOver";

		// Token: 0x040010C5 RID: 4293
		private const string GameStartEvent = "cityStarted";

		// Token: 0x040010C6 RID: 4294
		private const string ScreenVisitedEvent = "screenVisited";

		// Token: 0x040010C7 RID: 4295
		private const string LevelNameParameter = "cityName";

		// Token: 0x040010C8 RID: 4296
		private const string GameModeParameter = "cityMode";

		// Token: 0x040010C9 RID: 4297
		private const string ScoreParameter = "cityScore";

		// Token: 0x040010CA RID: 4298
		private const string TimeParameter = "cityTime";

		// Token: 0x040010CB RID: 4299
		private const string SelectedUpgradeParameter = "selectedUpgrade";

		// Token: 0x040010CC RID: 4300
		private const string OtherUpgradeParameter = "otherUpgrade";

		// Token: 0x040010CD RID: 4301
		private const string DestinationCountParameter = "destinationCount";

		// Token: 0x040010CE RID: 4302
		private const string ThemeParameter = "gameTheme";

		// Token: 0x040010CF RID: 4303
		private const string ScreenNameParameter = "screenName";

		// Token: 0x040010D0 RID: 4304
		private const string TutorialStageEvent = "tutorialStage";

		// Token: 0x040010D1 RID: 4305
		private const string TutorialSkippedEvent = "tutorialSkipped";

		// Token: 0x040010D2 RID: 4306
		private const string TutorialStartedEvent = "tutorialStarted";

		// Token: 0x040010D3 RID: 4307
		private const string TutorialCompletedEvent = "tutorialFinished";

		// Token: 0x040010D4 RID: 4308
		private const string TutorialVoluntaryStartParameter = "voluntaryStart";

		// Token: 0x040010D5 RID: 4309
		private const string TutorialStageParameter = "tutorialStage";

		// Token: 0x040010D6 RID: 4310
		private const string VersionNumberParameter = "gameVersion";

		// Token: 0x040010D7 RID: 4311
		private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

		// Token: 0x040010D8 RID: 4312
		private AnalyticsTracker.State _currentState;

		// Token: 0x0200033B RID: 827
		private enum State
		{
			// Token: 0x040010DA RID: 4314
			Uninitialized,
			// Token: 0x040010DB RID: 4315
			Initializing,
			// Token: 0x040010DC RID: 4316
			Initialized
		}
	}
}
