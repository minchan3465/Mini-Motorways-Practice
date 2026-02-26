using System;
using Factory;
using Factory.Pools;
using FixMath;
using Server;

namespace Motorways.Models
{
	// Token: 0x020004F1 RID: 1265
	public class MotorwayModel : Motorway, IModel, IReusable
	{
		// Token: 0x06002155 RID: 8533 RVA: 0x000850F2 File Offset: 0x000832F2
		public override bool Initialize(ITilemap tilemap, int id, int number, RoadState roadState = RoadState.None)
		{
			this.roadChunk = this._scope.Get<RoadChunkModel>();
			this._simulation.AddModel(this.roadChunk);
			this.hasConsumedUpgrade = false;
			return base.Initialize(tilemap, id, number, roadState);
		}

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x06002156 RID: 8534 RVA: 0x00085129 File Offset: 0x00083329
		public TileModel StartTile
		{
			get
			{
				return this._tilemapModel.GetTileModel(base.StartCoordinates);
			}
		}

		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x06002157 RID: 8535 RVA: 0x0008513C File Offset: 0x0008333C
		public TileModel EndTile
		{
			get
			{
				return this._tilemapModel.GetTileModel(base.EndCoordinates);
			}
		}

		// Token: 0x06002158 RID: 8536 RVA: 0x00085150 File Offset: 0x00083350
		public bool CanSetMotorwayAndNodeState(RoadState newState)
		{
			return base.CanSetState(newState) && Diagnostics.Verify(this.StartTile != null && this.EndTile != null, "Motorway is missing a tile.") && this.StartTile.Tile.CanSetNodeState(new RoadTileNode(base.StartDirection, RoadType.Motorway, base.Id), newState, Tile.TileChangePermissions.Full) && this.EndTile.Tile.CanSetNodeState(new RoadTileNode(base.EndDirection, RoadType.Motorway, base.Id), newState, Tile.TileChangePermissions.Full);
		}

		// Token: 0x06002159 RID: 8537 RVA: 0x000851D8 File Offset: 0x000833D8
		public bool CanBeReplacedByActivatingMothballedMotorway(out MotorwayModel mothballedReplacement)
		{
			if (base.State != RoadState.Planned)
			{
				mothballedReplacement = null;
				return false;
			}
			foreach (MotorwayModel motorway in this._simulation.GetModels<MotorwayModel>())
			{
				if (motorway.Number == base.Number && motorway.State == RoadState.Mothballed && ((motorway.StartTile == this.StartTile && motorway.EndTile == this.EndTile) || (motorway.StartTile == this.EndTile && motorway.EndTile == this.StartTile)))
				{
					mothballedReplacement = motorway;
					return true;
				}
			}
			mothballedReplacement = null;
			return false;
		}

		// Token: 0x0600215A RID: 8538 RVA: 0x00085274 File Offset: 0x00083474
		public void SetMotorwayAndNodeState(RoadState newState)
		{
			if (Diagnostics.Verify(this.StartTile != null && this.EndTile != null, "Motorway is missing a tile."))
			{
				Diagnostics.Verify(this.StartTile.Tile.SetNodeState(new RoadTileNode(base.StartDirection, RoadType.Motorway, base.Id), newState, Tile.TileChangePermissions.Full), "Failed to set node state {0} on motorway {1}'s start tile.", newState, base.Id);
				Diagnostics.Verify(this.EndTile.Tile.SetNodeState(new RoadTileNode(base.EndDirection, RoadType.Motorway, base.Id), newState, Tile.TileChangePermissions.Full), "Failed to set node state {0} on motorway {1}'s end tile.", newState, base.Id);
			}
			Diagnostics.Verify(base.SetState(newState), "Failed to set state {0} on motorway {1}.", newState, base.Id);
		}

		// Token: 0x0600215B RID: 8539 RVA: 0x00085347 File Offset: 0x00083547
		public void IncrementPermanence(Fix64 permanenceProgress, RoadState states = RoadState.Active)
		{
			if (base.State.HasFlag(states) && !base.IsPermanent)
			{
				base.PermanenceProgress += permanenceProgress;
			}
		}

		// Token: 0x0600215C RID: 8540 RVA: 0x0008537B File Offset: 0x0008357B
		public override void Reset()
		{
			base.Reset();
			this.roadChunk = null;
			this.startToEndLane = null;
			this.endToStartLane = null;
			this.hasConsumedUpgrade = false;
			this.isHighBuildPriority = false;
		}

		// Token: 0x0600215D RID: 8541 RVA: 0x000853A6 File Offset: 0x000835A6
		public override void OnReleasedFromScope(IScope scope)
		{
			base.OnReleasedFromScope(scope);
			if (this.roadChunk != null)
			{
				this._simulation.RemoveModel(this.roadChunk);
				this.roadChunk = null;
			}
		}

		// Token: 0x0600215E RID: 8542 RVA: 0x000853D0 File Offset: 0x000835D0
		public override string ToString()
		{
			return string.Format("[MotorwayModel Id={0}]", base.Id);
		}

		// Token: 0x04001B82 RID: 7042
		public RoadChunkModel roadChunk;

		// Token: 0x04001B83 RID: 7043
		public LaneModel startToEndLane;

		// Token: 0x04001B84 RID: 7044
		public LaneModel endToStartLane;

		// Token: 0x04001B85 RID: 7045
		public bool hasConsumedUpgrade;

		// Token: 0x04001B86 RID: 7046
		public bool isHighBuildPriority;

		// Token: 0x04001B87 RID: 7047
		[Dependency]
		private Scope _scope;

		// Token: 0x04001B88 RID: 7048
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x04001B89 RID: 7049
		[Dependency]
		private TilemapModel _tilemapModel;
	}
}
