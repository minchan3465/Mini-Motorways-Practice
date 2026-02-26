using System;
using Factory;
using Factory.Pools;
using Motorways.Models;
using Motorways.Views;
using NotificationService.Events;
using Server;

namespace Motorways
{
	// Token: 0x020003A7 RID: 935
	public class GameplayEventHandler : IReusable, DestinationModel.IObserver
	{
		// Token: 0x06001639 RID: 5689 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x0600163A RID: 5690 RVA: 0x0004C810 File Offset: 0x0004AA10
		public void Tick(MotorwaysGame motorwaysGame)
		{
			if (this._city.Rules.ShowsUI())
			{
				UpgradeDatabaseModel upgrades = this._simulation.GetModel<UpgradeDatabaseModel>();
				bool showUpgradeScreen = !motorwaysGame.HasGameEnded && upgrades != null && upgrades.pendingUpgradeChoices.Count > upgrades.numChoicesMade && this._screenStack.IsInGame() && !this._screenStack.HasPendingScreen() && !this._screenStack.AreAnyScreensTransitioning && this._gameUIScreen != null && this._gameUIScreen.UpgradeBar != null && this._gameUIScreen.UpgradeBar.IsVisible && (this._city.Rules.ScoringMode != ScoringMode.EfficiencyMilestones || this._gameUIScreen.IsElectiveUpgradeRequested) && this._city.Rules.ScoringMode != ScoringMode.None;
				if (motorwaysGame.PlayingBackSimJournal)
				{
					showUpgradeScreen = false;
				}
				if (showUpgradeScreen)
				{
					motorwaysGame.TrySave(GameJournalMotive.Autosave);
					this.ShowUpgradeScreen();
				}
			}
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x0004C918 File Offset: 0x0004AB18
		private void EndGame(DestinationView failedOnDestination)
		{
			ScreenStack screenStack = this._scope.Get<ScreenStack>();
			if (!screenStack.IsScreenActive(ScreenStack.MotorwaysScreen.GameOver))
			{
				this._feedbackGenerator.GenerateFeedback(HapticFeedbackType.HeavyImpact);
				this._notificationEventSystem.RecordEvent(new GameOvered
				{
					Map = this._scope.Get<MotorwaysGame>().MapDefinition.CityNameEnum
				}, true);
				screenStack.PushScreen<GameOverScreen>(ScreenStack.MotorwaysScreen.GameOver, delegate(GameOverScreen gameOverScreen)
				{
					gameOverScreen.focusPoint = failedOnDestination.transform.position;
				}, true, this._scope, true, null);
			}
		}

		// Token: 0x0600163C RID: 5692 RVA: 0x0004C99C File Offset: 0x0004AB9C
		private void ShowUpgradeScreen()
		{
			UpgradeDatabaseModel upgrades = this._simulation.GetModel<UpgradeDatabaseModel>();
			this._screenStack.PushScreen<GameUpgradeScreen>(ScreenStack.MotorwaysScreen.Upgrade, delegate(GameUpgradeScreen screen)
			{
				screen.SetNextButtonOptions(upgrades.pendingUpgradeChoices[0], 0f, -1);
			}, true, this._simulation.Scope, true, null).ApplyTheme(this._theme.TargetTheme);
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x0004C9F8 File Offset: 0x0004ABF8
		public void OnDestinationOvercrowded(DestinationModel destination)
		{
			if (!this._city.Rules.CanDestinationsOvercrowd)
			{
				return;
			}
			DestinationView view = this._viewIndex.GetDestinationView(destination);
			this.EndGame(view);
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnDestinationReceivedVehicle(DestinationModel destination, VehicleModel vehicle)
		{
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnDestinationChangedGroup(DestinationModel destination, int oldGroupIndex, int newGroupIndex)
		{
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnDestinationRemoved(DestinationModel destination)
		{
		}

		// Token: 0x040012EE RID: 4846
		[Dependency]
		private INotificationEventSystem _notificationEventSystem;

		// Token: 0x040012EF RID: 4847
		[Dependency]
		private HapticFeedbackGenerator _feedbackGenerator;

		// Token: 0x040012F0 RID: 4848
		[Dependency]
		private IScope _scope;

		// Token: 0x040012F1 RID: 4849
		[Dependency]
		private ScreenStack _screenStack;

		// Token: 0x040012F2 RID: 4850
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x040012F3 RID: 4851
		[Dependency]
		private MotorwaysThemeDatabase _theme;

		// Token: 0x040012F4 RID: 4852
		[Dependency]
		private ViewIndex _viewIndex;

		// Token: 0x040012F5 RID: 4853
		[Dependency]
		private City _city;

		// Token: 0x040012F6 RID: 4854
		[Dependency]
		private GameUIScreen _gameUIScreen;
	}
}
