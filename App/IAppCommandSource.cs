using System;
using System.Collections.Generic;

// Token: 0x0200015A RID: 346
public interface IAppCommandSource
{
	// Token: 0x06000790 RID: 1936
	void Start();

	// Token: 0x06000791 RID: 1937
	IEnumerable<IAppCommand> GetFrameCommands();

	// Token: 0x06000792 RID: 1938
	void SetRewiredMode(int mode);
}
