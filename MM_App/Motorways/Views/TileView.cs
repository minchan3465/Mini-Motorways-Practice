using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Models;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000603 RID: 1539
	public class TileView : MonoBehaviour, IView, IViewLateTick, TileModel.IObserver, Tile.IObserver, IReleasedFromScopeHandler, IReusable
	{
		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x06002AE1 RID: 10977 RVA: 0x000BBA99 File Offset: 0x000B9C99
		public TileDirectionBitfield ActiveConnectionDirections
		{
			get
			{
				return this._activeConnectionDirections;
			}
		}

		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x06002AE2 RID: 10978 RVA: 0x000BBAA1 File Offset: 0x000B9CA1
		public TileDirectionBitfield PreviouslyActiveConnectionDirections
		{
			get
			{
				return this._previouslyActiveConnectionDirections;
			}
		}

		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x06002AE3 RID: 10979 RVA: 0x000BBAA9 File Offset: 0x000B9CA9
		public TilemapView TilemapView
		{
			get
			{
				return this._tilemapView;
			}
		}

		// Token: 0x06002AE4 RID: 10980 RVA: 0x000BBAB4 File Offset: 0x000B9CB4
		public TileView()
		{
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				this._nodes[directionIndex] = new TileViewNode();
			}
		}

		// Token: 0x06002AE5 RID: 10981 RVA: 0x000BBB3C File Offset: 0x000B9D3C
		public void Initialize(TilemapView tilemap, Vector2Int coordinates)
		{
			base.transform.localPosition = TilemapView.GetWorldPositionForCoordinates(coordinates);
			this._tile = this._scope.Get<Tile>();
			this._tile.Initialize(tilemap, coordinates, TileContentType.None);
			this._tile.Subscribe(this);
			if (FeatureToggle.IsFeatureDisabled(Feature.RoadDrawingAnimations))
			{
				this._animateNewConnections = false;
			}
			else
			{
				this._animateNewConnections = (this._city.Rules.DoRoadsAnimation && !this._city.Definition.TileIsOverWater(coordinates) && !this._city.Definition.TileIsUnderAMountain(coordinates));
			}
			if (this._city.Rules.RoadsBecomePermanentOverTime)
			{
				this.tileViewPermanenceZoneUpdater = new TileViewPermanenceZoneUpdater(this, this._visualConstants, this._permanenceTextureMappingDatabase, this._viewClient);
			}
			this._isTicking = true;
		}

		// Token: 0x06002AE6 RID: 10982 RVA: 0x000BBC10 File Offset: 0x000B9E10
		public void Reset()
		{
			this._isCenterOfVisiblyActiveRoundabout = false;
			this._isVisiblyActiveRoundaboutPlaced = false;
			this._visiblyActiveRoundaboutConnection = RoadTileConnection.InvalidConnection;
			this._mothballedRoundaboutConnection = RoadTileConnection.InvalidConnection;
			this._isHighlighted = false;
			this._activeRoadView = null;
			this._completeRoadView = null;
			this._trafficLightView = null;
			this._unbuiltMotorwayView = null;
			this._roundaboutView = null;
			this._clientTileEdits.Clear();
			base.transform.localPosition = Vector3.zero;
			this._animateNewConnections = false;
			this.InteractionCircleOffset = default(Vector2);
			this.TrafficLightOffsets = null;
			this._isTicking = false;
			this._activeConnectionDirections = TileDirectionBitfield.None;
			this._previouslyActiveConnectionDirections = TileDirectionBitfield.None;
			this.tileViewPermanenceZoneUpdater = null;
			TileViewNode[] nodes = this._nodes;
			for (int i = 0; i < nodes.Length; i++)
			{
				nodes[i].Reset();
			}
		}

		// Token: 0x06002AE7 RID: 10983 RVA: 0x000BBCE4 File Offset: 0x000B9EE4
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._model != null)
			{
				this._model.Unsubscribe(this);
				this._model = null;
			}
			if (this._tile != null)
			{
				this._tile.Unsubscribe(this);
				scope.Release(this._tile);
				this._tile = null;
			}
			if (this._activeSignature != null)
			{
				scope.Release(this._activeSignature);
				this._activeSignature = null;
			}
			if (this._completeSignature != null)
			{
				scope.Release(this._completeSignature);
				this._completeSignature = null;
			}
			foreach (TileViewNode node in this._nodes)
			{
				if (node.deadEndRoad != null)
				{
					this._scope.Release(node.deadEndRoad);
					node.deadEndRoad = null;
				}
			}
			foreach (AnimatedRoadTileConnectionView animatedConnection in this._animatingConnections)
			{
				this._scope.Release(animatedConnection);
			}
			this._animatingConnections.Clear();
			foreach (AnimatedRoadTileConnectionView animatedConnection2 in this._animatingRoundaboutConnections)
			{
				this._scope.Release(animatedConnection2);
			}
			this._animatingRoundaboutConnections.Clear();
		}

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x06002AE8 RID: 10984 RVA: 0x000BBE5C File Offset: 0x000BA05C
		public TileModel Model
		{
			get
			{
				return this._model;
			}
		}

		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x06002AE9 RID: 10985 RVA: 0x000BBE64 File Offset: 0x000BA064
		// (set) Token: 0x06002AEA RID: 10986 RVA: 0x000BBE6C File Offset: 0x000BA06C
		public bool IsHighlighted
		{
			get
			{
				return this._isHighlighted;
			}
			set
			{
				this._changeHighlightView = (value != this._isHighlighted);
				this._isHighlighted = value;
			}
		}

		// Token: 0x06002AEB RID: 10987 RVA: 0x000BBE87 File Offset: 0x000BA087
		public void SetModel(TileModel tileModel)
		{
			this._model = tileModel;
			this._model.Subscribe(this);
			this._rebuildTile = true;
			this.ResumeTicking();
		}

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x06002AEC RID: 10988 RVA: 0x000BBEA9 File Offset: 0x000BA0A9
		public Vector2Int Coordinates
		{
			get
			{
				return this._tile.Coordinates;
			}
		}

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x06002AED RID: 10989 RVA: 0x000BBEB6 File Offset: 0x000BA0B6
		public Tile Tile
		{
			get
			{
				if (this._rebuildTile)
				{
					this.RebuildTile();
				}
				return this._tile;
			}
		}

		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x06002AEE RID: 10990 RVA: 0x000BBECC File Offset: 0x000BA0CC
		// (set) Token: 0x06002AEF RID: 10991 RVA: 0x000BBED4 File Offset: 0x000BA0D4
		public Vector2 InteractionCircleOffset { get; private set; } = Vector2.zero;

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x06002AF0 RID: 10992 RVA: 0x000BBEDD File Offset: 0x000BA0DD
		// (set) Token: 0x06002AF1 RID: 10993 RVA: 0x000BBEE5 File Offset: 0x000BA0E5
		public Vector2[] TrafficLightOffsets { get; private set; }

		// Token: 0x06002AF2 RID: 10994 RVA: 0x000BBEEE File Offset: 0x000BA0EE
		public void AddEdit(ClientTileEdit edit)
		{
			if (this._clientTileEdits.Contains(edit))
			{
				return;
			}
			this._clientTileEdits.Add(edit);
			this._rebuildTile = true;
			this.ResumeTicking();
		}

		// Token: 0x06002AF3 RID: 10995 RVA: 0x000BBF18 File Offset: 0x000BA118
		public void RemoveEdit(ClientTileEdit edit)
		{
			if (this._clientTileEdits.Remove(edit))
			{
				this.RebuildTile();
			}
		}

		// Token: 0x06002AF4 RID: 10996 RVA: 0x000BBF2E File Offset: 0x000BA12E
		public void OnTileChanged(Tile changedTile)
		{
			this._rebuildRoadViews = true;
			this.ResumeTicking();
		}

		// Token: 0x06002AF5 RID: 10997 RVA: 0x000BBF40 File Offset: 0x000BA140
		public void OnTileModelChanged(TileModel changedTileModel)
		{
			if (changedTileModel == this.Model)
			{
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
				this._rebuildTile = true;
				this.ResumeTicking();
			}
		}

		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x06002AF6 RID: 10998 RVA: 0x000BBFB4 File Offset: 0x000BA1B4
		public DeadEndRoadView ActiveDeadEnd
		{
			get
			{
				foreach (TileViewNode node in this._nodes)
				{
					if (node.deadEndRoad != null && node.deadEndRoad.RoadState == RoadState.Active)
					{
						return node.deadEndRoad;
					}
				}
				return null;
			}
		}

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x06002AF7 RID: 10999 RVA: 0x000BBFFE File Offset: 0x000BA1FE
		public bool CanAnimateNewConnections
		{
			get
			{
				return this._animateNewConnections;
			}
		}

		// Token: 0x06002AF8 RID: 11000 RVA: 0x000BC008 File Offset: 0x000BA208
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._rebuildTile)
			{
				this.RebuildTile();
			}
			bool rebuildStaticViews = false;
			RoadTileSignature newActiveSignature = null;
			RoadTileSignature newCompleteSignature = null;
			if (this._rebuildRoadViews)
			{
				newActiveSignature = this._tile.CreateSignature(RoadState.VisiblyActive);
				newCompleteSignature = this._tile.CreateSignature(RoadState.Planned | RoadState.Pending | RoadState.Active | RoadState.Mothballed);
				this.UpdateInteractionCircle(newActiveSignature, newCompleteSignature);
				this.NotifyTileViewChanged();
				this._rebuildRoadViews = false;
				rebuildStaticViews = true;
				this.UpdateActiveConnectionDirections(newActiveSignature);
				this.RebuildDynamicRoads(newActiveSignature, newCompleteSignature);
				for (int directionIndex = 0; directionIndex < 8; directionIndex++)
				{
					if (this._nodes[directionIndex].isDynamic)
					{
						TileDirection direction = (TileDirection)directionIndex;
						this.CreateViewsForStaticConnections(newActiveSignature.GetConnectionsToDirection(direction), RoadState.Active);
						this.CreateViewsForStaticConnections(newCompleteSignature.GetConnectionsToDirection(direction), RoadState.Mothballed);
					}
				}
			}
			bool areAnyAnimationsComplete = false;
			foreach (AnimatedRoadTileConnectionView animatedConnection in this._animatingConnections)
			{
				animatedConnection.Tick(timeInterval);
				areAnyAnimationsComplete |= animatedConnection.IsComplete;
			}
			int animatedRoundaboutConnectionIndex = 0;
			while (animatedRoundaboutConnectionIndex < this._animatingRoundaboutConnections.Count)
			{
				AnimatedRoadTileConnectionView animatedConnection2 = this._animatingRoundaboutConnections[animatedRoundaboutConnectionIndex];
				animatedConnection2.Tick(timeInterval);
				if (animatedConnection2.IsComplete)
				{
					this._animatingRoundaboutConnections.RemoveAt(animatedRoundaboutConnectionIndex);
					this._scope.Release(animatedConnection2);
				}
				else
				{
					animatedRoundaboutConnectionIndex++;
				}
			}
			if (areAnyAnimationsComplete)
			{
				for (int directionIndex2 = 0; directionIndex2 < 8; directionIndex2++)
				{
					TileViewNode node = this._nodes[directionIndex2];
					if (node.isDynamic)
					{
						if (this._animatingConnections.TrueForAll((AnimatedRoadTileConnectionView connection) => connection.IsComplete || connection.AnimationDirection == RoadAnimationDirection.AnimatingOut))
						{
							node.isDynamic = false;
							rebuildStaticViews = true;
						}
					}
				}
				int animationIndex = 0;
				while (animationIndex < this._animatingConnections.Count)
				{
					AnimatedRoadTileConnectionView animatingConnection = this._animatingConnections[animationIndex];
					if (this.CanReleaseAnimation(animatingConnection))
					{
						this._animatingConnections.RemoveAt(animationIndex);
						this._scope.Release(animatingConnection);
					}
					else
					{
						animationIndex++;
					}
				}
			}
			if (rebuildStaticViews)
			{
				if (newActiveSignature == null)
				{
					newActiveSignature = this._tile.CreateSignature(RoadState.VisiblyActive);
				}
				if (newCompleteSignature == null)
				{
					newCompleteSignature = this._tile.CreateSignature(RoadState.Planned | RoadState.Pending | RoadState.Active | RoadState.Mothballed);
				}
				this.RebuildStaticRoads(newActiveSignature, newCompleteSignature);
			}
			this._isTicking = (this._animatingConnections.Count > 0 || this._animatingRoundaboutConnections.Count > 0);
			foreach (TileViewNode node2 in this._nodes)
			{
				if (node2.deadEndRoad != null)
				{
					TickResult deadEndTickResult = node2.deadEndRoad.Tick(timeInterval, stepAlpha);
					if ((node2.deadEndRoad.RoadState == RoadState.None || node2.deadEndRoad.IsBeingReplaced) && !node2.deadEndRoad.IsDynamic)
					{
						this._scope.Release(node2.deadEndRoad);
						node2.deadEndRoad = null;
					}
					this._isTicking |= (deadEndTickResult == TickResult.ContinueTicking);
				}
			}
			if (this._changeHighlightView)
			{
				if (this._isHighlighted)
				{
					if (this._highlightView == null)
					{
						this._highlightView = TileSelectedView.Create(this._viewClient, this);
					}
					City city = this._scope.Get<City>();
					ClockModel clockModel = this._scope.Get<ClockModel>();
					if (city.IsTileInPlayableArea(this.Coordinates, clockModel.ExpansionTime))
					{
						this._highlightView.Appear();
					}
				}
				else
				{
					if (this._highlightView != null)
					{
						this._highlightView.Disappear();
					}
					this._highlightView = null;
				}
				this._changeHighlightView = false;
			}
			if (this.tileViewPermanenceZoneUpdater != null)
			{
				this.tileViewPermanenceZoneUpdater.Tick(timeInterval.Delta);
				this._isTicking = true;
			}
			if (!this._isTicking)
			{
				return TickResult.StopTicking;
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002AF9 RID: 11001 RVA: 0x000BC3C4 File Offset: 0x000BA5C4
		public void SetGameobjectActive(bool isActive)
		{
			foreach (TileViewNode node in this._nodes)
			{
				if (node.deadEndRoad != null)
				{
					node.deadEndRoad.SetGameobjectActive(isActive);
				}
			}
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002AFA RID: 11002 RVA: 0x000BC410 File Offset: 0x000BA610
		public void LateTick(TimeInterval tickTime, float stepAlpha)
		{
			TileViewPermanenceZoneUpdater tileViewPermanenceZoneUpdater = this.tileViewPermanenceZoneUpdater;
			if (tileViewPermanenceZoneUpdater == null)
			{
				return;
			}
			tileViewPermanenceZoneUpdater.LateTick(tickTime.Delta);
		}

		// Token: 0x06002AFB RID: 11003 RVA: 0x000BC428 File Offset: 0x000BA628
		public void ResumeTicking()
		{
			if (!this._isTicking)
			{
				this._isTicking = true;
				this._viewClient.ResumeTickingView(this);
			}
		}

		// Token: 0x06002AFC RID: 11004 RVA: 0x000BC448 File Offset: 0x000BA648
		public void ReconfigurePermanenceVisibility()
		{
			if (this._city.Rules.RoadsBecomePermanentOverTime)
			{
				if (this.tileViewPermanenceZoneUpdater == null)
				{
					this.tileViewPermanenceZoneUpdater = new TileViewPermanenceZoneUpdater(this, this._visualConstants, this._permanenceTextureMappingDatabase, this._viewClient);
				}
			}
			else
			{
				this.tileViewPermanenceZoneUpdater = null;
			}
			foreach (TileViewNode node in this._nodes)
			{
				if (node.deadEndRoad != null)
				{
					node.deadEndRoad.ReconfigurePermanenceVisibility();
				}
			}
			foreach (AnimatedRoadTileConnectionView animatedRoadTileConnectionView in this._animatingConnections)
			{
				animatedRoadTileConnectionView.SetPermanenceVisibility(this._city.Rules.RoadsBecomePermanentOverTime);
			}
			if (this._activeRoadView != null)
			{
				this._activeRoadView.ReconfigurePermanenceVisibility();
			}
			if (this._completeRoadView != null)
			{
				this._completeRoadView.ReconfigurePermanenceVisibility();
			}
		}

		// Token: 0x06002AFD RID: 11005 RVA: 0x000BC550 File Offset: 0x000BA750
		private bool CanReleaseAnimation(AnimatedRoadTileConnectionView animatingConnection)
		{
			return animatingConnection.IsComplete && (animatingConnection.AnimationDirection == RoadAnimationDirection.AnimatingOut || (!this._nodes[(int)animatingConnection.Connection.input.direction].isDynamic && !this._nodes[(int)animatingConnection.Connection.output.direction].isDynamic));
		}

		// Token: 0x06002AFE RID: 11006 RVA: 0x000BC5B4 File Offset: 0x000BA7B4
		private void CreateViewsForStaticConnections(IEnumerable<RoadTileConnection> connections, RoadState roadState)
		{
			foreach (RoadTileConnection connection in connections)
			{
				bool showMothballedConnectionUnderActiveConnection = false;
				if (roadState == RoadState.Mothballed && this._completeSignature != null && this._activeSignature != null)
				{
					showMothballedConnectionUnderActiveConnection = (this._completeSignature.HasConnection(connection) && !this._activeSignature.HasConnection(connection));
				}
				if (!connection.IsUTurn)
				{
					AnimatedRoadTileConnectionView animationForConnection = this.GetAnimationForConnection(connection);
					if (animationForConnection == null || (showMothballedConnectionUnderActiveConnection && animationForConnection.RoadState != RoadState.Mothballed))
					{
						animationForConnection = AnimatedRoadTileConnectionView.CreateStaticAnimation(this._scope, this, connection, roadState);
						this._animatingConnections.Add(animationForConnection);
					}
				}
			}
		}

		// Token: 0x06002AFF RID: 11007 RVA: 0x000BC678 File Offset: 0x000BA878
		private void RebuildTile()
		{
			this._rebuildTile = false;
			if (this.Model != null)
			{
				this.Model.Tile.CloneInto(this.Tile);
			}
			else
			{
				this.Tile.Clear();
			}
			foreach (ClientTileEdit clientTileEdit in this._clientTileEdits)
			{
				clientTileEdit.edit.ApplyToAffectedTile(this.Tile);
			}
			if (this.Tile.HasTrafficLight)
			{
				if (this._trafficLightView == null)
				{
					this._trafficLightView = this._scope.Get<TrafficLightView>();
					this._trafficLightView.transform.localPosition = base.transform.localPosition;
					this._viewClient.AddView(this._trafficLightView);
					this._trafficLightView.InitialiseInteractionCirclePosition(this);
				}
				else
				{
					this._trafficLightView.gameObject.SetActive(true);
				}
				if (this._trafficLightView.Model == null)
				{
					TileModel model = this.Model;
					if (((model != null) ? model.roadChunk.TrafficLight : null) != null)
					{
						this._trafficLightView.SetModel(this.Model.roadChunk.TrafficLight);
					}
				}
			}
			else if (this._trafficLightView != null)
			{
				this._viewClient.MarkViewForRemoval(this._trafficLightView);
				this._trafficLightView = null;
			}
			if (this.Tile.IsCenterOfRoundabout)
			{
				if (this._roundaboutView == null)
				{
					this._roundaboutView = this._scope.Get<RoundaboutView>();
					this._roundaboutView.transform.localPosition = base.transform.localPosition;
					this._viewClient.AddView(this._roundaboutView);
					this._roundaboutView.Initialize(this);
				}
			}
			else if (this._roundaboutView != null)
			{
				this._viewClient.MarkViewForRemoval(this._roundaboutView);
				this._roundaboutView = null;
			}
			if (this.Tile.UnbuiltMotorwayId == -1)
			{
				if (this._unbuiltMotorwayView != null)
				{
					this._viewClient.MarkViewForRemoval(this._unbuiltMotorwayView);
					this._unbuiltMotorwayView = null;
				}
				return;
			}
			if (this._unbuiltMotorwayView == null)
			{
				this._unbuiltMotorwayView = this._scope.Get<UnbuiltMotorwayView>();
				this._unbuiltMotorwayView.Initialize(this, base.transform.localPosition, this.InteractionCircleOffset, this.Tile.UnbuiltMotorwayNumber);
				this._viewClient.AddView(this._unbuiltMotorwayView);
				return;
			}
			this._unbuiltMotorwayView.gameObject.SetActive(true);
		}

		// Token: 0x06002B00 RID: 11008 RVA: 0x000BC91C File Offset: 0x000BAB1C
		private void RebuildDynamicRoads(RoadTileSignature newActiveSignature, RoadTileSignature newCompleteSignature)
		{
			bool canInputDeviceAnimate = this._inputState.CurrentDeviceInputType != DeviceInputType.Remote;
			TransitionStyle transitionStyle = (!this._viewClient.OnFirstFrame && this._animateNewConnections && canInputDeviceAnimate) ? TransitionStyle.Tween : TransitionStyle.Snap;
			bool newIsCenterOfVisiblyActiveRoundabout = this._tile.IsCenterOfRoundabout;
			RoadTileConnection newVisiblyActiveRoundaboutConnection = this._tile.GetRoundaboutConnection(RoadState.Planned | RoadState.Active);
			RoadTileConnection newMothballedRoundaboutConnection = this._tile.GetRoundaboutConnection(RoadState.Mothballed);
			if (newVisiblyActiveRoundaboutConnection != this._visiblyActiveRoundaboutConnection)
			{
				if (this._visiblyActiveRoundaboutConnection == RoadTileConnection.InvalidConnection)
				{
					transitionStyle = TransitionStyle.Snap;
				}
				else if (newVisiblyActiveRoundaboutConnection == RoadTileConnection.InvalidConnection)
				{
					if (!this._isVisiblyActiveRoundaboutPlaced)
					{
						transitionStyle = TransitionStyle.Snap;
					}
				}
				else
				{
					transitionStyle = TransitionStyle.Snap;
				}
			}
			else if (newIsCenterOfVisiblyActiveRoundabout != this._isCenterOfVisiblyActiveRoundabout && !this._isVisiblyActiveRoundaboutPlaced)
			{
				transitionStyle = TransitionStyle.Snap;
			}
			if (transitionStyle == TransitionStyle.Tween)
			{
				if (this._mothballedRoundaboutConnection != RoadTileConnection.InvalidConnection && newMothballedRoundaboutConnection != this._mothballedRoundaboutConnection && newVisiblyActiveRoundaboutConnection != this._mothballedRoundaboutConnection)
				{
					AnimatedRoadTileConnectionView roundaboutAnimationOut = AnimatedRoadTileConnectionView.CreateAnimationOut(this._scope, this, this._mothballedRoundaboutConnection, RoadState.Mothballed);
					this._animatingRoundaboutConnections.Add(roundaboutAnimationOut);
				}
				if (this._visiblyActiveRoundaboutConnection != RoadTileConnection.InvalidConnection && this._isVisiblyActiveRoundaboutPlaced && newVisiblyActiveRoundaboutConnection != this._visiblyActiveRoundaboutConnection && newMothballedRoundaboutConnection != this._visiblyActiveRoundaboutConnection)
				{
					AnimatedRoadTileConnectionView roundaboutAnimationOut2 = AnimatedRoadTileConnectionView.CreateAnimationOut(this._scope, this, this._visiblyActiveRoundaboutConnection, RoadState.Mothballed);
					this._animatingRoundaboutConnections.Add(roundaboutAnimationOut2);
				}
			}
			this._isCenterOfVisiblyActiveRoundabout = newIsCenterOfVisiblyActiveRoundabout;
			this._visiblyActiveRoundaboutConnection = newVisiblyActiveRoundaboutConnection;
			this._mothballedRoundaboutConnection = newMothballedRoundaboutConnection;
			TileModel model = this.Model;
			Tile tileModel = (model != null) ? model.Tile : null;
			if (this._isCenterOfVisiblyActiveRoundabout)
			{
				this._isVisiblyActiveRoundaboutPlaced = (tileModel != null && tileModel.IsCenterOfRoundabout);
			}
			else
			{
				this._isVisiblyActiveRoundaboutPlaced = (tileModel != null && tileModel.HasRoundabout(RoadState.Planned | RoadState.Active));
			}
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				TileViewNode node = this._nodes[directionIndex];
				RoadState newRoadState = this._tile.GetTwoLaneRoadStateInDirection((TileDirection)directionIndex);
				if (newRoadState != node.roadState)
				{
					this.SetNodeState(directionIndex, newRoadState, newActiveSignature, newCompleteSignature, transitionStyle);
				}
			}
			TileDirection activeDeadEndDirection = newActiveSignature.IsDeadEnd ? newActiveSignature.Connections.First<RoadTileConnection>().input.direction : TileDirection.None;
			TileDirectionBitfield mothballedDeadEndDirections = default(TileDirectionBitfield);
			if (newCompleteSignature.IsDeadEnd)
			{
				mothballedDeadEndDirections[newCompleteSignature.Connections.First<RoadTileConnection>().input.direction] = true;
			}
			if (this._tile.ContentType == TileContentType.House && this._model != null)
			{
				foreach (LaneModel drivewayLane in this._model.roadChunk.lanes)
				{
					if (drivewayLane.state == RoadState.Mothballed && drivewayLane.connection.IsUTurn)
					{
						mothballedDeadEndDirections[drivewayLane.connection.input.direction] = true;
					}
				}
			}
			for (int directionIndex2 = 0; directionIndex2 < 8; directionIndex2++)
			{
				TileViewNode node2 = this._nodes[directionIndex2];
				if (activeDeadEndDirection == (TileDirection)directionIndex2)
				{
					int nodeIndex = directionIndex2;
					RoadState newDeadEndState = RoadState.Active;
					TransitionStyle transitionStyle2 = transitionStyle;
					RoadTileSignature activeSignature = this._activeSignature;
					this.ShowDeadEnd(nodeIndex, newDeadEndState, transitionStyle2, (activeSignature != null) ? activeSignature.Connections : null, newActiveSignature.Connections, null);
				}
				else if (mothballedDeadEndDirections[(TileDirection)directionIndex2])
				{
					int nodeIndex2 = directionIndex2;
					RoadState newDeadEndState2 = RoadState.Mothballed;
					TransitionStyle transitionStyle3 = transitionStyle;
					RoadTileSignature completeSignature = this._completeSignature;
					this.ShowDeadEnd(nodeIndex2, newDeadEndState2, transitionStyle3, (completeSignature != null) ? completeSignature.Connections : null, newCompleteSignature.Connections, null);
				}
				else if (node2.deadEndRoad != null)
				{
					RoadTileSignature oldSignature;
					RoadTileSignature newSignature;
					RoadTileSignature ignoredSignature;
					if (node2.deadEndRoad.RoadState == RoadState.Mothballed)
					{
						oldSignature = this._completeSignature;
						newSignature = newCompleteSignature;
						ignoredSignature = newActiveSignature;
					}
					else
					{
						oldSignature = this._activeSignature;
						newSignature = newActiveSignature;
						ignoredSignature = null;
					}
					this.HideDeadEnd(directionIndex2, transitionStyle, (oldSignature != null) ? oldSignature.Connections : null, newSignature.Connections, (ignoredSignature != null) ? ignoredSignature.Connections : null);
				}
			}
		}

		// Token: 0x06002B01 RID: 11009 RVA: 0x000BCCF4 File Offset: 0x000BAEF4
		private void RebuildStaticRoads(RoadTileSignature newActiveSignature, RoadTileSignature newCompleteSignature)
		{
			using (RoadTileSignature staticActiveSignature = this.CreateStaticSignature(newActiveSignature))
			{
				this.SetStaticActiveSignature(staticActiveSignature);
				RoadTileSignature activeSignature = this._activeSignature;
				if (activeSignature != null)
				{
					activeSignature.Dispose();
				}
				this._activeSignature = newActiveSignature;
				using (RoadTileSignature staticCompleteSignature = this.CreateStaticSignature(newCompleteSignature))
				{
					this.SetStaticCompleteSignature(staticCompleteSignature);
					RoadTileSignature completeSignature = this._completeSignature;
					if (completeSignature != null)
					{
						completeSignature.Dispose();
					}
					this._completeSignature = newCompleteSignature;
					if (this._completeRoadView != null)
					{
						this._completeRoadView.gameObject.SetActive(!this._activeSignature.Equals(this._completeSignature));
					}
				}
			}
		}

		// Token: 0x06002B02 RID: 11010 RVA: 0x000BCDB4 File Offset: 0x000BAFB4
		private RoadTileSignature CreateStaticSignature(RoadTileSignature fullSignature)
		{
			RoadTileSignature staticSignature = this._scope.Get<RoadTileSignature>();
			foreach (RoadTileConnection connection in fullSignature.Connections)
			{
				if (!connection.IsUTurn && (connection.IsRoundabout || ((!this._nodes[(int)connection.input.direction].isDynamic || connection.input.type == RoadType.Roundabout) && (!this._nodes[(int)connection.output.direction].isDynamic || connection.output.type == RoadType.Roundabout))))
				{
					staticSignature.AddConnection(connection);
				}
			}
			return staticSignature;
		}

		// Token: 0x06002B03 RID: 11011 RVA: 0x000BCE70 File Offset: 0x000BB070
		private void UpdateActiveConnectionDirections(RoadTileSignature signature)
		{
			if (this.tileViewPermanenceZoneUpdater == null || this._activeConnectionDirections == signature.ConnectionDirections)
			{
				return;
			}
			this._previouslyActiveConnectionDirections = this._activeConnectionDirections;
			this._activeConnectionDirections = signature.ConnectionDirections;
			foreach (TileDirection activeConnectionDirection in this._activeConnectionDirections)
			{
				this._previouslyActiveConnectionDirections[activeConnectionDirection] = false;
			}
			this.tileViewPermanenceZoneUpdater.UpdateSolidZonePermanenceSources();
		}

		// Token: 0x06002B04 RID: 11012 RVA: 0x000BCEE8 File Offset: 0x000BB0E8
		private void SetStaticActiveSignature(RoadTileSignature staticSignature)
		{
			if (staticSignature.IsEmpty)
			{
				if (this._activeRoadView != null)
				{
					this._activeRoadView.SetSignature(staticSignature);
					return;
				}
			}
			else
			{
				if (this._activeRoadView == null)
				{
					this._activeRoadView = this._scope.Get<RoadView>();
					this._activeRoadView.transform.localPosition = base.transform.localPosition;
					this._viewClient.AddView(this._activeRoadView);
					this._activeRoadView.tileView = this;
				}
				this._activeRoadView.SetSignature(staticSignature);
			}
		}

		// Token: 0x06002B05 RID: 11013 RVA: 0x000BCF7C File Offset: 0x000BB17C
		private void SetStaticCompleteSignature(RoadTileSignature staticSignature)
		{
			if (staticSignature.IsEmpty)
			{
				if (this._completeRoadView != null)
				{
					this._completeRoadView.SetSignature(staticSignature);
					return;
				}
			}
			else
			{
				if (this._completeRoadView == null)
				{
					this._completeRoadView = this._scope.Get<RoadView>();
					this._completeRoadView.transform.localPosition = base.transform.localPosition;
					this._viewClient.AddView(this._completeRoadView);
					this._completeRoadView.tileView = this;
				}
				this._completeRoadView.SetSignature(staticSignature);
				this._completeRoadView.GetComponent<MeshRenderer>().sharedMaterial = this._completeRoadView.mothballedMaterial;
			}
		}

		// Token: 0x06002B06 RID: 11014 RVA: 0x000BD030 File Offset: 0x000BB230
		private void SetNodeState(int nodeIndex, RoadState newRoadState, RoadTileSignature newActiveSignature, RoadTileSignature newCompleteSignature, TransitionStyle transitionStyle)
		{
			TileViewNode node = this._nodes[nodeIndex];
			if (node.roadState == newRoadState)
			{
				return;
			}
			RoadState oldRoadState = node.roadState;
			node.roadState = newRoadState;
			if (transitionStyle == TransitionStyle.Snap)
			{
				return;
			}
			if ((newRoadState & RoadState.VisiblyActive) != RoadState.None)
			{
				if ((oldRoadState & RoadState.VisiblyActive) > RoadState.None)
				{
					return;
				}
				foreach (RoadTileConnection activeConnection in newActiveSignature.GetConnectionsToDirection((TileDirection)nodeIndex))
				{
					if (!activeConnection.IsUTurn)
					{
						this.AnimateConnectionIn(activeConnection, RoadState.Active, oldRoadState);
						node.isDynamic = true;
					}
				}
				using (IEnumerator<RoadTileConnection> enumerator = newCompleteSignature.GetConnectionsToDirection((TileDirection)nodeIndex).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RoadTileConnection mothballedConnection = enumerator.Current;
						if (!newActiveSignature.HasConnection(mothballedConnection) && !this._completeSignature.HasConnection(mothballedConnection) && !mothballedConnection.IsUTurn)
						{
							this.AnimateConnectionIn(mothballedConnection, RoadState.Mothballed, oldRoadState);
							node.isDynamic = true;
						}
					}
					return;
				}
			}
			if (newRoadState == RoadState.Mothballed)
			{
				foreach (AnimatedRoadTileConnectionView animatedConnection in this._animatingConnections)
				{
					if (animatedConnection.IsConnectedToDirection((TileDirection)nodeIndex))
					{
						animatedConnection.RoadState = RoadState.Mothballed;
					}
				}
				if (node.deadEndRoad != null)
				{
					node.deadEndRoad.SetRoadState(RoadState.Mothballed, TransitionStyle.Tween);
					return;
				}
			}
			else
			{
				foreach (RoadTileConnection activeConnection2 in this._activeSignature.GetConnectionsToDirection((TileDirection)nodeIndex))
				{
					if (!activeConnection2.IsUTurn)
					{
						this.AnimateConnectionOut(activeConnection2, RoadState.Active);
					}
				}
				foreach (RoadTileConnection mothballedConnection2 in this._completeSignature.GetConnectionsToDirection((TileDirection)nodeIndex))
				{
					if (!this._activeSignature.HasConnection(mothballedConnection2) && !mothballedConnection2.IsUTurn)
					{
						this.AnimateConnectionOut(mothballedConnection2, RoadState.Mothballed);
					}
				}
			}
		}

		// Token: 0x06002B07 RID: 11015 RVA: 0x000BD260 File Offset: 0x000BB460
		private void AnimateConnectionIn(RoadTileConnection connection, RoadState roadState, RoadState previousRoadState)
		{
			AnimatedRoadTileConnectionView connectionAnimation = this.GetAnimationForConnection(connection);
			if (connectionAnimation != null)
			{
				connectionAnimation.AnimationDirection = RoadAnimationDirection.AnimatingIn;
				connectionAnimation.RoadState = roadState;
			}
			else
			{
				connectionAnimation = AnimatedRoadTileConnectionView.CreateAnimationIn(this._scope, this, connection, roadState, previousRoadState);
				this._animatingConnections.Add(connectionAnimation);
			}
			TileViewNode inputNode = this._nodes[(int)connection.input.direction];
			if (inputNode.deadEndRoad != null && (inputNode.deadEndRoad.RoadState == RoadState.Mothballed || roadState == RoadState.Active))
			{
				inputNode.deadEndRoad.ReplaceWithConnection(connection);
			}
			TileViewNode outputNode = this._nodes[(int)connection.output.direction];
			if (outputNode.deadEndRoad != null && (outputNode.deadEndRoad.RoadState == RoadState.Mothballed || roadState == RoadState.Active))
			{
				outputNode.deadEndRoad.ReplaceWithConnection(connection);
			}
		}

		// Token: 0x06002B08 RID: 11016 RVA: 0x000BD32C File Offset: 0x000BB52C
		private void AnimateConnectionOut(RoadTileConnection connection, RoadState roadState)
		{
			AnimatedRoadTileConnectionView connectionAnimation = this.GetAnimationForConnection(connection);
			if (connectionAnimation != null)
			{
				if (connectionAnimation.AnimationDirection == RoadAnimationDirection.AnimatingIn)
				{
					connectionAnimation.AnimationDirection = RoadAnimationDirection.AnimatingOut;
					return;
				}
			}
			else
			{
				AnimatedRoadTileConnectionView disappearAnimation = AnimatedRoadTileConnectionView.CreateAnimationOut(this._scope, this, connection, roadState);
				this._animatingConnections.Add(disappearAnimation);
			}
		}

		// Token: 0x06002B09 RID: 11017 RVA: 0x000BD378 File Offset: 0x000BB578
		private AnimatedRoadTileConnectionView GetAnimationForConnection(RoadTileConnection connection)
		{
			RoadTileConnection reflectedConnection = connection.GetReflectedConnection();
			foreach (AnimatedRoadTileConnectionView existingAnimatingConnection in this._animatingConnections)
			{
				if (existingAnimatingConnection.Connection == connection || existingAnimatingConnection.Connection == reflectedConnection)
				{
					return existingAnimatingConnection;
				}
			}
			return null;
		}

		// Token: 0x06002B0A RID: 11018 RVA: 0x000BD3F0 File Offset: 0x000BB5F0
		private void ShowDeadEnd(int nodeIndex, RoadState newDeadEndState, TransitionStyle transitionStyle, IEnumerable<RoadTileConnection> previousConnections = null, IEnumerable<RoadTileConnection> newConnections = null, IEnumerable<RoadTileConnection> ignoredConnections = null)
		{
			TileViewNode node = this._nodes[nodeIndex];
			node.isDeadEndConnectedToMotorway = false;
			node.isDeadEndConnectedToEditingMotorway = false;
			if (node.deadEndRoad == null)
			{
				node.deadEndRoad = this._scope.Get<DeadEndRoadView>();
				node.deadEndRoad.transform.localPosition = TilemapView.GetWorldPositionForCoordinates(this.Coordinates);
				node.deadEndRoad.Initialize(this, (TileDirection)nodeIndex);
			}
			if (newDeadEndState == RoadState.Active)
			{
				int motorwayId = this._tile.GetMotorwayInDirection((TileDirection)nodeIndex, RoadState.VisiblyActive);
				if (motorwayId != -1)
				{
					MotorwayView motorway = this._tilemapView.GetMotorwayView(motorwayId);
					if (motorway != null && motorway.IsBeingEdited)
					{
						transitionStyle = TransitionStyle.Snap;
						node.isDeadEndConnectedToEditingMotorway = true;
					}
				}
			}
			node.isDeadEndConnectedToMotorway = (this._tile.GetMotorwayInDirection((TileDirection)nodeIndex, RoadState.Planned | RoadState.Pending | RoadState.Active | RoadState.Mothballed) != -1);
			if (!node.deadEndRoad.IsReplacing && transitionStyle == TransitionStyle.Tween && previousConnections != null)
			{
				RoadTileConnection connectionToEmergeFrom = RoadTileConnection.InvalidConnection;
				foreach (RoadTileConnection oldActiveConnection in previousConnections)
				{
					if (oldActiveConnection.input.direction == (TileDirection)nodeIndex && !oldActiveConnection.IsUTurn && (newConnections == null || !newConnections.Contains(oldActiveConnection)) && (connectionToEmergeFrom.output.direction == TileDirection.None || TileUtilities.GetDistanceBetweenDirections(connectionToEmergeFrom.input.direction, connectionToEmergeFrom.output.direction) < TileUtilities.GetDistanceBetweenDirections(oldActiveConnection.input.direction, oldActiveConnection.output.direction)))
					{
						connectionToEmergeFrom = oldActiveConnection;
					}
				}
				if (connectionToEmergeFrom.output.direction != TileDirection.None)
				{
					float widthFactor = 1f;
					AnimatedRoadTileConnectionView animatingConnection = this.GetAnimationForConnection(connectionToEmergeFrom);
					if (animatingConnection != null)
					{
						widthFactor = animatingConnection.OutlineWidthFactor;
					}
					node.deadEndRoad.AppearFromConnection(connectionToEmergeFrom, widthFactor);
				}
			}
			node.deadEndRoad.SetRoadState(newDeadEndState, transitionStyle);
		}

		// Token: 0x06002B0B RID: 11019 RVA: 0x000BD5E0 File Offset: 0x000BB7E0
		private void HideDeadEnd(int nodeIndex, TransitionStyle transitionStyle, IEnumerable<RoadTileConnection> previousConnections = null, IEnumerable<RoadTileConnection> newConnections = null, IEnumerable<RoadTileConnection> ignoredConnections = null)
		{
			TileViewNode node = this._nodes[nodeIndex];
			if (node.isDeadEndConnectedToEditingMotorway)
			{
				transitionStyle = TransitionStyle.Snap;
				node.isDeadEndConnectedToEditingMotorway = false;
			}
			if (node.deadEndRoad != null)
			{
				RoadTileConnection replacingConnection = RoadTileConnection.InvalidConnection;
				if (transitionStyle == TransitionStyle.Tween)
				{
					if (node.deadEndRoad.IsBeingReplaced)
					{
						replacingConnection = new RoadTileConnection(new RoadTileNode(node.deadEndRoad.Direction, RoadType.TwoLane, -1), new RoadTileNode(node.deadEndRoad.AutoDistortionTarget, RoadType.TwoLane, -1));
					}
					else if (newConnections != null)
					{
						foreach (RoadTileConnection newConnection in newConnections)
						{
							if (newConnection.input.direction == (TileDirection)nodeIndex && !newConnection.IsUTurn && (previousConnections == null || !previousConnections.Contains(newConnection)) && (replacingConnection.output.direction == TileDirection.None || TileUtilities.GetDistanceBetweenDirections(replacingConnection.input.direction, replacingConnection.output.direction) < TileUtilities.GetDistanceBetweenDirections(newConnection.input.direction, newConnection.output.direction)))
							{
								replacingConnection = newConnection;
							}
						}
					}
				}
				bool removeDeadEnd;
				if (replacingConnection.output.direction != TileDirection.None)
				{
					AnimatedRoadTileConnectionView replacingConnectionAnimation = this.GetAnimationForConnection(replacingConnection);
					removeDeadEnd = (replacingConnectionAnimation != null && replacingConnectionAnimation.AnimationDirection == RoadAnimationDirection.AnimatingOut);
					node.deadEndRoad.ReplaceWithConnection(replacingConnection);
				}
				else
				{
					removeDeadEnd = true;
				}
				if (removeDeadEnd)
				{
					node.deadEndRoad.SetRoadState(RoadState.None, node.isDeadEndConnectedToMotorway ? TransitionStyle.Snap : transitionStyle);
				}
			}
			node.isDeadEndConnectedToMotorway = false;
		}

		// Token: 0x06002B0C RID: 11020 RVA: 0x000BD77C File Offset: 0x000BB97C
		private void UpdateInteractionCircle(RoadTileSignature activeSignature, RoadTileSignature completeSignature)
		{
			RoadTileSignature interactionCircleSignature = activeSignature;
			if (this.Tile.HasTrafficLight || activeSignature.IsEmpty || (activeSignature.IsDeadEnd && this._tile.GetTwoLaneRoadCount(RoadState.Mothballed, Tile.MotorwayInclusion.Ignore) > 1))
			{
				interactionCircleSignature = completeSignature;
			}
			RoadTileDefinition interactionCircleDefinition = this._roadTileAtlas.GetDefinitionForSignature(interactionCircleSignature);
			if (Diagnostics.Verify(interactionCircleDefinition != null, "Could not find an interaction circle definition for the signature {0}.", interactionCircleSignature))
			{
				this.InteractionCircleOffset = interactionCircleDefinition.interactionCircleOffset;
				this.TrafficLightOffsets = interactionCircleDefinition.trafficLightOffsets;
			}
		}

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x06002B0D RID: 11021 RVA: 0x000BD7F1 File Offset: 0x000BB9F1
		private bool ContainsCarparkOrHouse
		{
			get
			{
				return this.Tile.ContentType == TileContentType.Carpark || this.Tile.ContentType == TileContentType.House;
			}
		}

		// Token: 0x06002B0E RID: 11022 RVA: 0x000BD814 File Offset: 0x000BBA14
		public bool ShouldDisplayDirectionAsPermanent(TileDirection direction)
		{
			if (this.ContainsCarparkOrHouse)
			{
				return true;
			}
			TileView adjacentTileView = this._tilemapView.GetTileView(TileUtilities.GetAdjacentCoordinates(this.Coordinates, direction));
			return adjacentTileView != null && adjacentTileView.ContainsCarparkOrHouse;
		}

		// Token: 0x06002B0F RID: 11023 RVA: 0x000BD858 File Offset: 0x000BBA58
		public float GetVisualNodePermanenceProgress(TileDirection direction)
		{
			float nodePermanence = (float)this.Tile.GetNodePermanenceProgress(direction);
			RoadTileConnection roundaboutConnection = this.Tile.GetRoundaboutConnection(RoadState.VisiblyActive);
			if (roundaboutConnection.input.direction == direction || roundaboutConnection.output.direction == direction)
			{
				Tile roundaboutCenterTile = this._tilemapView.GetTile(this.Tile.Coordinates - Roundabout.GetCoordinatesOffsetForConnection(roundaboutConnection));
				if (roundaboutCenterTile != null)
				{
					nodePermanence = (float)roundaboutCenterTile.RoundaboutPermanenceProgress;
				}
			}
			return this._visualConstants.DryingRoadFalloff.Evaluate(nodePermanence);
		}

		// Token: 0x06002B10 RID: 11024 RVA: 0x000BD8E5 File Offset: 0x000BBAE5
		public TileView GetTileViewInDirection(TileDirection direction)
		{
			if (direction == TileDirection.None)
			{
				return this;
			}
			return this.TilemapView.GetTileView(TileUtilities.GetAdjacentCoordinates(this.Coordinates, direction));
		}

		// Token: 0x06002B11 RID: 11025 RVA: 0x000BD904 File Offset: 0x000BBB04
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.black;
			if (this._scope != null && this._scope.Get<TilemapView>() != null)
			{
				Vector3 tileCentre = TilemapView.GetWorldPositionForCoordinates(this._tile.Coordinates);
				Gizmos.DrawLine(tileCentre + new Vector3((float)(-TilemapModel.HalfTileWidth), (float)(-TilemapModel.HalfTileWidth), 0f), tileCentre + new Vector3((float)TilemapModel.HalfTileWidth, (float)(-TilemapModel.HalfTileWidth), 0f));
				Gizmos.DrawLine(tileCentre + new Vector3((float)TilemapModel.HalfTileWidth, (float)(-TilemapModel.HalfTileWidth), 0f), tileCentre + new Vector3((float)TilemapModel.HalfTileWidth, (float)TilemapModel.HalfTileWidth, 0f));
				Gizmos.DrawLine(tileCentre + new Vector3((float)TilemapModel.HalfTileWidth, (float)TilemapModel.HalfTileWidth, 0f), tileCentre + new Vector3((float)(-TilemapModel.HalfTileWidth), (float)TilemapModel.HalfTileWidth, 0f));
				Gizmos.DrawLine(tileCentre + new Vector3((float)(-TilemapModel.HalfTileWidth), (float)TilemapModel.HalfTileWidth, 0f), tileCentre + new Vector3((float)(-TilemapModel.HalfTileWidth), (float)(-TilemapModel.HalfTileWidth), 0f));
			}
		}

		// Token: 0x06002B12 RID: 11026 RVA: 0x000022F5 File Offset: 0x000004F5
		[Conditional("UNITY_EDITOR")]
		private void ResetEditorFields()
		{
		}

		// Token: 0x06002B13 RID: 11027 RVA: 0x000022F5 File Offset: 0x000004F5
		[Conditional("UNITY_EDITOR")]
		private void UpdateEditorFields()
		{
		}

		// Token: 0x06002B14 RID: 11028 RVA: 0x000BDAB9 File Offset: 0x000BBCB9
		public void Subscribe(TileView.IObserver observer)
		{
			this._observers.Subscribe(observer);
		}

		// Token: 0x06002B15 RID: 11029 RVA: 0x000BDAC7 File Offset: 0x000BBCC7
		public bool Unsubscribe(TileView.IObserver observer)
		{
			return this._observers.Unsubscribe(observer);
		}

		// Token: 0x06002B16 RID: 11030 RVA: 0x000BDAD8 File Offset: 0x000BBCD8
		private void NotifyTileViewChanged()
		{
			foreach (TileView.IObserver observer in this._observers)
			{
				observer.OnTileViewChanged(this);
			}
		}

		// Token: 0x040024F2 RID: 9458
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("View.Tile");

		// Token: 0x040024F3 RID: 9459
		[Dependency]
		private IScope _scope;

		// Token: 0x040024F4 RID: 9460
		[Dependency]
		private City _city;

		// Token: 0x040024F5 RID: 9461
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x040024F6 RID: 9462
		[Dependency]
		private ClientUpgradeDatabase _clientUpgradeDatabase;

		// Token: 0x040024F7 RID: 9463
		[Dependency]
		private InputState _inputState;

		// Token: 0x040024F8 RID: 9464
		[Dependency]
		private TilemapView _tilemapView;

		// Token: 0x040024F9 RID: 9465
		[Dependency]
		private RoadTileAtlas _roadTileAtlas;

		// Token: 0x040024FA RID: 9466
		[Dependency]
		private PermanenceTextureMappingDatabase _permanenceTextureMappingDatabase;

		// Token: 0x040024FB RID: 9467
		[Dependency]
		public VisualConstantsData _visualConstants;

		// Token: 0x040024FC RID: 9468
		private readonly TileViewNode[] _nodes = new TileViewNode[8];

		// Token: 0x040024FD RID: 9469
		private bool _animateNewConnections;

		// Token: 0x040024FE RID: 9470
		private readonly List<AnimatedRoadTileConnectionView> _animatingConnections = new List<AnimatedRoadTileConnectionView>();

		// Token: 0x040024FF RID: 9471
		private bool _isCenterOfVisiblyActiveRoundabout;

		// Token: 0x04002500 RID: 9472
		private RoadTileConnection _visiblyActiveRoundaboutConnection = RoadTileConnection.InvalidConnection;

		// Token: 0x04002501 RID: 9473
		private bool _isVisiblyActiveRoundaboutPlaced;

		// Token: 0x04002502 RID: 9474
		private RoadTileConnection _mothballedRoundaboutConnection = RoadTileConnection.InvalidConnection;

		// Token: 0x04002503 RID: 9475
		private readonly List<AnimatedRoadTileConnectionView> _animatingRoundaboutConnections = new List<AnimatedRoadTileConnectionView>();

		// Token: 0x04002504 RID: 9476
		private Tile _tile;

		// Token: 0x04002505 RID: 9477
		private TileModel _model;

		// Token: 0x04002506 RID: 9478
		private readonly List<ClientTileEdit> _clientTileEdits = new List<ClientTileEdit>();

		// Token: 0x04002507 RID: 9479
		private bool _rebuildTile;

		// Token: 0x04002508 RID: 9480
		private bool _rebuildRoadViews;

		// Token: 0x04002509 RID: 9481
		private bool _changeHighlightView;

		// Token: 0x0400250A RID: 9482
		private bool _isHighlighted;

		// Token: 0x0400250B RID: 9483
		private RoadTileSignature _activeSignature;

		// Token: 0x0400250C RID: 9484
		private RoadView _activeRoadView;

		// Token: 0x0400250D RID: 9485
		private TileDirectionBitfield _activeConnectionDirections;

		// Token: 0x0400250E RID: 9486
		private TileDirectionBitfield _previouslyActiveConnectionDirections;

		// Token: 0x0400250F RID: 9487
		private RoadTileSignature _completeSignature;

		// Token: 0x04002510 RID: 9488
		private RoadView _completeRoadView;

		// Token: 0x04002511 RID: 9489
		private TileSelectedView _highlightView;

		// Token: 0x04002512 RID: 9490
		private TrafficLightView _trafficLightView;

		// Token: 0x04002513 RID: 9491
		private UnbuiltMotorwayView _unbuiltMotorwayView;

		// Token: 0x04002514 RID: 9492
		private RoundaboutView _roundaboutView;

		// Token: 0x04002515 RID: 9493
		private bool _isTicking;

		// Token: 0x04002516 RID: 9494
		private readonly ObserverList<TileView.IObserver> _observers = new ObserverList<TileView.IObserver>(1);

		// Token: 0x04002517 RID: 9495
		public TileViewPermanenceZoneUpdater tileViewPermanenceZoneUpdater;

		// Token: 0x02000604 RID: 1540
		public interface IObserver
		{
			// Token: 0x06002B18 RID: 11032
			void OnTileViewChanged(TileView changedTile);
		}
	}
}
