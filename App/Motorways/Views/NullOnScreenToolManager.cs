using System;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005B2 RID: 1458
	public class NullOnScreenToolManager : IOnScreenToolManager
	{
		// Token: 0x06002894 RID: 10388 RVA: 0x0000222C File Offset: 0x0000042C
		public bool IsPointInsideTool(Vector2 coordinates)
		{
			return false;
		}
	}
}
