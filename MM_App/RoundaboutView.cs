using System;
using Client;
using Factory;
using Factory.Pools;
using Motorways;
using Motorways.Views;
using UnityEngine;

// Token: 0x020001DF RID: 479
public class RoundaboutView : MonoBehaviour, IView, IThemeComponent, IReusable
{
	// Token: 0x06000B71 RID: 2929 RVA: 0x00027130 File Offset: 0x00025330
	public TickResult Tick(TimeInterval tickTime, float stepAlpha)
	{
		if (!this._city.Rules.RoadsBecomePermanentOverTime)
		{
			return TickResult.StopTicking;
		}
		this._interactionCircleView.SetPermanenceProgress(this._visualConstants.DryingInteractionCircleFalloff.Evaluate((float)this._tileView.Tile.RoundaboutPermanenceProgress));
		if (this._tileView.Tile.HasRoundabout(RoadState.Active) && this._tileView.Tile.IsRoundaboutPermanent)
		{
			return TickResult.StopTicking;
		}
		return TickResult.ContinueTicking;
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x000271AA File Offset: 0x000253AA
	public void SetGameobjectActive(bool isActive)
	{
		base.gameObject.SetActive(isActive);
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x000271B8 File Offset: 0x000253B8
	public void Initialize(TileView tileView)
	{
		this._tileView = tileView;
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x000271C1 File Offset: 0x000253C1
	public void InitializeTheme(IThemeDatabase themeDatabase)
	{
		this._interactionCircleView.InitializeTheme(themeDatabase);
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x000271CF File Offset: 0x000253CF
	public void ApplyTheme(ITheme theme)
	{
		this._interactionCircleView.ApplyTheme(theme);
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x000271DD File Offset: 0x000253DD
	public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
	{
		return this._interactionCircleView.ApplyBlendedTheme(oldTheme, newTheme, progress);
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x000271ED File Offset: 0x000253ED
	public void ReleaseTheme(IThemeDatabase themeDatabase)
	{
		this._interactionCircleView.ReleaseTheme(themeDatabase);
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x000271FB File Offset: 0x000253FB
	public void Reset()
	{
		this._tileView = null;
		base.transform.localPosition = Vector3.zero;
		this.ReconfigurePermanenceVisibility();
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x0002721A File Offset: 0x0002541A
	public void ReconfigurePermanenceVisibility()
	{
		this._interactionCircleView.SetPermanenceProgress(0f);
	}

	// Token: 0x04000692 RID: 1682
	[Dependency]
	private City _city;

	// Token: 0x04000693 RID: 1683
	[Dependency]
	private VisualConstantsData _visualConstants;

	// Token: 0x04000694 RID: 1684
	private TileView _tileView;

	// Token: 0x04000695 RID: 1685
	[SerializeField]
	private InteractionCircleView _interactionCircleView;
}
