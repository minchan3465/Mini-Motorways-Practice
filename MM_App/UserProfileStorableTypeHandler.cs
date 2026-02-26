using System;
using Factory;

// Token: 0x02000236 RID: 566
public class UserProfileStorableTypeHandler : IStorableTypeHandler
{
	// Token: 0x06000D66 RID: 3430 RVA: 0x0002C17C File Offset: 0x0002A37C
	public string GetFilename(IStorable storable)
	{
		ILegacyUserProfile userProfile = storable as ILegacyUserProfile;
		if (userProfile != null && Diagnostics.Verify(userProfile.Player != null, "You can't save a LegacyUserProfile that hasn't been assigned to a Player."))
		{
			return this.GetFilename(userProfile.Player.Id, null);
		}
		return null;
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x0002C1BC File Offset: 0x0002A3BC
	public string GetFilename(string playerId, string deviceId)
	{
		return StorableUtilities.GenerateFilename("userProfile_", ".json", playerId);
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x0002C1CE File Offset: 0x0002A3CE
	public bool IsFilenameRecognized(string filename, out string playerId, out string deviceId)
	{
		deviceId = null;
		return StorableUtilities.TryParseFilename(filename, "userProfile_", ".json", out playerId);
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x0002C1E4 File Offset: 0x0002A3E4
	public IStorable Load(byte[] data)
	{
		ILegacyUserProfile newUserProfile = this._scope.Get<ILegacyUserProfile>();
		if (!StorableUtilities.LoadJsonStorable(newUserProfile, data))
		{
			this._scope.Release(newUserProfile);
			return null;
		}
		return newUserProfile;
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x0002C218 File Offset: 0x0002A418
	public byte[] Store(IStorable storable)
	{
		ILegacyUserProfile userProfile = storable as ILegacyUserProfile;
		if (userProfile != null)
		{
			return StorableUtilities.StoreJsonStorable(userProfile);
		}
		return null;
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x0002C238 File Offset: 0x0002A438
	public bool ProcessLoadedStorable(IStorable storable, string playerId, string deviceId)
	{
		ILegacyUserProfile userProfile = storable as ILegacyUserProfile;
		if (userProfile != null)
		{
			this._playerDatabase.AddUserProfile(userProfile, playerId);
			return true;
		}
		return false;
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x0002C25F File Offset: 0x0002A45F
	public void ProcessDeletedStorable(string playerId, string deviceId)
	{
		this._playerDatabase.RemovePlayer(playerId);
	}

	// Token: 0x04000791 RID: 1937
	[Dependency]
	private PlayerDatabase _playerDatabase;

	// Token: 0x04000792 RID: 1938
	[Dependency]
	private IScope _scope;

	// Token: 0x04000793 RID: 1939
	private const string FilenamePrefix = "userProfile_";

	// Token: 0x04000794 RID: 1940
	private const string FilenameExtension = ".json";
}
