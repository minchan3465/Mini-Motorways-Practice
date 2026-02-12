using System;
using System.Collections.Generic;
using Client;
using Factory;
using JetBrains.Annotations;
using Motorways.UI;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000574 RID: 1396
	public class GameUIScreenWrapper : GameUIScreen
	{
		// Token: 0x06002625 RID: 9765 RVA: 0x000A1CB5 File Offset: 0x0009FEB5
		[Button(null)]
		[UsedImplicitly]
		public void UpdateSelectedScreen()
		{
			this.SetScreenForDeviceCategory(this.selectedScreen);
		}

		// Token: 0x06002626 RID: 9766 RVA: 0x000A1CC3 File Offset: 0x0009FEC3
		public override RectTransform GetRectTransform()
		{
			return this._currentActiveRectTransform;
		}

		// Token: 0x06002627 RID: 9767 RVA: 0x000A1CCC File Offset: 0x0009FECC
		protected override Transform GetUpgradeBarTransform()
		{
			UpgradeBarWrapper wrapper = base.UpgradeBar as UpgradeBarWrapper;
			if (Diagnostics.Verify(wrapper != null, "The upgrade bar isn't a wrapper but the UI is!"))
			{
				return wrapper.upgradeBars[(int)this.selectedScreen].transform;
			}
			return base.GetUpgradeBarTransform();
		}

		// Token: 0x06002628 RID: 9768 RVA: 0x000A1D14 File Offset: 0x0009FF14
		public void SetScreenForDeviceCategory(DeviceCategory deviceCategory)
		{
			this.selectedScreen = deviceCategory;
			foreach (ScoreView scoreView in this._additionalScoreViews)
			{
				scoreView.electiveUpgradeTicker.SetActive(false);
				scoreView.SetupView();
			}
			for (int deviceCategoryIndex = 0; deviceCategoryIndex < this.screens.Length; deviceCategoryIndex++)
			{
				GameUIScreen screen = this.screens[deviceCategoryIndex];
				if (deviceCategoryIndex == (int)this.selectedScreen)
				{
					screen.GetComponent<DelegateCanvasGroup>().SetInteractable(true);
					screen.GetComponent<DelegateCanvasGroup>().SetBlocksRaycasts(true);
					screen.GetComponent<DelegateCanvasGroup>().Alpha = 1f;
					this.playableArea = screen.playableArea;
					this._currentActiveRectTransform = screen.GetRectTransform();
					screen.transform.SetParent(base.transform.parent);
					this._gameCamera.AttachCameraToCanvas(screen.GetComponent<Canvas>(), CameraLayer.UI);
				}
				else
				{
					screen.GetComponent<DelegateCanvasGroup>().SetInteractable(false);
					screen.GetComponent<DelegateCanvasGroup>().SetBlocksRaycasts(false);
					screen.GetComponent<DelegateCanvasGroup>().Alpha = 0f;
					screen.transform.SetParent(base.transform);
				}
			}
		}

		// Token: 0x06002629 RID: 9769 RVA: 0x000A1E48 File Offset: 0x000A0048
		public override void SetUIVisible(bool visible, bool instantly = false, bool forceHide = false, bool forceHideWorldGrid = false)
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetUIVisible(visible, instantly, forceHide, forceHideWorldGrid);
			}
			base.SetUIVisible(visible, instantly, forceHide, forceHideWorldGrid);
		}

		// Token: 0x0600262A RID: 9770 RVA: 0x000A1E84 File Offset: 0x000A0084
		public override void SetScoreVisible(bool visible)
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetScoreVisible(visible);
			}
			base.SetScoreVisible(visible);
		}

		// Token: 0x0600262B RID: 9771 RVA: 0x000A1EB8 File Offset: 0x000A00B8
		public override TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			foreach (ClockView clockView in this._additionalClockViews)
			{
				if (clockView != this._mainClockView)
				{
					ClockView clockView2 = clockView;
					if (clockView2.ClockModel == null)
					{
						clockView2.ClockModel = this._mainClockView.ClockModel;
					}
					clockView.Tick(timeInterval, stepAlpha);
				}
			}
			foreach (ScoreView scoreView in this._additionalScoreViews)
			{
				if (scoreView != this._mainScoreView)
				{
					ScoreView scoreView2 = scoreView;
					if (scoreView2.ScoreModel == null)
					{
						scoreView2.ScoreModel = this._mainScoreView.ScoreModel;
					}
					scoreView.Tick(timeInterval, stepAlpha);
				}
			}
			TickResult result = base.Tick(timeInterval, stepAlpha);
			this._worldGrid.localScale = this._worldGridSize;
			return result;
		}

		// Token: 0x0600262C RID: 9772 RVA: 0x000A1FCC File Offset: 0x000A01CC
		protected override void SetElectiveUpgradeAvailable(bool available)
		{
			foreach (ScoreView scoreView in this._additionalScoreViews)
			{
				scoreView.electiveUpgradeAnimator.SetBool(ScoreView.UpgradeAvailableId, available);
			}
			base.SetElectiveUpgradeAvailable(available);
		}

		// Token: 0x0600262D RID: 9773 RVA: 0x000A2030 File Offset: 0x000A0230
		public override void SetDrawButtonsVisible(bool visible)
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetDrawButtonsVisible(visible);
			}
			base.SetDrawButtonsVisible(visible);
		}

		// Token: 0x0600262E RID: 9774 RVA: 0x000A2064 File Offset: 0x000A0264
		public override void SetVcrButtonState(bool paused, TimeScale timeScale)
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetVcrButtonState(paused, timeScale);
			}
			base.SetVcrButtonState(paused, timeScale);
			foreach (ClockView clockView in this._additionalClockViews)
			{
				clockView.IsVisuallyPaused = paused;
			}
		}

		// Token: 0x0600262F RID: 9775 RVA: 0x000A20DC File Offset: 0x000A02DC
		public override void OnPausePressed()
		{
			base.OnPausePressed();
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnPausePressed();
			}
		}

		// Token: 0x06002630 RID: 9776 RVA: 0x000A210C File Offset: 0x000A030C
		public override void OnPlayPressed()
		{
			base.OnPlayPressed();
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnPlayPressed();
			}
		}

		// Token: 0x06002631 RID: 9777 RVA: 0x000A213C File Offset: 0x000A033C
		public override void OnFastForwardPressed()
		{
			base.OnFastForwardPressed();
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnFastForwardPressed();
			}
		}

		// Token: 0x06002632 RID: 9778 RVA: 0x000A216C File Offset: 0x000A036C
		public override void OnExtraFastForwardPressed()
		{
			base.OnExtraFastForwardPressed();
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnExtraFastForwardPressed();
			}
		}

		// Token: 0x06002633 RID: 9779 RVA: 0x000A219C File Offset: 0x000A039C
		public override void SetClockVisibility(bool visible)
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetClockVisibility(visible);
			}
			base.SetClockVisibility(visible);
		}

		// Token: 0x06002634 RID: 9780 RVA: 0x000A21D0 File Offset: 0x000A03D0
		public override void SetMenuButtonVisible(bool visible)
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetMenuButtonVisible(visible);
			}
			base.SetMenuButtonVisible(visible);
		}

		// Token: 0x06002635 RID: 9781 RVA: 0x000A2204 File Offset: 0x000A0404
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].TransitionIn(outScreen);
			}
			base.TransitionIn(outScreen);
			this.selectedScreen = (((float)Screen.width / (float)Screen.height < 1.5f) ? DeviceCategory.Tablet : DeviceCategory.Desktop);
			this.SetScreenForDeviceCategory(this.selectedScreen);
		}

		// Token: 0x06002636 RID: 9782 RVA: 0x000A2260 File Offset: 0x000A0460
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].TransitionOut(inScreen);
			}
			base.TransitionOut(inScreen);
		}

		// Token: 0x06002637 RID: 9783 RVA: 0x000A2294 File Offset: 0x000A0494
		public override void OnTransitionedIn()
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnTransitionedIn();
			}
			base.OnTransitionedIn();
			this.SetScreenForDeviceCategory(this.selectedScreen);
		}

		// Token: 0x06002638 RID: 9784 RVA: 0x000A22D0 File Offset: 0x000A04D0
		public override void OnTransitionedOut()
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnTransitionedOut();
			}
			base.OnTransitionedOut();
		}

		// Token: 0x06002639 RID: 9785 RVA: 0x000A2300 File Offset: 0x000A0500
		public override void InitScreen(IScope gameScope, bool blocksGameInput)
		{
			this._worldGridSize = this._worldGrid.localScale;
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].InitScreen(gameScope, blocksGameInput);
			}
			base.InitScreen(gameScope, blocksGameInput);
			this._worldGrid.localScale = this._worldGridSize;
		}

		// Token: 0x0600263A RID: 9786 RVA: 0x000A2358 File Offset: 0x000A0558
		public override void OnCreatedInScope(IScope scope)
		{
			foreach (GameUIScreen screen in this.screens)
			{
				scope.Assemble(screen);
			}
			base.OnCreatedInScope(scope);
			this._mainClockView = scope.Get<ClockView>();
			this._mainScoreView = scope.Get<ScoreView>();
			foreach (ClockView clockView in base.GetComponentsInChildren<ClockView>())
			{
				if (clockView != this._mainClockView)
				{
					this._additionalClockViews.Add(clockView);
					scope.Assemble(clockView);
					clockView.ClockModel = this._mainClockView.ClockModel;
				}
			}
			foreach (ScoreView scoreView in base.GetComponentsInChildren<ScoreView>())
			{
				if (scoreView != this._mainScoreView)
				{
					this._additionalScoreViews.Add(scoreView);
					scope.Assemble(scoreView);
					scoreView.ScoreModel = this._mainScoreView.ScoreModel;
				}
			}
			this._currentActiveRectTransform = this.screens[0].GetRectTransform();
		}

		// Token: 0x0600263B RID: 9787 RVA: 0x000A245C File Offset: 0x000A065C
		public override void OnReleasedFromScope(IScope scope)
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnReleasedFromScope(scope);
			}
			base.OnReleasedFromScope(scope);
			this._additionalClockViews.Clear();
			this._additionalScoreViews.Clear();
		}

		// Token: 0x0600263C RID: 9788 RVA: 0x000A24A4 File Offset: 0x000A06A4
		public override void Reset()
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Reset();
			}
			base.Reset();
		}

		// Token: 0x0600263D RID: 9789 RVA: 0x000A24D4 File Offset: 0x000A06D4
		public override void ScaleToCamera()
		{
			GameUIScreen[] array = this.screens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ScaleToCamera();
			}
			base.ScaleToCamera();
		}

		// Token: 0x04002027 RID: 8231
		[EnumTypedArray(typeof(DeviceCategory))]
		public GameUIScreen[] screens;

		// Token: 0x04002028 RID: 8232
		private readonly List<ClockView> _additionalClockViews = new List<ClockView>();

		// Token: 0x04002029 RID: 8233
		private readonly List<ScoreView> _additionalScoreViews = new List<ScoreView>();

		// Token: 0x0400202A RID: 8234
		public DeviceCategory selectedScreen;

		// Token: 0x0400202B RID: 8235
		private ClockView _mainClockView;

		// Token: 0x0400202C RID: 8236
		private ScoreView _mainScoreView;

		// Token: 0x0400202D RID: 8237
		private Vector3 _worldGridSize = Vector3.zero;

		// Token: 0x0400202E RID: 8238
		private RectTransform _currentActiveRectTransform;
	}
}
