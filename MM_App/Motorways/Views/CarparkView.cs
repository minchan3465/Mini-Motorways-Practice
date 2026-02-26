using System;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Motorways.Processes;
using Motorways.Views.MeshGeneration;
using Server;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200058B RID: 1419
	[SelectionBase]
	public class CarparkView : MonoBehaviour, IView, CarparkModel.IObserver, IReleasedFromScopeHandler, IReusable
	{
		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x06002737 RID: 10039 RVA: 0x000A7931 File Offset: 0x000A5B31
		public CarparkModel Model
		{
			get
			{
				return this._model;
			}
		}

		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x06002738 RID: 10040 RVA: 0x000A7939 File Offset: 0x000A5B39
		private GameObject SecondDestinationOutline
		{
			get
			{
				if (!this._isReversedLayout)
				{
					return this._secondDestinationOutline;
				}
				return this._secondDestinationOutlineReversed;
			}
		}

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x06002739 RID: 10041 RVA: 0x000A7950 File Offset: 0x000A5B50
		private GameObject SecondStationOutline
		{
			get
			{
				if (!this._isReversedLayout)
				{
					return this._secondStationOutline;
				}
				return this._secondStationOutlineReversed;
			}
		}

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x0600273A RID: 10042 RVA: 0x000A7967 File Offset: 0x000A5B67
		private GameObject ExtraParkingSpaceLine
		{
			get
			{
				if (!this._isReversedLayout)
				{
					return this._extraParkingSpaceLine;
				}
				return this._extraParkingSpaceLineReversed;
			}
		}

		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x0600273B RID: 10043 RVA: 0x000A797E File Offset: 0x000A5B7E
		public Transform BoatDockingTransform
		{
			get
			{
				return this._boatDockingTransform;
			}
		}

		// Token: 0x0600273C RID: 10044 RVA: 0x000A7988 File Offset: 0x000A5B88
		public Bounds GetBounds()
		{
			float tileWidth = (float)TilemapModel.TileWidth;
			Vector3 halfTileDimensions = new Vector3(tileWidth, tileWidth, 0f) * 0.5f;
			Vector2Int firstTileCoordinates = this._model.TileModels[0].Coordinates;
			Vector3 firstPosition = new Vector3((float)firstTileCoordinates.x * tileWidth, (float)firstTileCoordinates.y * tileWidth, base.transform.position.z);
			Bounds result = new Bounds(firstPosition, Vector3.zero);
			foreach (TileModel tileModel in this._model.TileModels)
			{
				Vector2Int tileCoordinates = tileModel.Coordinates;
				Vector3 a = new Vector3((float)tileCoordinates.x * tileWidth, (float)tileCoordinates.y * tileWidth, firstPosition.z);
				Vector3 tileMinPos = a - halfTileDimensions;
				result.Encapsulate(tileMinPos);
				Vector3 tilMaxPos = a + halfTileDimensions;
				result.Encapsulate(tilMaxPos);
			}
			return result;
		}

		// Token: 0x0600273D RID: 10045 RVA: 0x000A7A98 File Offset: 0x000A5C98
		public Bounds GetEmptyDestinationSlotBounds()
		{
			if (!Diagnostics.Verify(this._model.destinationOffsets.Count == 2 && this._model.destinations.Count == 1, "There should be exactly one empty destination slot available."))
			{
				return default(Bounds);
			}
			Vector2Int offset = this._model.destinationOffsets[this._model.destinations.Count];
			Vector2Int position = this._model.origin + offset;
			float tileWidth = (float)TilemapModel.TileWidth;
			float halfTileWidth = (float)TilemapModel.HalfTileWidth;
			Vector2Int footprint = BuildingSpawningProcess.DestinationFootprint;
			Vector2 size = new Vector2(tileWidth * (float)footprint.x, tileWidth * (float)footprint.y);
			return new Bounds(new Vector2((float)position.x * tileWidth - halfTileWidth, (float)position.y * tileWidth - halfTileWidth) + size / 2f, size);
		}

		// Token: 0x0600273E RID: 10046 RVA: 0x000022F5 File Offset: 0x000004F5
		private void Awake()
		{
		}

		// Token: 0x0600273F RID: 10047 RVA: 0x000A7B94 File Offset: 0x000A5D94
		private void Initialize(CarparkModel model)
		{
			this._model = model;
			this._model.Subscribe(this);
			Transform carparkTransform = base.transform;
			carparkTransform.position = TilemapView.GetWorldPositionForCoordinates(model.TopLeftCarparkTileCoordinate);
			switch (model.carparkSide)
			{
			case TileDirection.North:
				if (model.SupportsTwoDestinations)
				{
					this._isReversedLayout = true;
					goto IL_B4;
				}
				goto IL_B4;
			case TileDirection.East:
				if (model.SupportsTwoDestinations)
				{
					carparkTransform.localEulerAngles = new Vector3(0f, 0f, -90f);
					this._isReversedLayout = true;
					goto IL_B4;
				}
				goto IL_B4;
			case TileDirection.South:
				goto IL_B4;
			case TileDirection.West:
				carparkTransform.localEulerAngles = new Vector3(0f, 0f, -90f);
				goto IL_B4;
			}
			throw new ArgumentOutOfRangeException();
			IL_B4:
			this.AddToCombinedMesh(this._model);
			this.SetupSecondDestinationOutline();
			this.ExtraParkingSpaceLine.SetActive(true);
		}

		// Token: 0x06002740 RID: 10048 RVA: 0x000A7C74 File Offset: 0x000A5E74
		private void SetupSecondDestinationOutline()
		{
			bool showSecondDestinationOutline = this._model.SupportsTwoDestinations && this._model.destinations.Count < 2;
			bool isStation = this._model.destinations.Count == 1 && this._model.destinations[0].IsTrainStation;
			this.SecondDestinationOutline.SetActive(showSecondDestinationOutline && !isStation);
			this.SecondStationOutline.SetActive(showSecondDestinationOutline && isStation);
		}

		// Token: 0x06002741 RID: 10049 RVA: 0x000A7CF8 File Offset: 0x000A5EF8
		private void AddToCombinedMesh(CarparkModel model)
		{
			Mesh mesh = null;
			if (model.supportsBoats)
			{
				mesh = this._carparkMeshCombiner.BoatTerminalCarpark;
			}
			else if (model.SupportsTwoDestinations)
			{
				if (this._isReversedLayout)
				{
					mesh = this._carparkMeshCombiner.ReversedHorizontalDoubleCarpark;
				}
				else
				{
					mesh = this._carparkMeshCombiner.HorizontalDoubleCarpark;
				}
			}
			else if (model.entranceAtTopLeft)
			{
				mesh = this._carparkMeshCombiner.HorizontalSingleCarparkLeftEntrance;
			}
			else if (model.entranceAtBottomRight)
			{
				mesh = this._carparkMeshCombiner.HorizontalSingleCarparkRightEntrance;
			}
			if (mesh != null)
			{
				this._combinedMeshHandle = this._combinedMeshView.AddMesh(CombinedMeshView.CombinedMeshType.Carpark, mesh, base.transform.localToWorldMatrix);
			}
		}

		// Token: 0x06002742 RID: 10050 RVA: 0x000A7D9A File Offset: 0x000A5F9A
		public void OnDestinationAdded()
		{
			this.SetupSecondDestinationOutline();
		}

		// Token: 0x06002743 RID: 10051 RVA: 0x000A7DA2 File Offset: 0x000A5FA2
		public void OnCarparkRemoved(CarparkModel carparkModel)
		{
			this._isPendingDeletion = true;
			this._viewClient.ResumeTickingView(this);
			this._combinedMeshView.RemoveMesh(this._combinedMeshHandle);
		}

		// Token: 0x06002744 RID: 10052 RVA: 0x000A7DC8 File Offset: 0x000A5FC8
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._model != null)
			{
				this._model.Unsubscribe(this);
				this._model = null;
			}
		}

		// Token: 0x06002745 RID: 10053 RVA: 0x000A7DE8 File Offset: 0x000A5FE8
		public void Reset()
		{
			this.SecondDestinationOutline.SetActive(false);
			this.SecondStationOutline.SetActive(false);
			this.ExtraParkingSpaceLine.SetActive(false);
			this._model = null;
			this._isPendingDeletion = false;
			this._isReversedLayout = false;
			Transform transform = base.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		// Token: 0x06002746 RID: 10054 RVA: 0x000A7E54 File Offset: 0x000A6054
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (!this._isPendingDeletion)
			{
				return TickResult.StopTicking;
			}
			return TickResult.Destroy;
		}

		// Token: 0x06002747 RID: 10055 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x04002143 RID: 8515
		private CarparkModel _model;

		// Token: 0x04002144 RID: 8516
		[Dependency]
		private TilemapView _tilemap;

		// Token: 0x04002145 RID: 8517
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x04002146 RID: 8518
		[Dependency]
		private CombinedMeshView _combinedMeshView;

		// Token: 0x04002147 RID: 8519
		[Dependency]
		private CarparkMeshCombiner _carparkMeshCombiner;

		// Token: 0x04002148 RID: 8520
		private CombinedMeshView.Handle _combinedMeshHandle;

		// Token: 0x04002149 RID: 8521
		private bool _isPendingDeletion;

		// Token: 0x0400214A RID: 8522
		private bool _isReversedLayout;

		// Token: 0x0400214B RID: 8523
		[SerializeField]
		private GameObject _secondDestinationOutline;

		// Token: 0x0400214C RID: 8524
		[SerializeField]
		private GameObject _secondDestinationOutlineReversed;

		// Token: 0x0400214D RID: 8525
		[SerializeField]
		private GameObject _secondStationOutline;

		// Token: 0x0400214E RID: 8526
		[SerializeField]
		private GameObject _secondStationOutlineReversed;

		// Token: 0x0400214F RID: 8527
		[SerializeField]
		private GameObject _extraParkingSpaceLine;

		// Token: 0x04002150 RID: 8528
		[SerializeField]
		private GameObject _extraParkingSpaceLineReversed;

		// Token: 0x04002151 RID: 8529
		[SerializeField]
		private Transform _boatDockingTransform;

		// Token: 0x0200058C RID: 1420
		public class Builder : IViewBuilder
		{
			// Token: 0x06002749 RID: 10057 RVA: 0x000A7E64 File Offset: 0x000A6064
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				CarparkView carparkView = client.Scope.Get<CarparkView>();
				carparkView.Initialize(model as CarparkModel);
				client.AddView(carparkView);
			}
		}
	}
}
