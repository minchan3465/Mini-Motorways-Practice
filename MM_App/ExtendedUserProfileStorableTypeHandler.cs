using System;
using Factory;

// Token: 0x02000202 RID: 514
public class ExtendedUserProfileStorableTypeHandler : IStorableTypeHandler
{
	// Token: 0x06000C28 RID: 3112 RVA: 0x0002956C File Offset: 0x0002776C
	public string GetFilename(IStorable storable)
	{
		IExtendedUserProfile extendedUserProfile = storable as IExtendedUserProfile;
		if (extendedUserProfile != null && Diagnostics.Verify(extendedUserProfile.Player != null, "You can't save an ExtendedUserProfile that hasn't been assigned to a Player."))
		{
			return this.GetFilename(extendedUserProfile.Player.Id, null);
		}
		return null;
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x000295AC File Offset: 0x000277AC
	public string GetFilename(string playerId, string deviceId)
	{
		return StorableUtilities.GenerateFilename("extendedUserProfile_", ".json", playerId);
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x000295BE File Offset: 0x000277BE
	public bool IsFilenameRecognized(string filename, out string playerId, out string deviceId)
	{
		deviceId = null;
		return StorableUtilities.TryParseFilename(filename, "extendedUserProfile_", ".json", out playerId);
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x000295D4 File Offset: 0x000277D4
	public IStorable Load(byte[] data)
	{
		IExtendedUserProfile newExtendedUserProfile = this._scope.Get<IExtendedUserProfile>();
		if (!StorableUtilities.LoadJsonStorable(newExtendedUserProfile, data))
		{
			this._scope.Release(newExtendedUserProfile);
			return null;
		}
		return newExtendedUserProfile;
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x00029608 File Offset: 0x00027808
	public byte[] Store(IStorable storable)
	{
		IExtendedUserProfile extendedUserProfile = storable as IExtendedUserProfile;
		if (extendedUserProfile != null)
		{
			return StorableUtilities.StoreJsonStorable(extendedUserProfile);
		}
		return null;
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x00029628 File Offset: 0x00027828
	public bool ProcessLoadedStorable(IStorable storable, string playerId, string deviceId)
	{
		IExtendedUserProfile extendedUserProfile = storable as IExtendedUserProfile;
		if (extendedUserProfile != null)
		{
			this._playerDatabase.AddExtendedUserProfile(extendedUserProfile, playerId);
			return true;
		}
		return false;
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x0002964F File Offset: 0x0002784F
	public void ProcessDeletedStorable(string playerId, string deviceId)
	{
		this._playerDatabase.RemovePlayer(playerId);
	}

	// Token: 0x0400070E RID: 1806
	[Dependency]
	private PlayerDatabase _playerDatabase;

	// Token: 0x0400070F RID: 1807
	[Dependency]
	private IScope _scope;

	// Token: 0x04000710 RID: 1808
	private const string FilenamePrefix = "extendedUserProfile_";

	// Token: 0x04000711 RID: 1809
	private const string FilenameExtension = ".json";
}
