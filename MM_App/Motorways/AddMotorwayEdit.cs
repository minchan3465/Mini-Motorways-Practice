using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003EA RID: 1002
	public class AddMotorwayEdit : TileEdit
	{
		// Token: 0x0600183F RID: 6207 RVA: 0x0005691C File Offset: 0x00054B1C
		public override bool ApplyToAffectedTile(Tile tile)
		{
			bool foundTile = false;
			bool success = true;
			Fix64 motorwayPermanence = Fix64.Zero;
			if (this.replacedMotorwayId != -1)
			{
				Motorway replacedMotorway = tile.Tilemap.GetMotorway(this.replacedMotorwayId);
				if (Diagnostics.Verify(replacedMotorway != null, "Unable to find replaced motorway id {0}", replacedMotorway))
				{
					if (tile.Coordinates == replacedMotorway.StartCoordinates)
					{
						foundTile = true;
						success = (success && Diagnostics.Verify(tile.SetNodeState(new RoadTileNode(replacedMotorway.StartDirection, RoadType.Motorway, this.replacedMotorwayId), RoadState.Mothballed, Tile.TileChangePermissions.Full), "Failed to mothball replaced motorway's ({0}) start node.", this.replacedMotorwayId));
					}
					if (tile.Coordinates == replacedMotorway.EndCoordinates)
					{
						foundTile = true;
						success = (success && Diagnostics.Verify(tile.SetNodeState(new RoadTileNode(replacedMotorway.EndDirection, RoadType.Motorway, this.replacedMotorwayId), RoadState.Mothballed, Tile.TileChangePermissions.Full), "Failed to mothball replaced motorway's ({0}) end node.", this.replacedMotorwayId));
					}
					motorwayPermanence = replacedMotorway.PermanenceProgress;
				}
			}
			if (tile.Coordinates == this.startCoordinates)
			{
				foundTile = true;
				success = (success && Diagnostics.Verify(tile.SetNodeState(new RoadTileNode(this.startDirection, RoadType.Motorway, this.newMotorwayId), RoadState.Planned, Tile.TileChangePermissions.Full), "Failed to plan a new motorway's start node."));
				tile.SetNodePermanence(this.startDirection, motorwayPermanence);
			}
			if (tile.Coordinates == this.endCoordinates)
			{
				foundTile = true;
				success = (success && Diagnostics.Verify(tile.SetNodeState(new RoadTileNode(this.endDirection, RoadType.Motorway, this.newMotorwayId), RoadState.Planned, Tile.TileChangePermissions.Full), "Failed to plan a new motorway's end node."));
				tile.SetNodePermanence(this.endDirection, motorwayPermanence);
			}
			if (foundTile && success && tile.UnbuiltMotorwayId == this.newMotorwayId)
			{
				tile.UnbuiltMotorwayId = -1;
				tile.UnbuiltMotorwayNumber = 0;
			}
			return foundTile && success;
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x00056AC2 File Offset: 0x00054CC2
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			yield return tilemap.GetOrCreateTile(this.startCoordinates);
			yield return tilemap.GetOrCreateTile(this.endCoordinates);
			if (this.replacedMotorwayId != -1)
			{
				Motorway replacedMotorway = tilemap.GetMotorway(this.replacedMotorwayId);
				if (Diagnostics.Verify(replacedMotorway != null, "Unable to find replaced motorway id {0}", replacedMotorway))
				{
					if (replacedMotorway.StartCoordinates != this.startCoordinates && replacedMotorway.StartCoordinates != this.endCoordinates)
					{
						yield return tilemap.GetTile(replacedMotorway.StartCoordinates);
					}
					if (replacedMotorway.EndCoordinates != this.startCoordinates && replacedMotorway.EndCoordinates != this.endCoordinates)
					{
						yield return tilemap.GetTile(replacedMotorway.EndCoordinates);
					}
				}
				replacedMotorway = null;
			}
			yield break;
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x00056ADC File Offset: 0x00054CDC
		public override bool ApplyToAffectedMotorway(Motorway motorway)
		{
			if (motorway.Id == this.newMotorwayId)
			{
				motorway.SetState(RoadState.Planned);
				motorway.StartCoordinates = this.startCoordinates;
				motorway.StartDirection = this.startDirection;
				motorway.EndCoordinates = this.endCoordinates;
				motorway.EndDirection = this.endDirection;
				motorway.ConcreteCost = this.ConcreteCostForNewMotorway;
				if (this.replacedMotorwayId != -1)
				{
					Motorway replacedMotorway = motorway.Tilemap.GetMotorway(this.replacedMotorwayId);
					if (Diagnostics.Verify(replacedMotorway != null))
					{
						motorway.SetPermanence(replacedMotorway.PermanenceProgress);
					}
				}
				return true;
			}
			if (motorway.Id == this.replacedMotorwayId)
			{
				motorway.SetState(RoadState.Mothballed);
				int concreteGivenToNewMotorway = Mathf.Min(this.ConcreteCostForNewMotorway, motorway.ConcreteCost);
				motorway.ConcreteGivenToReplacement = concreteGivenToNewMotorway;
				return true;
			}
			return false;
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x00056BA2 File Offset: 0x00054DA2
		public override IEnumerable<Motorway> GetAffectedMotorways(ITilemap tilemap)
		{
			Motorway motorway = tilemap.GetMotorway(this.newMotorwayId);
			if (motorway == null)
			{
				motorway = tilemap.CreateMotorway(this.newMotorwayId, this.motorwayNumber, this.replacedMotorwayId);
			}
			if (Diagnostics.Verify(motorway != null, "Unable to find motorway from new motorway ID {0}", this.newMotorwayId))
			{
				yield return motorway;
			}
			if (this.replacedMotorwayId != -1)
			{
				Motorway replacedMotorway = tilemap.GetMotorway(this.replacedMotorwayId);
				if (Diagnostics.Verify(replacedMotorway != null, "Unable to find motorway from replaced motorway ID {0}", this.replacedMotorwayId))
				{
					yield return replacedMotorway;
				}
			}
			yield break;
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x00056BBC File Offset: 0x00054DBC
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			bool success = true;
			Motorway replacedMotorway = null;
			if (this.replacedMotorwayId != -1)
			{
				replacedMotorway = tilemap.GetMotorway(this.replacedMotorwayId);
			}
			int concreteCostForNewMotorway = this.ConcreteCostForNewMotorway;
			int concreteUsedByReplacedMotorway = 0;
			if (replacedMotorway != null)
			{
				concreteUsedByReplacedMotorway = replacedMotorway.ConcreteCost;
			}
			if (concreteCostForNewMotorway > concreteUsedByReplacedMotorway)
			{
				success = (success && upgradeDatabase.ConsumeUpgrade(UpgradeType.Concrete, concreteCostForNewMotorway - concreteUsedByReplacedMotorway));
			}
			else if (concreteCostForNewMotorway < concreteUsedByReplacedMotorway)
			{
				success = (success && upgradeDatabase.MothballUpgrade(UpgradeType.Concrete, concreteUsedByReplacedMotorway - concreteCostForNewMotorway));
			}
			return success;
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x00056C23 File Offset: 0x00054E23
		public override void ApplyToSimulation(ISimulation simulation)
		{
			if (this.replacedMotorwayId == -1)
			{
				simulation.GetModel<TilemapModel>().GetMotorwayModel(this.newMotorwayId).hasConsumedUpgrade = true;
			}
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06001845 RID: 6213 RVA: 0x00056C45 File Offset: 0x00054E45
		public int ConcreteCostForNewMotorway
		{
			get
			{
				return this._behaviour.GetConcreteCostForMotorway(this.startCoordinates, this.endCoordinates);
			}
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x00056C60 File Offset: 0x00054E60
		public override void Reset()
		{
			base.Reset();
			this.newMotorwayId = 0;
			this.replacedMotorwayId = 0;
			this.motorwayNumber = 0;
			this.startCoordinates = default(Vector2Int);
			this.startDirection = TileDirection.None;
			this.endCoordinates = default(Vector2Int);
			this.endDirection = TileDirection.None;
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x00056CAE File Offset: 0x00054EAE
		public static AddMotorwayEdit Create(IScope scope, int newMotorwayId, int motorwayNumber, Vector2Int startCoordinates, TileDirection startDirection, Vector2Int endCoordinates, TileDirection endDirection, int replacedMotorwayId)
		{
			AddMotorwayEdit addMotorwayEdit = scope.Get<AddMotorwayEdit>();
			addMotorwayEdit.newMotorwayId = newMotorwayId;
			addMotorwayEdit.startCoordinates = startCoordinates;
			addMotorwayEdit.startDirection = startDirection;
			addMotorwayEdit.endCoordinates = endCoordinates;
			addMotorwayEdit.endDirection = endDirection;
			addMotorwayEdit.replacedMotorwayId = replacedMotorwayId;
			addMotorwayEdit.motorwayNumber = motorwayNumber;
			return addMotorwayEdit;
		}

		// Token: 0x040014B7 RID: 5303
		private int newMotorwayId;

		// Token: 0x040014B8 RID: 5304
		private int replacedMotorwayId;

		// Token: 0x040014B9 RID: 5305
		private int motorwayNumber;

		// Token: 0x040014BA RID: 5306
		private Vector2Int startCoordinates;

		// Token: 0x040014BB RID: 5307
		private TileDirection startDirection;

		// Token: 0x040014BC RID: 5308
		private Vector2Int endCoordinates;

		// Token: 0x040014BD RID: 5309
		private TileDirection endDirection;
	}
}
