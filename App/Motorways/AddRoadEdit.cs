using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003ED RID: 1005
	public class AddRoadEdit : TileEdit
	{
		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06001859 RID: 6233 RVA: 0x00057067 File Offset: 0x00055267
		// (set) Token: 0x0600185A RID: 6234 RVA: 0x0005706F File Offset: 0x0005526F
		[Serialize(true, null)]
		public Vector2Int OriginCoordinates { get; private set; }

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x0600185B RID: 6235 RVA: 0x00057078 File Offset: 0x00055278
		// (set) Token: 0x0600185C RID: 6236 RVA: 0x00057080 File Offset: 0x00055280
		[Serialize(true, null)]
		public TileDirection OriginDirection { get; private set; }

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x0600185D RID: 6237 RVA: 0x00057089 File Offset: 0x00055289
		public Vector2Int DestinationCoordinates
		{
			get
			{
				return TileUtilities.GetAdjacentCoordinates(this.OriginCoordinates, this.OriginDirection);
			}
		}

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x0600185E RID: 6238 RVA: 0x0005709C File Offset: 0x0005529C
		public TileDirection DestinationDirection
		{
			get
			{
				return TileUtilities.GetOppositeDirection(this.OriginDirection);
			}
		}

		// Token: 0x0600185F RID: 6239 RVA: 0x000570A9 File Offset: 0x000552A9
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			yield return tilemap.GetOrCreateTile(this.OriginCoordinates);
			yield return tilemap.GetOrCreateTile(this.DestinationCoordinates);
			yield break;
		}

		// Token: 0x06001860 RID: 6240 RVA: 0x000570C0 File Offset: 0x000552C0
		public override bool ApplyToAffectedTile(Tile tile)
		{
			TileDirection direction = TileDirection.None;
			if (tile.Coordinates == this.OriginCoordinates)
			{
				direction = this.OriginDirection;
			}
			else if (tile.Coordinates == this.DestinationCoordinates)
			{
				direction = this.DestinationDirection;
			}
			if (direction == TileDirection.None)
			{
				return false;
			}
			if (Roundabout.IsTileCenterOfRoundabout(tile, RoadState.VisiblyActive))
			{
				return true;
			}
			tile.SetNodeState(new RoadTileNode(direction, RoadType.TwoLane, -1), RoadState.Pending, Tile.TileChangePermissions.Full);
			return true;
		}

		// Token: 0x06001861 RID: 6241 RVA: 0x0005712C File Offset: 0x0005532C
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			Tile originTile = tilemap.GetOrCreateTile(this.OriginCoordinates);
			Tile destinationTile = tilemap.GetOrCreateTile(this.DestinationCoordinates);
			if (this._isJoiningPassages)
			{
				upgradeDatabase.MothballUpgrade(this._passageType, 1);
				upgradeDatabase.ReleaseMothballedUpgrade(this._passageType, 1);
			}
			else if (this._isStartingPassage)
			{
				upgradeDatabase.ConsumeUpgrade(this._passageType, 1);
			}
			int concreteCost = this._behaviour.GetConcreteCostForConnection(originTile, destinationTile);
			if (concreteCost <= 0)
			{
				return true;
			}
			if (originTile.GetTwoLaneRoadStateInDirection(this.OriginDirection) == RoadState.Mothballed || destinationTile.GetTwoLaneRoadStateInDirection(TileUtilities.GetOppositeDirection(this.OriginDirection)) == RoadState.Mothballed)
			{
				return upgradeDatabase.UnmothballUpgrade(UpgradeType.Concrete, concreteCost);
			}
			return upgradeDatabase.ConsumeUpgrade(UpgradeType.Concrete, concreteCost);
		}

		// Token: 0x06001862 RID: 6242 RVA: 0x000571DC File Offset: 0x000553DC
		public override void Reset()
		{
			base.Reset();
			this.OriginCoordinates = default(Vector2Int);
			this.OriginDirection = TileDirection.North;
			this._isStartingPassage = false;
			this._isJoiningPassages = false;
			this._passageType = UpgradeType.Concrete;
		}

		// Token: 0x06001863 RID: 6243 RVA: 0x0005721C File Offset: 0x0005541C
		public static AddRoadEdit Create(IScope scope, ITilemap tilemap, Vector2Int originCoordinates, TileDirection direction, CityDefinition cityDefinition)
		{
			AddRoadEdit newEdit = scope.Get<AddRoadEdit>();
			newEdit.OriginCoordinates = originCoordinates;
			newEdit.OriginDirection = direction;
			Vector2Int destinationCoordinates = newEdit.DestinationCoordinates;
			UpgradeType startedPassageType;
			UpgradeType joinedPassageType;
			if (Passage.WillConnectionStartPassage(cityDefinition, tilemap, originCoordinates, destinationCoordinates, out startedPassageType))
			{
				newEdit._isStartingPassage = true;
				newEdit._passageType = startedPassageType;
			}
			else if (Passage.WillConnectionJoinPassages(cityDefinition, tilemap, originCoordinates, destinationCoordinates, out joinedPassageType))
			{
				newEdit._isJoiningPassages = true;
				newEdit._passageType = joinedPassageType;
			}
			return newEdit;
		}

		// Token: 0x040014CD RID: 5325
		private bool _isStartingPassage;

		// Token: 0x040014CE RID: 5326
		private bool _isJoiningPassages;

		// Token: 0x040014CF RID: 5327
		private UpgradeType _passageType;
	}
}
