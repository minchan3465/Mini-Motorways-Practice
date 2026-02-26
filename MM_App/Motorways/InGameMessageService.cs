using System;
using System.Collections;
using System.Collections.Generic;
using Factory;
using Motorways.Views;
using NotificationService.Events;
using UnityEngine;
using UnityEngine.Networking;

namespace Motorways
{
	// Token: 0x020003AD RID: 941
	public class InGameMessageService : ICreatedInScopeHandler, MainMenuScreen.IObserver
	{
		// Token: 0x06001654 RID: 5716 RVA: 0x0004D3EF File Offset: 0x0004B5EF
		public void OnCreatedInScope(IScope scope)
		{
			this._mainMenu.Subscribe(this);
		}

		// Token: 0x06001655 RID: 5717 RVA: 0x0004D400 File Offset: 0x0004B600
		public void OnMainMenuTransitionedIn()
		{
			if (!this._hasShowniCloudWarning && Application.platform == RuntimePlatform.tvOS)
			{
				if ((this._storage.Status.issues & PersistentStorageServiceIssues.NotAuthenticated) == PersistentStorageServiceIssues.NotAuthenticated)
				{
					this._messenger.DisplayMessage(StandaloneLocString.CreateString(this._scope, StringId.iCloudNotLoggedIn));
					this._hasShowniCloudWarning = true;
				}
				else if ((this._storage.Status.issues & PersistentStorageServiceIssues.NotAvailable) == PersistentStorageServiceIssues.NotAvailable)
				{
					this._messenger.DisplayMessage(StandaloneLocString.CreateString(this._scope, StringId.iCloudNotConnectedToInternet));
					this._hasShowniCloudWarning = true;
				}
				if (this._hasShowniCloudWarning)
				{
					return;
				}
			}
			if (this.AreMenuMessagesEnabled())
			{
				this._mainMenu.StartCoroutine(this.DoDisplayMessages());
			}
		}

		// Token: 0x06001656 RID: 5718 RVA: 0x0004D4B0 File Offset: 0x0004B6B0
		private bool AreMenuMessagesEnabled()
		{
			return !FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo) && this._player.AreMenuMessagesEnabled;
		}

		// Token: 0x06001657 RID: 5719 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnMainMenuTransitionOut()
		{
		}

		// Token: 0x06001658 RID: 5720 RVA: 0x0004D4C8 File Offset: 0x0004B6C8
		private IEnumerator DoDisplayMessages()
		{
			yield return this._mainMenu.StartCoroutine(this.DoVersionUpdateCheck());
			StandaloneLocString newMessage;
			if (this.HasMessageToDisplay(out newMessage))
			{
				this._messenger.DisplayMessage(newMessage);
			}
			yield break;
		}

		// Token: 0x06001659 RID: 5721 RVA: 0x0004D4D7 File Offset: 0x0004B6D7
		private IEnumerator DoVersionUpdateCheck()
		{
			string url = this.GetVersionCheckUrl();
			if (!Diagnostics.Verify(!string.IsNullOrEmpty(url), "No URL found for this platform"))
			{
				yield break;
			}
			UnityWebRequest webRequest = UnityWebRequest.Get(url);
			yield return webRequest.SendWebRequest();
			if (!Diagnostics.Verify(webRequest.result == UnityWebRequest.Result.Success, "Failed to get request from {0} with error {1}", url, webRequest.error))
			{
				yield break;
			}
			JSON.Dictionary dictionary = JSON.ToDictionary(JSON.LoadFromString(webRequest.downloadHandler.text));
			JSON.Dictionary dictionary2;
			if (dictionary == null)
			{
				dictionary2 = null;
			}
			else
			{
				JSON.Array array = dictionary.GetArray("results");
				dictionary2 = ((array != null) ? array.GetDictionary(0) : null);
			}
			JSON.Dictionary results = dictionary2;
			if (!Diagnostics.Verify(results != null, "Failed to find results in json: {0}", webRequest.downloadHandler.text))
			{
				yield break;
			}
			this._appStoreVersion = new System.Version(results.GetString("version"));
			yield break;
		}

		// Token: 0x0600165A RID: 5722 RVA: 0x00047AF0 File Offset: 0x00045CF0
		private string GetVersionCheckUrl()
		{
			return string.Empty;
		}

		// Token: 0x0600165B RID: 5723 RVA: 0x0004D4E6 File Offset: 0x0004B6E6
		private bool HasMessageToDisplay(out StandaloneLocString result)
		{
			return this.CheckForNewVersionMessage(out result) || this.CheckCreativeModeMessage(out result) || this.CheckResumeGameMessage(out result) || this.CheckWeeklyChallengeMessage(out result) || this.CheckDailyChallengeMessage(out result);
		}

		// Token: 0x0600165C RID: 5724 RVA: 0x0004D518 File Offset: 0x0004B718
		private bool CheckForNewVersionMessage(out StandaloneLocString message)
		{
			if (this._appStoreVersion == null)
			{
				message = null;
				return false;
			}
			System.Version appVersion = new System.Version(Application.version);
			if (this._appStoreVersion.CompareTo(appVersion) > 0 && !this._messagesSeenThisSession.Contains(StringId.InGame_Messages_RecurringNewUpdate_Text))
			{
				message = StandaloneLocString.CreateString(this._scope, StringId.InGame_Messages_RecurringNewUpdate_Text);
				this._messagesSeenThisSession.Add(StringId.InGame_Messages_RecurringNewUpdate_Text);
				return true;
			}
			message = null;
			return false;
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x0004D590 File Offset: 0x0004B790
		private bool CheckTutorialMessage(out StandaloneLocString message)
		{
			DateTime lastTutorial;
			if (!this._player.IsAnyTutorialCompleted && this.HasPlayedTutorialInLast30Days(out lastTutorial) && (lastTutorial - GameDateTime.LocalToday).TotalDays > 1.0 && !this._player.HasSeenNewContent("TutorialInGameMessagePromptKey"))
			{
				message = StandaloneLocString.CreateString(this._scope, StringId.InGame_Messages_1OffTutorial1Day_Text);
				return true;
			}
			message = null;
			return false;
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x0004D5FC File Offset: 0x0004B7FC
		private bool HasPlayedTutorialInLast30Days(out DateTime datePlayed)
		{
			foreach (NotificationEvent notificationEvent in this._events.AllEvents)
			{
				PlayedMap playedMapEvent = notificationEvent.EventType as PlayedMap;
				if (playedMapEvent != null && playedMapEvent.Map == MapDefinition.CityNames.None)
				{
					datePlayed = notificationEvent.OccuredAt;
					return true;
				}
			}
			datePlayed = GameDateTime.LocalToday;
			return false;
		}

		// Token: 0x0600165F RID: 5727 RVA: 0x0004D684 File Offset: 0x0004B884
		private bool CheckResumeGameMessage(out StandaloneLocString message)
		{
			if (this._player.HasLocalSavedGame && !this._messagesSeenThisSession.Contains(StringId.InGame_Messages_RecurringResumeSavedGame_Text) && (this._player.LocalSavedGame.UtcTimestamp - GameDateTime.UtcToday).TotalDays > 3.0)
			{
				this._messagesSeenThisSession.Add(StringId.InGame_Messages_RecurringResumeSavedGame_Text);
				message = StandaloneLocString.CreateString(this._scope, StringId.InGame_Messages_RecurringResumeSavedGame_Text);
				return true;
			}
			message = null;
			return false;
		}

		// Token: 0x06001660 RID: 5728 RVA: 0x0004D706 File Offset: 0x0004B906
		private bool CheckCreativeModeMessage(out StandaloneLocString message)
		{
			if (this._player.HasSeenCreativeInGameMessage)
			{
				message = null;
				return false;
			}
			this._player.HasSeenCreativeInGameMessage = true;
			message = StandaloneLocString.CreateString(this._scope, StringId.InGame_Messages_CreativeMode);
			return true;
		}

		// Token: 0x06001661 RID: 5729 RVA: 0x0004D73C File Offset: 0x0004B93C
		private bool CheckWeeklyChallengeMessage(out StandaloneLocString message)
		{
			if (!this._challengeSystem.AreChallengesUnlocked(this._player) || !this._softwareCapabilities.AllowsTimedChallengeMessages())
			{
				message = null;
				return false;
			}
			int expiry = this._challengeSystem.WeeklyChallenge.TimeEnd;
			if (this._player.GetChallengeScore(MapChallenge.ChallengeType.Weekly, expiry).Score > 0)
			{
				message = null;
				return false;
			}
			int hoursLeft = this._challengeSystem.WeeklyChallenge.SecondsLeft / 60 / 60;
			int daysLeft = hoursLeft / 24;
			if (!this._messagesSeenThisSession.Contains(StringId.Local_Notifications_RecurringWC1Day_Text) && daysLeft > 6)
			{
				this._messagesSeenThisSession.Add(StringId.Local_Notifications_RecurringWC1Day_Text);
				message = StandaloneLocString.CreateString(this._scope, StringId.Local_Notifications_RecurringWC1Day_Text);
				return true;
			}
			if (!this._messagesSeenThisSession.Contains(StringId.InGame_Messages_RecurringWC6Days_Text) && hoursLeft <= 24)
			{
				this._messagesSeenThisSession.Add(StringId.InGame_Messages_RecurringWC6Days_Text);
				message = this.CreateStringKeyWithIntParameter(StringId.InGame_Messages_RecurringWC6Days_Text, StringParameterId.Hour, hoursLeft);
				return true;
			}
			message = null;
			return false;
		}

		// Token: 0x06001662 RID: 5730 RVA: 0x0004D830 File Offset: 0x0004BA30
		public bool CheckDailyChallengeMessage(out StandaloneLocString message)
		{
			if (!this._challengeSystem.AreChallengesUnlocked(this._player) || !this._softwareCapabilities.AllowsTimedChallengeMessages())
			{
				message = null;
				return false;
			}
			int expiry = this._challengeSystem.DailyChallenge.TimeEnd;
			if (this._player.GetChallengeScore(MapChallenge.ChallengeType.Daily, expiry).Score > 0)
			{
				message = null;
				return false;
			}
			int hoursLeft = this._challengeSystem.DailyChallenge.SecondsLeft / 60 / 60;
			if (!this._messagesSeenThisSession.Contains(StringId.InGame_Messages_RecurringDC3Hours_Text) && hoursLeft < 3)
			{
				this._messagesSeenThisSession.Add(StringId.InGame_Messages_RecurringDC3Hours_Text);
				message = StandaloneLocString.CreateString(this._scope, StringId.InGame_Messages_RecurringDC3Hours_Text);
				return true;
			}
			if (!this._messagesSeenThisSession.Contains(StringId.InGame_Messages_RecurringDC20Hours_Text) && hoursLeft > 20)
			{
				this._messagesSeenThisSession.Add(StringId.InGame_Messages_RecurringDC20Hours_Text);
				message = this.CreateStringKeyWithIntParameter(StringId.InGame_Messages_RecurringDC20Hours_Text, StringParameterId.Hour, hoursLeft);
				return true;
			}
			message = null;
			return false;
		}

		// Token: 0x06001663 RID: 5731 RVA: 0x0004D91C File Offset: 0x0004BB1C
		public StandaloneLocString CreateStringKeyWithIntParameter(StringId stringId, StringParameterId parameterType, int value)
		{
			MotorwaysStringKey titleKey = this._scope.Get<MotorwaysStringKey>();
			titleKey.InitWithStringId(stringId, value, new Dictionary<string, string>
			{
				{
					parameterType.ToString(),
					value.ToString()
				}
			});
			return StandaloneLocString.CreateString(this._scope, titleKey);
		}

		// Token: 0x04001307 RID: 4871
		[Dependency]
		private Scope _scope;

		// Token: 0x04001308 RID: 4872
		[Dependency]
		private MainMenuScreen _mainMenu;

		// Token: 0x04001309 RID: 4873
		[Dependency]
		private InGameMessageUIManager _messenger;

		// Token: 0x0400130A RID: 4874
		[Dependency]
		private ChallengeSystem _challengeSystem;

		// Token: 0x0400130B RID: 4875
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x0400130C RID: 4876
		[Dependency]
		private INotificationEventSystem _events;

		// Token: 0x0400130D RID: 4877
		[Dependency]
		private IPersistentStorageService _storage;

		// Token: 0x0400130E RID: 4878
		[Dependency]
		private ISoftwareCapabilities _softwareCapabilities;

		// Token: 0x0400130F RID: 4879
		[Serialize(false, null)]
		private HashSet<StringId> _messagesSeenThisSession = new HashSet<StringId>();

		// Token: 0x04001310 RID: 4880
		private System.Version _appStoreVersion;

		// Token: 0x04001311 RID: 4881
		private bool _hasShowniCloudWarning;
	}
}
