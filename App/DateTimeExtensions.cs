using System;

// Token: 0x02000258 RID: 600
public static class DateTimeExtensions
{
	// Token: 0x06000E3D RID: 3645 RVA: 0x00030224 File Offset: 0x0002E424
	public static void ToInts(this DateTime fromDateTime, out int lowBits, out int highBits)
	{
		long saveTimeLong = fromDateTime.ToBinary();
		lowBits = (int)saveTimeLong;
		highBits = (int)(saveTimeLong >> 32);
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x00030244 File Offset: 0x0002E444
	public static DateTime FromInts(int lowBits, int highBits)
	{
		return DateTime.FromBinary((long)highBits << 32 | (long)((ulong)lowBits));
	}
}
