using System;
using System.Collections;
using System.Collections.Generic;
using Client;
using Easing;
using Factory;
using Factory.Pools;
using Motorways;
using Motorways.Audio;
using Motorways.UI;
using Motorways.UI.NewContentIndicators;
using Popups;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Screens
{
	// Token: 0x02000295 RID: 661
	[RequireComponent(typeof(RectTransform))]
	public class BaseScalingScreen : MonoBehaviour, IScreen, InputState.IObserver, MenuNavigation.IObserver, IReusable, ICreatedInScopeHandler, IReleasedFromScopeHandler
	{
		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06001041 RID: 4161 RVA: 0x00036896 File Offset: 0x00034A96
		public ScreenStack.MotorwaysScreen ScreenType
		{
			get
			{
				return this._screenStack.GetScreenEnumForSystemType(base.GetType());
			}
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x000368A9 File Offset: 0x00034AA9
		public virtual void Awake()
		{
			this._rectTransform = base.GetComponent<RectTransform>();
			this._canvas = base.GetComponent<Canvas>();
			this._canvasGroup = base.GetComponent<DelegateCanvasGroup>();
			this._canvasGroup.SetBlocksRaycasts(false);
			this._canvasGroup.SetInteractable(false);
		}

		// Token: 0x06001043 RID: 4163 RVA: 0x000368E8 File Offset: 0x00034AE8
		protected void RegisterAllLocalizedTextChildren()
		{
			this.UnregisterLocalizedTextChildren();
			base.GetComponentsInChildren<LocalizedTextUI>(true, this.allLocalizedText);
			for (int newIndex = 0; newIndex < this.allLocalizedText.Count; newIndex++)
			{
				if (!this.allLocalizedText[newIndex].isInitialized)
				{
					this.allLocalizedText[newIndex].HandleParentAllocated(this._appScope);
				}
				this._localeDatabase.AddLocalizedObject(this.allLocalizedText[newIndex]);
			}
		}

		// Token: 0x06001044 RID: 4164 RVA: 0x00036960 File Offset: 0x00034B60
		public void RegisterAdditionalLocalizedTextChildren(List<LocalizedTextUI> additionalLocalizedTexts)
		{
			this.allLocalizedText.AddRange(additionalLocalizedTexts);
			for (int newIndex = 0; newIndex < additionalLocalizedTexts.Count; newIndex++)
			{
				if (!additionalLocalizedTexts[newIndex].isInitialized)
				{
					additionalLocalizedTexts[newIndex].HandleParentAllocated(this._appScope);
				}
				this._localeDatabase.AddLocalizedObject(additionalLocalizedTexts[newIndex]);
			}
		}

		// Token: 0x06001045 RID: 4165 RVA: 0x000369BC File Offset: 0x00034BBC
		public void RegisterButtons()
		{
			base.GetComponentsInChildren<VariableDeviceSelectable>(true, this._allButtons);
			for (int buttonIndex = 0; buttonIndex < this._allButtons.Count; buttonIndex++)
			{
				if (!this._allButtons[buttonIndex].IsInitialized)
				{
					this._allButtons[buttonIndex].Initialize(this._appScope);
				}
			}
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x00036A18 File Offset: 0x00034C18
		public void RegisterAdditionalButtons(List<VariableDeviceSelectable> additionalButtons)
		{
			this._allButtons.AddRange(additionalButtons);
			for (int buttonIndex = 0; buttonIndex < additionalButtons.Count; buttonIndex++)
			{
				if (!additionalButtons[buttonIndex].IsInitialized)
				{
					additionalButtons[buttonIndex].Initialize(this._appScope);
				}
			}
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x00036A64 File Offset: 0x00034C64
		public virtual void RegisterThemeComponents(ITheme theme)
		{
			this.UnregisterThemeComponents();
			this.GetAutoThemeComponents(this.themeComponents);
			if (this.themeComponents != null)
			{
				foreach (IThemeComponent themeComponent in this.themeComponents)
				{
					themeComponent.InitializeTheme(this._themeDatabase);
				}
			}
			MotorwaysThemeDatabase.Log.Info("Registering theme components for screen: {0}", new object[]
			{
				base.gameObject.name
			});
			if (theme != null)
			{
				this.ApplyTheme(theme);
			}
		}

		// Token: 0x06001048 RID: 4168 RVA: 0x00036B04 File Offset: 0x00034D04
		protected virtual void GetAutoThemeComponents(List<IThemeComponent> components)
		{
			base.GetComponentsInChildren<IThemeComponent>(true, components);
		}

		// Token: 0x06001049 RID: 4169 RVA: 0x00036B10 File Offset: 0x00034D10
		protected void UnregisterLocalizedTextChildren()
		{
			foreach (LocalizedTextUI localizedText in this.allLocalizedText)
			{
				localizedText.Unregister();
				this._localeDatabase.RemoveLocalizedObject(localizedText);
			}
			this.allLocalizedText.Clear();
		}

		// Token: 0x0600104A RID: 4170 RVA: 0x00036B7C File Offset: 0x00034D7C
		protected void UnregisterButtons()
		{
			foreach (VariableDeviceSelectable variableDeviceSelectable in this._allButtons)
			{
				variableDeviceSelectable.Unregister();
			}
			this._allButtons.Clear();
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x00036BD8 File Offset: 0x00034DD8
		protected virtual void UnregisterThemeComponents()
		{
			foreach (IThemeComponent themeComponent in this.themeComponents)
			{
				themeComponent.ReleaseTheme(this._themeDatabase);
			}
			this.themeComponents.Clear();
		}

		// Token: 0x0600104C RID: 4172 RVA: 0x00036C3C File Offset: 0x00034E3C
		public void RegisterAdditionalThemeComponents(List<IThemeComponent> additionalThemeComponents)
		{
			this._additionalThemeComponents.AddRange(additionalThemeComponents);
			ITheme theme = this._themeDatabase.GetTheme();
			foreach (IThemeComponent themeComponent in additionalThemeComponents)
			{
				themeComponent.InitializeTheme(this._themeDatabase);
				if (theme != null)
				{
					themeComponent.ApplyTheme(theme);
				}
			}
		}

		// Token: 0x0600104D RID: 4173 RVA: 0x00036CB4 File Offset: 0x00034EB4
		public void UnregisterAdditionalThemeComponents(List<IThemeComponent> additionalThemeComponents)
		{
			foreach (IThemeComponent themeComponent in additionalThemeComponents)
			{
				this._additionalThemeComponents.Remove(themeComponent);
				themeComponent.ReleaseTheme(this._themeDatabase);
			}
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x00036D14 File Offset: 0x00034F14
		public virtual void ApplyTheme(ITheme newTheme)
		{
			if (newTheme != null)
			{
				for (int themeComponentIndex = 0; themeComponentIndex < this.themeComponents.Count; themeComponentIndex++)
				{
					if (ObjectUtils.IsNullOrDestroyed<IThemeComponent>(this.themeComponents[themeComponentIndex]))
					{
						this.themeComponents.RemoveAt(themeComponentIndex);
						themeComponentIndex--;
					}
					else
					{
						this.themeComponents[themeComponentIndex].ApplyTheme(newTheme);
					}
				}
				for (int additionalThemeIndex = 0; additionalThemeIndex < this._additionalThemeComponents.Count; additionalThemeIndex++)
				{
					if (ObjectUtils.IsNullOrDestroyed<IThemeComponent>(this._additionalThemeComponents[additionalThemeIndex]))
					{
						this._additionalThemeComponents.RemoveAt(additionalThemeIndex);
						additionalThemeIndex--;
					}
					else
					{
						this._additionalThemeComponents[additionalThemeIndex].ApplyTheme(newTheme);
					}
				}
				return;
			}
			MotorwaysThemeDatabase.Log.Warn("Trying to apply a null theme to screen {0}", new object[]
			{
				base.gameObject.name
			});
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x00036DE8 File Offset: 0x00034FE8
		public virtual void ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			if (newTheme != null && oldTheme != null)
			{
				if (this._lastThemeBlendedFrom != oldTheme || this._lastThemeBlendedTo != newTheme)
				{
					this._lastThemeBlendedFrom = oldTheme;
					this._lastThemeBlendedTo = newTheme;
					this._dynamicThemeComponents.Clear();
					for (int themeComponentIndex = 0; themeComponentIndex < this.themeComponents.Count; themeComponentIndex++)
					{
						IThemeComponent component = this.themeComponents[themeComponentIndex];
						if (ObjectUtils.IsNullOrDestroyed<IThemeComponent>(component))
						{
							this.themeComponents.RemoveAt(themeComponentIndex);
							themeComponentIndex--;
						}
						else if (component.ApplyBlendedTheme(oldTheme, newTheme, progress) == ThemeBlendingResult.ContinueBlending)
						{
							this._dynamicThemeComponents.Add(component);
						}
					}
				}
				else
				{
					for (int themeComponentIndex2 = 0; themeComponentIndex2 < this._dynamicThemeComponents.Count; themeComponentIndex2++)
					{
						IThemeComponent component2 = this._dynamicThemeComponents[themeComponentIndex2];
						if (ObjectUtils.IsNullOrDestroyed<IThemeComponent>(component2))
						{
							this._dynamicThemeComponents.RemoveAt(themeComponentIndex2);
							themeComponentIndex2--;
						}
						else
						{
							component2.ApplyBlendedTheme(oldTheme, newTheme, progress);
						}
					}
				}
				for (int additionalThemeIndex = 0; additionalThemeIndex < this._additionalThemeComponents.Count; additionalThemeIndex++)
				{
					if (ObjectUtils.IsNullOrDestroyed<IThemeComponent>(this._additionalThemeComponents[additionalThemeIndex]))
					{
						this._additionalThemeComponents.RemoveAt(additionalThemeIndex);
						additionalThemeIndex--;
					}
					else
					{
						this._additionalThemeComponents[additionalThemeIndex].ApplyBlendedTheme(oldTheme, newTheme, progress);
					}
				}
				return;
			}
			MotorwaysThemeDatabase.Log.Warn("Trying to apply a null theme to screen " + base.gameObject.name, Array.Empty<object>());
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x00036F48 File Offset: 0x00035148
		public virtual void Tick(float deltaTime)
		{
			if (this.IsTransitioningIn())
			{
				this._transitionInPercentage = Mathf.Clamp01(this._transitionInPercentage + this.TransitionInPercentageChange(deltaTime));
				this.TransitionInTick();
			}
			if (this.IsTransitioningOut())
			{
				this._transitionOutPercentage = Mathf.Clamp01(this._transitionOutPercentage + this.TransitionOutPercentageChange(deltaTime));
				this.TransitionOutTick();
			}
			if (this._scaleToCamera)
			{
				this.ScaleToCamera();
			}
			if (this._alignToCamera)
			{
				Bounds cameraBounds = this._gameCamera.GetScreenBounds(-1f);
				this._rectTransform.position = new Vector3(cameraBounds.center.x, cameraBounds.center.y, this._rectTransform.position.z);
			}
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x00037004 File Offset: 0x00035204
		public virtual void ScaleToCamera()
		{
			Bounds cameraBounds = this._gameCamera.GetScreenBounds(BaseScalingScreen.referenceAspectRatio);
			float largerAspectRatio = Mathf.Max(BaseScalingScreen.referenceAspectRatio, this._gameCamera.AspectRatio);
			float smallerAspectRatio = Mathf.Min(BaseScalingScreen.referenceAspectRatio, this._gameCamera.AspectRatio);
			Vector2 newScale = (cameraBounds.max - cameraBounds.min) / BaseScalingScreen.referenceResolution * (smallerAspectRatio / largerAspectRatio);
			this._rectTransform.localScale = newScale;
			float scaledHeight = BaseScalingScreen.referenceResolution.y * (largerAspectRatio / smallerAspectRatio);
			this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledHeight);
			this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledHeight * this._gameCamera.AspectRatio);
		}

		// Token: 0x06001052 RID: 4178 RVA: 0x000370C4 File Offset: 0x000352C4
		protected void ScaleToGameCamera()
		{
			Bounds cameraBounds = this._gameCamera.GetScreenBounds(-1f);
			Vector2 newScale = (cameraBounds.max - cameraBounds.min) / (this._rectTransform.offsetMax - this._rectTransform.offsetMin);
			this._rectTransform.localScale = newScale;
			this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this._gameCamera.Width);
			this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this._gameCamera.Height);
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x0003715C File Offset: 0x0003535C
		public virtual void Enable(bool shouldBeVisible)
		{
			ScreenStack.Log.Info(base.gameObject, shouldBeVisible ? "Enabling a {0} screen." : "Disabling a {0} screen.", new object[]
			{
				base.GetType()
			});
			base.gameObject.SetActive(shouldBeVisible);
			if (this._scaleToCamera)
			{
				this.ScaleToCamera();
			}
		}

		// Token: 0x06001054 RID: 4180 RVA: 0x000371B4 File Offset: 0x000353B4
		public virtual void TransitionInTick()
		{
			float lerp = Easings.CubicEaseInOut(this.TransitionInPercentage());
			if (this._transitionDetails.cameraControl.Contains(TransitionCameraControl.Position))
			{
				Vector3 newPosition = this._transitionDetails.spline.Evaluate(lerp);
				this._gameCamera.SetPosition(newPosition);
			}
			if (this._transitionDetails.cameraControl.Contains(TransitionCameraControl.Rotation))
			{
				this._gameCamera.transform.rotation = this._transitionDetails.spline.EvaluateRotation(lerp);
			}
			if (this._transitionDetails.cameraControl.Contains(TransitionCameraControl.Scale))
			{
				this._gameCamera.OrthographicSize = Mathf.Lerp(this._previousCameraZoom, this._screenStack.GetZoomFor(this.ScreenType), Easings.SineEaseInOut(lerp));
			}
		}

		// Token: 0x06001055 RID: 4181 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void TransitionOutTick()
		{
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x00037278 File Offset: 0x00035478
		public virtual void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			base.gameObject.SetActive(true);
			this._previousCameraPosition = this._gameCamera.transform.position;
			this._previousCameraRotation = this._gameCamera.transform.rotation;
			this._previousCameraZoom = this._gameCamera.OrthographicSize;
			ScreenStack.Log.Info(base.gameObject, "Starting a transition into {0} screen.", new object[]
			{
				base.GetType()
			});
			this._transitionInPercentage = 0f;
			this._transitionOutPercentage = -1f;
			this._appScope.Get<IInputState>().BlockAllInput = true;
			this._canvasGroup.SetBlocksRaycasts(false);
			this._canvasGroup.SetInteractable(true);
			this._transitionDetails = this._screenStack.GetTransitionDetailsFrom(outScreen, this.ScreenType);
			this._transitionDuration = this._transitionDetails.duration;
			if (outScreen == ScreenStack.MotorwaysScreen.None)
			{
				this._overrideNextTransitionDuration = 0f;
			}
			base.transform.rotation = this._screenStack.GetRotationFor(this.ScreenType);
			Vector3 newPosition = this._screenStack.GetPositionFor(this.ScreenType);
			newPosition.z = base.transform.position.z;
			base.transform.position = newPosition;
			this._skipTransitions = (this._player.HasActivePlayer && this._player.IsSkipTransitionsEnabled);
			bool cameraMoves = !this._skipTransitions;
			switch (this.ScreenType)
			{
			case ScreenStack.MotorwaysScreen.MainMenu:
				Get.State |= StateType.MenuMain;
				break;
			case ScreenStack.MotorwaysScreen.InGame:
				Get.State |= StateType.GameActive;
				break;
			case ScreenStack.MotorwaysScreen.Pause:
			case ScreenStack.MotorwaysScreen.ChallengeInfo:
				Get.State |= StateType.MenuPause;
				break;
			case ScreenStack.MotorwaysScreen.GameOver:
				Get.State |= StateType.GameOver;
				cameraMoves = false;
				this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.GameOver, 0.5f, -1f, true, null));
				break;
			case ScreenStack.MotorwaysScreen.Upgrade:
				Get.State |= StateType.MenuUpgrades;
				cameraMoves = false;
				break;
			case ScreenStack.MotorwaysScreen.OptionsMain:
				Get.State |= StateType.MenuOptions;
				break;
			case ScreenStack.MotorwaysScreen.MapSelect:
				Get.State |= StateType.MenuMapSelect;
				break;
			case ScreenStack.MotorwaysScreen.Credits:
				Get.State |= StateType.Credits;
				break;
			case ScreenStack.MotorwaysScreen.ResumeGame:
				Get.State |= StateType.MenuResume;
				break;
			case ScreenStack.MotorwaysScreen.Startup:
				cameraMoves = false;
				break;
			case ScreenStack.MotorwaysScreen.Photo:
			case ScreenStack.MotorwaysScreen.Movie:
				Get.State |= StateType.MenuPhoto;
				cameraMoves = false;
				if (outScreen == ScreenStack.MotorwaysScreen.GameOver)
				{
					Get.State |= StateType.GameOver;
				}
				break;
			}
			ScreenStack.MotorwaysScreen screenType = this.ScreenType;
			if ((screenType == ScreenStack.MotorwaysScreen.MainMenu || screenType - ScreenStack.MotorwaysScreen.OptionsMain <= 3) && Get.Loadout != null && Get.Loadout.MusicData != null)
			{
				AudioSample bass = Get.Loadout.MusicData.Bass;
				if (bass != null)
				{
					bass.FadeOutAndStop(0.5);
				}
				MusicData musicData = Get.Loadout.MusicData;
				AudioPlayer @default = AudioPlayer.Default;
				musicData.Bass = ((@default != null) ? @default.PlaySample("bass_" + Note.SCALE[Get.Loadout.MusicData.CurrentScale.Key], 0.5f, 0.5f, Get.State.HasFlag(StateType.ModeNight) ? -0.5f : 1f, 0.5, -1.0, false, null, false, false, 0f, false) : null);
				int commonTones = Rando.Range(2, 5, -1);
				if (outScreen == ScreenStack.MotorwaysScreen.Startup)
				{
					commonTones = Rando.Pick<int>(new int[]
					{
						0,
						1
					});
				}
				Get.Loadout.MusicData.UpdateNoteWindow(commonTones, 1f, 0, 0f, false);
				if (outScreen != ScreenStack.MotorwaysScreen.Startup)
				{
					Get.Mixbus.BoingPitchInPlace(Rando.Range(0.5f, 1.5f, -1), Rando.Range(4f, 12f, -1), Settings.PITCH_BOING_IN_PLACE.Random(-1), Rando.Pick<float>(new float[]
					{
						0f,
						0.5f
					}));
				}
			}
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.Transition, UIAudioProfile.None, this.GetTransitionDuration(), cameraMoves, null, this.ScreenType, outScreen));
			ITheme currentTheme = this._themeDatabase.GetTheme();
			if (currentTheme != null)
			{
				this.ApplyTheme(currentTheme);
			}
			this._analytics.TrackScreenEntered(this.ScreenType);
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x000376E4 File Offset: 0x000358E4
		public virtual void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			ScreenStack.Log.Info(base.gameObject, "Starting a transition out from {0} screen.", new object[]
			{
				base.GetType()
			});
			this._transitionDetails = this._screenStack.GetTransitionDetailsFrom(this.ScreenType, inScreen);
			this._transitionDuration = this._transitionDetails.duration;
			this._transitionOutPercentage = 0f;
			this._transitionInPercentage = -1f;
			this._canvasGroup.SetBlocksRaycasts(false);
			this._canvasGroup.SetInteractable(false);
			switch (this.ScreenType)
			{
			case ScreenStack.MotorwaysScreen.MainMenu:
				Get.State &= ~StateType.MenuMain;
				break;
			case ScreenStack.MotorwaysScreen.InGame:
				Get.State &= ~StateType.GameActive;
				break;
			case ScreenStack.MotorwaysScreen.Pause:
			case ScreenStack.MotorwaysScreen.ChallengeInfo:
				Get.State &= ~StateType.MenuPause;
				break;
			case ScreenStack.MotorwaysScreen.GameOver:
				Get.State &= ~StateType.GameOver;
				break;
			case ScreenStack.MotorwaysScreen.Upgrade:
				Get.State &= ~StateType.MenuUpgrades;
				break;
			case ScreenStack.MotorwaysScreen.OptionsMain:
				Get.State &= ~StateType.MenuOptions;
				break;
			case ScreenStack.MotorwaysScreen.MapSelect:
				Get.State &= ~StateType.MenuMapSelect;
				break;
			case ScreenStack.MotorwaysScreen.Credits:
				Get.State &= ~StateType.Credits;
				break;
			case ScreenStack.MotorwaysScreen.ResumeGame:
				Get.State &= ~StateType.MenuResume;
				break;
			case ScreenStack.MotorwaysScreen.Photo:
			case ScreenStack.MotorwaysScreen.Movie:
				Get.State &= ~StateType.MenuPhoto;
				break;
			}
			this._skipTransitions = this._player.IsSkipTransitionsEnabled;
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x00037878 File Offset: 0x00035A78
		public float TransitionInPercentageChange(float deltaTime)
		{
			float transitionDuration = this.GetTransitionDuration();
			if (transitionDuration <= 1E-45f)
			{
				return 1.1f;
			}
			return deltaTime * (1f / transitionDuration);
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x000378A4 File Offset: 0x00035AA4
		public float TransitionOutPercentageChange(float deltaTime)
		{
			float transitionDuration = this.GetTransitionDuration();
			if (transitionDuration <= 1E-45f)
			{
				return 1.1f;
			}
			return deltaTime * (1f / transitionDuration);
		}

		// Token: 0x0600105A RID: 4186 RVA: 0x000378CF File Offset: 0x00035ACF
		public virtual float GetTransitionDuration()
		{
			if (this._overrideNextTransitionDuration > 0f)
			{
				return this._overrideNextTransitionDuration;
			}
			if (this._skipTransitions || Mathf.Abs(this._overrideNextTransitionDuration) < 1E-45f)
			{
				return 0f;
			}
			return this._transitionDuration;
		}

		// Token: 0x0600105B RID: 4187 RVA: 0x0003790B File Offset: 0x00035B0B
		public virtual float TransitionInPercentage()
		{
			return this._transitionInPercentage;
		}

		// Token: 0x0600105C RID: 4188 RVA: 0x00037913 File Offset: 0x00035B13
		public virtual float TransitionOutPercentage()
		{
			return this._transitionOutPercentage;
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x0003791B File Offset: 0x00035B1B
		public virtual bool IsTransitioningIn()
		{
			return this._transitionInPercentage >= 0f && this._transitionInPercentage < 1f;
		}

		// Token: 0x0600105E RID: 4190 RVA: 0x00037939 File Offset: 0x00035B39
		public virtual bool IsTransitioningOut()
		{
			return this._transitionOutPercentage >= 0f && this._transitionOutPercentage < 1f;
		}

		// Token: 0x0600105F RID: 4191 RVA: 0x00037957 File Offset: 0x00035B57
		public virtual void OnTransitionedIn()
		{
			this._overrideNextTransitionDuration = -1f;
			this._appScope.Get<IInputState>().BlockAllInput = false;
			this.ShowNewContentIndicators();
			this.OnGainedFocus();
		}

		// Token: 0x06001060 RID: 4192 RVA: 0x00037984 File Offset: 0x00035B84
		protected void ShowNewContentIndicators()
		{
			List<VariableDeviceSelectable> newContentItems = new List<VariableDeviceSelectable>();
			List<VariableDeviceSelectable> newContentContainers = new List<VariableDeviceSelectable>();
			List<VariableDeviceSelectable> newContentWithIntro = new List<VariableDeviceSelectable>();
			List<VariableDeviceSelectable> newContentInIdle = new List<VariableDeviceSelectable>();
			HashSet<string> introducedContentIds = new HashSet<string>();
			foreach (VariableDeviceSelectable button in this._allButtons)
			{
				if (button.gameObject.activeInHierarchy)
				{
					if (button.IsNewContentItem(this._appScope))
					{
						newContentItems.Add(button);
					}
					else if (button.IsNewContentContainer(this._appScope))
					{
						newContentContainers.Add(button);
					}
				}
			}
			foreach (VariableDeviceSelectable newContentContainer in newContentContainers)
			{
				if (!introducedContentIds.Contains(newContentContainer.NewContentId))
				{
					newContentWithIntro.Add(newContentContainer);
					introducedContentIds.UnionWith(newContentContainer.ContainedNewContentIds);
				}
				else
				{
					newContentInIdle.Add(newContentContainer);
				}
			}
			foreach (VariableDeviceSelectable newContentItem in newContentItems)
			{
				if (!introducedContentIds.Contains(newContentItem.NewContentId))
				{
					newContentWithIntro.Add(newContentItem);
				}
				else
				{
					newContentInIdle.Add(newContentItem);
				}
			}
			foreach (VariableDeviceSelectable newContentItem2 in newContentInIdle)
			{
				if (!newContentItem2.IsManuallyTriggered)
				{
					newContentItem2.ShowNewContentIndicatorIfNeeded(false);
				}
			}
			if (newContentWithIntro.Count > 0 && base.gameObject.activeInHierarchy)
			{
				base.StartCoroutine(this.ShowNewContentIndicatorIntrosIfNeeded(newContentWithIntro));
			}
		}

		// Token: 0x06001061 RID: 4193 RVA: 0x00037B60 File Offset: 0x00035D60
		private IEnumerator ShowNewContentIndicatorIntrosIfNeeded(List<VariableDeviceSelectable> newContentWithIntro)
		{
			NewContentData newContentData = this._appScope.Get<NewContentData>();
			foreach (VariableDeviceSelectable newContentContainer in newContentWithIntro)
			{
				if (!newContentContainer.IsManuallyTriggered && newContentContainer.ShowNewContentIndicatorIfNeeded(true))
				{
					yield return new WaitForSeconds(newContentData.DelayBetweenNciIntros);
				}
			}
			List<VariableDeviceSelectable>.Enumerator enumerator = default(List<VariableDeviceSelectable>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06001062 RID: 4194 RVA: 0x00037B76 File Offset: 0x00035D76
		public virtual void OnTransitionedOut()
		{
			this._overrideNextTransitionDuration = -1f;
			base.gameObject.SetActive(false);
		}

		// Token: 0x06001063 RID: 4195 RVA: 0x00037B8F File Offset: 0x00035D8F
		public virtual void OnLostFocus()
		{
			this._canvasGroup.SetInteractable(false);
			this._canvasGroup.SetBlocksRaycasts(false);
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x00037BAC File Offset: 0x00035DAC
		public virtual void OnGainedFocus()
		{
			this._canvasGroup.SetInteractable(true);
			this._canvasGroup.SetBlocksRaycasts(true);
			if (this.firstFocus != null && this._appScope.Get<InputState>().CurrentInputTypeRequiresFocus)
			{
				this._navigation.SetNewFocus(this.firstFocus);
			}
		}

		// Token: 0x06001065 RID: 4197 RVA: 0x00037C02 File Offset: 0x00035E02
		public void SkipNextTransition()
		{
			this._overrideNextTransitionDuration = 0f;
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x00037C0F File Offset: 0x00035E0F
		public void OverrideNextTransition(float duration)
		{
			this._overrideNextTransitionDuration = duration;
		}

		// Token: 0x06001067 RID: 4199 RVA: 0x00037C18 File Offset: 0x00035E18
		public virtual void OnCreatedInScope(IScope scope)
		{
			base.gameObject.SetActive(true);
			this.RegisterAllLocalizedTextChildren();
			this.RegisterButtons();
			this._inputState.Subscribe(this);
			this._navigation.Subscribe(this);
			this.RegisterThemeComponents(this._themeDatabase.GetTheme());
			if (this._canvas != null)
			{
				this._canvas.worldCamera = Camera.main;
			}
		}

		// Token: 0x06001068 RID: 4200 RVA: 0x00037C84 File Offset: 0x00035E84
		public virtual void Reset()
		{
			this._overrideNextTransitionDuration = -1f;
			this._transitionDuration = 0f;
			this._previousCameraZoom = 0f;
			this._skipTransitions = false;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
		}

		// Token: 0x06001069 RID: 4201 RVA: 0x00037CEC File Offset: 0x00035EEC
		public virtual void OnReleasedFromScope(IScope scope)
		{
			ScreenStack.Log.Info(base.gameObject, "Releasing a {0} screen.", new object[]
			{
				base.GetType()
			});
			this.UnregisterLocalizedTextChildren();
			this.UnregisterButtons();
			this.UnregisterThemeComponents();
			this._inputState.Unsubscribe(this);
			this._navigation.Unsubscribe(this);
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600106A RID: 4202 RVA: 0x00037D55 File Offset: 0x00035F55
		public bool IsVisible()
		{
			return Mathf.Approximately(this.TransitionInPercentage(), 1f) && Mathf.Approximately(this.TransitionOutPercentage(), -1f);
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool CanTransitionIn()
		{
			return true;
		}

		// Token: 0x0600106C RID: 4204 RVA: 0x00037D7B File Offset: 0x00035F7B
		public virtual void BackActivated()
		{
			if (this.backButton != null)
			{
				this.backButton.OnSubmit(null);
			}
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void PageSelected(Vector2 direction)
		{
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x00037D97 File Offset: 0x00035F97
		public bool CanPopScreen()
		{
			return this.PopScreenAllowed;
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x00037D9F File Offset: 0x00035F9F
		public virtual void OnCurrentDeviceInputTypeChanged(DeviceInputType newInputType)
		{
			if (InputState.DeviceInputTypeRequiresFocus(newInputType))
			{
				this._navigation.SetNewFocus(this.firstFocus);
				return;
			}
			this._navigation.ClearFocus(false);
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x00037DC7 File Offset: 0x00035FC7
		public void OnMoveCursorWithNullFocus()
		{
			if (this == (BaseScalingScreen)this._screenStack.GetTopVisibleScreen())
			{
				this._navigation.SetNewFocus(this.firstFocus);
			}
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnMoveCursor(Selectable currentFocus, MoveDirection direction)
		{
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x00004BD9 File Offset: 0x00002DD9
		public virtual Selectable OverrideAutomaticNavigation()
		{
			return null;
		}

		// Token: 0x06001073 RID: 4211 RVA: 0x00037DF4 File Offset: 0x00035FF4
		public static void SetNavigationOnRight(Selectable selectable, Selectable selectOnRight)
		{
			Navigation nav = selectable.navigation;
			nav.selectOnRight = selectOnRight;
			selectable.navigation = nav;
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x00037E18 File Offset: 0x00036018
		public static void SetNavigationOnLeft(Selectable selectable, Selectable selectOnLeft)
		{
			Navigation nav = selectable.navigation;
			nav.selectOnLeft = selectOnLeft;
			selectable.navigation = nav;
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x00037E3C File Offset: 0x0003603C
		public static void SetNavigationOnUp(Selectable selectable, Selectable selectOnUp)
		{
			Navigation nav = selectable.navigation;
			nav.selectOnUp = selectOnUp;
			selectable.navigation = nav;
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x00037E60 File Offset: 0x00036060
		public static void SetNavigationOnDown(Selectable selectable, Selectable selectOnDown)
		{
			Navigation nav = selectable.navigation;
			nav.selectOnDown = selectOnDown;
			selectable.navigation = nav;
		}

		// Token: 0x04000E57 RID: 3671
		public VariableDeviceSelectable firstFocus;

		// Token: 0x04000E58 RID: 3672
		public TouchButton backButton;

		// Token: 0x04000E59 RID: 3673
		public TouchButton previousBackButton;

		// Token: 0x04000E5A RID: 3674
		[Dependency]
		protected ScreenStack _screenStack;

		// Token: 0x04000E5B RID: 3675
		[Dependency]
		protected PopupStack popupStack;

		// Token: 0x04000E5C RID: 3676
		[Dependency]
		protected IScope _appScope;

		// Token: 0x04000E5D RID: 3677
		[Dependency]
		protected GameCamera _gameCamera;

		// Token: 0x04000E5E RID: 3678
		[Dependency]
		protected IAudioSystem _audioSystem;

		// Token: 0x04000E5F RID: 3679
		[Dependency]
		protected MotorwaysThemeDatabase _themeDatabase;

		// Token: 0x04000E60 RID: 3680
		[Dependency]
		protected ActivePlayer _player;

		// Token: 0x04000E61 RID: 3681
		[Dependency]
		protected MenuNavigation _navigation;

		// Token: 0x04000E62 RID: 3682
		[Dependency]
		protected InputState _inputState;

		// Token: 0x04000E63 RID: 3683
		[Dependency]
		protected AnalyticsTracker _analytics;

		// Token: 0x04000E64 RID: 3684
		[Dependency]
		private LocaleDatabase _localeDatabase;

		// Token: 0x04000E65 RID: 3685
		protected RectTransform _rectTransform;

		// Token: 0x04000E66 RID: 3686
		protected DelegateCanvasGroup _canvasGroup;

		// Token: 0x04000E67 RID: 3687
		protected Canvas _canvas;

		// Token: 0x04000E68 RID: 3688
		[SerializeField]
		protected bool _alignToCamera = true;

		// Token: 0x04000E69 RID: 3689
		[SerializeField]
		protected bool _scaleToCamera = true;

		// Token: 0x04000E6A RID: 3690
		private float _transitionInPercentage = -1f;

		// Token: 0x04000E6B RID: 3691
		private float _transitionOutPercentage = -1f;

		// Token: 0x04000E6C RID: 3692
		protected bool _skipTransitions;

		// Token: 0x04000E6D RID: 3693
		protected float _overrideNextTransitionDuration = -1f;

		// Token: 0x04000E6E RID: 3694
		private float _transitionDuration;

		// Token: 0x04000E6F RID: 3695
		protected ScreenTransition _transitionDetails;

		// Token: 0x04000E70 RID: 3696
		private Vector3 _previousCameraPosition;

		// Token: 0x04000E71 RID: 3697
		private Quaternion _previousCameraRotation;

		// Token: 0x04000E72 RID: 3698
		protected float _previousCameraZoom;

		// Token: 0x04000E73 RID: 3699
		protected static readonly Vector2 referenceResolution = new Vector2(1920f, 1080f);

		// Token: 0x04000E74 RID: 3700
		protected static readonly float referenceAspectRatio = 1.7777778f;

		// Token: 0x04000E75 RID: 3701
		private List<LocalizedTextUI> allLocalizedText = new List<LocalizedTextUI>();

		// Token: 0x04000E76 RID: 3702
		private List<VariableDeviceSelectable> _allButtons = new List<VariableDeviceSelectable>();

		// Token: 0x04000E77 RID: 3703
		[SerializeField]
		public List<IThemeComponent> themeComponents = new List<IThemeComponent>();

		// Token: 0x04000E78 RID: 3704
		private readonly List<IThemeComponent> _additionalThemeComponents = new List<IThemeComponent>();

		// Token: 0x04000E79 RID: 3705
		private readonly List<IThemeComponent> _dynamicThemeComponents = new List<IThemeComponent>();

		// Token: 0x04000E7A RID: 3706
		private ITheme _lastThemeBlendedFrom;

		// Token: 0x04000E7B RID: 3707
		private ITheme _lastThemeBlendedTo;

		// Token: 0x04000E7C RID: 3708
		[SerializeField]
		protected bool PopScreenAllowed = true;
	}
}
