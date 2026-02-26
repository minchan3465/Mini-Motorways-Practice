using System;
using System.Collections.Generic;
using Factory;

// Token: 0x02000205 RID: 517
public interface IActivePlayer
{
	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x06000C47 RID: 3143
	string Id { get; }

	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x06000C48 RID: 3144
	// (set) Token: 0x06000C49 RID: 3145
	bool IsVibrationEnabled { get; set; }

	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x06000C4A RID: 3146
	bool HasAvatar { get; }

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x06000C4B RID: 3147
	// (set) Token: 0x06000C4C RID: 3148
	int AvatarColorIndex { get; set; }

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x06000C4D RID: 3149
	// (set) Token: 0x06000C4E RID: 3150
	int AvatarIconIndex { get; set; }

	// Token: 0x06000C4F RID: 3151
	bool IsAchievementCompleted(AchievementDefinition achievementDefinition);

	// Token: 0x06000C50 RID: 3152
	void CompleteAchievement(AchievementDefinition achievementDefinition, bool showNotification);

	// Token: 0x06000C51 RID: 3153
	bool HasSeenNewContent(string newContentId);

	// Token: 0x06000C52 RID: 3154
	void SetNewContentSeen(string newContentId);

	// Token: 0x06000C53 RID: 3155
	void ClearNewContentSeen(string specificContent = null);

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x06000C54 RID: 3156
	// (set) Token: 0x06000C55 RID: 3157
	LocaleDatabase.LocaleId LocaleId { get; set; }

	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x06000C56 RID: 3158
	// (set) Token: 0x06000C57 RID: 3159
	bool SyncToCloud { get; set; }

	// Token: 0x06000C58 RID: 3160
	Dictionary<string, string> GetDeviceControlMapping(string deviceName);

	// Token: 0x06000C59 RID: 3161
	void SetDeviceControlMappings(string deviceName, Dictionary<string, string> deviceControlMappings);

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x06000C5A RID: 3162
	bool HasLocalSavedGame { get; }

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x06000C5B RID: 3163
	// (set) Token: 0x06000C5C RID: 3164
	IGameJournalSave LocalSavedGame { get; set; }

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x06000C5D RID: 3165
	// (set) Token: 0x06000C5E RID: 3166
	bool IsChallengeRemindersEnabledSetting { get; set; }

	// Token: 0x170002AC RID: 684
	// (get) Token: 0x06000C5F RID: 3167
	// (set) Token: 0x06000C60 RID: 3168
	bool IsContentRemindersEnabledSetting { get; set; }

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x06000C61 RID: 3169
	bool HasForeignSavedGames { get; }

	// Token: 0x06000C62 RID: 3170
	void AddForeignSavedGame(IGameJournalSave newForeignSavedGame);

	// Token: 0x170002AE RID: 686
	// (get) Token: 0x06000C63 RID: 3171
	IEnumerable<IGameJournalSave> ForeignSavedGames { get; }

	// Token: 0x06000C64 RID: 3172
	IGameJournalSave GetForeignSavedGame(string gameId);

	// Token: 0x06000C65 RID: 3173
	void RemoveSavedGame(IGameJournalSave savedGame);

	// Token: 0x06000C66 RID: 3174
	void Touch();

	// Token: 0x06000C67 RID: 3175
	void ActivatePlayer(Player newActivePlayer);

	// Token: 0x170002AF RID: 687
	// (get) Token: 0x06000C68 RID: 3176
	Player Player { get; }

	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x06000C69 RID: 3177
	bool HasActivePlayer { get; }

	// Token: 0x170002B1 RID: 689
	// (get) Token: 0x06000C6A RID: 3178
	ILegacyUserProfile UserProfile { get; }

	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x06000C6B RID: 3179
	IExtendedUserProfile ExtendedUserProfile { get; }

	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x06000C6C RID: 3180
	IDeviceSettings DeviceSettings { get; }

	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x06000C6D RID: 3181
	IScope Scope { get; }

	// Token: 0x14000029 RID: 41
	// (add) Token: 0x06000C6E RID: 3182
	// (remove) Token: 0x06000C6F RID: 3183
	event Action DataChanged;

	// Token: 0x1400002A RID: 42
	// (add) Token: 0x06000C70 RID: 3184
	// (remove) Token: 0x06000C71 RID: 3185
	event Action SavedGamesChanged;

	// Token: 0x1400002B RID: 43
	// (add) Token: 0x06000C72 RID: 3186
	// (remove) Token: 0x06000C73 RID: 3187
	event PlayedChangedEventHandler PlayerChanged;
}
