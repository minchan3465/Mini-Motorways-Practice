using System;
using Factory;
using JetBrains.Annotations;
using Motorways;
using Popups;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000237 RID: 567
public class ModeInfoPopup : BasePopup
{
	// Token: 0x06000D6E RID: 3438 RVA: 0x0002C270 File Offset: 0x0002A470
	public void Initialize(IScope scope, GameMode gameMode, Action onConfirmed = null)
	{
		switch (gameMode)
		{
		case GameMode.Endless:
			this._headerText.SetStringId(scope, StringId.Endless_Mode_Name);
			this._info1TitleText.SetStringId(scope, StringId.ModeInfoPopup_Endless1_Title);
			this._info1BodyText.SetStringId(scope, StringId.ModeInfoPopup_Endless1_Body);
			this._info2TitleText.SetStringId(scope, StringId.ModeInfoPopup_Endless2_Title);
			this._info2BodyText.SetStringId(scope, StringId.ModeInfoPopup_Endless2_Body);
			this._image1.sprite = this._endlessSprite1;
			this._image2.sprite = this._endlessSprite2;
			break;
		case GameMode.Expert:
			this._headerText.SetStringId(scope, StringId.Expert_Mode_Name);
			this._info1TitleText.SetStringId(scope, StringId.ModeInfoPopup_Expert1_Title);
			this._info1BodyText.SetStringId(scope, StringId.ModeInfoPopup_Expert1_Body);
			this._info2TitleText.SetStringId(scope, StringId.ModeInfoPopup_Expert2_Title);
			this._info2BodyText.SetStringId(scope, StringId.ModeInfoPopup_Expert2_Body);
			this._image1.sprite = this._expertSprite1;
			this._image2.sprite = this._expertSprite2;
			break;
		case GameMode.Creative:
			this._headerText.SetStringId(scope, StringId.Creative_Mode_Name);
			this._info1TitleText.SetStringId(scope, StringId.Tutorial_CreativeMode_Info1_Header);
			if (scope.Get<InputState>().CurrentDeviceInputType == DeviceInputType.Mouse)
			{
				this._info1BodyText.SetStringId(scope, StringId.Tutorial_CreativeMode_Info1_Body_Mouse);
			}
			else
			{
				this._info1BodyText.SetStringId(scope, StringId.Tutorial_CreativeMode_Info1_Body_TouchOrController);
			}
			this._info2TitleText.SetStringId(scope, StringId.Tutorial_CreativeMode_Info2_Header);
			this._info2BodyText.SetStringId(scope, StringId.Tutorial_CreativeMode_Info2_Body);
			this._image1.sprite = this._creativeSprite1;
			this._image2.sprite = this._creativeSprite2;
			break;
		}
		this._onConfirmed = onConfirmed;
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x0002C445 File Offset: 0x0002A645
	public override void OnPopupClosed()
	{
		base.OnPopupClosed();
		Action onConfirmed = this._onConfirmed;
		if (onConfirmed == null)
		{
			return;
		}
		onConfirmed();
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x0002C45D File Offset: 0x0002A65D
	[UsedImplicitly]
	public void ClosePressed()
	{
		this._popupStack.PopPopup(false);
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x0002C46B File Offset: 0x0002A66B
	public override void Reset()
	{
		base.Reset();
	}

	// Token: 0x04000795 RID: 1941
	[Dependency]
	protected PopupStack _popupStack;

	// Token: 0x04000796 RID: 1942
	[Dependency]
	protected GameCamera _gameCamera;

	// Token: 0x04000797 RID: 1943
	[SerializeField]
	protected LocalizedTextUI _headerText;

	// Token: 0x04000798 RID: 1944
	[SerializeField]
	protected LocalizedTextUI _info1TitleText;

	// Token: 0x04000799 RID: 1945
	[SerializeField]
	protected LocalizedTextUI _info2TitleText;

	// Token: 0x0400079A RID: 1946
	[SerializeField]
	protected LocalizedTextUI _info1BodyText;

	// Token: 0x0400079B RID: 1947
	[SerializeField]
	protected LocalizedTextUI _info2BodyText;

	// Token: 0x0400079C RID: 1948
	[SerializeField]
	private Image _image1;

	// Token: 0x0400079D RID: 1949
	[SerializeField]
	private Image _image2;

	// Token: 0x0400079E RID: 1950
	[SerializeField]
	private Sprite _endlessSprite1;

	// Token: 0x0400079F RID: 1951
	[SerializeField]
	private Sprite _endlessSprite2;

	// Token: 0x040007A0 RID: 1952
	[SerializeField]
	private Sprite _expertSprite1;

	// Token: 0x040007A1 RID: 1953
	[SerializeField]
	private Sprite _expertSprite2;

	// Token: 0x040007A2 RID: 1954
	[SerializeField]
	private Sprite _creativeSprite1;

	// Token: 0x040007A3 RID: 1955
	[SerializeField]
	private Sprite _creativeSprite2;

	// Token: 0x040007A4 RID: 1956
	private Action _onConfirmed;
}
