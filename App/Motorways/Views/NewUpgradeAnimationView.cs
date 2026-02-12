using System;
using Client;
using Easing;
using Factory;
using Factory.Pools;
using Motorways.Themes;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200060F RID: 1551
	[RequireComponent(typeof(RectTransform))]
	public class NewUpgradeAnimationView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x06002B5F RID: 11103 RVA: 0x000BF72C File Offset: 0x000BD92C
		public UpgradeIcon UpgradeIcon
		{
			get
			{
				return this._upgradeIcon;
			}
		}

		// Token: 0x06002B60 RID: 11104 RVA: 0x000BF734 File Offset: 0x000BD934
		public void OnEnable()
		{
			this._rect = base.GetComponent<RectTransform>();
			this._upgradeIcon = base.GetComponent<UpgradeIcon>();
			this._canvasGroup = base.GetComponent<CanvasGroup>();
		}

		// Token: 0x06002B61 RID: 11105 RVA: 0x000BF75C File Offset: 0x000BD95C
		public void Reset()
		{
			base.transform.localPosition = Vector3.zero;
			base.transform.localScale = Vector3.one;
			this._startScale = default(Vector2);
			this._startPosition = default(Vector3);
			this._lerp = 0f;
			this._count = 1;
			this._upgradeType = UpgradeType.Concrete;
			this._isStartingPositionSet = false;
			this._hiding = false;
		}

		// Token: 0x06002B62 RID: 11106 RVA: 0x000BF7C8 File Offset: 0x000BD9C8
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (!this._isStartingPositionSet)
			{
				this._startPosition = this._rect.position;
				this._isStartingPositionSet = true;
			}
			if (this._lerp > 1f)
			{
				this._lerp = 0f;
				this._upgradeBar.AddToUpgradeButtonStack(this._upgradeType, true, this._count);
				return TickResult.Destroy;
			}
			if (this._lerp >= 0f)
			{
				this._rect.position = Vector3.Lerp(this._startPosition, this._endTransform.position, Easings.Interpolate(this._lerp, this._positionEasing));
				this._rect.sizeDelta = Vector2.Lerp(this._startScale, this._endTransform.sizeDelta, Easings.Interpolate(this._lerp, this._scaleEasing));
			}
			else if (this._lerp < 0f)
			{
				this._rect.position = this._startPosition;
				this._rect.sizeDelta = this._startScale;
			}
			this._lerp += timeInterval.Delta * (1f / this.animationDuration);
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002B63 RID: 11107 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002B64 RID: 11108 RVA: 0x000BF8EA File Offset: 0x000BDAEA
		private void Update()
		{
			if (this._hiding && this._canvasGroup.alpha > 0f)
			{
				this._canvasGroup.alpha -= Time.deltaTime * 2f;
			}
		}

		// Token: 0x06002B65 RID: 11109 RVA: 0x000BF923 File Offset: 0x000BDB23
		public void Hide()
		{
			this._hiding = true;
		}

		// Token: 0x06002B66 RID: 11110 RVA: 0x000BF92C File Offset: 0x000BDB2C
		public void Initialize(Vector2 startScale, RectTransform destination, Sprite sprite, UpgradeType upgradeType, Theme theme, float delay = 0f, int count = 1)
		{
			this._canvasGroup.alpha = 1f;
			this._upgradeType = upgradeType;
			this._startScale = startScale;
			this._rect.sizeDelta = this._startScale;
			this._upgradeIcon.iconRenderer.sprite = sprite;
			this._endTransform = destination;
			this._lerp = -delay;
			this._upgradeIcon.ApplyTheme(theme);
			base.transform.SetAsLastSibling();
			this._count = count;
			this._hiding = false;
		}

		// Token: 0x04002589 RID: 9609
		private RectTransform _rect;

		// Token: 0x0400258A RID: 9610
		private CanvasGroup _canvasGroup;

		// Token: 0x0400258B RID: 9611
		private UpgradeIcon _upgradeIcon;

		// Token: 0x0400258C RID: 9612
		private Vector2 _startScale;

		// Token: 0x0400258D RID: 9613
		private Vector3 _startPosition;

		// Token: 0x0400258E RID: 9614
		private RectTransform _endTransform;

		// Token: 0x0400258F RID: 9615
		private float _lerp;

		// Token: 0x04002590 RID: 9616
		private UpgradeType _upgradeType;

		// Token: 0x04002591 RID: 9617
		[Dependency]
		private UpgradeBarClient _upgradeBar;

		// Token: 0x04002592 RID: 9618
		private int _count = 1;

		// Token: 0x04002593 RID: 9619
		private bool _isStartingPositionSet;

		// Token: 0x04002594 RID: 9620
		[MinValue(1E-45f)]
		public float animationDuration = 0.6f;

		// Token: 0x04002595 RID: 9621
		[MinValue(1E-45f)]
		[Tooltip("The time between multiple instances of the animation when the player earns more than one upgrade at once.")]
		public float animationSpacing = 0.2f;

		// Token: 0x04002596 RID: 9622
		[SerializeField]
		private Easings.Functions _positionEasing;

		// Token: 0x04002597 RID: 9623
		[SerializeField]
		private Easings.Functions _scaleEasing = Easings.Functions.SineEaseIn;

		// Token: 0x04002598 RID: 9624
		private bool _hiding;

		// Token: 0x04002599 RID: 9625
		private const int HideSpeed = 2;
	}
}
