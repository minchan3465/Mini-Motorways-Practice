using System;

// Token: 0x0200020F RID: 527
public interface ILegacyUserProfile : IJsonSerializableSaveData, IStorable
{
	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x06000CA2 RID: 3234
	// (set) Token: 0x06000CA3 RID: 3235
	Player Player { get; set; }

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x06000CA4 RID: 3236
	// (set) Token: 0x06000CA5 RID: 3237
	bool IsVibrationEnabled { get; set; }

	// Token: 0x06000CA6 RID: 3238
	bool IsAchievementCompleted(AchievementDefinition achievementDefinition);

	// Token: 0x06000CA7 RID: 3239
	void CompleteAchievement(AchievementDefinition achievementDefinition, bool showNotification);

	// Token: 0x06000CA8 RID: 3240
	void RecordGameStatistics(IGameStatistics gameStatistics);
}
