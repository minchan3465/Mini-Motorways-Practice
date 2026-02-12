using System;
using TMPro;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class CouldNotLoadLibrariesPopup : MonoBehaviour
{
	// Token: 0x06000A9F RID: 2719 RVA: 0x000232C3 File Offset: 0x000214C3
	private void Awake()
	{
		this.SetTextFromLocalization(this.headerText, StringId.Error_MotorwaysDLL_Title);
		this.SetTextFromLocalization(this.bodyText, StringId.Error_MotorwaysDLL_Description);
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x000232E8 File Offset: 0x000214E8
	public void SetTextFromLocalization(TextMeshProUGUI textMeshProUGUI, StringId stringId)
	{
		string localizedString;
		TMP_FontAsset fontAsset;
		if (this._localizer.GetLocalization(stringId, out localizedString, out fontAsset))
		{
			textMeshProUGUI.font = fontAsset;
			textMeshProUGUI.text = localizedString;
		}
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x00023318 File Offset: 0x00021518
	public void SetMissingLibraryFilename(string filename)
	{
		if (this.headerText != null)
		{
			string headerWithFilename = this.headerText.text.Replace("{filename}", filename);
			this.headerText.text = headerWithFilename;
		}
		if (this.bodyText != null)
		{
			string bodyWithFilename = this.bodyText.text.Replace("{filename}", filename);
			this.bodyText.text = bodyWithFilename;
		}
	}

	// Token: 0x040005B1 RID: 1457
	private const string filenameToken = "{filename}";

	// Token: 0x040005B2 RID: 1458
	public TextMeshProUGUI headerText;

	// Token: 0x040005B3 RID: 1459
	public TextMeshProUGUI bodyText;

	// Token: 0x040005B4 RID: 1460
	public BakedLocalizer _localizer;
}
