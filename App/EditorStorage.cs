using System;
using System.Collections.Generic;
using System.IO;
using Factory;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class EditorStorage : LocalFileStorage, ICreatedInScopeHandler
{
	// Token: 0x0600051B RID: 1307 RVA: 0x00011D20 File Offset: 0x0000FF20
	public override void LoadAll(Action loadCompleteCallback)
	{
		IStorableTypeHandler savedGameHandler = this._storableTypeHandlerRegistry.GetHandlerForType<IGameJournalSave>();
		if (savedGameHandler != null && Directory.Exists(EditorStorage.GlobalSavedGamePath))
		{
			foreach (string globalSavedGameFilepath in Directory.GetFiles(EditorStorage.GlobalSavedGamePath))
			{
				string filename = Path.GetFileName(globalSavedGameFilepath);
				if (!EditorStorage.IgnoredFilenames.Contains(filename))
				{
					byte[] globalStorableData;
					try
					{
						globalStorableData = File.ReadAllBytes(globalSavedGameFilepath);
					}
					catch (Exception exception)
					{
						LocalFileStorage.Log.Warn("Failed to load global saved game from {0}.\n{1}", new object[]
						{
							globalSavedGameFilepath,
							exception
						});
						goto IL_BD;
					}
					IStorable globalStorable = savedGameHandler.Load(globalStorableData);
					if (globalStorable == null)
					{
						LocalFileStorage.Log.Warn("Failed to load global saved game from {0}.", new object[]
						{
							globalSavedGameFilepath
						});
					}
					else
					{
						IGameJournalSave globalSavedGame = globalStorable as IGameJournalSave;
						if (globalSavedGame != null)
						{
							this._playerDatabase.AddGlobalSavedGame(globalSavedGame);
						}
					}
				}
				IL_BD:;
			}
		}
		base.LoadAll(loadCompleteCallback);
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x00011E10 File Offset: 0x00010010
	public void OnCreatedInScope(IScope scope)
	{
		EditorStorage.Instance = this;
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x00011E18 File Offset: 0x00010018
	private void ToggleStatusIssue(PersistentStorageServiceIssues issueToToggle)
	{
		this._status.issues = (this._status.issues ^ issueToToggle);
		base.SetStatus(this._status);
	}

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x0600051E RID: 1310 RVA: 0x00011E36 File Offset: 0x00010036
	// (set) Token: 0x0600051F RID: 1311 RVA: 0x00011E43 File Offset: 0x00010043
	private string StatusMessageKey
	{
		get
		{
			return this._status.messageKey;
		}
		set
		{
			if (this._status.messageKey != value)
			{
				this._status.messageKey = value;
				base.SetStatus(this._status);
			}
		}
	}

	// Token: 0x04000224 RID: 548
	private PersistentStorageServiceStatus _status;

	// Token: 0x04000225 RID: 549
	private static List<string> IgnoredFilenames = new List<string>
	{
		"Thumbs.db",
		"desktop.ini",
		".DS_Store",
		".Spotlight-V100",
		".Trashes"
	};

	// Token: 0x04000226 RID: 550
	[Dependency]
	private PlayerDatabase _playerDatabase;

	// Token: 0x04000227 RID: 551
	private static EditorStorage Instance;

	// Token: 0x04000228 RID: 552
	public static readonly string GlobalSavedGamePath = Path.Combine(Application.persistentDataPath, "EditorGameJournals");
}
