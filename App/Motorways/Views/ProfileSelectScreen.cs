using System;
using System.Collections.Generic;
using System.Text;
using Factory;
using Motorways.Processes;
using Motorways.UI;
using Popups;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000568 RID: 1384
	public class ProfileSelectScreen : ScrollingButtonScreen
	{
		// Token: 0x060025A0 RID: 9632 RVA: 0x0009F36C File Offset: 0x0009D56C
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			if (this._tutorialCityDefinition == null)
			{
				this._tutorialCityDefinition = AssetBundleUtility.LoadPrefabAsync(this._tutorialDefinition.mapAssetBundle, this._tutorialDefinition.mapPrefabName, this);
			}
			this._lastActivePlayerId = this._activePlayer.Id;
			if (!this._activePlayer.HasAvatar)
			{
				this.ConfigureNewPlayer(this._activePlayer.Player);
			}
			if (this._currentlySelectedButtonIndex >= base.ButtonCount)
			{
				this._currentlySelectedButtonIndex = base.ButtonCount - 1;
			}
			int buttonIndex = 0;
			foreach (Player player in this._playerDatabase.Players)
			{
				ProfileSelectButton profileSelectButton = this.buttons[buttonIndex] as ProfileSelectButton;
				if (profileSelectButton != null)
				{
					profileSelectButton.SetPlayer(player);
				}
				buttonIndex++;
			}
			this.ScrollToButton(base.CurrentlySelectedButton, true);
			base.SetMapButtonValues(this.scrollRect.normalizedPosition);
			base.TransitionIn(outScreen);
		}

		// Token: 0x060025A1 RID: 9633 RVA: 0x0009F474 File Offset: 0x0009D674
		private int IndexOfCurrentPlayerCard()
		{
			int index = 0;
			using (IEnumerator<Player> enumerator = this._playerDatabase.Players.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == this._activePlayer.Player)
					{
						return index;
					}
					index++;
				}
			}
			Diagnostics.FailAssert("We somehow have an active player that isn't in the player database!", Array.Empty<object>());
			return 0;
		}

		// Token: 0x060025A2 RID: 9634 RVA: 0x0009F4E8 File Offset: 0x0009D6E8
		public void TransitionInNewCreateButton()
		{
			this.buttons[base.ButtonCount - 1].EnterFromHidden(null);
		}

		// Token: 0x060025A3 RID: 9635 RVA: 0x0009F504 File Offset: 0x0009D704
		public void OnMainButton()
		{
			if (this.CurrentlySelectedProfileButton.IsSelectedProfile)
			{
				this._screenStack.PopOneScreen();
				return;
			}
			if (this.CurrentlySelectedProfileButton.IsCreateButton)
			{
				this.CreateNewProfileFromCurrentButton();
				this.ShowFTUXAccessibilityForTutorial();
				return;
			}
			this._activePlayer.ActivatePlayer(this.CurrentlySelectedProfileButton.Player);
			this.SetProfileButtonSelected(this.CurrentlySelectedProfileButton);
			this._selectProfileButtonText.SetStringId(this._appScope, StringId.Play);
		}

		// Token: 0x060025A4 RID: 9636 RVA: 0x0009F57C File Offset: 0x0009D77C
		public void OnBack()
		{
			if (this._lastActivePlayerId != this._activePlayer.Id)
			{
				this._activePlayer.Touch();
			}
			this._screenStack.PopOneScreen();
			if (FeatureToggle.IsFeatureEnabled(Feature.RecordStorageAuditTrail))
			{
				Diagnostics.Report report = new Diagnostics.Report();
				report.Motive = "storage";
				report.AttachFile("storage_audit.txt", Encoding.UTF8.GetBytes(this._trail.ToJson()));
				report.Upload();
			}
		}

		// Token: 0x060025A5 RID: 9637 RVA: 0x0009F5F8 File Offset: 0x0009D7F8
		public void OnEditProfile(ProfileSelectButton button)
		{
			this._screenStack.PushScreen<ProfileCreationScreen>(ScreenStack.MotorwaysScreen.ProfileCreation, delegate(ProfileCreationScreen profileCreationScreen)
			{
				profileCreationScreen.PrepareScreen(button.Player);
			}, false, null, true, null);
		}

		// Token: 0x060025A6 RID: 9638 RVA: 0x0009F630 File Offset: 0x0009D830
		public void OnProfileCreateButtonPressed(ProfileSelectButton button)
		{
			if (Diagnostics.Verify(button.IsCreateButton))
			{
				if (this.CurrentlySelectedProfileButton == button)
				{
					this.CreateNewProfileFromCurrentButton();
					this.ShowFTUXAccessibilityForTutorial();
					this._selectProfileButtonText.SetStringId(this._appScope, StringId.Play);
					return;
				}
				this.ScrollToButton(button, false);
			}
		}

		// Token: 0x060025A7 RID: 9639 RVA: 0x0009F681 File Offset: 0x0009D881
		private void ShowFTUXAccessibilityForTutorial()
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.FTUX_Accessibility))
			{
				this.popupStack.PushConfirmationPopup<ConfirmationPopup>(StringId.FTUX_Accessibility_ReplayTutorialPrompt, new Action(this.SkipTutorial), new Action(this.EnterTutorial), StringId.FTUX_Accessibility_ReplayTutorialDescription);
			}
		}

		// Token: 0x060025A8 RID: 9640 RVA: 0x0009F6BA File Offset: 0x0009D8BA
		private void SkipTutorial()
		{
			this._activePlayer.SetTutorialTypeComplete(TutorialProgressionProcess.TutorialTypeForInputType(this._inputState.CurrentDeviceInputType));
			this._activePlayer.SetNewContentSeen("NewControllerSchemePopup");
			this._activePlayer.SetNewContentSeen("NewColorblindPopup");
		}

		// Token: 0x060025A9 RID: 9641 RVA: 0x0009F6F7 File Offset: 0x0009D8F7
		private void EnterTutorial()
		{
			this._screenStack.ReplaceScreenOnTop<GameContainerScreen>(ScreenStack.MotorwaysScreen.InGame, delegate(GameContainerScreen newScreen)
			{
				newScreen.PrepareForMap(UnityEngine.Object.Instantiate<GameObject>(this._tutorialCityDefinition.asset as GameObject).GetComponent<CityDefinition>(), this._tutorialDefinition, GameMode.Tutorial, null, false);
				this._analytics.TrackTutorialStarted(true);
			}, null, true);
		}

		// Token: 0x060025AA RID: 9642 RVA: 0x0009F714 File Offset: 0x0009D914
		protected override void OnSelectButton()
		{
			base.OnSelectButton();
			this.SetupNavigationForButtons();
			if (this.CurrentlySelectedProfileButton.IsSelectedProfile)
			{
				this._selectProfileButtonText.SetStringId(this._appScope, StringId.Play);
				return;
			}
			if (this.CurrentlySelectedProfileButton.IsCreateButton)
			{
				this._selectProfileButtonText.SetStringId(this._appScope, StringId.Create);
				return;
			}
			this._selectProfileButtonText.SetStringId(this._appScope, StringId.Select);
		}

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x060025AB RID: 9643 RVA: 0x0009F785 File Offset: 0x0009D985
		private IEnumerable<ProfileSelectButton> ProfileButtons
		{
			get
			{
				foreach (AnimatedCard button in this.buttons)
				{
					yield return button as ProfileSelectButton;
				}
				List<AnimatedCard>.Enumerator enumerator = default(List<AnimatedCard>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x060025AC RID: 9644 RVA: 0x0009F795 File Offset: 0x0009D995
		public void PrepareScreen()
		{
			base.AssignOriginPosition();
			this.CreateProfileButtons();
			Canvas.ForceUpdateCanvases();
			this.ScrollToButton(base.CurrentlySelectedButton, true);
		}

		// Token: 0x060025AD RID: 9645 RVA: 0x0009F7B8 File Offset: 0x0009D9B8
		private void CreateProfileButtons()
		{
			this.DestroyProfileButtons();
			foreach (Player player in this._playerDatabase.Players)
			{
				if (!player.HasAvatar)
				{
					player.ChooseAvatar(this._visualConstants.ProfileIconCount, 6);
				}
				ProfileSelectButton newButton = this._appScope.Get<ProfileSelectButton>();
				newButton.transform.SetParent(this.buttonParent, false);
				newButton.Initialize(player);
				this.buttons.Add(newButton);
				if (this._activePlayer.Player == player)
				{
					newButton.IsSelectedProfile = true;
				}
			}
			if (this.CanAddNewCreateProfileButton)
			{
				this.buttons.Add(this.CreateNewProfileButton());
			}
			base.RegisterAllLocalizedTextChildren();
			base.RegisterButtons();
			this.RegisterThemeComponents(this._themeDatabase.GetTheme());
			this.SetupNavigationForButtons();
			this._currentlySelectedButtonIndex = this.IndexOfCurrentPlayerCard();
			this.ScrollToButton(base.CurrentlySelectedButton, true);
		}

		// Token: 0x060025AE RID: 9646 RVA: 0x0009F8C0 File Offset: 0x0009DAC0
		private ProfileSelectButton CreateNewProfileButton()
		{
			ProfileSelectButton profileSelectButton = this._appScope.Get<ProfileSelectButton>();
			profileSelectButton.transform.SetParent(this.buttonParent, false);
			profileSelectButton.Initialize(null);
			profileSelectButton.IsCreateButton = true;
			return profileSelectButton;
		}

		// Token: 0x060025AF RID: 9647 RVA: 0x0009F8F0 File Offset: 0x0009DAF0
		private void AddNewCreateProfileButton()
		{
			ProfileSelectButton newProfileButton = this.CreateNewProfileButton();
			newProfileButton.SetHideRight();
			base.AddNewButtonToExistingSet(newProfileButton);
			this.SetupNavigationForButtons();
			this.ScrollToButton(this.buttons[base.ButtonCount - 2], false);
		}

		// Token: 0x060025B0 RID: 9648 RVA: 0x0009F934 File Offset: 0x0009DB34
		private void SetupNavigationForButtons()
		{
			for (int buttonIndex = 0; buttonIndex < base.ButtonCount; buttonIndex++)
			{
				ProfileSelectButton previousButton = (buttonIndex > 0) ? (this.buttons[buttonIndex - 1] as ProfileSelectButton) : null;
				(this.buttons[buttonIndex] as ProfileSelectButton).SetupButtonNavigation(previousButton, this.firstFocus, this.backButton);
			}
			AnimatedCard.SetNavigationOnUp(this.firstFocus, this.CurrentlySelectedProfileButton.editButton);
			AnimatedCard.SetNavigationOnDown(this.backButton, this.CurrentlySelectedProfileButton.editButton);
		}

		// Token: 0x060025B1 RID: 9649 RVA: 0x0009F9BC File Offset: 0x0009DBBC
		private void DestroyProfileButtons()
		{
			if (base.ButtonCount > 0)
			{
				for (int buttonIndex = 0; buttonIndex < base.ButtonCount; buttonIndex++)
				{
					this._appScope.Release(this.buttons[buttonIndex]);
				}
				this.buttons.Clear();
			}
		}

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x060025B2 RID: 9650 RVA: 0x0009FA06 File Offset: 0x0009DC06
		private ProfileSelectButton CurrentlySelectedProfileButton
		{
			get
			{
				return base.CurrentlySelectedButton as ProfileSelectButton;
			}
		}

		// Token: 0x060025B3 RID: 9651 RVA: 0x0009FA14 File Offset: 0x0009DC14
		private void SetProfileButtonSelected(ProfileSelectButton button)
		{
			foreach (ProfileSelectButton profileSelectButton in this.ProfileButtons)
			{
				profileSelectButton.IsSelectedProfile = (profileSelectButton == button);
			}
		}

		// Token: 0x060025B4 RID: 9652 RVA: 0x0009FA68 File Offset: 0x0009DC68
		private void CreateNewProfileFromCurrentButton()
		{
			Player newPlayer = this._playerDatabase.CreatePlayer();
			this.ConfigureNewPlayer(newPlayer);
			this._activePlayer.ActivatePlayer(newPlayer);
			this.CurrentlySelectedProfileButton.TurnIntoNewProfile(newPlayer);
			this.SetProfileButtonSelected(this.CurrentlySelectedProfileButton);
			if (this.CanAddNewCreateProfileButton)
			{
				this.AddNewCreateProfileButton();
				return;
			}
			this._selectProfileButtonText.SetStringId(this._appScope, StringId.Play);
		}

		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x060025B5 RID: 9653 RVA: 0x0009FACF File Offset: 0x0009DCCF
		private bool CanAddNewCreateProfileButton
		{
			get
			{
				return this._playerDatabase.PlayerCount < 6;
			}
		}

		// Token: 0x060025B6 RID: 9654 RVA: 0x0009FAE0 File Offset: 0x0009DCE0
		private void ConfigureNewPlayer(Player newPlayer)
		{
			int iconCount = this._visualConstants.ProfileIconCount;
			int backgroundCount = 6;
			newPlayer.ChooseAvatar(iconCount, backgroundCount);
			LegacyMotorwaysUserProfile motorwaysUserProfile = newPlayer.UserProfile as LegacyMotorwaysUserProfile;
			if (motorwaysUserProfile != null)
			{
				ProfileSelectScreen.Log.Info("Setting tutorial for input type {0} complete for new player {1}.", new object[]
				{
					this._inputState.CurrentDeviceInputType,
					newPlayer.Id
				});
				motorwaysUserProfile.SetTutorialTypeComplete(TutorialProgressionProcess.TutorialTypeForInputType(this._inputState.CurrentDeviceInputType));
			}
		}

		// Token: 0x04001FC1 RID: 8129
		[SerializeField]
		private LocalizedTextUI _selectProfileButtonText;

		// Token: 0x04001FC2 RID: 8130
		[SerializeField]
		private MapDefinition _tutorialDefinition;

		// Token: 0x04001FC3 RID: 8131
		private AssetBundleUtility.AsyncLoadResult _tutorialCityDefinition;

		// Token: 0x04001FC4 RID: 8132
		[Dependency]
		private PlayerDatabase _playerDatabase;

		// Token: 0x04001FC5 RID: 8133
		[Dependency]
		private ActivePlayer _activePlayer;

		// Token: 0x04001FC6 RID: 8134
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x04001FC7 RID: 8135
		[Dependency]
		private Diagnostics.StorageAuditTrail _trail;

		// Token: 0x04001FC8 RID: 8136
		private string _lastActivePlayerId;

		// Token: 0x04001FC9 RID: 8137
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ProfileSelectScreen");

		// Token: 0x04001FCA RID: 8138
		private const int MaximumPlayers = 6;
	}
}
