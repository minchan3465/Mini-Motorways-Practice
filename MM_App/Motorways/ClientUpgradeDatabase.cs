using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using Motorways.Models;
using Motorways.Views;

namespace Motorways
{
	// Token: 0x020003A6 RID: 934
	public class ClientUpgradeDatabase : UpgradeDatabase, UpgradeDatabase.IObserver, IReleasedFromScopeHandler, IReusable
	{
		// Token: 0x06001626 RID: 5670 RVA: 0x0004C54B File Offset: 0x0004A74B
		public void Initialize(UpgradeDatabaseModel model)
		{
			this._model = model;
			this._model.Subscribe(this);
			this._dirty = true;
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x0004C568 File Offset: 0x0004A768
		public override void Reset()
		{
			base.Reset();
			this._clientTileEdits.Clear();
			this._model = null;
			this._dirty = false;
			for (int upgradeIndex = 0; upgradeIndex < 9; upgradeIndex++)
			{
				this._availableOrDraftUpgrades[upgradeIndex] = 0;
			}
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x0004C5AA File Offset: 0x0004A7AA
		public int GetAvailableOrDraftUpgradeCount(UpgradeType upgradeType)
		{
			this.UpdateDatabase();
			return this._availableOrDraftUpgrades[(int)upgradeType];
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x0004C5BA File Offset: 0x0004A7BA
		public override int GetAvailableUpgradeCount(UpgradeType upgradeType)
		{
			this.UpdateDatabase();
			return base.GetAvailableUpgradeCount(upgradeType);
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x0004C5C9 File Offset: 0x0004A7C9
		public override bool HasUpgradeAvailable(UpgradeType upgradeType, int quantityRequired = 1)
		{
			this.UpdateDatabase();
			return base.HasUpgradeAvailable(upgradeType, quantityRequired);
		}

		// Token: 0x0600162B RID: 5675 RVA: 0x0004C5D9 File Offset: 0x0004A7D9
		public override bool ConsumeUpgrade(UpgradeType upgradeType, int quantityToConsume = 1)
		{
			this.UpdateDatabase();
			return base.ConsumeUpgrade(upgradeType, quantityToConsume);
		}

		// Token: 0x0600162C RID: 5676 RVA: 0x0004C5E9 File Offset: 0x0004A7E9
		public override bool MothballUpgrade(UpgradeType upgradeType, int quantityToMothball = 1)
		{
			this.UpdateDatabase();
			return base.MothballUpgrade(upgradeType, quantityToMothball);
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x0004C5F9 File Offset: 0x0004A7F9
		public override bool UnmothballUpgrade(UpgradeType upgradeType, int quantityToUnmothball = 1)
		{
			this.UpdateDatabase();
			return base.UnmothballUpgrade(upgradeType, quantityToUnmothball);
		}

		// Token: 0x0600162E RID: 5678 RVA: 0x0004C609 File Offset: 0x0004A809
		public override bool ReleaseMothballedUpgrade(UpgradeType upgradeType, int quantityToRelease = 1)
		{
			this.UpdateDatabase();
			return base.ReleaseMothballedUpgrade(upgradeType, quantityToRelease);
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x0004C619 File Offset: 0x0004A819
		public override bool ApplyEdit(TileEdit edit, ITilemap tilemap)
		{
			this.UpdateDatabase();
			return base.ApplyEdit(edit, tilemap);
		}

		// Token: 0x06001630 RID: 5680 RVA: 0x0004C629 File Offset: 0x0004A829
		public override void CloneInto(UpgradeDatabase cloneDatabase)
		{
			this.UpdateDatabase();
			base.CloneInto(cloneDatabase);
		}

		// Token: 0x06001631 RID: 5681 RVA: 0x0004C638 File Offset: 0x0004A838
		public void AddTileEdit(ClientTileEdit tileEdit)
		{
			this._clientTileEdits.Add(tileEdit);
			this._dirty = true;
		}

		// Token: 0x06001632 RID: 5682 RVA: 0x0004C64E File Offset: 0x0004A84E
		public void RemoveTileEdit(ClientTileEdit tileEdit)
		{
			this._clientTileEdits.Remove(tileEdit);
			this._dirty = true;
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x0004C664 File Offset: 0x0004A864
		public void OnDraftEditsScheduled()
		{
			this._dirty = true;
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x0004C670 File Offset: 0x0004A870
		public void OnEditApplied(UpgradeDatabase database, TileEdit tileEdit)
		{
			foreach (ClientTileEdit clientTileEdit in this._clientTileEdits)
			{
				if (clientTileEdit.edit == tileEdit)
				{
					this._clientTileEdits.Remove(clientTileEdit);
					this._dirty = true;
					break;
				}
			}
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x0004C664 File Offset: 0x0004A864
		public void OnUpgradesChanged(UpgradeDatabase database)
		{
			this._dirty = true;
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x0004C6DC File Offset: 0x0004A8DC
		private void UpdateDatabase()
		{
			if (this._dirty)
			{
				this._dirty = false;
				this._model.CloneInto(this);
				foreach (ClientTileEdit clientTileEdit in this._clientTileEdits)
				{
					if (!clientTileEdit.isDraft)
					{
						this.ApplyEdit(clientTileEdit.edit, this._tilemapView);
					}
				}
				Array.Copy(this._availableUpgrades, this._availableOrDraftUpgrades, this._availableUpgrades.Length);
				foreach (ClientTileEdit clientTileEdit2 in this._clientTileEdits)
				{
					if (clientTileEdit2.isDraft)
					{
						this.ApplyEdit(clientTileEdit2.edit, this._tilemapView);
					}
				}
			}
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x0004C7D0 File Offset: 0x0004A9D0
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._model != null)
			{
				this._model.Unsubscribe(this);
				this._model = null;
			}
		}

		// Token: 0x040012E9 RID: 4841
		private UpgradeDatabaseModel _model;

		// Token: 0x040012EA RID: 4842
		private bool _dirty;

		// Token: 0x040012EB RID: 4843
		private int[] _availableOrDraftUpgrades = new int[9];

		// Token: 0x040012EC RID: 4844
		private HashSet<ClientTileEdit> _clientTileEdits = new HashSet<ClientTileEdit>();

		// Token: 0x040012ED RID: 4845
		[Dependency]
		private TilemapView _tilemapView;
	}
}
