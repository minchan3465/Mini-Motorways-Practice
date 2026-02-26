using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Client;
using Factory;
using JetBrains.Annotations;
using Motorways.Audio;
using Motorways.Leaderboards;
using Motorways.Themes;
using Motorways.Views;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x0200072B RID: 1835
	public class MapButton : AnimatedCard
	{
		// Token: 0x17000845 RID: 2117
		// (get) Token: 0x06003281 RID: 12929 RVA: 0x000EEDB1 File Offset: 0x000ECFB1
		public MapButtonTab LeaderboardTabButton
		{
			get
			{
				return this._leaderboardTabButton;
			}
		}

		// Token: 0x17000846 RID: 2118
		// (get) Token: 0x06003282 RID: 12930 RVA: 0x000EEDB9 File Offset: 0x000ECFB9
		public MapButtonTab ChallengeTabButton
		{
			get
			{
				return this._challengeTabButton;
			}
		}

		// Token: 0x17000847 RID: 2119
		// (get) Token: 0x06003283 RID: 12931 RVA: 0x000EEDC1 File Offset: 0x000ECFC1
		public MapButtonTab ModeSelectTabButton
		{
			get
			{
				return this._modeSelectTabButton;
			}
		}

		// Token: 0x17000848 RID: 2120
		// (get) Token: 0x06003284 RID: 12932 RVA: 0x000EEDC9 File Offset: 0x000ECFC9
		public ThemeSelectButton ColorfulSelect
		{
			get
			{
				return this._mainCard.ColorfulSelect;
			}
		}

		// Token: 0x17000849 RID: 2121
		// (get) Token: 0x06003285 RID: 12933 RVA: 0x000EEDD6 File Offset: 0x000ECFD6
		public ThemeSelectButton DarkSelect
		{
			get
			{
				return this._mainCard.DarkSelect;
			}
		}

		// Token: 0x1700084A RID: 2122
		// (get) Token: 0x06003286 RID: 12934 RVA: 0x000EEDE3 File Offset: 0x000ECFE3
		public ThemeSelectButton MapsSelect
		{
			get
			{
				return this._mainCard.MapsSelect;
			}
		}

		// Token: 0x1700084B RID: 2123
		// (get) Token: 0x06003287 RID: 12935 RVA: 0x000EEDF0 File Offset: 0x000ECFF0
		public TouchButton MoreInfoButton
		{
			get
			{
				return this._mainCard.MoreInfoButton;
			}
		}

		// Token: 0x1700084C RID: 2124
		// (get) Token: 0x06003288 RID: 12936 RVA: 0x000EEDFD File Offset: 0x000ECFFD
		public StringId PlayTextStringId
		{
			get
			{
				return StringId.Play;
			}
		}

		// Token: 0x1400004D RID: 77
		// (add) Token: 0x06003289 RID: 12937 RVA: 0x000EEE04 File Offset: 0x000ED004
		// (remove) Token: 0x0600328A RID: 12938 RVA: 0x000EEE3C File Offset: 0x000ED03C
		public event Action<MapButton> onChallengeExpired;

		// Token: 0x1400004E RID: 78
		// (add) Token: 0x0600328B RID: 12939 RVA: 0x000EEE74 File Offset: 0x000ED074
		// (remove) Token: 0x0600328C RID: 12940 RVA: 0x000EEEAC File Offset: 0x000ED0AC
		public event Action<MapButton> onSelected;

		// Token: 0x1400004F RID: 79
		// (add) Token: 0x0600328D RID: 12941 RVA: 0x000EEEE4 File Offset: 0x000ED0E4
		// (remove) Token: 0x0600328E RID: 12942 RVA: 0x000EEF1C File Offset: 0x000ED11C
		public event Action onShowMoreChallengeInfo;

		// Token: 0x14000050 RID: 80
		// (add) Token: 0x0600328F RID: 12943 RVA: 0x000EEF54 File Offset: 0x000ED154
		// (remove) Token: 0x06003290 RID: 12944 RVA: 0x000EEF8C File Offset: 0x000ED18C
		public event Action onShowModeInfo;

		// Token: 0x14000051 RID: 81
		// (add) Token: 0x06003291 RID: 12945 RVA: 0x000EEFC4 File Offset: 0x000ED1C4
		// (remove) Token: 0x06003292 RID: 12946 RVA: 0x000EEFFC File Offset: 0x000ED1FC
		public event Action onExpertModeLockedPressed;

		// Token: 0x1700084D RID: 2125
		// (get) Token: 0x06003293 RID: 12947 RVA: 0x000EF031 File Offset: 0x000ED231
		public MapButtonMainCard MainCard
		{
			get
			{
				return this._mainCard;
			}
		}

		// Token: 0x1700084E RID: 2126
		// (get) Token: 0x06003295 RID: 12949 RVA: 0x000EF042 File Offset: 0x000ED242
		// (set) Token: 0x06003294 RID: 12948 RVA: 0x000EF039 File Offset: 0x000ED239
		private MapButtonLeaderboardCard LeaderboardCard { get; set; }

		// Token: 0x1700084F RID: 2127
		// (get) Token: 0x06003297 RID: 12951 RVA: 0x000EF053 File Offset: 0x000ED253
		// (set) Token: 0x06003296 RID: 12950 RVA: 0x000EF04A File Offset: 0x000ED24A
		private MapButtonLockedCard LockedCard { get; set; }

		// Token: 0x17000850 RID: 2128
		// (get) Token: 0x06003299 RID: 12953 RVA: 0x000EF064 File Offset: 0x000ED264
		// (set) Token: 0x06003298 RID: 12952 RVA: 0x000EF05B File Offset: 0x000ED25B
		private MapButtonChallengeCard ChallengeCard { get; set; }

		// Token: 0x17000851 RID: 2129
		// (get) Token: 0x0600329B RID: 12955 RVA: 0x000EF075 File Offset: 0x000ED275
		// (set) Token: 0x0600329A RID: 12954 RVA: 0x000EF06C File Offset: 0x000ED26C
		public MapButtonModeSelectCard ModeSelectCard { get; set; }

		// Token: 0x17000852 RID: 2130
		// (get) Token: 0x0600329C RID: 12956 RVA: 0x000EF080 File Offset: 0x000ED280
		public CityChallengeData SelectedChallenge
		{
			get
			{
				CityChallengeData[] challenges = this._mapDefinition.cityChallenges;
				if (this.SelectedChallengeIndex >= 0 && this.SelectedChallengeIndex < challenges.Length)
				{
					return challenges[this.SelectedChallengeIndex];
				}
				return null;
			}
		}

		// Token: 0x17000853 RID: 2131
		// (get) Token: 0x0600329D RID: 12957 RVA: 0x000EF0B7 File Offset: 0x000ED2B7
		// (set) Token: 0x0600329E RID: 12958 RVA: 0x000EF0BF File Offset: 0x000ED2BF
		public int SelectedChallengeIndex { get; set; } = -1;

		// Token: 0x17000854 RID: 2132
		// (get) Token: 0x0600329F RID: 12959 RVA: 0x000EF0C8 File Offset: 0x000ED2C8
		public bool AreChallengesLocked
		{
			get
			{
				if (FeatureToggle.IsFeatureDisabled(Feature.MapUnlocks))
				{
					return false;
				}
				int score = this.MapSelectScreen.GetBestScoreForCityLeaderboard(this._mapDefinition.cityName, GameMode.Normal);
				return this._mapDefinition.challengeModeTargetScore > score;
			}
		}

		// Token: 0x060032A0 RID: 12960 RVA: 0x000EF106 File Offset: 0x000ED306
		public void DeselectCityChallenge()
		{
			this.SelectedChallengeIndex = -1;
			this.LeaderboardShowsSelectedChallenge = false;
			if (this.ChallengeCard != null)
			{
				this.ChallengeCard.DeselectCityChallenge();
			}
		}

		// Token: 0x17000855 RID: 2133
		// (get) Token: 0x060032A1 RID: 12961 RVA: 0x000EF12F File Offset: 0x000ED32F
		// (set) Token: 0x060032A2 RID: 12962 RVA: 0x000EF137 File Offset: 0x000ED337
		public bool LeaderboardShowsSelectedChallenge
		{
			get
			{
				return this._leaderboardShowsSelectedChallenge;
			}
			set
			{
				this._leaderboardShowsSelectedChallenge = value;
			}
		}

		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x060032A3 RID: 12963 RVA: 0x000EF140 File Offset: 0x000ED340
		private bool HasCityChallenge
		{
			get
			{
				return this.SelectedChallengeIndex >= 0;
			}
		}

		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x060032A4 RID: 12964 RVA: 0x000EF14E File Offset: 0x000ED34E
		// (set) Token: 0x060032A5 RID: 12965 RVA: 0x000EF156 File Offset: 0x000ED356
		public bool IsRandomChallengeCard { get; private set; }

		// Token: 0x17000858 RID: 2136
		// (get) Token: 0x060032A6 RID: 12966 RVA: 0x000EF15F File Offset: 0x000ED35F
		public MapSelectScreen MapSelectScreen
		{
			get
			{
				return this._screen;
			}
		}

		// Token: 0x17000859 RID: 2137
		// (get) Token: 0x060032A7 RID: 12967 RVA: 0x000EF167 File Offset: 0x000ED367
		// (set) Token: 0x060032A8 RID: 12968 RVA: 0x000EF16F File Offset: 0x000ED36F
		public MapButton.MapButtonType Type { get; private set; }

		// Token: 0x1700085A RID: 2138
		// (get) Token: 0x060032A9 RID: 12969 RVA: 0x000EF178 File Offset: 0x000ED378
		public Selectable PlayButton
		{
			get
			{
				return this._playButton;
			}
		}

		// Token: 0x1700085B RID: 2139
		// (get) Token: 0x060032AA RID: 12970 RVA: 0x000EF180 File Offset: 0x000ED380
		public bool IsLocked
		{
			get
			{
				return this._currentCard == MapButton.Card.Locked;
			}
		}

		// Token: 0x1700085C RID: 2140
		// (get) Token: 0x060032AB RID: 12971 RVA: 0x000EF18B File Offset: 0x000ED38B
		public MapButton.Card CurrentCard
		{
			get
			{
				return this._currentCard;
			}
		}

		// Token: 0x1700085D RID: 2141
		// (get) Token: 0x060032AC RID: 12972 RVA: 0x000EF193 File Offset: 0x000ED393
		public MapDefinition MapDefinition
		{
			get
			{
				return this._mapDefinition;
			}
		}

		// Token: 0x1700085E RID: 2142
		// (get) Token: 0x060032AD RID: 12973 RVA: 0x000EF19B File Offset: 0x000ED39B
		public MapChallenge MapChallenge
		{
			get
			{
				if (this.HasCityChallenge)
				{
					return MapChallenge.CreateCityChallenge(this._scope.Get<ChallengeSystem>(), this.SelectedChallengeIndex, this.MapDefinition, this.SelectedChallenge.challenges, 0UL);
				}
				return this._mapChallenge;
			}
		}

		// Token: 0x1700085F RID: 2143
		// (get) Token: 0x060032AE RID: 12974 RVA: 0x000EF1D5 File Offset: 0x000ED3D5
		public bool HasExpired
		{
			get
			{
				return this._mapChallenge.HasExpired();
			}
		}

		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x060032AF RID: 12975 RVA: 0x000EF1E4 File Offset: 0x000ED3E4
		public override string NewContentId
		{
			get
			{
				MapButton.MapButtonType type = this.Type;
				if (type == MapButton.MapButtonType.City)
				{
					return "NewCity-" + this.MapDefinition.cityName;
				}
				if (type - MapButton.MapButtonType.DailyChallenge <= 1)
				{
					return string.Format("New{0}-{1}", this.Type, this._mapChallenge.TimeStart);
				}
				Diagnostics.FailAssert("Unhandled map type: {0}", new object[]
				{
					this.Type
				});
				return null;
			}
		}

		// Token: 0x060032B0 RID: 12976 RVA: 0x000EF25C File Offset: 0x000ED45C
		public GameMode GetCurrentSelectedGameMode()
		{
			if (this.Type != MapButton.MapButtonType.City)
			{
				return GameMode.Normal;
			}
			if (this.ModeSelectCard != null)
			{
				return this.ModeSelectCard.GameMode;
			}
			return this._player.GetSelectedModeForMap(this._mapDefinition.mapName);
		}

		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x060032B1 RID: 12977 RVA: 0x000020AA File Offset: 0x000002AA
		private protected override bool BypassNewContentData
		{
			protected get
			{
				return true;
			}
		}

		// Token: 0x060032B2 RID: 12978 RVA: 0x000EF298 File Offset: 0x000ED498
		protected override void Awake()
		{
			base.Awake();
			this._animator = base.GetComponent<Animator>();
			this._lockUnlockButton.gameObject.SetActive(FeatureToggle.IsFeatureEnabled(Feature.DebugMapUnlockButton));
		}

		// Token: 0x060032B3 RID: 12979 RVA: 0x000EF2C4 File Offset: 0x000ED4C4
		public override void RegisterThemeComponents()
		{
			base.GetComponentsInChildren<IThemeComponent>(true, this._themedMapButtonComponents);
			foreach (IThemeComponent themeComponent in this._themedMapButtonComponents)
			{
				themeComponent.InitializeTheme(this._themeDatabase);
			}
		}

		// Token: 0x060032B4 RID: 12980 RVA: 0x000EF328 File Offset: 0x000ED528
		public override void UnregisterThemeComponents()
		{
			foreach (IThemeComponent themeComponent in this._themedMapButtonComponents)
			{
				themeComponent.ReleaseTheme(this._themeDatabase);
			}
			this._themedMapButtonComponents.Clear();
		}

		// Token: 0x060032B5 RID: 12981 RVA: 0x000EF38C File Offset: 0x000ED58C
		protected override void Update()
		{
			base.Update();
			if (this.IsChallengeMapButton() && this._scope != null && !this._isUpdatingChallenge)
			{
				if (this._mapChallenge.HasExpired())
				{
					Action<MapButton> action = this.onChallengeExpired;
					if (action != null)
					{
						action(this);
					}
					this._isUpdatingChallenge = true;
					return;
				}
				int secondsLeft = this._mapChallenge.SecondsLeft;
				int minutesLeft = secondsLeft / 60;
				int hoursLeft = minutesLeft / 60;
				int daysLeft = hoursLeft / 24;
				StringId stringId;
				int displayCount;
				if (daysLeft > 0)
				{
					stringId = StringId.Challenge_TimeLeft_Days;
					displayCount = daysLeft;
				}
				else if (hoursLeft > 0)
				{
					stringId = StringId.Challenge_TimeLeft_Hours;
					displayCount = hoursLeft;
				}
				else if (minutesLeft > 0)
				{
					stringId = StringId.Challenge_TimeLeft_Minutes;
					displayCount = minutesLeft;
				}
				else
				{
					stringId = StringId.Challenge_TimeLeft_Seconds;
					displayCount = secondsLeft;
				}
				if (stringId != this._previousChallengeTimerKey || displayCount != this._previousChallengeTimerCount)
				{
					this._challengeTimeLeftKey.InitWithStringId(stringId, displayCount, new Dictionary<string, string>
					{
						{
							"Num",
							displayCount.ToString()
						}
					});
					this._mainCard.TimeLeftText.LocString = StandaloneLocString.CreateString(this._scope, this._challengeTimeLeftKey);
					this._previousChallengeTimerKey = stringId;
					this._previousChallengeTimerCount = displayCount;
				}
			}
		}

		// Token: 0x060032B6 RID: 12982 RVA: 0x000EF4A7 File Offset: 0x000ED6A7
		public void OnClicked()
		{
			this._screen.SelectMap(this);
		}

		// Token: 0x060032B7 RID: 12983 RVA: 0x000EF4B5 File Offset: 0x000ED6B5
		public void ScrollToMe()
		{
			this._screen.ScrollToButton(this, false);
		}

		// Token: 0x060032B8 RID: 12984 RVA: 0x000EF4C4 File Offset: 0x000ED6C4
		public void ShowChallengeInfo()
		{
			if (this.Type == MapButton.MapButtonType.DailyChallenge)
			{
				this._screen.ShowDailyChallengeInfo();
				return;
			}
			if (this.Type == MapButton.MapButtonType.WeeklyChallenge)
			{
				this._screen.ShowWeeklyChallengeInfo();
			}
		}

		// Token: 0x060032B9 RID: 12985 RVA: 0x000EF4EF File Offset: 0x000ED6EF
		public void SetThemePreference(MotorwaysThemePreference selectedTheme)
		{
			if (selectedTheme != this._selectedTheme)
			{
				this._selectedTheme = selectedTheme;
				this._screen.SetThemePreference(selectedTheme);
			}
		}

		// Token: 0x060032BA RID: 12986 RVA: 0x000EF510 File Offset: 0x000ED710
		public void EnsureThemeButtonSelectedState(MotorwaysThemePreference? newTheme = null)
		{
			this._selectedTheme = (newTheme ?? this._selectedTheme);
			switch (this._selectedTheme)
			{
			case MotorwaysThemePreference.Dark:
			case MotorwaysThemePreference.DarkColorblind:
				this.ColorfulSelect.SetUnselected();
				this.DarkSelect.SetSelected();
				this.MapsSelect.SetUnselected();
				return;
			case MotorwaysThemePreference.Maps:
				this.ColorfulSelect.SetUnselected();
				this.DarkSelect.SetUnselected();
				this.MapsSelect.SetSelected();
				return;
			}
			this.ColorfulSelect.SetSelected();
			this.DarkSelect.SetUnselected();
			this.MapsSelect.SetUnselected();
		}

		// Token: 0x060032BB RID: 12987 RVA: 0x000EF5C4 File Offset: 0x000ED7C4
		public void SetupButtonNavigation()
		{
			MapButton prevMapButton = this.MapSelectScreen.GetPreviousButton(this);
			MapButton nextMapButton = this.MapSelectScreen.GetNextButton(this);
			Selectable playButton = this.MapSelectScreen.firstFocus;
			Selectable backButton = this.MapSelectScreen.backButton;
			MotorwaysThemePreference themePreference = this._themeDatabase.ThemePreference;
			TouchButton prevMainButton = (prevMapButton == null) ? null : ((prevMapButton._mainTabButton == null) ? null : prevMapButton._mainTabButton.GetComponent<TouchButton>());
			TouchButton prevLeaderboardButton = (prevMapButton == null) ? null : ((prevMapButton._leaderboardTabButton == null) ? null : prevMapButton._leaderboardTabButton.GetComponent<TouchButton>());
			TouchButton prevChallengeButton = (prevMapButton == null) ? null : ((prevMapButton._challengeTabButton == null) ? null : prevMapButton._challengeTabButton.GetComponent<TouchButton>());
			TouchButton prevModeSelectButton = (prevMapButton == null) ? null : ((prevMapButton._modeSelectTabButton == null) ? null : prevMapButton._modeSelectTabButton.GetComponent<TouchButton>());
			TouchButton prevMoreInfoButton = (prevMapButton == null) ? null : prevMapButton.MoreInfoButton;
			if (prevMapButton != null && prevMapButton.IsLocked)
			{
				prevMainButton = prevMapButton.LockedCard.TouchButton;
				prevLeaderboardButton = prevMapButton.LockedCard.TouchButton;
				prevChallengeButton = prevMapButton.LockedCard.TouchButton;
				prevModeSelectButton = prevMapButton.LockedCard.TouchButton;
				prevMoreInfoButton = prevMapButton.LockedCard.TouchButton;
			}
			MapButton.<>c__DisplayClass138_0 CS$<>8__locals1;
			CS$<>8__locals1.nextMainButton = ((nextMapButton == null) ? null : ((nextMapButton._mainTabButton == null) ? null : nextMapButton._mainTabButton.GetComponent<TouchButton>()));
			CS$<>8__locals1.nextLeaderboardButton = ((nextMapButton == null) ? null : ((nextMapButton._leaderboardTabButton == null) ? null : nextMapButton._leaderboardTabButton.GetComponent<TouchButton>()));
			CS$<>8__locals1.nextChallengeButton = ((nextMapButton == null) ? null : ((nextMapButton._challengeTabButton == null) ? null : nextMapButton._challengeTabButton.GetComponent<TouchButton>()));
			CS$<>8__locals1.nextModeSelectButton = ((nextMapButton == null) ? null : ((nextMapButton._modeSelectTabButton == null) ? null : nextMapButton._modeSelectTabButton.GetComponent<TouchButton>()));
			TouchButton nextMoreInfoButton = (nextMapButton == null) ? null : nextMapButton.MoreInfoButton;
			if (nextMapButton != null && nextMapButton.IsLocked)
			{
				CS$<>8__locals1.nextMainButton = nextMapButton.LockedCard.TouchButton;
				CS$<>8__locals1.nextLeaderboardButton = nextMapButton.LockedCard.TouchButton;
				CS$<>8__locals1.nextChallengeButton = nextMapButton.LockedCard.TouchButton;
				CS$<>8__locals1.nextModeSelectButton = nextMapButton.LockedCard.TouchButton;
				nextMoreInfoButton = nextMapButton.LockedCard.TouchButton;
			}
			CS$<>8__locals1.mainButton = this._mainTabButton.GetComponent<TouchButton>();
			CS$<>8__locals1.leaderboardButton = this._leaderboardTabButton.GetComponent<TouchButton>();
			CS$<>8__locals1.challengeButton = this._challengeTabButton.GetComponent<TouchButton>();
			CS$<>8__locals1.modeSelectButton = this._modeSelectTabButton.GetComponent<TouchButton>();
			TouchButton bottomMostButton = CS$<>8__locals1.leaderboardButton;
			if (this.IsLocked)
			{
				AnimatedCard.SetNavigationOnLeft(this.LockedCard.TouchButton, prevMainButton);
				AnimatedCard.SetNavigationOnRight(this.LockedCard.TouchButton, CS$<>8__locals1.nextMainButton);
				AnimatedCard.SetNavigationOnUp(this.LockedCard.TouchButton, backButton);
			}
			else
			{
				AnimatedCard.SetNavigationOnLeft(CS$<>8__locals1.mainButton, prevMainButton);
				AnimatedCard.SetNavigationOnRight(CS$<>8__locals1.mainButton, CS$<>8__locals1.nextMainButton);
				AnimatedCard.SetNavigationOnLeft(CS$<>8__locals1.leaderboardButton, prevLeaderboardButton);
				AnimatedCard.SetNavigationOnRight(CS$<>8__locals1.leaderboardButton, CS$<>8__locals1.nextLeaderboardButton);
				AnimatedCard.SetNavigationOnLeft(CS$<>8__locals1.challengeButton, (prevMapButton != null && prevMapButton.IsChallengeMapButton()) ? prevLeaderboardButton : prevChallengeButton);
				AnimatedCard.SetNavigationOnRight(CS$<>8__locals1.challengeButton, (nextMapButton != null && nextMapButton.IsChallengeMapButton()) ? CS$<>8__locals1.nextLeaderboardButton : CS$<>8__locals1.nextChallengeButton);
				AnimatedCard.SetNavigationOnLeft(CS$<>8__locals1.modeSelectButton, (prevMapButton != null && prevMapButton.IsChallengeMapButton()) ? prevLeaderboardButton : prevModeSelectButton);
				AnimatedCard.SetNavigationOnRight(CS$<>8__locals1.modeSelectButton, (nextMapButton != null && nextMapButton.IsChallengeMapButton()) ? CS$<>8__locals1.nextLeaderboardButton : CS$<>8__locals1.nextModeSelectButton);
				AnimatedCard.SetNavigationOnDown(bottomMostButton, playButton);
				switch (this.CurrentCard)
				{
				case MapButton.Card.Main:
					if (this.IsChallengeMapButton())
					{
						AnimatedCard.SetNavigationOnDown(bottomMostButton, this.MainCard.ChallengeButtonSet);
					}
					MapButton.<SetupButtonNavigation>g__SetTabButtonRightNavigation|138_0(true, null, ref CS$<>8__locals1);
					break;
				case MapButton.Card.Leaderboard:
					AnimatedCard.SetNavigationOnDown(bottomMostButton, this.LeaderboardCard.LeaderboardHistogramButton);
					if (this.LeaderboardCard.LeaderboardHistogramButton.isActiveAndEnabled)
					{
						MapButton.<SetupButtonNavigation>g__SetTabButtonRightNavigation|138_0(false, this.LeaderboardCard.LeaderboardHistogramButton, ref CS$<>8__locals1);
					}
					else
					{
						MapButton.<SetupButtonNavigation>g__SetTabButtonRightNavigation|138_0(false, this.LeaderboardCard.LeaderboardSelectorPrevious, ref CS$<>8__locals1);
					}
					AnimatedCard.SetNavigationOnUp(this.LeaderboardCard.LeaderboardSelectorPrevious, backButton);
					AnimatedCard.SetNavigationOnUp(this.LeaderboardCard.LeaderboardSelectorNext, backButton);
					AnimatedCard.SetNavigationOnUp(this.LeaderboardCard.LeaderboardHistogramButton, this.LeaderboardCard.LeaderboardErrorButton);
					AnimatedCard.SetNavigationOnUp(this.LeaderboardCard.LeaderboardGlobalButton, this.LeaderboardCard.LeaderboardErrorButton);
					AnimatedCard.SetNavigationOnUp(this.LeaderboardCard.LeaderboardFriendsButton, this.LeaderboardCard.LeaderboardErrorButton);
					AnimatedCard.SetNavigationOnUp(this.LeaderboardCard.LeaderboardSurroundingButton, this.LeaderboardCard.LeaderboardErrorButton);
					AnimatedCard.SetNavigationOnUp(this.LeaderboardCard.LeaderboardErrorButton, this.LeaderboardCard.LeaderboardSelectorPrevious);
					AnimatedCard.SetNavigationOnDown(this.LeaderboardCard.LeaderboardSelectorPrevious, this.LeaderboardCard.LeaderboardErrorButton);
					AnimatedCard.SetNavigationOnDown(this.LeaderboardCard.LeaderboardSelectorNext, this.LeaderboardCard.LeaderboardErrorButton);
					AnimatedCard.SetNavigationOnDown(this.LeaderboardCard.LeaderboardHistogramButton, playButton);
					AnimatedCard.SetNavigationOnDown(this.LeaderboardCard.LeaderboardGlobalButton, playButton);
					AnimatedCard.SetNavigationOnDown(this.LeaderboardCard.LeaderboardFriendsButton, playButton);
					AnimatedCard.SetNavigationOnDown(this.LeaderboardCard.LeaderboardSurroundingButton, playButton);
					AnimatedCard.SetNavigationOnDown(this.LeaderboardCard.LeaderboardErrorButton, this.LeaderboardCard.LeaderboardHistogramButton);
					AnimatedCard.SetNavigationOnRight(this.LeaderboardCard.LeaderboardSelectorPrevious, this.LeaderboardCard.LeaderboardSelectorNext);
					AnimatedCard.SetNavigationOnLeft(this.LeaderboardCard.LeaderboardSelectorNext, this.LeaderboardCard.LeaderboardSelectorPrevious);
					AnimatedCard.SetNavigationOnLeft(this.LeaderboardCard.LeaderboardSelectorPrevious, CS$<>8__locals1.leaderboardButton);
					AnimatedCard.SetNavigationOnLeft(this.LeaderboardCard.LeaderboardErrorButton, CS$<>8__locals1.leaderboardButton);
					AnimatedCard.SetNavigationOnLeft(this.LeaderboardCard.LeaderboardHistogramButton, CS$<>8__locals1.leaderboardButton);
					AnimatedCard.SetNavigationOnUp(playButton, bottomMostButton);
					AnimatedCard.SetNavigationOnRight(this.LeaderboardCard.LeaderboardSelectorNext, CS$<>8__locals1.nextLeaderboardButton);
					AnimatedCard.SetNavigationOnRight(this.LeaderboardCard.LeaderboardSurroundingButton, CS$<>8__locals1.nextLeaderboardButton);
					break;
				case MapButton.Card.Challenge:
					MapButton.<SetupButtonNavigation>g__SetTabButtonRightNavigation|138_0(false, (nextMapButton != null && nextMapButton.IsChallengeMapButton()) ? CS$<>8__locals1.nextLeaderboardButton : CS$<>8__locals1.nextChallengeButton, ref CS$<>8__locals1);
					AnimatedCard.SetNavigationOnUp(this.ChallengeCard.MoreInfoButton, this.MapSelectScreen.backButton);
					AnimatedCard.SetNavigationOnLeft(this.ChallengeCard.MoreInfoButton, CS$<>8__locals1.challengeButton);
					if (this.ChallengeCard.ShowingCardAsLocked)
					{
						MapButton.<SetupButtonNavigation>g__SetTabButtonRightNavigation|138_0(false, this.ChallengeCard.MoreInfoButton, ref CS$<>8__locals1);
					}
					else
					{
						TouchToggle[] challengeButtons = this.ChallengeCard.ChallengeButtons;
						int challengeButtonCount = challengeButtons.Length;
						for (int challengeButtonIndex = 0; challengeButtonIndex < challengeButtonCount; challengeButtonIndex++)
						{
							TouchToggle innerChallengeButton = challengeButtons[challengeButtonIndex];
							if (challengeButtonIndex == 0)
							{
								MapButton.<SetupButtonNavigation>g__SetTabButtonRightNavigation|138_0(false, innerChallengeButton, ref CS$<>8__locals1);
							}
							AnimatedCard.SetNavigationOnLeft(innerChallengeButton, CS$<>8__locals1.challengeButton);
							AnimatedCard.SetNavigationOnRight(innerChallengeButton, CS$<>8__locals1.nextChallengeButton);
						}
						AnimatedCard.SetNavigationOnLeft(this.ChallengeCard.ChallengeModifiersButton, CS$<>8__locals1.challengeButton);
						this.ChallengeCard.SetupChallengeModifiersButtonNavigation();
						bool hasSelectedChallenge = this.ChallengeCard.SelectedCityChallengeIndex != -1;
						if (challengeButtonCount > 0)
						{
							TouchToggle lastChallengeButton = challengeButtons[challengeButtonCount - 1];
							AnimatedCard.SetNavigationOnUp(this.ChallengeCard.ChallengeModifiersButton, lastChallengeButton);
							AnimatedCard.SetNavigationOnDown(this.ChallengeCard.ChallengeModifiersButton, hasSelectedChallenge ? playButton : null);
						}
						AnimatedCard.SetNavigationOnDown(bottomMostButton, hasSelectedChallenge ? this._screen.firstFocus : null);
					}
					this.ChallengeCard.OnChallengeSelected += this.OnChallengeSelected;
					break;
				case MapButton.Card.Mode:
					MapButton.<SetupButtonNavigation>g__SetTabButtonRightNavigation|138_0(false, this.ModeSelectCard.NormalButton, ref CS$<>8__locals1);
					AnimatedCard.SetNavigationOnLeft(this.ModeSelectCard.NormalButton, CS$<>8__locals1.modeSelectButton);
					AnimatedCard.SetNavigationOnLeft(this.ModeSelectCard.EndlessButton, CS$<>8__locals1.modeSelectButton);
					AnimatedCard.SetNavigationOnLeft(this.ModeSelectCard.ExpertButton, CS$<>8__locals1.modeSelectButton);
					AnimatedCard.SetNavigationOnLeft(this.ModeSelectCard.CreativeButton, CS$<>8__locals1.modeSelectButton);
					AnimatedCard.SetNavigationOnDown(this.ModeSelectCard.CreativeButton, playButton);
					AnimatedCard.SetNavigationOnUp(this.ModeSelectCard.InfoButton, backButton);
					if (this.GetCurrentSelectedGameMode() == GameMode.Normal)
					{
						AnimatedCard.SetNavigationOnUp(this.ModeSelectCard.NormalButton, backButton);
					}
					else
					{
						AnimatedCard.SetNavigationOnUp(this.ModeSelectCard.NormalButton, this.ModeSelectCard.InfoButton);
					}
					AnimatedCard.SetNavigationOnDown(CS$<>8__locals1.modeSelectButton, CS$<>8__locals1.challengeButton);
					AnimatedCard.SetNavigationOnRight(this.ModeSelectCard.NormalButton, CS$<>8__locals1.nextModeSelectButton);
					AnimatedCard.SetNavigationOnRight(this.ModeSelectCard.EndlessButton, CS$<>8__locals1.nextModeSelectButton);
					AnimatedCard.SetNavigationOnRight(this.ModeSelectCard.ExpertButton, CS$<>8__locals1.nextModeSelectButton);
					AnimatedCard.SetNavigationOnRight(this.ModeSelectCard.CreativeButton, CS$<>8__locals1.nextModeSelectButton);
					AnimatedCard.SetNavigationOnRight(this.ModeSelectCard.InfoButton, CS$<>8__locals1.nextModeSelectButton);
					break;
				}
			}
			if (this.IsChallengeMapButton())
			{
				AnimatedCard.SetNavigationOnUp(this.ColorfulSelect, this.MoreInfoButton);
				AnimatedCard.SetNavigationOnUp(this.DarkSelect, this.MoreInfoButton);
				AnimatedCard.SetNavigationOnUp(this.MapsSelect, this.MoreInfoButton);
				AnimatedCard.SetNavigationOnUp(this.MoreInfoButton, backButton);
				AnimatedCard.SetNavigationOnUp(this.MainCard.ChallengeButtonSet, bottomMostButton);
				AnimatedCard.SetNavigationOnDown(this.MainCard.ChallengeButtonSet, this._screen.firstFocus);
				AnimatedCard.SetNavigationOnLeft(this.MoreInfoButton, (prevMoreInfoButton != null) ? prevMoreInfoButton : prevMainButton);
				AnimatedCard.SetNavigationOnRight(this.MoreInfoButton, (nextMoreInfoButton != null) ? nextMoreInfoButton : prevMainButton);
				AnimatedCard.SetNavigationOnUp(CS$<>8__locals1.leaderboardButton, CS$<>8__locals1.mainButton);
				AnimatedCard.SetNavigationOnDown(CS$<>8__locals1.mainButton, CS$<>8__locals1.leaderboardButton);
			}
			else
			{
				AnimatedCard.SetNavigationOnUp(this.ColorfulSelect, backButton);
				AnimatedCard.SetNavigationOnUp(this.DarkSelect, backButton);
				AnimatedCard.SetNavigationOnUp(this.MapsSelect, backButton);
			}
			AnimatedCard.SetNavigationOnDown(this.ColorfulSelect, CS$<>8__locals1.mainButton);
			AnimatedCard.SetNavigationOnDown(this.DarkSelect, CS$<>8__locals1.mainButton);
			AnimatedCard.SetNavigationOnDown(this.MapsSelect, CS$<>8__locals1.mainButton);
			this._selectedTheme = themePreference;
			this.EnsureThemeButtonSelectedState(null);
			this.SetupNavigationToThemeButtons(themePreference);
		}

		// Token: 0x060032BC RID: 12988 RVA: 0x000F0058 File Offset: 0x000EE258
		private void OnChallengeSelected()
		{
			bool hasSelectedChallenge = this.ChallengeCard.SelectedCityChallengeIndex != -1;
			AnimatedCard.SetNavigationOnDown(this._leaderboardTabButton.GetComponent<TouchButton>(), hasSelectedChallenge ? this._screen.firstFocus : null);
			AnimatedCard.SetNavigationOnDown(this.ChallengeCard.ChallengeModifiersButton, hasSelectedChallenge ? this._screen.firstFocus : null);
			this.ResetModeSelection();
		}

		// Token: 0x060032BD RID: 12989 RVA: 0x000F00BF File Offset: 0x000EE2BF
		public bool IsChallengeMapButton()
		{
			return this.Type == MapButton.MapButtonType.DailyChallenge || this.Type == MapButton.MapButtonType.WeeklyChallenge;
		}

		// Token: 0x060032BE RID: 12990 RVA: 0x000F00D8 File Offset: 0x000EE2D8
		private void SetupNavigationToThemeButtons(MotorwaysThemePreference themePreference)
		{
			Selectable component = this._mainTabButton.GetComponent<TouchButton>();
			ThemeSelectButton themeSelectButton = this.GetThemeButton(themePreference);
			AnimatedCard.SetNavigationOnUp(component, themeSelectButton);
			if (this.IsChallengeMapButton())
			{
				AnimatedCard.SetNavigationOnDown(this.MoreInfoButton, themeSelectButton);
			}
		}

		// Token: 0x060032BF RID: 12991 RVA: 0x000F0112 File Offset: 0x000EE312
		private ThemeSelectButton GetThemeButton(MotorwaysThemePreference themePreference)
		{
			switch (themePreference)
			{
			case MotorwaysThemePreference.Dark:
			case MotorwaysThemePreference.DarkColorblind:
				return this.DarkSelect;
			case MotorwaysThemePreference.Maps:
				return this.MapsSelect;
			}
			return this.ColorfulSelect;
		}

		// Token: 0x17000862 RID: 2146
		// (get) Token: 0x060032C0 RID: 12992 RVA: 0x000F0142 File Offset: 0x000EE342
		private bool ShowsChallengeTab
		{
			get
			{
				return !FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo) && FeatureToggle.IsFeatureEnabled(Feature.CityChallenges) && this._mapChallenge == null;
			}
		}

		// Token: 0x060032C1 RID: 12993 RVA: 0x000F0164 File Offset: 0x000EE364
		private void ShowRelevantTabs()
		{
			this._mainTabButton.Show();
			this._leaderboardTabButton.Show();
			if (this.ShowsChallengeTab)
			{
				this._challengeTabButton.Show();
				this._modeSelectTabButton.Show();
			}
			else
			{
				this._challengeTabButton.Hide();
				this._modeSelectTabButton.Hide();
				this._leaderboardTabButton.transform.position = this._modeSelectTabButton.transform.position;
			}
			this._screen.SetScreenButtonNavigation();
		}

		// Token: 0x060032C2 RID: 12994 RVA: 0x000F01E8 File Offset: 0x000EE3E8
		public void SetSelected(bool isSelected)
		{
			if (isSelected)
			{
				base.interactable = false;
				if (this._currentCard != MapButton.Card.Locked)
				{
					this.ShowRelevantTabs();
				}
				Action<MapButton> action = this.onSelected;
				if (action != null)
				{
					action(this);
				}
			}
			else
			{
				base.interactable = true;
				if (this._currentCard != MapButton.Card.Locked)
				{
					this._mainTabButton.Hide();
					this._leaderboardTabButton.Hide();
					this._challengeTabButton.Hide();
					this._modeSelectTabButton.Hide();
				}
				this.DeselectCityChallenge();
				if (this._currentCard != MapButton.Card.Main && this._currentCard != MapButton.Card.Locked)
				{
					this.OnMainTabSelected();
					if (this.Type != MapButton.MapButtonType.DailyChallenge && this.Type != MapButton.MapButtonType.WeeklyChallenge)
					{
						this.SetupFrontCardForDefaultState();
					}
				}
			}
			this._mainCard.OnMapButtonSelected(isSelected);
			if (isSelected && this.IsNewContentItem(this._scope))
			{
				base.SetNewContentSeen(this._scope);
				if (!base.IsNewContent(this._scope))
				{
					base.PlayNewContentIndicatorExit();
				}
			}
		}

		// Token: 0x060032C3 RID: 12995 RVA: 0x000F02D0 File Offset: 0x000EE4D0
		public override bool IsNewContentItem(IScope appScope)
		{
			return base.IsNewContentItem(appScope) && !this.IsLocked;
		}

		// Token: 0x060032C4 RID: 12996 RVA: 0x000F02E6 File Offset: 0x000EE4E6
		public void SetChallengeIcons(ChallengeData[] challenges, ChallengeDatabase challengeDatabase)
		{
			this._mainCard.SetChallengeIcons(challenges, challengeDatabase);
		}

		// Token: 0x060032C5 RID: 12997 RVA: 0x000F02F5 File Offset: 0x000EE4F5
		public override void SetSelectedValue(float distance)
		{
			base.SetSelectedValue(distance);
			this.ColorfulSelect.SetSelectorAlpha(distance);
			this.DarkSelect.SetSelectorAlpha(distance);
			this.MapsSelect.SetSelectorAlpha(distance);
		}

		// Token: 0x060032C6 RID: 12998 RVA: 0x000F0324 File Offset: 0x000EE524
		public override void OnCardConfirmed()
		{
			base.OnCardConfirmed();
			if (this._currentCard != MapButton.Card.Main)
			{
				if (this._currentCard == MapButton.Card.Challenge)
				{
					this.SetupFrontCardForCityChallenge(this.SelectedChallengeIndex);
				}
				this.OnMainTabSelected();
			}
			this._mainTabButton.Hide();
			this._leaderboardTabButton.Hide();
			this._challengeTabButton.Hide();
			this._modeSelectTabButton.Hide();
		}

		// Token: 0x060032C7 RID: 12999 RVA: 0x000F0388 File Offset: 0x000EE588
		public void Initialize(MapSelectScreen screen, IScope scope, VisualConstantsData visualConstants)
		{
			if (FeatureToggle.IsFeatureDisabled(Feature.RandomChallengesMapButton))
			{
				Diagnostics.FailAssert("Tried to initialize a random map button without having the feature enabled!", Array.Empty<object>());
			}
			this._scope = scope;
			this._player = this._scope.Get<ActivePlayer>();
			this._visualConstants = visualConstants;
			this.IsRandomChallengeCard = true;
			this.AssignRandomMapChallenge();
			this.Initialize(screen, this._mapDefinition, scope, 0, this._visualConstants, this._mapChallenge);
			this.Type = MapButton.MapButtonType.City;
			this._mainCard.Header.SetStringId(scope, StringId.Challenge_RandomChallengesMapTitle);
			this._mainCard.Description.SetStringId(scope, StringId.Challenge_RandomChallengesMapDescription);
			this._mainCard.BestScoreText.gameObject.SetActive(false);
			this._mainCard.CurrentModeText.gameObject.SetActive(false);
			this._mainTabButton.gameObject.SetActive(false);
			this._leaderboardTabButton.gameObject.SetActive(false);
			this._challengeTabButton.gameObject.SetActive(false);
			this._modeSelectTabButton.gameObject.SetActive(false);
		}

		// Token: 0x060032C8 RID: 13000 RVA: 0x000F049C File Offset: 0x000EE69C
		public void Initialize(MapSelectScreen screen, MapDefinition definition, IScope scope, int bestScore, VisualConstantsData visualConstants, MapChallenge mapChallenge = null)
		{
			this._screen = screen;
			this._mapDefinition = definition;
			this._scope = scope;
			this._player = this._scope.Get<ActivePlayer>();
			this._mapChallenge = mapChallenge;
			this.Type = MapButton.GetButtonType(mapChallenge);
			this._challengeTimeLeftKey = scope.Get<MotorwaysStringKey>();
			this._visualConstants = visualConstants;
			if (this._mainCard != null)
			{
				UnityEngine.Object.Destroy(this._mainCard.gameObject);
			}
			switch (this.Type)
			{
			case MapButton.MapButtonType.City:
				this._mainCard = UnityEngine.Object.Instantiate<MapButtonMainCard>(this.cityCardPrefab, this.mainCardParent);
				this._mainCard.parentMapButton = this;
				this._mainCard.Header.SetStringId(scope, definition.mapName);
				this._mainCard.Description.SetStringId(scope, definition.mapDescription);
				this.SetBestScoreTextOnMainCard(scope, bestScore);
				break;
			case MapButton.MapButtonType.DailyChallenge:
				this._mainCard = UnityEngine.Object.Instantiate<MapButtonMainCard>(this._mainChallengeCardPrefab, this.mainCardParent);
				this._mainCard.Header.SetStringId(this._scope, StringId.DailyChallenge);
				this._mainCard.parentMapButton = this;
				this.SetChallengeData(mapChallenge, scope, bestScore);
				break;
			case MapButton.MapButtonType.WeeklyChallenge:
				this._mainCard = UnityEngine.Object.Instantiate<MapButtonMainCard>(this._mainChallengeCardPrefab, this.mainCardParent);
				this._mainCard.Header.SetStringId(this._scope, StringId.WeeklyChallenge);
				this._mainCard.parentMapButton = this;
				this.SetChallengeData(mapChallenge, scope, bestScore);
				break;
			}
			this._mainCard.SetMapType(definition.isTrainMap, definition.isBoatMap);
			this.BaseInitializeCard(this._mainCard);
			this._mainCard.onMoreChallengeInfoPressed += delegate()
			{
				Action action = this.onShowMoreChallengeInfo;
				if (action == null)
				{
					return;
				}
				action();
			};
			this._mainCard.ColorfulSelect.mapButton = this;
			this._mainCard.DarkSelect.mapButton = this;
			this._mainCard.MapsSelect.mapButton = this;
			if (this.LockedCard != null)
			{
				UnityEngine.Object.Destroy(this.LockedCard.gameObject);
			}
			if (this.LeaderboardCard != null)
			{
				UnityEngine.Object.Destroy(this.LeaderboardCard.gameObject);
			}
			if (this.ModeSelectCard != null)
			{
				UnityEngine.Object.Destroy(this.ModeSelectCard.gameObject);
			}
			this._themeDatabase = scope.Get<MotorwaysThemeDatabase>();
			this._previousTheme = this._mapDefinition.themes[(int)this._themeDatabase.ThemePreference];
			this.UnregisterThemeComponents();
			this.RegisterThemeComponents();
			this._mainCard.PreviewImage.sprite = this._mapDefinition.themePreviewSprites[(int)this._themeDatabase.ThemePreference];
			this._mainTabButton.OnClicked();
			this._leaderboardTabButton.OnOtherTabSelected();
			this._challengeTabButton.OnOtherTabSelected();
			this._modeSelectTabButton.OnOtherTabSelected();
			if (!this.AreChallengesLocked)
			{
				this._challengeTabButton.TouchButton.SetNewContentID(MapButtonChallengeCard.GetNewContentIndicatorID(definition), true, true);
			}
			if (this._mapDefinition.IsExpertModeUnlocked(this._scope))
			{
				this._modeSelectTabButton.TouchButton.SetNewContentID(MapButtonModeSelectCard.GetNewContentIndicatorID(definition), true, true);
			}
			this._currentCard = MapButton.Card.Main;
			this.SetNextCard();
			this._mainCard.Initialize(this._scope, this._visualConstants);
			if (!this.ShowsChallengeTab)
			{
				this._challengeTabButton.gameObject.SetActive(false);
				this._modeSelectTabButton.gameObject.SetActive(false);
			}
		}

		// Token: 0x060032C9 RID: 13001 RVA: 0x000F0814 File Offset: 0x000EEA14
		private void InitializeRecurringLeaderboardSelector()
		{
			TouchOptionButton recurringLeaderboardSelector = this.LeaderboardCard.RecurringLeaderboardSelector;
			recurringLeaderboardSelector.gameObject.SetActive(false);
			recurringLeaderboardSelector.onOptionChanged.AddListener(new UnityAction<int>(this.OnLeaderboardSelectorChanged));
			if (this._mapChallenge != null)
			{
				recurringLeaderboardSelector.gameObject.SetActive(true);
				if (this._mapChallenge.type == MapChallenge.ChallengeType.Daily)
				{
					ChallengeSystem challengeSystem = this._scope.Get<ChallengeSystem>();
					this.SetRecurringLeaderboardStartDay(challengeSystem.DailyChallenge.StartOfChallenge.DayOfWeek);
					recurringLeaderboardSelector.SetOption(recurringLeaderboardSelector.options.Length - 1, false);
				}
				else if (this._mapChallenge.type == MapChallenge.ChallengeType.Weekly)
				{
					recurringLeaderboardSelector.options = this.LeaderboardCard.RecurringWeekOptions;
					recurringLeaderboardSelector.SetOption(recurringLeaderboardSelector.options.Length - 1, false);
				}
			}
			else if (this._mapDefinition.cityChallenges.Length != 0)
			{
				recurringLeaderboardSelector.gameObject.SetActive(true);
				recurringLeaderboardSelector.options = this.LeaderboardCard.RecurringTypeOptions;
				recurringLeaderboardSelector.SetOption(0, false);
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				recurringLeaderboardSelector.gameObject.SetActive(false);
			}
		}

		// Token: 0x060032CA RID: 13002 RVA: 0x000F0928 File Offset: 0x000EEB28
		public void SetRecurringLeaderboardStartDay(DayOfWeek dayOfWeek)
		{
			TouchOptionButton recurringLeaderboardSelector = this.LeaderboardCard.RecurringLeaderboardSelector;
			GameObject[] sortedDays = new GameObject[7];
			for (int dayIndex = 0; dayIndex < 7; dayIndex++)
			{
				int offsetIndex = (int)((dayIndex + dayOfWeek + 1) % (DayOfWeek)7);
				sortedDays[dayIndex] = this.LeaderboardCard.RecurringDayOptions[offsetIndex];
			}
			recurringLeaderboardSelector.options = sortedDays;
		}

		// Token: 0x060032CB RID: 13003 RVA: 0x000F0974 File Offset: 0x000EEB74
		public void RefreshLeaderboardOptions(MapChallenge challenge, IScope scope)
		{
			if (this.LeaderboardCard != null)
			{
				if (challenge.type == MapChallenge.ChallengeType.Daily)
				{
					ChallengeSystem challengeSystem = scope.Get<ChallengeSystem>();
					this.SetRecurringLeaderboardStartDay(challengeSystem.DailyChallenge.StartOfChallenge.DayOfWeek);
				}
				TouchOptionButton recurringLeaderboardSelector = this.LeaderboardCard.RecurringLeaderboardSelector;
				recurringLeaderboardSelector.SetOption(recurringLeaderboardSelector.options.Length - 1, false);
			}
		}

		// Token: 0x060032CC RID: 13004 RVA: 0x000F09D4 File Offset: 0x000EEBD4
		private void InitializeLeaderboardPanel(MapButton button)
		{
			if (!Diagnostics.Verify(button.LeaderboardCard.LeaderboardPanel != null))
			{
				return;
			}
			bool showSelectedChallenge = this.LeaderboardShowsSelectedChallenge;
			if (!showSelectedChallenge)
			{
				this.DeselectCityChallenge();
			}
			button.LeaderboardCard.LeaderboardPanel.Initialize(this._scope, this.LeaderboardCard.RecurringLeaderboardSelector, this);
			if (this.GetCurrentSelectedGameMode() == GameMode.Expert && !showSelectedChallenge)
			{
				this.LeaderboardCard.RecurringLeaderboardSelector.SetOption(1);
			}
			LeaderboardId leaderboardId = this.GetLeaderboardIdForMapButton();
			button.LeaderboardCard.LeaderboardPanel.ShowLeaderboardFor(this.GetDefaultLeaderboardType(), leaderboardId);
			if (showSelectedChallenge)
			{
				this.DeselectCityChallenge();
			}
		}

		// Token: 0x060032CD RID: 13005 RVA: 0x000F0A70 File Offset: 0x000EEC70
		private void OnLeaderboardSelectorChanged(int index)
		{
			this.LeaderboardCard.LeaderboardPanel.ShowLeaderboardFor(this.GetDefaultLeaderboardType(), this.GetLeaderboardIdForMapButton());
		}

		// Token: 0x060032CE RID: 13006 RVA: 0x000F0A90 File Offset: 0x000EEC90
		private LeaderboardType GetDefaultLeaderboardType()
		{
			if (this._screen.PlayerSelectedLeaderboardType != null)
			{
				LeaderboardType playerSelectedValue = this._screen.PlayerSelectedLeaderboardType.Value;
				if (playerSelectedValue != LeaderboardType.Global || this.IsChallengeMapButton())
				{
					return playerSelectedValue;
				}
			}
			return LeaderboardType.Histogram;
		}

		// Token: 0x060032CF RID: 13007 RVA: 0x000F0AD4 File Offset: 0x000EECD4
		public void AssignRandomMapChallenge()
		{
			ChallengeSystem challengeSystem = this._scope.Get<ChallengeSystem>();
			ChallengeDatabase challengeDatabase = this._scope.Get<ChallengeDatabase>();
			this._mapChallenge = MapChallenge.CreateMysteryChallenge(challengeSystem, challengeDatabase);
			this._mapDefinition = this._mapChallenge.mapDefinition;
		}

		// Token: 0x060032D0 RID: 13008 RVA: 0x000F0B18 File Offset: 0x000EED18
		private static MapButton.MapButtonType GetButtonType(MapChallenge mapChallenge)
		{
			MapButton.MapButtonType type = MapButton.MapButtonType.City;
			if (mapChallenge != null)
			{
				if (mapChallenge.type == MapChallenge.ChallengeType.Daily)
				{
					type = MapButton.MapButtonType.DailyChallenge;
				}
				else if (mapChallenge.type == MapChallenge.ChallengeType.Weekly)
				{
					type = MapButton.MapButtonType.WeeklyChallenge;
				}
			}
			return type;
		}

		// Token: 0x060032D1 RID: 13009 RVA: 0x000F0B44 File Offset: 0x000EED44
		public void SetChallengeData(MapChallenge mapChallenge, IScope scope, int bestScore = 0)
		{
			this._mapChallenge = mapChallenge;
			this._mapDefinition = mapChallenge.mapDefinition;
			this._mainCard.Header.SetStringId(scope, (this.Type == MapButton.MapButtonType.DailyChallenge) ? StringId.DailyChallenge : StringId.WeeklyChallenge);
			this._mainCard.Description.SetStringId(scope, mapChallenge.mapDefinition.mapName);
			this._isUpdatingChallenge = false;
			this.SetBestScoreTextOnMainCard(scope, bestScore);
			this._mainCard.SetMapType(mapChallenge.mapDefinition.isTrainMap, mapChallenge.mapDefinition.isBoatMap);
		}

		// Token: 0x060032D2 RID: 13010 RVA: 0x000F0BD8 File Offset: 0x000EEDD8
		public void SetBestScoreTextOnModeCard(IScope scope, int bestScore)
		{
			if (this.ModeSelectCard != null)
			{
				this.SetBestScoreText(scope, bestScore, this.ModeSelectCard.BestScoreText);
			}
		}

		// Token: 0x060032D3 RID: 13011 RVA: 0x000F0BFB File Offset: 0x000EEDFB
		public void SetBestScoreTextOnMainCard(IScope scope, int bestScore)
		{
			this.SetBestScoreText(scope, bestScore, this.MainCard.BestScoreText);
		}

		// Token: 0x060032D4 RID: 13012 RVA: 0x000F0C10 File Offset: 0x000EEE10
		private void SetBestScoreText(IScope scope, int bestScore, LocalizedTextUI textUI)
		{
			if (this.IsRandomChallengeCard)
			{
				return;
			}
			MotorwaysStringKey bestScoreStringKey = scope.Get<MotorwaysStringKey>();
			if (this.Type != MapButton.MapButtonType.City)
			{
				if (this.IsChallengeMapButton())
				{
					if (bestScore != -2)
					{
						if (bestScore == -1)
						{
							bestScoreStringKey.InitWithStringId(StringId.New);
						}
						else
						{
							bestScoreStringKey.InitWithStringId(StringId.Score, bestScore, new Dictionary<string, string>
							{
								{
									"Num",
									bestScore.ToString()
								}
							});
						}
					}
					else
					{
						bestScoreStringKey.InitWithStringId(StringId.InProgress);
					}
					textUI.LocString = StandaloneLocString.CreateString(scope, bestScoreStringKey);
				}
				return;
			}
			GameMode gameMode = this.GetCurrentSelectedGameMode();
			if (gameMode == GameMode.Endless || gameMode == GameMode.Creative)
			{
				textUI.gameObject.SetActive(false);
				return;
			}
			textUI.gameObject.SetActive(true);
			bestScoreStringKey.InitWithStringId(StringId.BestScore, bestScore, new Dictionary<string, string>
			{
				{
					"Num",
					bestScore.ToString()
				}
			});
			textUI.LocString = StandaloneLocString.CreateString(scope, bestScoreStringKey);
		}

		// Token: 0x060032D5 RID: 13013 RVA: 0x000F0CDD File Offset: 0x000EEEDD
		private void OnMainTabSelected()
		{
			if (this._currentCard != MapButton.Card.Main)
			{
				this.ShowCard(MapButton.Card.Main);
			}
		}

		// Token: 0x060032D6 RID: 13014 RVA: 0x000F0CEE File Offset: 0x000EEEEE
		[UsedImplicitly]
		public void OnMainTabClicked()
		{
			this.DeselectCityChallenge();
			if (this.Type == MapButton.MapButtonType.City)
			{
				this.SetupFrontCardForDefaultState();
			}
			this.OnMainTabSelected();
		}

		// Token: 0x060032D7 RID: 13015 RVA: 0x000F0D0C File Offset: 0x000EEF0C
		private void SetupFrontCardForDefaultState()
		{
			GameMode gameMode = this.GetCurrentSelectedGameMode();
			int score = this.MapSelectScreen.GetBestScoreForCityLeaderboard(this._mapDefinition.cityName, gameMode);
			this.SetBestScoreTextOnMainCard(this._scope, score);
			this._mainCard.Description.LocString = StandaloneLocString.CreateString(this._scope, this.MapDefinition.mapDescription);
		}

		// Token: 0x060032D8 RID: 13016 RVA: 0x000F0D6C File Offset: 0x000EEF6C
		public void SetupFrontCardForCityChallenge(int challengeIndex)
		{
			CityChallengeStatistics stats = this._player.GetCityChallengeScore(this.MapDefinition.cityName, GameMode.Normal, challengeIndex, true);
			this.SetBestScoreTextOnMainCard(this._scope, stats.BestScore);
			this._mainCard.Description.LocString = StandaloneLocString.CreateString(this._scope, this.MapDefinition.cityChallenges[challengeIndex].titleStringId);
		}

		// Token: 0x060032D9 RID: 13017 RVA: 0x000F0DD2 File Offset: 0x000EEFD2
		public void ShowCard(MapButton.Card card)
		{
			this._currentCard = card;
			base.TweenToNextCard();
			base.onAnimationMidFlip += this.OnTabSelectMidFlip;
		}

		// Token: 0x060032DA RID: 13018 RVA: 0x000F0DF4 File Offset: 0x000EEFF4
		public void OnLeaderboardTabSelected()
		{
			if (this._currentCard != MapButton.Card.Leaderboard)
			{
				this.ShowCard(MapButton.Card.Leaderboard);
			}
		}

		// Token: 0x060032DB RID: 13019 RVA: 0x000F0E06 File Offset: 0x000EF006
		public void OnChallengeTabSelected()
		{
			if (this._currentCard != MapButton.Card.Challenge)
			{
				this.ShowCard(MapButton.Card.Challenge);
			}
		}

		// Token: 0x060032DC RID: 13020 RVA: 0x000F0E18 File Offset: 0x000EF018
		public override void OnTabSelectMidFlip()
		{
			this.SetNextCard();
			base.OnTabSelectMidFlip();
		}

		// Token: 0x060032DD RID: 13021 RVA: 0x000F0E26 File Offset: 0x000EF026
		public void OnChallengeModeMoreInfoButtonClicked()
		{
			this._screen.ShowChallengeModeInfoPopup();
		}

		// Token: 0x060032DE RID: 13022 RVA: 0x000F0E33 File Offset: 0x000EF033
		private void OnModeSelectTabSelected()
		{
			if (this._currentCard != MapButton.Card.Mode)
			{
				this.ShowCard(MapButton.Card.Mode);
			}
		}

		// Token: 0x060032DF RID: 13023 RVA: 0x000F0E45 File Offset: 0x000EF045
		[UsedImplicitly]
		public void OnModeSelectTabClicked()
		{
			this.DeselectCityChallenge();
			if (this.Type == MapButton.MapButtonType.City)
			{
				this.SetupFrontCardForDefaultState();
			}
			this.OnModeSelectTabSelected();
		}

		// Token: 0x060032E0 RID: 13024 RVA: 0x000F0E64 File Offset: 0x000EF064
		public void RefreshTabs()
		{
			switch (this._currentCard)
			{
			case MapButton.Card.Main:
				this._mainTabButton.SetSelected(true);
				this._leaderboardTabButton.SetSelected(false);
				this._challengeTabButton.SetSelected(false);
				this._modeSelectTabButton.SetSelected(false);
				return;
			case MapButton.Card.Leaderboard:
				this._mainTabButton.SetSelected(false);
				this._leaderboardTabButton.SetSelected(true);
				this._challengeTabButton.SetSelected(false);
				this._modeSelectTabButton.SetSelected(false);
				return;
			case MapButton.Card.Locked:
				this._mainTabButton.SetSelected(false);
				this._leaderboardTabButton.SetSelected(false);
				this._challengeTabButton.SetSelected(false);
				this._modeSelectTabButton.SetSelected(false);
				return;
			case MapButton.Card.Challenge:
				this._mainTabButton.SetSelected(false);
				this._leaderboardTabButton.SetSelected(false);
				this._challengeTabButton.SetSelected(true);
				this._modeSelectTabButton.SetSelected(false);
				return;
			case MapButton.Card.Mode:
				this._mainTabButton.SetSelected(false);
				this._leaderboardTabButton.SetSelected(false);
				this._challengeTabButton.SetSelected(false);
				this._modeSelectTabButton.SetSelected(true);
				return;
			default:
				return;
			}
		}

		// Token: 0x060032E1 RID: 13025 RVA: 0x000F0F88 File Offset: 0x000EF188
		private void SetNextCard()
		{
			switch (this._currentCard)
			{
			case MapButton.Card.Main:
				this.SetCardVisible(this._mainCard);
				this.SetCardInvisible(this.LeaderboardCard);
				this.SetCardInvisible(this.LockedCard);
				this.SetCardInvisible(this.ChallengeCard);
				this.SetCardInvisible(this.ModeSelectCard);
				this._mainTabButton.OnClicked();
				this._leaderboardTabButton.OnOtherTabSelected();
				this._challengeTabButton.OnOtherTabSelected();
				this._modeSelectTabButton.OnOtherTabSelected();
				this.SetExpanded(AnimatedCard.ExpansionLevel.Narrow);
				break;
			case MapButton.Card.Leaderboard:
				if (this.LeaderboardCard == null)
				{
					this.LeaderboardCard = UnityEngine.Object.Instantiate<MapButtonLeaderboardCard>(this._leaderboardCardPrefab, this.leaderboardCardParent);
					this.BaseInitializeCard(this.LeaderboardCard);
					this.InitializeRecurringLeaderboardSelector();
				}
				this.SetCardInvisible(this._mainCard);
				this.SetCardVisible(this.LeaderboardCard);
				this.SetCardInvisible(this.LockedCard);
				this.SetCardInvisible(this.ChallengeCard);
				this.SetCardInvisible(this.ModeSelectCard);
				this._mainTabButton.OnOtherTabSelected();
				this._leaderboardTabButton.OnClicked();
				this._challengeTabButton.OnOtherTabSelected();
				this._modeSelectTabButton.OnOtherTabSelected();
				this.SetExpanded(AnimatedCard.ExpansionLevel.Wide);
				this.InitializeLeaderboardPanel(this);
				break;
			case MapButton.Card.Locked:
				if (this.LockedCard == null)
				{
					this.LockedCard = UnityEngine.Object.Instantiate<MapButtonLockedCard>(this._lockedCardPrefab, this._lockCardParent);
					this.BaseInitializeCard(this.LockedCard);
					this.LockedCard.Header.SetStringId(this._scope, this._mapDefinition.mapName);
					this.LockedCard.OnNavButtonClicked += this.ScrollToMe;
				}
				this.SetCardInvisible(this._mainCard);
				this.SetCardInvisible(this.LeaderboardCard);
				this.SetCardInvisible(this.ModeSelectCard);
				this.SetCardVisible(this.LockedCard);
				this.SetCardInvisible(this.ChallengeCard);
				this._mainTabButton.Hide();
				this._leaderboardTabButton.Hide();
				break;
			case MapButton.Card.Challenge:
				if (this.ChallengeCard == null)
				{
					this.ChallengeCard = UnityEngine.Object.Instantiate<MapButtonChallengeCard>(this._challengeCardPrefab, this._challengeCardParent);
					this.BaseInitializeCard(this.ChallengeCard);
					this.ChallengeCard.Initialize(this._scope, this);
				}
				this.ChallengeCard.UpdateChallengeButtonScores();
				if (this.LeaderboardShowsSelectedChallenge)
				{
					this.ChallengeCard.SelectChallengeIndex(this.SelectedChallengeIndex);
					this.LeaderboardShowsSelectedChallenge = true;
				}
				this.SetCardInvisible(this._mainCard);
				this.SetCardInvisible(this.LeaderboardCard);
				this.SetCardInvisible(this.LockedCard);
				this.SetCardInvisible(this.ModeSelectCard);
				this.SetCardVisible(this.ChallengeCard);
				this._challengeTabButton.OnClicked();
				this._leaderboardTabButton.OnOtherTabSelected();
				this._mainTabButton.OnOtherTabSelected();
				this._modeSelectTabButton.OnOtherTabSelected();
				this.SetExpanded(AnimatedCard.ExpansionLevel.Wide);
				break;
			case MapButton.Card.Mode:
				if (this.ModeSelectCard == null)
				{
					this.ModeSelectCard = UnityEngine.Object.Instantiate<MapButtonModeSelectCard>(this._modeSelectCardPrefab, this._modeSelectCardParent);
					this.BaseInitializeCard(this.ModeSelectCard);
					this.ModeSelectCard.Initialize(this._scope, this._visualConstants, this);
					GameMode gameMode = this.GetCurrentSelectedGameMode();
					int score = this.MapSelectScreen.GetBestScoreForCityLeaderboard(this._mapDefinition.cityName, gameMode);
					this.SetBestScoreTextOnModeCard(this._scope, score);
					this.ModeSelectCard.onMoreModeInfoPressed += delegate()
					{
						Action action = this.onShowModeInfo;
						if (action == null)
						{
							return;
						}
						action();
					};
					this.ModeSelectCard.onModePressed += delegate()
					{
						this.OnModeSelected();
					};
					this.ModeSelectCard.onExpertLockedPressed += delegate()
					{
						Action action = this.onExpertModeLockedPressed;
						if (action == null)
						{
							return;
						}
						action();
					};
				}
				base.onFlipAnimationComplete -= this.ModeSelectCard.UpdateButtonLockStatus;
				base.onFlipAnimationComplete += this.ModeSelectCard.UpdateButtonLockStatus;
				this.SetCardVisible(this.ModeSelectCard);
				this.SetCardInvisible(this._mainCard);
				this.SetCardInvisible(this.LeaderboardCard);
				this.SetCardInvisible(this.LockedCard);
				this.SetCardInvisible(this.ChallengeCard);
				this._modeSelectTabButton.OnClicked();
				this._leaderboardTabButton.OnOtherTabSelected();
				this._challengeTabButton.OnOtherTabSelected();
				this._mainTabButton.OnOtherTabSelected();
				this.SetExpanded(AnimatedCard.ExpansionLevel.Medium);
				break;
			}
			this.SetupButtonNavigation();
			this.ColorfulSelect.gameObject.SetActive(this._currentCard == MapButton.Card.Main);
			this.DarkSelect.gameObject.SetActive(this._currentCard == MapButton.Card.Main);
			this.MapsSelect.gameObject.SetActive(this._currentCard == MapButton.Card.Main && !this._themeDatabase.IsInColorblindMode);
		}

		// Token: 0x060032E2 RID: 13026 RVA: 0x000F143F File Offset: 0x000EF63F
		protected override void SetExpanded(AnimatedCard.ExpansionLevel expansionLevel)
		{
			base.SetExpanded(expansionLevel);
			this._screen.OffsetNeighbouringCardsToButton(this, expansionLevel);
		}

		// Token: 0x060032E3 RID: 13027 RVA: 0x000F1455 File Offset: 0x000EF655
		private void SetCardVisible(MapButtonCard card)
		{
			card.SetVisible(true);
		}

		// Token: 0x060032E4 RID: 13028 RVA: 0x000F145E File Offset: 0x000EF65E
		private void SetCardInvisible(MapButtonCard card)
		{
			if (card != null)
			{
				card.SetVisible(false);
			}
		}

		// Token: 0x060032E5 RID: 13029 RVA: 0x000F1470 File Offset: 0x000EF670
		private void BaseInitializeCard(MapButtonCard card)
		{
			List<IThemeComponent> additionalThemeComponents = new List<IThemeComponent>();
			card.GetComponentsInChildren<IThemeComponent>(true, additionalThemeComponents);
			this.MapSelectScreen.RegisterAdditionalThemeComponents(additionalThemeComponents);
			List<VariableDeviceSelectable> additionalButtons = new List<VariableDeviceSelectable>();
			card.GetComponentsInChildren<VariableDeviceSelectable>(true, additionalButtons);
			this.MapSelectScreen.RegisterAdditionalButtons(additionalButtons);
			List<LocalizedTextUI> additionalLocalizedTexts = new List<LocalizedTextUI>();
			card.GetComponentsInChildren<LocalizedTextUI>(true, additionalLocalizedTexts);
			this.MapSelectScreen.RegisterAdditionalLocalizedTextChildren(additionalLocalizedTexts);
		}

		// Token: 0x060032E6 RID: 13030 RVA: 0x000F14CC File Offset: 0x000EF6CC
		public void ApplyTheme()
		{
			Color colorfulButtonColor = this._mapDefinition.themes[this._themeDatabase.IsInColorblindMode ? 3 : 0].GetColor(ThemedMaterialType.PrimaryMenu, "_Color");
			this.ColorfulSelect.themeColorPreviewImage.color = colorfulButtonColor;
			foreach (IThemeComponent themeComponent in this._themedMapButtonComponents)
			{
				themeComponent.ApplyTheme(this._mapDefinition.themes[(int)this._themeDatabase.ThemePreference]);
			}
			this._mainCard.PreviewImage.sprite = this._mapDefinition.themePreviewSprites[(int)this._themeDatabase.ThemePreference];
			this.SetupNavigationToThemeButtons(this._themeDatabase.ThemePreference);
			this._previousTheme = this._mapDefinition.themes[(int)this._themeDatabase.ThemePreference];
		}

		// Token: 0x060032E7 RID: 13031 RVA: 0x000F15C4 File Offset: 0x000EF7C4
		public void ApplyBlendedTheme(float progress)
		{
			Color colorfulButtonColor = this._mapDefinition.themes[this._themeDatabase.IsInColorblindMode ? 3 : 0].GetColor(ThemedMaterialType.PrimaryMenu, "_Color");
			this.ColorfulSelect.themeColorPreviewImage.color = colorfulButtonColor;
			this._mainCard.PreviewImage.sprite = this._mapDefinition.themePreviewSprites[(int)this._themeDatabase.ThemePreference];
			ITheme newTheme = this._mapDefinition.themes[(int)this._themeDatabase.ThemePreference];
			if (this._lastThemeBlendedFrom != this._previousTheme || this._lastThemeBlendedTo != newTheme)
			{
				this._lastThemeBlendedFrom = this._previousTheme;
				this._lastThemeBlendedTo = newTheme;
				this._dynamicMapButtonComponents.Clear();
				using (List<IThemeComponent>.Enumerator enumerator = this._themedMapButtonComponents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IThemeComponent themedMapButtonComponent = enumerator.Current;
						if (themedMapButtonComponent.ApplyBlendedTheme(this._previousTheme, newTheme, progress) == ThemeBlendingResult.ContinueBlending)
						{
							this._dynamicMapButtonComponents.Add(themedMapButtonComponent);
						}
					}
					goto IL_136;
				}
			}
			foreach (IThemeComponent themeComponent in this._dynamicMapButtonComponents)
			{
				themeComponent.ApplyBlendedTheme(this._previousTheme, newTheme, progress);
			}
			IL_136:
			if (progress >= 1f)
			{
				this._previousTheme = newTheme;
			}
			this.SetupNavigationToThemeButtons(this._themeDatabase.ThemePreference);
		}

		// Token: 0x060032E8 RID: 13032 RVA: 0x000F1744 File Offset: 0x000EF944
		public void SetLocked(StringId headerId, StringId descriptionId)
		{
			this._currentCard = MapButton.Card.Locked;
			this.SetNextCard();
			if (headerId == StringId.None)
			{
				this.LockedCard.DescriptionHeader.gameObject.SetActive(false);
			}
			else
			{
				this.LockedCard.DescriptionHeader.gameObject.SetActive(true);
				this.LockedCard.DescriptionHeader.SetStringId(this._scope, headerId);
			}
			this.LockedCard.Description.SetStringId(this._scope, descriptionId);
		}

		// Token: 0x060032E9 RID: 13033 RVA: 0x000F17BF File Offset: 0x000EF9BF
		public void HackSetUnlocked()
		{
			this._currentCard = MapButton.Card.Main;
			this.SetNextCard();
		}

		// Token: 0x060032EA RID: 13034 RVA: 0x000F17D0 File Offset: 0x000EF9D0
		public void SetUnlocked()
		{
			this._currentCard = MapButton.Card.Main;
			this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.UnlockMap, 0.5f, -1f, true, null));
			this.LockedCard.PlayUnlockAnimation(null);
		}

		// Token: 0x060032EB RID: 13035 RVA: 0x000F181E File Offset: 0x000EFA1E
		public void FlipCard()
		{
			base.onAnimationMidFlip += this.OnTabSelectMidFlip;
			base.TweenToNextCard();
		}

		// Token: 0x060032EC RID: 13036 RVA: 0x000F1839 File Offset: 0x000EFA39
		public bool IsSelected()
		{
			return this._screen.ButtonCount != 0 && this._screen.CurrentlySelectedMapButton == this;
		}

		// Token: 0x060032ED RID: 13037 RVA: 0x000F185C File Offset: 0x000EFA5C
		private LeaderboardId GetLeaderboardIdForMapButton()
		{
			int selectedOptionIndex = this.LeaderboardCard.RecurringLeaderboardSelector.SelectedOptionIndex;
			if (this.MapChallenge != null)
			{
				switch (this.MapChallenge.type)
				{
				case MapChallenge.ChallengeType.Daily:
				{
					GameObject selectedOption = this.LeaderboardCard.RecurringLeaderboardSelector.options[selectedOptionIndex];
					DayOfWeek dayOfWeek;
					if (Enum.TryParse<DayOfWeek>(selectedOption.name, out dayOfWeek))
					{
						return new DailyLeaderboardId(ChallengeSystem.ToTimestamp(ChallengeSystem.GetStartOfLastOccurence(dayOfWeek)));
					}
					Diagnostics.FailAssert("Invalid daily challenge leaderboard option: {0}", new object[]
					{
						selectedOption.name
					});
					return null;
				}
				case MapChallenge.ChallengeType.Weekly:
				{
					ChallengeSystem.LeaderboardWeek currentWeek = ChallengeSystem.GetLeaderboardWeek(this.MapChallenge.TimeStart);
					if (selectedOptionIndex == 0 || selectedOptionIndex == 1)
					{
						return new WeeklyLeaderboardId(ChallengeSystem.ToTimestamp(ChallengeSystem.GetStartOfLastOccurence((selectedOptionIndex == 1) ? currentWeek : currentWeek.Other())));
					}
					Diagnostics.FailAssert("Invalid weekly challenge leaderboard option index: {0}", new object[]
					{
						selectedOptionIndex
					});
					return null;
				}
				case MapChallenge.ChallengeType.City:
					return new CityLeaderboardId(this.MapDefinition.CityNameEnum, CityGameMode.CityChallenge, this.SelectedChallengeIndex);
				}
				Diagnostics.FailAssert("Invalid challenge type for leaderboard: {0}", new object[]
				{
					this.MapChallenge.type
				});
				return null;
			}
			if (selectedOptionIndex >= 0 && selectedOptionIndex < this.LeaderboardCard.RecurringLeaderboardSelector.options.Length)
			{
				return new CityLeaderboardId(this._mapDefinition.CityNameEnum, LeaderboardSelectorInfo.GetGameModeForIndex(selectedOptionIndex), (selectedOptionIndex >= 2) ? (selectedOptionIndex - 2) : -1);
			}
			return null;
		}

		// Token: 0x060032EE RID: 13038 RVA: 0x000F19C8 File Offset: 0x000EFBC8
		private void OnModeSelected()
		{
			this._mainCard.UpdateModeStrings(this.GetCurrentSelectedGameMode());
			if (this.GetCurrentSelectedGameMode() != GameMode.Normal)
			{
				this.DeselectCityChallenge();
			}
			GameMode gameMode = this.GetCurrentSelectedGameMode();
			int score = this.MapSelectScreen.GetBestScoreForCityLeaderboard(this._mapDefinition.cityName, gameMode);
			this.SetBestScoreText(this._scope, score, this.MainCard.BestScoreText);
			this.SetBestScoreTextOnModeCard(this._scope, score);
			if (this.GetCurrentSelectedGameMode() == GameMode.Normal)
			{
				AnimatedCard.SetNavigationOnUp(this.ModeSelectCard.NormalButton, this.MapSelectScreen.backButton);
				return;
			}
			AnimatedCard.SetNavigationOnUp(this.ModeSelectCard.NormalButton, this.ModeSelectCard.InfoButton);
		}

		// Token: 0x060032EF RID: 13039 RVA: 0x000F1A78 File Offset: 0x000EFC78
		private void ResetModeSelection()
		{
			if (this.ModeSelectCard != null)
			{
				this.ModeSelectCard.ResetToNormal();
				this._mainCard.UpdateModeStrings(this.GetCurrentSelectedGameMode());
			}
			else
			{
				this._mainCard.UpdateModeStrings(GameMode.Normal);
				this._player.SetSelectedGameMode(this._mapDefinition.mapName, GameMode.Normal);
			}
			int score = this.MapSelectScreen.GetBestScoreForCityLeaderboard(this._mapDefinition.cityName, GameMode.Normal);
			this.SetBestScoreText(this._scope, score, this.MainCard.BestScoreText);
			this.SetBestScoreTextOnModeCard(this._scope, score);
		}

		// Token: 0x060032F0 RID: 13040 RVA: 0x000F1B14 File Offset: 0x000EFD14
		public void LockUnlockPressed()
		{
			if (FeatureToggle.IsFeatureDisabled(Feature.DebugMapUnlockButton))
			{
				return;
			}
			if (this._currentCard == MapButton.Card.Locked)
			{
				this.ToggleLockInAchievements(true);
				this.SetUnlocked();
				StorableUtilities.StoreJsonStorable(this._player.ExtendedUserProfile);
				return;
			}
			this.ToggleLockInAchievements(false);
			if (this.MapDefinition.HowToUnlockDescription == StringId.None)
			{
				this.SetLocked(StringId.MapUnlock_ToUnlock, StringId.MapUnlock_ToUnlock);
				this._leaderboardTabButton.Hide();
				this._challengeTabButton.Hide();
				this._modeSelectTabButton.Hide();
				this._mainTabButton.Hide();
			}
			else
			{
				this.SetLocked(StringId.MapUnlock_ToUnlock, this.MapDefinition.HowToUnlockDescription);
				this._leaderboardTabButton.Hide();
				this._challengeTabButton.Hide();
				this._modeSelectTabButton.Hide();
				this._mainTabButton.Hide();
			}
			StorableUtilities.StoreJsonStorable(this._player.ExtendedUserProfile);
		}

		// Token: 0x060032F1 RID: 13041 RVA: 0x000F1BF8 File Offset: 0x000EFDF8
		private void ToggleLockInAchievements(bool isNowLocked)
		{
			if (FeatureToggle.IsFeatureDisabled(Feature.DebugMapUnlockButton))
			{
				return;
			}
			AchievementDatabase achievements = this._scope.Get<AchievementDatabase>();
			if (!isNowLocked)
			{
				MotorwaysCityStatistics cityStats = this._player.MotorwaysUserProfile.GetCityStatisticsForCity(this.MapDefinition.cityName, GameMode.Normal, false);
				if (cityStats != null)
				{
					cityStats.MaxTrips = 0;
				}
			}
			foreach (AchievementData achievementData in achievements.allAchievementData)
			{
				MotorwaysAchievementData motorwaysAchievement = achievementData as MotorwaysAchievementData;
				if (motorwaysAchievement != null)
				{
					bool found = false;
					using (List<AchievementData>.Enumerator enumerator2 = this.MapDefinition._achievementsThatUnlockMap.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current == motorwaysAchievement)
							{
								found = true;
								break;
							}
						}
					}
					if (!found)
					{
						continue;
					}
				}
				AchievementDefinition achievement = achievements[achievementData.GetId()];
				bool isUnlocked = this._player.IsAchievementCompleted(achievement);
				if (isNowLocked != isUnlocked)
				{
					if (isNowLocked)
					{
						this._player.CompleteAchievement(achievement, true);
					}
					else
					{
						foreach (Achievement playerAchievement in ((List<Achievement>)MapButton.GetInstanceField(typeof(LegacyBaseUserProfile), this._player.UserProfile, "_achievements")))
						{
							if (playerAchievement.Id == achievement.Id)
							{
								this._player.MotorwaysUserProfile.RemoveAchievement(playerAchievement.Definition);
								MotorwaysAchievementDefinition motorwaysAchievementDefinition = playerAchievement.Definition as MotorwaysAchievementDefinition;
								if (motorwaysAchievementDefinition != null)
								{
									MotorwaysCityStatistics cityStats2 = this._player.MotorwaysUserProfile.GetCityStatisticsForCity(motorwaysAchievementDefinition.CityName, GameMode.Normal, false);
									if (cityStats2 != null)
									{
										cityStats2.MaxTrips = 0;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060032F2 RID: 13042 RVA: 0x000F1E10 File Offset: 0x000F0010
		public static object GetInstanceField(Type type, object instance, string fieldName)
		{
			BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			return type.GetField(fieldName, bindFlags).GetValue(instance);
		}

		// Token: 0x060032F5 RID: 13045 RVA: 0x000F1E6C File Offset: 0x000F006C
		[CompilerGenerated]
		internal static void <SetupButtonNavigation>g__SetTabButtonRightNavigation|138_0(bool goToNextMap, Selectable selectOnRight = null, ref MapButton.<>c__DisplayClass138_0 A_2)
		{
			AnimatedCard.SetNavigationOnRight(A_2.mainButton, goToNextMap ? A_2.nextMainButton : selectOnRight);
			AnimatedCard.SetNavigationOnRight(A_2.leaderboardButton, goToNextMap ? A_2.nextLeaderboardButton : selectOnRight);
			AnimatedCard.SetNavigationOnRight(A_2.challengeButton, goToNextMap ? A_2.nextChallengeButton : selectOnRight);
			AnimatedCard.SetNavigationOnRight(A_2.modeSelectButton, goToNextMap ? A_2.nextModeSelectButton : selectOnRight);
		}

		// Token: 0x04002B52 RID: 11090
		[SerializeField]
		private RectTransform mainCardParent;

		// Token: 0x04002B53 RID: 11091
		[SerializeField]
		private RectTransform leaderboardCardParent;

		// Token: 0x04002B54 RID: 11092
		[FormerlySerializedAs("lockCardParent")]
		[SerializeField]
		private RectTransform _lockCardParent;

		// Token: 0x04002B55 RID: 11093
		[SerializeField]
		private RectTransform _challengeCardParent;

		// Token: 0x04002B56 RID: 11094
		[SerializeField]
		private RectTransform _modeSelectCardParent;

		// Token: 0x04002B57 RID: 11095
		[SerializeField]
		private MapButtonMainCard cityCardPrefab;

		// Token: 0x04002B58 RID: 11096
		[SerializeField]
		private MapButtonLeaderboardCard _leaderboardCardPrefab;

		// Token: 0x04002B59 RID: 11097
		[SerializeField]
		private MapButtonLockedCard _lockedCardPrefab;

		// Token: 0x04002B5A RID: 11098
		[SerializeField]
		private MapButtonMainCard _mainChallengeCardPrefab;

		// Token: 0x04002B5B RID: 11099
		[SerializeField]
		private MapButtonChallengeCard _challengeCardPrefab;

		// Token: 0x04002B5C RID: 11100
		[SerializeField]
		private MapButtonModeSelectCard _modeSelectCardPrefab;

		// Token: 0x04002B5D RID: 11101
		[SerializeField]
		private MapButtonTab _mainTabButton;

		// Token: 0x04002B5E RID: 11102
		[SerializeField]
		private MapButtonTab _leaderboardTabButton;

		// Token: 0x04002B5F RID: 11103
		[SerializeField]
		private MapButtonTab _challengeTabButton;

		// Token: 0x04002B60 RID: 11104
		[SerializeField]
		private MapButtonTab _modeSelectTabButton;

		// Token: 0x04002B61 RID: 11105
		[SerializeField]
		private TouchButton _lockUnlockButton;

		// Token: 0x04002B62 RID: 11106
		private VisualConstantsData _visualConstants;

		// Token: 0x04002B63 RID: 11107
		private readonly List<IThemeComponent> _themedMapButtonComponents = new List<IThemeComponent>();

		// Token: 0x04002B64 RID: 11108
		private readonly List<IThemeComponent> _dynamicMapButtonComponents = new List<IThemeComponent>();

		// Token: 0x04002B65 RID: 11109
		private ITheme _lastThemeBlendedFrom;

		// Token: 0x04002B66 RID: 11110
		private ITheme _lastThemeBlendedTo;

		// Token: 0x04002B6C RID: 11116
		private MapButtonMainCard _mainCard;

		// Token: 0x04002B72 RID: 11122
		public bool _leaderboardShowsSelectedChallenge;

		// Token: 0x04002B74 RID: 11124
		private MapSelectScreen _screen;

		// Token: 0x04002B75 RID: 11125
		private MapDefinition _mapDefinition;

		// Token: 0x04002B76 RID: 11126
		private MapChallenge _mapChallenge;

		// Token: 0x04002B77 RID: 11127
		private StringKey _challengeTimeLeftKey;

		// Token: 0x04002B78 RID: 11128
		private StringId _previousChallengeTimerKey;

		// Token: 0x04002B79 RID: 11129
		private int _previousChallengeTimerCount;

		// Token: 0x04002B7A RID: 11130
		private bool _isUpdatingChallenge;

		// Token: 0x04002B7B RID: 11131
		private IScope _scope;

		// Token: 0x04002B7C RID: 11132
		private ActivePlayer _player;

		// Token: 0x04002B7D RID: 11133
		private MotorwaysThemeDatabase _themeDatabase;

		// Token: 0x04002B7F RID: 11135
		private MapButton.Card _currentCard;

		// Token: 0x04002B80 RID: 11136
		private ITheme _previousTheme;

		// Token: 0x04002B81 RID: 11137
		private MotorwaysThemePreference _selectedTheme;

		// Token: 0x04002B82 RID: 11138
		private Selectable _playButton;

		// Token: 0x04002B83 RID: 11139
		private static readonly ProfilerMarker Profiler_ApplyBlendedTheme = new ProfilerMarker(ProfilerCategory.Scripts, "MapButton.ApplyBlendedTheme()");

		// Token: 0x0200072C RID: 1836
		public enum MapButtonType
		{
			// Token: 0x04002B85 RID: 11141
			City,
			// Token: 0x04002B86 RID: 11142
			DailyChallenge,
			// Token: 0x04002B87 RID: 11143
			WeeklyChallenge
		}

		// Token: 0x0200072D RID: 1837
		public enum Card
		{
			// Token: 0x04002B89 RID: 11145
			Main,
			// Token: 0x04002B8A RID: 11146
			Leaderboard,
			// Token: 0x04002B8B RID: 11147
			Locked,
			// Token: 0x04002B8C RID: 11148
			Challenge,
			// Token: 0x04002B8D RID: 11149
			Mode
		}
	}
}
