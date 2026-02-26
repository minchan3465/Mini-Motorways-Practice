using System;
using Factory;

// Token: 0x02000231 RID: 561
public class SavedGameStorableTypeHandler : IStorableTypeHandler
{
	// Token: 0x06000D52 RID: 3410 RVA: 0x0002BC44 File Offset: 0x00029E44
	public string GetFilename(IStorable storable)
	{
		IGameJournalSave savedGame = storable as IGameJournalSave;
		if (savedGame != null)
		{
			return this.GetFilename(savedGame.Player.Id, this._hardwareCapabilities.UniqueDeviceId);
		}
		return null;
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x0002BC79 File Offset: 0x00029E79
	public string GetFilename(string playerId, string deviceId)
	{
		return StorableUtilities.GenerateFilename("gameJournal_", ".dat", playerId, deviceId);
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x0002BC8C File Offset: 0x00029E8C
	public bool IsFilenameRecognized(string filename, out string playerId, out string deviceId)
	{
		return StorableUtilities.TryParseFilename(filename, "gameJournal_", ".dat", out playerId, out deviceId);
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x0002BCA0 File Offset: 0x00029EA0
	public IStorable Load(byte[] data)
	{
		IGameJournalSave newSavedGame = this._scope.Get<IGameJournalSave>();
		if (StorableUtilities.LoadBinaryStorable(newSavedGame, data) != StorableUtilities.LoadResult.Success)
		{
			this._scope.Release(newSavedGame);
			return null;
		}
		return newSavedGame;
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x0002BCD4 File Offset: 0x00029ED4
	public byte[] Store(IStorable storable)
	{
		IGameJournalSave savedGame = storable as IGameJournalSave;
		if (savedGame != null)
		{
			return StorableUtilities.StoreBinaryStorable(savedGame);
		}
		return null;
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x0002BCF4 File Offset: 0x00029EF4
	public bool ProcessLoadedStorable(IStorable storable, string playerId, string deviceId)
	{
		IGameJournalSave savedGame = storable as IGameJournalSave;
		if (savedGame != null)
		{
			this._playerDatabase.AddSavedGame(playerId, deviceId, savedGame);
			return true;
		}
		return false;
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x0002BD1C File Offset: 0x00029F1C
	public void ProcessDeletedStorable(string playerId, string deviceId)
	{
		Player player = this._playerDatabase.GetPlayer(playerId);
		if (player == null)
		{
			return;
		}
		player.RemoveSavedGame(deviceId);
	}

	// Token: 0x04000783 RID: 1923
	[Dependency]
	private IHardwareCapabilities _hardwareCapabilities;

	// Token: 0x04000784 RID: 1924
	[Dependency]
	private PlayerDatabase _playerDatabase;

	// Token: 0x04000785 RID: 1925
	[Dependency]
	private IScope _scope;

	// Token: 0x04000786 RID: 1926
	private const string FilenamePrefix = "gameJournal_";

	// Token: 0x04000787 RID: 1927
	private const string FilenameExtension = ".dat";
}
