using System;
using System.Collections.Generic;
using Factory;
using JetBrains.Annotations;
using Motorways.Audio;
using Motorways.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x02000730 RID: 1840
	public class MapButtonChallengeCard : MapButtonCard
	{
		// Token: 0x17000864 RID: 2148
		// (get) Token: 0x06003301 RID: 13057 RVA: 0x000F1F6B File Offset: 0x000F016B
		public TouchButton MoreInfoButton
		{
			get
			{
				return this._moreInfoButton;
			}
		}

		// Token: 0x17000865 RID: 2149
		// (get) Token: 0x06003302 RID: 13058 RVA: 0x000F1F73 File Offset: 0x000F0173
		private CityChallengeData[] Challenges
		{
			get
			{
				return this._owningMapButton.MapDefinition.cityChallenges;
			}
		}

		// Token: 0x17000866 RID: 2150
		// (get) Token: 0x06003303 RID: 13059 RVA: 0x000F1F85 File Offset: 0x000F0185
		// (set) Token: 0x06003304 RID: 13060 RVA: 0x000F1F92 File Offset: 0x000F0192
		public bool LeaderboardShowsSelectedChallenge
		{
			get
			{
				return this._owningMapButton._leaderboardShowsSelectedChallenge;
			}
			set
			{
				this._owningMapButton._leaderboardShowsSelectedChallenge = value;
			}
		}

		// Token: 0x14000052 RID: 82
		// (add) Token: 0x06003305 RID: 13061 RVA: 0x000F1FA0 File Offset: 0x000F01A0
		// (remove) Token: 0x06003306 RID: 13062 RVA: 0x000F1FD8 File Offset: 0x000F01D8
		private event Action _onUnlockAnimationComplete;

		// Token: 0x14000053 RID: 83
		// (add) Token: 0x06003307 RID: 13063 RVA: 0x000F2010 File Offset: 0x000F0210
		// (remove) Token: 0x06003308 RID: 13064 RVA: 0x000F2048 File Offset: 0x000F0248
		public event Action OnChallengeSelected;

		// Token: 0x17000867 RID: 2151
		// (get) Token: 0x06003309 RID: 13065 RVA: 0x000F207D File Offset: 0x000F027D
		private CityChallengeData SelectedChallenge
		{
			get
			{
				return this._owningMapButton.SelectedChallenge;
			}
		}

		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x0600330A RID: 13066 RVA: 0x000F208A File Offset: 0x000F028A
		// (set) Token: 0x0600330B RID: 13067 RVA: 0x000F2097 File Offset: 0x000F0297
		public int SelectedCityChallengeIndex
		{
			get
			{
				return this._owningMapButton.SelectedChallengeIndex;
			}
			private set
			{
				this._owningMapButton.SelectedChallengeIndex = value;
			}
		}

		// Token: 0x17000869 RID: 2153
		// (get) Token: 0x0600330C RID: 13068 RVA: 0x000F20A5 File Offset: 0x000F02A5
		// (set) Token: 0x0600330D RID: 13069 RVA: 0x000F20B0 File Offset: 0x000F02B0
		public bool ShowingCardAsLocked
		{
			get
			{
				return this._showingCardAsLocked;
			}
			private set
			{
				this._showingCardAsLocked = value;
				this._lockedCanvasGroup.SetBlocksRaycasts(this._showingCardAsLocked);
				this._lockedCanvasGroup.gameObject.SetActive(this._showingCardAsLocked);
				this.SetChallengeButtonsActive(!this._showingCardAsLocked);
				this.InitializeFooter(this._showingCardAsLocked);
			}
		}

		// Token: 0x1700086A RID: 2154
		// (get) Token: 0x0600330E RID: 13070 RVA: 0x000F2106 File Offset: 0x000F0306
		public TouchToggle[] ChallengeButtons
		{
			get
			{
				return this._challengeButtons;
			}
		}

		// Token: 0x1700086B RID: 2155
		// (get) Token: 0x0600330F RID: 13071 RVA: 0x000F210E File Offset: 0x000F030E
		public TouchButton ChallengeModifiersButton
		{
			get
			{
				return this._challengeModifiersButton;
			}
		}

		// Token: 0x06003310 RID: 13072 RVA: 0x000F2118 File Offset: 0x000F0318
		private void ResetCard()
		{
			ChallengeIcon[] challengeIcons = this._challengeIcons;
			for (int i = 0; i < challengeIcons.Length; i++)
			{
				challengeIcons[i].gameObject.SetActive(false);
			}
			this._unlockText.gameObject.SetActive(false);
			this._selectChallengeText.gameObject.SetActive(false);
			this._scoreText.gameObject.SetActive(false);
			this._challengeModifiersButton.gameObject.SetActive(false);
		}

		// Token: 0x06003311 RID: 13073 RVA: 0x000F218C File Offset: 0x000F038C
		public void Initialize(IScope scope, MapButton owningMapButton)
		{
			this.ResetCard();
			this._scope = scope;
			this._owningMapButton = owningMapButton;
			this.InitializeFormattedLocalizedStrings();
		}

		// Token: 0x06003312 RID: 13074 RVA: 0x000F21A8 File Offset: 0x000F03A8
		public static string GetNewContentIndicatorID(MapDefinition mapDefinition)
		{
			return "ChallengeTab-" + mapDefinition.cityName.ToLower();
		}

		// Token: 0x06003313 RID: 13075 RVA: 0x000F21BF File Offset: 0x000F03BF
		public static string GetUnlockAnimationNciID(MapDefinition mapDefinition)
		{
			return "ChallengeUnlockAnimation-" + mapDefinition.cityName.ToLower();
		}

		// Token: 0x06003314 RID: 13076 RVA: 0x000F21D8 File Offset: 0x000F03D8
		private void InitializeFormattedLocalizedStrings()
		{
			MotorwaysStringKey key = this._scope.Get<MotorwaysStringKey>();
			for (int buttonIndex = 0; buttonIndex < this._challengeButtons.Length; buttonIndex++)
			{
				if (this.Challenges != null && buttonIndex < this.Challenges.Length)
				{
					this.UpdateChallengeButtonScore(buttonIndex);
					key.InitWithString(this.Challenges[buttonIndex].descriptionStringId);
					this._challengeButtonTitles[buttonIndex].LocString = StandaloneLocString.CreateString(this._scope, key);
				}
			}
			key.InitWithStringId(StringId.CityChallenge_UnlockChallenge, this._owningMapButton.MapDefinition.challengeModeTargetScore, new Dictionary<string, string>
			{
				{
					"Num",
					this._owningMapButton.MapDefinition.challengeModeTargetScore.ToString()
				}
			});
			this._unlockText.LocString = StandaloneLocString.CreateString(this._scope, key);
		}

		// Token: 0x06003315 RID: 13077 RVA: 0x000F22A4 File Offset: 0x000F04A4
		public override void SetVisible(bool isVisible)
		{
			base.SetVisible(isVisible);
			if (isVisible)
			{
				bool hasSeenUnlockAnimation = this._scope.Get<ActivePlayer>().HasSeenNewContent(MapButtonChallengeCard.GetUnlockAnimationNciID(this._owningMapButton.MapDefinition));
				if (!this._owningMapButton.AreChallengesLocked && !hasSeenUnlockAnimation)
				{
					this.ShowingCardAsLocked = true;
					this.PlayUnlockAnimation(delegate
					{
						this.ShowingCardAsLocked = false;
					});
					return;
				}
				this.ShowingCardAsLocked = this._owningMapButton.AreChallengesLocked;
			}
		}

		// Token: 0x06003316 RID: 13078 RVA: 0x000F2318 File Offset: 0x000F0518
		private void UpdateChallengeButtonScore(int buttonIndex)
		{
			ActivePlayer activePlayer = this._scope.Get<ActivePlayer>();
			MotorwaysStringKey key = this._scope.Get<MotorwaysStringKey>();
			CityChallengeStatistics stats = activePlayer.GetCityChallengeScore(this._owningMapButton.MapDefinition.cityName, GameMode.Normal, buttonIndex, true);
			key.InitWithStringId(StringId.BestScore, stats.BestScore, new Dictionary<string, string>
			{
				{
					"Num",
					stats.BestScore.ToString()
				}
			});
			this._challengeButtonScores[buttonIndex].LocString = StandaloneLocString.CreateString(this._scope, key);
		}

		// Token: 0x06003317 RID: 13079 RVA: 0x000F239C File Offset: 0x000F059C
		public void UpdateChallengeButtonScores()
		{
			for (int buttonIndex = 0; buttonIndex < this._challengeButtons.Length; buttonIndex++)
			{
				if (this.Challenges != null && buttonIndex < this.Challenges.Length)
				{
					this.UpdateChallengeButtonScore(buttonIndex);
				}
			}
		}

		// Token: 0x06003318 RID: 13080 RVA: 0x000F23D8 File Offset: 0x000F05D8
		private void SetChallengeButtonsActive(bool active)
		{
			for (int buttonIndex = 0; buttonIndex < this._challengeButtons.Length; buttonIndex++)
			{
				this._challengeButtons[buttonIndex].gameObject.SetActive(active && buttonIndex < this.Challenges.Length);
			}
			if (active)
			{
				this.RefreshSelectedButtonAnimations();
			}
		}

		// Token: 0x06003319 RID: 13081 RVA: 0x000F2424 File Offset: 0x000F0624
		private void InitializeFooter(bool isLocked)
		{
			this._unlockText.gameObject.SetActive(isLocked);
			if (isLocked)
			{
				this._selectChallengeText.gameObject.SetActive(false);
				this._scoreText.gameObject.SetActive(false);
				this._challengeModifiersButton.gameObject.SetActive(false);
				return;
			}
			bool isChallengeSelected = this.SelectedCityChallengeIndex != -1;
			this._selectChallengeText.gameObject.SetActive(!isChallengeSelected);
			this._scoreText.gameObject.SetActive(isChallengeSelected);
			this._challengeModifiersButton.gameObject.SetActive(isChallengeSelected);
		}

		// Token: 0x0600331A RID: 13082 RVA: 0x000F24BC File Offset: 0x000F06BC
		public void OnChallengeIconPressed()
		{
			this._scope.Get<ScreenStack>().PushScreen<ChallengeInfoScreen>(ScreenStack.MotorwaysScreen.ChallengeInfo, delegate(ChallengeInfoScreen screen)
			{
				screen.PrepareScreenForCityChallenge(this._owningMapButton.MapDefinition, this.SelectedCityChallengeIndex, StringId.Back, true, false);
			}, true, null, true, null);
		}

		// Token: 0x0600331B RID: 13083 RVA: 0x000F24E4 File Offset: 0x000F06E4
		private void RefreshSelectedButtonAnimations()
		{
			for (int challengeButtonIndex = 0; challengeButtonIndex < this._challengeButtons.Length; challengeButtonIndex++)
			{
				this._challengeButtons[challengeButtonIndex].GetComponent<Animator>().SetBool(this.ChallengeSelected, challengeButtonIndex == this.SelectedCityChallengeIndex);
			}
		}

		// Token: 0x0600331C RID: 13084 RVA: 0x000F2525 File Offset: 0x000F0725
		[UsedImplicitly]
		private void OnButtonSelected(int selectedButtonIndex)
		{
			if (this.SelectedCityChallengeIndex != selectedButtonIndex)
			{
				this.SelectChallengeIndex(selectedButtonIndex);
			}
		}

		// Token: 0x0600331D RID: 13085 RVA: 0x000F2538 File Offset: 0x000F0738
		public void SelectChallengeIndex(int challengeIndex)
		{
			this.SelectedCityChallengeIndex = challengeIndex;
			this.LeaderboardShowsSelectedChallenge = false;
			this.RefreshSelectedButtonAnimations();
			for (int iconIndex = 0; iconIndex < this._challengeIcons.Length; iconIndex++)
			{
				if (iconIndex < this.SelectedChallenge.challenges.Length)
				{
					ChallengeData challenge = this.SelectedChallenge.challenges[iconIndex];
					this._challengeIcons[iconIndex].gameObject.SetActive(true);
					this._challengeIcons[iconIndex].SetChallengeIcons(challenge.icon, false, challenge.subIcon, challenge.subIconBackground);
				}
				else
				{
					this._challengeIcons[iconIndex].gameObject.SetActive(false);
				}
			}
			CityChallengeStatistics stats = this._scope.Get<ActivePlayer>().GetCityChallengeScore(this._owningMapButton.MapDefinition.cityName, GameMode.Normal, this.SelectedCityChallengeIndex, true);
			MotorwaysStringKey key = this._scope.Get<MotorwaysStringKey>();
			if (stats.BestScore >= this.SelectedChallenge.targetScore)
			{
				key.InitWithStringId(StringId.BestScore, stats.BestScore, new Dictionary<string, string>
				{
					{
						"Num",
						stats.BestScore.ToString()
					}
				});
			}
			else
			{
				key.InitWithStringId(StringId.TargetScore, this.SelectedChallenge.targetScore, new Dictionary<string, string>
				{
					{
						"Num",
						this.SelectedChallenge.targetScore.ToString()
					}
				});
			}
			this._scoreText.LocString = StandaloneLocString.CreateString(this._scope, key);
			this.InitializeFooter(this.ShowingCardAsLocked);
			Action onChallengeSelected = this.OnChallengeSelected;
			if (onChallengeSelected == null)
			{
				return;
			}
			onChallengeSelected();
		}

		// Token: 0x0600331E RID: 13086 RVA: 0x000F26B0 File Offset: 0x000F08B0
		public void DeselectCityChallenge()
		{
			TouchToggle[] challengeButtons = this._challengeButtons;
			for (int i = 0; i < challengeButtons.Length; i++)
			{
				challengeButtons[i].GetComponent<Animator>().SetBool(this.ChallengeSelected, false);
			}
			ChallengeIcon[] challengeIcons = this._challengeIcons;
			for (int i = 0; i < challengeIcons.Length; i++)
			{
				challengeIcons[i].gameObject.SetActive(false);
			}
			this._scoreText.LocString = null;
			this.InitializeFooter(this.ShowingCardAsLocked);
		}

		// Token: 0x0600331F RID: 13087 RVA: 0x000F2724 File Offset: 0x000F0924
		public void SetupChallengeModifiersButtonNavigation()
		{
			int challengeButtonCount = this._challengeButtons.Length;
			if (!Diagnostics.Verify(challengeButtonCount > 0, "No city challenge buttons"))
			{
				return;
			}
			Selectable selectable = this._challengeButtons[challengeButtonCount - 1];
			Selectable playButton = this._owningMapButton.MapSelectScreen.firstFocus;
			AnimatedCard.SetNavigationOnDown(selectable, this._challengeModifiersButton);
			AnimatedCard.SetNavigationOnUp(playButton, this._challengeModifiersButton);
		}

		// Token: 0x06003320 RID: 13088 RVA: 0x000F2780 File Offset: 0x000F0980
		private void PlayUnlockAnimation(Action onComplete)
		{
			this._scope.Get<AudioSystem>().ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.UnlockMap, 0.5f, -1f, true, null));
			this._scope.Get<ActivePlayer>().SetNewContentSeen(MapButtonChallengeCard.GetNewContentIndicatorID(this._owningMapButton.MapDefinition));
			this._scope.Get<ActivePlayer>().SetNewContentSeen(MapButtonChallengeCard.GetUnlockAnimationNciID(this._owningMapButton.MapDefinition));
			this._onUnlockAnimationComplete += onComplete;
			this._unlockEffectAnimator.SetTrigger(this.ChallengesUnlocked);
		}

		// Token: 0x06003321 RID: 13089 RVA: 0x000F2818 File Offset: 0x000F0A18
		[UsedImplicitly]
		public void UnlockAnimationComplete()
		{
			Action onUnlockAnimationComplete = this._onUnlockAnimationComplete;
			if (onUnlockAnimationComplete == null)
			{
				return;
			}
			onUnlockAnimationComplete();
		}

		// Token: 0x06003322 RID: 13090 RVA: 0x000F282A File Offset: 0x000F0A2A
		[UsedImplicitly]
		public void OnMoreInfoButtonClicked()
		{
			this._owningMapButton.OnChallengeModeMoreInfoButtonClicked();
		}

		// Token: 0x06003323 RID: 13091 RVA: 0x000F2837 File Offset: 0x000F0A37
		[UsedImplicitly]
		public void OnMoreInfoButtonSelected()
		{
			if (Diagnostics.Verify(this._owningMapButton != null))
			{
				this._owningMapButton.ScrollToMe();
			}
		}

		// Token: 0x04002B97 RID: 11159
		[SerializeField]
		private TouchToggle[] _challengeButtons;

		// Token: 0x04002B98 RID: 11160
		[SerializeField]
		private LocalizedTextUI[] _challengeButtonTitles;

		// Token: 0x04002B99 RID: 11161
		[SerializeField]
		private LocalizedTextUI[] _challengeButtonScores;

		// Token: 0x04002B9A RID: 11162
		[SerializeField]
		private LocalizedTextUI _scoreText;

		// Token: 0x04002B9B RID: 11163
		[SerializeField]
		private LocalizedTextUI _selectChallengeText;

		// Token: 0x04002B9C RID: 11164
		[SerializeField]
		private ChallengeIcon[] _challengeIcons;

		// Token: 0x04002B9D RID: 11165
		[SerializeField]
		private DelegateCanvasGroup _lockedCanvasGroup;

		// Token: 0x04002B9E RID: 11166
		[SerializeField]
		private LocalizedTextUI _unlockText;

		// Token: 0x04002B9F RID: 11167
		[SerializeField]
		private TouchButton _challengeModifiersButton;

		// Token: 0x04002BA0 RID: 11168
		[SerializeField]
		private TouchButton _moreInfoButton;

		// Token: 0x04002BA1 RID: 11169
		[SerializeField]
		private Animator _unlockEffectAnimator;

		// Token: 0x04002BA2 RID: 11170
		private IScope _scope;

		// Token: 0x04002BA3 RID: 11171
		private MapButton _owningMapButton;

		// Token: 0x04002BA4 RID: 11172
		private readonly int ChallengeSelected = Animator.StringToHash("ChallengeSelected");

		// Token: 0x04002BA5 RID: 11173
		private readonly int ChallengesUnlocked = Animator.StringToHash("Unlock");

		// Token: 0x04002BA8 RID: 11176
		private bool _showingCardAsLocked;
	}
}
