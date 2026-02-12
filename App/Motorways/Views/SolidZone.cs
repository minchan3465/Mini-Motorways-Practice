using System;
using System.Collections.Generic;

namespace Motorways.Views
{
	// Token: 0x020005F2 RID: 1522
	public class SolidZone
	{
		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x06002A46 RID: 10822 RVA: 0x000B8CC2 File Offset: 0x000B6EC2
		public PermanenceSourceUpdateOrder PermanenceSourceUpdateOrder
		{
			get
			{
				return this._permanenceSourceUpdateOrder;
			}
		}

		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x06002A47 RID: 10823 RVA: 0x000B8CCA File Offset: 0x000B6ECA
		public PermanenceSourceType SourceType
		{
			get
			{
				return this._sourceType;
			}
		}

		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x06002A48 RID: 10824 RVA: 0x000B8CD2 File Offset: 0x000B6ED2
		public TileDirection SourceDirection
		{
			get
			{
				return this._sourcePermanenceDirection;
			}
		}

		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x06002A49 RID: 10825 RVA: 0x000B8CDA File Offset: 0x000B6EDA
		public TileView SourceTileView
		{
			get
			{
				return this._sourceTileView;
			}
		}

		// Token: 0x06002A4A RID: 10826 RVA: 0x000B8CE2 File Offset: 0x000B6EE2
		public SolidZone(PermanenceTextureMappingDatabase.ZoneAddress zoneAddress, int shaderIndex, SolidZone defaultSolidZone = null)
		{
			this.zoneAddress = zoneAddress;
			this.shaderIndex = shaderIndex;
			this._defaultSolidZone = defaultSolidZone;
			this.ResetToDefaultSource();
		}

		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x06002A4B RID: 10827 RVA: 0x000B8D1E File Offset: 0x000B6F1E
		private bool IsDefaultZone
		{
			get
			{
				return this._defaultSolidZone == null;
			}
		}

		// Token: 0x06002A4C RID: 10828 RVA: 0x000B8D2C File Offset: 0x000B6F2C
		public void ResetToDefaultSource()
		{
			this._sourceType = PermanenceSourceType.Default;
			this._permanenceSourceUpdateOrder = PermanenceSourceUpdateOrder.Secondary;
			this._solidZoneSources.Clear();
			if (this._defaultSolidZone != null)
			{
				this._solidZoneSources.Add(this._defaultSolidZone);
			}
			this._sourceTileView = null;
			this._sourceTieBreaker = null;
			this._sourcePermanenceDirection = TileDirection.None;
		}

		// Token: 0x06002A4D RID: 10829 RVA: 0x000B8D85 File Offset: 0x000B6F85
		public void SetTileAndDirectionSource(TileView tileView, TileDirection tileDirection)
		{
			this._permanenceSourceUpdateOrder = PermanenceSourceUpdateOrder.Primary;
			this._sourceTileView = tileView;
			this._sourcePermanenceDirection = tileDirection;
			this._sourceType = PermanenceSourceType.TileAndDirection;
			this._solidZoneSources.Clear();
			this._sourceTieBreaker = new SolidZoneTieBreaker?(SolidZoneTieBreaker.FirstWins);
		}

		// Token: 0x06002A4E RID: 10830 RVA: 0x000B8DBA File Offset: 0x000B6FBA
		public void SetFixedValueSource(TileView tileView, TileDirection tileDirection, float permanenceValue)
		{
			this._permanenceSourceUpdateOrder = PermanenceSourceUpdateOrder.Primary;
			this._sourceTileView = tileView;
			this._sourcePermanenceDirection = tileDirection;
			this._sourceFixedValue = permanenceValue;
			this._sourceType = PermanenceSourceType.FixedValue;
			this._solidZoneSources.Clear();
			this._sourceTieBreaker = new SolidZoneTieBreaker?(SolidZoneTieBreaker.FirstWins);
		}

		// Token: 0x06002A4F RID: 10831 RVA: 0x000B8DF8 File Offset: 0x000B6FF8
		public void OfferSolidZoneSource(SolidZone solidZone, SolidZoneTieBreaker tieBreaker, PermanenceSourceUpdateOrder updateOrder = PermanenceSourceUpdateOrder.Secondary)
		{
			if (solidZone == this)
			{
				Diagnostics.FailAssert("SolidZone cannot be its own source!", Array.Empty<object>());
				return;
			}
			if (tieBreaker == SolidZoneTieBreaker.FirstWins && this._sourceTieBreaker != null)
			{
				Diagnostics.Log.Error("SolidZone", "Multiple solid zones offered for with a 'First' tiebreaker", Array.Empty<object>());
			}
			if (this._sourceType != PermanenceSourceType.SolidZone)
			{
				this._solidZoneSources.Clear();
			}
			this._sourceTileView = null;
			this._sourcePermanenceDirection = TileDirection.None;
			this._sourceType = PermanenceSourceType.SolidZone;
			SolidZoneTieBreaker valueOrDefault = this._sourceTieBreaker.GetValueOrDefault();
			if (this._sourceTieBreaker == null)
			{
				this._sourceTieBreaker = new SolidZoneTieBreaker?(tieBreaker);
			}
			this._solidZoneSources.Add(solidZone);
			this._permanenceSourceUpdateOrder = updateOrder;
		}

		// Token: 0x06002A50 RID: 10832 RVA: 0x000B8EA0 File Offset: 0x000B70A0
		public bool UsesSameSourceAs(SolidZone otherSolidZone)
		{
			if (this.SourceType != otherSolidZone.SourceType)
			{
				return false;
			}
			if (this.SourceType == PermanenceSourceType.SolidZone)
			{
				SolidZone solidZone = this.FindBaseSolidZone();
				SolidZone otherBaseSolidZone = otherSolidZone.FindBaseSolidZone();
				return solidZone == otherBaseSolidZone;
			}
			return this.SourceType == PermanenceSourceType.TileAndDirection && this._sourceTileView == otherSolidZone._sourceTileView && this._sourcePermanenceDirection == otherSolidZone._sourcePermanenceDirection;
		}

		// Token: 0x06002A51 RID: 10833 RVA: 0x000B8F08 File Offset: 0x000B7108
		public SolidZone FindBaseSolidZone()
		{
			if (this._solidZoneSources.Count == 0)
			{
				return this;
			}
			SolidZone currentSolidZone = this._solidZoneSources[0];
			while (currentSolidZone.SourceType == PermanenceSourceType.SolidZone && !currentSolidZone.IsDefaultZone && currentSolidZone._solidZoneSources.Count > 0)
			{
				currentSolidZone = currentSolidZone._solidZoneSources[0];
			}
			return currentSolidZone;
		}

		// Token: 0x06002A52 RID: 10834 RVA: 0x000B8F60 File Offset: 0x000B7160
		public void RemoveFixedSourcesIfOtherSourcesHaveBeenOffered()
		{
			bool containsNonFixedValueSource = false;
			using (List<SolidZone>.Enumerator enumerator = this._solidZoneSources.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.SourceType != PermanenceSourceType.FixedValue)
					{
						containsNonFixedValueSource = true;
					}
				}
			}
			if (containsNonFixedValueSource)
			{
				for (int sourceIndex = this._solidZoneSources.Count - 1; sourceIndex >= 0; sourceIndex--)
				{
					if (this._solidZoneSources[sourceIndex].SourceType == PermanenceSourceType.FixedValue)
					{
						this._solidZoneSources.RemoveAt(sourceIndex);
					}
				}
			}
		}

		// Token: 0x06002A53 RID: 10835 RVA: 0x000B8FF4 File Offset: 0x000B71F4
		public void OfferSolidZoneSource(TileView tileView, TileDirection tileDirection, SolidZoneTieBreaker tieBreaker)
		{
			if (this._permanenceSourceUpdateOrder == PermanenceSourceUpdateOrder.Primary)
			{
				return;
			}
			SolidZone solidZone = tileView.tileViewPermanenceZoneUpdater.GetSolidZoneInDirection(tileDirection, PermanenceTextureMappingDatabase.ZoneSharing.Local);
			this.OfferSolidZoneSource(solidZone, tieBreaker, PermanenceSourceUpdateOrder.Secondary);
		}

		// Token: 0x06002A54 RID: 10836 RVA: 0x000B9024 File Offset: 0x000B7224
		public void DecideSolidZoneSourceWinner()
		{
			if (this._solidZoneSources.Count > 1)
			{
				SolidZone solidZoneToDisplay = this.FindWinningSolidZone();
				this._solidZoneSources.Clear();
				this._solidZoneSources.Add(solidZoneToDisplay);
			}
		}

		// Token: 0x17000729 RID: 1833
		// (get) Token: 0x06002A55 RID: 10837 RVA: 0x000B9060 File Offset: 0x000B7260
		public float SourcePermanence
		{
			get
			{
				switch (this._sourceType)
				{
				case PermanenceSourceType.Default:
					if (!this.IsDefaultZone)
					{
						return this._solidZoneSources[0].SourcePermanence;
					}
					return 0f;
				case PermanenceSourceType.FixedValue:
					return this._sourceFixedValue;
				case PermanenceSourceType.TileAndDirection:
					return this._sourceTileView.GetVisualNodePermanenceProgress(this._sourcePermanenceDirection);
				case PermanenceSourceType.SolidZone:
					return this._solidZoneSources[0].SourcePermanence;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		// Token: 0x06002A56 RID: 10838 RVA: 0x000B90DC File Offset: 0x000B72DC
		private SolidZone FindWinningSolidZone()
		{
			SolidZone solidZoneToDisplay = this._solidZoneSources[0];
			SolidZoneTieBreaker? sourceTieBreaker = this._sourceTieBreaker;
			SolidZoneTieBreaker solidZoneTieBreaker = SolidZoneTieBreaker.FirstWins;
			if (sourceTieBreaker.GetValueOrDefault() == solidZoneTieBreaker & sourceTieBreaker != null)
			{
				return solidZoneToDisplay;
			}
			for (int i = 1; i < this._solidZoneSources.Count; i++)
			{
				SolidZone solidZone = this._solidZoneSources[i];
				sourceTieBreaker = this._sourceTieBreaker;
				solidZoneTieBreaker = SolidZoneTieBreaker.LeastPermanent;
				if ((sourceTieBreaker.GetValueOrDefault() == solidZoneTieBreaker & sourceTieBreaker != null) && solidZone.SourcePermanence < solidZoneToDisplay.SourcePermanence)
				{
					solidZoneToDisplay = solidZone;
				}
				sourceTieBreaker = this._sourceTieBreaker;
				solidZoneTieBreaker = SolidZoneTieBreaker.MostPermanent;
				if ((sourceTieBreaker.GetValueOrDefault() == solidZoneTieBreaker & sourceTieBreaker != null) && solidZone.SourcePermanence > solidZoneToDisplay.SourcePermanence)
				{
					solidZoneToDisplay = solidZone;
				}
			}
			return solidZoneToDisplay;
		}

		// Token: 0x1700072A RID: 1834
		// (get) Token: 0x06002A57 RID: 10839 RVA: 0x000B9198 File Offset: 0x000B7398
		public List<string> DebugStrings
		{
			get
			{
				List<string> strings = new List<string>
				{
					string.Format("({0} - {1})", this.SourceType, this.PermanenceSourceUpdateOrder)
				};
				if (this._sourceTileView != null)
				{
					strings.Add("Tile: " + this._sourceTileView.name + " - Direction: " + this._sourcePermanenceDirection.ToShortString());
				}
				if (this.SourceType == PermanenceSourceType.FixedValue)
				{
					strings.Add(string.Format("Fixed Value: {0}", this._sourceFixedValue));
				}
				else if (this.SourceType == PermanenceSourceType.SolidZone)
				{
					strings.Add(string.Format("Source Count: {0}", this._solidZoneSources.Count));
					strings.Add("First Entry: " + this._solidZoneSources[0].zoneAddress.ToString());
				}
				return strings;
			}
		}

		// Token: 0x04002461 RID: 9313
		public PermanenceTextureMappingDatabase.ZoneAddress zoneAddress;

		// Token: 0x04002462 RID: 9314
		public readonly int shaderIndex;

		// Token: 0x04002463 RID: 9315
		private PermanenceSourceType _sourceType;

		// Token: 0x04002464 RID: 9316
		private PermanenceSourceUpdateOrder _permanenceSourceUpdateOrder = PermanenceSourceUpdateOrder.Secondary;

		// Token: 0x04002465 RID: 9317
		private TileView _sourceTileView;

		// Token: 0x04002466 RID: 9318
		private TileDirection _sourcePermanenceDirection = TileDirection.None;

		// Token: 0x04002467 RID: 9319
		private float _sourceFixedValue;

		// Token: 0x04002468 RID: 9320
		private readonly List<SolidZone> _solidZoneSources = new List<SolidZone>();

		// Token: 0x04002469 RID: 9321
		private SolidZoneTieBreaker? _sourceTieBreaker;

		// Token: 0x0400246A RID: 9322
		private readonly SolidZone _defaultSolidZone;
	}
}
