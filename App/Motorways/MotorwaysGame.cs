using System;
using System.Collections.Generic;
using System.IO;
using Client;
using Factory;
using FixMath;
using Motorways.Audio;
using Motorways.Commands;
using Motorways.Leaderboards;
using Motorways.Models;
using Motorways.UI;
using Motorways.Views;
using Server;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003B6 RID: 950
	public class MotorwaysGame : Game
	{
		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x0600168D RID: 5773 RVA: 0x00050649 File Offset: 0x0004E849
		public IdleVehicleChecker IdleVehicleChecker
		{
			get
			{
				return this._idleVehicleChecker;
			}
		}

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x0600168E RID: 5774 RVA: 0x00050651 File Offset: 0x0004E851
		// (set) Token: 0x0600168F RID: 5775 RVA: 0x00050659 File Offset: 0x0004E859
		public MapDefinition MapDefinition { get; protected set; }

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06001690 RID: 5776 RVA: 0x00050662 File Offset: 0x0004E862
		public MotorwaysThemeDatabase Theme
		{
			get
			{
				return this._theme;
			}
		}

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06001691 RID: 5777 RVA: 0x0005066A File Offset: 0x0004E86A
		// (set) Token: 0x06001692 RID: 5778 RVA: 0x00050672 File Offset: 0x0004E872
		public bool HasGameEnded { get; private set; }

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06001693 RID: 5779 RVA: 0x0005067B File Offset: 0x0004E87B
		// (set) Token: 0x06001694 RID: 5780 RVA: 0x00050683 File Offset: 0x0004E883
		public CityDefinition StartedWithCityDefinition { get; protected set; }

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06001695 RID: 5781 RVA: 0x0005068C File Offset: 0x0004E88C
		// (set) Token: 0x06001696 RID: 5782 RVA: 0x00050694 File Offset: 0x0004E894
		public GameMode StartedWithGameMode { get; private set; }

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06001697 RID: 5783 RVA: 0x0005069D File Offset: 0x0004E89D
		public bool PlayingBackSimJournal
		{
			get
			{
				return this._playingBackSimJournal;
			}
		}

		// Token: 0x06001698 RID: 5784 RVA: 0x000506A8 File Offset: 0x0004E8A8
		public void Start(CityDefinition cityDefinition, GameMode mode, MapChallenge mapChallenge, bool replaceExistingRules = false)
		{
			this._survivedFrame = true;
			this._lastSaveRealTimeSeconds = Time.time;
			this._lastSaveGameTimeHours = this._simulationClock.Time;
			GameStartReason startReason = GameStartReason.New;
			CityModel cityModel = this._simulation.GetModel<CityModel>();
			if (cityModel != null)
			{
				startReason = GameStartReason.Resumed;
			}
			this._debugRenderSetManager.Register(cityDefinition);
			base.Start(startReason);
			this.HasGameEnded = false;
			this.StartedWithCityDefinition = cityDefinition;
			this.StartedWithGameMode = mode;
			if (cityModel != null)
			{
				this.StartedWithGameMode = cityModel.InitialMode;
			}
			GameRules rules;
			if (cityModel != null)
			{
				if (cityModel.Rules == null)
				{
					this.FixDeserializedSimulation(cityDefinition);
				}
				if (replaceExistingRules)
				{
					rules = MotorwaysGame.CreateRulesForMode(base.Scope, mode);
					cityModel.SetGameMode(mode, rules);
					this._city.Initialize(this._city.Definition, rules);
				}
				else
				{
					rules = cityModel.Rules;
				}
				this._placer.SetTileData(this._city.Definition.TileWeightData);
			}
			else
			{
				rules = MotorwaysGame.CreateRulesForMode(base.Scope, mode);
				if (!this.TryLoadSimulationJournal(rules))
				{
					string cityName = (this.MapDefinition != null) ? this.MapDefinition.cityName : "unknown";
					InitCityCommand initCityCommand;
					if (mapChallenge == null)
					{
						initCityCommand = InitCityCommand.CreateNormalCity(base.Scope, cityName, cityDefinition, mode, rules, global::Random.NextSimulationSeed());
					}
					else
					{
						if (mapChallenge.type == MapChallenge.ChallengeType.City)
						{
							mapChallenge.seed = (ulong)global::Random.NextSimulationSeed();
						}
						initCityCommand = InitCityCommand.CreateChallengeCity(base.Scope, cityName, cityDefinition, mode, rules, mapChallenge);
					}
					this._simulation.ScheduleCommand(initCityCommand);
				}
			}
			this.StartAudio();
			this._audioSync.StartClock();
			this._audioSystem.SignalPulse += this.OnAudioPulse;
			this._simulation.Subscribe(this._view);
			this._connectivityUpdater.Start();
			MotorwaysClient viewClient = this._view as MotorwaysClient;
			GameUIScreen inGameUI = base.Scope.Get<GameUIScreen>();
			if (rules.ShowsUI())
			{
				inGameUI.InitScreen(base.Scope, false);
				viewClient.AddView(inGameUI);
				viewClient.AddView(base.Scope.Get<HotkeyDebugView>());
				viewClient.AddView(base.Scope.Get<CityScheduleView>());
				viewClient.AddView(base.Scope.Get<TutorialDebugView>());
				viewClient.AddView(base.Scope.Get<IdleVehicleCheckerDebugView>());
				viewClient.AddView(base.Scope.Get<SimulationToggleDebugView>());
			}
			else
			{
				inGameUI.gameObject.SetActive(false);
			}
			if (rules.UseCamera())
			{
				base.Scope.Get<CameraView>().Initialize(rules);
			}
			if (rules.CanInteract())
			{
				base.Scope.Get<PlayerActionController>().SetGameScope(base.Scope);
			}
			if (rules.CanInteract())
			{
				MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles, base.Scope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions);
			}
			if (rules.RecordsGameStatistics())
			{
				this._lastRecordedStatistics = base.Scope.Get<MotorwaysGameStatistics>();
				this._lastRecordedStatistics.InitFromGame(this);
				this._achievementStatisticsAtStartOfGame = new AchievementStatistics();
				if (startReason != GameStartReason.New)
				{
					this._achievementStatisticsAtStartOfGame.LogUpgradeStatistics(this, NullAchievementHandler.Instance, null);
				}
			}
			if (rules.CanSave() && startReason == GameStartReason.New && this._challengeSystem.GetActiveDailyChallengeSaves(this._player, false).Count > 0)
			{
				MotorwaysTimedChallengeScore challengeScore = this._player.GetChallengeScore(MapChallenge.ChallengeType.Daily, this._challengeSystem.DailyChallenge.TimeEnd);
				if (challengeScore.ScoreState == LeaderboardScoreState.Editable)
				{
					challengeScore.LockScore();
				}
				LeaderboardId leaderboardId = new DailyLeaderboardId(this._challengeSystem.DailyChallenge.TimeStart);
				this._leaderboardService.RequestLocalEntry(leaderboardId, delegate(LeaderboardEntry localEntry, long totalLeaderboardEntryCount, LeaderboardError error)
				{
					if (error != null || localEntry == null)
					{
						return;
					}
					if (localEntry.ScoreState == LeaderboardScoreState.Editable)
					{
						this._leaderboardService.SubmitScore(leaderboardId, localEntry.Score, LeaderboardScoreState.Locked);
					}
				});
			}
			if (mode != GameMode.Background)
			{
				this._player.Touch();
			}
			this._idleVehicleChecker.Initialize(this);
			this._gameplayEventHandler = base.Scope.Get<GameplayEventHandler>();
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x00050A5C File Offset: 0x0004EC5C
		public bool FixDeserializedSimulation(CityDefinition cityDefinition)
		{
			CityModel cityModel = this._simulation.GetModel<CityModel>();
			if (cityModel == null)
			{
				return false;
			}
			GameRules rules = MotorwaysGame.CreateRulesForMode(base.Scope, cityModel.Mode);
			cityModel.SetGameMode(cityModel.Mode, rules);
			if (FeatureToggle.IsFeatureEnabled(Feature.RecordIntersectionDecisions) && this._simulation.GetModel<IntersectionDecisionDatabaseModel>() == null)
			{
				IntersectionDecisionDatabaseModel decisionDatabaseModel = this._simulation.Scope.Get<IntersectionDecisionDatabaseModel>();
				this._simulation.AddModel(decisionDatabaseModel);
			}
			this._city.Initialize(cityDefinition, rules);
			return true;
		}

		// Token: 0x0600169A RID: 5786 RVA: 0x00050ADC File Offset: 0x0004ECDC
		public void ContinueInMode(GameMode mode)
		{
			GameRules newRules = MotorwaysGame.CreateRulesForMode(base.Scope, mode);
			this.HasGameEnded = false;
			this._city.SetGameRules(newRules);
			this._simulation.GetModel<CityModel>().SetGameMode(mode, newRules);
		}

		// Token: 0x0600169B RID: 5787 RVA: 0x00050B1C File Offset: 0x0004ED1C
		public override bool TrySave(GameJournalMotive motive)
		{
			if (!this._city.Rules.CanSave())
			{
				return false;
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				return false;
			}
			if (this.HasGameEnded)
			{
				return false;
			}
			MotorwaysGameJournalSave savedGame = base.Scope.Get<MotorwaysGameJournalSave>();
			if (!savedGame.InitializeFromSimulation(this._simulation, motive))
			{
				base.Scope.Release(savedGame);
				return false;
			}
			if (motive == GameJournalMotive.AppDeactivated && this._survivedFrame)
			{
				StartupScreen.CanAutoResumeDeactivatedGame = true;
			}
			this._player.LocalSavedGame = savedGame;
			return true;
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x00050B9C File Offset: 0x0004ED9C
		public Diagnostics.Report GenerateDiagnosticReport(string motive, DiagnosticReportAttachments attachments)
		{
			Diagnostics.Report report = new Diagnostics.Report();
			report.Motive = motive;
			report.SetMetadata("buildName", global::Version.Name, true);
			report.SetMetadata("buildTimestamp", global::Version.Timestamp.ToString(), true);
			if (!string.IsNullOrEmpty(global::Version.CommitHash))
			{
				report.SetMetadata("commitHash", global::Version.CommitHash, true);
			}
			try
			{
				List<string> toggledFeatures = new List<string>();
				foreach (object obj in Enum.GetValues(typeof(Feature)))
				{
					Feature feature = (Feature)obj;
					if (FeatureToggle.IsFeatureEnabled(feature))
					{
						toggledFeatures.Add(feature.ToString());
					}
				}
				if (toggledFeatures.Count > 0)
				{
					report.SetMetadata("buildFeatures", string.Join(";", toggledFeatures), false);
				}
			}
			catch (Exception e)
			{
				Game.Log.Error("Caught exception while trying to attach the list of toggled features to a diagnostic report.\n{0}", new object[]
				{
					e.ToString()
				});
			}
			try
			{
				ScoreModel scoreModel = this._simulation.GetModel<ScoreModel>();
				if (scoreModel != null)
				{
					if (this._city.Rules.ScoringMode == ScoringMode.Trips)
					{
						report.SetMetadata("score", scoreModel.Score.ToString(), false);
					}
					else if (this._city.Rules.ScoringMode == ScoringMode.EfficiencyMilestones)
					{
						report.SetMetadata("score", scoreModel.CurrentEfficiencyMilestone.ToString(), false);
					}
				}
			}
			catch (Exception e2)
			{
				Game.Log.Error("Caught exception while trying to attach score to a diagnostic report.\n{0}", new object[]
				{
					e2.ToString()
				});
			}
			try
			{
				ActiveChallengesModel activeChallengeModel = this._simulation.GetModel<ActiveChallengesModel>();
				if (activeChallengeModel.challenges.Count != 0)
				{
					string challengeNames = string.Empty;
					foreach (ChallengeData challenge in activeChallengeModel.challenges)
					{
						challengeNames = challengeNames + challenge.name + ";";
					}
					report.SetMetadata("challenges", challengeNames, false);
					report.SetMetadata("challengesType", activeChallengeModel.challengeType.ToString(), true);
				}
			}
			catch (Exception e3)
			{
				Game.Log.Error("Caught exception while trying to attach challenge info to a diagnostic report.\n{0}", new object[]
				{
					e3.ToString()
				});
			}
			try
			{
				ClockModel clockModel = this._simulation.GetModel<ClockModel>();
				if (clockModel != null)
				{
					report.SetMetadata("simulationTime", ((float)clockModel.NextFrame.time).ToString(), false);
				}
			}
			catch (Exception e4)
			{
				Game.Log.Error("Caught exception while trying to attach time to a diagnostic report.\n{0}", new object[]
				{
					e4.ToString()
				});
			}
			if (this.MapDefinition != null)
			{
				report.SetMetadata("city", this.MapDefinition.mapName, true);
			}
			CityModel cityModel = this._simulation.GetModel<CityModel>();
			if (cityModel != null)
			{
				report.SetMetadata("gameMode", cityModel.Mode.ToString(), true);
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.TrackAnalyticsInDiagnosticReports))
			{
				try
				{
					int bigPinCount = 0;
					int destinationCount = 0;
					Dictionary<int, int> destinationCountPerGroup = new Dictionary<int, int>();
					foreach (DestinationModel destinationModel in this._simulation.GetModels<DestinationModel>())
					{
						destinationCount++;
						if (destinationModel.IsOvercrowding)
						{
							bigPinCount++;
						}
						if (destinationCountPerGroup.ContainsKey(destinationModel.GroupIndex))
						{
							Dictionary<int, int> dictionary = destinationCountPerGroup;
							int groupIndex3 = destinationModel.GroupIndex;
							dictionary[groupIndex3]++;
						}
						else
						{
							destinationCountPerGroup.Add(destinationModel.GroupIndex, 1);
						}
					}
					report.SetMetadata("bigPins", bigPinCount.ToString(), false);
					report.SetMetadata("destinations", destinationCount.ToString(), false);
					DemandModel demand = base.Scope.Get<DemandModel>();
					foreach (int groupIndex in destinationCountPerGroup.Keys)
					{
						report.SetMetadata(string.Format("group{0}Destinations", groupIndex), destinationCountPerGroup[groupIndex].ToString(), false);
						int allocatedPins = 0;
						List<Fix64> pinTimes;
						if (demand.allocatedPinsInLastWeek.TryGetValue(groupIndex, out pinTimes))
						{
							allocatedPins += pinTimes.Count;
						}
						report.SetMetadata(string.Format("group{0}NewPins", groupIndex), allocatedPins.ToString(), false);
					}
					CityPlanModel cityPlanModel = base.Scope.Get<CityPlanModel>();
					for (int groupIndex2 = 0; groupIndex2 < cityPlanModel.groupHouseCounts.Length; groupIndex2++)
					{
						if (cityPlanModel.groupHouseCounts[groupIndex2] > 0)
						{
							report.SetMetadata(string.Format("group{0}Houses", groupIndex2), cityPlanModel.groupHouseCounts[groupIndex2].ToString(), false);
						}
					}
				}
				catch (Exception e5)
				{
					Game.Log.Error("Caught exception while trying to attach game statistics to a diagnostic report.\n{0}", new object[]
					{
						e5.ToString()
					});
				}
				UpgradeDatabaseModel upgrades = this._simulation.GetModel<UpgradeDatabaseModel>();
				for (int upgradeTypeIndex = 0; upgradeTypeIndex < 9; upgradeTypeIndex++)
				{
					UpgradeType upgradeType = (UpgradeType)upgradeTypeIndex;
					int timesUpgradePlaced;
					if (upgrades.numberOfTimesAnUpgradeIsPlaced.TryGetValue(upgradeType, out timesUpgradePlaced) || upgrades.timesUpgradePresented[upgradeTypeIndex] > 0)
					{
						report.SetMetadata(string.Format("upgrade{0}_currentlyUsed", upgradeType), (upgrades.GetTotalUpgradeCount(upgradeType) - upgrades.GetAvailableUpgradeCount(upgradeType)).ToString(), false);
						report.SetMetadata(string.Format("upgrade{0}_totalAwarded", upgradeType), upgrades.GetTotalUpgradeCount(upgradeType).ToString(), false);
						report.SetMetadata(string.Format("upgrade{0}_timesPlaced", upgradeType), timesUpgradePlaced.ToString(), false);
						report.SetMetadata(string.Format("upgrade{0}_presented", upgradeType), upgrades.timesUpgradePresented[(int)upgradeType].ToString(), false);
						report.SetMetadata(string.Format("upgrade{0}_packagesTaken", upgradeType), upgrades.NumberOfPackagesTakenOf(upgradeType).ToString(), false);
					}
				}
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.RecordAppJournal) && attachments.HasFlag(DiagnosticReportAttachments.AppCommandJournal))
			{
				IScope appScope = base.Scope.Get<App>().Scope;
				AppCommandJournal appCommandJournal = base.Scope.Get<AppCommandJournal>();
				MemoryStream journalStream = new MemoryStream();
				using (BinaryWriter journalWriter = new BinaryWriter(journalStream))
				{
					appScope.Export(appCommandJournal, journalWriter);
				}
				report.AttachFile("commands.appjournal", journalStream.ToArray());
			}
			if (attachments.HasFlag(DiagnosticReportAttachments.SimCommandJournal))
			{
				CommandJournal simCommandJournal = base.Scope.Get<CommandJournal>();
				if (simCommandJournal.EntryCount > 0)
				{
					MemoryStream journalStream2 = new MemoryStream();
					using (BinaryWriter journalWriter2 = new BinaryWriter(journalStream2))
					{
						base.Scope.Export(simCommandJournal, journalWriter2);
					}
					report.AttachFile("commands.simjournal", journalStream2.ToArray());
				}
			}
			if (attachments.HasFlag(DiagnosticReportAttachments.SimArchive) && this._city.Rules.CanSave())
			{
				MotorwaysGameJournalSave save = base.Scope.Get<MotorwaysGameJournalSave>();
				if (save.InitializeFromSimulation(this._simulation, GameJournalMotive.DiagnosticsReport))
				{
					MemoryStream saveStream = new MemoryStream();
					using (BinaryWriter saveWriter = new BinaryWriter(saveStream))
					{
						save.OnSerializeBeforeData(saveWriter);
						saveWriter.Write(save.GetBytesForSerializing());
					}
					report.AttachFile("simulation.gamejournal", saveStream.ToArray());
				}
				base.Scope.Release(save);
			}
			if (attachments.HasFlag(DiagnosticReportAttachments.Log) && FeatureToggle.IsFeatureEnabled(Feature.RecordLogs))
			{
				byte[] log = Diagnostics.Log.RecordedLog;
				if (log != null)
				{
					report.AttachFile("log.txt", log);
				}
			}
			if (attachments.HasFlag(DiagnosticReportAttachments.Screenshot))
			{
				float oldBlurStrength = this._camera.customBlur.Strength;
				this._camera.customBlur.Strength = 0f;
				GameObject captureCameraObject = new GameObject();
				Camera captureCamera = captureCameraObject.AddComponent<Camera>();
				captureCamera.CopyFrom(this._camera.DefaultCamera);
				Fix64 fixedZoom = this._city.GetCameraSizeAtTime(base.Scope.Get<ClockModel>().NextFrame.time);
				RectFixed playableArea = this._city.GetClientPlayableAreaAtZoom(fixedZoom, City.PlayableAreaRoundingType.AllowPartialTiles);
				Vector3 nativeOrigin = new Vector3((float)playableArea.Center.x, (float)playableArea.Center.y, captureCamera.transform.position.z);
				captureCamera.transform.position = nativeOrigin;
				captureCamera.orthographicSize = base.Scope.Get<CameraView>().MaxZoom;
				DelegateCanvasGroup component = base.Scope.Get<GameUIScreen>().GetComponent<DelegateCanvasGroup>();
				float oldUiAlpha = component.Alpha;
				component.Alpha = 0f;
				((MotorwaysThemeDatabase)this._themeDatabase).materialCollection.SetWorldGridThickness(0f);
				float screenshotScale = Mathf.Min(1f, 1024f / (float)Mathf.Max(Screen.width, Screen.height));
				RenderTexture tempRenderTarget = RenderTexture.GetTemporary(Mathf.RoundToInt((float)Screen.width * screenshotScale), Mathf.RoundToInt((float)Screen.height * screenshotScale), 24, RenderTextureFormat.ARGB32);
				RenderTexture backup = RenderTexture.active;
				RenderTexture.active = tempRenderTarget;
				captureCamera.targetTexture = tempRenderTarget;
				captureCamera.Render();
				Texture2D texture2D = new Texture2D(captureCamera.targetTexture.width, captureCamera.targetTexture.height, TextureFormat.RGB24, false);
				texture2D.ReadPixels(new Rect(0f, 0f, (float)captureCamera.targetTexture.width, (float)captureCamera.targetTexture.height), 0, 0);
				texture2D.Apply();
				byte[] jpgBytes = texture2D.EncodeToJPG();
				report.AttachFile("screenshot.jpg", jpgBytes);
				RenderTexture.active = backup;
				UnityEngine.Object.Destroy(texture2D);
				UnityEngine.Object.Destroy(captureCameraObject);
				RenderTexture.ReleaseTemporary(tempRenderTarget);
				component.Alpha = oldUiAlpha;
				this._camera.customBlur.Strength = oldBlurStrength;
			}
			return report;
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x000516A8 File Offset: 0x0004F8A8
		public override void StopAudio()
		{
			string cityName = (this.StartedWithCityDefinition != null) ? this.StartedWithCityDefinition.name : "unknown";
			string log = cityName + ".StopAudio() : AudioEnvironment has already been nuked. Skipping ...";
			if (this._audioEnvironment != null)
			{
				this._audioEnvironment.Kill();
				this._audioEnvironment = null;
				log = cityName + ".StopAudio() : Success. Killing AudioEnvironment.";
			}
			Dbug.Log.Info(log, Array.Empty<object>());
		}

		// Token: 0x0600169E RID: 5790 RVA: 0x00051718 File Offset: 0x0004F918
		public void ClearPathfinder()
		{
			this._pathfinder.Clear();
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x00051725 File Offset: 0x0004F925
		public void PausePathfinder()
		{
			this._pathfinder.PauseUpdate();
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x00051732 File Offset: 0x0004F932
		public void ResumePathfinder()
		{
			this._pathfinder.ResumeUpdate();
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x00051740 File Offset: 0x0004F940
		public void StartAudio()
		{
			if (this._audioEnvironment != null && this._audioEnvironment.Active)
			{
				Dbug.Log.Info(this.StartedWithCityDefinition.name + ".StartAudio(): AudioEnvironment is already active. Skipping ...", Array.Empty<object>());
				return;
			}
			AudioLoadout loadout = null;
			if (this.StartedWithCityDefinition.audioLoadout != null)
			{
				Dbug.Log.Info(this.StartedWithCityDefinition.name + ".StartAudio() : Refreshing City Loadout.", Array.Empty<object>());
				loadout = this._audioSystem.GetLoadout(this.StartedWithCityDefinition.audioLoadout.name);
			}
			if (loadout != null)
			{
				Dbug.Log.Info(this.StartedWithCityDefinition.name + ".StartAudio() : Activate Audio Environment With a New Loadout + City.", Array.Empty<object>());
				this._audioEnvironment = new AudioEnvironment(loadout, this._city, this);
			}
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x00051816 File Offset: 0x0004FA16
		public void SetMapDefinition(MapDefinition newMapDefinition)
		{
			this.MapDefinition = newMapDefinition;
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x00051820 File Offset: 0x0004FA20
		public override void OnGameEnd(GameEndReason gameEndReason)
		{
			base.OnGameEnd(gameEndReason);
			this.SetPaused(true);
			this._themeDatabase.DisableDeleteModeOverrides();
			if (this.HasGameEnded)
			{
				Diagnostics.FailAssert("A game can only be ended once.", Array.Empty<object>());
				return;
			}
			this.HasGameEnded = true;
			if (gameEndReason == GameEndReason.GameOver)
			{
				this.DeleteLocalSave();
				base.Scope.Get<CameraView>().ResetPlayerViewport();
				base.Scope.Get<AnalyticsTracker>().TrackGameComplete(this);
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.SubmitDiagnosticReportOnGameOver))
			{
				this.UploadDiagnosticsReport(gameEndReason);
			}
			if (this._simulation.Scope.Get<ScoreModel>().Score != 0)
			{
				GameRules rules = this._city.Rules;
				if (rules.RecordsGameStatistics())
				{
					this.RecordGameStatistics(new GameEndReason?(gameEndReason));
				}
				if (rules.SupportsLeaderboards())
				{
					this.UpdateLeaderboardIfRequired(gameEndReason);
				}
			}
			else
			{
				this.DeleteLocalSave();
			}
			this._player.Touch();
			this._debugRenderSetManager.Unregister(this.StartedWithCityDefinition);
		}

		// Token: 0x060016A4 RID: 5796 RVA: 0x00051908 File Offset: 0x0004FB08
		private void UpdateLeaderboardIfRequired(GameEndReason gameEndReason)
		{
			ActiveChallengesModel activeChallenge = this._simulation.Scope.Get<ActiveChallengesModel>();
			if (activeChallenge.challengeType == MapChallenge.ChallengeType.Mystery)
			{
				return;
			}
			bool isDailyOrWeeklyChallenge = activeChallenge.HasChallenges && (activeChallenge.challengeType == MapChallenge.ChallengeType.Daily || activeChallenge.challengeType == MapChallenge.ChallengeType.Weekly);
			if (isDailyOrWeeklyChallenge && !activeChallenge.IsActiveWithGracePeriod)
			{
				return;
			}
			if (!isDailyOrWeeklyChallenge && gameEndReason != GameEndReason.GameOver)
			{
				return;
			}
			int score = base.Scope.Get<ScoreModel>().Score;
			LeaderboardScoreState scoreState = MotorwaysScoreValidation.ShouldLockScoreWhenGameEnds(activeChallenge.challengeType, gameEndReason) ? LeaderboardScoreState.Locked : LeaderboardScoreState.Editable;
			this._leaderboardService.SubmitScore(this.GetLeaderboardIdForGame(), score, scoreState);
		}

		// Token: 0x060016A5 RID: 5797 RVA: 0x0005199C File Offset: 0x0004FB9C
		public override void OnReleasedFromScope(IScope scope)
		{
			base.OnReleasedFromScope(scope);
			if (this._lastRecordedStatistics != null)
			{
				base.Scope.Release(this._lastRecordedStatistics);
				this._lastRecordedStatistics = null;
			}
		}

		// Token: 0x060016A6 RID: 5798 RVA: 0x000519C6 File Offset: 0x0004FBC6
		private void DeleteLocalSave()
		{
			if (this._player.HasLocalSavedGame)
			{
				Game.Log.Info("Deleting local save after ending the game.", Array.Empty<object>());
				this._player.LocalSavedGame = null;
			}
		}

		// Token: 0x060016A7 RID: 5799 RVA: 0x000519F8 File Offset: 0x0004FBF8
		private void UploadDiagnosticsReport(GameEndReason gameEndReason)
		{
			string reportMotive = gameEndReason.ToString();
			if (reportMotive.Length >= 2)
			{
				reportMotive = char.ToLower(reportMotive[0]).ToString() + reportMotive.Substring(1);
			}
			this.GenerateDiagnosticReport(reportMotive, DiagnosticReportAttachments.SimCommandJournal | DiagnosticReportAttachments.Screenshot).Upload();
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x00051A4C File Offset: 0x0004FC4C
		public void RecordGameStatistics(GameEndReason? gameEndReason = null)
		{
			if (this._city.Rules.RecordsGameStatistics())
			{
				MotorwaysGameStatistics currentStatistics = base.Scope.Get<MotorwaysGameStatistics>();
				currentStatistics.InitFromGameIncrementally(this, this._lastRecordedStatistics, gameEndReason);
				this._player.RecordGameStatistics(currentStatistics);
				if (this._city.Rules.ScoringMode == ScoringMode.Trips)
				{
					this._player.AchievementStatistics.LogScoreStatistics(currentStatistics, this._achievementHandler);
				}
				this._player.AchievementStatistics.LogUpgradeStatistics(this, this._achievementHandler, this._achievementStatisticsAtStartOfGame);
				if (!(gameEndReason != GameEndReason.GameOver))
				{
					this._player.AchievementStatistics.LogGameOverStatistics(this, this._achievementHandler);
				}
				this._player.CheckLifetimeAchievements();
				if (this._lastRecordedStatistics != null)
				{
					base.Scope.Release(this._lastRecordedStatistics);
				}
				this._lastRecordedStatistics = currentStatistics;
				ActiveChallengesModel activeChallenges = base.Scope.Get<ActiveChallengesModel>();
				if (activeChallenges.HasChallenges && activeChallenges.challengeType == MapChallenge.ChallengeType.City)
				{
					int currentCityChallengeIndex = -1;
					for (int cityChallengeIndex = 0; cityChallengeIndex < this.MapDefinition.cityChallenges.Length; cityChallengeIndex++)
					{
						bool sameChallengesAsCityChallenge = true;
						if (this.MapDefinition.cityChallenges[cityChallengeIndex].challenges.Length == activeChallenges.challenges.Count)
						{
							foreach (ChallengeData challenge in this.MapDefinition.cityChallenges[cityChallengeIndex].challenges)
							{
								if (!activeChallenges.challenges.Contains(challenge))
								{
									sameChallengesAsCityChallenge = false;
									break;
								}
							}
							if (sameChallengesAsCityChallenge)
							{
								currentCityChallengeIndex = cityChallengeIndex;
							}
						}
					}
					CityChallengeStatistics stats = this._player.GetCityChallengeScore(this.MapDefinition.cityName, GameMode.Normal, currentCityChallengeIndex, true);
					int currentScore = base.Scope.Get<ScoreModel>().Score;
					if (stats.BestScore < currentScore)
					{
						stats.BestScore = currentScore;
					}
				}
			}
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x00051C24 File Offset: 0x0004FE24
		public override void Tick(float frameTime)
		{
			if (!this._survivedFrame)
			{
				StartupScreen.CanAutoResumeDeactivatedGame = false;
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.SubmitDiagnosticReportOnException))
			{
				bool sendExceptionReport = !this._survivedFrame && this._exceptionReport == null;
				if (FeatureToggle.IsFeatureEnabled(Feature.SubmitOnlyOneDiagnosticReportOnExceptionPerGame))
				{
					sendExceptionReport &= !this._hasSubmittedExceptionReport;
				}
				if (sendExceptionReport)
				{
					this._exceptionReport = this.GenerateDiagnosticReport("exception", DiagnosticReportAttachments.SimCommandJournal | DiagnosticReportAttachments.Log);
					if (!string.IsNullOrEmpty(Diagnostics.Exception.LastException))
					{
						this._exceptionReport.SetMetadata("exception", Diagnostics.Exception.LastException, false);
						Diagnostics.Exception.LastException = null;
					}
					if (!string.IsNullOrEmpty(Diagnostics.Exception.LastExceptionStackTrace))
					{
						this._exceptionReport.SetMetadata("stackTrace", Diagnostics.Exception.LastExceptionStackTrace, false);
						Diagnostics.Exception.LastExceptionStackTrace = null;
					}
					this._exceptionReport.Upload();
					this._hasSubmittedExceptionReport = true;
				}
				if (this._exceptionReport != null && this._exceptionReport.Id >= 0 && !this._loggedExceptionReportId)
				{
					this._loggedExceptionReportId = true;
					Debug.LogFormat("Caught exception during MotorwaysGame.Tick() and submitted report with id {0}.", new object[]
					{
						this._exceptionReport.Id
					});
				}
			}
			this._survivedFrame = false;
			if (this._playingBackSimJournal)
			{
				IInputState inputState = base.Scope.Get<IInputState>();
				if (this._simulation.HasAnyScheduledCommands)
				{
					inputState.BlockActions = true;
					int playbackFrameCount = base.Scope.Get<Server.Clock>().FrameCount;
					if (playbackFrameCount - this._lastLoggedPlaybackFrame >= 25)
					{
						this._lastLoggedPlaybackFrame = playbackFrameCount;
						Game.Log.Info("Journal playback up to simulation frame {0} / {1}.", new object[]
						{
							playbackFrameCount,
							this._playbackDuration
						});
					}
				}
				else
				{
					inputState.BlockActions = false;
					this._playingBackSimJournal = false;
					Game.Log.Info("Completed journal playback, switching to standard execution.", Array.Empty<object>());
				}
			}
			else if (this.StartedWithGameMode != GameMode.Background && FeatureToggle.IsFeatureEnabled(Feature.ValidateSimulationDeterminism) && !this._simulation.IsPaused && this._simulationClock.FrameCount - this._lastSnapshotFrame > 10)
			{
				this._lastSnapshotFrame = this._simulationClock.FrameCount;
				this._simulation.ScheduleCommand(base.Scope.Get<SnapshotCommand>());
			}
			base.Tick(frameTime);
			this._idleVehicleChecker.RunCheck();
			if (this._playingBackSimJournal && this._simulation.IsPaused && this._simulation.HasAnyScheduledCommands)
			{
				int nextScheduledFrameIndex = this._simulation.NextScheduledCommand.FrameIndex;
				while (nextScheduledFrameIndex > this._simulationClock.FrameCount + 1)
				{
					base.Tick(frameTime);
				}
			}
			this._connectivityUpdater.Tick();
			if (this._audioEnvironment != null)
			{
				this._audioEnvironment.Update();
			}
			this._survivedFrame = true;
			if (this._city.Rules.ShouldSavePeriodically)
			{
				Fix64 simulationHours = this._simulationClock.Time / (Fix64)0.8333333333333334;
				if (Time.time - this._lastSaveRealTimeSeconds > 300f || simulationHours - this._lastSaveGameTimeHours > this._saveIntervalGameTimeHours)
				{
					this.TrySave(GameJournalMotive.Autosave);
					this._lastSaveRealTimeSeconds = Time.time;
					this._lastSaveGameTimeHours = simulationHours;
				}
			}
			this._gameplayEventHandler.Tick(this);
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x060016AA RID: 5802 RVA: 0x00051F48 File Offset: 0x00050148
		// (set) Token: 0x060016AB RID: 5803 RVA: 0x00051F50 File Offset: 0x00050150
		public float DebugTimescale
		{
			get
			{
				return this._debugTimescale;
			}
			set
			{
				this._debugTimescale = Mathf.Max(0f, value);
			}
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x00051F64 File Offset: 0x00050164
		public void TickDuringTransition(float frameTime)
		{
			CameraView cameraView = base.Scope.Get<ViewClient>().CameraView;
			if (cameraView != null)
			{
				this._timeInterval.UnsyncedDelta = frameTime;
				this._timeInterval.Delta = frameTime;
				cameraView.Tick(this._timeInterval, 0f);
			}
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x00051FB0 File Offset: 0x000501B0
		public static GameRules CreateRulesForMode(IScope gameScope, GameMode mode)
		{
			switch (mode)
			{
			case GameMode.Tutorial:
				return gameScope.Get<TutorialGameRules>();
			case GameMode.Background:
				return gameScope.Get<BackgroundGameRules>();
			case GameMode.Endless:
				return gameScope.Get<EndlessGameRules>();
			case GameMode.Expert:
				return gameScope.Get<ExpertGameRules>();
			case GameMode.Movie:
				return gameScope.Get<MovieGameRules>();
			case GameMode.Cinematic:
				return gameScope.Get<CinematicGameRules>();
			case GameMode.Creative:
				return gameScope.Get<CreativeGameRules>();
			}
			return gameScope.Get<GameRules>();
		}

		// Token: 0x060016AE RID: 5806 RVA: 0x0005201C File Offset: 0x0005021C
		protected override void AdjustTimeInterval(TimeInterval timeInterval)
		{
			timeInterval.UnsyncedDelta *= this._debugTimescale;
			timeInterval.Delta *= this._debugTimescale;
			this._audioSync.SyncTimeInterval(timeInterval, this._nextPulseTime, this._audioSystem);
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x0005205C File Offset: 0x0005025C
		private void OnAudioPulse(double pulseTime, int pulseIndex, int pulseLoopCount)
		{
			this._nextPulseTime = pulseTime;
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x00052068 File Offset: 0x00050268
		public override bool CanInteract()
		{
			City city = this._simulation.Scope.Get<City>();
			if (city != null && base.CanInteract())
			{
				GameRules rules = city.Rules;
				if (rules != null)
				{
					return rules.CanInteract();
				}
			}
			Diagnostics.FailAssert("We should never get here!", Array.Empty<object>());
			return false;
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x000520B4 File Offset: 0x000502B4
		private bool TryLoadSimulationJournal(GameRules rules)
		{
			this._playingBackSimJournal = false;
			this._playbackDuration = -1;
			this._lastLoggedPlaybackFrame = 0;
			if (this.StartedWithGameMode == GameMode.Normal)
			{
				string simJournalPath = null;
				if (FeatureToggle.IsFeatureEnabled(Feature.SoakTest))
				{
					simJournalPath = string.Format("{0}/SoakTestJournals/{1}.bytes", Application.streamingAssetsPath, this.MapDefinition.cityName);
				}
				else if (Application.isEditor)
				{
					AppRuntime appRuntime = UnityEngine.Object.FindObjectOfType<AppRuntime>();
					simJournalPath = ((appRuntime != null) ? appRuntime._playbackSimJournalPath : null);
				}
				if (!string.IsNullOrEmpty(simJournalPath) && File.Exists(simJournalPath))
				{
					CommandJournal commands = null;
					using (BinaryReader journalReader = new BinaryReader(File.Open(simJournalPath, FileMode.Open, FileAccess.Read)))
					{
						commands = base.Scope.Import<CommandJournal>(journalReader);
					}
					bool isJournalValid = false;
					if (commands != null)
					{
						int commandIndex = 0;
						while (commandIndex < commands.EntryCount)
						{
							Command command = commands.GetEntry(commandIndex);
							if (command is InitCityCommand)
							{
								InitCityCommand initCityCommand = command as InitCityCommand;
								if (this.MapDefinition == null || this.MapDefinition.cityName != initCityCommand.CityName)
								{
									Game.Log.Warn("Not loading simulation command journal; it is for {0}, but this game has loaded {1}.", new object[]
									{
										initCityCommand.CityName,
										(this.MapDefinition != null) ? this.MapDefinition.cityName : "unknown"
									});
									break;
								}
								initCityCommand.Rules = rules;
								initCityCommand.CityDefinition = this.StartedWithCityDefinition;
								isJournalValid = true;
								break;
							}
							else
							{
								commandIndex++;
							}
						}
						if (isJournalValid)
						{
							for (int commandIndex2 = 0; commandIndex2 < commands.EntryCount; commandIndex2++)
							{
								Command command2 = commands.GetEntry(commandIndex2);
								this._simulation.ScheduleCommand(command2);
								this._playbackDuration = Mathf.Max(this._playbackDuration, command2.FrameIndex);
							}
							commands.Clear();
							if (FeatureToggle.IsFeatureEnabled(Feature.SoakTest))
							{
								Command unpauseCommand = SetPausedCommand.Create(base.Scope, false);
								unpauseCommand.FrameIndex = this._playbackDuration + 1;
								this._simulation.ScheduleCommand(unpauseCommand);
							}
							this._playingBackSimJournal = true;
							return true;
						}
						for (int commandIndex3 = 0; commandIndex3 < commands.EntryCount; commandIndex3++)
						{
							base.Scope.Release(commands.GetEntry(commandIndex3));
						}
						commands.Clear();
						Game.Log.Warn("Unable to find InitCityCommand in simulation command journal.", Array.Empty<object>());
					}
					else
					{
						Game.Log.Warn("Unable to deserialise simulation command journal from file {0}.", new object[]
						{
							simJournalPath
						});
					}
				}
			}
			return false;
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x00052324 File Offset: 0x00050524
		public LeaderboardId GetLeaderboardIdForGame()
		{
			ActiveChallengesModel challengeModel = base.Scope.Get<ActiveChallengesModel>();
			if (!challengeModel.HasChallenges)
			{
				CityGameMode gameMode = (this.StartedWithGameMode == GameMode.Expert) ? CityGameMode.Expert : CityGameMode.Normal;
				return new CityLeaderboardId(this.MapDefinition.CityNameEnum, gameMode, -1);
			}
			if (challengeModel.challengeType == MapChallenge.ChallengeType.Daily)
			{
				return new DailyLeaderboardId(challengeModel.timeStart);
			}
			if (challengeModel.challengeType == MapChallenge.ChallengeType.Weekly)
			{
				return new WeeklyLeaderboardId(challengeModel.timeStart);
			}
			if (challengeModel.challengeType == MapChallenge.ChallengeType.City)
			{
				return new CityLeaderboardId(this.MapDefinition.CityNameEnum, CityGameMode.CityChallenge, challengeModel.cityChallengeIndex);
			}
			Diagnostics.FailAssert("Invalid challenge type for leaderboardId: {0}", new object[]
			{
				challengeModel.challengeType
			});
			return null;
		}

		// Token: 0x04001328 RID: 4904
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04001329 RID: 4905
		[Dependency]
		private City _city;

		// Token: 0x0400132A RID: 4906
		[Dependency]
		private MotorwaysThemeDatabase _theme;

		// Token: 0x0400132B RID: 4907
		[Dependency]
		private NetworkConnectivityUpdater _connectivityUpdater;

		// Token: 0x0400132C RID: 4908
		[Dependency]
		private BuildingPlacer _placer;

		// Token: 0x0400132D RID: 4909
		[Dependency]
		private GameCamera _camera;

		// Token: 0x0400132E RID: 4910
		[Dependency]
		private Server.Clock _simulationClock;

		// Token: 0x0400132F RID: 4911
		[Dependency]
		private LeaderboardService _leaderboardService;

		// Token: 0x04001330 RID: 4912
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04001331 RID: 4913
		[Dependency]
		private ChallengeSystem _challengeSystem;

		// Token: 0x04001332 RID: 4914
		[Dependency]
		private Pathfinder _pathfinder;

		// Token: 0x04001333 RID: 4915
		[Dependency]
		private IAchievementHandler _achievementHandler;

		// Token: 0x04001334 RID: 4916
		private readonly IdleVehicleChecker _idleVehicleChecker = new IdleVehicleChecker();

		// Token: 0x04001335 RID: 4917
		[Dependency]
		private IDebugRenderSetManager _debugRenderSetManager;

		// Token: 0x04001336 RID: 4918
		private GameplayEventHandler _gameplayEventHandler;

		// Token: 0x04001338 RID: 4920
		private MotorwaysGameStatistics _lastRecordedStatistics;

		// Token: 0x04001339 RID: 4921
		private AchievementStatistics _achievementStatisticsAtStartOfGame;

		// Token: 0x0400133A RID: 4922
		private double _nextPulseTime = -1.0;

		// Token: 0x0400133B RID: 4923
		private AudioSync _audioSync = new AudioSync();

		// Token: 0x0400133C RID: 4924
		private AudioEnvironment _audioEnvironment;

		// Token: 0x0400133D RID: 4925
		private int _lastSnapshotFrame;

		// Token: 0x0400133E RID: 4926
		private const int SnapshotPeriod = 10;

		// Token: 0x0400133F RID: 4927
		private bool _survivedFrame;

		// Token: 0x04001340 RID: 4928
		private bool _loggedExceptionReportId;

		// Token: 0x04001341 RID: 4929
		private Diagnostics.Report _exceptionReport;

		// Token: 0x04001342 RID: 4930
		private bool _hasSubmittedExceptionReport;

		// Token: 0x04001344 RID: 4932
		private float _debugTimescale = 1f;

		// Token: 0x04001345 RID: 4933
		private int _lastLoggedPlaybackFrame;

		// Token: 0x04001346 RID: 4934
		private int _playbackDuration;

		// Token: 0x04001347 RID: 4935
		private bool _playingBackSimJournal;

		// Token: 0x04001348 RID: 4936
		private float _lastSaveRealTimeSeconds;

		// Token: 0x04001349 RID: 4937
		private Fix64 _lastSaveGameTimeHours;

		// Token: 0x0400134A RID: 4938
		private const float SaveIntervalRealTimeSeconds = 300f;

		// Token: 0x0400134B RID: 4939
		private readonly Fix64 _saveIntervalGameTimeHours = new Fix64(24);
	}
}
