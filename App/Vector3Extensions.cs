using System;
using UnityEngine;

// Token: 0x02000270 RID: 624
public static class Vector3Extensions
{
	// Token: 0x06000EE8 RID: 3816 RVA: 0x000325EA File Offset: 0x000307EA
	public static void ScaleUniform(this Vector3 vector3, float scale)
	{
		vector3.Scale(new Vector3(scale, scale, scale));
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x000325FC File Offset: 0x000307FC
	public static bool IsCardinal2D(this Vector3 vector3, Vector3 other)
	{
		Vector3 delta = other - vector3;
		if (delta.x < 0f)
		{
			delta.x = -delta.x;
		}
		if (delta.y < 0f)
		{
			delta.y = -delta.y;
		}
		return delta.x < float.Epsilon || delta.y < float.Epsilon;
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x00032663 File Offset: 0x00030863
	public static Vector3 RotateCW2D(this Vector3 vector)
	{
		return new Vector3(vector.y, -vector.x, 0f);
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x0003267C File Offset: 0x0003087C
	public static Vector3 RotateCCW2D(this Vector3 vector)
	{
		return new Vector3(-vector.y, vector.x, 0f);
	}
}
