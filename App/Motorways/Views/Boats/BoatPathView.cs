using System;
using System.Collections.Generic;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Views.Boats
{
	// Token: 0x0200062D RID: 1581
	public class BoatPathView : MonoBehaviour, IView, BoatPathTileModel.IObserver, IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x1700076E RID: 1902
		// (get) Token: 0x06002C16 RID: 11286 RVA: 0x000C3670 File Offset: 0x000C1870
		public int LineSegmentCount
		{
			get
			{
				return this._lineSegments.Count;
			}
		}

		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x06002C17 RID: 11287 RVA: 0x000C367D File Offset: 0x000C187D
		public List<LineSegment> LineSegments
		{
			get
			{
				return this._lineSegments;
			}
		}

		// Token: 0x17000770 RID: 1904
		// (get) Token: 0x06002C18 RID: 11288 RVA: 0x000C3685 File Offset: 0x000C1885
		public BoatPathTileModel Model
		{
			get
			{
				return this._tileModel;
			}
		}

		// Token: 0x06002C19 RID: 11289 RVA: 0x000C3690 File Offset: 0x000C1890
		private void Initialize(BoatPathTileModel boatPathModel)
		{
			this._tileModel = boatPathModel;
			this._tileModel.Subscribe(this);
			Vector2Fixed tileCentre = TilemapModel.GetWorldPositionForCoordinates(this._tileModel.Coordinates);
			base.transform.position = (Vector3)tileCentre;
			BoatPathTileDefinition definition = this._boatPathTileAtlas.GetDefinition(this._tileModel.TileModel.Tile.BoatPathConnection);
			if (Diagnostics.Verify(definition != null))
			{
				List<Vector2Fixed> logicalPath = definition.path.GetLogicalPoints(tileCentre);
				for (int lineIndex = 0; lineIndex < logicalPath.Count - 1; lineIndex++)
				{
					this._lineSegments.Add(new LineSegment((Vector2)logicalPath[lineIndex], (Vector2)logicalPath[lineIndex + 1]));
				}
			}
			this._viewIndex.AddBoatPathView(this);
		}

		// Token: 0x06002C1A RID: 11290 RVA: 0x000020AA File Offset: 0x000002AA
		public TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			return TickResult.StopTicking;
		}

		// Token: 0x06002C1B RID: 11291 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002C1C RID: 11292 RVA: 0x000C3754 File Offset: 0x000C1954
		public void Reset()
		{
			this._tileModel = null;
			this._lineSegments.Clear();
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
		}

		// Token: 0x06002C1D RID: 11293 RVA: 0x000C3788 File Offset: 0x000C1988
		public void OnReleasedFromScope(IScope scope)
		{
			this._viewIndex.RemoveBoatPathView(this);
		}

		// Token: 0x04002652 RID: 9810
		private BoatPathTileModel _tileModel;

		// Token: 0x04002653 RID: 9811
		private readonly List<LineSegment> _lineSegments = new List<LineSegment>();

		// Token: 0x04002654 RID: 9812
		[Dependency]
		private BoatPathTileAtlas _boatPathTileAtlas;

		// Token: 0x04002655 RID: 9813
		[Dependency]
		private ViewIndex _viewIndex;

		// Token: 0x0200062E RID: 1582
		public class Builder : IViewBuilder
		{
			// Token: 0x06002C1F RID: 11295 RVA: 0x000C37AC File Offset: 0x000C19AC
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				BoatPathView boatPathView = client.Scope.Get<BoatPathView>();
				boatPathView.Initialize(model as BoatPathTileModel);
				client.AddView(boatPathView);
			}
		}
	}
}
