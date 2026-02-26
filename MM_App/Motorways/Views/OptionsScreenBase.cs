using System;
using System.Collections;
using System.Collections.Generic;
using Factory;
using Motorways.Audio;
using Motorways.UI;
using NaughtyAttributes;
using Notifications;
using Notifications.Services;
using NotificationService.Events;
using Popups;
using Screens;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x0200055C RID: 1372
	public class OptionsScreenBase : BaseScalingScreen
	{
		// Token: 0x060024E5 RID: 9445 RVA: 0x0009AA6C File Offset: 0x00098C6C
		public void OnFullscreenButtonToggled(bool isFullScreen)
		{
			if (!this._hardwareCapabilities.SupportsChangingResolution)
			{
				return;
			}
			Resolution nativeResolution = this.GetNativeResolution();
			if (isFullScreen)
			{
				Screen.SetResolution(nativeResolution.width, nativeResolution.height, FullScreenMode.MaximizedWindow);
				OptionsScreenBase.Log.Info("Set resolution to {0}x{1}, {2}", new object[]
				{
					nativeResolution.width,
					nativeResolution.height,
					true
				});
				for (int resolutionIndex = 0; resolutionIndex < this._displayedResolutions.Count; resolutionIndex++)
				{
					if (this._displayedResolutions[resolutionIndex].width == nativeResolution.width && this._displayedResolutions[resolutionIndex].height == nativeResolution.height)
					{
						this.resolutionsDropdown.SetSelectedOption(resolutionIndex);
						break;
					}
				}
			}
			else if (Screen.fullScreen || Application.isEditor)
			{
				int resolutionIndexToUse = -1;
				Vector2 halfNativeResolutionVector = new Vector2((float)nativeResolution.width * 0.5f, (float)nativeResolution.height * 0.5f);
				float bestDistanceToHalfNativeResolution = float.MaxValue;
				for (int resolutionIndex2 = 0; resolutionIndex2 < this._displayedResolutions.Count; resolutionIndex2++)
				{
					Resolution resolution = this._displayedResolutions[resolutionIndex2];
					Vector2 resolutionVector = new Vector2((float)resolution.width, (float)resolution.height);
					float currentDistanceToHalfNativeResolution = Vector2.Distance(halfNativeResolutionVector, resolutionVector);
					if (currentDistanceToHalfNativeResolution < bestDistanceToHalfNativeResolution)
					{
						resolutionIndexToUse = resolutionIndex2;
						bestDistanceToHalfNativeResolution = currentDistanceToHalfNativeResolution;
					}
				}
				Resolution resolutionToUse = this._displayedResolutions[resolutionIndexToUse];
				Screen.SetResolution(resolutionToUse.width, resolutionToUse.height, false);
				OptionsScreenBase.Log.Info("Set resolution to {0}, fullscreen: {1}", new object[]
				{
					resolutionToUse,
					false
				});
				this.resolutionsDropdown.SetSelectedOption(resolutionIndexToUse);
			}
			else
			{
				OptionsScreenBase.Log.Info("Ignoring switch to windowed because the app's window isn't fullscreen.", Array.Empty<object>());
			}
			base.StartCoroutine(this.ResizeOptionsScreenAtEndOfFrame(new Vector2((float)Screen.width, (float)Screen.height)));
		}

		// Token: 0x060024E6 RID: 9446 RVA: 0x0009AC6C File Offset: 0x00098E6C
		private Resolution GetNativeResolution()
		{
			if (DesktopHardwareCapabilities.SafeAreaHeight > 0)
			{
				Vector2Int safeAreaResolution = DesktopHardwareCapabilities.GetClosestResolution(DesktopHardwareCapabilities.SafeAreaDimensions);
				foreach (Resolution resolution in this._displayedResolutions)
				{
					if (resolution.width == safeAreaResolution.x && resolution.height == safeAreaResolution.y)
					{
						Diagnostics.Log.Channel log = OptionsScreenBase.Log;
						string message = "Selecting {0}x{1} as the native resolution to fit the screen's safe area better than the actual resolution of {2}x{3}.";
						object[] array = new object[4];
						array[0] = resolution.width;
						array[1] = resolution.height;
						int num = 2;
						Resolution resolution2 = this._displayedResolutions[0];
						array[num] = resolution2.width;
						int num2 = 3;
						resolution2 = this._displayedResolutions[0];
						array[num2] = resolution2.height;
						log.Info(message, array);
						return resolution;
					}
					OptionsScreenBase.Log.Warn("Couldn't find a resolution to fit the safe area of {0}x{1}.", new object[]
					{
						safeAreaResolution.x,
						safeAreaResolution.y
					});
				}
			}
			return this._displayedResolutions[0];
		}

		// Token: 0x060024E7 RID: 9447 RVA: 0x0009ADA4 File Offset: 0x00098FA4
		public void OnCloudSavesButtonToggled(bool cloudSavesOn)
		{
			this._player.SyncToCloud = cloudSavesOn;
		}

		// Token: 0x060024E8 RID: 9448 RVA: 0x0009ADB2 File Offset: 0x00098FB2
		public void OnNightmodeButtonToggled(bool nightmodeOn)
		{
			this._themeDatabase.SetNightMode(nightmodeOn, true);
			this.colorblindCustomisationPanel.BuildVisualPanel();
		}

		// Token: 0x060024E9 RID: 9449 RVA: 0x0009ADCC File Offset: 0x00098FCC
		public void OnColorblindButtonToggled(bool colorblindOn)
		{
			this._themeDatabase.SetColorblindMode(colorblindOn, true);
			this.colorblindCustomisationPanel.gameObject.SetActive(this._themeDatabase.IsInColorblindMode);
			this._player.SetNewContentSeen("NewColorblindPopup");
		}

		// Token: 0x060024EA RID: 9450 RVA: 0x0009AE06 File Offset: 0x00099006
		public void OnSkipTransitionsButtonToggled(bool doSkipTransitions)
		{
			this._player.IsSkipTransitionsEnabled = !doSkipTransitions;
		}

		// Token: 0x060024EB RID: 9451 RVA: 0x0009AE17 File Offset: 0x00099017
		public void OnAntiAliasingLevelChanged(int newAntiAliasingLevelOptionsValue)
		{
			this._player.AntiAliasingLevel = newAntiAliasingLevelOptionsValue;
			OptionsScreenBase.SetAntiAliasingLevel(this._player.AntiAliasingMSAALevelForUniversalRenderPipeline);
		}

		// Token: 0x060024EC RID: 9452 RVA: 0x0009AE38 File Offset: 0x00099038
		public static void SetAntiAliasingLevel(int newAntiAliasingLevel)
		{
			UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (universalRenderPipelineAsset != null)
			{
				universalRenderPipelineAsset.msaaSampleCount = newAntiAliasingLevel;
			}
		}

		// Token: 0x060024ED RID: 9453 RVA: 0x0009AE5A File Offset: 0x0009905A
		public void OnZoomButtonToggled(bool zoomOn)
		{
			this._player.IsZoomEnabled = zoomOn;
		}

		// Token: 0x060024EE RID: 9454 RVA: 0x0009AE68 File Offset: 0x00099068
		public void OnZoomLevelChanged(int newZoomLevel)
		{
			this._player.ZoomLevel = newZoomLevel;
		}

		// Token: 0x060024EF RID: 9455 RVA: 0x0009AE76 File Offset: 0x00099076
		public void OnControllerSensitivityChanged(int newSensitivity)
		{
			this._player.ControllerSensitivity = newSensitivity;
		}

		// Token: 0x060024F0 RID: 9456 RVA: 0x0009AE84 File Offset: 0x00099084
		public void OnDisplayChanged(int newDisplayValue)
		{
			if (Diagnostics.Verify(MultiDisplayCapabilitiesBridge.SetActiveDisplayIndex(newDisplayValue), "Failed to change selected display to {0}", newDisplayValue))
			{
				this._player.SelectedDisplay = MultiDisplayCapabilitiesBridge.GetActiveDisplayIndex();
			}
			this.UpdateResolutions();
			if (Screen.fullScreen || Application.isEditor)
			{
				int newResolutionIndex = 0;
				for (int resolutionIndex = 0; resolutionIndex < this._displayedResolutions.Count; resolutionIndex++)
				{
					if (this._displayedResolutions[resolutionIndex].height == Screen.currentResolution.height && this._displayedResolutions[resolutionIndex].width == Screen.currentResolution.width)
					{
						newResolutionIndex = resolutionIndex;
						break;
					}
				}
				this.resolutionsDropdown.SetSelectedOption(newResolutionIndex);
			}
		}

		// Token: 0x060024F1 RID: 9457 RVA: 0x0009AF3B File Offset: 0x0009913B
		public void OnVibrationButtonToggled(bool enableVibrations)
		{
			this._player.IsVibrationEnabled = enableVibrations;
		}

		// Token: 0x060024F2 RID: 9458 RVA: 0x0009AF49 File Offset: 0x00099149
		public void OnDrawModeToggleButtonToggled(bool enableDrawModeToggle)
		{
			this._player.IsDrawModeToggleEnabled = enableDrawModeToggle;
		}

		// Token: 0x060024F3 RID: 9459 RVA: 0x0009AF57 File Offset: 0x00099157
		private void OnTelemetryButtonToggled(bool enableTelemetry)
		{
			this._player.IsTelemetryEnabled = enableTelemetry;
		}

		// Token: 0x060024F4 RID: 9460 RVA: 0x0009AF65 File Offset: 0x00099165
		private void OnHoldDrawButtonToggled(bool enableHoldToDraw)
		{
			this._player.IsTapDrawEnabled = !enableHoldToDraw;
		}

		// Token: 0x060024F5 RID: 9461 RVA: 0x0009AF76 File Offset: 0x00099176
		public void OnVolumeChanged(int newValue)
		{
			this._player.VolumeSetting = newValue;
		}

		// Token: 0x060024F6 RID: 9462 RVA: 0x0009AF84 File Offset: 0x00099184
		public void OnSoundscapeChanged(int newValue)
		{
			this._player.Soundscape = newValue;
		}

		// Token: 0x060024F7 RID: 9463 RVA: 0x0009AF94 File Offset: 0x00099194
		public void OnResolutionSelected(int resolutionIndex)
		{
			Resolution selectedResolution = this._displayedResolutions[resolutionIndex];
			Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
			OptionsScreenBase.Log.Info("Setting resolution to {0}x{1}", new object[]
			{
				selectedResolution.width,
				selectedResolution.height
			});
			base.StartCoroutine(this.ResizeOptionsScreenAtEndOfFrame(new Vector2((float)Screen.width, (float)Screen.height)));
		}

		// Token: 0x060024F8 RID: 9464 RVA: 0x0009B017 File Offset: 0x00099217
		private IEnumerator ResizeOptionsScreenAtEndOfFrame(Vector2 oldResolution)
		{
			yield return new WaitForEndOfFrame();
			if (Application.isEditor)
			{
				yield break;
			}
			int num;
			for (int attemptNumber = 0; attemptNumber < 100; attemptNumber = num + 1)
			{
				if (Diagnostics.Verify(oldResolution.x != (float)Screen.width || oldResolution.y != (float)Screen.height, "We waited for the end of the frame and the screen size still isn't different! Old res: {0} - new res: {1}. Attempt number {2}", oldResolution, new Vector2((float)Screen.width, (float)Screen.height), attemptNumber))
				{
					OptionsScreenBase.Log.Info("Refreshing the options screen based on current resolution: {0}x{1} from {2}", new object[]
					{
						Screen.width,
						Screen.height,
						oldResolution
					});
					this.optionsPages.RefreshPageTransforms(1);
					break;
				}
				yield return new WaitForEndOfFrame();
				num = attemptNumber;
			}
			yield break;
		}

		// Token: 0x060024F9 RID: 9465 RVA: 0x0009B02D File Offset: 0x0009922D
		public void OnMenuMessagesButtonToggled(bool enableMenuMessages)
		{
			this._player.AreMenuMessagesEnabled = enableMenuMessages;
		}

		// Token: 0x060024FA RID: 9466 RVA: 0x0009B03B File Offset: 0x0009923B
		public void OnChallengeRemindersButtonToggled(bool enableChallengeReminders)
		{
			this._player.IsChallengeRemindersEnabledSetting = enableChallengeReminders;
			this._notificationScheduler.ScheduleNotifications();
		}

		// Token: 0x060024FB RID: 9467 RVA: 0x0009B054 File Offset: 0x00099254
		public void OnContentRemindersButtonToggled(bool enableContentReminders)
		{
			this._player.IsContentRemindersEnabledSetting = enableContentReminders;
			this._notificationScheduler.ScheduleNotifications();
		}

		// Token: 0x060024FC RID: 9468 RVA: 0x0009B070 File Offset: 0x00099270
		public void OnEnableNotificationsButtonPressed()
		{
			AuthorizationStatus authorizationStatus = this._systemNotificationService.AuthorizationStatus;
			if (authorizationStatus <= AuthorizationStatus.Denied)
			{
				this._systemNotificationService.RequestAuthorization(delegate(bool granted)
				{
					this.OnNotificationAuthorizationRequestComplete = new Action(this.UpdateButtonStatesFromSettings);
					if (!granted && this._systemNotificationService.AuthorizationStatus == AuthorizationStatus.Denied)
					{
						iOSSystemNotificationService iOSSystemNotificationService = this._systemNotificationService as iOSSystemNotificationService;
						if (iOSSystemNotificationService != null)
						{
							iOSSystemNotificationService.OpenApplicationSettings();
						}
					}
				});
				return;
			}
			Diagnostics.FailAssert("Enable Notifications button pressed when status was {0}. This should not be possible.", new object[]
			{
				this._systemNotificationService.AuthorizationStatus
			});
		}

		// Token: 0x060024FD RID: 9469 RVA: 0x0009B0C8 File Offset: 0x000992C8
		public void OnSendTestNotificationButtonPressed()
		{
			this._notificationScheduler.ScheduleTestNotification();
		}

		// Token: 0x060024FE RID: 9470 RVA: 0x0009B0D5 File Offset: 0x000992D5
		public void OnClearEventsButtonPressed()
		{
			this._notificationEventSystem.RemoveAll();
			this._notificationEventSystem.RecordEvent(new OpenedMiniMotorways(), true);
			this.UpdateClearEventsButtonText();
		}

		// Token: 0x060024FF RID: 9471 RVA: 0x0009B0F9 File Offset: 0x000992F9
		public void OnResetAchievementsButton()
		{
			GameCenterShared.GCResetAchievements();
		}

		// Token: 0x06002500 RID: 9472 RVA: 0x0009B100 File Offset: 0x00099300
		public void OnOpeniCloudFaq()
		{
			Application.OpenURL(this._visualConstants.iCloudLinkString);
		}

		// Token: 0x06002501 RID: 9473 RVA: 0x0009B112 File Offset: 0x00099312
		private void UpdateClearEventsButtonText()
		{
			this.clearNotificationEventsButton.transform.GetComponentInChildren<TMP_Text>().text = string.Format("Clear {0} Events", this._notificationEventSystem.AllEvents.Count);
		}

		// Token: 0x06002502 RID: 9474 RVA: 0x0009B148 File Offset: 0x00099348
		private void UpdateSendTestNotificationButtonText()
		{
			this.sendTestNotificationButton.transform.GetComponentInChildren<TMP_Text>().text = string.Format("Send Test Notification (in {0}s)", 15);
		}

		// Token: 0x06002503 RID: 9475 RVA: 0x0009B170 File Offset: 0x00099370
		public void OnBack()
		{
			if (this.resolutionsDropdown.dropdownList.activeSelf)
			{
				this.resolutionsDropdown.DismissDropdown();
				return;
			}
			this._screenStack.PopOneScreen();
		}

		// Token: 0x06002504 RID: 9476 RVA: 0x0009B19C File Offset: 0x0009939C
		public void OnGamepadInputTypeSelected()
		{
			this.SetControllerCanvas(this._hardwareCapabilities.CurrentGamepadStyle);
			this.siriRemoteCanvas.alpha = 0f;
			this.keyboardCanvas.alpha = 0f;
			this.mouseCanvas.alpha = 0f;
			this.touchCanvas.alpha = 0f;
		}

		// Token: 0x06002505 RID: 9477 RVA: 0x0009B1FC File Offset: 0x000993FC
		public void OnRemoteInputTypeSelected()
		{
			this.SetControllerCanvas(DeviceInputGamepadStyle.None);
			this.siriRemoteCanvas.alpha = 1f;
			this.keyboardCanvas.alpha = 0f;
			this.mouseCanvas.alpha = 0f;
			this.touchCanvas.alpha = 0f;
		}

		// Token: 0x06002506 RID: 9478 RVA: 0x0009B250 File Offset: 0x00099450
		public void OnMouseInputTypeSelected()
		{
			this.SetControllerCanvas(DeviceInputGamepadStyle.None);
			this.siriRemoteCanvas.alpha = 0f;
			this.keyboardCanvas.alpha = 0f;
			this.mouseCanvas.alpha = 1f;
			this.touchCanvas.alpha = 0f;
		}

		// Token: 0x06002507 RID: 9479 RVA: 0x0009B2A4 File Offset: 0x000994A4
		public void OnKeyboardInputTypeSelected()
		{
			this.SetControllerCanvas(DeviceInputGamepadStyle.None);
			this.siriRemoteCanvas.alpha = 0f;
			this.keyboardCanvas.alpha = 1f;
			this.mouseCanvas.alpha = 0f;
			this.touchCanvas.alpha = 0f;
		}

		// Token: 0x06002508 RID: 9480 RVA: 0x0009B2F8 File Offset: 0x000994F8
		public void OnTouchInputTypeSelected()
		{
			this.SetControllerCanvas(DeviceInputGamepadStyle.None);
			this.siriRemoteCanvas.alpha = 0f;
			this.keyboardCanvas.alpha = 0f;
			this.mouseCanvas.alpha = 0f;
			this.touchCanvas.alpha = 1f;
		}

		// Token: 0x06002509 RID: 9481 RVA: 0x0009B34C File Offset: 0x0009954C
		public void RefreshControllerSymbols()
		{
			if (this._controllerSymbols == null)
			{
				this._controllerSymbols = base.gameObject.GetComponentsInChildren<ControllerSymbol>();
			}
			ControllerSymbol[] controllerSymbols = this._controllerSymbols;
			for (int i = 0; i < controllerSymbols.Length; i++)
			{
				controllerSymbols[i].Initialize(this._controllerButtonToSymbolService);
			}
			if (this._controllerButtonToSymbolService.HasMappings)
			{
				bool shouldShowControllerDiagram = false;
				foreach (ControllerSymbol controllerSymbol in this._controllerSymbols)
				{
					if (controllerSymbol.shouldUseControllerButton)
					{
						shouldShowControllerDiagram |= controllerSymbol.IsUsingDefaultSymbol;
					}
				}
				this.mfiControllerDiagram.SetActive(shouldShowControllerDiagram);
			}
			this.RegisterThemeComponents(this._themeDatabase.GetTheme());
		}

		// Token: 0x0600250A RID: 9482 RVA: 0x0009B3EC File Offset: 0x000995EC
		private void SetControllerCanvas(DeviceInputGamepadStyle gamepadStyle)
		{
			this.controllerCanvas.alpha = (float)((gamepadStyle == DeviceInputGamepadStyle.Generic) ? 1 : 0);
			this.switchJoyconDualCanvas.alpha = (float)((gamepadStyle == DeviceInputGamepadStyle.SwitchJoyConDual) ? 1 : 0);
			this.switchHandheldCanvas.alpha = (float)((gamepadStyle == DeviceInputGamepadStyle.SwitchHandheld) ? 1 : 0);
			this.switchProCanvas.alpha = (float)((gamepadStyle == DeviceInputGamepadStyle.SwitchPro) ? 1 : 0);
			this.switchJoyconLCanvas.alpha = (float)((gamepadStyle == DeviceInputGamepadStyle.SwitchJoyConL) ? 1 : 0);
			this.switchJoyconRCanvas.alpha = (float)((gamepadStyle == DeviceInputGamepadStyle.SwitchJoyConR) ? 1 : 0);
		}

		// Token: 0x0600250B RID: 9483 RVA: 0x0009B474 File Offset: 0x00099674
		private TouchButton GetGamepadStyleButton(DeviceInputGamepadStyle gamepadStyle)
		{
			TouchButton result;
			switch (gamepadStyle)
			{
			case DeviceInputGamepadStyle.SwitchJoyConDual:
				result = this.switchJoyconDualButton;
				break;
			case DeviceInputGamepadStyle.SwitchHandheld:
				result = this.switchHandheldButton;
				break;
			case DeviceInputGamepadStyle.SwitchPro:
				result = this.switchProButton;
				break;
			case DeviceInputGamepadStyle.SwitchJoyConL:
				result = this.switchJoyconLButton;
				break;
			case DeviceInputGamepadStyle.SwitchJoyConR:
				result = this.switchJoyconRButton;
				break;
			default:
				result = this.gamepadInputButton;
				break;
			}
			return result;
		}

		// Token: 0x0600250C RID: 9484 RVA: 0x0009B4D4 File Offset: 0x000996D4
		public void OnNewPageSelected()
		{
			this.SetOptionButtonsRightNavigation(this.optionsPages.GetFirstSelectableOnCurrentPage());
		}

		// Token: 0x0600250D RID: 9485 RVA: 0x0009B4E8 File Offset: 0x000996E8
		private void SetOptionButtonsRightNavigation(Selectable newRightSideSelectable)
		{
			foreach (TouchButton touchButton in this.tabButtonGroup.buttons)
			{
				Navigation nav = touchButton.navigation;
				nav.selectOnRight = newRightSideSelectable;
				touchButton.navigation = nav;
			}
		}

		// Token: 0x0600250E RID: 9486 RVA: 0x0009B550 File Offset: 0x00099750
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			base.TransitionIn(outScreen);
			if (this is OptionsScreenPause)
			{
				this._canvasGroup.Alpha = 0f;
			}
			if (this._tutorialCityDefinition == null && this is OptionsScreenMain)
			{
				this._tutorialCityDefinition = AssetBundleUtility.LoadPrefabAsync(this.tutorialDefinition.mapAssetBundle, this.tutorialDefinition.mapPrefabName, this);
			}
			this.antiAliasingLevelOptions.gameObject.SetActive(this._hardwareCapabilities.SupportsAntiAliasingOptions);
			this.controllerSensitivityOptions.gameObject.SetActive(FeatureToggle.IsFeatureEnabled(Feature.ControllerSensitivityOption));
			this.fullscreenToggle.gameObject.SetActive(this._hardwareCapabilities.SupportsChangingResolution);
			int displayCount = MultiDisplayCapabilitiesBridge.GetDisplayCount();
			this.displaySelectionOptions.gameObject.SetActive(displayCount > 1 && FeatureToggle.IsFeatureEnabled(Feature.DisplaySelection));
			this.UpdateResolutions();
			this.vibrationsToggle.gameObject.SetActive(this._hardwareCapabilities.SupportsHapticFeedback);
			this.drawModeToggleToggle.gameObject.SetActive(this._hardwareCapabilities.DefaultDeviceInputType == DeviceInputType.Mouse);
			this.telemetryToggle.gameObject.SetActive(FeatureToggle.IsFeatureEnabled(Feature.TelemetryToggle));
			this.privacyButton.gameObject.SetActive(FeatureToggle.IsFeatureEnabled(Feature.TelemetryToggle) && this is OptionsScreenMain);
			this.telemetryToggle.onOptionTriggered.AddListener(new UnityAction<bool>(this.OnTelemetryButtonToggled));
			this.holdToDrawToggle.onOptionTriggered.AddListener(new UnityAction<bool>(this.OnHoldDrawButtonToggled));
			this.optionsPages.RefreshPageTransforms(0);
			this.firstFocus.OnActivate();
			this._reachability.ConnectivityChanged += this.OnInternetConnectivityChanged;
			this.OnInternetConnectivityChanged(this._reachability.Connectivity);
			this._storage.StatusChanged += this.OnStorageStatusChanged;
			this.OnStorageStatusChanged(this._storage.Status);
			Get.State |= StateType.MenuOptions;
			this.UpdateButtonStatesFromSettings();
			if (this.languagePanel.childCount == 0)
			{
				this.SetupButtons();
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.OptionsDebugMenu))
			{
				this.debugPageButton.gameObject.SetActive(true);
				this.debugOptionsPage.InitializeButtons();
			}
			else
			{
				this.debugPageButton.gameObject.SetActive(false);
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.MessageDebugButtons) && this is OptionsScreenMain)
			{
				this.clearNotificationEventsButton.gameObject.SetActive(true);
				this.UpdateClearEventsButtonText();
				this.sendTestNotificationButton.gameObject.SetActive(true);
				this.UpdateSendTestNotificationButtonText();
			}
			else
			{
				this.sendTestNotificationButton.gameObject.SetActive(false);
				this.clearNotificationEventsButton.gameObject.SetActive(false);
			}
			base.RegisterButtons();
			this.RegisterThemeComponents(this._themeDatabase.GetTheme());
			base.RegisterAllLocalizedTextChildren();
			this.optionsCanvasGroup.alpha = 1f;
			this.optionsCanvasGroup.interactable = true;
			this._player.DataChanged += this.UpdateButtonStatesFromSettings;
			if (FeatureToggle.IsFeatureEnabled(Feature.ResetAchievementsButton))
			{
				this.resetAchievementButton.gameObject.SetActive(true);
			}
			else
			{
				this.resetAchievementButton.gameObject.SetActive(false);
			}
			this.versionString.LocString = StandaloneLocString.CreateNonLocalizedString(this._appScope, string.Format("Mini Motorways {0} ({1})", global::Version.Name, global::Version.Timestamp));
			this.SetOptionButtonsRightNavigation(this.optionsPages.GetFirstSelectableOnCurrentPage());
			this._hardwareCapabilities.OnGamepadStyleChanged += this.OnGamepadStyleChanged;
			this.colorblindCustomisationPanel.Initialise(this._appScope, this.popupStack);
			this.colorblindCustomisationPanel.gameObject.SetActive(this._themeDatabase.IsInColorblindMode);
			this.colorblindCustomisationPanel.onUpdated += this.OnColorblindCustomisationUpdated;
			this.zoomToggleTouchButton = this.zoomToggle.GetComponent<TouchButton>();
			this.zoomToggle.gameObject.SetActive(FeatureToggle.IsFeatureEnabled(Feature.AutoZoomEnabledOption));
			if (this.zoomToggleTouchButton != null)
			{
				if (FeatureToggle.IsFeatureEnabled(Feature.AutoZoomEnabledOption))
				{
					BaseScalingScreen.SetNavigationOnUp(this.zoomLevelOptions.rightButton, this.zoomToggleTouchButton);
					BaseScalingScreen.SetNavigationOnDown(this.controllerSensitivityOptions.rightButton, this.zoomToggleTouchButton);
				}
				else
				{
					BaseScalingScreen.SetNavigationOnUp(this.zoomLevelOptions.rightButton, this.controllerSensitivityOptions.rightButton);
					BaseScalingScreen.SetNavigationOnDown(this.controllerSensitivityOptions.rightButton, this.zoomLevelOptions.rightButton);
				}
			}
			this.zoomLevelOptions.gameObject.SetActive(true);
			this.displayButton.gameObject.SetActive(this._softwareCapabilities.SupportsDisplayOptions);
			base.StartCoroutine(this.UpdateMaxLengthButtons());
		}

		// Token: 0x0600250F RID: 9487 RVA: 0x0009B9F1 File Offset: 0x00099BF1
		private void OnColorblindCustomisationUpdated()
		{
			this._themeDatabase.UpdateThemeFromCurrentDefinition(true);
		}

		// Token: 0x06002510 RID: 9488 RVA: 0x0009BA00 File Offset: 0x00099C00
		private void UpdateResolutions()
		{
			if (this._hardwareCapabilities.SupportsChangingResolution)
			{
				this._displayedResolutions.Clear();
				List<string> resolutionStrings = new List<string>();
				OptionsScreenBase.Log.Info("Loading up {0} resolutions and trying to find index for current resolution: {1}x{2}", new object[]
				{
					Screen.resolutions.Length,
					Screen.width,
					Screen.height
				});
				int currentResolutionIndex = -1;
				for (int resolutionIndex = 0; resolutionIndex < Screen.resolutions.Length; resolutionIndex++)
				{
					Resolution resolution = Screen.resolutions[resolutionIndex];
					if (!this.AlreadyContainsResolution(resolution))
					{
						if (Screen.width == resolution.width && Screen.height == resolution.height)
						{
							currentResolutionIndex = resolutionStrings.Count;
						}
						resolutionStrings.Add(string.Format("{0}x{1}", resolution.width, resolution.height));
						this._displayedResolutions.Add(resolution);
					}
				}
				if (currentResolutionIndex < 0)
				{
					currentResolutionIndex = Screen.resolutions.Length - 1;
				}
				this._displayedResolutions.Reverse();
				resolutionStrings.Reverse();
				currentResolutionIndex = resolutionStrings.Count - 1 - currentResolutionIndex;
				this.resolutionsDropdown.gameObject.SetActive(true);
				this.resolutionsDropdown.PopulateList(resolutionStrings, currentResolutionIndex, this._appScope, false);
				return;
			}
			this.resolutionsDropdown.gameObject.SetActive(false);
		}

		// Token: 0x06002511 RID: 9489 RVA: 0x0009BB50 File Offset: 0x00099D50
		private bool AlreadyContainsResolution(Resolution resolution)
		{
			foreach (Resolution displayedResolution in this._displayedResolutions)
			{
				if (displayedResolution.width == resolution.width && displayedResolution.height == resolution.height)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002512 RID: 9490 RVA: 0x0009BBC4 File Offset: 0x00099DC4
		public void SetupControllerHelpButtons()
		{
			this.gamepadInputButton.gameObject.SetActive(false);
			this.touchInputButton.gameObject.SetActive(false);
			this.siriRemoteButton.gameObject.SetActive(false);
			this.keyboardButton.gameObject.SetActive(false);
			this.mouseButton.gameObject.SetActive(false);
			this.switchProButton.gameObject.SetActive(false);
			this.switchHandheldButton.gameObject.SetActive(false);
			this.switchJoyconDualButton.gameObject.SetActive(false);
			this.switchJoyconLButton.gameObject.SetActive(false);
			this.switchJoyconRButton.gameObject.SetActive(false);
			this.GetGamepadStyleButton(this._hardwareCapabilities.CurrentGamepadStyle).gameObject.SetActive(true);
			if (this._hardwareCapabilities.DefaultDeviceInputType == DeviceInputType.Mouse)
			{
				this.keyboardButton.gameObject.SetActive(true);
				this.mouseButton.gameObject.SetActive(true);
			}
			if (this._hardwareCapabilities.DefaultDeviceInputType == DeviceInputType.Touch || this._hardwareCapabilities.CurrentGamepadStyle == DeviceInputGamepadStyle.SwitchHandheld)
			{
				this.touchInputButton.gameObject.SetActive(true);
			}
			switch (this._inputState.CurrentDeviceInputType)
			{
			case DeviceInputType.Touch:
				this.inputMethodButtonGroup.OnButtonClicked(this.touchInputButton);
				this.OnTouchInputTypeSelected();
				break;
			case DeviceInputType.Mouse:
				this.inputMethodButtonGroup.OnButtonClicked(this.mouseButton);
				this.OnMouseInputTypeSelected();
				break;
			case DeviceInputType.Remote:
				this.inputMethodButtonGroup.OnButtonClicked(this.siriRemoteButton);
				this.OnRemoteInputTypeSelected();
				break;
			case DeviceInputType.Controller:
			{
				TouchButton gamepadButton = this.GetGamepadStyleButton(this._hardwareCapabilities.CurrentGamepadStyle);
				this.inputMethodButtonGroup.OnButtonClicked(gamepadButton);
				this.OnGamepadInputTypeSelected();
				break;
			}
			}
			GameObject[] toolbarLockingControls = this._toolbarLockingControls;
			for (int i = 0; i < toolbarLockingControls.Length; i++)
			{
				toolbarLockingControls[i].SetActive(AppContainer.Environment.DeviceCategory == DeviceCategory.Desktop);
			}
		}

		// Token: 0x06002513 RID: 9491 RVA: 0x0009BDB0 File Offset: 0x00099FB0
		private void OnGamepadStyleChanged(DeviceInputGamepadStyle gamepadStyle)
		{
			if (this.optionsPages.CurrentPage == 4)
			{
				this.SetupControllerHelpButtons();
			}
		}

		// Token: 0x06002514 RID: 9492 RVA: 0x0009BDC6 File Offset: 0x00099FC6
		public override void OnTransitionedIn()
		{
			base.OnTransitionedIn();
			this.firstFocus.Select();
		}

		// Token: 0x06002515 RID: 9493 RVA: 0x0009BDDC File Offset: 0x00099FDC
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			base.TransitionOut(inScreen);
			this._skipTransitions = (this._skipTransitions || this is OptionsScreenPause);
			this._player.DataChanged -= this.UpdateButtonStatesFromSettings;
			this._reachability.ConnectivityChanged -= this.OnInternetConnectivityChanged;
			this._storage.StatusChanged -= this.OnStorageStatusChanged;
			Get.State &= ~StateType.MenuOptions;
			this._hardwareCapabilities.OnGamepadStyleChanged -= this.OnGamepadStyleChanged;
		}

		// Token: 0x06002516 RID: 9494 RVA: 0x0009BE73 File Offset: 0x0009A073
		public override void OnGainedFocus()
		{
			base.OnGainedFocus();
			this._shouldFadeIn = (this is OptionsScreenPause);
			this._shouldFadeOut = false;
		}

		// Token: 0x06002517 RID: 9495 RVA: 0x0009BE91 File Offset: 0x0009A091
		public override void OnLostFocus()
		{
			base.OnLostFocus();
			this._shouldFadeIn = false;
			this._shouldFadeOut = (this is OptionsScreenPause);
		}

		// Token: 0x06002518 RID: 9496 RVA: 0x0009BEB0 File Offset: 0x0009A0B0
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (this._shouldFadeIn || this._shouldFadeOut)
			{
				float newAlpha = this._canvasGroup.Alpha + (float)(this._shouldFadeIn ? 1 : -1) * Time.deltaTime / this._fadeDuration;
				if (newAlpha <= 0f || newAlpha >= 1f)
				{
					newAlpha = Mathf.Clamp(newAlpha, 0f, 1f);
					this._shouldFadeIn = false;
					this._shouldFadeOut = false;
				}
				this._canvasGroup.Alpha = newAlpha;
			}
			if (this._hasNewiCloudMessage)
			{
				this._hasNewiCloudMessage = false;
				this.SetiCloudMessage(this._iCloudMessageKey);
			}
			if (this._enterTutorialNextTick)
			{
				this._screenStack.PushScreen<GameContainerScreen>(ScreenStack.MotorwaysScreen.InGame, delegate(GameContainerScreen newScreen)
				{
					newScreen.PrepareForMap(UnityEngine.Object.Instantiate<GameObject>(this._tutorialCityDefinition.asset as GameObject).GetComponent<CityDefinition>(), this.tutorialDefinition, GameMode.Tutorial, null, false);
					this._analytics.TrackTutorialStarted(true);
				}, false, null, true, null);
				this._enterTutorialNextTick = false;
			}
			if (this.OnNotificationAuthorizationRequestComplete != null)
			{
				this.OnNotificationAuthorizationRequestComplete();
				this.OnNotificationAuthorizationRequestComplete = null;
			}
		}

		// Token: 0x06002519 RID: 9497 RVA: 0x0009BF97 File Offset: 0x0009A197
		private void OnApplicationPause(bool pauseStatus)
		{
			if (!pauseStatus)
			{
				this.UpdateButtonStatesFromSettings();
				this.RefreshControllerSymbols();
			}
		}

		// Token: 0x0600251A RID: 9498 RVA: 0x0009BFA8 File Offset: 0x0009A1A8
		private void UpdateButtonStatesFromSettings()
		{
			this._themeDatabase.OnPlayerDataChanged();
			this.nightModeToggle.SetOption(this._themeDatabase.IsInNightMode ? 1 : 0, true, false);
			this.colorblindModeToggle.SetOption(this._themeDatabase.IsInColorblindMode ? 1 : 0, true, false);
			this.skipTransitionsToggle.SetOption(this._player.IsSkipTransitionsEnabled ? 0 : 1, true, false);
			this.antiAliasingLevelOptions.SetOption(this._player.AntiAliasingLevel, false);
			this.controllerSensitivityOptions.SetOption(this._player.ControllerSensitivity, false);
			this.vibrationsToggle.SetOption(this._player.IsVibrationEnabled ? 1 : 0, true, false);
			this.drawModeToggleToggle.SetOption(this._player.IsDrawModeToggleEnabled ? 1 : 0, true, false);
			this.telemetryToggle.SetOption(this._player.IsTelemetryEnabled ? 1 : 0, true, false);
			this.holdToDrawToggle.SetOption(this._player.IsTapDrawEnabled ? 0 : 1, true, false);
			this.fullscreenToggle.SetOption(Screen.fullScreen ? 1 : 0, true, false);
			this.volumeOptions.SetOption(this._player.VolumeSetting, false);
			this.soundscapeOptions.SetOption(this._player.Soundscape, false);
			this.zoomToggle.SetOption(this._player.IsZoomEnabled ? 1 : 0, true, false);
			this.zoomLevelOptions.SetOption(this._player.ZoomLevel, false);
			if (!Screen.fullScreen)
			{
				this._player.SelectedDisplay = MultiDisplayCapabilitiesBridge.GetActiveDisplayIndex();
			}
			this.displaySelectionOptions.SetOption(this._player.SelectedDisplay, false);
			for (int displayIndex = 0; displayIndex < this.displaySelectionOptions.NumberOfOptions; displayIndex++)
			{
				if (displayIndex >= this._hardwareCapabilities.DisplayCount)
				{
					this.displaySelectionOptions.SkipOption(displayIndex);
				}
				else
				{
					this.displaySelectionOptions.UnskipOption(displayIndex);
				}
			}
			IAudioSystem audioSystem = this._appScope.Get<IAudioSystem>();
			this.VolumeControls.gameObject.SetActive(audioSystem.RequiresVolumeControl);
			IPersistentStorageService persistentStorageService = this._appScope.Get<IPersistentStorageService>();
			this.iCloudButton.gameObject.SetActive(persistentStorageService.RequiresOptionsPanel && this is OptionsScreenMain);
			this.crossSaveButton.gameObject.SetActive(this._cloudSyncService.IsSupported && this is OptionsScreenMain);
			if (this._systemNotificationService.RequiresOptionsPanel && this is OptionsScreenMain)
			{
				this.messagesButton.gameObject.SetActive(true);
				this.menuMessagesButton.SetOption(this._player.AreMenuMessagesEnabled ? 1 : 0);
				this.challengeRemindersButton.SetOption(this._player.IsChallengeRemindersEnabledSetting ? 1 : 0);
				this.contentRemindersButton.SetOption(this._player.IsContentRemindersEnabledSetting ? 1 : 0);
				this.notificationsStatusText.SetStringId(this._appScope, this.SystemNotificationsAuthorized ? StringId.OptionsNotificationsAreEnabled : StringId.OptionsNotificationsAreDisabled);
				this.notificationsStatusText.transform.parent.parent.gameObject.SetActive(this._systemNotificationService.IsAvailable);
				this.challengeRemindersButton.gameObject.SetActive(this._systemNotificationService.IsAvailable && this.SystemNotificationsAuthorized);
				this.contentRemindersButton.gameObject.SetActive(this._systemNotificationService.IsAvailable && this.SystemNotificationsAuthorized);
				this.enableNotificationsButton.gameObject.SetActive(this._systemNotificationService.IsAvailable && !this.SystemNotificationsAuthorized);
				return;
			}
			this.messagesButton.gameObject.SetActive(false);
		}

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x0600251B RID: 9499 RVA: 0x0009C379 File Offset: 0x0009A579
		private bool SystemNotificationsAuthorized
		{
			get
			{
				return this._systemNotificationService.AuthorizationStatus == AuthorizationStatus.Authorized;
			}
		}

		// Token: 0x0600251C RID: 9500 RVA: 0x0009C389 File Offset: 0x0009A589
		private void OnInternetConnectivityChanged(InternetConnectivity connectivity)
		{
			SymbolOptionButton symbolOptionButton = this.onlineIndicator;
			if (symbolOptionButton != null)
			{
				symbolOptionButton.SetOption((connectivity == InternetConnectivity.Connected) ? 1 : 0, true, false);
			}
			this._hasNewiCloudMessage = true;
		}

		// Token: 0x0600251D RID: 9501 RVA: 0x0009C3B0 File Offset: 0x0009A5B0
		private void OnStorageStatusChanged(PersistentStorageServiceStatus status)
		{
			OptionsScreenBase.Log.Info("Updating storage status to show issues {0} and message {1}.", new object[]
			{
				status.issues,
				status.messageKey
			});
			bool signedInToiCloud = (status.issues & PersistentStorageServiceIssues.NotAuthenticated) != PersistentStorageServiceIssues.NotAuthenticated;
			bool syncedWithiCloud = (status.issues & PersistentStorageServiceIssues.NotAvailable) != PersistentStorageServiceIssues.NotAvailable;
			if (this._reachability.Connectivity == InternetConnectivity.Disconnected || this._reachability.Connectivity == InternetConnectivity.Unknown)
			{
				signedInToiCloud = false;
				syncedWithiCloud = false;
			}
			SymbolOptionButton symbolOptionButton = this.signedInToiCloudIndicator;
			if (symbolOptionButton != null)
			{
				symbolOptionButton.SetOption(signedInToiCloud ? 1 : 0, true, false);
			}
			SymbolOptionButton symbolOptionButton2 = this.syncedWithiCloudIndicator;
			if (symbolOptionButton2 != null)
			{
				symbolOptionButton2.SetOption((signedInToiCloud && syncedWithiCloud) ? 1 : 0, true, false);
			}
			if (this._faqButton != null)
			{
				bool signedOutWithCachediCloudData = (this._storage.Status.issues & PersistentStorageServiceIssues.RecentUnauthenticatedData) > PersistentStorageServiceIssues.None;
				bool storageFull = (this._storage.Status.issues & PersistentStorageServiceIssues.QuotaExceeded) > PersistentStorageServiceIssues.None;
				GameObject gameObject = this._faqButton.gameObject;
				if (gameObject != null)
				{
					gameObject.SetActive(signedOutWithCachediCloudData || storageFull);
				}
			}
			if (this.iCloudStatusMessage != null)
			{
				this.SetiCloudMessage(this._iCloudMessageKey);
			}
			this._iCloudMessageKey = status.messageKey;
			this._hasNewiCloudMessage = true;
		}

		// Token: 0x0600251E RID: 9502 RVA: 0x0009C4E0 File Offset: 0x0009A6E0
		private void SetiCloudMessage(string messageStringKey)
		{
			if (!string.IsNullOrEmpty(messageStringKey))
			{
				StringKey newStringKey = this._appScope.Get<StringKey>();
				newStringKey.InitWithString(messageStringKey);
				this.iCloudStatusMessage.LocString = StandaloneLocString.CreateString(this._appScope, newStringKey);
				this.iCloudStatusMessage.gameObject.SetActive(true);
				return;
			}
			if (this._reachability.Connectivity == InternetConnectivity.Disconnected)
			{
				this.iCloudStatusMessage.LocString = StandaloneLocString.CreateString(this._appScope, StringId.iCloudNotConnectedToInternet);
				this.iCloudStatusMessage.gameObject.SetActive(true);
				return;
			}
			this.iCloudStatusMessage.LocString = StandaloneLocString.CreateNonLocalizedString(this._appScope, "");
			this.iCloudStatusMessage.gameObject.SetActive(false);
		}

		// Token: 0x0600251F RID: 9503 RVA: 0x0009C598 File Offset: 0x0009A798
		public Selectable GetButtonForActiveLanguage()
		{
			if (Diagnostics.Verify(this.languageButtons != null && this.languageButtons.Count != 0, "Language buttons not set up when trying to transition into the language screen!"))
			{
				Locale locale = this._locales.CurrentLocale;
				int localeIndex = this._locales.GetIndex(locale);
				foreach (LanguageButton languageButton in this.languageButtons)
				{
					if (languageButton.LocaleIndex == localeIndex)
					{
						return languageButton.GetComponent<Selectable>();
					}
				}
			}
			return null;
		}

		// Token: 0x06002520 RID: 9504 RVA: 0x0009C63C File Offset: 0x0009A83C
		public void SetLocale(int index)
		{
			this._player.LocaleId = this._locales.GetLocale(index).Id;
			Get.State &= ~StateType.MenuLanguage;
			base.StartCoroutine(this.UpdateMaxLengthButtons());
		}

		// Token: 0x06002521 RID: 9505 RVA: 0x0009C677 File Offset: 0x0009A877
		private IEnumerator UpdateMaxLengthButtons()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.crossSaveButton.GetComponent<RectTransform>());
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.importSaveButton.GetComponent<RectTransform>());
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.creditsButton.GetComponent<RectTransform>());
			yield break;
		}

		// Token: 0x06002522 RID: 9506 RVA: 0x0009C688 File Offset: 0x0009A888
		private void SetupButtons()
		{
			if (this.languageButtons == null)
			{
				this.languageButtons = new List<LanguageButton>();
			}
			else
			{
				this.languageButtons.Clear();
			}
			ToggleButtonGroup group = this.languagePanel.GetComponent<ToggleButtonGroup>();
			LocaleDatabase.LocaleId currentLocaleId = this._player.LocaleId;
			this.firstLanguageButton = null;
			for (int localeIndex = 0; localeIndex < this._locales.LocaleCount; localeIndex++)
			{
				LanguageButton newButton = UnityEngine.Object.Instantiate<LanguageButton>(this.localeButtonPrefab);
				Locale locale = this._locales.GetLocale(localeIndex);
				newButton.Initialize(locale, localeIndex, this._fontDatabase, this, group, currentLocaleId == locale.Id);
				newButton.transform.SetParent(this.languagePanel);
				newButton.transform.localScale = Vector3.one;
				this.firstLanguageButton = ((this.firstLanguageButton != null) ? this.firstLanguageButton : newButton.GetComponent<Selectable>());
				this.languageButtons.Add(newButton);
			}
			group.EnsureValidState();
		}

		// Token: 0x06002523 RID: 9507 RVA: 0x0009C77C File Offset: 0x0009A97C
		public void OnTutorial()
		{
			if (this._tutorialCityDefinition.HasValue)
			{
				this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.MenuExit, 0.5f, -1f, true, null));
				if (this._skipTransitions)
				{
					this._screenStack.FadeNextTransition(this.skippedTransitionFadeDuration);
				}
				this._enterTutorialNextTick = true;
			}
		}

		// Token: 0x06002524 RID: 9508 RVA: 0x0009C7E4 File Offset: 0x0009A9E4
		public void OnImportSteamSaveDataButtonPressed()
		{
			this.popupStack.PushPopup<CrossSavePopup>(0f, false).StartSteamSync();
		}

		// Token: 0x06002525 RID: 9509 RVA: 0x0009C7FC File Offset: 0x0009A9FC
		public void OnCrossSaveHelp()
		{
			this.popupStack.PushPopup<GenericPopup>(0f, false).Initialise(StringId.Options_CrossSave, new StringId[]
			{
				StringId.CrossSave_Explanation_1,
				StringId.CrossSave_Explanation_2
			});
		}

		// Token: 0x06002526 RID: 9510 RVA: 0x0009C82F File Offset: 0x0009AA2F
		private void UpdateFocusBeforeModalScreen()
		{
			MenuNavigation menuNavigation = this._appScope.Get<MenuNavigation>();
			this._focusBeforeModalScreen = ((menuNavigation != null) ? menuNavigation.GetCurrentFocus() : null);
		}

		// Token: 0x06002527 RID: 9511 RVA: 0x0009C84E File Offset: 0x0009AA4E
		private void UpdateFocusAfterModalScreen()
		{
			if (this._appScope.Get<InputState>().CurrentInputTypeRequiresFocus)
			{
				MenuNavigation menuNavigation = this._appScope.Get<MenuNavigation>();
				if (menuNavigation == null)
				{
					return;
				}
				menuNavigation.SetNewFocus(this._focusBeforeModalScreen);
			}
		}

		// Token: 0x06002528 RID: 9512 RVA: 0x0009C87D File Offset: 0x0009AA7D
		public override void Reset()
		{
			base.Reset();
			this._controllerSymbols = null;
		}

		// Token: 0x04001EF4 RID: 7924
		public MapDefinition tutorialDefinition;

		// Token: 0x04001EF5 RID: 7925
		private AssetBundleUtility.AsyncLoadResult _tutorialCityDefinition;

		// Token: 0x04001EF6 RID: 7926
		public SymbolOptionButton onlineIndicator;

		// Token: 0x04001EF7 RID: 7927
		public SymbolOptionButton signedInToiCloudIndicator;

		// Token: 0x04001EF8 RID: 7928
		public SymbolOptionButton syncedWithiCloudIndicator;

		// Token: 0x04001EF9 RID: 7929
		public LocalizedTextUI iCloudStatusMessage;

		// Token: 0x04001EFA RID: 7930
		public SymbolOptionButton fullscreenToggle;

		// Token: 0x04001EFB RID: 7931
		public SymbolOptionButton nightModeToggle;

		// Token: 0x04001EFC RID: 7932
		public SymbolOptionButton colorblindModeToggle;

		// Token: 0x04001EFD RID: 7933
		public SymbolOptionButton skipTransitionsToggle;

		// Token: 0x04001EFE RID: 7934
		public TouchOptionButton antiAliasingLevelOptions;

		// Token: 0x04001EFF RID: 7935
		public SymbolOptionButton vibrationsToggle;

		// Token: 0x04001F00 RID: 7936
		public SymbolOptionButton drawModeToggleToggle;

		// Token: 0x04001F01 RID: 7937
		public SymbolOptionButton telemetryToggle;

		// Token: 0x04001F02 RID: 7938
		[FormerlySerializedAs("tapDrawToggle")]
		public SymbolOptionButton holdToDrawToggle;

		// Token: 0x04001F03 RID: 7939
		public TouchOptionButton volumeOptions;

		// Token: 0x04001F04 RID: 7940
		public TouchOptionButton soundscapeOptions;

		// Token: 0x04001F05 RID: 7941
		public TouchOptionButton controllerSensitivityOptions;

		// Token: 0x04001F06 RID: 7942
		public SymbolOptionButton zoomToggle;

		// Token: 0x04001F07 RID: 7943
		private TouchButton zoomToggleTouchButton;

		// Token: 0x04001F08 RID: 7944
		public TouchOptionButton zoomLevelOptions;

		// Token: 0x04001F09 RID: 7945
		public ColorblindCustomisationPanel colorblindCustomisationPanel;

		// Token: 0x04001F0A RID: 7946
		public DropdownBox resolutionsDropdown;

		// Token: 0x04001F0B RID: 7947
		public TouchOptionButton displaySelectionOptions;

		// Token: 0x04001F0C RID: 7948
		public PaginatedScrollView optionsPages;

		// Token: 0x04001F0D RID: 7949
		public LocalizedTextUI versionString;

		// Token: 0x04001F0E RID: 7950
		[Dependency]
		private IHardwareCapabilities _hardwareCapabilities;

		// Token: 0x04001F0F RID: 7951
		[Dependency]
		private ISoftwareCapabilities _softwareCapabilities;

		// Token: 0x04001F10 RID: 7952
		[Dependency]
		private IPersistentStorageService _storage;

		// Token: 0x04001F11 RID: 7953
		[Dependency]
		private LocaleDatabase _locales;

		// Token: 0x04001F12 RID: 7954
		[Dependency]
		private FontDatabase _fontDatabase;

		// Token: 0x04001F13 RID: 7955
		[Dependency]
		private ISystemNotificationService _systemNotificationService;

		// Token: 0x04001F14 RID: 7956
		[Dependency]
		private NotificationScheduler _notificationScheduler;

		// Token: 0x04001F15 RID: 7957
		[Dependency]
		private INotificationEventSystem _notificationEventSystem;

		// Token: 0x04001F16 RID: 7958
		[Dependency]
		private IControllerButtonToSymbolService _controllerButtonToSymbolService;

		// Token: 0x04001F17 RID: 7959
		[Dependency]
		private ISteamCloudSyncService _cloudSyncService;

		// Token: 0x04001F18 RID: 7960
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x04001F19 RID: 7961
		public LanguageButton localeButtonPrefab;

		// Token: 0x04001F1A RID: 7962
		private Selectable firstLanguageButton;

		// Token: 0x04001F1B RID: 7963
		public RectTransform languagePanel;

		// Token: 0x04001F1C RID: 7964
		private List<LanguageButton> languageButtons;

		// Token: 0x04001F1D RID: 7965
		public CanvasGroup optionsCanvasGroup;

		// Token: 0x04001F1E RID: 7966
		public CanvasGroup controllerCanvas;

		// Token: 0x04001F1F RID: 7967
		public CanvasGroup siriRemoteCanvas;

		// Token: 0x04001F20 RID: 7968
		public CanvasGroup keyboardCanvas;

		// Token: 0x04001F21 RID: 7969
		public CanvasGroup mouseCanvas;

		// Token: 0x04001F22 RID: 7970
		public CanvasGroup touchCanvas;

		// Token: 0x04001F23 RID: 7971
		public CanvasGroup switchJoyconDualCanvas;

		// Token: 0x04001F24 RID: 7972
		public CanvasGroup switchHandheldCanvas;

		// Token: 0x04001F25 RID: 7973
		public CanvasGroup switchProCanvas;

		// Token: 0x04001F26 RID: 7974
		public CanvasGroup switchJoyconLCanvas;

		// Token: 0x04001F27 RID: 7975
		public CanvasGroup switchJoyconRCanvas;

		// Token: 0x04001F28 RID: 7976
		public ButtonGroup inputMethodButtonGroup;

		// Token: 0x04001F29 RID: 7977
		public TouchButton siriRemoteButton;

		// Token: 0x04001F2A RID: 7978
		public TouchButton keyboardButton;

		// Token: 0x04001F2B RID: 7979
		public TouchButton mouseButton;

		// Token: 0x04001F2C RID: 7980
		public TouchButton touchInputButton;

		// Token: 0x04001F2D RID: 7981
		public TouchButton gamepadInputButton;

		// Token: 0x04001F2E RID: 7982
		public TouchButton switchJoyconDualButton;

		// Token: 0x04001F2F RID: 7983
		public TouchButton switchHandheldButton;

		// Token: 0x04001F30 RID: 7984
		public TouchButton switchProButton;

		// Token: 0x04001F31 RID: 7985
		public TouchButton switchJoyconLButton;

		// Token: 0x04001F32 RID: 7986
		public TouchButton switchJoyconRButton;

		// Token: 0x04001F33 RID: 7987
		private bool _hasNewiCloudMessage;

		// Token: 0x04001F34 RID: 7988
		private string _iCloudMessageKey;

		// Token: 0x04001F35 RID: 7989
		public TouchButton _faqButton;

		// Token: 0x04001F36 RID: 7990
		private Selectable _focusBeforeModalScreen;

		// Token: 0x04001F37 RID: 7991
		public TouchButton resetAchievementButton;

		// Token: 0x04001F38 RID: 7992
		public SymbolOptionButton menuMessagesButton;

		// Token: 0x04001F39 RID: 7993
		public SymbolOptionButton challengeRemindersButton;

		// Token: 0x04001F3A RID: 7994
		public SymbolOptionButton contentRemindersButton;

		// Token: 0x04001F3B RID: 7995
		public LocalizedTextUI notificationsStatusText;

		// Token: 0x04001F3C RID: 7996
		public TouchButton enableNotificationsButton;

		// Token: 0x04001F3D RID: 7997
		public TouchButton clearNotificationEventsButton;

		// Token: 0x04001F3E RID: 7998
		public TouchButton sendTestNotificationButton;

		// Token: 0x04001F3F RID: 7999
		public TouchButton debugPageButton;

		// Token: 0x04001F40 RID: 8000
		public DebugOptionsPage debugOptionsPage;

		// Token: 0x04001F41 RID: 8001
		public TouchButton audioButton;

		// Token: 0x04001F42 RID: 8002
		public TouchButton displayButton;

		// Token: 0x04001F43 RID: 8003
		public TouchButton iCloudButton;

		// Token: 0x04001F44 RID: 8004
		public TouchButton crossSaveButton;

		// Token: 0x04001F45 RID: 8005
		public TouchButton creditsButton;

		// Token: 0x04001F46 RID: 8006
		public TouchButton messagesButton;

		// Token: 0x04001F47 RID: 8007
		public TouchButton privacyButton;

		// Token: 0x04001F48 RID: 8008
		public ButtonGroup tabButtonGroup;

		// Token: 0x04001F49 RID: 8009
		public Transform importSaveButton;

		// Token: 0x04001F4A RID: 8010
		public TouchOptionButton VolumeControls;

		// Token: 0x04001F4B RID: 8011
		public GameObject mfiControllerDiagram;

		// Token: 0x04001F4C RID: 8012
		[MinValue(0)]
		[Tooltip("The duration of the fade to black if Skip Transitions is on")]
		public float skippedTransitionFadeDuration = 1f;

		// Token: 0x04001F4D RID: 8013
		private bool _enterTutorialNextTick;

		// Token: 0x04001F4E RID: 8014
		[Dependency]
		private IReachability _reachability;

		// Token: 0x04001F4F RID: 8015
		private Action OnNotificationAuthorizationRequestComplete;

		// Token: 0x04001F50 RID: 8016
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("OptionsScreen");

		// Token: 0x04001F51 RID: 8017
		private List<Resolution> _displayedResolutions = new List<Resolution>();

		// Token: 0x04001F52 RID: 8018
		private ControllerSymbol[] _controllerSymbols;

		// Token: 0x04001F53 RID: 8019
		[SerializeField]
		private GameObject[] _toolbarLockingControls;

		// Token: 0x04001F54 RID: 8020
		[Tooltip("How long in seconds the options screen should fade in/out when it loses focus")]
		[SerializeField]
		private float _fadeDuration;

		// Token: 0x04001F55 RID: 8021
		private bool _shouldFadeIn;

		// Token: 0x04001F56 RID: 8022
		private bool _shouldFadeOut;
	}
}
