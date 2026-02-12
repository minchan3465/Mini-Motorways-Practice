using System;
using System.Collections.Generic;
using Client;
using Easing;
using Factory;
using Factory.Pools;
using Motorways.Utility;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000592 RID: 1426
	public class DeadEndRoadView : MonoBehaviour, IView, IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x06002799 RID: 10137 RVA: 0x000A87B8 File Offset: 0x000A69B8
		public void Initialize(TileView tileView, TileDirection direction)
		{
			this._direction = direction;
			RoadTileConnection deadEndConnection = new RoadTileConnection(new RoadTileNode(direction, RoadType.TwoLane, -1), new RoadTileNode(direction, RoadType.TwoLane, -1));
			RoadTileConnectionStrokePath strokePath = this._roadTileAtlas.GetStrokePathForConnection(deadEndConnection);
			this._straightStrokePath = strokePath;
			using (RoadTileSignature signature = this._scope.Get<RoadTileSignature>())
			{
				signature.AddConnection(deadEndConnection);
				this._staticView = this._scope.Get<RoadView>();
				this._staticView.tileView = tileView;
				this._staticView.SetSignature(signature);
				this._staticView.transform.SetParent(base.transform, false);
				this._staticView.gameObject.SetActive(false);
				this._dynamicRoadMesh.Initialize(tileView, this._permanenceZoneTextureLibrary, this._city.Rules.RoadsBecomePermanentOverTime);
			}
		}

		// Token: 0x0600279A RID: 10138 RVA: 0x000A889C File Offset: 0x000A6A9C
		public void ReconfigurePermanenceVisibility()
		{
			this._dynamicRoadMesh.SetPermanenceVisibility(this._city.Rules.RoadsBecomePermanentOverTime);
			this._staticView.ReconfigurePermanenceVisibility();
		}

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x0600279B RID: 10139 RVA: 0x000A88C4 File Offset: 0x000A6AC4
		public TileDirection Direction
		{
			get
			{
				return this._direction;
			}
		}

		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x0600279C RID: 10140 RVA: 0x000A88CC File Offset: 0x000A6ACC
		public Spline.BezierSpline MedianSpline
		{
			get
			{
				if (this._isDynamic && this._distortedSpline != null)
				{
					return this._distortedSpline;
				}
				return this._straightStrokePath.pathSpline;
			}
		}

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x0600279D RID: 10141 RVA: 0x000A88F0 File Offset: 0x000A6AF0
		public TileDirection AutoDistortionTarget
		{
			get
			{
				return this._autoDistortionTarget;
			}
		}

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x0600279E RID: 10142 RVA: 0x000A88F8 File Offset: 0x000A6AF8
		// (set) Token: 0x0600279F RID: 10143 RVA: 0x000A8900 File Offset: 0x000A6B00
		public bool IsBeingReplaced { get; private set; }

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x060027A0 RID: 10144 RVA: 0x000A8909 File Offset: 0x000A6B09
		// (set) Token: 0x060027A1 RID: 10145 RVA: 0x000A8911 File Offset: 0x000A6B11
		public bool IsReplacing { get; private set; }

		// Token: 0x060027A2 RID: 10146 RVA: 0x000A891C File Offset: 0x000A6B1C
		public void AppearFromConnection(RoadTileConnection replacedConnection, float widthFactor = 1f)
		{
			if (this.SetAutoDistortionTarget(replacedConnection))
			{
				if (this._animationDirection == RoadAnimationDirection.None)
				{
					this.WidthFactor = widthFactor;
				}
				this._autoDistortionTween.Start(1f, 0f, this._visualConstants.DeadEndEmergeDuration, this._visualConstants.DeadEndEmergeEasingFunction, 0f);
				this._autoDistortionTweenTime = this._visualConstants.DeadEndEmergeDuration;
				this.IsDynamic = true;
				this.IsBeingReplaced = false;
				this.IsReplacing = true;
			}
		}

		// Token: 0x060027A3 RID: 10147 RVA: 0x000A8998 File Offset: 0x000A6B98
		public void ReplaceWithConnection(RoadTileConnection replacingConnection)
		{
			if (this.IsBeingReplaced)
			{
				return;
			}
			if (this.SetAutoDistortionTarget(replacingConnection))
			{
				float autoDistortionStartFactor = 0f;
				if (this._isManuallyDistorting)
				{
					if (this._autoDistortionTarget == this._manualDistortionTarget)
					{
						autoDistortionStartFactor = this._manualDistortionFactor;
						this.ClearManualDistortion();
					}
					else
					{
						this.CancelManualDistortion();
					}
				}
				this._autoDistortionTween.Start(autoDistortionStartFactor, 1f, this._visualConstants.DeadEndCollapseDuration, this._visualConstants.DeadEndCollapseEasingFunction, 0f);
				this._autoDistortionTweenTime = Mathf.Max(this._visualConstants.DeadEndCollapseDuration, this._visualConstants.AppearDuration);
				this.IsDynamic = true;
				this.IsBeingReplaced = true;
				this.IsReplacing = false;
				return;
			}
			this.CancelManualDistortion();
		}

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x060027A4 RID: 10148 RVA: 0x000A8A54 File Offset: 0x000A6C54
		public TileDirection ManualDistortionTarget
		{
			get
			{
				return this._manualDistortionTarget;
			}
		}

		// Token: 0x060027A5 RID: 10149 RVA: 0x000A8A5C File Offset: 0x000A6C5C
		public void SetManualDistortionTarget(TileDirection outputTarget, float easeDuration = 0f, Easings.Functions easeType = Easings.Functions.Linear)
		{
			if (this._manualDistortionTarget != outputTarget)
			{
				Spline.BezierSpline distortionSpline = this.GetSplineForConnection(outputTarget);
				if (distortionSpline == null)
				{
					return;
				}
				if (this._isManuallyDistorting)
				{
					if (easeDuration > 0f)
					{
						if (this._previousManualDistortionSpline == null)
						{
							this._previousManualDistortionSpline = this._manualDistortionSpline;
						}
						else
						{
							this._previousManualDistortionSpline = this.SlerpSpline(this._previousManualDistortionSpline, this._manualDistortionSpline, this._manualDistortionTargetTween.Value);
						}
						this._manualDistortionTargetTween.Start(0f, 1f, easeDuration, easeType, 0f);
					}
					else
					{
						this._previousManualDistortionSpline = null;
						this._manualDistortionTargetTween.Stop();
					}
				}
				if (this._manualDistortionFactorScale <= 0f || (this._manualDistortionFactorScaleTween.IsActive && this._manualDistortionFactorScaleTween.End <= 0f))
				{
					this._manualDistortionFactorScaleTween.Start(this._manualDistortionFactorScale, 1f, this._visualConstants.DeadEndEditDistortionStartDuration, this._visualConstants.DeadEndEditDistortionStartEasingFunction, 0f);
				}
				this._manualDistortionTarget = outputTarget;
				this._manualDistortionSpline = distortionSpline;
			}
			this._isManuallyDistorting = true;
			this.IsDynamic = true;
		}

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x060027A6 RID: 10150 RVA: 0x000A8B73 File Offset: 0x000A6D73
		// (set) Token: 0x060027A7 RID: 10151 RVA: 0x000A8B7B File Offset: 0x000A6D7B
		public float ManualDistortionFactor
		{
			get
			{
				return this._manualDistortionFactor;
			}
			set
			{
				this._manualDistortionFactor = Mathf.Clamp01(value);
				if (this._manualDistortionFactorScaleTween.IsActive && this._manualDistortionFactorScaleTween.End <= 0f)
				{
					this._manualDistortionFactorScaleTween.Stop();
				}
			}
		}

		// Token: 0x060027A8 RID: 10152 RVA: 0x000A8BB4 File Offset: 0x000A6DB4
		public void CancelManualDistortion()
		{
			if (this._isManuallyDistorting && (!this._manualDistortionFactorScaleTween.IsActive || this._manualDistortionFactorScaleTween.End > 0f))
			{
				this._manualDistortionFactorScaleTween.Start(this._manualDistortionFactorScale, 0f, this._visualConstants.DeadEndEditDistortionReturnDuration, this._visualConstants.DeadEndEditDistortionReturnEasingFunction, 0f);
			}
		}

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x060027A9 RID: 10153 RVA: 0x000A8C19 File Offset: 0x000A6E19
		public RoadState RoadState
		{
			get
			{
				return this._roadState;
			}
		}

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x060027AA RID: 10154 RVA: 0x000A8C21 File Offset: 0x000A6E21
		// (set) Token: 0x060027AB RID: 10155 RVA: 0x000A8C29 File Offset: 0x000A6E29
		public float WidthFactor
		{
			get
			{
				return this._widthFactor;
			}
			set
			{
				this._widthFactor = value;
				this._widthTween.Stop();
				this._animationDirection = RoadAnimationDirection.None;
			}
		}

		// Token: 0x060027AC RID: 10156 RVA: 0x000A8C44 File Offset: 0x000A6E44
		public void SetRoadState(RoadState newRoadState, TransitionStyle transitionStyle = TransitionStyle.Tween)
		{
			if (this._roadState == newRoadState)
			{
				return;
			}
			this._dynamicRoadMesh.RoadState = newRoadState;
			this._staticView.baseRenderer.material = ((newRoadState == RoadState.Mothballed) ? this._staticView.mothballedMaterial : this._staticView.activeMaterial);
			RoadState previousRoadState = this._roadState;
			this._roadState = newRoadState;
			if (newRoadState == RoadState.None || previousRoadState == RoadState.None)
			{
				float newWidth = (float)((newRoadState == RoadState.None) ? 0 : 1);
				if (transitionStyle == TransitionStyle.Snap)
				{
					this.WidthFactor = newWidth;
					this._animationDirection = RoadAnimationDirection.None;
					this.IsDynamic = false;
					return;
				}
				float startWidth;
				float endWidth;
				float duration;
				if (newRoadState == RoadState.None)
				{
					startWidth = 1f;
					endWidth = 0f;
					duration = this._visualConstants.DisappearDuration;
					this._animationDirection = RoadAnimationDirection.AnimatingOut;
					this._dynamicRoadMesh.CursorWidthFactor = 0f;
				}
				else
				{
					startWidth = 0f;
					endWidth = 1f;
					duration = this._visualConstants.AppearDuration;
					this._animationDirection = RoadAnimationDirection.AnimatingIn;
					this._dynamicRoadMesh.CursorWidthFactor = 1f;
				}
				this._widthTween.Start(this.WidthFactor, startWidth, endWidth, duration);
				this.IsDynamic = true;
			}
		}

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x060027AD RID: 10157 RVA: 0x000A8D4F File Offset: 0x000A6F4F
		// (set) Token: 0x060027AE RID: 10158 RVA: 0x000A8D58 File Offset: 0x000A6F58
		public bool IsDynamic
		{
			get
			{
				return this._isDynamic;
			}
			private set
			{
				this._isDynamic = value;
				this._dynamicRoadMesh.gameObject.SetActive(this._isDynamic);
				this._staticView.gameObject.SetActive(!this._isDynamic && this._roadState != RoadState.None && !this.IsBeingReplaced);
				if (this._isDynamic)
				{
					this._staticView.tileView.ResumeTicking();
				}
			}
		}

		// Token: 0x060027AF RID: 10159 RVA: 0x000A8DC8 File Offset: 0x000A6FC8
		public TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			if (this._isDynamic)
			{
				List<Vector2> points = this._straightStrokePath.pathPoints;
				bool stillAnimating = false;
				if (this._autoDistortionTweenTime > 0f || this._isManuallyDistorting)
				{
					this._distortedSpline = this._straightStrokePath.pathSpline;
					if (this._autoDistortionTweenTime > 0f)
					{
						if (this._autoDistortionTween.IsActive)
						{
							this._autoDistortionTween.Tick(tickTime.Delta);
						}
						float splineT = this._autoDistortionTween.Value;
						this._distortedSpline = this.SlerpSpline(this._straightStrokePath.pathSpline, this._autoDistortionSpline, splineT);
						this._autoDistortionTweenTime -= tickTime.Delta;
						stillAnimating |= (this._autoDistortionTweenTime > 0f);
					}
					if (this._isManuallyDistorting)
					{
						bool wasManualDistortionCancelled = false;
						if (this._manualDistortionFactorScaleTween.IsActive)
						{
							this._manualDistortionFactorScale = this._manualDistortionFactorScaleTween.Tick(tickTime.Delta);
							if (!this._manualDistortionFactorScaleTween.IsActive && this._manualDistortionFactorScale <= 0f)
							{
								wasManualDistortionCancelled = true;
								this.ClearManualDistortion();
							}
						}
						if (!wasManualDistortionCancelled)
						{
							Spline.BezierSpline manualDistortionSpline = this._manualDistortionSpline;
							if (this._manualDistortionTargetTween.IsActive)
							{
								float distortionT = this._manualDistortionTargetTween.Tick(tickTime.Delta);
								manualDistortionSpline = this.SlerpSpline(this._previousManualDistortionSpline, this._manualDistortionSpline, distortionT);
								if (!this._manualDistortionTargetTween.IsActive)
								{
									this._previousManualDistortionSpline = null;
								}
							}
							this._distortedSpline = this.SlerpSpline(this._distortedSpline, manualDistortionSpline, this._manualDistortionFactor * this._manualDistortionFactorScale);
							stillAnimating = true;
						}
					}
					Spline.RasterizedSpline rasterizedSpline = this._distortedSpline.Rasterize(25);
					float distortionLength = (this._straightStrokePath.pathSpline.inPoint - this._straightStrokePath.pathSpline.outPoint).magnitude;
					rasterizedSpline.Truncate(distortionLength);
					points = rasterizedSpline.Positions;
					if (TileUtilities.IsDirectionDiagonal(this._direction))
					{
						points.Insert(0, this._straightStrokePath.pathPoints[0]);
					}
				}
				else
				{
					this._distortedSpline = null;
				}
				if (this._widthTween.IsActive)
				{
					this._widthFactor = this._widthTween.Tick(tickTime.Delta);
					this._dynamicRoadMesh.OutlineWidthFactor = this._widthFactor;
					this._dynamicRoadMesh.RoadWidthFactor = this._widthFactor;
					if (this._widthTween.IsActive)
					{
						stillAnimating = true;
					}
					else
					{
						this._animationDirection = RoadAnimationDirection.None;
					}
				}
				else
				{
					this._dynamicRoadMesh.OutlineWidthFactor = this._widthFactor;
					this._dynamicRoadMesh.RoadWidthFactor = this._widthFactor;
				}
				if (!stillAnimating)
				{
					this.IsDynamic = false;
				}
				else
				{
					this._dynamicRoadMesh.SetPathPoints(points);
				}
			}
			if (this._city.Rules.RoadsBecomePermanentOverTime)
			{
				this._dynamicRoadMesh.UpdatePermanenceShaderValues();
				this._staticView.Tick(tickTime, stepAlpha);
				return TickResult.ContinueTicking;
			}
			if (!this._isDynamic)
			{
				return TickResult.StopTicking;
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x060027B0 RID: 10160 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x060027B1 RID: 10161 RVA: 0x000A90A7 File Offset: 0x000A72A7
		public void Subscribe(DeadEndRoadView.IObserver observer)
		{
			this._observers.Subscribe(observer);
		}

		// Token: 0x060027B2 RID: 10162 RVA: 0x000A90B5 File Offset: 0x000A72B5
		public void Unsubscribe(DeadEndRoadView.IObserver observer)
		{
			this._observers.Unsubscribe(observer);
		}

		// Token: 0x060027B3 RID: 10163 RVA: 0x000A90C4 File Offset: 0x000A72C4
		public void Reset()
		{
			this._roadState = RoadState.None;
			this._direction = TileDirection.North;
			this._distortedSpline = null;
			this._straightStrokePath = null;
			this.IsBeingReplaced = false;
			this.IsReplacing = false;
			this._autoDistortionTarget = TileDirection.None;
			this._autoDistortionSpline = null;
			this._autoDistortionTween.Reset();
			this._autoDistortionTweenTime = 0f;
			this._manualDistortionFactor = 0f;
			this._isManuallyDistorting = false;
			this._manualDistortionTarget = TileDirection.None;
			this._manualDistortionSpline = null;
			this._manualDistortionFactorScale = 0f;
			this._manualDistortionFactorScaleTween.Reset();
			this._manualDistortionTargetTween.Reset();
			this._animationDirection = RoadAnimationDirection.None;
			this._widthFactor = 0f;
			this._widthTween.Reset();
			this._widthTween.Reset();
			this._isDynamic = false;
			this._dynamicRoadMesh.Reset();
			base.transform.position = Vector3.zero;
		}

		// Token: 0x060027B4 RID: 10164 RVA: 0x000A91AC File Offset: 0x000A73AC
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._staticView != null)
			{
				this._staticView.transform.SetParent(null, false);
				this._staticView.gameObject.SetActive(true);
				this._scope.Release(this._staticView);
				this._staticView = null;
			}
			foreach (DeadEndRoadView.IObserver observer in this._observers)
			{
				observer.OnDeadEndReleased(this);
			}
		}

		// Token: 0x060027B5 RID: 10165 RVA: 0x000A9228 File Offset: 0x000A7428
		private bool SetAutoDistortionTarget(RoadTileConnection connection)
		{
			TileDirection otherDirection = (connection.input.direction == this._direction) ? connection.output.direction : connection.input.direction;
			if (this._autoDistortionTarget == otherDirection)
			{
				return true;
			}
			Spline.BezierSpline distortionSpline = this.GetSplineForConnection(otherDirection);
			if (distortionSpline != null)
			{
				this._autoDistortionTarget = otherDirection;
				this._autoDistortionSpline = distortionSpline;
				return true;
			}
			return false;
		}

		// Token: 0x060027B6 RID: 10166 RVA: 0x000A9288 File Offset: 0x000A7488
		private void ClearManualDistortion()
		{
			this._manualDistortionFactor = 0f;
			this._isManuallyDistorting = false;
			this._manualDistortionTarget = TileDirection.None;
			this._manualDistortionSpline = null;
			this._manualDistortionFactorScale = 0f;
			this._manualDistortionFactorScaleTween.Stop();
			this._manualDistortionTargetTween.Stop();
			this._previousManualDistortionSpline = null;
		}

		// Token: 0x060027B7 RID: 10167 RVA: 0x000A92E0 File Offset: 0x000A74E0
		private Spline.BezierSpline GetSplineForConnection(TileDirection outputDirection)
		{
			RoadTileConnection connection = new RoadTileConnection(this._direction, outputDirection);
			RoadTileConnectionStrokePath connectionStrokePath = this._roadTileAtlas.GetStrokePathForConnection(connection);
			if (!Diagnostics.Verify(connectionStrokePath != null, "Unable to find mesh for distortion connection {0}.", connection))
			{
				return null;
			}
			return connectionStrokePath.pathSpline;
		}

		// Token: 0x060027B8 RID: 10168 RVA: 0x000A9326 File Offset: 0x000A7526
		private Spline.BezierSpline SlerpSpline(Spline.BezierSpline a, Spline.BezierSpline b, float t)
		{
			return Spline.BezierSpline.Lerp(a, b, t);
		}

		// Token: 0x060027B9 RID: 10169 RVA: 0x000A9330 File Offset: 0x000A7530
		private Vector2 SlerpVector(Vector2 a, Vector2 b, float t)
		{
			float angle = Mathf.LerpAngle(Mathf.Atan2(a.y, a.x) * 57.29578f, Mathf.Atan2(b.y, b.x) * 57.29578f, t) * 0.017453292f;
			float magnitude = Mathf.Lerp(a.magnitude, b.magnitude, t);
			return new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
		}

		// Token: 0x060027BA RID: 10170 RVA: 0x000A93A4 File Offset: 0x000A75A4
		public void OnDrawGizmosSelected()
		{
			Vector3 tileCentre = base.transform.position;
			RoadTileConnectionStrokePath straightStrokePath = this._straightStrokePath;
			if (((straightStrokePath != null) ? straightStrokePath.pathSpline : null) != null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawLine(tileCentre + this._straightStrokePath.pathSpline.inPoint, tileCentre + this._straightStrokePath.pathSpline.inHandle);
				Gizmos.DrawLine(tileCentre + this._straightStrokePath.pathSpline.inHandle, tileCentre + this._straightStrokePath.pathSpline.outHandle);
				Gizmos.DrawLine(tileCentre + this._straightStrokePath.pathSpline.outHandle, tileCentre + this._straightStrokePath.pathSpline.outPoint);
			}
			if (this._autoDistortionSpline != null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(tileCentre + this._autoDistortionSpline.inPoint, tileCentre + this._autoDistortionSpline.inHandle);
				Gizmos.DrawLine(tileCentre + this._autoDistortionSpline.inHandle, tileCentre + this._autoDistortionSpline.outHandle);
				Gizmos.DrawLine(tileCentre + this._autoDistortionSpline.outHandle, tileCentre + this._autoDistortionSpline.outPoint);
			}
		}

		// Token: 0x0400217E RID: 8574
		private TileDirection _direction;

		// Token: 0x0400217F RID: 8575
		private RoadTileConnectionStrokePath _straightStrokePath;

		// Token: 0x04002180 RID: 8576
		private Spline.BezierSpline _distortedSpline;

		// Token: 0x04002181 RID: 8577
		private TileDirection _autoDistortionTarget = TileDirection.None;

		// Token: 0x04002182 RID: 8578
		private Spline.BezierSpline _autoDistortionSpline;

		// Token: 0x04002183 RID: 8579
		private readonly TweenFloat _autoDistortionTween = new TweenFloat();

		// Token: 0x04002184 RID: 8580
		private float _autoDistortionTweenTime;

		// Token: 0x04002185 RID: 8581
		private bool _isManuallyDistorting;

		// Token: 0x04002186 RID: 8582
		private TileDirection _manualDistortionTarget = TileDirection.None;

		// Token: 0x04002187 RID: 8583
		private Spline.BezierSpline _manualDistortionSpline;

		// Token: 0x04002188 RID: 8584
		private float _manualDistortionFactor;

		// Token: 0x04002189 RID: 8585
		private float _manualDistortionFactorScale;

		// Token: 0x0400218A RID: 8586
		private readonly TweenFloat _manualDistortionFactorScaleTween = new TweenFloat();

		// Token: 0x0400218B RID: 8587
		private Spline.BezierSpline _previousManualDistortionSpline;

		// Token: 0x0400218C RID: 8588
		private readonly TweenFloat _manualDistortionTargetTween = new TweenFloat();

		// Token: 0x0400218D RID: 8589
		private RoadAnimationDirection _animationDirection;

		// Token: 0x0400218E RID: 8590
		private float _widthFactor;

		// Token: 0x0400218F RID: 8591
		private readonly TweenFloat _widthTween = new TweenFloat();

		// Token: 0x04002190 RID: 8592
		private bool _isDynamic;

		// Token: 0x04002191 RID: 8593
		[SerializeField]
		private DynamicRoadMesh _dynamicRoadMesh;

		// Token: 0x04002192 RID: 8594
		private RoadView _staticView;

		// Token: 0x04002193 RID: 8595
		private RoadState _roadState;

		// Token: 0x04002194 RID: 8596
		private readonly ObserverList<DeadEndRoadView.IObserver> _observers = new ObserverList<DeadEndRoadView.IObserver>(1);

		// Token: 0x04002195 RID: 8597
		[Dependency]
		private RoadTileAtlas _roadTileAtlas;

		// Token: 0x04002196 RID: 8598
		[Dependency]
		private IScope _scope;

		// Token: 0x04002197 RID: 8599
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x04002198 RID: 8600
		[Dependency]
		private City _city;

		// Token: 0x04002199 RID: 8601
		[Dependency]
		private PermanenceZoneTextureLibrary _permanenceZoneTextureLibrary;

		// Token: 0x02000593 RID: 1427
		public interface IObserver
		{
			// Token: 0x060027BC RID: 10172
			void OnDeadEndReleased(DeadEndRoadView deadEnd);
		}
	}
}
