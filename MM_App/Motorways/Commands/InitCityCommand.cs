using System;
using Factory;
using Motorways.Models;
using Motorways.Processes;
using Server;
using UnityEngine;

namespace Motorways.Commands
{
	// Token: 0x0200051E RID: 1310
	[Factory.Serializable(1)]
	public class InitCityCommand : Command
	{
		// Token: 0x060022A7 RID: 8871 RVA: 0x0008C231 File Offset: 0x0008A431
		public void Initialize(string cityName, CityDefinition cityDefinition, GameMode mode, GameRules rules, ulong seed)
		{
			this._cityName = cityName;
			this._cityDefinition = cityDefinition;
			this._mode = mode;
			this._rules = rules;
			this._seed = seed;
		}

		// Token: 0x060022A8 RID: 8872 RVA: 0x0008C258 File Offset: 0x0008A458
		private void SetChallengeData(MapChallenge.ChallengeType challengeType, int cityChallengeIndex, ChallengeData[] challenges, int timeStart, int timeEnd)
		{
			this._challengeType = challengeType;
			this._cityChallengeIndex = cityChallengeIndex;
			this._challenges = challenges;
			this._challengeTimeStart = timeStart;
			this._challengeTimeEnd = timeEnd;
		}

		// Token: 0x060022A9 RID: 8873 RVA: 0x0008C280 File Offset: 0x0008A480
		public override void Reset()
		{
			base.Reset();
			this._cityName = "";
			this._cityDefinition = null;
			this._mode = GameMode.Normal;
			this._rules = null;
			this._seed = 0UL;
			this._challengeType = MapChallenge.ChallengeType.None;
			this._cityChallengeIndex = -1;
			this._challenges = null;
			this._challengeTimeStart = 0;
			this._challengeTimeEnd = 0;
		}

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x060022AA RID: 8874 RVA: 0x0008C2DE File Offset: 0x0008A4DE
		// (set) Token: 0x060022AB RID: 8875 RVA: 0x0008C2E6 File Offset: 0x0008A4E6
		public CityDefinition CityDefinition
		{
			get
			{
				return this._cityDefinition;
			}
			set
			{
				if (Diagnostics.Verify(this._cityDefinition == null, "Only set CityDefinition manually to fix up a deserialized InitCityCommand."))
				{
					this._cityDefinition = value;
				}
			}
		}

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x060022AC RID: 8876 RVA: 0x0008C307 File Offset: 0x0008A507
		public string CityName
		{
			get
			{
				return this._cityName;
			}
		}

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x060022AD RID: 8877 RVA: 0x0008C30F File Offset: 0x0008A50F
		// (set) Token: 0x060022AE RID: 8878 RVA: 0x0008C317 File Offset: 0x0008A517
		public GameRules Rules
		{
			get
			{
				return this._rules;
			}
			set
			{
				if (Diagnostics.Verify(this._rules == null, "Only set GameRules manually to fix up a deserialized InitCityCommand."))
				{
					this._rules = value;
				}
			}
		}

		// Token: 0x060022AF RID: 8879 RVA: 0x0008C338 File Offset: 0x0008A538
		public override void Execute(ISimulation simulation)
		{
			CityModel cityModel = simulation.Scope.Get<CityModel>();
			cityModel.cityName = this._cityName;
			cityModel.StartGameInMode(this._mode, this._rules);
			cityModel.pseudorandomGenerator = this._scope.Get<PseudorandomGenerator>();
			cityModel.pseudorandomGenerator.Seed = this._seed;
			simulation.AddModel(cityModel);
			if (this._cityDefinition != null)
			{
				this._city.Initialize(this._cityDefinition, this._rules);
			}
			simulation.AddProcess(this._scope.Get<ClockProcess>());
			simulation.AddProcess(this._scope.Get<BuildingSpawningProcess>());
			simulation.AddProcess(this._scope.Get<BuildMotorwaysProcess>());
			simulation.AddProcess(this._scope.Get<BuildRoundaboutsProcess>());
			simulation.AddProcess(this._scope.Get<LaneUpdateProcess>());
			simulation.AddProcess(this._scope.Get<GenerateDemandProcess>());
			simulation.AddProcess(this._scope.Get<DispatchVehiclesProcess>());
			simulation.AddProcess(this._scope.Get<TrafficLightAlternatingProcess>());
			simulation.AddProcess(this._scope.Get<VehiclePathfindingProcess>());
			simulation.AddProcess(this._scope.Get<IntersectionEvaluatingProcess>());
			simulation.AddProcess(this._scope.Get<TrainSpawningProcess>());
			simulation.AddProcess(this._scope.Get<BoatSpawningProcess>());
			simulation.AddProcess(this._scope.Get<TrainMovementProcess>());
			simulation.AddProcess(this._scope.Get<BoatMovementProcess>());
			simulation.AddProcess(this._scope.Get<OpenTrainCrossingsProcess>());
			simulation.AddProcess(this._scope.Get<VehicleSpawningProcess>());
			simulation.AddProcess(this._scope.Get<VehicleMovementProcess>());
			simulation.AddProcess(this._scope.Get<ParkVehiclesProcess>());
			simulation.AddProcess(this._scope.Get<TutorialProgressionProcess>());
			simulation.AddProcess(this._scope.Get<ReleaseMothballedLanesProcess>());
			simulation.AddProcess(this._scope.Get<ReleaseMotorwaysProcess>());
			simulation.AddProcess(this._scope.Get<TilePermanenceUpdatingProcess>());
			simulation.AddProcess(this._scope.Get<EfficiencyCalculationProcess>());
			simulation.AddProcess(this._scope.Get<FailureStateProcess>());
			simulation.AddProcess(this._scope.Get<UpgradeAwardingProcess>());
			simulation.AddProcess(this._scope.Get<AchievementCheckingProcess>());
			CityPlanModel cityPlanModel = simulation.Scope.Get<CityPlanModel>();
			simulation.AddModel(cityPlanModel);
			DemandModel demandModel = simulation.Scope.Get<DemandModel>();
			simulation.AddModel(demandModel);
			TilemapModel tilemapModel = simulation.Scope.Get<TilemapModel>();
			simulation.AddModel(tilemapModel);
			ClockModel clock = simulation.Scope.Get<ClockModel>();
			simulation.AddModel(clock);
			ScoreModel score = simulation.Scope.Get<ScoreModel>();
			simulation.AddModel(score);
			if (FeatureToggle.IsFeatureEnabled(Feature.ValidateSimulationDeterminism))
			{
				SnapshotModel snapshotModel = simulation.Scope.Get<SnapshotModel>();
				simulation.AddModel(snapshotModel);
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.RecordIntersectionDecisions))
			{
				IntersectionDecisionDatabaseModel decisionDatabaseModel = simulation.Scope.Get<IntersectionDecisionDatabaseModel>();
				simulation.AddModel(decisionDatabaseModel);
			}
			ActiveChallengesModel activeChallengesModel = simulation.Scope.Get<ActiveChallengesModel>();
			if (this._challengeType != MapChallenge.ChallengeType.None && this._rules.SupportsChallenges())
			{
				activeChallengesModel.challengeType = this._challengeType;
				activeChallengesModel.cityChallengeIndex = this._cityChallengeIndex;
				activeChallengesModel.timeEnd = this._challengeTimeEnd;
				activeChallengesModel.timeStart = this._challengeTimeStart;
				activeChallengesModel.challenges.AddRange(this._challenges);
				activeChallengesModel.initialSeed = this._seed;
			}
			else if (!(this._cityDefinition is MockCityDefinition) && this._challengeDatabase.debugInjectedChallenges != null && this._challengeDatabase.debugInjectedChallenges.Count > 0 && this._rules.SupportsChallenges() && FeatureToggle.IsFeatureEnabled(Feature.InjectDebugChallenges))
			{
				activeChallengesModel.challenges.AddRange(this._challengeDatabase.debugInjectedChallenges);
			}
			UpgradeDatabaseModel upgradeDatabase = simulation.Scope.Get<UpgradeDatabaseModel>();
			upgradeDatabase.AwardStartingPackages();
			simulation.AddModel(upgradeDatabase);
			simulation.AddModel(activeChallengesModel);
			GameBehaviourModel gameBehaviourModel = simulation.Scope.Get<GameBehaviourModel>();
			simulation.AddModel(gameBehaviourModel);
			if (gameBehaviourModel.OverridesGameModeWithExpert() && this._mode != GameMode.Expert)
			{
				this._game.ContinueInMode(GameMode.Expert);
			}
			if (this._cityDefinition != null)
			{
				cityModel.startOffset = this._cityDefinition.GenerateCityStartOffset(cityModel.pseudorandomGenerator);
				Command.Log.Info("Generated start offset {0}.", new object[]
				{
					cityModel.startOffset
				});
				this._placer.SetTileData(this._city.Definition.TileWeightData);
				this._city.GenerateCityLayout();
				this._city.SetupTrainNetwork(simulation);
				this._city.SetupBoatPathNetwork(simulation);
				this._city.PopulateTrees(simulation);
				if (this._city.Definition.bonusTreeGrassObjects != null)
				{
					GameObject[] bonusTreeGrassObjects = this._city.Definition.bonusTreeGrassObjects;
					for (int i = 0; i < bonusTreeGrassObjects.Length; i++)
					{
						bonusTreeGrassObjects[i].SetActive(gameBehaviourModel.UsesBonusTrees);
					}
				}
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.WhatTheCarEasterEgg))
			{
				EasterEggModel easterEggModel = simulation.Scope.Get<EasterEggModel>();
				simulation.AddModel(easterEggModel);
			}
		}

		// Token: 0x060022B0 RID: 8880 RVA: 0x0008C860 File Offset: 0x0008AA60
		public static InitCityCommand CreateNormalCity(IScope scope, string cityName, CityDefinition cityDefinition, GameMode mode, GameRules rules, uint seed)
		{
			InitCityCommand initCityCommand = scope.Get<InitCityCommand>();
			initCityCommand.Initialize(cityName, cityDefinition, mode, rules, (ulong)seed);
			return initCityCommand;
		}

		// Token: 0x060022B1 RID: 8881 RVA: 0x0008C878 File Offset: 0x0008AA78
		public static InitCityCommand CreateChallengeCity(IScope scope, string cityName, CityDefinition cityDefinition, GameMode mode, GameRules rules, MapChallenge mapChallenge)
		{
			InitCityCommand initCityCommand = scope.Get<InitCityCommand>();
			initCityCommand.Initialize(cityName, cityDefinition, mode, rules, mapChallenge.seed);
			initCityCommand.SetChallengeData(mapChallenge.type, mapChallenge.cityChallengeIndex, mapChallenge.challenges, mapChallenge.TimeStart, mapChallenge.TimeEnd);
			return initCityCommand;
		}

		// Token: 0x04001CBD RID: 7357
		[Dependency]
		private City _city;

		// Token: 0x04001CBE RID: 7358
		[Dependency]
		private IScope _scope;

		// Token: 0x04001CBF RID: 7359
		[Dependency]
		private BuildingPlacer _placer;

		// Token: 0x04001CC0 RID: 7360
		[Dependency]
		private ChallengeDatabase _challengeDatabase;

		// Token: 0x04001CC1 RID: 7361
		[Dependency]
		private MotorwaysGame _game;

		// Token: 0x04001CC2 RID: 7362
		private string _cityName;

		// Token: 0x04001CC3 RID: 7363
		[Serialize(false, null)]
		private CityDefinition _cityDefinition;

		// Token: 0x04001CC4 RID: 7364
		private GameMode _mode;

		// Token: 0x04001CC5 RID: 7365
		[Serialize(false, null)]
		private GameRules _rules;

		// Token: 0x04001CC6 RID: 7366
		private ulong _seed;

		// Token: 0x04001CC7 RID: 7367
		private MapChallenge.ChallengeType _challengeType;

		// Token: 0x04001CC8 RID: 7368
		private int _cityChallengeIndex = -1;

		// Token: 0x04001CC9 RID: 7369
		private ChallengeData[] _challenges;

		// Token: 0x04001CCA RID: 7370
		private int _challengeTimeStart;

		// Token: 0x04001CCB RID: 7371
		private int _challengeTimeEnd;
	}
}
