using System;
using Client;
using Easing;
using Motorways;
using Motorways.Themes;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200026E RID: 622
[RequireComponent(typeof(RectTransform), typeof(CanvasGroup), typeof(Animator))]
public class UpgradeIcon : MonoBehaviour, IThemeComponent
{
	// Token: 0x1700030F RID: 783
	// (get) Token: 0x06000ECA RID: 3786 RVA: 0x00031E58 File Offset: 0x00030058
	// (set) Token: 0x06000ECB RID: 3787 RVA: 0x00031E60 File Offset: 0x00030060
	public bool IsHighlighted
	{
		get
		{
			return this._isHighlighted;
		}
		set
		{
			this._isHighlighted = value;
			this.UpdateColors();
		}
	}

	// Token: 0x17000310 RID: 784
	// (get) Token: 0x06000ECC RID: 3788 RVA: 0x00031E6F File Offset: 0x0003006F
	// (set) Token: 0x06000ECD RID: 3789 RVA: 0x00031E77 File Offset: 0x00030077
	public RectTransform Rect { get; private set; }

	// Token: 0x06000ECE RID: 3790 RVA: 0x00031E80 File Offset: 0x00030080
	public void SetToCircle()
	{
		if (this.fillRenderer != null)
		{
			this.fillRenderer.material = this.circleMaterial;
		}
		if (this.outlineRenderer != null)
		{
			this.outlineRenderer.material = this.circleOutlineMaterial;
		}
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x00031EC0 File Offset: 0x000300C0
	public void SetToDiamond()
	{
		if (this.fillRenderer != null)
		{
			this.fillRenderer.material = this.diamondMaterial;
		}
		if (this.outlineRenderer != null)
		{
			this.outlineRenderer.material = this.diamondOutlineMaterial;
		}
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x00031F00 File Offset: 0x00030100
	public void Bounce()
	{
		this._scaleTween.Start(0.7f, 1f, 0.5f, Easings.Functions.BounceEaseOut, 0f);
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x00031F24 File Offset: 0x00030124
	public void SetVisible(bool nowVisible, TransitionStyle animate = TransitionStyle.Snap)
	{
		if (this.fillRenderer != null)
		{
			this.fillRenderer.enabled = (nowVisible || animate == TransitionStyle.Tween);
		}
		if (this.outlineRenderer != null)
		{
			this.outlineRenderer.enabled = (nowVisible || animate == TransitionStyle.Tween);
		}
		if (this.iconRenderer != null)
		{
			this.iconRenderer.enabled = (nowVisible || animate == TransitionStyle.Tween);
		}
		if (nowVisible)
		{
			this.UpdateColors();
		}
		if (animate == TransitionStyle.Tween)
		{
			if (nowVisible && !this._isVisible)
			{
				this._scaleTween.Start(0.7f, 1f, 0.5f, Easings.Functions.BounceEaseOut, 0f);
			}
			else if (!nowVisible && this._isVisible)
			{
				this._scaleTween.Start(1f, 0f, 0.1f, Easings.Functions.Linear, 0f);
			}
		}
		this._isVisible = nowVisible;
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x00032006 File Offset: 0x00030206
	public void SetCutoutRect(RectTransform cutoutRect)
	{
		this._cutoutRect = cutoutRect;
		this.fillRenderer.material = new Material(this.fillRenderer.material);
		this.UpdateCutoutRect();
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x00032030 File Offset: 0x00030230
	private void UpdateCutoutRect()
	{
		if (this._cutoutRect)
		{
			Material cutoutMaterial = this._cutoutRect.GetComponent<UpgradeIcon>().fillRenderer.material;
			if (cutoutMaterial.HasProperty(UpgradeIcon.CircleRadiusPropertyId))
			{
				float cutoutRadius = cutoutMaterial.GetFloat(UpgradeIcon.CircleRadiusPropertyId);
				Vector3 relativePosition = this._rectTransform.InverseTransformPoint(this._cutoutRect.position) / (this._rectTransform.rect.size / 2f);
				relativePosition.x *= -1f;
				float relativeRadius = this._cutoutRect.rect.size.x * this._cutoutRect.lossyScale.x * cutoutRadius / (this._rectTransform.rect.size.x * this._rectTransform.lossyScale.x);
				relativeRadius += this._cutoutRadiusPadding;
				this.fillRenderer.material.SetVector(UpgradeIcon.CutoutPositionPropertyId, relativePosition);
				this.fillRenderer.material.SetFloat(UpgradeIcon.CutoutRadiusPropertyId, relativeRadius);
				return;
			}
			this.fillRenderer.material.SetFloat(UpgradeIcon.CutoutRadiusPropertyId, 0f);
		}
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x00032180 File Offset: 0x00030380
	public void Awake()
	{
		this.Rect = base.GetComponent<RectTransform>();
		this._canvasGroup = base.GetComponent<CanvasGroup>();
		this._animator = base.GetComponent<Animator>();
		if (!Diagnostics.Verify(Enum.TryParse<ThemedMaterialType>(this._highlightThemeColor, out this._highlightThemeColorEnum)))
		{
			this._highlightThemeColorEnum = ThemedMaterialType.HighlightedButton;
		}
		if (!Diagnostics.Verify(Enum.TryParse<ThemedMaterialType>(this._baseThemeColor, out this._baseThemeColorEnum)))
		{
			this._baseThemeColorEnum = ThemedMaterialType.Dark;
		}
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x000321F0 File Offset: 0x000303F0
	private void OnEnable()
	{
		this._canvasGroup = base.GetComponent<CanvasGroup>();
		this.SetVisible(this._isVisible, TransitionStyle.Snap);
	}

	// Token: 0x17000311 RID: 785
	// (get) Token: 0x06000ED6 RID: 3798 RVA: 0x0003220B File Offset: 0x0003040B
	// (set) Token: 0x06000ED7 RID: 3799 RVA: 0x00032218 File Offset: 0x00030418
	private float Alpha
	{
		get
		{
			return this._canvasGroup.alpha;
		}
		set
		{
			if (this._canvasGroup == null)
			{
				this._canvasGroup = base.GetComponent<CanvasGroup>();
			}
			this._canvasGroup.alpha = Math.Min(value, this._canvasGroup.alpha);
		}
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x00032250 File Offset: 0x00030450
	public void SetOutlineIndex(int index)
	{
		this._outlineIndex = index;
		this.UpdateColors();
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x00032260 File Offset: 0x00030460
	private void UpdateColors()
	{
		if (!this._isVisible || this.IsDisabled)
		{
			return;
		}
		for (int highlightIndex = 0; highlightIndex < this._highlightTargets.Length; highlightIndex++)
		{
			Image highlightTarget = this._highlightTargets[highlightIndex];
			if (highlightTarget != null)
			{
				highlightTarget.color = (this.IsHighlighted ? this._highlightColor : this._darkColor);
			}
		}
		if (this._isStackIcon)
		{
			this.Alpha = 1f - (float)this._outlineIndex / (float)UpgradeButtonStack.MaxVisibleIcons;
			if (this.iconRenderer != null)
			{
				this.iconRenderer.enabled = (this._outlineIndex == 0);
			}
		}
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x00032303 File Offset: 0x00030503
	public void Pulse()
	{
		this._animator.SetTrigger(UpgradeIcon.PulseTrigger);
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x000022F5 File Offset: 0x000004F5
	public void InitializeTheme(IThemeDatabase themeDatabase)
	{
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x00032318 File Offset: 0x00030518
	public void ApplyTheme(ITheme theme)
	{
		Theme motorwaysTheme = (Theme)theme;
		if (motorwaysTheme == null)
		{
			return;
		}
		this._darkColor = motorwaysTheme.GetColor(this._baseThemeColorEnum, "_Color");
		this._highlightColor = motorwaysTheme.GetColor(this._highlightThemeColorEnum, "_Color");
		this.UpdateColors();
		if (this._themedComponents == null)
		{
			this._themedComponents = base.GetComponentsInChildren<ThemedComponent>();
		}
		ThemedComponent[] themedComponents = this._themedComponents;
		for (int i = 0; i < themedComponents.Length; i++)
		{
			themedComponents[i].ApplyTheme(theme);
		}
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x0003239C File Offset: 0x0003059C
	public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
	{
		Theme motorwaysTheme = (Theme)newTheme;
		this._darkColor = motorwaysTheme.GetColor(this._baseThemeColorEnum, "_Color");
		this._highlightColor = motorwaysTheme.GetColor(this._highlightThemeColorEnum, "_Color");
		this.UpdateColors();
		ThemeBlendingResult blendingResult = ThemeBlendingResult.StopBlending;
		if (this._themedComponents == null)
		{
			this._themedComponents = base.GetComponentsInChildren<ThemedComponent>();
		}
		ThemedComponent[] themedComponents = this._themedComponents;
		for (int i = 0; i < themedComponents.Length; i++)
		{
			if (themedComponents[i].ApplyBlendedTheme(oldTheme, newTheme, progress) == ThemeBlendingResult.ContinueBlending)
			{
				blendingResult = ThemeBlendingResult.ContinueBlending;
			}
		}
		return blendingResult;
	}

	// Token: 0x06000EDE RID: 3806 RVA: 0x000022F5 File Offset: 0x000004F5
	public void ReleaseTheme(IThemeDatabase themeDatabase)
	{
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x00032420 File Offset: 0x00030620
	private void Update()
	{
		if (this._scaleTween.IsActive)
		{
			this._scaleTween.Tick(Time.deltaTime);
			base.transform.localScale = Vector3.one * this._scaleTween.Value;
		}
		this.UpdateCutoutRect();
	}

	// Token: 0x04000D88 RID: 3464
	[SerializeField]
	private RectTransform _rectTransform;

	// Token: 0x04000D89 RID: 3465
	public Image iconRenderer;

	// Token: 0x04000D8A RID: 3466
	public Image fillRenderer;

	// Token: 0x04000D8B RID: 3467
	public Image outlineRenderer;

	// Token: 0x04000D8C RID: 3468
	[SerializeField]
	private Image[] _highlightTargets;

	// Token: 0x04000D8D RID: 3469
	[SerializeField]
	private Material circleMaterial;

	// Token: 0x04000D8E RID: 3470
	[SerializeField]
	private Material circleOutlineMaterial;

	// Token: 0x04000D8F RID: 3471
	[SerializeField]
	private Material diamondMaterial;

	// Token: 0x04000D90 RID: 3472
	[SerializeField]
	private Material diamondOutlineMaterial;

	// Token: 0x04000D91 RID: 3473
	[Range(0f, 1f)]
	[SerializeField]
	private float _cutoutRadiusPadding = 0.15f;

	// Token: 0x04000D92 RID: 3474
	[SerializeField]
	private bool _isStackIcon = true;

	// Token: 0x04000D93 RID: 3475
	private ThemedComponent[] _themedComponents;

	// Token: 0x04000D94 RID: 3476
	private Animator _animator;

	// Token: 0x04000D95 RID: 3477
	private TweenFloat _scaleTween = new TweenFloat();

	// Token: 0x04000D96 RID: 3478
	private const float BounceTweenDuration = 0.5f;

	// Token: 0x04000D97 RID: 3479
	private const float ShrinkTweenDuration = 0.1f;

	// Token: 0x04000D98 RID: 3480
	private const float BounceTweenScaleStart = 0.7f;

	// Token: 0x04000D99 RID: 3481
	private const float BounceTweenScaleEnd = 1f;

	// Token: 0x04000D9A RID: 3482
	private static readonly int PulseTrigger = Animator.StringToHash("Pulse");

	// Token: 0x04000D9B RID: 3483
	[SerializeField]
	[StringEnumSearch(typeof(ThemedMaterialType))]
	private string _baseThemeColor = ThemedMaterialType.Dark.ToString();

	// Token: 0x04000D9C RID: 3484
	private ThemedMaterialType _baseThemeColorEnum = ThemedMaterialType.Dark;

	// Token: 0x04000D9D RID: 3485
	[StringEnumSearch(typeof(ThemedMaterialType))]
	[SerializeField]
	private string _highlightThemeColor = ThemedMaterialType.HighlightedButton.ToString();

	// Token: 0x04000D9E RID: 3486
	private ThemedMaterialType _highlightThemeColorEnum = ThemedMaterialType.HighlightedButton;

	// Token: 0x04000D9F RID: 3487
	private Color _darkColor = Color.black;

	// Token: 0x04000DA0 RID: 3488
	private Color _highlightColor = Color.yellow;

	// Token: 0x04000DA1 RID: 3489
	private bool _isVisible = true;

	// Token: 0x04000DA2 RID: 3490
	private bool _isHighlighted;

	// Token: 0x04000DA3 RID: 3491
	public bool IsDisabled;

	// Token: 0x04000DA4 RID: 3492
	private int _outlineIndex;

	// Token: 0x04000DA6 RID: 3494
	private RectTransform _cutoutRect;

	// Token: 0x04000DA7 RID: 3495
	private CanvasGroup _canvasGroup;

	// Token: 0x04000DA8 RID: 3496
	private static readonly int CircleRadiusPropertyId = Shader.PropertyToID("_CircleSize");

	// Token: 0x04000DA9 RID: 3497
	private static readonly int CutoutPositionPropertyId = Shader.PropertyToID("_CutoutPosition");

	// Token: 0x04000DAA RID: 3498
	private static readonly int CutoutRadiusPropertyId = Shader.PropertyToID("_CutoutRadius");
}
