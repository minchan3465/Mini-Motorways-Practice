using System;
using Client;
using Factory;
using Factory.Pools;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000611 RID: 1553
	public class UnbuiltMotorwayView : MonoBehaviour, IView, IThemeComponent, IReusable, TileView.IObserver, IReleasedFromScopeHandler
	{
		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x06002B6A RID: 11114 RVA: 0x000BF9EA File Offset: 0x000BDBEA
		// (set) Token: 0x06002B6B RID: 11115 RVA: 0x000BF9F2 File Offset: 0x000BDBF2
		[Dependency]
		public City City { get; private set; }

		// Token: 0x06002B6C RID: 11116 RVA: 0x000BF9FC File Offset: 0x000BDBFC
		public void Initialize(TileView tileView, Vector2 position, Vector2 interactionCircleOffset, int number)
		{
			this._tileView = tileView;
			this._tileView.Subscribe(this);
			base.transform.localPosition = position;
			this._handleView = base.GetComponentInChildren<UnbuiltMotorwayHandleView>();
			if (Diagnostics.Verify(this._handleView != null, "No UnbuiltMotorwayHandleView found on object."))
			{
				this._handleView.Initialize(this._scope, number);
			}
			if (Diagnostics.Verify(this.interactionCircle != null, "InteractionCircle not found on UnbuiltMotorwayView."))
			{
				this.interactionCircle.transform.localPosition = interactionCircleOffset;
			}
		}

		// Token: 0x06002B6D RID: 11117 RVA: 0x000BFA92 File Offset: 0x000BDC92
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._tileView != null)
			{
				this._tileView.Unsubscribe(this);
			}
		}

		// Token: 0x06002B6E RID: 11118 RVA: 0x000BFAAF File Offset: 0x000BDCAF
		public void Reset()
		{
			this.City = null;
			this._handleView = null;
			base.transform.localPosition = Vector3.zero;
			this._interactionCirclePositionTween.Reset();
		}

		// Token: 0x06002B6F RID: 11119 RVA: 0x000BFADC File Offset: 0x000BDCDC
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._handleView != null)
			{
				this._handleView.Tick(timeInterval, stepAlpha);
			}
			if (this._interactionCirclePositionTween.IsActive)
			{
				this.interactionCircle.transform.localPosition = this._interactionCirclePositionTween.Tick(timeInterval.Delta);
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002B70 RID: 11120 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002B71 RID: 11121 RVA: 0x000BFB34 File Offset: 0x000BDD34
		public void OnTileViewChanged(TileView changedTileView)
		{
			this._interactionCirclePositionTween.Start(this.interactionCircle.transform.localPosition, changedTileView.InteractionCircleOffset, this._visualConstants.InteractionCircleOffsetAdjustmentDuration, this._visualConstants.InteractionCircleAndTrafficLightAdjustmentEasingFunction, 0f);
		}

		// Token: 0x06002B72 RID: 11122 RVA: 0x000BFB82 File Offset: 0x000BDD82
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
			if (this._handleView != null)
			{
				this._handleView.InitializeTheme(themeDatabase);
			}
		}

		// Token: 0x06002B73 RID: 11123 RVA: 0x000BFB9E File Offset: 0x000BDD9E
		public void ApplyTheme(ITheme newTheme)
		{
			if (this._handleView != null)
			{
				this._handleView.ApplyTheme(newTheme);
			}
		}

		// Token: 0x06002B74 RID: 11124 RVA: 0x000BFBBA File Offset: 0x000BDDBA
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			if (this._handleView != null)
			{
				this._handleView.ApplyTheme(newTheme);
			}
			return ThemeBlendingResult.StopBlending;
		}

		// Token: 0x06002B75 RID: 11125 RVA: 0x000BFBD7 File Offset: 0x000BDDD7
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
			if (this._handleView != null)
			{
				this._handleView.ReleaseTheme(themeDatabase);
			}
		}

		// Token: 0x0400259A RID: 9626
		public GameObject interactionCircle;

		// Token: 0x0400259C RID: 9628
		[Dependency]
		private IScope _scope;

		// Token: 0x0400259D RID: 9629
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x0400259E RID: 9630
		private UnbuiltMotorwayHandleView _handleView;

		// Token: 0x0400259F RID: 9631
		private readonly TweenVector3 _interactionCirclePositionTween = new TweenVector3();

		// Token: 0x040025A0 RID: 9632
		private TileView _tileView;
	}
}
