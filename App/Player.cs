using System;
using System.Collections.Generic;
using System.Globalization;
using Factory;

// Token: 0x02000228 RID: 552
public class Player : IReleasedFromScopeHandler
{
	// Token: 0x06000D04 RID: 3332 RVA: 0x0002A8F8 File Offset: 0x00028AF8
	public void Initialize(string id)
	{
		this._id = id;
		this._userProfile = this._scope.Get<ILegacyUserProfile>();
		this._userProfile.DataChanged += this.OnDataChanged;
		this._userProfile.Player = this;
		this._extendedUserProfile = this._scope.Get<IExtendedUserProfile>();
		this._extendedUserProfile.DataChanged += this.OnDataChanged;
		this._extendedUserProfile.Player = this;
		this._deviceSettings = this._scope.Get<IDeviceSettings>();
		this._deviceSettings.DataChanged += this.OnDataChanged;
		this._deviceSettings.Player = this;
	}

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x06000D05 RID: 3333 RVA: 0x0002A9A8 File Offset: 0x00028BA8
	public string Id
	{
		get
		{
			return this._id;
		}
	}

	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x06000D06 RID: 3334 RVA: 0x0002A9B0 File Offset: 0x00028BB0
	public DateTime LastPlayedUtcTimeOnLocalDevice
	{
		get
		{
			if (this._deviceSettings.LastPlayedUtcTime > DateTime.MinValue)
			{
				Player.Log.Info("Player {0} has a last-played time on this device of {1}.", new object[]
				{
					this._id,
					this._deviceSettings.LastPlayedUtcTime.ToString(DateTimeFormatInfo.InvariantInfo)
				});
				return this._deviceSettings.LastPlayedUtcTime;
			}
			DateTime lastPlayedTime = this._userProfile.UtcTimestamp;
			if (this._extendedUserProfile.UtcTimestamp > lastPlayedTime)
			{
				lastPlayedTime = this._extendedUserProfile.UtcTimestamp;
			}
			if (this._deviceSettings.UtcTimestamp > lastPlayedTime)
			{
				lastPlayedTime = this._deviceSettings.UtcTimestamp;
			}
			if (this._localSavedGame != null && this._localSavedGame.UtcTimestamp > lastPlayedTime)
			{
				lastPlayedTime = this._localSavedGame.UtcTimestamp;
			}
			Player.Log.Info("Player {0} has no last-played time on this device, estimating a time of {1} instead.", new object[]
			{
				this._id,
				lastPlayedTime.ToString(DateTimeFormatInfo.InvariantInfo)
			});
			return lastPlayedTime;
		}
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x0002AAB8 File Offset: 0x00028CB8
	public void ChooseAvatar(int colorCount, int iconCount)
	{
		List<int> colorIndices = new List<int>();
		while (colorIndices.Count < colorCount)
		{
			colorIndices.Add(colorIndices.Count);
		}
		global::Random.ShuffleList<int>(colorIndices);
		List<int> iconIndices = new List<int>();
		while (iconIndices.Count < iconCount)
		{
			iconIndices.Add(iconIndices.Count);
		}
		global::Random.ShuffleList<int>(iconIndices);
		this.ExtendedUserProfile.AvatarColorIndex = -1;
		this.ExtendedUserProfile.AvatarIconIndex = -1;
		foreach (int colorIndex in colorIndices)
		{
			bool indexAvailable = true;
			using (IEnumerator<Player> enumerator2 = this._playerDatabase.Players.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.AvatarColorIndex == colorIndex)
					{
						indexAvailable = false;
						break;
					}
				}
			}
			if (indexAvailable)
			{
				this.ExtendedUserProfile.AvatarColorIndex = colorIndex;
				break;
			}
		}
		foreach (int iconIndex in iconIndices)
		{
			bool indexAvailable2 = true;
			using (IEnumerator<Player> enumerator2 = this._playerDatabase.Players.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.AvatarIconIndex == iconIndex)
					{
						indexAvailable2 = false;
						break;
					}
				}
			}
			if (indexAvailable2)
			{
				this.ExtendedUserProfile.AvatarIconIndex = iconIndex;
				break;
			}
		}
		if (this.ExtendedUserProfile.AvatarColorIndex == -1)
		{
			this.ExtendedUserProfile.AvatarColorIndex = global::Random.Range(0, colorCount);
		}
		if (this.ExtendedUserProfile.AvatarIconIndex == -1)
		{
			this.ExtendedUserProfile.AvatarIconIndex = global::Random.Range(0, iconCount);
		}
	}

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x06000D08 RID: 3336 RVA: 0x0002AC98 File Offset: 0x00028E98
	public bool HasAvatar
	{
		get
		{
			return this.AvatarColorIndex != -1 && this.AvatarIconIndex != -1;
		}
	}

	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x06000D09 RID: 3337 RVA: 0x0002ACB1 File Offset: 0x00028EB1
	// (set) Token: 0x06000D0A RID: 3338 RVA: 0x0002ACBE File Offset: 0x00028EBE
	public int AvatarColorIndex
	{
		get
		{
			return this.ExtendedUserProfile.AvatarColorIndex;
		}
		set
		{
			this.ExtendedUserProfile.AvatarColorIndex = value;
		}
	}

	// Token: 0x170002D4 RID: 724
	// (get) Token: 0x06000D0B RID: 3339 RVA: 0x0002ACCC File Offset: 0x00028ECC
	// (set) Token: 0x06000D0C RID: 3340 RVA: 0x0002ACD9 File Offset: 0x00028ED9
	public int AvatarIconIndex
	{
		get
		{
			return this.ExtendedUserProfile.AvatarIconIndex;
		}
		set
		{
			this.ExtendedUserProfile.AvatarIconIndex = value;
		}
	}

	// Token: 0x170002D5 RID: 725
	// (get) Token: 0x06000D0D RID: 3341 RVA: 0x0002ACE7 File Offset: 0x00028EE7
	// (set) Token: 0x06000D0E RID: 3342 RVA: 0x0002ACF4 File Offset: 0x00028EF4
	public LocaleDatabase.LocaleId LocaleId
	{
		get
		{
			return this.DeviceSettings.LastLocaleId;
		}
		set
		{
			this.DeviceSettings.LastLocaleId = value;
		}
	}

	// Token: 0x170002D6 RID: 726
	// (get) Token: 0x06000D0F RID: 3343 RVA: 0x0002AD02 File Offset: 0x00028F02
	public bool HasLocalSavedGame
	{
		get
		{
			return this._localSavedGame != null;
		}
	}

	// Token: 0x170002D7 RID: 727
	// (get) Token: 0x06000D10 RID: 3344 RVA: 0x0002AD0D File Offset: 0x00028F0D
	// (set) Token: 0x06000D11 RID: 3345 RVA: 0x0002AD18 File Offset: 0x00028F18
	public IGameJournalSave LocalSavedGame
	{
		get
		{
			return this._localSavedGame;
		}
		set
		{
			if (this._localSavedGame == value)
			{
				return;
			}
			if (this._localSavedGame != null)
			{
				this._scope.Release(this._localSavedGame);
			}
			this._localSavedGame = value;
			if (this._localSavedGame != null)
			{
				this._localSavedGame.Player = this;
			}
			Action savedGamesChanged = this.SavedGamesChanged;
			if (savedGamesChanged == null)
			{
				return;
			}
			savedGamesChanged();
		}
	}

	// Token: 0x170002D8 RID: 728
	// (get) Token: 0x06000D12 RID: 3346 RVA: 0x0002AD74 File Offset: 0x00028F74
	public bool HasForeignSavedGames
	{
		get
		{
			return this._foreignSavedGames.Count + this._playerDatabase.GlobalSavedGames.Count > 0;
		}
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x0002AD98 File Offset: 0x00028F98
	public void AddForeignSavedGame(IGameJournalSave newForeignSavedGame)
	{
		int foreignSavedGameIndex = 0;
		while (foreignSavedGameIndex < this._foreignSavedGames.Count)
		{
			if (this._foreignSavedGames[foreignSavedGameIndex].DeviceId == newForeignSavedGame.DeviceId)
			{
				IGameJournalSave oldSavedGame = this._foreignSavedGames[foreignSavedGameIndex];
				this._foreignSavedGames.RemoveAt(foreignSavedGameIndex);
				this._scope.Release(oldSavedGame);
			}
			else
			{
				foreignSavedGameIndex++;
			}
		}
		newForeignSavedGame.Player = this;
		this._foreignSavedGames.Add(newForeignSavedGame);
		Action savedGamesChanged = this.SavedGamesChanged;
		if (savedGamesChanged == null)
		{
			return;
		}
		savedGamesChanged();
	}

	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x06000D14 RID: 3348 RVA: 0x0002AE24 File Offset: 0x00029024
	public IEnumerable<IGameJournalSave> ForeignSavedGames
	{
		get
		{
			foreach (IGameJournalSave foreignSavedGame in this._foreignSavedGames)
			{
				yield return foreignSavedGame;
			}
			List<IGameJournalSave>.Enumerator enumerator = default(List<IGameJournalSave>.Enumerator);
			foreach (IGameJournalSave globalSavedGame in this._playerDatabase.GlobalSavedGames)
			{
				yield return globalSavedGame;
			}
			IEnumerator<IGameJournalSave> enumerator2 = null;
			yield break;
			yield break;
		}
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x0002AE34 File Offset: 0x00029034
	public IGameJournalSave GetForeignSavedGame(string savedGameId)
	{
		foreach (IGameJournalSave foreignSavedGame in this._foreignSavedGames)
		{
			if (foreignSavedGame.DeviceId == savedGameId)
			{
				return foreignSavedGame;
			}
		}
		foreach (IGameJournalSave globalSavedGame in this._playerDatabase.GlobalSavedGames)
		{
			if (globalSavedGame.DeviceId == savedGameId)
			{
				return globalSavedGame;
			}
		}
		return null;
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x0002AEE4 File Offset: 0x000290E4
	public void RemoveSavedGame(IGameJournalSave savedGame)
	{
		if (savedGame == this._localSavedGame)
		{
			this.LocalSavedGame = null;
			return;
		}
		if (this._foreignSavedGames.Contains(savedGame))
		{
			this._foreignSavedGames.Remove(savedGame);
			this._storage.Delete(savedGame);
			Action savedGamesChanged = this.SavedGamesChanged;
			if (savedGamesChanged == null)
			{
				return;
			}
			savedGamesChanged();
		}
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x0002AF3C File Offset: 0x0002913C
	public void RemoveSavedGame(string savedGameDeviceId)
	{
		if (this._localSavedGame != null && (savedGameDeviceId == this._localSavedGame.DeviceId || savedGameDeviceId == PlayerDatabase.LegacyDeviceId))
		{
			this.RemoveSavedGame(this._localSavedGame);
			return;
		}
		foreach (IGameJournalSave foreignSavedGame in this._foreignSavedGames)
		{
			if (foreignSavedGame.DeviceId == savedGameDeviceId)
			{
				this.RemoveSavedGame(foreignSavedGame);
				break;
			}
		}
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x0002AFD4 File Offset: 0x000291D4
	public void MergeUserProfile(ILegacyUserProfile newUserProfile)
	{
		this._userProfile.Merge(newUserProfile, true);
		this._auditTrail.RecordEvent("Player.MergeUserProfile", delegate(Dictionary<string, string> metadata)
		{
			metadata["mergedTimestamp"] = this._userProfile.UtcTimestamp.ToString(DateTimeFormatInfo.InvariantInfo);
			metadata["mergedJson"] = Json.Serialize(this._userProfile.SerializeToJson(), false);
		});
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x0002AFFF File Offset: 0x000291FF
	public void MergeExtendedUserProfile(IExtendedUserProfile newExtendedUserProfile)
	{
		this._extendedUserProfile.Merge(newExtendedUserProfile, true);
		this._auditTrail.RecordEvent("Player.MergeExtendedUserProfile", delegate(Dictionary<string, string> metadata)
		{
			metadata["mergedTimestamp"] = this._extendedUserProfile.UtcTimestamp.ToString(DateTimeFormatInfo.InvariantInfo);
			metadata["mergedJson"] = Json.Serialize(this._extendedUserProfile.SerializeToJson(), false);
		});
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x0002B02A File Offset: 0x0002922A
	public void MergeDeviceSettings(IDeviceSettings newDeviceSettings)
	{
		this._deviceSettings.Merge(newDeviceSettings, true);
		this._auditTrail.RecordEvent("Player.MergeDeviceSettings", delegate(Dictionary<string, string> metadata)
		{
			metadata["mergedTimestamp"] = this._deviceSettings.UtcTimestamp.ToString(DateTimeFormatInfo.InvariantInfo);
			metadata["mergedJson"] = Json.Serialize(this._deviceSettings.SerializeToJson(), false);
		});
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x0002B058 File Offset: 0x00029258
	public static int CompareAccessTime(Player x, Player y)
	{
		int result = x.DeviceSettings.LastPlayedUtcTime.CompareTo(y.DeviceSettings.LastPlayedUtcTime);
		if (result != 0)
		{
			return result;
		}
		return x.LastPlayedUtcTimeOnLocalDevice.CompareTo(y.LastPlayedUtcTimeOnLocalDevice);
	}

	// Token: 0x14000030 RID: 48
	// (add) Token: 0x06000D1C RID: 3356 RVA: 0x0002B0A0 File Offset: 0x000292A0
	// (remove) Token: 0x06000D1D RID: 3357 RVA: 0x0002B0D8 File Offset: 0x000292D8
	public event Action DataChanged;

	// Token: 0x14000031 RID: 49
	// (add) Token: 0x06000D1E RID: 3358 RVA: 0x0002B110 File Offset: 0x00029310
	// (remove) Token: 0x06000D1F RID: 3359 RVA: 0x0002B148 File Offset: 0x00029348
	public event Action SavedGamesChanged;

	// Token: 0x170002DA RID: 730
	// (get) Token: 0x06000D20 RID: 3360 RVA: 0x0002B17D File Offset: 0x0002937D
	public ILegacyUserProfile UserProfile
	{
		get
		{
			return this._userProfile;
		}
	}

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x06000D21 RID: 3361 RVA: 0x0002B185 File Offset: 0x00029385
	public IExtendedUserProfile ExtendedUserProfile
	{
		get
		{
			return this._extendedUserProfile;
		}
	}

	// Token: 0x170002DC RID: 732
	// (get) Token: 0x06000D22 RID: 3362 RVA: 0x0002B18D File Offset: 0x0002938D
	public IDeviceSettings DeviceSettings
	{
		get
		{
			return this._deviceSettings;
		}
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x0002B198 File Offset: 0x00029398
	public void OnReleasedFromScope(IScope scope)
	{
		this._scope.Release(this._userProfile);
		this._scope.Release(this._extendedUserProfile);
		this._scope.Release(this._deviceSettings);
		if (this._localSavedGame != null)
		{
			this._scope.Release(this._localSavedGame);
		}
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x0002B1F5 File Offset: 0x000293F5
	private void OnDataChanged()
	{
		Action dataChanged = this.DataChanged;
		if (dataChanged == null)
		{
			return;
		}
		dataChanged();
	}

	// Token: 0x0400075B RID: 1883
	private string _id;

	// Token: 0x0400075C RID: 1884
	private ILegacyUserProfile _userProfile;

	// Token: 0x0400075D RID: 1885
	private IExtendedUserProfile _extendedUserProfile;

	// Token: 0x0400075E RID: 1886
	private IDeviceSettings _deviceSettings;

	// Token: 0x0400075F RID: 1887
	private IGameJournalSave _localSavedGame;

	// Token: 0x04000760 RID: 1888
	private readonly List<IGameJournalSave> _foreignSavedGames = new List<IGameJournalSave>();

	// Token: 0x04000761 RID: 1889
	[Dependency]
	private IScope _scope;

	// Token: 0x04000762 RID: 1890
	[Dependency]
	private IPersistentStorageService _storage;

	// Token: 0x04000763 RID: 1891
	[Dependency]
	private PlayerDatabase _playerDatabase;

	// Token: 0x04000764 RID: 1892
	[Dependency]
	private Diagnostics.StorageAuditTrail _auditTrail;

	// Token: 0x04000765 RID: 1893
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Player");
}
