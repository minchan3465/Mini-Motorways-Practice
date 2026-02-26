using System;

// Token: 0x020000E5 RID: 229
public interface IAchievementHandler
{
	// Token: 0x060004B4 RID: 1204
	void OnAppStart();

	// Token: 0x060004B5 RID: 1205
	bool CompleteAchievement(Achievement achievement, bool showNotification);

	// Token: 0x060004B6 RID: 1206
	bool IsAchievementCompleted(AchievementDefinition achievement);

	// Token: 0x060004B7 RID: 1207
	bool IncrementStatistic(string statisticId, int increment);
}
