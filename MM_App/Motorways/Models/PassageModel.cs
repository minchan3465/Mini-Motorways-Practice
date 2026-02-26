using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004F2 RID: 1266
	public class PassageModel : IModel, IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x06002160 RID: 8544 RVA: 0x000853EF File Offset: 0x000835EF
		public Passage Passage
		{
			get
			{
				return this._passage;
			}
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x000853F7 File Offset: 0x000835F7
		public void Initialize(UpgradeType upgradeType, Vector2Int startCoordinates, Vector2Int firstCrossingCoordinates)
		{
			this._passage = this._scope.Get<Passage>();
			this._passage.Initialize(upgradeType, startCoordinates, firstCrossingCoordinates);
			this.ExtendOverActiveConnections();
		}

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x06002162 RID: 8546 RVA: 0x00085420 File Offset: 0x00083620
		public RoadState State
		{
			get
			{
				if (!Diagnostics.Verify(this._passage.CrossingCoordinates.Count > 0, "PassageModel has no crossing coordinates."))
				{
					return RoadState.None;
				}
				Vector2Int firstCrossingCoordinates = this._passage.CrossingCoordinates[0];
				TileDirection firstCrossingDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(firstCrossingCoordinates, this._passage.StartCoordinates);
				if (!Diagnostics.Verify(firstCrossingDirection != TileDirection.None, "PassageModel has invalid crossing coordinates."))
				{
					return RoadState.None;
				}
				Tile firstCrossingTile = this._tilemap.GetTile(firstCrossingCoordinates);
				if (!Diagnostics.Verify(firstCrossingTile != null, "PassageModel has no crossing tile."))
				{
					return RoadState.None;
				}
				return firstCrossingTile.GetTwoLaneRoadStateInDirection(firstCrossingDirection);
			}
		}

		// Token: 0x06002163 RID: 8547 RVA: 0x000854B0 File Offset: 0x000836B0
		public void ExtendOverActiveConnections()
		{
			if (!Diagnostics.Verify(!this._passage.IsComplete, "Cannot extend a complete passage."))
			{
				return;
			}
			IList<Vector2Int> crossingCoordinates = this._passage.CrossingCoordinates;
			Tile lastCrossingTile = this._tilemap.GetTile(crossingCoordinates[crossingCoordinates.Count - 1]);
			if (!Diagnostics.Verify(lastCrossingTile != null, "All of a passage's tiles should exist."))
			{
				return;
			}
			TileDirection lastCrossingDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(lastCrossingTile.Coordinates, (crossingCoordinates.Count > 1) ? crossingCoordinates[crossingCoordinates.Count - 2] : this._passage.StartCoordinates);
			while (lastCrossingTile != null)
			{
				Tile nextCrossingTile = null;
				TileDirectionBitfield activeNodeDirections = lastCrossingTile.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Ignore);
				activeNodeDirections[lastCrossingDirection] = false;
				if (activeNodeDirections.Count > 0)
				{
					lastCrossingDirection = TileUtilities.GetOppositeDirection(activeNodeDirections[0]);
					Vector2Int nextTileCoordinates = TileUtilities.GetAdjacentCoordinates(lastCrossingTile.Coordinates, activeNodeDirections[0]);
					if ((this._passage.UpgradeType == UpgradeType.Bridge && this._city.Definition.TileIsOverWater(nextTileCoordinates)) || (this._passage.UpgradeType == UpgradeType.Tunnel && this._city.Definition.TileIsUnderAMountain(nextTileCoordinates)))
					{
						nextCrossingTile = this._tilemap.GetTile(nextTileCoordinates);
						if (Diagnostics.Verify(nextCrossingTile != null, "All of a passage's tiles should exist."))
						{
							crossingCoordinates.Add(nextTileCoordinates);
						}
					}
					else
					{
						this._passage.EndCoordinates = nextTileCoordinates;
					}
				}
				lastCrossingTile = nextCrossingTile;
			}
		}

		// Token: 0x06002164 RID: 8548 RVA: 0x0008560A File Offset: 0x0008380A
		public void Reset()
		{
			this._passage = null;
		}

		// Token: 0x06002165 RID: 8549 RVA: 0x00085613 File Offset: 0x00083813
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._passage != null)
			{
				scope.Release(this._passage);
				this._passage = null;
			}
		}

		// Token: 0x04001B8A RID: 7050
		private Passage _passage;

		// Token: 0x04001B8B RID: 7051
		[Dependency]
		private IScope _scope;

		// Token: 0x04001B8C RID: 7052
		[Dependency]
		private City _city;

		// Token: 0x04001B8D RID: 7053
		[Dependency]
		private TilemapModel _tilemap;
	}
}
