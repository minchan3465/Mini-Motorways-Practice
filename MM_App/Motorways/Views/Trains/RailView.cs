using System;
using System.Collections.Generic;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Views.Trains
{
	// Token: 0x02000616 RID: 1558
	public class RailView : MonoBehaviour, IView, RailTileModel.IObserver, IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x1700075F RID: 1887
		// (get) Token: 0x06002BAD RID: 11181 RVA: 0x000C1034 File Offset: 0x000BF234
		public int LineSegmentCount
		{
			get
			{
				return this._lineSegments.Count;
			}
		}

		// Token: 0x17000760 RID: 1888
		// (get) Token: 0x06002BAE RID: 11182 RVA: 0x000C1041 File Offset: 0x000BF241
		public List<LineSegment> LineSegments
		{
			get
			{
				return this._lineSegments;
			}
		}

		// Token: 0x17000761 RID: 1889
		// (get) Token: 0x06002BAF RID: 11183 RVA: 0x000C1049 File Offset: 0x000BF249
		public RailTileModel Model
		{
			get
			{
				return this._tileModel;
			}
		}

		// Token: 0x17000762 RID: 1890
		// (get) Token: 0x06002BB0 RID: 11184 RVA: 0x000C1051 File Offset: 0x000BF251
		public BridgeSpouts BridgeSpouts
		{
			get
			{
				return this._bridgeSpouts;
			}
		}

		// Token: 0x06002BB1 RID: 11185 RVA: 0x000C105C File Offset: 0x000BF25C
		private void Initialize(RailTileModel railTileModel)
		{
			this._tileModel = railTileModel;
			this._tileModel.Subscribe(this);
			this.DisableBridgeVisuals();
			Vector2Fixed tileCentre = TilemapModel.GetWorldPositionForCoordinates(this._tileModel.Coordinates);
			base.transform.position = (Vector3)tileCentre;
			RailTileDefinition definition = this._railTileAtlas.GetDefinition(this._tileModel.TileModel.Tile.RailConnection);
			if (Diagnostics.Verify(definition != null))
			{
				List<Vector2Fixed> logicalPath = definition.path.GetLogicalPoints(tileCentre);
				for (int lineIndex = 0; lineIndex < logicalPath.Count - 1; lineIndex++)
				{
					this._lineSegments.Add(new LineSegment((Vector2)logicalPath[lineIndex], (Vector2)logicalPath[lineIndex + 1]));
				}
			}
			if (this._city.Definition.TileIsOverWater(this._tileModel.Coordinates))
			{
				if (this._tileModel.PreviousRailModel != null && !this._city.Definition.TileIsOverWater(this._tileModel.PreviousRailModel.Coordinates))
				{
					TileDirection inputDirection = this._tileModel.TileModel.Tile.RailConnection.input;
					this._bridgeSpouts.SetSpoutActiveInDirection(inputDirection, UpgradeType.Bridge);
				}
				this.SetBridgeActive();
			}
			else if (this._tileModel.PreviousRailModel != null && this._city.Definition.TileIsOverWater(this._tileModel.PreviousRailModel.Coordinates))
			{
				RailView railView = this._viewIndex.GetRailView(this._tileModel.PreviousRailModel);
				TileDirection inputDirection2 = this._tileModel.TileModel.Tile.RailConnection.output;
				railView.BridgeSpouts.SetSpoutActiveInDirection(inputDirection2, UpgradeType.Bridge);
			}
			this._viewIndex.AddRailView(this);
		}

		// Token: 0x06002BB2 RID: 11186 RVA: 0x000C1214 File Offset: 0x000BF414
		private void SetBridgeActive()
		{
			this._centerVisual.gameObject.SetActive(true);
			this._firstOutline.gameObject.SetActive(true);
			this._secondOutline.gameObject.SetActive(true);
			switch (this._tileModel.TileModel.Tile.RailConnection.input)
			{
			case TileDirection.None:
				break;
			case TileDirection.North:
			case TileDirection.South:
				this._centerVisual.transform.rotation = Quaternion.Euler(0f, 90f, -90f);
				return;
			case TileDirection.NorthEast:
			case TileDirection.SouthWest:
				this._centerVisual.transform.rotation = Quaternion.Euler(225f, 90f, -90f);
				return;
			case TileDirection.East:
			case TileDirection.West:
				this._centerVisual.transform.rotation = Quaternion.Euler(270f, 90f, -90f);
				return;
			case TileDirection.SouthEast:
			case TileDirection.NorthWest:
				this._centerVisual.transform.rotation = Quaternion.Euler(45f, 90f, -90f);
				break;
			default:
				return;
			}
		}

		// Token: 0x06002BB3 RID: 11187 RVA: 0x000C132F File Offset: 0x000BF52F
		private void DisableBridgeVisuals()
		{
			this._centerVisual.gameObject.SetActive(false);
			this._firstOutline.gameObject.SetActive(false);
			this._secondOutline.gameObject.SetActive(false);
			this._bridgeSpouts.DisableAllSpouts();
		}

		// Token: 0x06002BB4 RID: 11188 RVA: 0x000020AA File Offset: 0x000002AA
		public TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			return TickResult.StopTicking;
		}

		// Token: 0x06002BB5 RID: 11189 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002BB6 RID: 11190 RVA: 0x000C1370 File Offset: 0x000BF570
		private void OnDrawGizmosSelected()
		{
			if (this._lineSegments != null && this._lineSegments.Count > 0)
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawCube(this._lineSegments[0].Start, new Vector3(0.2f, 0.2f, 0.2f));
			}
		}

		// Token: 0x06002BB7 RID: 11191 RVA: 0x000C13D0 File Offset: 0x000BF5D0
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.magenta;
			foreach (LineSegment lineSegment in this._lineSegments)
			{
				Gizmos.DrawLine(lineSegment.Start, lineSegment.End);
			}
			if (this._tileModel == null)
			{
				return;
			}
			Gizmos.color = ((this._tileModel.SignalState == TrainSignalState.Open) ? Color.green : Color.red);
			Gizmos.DrawCube((Vector3)this._tileModel.TileModel.WorldPosition, new Vector3(0.2f, 0.2f, 0.2f));
		}

		// Token: 0x06002BB8 RID: 11192 RVA: 0x000C149C File Offset: 0x000BF69C
		public void Reset()
		{
			this._tileModel = null;
			this._lineSegments.Clear();
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this.DisableBridgeVisuals();
		}

		// Token: 0x06002BB9 RID: 11193 RVA: 0x000C14D6 File Offset: 0x000BF6D6
		public void OnReleasedFromScope(IScope scope)
		{
			this._viewIndex.RemoveRailView(this);
		}

		// Token: 0x040025DE RID: 9694
		private RailTileModel _tileModel;

		// Token: 0x040025DF RID: 9695
		private readonly List<LineSegment> _lineSegments = new List<LineSegment>();

		// Token: 0x040025E0 RID: 9696
		[Dependency]
		private RailTileAtlas _railTileAtlas;

		// Token: 0x040025E1 RID: 9697
		[Dependency]
		private ViewIndex _viewIndex;

		// Token: 0x040025E2 RID: 9698
		[Dependency]
		private City _city;

		// Token: 0x040025E3 RID: 9699
		[SerializeField]
		private BridgeSpouts _bridgeSpouts;

		// Token: 0x040025E4 RID: 9700
		[SerializeField]
		private GameObject _centerVisual;

		// Token: 0x040025E5 RID: 9701
		[SerializeField]
		private GameObject _firstOutline;

		// Token: 0x040025E6 RID: 9702
		[SerializeField]
		private GameObject _secondOutline;

		// Token: 0x02000617 RID: 1559
		public class Builder : IViewBuilder
		{
			// Token: 0x06002BBB RID: 11195 RVA: 0x000C14F8 File Offset: 0x000BF6F8
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				RailView railView = client.Scope.Get<RailView>();
				railView.Initialize(model as RailTileModel);
				client.AddView(railView);
			}
		}
	}
}
