using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

// Token: 0x0200017B RID: 379
public class JournalAppCommandSource : IAppCommandSource
{
	// Token: 0x06000885 RID: 2181 RVA: 0x000022F5 File Offset: 0x000004F5
	public void Start()
	{
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x0001A760 File Offset: 0x00018960
	public IEnumerable<IAppCommand> GetFrameCommands()
	{
		this._frameCommands.Clear();
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			this._journalFramesPerRuntimeFrame++;
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			this._journalFramesPerRuntimeFrame = Mathf.Max(1, this._journalFramesPerRuntimeFrame - 1);
		}
		for (int journalFrameCount = 0; journalFrameCount < this._journalFramesPerRuntimeFrame; journalFrameCount++)
		{
			if (this._commandCursor < this._journal.EntryCount)
			{
				IAppCommand firstCommand = this._journal.GetEntry(this._commandCursor);
				float frameTimestamp = firstCommand.Timestamp;
				this._frameCommands.Add(firstCommand);
				this._commandCursor++;
				while (this._commandCursor < this._journal.EntryCount)
				{
					IAppCommand nextCommand = this._journal.GetEntry(this._commandCursor);
					if (nextCommand.Timestamp > frameTimestamp)
					{
						break;
					}
					this._frameCommands.Add(nextCommand);
					this._commandCursor++;
				}
			}
		}
		return this._frameCommands;
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetRewiredMode(int mode)
	{
	}

	// Token: 0x040003F0 RID: 1008
	[Dependency]
	private AppCommandJournal _journal;

	// Token: 0x040003F1 RID: 1009
	private int _commandCursor;

	// Token: 0x040003F2 RID: 1010
	private readonly List<IAppCommand> _frameCommands = new List<IAppCommand>(8);

	// Token: 0x040003F3 RID: 1011
	private int _journalFramesPerRuntimeFrame = 1;
}
