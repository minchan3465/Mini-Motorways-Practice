using System;
using Client;
using Factory;
using FixMath;
using Motorways.Models;
using Motorways.Views;
using Motorways.Views.Boats;
using Motorways.Views.MeshGeneration;
using Motorways.Views.Trains;
using Server;

namespace Motorways
{
	// Token: 0x020003B3 RID: 947
	public class MotorwaysClient : ViewClient
	{
		// Token: 0x06001687 RID: 5767 RVA: 0x00050440 File Offset: 0x0004E640
		public override void Start()
		{
			base.Start();
			if (FeatureToggle.IsFeatureEnabled(Feature.InGameDevTools) && this._motorwaysGame.StartedWithGameMode != GameMode.Background)
			{
				this._devToolsRegistry.RegisterTools();
			}
			base.RegisterViewBuilder<TilemapModel>(new TilemapView.Builder());
			base.RegisterViewBuilder<ClockModel>(new ClockView.Builder());
			base.RegisterViewBuilder<ScoreModel>(new ScoreView.Builder());
			base.RegisterViewBuilder<VehicleModel>(new VehicleView.Builder());
			base.RegisterViewBuilder<HouseModel>(new HouseView.Builder());
			base.RegisterViewBuilder<TreeModel>(new TreeView.Builder());
			base.RegisterViewBuilder<DestinationModel>(new DestinationView.Builder());
			base.RegisterViewBuilder<CarparkModel>(new CarparkView.Builder());
			base.RegisterViewBuilder<RailTileModel>(new RailView.Builder());
			base.RegisterViewBuilder<TrainModel>(new TrainView.Builder());
			base.RegisterViewBuilder<TrainCrossingModel>(new TrainCrossingView.Builder());
			base.RegisterViewBuilder<BoatPathTileModel>(new BoatPathView.Builder());
			base.RegisterViewBuilder<BoatModel>(new BoatView.Builder());
			base.RegisterViewBuilder<AnchoredMessageModel>(new AnchoredMessageView.Builder());
			this._upgradeDatabase = base.Scope.Get<ClientUpgradeDatabase>();
			base.RegisterViewBuilder<UpgradeDatabaseModel>(new MotorwaysClient.UpgradeDatabaseConnector(this));
			base.AddView(base.Scope.Get<CameraView>());
			base.AddView(base.Scope.Get<NotificationView>());
			base.AddView(base.Scope.Get<ChallengeView>());
			base.AddView(base.Scope.Get<BuildingsIndicatorView>());
			base.AddView(base.Scope.Get<CombinedMeshView>());
			base.AddView(base.Scope.Get<CitySpawningView>());
			this._combinedMeshThemeComponent = base.Scope.Get<CombinedMeshThemeComponent>();
			base.AddThemeComponent(this._combinedMeshThemeComponent);
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x000505AF File Offset: 0x0004E7AF
		public override void Tick(TimeInterval timeInterval, float stepAlpha)
		{
			base.Tick(timeInterval, stepAlpha);
			this._devToolsRegistry.RespondToInGameToolUse();
		}

		// Token: 0x06001689 RID: 5769 RVA: 0x000505C4 File Offset: 0x0004E7C4
		public override void OnReleasedFromScope(IScope scope)
		{
			if (this._upgradeDatabase != null)
			{
				base.Scope.Release(this._upgradeDatabase);
				this._upgradeDatabase = null;
			}
			if (this._combinedMeshThemeComponent != null)
			{
				base.Scope.Release(this._combinedMeshThemeComponent);
				this._combinedMeshThemeComponent = null;
			}
			base.OnReleasedFromScope(scope);
		}

		// Token: 0x0400131D RID: 4893
		private CombinedMeshThemeComponent _combinedMeshThemeComponent;

		// Token: 0x0400131E RID: 4894
		private ClientUpgradeDatabase _upgradeDatabase;

		// Token: 0x0400131F RID: 4895
		[Dependency]
		protected IInGameDevToolsRegistry _devToolsRegistry;

		// Token: 0x04001320 RID: 4896
		[Dependency]
		private MotorwaysGame _motorwaysGame;

		// Token: 0x020003B4 RID: 948
		public class UpgradeDatabaseConnector : IViewBuilder
		{
			// Token: 0x0600168B RID: 5771 RVA: 0x00050622 File Offset: 0x0004E822
			public UpgradeDatabaseConnector(MotorwaysClient client)
			{
				this._client = client;
			}

			// Token: 0x0600168C RID: 5772 RVA: 0x00050631 File Offset: 0x0004E831
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				this._client._upgradeDatabase.Initialize(model as UpgradeDatabaseModel);
			}

			// Token: 0x04001321 RID: 4897
			private MotorwaysClient _client;
		}
	}
}
