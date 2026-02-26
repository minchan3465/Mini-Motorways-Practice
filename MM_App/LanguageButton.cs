using System;
using Motorways.UI;
using Motorways.Views;
using TMPro;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class LanguageButton : MonoBehaviour
{
	// Token: 0x17000274 RID: 628
	// (get) Token: 0x06000B00 RID: 2816 RVA: 0x00024EBD File Offset: 0x000230BD
	// (set) Token: 0x06000B01 RID: 2817 RVA: 0x00024EC5 File Offset: 0x000230C5
	public int LocaleIndex { get; private set; }

	// Token: 0x17000275 RID: 629
	// (get) Token: 0x06000B02 RID: 2818 RVA: 0x00024ECE File Offset: 0x000230CE
	// (set) Token: 0x06000B03 RID: 2819 RVA: 0x00024ED6 File Offset: 0x000230D6
	public LocaleDatabase.LocaleId LocaleId { get; private set; }

	// Token: 0x06000B04 RID: 2820 RVA: 0x00024EE0 File Offset: 0x000230E0
	public void Initialize(Locale locale, int localeIndex, FontDatabase fonts, OptionsScreenBase optionsScreen, ToggleButtonGroup group, bool isSelected)
	{
		this.text.text = locale.Name;
		this.LocaleId = locale.Id;
		this.text.font = fonts.GetFont(locale.Charset).FontAsset;
		this.text.isRightToLeftText = (locale.TextDirection == TextDirection.RightToLeft);
		this.LocaleIndex = localeIndex;
		this.optionsScreen = optionsScreen;
		group.RegisterToggle(this.touchToggle);
		this.touchToggle.IsOn = isSelected;
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x00024F63 File Offset: 0x00023163
	public void OnClick()
	{
		this.optionsScreen.SetLocale(this.LocaleIndex);
	}

	// Token: 0x0400061F RID: 1567
	public TextMeshProUGUI text;

	// Token: 0x04000620 RID: 1568
	public TouchToggle touchToggle;

	// Token: 0x04000623 RID: 1571
	public OptionsScreenBase optionsScreen;
}
