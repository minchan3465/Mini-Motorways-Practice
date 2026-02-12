using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Factory;
using Motorways;
using Motorways.Utility;
using Motorways.Views;
using Popups;
using Screens;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000241 RID: 577
public class ScreenStack
{
	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x06000DA7 RID: 3495 RVA: 0x0002D11D File Offset: 0x0002B31D
	public bool AreAnyScreensTransitioning
	{
		get
		{
			return this._screenTransitioningIn != null || this._screensTransitioningOut.Count > 0;
		}
	}

	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x06000DA8 RID: 3496 RVA: 0x0002D137 File Offset: 0x0002B337
	// (set) Token: 0x06000DA9 RID: 3497 RVA: 0x0002D13F File Offset: 0x0002B33F
	public Canvas FadeToBlackCanvas { get; private set; }

	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x06000DAA RID: 3498 RVA: 0x0002D148 File Offset: 0x0002B348
	public bool IsFading
	{
		get
		{
			return this._fadeStage > ScreenStack.FadeStage.None;
		}
	}

	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x06000DAB RID: 3499 RVA: 0x0002D153 File Offset: 0x0002B353
	public bool ExitingToMainMenu
	{
		get
		{
			return this._forceExitToMainMenu;
		}
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x0002D15C File Offset: 0x0002B35C
	public virtual ScreenType PushScreen<ScreenType>(ScreenStack.MotorwaysScreen screenType, Action<ScreenType> prepAction, bool additive = false, IScope gameScope = null, bool blocksGameInput = true, IScreen overrideTransitionFrom = null) where ScreenType : class, IScreen
	{
		ScreenType screenInst = this.CreateOrRetrieveScreenAndPrep<ScreenType>(screenType, prepAction, gameScope, blocksGameInput);
		this.PushScreen(screenInst, additive, overrideTransitionFrom);
		return screenInst;
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x0002D186 File Offset: 0x0002B386
	public virtual ScreenType PushScreen<ScreenType>(ScreenStack.MotorwaysScreen screenType, bool additive = false, IScope gameScope = null, bool blocksGameInput = true) where ScreenType : class, IScreen
	{
		return this.PushScreen<ScreenType>(screenType, null, additive, gameScope, blocksGameInput, null);
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x0002D198 File Offset: 0x0002B398
	public virtual IScreen PushScreen(ScreenStack.MotorwaysScreen screenType, bool additive = false, IScope gameScope = null, bool blocksGameInput = true)
	{
		IScreen screenInst = this.CreateOrRetrieveScreen(screenType);
		this.InitializeInGameScreen(screenInst, gameScope, blocksGameInput);
		this.PushScreen(screenInst, additive, null);
		return screenInst;
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x0002D1C4 File Offset: 0x0002B3C4
	protected virtual void PushScreen(IScreen newScreen, bool additive = false, IScreen overrideTransitionFrom = null)
	{
		if (newScreen.CanTransitionIn())
		{
			IScreen previousScreen = overrideTransitionFrom ?? ((this._screenStack.Count > 0) ? this._screenStack[this._screenStack.Count - 1] : null);
			this.AddScreenToStack(newScreen);
			this.StartScreenTransitions(previousScreen, newScreen, additive);
			this._pendingScreen = null;
			return;
		}
		this._pendingScreen = newScreen;
		this._isPendingScreenAdditive = additive;
		this._pendingScreenGraceTimer = 1.5f;
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x0002D23C File Offset: 0x0002B43C
	public virtual ScreenType ReplaceScreenOnTop<ScreenType>(ScreenStack.MotorwaysScreen screenType, Action<ScreenType> prepAction, IScope gameScope = null, bool blocksGameInput = true) where ScreenType : class, IScreen
	{
		ScreenType screenInst = this.CreateOrRetrieveScreenAndPrep<ScreenType>(screenType, prepAction, gameScope, blocksGameInput);
		this.ReplaceScreenOnTop(screenInst);
		return screenInst;
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x0002D262 File Offset: 0x0002B462
	public virtual ScreenType ReplaceScreenOnTop<ScreenType>(ScreenStack.MotorwaysScreen screenType, IScope gameScope = null, bool blocksGameInput = true) where ScreenType : class, IScreen
	{
		return this.ReplaceScreenOnTop<ScreenType>(screenType, null, gameScope, blocksGameInput);
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x0002D270 File Offset: 0x0002B470
	public virtual IScreen ReplaceScreenOnTop(ScreenStack.MotorwaysScreen screenType, IScope gameScope = null, bool blocksGameInput = true)
	{
		IScreen screenInst = this.CreateOrRetrieveScreen(screenType);
		this.InitializeInGameScreen(screenInst, gameScope, blocksGameInput);
		this.ReplaceScreenOnTop(screenInst);
		return screenInst;
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x0002D298 File Offset: 0x0002B498
	public virtual void ReplaceScreenOnTop(IScreen newScreen)
	{
		IScreen previousScreen = this._screenStack[this._screenStack.Count - 1];
		this._screenStack.RemoveAt(this._screenStack.Count - 1);
		this.AddScreenToStack(newScreen);
		this.StartScreenTransitions(previousScreen, newScreen, false);
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x0002D2E8 File Offset: 0x0002B4E8
	public virtual ScreenType ReplaceScreens<ScreenType>(ScreenStack.MotorwaysScreen screenType, Action<ScreenType> prepAction, Type includingMostRecentScreenOfType, IScope gameScope = null, bool blocksGameInput = true) where ScreenType : class, IScreen
	{
		ScreenType screenInst = this.CreateOrRetrieveScreenAndPrep<ScreenType>(screenType, prepAction, gameScope, blocksGameInput);
		this.ReplaceScreens(screenInst, includingMostRecentScreenOfType);
		return screenInst;
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x0002D310 File Offset: 0x0002B510
	public virtual ScreenType ReplaceScreens<ScreenType>(ScreenStack.MotorwaysScreen screenType, Type includingMostRecentScreenOfType, IScope gameScope = null, bool blocksGameInput = true) where ScreenType : class, IScreen
	{
		return this.ReplaceScreens<ScreenType>(screenType, null, includingMostRecentScreenOfType, gameScope, blocksGameInput);
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x0002D320 File Offset: 0x0002B520
	public virtual IScreen ReplaceScreenOnTop(ScreenStack.MotorwaysScreen screenType, Type includingMostRecentScreenOfType, IScope gameScope = null, bool blocksGameInput = true)
	{
		IScreen screenInst = this.CreateOrRetrieveScreen(screenType);
		this.InitializeInGameScreen(screenInst, gameScope, blocksGameInput);
		this.ReplaceScreens(screenInst, includingMostRecentScreenOfType);
		return screenInst;
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x0002D34C File Offset: 0x0002B54C
	public virtual void ReplaceScreens(IScreen newScreen, Type includingMostRecentScreenOfType)
	{
		IScreen previousScreen = this._screenStack[this._screenStack.Count - 1];
		IScreen foundScreenOfType = null;
		int screenIndex;
		for (screenIndex = this._screenStack.Count - 1; screenIndex >= 0; screenIndex--)
		{
			if (includingMostRecentScreenOfType.IsAssignableFrom(this._screenStack[screenIndex].GetType()))
			{
				foundScreenOfType = this._screenStack[screenIndex];
				break;
			}
		}
		if (Diagnostics.Verify(foundScreenOfType != null, "We were unable to find a screen of type {0} in the stack!  Aborting the ReplaceScreens().", includingMostRecentScreenOfType.ToString()))
		{
			for (int screensToTransitionIndex = screenIndex; screensToTransitionIndex < this._screenStack.Count; screensToTransitionIndex++)
			{
				this.StartScreenTransitions(this._screenStack[screensToTransitionIndex], null, false);
			}
			this._screenStack.RemoveRange(screenIndex, this._screenStack.Count - screenIndex);
			this.AddScreenToStack(newScreen);
			this.StartScreenTransitions(previousScreen, newScreen, false);
		}
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x0002D420 File Offset: 0x0002B620
	public virtual void PopOneScreen()
	{
		if (Diagnostics.Verify(this._screenStack.Count > 1, "Trying to pop back a screen when we only have {0} screens.", this._screenStack.Count))
		{
			IScreen outScreen = this._screenStack[this._screenStack.Count - 1];
			if (outScreen.CanPopScreen())
			{
				this._screenStack.RemoveAt(this._screenStack.Count - 1);
				this._screenTransitioningIn = this._screenStack[this._screenStack.Count - 1];
				this.StartScreenTransitions(outScreen, this._screenTransitioningIn, false);
				return;
			}
			string str = "Cant pop screen: ";
			IScreen screen = outScreen;
			Debug.Log(str + ((screen != null) ? screen.ToString() : null));
		}
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x0002D4DC File Offset: 0x0002B6DC
	public virtual void PopToScreenOfType(ScreenStack.MotorwaysScreen screenType, bool inclusive = false)
	{
		this.PopToScreenOfType(this.GetScreenTypeForEnum(screenType).screenSystemType, inclusive);
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x0002D4F4 File Offset: 0x0002B6F4
	public virtual void PopToScreenOfType(Type screenType, bool inclusive = false)
	{
		ScreenStack.Log.Info("Popping {0} screen {1}.", new object[]
		{
			inclusive ? "past" : "to",
			screenType
		});
		IScreen foundScreenOfType = null;
		IScreen nextScreen = null;
		int indexOfScreenToRemoveTo = -1;
		for (int screenIndex = this._screenStack.Count - 1; screenIndex >= 0; screenIndex--)
		{
			ScreenStack.Log.Info("Checking screen {0} ...", new object[]
			{
				this._screenStack[screenIndex].GetType()
			});
			if (foundScreenOfType == null && screenType.IsAssignableFrom(this._screenStack[screenIndex].GetType()))
			{
				foundScreenOfType = this._screenStack[screenIndex];
				if (!inclusive)
				{
					nextScreen = foundScreenOfType;
					indexOfScreenToRemoveTo = screenIndex + 1;
					break;
				}
			}
			else if (foundScreenOfType != null)
			{
				nextScreen = this._screenStack[screenIndex];
				indexOfScreenToRemoveTo = screenIndex + 1;
				break;
			}
		}
		if (Diagnostics.Verify(foundScreenOfType != null, "We were unable to find a screen of type {0} in the stack! Aborting the PopBackToScreenOfType().", screenType.ToString()) && Diagnostics.Verify(indexOfScreenToRemoveTo >= 0, "We didn't find a final screen to arrive at out of {0} screens.", this._screenStack.Count))
		{
			for (int screensToTransitionIndex = indexOfScreenToRemoveTo; screensToTransitionIndex < this._screenStack.Count - 1; screensToTransitionIndex++)
			{
				if (Diagnostics.Verify(screensToTransitionIndex < this._screenStack.Count - 1 && screensToTransitionIndex >= 0, "Screen index out of bounds at {0} of {1}", screensToTransitionIndex, this._screenStack.Count))
				{
					this.StartScreenTransitions(this._screenStack[screensToTransitionIndex], null, false);
				}
			}
			try
			{
				this.StartScreenTransitions(this._screenStack[this._screenStack.Count - 1], nextScreen, false);
			}
			catch (Exception e)
			{
				Diagnostics.FailAssert("A error occured when transitioning to {0} raised exception: {1}", new object[]
				{
					(nextScreen != null) ? nextScreen.GetType() : null,
					e.ToString()
				});
			}
			ScreenStack.Log.Info("Removing {0} screens", new object[]
			{
				this._screenStack.Count - indexOfScreenToRemoveTo
			});
			this._screenStack.RemoveRange(indexOfScreenToRemoveTo, this._screenStack.Count - indexOfScreenToRemoveTo);
		}
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x0002D710 File Offset: 0x0002B910
	public virtual IScreen CreateOrRetrieveScreen(ScreenStack.MotorwaysScreen screenType)
	{
		return this.GetScreenTypeForEnum(screenType).GetScreenInstance(this._appScope);
	}

	// Token: 0x06000DBC RID: 3516 RVA: 0x0002D724 File Offset: 0x0002B924
	public virtual ScreenType CreateOrRetrieveScreen<ScreenType>(ScreenStack.MotorwaysScreen screenType) where ScreenType : class, IScreen
	{
		IScreen screenInst = this.GetScreenTypeForEnum(screenType).GetScreenInstance(this._appScope);
		if (Diagnostics.Verify(screenInst != null, "We were unable to get a screen instance for screen type {0}.", screenType.ToString()) && Diagnostics.Verify(typeof(ScreenType).IsAssignableFrom(screenInst.GetType()), "We got a screen for type {0}, but the type returned {1} doesn't match the requested system type {2}.", screenType.ToString(), screenInst.GetType().ToString(), typeof(ScreenType).ToString()))
		{
			ScreenType castScreenInst = (ScreenType)((object)screenInst);
			if (Diagnostics.Verify(castScreenInst != null, "We failed to cast the properly generated {0} screen to the system type {1}.", screenType.ToString(), typeof(ScreenType)))
			{
				return castScreenInst;
			}
		}
		return default(ScreenType);
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x0002D7E8 File Offset: 0x0002B9E8
	public virtual ScreenType CreateOrRetrieveScreenAndPrep<ScreenType>(ScreenStack.MotorwaysScreen screenType, Action<ScreenType> prepareAction, IScope gameScope = null, bool blocksGameInput = true) where ScreenType : class, IScreen
	{
		ScreenType castScreenInst = this.CreateOrRetrieveScreen<ScreenType>(screenType);
		if (castScreenInst != null)
		{
			this.InitializeInGameScreen(castScreenInst, gameScope, blocksGameInput);
			castScreenInst.Enable(true);
			if (prepareAction != null)
			{
				prepareAction(castScreenInst);
			}
		}
		return castScreenInst;
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x0002D82C File Offset: 0x0002BA2C
	public virtual ScreenType GetActiveScreen<ScreenType>() where ScreenType : IScreen
	{
		return (ScreenType)((object)this.GetActiveScreen(typeof(ScreenType)));
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x0002D843 File Offset: 0x0002BA43
	public virtual IScreen GetActiveScreen(ScreenStack.MotorwaysScreen screenType)
	{
		return this.GetActiveScreen(this.GetScreenTypeForEnum(screenType).screenSystemType);
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x0002D858 File Offset: 0x0002BA58
	public virtual IScreen GetActiveScreen(Type systemType)
	{
		for (int activeIndex = 0; activeIndex < this._activeScreens.Count; activeIndex++)
		{
			if (systemType.IsAssignableFrom(this._activeScreens[activeIndex].GetType()))
			{
				return this._activeScreens[activeIndex];
			}
		}
		return null;
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x0002D8A2 File Offset: 0x0002BAA2
	public IEnumerable<IScreen> GetActiveScreens()
	{
		return this._activeScreens;
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x0002D8AC File Offset: 0x0002BAAC
	public ScreenStack.MotorwaysScreen GetScreenTypeBelowScreenType(ScreenStack.MotorwaysScreen screenType)
	{
		for (int screenStackIndex = 1; screenStackIndex < this._activeScreens.Count; screenStackIndex++)
		{
			if (this._activeScreens[screenStackIndex].GetType() == this.GetScreenTypeForEnum(screenType).screenSystemType)
			{
				return this.GetScreenTypeForSystemType(this._activeScreens[screenStackIndex - 1].GetType()).screenEnumType;
			}
		}
		return ScreenStack.MotorwaysScreen.None;
	}

	// Token: 0x06000DC3 RID: 3523 RVA: 0x0002D913 File Offset: 0x0002BB13
	public virtual bool IsScreenInStack<ScreenType>() where ScreenType : IScreen
	{
		return this.IsScreenInStack(typeof(ScreenType));
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x0002D928 File Offset: 0x0002BB28
	public virtual bool IsScreenInStack(Type systemType)
	{
		for (int stackIndex = 0; stackIndex < this._screenStack.Count; stackIndex++)
		{
			if (systemType.IsAssignableFrom(this._screenStack[stackIndex].GetType()))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x0002D967 File Offset: 0x0002BB67
	public virtual bool IsScreenInStack(ScreenStack.MotorwaysScreen screenType)
	{
		return this.IsScreenInStack(this.GetScreenTypeForEnum(screenType).screenSystemType);
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x0002D97B File Offset: 0x0002BB7B
	public virtual bool IsScreenVisible<ScreenType>() where ScreenType : IScreen
	{
		return this.IsScreenVisible(typeof(ScreenType));
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x0002D990 File Offset: 0x0002BB90
	public virtual bool IsScreenVisible(Type systemType)
	{
		for (int visibleIndex = 0; visibleIndex < this._visibleScreens.Count; visibleIndex++)
		{
			if (systemType.IsAssignableFrom(this._visibleScreens[visibleIndex].GetType()))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x0002D9CF File Offset: 0x0002BBCF
	public virtual bool IsScreenVisible(ScreenStack.MotorwaysScreen screenType)
	{
		return this.IsScreenVisible(this.GetScreenTypeForEnum(screenType).screenSystemType);
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x0002D9E3 File Offset: 0x0002BBE3
	public virtual bool IsScreenActive<ScreenType>() where ScreenType : IScreen
	{
		return this.IsScreenActive(typeof(ScreenType));
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x0002D9F8 File Offset: 0x0002BBF8
	public virtual bool IsScreenActive(Type systemType)
	{
		for (int activeIndex = 0; activeIndex < this._activeScreens.Count; activeIndex++)
		{
			if (systemType.IsInstanceOfType(this._activeScreens[activeIndex]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x0002DA32 File Offset: 0x0002BC32
	public virtual bool IsScreenActive(ScreenStack.MotorwaysScreen screenType)
	{
		return this.IsScreenActive(this.GetScreenTypeForEnum(screenType).screenSystemType);
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x0002DA46 File Offset: 0x0002BC46
	public virtual bool IsScreenPending<ScreenType>() where ScreenType : IScreen
	{
		return this._pendingScreen != null && this._pendingScreen.GetType() == typeof(ScreenType);
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x0002DA6C File Offset: 0x0002BC6C
	public bool HasPendingScreen()
	{
		return this._pendingScreen != null;
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x0002DA77 File Offset: 0x0002BC77
	public virtual bool IsInGame()
	{
		return this.GetScreenTypeForEnum(ScreenStack.MotorwaysScreen.InGame).screenSystemType.IsAssignableFrom(this._screenStack[this._screenStack.Count - 1].GetType());
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x0002DAA7 File Offset: 0x0002BCA7
	public void FadeNextTransition(float duration)
	{
		this._fadeDuration = duration;
		this._fadeTimer = duration;
		this._fadeStage = ScreenStack.FadeStage.FadeToBlack;
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x0002DAC0 File Offset: 0x0002BCC0
	public virtual Game GetGameIfInGame()
	{
		GameContainerScreen gameScreen = this.GetActiveScreen<GameContainerScreen>();
		if (gameScreen != null)
		{
			return gameScreen.GetActiveGame();
		}
		return null;
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x0002DAE5 File Offset: 0x0002BCE5
	public virtual ScreenStack.MotorwaysScreen CurrentVisibleScreenType()
	{
		return this.GetScreenTypeForSystemType(this._visibleScreens[this._visibleScreens.Count - 1].GetType()).screenEnumType;
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x0002DB0F File Offset: 0x0002BD0F
	public bool HasVisibleScreens()
	{
		return this._visibleScreens.Count > 0;
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x0002DB1F File Offset: 0x0002BD1F
	public virtual IScreen GetTopVisibleScreen()
	{
		if (!Diagnostics.Verify(this._visibleScreens.Count >= 1, "Trying to get a visible screen when we don't have one"))
		{
			return null;
		}
		return this._visibleScreens[this._visibleScreens.Count - 1];
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x0002DB58 File Offset: 0x0002BD58
	public ScreenStack.MotorwaysScreen GetTopActiveScreenType()
	{
		if (this.GetScreenTypeForSystemType(this._activeScreens[this._activeScreens.Count - 1].GetType()) == null)
		{
			return ScreenStack.MotorwaysScreen.None;
		}
		return this.GetScreenTypeForSystemType(this._activeScreens[this._activeScreens.Count - 1].GetType()).screenEnumType;
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x0002DBB4 File Offset: 0x0002BDB4
	private bool IsInGameScreen(ScreenStack.MotorwaysScreen screen)
	{
		return this._menuDefinition.IsInGameScreen(screen) || (this.GetGameIfInGame() != null && screen == ScreenStack.MotorwaysScreen.ChallengeInfo);
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x0002DBD8 File Offset: 0x0002BDD8
	public Vector3 GetPositionFor(ScreenStack.MotorwaysScreen screen)
	{
		if (this.IsInGameScreen(screen))
		{
			if (screen == ScreenStack.MotorwaysScreen.GameOver)
			{
				GameOverScreen gameOverScreen = this.GetActiveScreen<GameOverScreen>();
				if (gameOverScreen != null)
				{
					return gameOverScreen.focusPoint;
				}
			}
			Game game = this.GetGameIfInGame();
			if (Diagnostics.Verify(game != null, "Game can't be null by the time we're transitioning to the game!"))
			{
				CameraView cameraView = game.Scope.Get<CameraView>();
				if (screen == ScreenStack.MotorwaysScreen.InGame)
				{
					return cameraView.DesiredPosition;
				}
				return cameraView.CurrentUnfocusedPosition;
			}
		}
		if (screen == ScreenStack.MotorwaysScreen.MapSelect || screen == ScreenStack.MotorwaysScreen.ResumeGame || screen == ScreenStack.MotorwaysScreen.ProfileSelect)
		{
			ScrollingButtonScreen selectScreen = null;
			if (screen != ScreenStack.MotorwaysScreen.MapSelect)
			{
				if (screen != ScreenStack.MotorwaysScreen.ResumeGame)
				{
					if (screen == ScreenStack.MotorwaysScreen.ProfileSelect)
					{
						selectScreen = this.GetActiveScreen<ProfileSelectScreen>();
					}
				}
				else
				{
					selectScreen = this.GetActiveScreen<ResumeGameScreen>();
				}
			}
			else
			{
				selectScreen = this.GetActiveScreen<MapSelectScreen>();
			}
			if (selectScreen != null && selectScreen.HasValidCameraPosition())
			{
				return selectScreen.GetCameraPosition();
			}
		}
		return this._menuDefinition.GetPositionFor(screen);
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x0002DC99 File Offset: 0x0002BE99
	public Quaternion GetRotationFor(ScreenStack.MotorwaysScreen screen)
	{
		return this._menuDefinition.GetRotationFor(screen);
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x0002DCA8 File Offset: 0x0002BEA8
	public float GetZoomFor(ScreenStack.MotorwaysScreen screen)
	{
		if (screen == ScreenStack.MotorwaysScreen.GameOver)
		{
			return this._menuDefinition.GetZoomFor(screen);
		}
		if (this._menuDefinition.IsInGameScreen(screen))
		{
			Game game = this.GetGameIfInGame();
			if (Diagnostics.Verify(game != null, "Game can't be null by the time we're transitioning to the game!"))
			{
				CameraView cameraView = game.Scope.Get<CameraView>();
				cameraView.UpdateMaxZoom();
				return cameraView.MaxZoom;
			}
		}
		return this._menuDefinition.GetZoomFor(screen);
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x0002DD10 File Offset: 0x0002BF10
	public ScreenTransition GetTransitionDetailsFrom(ScreenStack.MotorwaysScreen origin, ScreenStack.MotorwaysScreen destination)
	{
		if (Diagnostics.Verify(this._menuDefinition != null, "_menuDefinition is null! : ScreenStack.GetTransitionDetailsFrom()"))
		{
			NodeConnection connection = this._menuDefinition.GetConnectionFrom(origin, destination);
			Quaternion startRotation = this.GetRotationFor(connection.startNode.screen);
			Vector3 startPosition = this.GetPositionFor(connection.startNode.screen);
			Vector3 startHandle = connection.entryHandle;
			Vector3 endHandle = connection.exitHandle;
			Vector3 endPosition = this.GetPositionFor(connection.endNode.screen);
			Quaternion endRotation = this.GetRotationFor(connection.endNode.screen);
			return new ScreenTransition
			{
				spline = new Spline.BezierSplineWithRotation(startPosition, startPosition + startHandle, endHandle + endPosition, endPosition, startRotation, endRotation),
				duration = connection.duration,
				cameraControl = connection.cameraControl
			};
		}
		return null;
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x0002DDF4 File Offset: 0x0002BFF4
	public virtual void DONTCALL_RegisterTestScreenType<ScreenType>(ScreenStack.MotorwaysScreen newScreenEnumType, string newAssetBundle, string newPrefabName, ScreenType newScreenInstance = default(ScreenType)) where ScreenType : class, IScreen
	{
		if (Diagnostics.Verify(this.GetScreenTypeForSystemType(typeof(ScreenType)) == null, "We shouldn't have a screen of type {0} already registered!", typeof(ScreenType).ToString()))
		{
			this._availableScreenTypes.Add(ScreenStack.MotorwaysScreenType.ForScreenType<ScreenType>(newScreenEnumType, newAssetBundle, newPrefabName, newScreenInstance));
		}
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x0002DE44 File Offset: 0x0002C044
	public virtual void Start()
	{
		this.FadeToBlackCanvas = new GameObject("FadeToBlack").AddComponent<Canvas>();
		this.FadeToBlackCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		this._camera.AttachCameraToCanvas(this.FadeToBlackCanvas, CameraLayer.Default);
		this.FadeToBlackCanvas.sortingLayerName = "UI";
		this.FadeToBlackCanvas.sortingOrder = 10;
		if (FeatureToggle.IsFeatureEnabled(Feature.BetaWatermark))
		{
			TextMeshProUGUI textMeshProUGUI = new GameObject("Text").AddComponent<TextMeshProUGUI>();
			textMeshProUGUI.SetText(string.Format("CONFIDENTIAL\nMini Motorways {0} ({1})", global::Version.Name, global::Version.Timestamp), true);
			textMeshProUGUI.color = Color.grey;
			textMeshProUGUI.gameObject.transform.SetParent(this.FadeToBlackCanvas.gameObject.transform, false);
			RectTransform component = textMeshProUGUI.GetComponent<RectTransform>();
			component.pivot = new Vector2(0f, 0.5f);
			component.sizeDelta = new Vector2(1000f, 100f);
			component.anchorMax = Vector2.zero;
			component.anchorMin = Vector2.zero;
			component.anchoredPosition = new Vector2(300f, 150f);
			Canvas overlayCanvas = UnityEngine.Object.Instantiate<Canvas>(this.FadeToBlackCanvas);
			overlayCanvas.gameObject.name = "BetaTextOverlayCanvas";
			this._camera.AttachCameraToCanvas(overlayCanvas, CameraLayer.Overlay);
			overlayCanvas.gameObject.layer = this._camera.OverlayLayerIndex;
		}
		this._fadeToBlackImage = this.FadeToBlackCanvas.gameObject.AddComponent<Image>();
		this._fadeToBlackImage.color = Color.clear;
		this.PushScreen(this._appScope.Get<IInitialGameScreen>(), false, null);
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x0002DFDC File Offset: 0x0002C1DC
	public virtual void Tick(float deltaTime)
	{
		if (this._pendingScreen != null && this._pendingScreen.CanTransitionIn())
		{
			this._pendingScreenGraceTimer -= deltaTime;
			if (this._pendingScreenGraceTimer <= 0f)
			{
				this.PushScreen(this._pendingScreen, this._isPendingScreenAdditive, null);
				this._pendingScreen = null;
			}
		}
		else if (this._pendingScreen != null)
		{
			this._pendingScreenGraceTimer = 1.5f;
		}
		if (this._fadeTimer > 0f)
		{
			this._fadeTimer -= deltaTime;
			if (this._fadeStage == ScreenStack.FadeStage.FadeToBlack)
			{
				this._fadeToBlackImage.color = Color.Lerp(Color.clear, Color.black, 1f - this._fadeTimer / this._fadeDuration);
			}
			else if (this._fadeStage == ScreenStack.FadeStage.FadeFromBlack)
			{
				this._fadeToBlackImage.color = Color.Lerp(Color.black, Color.clear, 1f - this._fadeTimer / this._fadeDuration);
				if (this._fadeTimer < 0f)
				{
					this._fadeStage = ScreenStack.FadeStage.None;
				}
			}
		}
		else
		{
			for (int screenIndex = 0; screenIndex < this._activeScreens.Count; screenIndex++)
			{
				this._activeScreens[screenIndex].Tick(deltaTime);
			}
		}
		if (this._screensTransitioningOut.Count > 0)
		{
			for (int transitioningOutScreenIndex = this._screensTransitioningOut.Count - 1; transitioningOutScreenIndex >= 0; transitioningOutScreenIndex--)
			{
				IScreen screenTransitioningOut = this._screensTransitioningOut[transitioningOutScreenIndex];
				if (!screenTransitioningOut.IsTransitioningOut())
				{
					ScreenStack.Log.Info("Screen {0} has transitioned out.", new object[]
					{
						screenTransitioningOut
					});
					screenTransitioningOut.OnTransitionedOut();
					if (!this._screenStack.Contains(screenTransitioningOut))
					{
						this._activeScreens.Remove(screenTransitioningOut);
						this.RemoveScreenInstanceOfType(this.GetScreenEnumForSystemType(screenTransitioningOut.GetType()));
					}
					this._screensTransitioningOut.Remove(screenTransitioningOut);
				}
			}
		}
		if (this._screenTransitioningIn != null && !this._screenTransitioningIn.IsTransitioningIn())
		{
			ScreenStack.Log.Info("Screen {0} has transitioned in.", new object[]
			{
				this._screenTransitioningIn
			});
			if (this._fadeTimer <= 0f && this._fadeStage == ScreenStack.FadeStage.FadeToBlack)
			{
				this._fadeStage = ScreenStack.FadeStage.FadeFromBlack;
				this._fadeTimer = this._fadeDuration;
			}
			if (!this._visibleScreens.Contains(this._screenTransitioningIn))
			{
				this._visibleScreens.Add(this._screenTransitioningIn);
			}
			this._screenTransitioningIn.OnTransitionedIn();
			this._screenTransitioningIn = null;
		}
		if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo) && this._screenTransitioningIn == null && this._screensTransitioningOut.Count == 0)
		{
			ScreenStack.MotorwaysScreen topScreen = this.GetTopActiveScreenType();
			if (topScreen != ScreenStack.MotorwaysScreen.None && topScreen != ScreenStack.MotorwaysScreen.MainMenu && this._screenTransitioningIn == null && (this._visibleScreens.Count == 0 || this.CurrentVisibleScreenType() != ScreenStack.MotorwaysScreen.MainMenu) && (Time.time - this._inputState.LastInputTimestamp > 115f || this._forceExitToMainMenu))
			{
				this._forceExitToMainMenu = false;
				this._appScope.Get<PopupStack>().PushPopup<AppleDemoCardPopup>(0f, true).Initialise(true);
				this._inputState.BlockAllInput = true;
				if (topScreen == ScreenStack.MotorwaysScreen.Upgrade || topScreen == ScreenStack.MotorwaysScreen.Photo || topScreen == ScreenStack.MotorwaysScreen.CinematicMode)
				{
					this.PopOneScreen();
					this._forceExitToMainMenu = true;
				}
				else
				{
					this._themeDatabase.SetCurrentMapDefinition(this.GetActiveScreen<StartupScreen>().mapDefinition, 1f);
					GameContainerScreen gameContainer = this.GetActiveScreen<GameContainerScreen>();
					if (gameContainer != null)
					{
						Game game = gameContainer.GetActiveGame();
						game.StopAudio();
						if (topScreen != ScreenStack.MotorwaysScreen.GameOver)
						{
							game.Scope.Get<GameUIScreen>().SetUIVisible(false, false, true, false);
							game.OnGameEnd(GameEndReason.Exit);
						}
					}
					this.PopToScreenOfType(ScreenStack.MotorwaysScreen.MainMenu, false);
					this._inputState.BlockAllInput = false;
				}
				this._player.MotorwaysUserProfile.ClearCityStatistics();
			}
		}
		if (this._forceExitToMainMenu && !FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
		{
			if (this.AreAnyScreensTransitioning)
			{
				return;
			}
			ScreenStack.MotorwaysScreen topScreen2 = this.GetTopActiveScreenType();
			if (!this._menuDefinition.TransitionExists(topScreen2, ScreenStack.MotorwaysScreen.MainMenu))
			{
				this.PopOneScreen();
				return;
			}
			GameContainerScreen gameContainer2 = this.GetActiveScreen<GameContainerScreen>();
			if (gameContainer2 != null)
			{
				Game game2 = gameContainer2.GetActiveGame();
				game2.StopAudio();
				if (topScreen2 != ScreenStack.MotorwaysScreen.GameOver)
				{
					game2.Scope.Get<GameUIScreen>().SetUIVisible(false, false, true, false);
					game2.OnGameEnd(GameEndReason.Exit);
				}
			}
			this._themeDatabase.SetCurrentMapDefinition(this.GetActiveScreen<StartupScreen>().mapDefinition, 1f);
			this.PopToScreenOfType(ScreenStack.MotorwaysScreen.MainMenu, false);
		}
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x0002E434 File Offset: 0x0002C634
	public ScreenStack.MotorwaysScreen GetScreenEnumForSystemType(Type screenType)
	{
		ScreenStack.MotorwaysScreenType type = this.GetScreenTypeForSystemType(screenType);
		if (type != null)
		{
			return type.screenEnumType;
		}
		return ScreenStack.MotorwaysScreen.None;
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x0002E454 File Offset: 0x0002C654
	public Task ExitToMainMenu()
	{
		ScreenStack.<ExitToMainMenu>d__86 <ExitToMainMenu>d__;
		<ExitToMainMenu>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ExitToMainMenu>d__.<>4__this = this;
		<ExitToMainMenu>d__.<>1__state = -1;
		<ExitToMainMenu>d__.<>t__builder.Start<ScreenStack.<ExitToMainMenu>d__86>(ref <ExitToMainMenu>d__);
		return <ExitToMainMenu>d__.<>t__builder.Task;
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x0002E497 File Offset: 0x0002C697
	private bool IsInMainMenu()
	{
		return this.HasVisibleScreens() && !this.AreAnyScreensTransitioning && this.GetTopVisibleScreen() is MainMenuScreen;
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x0002E4BC File Offset: 0x0002C6BC
	public void OnApplicationPaused()
	{
		if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
		{
			ScreenStack.MotorwaysScreen topScreen = this.GetTopActiveScreenType();
			if (topScreen != ScreenStack.MotorwaysScreen.None && topScreen != ScreenStack.MotorwaysScreen.MainMenu && topScreen != ScreenStack.MotorwaysScreen.Startup && this._screenTransitioningIn == null && (this._visibleScreens.Count == 0 || this.CurrentVisibleScreenType() != ScreenStack.MotorwaysScreen.MainMenu))
			{
				this._forceExitToMainMenu = false;
				this._inputState.BlockAllInput = true;
				if (topScreen == ScreenStack.MotorwaysScreen.Upgrade || topScreen == ScreenStack.MotorwaysScreen.Photo || topScreen == ScreenStack.MotorwaysScreen.CinematicMode)
				{
					this.PopOneScreen();
					this._forceExitToMainMenu = true;
				}
				else
				{
					this._appScope.Get<PopupStack>().PushPopup<AppleDemoCardPopup>(0f, true).Initialise(true);
					this._themeDatabase.SetCurrentMapDefinition(this.GetActiveScreen<StartupScreen>().mapDefinition, 1f);
					GameContainerScreen gameContainer = this.GetActiveScreen<GameContainerScreen>();
					if (gameContainer != null)
					{
						Game game = gameContainer.GetActiveGame();
						game.StopAudio();
						if (topScreen != ScreenStack.MotorwaysScreen.GameOver)
						{
							game.Scope.Get<GameUIScreen>().SetUIVisible(false, false, true, false);
							game.OnGameEnd(GameEndReason.Exit);
						}
					}
					this.PopToScreenOfType(ScreenStack.MotorwaysScreen.MainMenu, false);
					this._inputState.BlockAllInput = false;
				}
				this._player.MotorwaysUserProfile.ClearCityStatistics();
			}
		}
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x0002E5DE File Offset: 0x0002C7DE
	protected virtual void AddScreenToStack(IScreen newScreen)
	{
		this._screenStack.Add(newScreen);
		this._activeScreens.Add(newScreen);
		newScreen.Enable(true);
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x0002E600 File Offset: 0x0002C800
	protected virtual void StartScreenTransitions(IScreen transitionOut, IScreen transitionIn, bool additive = false)
	{
		this._screenTransitioningIn = transitionIn;
		if (this._screenTransitioningIn != null)
		{
			ScreenStack.MotorwaysScreen outScreenType = ScreenStack.MotorwaysScreen.None;
			if (transitionOut != null)
			{
				ScreenStack.MotorwaysScreenType screenType = this.GetScreenTypeForSystemType(transitionOut.GetType());
				if (screenType != null)
				{
					outScreenType = screenType.screenEnumType;
				}
			}
			this._screenTransitioningIn.TransitionIn(outScreenType);
		}
		if (transitionOut != null)
		{
			if (!this._screensTransitioningOut.Contains(transitionOut))
			{
				this._screensTransitioningOut.Add(transitionOut);
			}
			ScreenStack.MotorwaysScreen inScreenType = ScreenStack.MotorwaysScreen.None;
			if (transitionIn != null)
			{
				ScreenStack.MotorwaysScreenType screenType2 = this.GetScreenTypeForSystemType(transitionIn.GetType());
				if (screenType2 != null)
				{
					inScreenType = screenType2.screenEnumType;
				}
			}
			transitionOut.TransitionOut(inScreenType);
			if (!additive && this._visibleScreens.Contains(transitionOut))
			{
				this._visibleScreens.Remove(transitionOut);
			}
		}
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x0002E6A4 File Offset: 0x0002C8A4
	protected virtual ScreenStack.MotorwaysScreenType GetScreenTypeForEnum(ScreenStack.MotorwaysScreen screenType)
	{
		for (int screenTypeIndex = 0; screenTypeIndex < this._availableScreenTypes.Count; screenTypeIndex++)
		{
			if (this._availableScreenTypes[screenTypeIndex].screenEnumType == screenType)
			{
				return this._availableScreenTypes[screenTypeIndex];
			}
		}
		return null;
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x0002E6EC File Offset: 0x0002C8EC
	protected virtual void RemoveScreenInstanceOfType(ScreenStack.MotorwaysScreen screenType)
	{
		for (int screenTypeIndex = 0; screenTypeIndex < this._availableScreenTypes.Count; screenTypeIndex++)
		{
			if (this._availableScreenTypes[screenTypeIndex].screenEnumType == screenType)
			{
				if (this._availableScreenTypes[screenTypeIndex].screenInstance != null)
				{
					this._appScope.Release(this._availableScreenTypes[screenTypeIndex].screenInstance);
				}
				this._availableScreenTypes[screenTypeIndex].screenInstance = null;
				return;
			}
		}
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x0002E768 File Offset: 0x0002C968
	protected virtual ScreenStack.MotorwaysScreenType GetScreenTypeForSystemType(Type screenType)
	{
		for (int screenTypeIndex = 0; screenTypeIndex < this._availableScreenTypes.Count; screenTypeIndex++)
		{
			if (this._availableScreenTypes[screenTypeIndex].screenSystemType == screenType)
			{
				return this._availableScreenTypes[screenTypeIndex];
			}
		}
		return null;
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x0002E7B4 File Offset: 0x0002C9B4
	protected virtual bool InitializeInGameScreen(IScreen screenInst, IScope withGameScope, bool blocksGameInput)
	{
		if (typeof(InGameScalingScreen).IsAssignableFrom(screenInst.GetType()) && Diagnostics.Verify(withGameScope != null, "We are attempting to init a {0} screen which requires a game Scope to be initialized, but one was not provided!", screenInst.GetType()))
		{
			InGameScalingScreen inGameScreen = screenInst as InGameScalingScreen;
			if (inGameScreen != null)
			{
				inGameScreen.InitScreen(withGameScope, blocksGameInput);
				return true;
			}
		}
		return false;
	}

	// Token: 0x040007C9 RID: 1993
	public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ScreenStack");

	// Token: 0x040007CA RID: 1994
	private List<ScreenStack.MotorwaysScreenType> _availableScreenTypes = new List<ScreenStack.MotorwaysScreenType>
	{
		ScreenStack.MotorwaysScreenType.ForScreenType<MainMenuScreen>(ScreenStack.MotorwaysScreen.MainMenu, "core", "MainMenuScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<OptionsScreenMain>(ScreenStack.MotorwaysScreen.OptionsMain, "core", "OptionsScreenMain", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<OptionsScreenPause>(ScreenStack.MotorwaysScreen.OptionsPause, "core", "OptionsScreenPause", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<MapSelectScreen>(ScreenStack.MotorwaysScreen.MapSelect, "core", "MapSelectScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<GameContainerScreen>(ScreenStack.MotorwaysScreen.InGame, "core", "GameContainerScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<GameOverScreen>(ScreenStack.MotorwaysScreen.GameOver, "core", "GameOverScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<GameUpgradeScreen>(ScreenStack.MotorwaysScreen.Upgrade, "core", "GameUpgradeScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<PauseScreen>(ScreenStack.MotorwaysScreen.Pause, "core", "PauseScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<ResumeGameScreen>(ScreenStack.MotorwaysScreen.ResumeGame, "core", "ResumeGameScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<StartupScreen>(ScreenStack.MotorwaysScreen.Startup, "core", "StartupScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<PhotoScreen>(ScreenStack.MotorwaysScreen.Photo, "core", "PhotoScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<ChallengeInfoScreen>(ScreenStack.MotorwaysScreen.ChallengeInfo, "core", "ChallengeInfoScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<ProfileSelectScreen>(ScreenStack.MotorwaysScreen.ProfileSelect, "core", "ProfileSelectScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<ProfileCreationScreen>(ScreenStack.MotorwaysScreen.ProfileCreation, "core", "ProfileCreationScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<MovieScreen>(ScreenStack.MotorwaysScreen.Movie, "core", "MovieScreen", null),
		ScreenStack.MotorwaysScreenType.ForScreenType<CinematicModeScreen>(ScreenStack.MotorwaysScreen.CinematicMode, "core", "CinematicModeScreen", null)
	};

	// Token: 0x040007CB RID: 1995
	[Dependency]
	protected IScope _appScope;

	// Token: 0x040007CC RID: 1996
	[Dependency]
	protected MenuPlacementDefinition _menuDefinition;

	// Token: 0x040007CD RID: 1997
	[Dependency]
	protected GameCamera _camera;

	// Token: 0x040007CE RID: 1998
	[Dependency]
	protected InputState _inputState;

	// Token: 0x040007CF RID: 1999
	[Dependency]
	protected MotorwaysThemeDatabase _themeDatabase;

	// Token: 0x040007D0 RID: 2000
	[Dependency]
	protected ActivePlayer _player;

	// Token: 0x040007D1 RID: 2001
	public const float DemoIdleReturnDuration = 115f;

	// Token: 0x040007D2 RID: 2002
	private List<IScreen> _screenStack = new List<IScreen>();

	// Token: 0x040007D3 RID: 2003
	private List<IScreen> _activeScreens = new List<IScreen>();

	// Token: 0x040007D4 RID: 2004
	private List<IScreen> _visibleScreens = new List<IScreen>();

	// Token: 0x040007D5 RID: 2005
	private List<IScreen> _screensTransitioningOut = new List<IScreen>();

	// Token: 0x040007D6 RID: 2006
	private IScreen _screenTransitioningIn;

	// Token: 0x040007D7 RID: 2007
	private IScreen _pendingScreen;

	// Token: 0x040007D8 RID: 2008
	private bool _isPendingScreenAdditive;

	// Token: 0x040007D9 RID: 2009
	private const float _pendingScreenGraceDuration = 1.5f;

	// Token: 0x040007DA RID: 2010
	private float _pendingScreenGraceTimer = -1f;

	// Token: 0x040007DC RID: 2012
	private Image _fadeToBlackImage;

	// Token: 0x040007DD RID: 2013
	private float _fadeTimer;

	// Token: 0x040007DE RID: 2014
	private float _fadeDuration = 1f;

	// Token: 0x040007DF RID: 2015
	private ScreenStack.FadeStage _fadeStage;

	// Token: 0x040007E0 RID: 2016
	private bool _forceExitToMainMenu;

	// Token: 0x040007E1 RID: 2017
	private static readonly ProfilerMarker Profiler_Tick = new ProfilerMarker("ScreenStack.Tick");

	// Token: 0x02000242 RID: 578
	public enum MotorwaysScreen
	{
		// Token: 0x040007E3 RID: 2019
		None = -1,
		// Token: 0x040007E4 RID: 2020
		MainMenu,
		// Token: 0x040007E5 RID: 2021
		InGame,
		// Token: 0x040007E6 RID: 2022
		Pause,
		// Token: 0x040007E7 RID: 2023
		GameOver,
		// Token: 0x040007E8 RID: 2024
		Upgrade,
		// Token: 0x040007E9 RID: 2025
		OptionsMain,
		// Token: 0x040007EA RID: 2026
		MapSelect,
		// Token: 0x040007EB RID: 2027
		Credits,
		// Token: 0x040007EC RID: 2028
		ResumeGame,
		// Token: 0x040007ED RID: 2029
		Startup,
		// Token: 0x040007EE RID: 2030
		Photo,
		// Token: 0x040007EF RID: 2031
		ChallengeInfo,
		// Token: 0x040007F0 RID: 2032
		ProfileSelect,
		// Token: 0x040007F1 RID: 2033
		ProfileCreation,
		// Token: 0x040007F2 RID: 2034
		Movie,
		// Token: 0x040007F3 RID: 2035
		CinematicMode,
		// Token: 0x040007F4 RID: 2036
		OptionsPause
	}

	// Token: 0x02000243 RID: 579
	private enum FadeStage
	{
		// Token: 0x040007F6 RID: 2038
		None,
		// Token: 0x040007F7 RID: 2039
		FadeToBlack,
		// Token: 0x040007F8 RID: 2040
		FadeFromBlack
	}

	// Token: 0x02000244 RID: 580
	public class MotorwaysScreenType
	{
		// Token: 0x06000DE9 RID: 3561 RVA: 0x0002EA04 File Offset: 0x0002CC04
		public static ScreenStack.MotorwaysScreenType ForScreenType<ScreenType>(ScreenStack.MotorwaysScreen newScreenEnumType, string newAssetBundle, string newPrefabName, ScreenType newScreenInstance = default(ScreenType)) where ScreenType : class, IScreen
		{
			ScreenStack.MotorwaysScreenType newInstance = new ScreenStack.MotorwaysScreenType();
			newInstance.screenEnumType = newScreenEnumType;
			newInstance.assetBundle = newAssetBundle;
			newInstance.prefabName = newPrefabName;
			newInstance.screenSystemType = typeof(ScreenType);
			if (newScreenInstance != null && Diagnostics.Verify(newInstance.screenSystemType.IsInstanceOfType(newScreenInstance), "We are trying to explicitly provide a screen instance for a type that it does not match!  Expected type {0}, but found type {1}.", typeof(ScreenType), newInstance.GetType()))
			{
				newInstance.screenInstance = newScreenInstance;
			}
			return newInstance;
		}

		// Token: 0x06000DEA RID: 3562 RVA: 0x0002EA80 File Offset: 0x0002CC80
		public IScreen GetScreenInstance(IScope appScope)
		{
			if (this.screenInstance == null)
			{
				object screenObject = appScope.Get(this.screenSystemType);
				if (Diagnostics.Verify(this.screenSystemType.IsAssignableFrom(screenObject.GetType()), "We tried to receive an instance of a screen, but the type doesn't match our expected type!  Expected type {0}, but found type {1}.", this.screenSystemType, screenObject.GetType()))
				{
					this.screenInstance = (IScreen)screenObject;
				}
				if (this.screenInstance == null && this.assetBundle != "" && this.prefabName != "")
				{
					GameObject screenPrefab = AssetBundleUtility.LoadPrefab(this.assetBundle, this.prefabName);
					if (Diagnostics.Verify(screenPrefab != null, "We were unable to load the screen prefab for screen {0} using asset bundle {1} and prefab name {2}.", this.screenEnumType.ToString(), this.assetBundle, this.prefabName))
					{
						screenObject = screenPrefab.GetComponentInChildren(this.screenSystemType);
						if (Diagnostics.Verify(screenObject != null, "We successfully loaded the prefab for screen {0}, but were unable to find the expected component of type {1} in the prefab.", this.screenEnumType.ToString(), this.screenSystemType.ToString()))
						{
							this.screenInstance = (IScreen)screenObject;
							Diagnostics.FailAssert("This is broken at the moment because we don't have a way to do dependency injection without allocating through the App.", Array.Empty<object>());
						}
					}
				}
			}
			return this.screenInstance;
		}

		// Token: 0x040007F9 RID: 2041
		public ScreenStack.MotorwaysScreen screenEnumType;

		// Token: 0x040007FA RID: 2042
		public string assetBundle;

		// Token: 0x040007FB RID: 2043
		public string prefabName;

		// Token: 0x040007FC RID: 2044
		public IScreen screenInstance;

		// Token: 0x040007FD RID: 2045
		public Type screenSystemType;
	}
}
