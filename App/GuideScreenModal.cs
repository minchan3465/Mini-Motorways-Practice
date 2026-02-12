using System;
using Client;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001CD RID: 461
public class GuideScreenModal : MonoBehaviour, IThemeComponent
{
	// Token: 0x06000ADE RID: 2782 RVA: 0x00024243 File Offset: 0x00022443
	private void OnEnable()
	{
		this.Reset();
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x0002424B File Offset: 0x0002244B
	private void Reset()
	{
		this.SetOption(0);
		this._screens.SetOption(0, false);
		this._canvas.alpha = 1f;
		this._canvas.blocksRaycasts = true;
		this._canvas.interactable = true;
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x00024289 File Offset: 0x00022489
	protected void Awake()
	{
		this._canvas = base.GetComponent<CanvasGroup>();
		this._rect = base.GetComponent<RectTransform>();
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x00024243 File Offset: 0x00022443
	protected void Start()
	{
		this.Reset();
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x000242A3 File Offset: 0x000224A3
	private void Hide()
	{
		this._canvas.alpha = 0f;
		this._canvas.blocksRaycasts = false;
		this._canvas.interactable = false;
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x000242CD File Offset: 0x000224CD
	public void SetOption(int index)
	{
		this._selectedButtonDotIndex = index;
		this.UpdateColors();
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x000242DC File Offset: 0x000224DC
	private void UpdateColors()
	{
		Diagnostics.Verify(this._selectedButtonDotIndex < this.visualiserDots.Length, "You don't have enough visualiser dots set up! Required {0} but have {1}. Add more dot prefabs to {3}", this._selectedButtonDotIndex, this.visualiserDots.Length, base.name);
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x000022F5 File Offset: 0x000004F5
	public void InitializeTheme(IThemeDatabase themeDatabase)
	{
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x000022F5 File Offset: 0x000004F5
	public void ApplyTheme(ITheme newTheme)
	{
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x000020AA File Offset: 0x000002AA
	public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
	{
		return ThemeBlendingResult.StopBlending;
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x000022F5 File Offset: 0x000004F5
	public void ReleaseTheme(IThemeDatabase themeDatabase)
	{
	}

	// Token: 0x040005F3 RID: 1523
	private CanvasGroup _canvas;

	// Token: 0x040005F4 RID: 1524
	private RectTransform _rect;

	// Token: 0x040005F5 RID: 1525
	[SerializeField]
	private TouchOptionButton _screens;

	// Token: 0x040005F6 RID: 1526
	private int _selectedButtonDotIndex;

	// Token: 0x040005F7 RID: 1527
	public Image[] visualiserDots;
}
