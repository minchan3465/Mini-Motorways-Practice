using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

// Token: 0x020000BB RID: 187
[CreateAssetMenu(fileName = "New Achievement Database", menuName = "Motorways/Achievements/Achievement Collection", order = 2)]
public class AchievementDatabase : ScriptableObject, IReleasedFromScopeHandler
{
	// Token: 0x17000082 RID: 130
	public AchievementDefinition this[int key]
	{
		get
		{
			return this.achievements[key];
		}
	}

	// Token: 0x17000083 RID: 131
	public AchievementDefinition this[string key]
	{
		get
		{
			for (int achievementIndex = 0; achievementIndex < this.achievements.Count; achievementIndex++)
			{
				if (this.achievements[achievementIndex].Id == key)
				{
					return this.achievements[achievementIndex];
				}
			}
			return null;
		}
	}

	// Token: 0x17000084 RID: 132
	public AchievementDefinition this[Enum key]
	{
		get
		{
			string keyAsString = key.ToString();
			return this[keyAsString];
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x06000362 RID: 866 RVA: 0x0000E47F File Offset: 0x0000C67F
	public int Count
	{
		get
		{
			return this.achievements.Count;
		}
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0000E48C File Offset: 0x0000C68C
	public bool Load()
	{
		for (int achievementIndex = 0; achievementIndex < this.allAchievementData.Count; achievementIndex++)
		{
			AchievementDefinition achievement = AchievementDefinition.FromAchievementData(this.allAchievementData[achievementIndex], this._scope);
			if (achievement == null)
			{
				AchievementDatabase.Log.Warn("Failed to load achievement {0}.", new object[]
				{
					achievementIndex
				});
			}
			else
			{
				this.achievements.Add(achievement);
			}
		}
		return true;
	}

	// Token: 0x06000364 RID: 868 RVA: 0x0000E4F8 File Offset: 0x0000C6F8
	public bool ContainsAchievement(string achievementName)
	{
		for (int achievementIndex = 0; achievementIndex < this.achievements.Count; achievementIndex++)
		{
			if (this.achievements[achievementIndex].Id == achievementName)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000365 RID: 869 RVA: 0x0000E538 File Offset: 0x0000C738
	public bool ContainsAchievement(Enum achievementNameEnum)
	{
		string achievementNameAsString = achievementNameEnum.ToString();
		return this.ContainsAchievement(achievementNameAsString);
	}

	// Token: 0x06000366 RID: 870 RVA: 0x0000E554 File Offset: 0x0000C754
	public void OnReleasedFromScope(IScope scope)
	{
		for (int achievementIndex = 0; achievementIndex < this.achievements.Count; achievementIndex++)
		{
			scope.Release(this.achievements[achievementIndex]);
		}
	}

	// Token: 0x04000170 RID: 368
	public const string MenuItemFolder = "Motorways/Achievements/";

	// Token: 0x04000171 RID: 369
	public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("AchievementDatabase");

	// Token: 0x04000172 RID: 370
	private List<AchievementDefinition> achievements = new List<AchievementDefinition>();

	// Token: 0x04000173 RID: 371
	public List<AchievementData> allAchievementData = new List<AchievementData>();

	// Token: 0x04000174 RID: 372
	[Dependency]
	protected IScope _scope;
}
