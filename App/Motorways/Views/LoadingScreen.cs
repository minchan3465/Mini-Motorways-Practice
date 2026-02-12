using System;
using System.Collections.Generic;
using Easing;
using Factory;
using Helpers.GameCenter;
using Popups;
using Screens;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000549 RID: 1353
	public class LoadingScreen : BaseScalingScreen, IInitialGameScreen, IScreen
	{
		// Token: 0x06002416 RID: 9238 RVA: 0x000955AC File Offset: 0x000937AC
		public override void OnCreatedInScope(IScope scope)
		{
			base.OnCreatedInScope(scope);
			base.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
			this._loadInterruptItems.Enqueue(new LoadingScreen.CloudSaveWarning(this._storage, this.popupStack));
			this._loadInterruptItems.Enqueue(new LoadingScreen.GameCenterWarning(this._gameCenterAuthentication, this.popupStack));
		}

		// Token: 0x06002417 RID: 9239 RVA: 0x00095604 File Offset: 0x00093804
		public override void Reset()
		{
			base.Reset();
			this._hasPlayerDataLoaded = false;
			this._hasActivatedPlayer = false;
			this._stage = LoadingScreen.LoadingStage.WaitingInvisibly;
			this._timeVisible = 0f;
			this._spinnerTweenTimer = 0f;
			this._loadInterruptItems.Clear();
		}

		// Token: 0x06002418 RID: 9240 RVA: 0x00095644 File Offset: 0x00093844
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (this._hasPlayerDataLoaded && this._timeVisible > this._minTimeVisible && !this._hasActivatedPlayer)
			{
				List<Player> emptyPlayers = null;
				if (this._loadInterruptItems.Count > 0)
				{
					LoadingScreen.LoadInterruptItem currentLoadInterruptItem = this._loadInterruptItems.Peek();
					if (currentLoadInterruptItem.interruptionState == LoadingScreen.LoadInterruptItem.InterruptionState.WaitingForCheck)
					{
						if (currentLoadInterruptItem.ShouldInterrupt())
						{
							currentLoadInterruptItem.PresentInterruption();
						}
						else
						{
							currentLoadInterruptItem.interruptionState = LoadingScreen.LoadInterruptItem.InterruptionState.Done;
						}
					}
					if (currentLoadInterruptItem.interruptionState == LoadingScreen.LoadInterruptItem.InterruptionState.Done)
					{
						this._loadInterruptItems.Dequeue();
					}
				}
				if (this._loadInterruptItems.Count > 0)
				{
					return;
				}
				foreach (Player player in this._playerDatabase.Players)
				{
					if (!player.HasAvatar)
					{
						bool isEmptyPlayer = false;
						if (player.LastPlayedUtcTimeOnLocalDevice == DateTime.MinValue)
						{
							LoadingScreen.Log.Info("Deleting empty player {0} because they don't have a valid last played time.", Array.Empty<object>());
							isEmptyPlayer = true;
						}
						LegacyMotorwaysUserProfile motorwaysUserProfile = player.UserProfile as LegacyMotorwaysUserProfile;
						if (motorwaysUserProfile != null && motorwaysUserProfile.TotalPlayTime == 0)
						{
							LoadingScreen.Log.Info("Deleting empty player {0} because they don't have any play time.", Array.Empty<object>());
							isEmptyPlayer = true;
						}
						if (isEmptyPlayer)
						{
							if (emptyPlayers == null)
							{
								emptyPlayers = new List<Player>();
							}
							emptyPlayers.Add(player);
						}
						else
						{
							DateTime originalTimestamp = player.ExtendedUserProfile.UtcTimestamp;
							int iconCount = this._visualConstants.ProfileIconCount;
							int backgroundCount = 6;
							player.ChooseAvatar(iconCount, backgroundCount);
							player.ExtendedUserProfile.UtcTimestamp = originalTimestamp;
						}
					}
				}
				if (emptyPlayers != null)
				{
					foreach (Player emptyPlayer in emptyPlayers)
					{
						this._playerDatabase.DeletePlayer(emptyPlayer);
					}
				}
				this._hasActivatedPlayer = true;
				Player activePlayer = this._playerDatabase.MostRecentPlayer;
				if (activePlayer == null)
				{
					activePlayer = this._playerDatabase.CreatePlayer();
					activePlayer.LocaleId = this._softwareCapabilities.PreferredLocaleId;
					int iconCount2 = this._visualConstants.ProfileIconCount;
					int backgroundCount2 = 6;
					activePlayer.ChooseAvatar(iconCount2, backgroundCount2);
				}
				this._activePlayer.ActivatePlayer(activePlayer);
				if (this._stage == LoadingScreen.LoadingStage.WaitingInvisibly)
				{
					this._screenStack.PushScreen(ScreenStack.MotorwaysScreen.Startup, false, null, true);
					this._stage = LoadingScreen.LoadingStage.Transitioned;
				}
			}
			this._timeVisible += deltaTime;
			float spinnerAlpha = 0f;
			switch (this._stage)
			{
			case LoadingScreen.LoadingStage.WaitingInvisibly:
				if (this._timeVisible > this._maxTimeVisibleWithoutSpinner)
				{
					this._stage = LoadingScreen.LoadingStage.FadingIn;
					this._spinnerTweenTimer = 0f;
				}
				break;
			case LoadingScreen.LoadingStage.FadingIn:
				this._spinnerTweenTimer += deltaTime;
				spinnerAlpha = this._spinnerTweenTimer / this._spinnerTweenDuration;
				if (this._spinnerTweenTimer >= this._spinnerTweenDuration)
				{
					this._stage = LoadingScreen.LoadingStage.WaitingVisibly;
				}
				break;
			case LoadingScreen.LoadingStage.WaitingVisibly:
				spinnerAlpha = 1f;
				if (this._hasActivatedPlayer)
				{
					this._spinnerTweenTimer = 0f;
					this._stage = LoadingScreen.LoadingStage.FadingOut;
				}
				break;
			case LoadingScreen.LoadingStage.FadingOut:
				this._spinnerTweenTimer += deltaTime;
				spinnerAlpha = 1f - this._spinnerTweenTimer / this._spinnerTweenDuration;
				if (this._spinnerTweenTimer >= this._spinnerTweenDuration)
				{
					this._screenStack.PushScreen(ScreenStack.MotorwaysScreen.Startup, false, null, true);
					this._stage = LoadingScreen.LoadingStage.Transitioned;
				}
				break;
			}
			Color spinnerColor = this._loadingSpinner.color;
			spinnerColor.a = Easings.Interpolate(Mathf.Clamp01(spinnerAlpha), this._spinnerTweenEasing);
			this._loadingSpinner.color = spinnerColor;
		}

		// Token: 0x06002419 RID: 9241 RVA: 0x000959DC File Offset: 0x00093BDC
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			base.TransitionIn(outScreen);
			this._storage.LoadAll(new Action(this.OnPlayerDataLoaded));
		}

		// Token: 0x0600241A RID: 9242 RVA: 0x000959FC File Offset: 0x00093BFC
		private void OnPlayerDataLoaded()
		{
			this._hasPlayerDataLoaded = true;
		}

		// Token: 0x04001E1E RID: 7710
		private readonly Queue<LoadingScreen.LoadInterruptItem> _loadInterruptItems = new Queue<LoadingScreen.LoadInterruptItem>();

		// Token: 0x04001E1F RID: 7711
		private bool _hasPlayerDataLoaded;

		// Token: 0x04001E20 RID: 7712
		private bool _hasActivatedPlayer;

		// Token: 0x04001E21 RID: 7713
		[SerializeField]
		private Image _loadingSpinner;

		// Token: 0x04001E22 RID: 7714
		private LoadingScreen.LoadingStage _stage;

		// Token: 0x04001E23 RID: 7715
		private float _timeVisible;

		// Token: 0x04001E24 RID: 7716
		private float _spinnerTweenTimer;

		// Token: 0x04001E25 RID: 7717
		[SerializeField]
		private float _maxTimeVisibleWithoutSpinner = 1.5f;

		// Token: 0x04001E26 RID: 7718
		[SerializeField]
		private float _spinnerTweenDuration = 0.4f;

		// Token: 0x04001E27 RID: 7719
		[SerializeField]
		private Easings.Functions _spinnerTweenEasing = Easings.Functions.SineEaseInOut;

		// Token: 0x04001E28 RID: 7720
		[SerializeField]
		[Tooltip("Force the loading screen to be visible for at least this many seconds. This is useful for testing the screen.")]
		private float _minTimeVisible;

		// Token: 0x04001E29 RID: 7721
		[Dependency]
		private ISoftwareCapabilities _softwareCapabilities;

		// Token: 0x04001E2A RID: 7722
		[Dependency]
		private IPersistentStorageService _storage;

		// Token: 0x04001E2B RID: 7723
		[Dependency]
		private PlayerDatabase _playerDatabase;

		// Token: 0x04001E2C RID: 7724
		[Dependency]
		private IActivePlayer _activePlayer;

		// Token: 0x04001E2D RID: 7725
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x04001E2E RID: 7726
		[Dependency]
		private IGameCenterAuthentication _gameCenterAuthentication;

		// Token: 0x04001E2F RID: 7727
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("LoadingScreen");

		// Token: 0x0200054A RID: 1354
		private enum LoadingStage
		{
			// Token: 0x04001E31 RID: 7729
			WaitingInvisibly,
			// Token: 0x04001E32 RID: 7730
			FadingIn,
			// Token: 0x04001E33 RID: 7731
			WaitingVisibly,
			// Token: 0x04001E34 RID: 7732
			FadingOut,
			// Token: 0x04001E35 RID: 7733
			Transitioned
		}

		// Token: 0x0200054B RID: 1355
		private abstract class LoadInterruptItem
		{
			// Token: 0x0600241D RID: 9245
			public abstract bool ShouldInterrupt();

			// Token: 0x0600241E RID: 9246 RVA: 0x00095A47 File Offset: 0x00093C47
			public void PresentInterruption()
			{
				this.interruptionState = LoadingScreen.LoadInterruptItem.InterruptionState.WaitingForInterruptToEnd;
				this.PresentInterruptionImpl();
			}

			// Token: 0x0600241F RID: 9247
			protected abstract void PresentInterruptionImpl();

			// Token: 0x04001E36 RID: 7734
			public LoadingScreen.LoadInterruptItem.InterruptionState interruptionState;

			// Token: 0x0200054C RID: 1356
			public enum InterruptionState
			{
				// Token: 0x04001E38 RID: 7736
				WaitingForCheck,
				// Token: 0x04001E39 RID: 7737
				WaitingForInterruptToEnd,
				// Token: 0x04001E3A RID: 7738
				Done
			}
		}

		// Token: 0x0200054D RID: 1357
		private class CloudSaveWarning : LoadingScreen.LoadInterruptItem
		{
			// Token: 0x06002421 RID: 9249 RVA: 0x00095A56 File Offset: 0x00093C56
			public CloudSaveWarning(IPersistentStorageService storage, PopupStack popupStack)
			{
				this._storage = storage;
				this._popupStack = popupStack;
			}

			// Token: 0x06002422 RID: 9250 RVA: 0x0000222C File Offset: 0x0000042C
			public override bool ShouldInterrupt()
			{
				return false;
			}

			// Token: 0x06002423 RID: 9251 RVA: 0x00095A6C File Offset: 0x00093C6C
			protected override void PresentInterruptionImpl()
			{
				this._popupStack.PushPopup<LoadScreenInterruptionPopup>(0f, false).Initialise(StringId.Options_iCloud, StringId.Options_iCloud_CacheIssue_NotSignedIn, delegate
				{
					this.interruptionState = LoadingScreen.LoadInterruptItem.InterruptionState.Done;
				});
			}

			// Token: 0x04001E3B RID: 7739
			private IPersistentStorageService _storage;

			// Token: 0x04001E3C RID: 7740
			private PopupStack _popupStack;
		}

		// Token: 0x0200054E RID: 1358
		private class GameCenterWarning : LoadingScreen.LoadInterruptItem
		{
			// Token: 0x06002425 RID: 9253 RVA: 0x00095AA0 File Offset: 0x00093CA0
			public GameCenterWarning(IGameCenterAuthentication gameCenterAuthentication, PopupStack popupStack)
			{
				this._gameCenterAuthentication = gameCenterAuthentication;
				this._popupStack = popupStack;
			}

			// Token: 0x06002426 RID: 9254 RVA: 0x00095AB6 File Offset: 0x00093CB6
			public override bool ShouldInterrupt()
			{
				return this._gameCenterAuthentication.RequiresRetry;
			}

			// Token: 0x06002427 RID: 9255 RVA: 0x00095AC3 File Offset: 0x00093CC3
			protected override void PresentInterruptionImpl()
			{
				this._popupStack.PushPopup<LoadScreenInterruptionPopup>(0f, false).Initialise(StringId.GameCenterLoginRetryRequiredTitle, StringId.GameCenterLoginRetryRequiredDescription, delegate
				{
					this.interruptionState = LoadingScreen.LoadInterruptItem.InterruptionState.Done;
				});
			}

			// Token: 0x04001E3D RID: 7741
			private IGameCenterAuthentication _gameCenterAuthentication;

			// Token: 0x04001E3E RID: 7742
			private PopupStack _popupStack;
		}
	}
}
