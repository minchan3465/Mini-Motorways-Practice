using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200040A RID: 1034
	public class RestoreMothballedPassageEdit : TileEdit, IReleasedFromScopeHandler
	{
		// Token: 0x06001949 RID: 6473 RVA: 0x0005A36B File Offset: 0x0005856B
		public override void Reset()
		{
			base.Reset();
			this._passage = null;
		}

		// Token: 0x0600194A RID: 6474 RVA: 0x0005A37A File Offset: 0x0005857A
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			yield return tilemap.GetOrCreateTile(this._passage.StartCoordinates);
			foreach (Vector2Int crossingCoordinates in this._passage.CrossingCoordinates)
			{
				yield return tilemap.GetOrCreateTile(crossingCoordinates);
			}
			IEnumerator<Vector2Int> enumerator = null;
			if (this._passage.IsComplete && this._passage.StartCoordinates != this._passage.EndCoordinates)
			{
				yield return tilemap.GetOrCreateTile(this._passage.EndCoordinates);
			}
			yield break;
			yield break;
		}

		// Token: 0x0600194B RID: 6475 RVA: 0x0005A394 File Offset: 0x00058594
		public override bool ApplyToAffectedTile(Tile tile)
		{
			if (tile.Coordinates == this._passage.StartCoordinates)
			{
				if (tile.IsCenterOfRoundabout)
				{
					return true;
				}
				TileDirection passageDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(this._passage.StartCoordinates, this._passage.CrossingCoordinates[0]);
				return tile.SetNodeState(new RoadTileNode(passageDirection, RoadType.TwoLane, -1), RoadState.Pending, Tile.TileChangePermissions.Full);
			}
			else
			{
				if (!this._passage.IsComplete || !(tile.Coordinates == this._passage.EndCoordinates))
				{
					bool isSuccessful = true;
					foreach (TileDirection mothballedPassageDirection in tile.GetTwoLaneRoads(RoadState.Mothballed, Tile.MotorwayInclusion.Ignore))
					{
						isSuccessful = (tile.SetNodeState(new RoadTileNode(mothballedPassageDirection, RoadType.TwoLane, -1), RoadState.Pending, Tile.TileChangePermissions.Full) && isSuccessful);
					}
					return isSuccessful;
				}
				if (tile.IsCenterOfRoundabout)
				{
					return true;
				}
				TileDirection passageDirection2 = TileUtilities.GetDirectionBetweenAdjacentCoordinates(this._passage.EndCoordinates, this._passage.CrossingCoordinates[this._passage.CrossingCoordinates.Count - 1]);
				return tile.SetNodeState(new RoadTileNode(passageDirection2, RoadType.TwoLane, -1), RoadState.Pending, Tile.TileChangePermissions.Full);
			}
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x0005A4A7 File Offset: 0x000586A7
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			upgradeDatabase.UnmothballUpgrade(this._passage.UpgradeType, 1);
			upgradeDatabase.UnmothballUpgrade(UpgradeType.Concrete, this._passage.GetConcreteCost(tilemap));
			return true;
		}

		// Token: 0x0600194D RID: 6477 RVA: 0x0005A4D1 File Offset: 0x000586D1
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._passage != null)
			{
				scope.Release(this._passage);
				this._passage = null;
			}
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x0005A4F0 File Offset: 0x000586F0
		public static RestoreMothballedPassageEdit Create(IScope scope, ITilemap tilemap, Vector2Int originCoordinates, TileDirection direction, City city)
		{
			List<Passage> passages;
			if (city.Definition.TileIsOverWater(originCoordinates) || city.Definition.TileIsUnderAMountain(originCoordinates))
			{
				passages = Passage.GetPassagesOnTile(scope, city.Definition, tilemap, originCoordinates, RoadState.Mothballed);
			}
			else
			{
				Vector2Int firstCrossingCoordinate = TileUtilities.GetAdjacentCoordinates(originCoordinates, direction);
				passages = Passage.GetPassagesOnTile(scope, city.Definition, tilemap, firstCrossingCoordinate, RoadState.Mothballed);
			}
			RestoreMothballedPassageEdit newEdit = null;
			if (Diagnostics.Verify(passages != null && passages.Count == 1))
			{
				newEdit = scope.Get<RestoreMothballedPassageEdit>();
				newEdit._passage = passages[0];
				passages = null;
			}
			if (passages != null)
			{
				foreach (Passage passage in passages)
				{
					scope.Release(passage);
				}
			}
			return newEdit;
		}

		// Token: 0x04001565 RID: 5477
		private Passage _passage;
	}
}
