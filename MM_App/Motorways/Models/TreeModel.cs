using System;
using Factory;
using Server;

namespace Motorways.Models
{
	// Token: 0x02000510 RID: 1296
	public class TreeModel : Model<EmptyModelFrame, TreeModel.IObserver>
	{
		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x06002256 RID: 8790 RVA: 0x0008A9EB File Offset: 0x00088BEB
		// (set) Token: 0x06002257 RID: 8791 RVA: 0x0008A9F3 File Offset: 0x00088BF3
		[Serialize(true, null)]
		public int PrefabIndex { get; private set; }

		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x06002258 RID: 8792 RVA: 0x0008A9FC File Offset: 0x00088BFC
		// (set) Token: 0x06002259 RID: 8793 RVA: 0x0008AA04 File Offset: 0x00088C04
		[Serialize(true, null)]
		public TileModel TileModel { get; private set; }

		// Token: 0x0600225A RID: 8794 RVA: 0x0008AA10 File Offset: 0x00088C10
		public void Bulldoze()
		{
			foreach (TreeModel.IObserver observer in base.Observers)
			{
				observer.OnBulldozed();
			}
			this.TileModel.Tile.SetContentType(TileContentType.None, null);
			this.simulation.RemoveModel(this);
			if (this._city.Rules.RecordsGameStatistics())
			{
				this._player.AchievementStatistics.OnTreeBulldozed(this._player.Scope.Get<IAchievementHandler>());
			}
		}

		// Token: 0x0600225B RID: 8795 RVA: 0x0008AA91 File Offset: 0x00088C91
		public virtual void Initialize(int prefabIndex, TileModel tileModel)
		{
			this.PrefabIndex = prefabIndex;
			this.TileModel = tileModel;
			this.TileModel.Tile.SetContentType(TileContentType.Tree, this);
		}

		// Token: 0x0600225C RID: 8796 RVA: 0x0008AAB3 File Offset: 0x00088CB3
		public override void Reset()
		{
			base.Reset();
			this.TileModel = null;
			this.PrefabIndex = 0;
		}

		// Token: 0x0600225D RID: 8797 RVA: 0x0008AAC9 File Offset: 0x00088CC9
		public TreeModel() : base(1)
		{
		}

		// Token: 0x04001C28 RID: 7208
		[Dependency]
		private ISimulation simulation;

		// Token: 0x04001C29 RID: 7209
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04001C2A RID: 7210
		[Dependency]
		private City _city;

		// Token: 0x02000511 RID: 1297
		public interface IObserver
		{
			// Token: 0x0600225E RID: 8798
			void OnBulldozed();
		}
	}
}
