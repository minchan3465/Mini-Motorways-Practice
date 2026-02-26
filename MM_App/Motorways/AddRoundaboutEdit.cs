using System;
using System.Collections.Generic;
using System.Linq;
using Factory;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003F1 RID: 1009
	public class AddRoundaboutEdit : TileEdit
	{
		// Token: 0x0600187B RID: 6267 RVA: 0x00057583 File Offset: 0x00055783
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			foreach (Vector2Int coordinatesOffset in Roundabout.GetCoordinatesOffsets())
			{
				yield return tilemap.GetOrCreateTile(this._originCoordinates + coordinatesOffset);
			}
			IEnumerator<Vector2Int> enumerator = null;
			foreach (Vector2Int neighborCoordinatesOffset in Roundabout.GetNeighborCoordinatesOffsets())
			{
				Tile neighborTile = tilemap.GetTile(this._originCoordinates + neighborCoordinatesOffset);
				if (neighborTile != null)
				{
					yield return neighborTile;
				}
			}
			enumerator = null;
			yield return tilemap.GetOrCreateTile(this._originCoordinates + Roundabout.GetCenterOffset());
			yield break;
			yield break;
		}

		// Token: 0x0600187C RID: 6268 RVA: 0x0005759C File Offset: 0x0005579C
		public override bool ApplyToAffectedTile(Tile tile)
		{
			Vector2Int offset = tile.Coordinates - this._originCoordinates;
			if (Roundabout.IsCoordinatesOffsetInRoundabout(offset))
			{
				return tile.SetRoundaboutState(Roundabout.GetConnectionForCoordinatesOffset(offset), this._isReplacingMothballedRoundabout ? RoadState.Active : RoadState.Planned);
			}
			if (tile.Coordinates == this._originCoordinates)
			{
				tile.IsCenterOfRoundabout = true;
				if (this._isReplacingMothballedRoundabout)
				{
					foreach (TileDirection activeNodeDirection in tile.GetTwoLaneRoads(RoadState.ActiveOrPending, Tile.MotorwayInclusion.Ignore))
					{
						tile.SetNodeState(new RoadTileNode(activeNodeDirection, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
					}
				}
			}
			foreach (TileDirection invalidNodeDirection in Roundabout.GetInvalidNodeDirectionsForNeighbor(offset))
			{
				if ((tile.GetTwoLaneRoadStateInDirection(invalidNodeDirection) & RoadState.ActiveOrPending) != RoadState.None)
				{
					tile.SetNodeState(new RoadTileNode(invalidNodeDirection, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
				}
			}
			return true;
		}

		// Token: 0x0600187D RID: 6269 RVA: 0x0005767C File Offset: 0x0005587C
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			if (this._concreteToMothball > 0)
			{
				upgradeDatabase.MothballUpgrade(UpgradeType.Concrete, this._concreteToMothball);
			}
			if (this._concreteToRelease > 0)
			{
				upgradeDatabase.ReleaseMothballedUpgrade(UpgradeType.Concrete, this._concreteToRelease);
			}
			if (this._isReplacingMothballedRoundabout)
			{
				return upgradeDatabase.UnmothballUpgrade(UpgradeType.Roundabout, 1);
			}
			return upgradeDatabase.ConsumeUpgrade(UpgradeType.Roundabout, 1);
		}

		// Token: 0x0600187E RID: 6270 RVA: 0x000576D0 File Offset: 0x000558D0
		public override void ApplyToSimulation(ISimulation simulation)
		{
			RoundaboutModel existingRoundabout = null;
			foreach (RoundaboutModel roundabout in simulation.GetModels<RoundaboutModel>())
			{
				if (roundabout.OriginCoordinates == this._originCoordinates)
				{
					existingRoundabout = roundabout;
					break;
				}
			}
			if (!this._isReplacingMothballedRoundabout && Diagnostics.Verify(existingRoundabout == null))
			{
				RoundaboutModel newRoundabout = this._scope.Get<RoundaboutModel>();
				newRoundabout.Initialize(this._originCoordinates, this._replacedConnections);
				simulation.AddModel(newRoundabout);
			}
		}

		// Token: 0x0600187F RID: 6271 RVA: 0x00057754 File Offset: 0x00055954
		public override void Reset()
		{
			base.Reset();
			this._originCoordinates = default(Vector2Int);
			this._concreteToMothball = 0;
			this._concreteToRelease = 0;
			this._isReplacingMothballedRoundabout = false;
			this._replacedConnections = null;
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x00057784 File Offset: 0x00055984
		public static AddRoundaboutEdit Create(IScope scope, Vector2Int originCoordinates, ITilemap tilemap)
		{
			bool roadsBecomePermanentOverTime = scope.Get<City>().Rules.RoadsBecomePermanentOverTime;
			bool isReplacingMothballedRoundabout = false;
			Vector2Int originCoordinatesOffset = Roundabout.GetCoordinatesOffsets()[0];
			Tile originTile = tilemap.GetTile(originCoordinates + originCoordinatesOffset);
			if (originTile != null)
			{
				RoadTileConnection originRoundaboutConnection = Roundabout.GetConnectionForCoordinatesOffset(originCoordinatesOffset);
				if (originTile.HasRoundabout(RoadState.Mothballed) && originTile.GetRoundaboutConnection(RoadState.Mothballed).Equals(originRoundaboutConnection))
				{
					isReplacingMothballedRoundabout = true;
				}
			}
			HashSet<AdjacentTileConnection> mothballedConnections = new HashSet<AdjacentTileConnection>();
			HashSet<AdjacentTileConnection> permanentConnections = new HashSet<AdjacentTileConnection>();
			foreach (Vector2Int coordinatesOffset in Roundabout.GetCoordinatesOffsets())
			{
				RoadTileConnection roundaboutConnection = Roundabout.GetConnectionForCoordinatesOffset(coordinatesOffset);
				Tile includedRoundaboutTile = tilemap.GetTile(originCoordinates + coordinatesOffset);
				if (includedRoundaboutTile != null)
				{
					foreach (TileDirection paidRoadDirection in includedRoundaboutTile.GetTwoLaneRoads(RoadState.ActiveOrPending, Tile.MotorwayInclusion.Ignore))
					{
						if (!Roundabout.CanConnectionAddExitNode(roundaboutConnection, new RoadTileNode(paidRoadDirection, RoadType.TwoLane, -1)))
						{
							AdjacentTileConnection connection = new AdjacentTileConnection(includedRoundaboutTile.Coordinates, paidRoadDirection);
							mothballedConnections.Add(connection);
							if (includedRoundaboutTile.IsNodePermanent(paidRoadDirection) && roadsBecomePermanentOverTime)
							{
								permanentConnections.Add(connection);
							}
						}
					}
				}
			}
			int concreteToMothball = 0;
			int concreteToRelease = 0;
			if (mothballedConnections.Any<AdjacentTileConnection>())
			{
				GameBehaviourModel behaviour = scope.Get<GameBehaviourModel>();
				int permanentConcrete = 0;
				foreach (AdjacentTileConnection connection2 in mothballedConnections)
				{
					concreteToMothball += behaviour.GetConcreteCostForConnection(tilemap, connection2.OriginCoordinates, connection2.DestinationCoordinates);
				}
				if (isReplacingMothballedRoundabout)
				{
					foreach (AdjacentTileConnection connection3 in permanentConnections)
					{
						permanentConcrete += behaviour.GetConcreteCostForConnection(tilemap, connection3.OriginCoordinates, connection3.DestinationCoordinates);
					}
					concreteToRelease = concreteToMothball - permanentConcrete;
				}
			}
			Tile centerTile = tilemap.GetTile(originCoordinates);
			if (centerTile != null && roadsBecomePermanentOverTime)
			{
				foreach (TileDirection direction in centerTile.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Ignore))
				{
					if (centerTile.IsNodePermanent(direction))
					{
						permanentConnections.Add(new AdjacentTileConnection(centerTile.Coordinates, direction));
					}
				}
			}
			AddRoundaboutEdit addRoundaboutEdit = scope.Get<AddRoundaboutEdit>();
			addRoundaboutEdit._originCoordinates = originCoordinates;
			addRoundaboutEdit._concreteToMothball = concreteToMothball;
			addRoundaboutEdit._concreteToRelease = concreteToRelease;
			addRoundaboutEdit._isReplacingMothballedRoundabout = isReplacingMothballedRoundabout;
			addRoundaboutEdit._replacedConnections = new List<AdjacentTileConnection>(permanentConnections);
			return addRoundaboutEdit;
		}

		// Token: 0x040014E2 RID: 5346
		private Vector2Int _originCoordinates;

		// Token: 0x040014E3 RID: 5347
		private int _concreteToMothball;

		// Token: 0x040014E4 RID: 5348
		private int _concreteToRelease;

		// Token: 0x040014E5 RID: 5349
		private bool _isReplacingMothballedRoundabout;

		// Token: 0x040014E6 RID: 5350
		private List<AdjacentTileConnection> _replacedConnections;

		// Token: 0x040014E7 RID: 5351
		[Dependency]
		private IScope _scope;
	}
}
