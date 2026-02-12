using System;
using System.Collections.Generic;
using Factory;
using JetBrains.Annotations;
using Motorways.Processes;
using NotificationService.Events;

namespace Motorways
{
	// Token: 0x02000339 RID: 825
	public class ActivePlayer : IActivePlayer
	{
		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x060013FC RID: 5116 RVA: 0x00041B47 File Offset: 0x0003FD47
		public string Id
		{
			get
			{
				return this.Player.Id;
			}
		}

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x060013FD RID: 5117 RVA: 0x00041B54 File Offset: 0x0003FD54
		// (set) Token: 0x060013FE RID: 5118 RVA: 0x00041B61 File Offset: 0x0003FD61
		public bool IsVibrationEnabled
		{
			get
			{
				return this.UserProfile.IsVibrationEnabled;
			}
			set
			{
				this.UserProfile.IsVibrationEnabled = value;
			}
		}

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x060013FF RID: 5119 RVA: 0x00041B6F File Offset: 0x0003FD6F
		// (set) Token: 0x06001400 RID: 5120 RVA: 0x00041B7C File Offset: 0x0003FD7C
		public bool IsColorblindModeEnabled
		{
			get
			{
				return this.MotorwaysUserProfile.IsColorblindModeEnabled;
			}
			set
			{
				this.MotorwaysUserProfile.IsColorblindModeEnabled = value;
			}
		}

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06001401 RID: 5121 RVA: 0x00041B8A File Offset: 0x0003FD8A
		// (set) Token: 0x06001402 RID: 5122 RVA: 0x00041B97 File Offset: 0x0003FD97
		public bool IsSkipTransitionsEnabled
		{
			get
			{
				return this.MotorwaysUserProfile.IsSkipTransitionsEnabled;
			}
			set
			{
				this.MotorwaysUserProfile.IsSkipTransitionsEnabled = value;
			}
		}

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06001403 RID: 5123 RVA: 0x00041BA5 File Offset: 0x0003FDA5
		public bool HasAvatar
		{
			get
			{
				return this._player.HasAvatar;
			}
		}

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06001404 RID: 5124 RVA: 0x00041BB2 File Offset: 0x0003FDB2
		// (set) Token: 0x06001405 RID: 5125 RVA: 0x00041BBF File Offset: 0x0003FDBF
		public int AvatarColorIndex
		{
			get
			{
				return this._player.AvatarColorIndex;
			}
			set
			{
				this._player.AvatarColorIndex = value;
			}
		}

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06001406 RID: 5126 RVA: 0x00041BCD File Offset: 0x0003FDCD
		// (set) Token: 0x06001407 RID: 5127 RVA: 0x00041BDA File Offset: 0x0003FDDA
		public int AvatarIconIndex
		{
			get
			{
				return this._player.AvatarIconIndex;
			}
			set
			{
				this._player.AvatarIconIndex = value;
			}
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x00041BE8 File Offset: 0x0003FDE8
		public bool IsAchievementCompleted(AchievementDefinition achievementDefinition)
		{
			return this.UserProfile.IsAchievementCompleted(achievementDefinition);
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x00041BF6 File Offset: 0x0003FDF6
		public void CompleteAchievement(AchievementDefinition achievementDefinition, bool showNotification)
		{
			this.UserProfile.CompleteAchievement(achievementDefinition, showNotification);
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x00041C08 File Offset: 0x0003FE08
		private void CheckAchievementsRetroactively()
		{
			for (int achievementIndex = 0; achievementIndex < this._achievements.Count; achievementIndex++)
			{
				MotorwaysAchievementDefinition achievement = this._achievements[achievementIndex] as MotorwaysAchievementDefinition;
				if (achievement != null && achievement.IsRetroactivelySatisfied(this) && !this.IsAchievementCompleted(achievement))
				{
					this.CompleteAchievement(achievement, false);
				}
			}
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x00041C5C File Offset: 0x0003FE5C
		public void CheckLifetimeAchievements()
		{
			for (int achievementIndex = 0; achievementIndex < this._achievements.Count; achievementIndex++)
			{
				MotorwaysAchievementDefinition achievement = this._achievements[achievementIndex] as MotorwaysAchievementDefinition;
				if (achievement != null && achievement.Scale == AchievementScale.Lifetime && achievement.IsLifetimeAchievementSatisfied(this) && !this.IsAchievementCompleted(achievement))
				{
					this.CompleteAchievement(achievement, true);
				}
			}
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x00041CB7 File Offset: 0x0003FEB7
		public bool HasSeenNewContent(string newContentId)
		{
			return this.ExtendedUserProfile.HasSeenNewContent(newContentId);
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x00041CC5 File Offset: 0x0003FEC5
		public void SetNewContentSeen(string newContentId)
		{
			this.ExtendedUserProfile.SetNewContentSeen(newContentId);
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x00041CD3 File Offset: 0x0003FED3
		public void ClearNewContentSeen(string specificContent = null)
		{
			this.ExtendedUserProfile.ClearNewContentSeen(specificContent);
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x00041CE1 File Offset: 0x0003FEE1
		public GameMode GetSelectedModeForMap(string mapId)
		{
			return this.ExtendedUserProfile.GetSelectedModeForMap(mapId);
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x00041CEF File Offset: 0x0003FEEF
		public void SetSelectedGameMode(string mapName, GameMode gameMode)
		{
			this.ExtendedUserProfile.SetSelectedGameModeForMap(mapName, gameMode);
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x00041CFE File Offset: 0x0003FEFE
		public void SetTutorialTypeComplete(TutorialProgressionProcess.TutorialType completedType)
		{
			this.MotorwaysUserProfile.SetTutorialTypeComplete(completedType);
			this.CheckLifetimeAchievements();
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x00041D12 File Offset: 0x0003FF12
		public bool IsTutorialTypeCompleted(TutorialProgressionProcess.TutorialType completedType)
		{
			return this.MotorwaysUserProfile.IsTutorialTypeCompleted(completedType);
		}

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06001413 RID: 5139 RVA: 0x00041D20 File Offset: 0x0003FF20
		public bool IsAnyTutorialCompleted
		{
			get
			{
				return this.MotorwaysUserProfile.IsAnyTutorialCompleted();
			}
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x00041D2D File Offset: 0x0003FF2D
		public MotorwaysCityStatistics GetCityStatisticsForCity(string cityId, GameMode mode, bool createIfNecessary = false)
		{
			return this.MotorwaysUserProfile.GetCityStatisticsForCity(cityId, mode, createIfNecessary);
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x00041D3D File Offset: 0x0003FF3D
		[NotNull]
		public MotorwaysTimedChallengeScore GetChallengeScore(MapChallenge.ChallengeType challengeType, int expiry)
		{
			return this.MotorwaysExtendedUserProfile.GetChallengeScore(challengeType, expiry);
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x00041D4C File Offset: 0x0003FF4C
		public CityChallengeStatistics GetCityChallengeScore(string cityId, GameMode mode, int challengeIndex, bool createIfEmpty = true)
		{
			return this.MotorwaysExtendedUserProfile.GetCityChallengeScore(cityId, mode, challengeIndex, createIfEmpty);
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x00041D5E File Offset: 0x0003FF5E
		public IEnumerable<CityChallengeStatistics> GetCityChallengeScores(string cityId, GameMode mode)
		{
			return this.MotorwaysExtendedUserProfile.GetCityChallengeScores(cityId, mode);
		}

		// Token: 0x06001418 RID: 5144 RVA: 0x00041D6D File Offset: 0x0003FF6D
		public void RecordGameStatistics(IGameStatistics gameStatistics)
		{
			this.MotorwaysUserProfile.RecordGameStatistics(gameStatistics);
			this.MotorwaysExtendedUserProfile.RecordGameStatistics(gameStatistics);
		}

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06001419 RID: 5145 RVA: 0x00041D87 File Offset: 0x0003FF87
		// (set) Token: 0x0600141A RID: 5146 RVA: 0x00041D94 File Offset: 0x0003FF94
		public LocaleDatabase.LocaleId LocaleId
		{
			get
			{
				return this._player.LocaleId;
			}
			set
			{
				this._player.LocaleId = value;
			}
		}

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x0600141B RID: 5147 RVA: 0x00041DA2 File Offset: 0x0003FFA2
		// (set) Token: 0x0600141C RID: 5148 RVA: 0x00041DAF File Offset: 0x0003FFAF
		public bool SyncToCloud
		{
			get
			{
				return this.DeviceSettings.SyncToCloud;
			}
			set
			{
				this.DeviceSettings.SyncToCloud = value;
			}
		}

		// Token: 0x0600141D RID: 5149 RVA: 0x00041DBD File Offset: 0x0003FFBD
		public Dictionary<string, string> GetDeviceControlMapping(string deviceName)
		{
			return this.DeviceSettings.GetDeviceControlMapping(deviceName);
		}

		// Token: 0x0600141E RID: 5150 RVA: 0x00041DCB File Offset: 0x0003FFCB
		public void SetDeviceControlMappings(string deviceName, Dictionary<string, string> deviceControlMappings)
		{
			this.DeviceSettings.SetDeviceControlMappings(deviceName, deviceControlMappings);
		}

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x0600141F RID: 5151 RVA: 0x00041DDA File Offset: 0x0003FFDA
		// (set) Token: 0x06001420 RID: 5152 RVA: 0x00041DE7 File Offset: 0x0003FFE7
		public string ColorfulOption
		{
			get
			{
				return this.MotorwaysDeviceSettings.ColorfulOption;
			}
			set
			{
				this.MotorwaysDeviceSettings.ColorfulOption = value;
			}
		}

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06001421 RID: 5153 RVA: 0x00041DF5 File Offset: 0x0003FFF5
		// (set) Token: 0x06001422 RID: 5154 RVA: 0x00041E02 File Offset: 0x00040002
		public bool IsNightModeEnabled
		{
			get
			{
				return this.MotorwaysDeviceSettings.IsNightModeEnabled;
			}
			set
			{
				this.MotorwaysDeviceSettings.IsNightModeEnabled = value;
			}
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06001423 RID: 5155 RVA: 0x00041E10 File Offset: 0x00040010
		// (set) Token: 0x06001424 RID: 5156 RVA: 0x00041E1D File Offset: 0x0004001D
		public int AntiAliasingLevel
		{
			get
			{
				return this.MotorwaysDeviceSettings.AntiAliasingLevel;
			}
			set
			{
				this.MotorwaysDeviceSettings.AntiAliasingLevel = value;
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06001425 RID: 5157 RVA: 0x00041E2B File Offset: 0x0004002B
		public int AntiAliasingMSAALevelForUniversalRenderPipeline
		{
			get
			{
				if (this.MotorwaysDeviceSettings.AntiAliasingLevel >= 0)
				{
					return 1 << this.MotorwaysDeviceSettings.AntiAliasingLevel;
				}
				return 1;
			}
		}

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06001426 RID: 5158 RVA: 0x00041E4D File Offset: 0x0004004D
		// (set) Token: 0x06001427 RID: 5159 RVA: 0x00041E5A File Offset: 0x0004005A
		public int SelectedDisplay
		{
			get
			{
				return this.MotorwaysDeviceSettings.SelectedDisplay;
			}
			set
			{
				this.MotorwaysDeviceSettings.SelectedDisplay = value;
			}
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06001428 RID: 5160 RVA: 0x00041E68 File Offset: 0x00040068
		// (set) Token: 0x06001429 RID: 5161 RVA: 0x00041E75 File Offset: 0x00040075
		public bool IsZoomEnabled
		{
			get
			{
				return this.MotorwaysDeviceSettings.IsZoomEnabled;
			}
			set
			{
				this.MotorwaysDeviceSettings.IsZoomEnabled = value;
			}
		}

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x0600142A RID: 5162 RVA: 0x00041E83 File Offset: 0x00040083
		// (set) Token: 0x0600142B RID: 5163 RVA: 0x00041E90 File Offset: 0x00040090
		public int ZoomLevel
		{
			get
			{
				return this.MotorwaysDeviceSettings.ZoomLevel;
			}
			set
			{
				this.MotorwaysDeviceSettings.ZoomLevel = value;
			}
		}

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x0600142C RID: 5164 RVA: 0x00041E9E File Offset: 0x0004009E
		public int PreviousVolumeSetting
		{
			get
			{
				return this._previousVolume;
			}
		}

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x0600142D RID: 5165 RVA: 0x00041EA6 File Offset: 0x000400A6
		// (set) Token: 0x0600142E RID: 5166 RVA: 0x00041EB3 File Offset: 0x000400B3
		public int VolumeSetting
		{
			get
			{
				return this.MotorwaysDeviceSettings.VolumeSetting;
			}
			set
			{
				this._previousVolume = this.MotorwaysDeviceSettings.VolumeSetting;
				this.MotorwaysDeviceSettings.VolumeSetting = value;
				if (this._previousVolume == 0 && this.MotorwaysDeviceSettings.VolumeSetting == 0)
				{
					this._previousVolume = 3;
				}
			}
		}

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x0600142F RID: 5167 RVA: 0x00041EEE File Offset: 0x000400EE
		// (set) Token: 0x06001430 RID: 5168 RVA: 0x00041EFB File Offset: 0x000400FB
		public int Soundscape
		{
			get
			{
				return this.MotorwaysDeviceSettings.Soundscape;
			}
			set
			{
				this.MotorwaysDeviceSettings.Soundscape = value;
			}
		}

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06001431 RID: 5169 RVA: 0x00041F09 File Offset: 0x00040109
		public NotificationEvent? LatestNotificationEvent
		{
			get
			{
				return this.MotorwaysExtendedUserProfile.LatestNotificationEvent;
			}
		}

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06001432 RID: 5170 RVA: 0x00041F16 File Offset: 0x00040116
		public List<NotificationEvent> NotificationEvents
		{
			get
			{
				return this.MotorwaysExtendedUserProfile.NotificationEvents;
			}
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06001433 RID: 5171 RVA: 0x00041F23 File Offset: 0x00040123
		public AchievementStatistics AchievementStatistics
		{
			get
			{
				return this.MotorwaysExtendedUserProfile.AchievementStatistics;
			}
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x00041F30 File Offset: 0x00040130
		public void AddGameNotificationEvent(NotificationEvent notificationEvent)
		{
			this.MotorwaysExtendedUserProfile.AddGameNotificationEvent(notificationEvent);
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x00041F3E File Offset: 0x0004013E
		public void UpdateGameNotificationEventWithId(int id, NotificationEvent updatedNotificationEvent)
		{
			this.MotorwaysExtendedUserProfile.UpdateGameNotificationEventWithId(id, updatedNotificationEvent);
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x00041F4D File Offset: 0x0004014D
		public void RemoveAllNotificationEvents()
		{
			this.MotorwaysExtendedUserProfile.RemoveAllGameNotificationsEvents();
		}

		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06001437 RID: 5175 RVA: 0x00041F5A File Offset: 0x0004015A
		// (set) Token: 0x06001438 RID: 5176 RVA: 0x00041F67 File Offset: 0x00040167
		public bool AreMenuMessagesEnabled
		{
			get
			{
				return this.MotorwaysExtendedUserProfile.AreMenuMessagesEnabled;
			}
			set
			{
				this.MotorwaysExtendedUserProfile.AreMenuMessagesEnabled = value;
			}
		}

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06001439 RID: 5177 RVA: 0x00041F75 File Offset: 0x00040175
		// (set) Token: 0x0600143A RID: 5178 RVA: 0x00041F82 File Offset: 0x00040182
		public bool HasSeenCreativeInGameMessage
		{
			get
			{
				return this.MotorwaysExtendedUserProfile.CreativeInGameMessageSeen;
			}
			set
			{
				this.MotorwaysExtendedUserProfile.CreativeInGameMessageSeen = value;
			}
		}

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x0600143B RID: 5179 RVA: 0x00041F90 File Offset: 0x00040190
		// (set) Token: 0x0600143C RID: 5180 RVA: 0x00041F9D File Offset: 0x0004019D
		public bool IsChallengeRemindersEnabledSetting
		{
			get
			{
				return this.MotorwaysDeviceSettings.IsChallengeRemindersEnabledSetting;
			}
			set
			{
				this.MotorwaysDeviceSettings.IsChallengeRemindersEnabledSetting = value;
			}
		}

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x0600143D RID: 5181 RVA: 0x00041FAB File Offset: 0x000401AB
		// (set) Token: 0x0600143E RID: 5182 RVA: 0x00041FB8 File Offset: 0x000401B8
		public bool IsContentRemindersEnabledSetting
		{
			get
			{
				return this.MotorwaysDeviceSettings.IsContentRemindersEnabledSetting;
			}
			set
			{
				this.MotorwaysDeviceSettings.IsContentRemindersEnabledSetting = value;
			}
		}

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x0600143F RID: 5183 RVA: 0x00041FC6 File Offset: 0x000401C6
		// (set) Token: 0x06001440 RID: 5184 RVA: 0x00041FD3 File Offset: 0x000401D3
		public bool IsTapDrawEnabled
		{
			get
			{
				return this.MotorwaysExtendedUserProfile.IsTapDrawEnabled;
			}
			set
			{
				this.MotorwaysExtendedUserProfile.IsTapDrawEnabled = value;
			}
		}

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06001441 RID: 5185 RVA: 0x00041FE1 File Offset: 0x000401E1
		// (set) Token: 0x06001442 RID: 5186 RVA: 0x00041FEE File Offset: 0x000401EE
		public int ControllerSensitivity
		{
			get
			{
				return this.MotorwaysExtendedUserProfile.ControllerSensitivity;
			}
			set
			{
				this.MotorwaysExtendedUserProfile.ControllerSensitivity = value;
			}
		}

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06001443 RID: 5187 RVA: 0x00041FFC File Offset: 0x000401FC
		// (set) Token: 0x06001444 RID: 5188 RVA: 0x00042009 File Offset: 0x00040209
		public bool IsDrawModeToggleEnabled
		{
			get
			{
				return this.MotorwaysExtendedUserProfile.IsDrawModeToggleEnabled;
			}
			set
			{
				this.MotorwaysExtendedUserProfile.IsDrawModeToggleEnabled = value;
			}
		}

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06001445 RID: 5189 RVA: 0x00042017 File Offset: 0x00040217
		// (set) Token: 0x06001446 RID: 5190 RVA: 0x00042024 File Offset: 0x00040224
		public bool DoesHudStartLocked
		{
			get
			{
				return this.MotorwaysExtendedUserProfile.DoesHudStartLocked;
			}
			set
			{
				this.MotorwaysExtendedUserProfile.DoesHudStartLocked = value;
			}
		}

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06001447 RID: 5191 RVA: 0x00042032 File Offset: 0x00040232
		// (set) Token: 0x06001448 RID: 5192 RVA: 0x0004203F File Offset: 0x0004023F
		public bool IsTelemetryEnabled
		{
			get
			{
				return this.MotorwaysExtendedUserProfile.IsTelemetryEnabled;
			}
			set
			{
				this.MotorwaysExtendedUserProfile.IsTelemetryEnabled = value;
			}
		}

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06001449 RID: 5193 RVA: 0x0004204D File Offset: 0x0004024D
		public bool HasLocalSavedGame
		{
			get
			{
				return this._player.HasLocalSavedGame;
			}
		}

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x0600144A RID: 5194 RVA: 0x0004205A File Offset: 0x0004025A
		// (set) Token: 0x0600144B RID: 5195 RVA: 0x00042068 File Offset: 0x00040268
		public IGameJournalSave LocalSavedGame
		{
			get
			{
				return this._player.LocalSavedGame;
			}
			set
			{
				IGameJournalSave oldSavedGame = this._player.LocalSavedGame;
				if (oldSavedGame != null && value == null)
				{
					this._storage.Delete(oldSavedGame);
				}
				this._player.LocalSavedGame = value;
				if (value != null)
				{
					this._storage.Store(value, new StoreCompleted(this.OnSaveCompleted));
				}
			}
		}

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x0600144C RID: 5196 RVA: 0x000420BC File Offset: 0x000402BC
		public bool DidFailLastSave
		{
			get
			{
				return this._didFailLastSave;
			}
		}

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x0600144D RID: 5197 RVA: 0x000420C4 File Offset: 0x000402C4
		public bool HasNotifiedPlayerOfSaveFailure
		{
			get
			{
				return this._hasNotifiedPlayerOfSaveFailure;
			}
		}

		// Token: 0x0600144E RID: 5198 RVA: 0x000420CC File Offset: 0x000402CC
		public void NotifyPlayerOfSaveFailure()
		{
			this._hasNotifiedPlayerOfSaveFailure = true;
		}

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x0600144F RID: 5199 RVA: 0x000420D5 File Offset: 0x000402D5
		public bool HasForeignSavedGames
		{
			get
			{
				return this._player.HasForeignSavedGames;
			}
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x000420E2 File Offset: 0x000402E2
		public void AddForeignSavedGame(IGameJournalSave newForeignSavedGame)
		{
			this._player.AddForeignSavedGame(newForeignSavedGame);
			Action savedGamesChanged = this.SavedGamesChanged;
			if (savedGamesChanged == null)
			{
				return;
			}
			savedGamesChanged();
		}

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06001451 RID: 5201 RVA: 0x00042100 File Offset: 0x00040300
		public IEnumerable<IGameJournalSave> ForeignSavedGames
		{
			get
			{
				return this._player.ForeignSavedGames;
			}
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x0004210D File Offset: 0x0004030D
		public IGameJournalSave GetForeignSavedGame(string gameId)
		{
			return this._player.GetForeignSavedGame(gameId);
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x0004211B File Offset: 0x0004031B
		public void RemoveSavedGame(IGameJournalSave savedGame)
		{
			this._player.RemoveSavedGame(savedGame);
			Action savedGamesChanged = this.SavedGamesChanged;
			if (savedGamesChanged == null)
			{
				return;
			}
			savedGamesChanged();
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x00042139 File Offset: 0x00040339
		public void Touch()
		{
			ActivePlayer.Log.Info("Touching player {0}.", new object[]
			{
				this._player.Id
			});
			this._player.DeviceSettings.LastPlayedUtcTime = DateTime.UtcNow;
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x00042174 File Offset: 0x00040374
		public void ActivatePlayer(Player newActivePlayer)
		{
			ActivePlayer.Log.Info("Activating player {0}.", new object[]
			{
				newActivePlayer.Id
			});
			Player oldPlayer = this._player;
			if (oldPlayer != null)
			{
				oldPlayer.DataChanged -= this.OnDataChanged;
				oldPlayer.SavedGamesChanged -= this.OnSavedGamesChanged;
			}
			this._player = newActivePlayer;
			this._player.DataChanged += this.OnDataChanged;
			this._player.SavedGamesChanged += this.OnSavedGamesChanged;
			PlayedChangedEventHandler playerChanged = this.PlayerChanged;
			if (playerChanged != null)
			{
				playerChanged(oldPlayer, newActivePlayer);
			}
			Action dataChanged = this.DataChanged;
			if (dataChanged != null)
			{
				dataChanged();
			}
			Action savedGamesChanged = this.SavedGamesChanged;
			if (savedGamesChanged != null)
			{
				savedGamesChanged();
			}
			this.CheckAchievementsRetroactively();
			this.Touch();
		}

		// Token: 0x14000037 RID: 55
		// (add) Token: 0x06001456 RID: 5206 RVA: 0x00042244 File Offset: 0x00040444
		// (remove) Token: 0x06001457 RID: 5207 RVA: 0x0004227C File Offset: 0x0004047C
		public event Action DataChanged;

		// Token: 0x14000038 RID: 56
		// (add) Token: 0x06001458 RID: 5208 RVA: 0x000422B4 File Offset: 0x000404B4
		// (remove) Token: 0x06001459 RID: 5209 RVA: 0x000422EC File Offset: 0x000404EC
		public event Action SavedGamesChanged;

		// Token: 0x14000039 RID: 57
		// (add) Token: 0x0600145A RID: 5210 RVA: 0x00042324 File Offset: 0x00040524
		// (remove) Token: 0x0600145B RID: 5211 RVA: 0x0004235C File Offset: 0x0004055C
		public event PlayedChangedEventHandler PlayerChanged;

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x0600145C RID: 5212 RVA: 0x00042391 File Offset: 0x00040591
		public ILegacyUserProfile UserProfile
		{
			get
			{
				return this._player.UserProfile;
			}
		}

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x0600145D RID: 5213 RVA: 0x0004239E File Offset: 0x0004059E
		public IExtendedUserProfile ExtendedUserProfile
		{
			get
			{
				return this._player.ExtendedUserProfile;
			}
		}

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x0600145E RID: 5214 RVA: 0x000423AB File Offset: 0x000405AB
		public LegacyMotorwaysUserProfile MotorwaysUserProfile
		{
			get
			{
				return (LegacyMotorwaysUserProfile)this._player.UserProfile;
			}
		}

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x0600145F RID: 5215 RVA: 0x000423BD File Offset: 0x000405BD
		public MotorwaysExtendedUserProfile MotorwaysExtendedUserProfile
		{
			get
			{
				return (MotorwaysExtendedUserProfile)this._player.ExtendedUserProfile;
			}
		}

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06001460 RID: 5216 RVA: 0x000423CF File Offset: 0x000405CF
		public IDeviceSettings DeviceSettings
		{
			get
			{
				return this._player.DeviceSettings;
			}
		}

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06001461 RID: 5217 RVA: 0x000423DC File Offset: 0x000405DC
		public MotorwaysDeviceSettings MotorwaysDeviceSettings
		{
			get
			{
				return (MotorwaysDeviceSettings)this._player.DeviceSettings;
			}
		}

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06001462 RID: 5218 RVA: 0x000423EE File Offset: 0x000405EE
		public Player Player
		{
			get
			{
				return this._player;
			}
		}

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06001463 RID: 5219 RVA: 0x000423F6 File Offset: 0x000405F6
		public bool HasActivePlayer
		{
			get
			{
				return this._player != null;
			}
		}

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06001464 RID: 5220 RVA: 0x00042401 File Offset: 0x00040601
		// (set) Token: 0x06001465 RID: 5221 RVA: 0x00042409 File Offset: 0x00040609
		[Dependency]
		public IScope Scope { get; private set; }

		// Token: 0x06001466 RID: 5222 RVA: 0x00042412 File Offset: 0x00040612
		private void OnSaveCompleted(StoreOperationResult result)
		{
			if (result == StoreOperationResult.Failed)
			{
				this._didFailLastSave = true;
				this._hasNotifiedPlayerOfSaveFailure = false;
			}
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x00042425 File Offset: 0x00040625
		private void OnDataChanged()
		{
			Action dataChanged = this.DataChanged;
			if (dataChanged == null)
			{
				return;
			}
			dataChanged();
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x00042437 File Offset: 0x00040637
		private void OnSavedGamesChanged()
		{
			Action savedGamesChanged = this.SavedGamesChanged;
			if (savedGamesChanged == null)
			{
				return;
			}
			savedGamesChanged();
		}

		// Token: 0x040010B3 RID: 4275
		[Dependency]
		private AchievementDatabase _achievements;

		// Token: 0x040010B4 RID: 4276
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ActivePlayer");

		// Token: 0x040010B5 RID: 4277
		private bool _didFailLastSave;

		// Token: 0x040010B6 RID: 4278
		private bool _hasNotifiedPlayerOfSaveFailure;

		// Token: 0x040010B7 RID: 4279
		private int _previousVolume;

		// Token: 0x040010B8 RID: 4280
		private const int DefaultVolumeSetting = 3;

		// Token: 0x040010BC RID: 4284
		[Dependency]
		private IPersistentStorageService _storage;

		// Token: 0x040010BE RID: 4286
		private Player _player;
	}
}
