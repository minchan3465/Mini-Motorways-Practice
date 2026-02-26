using System;
using Motorways;

// Token: 0x0200020B RID: 523
public interface IExtendedUserProfile : IJsonSerializableSaveData, IStorable
{
	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x06000C86 RID: 3206
	int Version { get; }

	// Token: 0x170002BA RID: 698
	// (get) Token: 0x06000C87 RID: 3207
	// (set) Token: 0x06000C88 RID: 3208
	Player Player { get; set; }

	// Token: 0x170002BB RID: 699
	// (get) Token: 0x06000C89 RID: 3209
	// (set) Token: 0x06000C8A RID: 3210
	int AvatarColorIndex { get; set; }

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x06000C8B RID: 3211
	// (set) Token: 0x06000C8C RID: 3212
	int AvatarIconIndex { get; set; }

	// Token: 0x170002BD RID: 701
	// (get) Token: 0x06000C8D RID: 3213
	// (set) Token: 0x06000C8E RID: 3214
	iCloudProvenance iCloudProvenance { get; set; }

	// Token: 0x170002BE RID: 702
	// (get) Token: 0x06000C8F RID: 3215
	// (set) Token: 0x06000C90 RID: 3216
	int LastTimeDailyChallengeSeen { get; set; }

	// Token: 0x170002BF RID: 703
	// (get) Token: 0x06000C91 RID: 3217
	// (set) Token: 0x06000C92 RID: 3218
	int LastTimeWeeklyChallengeSeen { get; set; }

	// Token: 0x06000C93 RID: 3219
	bool HasSeenNewContent(string newContentId);

	// Token: 0x06000C94 RID: 3220
	void SetNewContentSeen(string newContentId);

	// Token: 0x06000C95 RID: 3221
	void ClearNewContentSeen(string specificContent = null);

	// Token: 0x06000C96 RID: 3222
	GameMode GetSelectedModeForMap(string mapId);

	// Token: 0x06000C97 RID: 3223
	void SetSelectedGameModeForMap(string mapId, GameMode gameMode);
}
