using System;
using System.Collections.Generic;
using Factory;

// Token: 0x0200021A RID: 538
public abstract class LegacyBaseUserProfile : ForwardCompatibleJsonSaveData, ILegacyUserProfile, IJsonSerializableSaveData, IStorable
{
	// Token: 0x170002CA RID: 714
	// (get) Token: 0x06000CD4 RID: 3284 RVA: 0x00029AF2 File Offset: 0x00027CF2
	public List<Achievement> Achievements
	{
		get
		{
			return this._achievements;
		}
	}

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x06000CD5 RID: 3285 RVA: 0x00029AFA File Offset: 0x00027CFA
	// (set) Token: 0x06000CD6 RID: 3286 RVA: 0x00029B02 File Offset: 0x00027D02
	public Player Player { get; set; }

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x06000CD7 RID: 3287 RVA: 0x00029B0B File Offset: 0x00027D0B
	// (set) Token: 0x06000CD8 RID: 3288 RVA: 0x00029B13 File Offset: 0x00027D13
	public bool IsVibrationEnabled
	{
		get
		{
			return this._isVibrationEnabled;
		}
		set
		{
			if (this._isVibrationEnabled != value)
			{
				this._isVibrationEnabled = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x170002CD RID: 717
	// (get) Token: 0x06000CD9 RID: 3289 RVA: 0x00029B2B File Offset: 0x00027D2B
	public int Version
	{
		get
		{
			return this._version;
		}
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x00029B34 File Offset: 0x00027D34
	protected override void MergeValues(ForwardCompatibleJsonSaveData otherSaveData)
	{
		LegacyBaseUserProfile otherUserProfile = otherSaveData as LegacyBaseUserProfile;
		if (otherUserProfile != null)
		{
			this.IsVibrationEnabled = base.ChooseLatest<bool>(this._isVibrationEnabled, otherUserProfile._isVibrationEnabled, otherUserProfile.UtcTimestamp);
			using (List<Achievement>.Enumerator enumerator = otherUserProfile._achievements.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Achievement theirAchievement = enumerator.Current;
					Achievement ourAchievement = this._achievements.Find((Achievement achievement) => achievement.Id == theirAchievement.Id);
					if (ourAchievement != null)
					{
						if (ourAchievement.Merge(theirAchievement))
						{
							base.OnValueChanged();
						}
					}
					else
					{
						this._achievements.Add(theirAchievement);
						base.OnValueChanged();
					}
				}
			}
		}
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x00029C00 File Offset: 0x00027E00
	protected override void LoadFromJson(JSON.Dictionary jsonDictionary)
	{
		this._version = jsonDictionary.GetInt(LegacyBaseUserProfile.VersionKey, 0);
		JSON.Array achievementsArray = jsonDictionary.GetArray(LegacyBaseUserProfile.AchievementsKey);
		if (achievementsArray != null)
		{
			this._achievements = new List<Achievement>(achievementsArray.Count);
			for (int achievementIndex = 0; achievementIndex < achievementsArray.Count; achievementIndex++)
			{
				Achievement newAchievement = this._scope.Get<Achievement>();
				newAchievement.InitFromJson(achievementsArray.GetDictionary(achievementIndex));
				this._achievements.Add(newAchievement);
			}
		}
		this._isVibrationEnabled = jsonDictionary.GetBool(LegacyBaseUserProfile.VibrationKey, true);
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x00029C88 File Offset: 0x00027E88
	protected override void SaveToJson(Dictionary<string, object> jsonDictionary)
	{
		jsonDictionary[LegacyBaseUserProfile.VersionKey] = this._version;
		List<object> achievementsList = new List<object>();
		for (int achievementIndex = 0; achievementIndex < this._achievements.Count; achievementIndex++)
		{
			achievementsList.Add(this._achievements[achievementIndex].ToJson());
		}
		jsonDictionary[LegacyBaseUserProfile.AchievementsKey] = achievementsList;
		jsonDictionary[LegacyBaseUserProfile.VibrationKey] = this._isVibrationEnabled;
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x00029D00 File Offset: 0x00027F00
	public bool IsAchievementCompleted(AchievementDefinition achievementDefinition)
	{
		for (int achievementIndex = 0; achievementIndex < this._achievements.Count; achievementIndex++)
		{
			if (this._achievements[achievementIndex].Id.Equals(achievementDefinition.Id, StringComparison.InvariantCultureIgnoreCase))
			{
				return this._achievements[achievementIndex].IsComplete();
			}
		}
		return false;
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x00029D58 File Offset: 0x00027F58
	public void CompleteAchievement(AchievementDefinition achievementDefinition, bool showNotification)
	{
		Achievement completedAchievement = null;
		for (int achievementIndex = 0; achievementIndex < this._achievements.Count; achievementIndex++)
		{
			if (this._achievements[achievementIndex].Id.Equals(achievementDefinition.Id, StringComparison.InvariantCultureIgnoreCase))
			{
				completedAchievement = this._achievements[achievementIndex];
				break;
			}
		}
		if (completedAchievement == null)
		{
			completedAchievement = this._scope.Get<Achievement>();
			completedAchievement.InitFromDefinition(achievementDefinition);
			this._achievements.Add(completedAchievement);
		}
		if (!completedAchievement.IsComplete())
		{
			completedAchievement.SetComplete(true);
			base.OnValueChanged();
		}
		if (!this._achievementHandler.IsAchievementCompleted(completedAchievement.Definition))
		{
			this._achievementHandler.CompleteAchievement(completedAchievement, showNotification);
		}
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x00029E04 File Offset: 0x00028004
	public void RemoveAchievement(AchievementDefinition achievementDefinition)
	{
		Achievement removedAchievement = null;
		for (int achievementIndex = 0; achievementIndex < this._achievements.Count; achievementIndex++)
		{
			if (this._achievements[achievementIndex].Id.Equals(achievementDefinition.Id, StringComparison.InvariantCultureIgnoreCase))
			{
				removedAchievement = this._achievements[achievementIndex];
				break;
			}
		}
		if (removedAchievement == null)
		{
			return;
		}
		if (removedAchievement.IsComplete())
		{
			removedAchievement.SetComplete(false);
			base.OnValueChanged();
		}
	}

	// Token: 0x06000CE0 RID: 3296
	public abstract void RecordGameStatistics(IGameStatistics gameStatistics);

	// Token: 0x0400072C RID: 1836
	private List<Achievement> _achievements = new List<Achievement>();

	// Token: 0x0400072D RID: 1837
	[Dependency]
	protected IScope _scope;

	// Token: 0x0400072E RID: 1838
	[Dependency]
	protected IAchievementHandler _achievementHandler;

	// Token: 0x0400072F RID: 1839
	private static string VersionKey = "_version";

	// Token: 0x04000730 RID: 1840
	private static string AchievementsKey = "_achievements";

	// Token: 0x04000731 RID: 1841
	private static string VibrationKey = "IsVibrationEnabled";

	// Token: 0x04000733 RID: 1843
	private bool _isVibrationEnabled;

	// Token: 0x04000734 RID: 1844
	private int _version = 1;

	// Token: 0x04000735 RID: 1845
	public const string DisplayAchievementDialogBoxEditorPref = "DisplayAchievementDialogBoxEditorPref";

	// Token: 0x0200021B RID: 539
	public enum UserProfileSerializationVersion
	{
		// Token: 0x04000737 RID: 1847
		InitialVersion,
		// Token: 0x04000738 RID: 1848
		Latest
	}
}
