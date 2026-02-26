using System;
using System.Collections.Generic;
using Client;
using Easing;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Constants;
using Motorways.Models;
using Motorways.Themes;
using Motorways.Utility;
using Rendering.RenderFeatures;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005DA RID: 1498
	public class MotorwayView : MonoBehaviour, IView, Motorway.IObserver, TileView.IObserver, IThemeComponent, ICreatedInScopeHandler, IReleasedFromScopeHandler, IReusable
	{
		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x060029CF RID: 10703 RVA: 0x000B42BF File Offset: 0x000B24BF
		// (set) Token: 0x060029D0 RID: 10704 RVA: 0x000B42C7 File Offset: 0x000B24C7
		[Dependency]
		public City City { get; private set; }

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x060029D1 RID: 10705 RVA: 0x000B42D0 File Offset: 0x000B24D0
		public TilemapView Tilemap
		{
			get
			{
				return this._tilemap;
			}
		}

		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x060029D2 RID: 10706 RVA: 0x000B42D8 File Offset: 0x000B24D8
		public MotorwaySpline Spline
		{
			get
			{
				return this._spline;
			}
		}

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x060029D3 RID: 10707 RVA: 0x000B42E0 File Offset: 0x000B24E0
		public Motorway Motorway
		{
			get
			{
				return this._clientMotorway;
			}
		}

		// Token: 0x060029D4 RID: 10708 RVA: 0x000B42E8 File Offset: 0x000B24E8
		public void Initialize(TilemapView tilemap, int id, int number, RoadState visualRoadState, MotorwayView replacedMotorwayView = null)
		{
			MotorwayView.Log.Info("Creating MotorwayView, id {0}.", new object[]
			{
				id
			});
			this.ImmediatelyTransitionVisualRoadStateTo(visualRoadState);
			this._clientMotorway = new Motorway();
			this._clientMotorway.Initialize(tilemap, id, number, RoadState.None);
			this._clientMotorway.Subscribe(this);
			tilemap.ResortMotorwaysOnNextTick();
			if (Diagnostics.Verify(this.handleView != null, "MotorwayHandleView is not set on MotorwayView prefab"))
			{
				this.handleView.Initialize(this._scope, this, number);
			}
			this._visualParameters.OnParameterChanged += this.OnVisualParameterChanged;
			this._shadowMeshRenderer.enabled = true;
			this._materialPropertyBlock.Clear();
			this._materialPropertyBlock.SetInt(ShaderConstants.LinearDistanceTableLength, 10);
			this._materialPropertyBlock.SetInt(ShaderConstants.HazardStripeLastIndex, 199);
			this._referenceMidpointAngle = 0f;
			this._referenceMidpointDistance = 0f;
			if (replacedMotorwayView != null)
			{
				Vector2 midpointDisplacement = replacedMotorwayView._splineMidpoint - replacedMotorwayView._naturalMidpoint;
				float midpointDistance = midpointDisplacement.magnitude;
				if (midpointDistance > 0.1f)
				{
					Vector2 motorwayVector = TilemapView.GetWorldPositionForCoordinates(replacedMotorwayView.EndCoordinates) - TilemapView.GetWorldPositionForCoordinates(replacedMotorwayView.StartCoordinates);
					float motorwayLength = motorwayVector.magnitude;
					motorwayVector /= motorwayLength;
					Vector2 midpointDirection = midpointDisplacement / midpointDistance;
					float referenceMidpointAngle = Mathf.Acos(Vector2.Dot(motorwayVector, midpointDirection));
					this._referenceMidpointAngle = referenceMidpointAngle * Mathf.Sign(Vector2.Dot(motorwayVector.GetTangent(), midpointDirection));
					this._referenceMidpointDistance = midpointDistance / motorwayLength;
					this._referenceMotorwayDirection = motorwayVector;
					this._hasCheckedReferenceMotorwayDirection = false;
				}
				this._replacedMotorway = replacedMotorwayView.Motorway;
				this._replacedMotorway.Subscribe(this);
			}
		}

		// Token: 0x060029D5 RID: 10709 RVA: 0x000B44A5 File Offset: 0x000B26A5
		private void OnVisualParameterChanged()
		{
			this._rebuildGeometry = true;
			this._reapplyPermanence = true;
			this.RebuildMotorwayView();
		}

		// Token: 0x060029D6 RID: 10710 RVA: 0x000B44BB File Offset: 0x000B26BB
		public void SetModel(MotorwayModel motorwayModel)
		{
			this._model = motorwayModel;
			this._model.Subscribe(this);
		}

		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x060029D7 RID: 10711 RVA: 0x000B44D0 File Offset: 0x000B26D0
		public MotorwayModel Model
		{
			get
			{
				return this._model;
			}
		}

		// Token: 0x060029D8 RID: 10712 RVA: 0x000B44D8 File Offset: 0x000B26D8
		public void AddEdit(ClientTileEdit edit)
		{
			this._clientTileEdits.Add(edit);
			this._rebuildMotorway = true;
		}

		// Token: 0x060029D9 RID: 10713 RVA: 0x000B44ED File Offset: 0x000B26ED
		public void RemoveEdit(ClientTileEdit edit)
		{
			this._clientTileEdits.Remove(edit);
			this._rebuildMotorway = true;
		}

		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x060029DA RID: 10714 RVA: 0x000B4503 File Offset: 0x000B2703
		// (set) Token: 0x060029DB RID: 10715 RVA: 0x000B450C File Offset: 0x000B270C
		public bool IsBeingEdited
		{
			get
			{
				return this._isBeingEdited;
			}
			set
			{
				if (!this._isBeingEdited && value)
				{
					this.BringToTop();
				}
				if (this._isBeingEdited && !value)
				{
					this._isMotorwayOnTop = false;
					this._tilemap.RecalculateDefaultMotorwaySortOrder();
					this._tilemap.ResortMotorwaysOnNextTick();
				}
				this._isBeingEdited = value;
			}
		}

		// Token: 0x17000717 RID: 1815
		// (set) Token: 0x060029DC RID: 10716 RVA: 0x000B455B File Offset: 0x000B275B
		public bool IsDraggingHandle
		{
			set
			{
				if (this._isDraggingHandle != value)
				{
					this._isDraggingHandle = value;
					if (this._isDraggingHandle)
					{
						this.BringToTop();
						this._handleDistanceFromMidpoint.Hold();
						return;
					}
					this._resortMotorwaysWhenSpringingIsComplete = true;
					this._handleDistanceFromMidpoint.SpringBackToExtents();
				}
			}
		}

		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x060029DD RID: 10717 RVA: 0x000B4599 File Offset: 0x000B2799
		// (set) Token: 0x060029DE RID: 10718 RVA: 0x000B45BC File Offset: 0x000B27BC
		public Vector2 RawHandlePosition
		{
			get
			{
				return this._naturalMidpoint + this._handleDirectionFromMidpoint * this._handleDistanceFromMidpoint.RawValue;
			}
			set
			{
				Vector2 handleDelta = value - this._naturalMidpoint;
				this._handleDistanceFromMidpoint.RawValue = handleDelta.magnitude;
				if (this._handleDistanceFromMidpoint.RawValue > 0f)
				{
					this._handleDirectionFromMidpoint = handleDelta / this._handleDistanceFromMidpoint.RawValue;
					return;
				}
				this._handleDirectionFromMidpoint = Vector3.zero;
			}
		}

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x060029DF RID: 10719 RVA: 0x000B4622 File Offset: 0x000B2822
		public Vector2 HandlePosition
		{
			get
			{
				return this._naturalMidpoint + this._handleDirectionFromMidpoint * this._handleDistanceFromMidpoint.ConstrainedValue;
			}
		}

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x060029E0 RID: 10720 RVA: 0x000B4645 File Offset: 0x000B2845
		public float HandleTension
		{
			get
			{
				if (this._handleDistanceFromMidpoint.IsWithinConstraints)
				{
					return 0f;
				}
				return (this._handleDistanceFromMidpoint.RawValue - this._handleDistanceFromMidpoint.ConstrainedValue) / this._handleDistanceFromMidpoint.Max;
			}
		}

		// Token: 0x060029E1 RID: 10721 RVA: 0x000B4680 File Offset: 0x000B2880
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._replacedMotorway != null)
			{
				Fix64 newPermanence = this._replacedMotorway.PermanenceProgress;
				this._clientMotorway.SetPermanence(newPermanence);
			}
			if (this._rebuildMotorway)
			{
				this.RebuildMotorway();
			}
			if (this._startInteractionCirclePositionTween.IsActive)
			{
				this._interactionCircleViewStart.transform.position = this._startInteractionCirclePositionTween.Tick(timeInterval.Delta);
			}
			if (this._endInteractionCirclePositionTween.IsActive)
			{
				this._interactionCircleViewEnd.transform.position = this._endInteractionCirclePositionTween.Tick(timeInterval.Delta);
			}
			this._handleDistanceFromMidpoint.Tick(timeInterval.Delta);
			if (this._splineMidpoint != this.HandlePosition)
			{
				this._splineMidpoint = this.HandlePosition;
				this._rebuildGeometry = true;
			}
			if (this._resortMotorwaysWhenSpringingIsComplete && !this._handleDistanceFromMidpoint.IsSpringing)
			{
				this._isMotorwayOnTop = false;
				this._resortMotorwaysWhenSpringingIsComplete = false;
				this._tilemap.ResortMotorwaysOnNextTick();
			}
			this.RebuildMotorwayView();
			if (this._reapplyPermanence)
			{
				this.SetPermanenceProgress(this.City.Rules.RoadsBecomePermanentOverTime ? this._clientMotorway.PermanenceProgress : Fix64.Zero);
				this.RecalculateHazardStripeVisibility();
				this._reapplyPermanence = false;
			}
			bool ensureHazardStripeUpdate = false;
			if (this._hazardStripeWidth.IsActive)
			{
				this._hazardStripeWidth.Tick(timeInterval.Delta);
				ensureHazardStripeUpdate = true;
			}
			if (this._hazardStripePermanenceOpacityFactor.IsActive)
			{
				this._hazardStripePermanenceOpacityFactor.Tick(timeInterval.Delta);
			}
			if (this._hazardStripeWidth.Value > 0f || ensureHazardStripeUpdate)
			{
				if (this._rebuildHazardStripes)
				{
					this.RebuildHazardStripes();
				}
				this.UpdateHazardStripesShaderParameters();
			}
			this._motorwayMeshRenderer.SetPropertyBlock(this._materialPropertyBlock);
			if (this._clientMotorway.State == RoadState.None && this._model == null)
			{
				MotorwayView.Log.Info("Closing MotorwayView {0}.", new object[]
				{
					this._clientMotorway.Id
				});
				return TickResult.Destroy;
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x060029E2 RID: 10722 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x060029E3 RID: 10723 RVA: 0x000B487E File Offset: 0x000B2A7E
		private void TransitionTo(RoadState toState)
		{
			if (toState == RoadState.Mothballed || this._visualRoadState == RoadState.Mothballed)
			{
				this._tilemap.RecalculateDefaultMotorwaySortOrder();
				this._tilemap.ResortMotorwaysOnNextTick();
			}
			this._visualRoadState = toState;
			this.RecalculateHazardStripeVisibility();
		}

		// Token: 0x060029E4 RID: 10724 RVA: 0x000B48B4 File Offset: 0x000B2AB4
		private void RecalculateHazardStripeVisibility()
		{
			bool isMotorwayDrying = this.City.Rules.RoadsBecomePermanentOverTime && !this._clientMotorway.IsPermanent;
			bool shouldHazardStripesBeVisible = false;
			bool shouldHazardStripesAnimate = true;
			bool shouldHazardStripesBeOpaque = true;
			RoadState visualRoadState = this._visualRoadState;
			if (visualRoadState != RoadState.Planned)
			{
				if (visualRoadState != RoadState.Active)
				{
					if (visualRoadState == RoadState.Mothballed)
					{
						shouldHazardStripesBeVisible = true;
					}
				}
				else
				{
					shouldHazardStripesBeVisible = isMotorwayDrying;
					shouldHazardStripesBeOpaque = !isMotorwayDrying;
				}
			}
			else
			{
				shouldHazardStripesBeVisible = true;
				shouldHazardStripesAnimate = false;
			}
			bool areHazardStripesVisible = this._hazardStripeWidth.End > 0f;
			if (areHazardStripesVisible != shouldHazardStripesBeVisible)
			{
				float completionPercent = Mathf.Clamp01(this._hazardStripeWidth.Value / this._visualParameters.maxHazardStripeWidth);
				if (shouldHazardStripesAnimate && !this._viewClient.OnFirstFrame)
				{
					if (shouldHazardStripesBeVisible)
					{
						this._hazardStripeWidth.Start(this._hazardStripeWidth.Value, this._visualParameters.maxHazardStripeWidth, (1f - completionPercent) * this._visualParameters.hazardStripeInDuration, this._visualParameters.hazardStripeAnimationFunction, 0f);
					}
					else
					{
						this._hazardStripeWidth.Start(this._hazardStripeWidth.Value, 0f, completionPercent * this._visualParameters.hazardStripeOutDuration, this._visualParameters.hazardStripeAnimationFunction, 0f);
					}
				}
				else
				{
					this._hazardStripeWidth.Set(shouldHazardStripesBeVisible ? this._visualParameters.maxHazardStripeWidth : 0f, 0f);
				}
			}
			if ((!this.City.Rules.RoadsBecomePermanentOverTime || this._hazardStripePermanenceOpacityFactor.End <= 0f) != shouldHazardStripesBeOpaque)
			{
				float targetFactor = (float)(shouldHazardStripesBeOpaque ? 0 : 1);
				if (!areHazardStripesVisible || this._viewClient.OnFirstFrame)
				{
					this._hazardStripePermanenceOpacityFactor.Set(targetFactor, 0f);
					return;
				}
				this._hazardStripePermanenceOpacityFactor.Start(this._hazardStripePermanenceOpacityFactor.Value, targetFactor, this._visualParameters.hazardStripeOpacityFactorFadeDuration, Easings.Functions.SineEaseInOut, 0f);
			}
		}

		// Token: 0x060029E5 RID: 10725 RVA: 0x000B4A94 File Offset: 0x000B2C94
		private void ImmediatelyTransitionVisualRoadStateTo(RoadState roadState)
		{
			if (roadState == RoadState.Planned || roadState == RoadState.Mothballed)
			{
				this._hazardStripeWidth.Set(this._visualParameters.maxHazardStripeWidth, 0f);
			}
			else if (roadState == RoadState.Active || roadState == RoadState.None)
			{
				this._hazardStripeWidth.Set(0f, 0f);
			}
			this._visualRoadState = roadState;
		}

		// Token: 0x060029E6 RID: 10726 RVA: 0x000B4AEC File Offset: 0x000B2CEC
		public void OnMotorwayChanged(Motorway motorway, Motorway.ChangeFlags changes)
		{
			if (motorway == this._model)
			{
				if (!this._rebuildMotorway)
				{
					MotorwayView.Log.Info("Simulation-side version of motorway {0} changed, client version scheduled for rebuild.", new object[]
					{
						this._model.Id
					});
					this._rebuildMotorway = true;
				}
				int clientTileIndex = 0;
				while (clientTileIndex < this._clientTileEdits.Count)
				{
					if (this._clientTileEdits[clientTileIndex].isScheduledOnSimulation)
					{
						this._clientUpgradeDatabase.RemoveTileEdit(this._clientTileEdits[clientTileIndex]);
						this._clientTileEdits.RemoveAt(clientTileIndex);
					}
					else
					{
						clientTileIndex++;
					}
				}
				if (motorway.State == RoadState.Active && this._replacedMotorway != null)
				{
					this._replacedMotorway.Unsubscribe(this);
					this._replacedMotorway = null;
				}
				if (motorway.State == RoadState.None)
				{
					this.RebuildMotorway();
					this._model = null;
				}
			}
			if (motorway == this._clientMotorway)
			{
				if (changes.HasFlag(Motorway.ChangeFlags.State))
				{
					this._rebuildGeometry = true;
				}
				if ((changes & (Motorway.ChangeFlags.StartTile | Motorway.ChangeFlags.EndTile)) != (Motorway.ChangeFlags)0)
				{
					this._rebuildGeometry = true;
					this._rebuildSplines = true;
				}
				if (changes.HasFlag(Motorway.ChangeFlags.Permanence))
				{
					this._reapplyPermanence = true;
				}
			}
			if (motorway == this._replacedMotorway && changes.HasFlag(Motorway.ChangeFlags.State) && motorway.State == RoadState.None)
			{
				this._replacedMotorway.Unsubscribe(this);
				this._replacedMotorway = null;
			}
		}

		// Token: 0x060029E7 RID: 10727 RVA: 0x000B4C50 File Offset: 0x000B2E50
		public void OnTileViewChanged(TileView changedTileView)
		{
			if (this._startTileView != null && this._endTileView != null)
			{
				if (changedTileView.Tile == this._startTileView.Tile)
				{
					this._startInteractionCirclePositionTween.Start(this._interactionCircleViewStart.transform.position, TilemapView.GetWorldPositionForCoordinates(this._visualStartCoordinates) + this._startTileView.InteractionCircleOffset, this._visualConstants.InteractionCircleOffsetAdjustmentDuration, this._visualConstants.InteractionCircleAndTrafficLightAdjustmentEasingFunction, 0f);
					return;
				}
				if (changedTileView.Tile == this._endTileView.Tile)
				{
					this._endInteractionCirclePositionTween.Start(this._interactionCircleViewEnd.transform.position, TilemapView.GetWorldPositionForCoordinates(this._visualEndCoordinates) + this._endTileView.InteractionCircleOffset, this._visualConstants.InteractionCircleOffsetAdjustmentDuration, this._visualConstants.InteractionCircleAndTrafficLightAdjustmentEasingFunction, 0f);
					return;
				}
				Diagnostics.FailAssert("MotorwayView.OnTileViewChanged called for tile {0} which is neither the start tile {1} or the end tile {2}", new object[]
				{
					changedTileView,
					this._startTileView.Tile,
					this._endTileView.Tile
				});
			}
		}

		// Token: 0x060029E8 RID: 10728 RVA: 0x000B4D84 File Offset: 0x000B2F84
		private void SetPermanenceProgress(Fix64 modelProgress)
		{
			float interactionCircleProgress = this._visualConstants.DryingInteractionCircleFalloff.Evaluate((float)modelProgress);
			this._interactionCircleViewStart.SetPermanenceProgress(interactionCircleProgress);
			this._interactionCircleViewEnd.SetPermanenceProgress(interactionCircleProgress);
			this.UpdateRoadColorShaderParameter();
		}

		// Token: 0x060029E9 RID: 10729 RVA: 0x000B4DC7 File Offset: 0x000B2FC7
		public void ReconfigurePermanenceVisibility()
		{
			this.SetPermanenceProgress(Fix64.Zero);
			this.RecalculateHazardStripeVisibility();
		}

		// Token: 0x060029EA RID: 10730 RVA: 0x000B4DDC File Offset: 0x000B2FDC
		public void OnCreatedInScope(IScope scope)
		{
			if (MotorwayView.motorwayMesh == null)
			{
				MotorwayView.ConstructMotorwayMesh(this._visualParameters.splineSegmentCount);
			}
			MeshFilter motorwayMeshFilter = this._motorwayMeshRenderer.GetComponent<MeshFilter>();
			if (Diagnostics.Verify(motorwayMeshFilter != null, "Could not find MeshFilter component for motorway"))
			{
				motorwayMeshFilter.sharedMesh = MotorwayView.motorwayMesh;
			}
			MeshFilter shadowMeshFilter = this._shadowMeshRenderer.GetComponent<MeshFilter>();
			if (Diagnostics.Verify(shadowMeshFilter != null, "Could not MeshFilter component for shadow motorway shadow."))
			{
				shadowMeshFilter.sharedMesh = MotorwayView.motorwayMesh;
			}
			this._materialPropertyBlock = new MaterialPropertyBlock();
			this._shadowMaterialPropertyBlock = new MaterialPropertyBlock();
			this._spline = new MotorwaySpline();
		}

		// Token: 0x060029EB RID: 10731 RVA: 0x000B4E7C File Offset: 0x000B307C
		private void BringToTop()
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.BringMotorwaysToTopWhenEdited))
			{
				string channel = "MotorwayView";
				string message = "Bring to top {0}";
				object[] array = new object[1];
				int num = 0;
				Motorway motorway = this.Motorway;
				array[num] = ((motorway != null) ? new int?(motorway.Number) : null);
				Diagnostics.Log.Info(channel, message, array);
				MotorwaySpline spline = this._spline;
				if (((spline != null) ? spline.spline : null) == null)
				{
					return;
				}
				this._depthBufferData[0] = 0f;
				this._depthBufferData[1] = 100000000f;
				this._depthBufferData[2] = -6.1f;
				this._materialPropertyBlock.SetFloatArray(ShaderConstants.DepthSegmentBuffer, this._depthBufferData);
				this._materialPropertyBlock.SetInt(ShaderConstants.DepthSegmentBufferLength, 3);
				this._isMotorwayOnTop = true;
			}
		}

		// Token: 0x060029EC RID: 10732 RVA: 0x000B4F3C File Offset: 0x000B313C
		public void SetMotorwayDepth(MotorwaySorter.MotorwayDepth motorwayDepth)
		{
			if (this._isMotorwayOnTop)
			{
				return;
			}
			List<float> depthSegmentBuffer = new List<float>();
			float previousEndDistance = 0f;
			foreach (MotorwaySorter.MotorwayDepthSegment depthSegment in motorwayDepth.DepthSegments)
			{
				depthSegmentBuffer.Add(previousEndDistance);
				depthSegmentBuffer.Add(depthSegment.endDistance);
				depthSegmentBuffer.Add(depthSegment.depth);
				previousEndDistance = depthSegment.endDistance;
			}
			float splineLength = this._spline.spline.Length();
			if (motorwayDepth.DepthSegments.Count == 0)
			{
				depthSegmentBuffer.Add(0f);
				depthSegmentBuffer.Add(splineLength);
				float defaultHeightForMotorway = this.GetWorldHeightForMotorway(this.Motorway);
				depthSegmentBuffer.Add(defaultHeightForMotorway);
			}
			else
			{
				depthSegmentBuffer.Add(previousEndDistance);
				depthSegmentBuffer.Add(splineLength);
				depthSegmentBuffer.Add(motorwayDepth.DepthSegments[motorwayDepth.DepthSegments.Count - 1].depth);
			}
			for (int depthSegmentBufferIndex = 0; depthSegmentBufferIndex < depthSegmentBuffer.Count; depthSegmentBufferIndex++)
			{
				this._depthBufferData[depthSegmentBufferIndex] = depthSegmentBuffer[depthSegmentBufferIndex];
			}
			this._materialPropertyBlock.SetFloatArray(ShaderConstants.DepthSegmentBuffer, this._depthBufferData);
			this._materialPropertyBlock.SetInt(ShaderConstants.DepthSegmentBufferLength, depthSegmentBuffer.Count);
		}

		// Token: 0x060029ED RID: 10733 RVA: 0x000B508C File Offset: 0x000B328C
		private float GetWorldHeightForMotorway(Motorway motorway)
		{
			return -3f + -1.1999998f * ((float)this._tilemap.GetDefaultSortOrderForMotorway(motorway) / (float)this._tilemap.MotorwayCount);
		}

		// Token: 0x060029EE RID: 10734 RVA: 0x000B50B4 File Offset: 0x000B32B4
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._clientMotorway != null)
			{
				this._clientMotorway.Unsubscribe(this);
				this._clientMotorway = null;
			}
			if (this._replacedMotorway != null)
			{
				this._replacedMotorway.Unsubscribe(this);
				this._replacedMotorway = null;
			}
			if (this._model != null)
			{
				this._model.Unsubscribe(this);
				this._model = null;
			}
			if (this._startTileView != null)
			{
				this._startTileView.Unsubscribe(this);
			}
			if (this._endTileView != null)
			{
				this._endTileView.Unsubscribe(this);
			}
		}

		// Token: 0x060029EF RID: 10735 RVA: 0x000B514C File Offset: 0x000B334C
		public void Reset()
		{
			this._clientTileEdits.Clear();
			this._rebuildMotorway = true;
			this._rebuildSplines = true;
			this._rebuildGeometry = true;
			this._reapplyPermanence = true;
			this._rebuildHazardStripes = true;
			this._clientMotorway = null;
			this._model = null;
			this._naturalMidpoint = default(Vector2);
			this._naturalTangent = default(Vector2);
			this._naturalStartHandleLength = 0f;
			this._naturalEndHandleLength = 0f;
			this._handleDistanceFromMidpoint.Reset();
			this._handleDirectionFromMidpoint = default(Vector2);
			this._splineMidpoint = default(Vector2);
			this._referenceMidpointAngle = 0f;
			this._referenceMidpointDistance = 0f;
			this._referenceMotorwayDirection = default(Vector2);
			this._hasCheckedReferenceMotorwayDirection = false;
			this._shadowOffset = 0f;
			this._startToEndPath = null;
			this._startToEndPathLength = 0f;
			this._endToStartPath = null;
			this._endToStartPathLength = 0f;
			this._visualRoadState = RoadState.None;
			this._hazardStripeWidth.Reset();
			this._hazardStripePermanenceOpacityFactor.Reset();
			this._visualStartCoordinates = default(Vector2Int);
			this._visualEndCoordinates = default(Vector2Int);
			this._visualSplineMidpoint = default(Vector2);
			this._motorwayViewColors = default(MotorwayView.MotorwayViewColors);
			this._depthBufferData = new float[60];
			this._isBeingEdited = false;
			this._isMotorwayOnTop = false;
			this._resortMotorwaysWhenSpringingIsComplete = false;
			this._startInteractionCirclePositionTween.Reset();
			this._endInteractionCirclePositionTween.Reset();
			this._materialPropertyBlock.Clear();
			this._visualParameters.OnParameterChanged -= this.OnVisualParameterChanged;
			this._shadowMeshRenderer.enabled = false;
			this._replacedMotorway = null;
		}

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x060029F0 RID: 10736 RVA: 0x000B52F8 File Offset: 0x000B34F8
		private Vector2Int StartCoordinates
		{
			get
			{
				return this._clientMotorway.StartCoordinates;
			}
		}

		// Token: 0x1700071C RID: 1820
		// (get) Token: 0x060029F1 RID: 10737 RVA: 0x000B5305 File Offset: 0x000B3505
		private TileDirection StartDirection
		{
			get
			{
				return this._clientMotorway.StartDirection;
			}
		}

		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x060029F2 RID: 10738 RVA: 0x000B5312 File Offset: 0x000B3512
		private Vector2Int EndCoordinates
		{
			get
			{
				return this._clientMotorway.EndCoordinates;
			}
		}

		// Token: 0x1700071E RID: 1822
		// (get) Token: 0x060029F3 RID: 10739 RVA: 0x000B531F File Offset: 0x000B351F
		private TileDirection EndDirection
		{
			get
			{
				return this._clientMotorway.EndDirection;
			}
		}

		// Token: 0x060029F4 RID: 10740 RVA: 0x000B532C File Offset: 0x000B352C
		private void RebuildMotorway()
		{
			this._rebuildMotorway = false;
			if (this._model != null)
			{
				this._model.CloneInto(this._clientMotorway);
			}
			else
			{
				this._clientMotorway.Clear();
			}
			foreach (ClientTileEdit clientTileEdit in this._clientTileEdits)
			{
				clientTileEdit.edit.ApplyToAffectedMotorway(this._clientMotorway);
			}
			if (this._rebuildSplines)
			{
				Vector2 startTilePosition = TilemapView.GetWorldPositionForCoordinates(this.StartCoordinates);
				Vector2 startDirection = TileUtilities.GetVectorForDirection(this.StartDirection);
				Vector2 endTilePosition = TilemapView.GetWorldPositionForCoordinates(this.EndCoordinates);
				Vector2 endDirection = TileUtilities.GetVectorForDirection(this.EndDirection);
				Vector2 startToEndDirection = endTilePosition - startTilePosition;
				float startToEndDistance = startToEndDirection.magnitude;
				startToEndDirection /= startToEndDistance;
				Vector3 motorwayCenter = (endTilePosition - startTilePosition) * 0.5f + startTilePosition;
				this._naturalMidpoint = motorwayCenter;
				this._naturalTangent = startToEndDirection;
				float startDot = Vector2.Dot(startToEndDirection, startDirection);
				float endDot = Vector2.Dot(-startToEndDirection, endDirection);
				if (startDot >= 0f && endDot >= 0f)
				{
					float tileOffset = (float)TilemapModel.HalfTileWidth;
					float startHandleLength = Mathf.Lerp(0.3f, 1f, startDot) * this.SplineHandleLengthFactor * startToEndDistance;
					float endHandleLength = Mathf.Lerp(0.3f, 1f, endDot) * this.SplineHandleLengthFactor * startToEndDistance;
					Spline.RasterizedSpline rasterizedEstablishingCurve = new Spline.BezierSpline(startTilePosition + startDirection * tileOffset, startTilePosition + startDirection * (tileOffset + startHandleLength), endTilePosition + endDirection * (tileOffset + endHandleLength), endTilePosition + endDirection * tileOffset).Rasterize(this.SplineResolution);
					List<LineSegment> establishingCurveSegments = new List<LineSegment>(rasterizedEstablishingCurve.Resolution - 1);
					for (int pointIndex = 0; pointIndex < rasterizedEstablishingCurve.Resolution - 1; pointIndex++)
					{
						establishingCurveSegments.Add(new LineSegment(rasterizedEstablishingCurve.Positions[pointIndex], rasterizedEstablishingCurve.Positions[pointIndex + 1]));
					}
					float curveLength = 0f;
					foreach (LineSegment curveSegment in establishingCurveSegments)
					{
						curveLength += curveSegment.Length;
					}
					float distanceToHalfway = curveLength * 0.5f;
					for (int curveSegmentIndex = 0; curveSegmentIndex < establishingCurveSegments.Count; curveSegmentIndex++)
					{
						LineSegment curveSegment2 = establishingCurveSegments[curveSegmentIndex];
						if (distanceToHalfway < curveSegment2.Length)
						{
							float t = distanceToHalfway / curveSegment2.Length;
							this._naturalMidpoint = curveSegment2.GetPosition(t);
							Vector2 tangent = curveSegment2.Direction;
							Vector2 precedingTangent = (curveSegmentIndex <= 0) ? tangent : establishingCurveSegments[curveSegmentIndex - 1].Direction;
							Vector2 followingTangent = (curveSegmentIndex >= establishingCurveSegments.Count - 1) ? tangent : establishingCurveSegments[curveSegmentIndex + 1].Direction;
							this._naturalTangent = Vector2.Lerp((precedingTangent + tangent) * 0.5f, (tangent + followingTangent) * 0.5f, t);
							break;
						}
						distanceToHalfway -= curveSegment2.Length;
					}
					this._naturalStartHandleLength = startHandleLength;
					this._naturalEndHandleLength = endHandleLength;
				}
				float tolerance = startToEndDistance * this.HandleToleranceFactor;
				this._handleDistanceFromMidpoint.Min = -tolerance;
				this._handleDistanceFromMidpoint.Max = tolerance;
				this._handleDistanceFromMidpoint.Hold();
				if (this._referenceMidpointDistance > 0f)
				{
					if (!this._hasCheckedReferenceMotorwayDirection)
					{
						this._hasCheckedReferenceMotorwayDirection = true;
						if (Vector2.Dot(this._referenceMotorwayDirection, startToEndDirection) < 0f)
						{
							if (this._referenceMidpointAngle < 0f)
							{
								this._referenceMidpointAngle += 3.1415927f;
							}
							else
							{
								this._referenceMidpointAngle -= 3.1415927f;
							}
						}
					}
					Vector2 midpointOffset = startToEndDirection.Rotated(-this._referenceMidpointAngle);
					this._handleDirectionFromMidpoint = midpointOffset;
					float midpointDisplacement = Mathf.Clamp(this._referenceMidpointDistance * startToEndDistance, 0f, tolerance);
					this._handleDistanceFromMidpoint.RawValue = midpointDisplacement;
					this._splineMidpoint = this._naturalMidpoint + midpointOffset * midpointDisplacement;
				}
				else
				{
					this._splineMidpoint = this._naturalMidpoint;
					this._handleDistanceFromMidpoint.RawValue = 0f;
				}
				this._shadowOffset = Mathf.Min(startToEndDistance * this.ShadowOffsetFactor, this.MaxShadowOffset);
				this._rebuildSplines = false;
			}
		}

		// Token: 0x060029F5 RID: 10741 RVA: 0x000B57DC File Offset: 0x000B39DC
		private void UpdateShaderParametersAndKeywords()
		{
			this.UpdateMotorwayOpacity();
			this._materialPropertyBlock.SetFloat(ShaderConstants.MinMotorwayWorldHeight, -3f);
			if (this._clientMotorway != null)
			{
				this._materialPropertyBlock.SetInt(ShaderConstants.MotorwayId, this._clientMotorway.Id);
			}
			this._materialPropertyBlock.SetFloat(ShaderConstants.RoadWidth, this._visualParameters.roadWidth);
			this._materialPropertyBlock.SetFloat(ShaderConstants.RoadOutlineWidth, this._visualParameters.roadOutlineWidth);
			this._materialPropertyBlock.SetFloat(ShaderConstants.BlendingSize, this._visualParameters.blendingSize);
			this._materialPropertyBlock.SetFloat(ShaderConstants.HazardFadeoutOffset, this._visualParameters.hazardStripeFadeoutOffset);
			this._materialPropertyBlock.SetFloat(ShaderConstants.HazardFadeoutDistance, this._visualParameters.hazardFadeoutDistance);
			this._materialPropertyBlock.SetFloat(ShaderConstants.FadeoutDistance, this._visualParameters.splineEndFadeoutDistance);
			this.UpdateHazardStripesShaderParameters();
			this.UpdateRoadColorShaderParameter();
			if (!this.IsMotorwayMothballed)
			{
				this._shadowMaterialPropertyBlock.SetFloat(ShaderConstants.RoadWidth, this._visualParameters.roadWidth);
				this._shadowMaterialPropertyBlock.SetFloat(ShaderConstants.RoadOutlineWidth, this._visualParameters.roadOutlineWidth);
				this._shadowMaterialPropertyBlock.SetFloat(ShaderConstants.BlendingSize, this._visualParameters.blendingSize);
			}
		}

		// Token: 0x060029F6 RID: 10742 RVA: 0x000B5930 File Offset: 0x000B3B30
		private void UpdateRoadColorShaderParameter()
		{
			if (this.City.Rules.RoadsBecomePermanentOverTime && (this._visualRoadState == RoadState.Active || this._visualRoadState == RoadState.Planned))
			{
				float permanenceProgress = (float)this._clientMotorway.PermanenceProgress;
				float visualPermanenceProgress = this._visualConstants.DryingRoadFalloff.Evaluate(permanenceProgress);
				Color endColor = Color.Lerp(this._motorwayViewColors.roadInnerNonPermanent, this._motorwayViewColors.roadInner, visualPermanenceProgress);
				this._materialPropertyBlock.SetColor(ShaderConstants.RoadColor, endColor);
				return;
			}
			this._materialPropertyBlock.SetColor(ShaderConstants.RoadColor, (this._visualRoadState == RoadState.Mothballed) ? this._motorwayViewColors.mothballedRoad : this._motorwayViewColors.roadInner);
		}

		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x060029F7 RID: 10743 RVA: 0x000B59E6 File Offset: 0x000B3BE6
		private bool IsMotorwayMothballed
		{
			get
			{
				return this._visualRoadState == RoadState.Mothballed || this._visualRoadState == RoadState.None;
			}
		}

		// Token: 0x060029F8 RID: 10744 RVA: 0x000B5A00 File Offset: 0x000B3C00
		public void UpdateMotorwayOpacity()
		{
			float motorwayInnerOpacity = this.IsMotorwayMothballed ? this._visualParameters.mothballedOpacity : 1f;
			this._materialPropertyBlock.SetFloat(ShaderConstants.MotorwayInnerOpacity, motorwayInnerOpacity * this._tilemap.ViewModeOpacity);
			this._materialPropertyBlock.SetFloat(ShaderConstants.MotorwayOuterOpacity, this._tilemap.ViewModeOpacity);
		}

		// Token: 0x060029F9 RID: 10745 RVA: 0x000B5A60 File Offset: 0x000B3C60
		private void UpdateHazardStripesShaderParameters()
		{
			float hazardStripeWidth = this._hazardStripeWidth.Value;
			this._materialPropertyBlock.SetFloat(ShaderConstants.HazardStripeWidth, hazardStripeWidth);
			this._materialPropertyBlock.SetFloat(ShaderConstants.HalfHazardStripeWidth, hazardStripeWidth * 0.5f);
			this._materialPropertyBlock.SetFloat(ShaderConstants.DistanceBetweenHazardStripes, this._visualParameters.splineDistanceBetweenStripes);
			if (this.City.Rules.RoadsBecomePermanentOverTime)
			{
				float permanenceProgress = this._visualConstants.DryingMotorwayHazardStripesFalloff.Evaluate((float)this._clientMotorway.PermanenceProgress);
				this._materialPropertyBlock.SetFloat(ShaderConstants.HazardStripeOpacity, Mathf.Lerp(1f, 1f - permanenceProgress, this._hazardStripePermanenceOpacityFactor.Value));
				return;
			}
			this._materialPropertyBlock.SetFloat(ShaderConstants.HazardStripeOpacity, 1f);
		}

		// Token: 0x060029FA RID: 10746 RVA: 0x000B5B34 File Offset: 0x000B3D34
		public void RebuildMotorwayView()
		{
			if (!this._rebuildGeometry)
			{
				return;
			}
			if (this._clientMotorway == null)
			{
				return;
			}
			if (this._clientMotorway.StartCoordinates == this._clientMotorway.EndCoordinates)
			{
				return;
			}
			if (this._visualRoadState != this._clientMotorway.State)
			{
				this.TransitionTo(this._clientMotorway.State);
			}
			bool shouldRenderMotorway = this._visualRoadState > RoadState.None;
			this._motorwayMeshRenderer.enabled = shouldRenderMotorway;
			bool isMotorwayMothballed = this._visualRoadState == RoadState.Mothballed || this._visualRoadState == RoadState.None;
			this._shadowMeshRenderer.enabled = !isMotorwayMothballed;
			if (this.handleView != null)
			{
				this.handleView.gameObject.SetActive(!isMotorwayMothballed);
			}
			if (this._interactionCircleViewStart != null)
			{
				this._interactionCircleViewStart.gameObject.SetActive(!isMotorwayMothballed);
			}
			if (this._interactionCircleViewEnd != null)
			{
				this._interactionCircleViewEnd.gameObject.SetActive(!isMotorwayMothballed);
			}
			if (!shouldRenderMotorway)
			{
				return;
			}
			if (this._rebuildGeometry)
			{
				this.UpdateShaderParametersAndKeywords();
				this._materialPropertyBlock.SetColor(ShaderConstants.MotorwayColor, this._motorwayViewColors.motorwayInner);
				this._materialPropertyBlock.SetColor(ShaderConstants.OutlineColor, this._motorwayViewColors.roadOutline);
				this._materialPropertyBlock.SetColor(ShaderConstants.MotorwayOutlineColor, this._motorwayViewColors.motorwayOuter);
				this._materialPropertyBlock.SetColor(ShaderConstants.ShadowColor, this._motorwayViewColors.shadow);
				this._shadowMaterialPropertyBlock.SetColor(ShaderConstants.ShadowColor, this._motorwayViewColors.shadow);
				this._motorwayMeshRenderer.sortingOrder = this.GetSortOrderIndexForMotorwayComponent(MotorwayView.MotorwayComponentSortOrder.Road);
				this._shadowFadeouts[0] = 0f;
				this._shadowFadeouts[1] = 0f;
				for (int shadowFadeoutsIndex = 0; shadowFadeoutsIndex < this._visualParameters.shadowFadeouts.Length; shadowFadeoutsIndex++)
				{
					ShadowTypeFadeouts shadowFadeout = this._visualParameters.shadowFadeouts[shadowFadeoutsIndex];
					int shadowFadeoutIndex = 2 * (shadowFadeoutsIndex + 1);
					this._shadowFadeouts[shadowFadeoutIndex] = shadowFadeout.startDistance;
					this._shadowFadeouts[shadowFadeoutIndex + 1] = shadowFadeout.endDistance;
				}
				this._materialPropertyBlock.SetFloatArray(ShaderConstants.ShadowFadeoutBuffer, this._shadowFadeouts);
				bool shouldRebuildPositionDependentData = this._visualStartCoordinates != this._clientMotorway.StartCoordinates || this._visualEndCoordinates != this._clientMotorway.EndCoordinates || this._visualSplineMidpoint != this._splineMidpoint;
				if (this._visualMotorwayModel != this.Model || (this.Model != null && (this._visualStartToEndLane != this.Model.startToEndLane || this._visualEndToStartLane != this.Model.endToStartLane)) || shouldRebuildPositionDependentData)
				{
					this._visualMotorwayModel = this.Model;
					if (this.Model != null)
					{
						this._visualStartToEndLane = this.Model.startToEndLane;
						this._visualEndToStartLane = this.Model.endToStartLane;
					}
					else
					{
						this._visualStartToEndLane = null;
						this._visualEndToStartLane = null;
					}
					this.RebuildLanePaths();
				}
				if (shouldRebuildPositionDependentData)
				{
					this._visualStartCoordinates = this._clientMotorway.StartCoordinates;
					this._visualEndCoordinates = this._clientMotorway.EndCoordinates;
					this._visualSplineMidpoint = this._splineMidpoint;
					Vector4[] splineSegments = this._spline.PackSplineSegments();
					this._materialPropertyBlock.SetVectorArray(ShaderConstants.SplineSegments, splineSegments);
					this._spline.CalculateLinearDistanceLookupTable(this._linearDistanceTable);
					this._materialPropertyBlock.SetFloatArray(ShaderConstants.LinearDistanceTable, this._linearDistanceTable);
					this._rebuildHazardStripes = true;
					if (this.handleView != null)
					{
						this.handleView.SetHandlePosition(this._visualSplineMidpoint);
					}
					if (this._interactionCircleViewStart != null)
					{
						if (this._startTileView != null)
						{
							this._startTileView.Unsubscribe(this);
						}
						this._startTileView = this._tilemap.GetTileView(this._visualStartCoordinates);
						this._startTileView.Subscribe(this);
						this._interactionCircleViewStart.transform.position = TilemapView.GetWorldPositionForCoordinates(this._visualStartCoordinates) + this._startTileView.InteractionCircleOffset;
						this._startInteractionCirclePositionTween.Stop();
					}
					if (this._interactionCircleViewEnd != null)
					{
						if (this._endTileView != null)
						{
							this._endTileView.Unsubscribe(this);
						}
						this._endTileView = this._tilemap.GetTileView(this._visualEndCoordinates);
						this._endTileView.Subscribe(this);
						this._interactionCircleViewEnd.transform.position = TilemapView.GetWorldPositionForCoordinates(this._visualEndCoordinates) + this._endTileView.InteractionCircleOffset;
						this._endInteractionCirclePositionTween.Stop();
					}
					if (!isMotorwayMothballed)
					{
						this._shadowMaterialPropertyBlock.SetVectorArray(ShaderConstants.SplineSegments, this._spline.AddShadowOffsetToSplineSegments(splineSegments, this._shadowOffset));
					}
				}
				this._shadowMeshRenderer.SetPropertyBlock(this._shadowMaterialPropertyBlock);
				this._rebuildGeometry = false;
			}
		}

		// Token: 0x060029FB RID: 10747 RVA: 0x000B6034 File Offset: 0x000B4234
		private void RebuildHazardStripes()
		{
			Vector4[] hazardTapeSamples = this._spline.GenerateHazardTapeStripeSamples(this._visualParameters.splineDistanceBetweenStripes, this._visualParameters.splineStripeRotationDegrees, this._visualParameters.roadWidth, this._visualParameters.maxHazardStripeWidth, 200, true);
			this._materialPropertyBlock.SetVectorArray(ShaderConstants.HazardStripeSamples, hazardTapeSamples);
			this._rebuildHazardStripes = false;
		}

		// Token: 0x060029FC RID: 10748 RVA: 0x000B6097 File Offset: 0x000B4297
		public int GetSortOrderIndexForMotorwayComponent(MotorwayView.MotorwayComponentSortOrder componentSortOrder)
		{
			return (int)componentSortOrder;
		}

		// Token: 0x060029FD RID: 10749 RVA: 0x000B609C File Offset: 0x000B429C
		private Vector3[] RasterizeSplines(MotorwayView.MotorwaySplineType splineGenerationType)
		{
			float distanceFromIntersectionScale = (float)TilemapModel.HalfTileWidth;
			Vector2 startHandleDirection = TileUtilities.GetVectorForDirection(this.StartDirection);
			Vector2 startTilePosition = TilemapView.GetWorldPositionForCoordinates(this.StartCoordinates) + startHandleDirection * distanceFromIntersectionScale;
			Vector2 vector = startTilePosition + startHandleDirection * this.RampExtrusion;
			float proximityTowardsStart = (startTilePosition - this._splineMidpoint).magnitude / (startTilePosition - this._naturalMidpoint).magnitude;
			Vector2 startTileHandlePosition = vector + startHandleDirection * (this._naturalStartHandleLength * 0.5f * proximityTowardsStart);
			Vector2 endHandleDirection = TileUtilities.GetVectorForDirection(this.EndDirection);
			Vector2 endTilePosition = TilemapView.GetWorldPositionForCoordinates(this.EndCoordinates) + endHandleDirection * distanceFromIntersectionScale;
			Vector2 endSplinePosition = endTilePosition + endHandleDirection * this.RampExtrusion;
			float proximityTowardsEnd = (endTilePosition - this._splineMidpoint).magnitude / (endTilePosition - this._naturalMidpoint).magnitude;
			Vector2 endTileHandlePosition = endSplinePosition + endHandleDirection * (this._naturalEndHandleLength * 0.5f * proximityTowardsEnd);
			Vector2 midPointHandleStart = -this._naturalTangent * ((this._naturalStartHandleLength + this._naturalEndHandleLength) * 0.25f);
			Vector2 midPointHandleEnd = -midPointHandleStart;
			Vector2 midPointOffset = (splineGenerationType == MotorwayView.MotorwaySplineType.Shadow) ? new Vector2(this._shadowOffset, -this._shadowOffset) : Vector2.zero;
			List<Vector3> points = new List<Vector3>(new Vector3[this.SplineResolution * 2 + 3]);
			points[0] = startTilePosition;
			points[points.Count - 1] = endTilePosition;
			Spline.BezierSpline spline = new Spline.BezierSpline(vector, startTileHandlePosition, midPointHandleStart + this._splineMidpoint + midPointOffset, this._splineMidpoint + midPointOffset);
			for (int splinePoint = 0; splinePoint <= this.SplineResolution; splinePoint++)
			{
				float splineEvaluationTime = 1f / (float)this.SplineResolution * (float)splinePoint;
				points[splinePoint + 1] = spline.Evaluate(splineEvaluationTime);
			}
			spline = new Spline.BezierSpline(this._splineMidpoint + midPointOffset, this._splineMidpoint + midPointHandleEnd + midPointOffset, endTileHandlePosition, endSplinePosition);
			for (int splinePoint2 = 1; splinePoint2 <= this.SplineResolution; splinePoint2++)
			{
				float splineEvaluationTime2 = 1f / (float)this.SplineResolution * (float)splinePoint2;
				points[this.SplineResolution + 1 + splinePoint2] = spline.Evaluate(splineEvaluationTime2);
			}
			return points.ToArray();
		}

		// Token: 0x060029FE RID: 10750 RVA: 0x000B6338 File Offset: 0x000B4538
		private static void ConstructMotorwayMesh(int segmentCount)
		{
			int num = 2 * (segmentCount + 1);
			int indexCount = 6 * segmentCount;
			Vector3[] vertices = new Vector3[num];
			Vector2[] uvs = new Vector2[num];
			int[] indices = new int[indexCount];
			float tStep = 1f / (float)segmentCount;
			int edgeLoopCount = segmentCount + 1;
			int vertexIndex = 0;
			int triangleIndex = 0;
			for (int edgeLoopIndex = 0; edgeLoopIndex < edgeLoopCount; edgeLoopIndex++)
			{
				float t = (float)edgeLoopIndex * tStep;
				uvs[vertexIndex] = new Vector2(t, 0f);
				uvs[vertexIndex + 1] = new Vector2(t, 1f);
				if (edgeLoopIndex < edgeLoopCount - 1)
				{
					indices[triangleIndex] = vertexIndex;
					indices[triangleIndex + 1] = vertexIndex + 2;
					indices[triangleIndex + 2] = vertexIndex + 1;
					indices[triangleIndex + 3] = vertexIndex + 1;
					indices[triangleIndex + 4] = vertexIndex + 2;
					indices[triangleIndex + 5] = vertexIndex + 3;
					triangleIndex += 6;
				}
				vertexIndex += 2;
			}
			if (MotorwayView.motorwayMesh == null)
			{
				MotorwayView.motorwayMesh = new Mesh();
			}
			MotorwayView.motorwayMesh.vertices = vertices;
			MotorwayView.motorwayMesh.uv = uvs;
			MotorwayView.motorwayMesh.SetTriangles(indices, 0);
			MotorwayView.motorwayMesh.bounds = new Bounds(Vector3.zero, new Vector3(1000f, 1000f));
		}

		// Token: 0x060029FF RID: 10751 RVA: 0x000B6468 File Offset: 0x000B4668
		private void RebuildLanePaths()
		{
			if (this._clientMotorway == null)
			{
				return;
			}
			Vector2 startCoordinatesWorldSpace = TilemapView.GetWorldPositionForCoordinates(this._clientMotorway.StartCoordinates);
			Vector2 endCoordinatesWorldSpace = TilemapView.GetWorldPositionForCoordinates(this._clientMotorway.EndCoordinates);
			this._spline.RebuildSegments(this._clientMotorway.StartDirection, this._clientMotorway.EndDirection, startCoordinatesWorldSpace, endCoordinatesWorldSpace, this._splineMidpoint, this._naturalMidpoint, this._naturalTangent, this._naturalStartHandleLength, this._naturalEndHandleLength);
			Spline.RasterizedSpline rasterizedSpline = this._spline.spline.RasterizeWithTangents(20);
			Spline.RasterizedSpline startToEndSpline = rasterizedSpline.Offset((float)RoadTileAtlas.LaneOffsetScale);
			Spline.RasterizedSpline endToStartSpline = rasterizedSpline.Offset((float)(-RoadTileAtlas.LaneOffsetScale));
			this._startToEndPath = startToEndSpline.Positions;
			this._endToStartPath = endToStartSpline.Positions;
			this._endToStartPath.Reverse();
			if (!this.skipModelPoints && this.Model != null && this.Model.startToEndLane != null && this.Model.endToStartLane != null)
			{
				LaneModel startToEndLane = this._model.startToEndLane;
				LaneModel endToStartLane = this._model.endToStartLane;
				if (TileUtilities.IsDirectionDiagonal(this.StartDirection))
				{
					if (startToEndLane.InboundLanes.Count >= 1)
					{
						LaneModel inboundLane = startToEndLane.InboundLanes[0];
						if (inboundLane.lanePoints.Count >= 1)
						{
							Vector2Fixed lastLanePoint = inboundLane.lanePoints[inboundLane.lanePoints.Count - 1];
							this._startToEndPath.Insert(0, new Vector2((float)lastLanePoint.x, (float)lastLanePoint.y));
						}
					}
					if (endToStartLane.OutboundLanes.Count >= 1)
					{
						LaneModel outboundLane = endToStartLane.OutboundLanes[0];
						if (outboundLane.lanePoints.Count >= 1)
						{
							Vector2Fixed firstLanePoint = outboundLane.lanePoints[0];
							this._endToStartPath.Add(new Vector2((float)firstLanePoint.x, (float)firstLanePoint.y));
						}
					}
				}
				if (TileUtilities.IsDirectionDiagonal(this.EndDirection))
				{
					if (startToEndLane.OutboundLanes.Count >= 1)
					{
						LaneModel outboundLane2 = startToEndLane.OutboundLanes[0];
						if (outboundLane2.lanePoints.Count >= 1)
						{
							Vector2Fixed firstLanePoint2 = outboundLane2.lanePoints[0];
							this._startToEndPath.Add(new Vector2((float)firstLanePoint2.x, (float)firstLanePoint2.y));
						}
					}
					if (endToStartLane.InboundLanes.Count >= 1)
					{
						LaneModel inboundLane2 = endToStartLane.InboundLanes[0];
						if (inboundLane2.lanePoints.Count >= 1)
						{
							Vector2Fixed lastLanePoint2 = inboundLane2.lanePoints[inboundLane2.lanePoints.Count - 1];
							this._endToStartPath.Insert(0, new Vector2((float)lastLanePoint2.x, (float)lastLanePoint2.y));
						}
					}
				}
			}
			this._startToEndPathLength = 0f;
			for (int pathIndex = 0; pathIndex < this._startToEndPath.Count - 1; pathIndex++)
			{
				this._startToEndPathLength += (this._startToEndPath[pathIndex + 1] - this._startToEndPath[pathIndex]).magnitude;
			}
			this._endToStartPathLength = 0f;
			for (int pathIndex2 = 0; pathIndex2 < this._endToStartPath.Count - 1; pathIndex2++)
			{
				this._endToStartPathLength += (this._endToStartPath[pathIndex2 + 1] - this._endToStartPath[pathIndex2]).magnitude;
			}
		}

		// Token: 0x06002A00 RID: 10752 RVA: 0x000B6828 File Offset: 0x000B4A28
		public float GetLaneLength(LaneModel motorwayLane)
		{
			if (this._rebuildMotorway)
			{
				this.RebuildMotorway();
			}
			this.RebuildMotorwayView();
			if (motorwayLane == this._model.startToEndLane)
			{
				return this._startToEndPathLength;
			}
			if (motorwayLane == this._model.endToStartLane)
			{
				return this._endToStartPathLength;
			}
			Diagnostics.FailAssert("Called MotorwayView.GetLaneLength with invalid lane {0}.", new object[]
			{
				motorwayLane
			});
			return -1f;
		}

		// Token: 0x06002A01 RID: 10753 RVA: 0x000B688C File Offset: 0x000B4A8C
		public List<Vector2> GetLanePoints(LaneModel motorwayLane)
		{
			if (this._rebuildMotorway)
			{
				this.RebuildMotorway();
			}
			this.RebuildMotorwayView();
			if (motorwayLane == this._model.startToEndLane)
			{
				return this._startToEndPath;
			}
			if (motorwayLane == this._model.endToStartLane)
			{
				return this._endToStartPath;
			}
			Diagnostics.FailAssert("Called MotorwayView.GetLanePoints with invalid lane {0}.", new object[]
			{
				motorwayLane
			});
			return null;
		}

		// Token: 0x06002A02 RID: 10754 RVA: 0x000B68EC File Offset: 0x000B4AEC
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
			this._interactionCircleViewStart.InitializeTheme(themeDatabase);
			this._interactionCircleViewEnd.InitializeTheme(themeDatabase);
		}

		// Token: 0x06002A03 RID: 10755 RVA: 0x000B6908 File Offset: 0x000B4B08
		public void ApplyTheme(ITheme theme)
		{
			Theme motorwaysTheme = (Theme)theme;
			this._motorwayViewColors.mothballedRoad = motorwaysTheme.GetColor(ThemedMaterialType.RoadMothballed, "_Color");
			this._motorwayViewColors.roadInner = motorwaysTheme.GetColor(ThemedMaterialType.RoadInner, "_Color");
			this._motorwayViewColors.roadInnerNonPermanent = motorwaysTheme.GetColor(ThemedMaterialType.RoadInner, "_DryingColor");
			this._motorwayViewColors.roadOutline = motorwaysTheme.GetColor(ThemedMaterialType.RoadOutline, "_Color");
			this._motorwayViewColors.motorwayInner = motorwaysTheme.GetColor(ThemedMaterialType.MotorwayInner, "_Color");
			this._motorwayViewColors.motorwayOuter = motorwaysTheme.GetColor(ThemedMaterialType.MotorwayOutline, "_Color");
			this._motorwayViewColors.shadow = motorwaysTheme.GetColor(ThemedMaterialType.Shadow, "_Color");
			this.handleView.ApplyTheme(theme);
			this._rebuildGeometry = true;
			this._reapplyPermanence = true;
			this.RebuildMotorwayView();
			this._interactionCircleViewStart.ApplyTheme(theme);
			this._interactionCircleViewEnd.ApplyTheme(theme);
		}

		// Token: 0x06002A04 RID: 10756 RVA: 0x000B69FC File Offset: 0x000B4BFC
		private Color GetBlendedColor(ThemedMaterialType themedMaterialType, Theme oldTheme, Theme newTheme, float progress, string property = "_Color")
		{
			return Color.LerpUnclamped(oldTheme.GetColor(themedMaterialType, property), newTheme.GetColor(themedMaterialType, property), progress);
		}

		// Token: 0x06002A05 RID: 10757 RVA: 0x000B6A18 File Offset: 0x000B4C18
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			Theme motorwaysThemeOld = (Theme)oldTheme;
			Theme motorwaysThemeNew = (Theme)newTheme;
			this._motorwayViewColors.mothballedRoad = this.GetBlendedColor(ThemedMaterialType.RoadMothballed, motorwaysThemeOld, motorwaysThemeNew, progress, "_Color");
			this._motorwayViewColors.roadInner = this.GetBlendedColor(ThemedMaterialType.RoadInner, motorwaysThemeOld, motorwaysThemeNew, progress, "_Color");
			this._motorwayViewColors.roadInnerNonPermanent = this.GetBlendedColor(ThemedMaterialType.RoadInner, motorwaysThemeOld, motorwaysThemeNew, progress, "_DryingColor");
			this._motorwayViewColors.roadOutline = this.GetBlendedColor(ThemedMaterialType.RoadOutline, motorwaysThemeOld, motorwaysThemeNew, progress, "_Color");
			this._motorwayViewColors.motorwayInner = this.GetBlendedColor(ThemedMaterialType.MotorwayInner, motorwaysThemeOld, motorwaysThemeNew, progress, "_Color");
			this._motorwayViewColors.motorwayOuter = this.GetBlendedColor(ThemedMaterialType.MotorwayOutline, motorwaysThemeOld, motorwaysThemeNew, progress, "_Color");
			this._motorwayViewColors.shadow = this.GetBlendedColor(ThemedMaterialType.Shadow, motorwaysThemeOld, motorwaysThemeNew, progress, "_Color");
			this.handleView.ApplyTheme(motorwaysThemeNew);
			this._rebuildGeometry = true;
			this._reapplyPermanence = true;
			this.RebuildMotorwayView();
			this._motorwayMeshRenderer.SetPropertyBlock(this._materialPropertyBlock);
			this._interactionCircleViewStart.ApplyBlendedTheme(oldTheme, newTheme, progress);
			this._interactionCircleViewEnd.ApplyBlendedTheme(oldTheme, newTheme, progress);
			return ThemeBlendingResult.ContinueBlending;
		}

		// Token: 0x06002A06 RID: 10758 RVA: 0x000B6B40 File Offset: 0x000B4D40
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
			this._interactionCircleViewStart.ReleaseTheme(themeDatabase);
			this._interactionCircleViewEnd.ReleaseTheme(themeDatabase);
		}

		// Token: 0x0400238E RID: 9102
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("View.Motorway");

		// Token: 0x0400238F RID: 9103
		[Dependency]
		private IScope _scope;

		// Token: 0x04002390 RID: 9104
		[Dependency]
		private MotorwayVisualParameters _visualParameters;

		// Token: 0x04002392 RID: 9106
		[Dependency]
		private TilemapView _tilemap;

		// Token: 0x04002393 RID: 9107
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x04002394 RID: 9108
		[Dependency]
		private ClientUpgradeDatabase _clientUpgradeDatabase;

		// Token: 0x04002395 RID: 9109
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x04002396 RID: 9110
		private Motorway _clientMotorway;

		// Token: 0x04002397 RID: 9111
		private MotorwayModel _model;

		// Token: 0x04002398 RID: 9112
		private MotorwaySpline _spline;

		// Token: 0x04002399 RID: 9113
		private readonly List<ClientTileEdit> _clientTileEdits = new List<ClientTileEdit>();

		// Token: 0x0400239A RID: 9114
		private bool _rebuildMotorway = true;

		// Token: 0x0400239B RID: 9115
		private bool _rebuildSplines = true;

		// Token: 0x0400239C RID: 9116
		private bool _rebuildGeometry = true;

		// Token: 0x0400239D RID: 9117
		private bool _reapplyPermanence = true;

		// Token: 0x0400239E RID: 9118
		private bool _rebuildHazardStripes = true;

		// Token: 0x0400239F RID: 9119
		private MotorwayView.MotorwayViewColors _motorwayViewColors;

		// Token: 0x040023A0 RID: 9120
		public MotorwayHandleView handleView;

		// Token: 0x040023A1 RID: 9121
		[SerializeField]
		private InteractionCircleView _interactionCircleViewStart;

		// Token: 0x040023A2 RID: 9122
		[SerializeField]
		private InteractionCircleView _interactionCircleViewEnd;

		// Token: 0x040023A3 RID: 9123
		public GameObject shadowObject;

		// Token: 0x040023A4 RID: 9124
		private Vector2 _naturalMidpoint;

		// Token: 0x040023A5 RID: 9125
		private Vector2 _naturalTangent;

		// Token: 0x040023A6 RID: 9126
		private float _naturalStartHandleLength;

		// Token: 0x040023A7 RID: 9127
		private float _naturalEndHandleLength;

		// Token: 0x040023A8 RID: 9128
		public float HandleToleranceFactor = 0.1f;

		// Token: 0x040023A9 RID: 9129
		private readonly InertialFloat _handleDistanceFromMidpoint = new InertialFloat(0.7f, Easings.Functions.ElasticEaseOut);

		// Token: 0x040023AA RID: 9130
		private Vector2 _handleDirectionFromMidpoint;

		// Token: 0x040023AB RID: 9131
		private Vector2 _splineMidpoint;

		// Token: 0x040023AC RID: 9132
		private float _referenceMidpointAngle;

		// Token: 0x040023AD RID: 9133
		private float _referenceMidpointDistance;

		// Token: 0x040023AE RID: 9134
		private Vector2 _referenceMotorwayDirection;

		// Token: 0x040023AF RID: 9135
		private bool _hasCheckedReferenceMotorwayDirection;

		// Token: 0x040023B0 RID: 9136
		public float SplineHandleLengthFactor = 0.3f;

		// Token: 0x040023B1 RID: 9137
		public float RampExtrusion = 0.5f;

		// Token: 0x040023B2 RID: 9138
		public int SplineResolution = 20;

		// Token: 0x040023B3 RID: 9139
		public float ShadowOffsetFactor = 0.1f;

		// Token: 0x040023B4 RID: 9140
		public float MaxShadowOffset = 2f;

		// Token: 0x040023B5 RID: 9141
		private float _shadowOffset;

		// Token: 0x040023B6 RID: 9142
		private List<Vector2> _startToEndPath;

		// Token: 0x040023B7 RID: 9143
		private float _startToEndPathLength;

		// Token: 0x040023B8 RID: 9144
		private List<Vector2> _endToStartPath;

		// Token: 0x040023B9 RID: 9145
		private float _endToStartPathLength;

		// Token: 0x040023BA RID: 9146
		private readonly TweenFloat _hazardStripeWidth = new TweenFloat();

		// Token: 0x040023BB RID: 9147
		private readonly TweenFloat _hazardStripePermanenceOpacityFactor = new TweenFloat();

		// Token: 0x040023BC RID: 9148
		private RoadState _visualRoadState;

		// Token: 0x040023BD RID: 9149
		private static Mesh motorwayMesh = null;

		// Token: 0x040023BE RID: 9150
		private const int LinearDistanceSampleCount = 10;

		// Token: 0x040023BF RID: 9151
		private readonly float[] _linearDistanceTable = new float[10];

		// Token: 0x040023C0 RID: 9152
		private const int FloatsPerDepthSegment = 3;

		// Token: 0x040023C1 RID: 9153
		private const int MaxDepthSegments = 20;

		// Token: 0x040023C2 RID: 9154
		private const int DepthBufferSampleCount = 60;

		// Token: 0x040023C3 RID: 9155
		private float[] _depthBufferData = new float[60];

		// Token: 0x040023C4 RID: 9156
		private static readonly int ShadowTypeCount = Enum.GetNames(typeof(ShadowTypeRenderPass.ShadowType)).Length;

		// Token: 0x040023C5 RID: 9157
		private readonly float[] _shadowFadeouts = new float[2 * (MotorwayView.ShadowTypeCount + 1)];

		// Token: 0x040023C6 RID: 9158
		private const int HazardTapeMaxSamples = 200;

		// Token: 0x040023C7 RID: 9159
		private MotorwayModel _visualMotorwayModel;

		// Token: 0x040023C8 RID: 9160
		private LaneModel _visualStartToEndLane;

		// Token: 0x040023C9 RID: 9161
		private LaneModel _visualEndToStartLane;

		// Token: 0x040023CA RID: 9162
		private Vector2Int _visualStartCoordinates;

		// Token: 0x040023CB RID: 9163
		private Vector2Int _visualEndCoordinates;

		// Token: 0x040023CC RID: 9164
		private Vector2 _visualSplineMidpoint;

		// Token: 0x040023CD RID: 9165
		private TileView _startTileView;

		// Token: 0x040023CE RID: 9166
		private TileView _endTileView;

		// Token: 0x040023CF RID: 9167
		private Motorway _replacedMotorway;

		// Token: 0x040023D0 RID: 9168
		[SerializeField]
		private MeshRenderer _motorwayMeshRenderer;

		// Token: 0x040023D1 RID: 9169
		[SerializeField]
		private MeshRenderer _shadowMeshRenderer;

		// Token: 0x040023D2 RID: 9170
		private MaterialPropertyBlock _materialPropertyBlock;

		// Token: 0x040023D3 RID: 9171
		private MaterialPropertyBlock _shadowMaterialPropertyBlock;

		// Token: 0x040023D4 RID: 9172
		private readonly TweenVector3 _startInteractionCirclePositionTween = new TweenVector3();

		// Token: 0x040023D5 RID: 9173
		private readonly TweenVector3 _endInteractionCirclePositionTween = new TweenVector3();

		// Token: 0x040023D6 RID: 9174
		private bool _isDraggingHandle;

		// Token: 0x040023D7 RID: 9175
		private bool _isBeingEdited;

		// Token: 0x040023D8 RID: 9176
		private bool _isMotorwayOnTop;

		// Token: 0x040023D9 RID: 9177
		private bool _resortMotorwaysWhenSpringingIsComplete;

		// Token: 0x040023DA RID: 9178
		private bool skipModelPoints;

		// Token: 0x040023DB RID: 9179
		private const int LanePathResolution = 20;

		// Token: 0x020005DB RID: 1499
		public enum MotorwayComponentSortOrder
		{
			// Token: 0x040023DD RID: 9181
			MountainBase,
			// Token: 0x040023DE RID: 9182
			MountainDots,
			// Token: 0x040023DF RID: 9183
			Shadow,
			// Token: 0x040023E0 RID: 9184
			Outline,
			// Token: 0x040023E1 RID: 9185
			Road,
			// Token: 0x040023E2 RID: 9186
			CarShadow,
			// Token: 0x040023E3 RID: 9187
			CarBody,
			// Token: 0x040023E4 RID: 9188
			CarDetails,
			// Token: 0x040023E5 RID: 9189
			CarHeadlightBeams,
			// Token: 0x040023E6 RID: 9190
			CarHeadlights,
			// Token: 0x040023E7 RID: 9191
			CarWindows,
			// Token: 0x040023E8 RID: 9192
			Count
		}

		// Token: 0x020005DC RID: 1500
		private enum MotorwaySplineType
		{
			// Token: 0x040023EA RID: 9194
			Mesh,
			// Token: 0x040023EB RID: 9195
			Lane,
			// Token: 0x040023EC RID: 9196
			Shadow
		}

		// Token: 0x020005DD RID: 1501
		private struct MotorwayViewColors
		{
			// Token: 0x040023ED RID: 9197
			public Color mothballedRoad;

			// Token: 0x040023EE RID: 9198
			public Color roadInner;

			// Token: 0x040023EF RID: 9199
			public Color roadInnerNonPermanent;

			// Token: 0x040023F0 RID: 9200
			public Color roadOutline;

			// Token: 0x040023F1 RID: 9201
			public Color motorwayInner;

			// Token: 0x040023F2 RID: 9202
			public Color motorwayOuter;

			// Token: 0x040023F3 RID: 9203
			public Color shadow;
		}
	}
}
