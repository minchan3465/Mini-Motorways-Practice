using System;
using System.Collections.Generic;
using Factory;
using Motorways.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x0200073E RID: 1854
	public class ProfileSelectButton : AnimatedCard
	{
		// Token: 0x17000897 RID: 2199
		// (get) Token: 0x060033C3 RID: 13251 RVA: 0x000F4D91 File Offset: 0x000F2F91
		// (set) Token: 0x060033C4 RID: 13252 RVA: 0x000F4D99 File Offset: 0x000F2F99
		public bool IsCreateButton
		{
			get
			{
				return this._isCreateButton;
			}
			set
			{
				this._isCreateButton = value;
				this._createPanel.Alpha = (float)(this._isCreateButton ? 1 : 0);
				this._createPanel.SetBlocksRaycasts(this._isCreateButton);
			}
		}

		// Token: 0x17000898 RID: 2200
		// (get) Token: 0x060033C5 RID: 13253 RVA: 0x000F4DCB File Offset: 0x000F2FCB
		public Player Player
		{
			get
			{
				return this._player;
			}
		}

		// Token: 0x17000899 RID: 2201
		// (get) Token: 0x060033C6 RID: 13254 RVA: 0x000F4DD3 File Offset: 0x000F2FD3
		// (set) Token: 0x060033C7 RID: 13255 RVA: 0x000F4DDB File Offset: 0x000F2FDB
		public bool IsSelectedProfile
		{
			get
			{
				return this._isSelectedProfile;
			}
			set
			{
				this._isSelectedProfile = value;
				this._currentlySelectedProfileTick.SetActive(this._isSelectedProfile);
			}
		}

		// Token: 0x1700089A RID: 2202
		// (get) Token: 0x060033C8 RID: 13256 RVA: 0x000F4DF5 File Offset: 0x000F2FF5
		// (set) Token: 0x060033C9 RID: 13257 RVA: 0x000F4DFD File Offset: 0x000F2FFD
		public int ProfileBackgroundIndex { get; private set; }

		// Token: 0x1700089B RID: 2203
		// (get) Token: 0x060033CA RID: 13258 RVA: 0x000F4E06 File Offset: 0x000F3006
		// (set) Token: 0x060033CB RID: 13259 RVA: 0x000F4E0E File Offset: 0x000F300E
		public int ProfileIconIndex { get; private set; }

		// Token: 0x060033CC RID: 13260 RVA: 0x000F4E17 File Offset: 0x000F3017
		public void Initialize(Player player)
		{
			this.IsCreateButton = false;
			base.Initialize(this._scope);
			if (player != null)
			{
				this.SetPlayer(player);
			}
		}

		// Token: 0x060033CD RID: 13261 RVA: 0x000F4E36 File Offset: 0x000F3036
		public void OnEditButtonPressed()
		{
			if (!this.IsCreateButton)
			{
				this._screen.OnEditProfile(this);
				return;
			}
			this.OnCreateButtonPressed();
		}

		// Token: 0x060033CE RID: 13262 RVA: 0x000F4E53 File Offset: 0x000F3053
		public void OnCreateButtonPressed()
		{
			this._screen.OnProfileCreateButtonPressed(this);
		}

		// Token: 0x060033CF RID: 13263 RVA: 0x000F4E61 File Offset: 0x000F3061
		public override void OnTabSelectMidFlip()
		{
			base.OnTabSelectMidFlip();
			if (this.IsCreateButton)
			{
				this.IsCreateButton = false;
				this._screen.TransitionInNewCreateButton();
			}
		}

		// Token: 0x060033D0 RID: 13264 RVA: 0x000F4E83 File Offset: 0x000F3083
		public void TurnIntoNewProfile(Player newPlayer)
		{
			if (Diagnostics.Verify(this.IsCreateButton, "We can't turn an existing profile button into a new button!"))
			{
				this.SetPlayer(newPlayer);
				base.onAnimationMidFlip += this.OnTabSelectMidFlip;
				base.TweenToNextCard();
			}
		}

		// Token: 0x060033D1 RID: 13265 RVA: 0x000F4EB7 File Offset: 0x000F30B7
		public void ScrollToMe()
		{
			this._screen.ScrollToButton(this, false);
		}

		// Token: 0x060033D2 RID: 13266 RVA: 0x000F4EC8 File Offset: 0x000F30C8
		public void SetupButtonNavigation(ProfileSelectButton previousButton, Selectable selectButton, Selectable backButton)
		{
			AnimatedCard.SetNavigationOnUp(this.editButton, backButton);
			AnimatedCard.SetNavigationOnDown(this.editButton, selectButton);
			if (previousButton != null)
			{
				AnimatedCard.SetNavigationOnLeft(this.editButton, previousButton.editButton);
				AnimatedCard.SetNavigationOnRight(previousButton.editButton, this.editButton);
			}
		}

		// Token: 0x060033D3 RID: 13267 RVA: 0x000F4F18 File Offset: 0x000F3118
		public override void OnCardConfirmed()
		{
			base.OnCardConfirmed();
			this._currentlySelectedProfileTick.SetActive(true);
		}

		// Token: 0x060033D4 RID: 13268 RVA: 0x000F4F2C File Offset: 0x000F312C
		public override void OnOtherCardConfirmed(bool pushLeft, float delay)
		{
			this._currentlySelectedProfileTick.SetActive(false);
		}

		// Token: 0x060033D5 RID: 13269 RVA: 0x000F4F3C File Offset: 0x000F313C
		public void SetPlayer(Player player)
		{
			this._player = player;
			this.ProfileBackgroundIndex = player.AvatarColorIndex;
			this.ProfileIconIndex = player.AvatarIconIndex;
			this._backgroundColor.color = this._themeDatabase.GetGlobalColor(ProfileCreationScreen.GetProfileColorEnumForIndex(this.ProfileBackgroundIndex));
			this._profileIcon.sprite = this._visualConstants.GetProfileIcon(this.ProfileIconIndex);
			Locale currentLocale = this._localeDatabase.CurrentLocale;
			int globalTotalTrips = 0;
			LegacyMotorwaysUserProfile userProfile = this._player.UserProfile as LegacyMotorwaysUserProfile;
			if (userProfile != null)
			{
				foreach (object obj in Enum.GetValues(typeof(MapDefinition.CityNames)))
				{
					MotorwaysCityStatistics stats = userProfile.GetCityStatisticsForCity(((MapDefinition.CityNames)obj).ToString(), GameMode.Normal, false);
					if (stats != null)
					{
						globalTotalTrips += stats.TotalTrips;
					}
				}
			}
			this._totalTripsText.LocString = StandaloneLocString.CreateString(this._scope, new MotorwaysStringKey(StringId.TotalTrips, new Dictionary<StringParameterId, string>
			{
				{
					StringParameterId.Num,
					this._localeDatabase.CurrentLocale.FormatNumber(globalTotalTrips)
				}
			}));
			string lastPlayedDate = currentLocale.FormatDateTime(player.LastPlayedUtcTimeOnLocalDevice.ToLocalTime(), true);
			this._lastDatePlayedText.LocString = StandaloneLocString.CreateString(this._scope, new MotorwaysStringKey(StringId.LastDatePlayed, new Dictionary<StringParameterId, string>
			{
				{
					StringParameterId.Date,
					lastPlayedDate
				}
			}));
		}

		// Token: 0x04002C35 RID: 11317
		[SerializeField]
		private Image _backgroundColor;

		// Token: 0x04002C36 RID: 11318
		[SerializeField]
		private Image _profileIcon;

		// Token: 0x04002C37 RID: 11319
		[SerializeField]
		private GameObject _currentlySelectedProfileTick;

		// Token: 0x04002C38 RID: 11320
		[SerializeField]
		private LocalizedTextUI _totalTripsText;

		// Token: 0x04002C39 RID: 11321
		[SerializeField]
		private LocalizedTextUI _lastDatePlayedText;

		// Token: 0x04002C3A RID: 11322
		public TouchButton editButton;

		// Token: 0x04002C3B RID: 11323
		[SerializeField]
		private DelegateCanvasGroup _createPanel;

		// Token: 0x04002C3C RID: 11324
		private Player _player;

		// Token: 0x04002C3D RID: 11325
		[Dependency]
		private IScope _scope;

		// Token: 0x04002C3E RID: 11326
		[Dependency]
		private ProfileSelectScreen _screen;

		// Token: 0x04002C3F RID: 11327
		[Dependency]
		private LocaleDatabase _localeDatabase;

		// Token: 0x04002C40 RID: 11328
		[Dependency]
		private MotorwaysThemeDatabase _themeDatabase;

		// Token: 0x04002C41 RID: 11329
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x04002C42 RID: 11330
		private bool _isCreateButton;

		// Token: 0x04002C43 RID: 11331
		private bool _isSelectedProfile;
	}
}
