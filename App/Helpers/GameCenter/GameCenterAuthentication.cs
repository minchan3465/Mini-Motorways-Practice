using System;
using System.Runtime.InteropServices;
using AOT;
using Factory;

namespace Helpers.GameCenter
{
	// Token: 0x0200078D RID: 1933
	public class GameCenterAuthentication : IGameCenterAuthentication, IReleasedFromScopeHandler
	{
		// Token: 0x06003571 RID: 13681 RVA: 0x000F9B38 File Offset: 0x000F7D38
		public void Authenticate()
		{
			GameCenterAuthentication.InputState = this._scope.Get<IInputState>();
			IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate<GameCenterShared.LogDelegate>(new GameCenterShared.LogDelegate(GameCenterAuthentication.OnLog));
			IntPtr nativeGameCenterFocusChangedDelegate = Marshal.GetFunctionPointerForDelegate<GameCenterShared.GameCenterFocusChangedDelegate>(new GameCenterShared.GameCenterFocusChangedDelegate(GameCenterAuthentication.OnGameCenterFocusChanged));
			IntPtr nativeGameCenterAuthAttemptedDelegate = Marshal.GetFunctionPointerForDelegate<GameCenterShared.GameCenterAuthAttemptedDelegate>(new GameCenterShared.GameCenterAuthAttemptedDelegate(GameCenterAuthentication.OnGameCenterAuthAttempted));
			GameCenterShared.GCStart(functionPointerForDelegate, nativeGameCenterFocusChangedDelegate, nativeGameCenterAuthAttemptedDelegate);
		}

		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x06003572 RID: 13682 RVA: 0x000F9B92 File Offset: 0x000F7D92
		public bool IsAuthenticated
		{
			get
			{
				return GameCenterShared.GCIsAuthenticated();
			}
		}

		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x06003573 RID: 13683 RVA: 0x000F9B99 File Offset: 0x000F7D99
		public bool RequiresRetry
		{
			get
			{
				return GameCenterAuthentication._requiresRetry;
			}
		}

		// Token: 0x06003574 RID: 13684 RVA: 0x00015E3F File Offset: 0x0001403F
		public void OnReleasedFromScope(IScope scope)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06003575 RID: 13685 RVA: 0x000F9BA0 File Offset: 0x000F7DA0
		[MonoPInvokeCallback(typeof(GameCenterShared.LogDelegate))]
		private static void OnLog(string logMessage)
		{
			GameCenterAuthentication.ObjectiveCLog.Info(logMessage, Array.Empty<object>());
		}

		// Token: 0x06003576 RID: 13686 RVA: 0x000F9BB2 File Offset: 0x000F7DB2
		[MonoPInvokeCallback(typeof(GameCenterShared.GameCenterFocusChangedDelegate))]
		private static void OnGameCenterFocusChanged(bool gameCenterHasFocus)
		{
			IInputState inputState = GameCenterAuthentication.InputState;
			if (inputState == null)
			{
				return;
			}
			inputState.OnInternalFocusChanged(!gameCenterHasFocus);
		}

		// Token: 0x06003577 RID: 13687 RVA: 0x000F9BC7 File Offset: 0x000F7DC7
		[MonoPInvokeCallback(typeof(GameCenterShared.GameCenterAuthAttemptedDelegate))]
		private static void OnGameCenterAuthAttempted(int result)
		{
			GameCenterAuthentication.ObjectiveCLog.Info(string.Format("OnGameCenterAuthAttempted(result:{0})", result), Array.Empty<object>());
			if (result == 2)
			{
				GameCenterAuthentication._requiresRetry = true;
			}
		}

		// Token: 0x04002D72 RID: 11634
		private static readonly Diagnostics.Log.Channel ObjectiveCLog = Diagnostics.Log.OpenChannel("Objective-C-GameCenter");

		// Token: 0x04002D73 RID: 11635
		[Dependency]
		private IScope _scope;

		// Token: 0x04002D74 RID: 11636
		private static IInputState InputState;

		// Token: 0x04002D75 RID: 11637
		private static bool _requiresRetry = false;

		// Token: 0x0200078E RID: 1934
		private enum GameCenterAuthState
		{
			// Token: 0x04002D77 RID: 11639
			NotAuthenticated,
			// Token: 0x04002D78 RID: 11640
			Authenticated,
			// Token: 0x04002D79 RID: 11641
			RequiresRetry
		}
	}
}
