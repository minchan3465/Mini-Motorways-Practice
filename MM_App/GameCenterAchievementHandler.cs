using System;
using Factory;
using Helpers.GameCenter;
using Motorways;

// Token: 0x020000CD RID: 205
public class GameCenterAchievementHandler : IAchievementHandler
{
	// Token: 0x0600042C RID: 1068 RVA: 0x0000F470 File Offset: 0x0000D670
	public bool CompleteAchievement(Achievement achievement, bool showNotification)
	{
		string achievementId;
		return this._gameCenterAuthentication.IsAuthenticated && GameCenterShared.GCAreAchievementsReady() && GameCenterAchievementHandler.TryGetPlatformAchievementId(achievement, out achievementId) && GameCenterShared.GCSetAchievement(achievementId, showNotification);
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x0000F4A8 File Offset: 0x0000D6A8
	public bool IsAchievementCompleted(AchievementDefinition achievement)
	{
		string achievementId;
		return this._gameCenterAuthentication.IsAuthenticated && GameCenterAchievementHandler.TryGetPlatformAchievementId(achievement, out achievementId) && GameCenterShared.GCIsAchievementComplete(achievementId);
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x0000222C File Offset: 0x0000042C
	public bool IncrementStatistic(string statisticId, int increment)
	{
		return false;
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x0000F4D6 File Offset: 0x0000D6D6
	private static bool TryGetPlatformAchievementId(Achievement fromAchievement, out string result)
	{
		if (Diagnostics.Verify(fromAchievement != null) && Diagnostics.Verify(fromAchievement.Definition != null))
		{
			return GameCenterAchievementHandler.TryGetPlatformAchievementId(fromAchievement.Definition, out result);
		}
		result = "";
		return false;
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x0000F508 File Offset: 0x0000D708
	private static bool TryGetPlatformAchievementId(AchievementDefinition fromAchievementDefinition, out string result)
	{
		if (!Diagnostics.Verify(fromAchievementDefinition != null))
		{
			result = "";
			return false;
		}
		return fromAchievementDefinition.TryGetStringDataForPlatformAndKey(AchievementData.AchievementPlatform.GameCenter, AchievementData.AchievementDataType.PlatformId, out result);
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x0000F527 File Offset: 0x0000D727
	public void OnAppStart()
	{
		this._activePlayer.PlayerChanged += delegate(Player oldPlayer, Player newPlayer)
		{
			if (!this._isSyncingAchievements)
			{
				this._isSyncingAchievements = true;
				this._tickRegistry.AppTicking += this.SyncProfileAchievementsToGameCenter;
			}
		};
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x0000F540 File Offset: 0x0000D740
	private void SyncProfileAchievementsToGameCenter(float deltaTime)
	{
		if (this._gameCenterAuthentication.IsAuthenticated && GameCenterShared.GCAreAchievementsReady() && this._activePlayer.HasActivePlayer)
		{
			foreach (Achievement achievement in this._activePlayer.MotorwaysUserProfile.Achievements)
			{
				if (achievement.IsComplete() && !this.IsAchievementCompleted(achievement.Definition))
				{
					this.CompleteAchievement(achievement, true);
				}
			}
			this._isSyncingAchievements = false;
			this._tickRegistry.AppTicking -= this.SyncProfileAchievementsToGameCenter;
		}
	}

	// Token: 0x040001A7 RID: 423
	[Dependency]
	private IGameCenterAuthentication _gameCenterAuthentication;

	// Token: 0x040001A8 RID: 424
	[Dependency]
	private TickRegistry _tickRegistry;

	// Token: 0x040001A9 RID: 425
	[Dependency]
	private ActivePlayer _activePlayer;

	// Token: 0x040001AA RID: 426
	private bool _isSyncingAchievements;
}
