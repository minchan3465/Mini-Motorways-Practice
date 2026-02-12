using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003FF RID: 1023
	public class RemoveMotorwaysEdit : TileEdit
	{
		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x060018ED RID: 6381 RVA: 0x00058AF3 File Offset: 0x00056CF3
		// (set) Token: 0x060018EE RID: 6382 RVA: 0x00058AFB File Offset: 0x00056CFB
		[Serialize(true, null)]
		public Vector2Int Coordinates { get; private set; }

		// Token: 0x060018EF RID: 6383 RVA: 0x00058B04 File Offset: 0x00056D04
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			Tile tile = tilemap.GetTile(this.Coordinates);
			if (tile != null)
			{
				TileDirectionBitfield motorwayRampDirections = tile.GetMotorwayRamps(RoadState.Planned | RoadState.Active);
				if (motorwayRampDirections.Count > 0)
				{
					List<Vector2Int> connectedTiles = new List<Vector2Int>();
					foreach (TileDirection rampDirection in motorwayRampDirections)
					{
						int motorwayId = tile.GetMotorwayInDirection(rampDirection, RoadState.Planned | RoadState.Active);
						TileEdit.Log.Info("Traversing motorway {0} in direction {1} from clearing tile.", new object[]
						{
							motorwayId,
							rampDirection
						});
						Motorway motorway = tilemap.GetMotorway(motorwayId);
						if (Diagnostics.Verify(motorway != null, "Unable to find expected motorway {0}.", motorwayId))
						{
							Vector2Int connectedTileCoordinates = (motorway.StartCoordinates == tile.Coordinates) ? motorway.EndCoordinates : motorway.StartCoordinates;
							if (!connectedTiles.Contains(connectedTileCoordinates))
							{
								TileEdit.Log.Info("Returning {0} as an affected tile from clearing tile {1}.", new object[]
								{
									connectedTileCoordinates,
									this.Coordinates
								});
								connectedTiles.Add(connectedTileCoordinates);
								yield return tilemap.GetTile(connectedTileCoordinates);
							}
						}
					}
					yield return tile;
					connectedTiles = null;
				}
			}
			yield break;
		}

		// Token: 0x060018F0 RID: 6384 RVA: 0x00058B1C File Offset: 0x00056D1C
		public override bool ApplyToAffectedTile(Tile tile)
		{
			Tile.Log.Info("Applying RemoveMotorwayEdit from tile {0}, to tile {1}.", new object[]
			{
				this.Coordinates,
				tile.Coordinates
			});
			if (tile.Coordinates.Equals(this.Coordinates))
			{
				foreach (TileDirection motorwayDirection in tile.GetMotorwayRamps(RoadState.Planned | RoadState.Active))
				{
					int motorwayId = tile.GetMotorwayInDirection(motorwayDirection, RoadState.Planned | RoadState.Active);
					tile.SetNodeState(new RoadTileNode(motorwayDirection, RoadType.Motorway, motorwayId), RoadState.Mothballed, Tile.TileChangePermissions.Full);
				}
				return true;
			}
			TileEdit.Log.Info("Mothballing motorway connections between {0} and non-adjacent tile {1}.", new object[]
			{
				this.Coordinates,
				tile.Coordinates
			});
			foreach (TileDirection motorwayDirection2 in tile.GetMotorwayRamps(RoadState.Planned | RoadState.Active))
			{
				int motorwayId2 = tile.GetMotorwayInDirection(motorwayDirection2, RoadState.Planned | RoadState.Active);
				Motorway motorway = tile.Tilemap.GetMotorway(motorwayId2);
				if (Diagnostics.Verify(motorway != null, "Unable to find motorway {0}.", motorwayId2) && (motorway.StartCoordinates == this.Coordinates || motorway.EndCoordinates == this.Coordinates))
				{
					TileEdit.Log.Info("Mothballing node for motorway {0}.", new object[]
					{
						motorwayId2
					});
					Diagnostics.Verify(tile.SetNodeState(new RoadTileNode(motorwayDirection2, RoadType.Motorway, motorwayId2), RoadState.Mothballed, Tile.TileChangePermissions.Full), "Failed to mothball node on motorway {0}, connected to cleared tile {1}.", motorwayId2, this.Coordinates);
				}
			}
			return true;
		}

		// Token: 0x060018F1 RID: 6385 RVA: 0x00058CBB File Offset: 0x00056EBB
		public override IEnumerable<Motorway> GetAffectedMotorways(ITilemap tilemap)
		{
			Tile tile = tilemap.GetTile(this.Coordinates);
			if (tile != null)
			{
				foreach (TileDirection motorwayDirection in tile.GetMotorwayRamps(RoadState.Planned | RoadState.Active))
				{
					int motorwayId = tile.GetMotorwayInDirection(motorwayDirection, RoadState.Planned | RoadState.Active);
					Motorway motorway = tilemap.GetMotorway(motorwayId);
					if (Diagnostics.Verify(motorway != null, "Unable to find motorway from ID {0}", motorwayId))
					{
						yield return motorway;
					}
				}
			}
			yield break;
		}

		// Token: 0x060018F2 RID: 6386 RVA: 0x00058CD2 File Offset: 0x00056ED2
		public override bool ApplyToAffectedMotorway(Motorway motorway)
		{
			motorway.SetState(RoadState.Mothballed);
			return true;
		}

		// Token: 0x060018F3 RID: 6387 RVA: 0x00058CE0 File Offset: 0x00056EE0
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			int concreteToMothball = 0;
			foreach (Motorway mothballedMotorway in this.GetAffectedMotorways(tilemap))
			{
				concreteToMothball += mothballedMotorway.ConcreteCost;
			}
			upgradeDatabase.MothballUpgrade(UpgradeType.Concrete, concreteToMothball);
			return true;
		}

		// Token: 0x060018F4 RID: 6388 RVA: 0x00058D3C File Offset: 0x00056F3C
		public override void Reset()
		{
			base.Reset();
			this.Coordinates = default(Vector2Int);
		}

		// Token: 0x060018F5 RID: 6389 RVA: 0x00058D5E File Offset: 0x00056F5E
		public static RemoveMotorwaysEdit Create(IScope scope, Vector2Int coordinates)
		{
			RemoveMotorwaysEdit removeMotorwaysEdit = scope.Get<RemoveMotorwaysEdit>();
			removeMotorwaysEdit.Coordinates = coordinates;
			return removeMotorwaysEdit;
		}
	}
}
