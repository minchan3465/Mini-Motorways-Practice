using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000402 RID: 1026
	public class RemovePassagesEdit : TileEdit, IReleasedFromScopeHandler
	{
		// Token: 0x06001907 RID: 6407 RVA: 0x00059107 File Offset: 0x00057307
		public override void Reset()
		{
			base.Reset();
			this._passages.Clear();
		}

		// Token: 0x06001908 RID: 6408 RVA: 0x0005911C File Offset: 0x0005731C
		public void OnReleasedFromScope(IScope scope)
		{
			foreach (Passage passage in this._passages)
			{
				scope.Release(passage);
			}
			this._passages.Clear();
		}

		// Token: 0x06001909 RID: 6409 RVA: 0x0005917C File Offset: 0x0005737C
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			int num;
			for (int passageIndex = 0; passageIndex < this._passages.Count; passageIndex = num)
			{
				Passage passage = this._passages[passageIndex];
				foreach (Vector2Int crossingCoordinate in passage.CrossingCoordinates)
				{
					Tile tile = tilemap.GetTile(crossingCoordinate);
					if (tile != null)
					{
						yield return tile;
					}
				}
				IEnumerator<Vector2Int> enumerator = null;
				Vector2Int startCoordinates = passage.StartCoordinates;
				bool isStartUnique = true;
				Vector2Int endCoordinates = passage.EndCoordinates;
				bool isEndUnique = passage.IsComplete && startCoordinates != endCoordinates;
				for (int previousPassageIndex = 0; previousPassageIndex < passageIndex; previousPassageIndex++)
				{
					Passage previousPassage = this._passages[previousPassageIndex];
					isStartUnique &= (previousPassage.StartCoordinates != startCoordinates && (!previousPassage.IsComplete || previousPassage.EndCoordinates != startCoordinates));
					isEndUnique &= (previousPassage.StartCoordinates != endCoordinates && (!previousPassage.IsComplete || previousPassage.EndCoordinates != endCoordinates));
				}
				if (isStartUnique)
				{
					Tile tile2 = tilemap.GetTile(startCoordinates);
					if (tile2 != null)
					{
						yield return tile2;
					}
				}
				if (isEndUnique)
				{
					Tile tile3 = tilemap.GetTile(endCoordinates);
					if (tile3 != null)
					{
						yield return tile3;
					}
				}
				passage = null;
				endCoordinates = default(Vector2Int);
				num = passageIndex + 1;
			}
			yield break;
			yield break;
		}

		// Token: 0x0600190A RID: 6410 RVA: 0x00059194 File Offset: 0x00057394
		public override bool ApplyToAffectedTile(Tile tile)
		{
			bool isTileObstructed = true;
			foreach (Passage passage in this._passages)
			{
				if (passage.StartCoordinates == tile.Coordinates)
				{
					if (!tile.IsCenterOfRoundabout)
					{
						TileDirection startDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(passage.StartCoordinates, passage.CrossingCoordinates[0]);
						tile.SetNodeState(new RoadTileNode(startDirection, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
					}
					isTileObstructed = false;
				}
				if (passage.IsComplete && passage.EndCoordinates == tile.Coordinates)
				{
					if (!tile.IsCenterOfRoundabout)
					{
						TileDirection endDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(passage.EndCoordinates, passage.CrossingCoordinates[passage.CrossingCoordinates.Count - 1]);
						tile.SetNodeState(new RoadTileNode(endDirection, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
					}
					isTileObstructed = false;
				}
			}
			if (isTileObstructed)
			{
				foreach (TileDirection activeDirection in tile.GetTwoLaneRoads(RoadState.ActiveOrPending, Tile.MotorwayInclusion.Ignore))
				{
					tile.SetNodeState(new RoadTileNode(activeDirection, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
				}
			}
			return true;
		}

		// Token: 0x0600190B RID: 6411 RVA: 0x000592CC File Offset: 0x000574CC
		public static RemovePassagesEdit Create(IScope scope, ITilemap tilemap, Vector2Int coordinates, CityDefinition cityDefinition, Tile.TileChangePermissions changePermissions)
		{
			RemovePassagesEdit newEdit = scope.Get<RemovePassagesEdit>();
			foreach (Passage passage in Passage.GetPassagesOnTile(scope, cityDefinition, tilemap, coordinates, RoadState.ActiveOrPending))
			{
				if (passage.CanBeCleared(tilemap, changePermissions))
				{
					newEdit._passages.Add(passage);
				}
				else
				{
					scope.Release(passage);
				}
			}
			return newEdit;
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x00059348 File Offset: 0x00057548
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			int concreteCost = 0;
			int bridgesRemoved = 0;
			int tunnelsRemoved = 0;
			foreach (Passage passage in this._passages)
			{
				UpgradeType upgradeType = passage.UpgradeType;
				if (upgradeType != UpgradeType.Bridge)
				{
					if (upgradeType != UpgradeType.Tunnel)
					{
						Diagnostics.FailAssert("Passage has unrecognised upgrade type.", Array.Empty<object>());
					}
					else
					{
						tunnelsRemoved++;
					}
				}
				else
				{
					bridgesRemoved++;
				}
				concreteCost += passage.GetConcreteCost(tilemap);
			}
			if (concreteCost > 0)
			{
				upgradeDatabase.MothballUpgrade(UpgradeType.Concrete, concreteCost);
			}
			if (bridgesRemoved > 0)
			{
				upgradeDatabase.MothballUpgrade(UpgradeType.Bridge, bridgesRemoved);
			}
			if (tunnelsRemoved > 0)
			{
				upgradeDatabase.MothballUpgrade(UpgradeType.Tunnel, tunnelsRemoved);
			}
			return true;
		}

		// Token: 0x04001539 RID: 5433
		private readonly List<Passage> _passages = new List<Passage>();
	}
}
