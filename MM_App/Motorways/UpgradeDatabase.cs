using System;
using System.Collections.Generic;
using Factory;
using Motorways.Models;

namespace Motorways
{
	// Token: 0x02000454 RID: 1108
	public class UpgradeDatabase
	{
		// Token: 0x06001B97 RID: 7063 RVA: 0x00064E04 File Offset: 0x00063004
		public virtual void Reset()
		{
			for (int upgradeIndex = 0; upgradeIndex < 9; upgradeIndex++)
			{
				this._totalUpgrades[upgradeIndex] = 0;
				this._availableUpgrades[upgradeIndex] = 0;
				this._mothballedUpgrades[upgradeIndex] = 0;
			}
			this.numberOfTimesAnUpgradeIsPlaced.Clear();
		}

		// Token: 0x06001B98 RID: 7064 RVA: 0x00064E44 File Offset: 0x00063044
		public void AwardStartingPackages()
		{
			for (int upgrade = 0; upgrade < 9; upgrade++)
			{
				this._totalUpgrades[upgrade] = this._behaviour.GetAmountOfStartingUpgradesForType((UpgradeType)upgrade);
				this._availableUpgrades[upgrade] = this._totalUpgrades[upgrade];
			}
		}

		// Token: 0x06001B99 RID: 7065 RVA: 0x00064E82 File Offset: 0x00063082
		public virtual int GetAvailableUpgradeCount(UpgradeType upgradeType)
		{
			return this._availableUpgrades[(int)upgradeType];
		}

		// Token: 0x06001B9A RID: 7066 RVA: 0x00064E8C File Offset: 0x0006308C
		public virtual int GetTotalUpgradeCount(UpgradeType upgradeType)
		{
			return this._totalUpgrades[(int)upgradeType];
		}

		// Token: 0x06001B9B RID: 7067 RVA: 0x00064E96 File Offset: 0x00063096
		public virtual int GetUsedUpgradeCount(UpgradeType upgradeType)
		{
			return this.GetTotalUpgradeCount(upgradeType) - this.GetAvailableUpgradeCount(upgradeType);
		}

		// Token: 0x06001B9C RID: 7068 RVA: 0x00064EA7 File Offset: 0x000630A7
		public virtual bool HasUpgradeAvailable(UpgradeType upgradeType, int quantityRequired = 1)
		{
			return this._behaviour.HasUnlimitedOfUpgrade(upgradeType) || this.GetAvailableUpgradeCount(upgradeType) >= quantityRequired;
		}

		// Token: 0x06001B9D RID: 7069 RVA: 0x00064EC6 File Offset: 0x000630C6
		public virtual bool AddUpgradeToTotal(UpgradeType upgradeType, int quantityToAdd = 1)
		{
			this._totalUpgrades[(int)upgradeType] += quantityToAdd;
			return true;
		}

		// Token: 0x06001B9E RID: 7070 RVA: 0x00064EDC File Offset: 0x000630DC
		public virtual bool ConsumeUpgrade(UpgradeType upgradeType, int quantityToConsume = 1)
		{
			if (this.HasUpgradeAvailable(upgradeType, quantityToConsume))
			{
				if (!this.numberOfTimesAnUpgradeIsPlaced.ContainsKey(upgradeType))
				{
					this.numberOfTimesAnUpgradeIsPlaced.Add(upgradeType, quantityToConsume);
				}
				else
				{
					Dictionary<UpgradeType, int> dictionary = this.numberOfTimesAnUpgradeIsPlaced;
					dictionary[upgradeType] += quantityToConsume;
				}
				if (this._behaviour.HasUnlimitedOfUpgrade(upgradeType))
				{
					this._totalUpgrades[(int)upgradeType] += quantityToConsume;
				}
				else
				{
					this._availableUpgrades[(int)upgradeType] -= quantityToConsume;
				}
				this.NotifyUpgradesChanged();
				return true;
			}
			return false;
		}

		// Token: 0x06001B9F RID: 7071 RVA: 0x00064F64 File Offset: 0x00063164
		public virtual bool MothballUpgrade(UpgradeType upgradeType, int quantityToMothball = 1)
		{
			int maxMothballedUpgrades = this._totalUpgrades[(int)upgradeType] - this._availableUpgrades[(int)upgradeType];
			if (!Diagnostics.Verify(this._mothballedUpgrades[(int)upgradeType] + quantityToMothball <= maxMothballedUpgrades, "Mothballed more {0} upgrades than we have. Already mothballed {1} and tried to mothball {2} more, but max is {3}.", upgradeType, this._mothballedUpgrades[(int)upgradeType], quantityToMothball, maxMothballedUpgrades))
			{
				this._mothballedUpgrades[(int)upgradeType] = maxMothballedUpgrades;
				this.NotifyUpgradesChanged();
				return false;
			}
			this._mothballedUpgrades[(int)upgradeType] += quantityToMothball;
			this.NotifyUpgradesChanged();
			return true;
		}

		// Token: 0x06001BA0 RID: 7072 RVA: 0x00064FEC File Offset: 0x000631EC
		public virtual bool UnmothballUpgrade(UpgradeType upgradeType, int quantityToUnmothball = 1)
		{
			if (!Diagnostics.Verify(this._mothballedUpgrades[(int)upgradeType] >= quantityToUnmothball, string.Format("Unmothballed more {0} upgrades than we have mothballed. Expected {1}, but tried to unmothball {2}. ", upgradeType, this._mothballedUpgrades[(int)upgradeType], quantityToUnmothball)))
			{
				this._mothballedUpgrades[(int)upgradeType] = 0;
				this.NotifyUpgradesChanged();
				return false;
			}
			this._mothballedUpgrades[(int)upgradeType] -= quantityToUnmothball;
			this.NotifyUpgradesChanged();
			return true;
		}

		// Token: 0x06001BA1 RID: 7073 RVA: 0x0006505C File Offset: 0x0006325C
		public virtual bool ReleaseMothballedUpgrade(UpgradeType upgradeType, int quantityToRelease = 1)
		{
			bool success = true;
			if (!Diagnostics.Verify(quantityToRelease <= this._mothballedUpgrades[(int)upgradeType], "Tried to release more {0} than were mothballed.", upgradeType))
			{
				quantityToRelease = this._mothballedUpgrades[(int)upgradeType];
				success = false;
			}
			this._mothballedUpgrades[(int)upgradeType] -= quantityToRelease;
			if (this._behaviour.HasUnlimitedOfUpgrade(upgradeType))
			{
				this._totalUpgrades[(int)upgradeType] -= quantityToRelease;
			}
			else
			{
				this._availableUpgrades[(int)upgradeType] += quantityToRelease;
			}
			this.NotifyUpgradesChanged();
			return success;
		}

		// Token: 0x06001BA2 RID: 7074 RVA: 0x000650E3 File Offset: 0x000632E3
		public virtual bool ApplyEdit(TileEdit edit, ITilemap tilemap)
		{
			this.NotifyEditApplied(edit);
			return edit.ApplyToUpgradeDatabase(this, tilemap);
		}

		// Token: 0x06001BA3 RID: 7075 RVA: 0x000650F4 File Offset: 0x000632F4
		public virtual void CloneInto(UpgradeDatabase cloneDatabase)
		{
			bool didCloneChange = false;
			for (int upgradeIndex = 0; upgradeIndex < 9; upgradeIndex++)
			{
				didCloneChange |= (this._totalUpgrades[upgradeIndex] != cloneDatabase._totalUpgrades[upgradeIndex]);
				cloneDatabase._totalUpgrades[upgradeIndex] = this._totalUpgrades[upgradeIndex];
				didCloneChange |= (this._availableUpgrades[upgradeIndex] != cloneDatabase._availableUpgrades[upgradeIndex]);
				cloneDatabase._availableUpgrades[upgradeIndex] = this._availableUpgrades[upgradeIndex];
				didCloneChange |= (this._mothballedUpgrades[upgradeIndex] != cloneDatabase._mothballedUpgrades[upgradeIndex]);
				cloneDatabase._mothballedUpgrades[upgradeIndex] = this._mothballedUpgrades[upgradeIndex];
			}
			if (didCloneChange)
			{
				cloneDatabase.NotifyUpgradesChanged();
			}
		}

		// Token: 0x06001BA4 RID: 7076 RVA: 0x00065194 File Offset: 0x00063394
		public void Subscribe(UpgradeDatabase.IObserver observer)
		{
			this._observers.Subscribe(observer);
		}

		// Token: 0x06001BA5 RID: 7077 RVA: 0x000651A2 File Offset: 0x000633A2
		public bool Unsubscribe(UpgradeDatabase.IObserver observer)
		{
			return this._observers.Unsubscribe(observer);
		}

		// Token: 0x06001BA6 RID: 7078 RVA: 0x000651B0 File Offset: 0x000633B0
		private void NotifyEditApplied(TileEdit edit)
		{
			foreach (UpgradeDatabase.IObserver observer in this._observers)
			{
				observer.OnEditApplied(this, edit);
			}
		}

		// Token: 0x06001BA7 RID: 7079 RVA: 0x000651E4 File Offset: 0x000633E4
		protected void NotifyUpgradesChanged()
		{
			foreach (UpgradeDatabase.IObserver observer in this._observers)
			{
				observer.OnUpgradesChanged(this);
			}
		}

		// Token: 0x04001711 RID: 5905
		public static readonly UpgradeType[] UpgradeTypes = new UpgradeType[]
		{
			UpgradeType.Concrete,
			UpgradeType.Bridge,
			UpgradeType.Motorway,
			UpgradeType.TrafficLight,
			UpgradeType.Roundabout,
			UpgradeType.Tunnel
		};

		// Token: 0x04001712 RID: 5906
		[Serialize(false, null)]
		private ObserverList<UpgradeDatabase.IObserver> _observers = new ObserverList<UpgradeDatabase.IObserver>(1);

		// Token: 0x04001713 RID: 5907
		[Serialize(false, null)]
		public Dictionary<UpgradeType, int> numberOfTimesAnUpgradeIsPlaced = new Dictionary<UpgradeType, int>();

		// Token: 0x04001714 RID: 5908
		protected int[] _totalUpgrades = new int[9];

		// Token: 0x04001715 RID: 5909
		protected int[] _availableUpgrades = new int[9];

		// Token: 0x04001716 RID: 5910
		protected int[] _mothballedUpgrades = new int[9];

		// Token: 0x04001717 RID: 5911
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x02000455 RID: 1109
		public interface IObserver
		{
			// Token: 0x06001BAA RID: 7082
			void OnEditApplied(UpgradeDatabase database, TileEdit appliedEdit);

			// Token: 0x06001BAB RID: 7083
			void OnUpgradesChanged(UpgradeDatabase database);
		}
	}
}
