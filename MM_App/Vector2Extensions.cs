using System;
using UnityEngine;

// Token: 0x0200026F RID: 623
public static class Vector2Extensions
{
	// Token: 0x06000EE2 RID: 3810 RVA: 0x0003253C File Offset: 0x0003073C
	public static Vector2 Rotated(this Vector2 vector, float angle)
	{
		float sin = Mathf.Sin(angle);
		float cos = Mathf.Cos(angle);
		return new Vector2(cos * vector.x - sin * vector.y, sin * vector.x + cos * vector.y);
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x0003257E File Offset: 0x0003077E
	public static Vector2 GetTangent(this Vector2 vector2)
	{
		return new Vector2(vector2.y, -vector2.x);
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x00032592 File Offset: 0x00030792
	public static Vector2 GetNormal(this Vector2 vector2)
	{
		return vector2.GetTangent();
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x0003259A File Offset: 0x0003079A
	public static float Cross(this Vector2 lhs, Vector2 rhs)
	{
		return lhs.x * rhs.y - lhs.y * rhs.x;
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x000325B7 File Offset: 0x000307B7
	public static Vector2Int GetNegatedVector(this Vector2Int vector2Int)
	{
		return new Vector2Int(-vector2Int.x, -vector2Int.y);
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x000325CE File Offset: 0x000307CE
	public static Vector3 ToVector3(this Vector2Int vector2Int)
	{
		return new Vector3((float)vector2Int.x, (float)vector2Int.y, 0f);
	}
}
