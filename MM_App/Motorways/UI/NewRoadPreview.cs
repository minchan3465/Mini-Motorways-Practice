using System;
using System.Collections.Generic;
using Client;
using Easing;
using Factory;
using Factory.Pools;
using Motorways.Utility;
using Motorways.Views;
using UnityEngine;

namespace Motorways.UI
{
	// Token: 0x02000738 RID: 1848
	public class NewRoadPreview : MonoBehaviour, IReusable, IView, ICreatedInScopeHandler, IReleasedFromScopeHandler, DeadEndRoadView.IObserver
	{
		// Token: 0x0600339B RID: 13211 RVA: 0x000F3CE0 File Offset: 0x000F1EE0
		public void SetPosition(Vector2Int fromCoordinates, Vector2 toPosition)
		{
			bool didOriginChange = this._originCoordinates != fromCoordinates;
			if (didOriginChange)
			{
				if (this.IsVisible)
				{
					this._directionWhenOriginChanged = TileUtilities.GetDirectionBetweenAdjacentCoordinates(this._originCoordinates, fromCoordinates);
					if (this._directionWhenOriginChanged == TileDirection.None)
					{
						this._directionWhenOriginChanged = TileUtilities.GetClosestDirection(fromCoordinates - this._originCoordinates);
					}
					this._pointerPositionWhenOriginChanged = this._pointerPosition;
					this._pointerMovementSinceOriginChanged = 0f;
				}
				this._originCoordinates = fromCoordinates;
				base.transform.position = TilemapView.GetWorldPositionForCoordinates(fromCoordinates);
				if (this._distortingDeadEnd != null)
				{
					this._distortingDeadEnd.Unsubscribe(this);
					this._distortingDeadEnd.CancelManualDistortion();
					this._distortingDeadEnd = null;
				}
			}
			if (!this.IsVisible)
			{
				this._coordinatesWhenMinified = fromCoordinates;
				this._pointerPositionWhenMinified = toPosition;
				this.IsVisible = true;
			}
			Vector2 centerOfCurrentTile = TilemapView.GetWorldPositionForCoordinates(this._originCoordinates);
			Vector2 tileCenterToPointer = toPosition - centerOfCurrentTile;
			TileDirection direction = TileUtilities.GetClosestDirection(tileCenterToPointer);
			this._previewLength = Mathf.Min(tileCenterToPointer.magnitude, this.MaximumLength);
			if (this._directionWhenOriginChanged != TileDirection.None)
			{
				this._pointerMovementSinceOriginChanged += Vector2.Distance(toPosition, this._pointerPosition);
				Vector2 displacementSinceOriginChanged = toPosition - this._pointerPositionWhenOriginChanged;
				if (this._pointerMovementSinceOriginChanged < this.DirectionChangeThreshold || displacementSinceOriginChanged.sqrMagnitude <= 0f || TileUtilities.GetClosestDirection(displacementSinceOriginChanged) == this._directionWhenOriginChanged)
				{
					direction = this._directionWhenOriginChanged;
				}
				else
				{
					this._directionWhenOriginChanged = TileDirection.None;
				}
			}
			bool didDirectionChange = false;
			if (direction != this._direction)
			{
				didDirectionChange = true;
				this._direction = direction;
			}
			TileView originTile = this._tilemap.GetTileView(fromCoordinates);
			DeadEndRoadView newDistortedDeadEnd = (originTile == null || originTile.Tile.ContentType == TileContentType.House || !originTile.CanAnimateNewConnections) ? null : originTile.ActiveDeadEnd;
			if (newDistortedDeadEnd != null && !newDistortedDeadEnd.IsBeingReplaced)
			{
				if (newDistortedDeadEnd != this._distortingDeadEnd)
				{
					this.CancelDeadEndDistortion();
				}
				if (newDistortedDeadEnd.Direction != this._direction)
				{
					if (newDistortedDeadEnd.ManualDistortionTarget != this._direction)
					{
						newDistortedDeadEnd.SetManualDistortionTarget(this._direction, this.AngleTweenDuration, this.AngleTweenEaseType);
					}
					newDistortedDeadEnd.ManualDistortionFactor = this.DeadEndDistortionCurve.Evaluate(this._previewLength * this._directionScale);
					if (this._distortingDeadEnd == null)
					{
						this._distortingDeadEnd = newDistortedDeadEnd;
						this._distortingDeadEnd.Subscribe(this);
					}
				}
				else
				{
					newDistortedDeadEnd.CancelManualDistortion();
					this.CancelDeadEndDistortion();
				}
			}
			else
			{
				this.CancelDeadEndDistortion();
			}
			this._directionScale = 1f;
			if (this._direction != TileDirection.None)
			{
				Vector2 rawDirectionVector = TileUtilities.GetVectorForDirection(this._direction);
				this._directionScale = Mathf.Clamp01(Vector2.Dot(tileCenterToPointer.normalized, rawDirectionVector));
				if (this._directionScale <= 0f && this._distortingDeadEnd == null)
				{
					this._hasExtended = false;
					this._coordinatesWhenMinified = this._originCoordinates;
					this._pointerPositionWhenMinified = toPosition;
					this._extensionTween.Stop();
				}
			}
			if (!this._hasExtended && (this._distortingDeadEnd != null || this._coordinatesWhenMinified != this._originCoordinates || ((Vector2.Distance(this._pointerPositionWhenMinified, toPosition) >= this.ExtensionMovementThreshold || this._previewLength >= this.ExtensionDistanceThreshold) && this._directionScale > 0f)))
			{
				this._hasExtended = true;
				this._extensionTween.Start(0f, 1f, this.ExtensionTweenDuration, this.ExtensionTweenEaseType, 0f);
			}
			if (didDirectionChange)
			{
				float targetAngle = (float)direction * -0.7853982f;
				if (didOriginChange || this._direction == TileDirection.None || !this._hasExtended)
				{
					this._angle = targetAngle;
				}
				else
				{
					this._angleTween.Start(this._angle, targetAngle, this.AngleTweenDuration, this.AngleTweenEaseType, 0f);
				}
			}
			this._pointerPosition = toPosition;
		}

		// Token: 0x0600339C RID: 13212 RVA: 0x000F40C0 File Offset: 0x000F22C0
		public void Remove()
		{
			TileView originTile = this._tilemap.GetTileView(this._originCoordinates);
			DeadEndRoadView distortedDeadEnd = (originTile == null) ? null : originTile.ActiveDeadEnd;
			if (distortedDeadEnd != null)
			{
				distortedDeadEnd.CancelManualDistortion();
			}
			this.IsVisible = false;
			this._isRemoving = true;
		}

		// Token: 0x0600339D RID: 13213 RVA: 0x000F4110 File Offset: 0x000F2310
		public TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			if (this._widthTween.IsActive)
			{
				float widthFactor = this._widthTween.Tick(tickTime.Delta);
				float curvedWidthFactor;
				if (this._widthCurveDirection == RoadAnimationDirection.AnimatingIn)
				{
					curvedWidthFactor = this.ScaleInCurve.Evaluate(widthFactor);
				}
				else
				{
					curvedWidthFactor = this.ScaleOutCurve.Evaluate(1f - widthFactor);
				}
				this._roadMesh.CursorWidthFactor = curvedWidthFactor;
			}
			float lengthScale = (float)((this._hasExtended && !this._isRemoving) ? 1 : 0);
			if (this._extensionTween.IsActive)
			{
				lengthScale = this._extensionTween.Tick(tickTime.Delta);
			}
			if (this._angleTween.IsActive)
			{
				this._angle = this._angleTween.Tick(tickTime.Delta);
			}
			float pathLength = this._previewLength * lengthScale * this._directionScale;
			if (this._distortingDeadEnd != null)
			{
				Spline.BezierSpline deadEndSpline = this._distortingDeadEnd.MedianSpline;
				pathLength += deadEndSpline.inPoint.magnitude;
				Spline.RasterizedSpline rasterizedSpline = deadEndSpline.Rasterize(25);
				rasterizedSpline.Truncate(pathLength);
				List<Vector2> splinePoints = rasterizedSpline.Positions;
				float splineLength = rasterizedSpline.Length;
				if (splineLength < pathLength)
				{
					Vector2 handleDirection = (deadEndSpline.outPoint - deadEndSpline.outHandle).normalized;
					splinePoints.Add(splinePoints[splinePoints.Count - 1] + handleDirection * (pathLength - splineLength));
				}
				this._roadMesh.SetPathPoints(splinePoints);
			}
			else
			{
				Vector2 pathEnd = TileUtilities.GetVectorForDirection(TileDirection.North).Rotated(this._angle);
				pathEnd *= pathLength;
				this._roadMesh.SetPathPoints(new List<Vector2>
				{
					Vector2.zero,
					pathEnd
				});
			}
			this._roadMesh.SetCursorRendererHazardStripesAngle(this._angle + 0.7853982f);
			float fadeoutStart = Mathf.Max(pathLength - this.FadeLength, this.FadeoutStartLength);
			float alpha = 1f - Mathf.Clamp01((pathLength - this.FadeoutStartLength) / this.FadeLength);
			alpha = Easings.CubicEaseOut(alpha);
			this._roadMesh.SetCursorRendererFadeout(Mathf.Clamp01(fadeoutStart / pathLength), 1f, alpha);
			if (this._widthTween.IsActive || !this._isRemoving)
			{
				return TickResult.ContinueTicking;
			}
			return TickResult.Destroy;
		}

		// Token: 0x0600339E RID: 13214 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x0600339F RID: 13215 RVA: 0x000F4349 File Offset: 0x000F2549
		public void SetHazardStripesEnabled(bool stripesEnabled, bool tween = false)
		{
			this._roadMesh.SetCursorRendererHazardStripesEnabled(stripesEnabled, tween);
		}

		// Token: 0x060033A0 RID: 13216 RVA: 0x000F4358 File Offset: 0x000F2558
		public void OnCreatedInScope(IScope scope)
		{
			this._roadMesh.HasEndCap = true;
			this.SetHazardStripesEnabled(false, false);
		}

		// Token: 0x060033A1 RID: 13217 RVA: 0x000F436E File Offset: 0x000F256E
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._distortingDeadEnd != null)
			{
				this._distortingDeadEnd.Unsubscribe(this);
				this._distortingDeadEnd = null;
			}
		}

		// Token: 0x060033A2 RID: 13218 RVA: 0x000F4391 File Offset: 0x000F2591
		public void OnDeadEndReleased(DeadEndRoadView deadEnd)
		{
			if (deadEnd == this._distortingDeadEnd)
			{
				this._distortingDeadEnd.Unsubscribe(this);
				this._distortingDeadEnd = null;
			}
		}

		// Token: 0x060033A3 RID: 13219 RVA: 0x000F43B4 File Offset: 0x000F25B4
		public void Reset()
		{
			base.transform.localPosition = Vector3.zero;
			this._isVisible = false;
			this._originCoordinates = default(Vector2Int);
			this._pointerPosition = default(Vector2);
			this._direction = TileDirection.None;
			this._previewLength = 0f;
			this._directionScale = 0f;
			this._distortingDeadEnd = null;
			this._directionWhenOriginChanged = TileDirection.None;
			this._pointerPositionWhenOriginChanged = default(Vector2);
			this._pointerMovementSinceOriginChanged = 0f;
			this._widthTween.Reset();
			this._widthCurveDirection = RoadAnimationDirection.None;
			this._hasExtended = false;
			this._extensionTween.Reset();
			this._coordinatesWhenMinified = default(Vector2Int);
			this._pointerPositionWhenMinified = default(Vector2);
			this._angle = 0f;
			this._angleTween.Reset();
			this._isRemoving = false;
		}

		// Token: 0x17000892 RID: 2194
		// (get) Token: 0x060033A4 RID: 13220 RVA: 0x000F448B File Offset: 0x000F268B
		// (set) Token: 0x060033A5 RID: 13221 RVA: 0x000F4494 File Offset: 0x000F2694
		private bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (value == this._isVisible)
				{
					return;
				}
				this._isVisible = value;
				if (!this._widthTween.IsActive)
				{
					this._widthCurveDirection = (this._isVisible ? RoadAnimationDirection.AnimatingIn : RoadAnimationDirection.AnimatingOut);
				}
				if (this._isVisible)
				{
					this._widthTween.Start(this._widthTween.Value, 0f, 1f, this.ScaleInDuration);
					return;
				}
				if (this._hasExtended)
				{
					this._extensionTween.Start(this._extensionTween.Value, 0f, this.ContractionTweenDuration, this._extensionTween.IsActive ? this.ExtensionTweenEaseType : this.ContractionTweenEaseType, 0f);
					this._widthTween.Start(this._widthTween.Value, 0f, this.ScaleOutDuration, Easings.Functions.Linear, this._extensionTween.Duration);
					return;
				}
				this._widthTween.Start(this._widthTween.Value, 1f, 0f, this.ScaleOutDuration);
			}
		}

		// Token: 0x060033A6 RID: 13222 RVA: 0x000F459D File Offset: 0x000F279D
		private void CancelDeadEndDistortion()
		{
			if (this._distortingDeadEnd != null)
			{
				this._distortingDeadEnd.CancelManualDistortion();
				this._distortingDeadEnd.Unsubscribe(this);
				this._distortingDeadEnd = null;
			}
		}

		// Token: 0x04002BF9 RID: 11257
		[SerializeField]
		private DynamicRoadMesh _roadMesh;

		// Token: 0x04002BFA RID: 11258
		[Min(0.001f)]
		[SerializeField]
		private float ScaleInDuration = 0.3f;

		// Token: 0x04002BFB RID: 11259
		[SerializeField]
		private AnimationCurve ScaleInCurve;

		// Token: 0x04002BFC RID: 11260
		[Min(0.001f)]
		[SerializeField]
		private float ScaleOutDuration = 0.1f;

		// Token: 0x04002BFD RID: 11261
		[SerializeField]
		private AnimationCurve ScaleOutCurve;

		// Token: 0x04002BFE RID: 11262
		[SerializeField]
		private float MaximumLength = 4f;

		// Token: 0x04002BFF RID: 11263
		[SerializeField]
		private float FadeoutStartLength = 4f;

		// Token: 0x04002C00 RID: 11264
		[SerializeField]
		private float FadeLength = 0.5f;

		// Token: 0x04002C01 RID: 11265
		private bool _isVisible;

		// Token: 0x04002C02 RID: 11266
		private Vector2Int _originCoordinates;

		// Token: 0x04002C03 RID: 11267
		private Vector2 _pointerPosition;

		// Token: 0x04002C04 RID: 11268
		private TileDirection _direction = TileDirection.None;

		// Token: 0x04002C05 RID: 11269
		private float _previewLength;

		// Token: 0x04002C06 RID: 11270
		private float _directionScale;

		// Token: 0x04002C07 RID: 11271
		private DeadEndRoadView _distortingDeadEnd;

		// Token: 0x04002C08 RID: 11272
		[Tooltip("How much the preview should distort a dead end to match the preview's direction. The x-axis is the preview's length in world-space. The y-axis is the distortion factor (0 is no distortion, 1 is full distortion).")]
		[SerializeField]
		private AnimationCurve DeadEndDistortionCurve;

		// Token: 0x04002C09 RID: 11273
		private TileDirection _directionWhenOriginChanged = TileDirection.None;

		// Token: 0x04002C0A RID: 11274
		private Vector2 _pointerPositionWhenOriginChanged;

		// Token: 0x04002C0B RID: 11275
		private float _pointerMovementSinceOriginChanged;

		// Token: 0x04002C0C RID: 11276
		[SerializeField]
		[Tooltip("How far the pointer must move after a road is built before the direction of the preview can be changed. Lower this to make the preview more responsive to direction changes after a road is built, at the expense of risking rapid changes in direction if the player's input is twitchy.")]
		private float DirectionChangeThreshold = 0.4f;

		// Token: 0x04002C0D RID: 11277
		private readonly TweenFloat _widthTween = new TweenFloat();

		// Token: 0x04002C0E RID: 11278
		private RoadAnimationDirection _widthCurveDirection;

		// Token: 0x04002C0F RID: 11279
		private bool _hasExtended;

		// Token: 0x04002C10 RID: 11280
		private readonly TweenFloat _extensionTween = new TweenFloat();

		// Token: 0x04002C11 RID: 11281
		private Vector2Int _coordinatesWhenMinified;

		// Token: 0x04002C12 RID: 11282
		private Vector2 _pointerPositionWhenMinified;

		// Token: 0x04002C13 RID: 11283
		[Tooltip("How far the pointer must move from its original position before the preview extends. Each tile is two units wide.")]
		[SerializeField]
		private float ExtensionMovementThreshold = 0.1f;

		// Token: 0x04002C14 RID: 11284
		[Tooltip("The maximum distance the pointer can be from the centre of its original tile before the preview extends, regardless of how far it has moved.")]
		[SerializeField]
		private float ExtensionDistanceThreshold = 0.1f;

		// Token: 0x04002C15 RID: 11285
		[SerializeField]
		[Tooltip("The duration of the tween out when the preview extends from a dot to a line.")]
		private float ExtensionTweenDuration = 0.3f;

		// Token: 0x04002C16 RID: 11286
		[SerializeField]
		[Tooltip("The easing function used when the preview extends from a dot to a line.")]
		private Easings.Functions ExtensionTweenEaseType;

		// Token: 0x04002C17 RID: 11287
		[SerializeField]
		[Tooltip("The duration of the tween out when the preview contracts from a line back into a dot.")]
		private float ContractionTweenDuration = 0.3f;

		// Token: 0x04002C18 RID: 11288
		[Tooltip("The easing function used when the preview contracts from a line back into a dot.")]
		[SerializeField]
		private Easings.Functions ContractionTweenEaseType;

		// Token: 0x04002C19 RID: 11289
		private float _angle;

		// Token: 0x04002C1A RID: 11290
		private readonly TweenRadians _angleTween = new TweenRadians();

		// Token: 0x04002C1B RID: 11291
		[SerializeField]
		[Tooltip("The duration of the tween when the preview changes in direction.")]
		private float AngleTweenDuration = 0.07f;

		// Token: 0x04002C1C RID: 11292
		[Tooltip("The easing function used when the preview changes in direction.")]
		[SerializeField]
		private Easings.Functions AngleTweenEaseType;

		// Token: 0x04002C1D RID: 11293
		private bool _isRemoving;

		// Token: 0x04002C1E RID: 11294
		[Dependency]
		private TilemapView _tilemap;

		// Token: 0x04002C1F RID: 11295
		private const float HazardStripeAngleOffset = 0.7853982f;
	}
}
