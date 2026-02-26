using System;
using Rewired;
using UnityEngine;

// Token: 0x02000239 RID: 569
public class ApplicationFocusListener : MonoBehaviour
{
	// Token: 0x06000D76 RID: 3446 RVA: 0x0002C4CC File Offset: 0x0002A6CC
	public void Initialize(RuntimeAppCommandSource appCommandSource)
	{
		this._appCommandSource = appCommandSource;
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x0002C4D5 File Offset: 0x0002A6D5
	private void OnApplicationFocus(bool hasFocus)
	{
		this._appCommandSource.AppHasFocus = hasFocus;
	}

	// Token: 0x040007A7 RID: 1959
	private RuntimeAppCommandSource _appCommandSource;

	// Token: 0x040007A8 RID: 1960
	public static Controller LastKnownController;
}
