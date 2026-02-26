using System;
using System.Collections;
using Client;
using Factory;
using Motorways.Audio;
using Motorways.Themes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x02000735 RID: 1845
	public class MapButtonModeSelectCard : MapButtonCard, ILocalized, IThemeComponent
	{
		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x06003366 RID: 13158 RVA: 0x000F311E File Offset: 0x000F131E
		// (set) Token: 0x06003367 RID: 13159 RVA: 0x000F3126 File Offset: 0x000F1326
		public GameMode GameMode { get; private set; }

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x06003368 RID: 13160 RVA: 0x000F312F File Offset: 0x000F132F
		public LocalizedTextUI BestScoreText
		{
			get
			{
				return this._bestScoreText;
			}
		}

		// Token: 0x14000057 RID: 87
		// (add) Token: 0x06003369 RID: 13161 RVA: 0x000F3138 File Offset: 0x000F1338
		// (remove) Token: 0x0600336A RID: 13162 RVA: 0x000F3170 File Offset: 0x000F1370
		public event Action onMoreModeInfoPressed;

		// Token: 0x14000058 RID: 88
		// (add) Token: 0x0600336B RID: 13163 RVA: 0x000F31A8 File Offset: 0x000F13A8
		// (remove) Token: 0x0600336C RID: 13164 RVA: 0x000F31E0 File Offset: 0x000F13E0
		public event Action onModePressed;

		// Token: 0x14000059 RID: 89
		// (add) Token: 0x0600336D RID: 13165 RVA: 0x000F3218 File Offset: 0x000F1418
		// (remove) Token: 0x0600336E RID: 13166 RVA: 0x000F3250 File Offset: 0x000F1450
		public event Action onExpertLockedPressed;

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x0600336F RID: 13167 RVA: 0x000F3285 File Offset: 0x000F1485
		public TouchButton NormalButton
		{
			get
			{
				return this._normalButton;
			}
		}

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x06003370 RID: 13168 RVA: 0x000F328D File Offset: 0x000F148D
		public TouchButton EndlessButton
		{
			get
			{
				return this._endlessButton;
			}
		}

		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x06003371 RID: 13169 RVA: 0x000F3295 File Offset: 0x000F1495
		public TouchButton ExpertButton
		{
			get
			{
				return this._expertButton;
			}
		}

		// Token: 0x1700088D RID: 2189
		// (get) Token: 0x06003372 RID: 13170 RVA: 0x000F329D File Offset: 0x000F149D
		public TouchButton CreativeButton
		{
			get
			{
				return this._creativeButton;
			}
		}

		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x06003373 RID: 13171 RVA: 0x000F32A5 File Offset: 0x000F14A5
		public TouchButton InfoButton
		{
			get
			{
				return this._infoButton;
			}
		}

		// Token: 0x06003374 RID: 13172 RVA: 0x000F32B0 File Offset: 0x000F14B0
		public void Initialize(IScope scope, VisualConstantsData visualConstantsData, MapButton owningButton)
		{
			this._scope = scope;
			this._visualConstantsData = visualConstantsData;
			this._owningMapButton = owningButton;
			this._player = this._scope.Get<ActivePlayer>();
			this._themeDatabase = this._scope.Get<MotorwaysThemeDatabase>();
			this.GameMode = this._player.GetSelectedModeForMap(this._owningMapButton.MapDefinition.mapName);
			if (this.GameMode == GameMode.Normal)
			{
				this._infoButton.interactable = false;
				this._infoButton.animator.SetTrigger(MapButtonModeSelectCard.Disabled);
			}
			Theme theme = this._themeDatabase.GetTheme() as Theme;
			this._classicModeTextColor = theme.GetColor(this._classicModeTextThemeColor, "_Color");
			this._buttonGroup.Initialize();
			this.UpdateModeStringsAndColors();
			this.UpdateButtonLockStatus(false);
			this._header.SetStringId(scope, this._owningMapButton.MapDefinition.mapName);
			LocaleDatabase localeDatabase = scope.Get<LocaleDatabase>();
			localeDatabase.AddLocalizedObject(this);
			this.HandleLocaleChanged(localeDatabase.CurrentLocale);
			base.StartCoroutine(this.UpdateButtonGroup());
		}

		// Token: 0x06003375 RID: 13173 RVA: 0x000F33C2 File Offset: 0x000F15C2
		private void OnDestroy()
		{
			this._scope.Get<LocaleDatabase>().RemoveLocalizedObject(this);
		}

		// Token: 0x06003376 RID: 13174 RVA: 0x000F33D8 File Offset: 0x000F15D8
		public void SetGameMode(GameMode gameMode, bool hasInfoButton)
		{
			this.GameMode = gameMode;
			this.UpdateModeStringsAndColors();
			this._infoButton.interactable = hasInfoButton;
			this._infoButton.animator.SetTrigger(hasInfoButton ? MapButtonModeSelectCard.Normal : MapButtonModeSelectCard.Disabled);
			this._player.SetSelectedGameMode(this._owningMapButton.MapDefinition.mapName, gameMode);
		}

		// Token: 0x06003377 RID: 13175 RVA: 0x000F3439 File Offset: 0x000F1639
		public void OnNormalModeSelected()
		{
			this.SetGameMode(GameMode.Normal, false);
			Action action = this.onModePressed;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06003378 RID: 13176 RVA: 0x000F3453 File Offset: 0x000F1653
		public void OnEndlessModeSelected()
		{
			this.SetGameMode(GameMode.Endless, true);
			Action action = this.onModePressed;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06003379 RID: 13177 RVA: 0x000F3470 File Offset: 0x000F1670
		public void OnExpertModeSelected()
		{
			if (!this._owningMapButton.MapDefinition.IsExpertModeUnlocked(this._scope))
			{
				Action action = this.onExpertLockedPressed;
				if (action != null)
				{
					action();
				}
				this.UpdateModeStringsAndColors();
				return;
			}
			this.SetGameMode(GameMode.Expert, true);
			Action action2 = this.onModePressed;
			if (action2 == null)
			{
				return;
			}
			action2();
		}

		// Token: 0x0600337A RID: 13178 RVA: 0x000F34C5 File Offset: 0x000F16C5
		public void OnCreativeModeSelected()
		{
			this.SetGameMode(GameMode.Creative, true);
			Action action = this.onModePressed;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x0600337B RID: 13179 RVA: 0x000F34E0 File Offset: 0x000F16E0
		public void OnRegainedFocus()
		{
			GameMode gameMode = this._player.GetSelectedModeForMap(this._owningMapButton.MapDefinition.mapName);
			Animator endlessAnimator = this._endlessButton.GetComponent<Animator>();
			Animator normalAnimator = this._normalButton.GetComponent<Animator>();
			Animator expertAnimator = this._expertButton.GetComponent<Animator>();
			Animator creativeAnimator = this._creativeButton.GetComponent<Animator>();
			switch (gameMode)
			{
			case GameMode.Normal:
				endlessAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				expertAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				creativeAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				normalAnimator.SetTrigger(MapButtonModeSelectCard.Selected);
				endlessAnimator.SetTrigger(MapButtonModeSelectCard.Lowlight);
				expertAnimator.SetTrigger(MapButtonModeSelectCard.Lowlight);
				creativeAnimator.SetTrigger(MapButtonModeSelectCard.Lowlight);
				return;
			case GameMode.Tutorial:
			case GameMode.Background:
				break;
			case GameMode.Endless:
				normalAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				endlessAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				expertAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				creativeAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				normalAnimator.SetTrigger(MapButtonModeSelectCard.Lowlight);
				endlessAnimator.SetTrigger(MapButtonModeSelectCard.Selected);
				expertAnimator.SetTrigger(MapButtonModeSelectCard.Lowlight);
				creativeAnimator.SetTrigger(MapButtonModeSelectCard.Lowlight);
				return;
			case GameMode.Expert:
				normalAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				endlessAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				expertAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				creativeAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				normalAnimator.SetTrigger(MapButtonModeSelectCard.Lowlight);
				endlessAnimator.SetTrigger(MapButtonModeSelectCard.Lowlight);
				expertAnimator.SetTrigger(MapButtonModeSelectCard.Selected);
				creativeAnimator.SetTrigger(MapButtonModeSelectCard.Lowlight);
				return;
			default:
				if (gameMode != GameMode.Creative)
				{
					return;
				}
				normalAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				endlessAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				expertAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				creativeAnimator.ResetTrigger(MapButtonModeSelectCard.Normal);
				normalAnimator.SetTrigger(MapButtonModeSelectCard.Lowlight);
				endlessAnimator.SetTrigger(MapButtonModeSelectCard.Lowlight);
				expertAnimator.SetTrigger(MapButtonModeSelectCard.Lowlight);
				creativeAnimator.SetTrigger(MapButtonModeSelectCard.Selected);
				break;
			}
		}

		// Token: 0x0600337C RID: 13180 RVA: 0x000F36BC File Offset: 0x000F18BC
		public void ResetToNormal()
		{
			this.SetGameMode(GameMode.Normal, false);
		}

		// Token: 0x0600337D RID: 13181 RVA: 0x000F36C8 File Offset: 0x000F18C8
		private void UpdateModeStringsAndColors()
		{
			if (this._scope == null)
			{
				return;
			}
			MotorwaysStringKey modeStringKey = this._scope.Get<MotorwaysStringKey>();
			GameMode gameMode = this.GameMode;
			switch (gameMode)
			{
			case GameMode.Normal:
				this._buttonGroup.OnButtonClicked(this.NormalButton);
				modeStringKey.InitWithStringId(StringId.Normal);
				this._modeNameText.TextField.color = this._classicModeTextColor;
				break;
			case GameMode.Tutorial:
			case GameMode.Background:
				break;
			case GameMode.Endless:
				this._buttonGroup.OnButtonClicked(this.EndlessButton);
				modeStringKey.InitWithStringId(StringId.Endless);
				this._modeNameText.TextField.color = this._visualConstantsData.EndlessTabButtonColor;
				break;
			case GameMode.Expert:
				this._buttonGroup.OnButtonClicked(this.ExpertButton);
				modeStringKey.InitWithStringId(StringId.Expert);
				this._modeNameText.TextField.color = this._visualConstantsData.ExpertTabButtonColor;
				break;
			default:
				if (gameMode == GameMode.Creative)
				{
					this._buttonGroup.OnButtonClicked(this.CreativeButton);
					modeStringKey.InitWithStringId(StringId.Creative);
					this._modeNameText.TextField.color = this._visualConstantsData.CreativeTabButtonColor;
				}
				break;
			}
			this._modeNameText.LocString = StandaloneLocString.CreateString(this._scope, modeStringKey);
		}

		// Token: 0x0600337E RID: 13182 RVA: 0x000F380E File Offset: 0x000F1A0E
		public void OnInfoButtonPressed()
		{
			Action action = this.onMoreModeInfoPressed;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x0600337F RID: 13183 RVA: 0x000F3820 File Offset: 0x000F1A20
		public void UpdateButtonLockStatus()
		{
			this.UpdateButtonLockStatus(true);
		}

		// Token: 0x06003380 RID: 13184 RVA: 0x000F3829 File Offset: 0x000F1A29
		private IEnumerator UpdateButtonGroup()
		{
			yield return new WaitForEndOfFrame();
			this._buttonGroup.OnButtonClicked(this._buttonGroup.activeButton);
			yield break;
		}

		// Token: 0x06003381 RID: 13185 RVA: 0x000F3838 File Offset: 0x000F1A38
		public void UpdateButtonLockStatus(bool playAnimations)
		{
			bool isLocked = !this._scope.Get<ActivePlayer>().HasSeenNewContent(MapButtonModeSelectCard.GetUnlockAnimationNciID(this._owningMapButton.MapDefinition)) || !this._owningMapButton.MapDefinition.IsExpertModeUnlocked(this._scope);
			if (FeatureToggle.IsFeatureDisabled(Feature.MapUnlocks))
			{
				isLocked = false;
			}
			if (playAnimations && isLocked && this._owningMapButton.MapDefinition.IsExpertModeUnlocked(this._scope) && this._owningMapButton.Type == MapButton.MapButtonType.City)
			{
				this.PlayExpertUnlockAnimation();
			}
			else
			{
				this._expertButton.animator.SetBool(MapButtonModeSelectCard.ShouldShowLockIcon, isLocked);
				if (isLocked)
				{
					this._expertButton.animator.Play(MapButtonModeSelectCard.IconShownStateId, 1);
					this._expertButton.animator.Update(0f);
				}
			}
			if (this.GameMode == GameMode.Normal)
			{
				this._infoButton.animator.Play(MapButtonModeSelectCard.Disabled, -1, 1f);
			}
		}

		// Token: 0x06003382 RID: 13186 RVA: 0x000F3928 File Offset: 0x000F1B28
		public static string GetUnlockAnimationNciID(MapDefinition mapDefinition)
		{
			return "ExpertUnlockAnimation-" + mapDefinition.cityName.ToLower();
		}

		// Token: 0x06003383 RID: 13187 RVA: 0x000F393F File Offset: 0x000F1B3F
		public static string GetNewContentIndicatorID(MapDefinition mapDefinition)
		{
			return "ExpertModeUnlock-" + mapDefinition.cityName.ToLower();
		}

		// Token: 0x06003384 RID: 13188 RVA: 0x000F3958 File Offset: 0x000F1B58
		private void PlayExpertUnlockAnimation()
		{
			this._scope.Get<AudioSystem>().ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.UnlockMap, 0.5f, -1f, true, null));
			this._scope.Get<ActivePlayer>().SetNewContentSeen(MapButtonModeSelectCard.GetUnlockAnimationNciID(this._owningMapButton.MapDefinition));
			this._scope.Get<ActivePlayer>().SetNewContentSeen(MapButtonModeSelectCard.GetNewContentIndicatorID(this._owningMapButton.MapDefinition));
			this._expertButton.animator.SetBool(MapButtonModeSelectCard.ShouldShowLockIcon, false);
		}

		// Token: 0x06003385 RID: 13189 RVA: 0x000F39EE File Offset: 0x000F1BEE
		public override void SetVisible(bool isVisible)
		{
			base.SetVisible(isVisible);
			this.UpdateModeStringsAndColors();
			this.UpdateButtonLockStatus(false);
		}

		// Token: 0x06003386 RID: 13190 RVA: 0x000F3A04 File Offset: 0x000F1C04
		public override void SetSelected(bool isSelected)
		{
			base.SetSelected(isSelected);
			this.UpdateButtonLockStatus(false);
		}

		// Token: 0x06003387 RID: 13191 RVA: 0x000F3A14 File Offset: 0x000F1C14
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

		// Token: 0x06003388 RID: 13192 RVA: 0x000F3A8B File Offset: 0x000F1C8B
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
			this.UpdateModeStringsAndColors();
		}

		// Token: 0x06003389 RID: 13193 RVA: 0x000F3A94 File Offset: 0x000F1C94
		public void ApplyTheme(ITheme newTheme)
		{
			Theme theme = (Theme)newTheme;
			this._classicModeTextColor = theme.GetColor(this._classicModeTextThemeColor, "_Color");
			this.UpdateModeStringsAndColors();
		}

		// Token: 0x0600338A RID: 13194 RVA: 0x000F3AC8 File Offset: 0x000F1CC8
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			Color oldColor = (oldTheme as Theme).GetColor(this._classicModeTextThemeColor, "_Color");
			Color newColor = (newTheme as Theme).GetColor(this._classicModeTextThemeColor, "_Color");
			this._classicModeTextColor = Color.Lerp(oldColor, newColor, progress);
			this.UpdateModeStringsAndColors();
			if (!(oldColor == newColor))
			{
				return ThemeBlendingResult.ContinueBlending;
			}
			return ThemeBlendingResult.StopBlending;
		}

		// Token: 0x0600338B RID: 13195 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x04002BD8 RID: 11224
		[SerializeField]
		private LocalizedTextUI _bestScoreText;

		// Token: 0x04002BD9 RID: 11225
		[SerializeField]
		private LocalizedTextUI _modeNameText;

		// Token: 0x04002BDA RID: 11226
		[SerializeField]
		private TouchButton _infoButton;

		// Token: 0x04002BDB RID: 11227
		[SerializeField]
		private TouchButton _normalButton;

		// Token: 0x04002BDC RID: 11228
		[SerializeField]
		private TouchButton _endlessButton;

		// Token: 0x04002BDD RID: 11229
		[SerializeField]
		private TouchButton _expertButton;

		// Token: 0x04002BDE RID: 11230
		[SerializeField]
		private TouchButton _creativeButton;

		// Token: 0x04002BDF RID: 11231
		[SerializeField]
		private LocalizedTextUI _header;

		// Token: 0x04002BE0 RID: 11232
		[SerializeField]
		private LocalizedTextUI[] _currentModeText;

		// Token: 0x04002BE1 RID: 11233
		[SerializeField]
		private ButtonGroup _buttonGroup;

		// Token: 0x04002BE2 RID: 11234
		private IScope _scope;

		// Token: 0x04002BE3 RID: 11235
		private VisualConstantsData _visualConstantsData;

		// Token: 0x04002BE4 RID: 11236
		private MapButton _owningMapButton;

		// Token: 0x04002BE5 RID: 11237
		private ActivePlayer _player;

		// Token: 0x04002BE6 RID: 11238
		private MotorwaysThemeDatabase _themeDatabase;

		// Token: 0x04002BE7 RID: 11239
		[SerializeField]
		private ThemedMaterialType _classicModeTextThemeColor = ThemedMaterialType.Dark;

		// Token: 0x04002BE8 RID: 11240
		private Color _classicModeTextColor = Color.black;

		// Token: 0x04002BE9 RID: 11241
		private static readonly int Disabled = Animator.StringToHash("Disabled");

		// Token: 0x04002BEA RID: 11242
		private static readonly int Normal = Animator.StringToHash("Normal");

		// Token: 0x04002BEB RID: 11243
		private static readonly int Lowlight = Animator.StringToHash("Lowlight");

		// Token: 0x04002BEC RID: 11244
		private static readonly int ShouldShowLockIcon = Animator.StringToHash("ShouldShowLockIcon");

		// Token: 0x04002BED RID: 11245
		private static readonly int IconShownStateId = Animator.StringToHash("IconShown");

		// Token: 0x04002BEE RID: 11246
		private static readonly int Selected = Animator.StringToHash("Selected");
	}
}
