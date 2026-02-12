using System;
using Client;
using Factory;
using JetBrains.Annotations;
using Motorways.Themes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x02000734 RID: 1844
	public class MapButtonMainCard : MapButtonCard, ILocalized, IThemeComponent
	{
		// Token: 0x1700087C RID: 2172
		// (get) Token: 0x06003344 RID: 13124 RVA: 0x000F2A8F File Offset: 0x000F0C8F
		public LocalizedTextUI Header
		{
			get
			{
				return this.header;
			}
		}

		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x06003345 RID: 13125 RVA: 0x000F2A97 File Offset: 0x000F0C97
		public LocalizedTextUI Description
		{
			get
			{
				return this.description;
			}
		}

		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x06003346 RID: 13126 RVA: 0x000F2A9F File Offset: 0x000F0C9F
		public LocalizedTextUI BestScoreText
		{
			get
			{
				return this.bestScoreText;
			}
		}

		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x06003347 RID: 13127 RVA: 0x000F2AA7 File Offset: 0x000F0CA7
		public LocalizedTextUI CurrentModeText
		{
			get
			{
				return this.currentModeText;
			}
		}

		// Token: 0x17000880 RID: 2176
		// (get) Token: 0x06003348 RID: 13128 RVA: 0x000F2AAF File Offset: 0x000F0CAF
		public LocalizedTextUI TimeLeftText
		{
			get
			{
				return this.timeLeftText;
			}
		}

		// Token: 0x17000881 RID: 2177
		// (get) Token: 0x06003349 RID: 13129 RVA: 0x000F2AB7 File Offset: 0x000F0CB7
		public Image PreviewImage
		{
			get
			{
				return this.previewImage;
			}
		}

		// Token: 0x17000882 RID: 2178
		// (get) Token: 0x0600334A RID: 13130 RVA: 0x000F2ABF File Offset: 0x000F0CBF
		public ThemeSelectButton ColorfulSelect
		{
			get
			{
				return this.colorfulSelect;
			}
		}

		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x0600334B RID: 13131 RVA: 0x000F2AC7 File Offset: 0x000F0CC7
		public ThemeSelectButton DarkSelect
		{
			get
			{
				return this.darkSelect;
			}
		}

		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x0600334C RID: 13132 RVA: 0x000F2ACF File Offset: 0x000F0CCF
		public ThemeSelectButton MapsSelect
		{
			get
			{
				return this.mapsSelect;
			}
		}

		// Token: 0x17000885 RID: 2181
		// (get) Token: 0x0600334D RID: 13133 RVA: 0x000F2AD7 File Offset: 0x000F0CD7
		public TouchButton MoreInfoButton
		{
			get
			{
				return this.moreInfoButton;
			}
		}

		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x0600334E RID: 13134 RVA: 0x000F2ADF File Offset: 0x000F0CDF
		public TouchButton ChallengeButtonSet
		{
			get
			{
				return this._challengeButtonSet;
			}
		}

		// Token: 0x14000056 RID: 86
		// (add) Token: 0x0600334F RID: 13135 RVA: 0x000F2AE8 File Offset: 0x000F0CE8
		// (remove) Token: 0x06003350 RID: 13136 RVA: 0x000F2B20 File Offset: 0x000F0D20
		public event Action onMoreChallengeInfoPressed;

		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x06003351 RID: 13137 RVA: 0x000F2B55 File Offset: 0x000F0D55
		// (set) Token: 0x06003352 RID: 13138 RVA: 0x000F2B5D File Offset: 0x000F0D5D
		public MapButton parentMapButton { get; set; }

		// Token: 0x06003353 RID: 13139 RVA: 0x000F2B68 File Offset: 0x000F0D68
		public void Initialize(IScope scope, VisualConstantsData visualConstantsData)
		{
			this._scope = scope;
			this._visualConstantsData = visualConstantsData;
			this._player = this._scope.Get<ActivePlayer>();
			this._themeDatabase = this._scope.Get<MotorwaysThemeDatabase>();
			Theme theme = this._themeDatabase.GetTheme() as Theme;
			this._classicModeTextColor = theme.GetColor(this._classicModeTextThemeColor, "_Color");
			GameMode gameMode = this._player.GetSelectedModeForMap(this.parentMapButton.MapDefinition.mapName);
			this.UpdateModeStrings(gameMode);
			LocaleDatabase localeDatabase = scope.Get<LocaleDatabase>();
			localeDatabase.AddLocalizedObject(this);
			this.HandleLocaleChanged(localeDatabase.CurrentLocale);
		}

		// Token: 0x06003354 RID: 13140 RVA: 0x000F2C0C File Offset: 0x000F0E0C
		public void SetMapType(bool isTrainMap, bool isBoatMap)
		{
			this._mapTypeIcon.gameObject.SetActive(isTrainMap || isBoatMap);
			this._mapTypeTrainSprite.gameObject.SetActive(isTrainMap);
			this._mapTypeBoatSprite.gameObject.SetActive(isBoatMap);
			this._shadowWithMapTypeIcon.gameObject.SetActive(isTrainMap || isBoatMap);
			this._shadowWithoutMapTypeIcon.gameObject.SetActive(!isTrainMap && !isBoatMap);
		}

		// Token: 0x06003355 RID: 13141 RVA: 0x000F2C7B File Offset: 0x000F0E7B
		private void OnDestroy()
		{
			this._scope.Get<LocaleDatabase>().RemoveLocalizedObject(this);
		}

		// Token: 0x06003356 RID: 13142 RVA: 0x000F2C90 File Offset: 0x000F0E90
		public override void OnMapButtonSelected(bool newIsMapButtonSelected)
		{
			base.OnMapButtonSelected(newIsMapButtonSelected);
			bool moreInfoVisible = newIsMapButtonSelected && this.parentMapButton.CurrentCard == MapButton.Card.Main;
			this.ShowHideMoreInfoButton(moreInfoVisible);
		}

		// Token: 0x06003357 RID: 13143 RVA: 0x000F2CC0 File Offset: 0x000F0EC0
		public override void SetSelected(bool isSelected)
		{
			base.SetSelected(isSelected);
			bool moreInfoVisible = this.parentMapButton.IsSelected() && isSelected;
			this.ShowHideMoreInfoButton(moreInfoVisible);
			if (this._player != null)
			{
				GameMode gameMode = this._player.GetSelectedModeForMap(this.parentMapButton.MapDefinition.mapName);
				this.UpdateModeStrings(gameMode);
			}
		}

		// Token: 0x06003358 RID: 13144 RVA: 0x000F2D14 File Offset: 0x000F0F14
		private void ShowHideMoreInfoButton(bool shouldShow)
		{
			this._showMoreInfoButton = shouldShow;
		}

		// Token: 0x06003359 RID: 13145 RVA: 0x000F2D1D File Offset: 0x000F0F1D
		private void Update()
		{
			if (this._moreInfoButtonCanvas != null)
			{
				this._moreInfoButtonCanvas.Alpha = Mathf.Clamp01(this._moreInfoButtonCanvas.Alpha + (this._showMoreInfoButton ? 0.2f : -0.2f));
			}
		}

		// Token: 0x0600335A RID: 13146 RVA: 0x000F2D5D File Offset: 0x000F0F5D
		public void ScrollToMe()
		{
			this.parentMapButton.ScrollToMe();
		}

		// Token: 0x0600335B RID: 13147 RVA: 0x000F2D6A File Offset: 0x000F0F6A
		public void OnChallengeButtonsPressed()
		{
			this.parentMapButton.ShowChallengeInfo();
		}

		// Token: 0x0600335C RID: 13148 RVA: 0x000F2D78 File Offset: 0x000F0F78
		public void SetChallengeIcons(ChallengeData[] challenges, ChallengeDatabase challengeDatabase)
		{
			for (int iconIndex = 0; iconIndex < this.challengeIcons.Length; iconIndex++)
			{
				if (iconIndex < challenges.Length)
				{
					ChallengeData challenge = challenges[iconIndex];
					bool isWildcardChallenge = challengeDatabase.IsChallengeWildcard(challenge);
					this.challengeIcons[iconIndex].SetChallengeIcons(challenge.icon, isWildcardChallenge, challenge.subIcon, challenge.subIconBackground);
					this.challengeIcons[iconIndex].gameObject.SetActive(true);
				}
				else
				{
					this.challengeIcons[iconIndex].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600335D RID: 13149 RVA: 0x000F2DF2 File Offset: 0x000F0FF2
		[UsedImplicitly]
		public void MoreInfoSelected()
		{
			if (Diagnostics.Verify(this.parentMapButton != null))
			{
				this.parentMapButton.ScrollToMe();
			}
		}

		// Token: 0x0600335E RID: 13150 RVA: 0x000F2E12 File Offset: 0x000F1012
		[UsedImplicitly]
		public void MoreChallengeInfoPressed()
		{
			Action action = this.onMoreChallengeInfoPressed;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x0600335F RID: 13151 RVA: 0x000F2E24 File Offset: 0x000F1024
		public void UpdateModeStrings(GameMode gameMode)
		{
			if (this.currentModeText == null)
			{
				return;
			}
			MotorwaysStringKey modeStringKey = this._scope.Get<MotorwaysStringKey>();
			switch (gameMode)
			{
			case GameMode.Normal:
				this.currentModeText.TextField.color = this._classicModeTextColor;
				modeStringKey.InitWithStringId(StringId.Normal);
				this.BestScoreText.gameObject.SetActive(true);
				this._modeTextHorizontalLayoutGroup.padding.bottom = 0;
				break;
			case GameMode.Tutorial:
			case GameMode.Background:
				break;
			case GameMode.Endless:
				modeStringKey.InitWithStringId(StringId.Endless);
				this.currentModeText.TextField.color = this._visualConstantsData.EndlessTabButtonColor;
				this.BestScoreText.gameObject.SetActive(false);
				this._modeTextHorizontalLayoutGroup.padding.bottom = 30;
				break;
			case GameMode.Expert:
				modeStringKey.InitWithStringId(StringId.Expert);
				this.currentModeText.TextField.color = this._visualConstantsData.ExpertTabButtonColor;
				this.BestScoreText.gameObject.SetActive(true);
				this._modeTextHorizontalLayoutGroup.padding.bottom = 0;
				break;
			default:
				if (gameMode == GameMode.Creative)
				{
					modeStringKey.InitWithStringId(StringId.Creative);
					this.currentModeText.TextField.color = this._visualConstantsData.CreativeTabButtonColor;
					this.BestScoreText.gameObject.SetActive(false);
					this._modeTextHorizontalLayoutGroup.padding.bottom = 30;
				}
				break;
			}
			this.currentModeText.LocString = StandaloneLocString.CreateString(this._scope, modeStringKey);
		}

		// Token: 0x06003360 RID: 13152 RVA: 0x000F2FB4 File Offset: 0x000F11B4
		public void HandleLocaleChanged(Locale newLocale)
		{
			if (this._currentModeText != null && this._currentModeText.Length == 2)
			{
				bool rightToLeft = newLocale.TextDirection == TextDirection.LeftToRight;
				int modeSiblingIndex = rightToLeft ? 0 : 1;
				this._currentModeText[0].transform.SetSiblingIndex(modeSiblingIndex);
				this._currentModeText[0].TextField.horizontalAlignment = (rightToLeft ? HorizontalAlignmentOptions.Right : HorizontalAlignmentOptions.Left);
				this._currentModeText[1].TextField.horizontalAlignment = (rightToLeft ? HorizontalAlignmentOptions.Left : HorizontalAlignmentOptions.Right);
			}
		}

		// Token: 0x06003361 RID: 13153 RVA: 0x000022F5 File Offset: 0x000004F5
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x06003362 RID: 13154 RVA: 0x000F302C File Offset: 0x000F122C
		public void ApplyTheme(ITheme newTheme)
		{
			Theme theme = (Theme)newTheme;
			this._classicModeTextColor = theme.GetColor(this._classicModeTextThemeColor, "_Color");
			if (this._player != null)
			{
				GameMode gameMode = this._player.GetSelectedModeForMap(this.parentMapButton.MapDefinition.mapName);
				this.UpdateModeStrings(gameMode);
			}
		}

		// Token: 0x06003363 RID: 13155 RVA: 0x000F3084 File Offset: 0x000F1284
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			Color oldColor = (oldTheme as Theme).GetColor(this._classicModeTextThemeColor, "_Color");
			Color newColor = (newTheme as Theme).GetColor(this._classicModeTextThemeColor, "_Color");
			this._classicModeTextColor = Color.Lerp(oldColor, newColor, progress);
			if (this._player != null)
			{
				GameMode gameMode = this._player.GetSelectedModeForMap(this.parentMapButton.MapDefinition.mapName);
				this.UpdateModeStrings(gameMode);
			}
			if (!(oldColor == newColor))
			{
				return ThemeBlendingResult.ContinueBlending;
			}
			return ThemeBlendingResult.StopBlending;
		}

		// Token: 0x06003364 RID: 13156 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x04002BB9 RID: 11193
		[SerializeField]
		private LocalizedTextUI header;

		// Token: 0x04002BBA RID: 11194
		[SerializeField]
		private LocalizedTextUI description;

		// Token: 0x04002BBB RID: 11195
		[SerializeField]
		private LocalizedTextUI bestScoreText;

		// Token: 0x04002BBC RID: 11196
		[SerializeField]
		private LocalizedTextUI currentModeText;

		// Token: 0x04002BBD RID: 11197
		[SerializeField]
		private LocalizedTextUI timeLeftText;

		// Token: 0x04002BBE RID: 11198
		[SerializeField]
		private Image previewImage;

		// Token: 0x04002BBF RID: 11199
		[SerializeField]
		private ThemeSelectButton colorfulSelect;

		// Token: 0x04002BC0 RID: 11200
		[SerializeField]
		private ThemeSelectButton darkSelect;

		// Token: 0x04002BC1 RID: 11201
		[SerializeField]
		private ThemeSelectButton mapsSelect;

		// Token: 0x04002BC2 RID: 11202
		[SerializeField]
		private TouchButton moreInfoButton;

		// Token: 0x04002BC3 RID: 11203
		[SerializeField]
		private ChallengeIcon[] challengeIcons;

		// Token: 0x04002BC4 RID: 11204
		[SerializeField]
		private TouchButton _challengeButtonSet;

		// Token: 0x04002BC5 RID: 11205
		[SerializeField]
		private DelegateCanvasGroup _moreInfoButtonCanvas;

		// Token: 0x04002BC6 RID: 11206
		[SerializeField]
		private HorizontalLayoutGroup _modeTextHorizontalLayoutGroup;

		// Token: 0x04002BC7 RID: 11207
		[SerializeField]
		private LocalizedTextUI[] _currentModeText;

		// Token: 0x04002BC8 RID: 11208
		[Header("Map Type Icon Settings")]
		[SerializeField]
		private Image _mapTypeIcon;

		// Token: 0x04002BC9 RID: 11209
		[SerializeField]
		private Image _mapTypeTrainSprite;

		// Token: 0x04002BCA RID: 11210
		[SerializeField]
		private Image _mapTypeBoatSprite;

		// Token: 0x04002BCB RID: 11211
		[SerializeField]
		private Image _shadowWithMapTypeIcon;

		// Token: 0x04002BCC RID: 11212
		[SerializeField]
		private Image _shadowWithoutMapTypeIcon;

		// Token: 0x04002BCD RID: 11213
		[SerializeField]
		[Space(20f)]
		private ThemedMaterialType _classicModeTextThemeColor = ThemedMaterialType.Dark;

		// Token: 0x04002BCE RID: 11214
		private bool _showMoreInfoButton;

		// Token: 0x04002BD0 RID: 11216
		private IScope _scope;

		// Token: 0x04002BD1 RID: 11217
		private VisualConstantsData _visualConstantsData;

		// Token: 0x04002BD2 RID: 11218
		private ActivePlayer _player;

		// Token: 0x04002BD3 RID: 11219
		private MotorwaysThemeDatabase _themeDatabase;

		// Token: 0x04002BD4 RID: 11220
		private Color _classicModeTextColor = Color.black;

		// Token: 0x04002BD5 RID: 11221
		private const int ModeTextEndlessPadding = 30;
	}
}
