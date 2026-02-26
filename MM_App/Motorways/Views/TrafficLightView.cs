using System;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Audio;
using Motorways.Models;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000609 RID: 1545
	public class TrafficLightView : MonoBehaviour, IView, TrafficLightModel.IObserver, TileView.IObserver, IThemeComponent, ICreatedInScopeHandler, IReleasedFromScopeHandler, IReusable
	{
		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x06002B25 RID: 11045 RVA: 0x000BE113 File Offset: 0x000BC313
		// (set) Token: 0x06002B26 RID: 11046 RVA: 0x000BE11B File Offset: 0x000BC31B
		[Dependency]
		public City City { get; private set; }

		// Token: 0x06002B27 RID: 11047 RVA: 0x000BE124 File Offset: 0x000BC324
		public void OnCreatedInScope(IScope scope)
		{
			Animator[] array = this.lightAnimators;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetBool(TrafficLightView.ActiveHash, false);
			}
		}

		// Token: 0x06002B28 RID: 11048 RVA: 0x000BE154 File Offset: 0x000BC354
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._trafficLightModel != null)
			{
				this._trafficLightModel.Unsubscribe(this);
				this._trafficLightModel = null;
			}
			if (this._tileView != null)
			{
				this._tileView.Unsubscribe(this);
				this._tileView = null;
			}
		}

		// Token: 0x06002B29 RID: 11049 RVA: 0x000BE194 File Offset: 0x000BC394
		public void Reset()
		{
			this._trafficLightModel = null;
			this._tileView = null;
			this.City = null;
			base.transform.localPosition = Vector3.zero;
			Animator[] array = this.lightAnimators;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetBool(TrafficLightView.ActiveHash, false);
			}
			this._interactionCirclePositionTween.Reset();
			TweenVector3[] trafficLightsOffsetsTweens = this._trafficLightsOffsetsTweens;
			for (int i = 0; i < trafficLightsOffsetsTweens.Length; i++)
			{
				trafficLightsOffsetsTweens[i].Reset();
			}
			this.ReconfigurePermanenceVisibility();
		}

		// Token: 0x06002B2A RID: 11050 RVA: 0x000BE217 File Offset: 0x000BC417
		public void ReconfigurePermanenceVisibility()
		{
			this._interactionCircleView.SetPermanenceProgress(0f);
		}

		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x06002B2B RID: 11051 RVA: 0x000BE229 File Offset: 0x000BC429
		public TrafficLightModel Model
		{
			get
			{
				return this._trafficLightModel;
			}
		}

		// Token: 0x06002B2C RID: 11052 RVA: 0x000BE234 File Offset: 0x000BC434
		public void SetModel(TrafficLightModel model)
		{
			this._trafficLightModel = model;
			this._trafficLightModel.Subscribe(this);
			this.UpdateLights();
			AudioSystem.Instance.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.UpgradePlaced, this._gameCamera.GetPanFromWorld(this._tileView.transform.position).x, -1f, true, null));
		}

		// Token: 0x06002B2D RID: 11053 RVA: 0x000BE2A2 File Offset: 0x000BC4A2
		public void InitialiseInteractionCirclePosition(TileView tileView)
		{
			this._tileView = tileView;
			tileView.Subscribe(this);
			this._interactionCircleView.transform.localPosition = this._tileView.InteractionCircleOffset;
			this._interactionCirclePositionTween.Stop();
		}

		// Token: 0x06002B2E RID: 11054 RVA: 0x000BE2E0 File Offset: 0x000BC4E0
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._interactionCirclePositionTween.IsActive)
			{
				this._interactionCircleView.transform.localPosition = this._interactionCirclePositionTween.Tick(timeInterval.Delta);
			}
			for (TileDirection direction = TileDirection.North; direction <= TileDirection.NorthWest; direction++)
			{
				if (this._trafficLightsOffsetsTweens[(int)direction].IsActive)
				{
					this.GetLightInDirection(direction).gameObject.transform.localPosition = this._trafficLightsOffsetsTweens[(int)direction].Tick(timeInterval.Delta);
				}
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002B2F RID: 11055 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002B30 RID: 11056 RVA: 0x000BE360 File Offset: 0x000BC560
		private void UpdateLights()
		{
			if (this._trafficLightModel != null)
			{
				TileDirectionBitfield currentActiveLightPair = this._trafficLightModel.ActivePair;
				for (TileDirection direction = TileDirection.North; direction <= TileDirection.NorthWest; direction++)
				{
					SpriteRenderer lightRenderer = this.GetLightInDirection(direction);
					Animator lightAnimator = this.GetLightAnimatorInDirection(direction);
					if (currentActiveLightPair[direction])
					{
						lightAnimator.SetBool(TrafficLightView.ActiveHash, true);
						if (this._trafficLightModel.amberLightsOn)
						{
							lightAnimator.SetTrigger(TrafficLightView.ChangeColorHash);
						}
						lightRenderer.color = (this._trafficLightModel.amberLightsOn ? this._amberLightColor : this._greenLightColor);
					}
					else
					{
						lightAnimator.SetBool(TrafficLightView.ActiveHash, false);
						lightRenderer.color = this._redLightColor;
					}
				}
			}
		}

		// Token: 0x06002B31 RID: 11057 RVA: 0x000BE40C File Offset: 0x000BC60C
		public SpriteRenderer GetLightInDirection(TileDirection direction)
		{
			return this.GetLightAnimatorInDirection(direction).GetComponent<SpriteRenderer>();
		}

		// Token: 0x06002B32 RID: 11058 RVA: 0x000BE41A File Offset: 0x000BC61A
		public Animator GetLightAnimatorInDirection(TileDirection direction)
		{
			return this.lightAnimators[(int)direction];
		}

		// Token: 0x06002B33 RID: 11059 RVA: 0x000BE424 File Offset: 0x000BC624
		public void OnTileViewChanged(TileView changedTile)
		{
			this._interactionCirclePositionTween.Start(this._interactionCircleView.transform.localPosition, changedTile.InteractionCircleOffset, this._visualConstants.InteractionCircleOffsetAdjustmentDuration, this._visualConstants.InteractionCircleAndTrafficLightAdjustmentEasingFunction, 0f);
			if (changedTile.TrafficLightOffsets.Length >= 8)
			{
				for (TileDirection tileDirection = TileDirection.North; tileDirection <= TileDirection.NorthWest; tileDirection++)
				{
					if (changedTile.TrafficLightOffsets[(int)tileDirection] != Vector2.zero)
					{
						SpriteRenderer lightRenderer = this.GetLightInDirection(tileDirection);
						this._trafficLightsOffsetsTweens[(int)tileDirection].Start(lightRenderer.gameObject.transform.localPosition, changedTile.TrafficLightOffsets[(int)tileDirection], this._visualConstants.TrafficLightsOffsetAdjustmentDuration, this._visualConstants.InteractionCircleAndTrafficLightAdjustmentEasingFunction, 0f);
					}
				}
			}
			this._interactionCircleView.SetPermanenceProgress(this.City.Rules.RoadsBecomePermanentOverTime ? this._visualConstants.DryingInteractionCircleFalloff.Evaluate((float)this._tileView.Tile.TrafficLightPermanenceProgress) : 0f);
		}

		// Token: 0x06002B34 RID: 11060 RVA: 0x000BE53C File Offset: 0x000BC73C
		public void OnLanesChanged()
		{
			this.UpdateLights();
		}

		// Token: 0x06002B35 RID: 11061 RVA: 0x000BE544 File Offset: 0x000BC744
		public void OnTrafficLightGreen(TrafficLightModel model, TileDirectionBitfield rightOfWay)
		{
			this.UpdateLights();
			this._audioSystem.ScheduleEvent(AudioEvent.CreateTrafficLightEvent(AudioEventType.TrafficLightGreen, this, rightOfWay));
		}

		// Token: 0x06002B36 RID: 11062 RVA: 0x000BE564 File Offset: 0x000BC764
		public void OnTrafficLightAmber(TrafficLightModel model)
		{
			this.UpdateLights();
			this._audioSystem.ScheduleEvent(AudioEvent.CreateTrafficLightEvent(AudioEventType.TrafficLightAmber, this, TileDirectionBitfield.None));
		}

		// Token: 0x06002B37 RID: 11063 RVA: 0x000BE588 File Offset: 0x000BC788
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
			this._interactionCircleView.InitializeTheme(themeDatabase);
		}

		// Token: 0x06002B38 RID: 11064 RVA: 0x000BE598 File Offset: 0x000BC798
		public void ApplyTheme(ITheme newTheme)
		{
			Theme theme = newTheme as Theme;
			if (theme != null)
			{
				this._redLightColor = theme.GetColor(ThemedMaterialType.TrafficLightRed, "_Color");
				this._amberLightColor = theme.GetColor(ThemedMaterialType.TrafficLightAmber, "_Color");
				this._greenLightColor = theme.GetColor(ThemedMaterialType.TrafficLightGreen, "_Color");
			}
			this.UpdateLights();
			this._interactionCircleView.ApplyTheme(theme);
		}

		// Token: 0x06002B39 RID: 11065 RVA: 0x000BE5FD File Offset: 0x000BC7FD
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			return this._interactionCircleView.ApplyBlendedTheme(oldTheme, newTheme, progress);
		}

		// Token: 0x06002B3A RID: 11066 RVA: 0x000BE60D File Offset: 0x000BC80D
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
			this._interactionCircleView.ReleaseTheme(themeDatabase);
		}

		// Token: 0x04002548 RID: 9544
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("TrafficLightView");

		// Token: 0x04002549 RID: 9545
		private TrafficLightModel _trafficLightModel;

		// Token: 0x0400254A RID: 9546
		private TileView _tileView;

		// Token: 0x0400254B RID: 9547
		[SerializeField]
		private InteractionCircleView _interactionCircleView;

		// Token: 0x0400254D RID: 9549
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x0400254E RID: 9550
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x0400254F RID: 9551
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x04002550 RID: 9552
		[EnumTypedArray(typeof(TileDirection))]
		[Space(10f)]
		[NonReorderable]
		public SpriteRenderer[] lightRenderers = new SpriteRenderer[8];

		// Token: 0x04002551 RID: 9553
		[NonReorderable]
		[EnumTypedArray(typeof(TileDirection))]
		public Animator[] lightAnimators = new Animator[8];

		// Token: 0x04002552 RID: 9554
		private Color _redLightColor;

		// Token: 0x04002553 RID: 9555
		private Color _amberLightColor;

		// Token: 0x04002554 RID: 9556
		private Color _greenLightColor;

		// Token: 0x04002555 RID: 9557
		private static readonly int ActiveHash = Animator.StringToHash("Active");

		// Token: 0x04002556 RID: 9558
		private static readonly int ChangeColorHash = Animator.StringToHash("ChangeColor");

		// Token: 0x04002557 RID: 9559
		private readonly TweenVector3 _interactionCirclePositionTween = new TweenVector3();

		// Token: 0x04002558 RID: 9560
		private readonly TweenVector3[] _trafficLightsOffsetsTweens = new TweenVector3[]
		{
			new TweenVector3(),
			new TweenVector3(),
			new TweenVector3(),
			new TweenVector3(),
			new TweenVector3(),
			new TweenVector3(),
			new TweenVector3(),
			new TweenVector3()
		};

		// Token: 0x04002559 RID: 9561
		private const string ChangeColor = "ChangeColor";

		// Token: 0x0400255A RID: 9562
		private const string Active = "Active";
	}
}
