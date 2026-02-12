using System;
using System.Collections.Generic;
using Factory;
using Motorways;

// Token: 0x020001FF RID: 511
public abstract class BaseExtendedUserProfile : ForwardCompatibleJsonSaveData, IExtendedUserProfile, IJsonSerializableSaveData, IStorable
{
	// Token: 0x17000293 RID: 659
	// (get) Token: 0x06000BF7 RID: 3063 RVA: 0x00028DEC File Offset: 0x00026FEC
	public int Version { get; }

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x06000BF8 RID: 3064 RVA: 0x00028DF4 File Offset: 0x00026FF4
	// (set) Token: 0x06000BF9 RID: 3065 RVA: 0x00028DFC File Offset: 0x00026FFC
	public Player Player { get; set; }

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x06000BFA RID: 3066 RVA: 0x00028E05 File Offset: 0x00027005
	// (set) Token: 0x06000BFB RID: 3067 RVA: 0x00028E0D File Offset: 0x0002700D
	public int AvatarColorIndex
	{
		get
		{
			return this._avatarColorIndex;
		}
		set
		{
			if (this._avatarColorIndex != value)
			{
				this._avatarColorIndex = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000296 RID: 662
	// (get) Token: 0x06000BFC RID: 3068 RVA: 0x00028E25 File Offset: 0x00027025
	// (set) Token: 0x06000BFD RID: 3069 RVA: 0x00028E2D File Offset: 0x0002702D
	public int AvatarIconIndex
	{
		get
		{
			return this._avatarIconIndex;
		}
		set
		{
			if (this._avatarIconIndex != value)
			{
				this._avatarIconIndex = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x06000BFE RID: 3070 RVA: 0x00028E45 File Offset: 0x00027045
	// (set) Token: 0x06000BFF RID: 3071 RVA: 0x00028E4D File Offset: 0x0002704D
	public iCloudProvenance iCloudProvenance
	{
		get
		{
			return this._iCloudProvenance;
		}
		set
		{
			if (this._iCloudProvenance != value)
			{
				this._iCloudProvenance = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x06000C00 RID: 3072 RVA: 0x00028E65 File Offset: 0x00027065
	// (set) Token: 0x06000C01 RID: 3073 RVA: 0x00028E6D File Offset: 0x0002706D
	public int LastTimeDailyChallengeSeen
	{
		get
		{
			return this._lastTimeDailyChallengeSeen;
		}
		set
		{
			if (this._lastTimeDailyChallengeSeen != value)
			{
				this._lastTimeDailyChallengeSeen = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x06000C02 RID: 3074 RVA: 0x00028E85 File Offset: 0x00027085
	// (set) Token: 0x06000C03 RID: 3075 RVA: 0x00028E8D File Offset: 0x0002708D
	public int LastTimeWeeklyChallengeSeen
	{
		get
		{
			return this._lastTimeWeeklyChallengeSeen;
		}
		set
		{
			if (this._lastTimeWeeklyChallengeSeen != value)
			{
				this._lastTimeWeeklyChallengeSeen = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x1700029A RID: 666
	// (get) Token: 0x06000C04 RID: 3076 RVA: 0x00028EA5 File Offset: 0x000270A5
	// (set) Token: 0x06000C05 RID: 3077 RVA: 0x00028EAD File Offset: 0x000270AD
	public bool CreativeInGameMessageSeen
	{
		get
		{
			return this._creativeInGameMessageSeen;
		}
		set
		{
			if (this._creativeInGameMessageSeen != value)
			{
				this._creativeInGameMessageSeen = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x00028EC5 File Offset: 0x000270C5
	public bool HasSeenNewContent(string newContentId)
	{
		return this._seenNewContentIDs.Contains(newContentId);
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x00028ED3 File Offset: 0x000270D3
	public GameMode GetSelectedModeForMap(string mapId)
	{
		if (this._selectedGameMode.ContainsKey(mapId))
		{
			return this._selectedGameMode[mapId];
		}
		this._selectedGameMode[mapId] = GameMode.Normal;
		return this._selectedGameMode[mapId];
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00028F09 File Offset: 0x00027109
	public void SetSelectedGameModeForMap(string mapId, GameMode gameMode)
	{
		this._selectedGameMode[mapId] = gameMode;
		base.OnValueChanged();
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x00028F1E File Offset: 0x0002711E
	public void SetNewContentSeen(string newContentId)
	{
		if (!this._seenNewContentIDs.Contains(newContentId))
		{
			this._seenNewContentIDs.Add(newContentId);
			base.OnValueChanged();
		}
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x00028F41 File Offset: 0x00027141
	public void ClearNewContentSeen(string specificContent = null)
	{
		if (this._seenNewContentIDs.Count > 0)
		{
			if (string.IsNullOrWhiteSpace(specificContent))
			{
				this._seenNewContentIDs.Clear();
			}
			else
			{
				this._seenNewContentIDs.Remove(specificContent);
			}
			base.OnValueChanged();
		}
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x00028F7C File Offset: 0x0002717C
	protected override void MergeValues(ForwardCompatibleJsonSaveData otherSaveData)
	{
		BaseExtendedUserProfile otherExtendedUserProfile = otherSaveData as BaseExtendedUserProfile;
		if (otherExtendedUserProfile != null)
		{
			this.iCloudProvenance = base.ChooseMax<iCloudProvenance>(this._iCloudProvenance, otherExtendedUserProfile._iCloudProvenance);
			this.AvatarColorIndex = base.ChooseLatest<int>(this._avatarColorIndex, otherExtendedUserProfile._avatarColorIndex, otherExtendedUserProfile.UtcTimestamp);
			this.AvatarIconIndex = base.ChooseLatest<int>(this._avatarIconIndex, otherExtendedUserProfile._avatarIconIndex, otherExtendedUserProfile.UtcTimestamp);
			this.LastTimeDailyChallengeSeen = base.ChooseMax<int>(this._lastTimeDailyChallengeSeen, otherExtendedUserProfile._lastTimeDailyChallengeSeen);
			this.LastTimeWeeklyChallengeSeen = base.ChooseMax<int>(this._lastTimeWeeklyChallengeSeen, otherExtendedUserProfile._lastTimeWeeklyChallengeSeen);
			this.CreativeInGameMessageSeen = (this._creativeInGameMessageSeen || otherExtendedUserProfile._creativeInGameMessageSeen);
			int count = this._seenNewContentIDs.Count;
			this._seenNewContentIDs.UnionWith(otherExtendedUserProfile._seenNewContentIDs);
			if (count != this._seenNewContentIDs.Count)
			{
				base.OnValueChanged();
			}
			foreach (KeyValuePair<string, GameMode> entry in otherExtendedUserProfile._selectedGameMode)
			{
				this._selectedGameMode[entry.Key] = entry.Value;
			}
		}
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x000290B8 File Offset: 0x000272B8
	protected override void LoadFromJson(JSON.Dictionary jsonDictionary)
	{
		this._iCloudProvenance = (iCloudProvenance)jsonDictionary.GetInt("iCloudProvenance", 0);
		this._avatarColorIndex = jsonDictionary.GetInt("ProfileBackgroundIndex", 0);
		this._avatarIconIndex = jsonDictionary.GetInt("ProfileIconIndex", 0);
		this._lastTimeDailyChallengeSeen = jsonDictionary.GetInt("LastTimeDailyChallengeSeen", 0);
		this._lastTimeWeeklyChallengeSeen = jsonDictionary.GetInt("LastTimeWeeklyChallengeSeen", 0);
		this._creativeInGameMessageSeen = jsonDictionary.GetBool("CreativeInGameMessageSeen", false);
		JSON.Array seenNewContentIdsArray = jsonDictionary.GetArray("_seenNewContentIDs");
		if (seenNewContentIdsArray != null)
		{
			for (int newContentIdIndex = 0; newContentIdIndex < seenNewContentIdsArray.Count; newContentIdIndex++)
			{
				string idString = seenNewContentIdsArray.GetString(newContentIdIndex);
				if (idString != null && this.CanLoadSeenContentId(idString))
				{
					this._seenNewContentIDs.Add(idString);
				}
			}
		}
		JSON.Dictionary lastSelectedGameModes = jsonDictionary.GetDictionary("_selectedGameMode");
		if (lastSelectedGameModes != null)
		{
			foreach (string key in lastSelectedGameModes.Keys)
			{
				this._selectedGameMode[key] = GameMode.Normal;
				string value = lastSelectedGameModes.GetString(key);
				GameMode jsonMode;
				if (!string.IsNullOrEmpty(value) && Diagnostics.Verify(Enum.TryParse<GameMode>(value, out jsonMode), "{0} is not a valid game mode! Setting to Normal.", jsonMode))
				{
					this._selectedGameMode[key] = jsonMode;
				}
			}
		}
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x00029210 File Offset: 0x00027410
	private bool CanLoadSeenContentId(string idString)
	{
		if (idString.StartsWith("NewWeeklyChallenge-"))
		{
			long timeStampLong;
			if (!long.TryParse(idString.Remove(0, "NewWeeklyChallenge-".Length), out timeStampLong))
			{
				return false;
			}
			DateTimeOffset timeStamp = DateTimeOffset.FromUnixTimeSeconds(timeStampLong);
			if (GameDateTime.UtcNow.Subtract(TimeSpan.FromDays(8.0)) > timeStamp)
			{
				return false;
			}
		}
		else if (idString.StartsWith("NewDailyChallenge-"))
		{
			long timeStampLong2;
			if (!long.TryParse(idString.Remove(0, "NewDailyChallenge-".Length), out timeStampLong2))
			{
				return false;
			}
			DateTimeOffset timeStamp2 = DateTimeOffset.FromUnixTimeSeconds(timeStampLong2);
			if (GameDateTime.UtcNow.Subtract(TimeSpan.FromDays(2.0)) > timeStamp2)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x000292D4 File Offset: 0x000274D4
	protected override void SaveToJson(Dictionary<string, object> jsonDictionary)
	{
		jsonDictionary["iCloudProvenance"] = (int)this._iCloudProvenance;
		jsonDictionary["ProfileBackgroundIndex"] = this._avatarColorIndex;
		jsonDictionary["ProfileIconIndex"] = this._avatarIconIndex;
		jsonDictionary["LastTimeDailyChallengeSeen"] = this._lastTimeDailyChallengeSeen;
		jsonDictionary["LastTimeWeeklyChallengeSeen"] = this._lastTimeWeeklyChallengeSeen;
		jsonDictionary["CreativeInGameMessageSeen"] = this._creativeInGameMessageSeen;
		List<object> seenNewContentIdsList = new List<object>(this._seenNewContentIDs.Count);
		foreach (string seenContentId in this._seenNewContentIDs)
		{
			seenNewContentIdsList.Add(seenContentId);
		}
		jsonDictionary["_seenNewContentIDs"] = seenNewContentIdsList;
		jsonDictionary["_selectedGameMode"] = this._selectedGameMode;
	}

	// Token: 0x06000C0F RID: 3087
	public abstract void RecordGameStatistics(IGameStatistics gameStatistics);

	// Token: 0x040006F1 RID: 1777
	[Dependency]
	protected IScope _scope;

	// Token: 0x040006F2 RID: 1778
	public const int InvalidAvatarColorIndex = -1;

	// Token: 0x040006F3 RID: 1779
	public const int InvalidAvatarIconIndex = -1;

	// Token: 0x040006F4 RID: 1780
	private const string iCloudProvenanceKey = "iCloudProvenance";

	// Token: 0x040006F5 RID: 1781
	private const string ProfileBackgroundIndexKey = "ProfileBackgroundIndex";

	// Token: 0x040006F6 RID: 1782
	private const string ProfileIconIndexKey = "ProfileIconIndex";

	// Token: 0x040006F7 RID: 1783
	private const string LastTimeDailyChallengeSeenKey = "LastTimeDailyChallengeSeen";

	// Token: 0x040006F8 RID: 1784
	private const string LastTimeWeeklyChallengeSeenKey = "LastTimeWeeklyChallengeSeen";

	// Token: 0x040006F9 RID: 1785
	private const string CreativeInGameMessageSeenKey = "CreativeInGameMessageSeen";

	// Token: 0x040006FC RID: 1788
	private int _avatarColorIndex = -1;

	// Token: 0x040006FD RID: 1789
	private int _avatarIconIndex = -1;

	// Token: 0x040006FE RID: 1790
	private iCloudProvenance _iCloudProvenance;

	// Token: 0x040006FF RID: 1791
	private int _lastTimeDailyChallengeSeen;

	// Token: 0x04000700 RID: 1792
	private int _lastTimeWeeklyChallengeSeen;

	// Token: 0x04000701 RID: 1793
	private bool _creativeInGameMessageSeen;

	// Token: 0x04000702 RID: 1794
	private readonly HashSet<string> _seenNewContentIDs = new HashSet<string>();

	// Token: 0x04000703 RID: 1795
	private readonly Dictionary<string, GameMode> _selectedGameMode = new Dictionary<string, GameMode>();
}
