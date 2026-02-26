using System;

// Token: 0x02000206 RID: 518
public static class ActivePlayerExtensions
{
	// Token: 0x06000C74 RID: 3188 RVA: 0x000299E8 File Offset: 0x00027BE8
	public static bool IsAchievementCompleted(this IActivePlayer player, Enum achievementIdEnum)
	{
		AchievementDefinition achievement = player.Scope.Get<AchievementDatabase>()[achievementIdEnum];
		return achievement != null && player.IsAchievementCompleted(achievement);
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x00029A14 File Offset: 0x00027C14
	public static bool IsAchievementCompleted(this IActivePlayer player, string achievementId)
	{
		AchievementDefinition achievement = player.Scope.Get<AchievementDatabase>()[achievementId];
		return achievement != null && player.IsAchievementCompleted(achievement);
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x00029A40 File Offset: 0x00027C40
	public static void CompleteAchievement(this IActivePlayer player, Enum achievementIdEnum)
	{
		AchievementDefinition achievement = player.Scope.Get<AchievementDatabase>()[achievementIdEnum];
		if (achievement != null)
		{
			player.CompleteAchievement(achievement, true);
		}
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x00029A6C File Offset: 0x00027C6C
	public static void CompleteAchievement(this IActivePlayer player, string achievementId)
	{
		AchievementDefinition achievement = player.Scope.Get<AchievementDatabase>()[achievementId];
		if (achievement != null)
		{
			player.CompleteAchievement(achievement, true);
		}
	}
}
