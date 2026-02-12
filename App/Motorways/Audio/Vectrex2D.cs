using System;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000663 RID: 1635
	public static class Vectrex2D
	{
		// Token: 0x06002D69 RID: 11625 RVA: 0x000D16DD File Offset: 0x000CF8DD
		public static Vector2 Swap(this Vector2 v2)
		{
			return new Vector2(v2.y, v2.x);
		}
	}
}
