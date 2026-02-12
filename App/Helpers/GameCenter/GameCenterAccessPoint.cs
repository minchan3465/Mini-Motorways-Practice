using System;
using UnityEngine;

namespace Helpers.GameCenter
{
	// Token: 0x0200078C RID: 1932
	public class GameCenterAccessPoint : IGameCenterAccessPoint
	{
		// Token: 0x0600356B RID: 13675 RVA: 0x000F9B01 File Offset: 0x000F7D01
		public bool IsAvailable()
		{
			return GameCenterShared.GCIsAccessPointAvailable();
		}

		// Token: 0x0600356C RID: 13676 RVA: 0x000F9B08 File Offset: 0x000F7D08
		public void Show()
		{
			GameCenterShared.GCShowAccessPoint();
		}

		// Token: 0x0600356D RID: 13677 RVA: 0x000F9B0F File Offset: 0x000F7D0F
		public void Hide()
		{
			GameCenterShared.GCHideAccessPoint();
		}

		// Token: 0x0600356E RID: 13678 RVA: 0x000F9B16 File Offset: 0x000F7D16
		public Rect GetRect()
		{
			return new Rect(GameCenterShared.GCGetAccessPointOriginX(), GameCenterShared.GCGetAccessPointOriginY(), GameCenterShared.GCGetAccessPointSizeWidth(), GameCenterShared.GCGetAccessPointSizeHeight());
		}

		// Token: 0x0600356F RID: 13679 RVA: 0x000F9B31 File Offset: 0x000F7D31
		public void Select()
		{
			GameCenterShared.GCSelectAccessPoint();
		}
	}
}
