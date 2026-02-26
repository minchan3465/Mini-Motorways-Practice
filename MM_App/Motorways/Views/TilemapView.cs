using System;
using System.Collections.Generic;
using System.Linq;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005FC RID: 1532
	public class TilemapView : MonoBehaviour, IView, ISimulationObserver, IViewClientObserver, ITilemap, IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x06002A97 RID: 10903 RVA: 0x000BABDC File Offset: 0x000B8DDC
		private ClockView ClockView
		{
			get
			{
				ClockView result;
				if ((result = this._clockViewBackingField) == null)
				{
					result = (this._clockViewBackingField = this._scope.Get<ClockView>());
				}
				return result;
			}
		}

		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x06002A98 RID: 10904 RVA: 0x000BAC07 File Offset: 0x000B8E07
		public MotorwayGeometryInfo MotorwayGeometryInfo
		{
			get
			{
				return this._motorwayGeometryInfo;
			}
		}

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x06002A99 RID: 10905 RVA: 0x000BAC0F File Offset: 0x000B8E0F
		public int MotorwayCount
		{
			get
			{
				return this._motorwayViews.Count;
			}
		}

		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x06002A9A RID: 10906 RVA: 0x000BAC1C File Offset: 0x000B8E1C
		public TilemapModel TilemapModel
		{
			get
			{
				return this._tilemapModel;
			}
		}

		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x06002A9B RID: 10907 RVA: 0x000BAC24 File Offset: 0x000B8E24
		public float ViewModeOpacity
		{
			get
			{
				return this._motorwayViewModeOpacity.Value;
			}
		}

		// Token: 0x17000736 RID: 1846
		// (set) Token: 0x06002A9C RID: 10908 RVA: 0x000BAC31 File Offset: 0x000B8E31
		public TilemapView.ViewMode viewMode
		{
			set
			{
				this._viewMode = value;
				this.StartMotorwayOpacityAnimation();
			}
		}

		// Token: 0x06002A9D RID: 10909 RVA: 0x000BAC40 File Offset: 0x000B8E40
		public int GetDefaultSortOrderForMotorway(Motorway motorway)
		{
			int sortOrder;
			if (!this._motorwayDefaultSortOrder.TryGetValue(motorway.Id, out sortOrder))
			{
				return 0;
			}
			return sortOrder;
		}

		// Token: 0x06002A9E RID: 10910 RVA: 0x000BAC65 File Offset: 0x000B8E65
		public void TurnOffMotorwayTransparency()
		{
			this._alwaysOpaqueMotorways = true;
			this._motorwayViewModeOpacity.Set(1f, 0f);
			this.UpdateMotorwayOpacityShaderValue();
		}

		// Token: 0x06002A9F RID: 10911 RVA: 0x000BAC89 File Offset: 0x000B8E89
		public void TurnOnMotorwayTransparency()
		{
			this._alwaysOpaqueMotorways = false;
			this._motorwayViewModeOpacity.Set(this.MotorwayOpacityTarget, 0f);
			this.UpdateMotorwayOpacityShaderValue();
		}

		// Token: 0x06002AA0 RID: 10912 RVA: 0x000BACB0 File Offset: 0x000B8EB0
		private void UpdateMotorwayOpacityShaderValue()
		{
			Shader.SetGlobalFloat(TilemapView.ViewModeOpacityShaderId, this._motorwayViewModeOpacity.Value);
			foreach (MotorwayView motorwayView in this._motorwayViews.Values)
			{
				motorwayView.UpdateMotorwayOpacity();
			}
		}

		// Token: 0x06002AA1 RID: 10913 RVA: 0x000BAD1C File Offset: 0x000B8F1C
		private void OnMotorwayVisualParametersChanged()
		{
			if (this.ShouldMotorwaysBeTransparent)
			{
				this._motorwayViewModeOpacity.Set(this.MotorwayOpacityTarget, 0f);
				this.UpdateMotorwayOpacityShaderValue();
			}
		}

		// Token: 0x06002AA2 RID: 10914 RVA: 0x000BAD44 File Offset: 0x000B8F44
		private void StartMotorwayOpacityAnimation()
		{
			float target = this.MotorwayOpacityTarget;
			float duration = this.ShouldMotorwaysBeTransparent ? this._motorwayVisualParameters.viewModeOpacityInDuration : this._motorwayVisualParameters.viewModeOpacityOutDuration;
			float durationFactor = Mathf.Abs(target - this._motorwayViewModeOpacity.Value) / (1f - this._motorwayVisualParameters.editModeOpacity);
			this._motorwayViewModeOpacity.Start(this._motorwayViewModeOpacity.Value, target, duration * durationFactor, this._motorwayVisualParameters.viewModeOpacityAnimationFunction, 0f);
		}

		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x06002AA3 RID: 10915 RVA: 0x000BADC8 File Offset: 0x000B8FC8
		private float MotorwayOpacityTarget
		{
			get
			{
				if (!this.ShouldMotorwaysBeTransparent)
				{
					return 1f;
				}
				return this._motorwayVisualParameters.editModeOpacity;
			}
		}

		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x06002AA4 RID: 10916 RVA: 0x000BADE3 File Offset: 0x000B8FE3
		private bool ShouldMotorwaysBeTransparent
		{
			get
			{
				return !this._alwaysOpaqueMotorways && (this._viewMode == TilemapView.ViewMode.Edit || (this.ClockView != null && this.ClockView.IsVisuallyPaused));
			}
		}

		// Token: 0x06002AA5 RID: 10917 RVA: 0x000BAE15 File Offset: 0x000B9015
		private void OnClockViewVisuallyPausedChanged(bool isVisuallyPaused)
		{
			this.StartMotorwayOpacityAnimation();
		}

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x06002AA6 RID: 10918 RVA: 0x000BAE20 File Offset: 0x000B9020
		public float ScreenDistanceBetweenTiles
		{
			get
			{
				float cameraZoom = this._gameCamera.OrthographicSize;
				if (this._screenDistanceZoom == cameraZoom)
				{
					return this._screenDistanceBetweenTiles;
				}
				this._screenDistanceZoom = cameraZoom;
				this._screenDistanceBetweenTiles = (this.GetScreenPositionFromTileCoordinates(Vector2Int.right) - this.GetScreenPositionFromTileCoordinates(Vector2Int.zero)).magnitude;
				return this._screenDistanceBetweenTiles;
			}
		}

		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x06002AA7 RID: 10919 RVA: 0x000BAE7F File Offset: 0x000B907F
		// (set) Token: 0x06002AA8 RID: 10920 RVA: 0x000BAE87 File Offset: 0x000B9087
		public int DebugZoneIndex
		{
			get
			{
				return this._debugZoneIndexValue;
			}
			set
			{
				this._debugZoneIndexValue = value;
				TilemapView.OnDebugZoneIndexChanged debugZoneIndexChanged = this.DebugZoneIndexChanged;
				if (debugZoneIndexChanged == null)
				{
					return;
				}
				debugZoneIndexChanged(this._debugZoneIndexValue);
			}
		}

		// Token: 0x14000049 RID: 73
		// (add) Token: 0x06002AA9 RID: 10921 RVA: 0x000BAEA8 File Offset: 0x000B90A8
		// (remove) Token: 0x06002AAA RID: 10922 RVA: 0x000BAEE0 File Offset: 0x000B90E0
		public event TilemapView.OnDebugZoneIndexChanged DebugZoneIndexChanged;

		// Token: 0x06002AAB RID: 10923 RVA: 0x000BAF18 File Offset: 0x000B9118
		public void Initialize(TilemapModel tilemapModel)
		{
			this._tilemapModel = tilemapModel;
			this._screenDistanceZoom = -1f;
			this._simulation.Subscribe(this);
			this._viewClient.Subscribe(this);
			this.ClockView.VisuallyPausedChanged += this.OnClockViewVisuallyPausedChanged;
			this._motorwayGeometryInfo = new MotorwayGeometryInfo(this._motorwayVisualParameters);
			this._motorwayVisualParameters.OnParameterChanged += this.OnMotorwayVisualParametersChanged;
		}

		// Token: 0x06002AAC RID: 10924 RVA: 0x000BAF8E File Offset: 0x000B918E
		public void OnReleasedFromScope(IScope scope)
		{
			this._simulation.Unsubscribe(this);
			this._viewClient.Unsubscribe(this);
		}

		// Token: 0x06002AAD RID: 10925 RVA: 0x000BAFAC File Offset: 0x000B91AC
		public void Reset()
		{
			this._observers.UnsubscribeAll();
			this._tilemapModel = null;
			this._tileViews.Clear();
			this._motorwayViews.Clear();
			this._screenDistanceZoom = -1f;
			this._screenDistanceBetweenTiles = 0f;
			this._motorwayViewModeOpacity.Reset();
			this._motorwaySorter.Reset();
			this._motorwayGeometryInfo = null;
			this._shouldResortMotorways = true;
			this._motorwayDefaultSortOrder.Clear();
			this.ClockView.VisuallyPausedChanged -= this.OnClockViewVisuallyPausedChanged;
			this._alwaysOpaqueMotorways = false;
			this._motorwayVisualParameters.OnParameterChanged -= this.OnMotorwayVisualParametersChanged;
		}

		// Token: 0x06002AAE RID: 10926 RVA: 0x000BB05C File Offset: 0x000B925C
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._motorwayViewModeOpacity.IsActive)
			{
				this._motorwayViewModeOpacity.Tick(timeInterval.Delta);
				this.UpdateMotorwayOpacityShaderValue();
			}
			if (this._shouldResortMotorways && this._motorwaySorter.CanCalculateDepthSegments(this._motorwayViews))
			{
				this._motorwayGeometryInfo.ComputeGeometryInfo(this._motorwayViews);
				this._motorwaySorter.CalculateDepthSegments(this._motorwayViews, this._motorwayGeometryInfo);
				this._shouldResortMotorways = false;
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002AAF RID: 10927 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002AB0 RID: 10928 RVA: 0x000BB0D9 File Offset: 0x000B92D9
		public Motorway GetMotorway(int motorwayId)
		{
			MotorwayView motorwayView = this.GetMotorwayView(motorwayId);
			if (motorwayView == null)
			{
				return null;
			}
			return motorwayView.Motorway;
		}

		// Token: 0x06002AB1 RID: 10929 RVA: 0x000BB0ED File Offset: 0x000B92ED
		public Motorway CreateMotorway(int motorwayId, int motorwayNumber, int replacedMotorwayId)
		{
			return this.CreateMotorwayView(motorwayId, motorwayNumber, replacedMotorwayId).Motorway;
		}

		// Token: 0x06002AB2 RID: 10930 RVA: 0x000BB100 File Offset: 0x000B9300
		public MotorwayView GetMotorwayView(int motorwayId)
		{
			MotorwayView motorwayView;
			if (this._motorwayViews.TryGetValue(motorwayId, out motorwayView))
			{
				return motorwayView;
			}
			return null;
		}

		// Token: 0x06002AB3 RID: 10931 RVA: 0x000BB120 File Offset: 0x000B9320
		public MotorwayView TryGetMotorwayViewForLane(LaneModel lane)
		{
			if (lane.connection.IsUTurn)
			{
				return null;
			}
			int motorwayId = lane.connection.input.motorwayId;
			if (motorwayId == -1 || motorwayId != lane.connection.output.motorwayId)
			{
				return null;
			}
			MotorwayView motorwayViewForLane = this.GetMotorwayView(motorwayId);
			if (!Diagnostics.Verify(motorwayViewForLane.Model.startToEndLane == lane || motorwayViewForLane.Model.endToStartLane == lane))
			{
				return null;
			}
			return motorwayViewForLane;
		}

		// Token: 0x06002AB4 RID: 10932 RVA: 0x000BB198 File Offset: 0x000B9398
		private MotorwayView CreateMotorwayView(int motorwayId, int motorwayNumber, RoadState roadState, MotorwayView replacedMotorwayView = null)
		{
			if (Diagnostics.Verify(!this._motorwayViews.ContainsKey(motorwayId), "Motorway view should not already exist on creation"))
			{
				MotorwayView view = this._scope.Get<MotorwayView>();
				if (this._motorwayViews.Count <= 0)
				{
					this._motorwayViewModeOpacity.Set(this.MotorwayOpacityTarget, 0f);
					this.UpdateMotorwayOpacityShaderValue();
				}
				view.Initialize(this, motorwayId, motorwayNumber, roadState, replacedMotorwayView);
				this._motorwayViews[motorwayId] = view;
				this._viewClient.AddView(view);
				this.RecalculateDefaultMotorwaySortOrder();
				if (FeatureToggle.IsFeatureDisabled(Feature.BringMotorwaysToTopWhenEdited))
				{
					this.ResortMotorwaysOnNextTick();
				}
				return view;
			}
			return null;
		}

		// Token: 0x06002AB5 RID: 10933 RVA: 0x000BB234 File Offset: 0x000B9434
		private MotorwayView CreateMotorwayView(int motorwayId, int motorwayNumber, int replacedMotorwayId)
		{
			MotorwayView replacedMotorwayView = null;
			RoadState roadState = RoadState.Planned;
			if (replacedMotorwayId != -1)
			{
				replacedMotorwayView = this.GetMotorwayView(replacedMotorwayId);
				Motorway replacedMotorway = this.GetMotorway(replacedMotorwayId);
				if (Diagnostics.Verify(replacedMotorway != null, "Replaced motorway should not be null"))
				{
					roadState = replacedMotorway.State;
				}
			}
			return this.CreateMotorwayView(motorwayId, motorwayNumber, roadState, replacedMotorwayView);
		}

		// Token: 0x06002AB6 RID: 10934 RVA: 0x000BB27A File Offset: 0x000B947A
		private MotorwayView CreateMotorwayViewForModel(MotorwayModel model)
		{
			return this.CreateMotorwayView(model.Id, model.Number, model.State, null);
		}

		// Token: 0x06002AB7 RID: 10935 RVA: 0x000BB295 File Offset: 0x000B9495
		public Tile GetTile(Vector2Int coordinates)
		{
			TileView tileView = this.GetTileView(coordinates);
			if (tileView == null)
			{
				return null;
			}
			return tileView.Tile;
		}

		// Token: 0x06002AB8 RID: 10936 RVA: 0x000BB2A9 File Offset: 0x000B94A9
		public Tile GetOrCreateTile(Vector2Int coordinates)
		{
			TileView orCreateTileView = this.GetOrCreateTileView(coordinates);
			if (orCreateTileView == null)
			{
				return null;
			}
			return orCreateTileView.Tile;
		}

		// Token: 0x06002AB9 RID: 10937 RVA: 0x000BB2C0 File Offset: 0x000B94C0
		public TileView GetTileView(Vector2Int coordinates)
		{
			TileView tileView;
			if (this._tileViews.TryGetValue(coordinates, out tileView))
			{
				return tileView;
			}
			return null;
		}

		// Token: 0x06002ABA RID: 10938 RVA: 0x000BB2E0 File Offset: 0x000B94E0
		public TileView GetOrCreateTileView(Vector2Int coordinates)
		{
			TileView view = this.GetTileView(coordinates);
			if (view == null)
			{
				view = this._scope.Get<TileView>();
				view.Initialize(this, coordinates);
				this._tileViews[coordinates] = view;
				this._viewClient.AddView(view);
			}
			return view;
		}

		// Token: 0x06002ABB RID: 10939 RVA: 0x000BB32C File Offset: 0x000B952C
		public Vector2Int GetMouseTilePosition()
		{
			return this.GetTileCoordinatesFromWorldPosition(this.GetMouseWorldPosition());
		}

		// Token: 0x06002ABC RID: 10940 RVA: 0x000BB33A File Offset: 0x000B953A
		public Vector2 GetMouseWorldPosition()
		{
			return this.GetWorldPositionFromScreenPosition(this._inputState.Mouse.Position);
		}

		// Token: 0x06002ABD RID: 10941 RVA: 0x000BB352 File Offset: 0x000B9552
		public Vector2Int GetTouchTilePosition(int touchIndex)
		{
			return this.GetTileCoordinatesFromWorldPosition(this.GetTouchWorldPosition(touchIndex));
		}

		// Token: 0x06002ABE RID: 10942 RVA: 0x000BB364 File Offset: 0x000B9564
		public Vector2 GetTouchWorldPosition(int touchIndex)
		{
			IPointerState pointer;
			if (this._inputState.TryGetTouch(touchIndex, out pointer))
			{
				return this._gameCamera.GetWorldFromScreen(pointer.Position);
			}
			return Vector2.zero;
		}

		// Token: 0x06002ABF RID: 10943 RVA: 0x000BB39D File Offset: 0x000B959D
		public Vector2 GetWorldPositionFromScreenPosition(Vector2 screenPosition)
		{
			return this._gameCamera.GetWorldFromScreen(screenPosition);
		}

		// Token: 0x06002AC0 RID: 10944 RVA: 0x000BB3B0 File Offset: 0x000B95B0
		public Vector2 GetScreenPositionFromTileCoordinates(Vector2Int tileCoordinates)
		{
			return this._gameCamera.GetScreenFromWorld(TilemapView.GetWorldPositionForCoordinates(tileCoordinates));
		}

		// Token: 0x06002AC1 RID: 10945 RVA: 0x000BB3C3 File Offset: 0x000B95C3
		public static Vector3 GetWorldPositionForCoordinates(Vector2Int coordinates)
		{
			return (Vector3)TilemapModel.GetWorldPositionForCoordinates(coordinates);
		}

		// Token: 0x06002AC2 RID: 10946 RVA: 0x000BB3D0 File Offset: 0x000B95D0
		public Vector2Int GetTileCoordinatesFromWorldPosition(Vector2 worldPosition)
		{
			return new Vector2Int(Mathf.RoundToInt(worldPosition.x / (float)TilemapModel.TileWidth), Mathf.RoundToInt(worldPosition.y / (float)TilemapModel.TileWidth));
		}

		// Token: 0x06002AC3 RID: 10947 RVA: 0x000BB405 File Offset: 0x000B9605
		public Vector2Int GetTileCoordinatesFromScreenPosition(Vector2 screenPosition)
		{
			return this.GetTileCoordinatesFromWorldPosition(this.GetWorldPositionFromScreenPosition(screenPosition));
		}

		// Token: 0x06002AC4 RID: 10948 RVA: 0x000BB414 File Offset: 0x000B9614
		private List<int> GetActiveMotorwayNumbers()
		{
			List<int> activeMotorwayNumbers = new List<int>();
			foreach (MotorwayView view in this._motorwayViews.Values)
			{
				if ((view.Motorway.State & RoadState.VisiblyActive) > RoadState.None && view.Motorway.Number != 0)
				{
					activeMotorwayNumbers.Add(view.Motorway.Number);
				}
			}
			foreach (TileView tileView in this._tileViews.Values)
			{
				if (tileView.Tile.UnbuiltMotorwayNumber != 0)
				{
					activeMotorwayNumbers.Add(tileView.Tile.UnbuiltMotorwayNumber);
				}
			}
			return activeMotorwayNumbers;
		}

		// Token: 0x06002AC5 RID: 10949 RVA: 0x000BB500 File Offset: 0x000B9700
		public int GetLowestAvailableMotorwayNumber()
		{
			List<int> activeMotorwayNumbers = this.GetActiveMotorwayNumbers();
			for (int motorwayNumber = 1; motorwayNumber <= activeMotorwayNumbers.Count + 1; motorwayNumber++)
			{
				if (!activeMotorwayNumbers.Contains(motorwayNumber))
				{
					return motorwayNumber;
				}
			}
			return 1;
		}

		// Token: 0x06002AC6 RID: 10950 RVA: 0x000BB534 File Offset: 0x000B9734
		public void OnModelAdded(ISimulation simulation, IModel model, Fix64 timestamp)
		{
			TileModel newTileModel = model as TileModel;
			if (newTileModel != null)
			{
				this.GetOrCreateTileView(newTileModel.Coordinates).SetModel(newTileModel);
				return;
			}
			MotorwayModel newMotorwayModel = model as MotorwayModel;
			if (newMotorwayModel != null)
			{
				MotorwayView motorwayView = this.GetMotorwayView(newMotorwayModel.Id);
				if (motorwayView == null)
				{
					motorwayView = this.CreateMotorwayViewForModel(newMotorwayModel);
				}
				motorwayView.SetModel(newMotorwayModel);
			}
		}

		// Token: 0x06002AC7 RID: 10951 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnModelRemoved(ISimulation simulation, IModel model, Fix64 timestamp)
		{
		}

		// Token: 0x06002AC8 RID: 10952 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnViewAdded(IClient client, IView view)
		{
		}

		// Token: 0x06002AC9 RID: 10953 RVA: 0x000BB590 File Offset: 0x000B9790
		public void OnViewRemoved(IClient client, IView view)
		{
			MotorwayView motorwayView = view as MotorwayView;
			if (motorwayView != null)
			{
				this._motorwayViews.Remove(motorwayView.Motorway.Id);
				this.RecalculateDefaultMotorwaySortOrder();
				this.ResortMotorwaysOnNextTick();
			}
		}

		// Token: 0x06002ACA RID: 10954 RVA: 0x000BB5CC File Offset: 0x000B97CC
		public ClientTileEdit GenerateClientTileEditAndAddEditToViews(TileEdit edit, bool isDraft)
		{
			ClientTileEdit clientTileEdit = new ClientTileEdit();
			clientTileEdit.edit = edit;
			clientTileEdit.isDraft = isDraft;
			foreach (Motorway affectedMotorway in edit.GetAffectedMotorways(this))
			{
				MotorwayView motorwayView = this.GetMotorwayView(affectedMotorway.Id);
				if (Diagnostics.Verify(motorwayView != null, "Expected to find view for motorway {0}.", affectedMotorway.Id))
				{
					motorwayView.AddEdit(clientTileEdit);
				}
			}
			foreach (Tile affectedTile in edit.GetAffectedTiles(this))
			{
				if (Diagnostics.Verify(affectedTile != null, "Expected tile should never be null at this point"))
				{
					TileView tileView = this.GetTileView(affectedTile.Coordinates);
					if (Diagnostics.Verify(tileView != null, "Expected to find view for tile at {0}.", affectedTile.Coordinates))
					{
						tileView.AddEdit(clientTileEdit);
						foreach (TilemapView.IObserver observer in this._observers)
						{
							observer.OnClientTileChanged(affectedTile);
						}
					}
				}
			}
			return clientTileEdit;
		}

		// Token: 0x06002ACB RID: 10955 RVA: 0x000BB704 File Offset: 0x000B9904
		public void RecalculateDefaultMotorwaySortOrder()
		{
			this._motorwayDefaultSortOrder.Clear();
			if (this._motorwayViews.Count == 0)
			{
				return;
			}
			int[] sortOrder = new int[this._motorwayViews.Count];
			for (int motorwayViewIndex = 0; motorwayViewIndex < this._motorwayViews.Values.Count; motorwayViewIndex++)
			{
				MotorwayView motorwayView = this._motorwayViews.Values.ElementAt(motorwayViewIndex);
				sortOrder[motorwayViewIndex] = motorwayView.Motorway.Id;
			}
			Array.Sort<int>(sortOrder, delegate(int motorwayIdA, int motorwayIdB)
			{
				Motorway motorwayA = this.GetMotorway(motorwayIdA);
				Motorway motorwayB = this.GetMotorway(motorwayIdB);
				bool motorwayAVisiblyActive = (motorwayA.State & RoadState.VisiblyActive) > RoadState.None;
				bool motorwayBVisiblyActive = (motorwayB.State & RoadState.VisiblyActive) > RoadState.None;
				if (!motorwayAVisiblyActive && !motorwayBVisiblyActive)
				{
					return motorwayIdA - motorwayIdB;
				}
				if (motorwayAVisiblyActive && !motorwayBVisiblyActive)
				{
					return 1;
				}
				if (!motorwayAVisiblyActive)
				{
					return -1;
				}
				return motorwayA.Number - motorwayB.Number;
			});
			for (int sortOrderIndex = 0; sortOrderIndex < sortOrder.Length; sortOrderIndex++)
			{
				int motorwayId = sortOrder[sortOrderIndex];
				this._motorwayDefaultSortOrder[motorwayId] = sortOrderIndex + 1;
			}
		}

		// Token: 0x06002ACC RID: 10956 RVA: 0x000BB7AB File Offset: 0x000B99AB
		public void ResortMotorwaysOnNextTick()
		{
			this._shouldResortMotorways = true;
		}

		// Token: 0x06002ACD RID: 10957 RVA: 0x000BB7B4 File Offset: 0x000B99B4
		public void Subscribe(TilemapView.IObserver observer)
		{
			this._observers.Subscribe(observer);
		}

		// Token: 0x06002ACE RID: 10958 RVA: 0x000BB7C2 File Offset: 0x000B99C2
		public bool Unsubscribe(TilemapView.IObserver observer)
		{
			return this._observers.Unsubscribe(observer);
		}

		// Token: 0x040024CD RID: 9421
		[Dependency]
		private IScope _scope;

		// Token: 0x040024CE RID: 9422
		[Dependency]
		private MotorwaysThemeDatabase _themeDatabase;

		// Token: 0x040024CF RID: 9423
		[Dependency]
		private MotorwayVisualParameters _motorwayVisualParameters;

		// Token: 0x040024D0 RID: 9424
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x040024D1 RID: 9425
		[Dependency]
		private InputState _inputState;

		// Token: 0x040024D2 RID: 9426
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x040024D3 RID: 9427
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x040024D4 RID: 9428
		private ClockView _clockViewBackingField;

		// Token: 0x040024D5 RID: 9429
		private MotorwayGeometryInfo _motorwayGeometryInfo;

		// Token: 0x040024D6 RID: 9430
		private readonly MotorwaySorter _motorwaySorter = new MotorwaySorter();

		// Token: 0x040024D7 RID: 9431
		private bool _shouldResortMotorways = true;

		// Token: 0x040024D8 RID: 9432
		private float _screenDistanceZoom = -1f;

		// Token: 0x040024D9 RID: 9433
		private float _screenDistanceBetweenTiles;

		// Token: 0x040024DA RID: 9434
		private TilemapModel _tilemapModel;

		// Token: 0x040024DB RID: 9435
		private Dictionary<int, MotorwayView> _motorwayViews = new Dictionary<int, MotorwayView>();

		// Token: 0x040024DC RID: 9436
		private Dictionary<Vector2Int, TileView> _tileViews = new Dictionary<Vector2Int, TileView>();

		// Token: 0x040024DD RID: 9437
		private Dictionary<int, int> _motorwayDefaultSortOrder = new Dictionary<int, int>();

		// Token: 0x040024DE RID: 9438
		private TilemapView.ViewMode _viewMode;

		// Token: 0x040024DF RID: 9439
		private readonly TweenFloat _motorwayViewModeOpacity = new TweenFloat();

		// Token: 0x040024E0 RID: 9440
		private static readonly int ViewModeOpacityShaderId = Shader.PropertyToID("_ViewModeOpacity");

		// Token: 0x040024E1 RID: 9441
		private const float MaxMotorwayOpacity = 1f;

		// Token: 0x040024E2 RID: 9442
		private bool _alwaysOpaqueMotorways;

		// Token: 0x040024E3 RID: 9443
		private int _debugZoneIndexValue;

		// Token: 0x040024E5 RID: 9445
		[Serialize(false, null)]
		private readonly ObserverList<TilemapView.IObserver> _observers = new ObserverList<TilemapView.IObserver>(1);

		// Token: 0x020005FD RID: 1533
		public enum ViewMode
		{
			// Token: 0x040024E7 RID: 9447
			Normal,
			// Token: 0x040024E8 RID: 9448
			Edit
		}

		// Token: 0x020005FE RID: 1534
		// (Invoke) Token: 0x06002AD3 RID: 10963
		public delegate void OnDebugZoneIndexChanged(int debugZoneIndex);

		// Token: 0x020005FF RID: 1535
		public interface IObserver
		{
			// Token: 0x06002AD6 RID: 10966
			void OnClientTileChanged(Tile tile);
		}

		// Token: 0x02000600 RID: 1536
		public class Builder : IViewBuilder
		{
			// Token: 0x06002AD7 RID: 10967 RVA: 0x000BB8A8 File Offset: 0x000B9AA8
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				if (typeof(TilemapModel).IsAssignableFrom(model.GetType()))
				{
					TilemapView tilemapView = client.Scope.Get<TilemapView>();
					tilemapView.Initialize(model as TilemapModel);
					client.AddView(tilemapView);
				}
			}
		}
	}
}
