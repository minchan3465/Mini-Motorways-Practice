using System;
using UnityEngine;

// Token: 0x020000CC RID: 204
public class NullGameCenterAccessPoint : IGameCenterAccessPoint
{
	// Token: 0x06000426 RID: 1062 RVA: 0x0000222C File Offset: 0x0000042C
	public bool IsAvailable()
	{
		return false;
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x000022F5 File Offset: 0x000004F5
	public void Show()
	{
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x000022F5 File Offset: 0x000004F5
	public void Hide()
	{
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x0000F468 File Offset: 0x0000D668
	public Rect GetRect()
	{
		return Rect.zero;
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x000022F5 File Offset: 0x000004F5
	public void Select()
	{
	}
}
