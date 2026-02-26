using System;
using UnityEngine;

namespace Utils
{
	// Token: 0x0200027D RID: 637
	public static class Vector3IntExtensions
	{
		// Token: 0x06000FC5 RID: 4037 RVA: 0x000354D3 File Offset: 0x000336D3
		public static Vector3Int RotateCW2D(this Vector3Int vector)
		{
			return new Vector3Int(vector.y, -vector.x, 0);
		}

		// Token: 0x06000FC6 RID: 4038 RVA: 0x000354EA File Offset: 0x000336EA
		public static Vector3Int RotateCCW2D(this Vector3Int vector)
		{
			return new Vector3Int(-vector.y, vector.x, 0);
		}
	}
}
