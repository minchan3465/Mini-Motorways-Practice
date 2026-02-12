using System;
using System.Collections.Generic;
using Server;

namespace Motorways.Models
{
	// Token: 0x02000509 RID: 1289
	public class TrainLineModel : Model<EmptyModelFrame, TrainLineModel.IObserver>
	{
		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x06002243 RID: 8771 RVA: 0x0008A722 File Offset: 0x00088922
		public int TrainCount
		{
			get
			{
				return this._trains.Count;
			}
		}

		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x06002244 RID: 8772 RVA: 0x0008A72F File Offset: 0x0008892F
		public bool IsLoop
		{
			get
			{
				return this._isLoop;
			}
		}

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x06002245 RID: 8773 RVA: 0x0008A737 File Offset: 0x00088937
		public RailTileModel StartTile
		{
			get
			{
				return this._startTile;
			}
		}

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x06002246 RID: 8774 RVA: 0x0008A73F File Offset: 0x0008893F
		public RailTileModel EndTile
		{
			get
			{
				return this._endTile;
			}
		}

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x06002247 RID: 8775 RVA: 0x0008A747 File Offset: 0x00088947
		public List<RailTileModel> TrainSpawnTiles
		{
			get
			{
				return this._trainSpawnTiles;
			}
		}

		// Token: 0x06002248 RID: 8776 RVA: 0x0008A74F File Offset: 0x0008894F
		public RailTileModel GetTrackAtIndex(int index)
		{
			return this._tiles[index];
		}

		// Token: 0x06002249 RID: 8777 RVA: 0x0008A75D File Offset: 0x0008895D
		public void Initialize(bool isLoop)
		{
			this._isLoop = isLoop;
		}

		// Token: 0x0600224A RID: 8778 RVA: 0x0008A768 File Offset: 0x00088968
		public void AddTile(RailTileModel railTileModel, RailType type)
		{
			railTileModel.Line = this;
			this._tiles.Add(railTileModel);
			RailTileConnection railConnection = railTileModel.TileModel.Tile.RailConnection;
			if (railConnection.input == TileDirection.None)
			{
				this._startTile = railTileModel;
			}
			if (railConnection.output == TileDirection.None)
			{
				this._endTile = railTileModel;
			}
			if (type == RailType.TrainOrigin)
			{
				this._trainSpawnTiles.Add(railTileModel);
			}
		}

		// Token: 0x0600224B RID: 8779 RVA: 0x0008A7C7 File Offset: 0x000889C7
		public void AddTrain(TrainModel trainModel)
		{
			this._trains.Add(trainModel);
		}

		// Token: 0x0600224C RID: 8780 RVA: 0x0008A7D5 File Offset: 0x000889D5
		public override void Reset()
		{
			base.Reset();
			this._trains.Clear();
			this._tiles.Clear();
			this._trainSpawnTiles.Clear();
			this._isLoop = false;
			this._startTile = null;
			this._endTile = null;
		}

		// Token: 0x0600224D RID: 8781 RVA: 0x0008A813 File Offset: 0x00088A13
		public TrainLineModel() : base(1)
		{
		}

		// Token: 0x04001C0E RID: 7182
		private readonly List<TrainModel> _trains = new List<TrainModel>();

		// Token: 0x04001C0F RID: 7183
		private readonly List<RailTileModel> _tiles = new List<RailTileModel>();

		// Token: 0x04001C10 RID: 7184
		private readonly List<RailTileModel> _trainSpawnTiles = new List<RailTileModel>();

		// Token: 0x04001C11 RID: 7185
		private bool _isLoop;

		// Token: 0x04001C12 RID: 7186
		private RailTileModel _startTile;

		// Token: 0x04001C13 RID: 7187
		private RailTileModel _endTile;

		// Token: 0x0200050A RID: 1290
		public interface IObserver
		{
		}
	}
}
