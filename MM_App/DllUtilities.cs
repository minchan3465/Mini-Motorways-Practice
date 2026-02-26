using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020001E5 RID: 485
public static class DllUtilities
{
	// Token: 0x06000B8B RID: 2955 RVA: 0x00027A48 File Offset: 0x00025C48
	public static bool AreLibrariesLoaded(out string missingLibraryFilename)
	{
		missingLibraryFilename = null;
		try
		{
			DllUtilities.EmptyAudioLibraryFunction();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			missingLibraryFilename = "decompressAudio";
		}
		try
		{
			DllUtilities.EmptyLibraryFunction();
		}
		catch (Exception exception2)
		{
			Debug.LogException(exception2);
			missingLibraryFilename = "dpcPlatform";
		}
		if (missingLibraryFilename != null)
		{
			missingLibraryFilename += ".dll";
			return false;
		}
		return true;
	}

	// Token: 0x06000B8C RID: 2956
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "EmptyFunction")]
	private static extern int EmptyLibraryFunction();

	// Token: 0x06000B8D RID: 2957
	[DllImport("decompressAudio", CallingConvention = CallingConvention.Cdecl, EntryPoint = "EmptyFunction")]
	private static extern int EmptyAudioLibraryFunction();

	// Token: 0x040006AC RID: 1708
	public const string InternalLibraryName = "__Internal";

	// Token: 0x040006AD RID: 1709
	public const string LibraryName = "dpcPlatform";

	// Token: 0x040006AE RID: 1710
	public const string AudioLibraryName = "decompressAudio";

	// Token: 0x040006AF RID: 1711
	public const string ArcadeLibraryName = "arcadex";
}
