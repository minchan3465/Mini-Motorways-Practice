using System;
using System.Collections.Generic;
using Factory;

// Token: 0x020000B6 RID: 182
public abstract class Achievement
{
	// Token: 0x1700007F RID: 127
	// (get) Token: 0x0600034E RID: 846
	// (set) Token: 0x0600034F RID: 847
	public abstract string Id { get; protected set; }

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000350 RID: 848 RVA: 0x0000E2AE File Offset: 0x0000C4AE
	// (set) Token: 0x06000351 RID: 849 RVA: 0x0000E2B6 File Offset: 0x0000C4B6
	private bool _isAwardedOnPlatform { get; set; }

	// Token: 0x06000352 RID: 850 RVA: 0x0000E2BF File Offset: 0x0000C4BF
	public void InitFromString(string stringId)
	{
		this.Id = stringId;
	}

	// Token: 0x06000353 RID: 851 RVA: 0x0000E2C8 File Offset: 0x0000C4C8
	public void InitFromDefinition(AchievementDefinition achievementDefinition)
	{
		this.Id = achievementDefinition.Id;
	}

	// Token: 0x06000354 RID: 852 RVA: 0x0000E2D6 File Offset: 0x0000C4D6
	public void InitFromJson(JSON.Dictionary jsonDictionary)
	{
		if (jsonDictionary == null)
		{
			return;
		}
		this.InitFromString(jsonDictionary.GetString("Id"));
		this._isComplete = jsonDictionary.GetBool("isComplete", false);
		this._isAwardedOnPlatform = jsonDictionary.GetBool("IsAwardedOnPlatform", false);
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06000355 RID: 853 RVA: 0x0000E311 File Offset: 0x0000C511
	public AchievementDefinition Definition
	{
		get
		{
			return this._achievementDatabase[this.Id];
		}
	}

	// Token: 0x06000356 RID: 854 RVA: 0x0000E324 File Offset: 0x0000C524
	public bool IsComplete()
	{
		return this._isComplete;
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0000E32C File Offset: 0x0000C52C
	public void SetComplete(bool isComplete)
	{
		this._isComplete = isComplete;
	}

	// Token: 0x06000358 RID: 856 RVA: 0x0000E335 File Offset: 0x0000C535
	public bool Merge(Achievement other)
	{
		if (!this._isComplete && other._isComplete)
		{
			this._isComplete = true;
			return true;
		}
		return false;
	}

	// Token: 0x06000359 RID: 857 RVA: 0x0000E354 File Offset: 0x0000C554
	public object ToJson()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["Id"] = this.Id;
		dictionary["isComplete"] = this._isComplete;
		dictionary["IsAwardedOnPlatform"] = this._isAwardedOnPlatform;
		return dictionary;
	}

	// Token: 0x0400015C RID: 348
	private bool _isComplete;

	// Token: 0x0400015E RID: 350
	[Dependency]
	protected AchievementDatabase _achievementDatabase;

	// Token: 0x0400015F RID: 351
	[Dependency]
	protected IAchievementHandler _achievementHandler;

	// Token: 0x04000160 RID: 352
	private const string IdKey = "Id";

	// Token: 0x04000161 RID: 353
	private const string IsCompleteKey = "isComplete";

	// Token: 0x04000162 RID: 354
	private const string IsAwardedOnPlatformKey = "IsAwardedOnPlatform";
}
