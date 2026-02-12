using System;
using System.Collections.Generic;
using JetBrains.Annotations;

// Token: 0x020000E8 RID: 232
public interface IFileSystem
{
	// Token: 0x060004C0 RID: 1216
	[NotNull]
	List<string> GetFilesInDirectory(string directory);

	// Token: 0x060004C1 RID: 1217
	[NotNull]
	List<string> GetDirectoriesInDirectory(string directory);

	// Token: 0x060004C2 RID: 1218
	[CanBeNull]
	byte[] ReadFile([NotNull] string filepath);

	// Token: 0x060004C3 RID: 1219
	bool WriteFile([NotNull] string filepath, [NotNull] byte[] data);

	// Token: 0x060004C4 RID: 1220
	bool DeleteFile([NotNull] string filepath);
}
