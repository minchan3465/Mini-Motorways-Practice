using System;
using UnityEngine;

// Token: 0x02000238 RID: 568
public class ModeInfoPopupInGame : ModeInfoPopup
{
	// Token: 0x06000D73 RID: 3443 RVA: 0x0002C47B File Offset: 0x0002A67B
	public override void OnOpened(float delay)
	{
		this._popupParent.SetTempOffset(this.BlurOffsetOverrideDay, this.BlurOffsetOverrideNight);
		base.OnOpened(delay);
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x0002C49B File Offset: 0x0002A69B
	public override void OnPopupClosed()
	{
		this._popupParent.ClearTempRange();
		base.OnPopupClosed();
	}

	// Token: 0x040007A5 RID: 1957
	[SerializeField]
	private float BlurOffsetOverrideDay = 0.6f;

	// Token: 0x040007A6 RID: 1958
	[SerializeField]
	private float BlurOffsetOverrideNight = 0.35f;
}
