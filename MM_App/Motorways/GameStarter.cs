using System;
using System.IO;
using Factory;
using Motorways.Models;
using Motorways.Views;
using Screens;
using Server;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003AA RID: 938
	public class GameStarter
	{
		// Token: 0x06001646 RID: 5702 RVA: 0x0004CA63 File Offset: 0x0004AC63
		public GameStarter(MonoBehaviour coroutineHost)
		{
			this._coroutineHost = coroutineHost;
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x0004CA79 File Offset: 0x0004AC79
		public bool StartFromSavedGame(MapLibrary mapLibrary, MotorwaysGameJournalSave save, bool replaceTopScreen = false, bool skipNextTransition = false, bool startPaused = false)
		{
			if (!this.LoadMapDefinition(mapLibrary.GetMapByName(save.CityId)))
			{
				return false;
			}
			this._save = save;
			this._replaceTopScreen = replaceTopScreen;
			this._skipTransitionIn = skipNextTransition;
			this._startPaused = startPaused;
			this._startScreen = ScreenStack.MotorwaysScreen.InGame;
			return true;
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x0004CAB7 File Offset: 0x0004ACB7
		public bool StartSavedGameFromCustomScreen(MapLibrary mapLibrary, MotorwaysGameJournalSave save, ScreenStack.MotorwaysScreen customScreen, bool skipNextTransition = false, bool startPaused = false)
		{
			if (!this.LoadMapDefinition(mapLibrary.GetMapByName(save.CityId)))
			{
				return false;
			}
			this._save = save;
			this._skipTransitionIn = skipNextTransition;
			this._startScreen = customScreen;
			this._startPaused = startPaused;
			return true;
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x0004CAEE File Offset: 0x0004ACEE
		public bool StartFromMapDefinition(MapDefinition mapDefinition, GameMode mode, float transitionInDuration = 0f, bool replaceTopScreen = false, bool startPaused = false)
		{
			if (!this.LoadMapDefinition(mapDefinition))
			{
				return false;
			}
			this._replaceTopScreen = replaceTopScreen;
			this._mode = mode;
			this._save = null;
			this._transitionInDuration = transitionInDuration;
			this._startScreen = ScreenStack.MotorwaysScreen.InGame;
			this._startPaused = startPaused;
			return true;
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x0600164A RID: 5706 RVA: 0x0004CB28 File Offset: 0x0004AD28
		public bool CanStart
		{
			get
			{
				AssetBundleUtility.AsyncLoadResult cityDefinitionLoader = this._cityDefinitionLoader;
				return cityDefinitionLoader != null && cityDefinitionLoader.HasValue;
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x0600164B RID: 5707 RVA: 0x0004CB47 File Offset: 0x0004AD47
		private bool UseCustomStartScreen
		{
			get
			{
				return this._startScreen == ScreenStack.MotorwaysScreen.Movie || this._startScreen == ScreenStack.MotorwaysScreen.CinematicMode;
			}
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x0004CB60 File Offset: 0x0004AD60
		public bool Start(ScreenStack screenStack, IScope appScope)
		{
			if (!Diagnostics.Verify(this.CanStart, "GameStarter not in a valid state to start. You should check against GameStarter.CanStart before calling this."))
			{
				return false;
			}
			if (this.UseCustomStartScreen && this._save == null)
			{
				Diagnostics.FailAssert("We can't load into a custom start screen with a fresh game!", Array.Empty<object>());
				return false;
			}
			CityDefinition cityDefinition = UnityEngine.Object.Instantiate<GameObject>(this._cityDefinitionLoader.asset as GameObject).GetComponent<CityDefinition>();
			if (!screenStack.IsScreenActive(ScreenStack.MotorwaysScreen.MapSelect))
			{
				MapSelectScreen mapSelectScreen = appScope.Get<MapSelectScreen>();
				mapSelectScreen.SavePreviouslyLockedMaps();
				appScope.Release(mapSelectScreen);
			}
			if (this._save == null)
			{
				if (this._replaceTopScreen)
				{
					screenStack.ReplaceScreenOnTop<GameContainerScreen>(ScreenStack.MotorwaysScreen.InGame, delegate(GameContainerScreen newScreen)
					{
						if (this._skipTransitionIn)
						{
							newScreen.SkipNextTransition();
						}
						else if (this._transitionInDuration > 0f)
						{
							newScreen.OverrideNextTransition(this._transitionInDuration);
						}
						newScreen.PrepareForMap(cityDefinition, this._mapDefinition, this._mode, this._mapChallenge, false);
					}, null, true);
				}
				else
				{
					IScreen transitionFromScreen = screenStack.GetTopVisibleScreen();
					if (!screenStack.IsScreenActive(ScreenStack.MotorwaysScreen.MainMenu))
					{
						screenStack.PushScreen<MainMenuScreen>(ScreenStack.MotorwaysScreen.MainMenu, false, null, true).gameObject.SetActive(false);
					}
					screenStack.PushScreen<GameContainerScreen>(ScreenStack.MotorwaysScreen.InGame, delegate(GameContainerScreen newScreen)
					{
						if (this._skipTransitionIn)
						{
							newScreen.SkipNextTransition();
						}
						else if (this._transitionInDuration > 0f)
						{
							newScreen.OverrideNextTransition(this._transitionInDuration);
						}
						newScreen.PrepareForMap(cityDefinition, this._mapDefinition, this._mode, this._mapChallenge, false);
					}, false, null, true, transitionFromScreen);
				}
				return true;
			}
			Game game = this._save.DeserializeGame(cityDefinition);
			MotorwaysGame motorwaysGame = game as MotorwaysGame;
			if (motorwaysGame != null)
			{
				motorwaysGame.FixDeserializedSimulation(cityDefinition);
				ISimulation simulation = motorwaysGame.Simulation;
				foreach (TileModel tileModel in simulation.GetModels<TileModel>())
				{
					if (tileModel.Tile.ContentType != TileContentType.None)
					{
						Vector2Int tileCoordinates = tileModel.Coordinates;
						if (cityDefinition.TileIsOverWater(tileCoordinates) || cityDefinition.TileIsUnderAMountain(tileCoordinates))
						{
							Diagnostics.FailAssert("Deserialised simulation has a tile with {0} in an invalid location.", new object[]
							{
								tileModel.Tile.ContentType
							});
							motorwaysGame = null;
							break;
						}
					}
				}
				if (motorwaysGame != null)
				{
					int maximumBuildingGroup = -1;
					foreach (HouseModel houseModel in simulation.GetModels<HouseModel>())
					{
						maximumBuildingGroup = Mathf.Max(maximumBuildingGroup, houseModel.GroupIndex);
					}
					foreach (DestinationModel destinationModel in simulation.GetModels<DestinationModel>())
					{
						maximumBuildingGroup = Mathf.Max(maximumBuildingGroup, destinationModel.GroupIndex);
					}
					int availableCityGroups = cityDefinition.schedulePlanner.scheduleGroups.Count;
					if (maximumBuildingGroup >= availableCityGroups)
					{
						Diagnostics.FailAssert("Deserialised simulation has a building with a group index of {0}, but the city only has {1}.", new object[]
						{
							maximumBuildingGroup,
							availableCityGroups
						});
						motorwaysGame = null;
					}
				}
				if (motorwaysGame != null)
				{
					bool isSimulationPaused = simulation.IsPaused;
					game.Simulation.IsPaused = true;
					try
					{
						simulation.Step();
						simulation.IsPaused = isSimulationPaused;
						game.Scope.Get<Clock>().Rewind();
					}
					catch (Exception stepException)
					{
						Diagnostics.FailAssert("Deserialised simulation failed a paused step.\n{0}", new object[]
						{
							stepException
						});
						motorwaysGame = null;
					}
					if (motorwaysGame != null && this._save.ChallengeType != MapChallenge.ChallengeType.None)
					{
						ActiveChallengesModel challengeModel = motorwaysGame.Simulation.GetModel<ActiveChallengesModel>();
						switch (this._save.ChallengeType)
						{
						case MapChallenge.ChallengeType.Daily:
							this._mapChallenge = MapChallenge.CreateDailyChallenge(game.Scope.Get<ChallengeSystem>(), this._mapDefinition, challengeModel.challenges.ToArray(), challengeModel.timeStart, challengeModel.timeEnd, challengeModel.initialSeed);
							break;
						case MapChallenge.ChallengeType.Weekly:
							this._mapChallenge = MapChallenge.CreateWeeklyChallenge(game.Scope.Get<ChallengeSystem>(), this._mapDefinition, challengeModel.challenges.ToArray(), challengeModel.timeStart, challengeModel.timeEnd, challengeModel.initialSeed);
							break;
						case MapChallenge.ChallengeType.Mystery:
							this._mapChallenge = MapChallenge.RebuildMysteryChallenge(game.Scope.Get<ChallengeSystem>(), this._mapDefinition, challengeModel.challenges.ToArray(), challengeModel.initialSeed);
							break;
						case MapChallenge.ChallengeType.City:
							this._mapChallenge = MapChallenge.CreateCityChallenge(game.Scope.Get<ChallengeSystem>(), challengeModel.cityChallengeIndex, this._mapDefinition, challengeModel.challenges.ToArray(), challengeModel.initialSeed);
							break;
						default:
							Diagnostics.FailAssert(string.Format("Invalid ChallengeType for game save: {0}", this._save.ChallengeType), Array.Empty<object>());
							break;
						}
					}
				}
			}
			if (motorwaysGame == null)
			{
				if (FeatureToggle.IsFeatureEnabled(Feature.DiagnosticReports))
				{
					Diagnostics.Report report = new Diagnostics.Report();
					report.Motive = "deserializeException";
					report.SetMetadata("buildName", global::Version.Name, true);
					report.SetMetadata("buildTimestamp", global::Version.Timestamp.ToString(), true);
					if (!string.IsNullOrEmpty(global::Version.CommitHash))
					{
						report.SetMetadata("commitHash", global::Version.CommitHash, true);
					}
					report.SetMetadata("city", this._save.CityId, true);
					report.SetMetadata("gameMode", this._save.Mode.ToString(), true);
					MemoryStream saveStream = new MemoryStream();
					using (BinaryWriter saveWriter = new BinaryWriter(saveStream))
					{
						this._save.OnSerializeBeforeData(saveWriter);
						saveWriter.Write(this._save.GetBytesForSerializing());
					}
					report.AttachFile("simulation.gamejournal", saveStream.ToArray());
					report.Upload();
				}
				UnityEngine.Object.Destroy(cityDefinition.gameObject);
				if (game != null)
				{
					game.Scope.Release(game);
				}
				return false;
			}
			if (this._replaceTopScreen)
			{
				screenStack.ReplaceScreenOnTop<BaseScalingScreen>(this._startScreen, delegate(BaseScalingScreen newScreen)
				{
					if (this._skipTransitionIn)
					{
						newScreen.SkipNextTransition();
					}
					else if (this._transitionInDuration > 0f)
					{
						newScreen.OverrideNextTransition(this._transitionInDuration);
					}
					IGameStartScreen startScreen = newScreen as IGameStartScreen;
					if (startScreen != null)
					{
						startScreen.PrepareForNewGame(cityDefinition, this._mapDefinition, motorwaysGame, this._mapChallenge, this._startPaused);
						return;
					}
					Diagnostics.FailAssert(string.Format("GameStarter attempting to start with unsupported ScreenStack.MotorwaysScreen: {0}", this._startScreen), Array.Empty<object>());
				}, motorwaysGame.Scope, true);
			}
			else
			{
				screenStack.PushScreen<BaseScalingScreen>(this._startScreen, delegate(BaseScalingScreen newScreen)
				{
					if (this._skipTransitionIn)
					{
						newScreen.SkipNextTransition();
					}
					else if (this._transitionInDuration > 0f)
					{
						newScreen.OverrideNextTransition(this._transitionInDuration);
					}
					IGameStartScreen startScreen = newScreen as IGameStartScreen;
					if (startScreen != null)
					{
						startScreen.PrepareForNewGame(cityDefinition, this._mapDefinition, motorwaysGame, this._mapChallenge, this._startPaused);
						return;
					}
					Diagnostics.FailAssert(string.Format("GameStarter attempting to start with unsupported ScreenStack.MotorwaysScreen: {0}", this._startScreen), Array.Empty<object>());
				}, false, motorwaysGame.Scope, true, null);
			}
			return true;
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x0004D154 File Offset: 0x0004B354
		private bool LoadMapDefinition(MapDefinition mapDefinition)
		{
			if (!Diagnostics.Verify(this._cityDefinitionLoader == null, "City definition loader isn't null!"))
			{
				return false;
			}
			if (Diagnostics.Verify(mapDefinition != null))
			{
				this._mapDefinition = mapDefinition;
			}
			this._cityDefinitionLoader = AssetBundleUtility.LoadPrefabAsync(this._mapDefinition.mapAssetBundle, this._mapDefinition.mapPrefabName, this._coroutineHost);
			return true;
		}

		// Token: 0x040012F9 RID: 4857
		private MonoBehaviour _coroutineHost;

		// Token: 0x040012FA RID: 4858
		private AssetBundleUtility.AsyncLoadResult _cityDefinitionLoader;

		// Token: 0x040012FB RID: 4859
		private MapDefinition _mapDefinition;

		// Token: 0x040012FC RID: 4860
		private MapChallenge _mapChallenge;

		// Token: 0x040012FD RID: 4861
		private GameMode _mode;

		// Token: 0x040012FE RID: 4862
		private MotorwaysGameJournalSave _save;

		// Token: 0x040012FF RID: 4863
		private bool _replaceTopScreen;

		// Token: 0x04001300 RID: 4864
		private bool _skipTransitionIn;

		// Token: 0x04001301 RID: 4865
		private bool _startPaused;

		// Token: 0x04001302 RID: 4866
		private float _transitionInDuration;

		// Token: 0x04001303 RID: 4867
		private ScreenStack.MotorwaysScreen _startScreen = ScreenStack.MotorwaysScreen.InGame;
	}
}
