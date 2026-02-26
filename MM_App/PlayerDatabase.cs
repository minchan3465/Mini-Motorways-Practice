using System;
using System.Collections.Generic;
using System.Globalization;
using Factory;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class PlayerDatabase
{
	// Token: 0x06000D34 RID: 3380 RVA: 0x0002B540 File Offset: 0x00029740
	public Player CreatePlayer()
	{
		string newPlayerId;
		do
		{
			newPlayerId = Guid.NewGuid().ToString().Replace("-", "");
		}
		while (this._players.ContainsKey(newPlayerId));
		Player newPlayer = this.CreatePlayer(newPlayerId);
		Locale currentLocale = this._localeDatabase.CurrentLocale;
		if (currentLocale != null)
		{
			newPlayer.LocaleId = currentLocale.Id;
		}
		return newPlayer;
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x0002B5A4 File Offset: 0x000297A4
	public void RemovePlayer(Player player)
	{
		this._auditTrail.RecordEvent("PlayerDatabase.RemovePlayer", delegate(Dictionary<string, string> metadata)
		{
			metadata["playerId"] = player.Id;
		});
		this._players.Remove(player.Id);
		if (this._activePlayer.Player == player)
		{
			Player newActivePlayer = this.MostRecentPlayer ?? this.CreatePlayer();
			this._activePlayer.ActivatePlayer(newActivePlayer);
		}
		this._scope.Release(player);
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x0002B634 File Offset: 0x00029834
	public void RemovePlayer(string playerId)
	{
		Player playerToRemove = this.GetPlayer(playerId);
		if (playerToRemove != null)
		{
			this.RemovePlayer(playerToRemove);
		}
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x0002B653 File Offset: 0x00029853
	public void DeletePlayer(Player player)
	{
		this._storage.DeletePlayer(player.Id);
		this.RemovePlayer(player);
	}

	// Token: 0x170002DF RID: 735
	// (get) Token: 0x06000D38 RID: 3384 RVA: 0x0002B670 File Offset: 0x00029870
	public Player MostRecentPlayer
	{
		get
		{
			Player mostRecentPlayer = null;
			foreach (Player player in this._players.Values)
			{
				if (mostRecentPlayer == null || Player.CompareAccessTime(player, mostRecentPlayer) > 0)
				{
					mostRecentPlayer = player;
				}
			}
			if (mostRecentPlayer == null)
			{
				PlayerDatabase.Log.Info("No most recent player found in the database.", Array.Empty<object>());
				return null;
			}
			PlayerDatabase.Log.Info("Selecting player {0} as the most recent player on this device. Last played time was {1}, last timestamp was {2}.", new object[]
			{
				mostRecentPlayer.Id,
				mostRecentPlayer.DeviceSettings.LastPlayedUtcTime.ToString(DateTimeFormatInfo.InvariantInfo),
				mostRecentPlayer.LastPlayedUtcTimeOnLocalDevice.ToString(DateTimeFormatInfo.InvariantInfo)
			});
			return mostRecentPlayer;
		}
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x0002B73C File Offset: 0x0002993C
	public Player GetPlayer(string playerId)
	{
		Player player;
		if (this._players.TryGetValue(playerId, out player))
		{
			return player;
		}
		return null;
	}

	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x06000D3A RID: 3386 RVA: 0x0002B75C File Offset: 0x0002995C
	public IEnumerable<Player> Players
	{
		get
		{
			return this._players.Values;
		}
	}

	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x06000D3B RID: 3387 RVA: 0x0002B769 File Offset: 0x00029969
	public int PlayerCount
	{
		get
		{
			return this._players.Count;
		}
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x0002B778 File Offset: 0x00029978
	public void AddUserProfile(ILegacyUserProfile newUserProfile, string playerId)
	{
		using (this._auditTrail.OpenEvent("PlayerDatabase.AddUserProfile", delegate(Dictionary<string, string> metadata)
		{
			metadata["playerId"] = playerId;
			metadata["newTimestamp"] = newUserProfile.UtcTimestamp.ToString(DateTimeFormatInfo.InvariantInfo);
			metadata["newJson"] = Json.Serialize(newUserProfile.SerializeToJson(), false);
		}))
		{
			this.GetOrCreatePlayer(playerId).MergeUserProfile(newUserProfile);
		}
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x0002B7EC File Offset: 0x000299EC
	public void AddExtendedUserProfile(IExtendedUserProfile newExtendedUserProfile, string playerId)
	{
		using (this._auditTrail.OpenEvent("PlayerDatabase.AddExtendedUserProfile", delegate(Dictionary<string, string> metadata)
		{
			metadata["playerId"] = playerId;
			metadata["newTimestamp"] = newExtendedUserProfile.UtcTimestamp.ToString(DateTimeFormatInfo.InvariantInfo);
			metadata["newJson"] = Json.Serialize(newExtendedUserProfile.SerializeToJson(), false);
		}))
		{
			this.GetOrCreatePlayer(playerId).MergeExtendedUserProfile(newExtendedUserProfile);
		}
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x0002B860 File Offset: 0x00029A60
	public void AddDeviceSettings(IDeviceSettings newDeviceSettings, string playerId, string deviceId)
	{
		using (this._auditTrail.OpenEvent("PlayerDatabase.AddDeviceSettings", delegate(Dictionary<string, string> metadata)
		{
			metadata["playerId"] = playerId;
			metadata["deviceId"] = deviceId;
			metadata["newTimestamp"] = newDeviceSettings.UtcTimestamp.ToString(DateTimeFormatInfo.InvariantInfo);
			metadata["newJson"] = Json.Serialize(newDeviceSettings.SerializeToJson(), false);
		}))
		{
			if (deviceId == this._hardwareCapabilities.UniqueDeviceId || deviceId == PlayerDatabase.LegacyDeviceId)
			{
				this.GetOrCreatePlayer(playerId).MergeDeviceSettings(newDeviceSettings);
			}
		}
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x0002B904 File Offset: 0x00029B04
	public void AddSavedGame(string playerId, string deviceId, IGameJournalSave newSavedGame)
	{
		using (this._auditTrail.OpenEvent("PlayerDatabase.AddSavedGame", delegate(Dictionary<string, string> metadata)
		{
			metadata["playerId"] = playerId;
			metadata["deviceId"] = deviceId;
		}))
		{
			Player player = this.GetOrCreatePlayer(playerId);
			if (deviceId == this._hardwareCapabilities.UniqueDeviceId || deviceId == PlayerDatabase.LegacyDeviceId)
			{
				newSavedGame.DeviceId = this._hardwareCapabilities.UniqueDeviceId;
				player.LocalSavedGame = newSavedGame;
			}
			else
			{
				newSavedGame.DeviceId = deviceId;
				player.AddForeignSavedGame(newSavedGame);
			}
		}
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x0002B9C4 File Offset: 0x00029BC4
	public void AddGlobalSavedGame(IGameJournalSave newGlobalSavedGame)
	{
		this._globalSavedGames.Add(newGlobalSavedGame);
	}

	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x06000D41 RID: 3393 RVA: 0x0002B9D2 File Offset: 0x00029BD2
	public IList<IGameJournalSave> GlobalSavedGames
	{
		get
		{
			return this._globalSavedGames;
		}
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x0002B9DC File Offset: 0x00029BDC
	private Player GetOrCreatePlayer(string playerId)
	{
		Player player;
		if (this._players.TryGetValue(playerId, out player))
		{
			return player;
		}
		return this.CreatePlayer(playerId);
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x0002BA04 File Offset: 0x00029C04
	private Player CreatePlayer(string playerId)
	{
		PlayerDatabase.Log.Info("Creating new player with id {0}.", new object[]
		{
			playerId
		});
		Player newPlayer = this._scope.Get<Player>();
		newPlayer.Initialize(playerId);
		this._players[playerId] = newPlayer;
		this._auditTrail.RecordEvent("PlayerDatabase.CreatePlayer", delegate(Dictionary<string, string> metadata)
		{
			metadata["playerId"] = playerId;
		});
		return newPlayer;
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x0002BA83 File Offset: 0x00029C83
	static PlayerDatabase()
	{
		PlayerDatabase.LegacyDeviceId = HashUtils.GetMD5(SystemInfo.deviceName);
	}

	// Token: 0x0400076E RID: 1902
	private readonly Dictionary<string, Player> _players = new Dictionary<string, Player>();

	// Token: 0x0400076F RID: 1903
	private readonly List<IGameJournalSave> _globalSavedGames = new List<IGameJournalSave>();

	// Token: 0x04000770 RID: 1904
	[Dependency]
	private IScope _scope;

	// Token: 0x04000771 RID: 1905
	[Dependency]
	private IHardwareCapabilities _hardwareCapabilities;

	// Token: 0x04000772 RID: 1906
	[Dependency]
	private IActivePlayer _activePlayer;

	// Token: 0x04000773 RID: 1907
	[Dependency]
	private LocaleDatabase _localeDatabase;

	// Token: 0x04000774 RID: 1908
	[Dependency]
	private IPersistentStorageService _storage;

	// Token: 0x04000775 RID: 1909
	[Dependency]
	private Diagnostics.StorageAuditTrail _auditTrail;

	// Token: 0x04000776 RID: 1910
	public static readonly string LegacyDeviceId;

	// Token: 0x04000777 RID: 1911
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("PlayerDatabase");
}
