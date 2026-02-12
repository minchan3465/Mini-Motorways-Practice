using System;
using System.Collections.Generic;
using Factory;
using Motorways.Audio;
using NaughtyAttributes;
using Notifications;
using NotificationService.Events;
using Popups;
using Screens;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x0200054F RID: 1359
	public class MainMenuScreen : BaseScalingScreen, InputState.IObserver
	{
		// Token: 0x06002429 RID: 9257 RVA: 0x00095AF4 File Offset: 0x00093CF4
		public override void OnCreatedInScope(IScope scope)
		{
			base.OnCreatedInScope(scope);
			this._gameCenterAccessPointButton.Initialise(scope);
			if (this._softwareCapabilities.SupportsEvergreenButton)
			{
				List<NewsAndNotificationObject> notifications = this._newsAndNotificationData.GetNotifications(this._hardwareCapabilities.Platform);
				if (notifications.Count > 0)
				{
					this._currentNewsAndNotificationObject = notifications[0];
				}
				this._evergreenButton.gameObject.SetActive(this._currentNewsAndNotificationObject != null);
				return;
			}
			this._evergreenButton.gameObject.SetActive(false);
		}

		// Token: 0x0600242A RID: 9258 RVA: 0x00095B7C File Offset: 0x00093D7C
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			this.EnsureOptionsMessageTabNCISetup();
			base.TransitionIn(outScreen);
			this._exitButton.gameObject.SetActive(this._hardwareCapabilities.SupportsManualExit);
			this._profileSelectButton.image.sprite = this._visualConstants.GetProfileIcon(this._activePlayer.AvatarIconIndex);
			this._profileSelectBackground.color = this._themeDatabase.GetGlobalColor(ProfileCreationScreen.GetProfileColorEnumForIndex(this._activePlayer.AvatarColorIndex));
			if (this._softwareCapabilities.SupportsMultipleProfiles || FeatureToggle.IsFeatureEnabled(Feature.ProfileSelectScreen))
			{
				this._profileSelectButton.gameObject.SetActive(true);
			}
			else
			{
				this._profileSelectButton.gameObject.SetActive(false);
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				this._player.MotorwaysUserProfile.ClearCityStatistics();
				this._tutorialButton.SetActive(true);
				this._optionsButton.SetActive(false);
			}
			else
			{
				this._tutorialButton.SetActive(false);
				this._optionsButton.SetActive(true);
			}
			if (this._evergreenButton != null && this._currentNewsAndNotificationObject != null)
			{
				this._evergreenButton.SetNewContentID(this._currentNewsAndNotificationObject.ContentIndicatorID, true, true);
				this._evergreenButton.ShowNewContentIndicatorIfNeeded(false);
			}
			this._player.DataChanged += this.UpdateResumeButtonState;
			this._player.SavedGamesChanged += this.UpdateResumeButtonState;
			this.UpdateResumeButtonState();
			if (FeatureToggle.IsFeatureEnabled(Feature.SoakTest))
			{
				this.OnPlay();
			}
		}

		// Token: 0x0600242B RID: 9259 RVA: 0x00095D04 File Offset: 0x00093F04
		public override void OnTransitionedIn()
		{
			base.OnTransitionedIn();
			this._softwareCapabilities.SetIsInMainMenuScreen(true);
			this._gameCenterAccessPointButton.Show();
			if (!FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				if (this.ShouldShowFTUXAccessibilityForSkipTransitions())
				{
					this.ShowFTUXAccessibilityForSkipTransitions();
				}
				else if (this.ShouldShowNotificationsPermissionsFlow())
				{
					this.ShowLocalNotificationPrePermissionPopup();
				}
				else if (this.ShouldShowUpdatedControllerSchemePopup())
				{
					this.ShowUpdatedControllerSchemePopup();
				}
				else if (this.ShouldShowUpdatedColorblindPopup())
				{
					this.ShowUpdatedColorblindPopup();
				}
				else
				{
					this._activePlayer.SetNewContentSeen("SkipTransitionsFTUXMessageFirstVisit");
				}
			}
			foreach (MainMenuScreen.IObserver observer in this.Observers)
			{
				observer.OnMainMenuTransitionedIn();
			}
			if ((this._storageService.Status.issues & PersistentStorageServiceIssues.RecentUnauthenticatedData) > PersistentStorageServiceIssues.None)
			{
				this.ShowiCloudUnauthenticated();
			}
		}

		// Token: 0x0600242C RID: 9260 RVA: 0x00095DC7 File Offset: 0x00093FC7
		protected virtual void UpdateResumeButtonState()
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				this.resumeButton.SetActive(false);
				return;
			}
			this.resumeButton.SetActive(this._player.HasLocalSavedGame || this._player.HasForeignSavedGames);
		}

		// Token: 0x0600242D RID: 9261 RVA: 0x00095E08 File Offset: 0x00094008
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			base.TransitionOut(inScreen);
			this._player.DataChanged -= this.UpdateResumeButtonState;
			this._softwareCapabilities.SetIsInMainMenuScreen(false);
			this._gameCenterAccessPointButton.Hide();
			foreach (MainMenuScreen.IObserver observer in this.Observers)
			{
				observer.OnMainMenuTransitionOut();
			}
		}

		// Token: 0x0600242E RID: 9262 RVA: 0x00095E6E File Offset: 0x0009406E
		public void OnPlay()
		{
			if (this.popupStack.HasVisiblePopups)
			{
				return;
			}
			this._screenStack.PushScreen<MapSelectScreen>(ScreenStack.MotorwaysScreen.MapSelect, delegate(MapSelectScreen screen)
			{
				screen.PrepareScreen(null, false, false);
			}, false, null, true, null);
		}

		// Token: 0x0600242F RID: 9263 RVA: 0x00095EAE File Offset: 0x000940AE
		public void OnOptions()
		{
			this._screenStack.PushScreen(ScreenStack.MotorwaysScreen.OptionsMain, false, null, true);
		}

		// Token: 0x06002430 RID: 9264 RVA: 0x00095EC0 File Offset: 0x000940C0
		public void OnResumeGame()
		{
			if (this._gameStarter == null && (this._player.HasLocalSavedGame || this._player.HasForeignSavedGames))
			{
				bool enterResumeScreen = this._player.HasForeignSavedGames;
				if (FeatureToggle.IsFeatureEnabled(Feature.AlwaysEnterResumeScreen))
				{
					enterResumeScreen = true;
				}
				if (!enterResumeScreen)
				{
					MotorwaysGameJournalSave localSave = (MotorwaysGameJournalSave)this._player.LocalSavedGame;
					if (localSave != null)
					{
						this._gameStarter = new GameStarter(this);
						this._gameStarter.StartFromSavedGame(this._mapDatabase.MapLibrary, localSave, false, false, false);
						this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.MenuExit, 0.5f, -1f, true, null));
						if (this._skipTransitions)
						{
							this._screenStack.FadeNextTransition(this.skippedTransitionFadeDuration);
							return;
						}
					}
				}
				else
				{
					this._screenStack.PushScreen(ScreenStack.MotorwaysScreen.ResumeGame, false, null, true);
				}
			}
		}

		// Token: 0x06002431 RID: 9265 RVA: 0x00095FA8 File Offset: 0x000941A8
		public void OnTutorial()
		{
			if (this.popupStack.HasVisiblePopups)
			{
				return;
			}
			StartupScreen startupScreen = this._screenStack.GetActiveScreen<StartupScreen>();
			if (Diagnostics.Verify(startupScreen != null, "Unable to find StartupScreen, it should always be present.") && Diagnostics.Verify(startupScreen.tutorialDefinition != null, startupScreen, "StartupScreen does not have an assigned tutorial definition"))
			{
				this._gameStarter = new GameStarter(this);
				if (this._gameStarter.StartFromMapDefinition(startupScreen.tutorialDefinition, GameMode.Tutorial, 0f, false, false))
				{
					this._analytics.TrackTutorialStarted(true);
					this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.MenuExit, 0.5f, -1f, true, null));
					return;
				}
				this._gameStarter = null;
			}
		}

		// Token: 0x06002432 RID: 9266 RVA: 0x00096067 File Offset: 0x00094267
		private void EnsureOptionsMessageTabNCISetup()
		{
			if (!this._activePlayer.HasSeenNewContent("OptionsScreenMessagePreNCI"))
			{
				this._activePlayer.SetNewContentSeen("OptionsScreenMessagePreNCI");
				this._activePlayer.SetNewContentSeen("OptionsScreenMessageTab");
			}
		}

		// Token: 0x06002433 RID: 9267 RVA: 0x0009609C File Offset: 0x0009429C
		public bool ShouldShowNotificationsPermissionsFlow()
		{
			if (!this._systemNotificationService.IsAvailable)
			{
				return false;
			}
			if (this._systemNotificationService.AuthorizationStatus != AuthorizationStatus.NotDetermined)
			{
				return false;
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.SkipGameCountAndViewedCheckForPermissionPopup))
			{
				return true;
			}
			if (this._activePlayer.HasSeenNewContent("LocalNotificationsPermissionRequest"))
			{
				return false;
			}
			int gameCount = 0;
			foreach (NotificationEvent notificationEvent in this._notificationEvents.AllEvents)
			{
				if (notificationEvent.EventType is GameOvered)
				{
					gameCount++;
				}
				if (gameCount >= 2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002434 RID: 9268 RVA: 0x0009614C File Offset: 0x0009434C
		public void ShowLocalNotificationPrePermissionPopup()
		{
			this._activePlayer.SetNewContentSeen("LocalNotificationsPermissionRequest");
			this.popupStack.PushConfirmationPopup<ConfirmationPopup>(StringId.Local_Notifications_PermissionsRequest_Title, new Action(this.OnPrePermissionDenied), new Action(this.OnPrePermissionGranted), StringId.Local_Notifications_PermissionsRequest_Description);
		}

		// Token: 0x06002435 RID: 9269 RVA: 0x0009618C File Offset: 0x0009438C
		private bool ShouldShowUpdatedControllerSchemePopup()
		{
			return !this._activePlayer.HasSeenNewContent("NewControllerSchemePopup") && this._activePlayer.IsAnyTutorialCompleted && this._inputState.CurrentDeviceInputType == DeviceInputType.Controller;
		}

		// Token: 0x06002436 RID: 9270 RVA: 0x000961BF File Offset: 0x000943BF
		private void ShowUpdatedControllerSchemePopup()
		{
			this._activePlayer.SetNewContentSeen("NewControllerSchemePopup");
			this.popupStack.PushConfirmationPopup<ConfirmationPopup>(StringId.NewControllerScheme_Title, null, new Action(this.OnTutorial), StringId.NewControllerScheme_Description);
		}

		// Token: 0x06002437 RID: 9271 RVA: 0x000961F4 File Offset: 0x000943F4
		private bool ShouldShowUpdatedColorblindPopup()
		{
			return !this._activePlayer.HasSeenNewContent("NewColorblindPopup") && this._activePlayer.IsAnyTutorialCompleted && this._activePlayer.IsColorblindModeEnabled;
		}

		// Token: 0x06002438 RID: 9272 RVA: 0x00096224 File Offset: 0x00094424
		private void ShowUpdatedColorblindPopup()
		{
			this._activePlayer.SetNewContentSeen("NewColorblindPopup");
			this.popupStack.PushConfirmationPopup<ConfirmationPopup>(StringId.NewColorblindPicker_Title, null, new Action(this.OnOptions), StringId.NewColorblindPicker_Description);
		}

		// Token: 0x06002439 RID: 9273 RVA: 0x00096259 File Offset: 0x00094459
		public override void OnCurrentDeviceInputTypeChanged(DeviceInputType newInputType)
		{
			if (this._screenStack.GetTopVisibleScreen() is MainMenuScreen && this.ShouldShowUpdatedControllerSchemePopup())
			{
				this.ShowUpdatedControllerSchemePopup();
			}
		}

		// Token: 0x0600243A RID: 9274 RVA: 0x0009627B File Offset: 0x0009447B
		private bool ShouldShowFTUXAccessibilityForSkipTransitions()
		{
			return FeatureToggle.IsFeatureEnabled(Feature.FTUX_Accessibility) && (!this._activePlayer.IsSkipTransitionsEnabled && !this._activePlayer.HasSeenNewContent("SkipTransitionsFTUXMessage")) && this._activePlayer.HasSeenNewContent("SkipTransitionsFTUXMessageFirstVisit");
		}

		// Token: 0x0600243B RID: 9275 RVA: 0x000962B9 File Offset: 0x000944B9
		private void ShowFTUXAccessibilityForSkipTransitions()
		{
			this._activePlayer.SetNewContentSeen("SkipTransitionsFTUXMessage");
			this.popupStack.PushConfirmationPopup<ConfirmationPopup>(StringId.SkipTransitions, null, delegate()
			{
				this._activePlayer.IsSkipTransitionsEnabled = true;
			}, StringId.FTUX_Accessibility_SkipTransitionDescription);
		}

		// Token: 0x0600243C RID: 9276 RVA: 0x000962EC File Offset: 0x000944EC
		private void OnPrePermissionDenied()
		{
			this._activePlayer.IsChallengeRemindersEnabledSetting = false;
			this._activePlayer.IsContentRemindersEnabledSetting = false;
			this._activePlayer.AreMenuMessagesEnabled = true;
			this._activePlayer.ClearNewContentSeen("OptionsScreenMessageTab");
			base.ShowNewContentIndicators();
			this._inGameMessages.DisplayMessage(StandaloneLocString.CreateString(this._appScope, StringId.Local_Notifications_PermissionsRequest_DeniedConfirmation));
		}

		// Token: 0x0600243D RID: 9277 RVA: 0x00096350 File Offset: 0x00094550
		private void OnPrePermissionGranted()
		{
			AuthorizationStatus currentAuthorizationStatus = this._systemNotificationService.AuthorizationStatus;
			if (currentAuthorizationStatus == AuthorizationStatus.NotDetermined)
			{
				this._systemNotificationService.RequestAuthorization(delegate(bool granted)
				{
					if (granted)
					{
						this.onNotificationAuthorizationCompleteHandler = new Action(this.OnSystemNotificationsGranted);
						return;
					}
					this.onNotificationAuthorizationCompleteHandler = new Action(this.OnSystemNotificationsDenied);
				});
				return;
			}
			if (currentAuthorizationStatus == AuthorizationStatus.Authorized)
			{
				this.OnSystemNotificationsGranted();
				return;
			}
			this.OnSystemNotificationsDenied();
		}

		// Token: 0x0600243E RID: 9278 RVA: 0x00096398 File Offset: 0x00094598
		private void OnSystemNotificationsDenied()
		{
			this._activePlayer.IsChallengeRemindersEnabledSetting = false;
			this._activePlayer.IsContentRemindersEnabledSetting = false;
			this._activePlayer.AreMenuMessagesEnabled = true;
			this._activePlayer.ClearNewContentSeen("OptionsScreenMessageTab");
			base.ShowNewContentIndicators();
			this._inGameMessages.DisplayMessage(StandaloneLocString.CreateString(this._appScope, StringId.Local_Notifications_PermissionsRequest_DeniedConfirmation));
		}

		// Token: 0x0600243F RID: 9279 RVA: 0x000963FC File Offset: 0x000945FC
		private void OnSystemNotificationsGranted()
		{
			this._inGameMessages.DisplayMessage(StandaloneLocString.CreateString(this._appScope, StringId.Local_Notifications_PermissionsRequest_Confirmation));
			this._activePlayer.IsChallengeRemindersEnabledSetting = true;
			this._activePlayer.IsContentRemindersEnabledSetting = true;
			this._activePlayer.AreMenuMessagesEnabled = true;
		}

		// Token: 0x06002440 RID: 9280 RVA: 0x00096448 File Offset: 0x00094648
		public void OnExit()
		{
			this._softwareCapabilities.OnAppShutdown();
			this._hardwareCapabilities.Exit();
		}

		// Token: 0x06002441 RID: 9281 RVA: 0x00096460 File Offset: 0x00094660
		public override void BackActivated()
		{
			if (this._inGameMessages.HasMessage)
			{
				this._inGameMessages.DismissCurrentMessage();
				return;
			}
			base.BackActivated();
		}

		// Token: 0x06002442 RID: 9282 RVA: 0x00096484 File Offset: 0x00094684
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (this.onNotificationAuthorizationCompleteHandler != null)
			{
				this.onNotificationAuthorizationCompleteHandler();
				this.onNotificationAuthorizationCompleteHandler = null;
			}
			if (this._gameStarter != null && this._gameStarter.CanStart)
			{
				this._gameStarter.Start(this._screenStack, this._appScope);
				this._gameStarter = null;
			}
		}

		// Token: 0x06002443 RID: 9283 RVA: 0x000964E6 File Offset: 0x000946E6
		public void OnProfileButtonPressed()
		{
			this._screenStack.PushScreen<ProfileSelectScreen>(ScreenStack.MotorwaysScreen.ProfileSelect, delegate(ProfileSelectScreen profileScreen)
			{
				profileScreen.PrepareScreen();
			}, false, null, true, null);
		}

		// Token: 0x06002444 RID: 9284 RVA: 0x00096519 File Offset: 0x00094719
		public void OnLogoPinAppear(int pinIndex)
		{
			this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.LogoPinAppear, 0.5f, (float)pinIndex, true, null));
		}

		// Token: 0x06002445 RID: 9285 RVA: 0x00096543 File Offset: 0x00094743
		public void OnLogoPinDisappear(int pinIndex)
		{
			this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.LogoPinDisappear, 0.5f, (float)pinIndex, true, null));
		}

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06002446 RID: 9286 RVA: 0x0009656D File Offset: 0x0009476D
		protected ObserverList<MainMenuScreen.IObserver> Observers
		{
			get
			{
				return this._observers;
			}
		}

		// Token: 0x06002447 RID: 9287 RVA: 0x00096575 File Offset: 0x00094775
		public void Subscribe(MainMenuScreen.IObserver observer)
		{
			this._observers.Subscribe(observer);
		}

		// Token: 0x06002448 RID: 9288 RVA: 0x00096583 File Offset: 0x00094783
		public bool Unsubscribe(MainMenuScreen.IObserver observer)
		{
			return this._observers.Unsubscribe(observer);
		}

		// Token: 0x06002449 RID: 9289 RVA: 0x00096594 File Offset: 0x00094794
		public void ShowEvergreenPopup()
		{
			if (!this._softwareCapabilities.SupportsEvergreenButton)
			{
				return;
			}
			this._activePlayer.SetNewContentSeen(this._currentNewsAndNotificationObject.ContentIndicatorID);
			this._evergreenButton.ShowNewContentIndicatorIfNeeded(false);
			this.popupStack.PushConfirmationPopup<ConfirmationPopup>(this._currentNewsAndNotificationObject.HeaderID, null, new Action(this.OpenEvergreenLink), this._currentNewsAndNotificationObject.BodyID);
		}

		// Token: 0x0600244A RID: 9290 RVA: 0x00096601 File Offset: 0x00094801
		private void OpenEvergreenLink()
		{
			if (!this._softwareCapabilities.SupportsEvergreenButton)
			{
				return;
			}
			if (Diagnostics.Verify(this._currentNewsAndNotificationObject.WebLink != null, "Evergreen should not be null if SupportsEvergreenButton is true"))
			{
				Application.OpenURL(this._currentNewsAndNotificationObject.WebLink);
			}
		}

		// Token: 0x0600244B RID: 9291 RVA: 0x0009663B File Offset: 0x0009483B
		private void ShowiCloudUnauthenticated()
		{
			this.popupStack.PushPopup<LoadScreenInterruptionPopup>(0f, false).Initialise(StringId.Options_iCloud, StringId.Options_iCloud_CacheIssue_NotSignedIn, null);
		}

		// Token: 0x04001E3F RID: 7743
		[Dependency]
		private MapDatabase _mapDatabase;

		// Token: 0x04001E40 RID: 7744
		[Dependency]
		private InGameMessageUIManager _inGameMessages;

		// Token: 0x04001E41 RID: 7745
		[Dependency]
		private IPersistentStorageService _storageService;

		// Token: 0x04001E42 RID: 7746
		private GameStarter _gameStarter;

		// Token: 0x04001E43 RID: 7747
		public GameObject resumeButton;

		// Token: 0x04001E44 RID: 7748
		[SerializeField]
		private GameObject _tutorialButton;

		// Token: 0x04001E45 RID: 7749
		[SerializeField]
		private GameObject _optionsButton;

		// Token: 0x04001E46 RID: 7750
		[SerializeField]
		private TouchButton _exitButton;

		// Token: 0x04001E47 RID: 7751
		[SerializeField]
		private GameCenterAccessPointButton _gameCenterAccessPointButton;

		// Token: 0x04001E48 RID: 7752
		[SerializeField]
		private TouchButton _profileSelectButton;

		// Token: 0x04001E49 RID: 7753
		[SerializeField]
		private Image _profileSelectBackground;

		// Token: 0x04001E4A RID: 7754
		[SerializeField]
		private TouchButton _evergreenButton;

		// Token: 0x04001E4B RID: 7755
		public RectTransform inGameMessageStartingPosition;

		// Token: 0x04001E4C RID: 7756
		public RectTransform inGameMessageStackStartPosition;

		// Token: 0x04001E4D RID: 7757
		[Dependency]
		private ISoftwareCapabilities _softwareCapabilities;

		// Token: 0x04001E4E RID: 7758
		[Dependency]
		private IHardwareCapabilities _hardwareCapabilities;

		// Token: 0x04001E4F RID: 7759
		[Dependency]
		private ActivePlayer _activePlayer;

		// Token: 0x04001E50 RID: 7760
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x04001E51 RID: 7761
		[Dependency]
		private ISystemNotificationService _systemNotificationService;

		// Token: 0x04001E52 RID: 7762
		[Dependency]
		private INotificationEventSystem _notificationEvents;

		// Token: 0x04001E53 RID: 7763
		[Dependency]
		private NewsAndNotificationData _newsAndNotificationData;

		// Token: 0x04001E54 RID: 7764
		[Tooltip("The duration of the fade to black if Skip Transitions is on")]
		[MinValue(0)]
		public float skippedTransitionFadeDuration = 1f;

		// Token: 0x04001E55 RID: 7765
		private NewsAndNotificationObject _currentNewsAndNotificationObject;

		// Token: 0x04001E56 RID: 7766
		public const string LocalNotificationsPermissionRequest = "LocalNotificationsPermissionRequest";

		// Token: 0x04001E57 RID: 7767
		public const string NewControllerSchemePopup = "NewControllerSchemePopup";

		// Token: 0x04001E58 RID: 7768
		public const string NewColorblindPopup = "NewColorblindPopup";

		// Token: 0x04001E59 RID: 7769
		private const string FTUX_AccessibilitySkipTransitionsFirstVisitNCI = "SkipTransitionsFTUXMessageFirstVisit";

		// Token: 0x04001E5A RID: 7770
		private const string FTUX_AccessibilitySkipTransitionsNCI = "SkipTransitionsFTUXMessage";

		// Token: 0x04001E5B RID: 7771
		private const string OptionsScreenMessageTabNCI = "OptionsScreenMessageTab";

		// Token: 0x04001E5C RID: 7772
		private Action onNotificationAuthorizationCompleteHandler;

		// Token: 0x04001E5D RID: 7773
		[Serialize(false, null)]
		private readonly ObserverList<MainMenuScreen.IObserver> _observers = new ObserverList<MainMenuScreen.IObserver>(1);

		// Token: 0x02000550 RID: 1360
		public interface IObserver
		{
			// Token: 0x0600244F RID: 9295
			void OnMainMenuTransitionedIn();

			// Token: 0x06002450 RID: 9296
			void OnMainMenuTransitionOut();
		}
	}
}
