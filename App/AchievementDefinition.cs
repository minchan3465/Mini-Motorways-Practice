using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

// Token: 0x020000BC RID: 188
public abstract class AchievementDefinition
{
	// Token: 0x17000086 RID: 134
	// (get) Token: 0x06000369 RID: 873 RVA: 0x0000E5B9 File Offset: 0x0000C7B9
	// (set) Token: 0x0600036A RID: 874 RVA: 0x0000E5C1 File Offset: 0x0000C7C1
	public string Id { get; protected set; }

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x0600036B RID: 875 RVA: 0x0000E5CA File Offset: 0x0000C7CA
	// (set) Token: 0x0600036C RID: 876 RVA: 0x0000E5D2 File Offset: 0x0000C7D2
	public Sprite Icon { get; protected set; }

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x0600036D RID: 877 RVA: 0x0000E5DB File Offset: 0x0000C7DB
	// (set) Token: 0x0600036E RID: 878 RVA: 0x0000E5E3 File Offset: 0x0000C7E3
	public bool HasLoggedFailure { get; set; }

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x0600036F RID: 879 RVA: 0x000020AA File Offset: 0x000002AA
	public virtual bool CanBeAwardedRetroactively
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000370 RID: 880 RVA: 0x0000E5EC File Offset: 0x0000C7EC
	public virtual int GetIntDataForPlatformAndKey(AchievementData.AchievementPlatform platform, AchievementData.AchievementDataType dataKey)
	{
		int data = -1;
		for (int platformDataIndex = 0; platformDataIndex < this.platformSpecificData.Count; platformDataIndex++)
		{
			AchievementData.AchievementPlatformSpecificData currentData = this.platformSpecificData[platformDataIndex];
			if (currentData.forPlatform == platform && currentData.dataKey == dataKey)
			{
				data = currentData.intData;
				break;
			}
		}
		return data;
	}

	// Token: 0x06000371 RID: 881 RVA: 0x0000E63C File Offset: 0x0000C83C
	public bool TryGetStringDataForPlatformAndKey(AchievementData.AchievementPlatform platform, AchievementData.AchievementDataType dataKey, out string result)
	{
		result = null;
		for (int platformDataIndex = 0; platformDataIndex < this.platformSpecificData.Count; platformDataIndex++)
		{
			AchievementData.AchievementPlatformSpecificData currentData = this.platformSpecificData[platformDataIndex];
			if (currentData.forPlatform == platform && currentData.dataKey == dataKey)
			{
				result = currentData.stringData;
				break;
			}
		}
		return result != null;
	}

	// Token: 0x06000372 RID: 882 RVA: 0x0000E690 File Offset: 0x0000C890
	public static AchievementDefinition FromAchievementData(AchievementData achievementData, IScope scope)
	{
		AchievementDefinition achievementDefinition = scope.Get<AchievementDefinition>();
		if (achievementDefinition.InitFromAchievementData(achievementData, scope))
		{
			return achievementDefinition;
		}
		return null;
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0000E6B4 File Offset: 0x0000C8B4
	public virtual bool InitFromAchievementData(AchievementData achievementData, IScope scope)
	{
		this.Id = achievementData.GetId();
		this.Icon = achievementData.achievementIcon;
		this.platformSpecificData = new List<AchievementData.AchievementPlatformSpecificData>(achievementData.platformSpecificData.Count);
		for (int platformDataIndex = 0; platformDataIndex < achievementData.platformSpecificData.Count; platformDataIndex++)
		{
			AchievementData.AchievementPlatformSpecificData currentData = achievementData.platformSpecificData[platformDataIndex];
			this.platformSpecificData.Add(currentData.Clone(null));
		}
		return true;
	}

	// Token: 0x04000178 RID: 376
	protected List<AchievementData.AchievementPlatformSpecificData> platformSpecificData = new List<AchievementData.AchievementPlatformSpecificData>();

	// Token: 0x04000179 RID: 377
	[Dependency]
	protected IScope _scope;
}
