using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class MultiDisplayCapabilitiesBridge
{
	// Token: 0x060003CA RID: 970 RVA: 0x0000EDFC File Offset: 0x0000CFFC
	public static int GetDisplayCount()
	{
		int result;
		try
		{
			result = MultiDisplayCapabilitiesBridge.GetDisplayCountNative();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			result = 1;
		}
		return result;
	}

	// Token: 0x060003CB RID: 971 RVA: 0x0000EE2C File Offset: 0x0000D02C
	public static int GetActiveDisplayIndex()
	{
		int result;
		try
		{
			result = MultiDisplayCapabilitiesBridge.GetActiveDisplayIndexNative();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			result = 1;
		}
		return result;
	}

	// Token: 0x060003CC RID: 972 RVA: 0x0000EE5C File Offset: 0x0000D05C
	public static bool SetActiveDisplayIndex(int index)
	{
		bool result;
		try
		{
			result = MultiDisplayCapabilitiesBridge.SetActiveDisplayIndexNative(index);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			result = false;
		}
		return result;
	}

	// Token: 0x060003CD RID: 973
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetDisplayCount")]
	private static extern int GetDisplayCountNative();

	// Token: 0x060003CE RID: 974
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetActiveDisplayIndex")]
	private static extern int GetActiveDisplayIndexNative();

	// Token: 0x060003CF RID: 975
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetActiveDisplay")]
	private static extern bool SetActiveDisplayIndexNative(int displayIndex);

	// Token: 0x04000192 RID: 402
	public const string LibraryName = "dpcPlatform";
}
