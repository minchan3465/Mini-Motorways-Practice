using System;
using System.Collections.Generic;
using Factory;
using Motorways.Themes;
using Motorways.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x0200073F RID: 1855
	public class ResumeMapButton : AnimatedCard
	{
		// Token: 0x1700089C RID: 2204
		// (get) Token: 0x060033D7 RID: 13271 RVA: 0x000F50C8 File Offset: 0x000F32C8
		public string GameID
		{
			get
			{
				return this._gameId;
			}
		}

		// Token: 0x060033D8 RID: 13272 RVA: 0x000F50D0 File Offset: 0x000F32D0
		public void OnClicked()
		{
			this._screen.SelectGame(this);
		}

		// Token: 0x060033D9 RID: 13273 RVA: 0x000F50DE File Offset: 0x000F32DE
		public void ScrollToMe()
		{
			this._screen.ScrollToButton(this, false);
		}

		// Token: 0x060033DA RID: 13274 RVA: 0x000F50ED File Offset: 0x000F32ED
		public void OnDelete()
		{
			this._screen.DeleteGame(this);
		}

		// Token: 0x060033DB RID: 13275 RVA: 0x000F50FC File Offset: 0x000F32FC
		public void Initialize(ResumeGameScreen screen, string savedGameId, MotorwaysGameJournalSave savedGame, MapDefinition cityDefinition, IScope scope)
		{
			this._screen = screen;
			this._gameId = savedGameId;
			this._mapDefinition = cityDefinition;
			this._scope = scope;
			this._saveGame = savedGame;
			this.header.SetStringId(this._scope, cityDefinition.mapName);
			StringId descriptionId = StringId.None;
			switch (savedGame.ChallengeType)
			{
			case MapChallenge.ChallengeType.Daily:
				descriptionId = StringId.DailyChallenge;
				break;
			case MapChallenge.ChallengeType.Weekly:
				descriptionId = StringId.WeeklyChallenge;
				break;
			case MapChallenge.ChallengeType.Mystery:
				descriptionId = StringId.MysteryUpgradeName;
				break;
			case MapChallenge.ChallengeType.City:
				if (Diagnostics.Verify(savedGame.ChallengeIndex >= 0, "Somehow marked this save as a City challenge but doesn't have a challenge index?"))
				{
					Diagnostics.Verify(Enum.TryParse<StringId>(cityDefinition.cityChallenges[savedGame.ChallengeIndex].titleStringId, out descriptionId));
				}
				break;
			}
			if (descriptionId != StringId.None)
			{
				this.description.SetStringId(this._scope, descriptionId);
			}
			else
			{
				this.description.TextField.text = "";
			}
			this._player = this._scope.Get<ActivePlayer>();
			Locale currentLocale = this._scope.Get<LocaleDatabase>().CurrentLocale;
			this.date.text = currentLocale.FormatDateTime(savedGame.UtcTimestamp.ToLocalTime(), false);
			FontDatabase fontDatabase = this._scope.Get<FontDatabase>();
			this.date.font = fontDatabase.GetFont(currentLocale.Charset).FontAsset;
			this.device.text = DeviceStringLookup.GetDeviceDisplayStringFromModel(savedGame.DeviceModel);
			this._themeDatabase = this._scope.Get<MotorwaysThemeDatabase>();
			this._visualConstantsData = this._scope.Get<VisualConstantsData>();
			this._themedMapScreenComponents = new List<ThemedComponent>();
			base.GetComponentsInChildren<ThemedComponent>(true, this._themedMapScreenComponents);
			this.previewImage.sprite = this._mapDefinition.themePreviewSprites[(int)this._themeDatabase.ThemePreference];
			this.deleteButton.gameObject.SetActive(savedGame.CanDelete);
		}

		// Token: 0x060033DC RID: 13276 RVA: 0x000F52DC File Offset: 0x000F34DC
		public void ApplyTheme()
		{
			foreach (ThemedComponent themedComponent in this._themedMapScreenComponents)
			{
				themedComponent.ApplyTheme(this._mapDefinition.themes[(int)this._themeDatabase.ThemePreference]);
			}
			this.previewImage.sprite = this._mapDefinition.themePreviewSprites[(int)this._themeDatabase.ThemePreference];
			this.UpdateModeStrings();
		}

		// Token: 0x060033DD RID: 13277 RVA: 0x000F536C File Offset: 0x000F356C
		private void UpdateModeStrings()
		{
			if (this._saveGame.ChallengeType == MapChallenge.ChallengeType.None)
			{
				MotorwaysStringKey modeStringKey = this._scope.Get<MotorwaysStringKey>();
				GameMode gameMode = this._saveGame.Mode;
				this._modeNameText.gameObject.SetActive(true);
				switch (gameMode)
				{
				case GameMode.Normal:
					modeStringKey.InitWithStringId(StringId.Normal);
					this._modeNameTextThemedComponent.ApplyTheme(this._themeDatabase.GetTheme());
					break;
				case GameMode.Tutorial:
				case GameMode.Background:
					break;
				case GameMode.Endless:
					modeStringKey.InitWithStringId(StringId.Endless);
					this._modeNameTextThemedComponent.enabled = false;
					this._modeNameText.TextField.color = this._visualConstantsData.EndlessTabButtonColor;
					break;
				case GameMode.Expert:
					modeStringKey.InitWithStringId(StringId.Expert);
					this._modeNameTextThemedComponent.enabled = false;
					this._modeNameText.TextField.color = this._visualConstantsData.ExpertTabButtonColor;
					break;
				default:
					if (gameMode == GameMode.Creative)
					{
						modeStringKey.InitWithStringId(StringId.Creative);
						this._modeNameTextThemedComponent.enabled = false;
						this._modeNameText.TextField.color = this._visualConstantsData.CreativeTabButtonColor;
					}
					break;
				}
				this._modeNameText.LocString = StandaloneLocString.CreateString(this._scope, modeStringKey);
				return;
			}
			this._modeNameText.gameObject.SetActive(false);
		}

		// Token: 0x04002C46 RID: 11334
		public LocalizedTextUI header;

		// Token: 0x04002C47 RID: 11335
		public LocalizedTextUI description;

		// Token: 0x04002C48 RID: 11336
		public TextMeshProUGUI date;

		// Token: 0x04002C49 RID: 11337
		public TextMeshProUGUI device;

		// Token: 0x04002C4A RID: 11338
		public TouchButton playTouchButton;

		// Token: 0x04002C4B RID: 11339
		private ResumeGameScreen _screen;

		// Token: 0x04002C4C RID: 11340
		private string _gameId;

		// Token: 0x04002C4D RID: 11341
		private MapDefinition _mapDefinition;

		// Token: 0x04002C4E RID: 11342
		public TouchButton deleteButton;

		// Token: 0x04002C4F RID: 11343
		public Image previewImage;

		// Token: 0x04002C50 RID: 11344
		protected List<ThemedComponent> _themedMapScreenComponents;

		// Token: 0x04002C51 RID: 11345
		private MotorwaysThemeDatabase _themeDatabase;

		// Token: 0x04002C52 RID: 11346
		private IScope _scope;

		// Token: 0x04002C53 RID: 11347
		[SerializeField]
		private LocalizedTextUI _modeNameText;

		// Token: 0x04002C54 RID: 11348
		[SerializeField]
		private ThemedComponent _modeNameTextThemedComponent;

		// Token: 0x04002C55 RID: 11349
		private VisualConstantsData _visualConstantsData;

		// Token: 0x04002C56 RID: 11350
		private ActivePlayer _player;

		// Token: 0x04002C57 RID: 11351
		private MotorwaysGameJournalSave _saveGame;
	}
}
