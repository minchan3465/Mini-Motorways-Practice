using System;
using Factory;
using Motorways.UI;
using Popups;
using Screens;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200023D RID: 573
public class MotorwaysMenuNavigation : MenuNavigation
{
	// Token: 0x06000D97 RID: 3479 RVA: 0x0002CCBC File Offset: 0x0002AEBC
	public override bool ActivateSelected()
	{
		if (this._activeFocus != null)
		{
			if (typeof(TouchButton).IsAssignableFrom(this._activeFocus.GetType()))
			{
				((TouchButton)this._activeFocus).OnSubmit(null);
				return true;
			}
			if (typeof(TouchToggle).IsAssignableFrom(this._activeFocus.GetType()))
			{
				((TouchToggle)this._activeFocus).OnSubmit(null);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x0002CD38 File Offset: 0x0002AF38
	public override void BackActivated()
	{
		IScreen topScreen = this._screenStack.GetTopVisibleScreen();
		if (this._popupStack.HasActivePopups && this._popupStack.GetTopPopup().CanBeDismissed())
		{
			this._popupStack.PopPopup(false);
			return;
		}
		BaseScalingScreen baseScalingScreen = topScreen as BaseScalingScreen;
		if (baseScalingScreen != null)
		{
			baseScalingScreen.BackActivated();
		}
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x0002CD90 File Offset: 0x0002AF90
	public override void PageSelected(Vector2 direction)
	{
		BaseScalingScreen baseScalingScreen = this._screenStack.GetTopVisibleScreen() as BaseScalingScreen;
		if (baseScalingScreen != null)
		{
			baseScalingScreen.PageSelected(direction);
		}
	}

	// Token: 0x040007B8 RID: 1976
	[Dependency]
	protected ScreenStack _screenStack;

	// Token: 0x040007B9 RID: 1977
	[Dependency]
	protected PopupStack _popupStack;
}
