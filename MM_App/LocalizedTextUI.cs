using System;
using Factory;
using TMPro;
using UnityEngine;

// Token: 0x02000262 RID: 610
[RequireComponent(typeof(TMP_Text), typeof(TextMeshProUGUI))]
public class LocalizedTextUI : MonoBehaviour, ILocalized
{
	// Token: 0x06000E77 RID: 3703 RVA: 0x000311E8 File Offset: 0x0002F3E8
	public bool SetStringId(IScope scope, StringId stringId)
	{
		if (!this.isInitialized)
		{
			this.startingStringIdString = stringId.ToString();
			return true;
		}
		StandaloneLocString newLocString = StandaloneLocString.CreateString(scope, stringId);
		if (Diagnostics.Verify(newLocString != null, "Could not find string for {0}.", stringId))
		{
			this.LocString = newLocString;
			return true;
		}
		return false;
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0003123C File Offset: 0x0002F43C
	public bool SetStringId(IScope scope, string stringId)
	{
		StringId castString;
		return Enum.TryParse<StringId>(stringId, out castString) && this.SetStringId(scope, castString);
	}

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x06000E79 RID: 3705 RVA: 0x0003125D File Offset: 0x0002F45D
	// (set) Token: 0x06000E7A RID: 3706 RVA: 0x00031265 File Offset: 0x0002F465
	public bool isInitialized { get; protected set; }

	// Token: 0x17000308 RID: 776
	// (get) Token: 0x06000E7B RID: 3707 RVA: 0x0003126E File Offset: 0x0002F46E
	public TMP_Text TextField
	{
		get
		{
			return this._textField;
		}
	}

	// Token: 0x17000309 RID: 777
	// (get) Token: 0x06000E7C RID: 3708 RVA: 0x00031276 File Offset: 0x0002F476
	// (set) Token: 0x06000E7D RID: 3709 RVA: 0x00031280 File Offset: 0x0002F480
	public StandaloneLocString LocString
	{
		get
		{
			return this._locString;
		}
		set
		{
			this._locString = value;
			if (!this.ignoreLocalization)
			{
				if (this._locString == null)
				{
					this._textField.text = "";
					return;
				}
				if (Diagnostics.Verify(this._textField != null, base.gameObject, "{0} doesn't have a textfield!", base.name))
				{
					this._textField.text = this._locString.ToString();
					if (this.isInitialized)
					{
						this.UpdateFont();
					}
				}
			}
		}
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x00031300 File Offset: 0x0002F500
	public void UpdateFont()
	{
		Locale newLocale = this._locString.Locale;
		if (newLocale != null)
		{
			FontDefinition newFont = this._fontDatabase.GetFont(newLocale.Charset);
			if (newFont != null && newFont.FontAsset != this._textField.font)
			{
				this._textField.font = newFont.FontAsset;
				if (this._baseCustomMaterial != null)
				{
					this._textField.fontSharedMaterial = newFont.GetCustomMaterial(this._textField.fontStyle, this._baseCustomMaterial);
				}
			}
		}
		this._textField.isRightToLeftText = this._locString.IsRightToLeft();
		if (!this._isAlwaysRightAligned && ((this._textField.isRightToLeftText && this.IsLeftAligned) || (!this._textField.isRightToLeftText && this.IsRightAligned)))
		{
			this.SwapAlignment();
		}
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x000313D8 File Offset: 0x0002F5D8
	private void SwapAlignment()
	{
		TextAlignmentOptions alignment = this._textField.alignment;
		if (alignment <= TextAlignmentOptions.Left)
		{
			if (alignment == TextAlignmentOptions.TopLeft)
			{
				this._textField.alignment = TextAlignmentOptions.TopRight;
				return;
			}
			if (alignment == TextAlignmentOptions.TopRight)
			{
				this._textField.alignment = TextAlignmentOptions.TopLeft;
				return;
			}
			if (alignment != TextAlignmentOptions.Left)
			{
				return;
			}
			this._textField.alignment = TextAlignmentOptions.Right;
			return;
		}
		else
		{
			if (alignment == TextAlignmentOptions.Right)
			{
				this._textField.alignment = TextAlignmentOptions.Left;
				return;
			}
			if (alignment == TextAlignmentOptions.BottomLeft)
			{
				this._textField.alignment = TextAlignmentOptions.BottomRight;
				return;
			}
			if (alignment != TextAlignmentOptions.BottomRight)
			{
				return;
			}
			this._textField.alignment = TextAlignmentOptions.BottomLeft;
			return;
		}
	}

	// Token: 0x1700030A RID: 778
	// (get) Token: 0x06000E80 RID: 3712 RVA: 0x00031490 File Offset: 0x0002F690
	private bool IsLeftAligned
	{
		get
		{
			return this._textField.alignment == TextAlignmentOptions.Left || this._textField.alignment == TextAlignmentOptions.TopLeft || this._textField.alignment == TextAlignmentOptions.BottomLeft;
		}
	}

	// Token: 0x1700030B RID: 779
	// (get) Token: 0x06000E81 RID: 3713 RVA: 0x000314CA File Offset: 0x0002F6CA
	private bool IsRightAligned
	{
		get
		{
			return this._textField.alignment == TextAlignmentOptions.Right || this._textField.alignment == TextAlignmentOptions.TopRight || this._textField.alignment == TextAlignmentOptions.BottomRight;
		}
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x00031504 File Offset: 0x0002F704
	public virtual void Awake()
	{
		this._textField = (this._textField ?? base.GetComponent<TMP_Text>());
		if (this._textField.fontSharedMaterial != this._textField.font.material)
		{
			this._baseCustomMaterial = this._textField.fontSharedMaterial;
		}
		this.isInitialized = false;
		this._isAlwaysRightAligned = this.IsRightAligned;
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x00031570 File Offset: 0x0002F770
	public virtual void HandleParentAllocated(IScope parentScope)
	{
		this._fontDatabase = parentScope.Get<FontDatabase>();
		this.isInitialized = true;
		StringId enumKey = StringId.None;
		if (Enum.TryParse<StringId>(this.startingStringIdString, out enumKey) && enumKey != StringId.None)
		{
			StandaloneLocString newString = StandaloneLocString.CreateString(parentScope, enumKey);
			this.LocString = newString;
		}
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x000315B3 File Offset: 0x0002F7B3
	public void Unregister()
	{
		this.isInitialized = false;
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x000315BC File Offset: 0x0002F7BC
	public void HandleLocaleChanged(Locale newLocale)
	{
		if (this._locString != null)
		{
			this._locString.ChangeLocale(newLocale);
			this.LocString = this._locString;
		}
	}

	// Token: 0x0400088B RID: 2187
	[HideInInspector]
	[EnumSearch(typeof(StringId), true)]
	public string startingStringIdString;

	// Token: 0x0400088C RID: 2188
	private Material _baseCustomMaterial;

	// Token: 0x0400088E RID: 2190
	public bool ignoreLocalization;

	// Token: 0x0400088F RID: 2191
	protected StandaloneLocString _locString;

	// Token: 0x04000890 RID: 2192
	[SerializeField]
	protected TMP_Text _textField;

	// Token: 0x04000891 RID: 2193
	private bool _isAlwaysRightAligned;

	// Token: 0x04000892 RID: 2194
	private FontDatabase _fontDatabase;
}
