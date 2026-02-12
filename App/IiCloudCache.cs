using System;
using System.Collections.Generic;

// Token: 0x02000111 RID: 273
public interface IiCloudCache
{
	// Token: 0x060005C8 RID: 1480
	bool HasFile(string filepath);

	// Token: 0x060005C9 RID: 1481
	byte[] ReadFile(string filepath);

	// Token: 0x060005CA RID: 1482
	bool WriteFile(string filepath, byte[] data);

	// Token: 0x060005CB RID: 1483
	bool HasSpaceToWriteFile(string filepath, int dataLength, out int bytesNeededToDelete);

	// Token: 0x060005CC RID: 1484
	IEnumerable<string> GetFilenamesInDirectory(string directory);

	// Token: 0x060005CD RID: 1485
	IEnumerable<string> GetDirectoriesInDirectory(string directory);

	// Token: 0x060005CE RID: 1486
	int GetFileSize(string filepath);

	// Token: 0x060005CF RID: 1487
	bool MoveFile(string filepath, string directory);

	// Token: 0x060005D0 RID: 1488
	bool DeleteFile(string filepath);

	// Token: 0x060005D1 RID: 1489
	void CopyNewFilesInDirectory(string sourceDirectory, string destinationDirectory);

	// Token: 0x060005D2 RID: 1490
	DateTime GetFileModifiedTime(string filepath);
}
