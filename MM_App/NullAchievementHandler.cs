using System;

// Token: 0x020000B5 RID: 181
public class NullAchievementHandler : IAchievementHandler
{
	// Token: 0x06000348 RID: 840 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnAppStart()
	{
	}

	// Token: 0x06000349 RID: 841 RVA: 0x000020AA File Offset: 0x000002AA
	public bool CompleteAchievement(Achievement achievement, bool showNotification)
	{
		return true;
	}

	// Token: 0x0600034A RID: 842 RVA: 0x000020AA File Offset: 0x000002AA
	public bool IsAchievementCompleted(AchievementDefinition achievement)
	{
		return true;
	}

	// Token: 0x0600034B RID: 843 RVA: 0x0000222C File Offset: 0x0000042C
	public bool IncrementStatistic(string statisticId, int increment)
	{
		return false;
	}

	// Token: 0x0400015B RID: 347
	public static NullAchievementHandler Instance = new NullAchievementHandler();
}
