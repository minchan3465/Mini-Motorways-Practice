using System;
using Client;
using Motorways;
using Motorways.Audio;
using Motorways.Themes;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C8 RID: 456
public class DrawModeToggle : MonoBehaviour, IThemeComponent
{
	// Token: 0x1700026F RID: 623
	// (get) Token: 0x06000AB7 RID: 2743 RVA: 0x00023866 File Offset: 0x00021A66
	// (set) Token: 0x06000AB8 RID: 2744 RVA: 0x0002386E File Offset: 0x00021A6E
	public RoadDrawMode DrawMode { get; private set; }

	// Token: 0x06000AB9 RID: 2745 RVA: 0x00023877 File Offset: 0x00021A77
	private void Awake()
	{
		this._circleY = this.selectionCircle.anchoredPosition.y;
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x00023890 File Offset: 0x00021A90
	private void Update()
	{
		if (this.DrawMode == RoadDrawMode.Add && this.selectionCircle.anchoredPosition.y < this._circleY)
		{
			Vector2 pos = this.selectionCircle.anchoredPosition;
			pos.y += this._circleY * this.toggleAnimationSpeed * Time.deltaTime;
			pos.y = Mathf.Min(this._circleY, pos.y);
			this.selectionCircle.anchoredPosition = pos;
			return;
		}
		if (this.DrawMode == RoadDrawMode.Remove && this.selectionCircle.anchoredPosition.y > -this._circleY)
		{
			Vector2 pos2 = this.selectionCircle.anchoredPosition;
			pos2.y -= this._circleY * this.toggleAnimationSpeed * Time.deltaTime;
			pos2.y = Mathf.Max(-this._circleY, pos2.y);
			this.selectionCircle.anchoredPosition = pos2;
		}
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x0002397D File Offset: 0x00021B7D
	public void Pulse()
	{
		this._animator.SetTrigger(DrawModeToggle.PulseTrigger);
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x00023990 File Offset: 0x00021B90
	public void SetDrawMode(RoadDrawMode mode)
	{
		this.DrawMode = mode;
		this.UpdateColors(false);
		AudioSystem.Instance.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.DrawMode, 0.75f, this._crossFadeDuration, mode == RoadDrawMode.Add, null));
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x000239DC File Offset: 0x00021BDC
	public void OnToggleAudio()
	{
		AudioSystem.Instance.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.Click, UIAudioProfile.DrawModeToggle, -1f, this.DrawMode == RoadDrawMode.Add, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x00023A08 File Offset: 0x00021C08
	public void UpdateColors(bool instantly = false)
	{
		this.drawIcon.CrossFadeColor((this.DrawMode == RoadDrawMode.Add) ? this._resolvedDarkColor : this._resolvedLightColor, instantly ? 0f : this._crossFadeDuration, true, false);
		this.deleteIcon.CrossFadeColor((this.DrawMode == RoadDrawMode.Remove) ? this._resolvedDarkColor : this._resolvedLightColor, instantly ? 0f : this._crossFadeDuration, true, false);
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x000022F5 File Offset: 0x000004F5
	public void InitializeTheme(IThemeDatabase themeDatabase)
	{
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x00023A7C File Offset: 0x00021C7C
	public void ApplyTheme(ITheme targetTheme)
	{
		Theme theme = (Theme)targetTheme;
		this._resolvedDarkColor = theme.GetColor(this._darkColor, "_Color");
		this._resolvedLightColor = theme.GetColor(this._lightColor, "_Color");
		this.UpdateColors(true);
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x00023AC8 File Offset: 0x00021CC8
	public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
	{
		Theme theme = (Theme)newTheme;
		this._resolvedDarkColor = theme.GetColor(this._darkColor, "_Color");
		this._resolvedLightColor = theme.GetColor(this._lightColor, "_Color");
		this.UpdateColors(false);
		return ThemeBlendingResult.StopBlending;
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x000022F5 File Offset: 0x000004F5
	public void ReleaseTheme(IThemeDatabase themeDatabase)
	{
	}

	// Token: 0x040005CB RID: 1483
	public Image drawIcon;

	// Token: 0x040005CC RID: 1484
	public Image deleteIcon;

	// Token: 0x040005CD RID: 1485
	public RectTransform selectionCircle;

	// Token: 0x040005CE RID: 1486
	public TouchButton touchButton;

	// Token: 0x040005D0 RID: 1488
	public float toggleAnimationSpeed = 15f;

	// Token: 0x040005D1 RID: 1489
	private float _circleY;

	// Token: 0x040005D2 RID: 1490
	[SerializeField]
	private ThemedMaterialType _darkColor = ThemedMaterialType.Dark;

	// Token: 0x040005D3 RID: 1491
	[SerializeField]
	private ThemedMaterialType _lightColor;

	// Token: 0x040005D4 RID: 1492
	private Color _resolvedDarkColor = Color.black;

	// Token: 0x040005D5 RID: 1493
	private Color _resolvedLightColor = Color.white;

	// Token: 0x040005D6 RID: 1494
	[SerializeField]
	private Animator _animator;

	// Token: 0x040005D7 RID: 1495
	private static readonly int PulseTrigger = Animator.StringToHash("Pulse");

	// Token: 0x040005D8 RID: 1496
	private float _crossFadeDuration = 0.1f;

	// Token: 0x020001C9 RID: 457
	public enum VisibleState
	{
		// Token: 0x040005DA RID: 1498
		AlwaysShowing,
		// Token: 0x040005DB RID: 1499
		ShowWhenFocused,
		// Token: 0x040005DC RID: 1500
		NeverShow
	}
}
