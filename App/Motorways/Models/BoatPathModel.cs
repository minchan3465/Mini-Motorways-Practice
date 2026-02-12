using System;
using System.Collections.Generic;
using Server;

namespace Motorways.Models
{
	// Token: 0x020004D1 RID: 1233
	public class BoatPathModel : Model<EmptyModelFrame, BoatPathModel.IObserver>
	{
		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x06002022 RID: 8226 RVA: 0x0007E5EF File Offset: 0x0007C7EF
		public int BoatCount
		{
			get
			{
				return this._boats.Count;
			}
		}

		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x06002023 RID: 8227 RVA: 0x0007E5FC File Offset: 0x0007C7FC
		public bool IsLoop
		{
			get
			{
				return this._isLoop;
			}
		}

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x06002024 RID: 8228 RVA: 0x0007E604 File Offset: 0x0007C804
		public BoatPathTileModel StartTile
		{
			get
			{
				return this._startTile;
			}
		}

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x06002025 RID: 8229 RVA: 0x0007E60C File Offset: 0x0007C80C
		public BoatPathTileModel EndTile
		{
			get
			{
				return this._endTile;
			}
		}

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x06002026 RID: 8230 RVA: 0x0007E614 File Offset: 0x0007C814
		public List<BoatPathTileModel> BoatSpawnTiles
		{
			get
			{
				return this._boatSpawnTiles;
			}
		}

		// Token: 0x06002027 RID: 8231 RVA: 0x0007E61C File Offset: 0x0007C81C
		public BoatPathTileModel GetTrackAtIndex(int index)
		{
			return this._tiles[index];
		}

		// Token: 0x06002028 RID: 8232 RVA: 0x0007E62A File Offset: 0x0007C82A
		public void Initialize(bool isLoop)
		{
			this._isLoop = isLoop;
		}

		// Token: 0x06002029 RID: 8233 RVA: 0x0007E634 File Offset: 0x0007C834
		public void AddTile(BoatPathTileModel boatPathTileModel, BoatPathType boatPathType)
		{
			boatPathTileModel.BoatPath = this;
			this._tiles.Add(boatPathTileModel);
			BoatPathTileConnection boatPathConnection = boatPathTileModel.TileModel.Tile.BoatPathConnection;
			if (boatPathConnection.input == TileDirection.None)
			{
				this._startTile = boatPathTileModel;
			}
			if (boatPathConnection.output == TileDirection.None)
			{
				this._endTile = boatPathTileModel;
			}
			if (boatPathType == BoatPathType.BoatOrigin)
			{
				this._boatSpawnTiles.Add(boatPathTileModel);
			}
		}

		// Token: 0x0600202A RID: 8234 RVA: 0x0007E693 File Offset: 0x0007C893
		public void AddBoat(BoatModel boatModel)
		{
			this._boats.Add(boatModel);
		}

		// Token: 0x0600202B RID: 8235 RVA: 0x0007E6A1 File Offset: 0x0007C8A1
		public override void Reset()
		{
			base.Reset();
			this._boats.Clear();
			this._tiles.Clear();
			this._boatSpawnTiles.Clear();
			this._isLoop = false;
			this._startTile = null;
			this._endTile = null;
		}

		// Token: 0x0600202C RID: 8236 RVA: 0x0007E6DF File Offset: 0x0007C8DF
		public BoatPathModel() : base(1)
		{
		}

		// Token: 0x04001AB3 RID: 6835
		private readonly List<BoatModel> _boats = new List<BoatModel>();

		// Token: 0x04001AB4 RID: 6836
		private readonly List<BoatPathTileModel> _tiles = new List<BoatPathTileModel>();

		// Token: 0x04001AB5 RID: 6837
		private readonly List<BoatPathTileModel> _boatSpawnTiles = new List<BoatPathTileModel>();

		// Token: 0x04001AB6 RID: 6838
		private bool _isLoop;

		// Token: 0x04001AB7 RID: 6839
		private BoatPathTileModel _startTile;

		// Token: 0x04001AB8 RID: 6840
		private BoatPathTileModel _endTile;

		// Token: 0x020004D2 RID: 1234
		public interface IObserver
		{
		}
	}
}
