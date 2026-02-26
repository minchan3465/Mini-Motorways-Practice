using System;

// Token: 0x020000D7 RID: 215
public class SteamworksAchievementHandler : IAchievementHandler
{
	// Token: 0x0600046A RID: 1130 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnAppStart()
	{
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x0000F644 File Offset: 0x0000D844
	public bool CompleteAchievement(Achievement achievement, bool showNotification)
	{
		AchievementDefinition definition = achievement.Definition;
		string achievementId;
		return definition != null && definition.TryGetStringDataForPlatformAndKey(AchievementData.AchievementPlatform.Steamworks, AchievementData.AchievementDataType.PlatformId, out achievementId) && SteamworksShared.CompleteAchievement(achievementId);
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x0000F674 File Offset: 0x0000D874
	public bool IsAchievementCompleted(AchievementDefinition achievement)
	{
		string achievementId;
		return achievement.TryGetStringDataForPlatformAndKey(AchievementData.AchievementPlatform.Steamworks, AchievementData.AchievementDataType.PlatformId, out achievementId) && SteamworksShared.IsAchievementCompleted(achievementId);
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x0000F695 File Offset: 0x0000D895
	public bool IncrementStatistic(string statisticId, int increment)
	{
		return increment <= 0 || SteamworksShared.IncrementStatistic(statisticId, increment);
	}
}
