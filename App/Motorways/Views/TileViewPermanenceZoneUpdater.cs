using System;
using System.Collections.Generic;
using Client;
using Easing;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005F3 RID: 1523
	public class TileViewPermanenceZoneUpdater
	{
		// Token: 0x1700072B RID: 1835
		// (get) Token: 0x06002A58 RID: 10840 RVA: 0x000B9286 File Offset: 0x000B7486
		public SolidZone[] SolidZones
		{
			get
			{
				return this._solidZones;
			}
		}

		// Token: 0x1700072C RID: 1836
		// (get) Token: 0x06002A59 RID: 10841 RVA: 0x000B928E File Offset: 0x000B748E
		public float[] ShaderSolidZonePermanenceValues
		{
			get
			{
				return this._shaderSolidZonePermanenceValues;
			}
		}

		// Token: 0x06002A5A RID: 10842 RVA: 0x000B9298 File Offset: 0x000B7498
		public TileViewPermanenceZoneUpdater(TileView tileView, VisualConstantsData visualConstants, PermanenceTextureMappingDatabase permanenceTextureMappingDatabase, ViewClient viewClient)
		{
			this._visualConstants = visualConstants;
			this._tileView = tileView;
			this._permanenceTextureMappingDatabase = permanenceTextureMappingDatabase;
			this._viewClient = viewClient;
			this._shaderSolidZonePermanenceValues = new float[permanenceTextureMappingDatabase.ShaderSolidZoneCount];
			this._solidZones = new SolidZone[permanenceTextureMappingDatabase.ShaderSolidZoneCount];
			PermanenceTextureMappingDatabase.ZoneAddress centerAddress = permanenceTextureMappingDatabase.solidZoneShaderIndices[0];
			this._centerSolidZone = new SolidZone(centerAddress, 0, null);
			this._solidZones[0] = this._centerSolidZone;
			for (int solidZoneShaderIndex = 1; solidZoneShaderIndex < permanenceTextureMappingDatabase.solidZoneShaderIndices.Length; solidZoneShaderIndex++)
			{
				PermanenceTextureMappingDatabase.ZoneAddress solidZoneAddress = permanenceTextureMappingDatabase.solidZoneShaderIndices[solidZoneShaderIndex];
				this._solidZones[solidZoneShaderIndex] = new SolidZone(solidZoneAddress, solidZoneShaderIndex, this._centerSolidZone);
			}
			this._animatedPermanenceZoneValues = new Dictionary<int, TweenFloat>();
		}

		// Token: 0x06002A5B RID: 10843 RVA: 0x000B936B File Offset: 0x000B756B
		private int GetZoneIndexFromAddress(PermanenceTextureMappingDatabase.ZoneAddress zoneAddress)
		{
			return this._permanenceTextureMappingDatabase.FindShaderSolidZoneIndex(zoneAddress);
		}

		// Token: 0x06002A5C RID: 10844 RVA: 0x000B9379 File Offset: 0x000B7579
		public SolidZone GetSolidZoneInDirection(TileDirection direction, PermanenceTextureMappingDatabase.ZoneSharing sharingStatus)
		{
			return this.GetSolidZone(direction, TileDirection.None, sharingStatus);
		}

		// Token: 0x06002A5D RID: 10845 RVA: 0x000B9384 File Offset: 0x000B7584
		public SolidZone GetSolidZone(TileDirection direction, TileDirection insideDirection, PermanenceTextureMappingDatabase.ZoneSharing sharingStatus)
		{
			PermanenceTextureMappingDatabase.ZoneAddress zoneAddress = new PermanenceTextureMappingDatabase.ZoneAddress(TileDirection.None, direction, insideDirection, sharingStatus);
			return this._solidZones[this.GetZoneIndexFromAddress(zoneAddress)];
		}

		// Token: 0x06002A5E RID: 10846 RVA: 0x000B93AA File Offset: 0x000B75AA
		private SolidZone GetPermanenceSourceForZone(PermanenceTextureMappingDatabase.ZoneAddress zoneAddress)
		{
			return this._solidZones[this.GetZoneIndexFromAddress(zoneAddress)];
		}

		// Token: 0x06002A5F RID: 10847 RVA: 0x000B93BC File Offset: 0x000B75BC
		public void UpdateSolidZonePermanenceSources()
		{
			SolidZone[] solidZones = this._solidZones;
			for (int i = 0; i < solidZones.Length; i++)
			{
				solidZones[i].ResetToDefaultSource();
			}
			foreach (TileDirection direction in this._tileView.ActiveConnectionDirections)
			{
				SolidZone solidZone = this.GetSolidZoneInDirection(direction, PermanenceTextureMappingDatabase.ZoneSharing.Local);
				if (this._tileView.ShouldDisplayDirectionAsPermanent(direction))
				{
					solidZone.SetFixedValueSource(this._tileView, direction, 1f);
				}
				else
				{
					solidZone.SetTileAndDirectionSource(this._tileView, direction);
				}
			}
			foreach (SolidZone solidZone2 in this._solidZones)
			{
				if (solidZone2.SourceType == PermanenceSourceType.TileAndDirection || solidZone2.SourceType == PermanenceSourceType.FixedValue)
				{
					this._centerSolidZone.OfferSolidZoneSource(solidZone2, SolidZoneTieBreaker.MostPermanent, PermanenceSourceUpdateOrder.Primary);
				}
			}
			this._centerSolidZone.RemoveFixedSourcesIfOtherSourcesHaveBeenOffered();
			foreach (TileDirection direction2 in this._tileView.ActiveConnectionDirections)
			{
				SolidZoneTieBreaker tieBreaker = TileUtilities.IsDirectionDiagonal(direction2) ? SolidZoneTieBreaker.MostPermanent : SolidZoneTieBreaker.LeastPermanent;
				TileDirection negativeDirection = TileUtilities.GetRotatedDirection(direction2, -1);
				TileDirection positiveDirection = TileUtilities.GetRotatedDirection(direction2, 1);
				SolidZone solidZoneInDirection = this.GetSolidZoneInDirection(negativeDirection, PermanenceTextureMappingDatabase.ZoneSharing.Local);
				SolidZone positiveSolidZone = this.GetSolidZoneInDirection(positiveDirection, PermanenceTextureMappingDatabase.ZoneSharing.Local);
				solidZoneInDirection.OfferSolidZoneSource(this._tileView, direction2, tieBreaker);
				positiveSolidZone.OfferSolidZoneSource(this._tileView, direction2, tieBreaker);
			}
			for (int shaderZoneIndex = 0; shaderZoneIndex < this._permanenceTextureMappingDatabase.solidZoneShaderIndices.Length; shaderZoneIndex++)
			{
				PermanenceTextureMappingDatabase.ZoneAddress zoneAddress = this._permanenceTextureMappingDatabase.solidZoneShaderIndices[shaderZoneIndex];
				if (zoneAddress.tile != TileDirection.None)
				{
					TileView zoneTileView = this._tileView.TilemapView.GetTileView(TileUtilities.GetAdjacentCoordinates(this._tileView.Coordinates, zoneAddress.tile));
					if (zoneTileView != null && (zoneTileView.ActiveConnectionDirections[zoneAddress.section] || zoneAddress.section == TileDirection.None))
					{
						this._solidZones[shaderZoneIndex].OfferSolidZoneSource(zoneTileView, zoneAddress.section, SolidZoneTieBreaker.FirstWins);
					}
				}
			}
			this.DecideSolidZoneSourceWinners();
			this.RemoveHarshAngles();
			this.UpdateSharedSolidZones(true);
			this.DecideSolidZoneSourceWinners();
			this.StartPrimaryZonePermanenceAnimations();
		}

		// Token: 0x06002A60 RID: 10848 RVA: 0x000B95E0 File Offset: 0x000B77E0
		private void UpdateSharedSolidZones(bool shouldUpdateAdjacentZones = true)
		{
			foreach (TileDirection direction in this._tileView.ActiveConnectionDirections)
			{
				if (TileUtilities.IsDirectionDiagonal(direction))
				{
					SolidZone sharedSolidZone = this.GetSolidZoneInDirection(direction, PermanenceTextureMappingDatabase.ZoneSharing.Shared);
					sharedSolidZone.OfferSolidZoneSource(this._tileView, direction, SolidZoneTieBreaker.MostPermanent);
					TileDirection adjacentTileDirection = TileUtilities.GetRotatedDirection(direction, -1);
					TileView adjacentTileView = this._tileView.TilemapView.GetTileView(TileUtilities.GetAdjacentCoordinates(this._tileView.Coordinates, adjacentTileDirection));
					if (adjacentTileView != null)
					{
						TileDirection adjacentTilePermanenceDirection = TileUtilities.GetRotatedDirection(direction, 2);
						if (adjacentTileView.ActiveConnectionDirections[adjacentTilePermanenceDirection])
						{
							sharedSolidZone.OfferSolidZoneSource(adjacentTileView, adjacentTilePermanenceDirection, SolidZoneTieBreaker.MostPermanent);
							if (shouldUpdateAdjacentZones)
							{
								this._adjacentTileViewsToUpdate.Add(adjacentTileView);
							}
						}
					}
				}
			}
			if (shouldUpdateAdjacentZones)
			{
				foreach (TileDirection direction2 in this._tileView.PreviouslyActiveConnectionDirections)
				{
					TileDirection adjacentTileDirection2 = TileUtilities.GetRotatedDirection(direction2, -1);
					TileView adjacentTileView2 = this._tileView.TilemapView.GetTileView(TileUtilities.GetAdjacentCoordinates(this._tileView.Coordinates, adjacentTileDirection2));
					if (adjacentTileView2 != null)
					{
						this._adjacentTileViewsToUpdate.Add(adjacentTileView2);
					}
				}
			}
			this._shouldUpdatePhantomZones = true;
		}

		// Token: 0x06002A61 RID: 10849 RVA: 0x000B9720 File Offset: 0x000B7920
		private void RemoveHarshAngles()
		{
			foreach (TileDirection direction in this._tileView.ActiveConnectionDirections)
			{
				if (TileUtilities.IsDirectionDiagonal(direction))
				{
					TileDirection negativeDirection = TileUtilities.GetRotatedDirection(direction, -1);
					TileDirection positiveDirection = TileUtilities.GetRotatedDirection(direction, 1);
					SolidZone negativeZoneSolidZone = this.GetPermanenceSourceForZone(PermanenceTextureMappingDatabase.ZoneAddress.LocalDirection(negativeDirection));
					SolidZone positiveZoneSolidZone = this.GetPermanenceSourceForZone(PermanenceTextureMappingDatabase.ZoneAddress.LocalDirection(positiveDirection));
					if (Mathf.Approximately(negativeZoneSolidZone.SourcePermanence, positiveZoneSolidZone.SourcePermanence) && negativeZoneSolidZone.UsesSameSourceAs(positiveZoneSolidZone))
					{
						positiveZoneSolidZone.ResetToDefaultSource();
					}
				}
			}
		}

		// Token: 0x06002A62 RID: 10850 RVA: 0x000B97B4 File Offset: 0x000B79B4
		private void StartAnimatingTowardsPermanenceValueForZoneIndex(int zoneIndex, float permanenceValue)
		{
			float currentFrom = this._shaderSolidZonePermanenceValues[zoneIndex];
			if (this._animatedPermanenceZoneValues.ContainsKey(zoneIndex))
			{
				this._animatedPermanenceZoneValues[zoneIndex] = new TweenFloat();
			}
			else
			{
				this._animatedPermanenceZoneValues.Add(zoneIndex, new TweenFloat());
			}
			this._animatedPermanenceZoneValues[zoneIndex].Start(currentFrom, permanenceValue, this._visualConstants.ExpertPermanentRoadsFadeDuration, Easings.Functions.Linear, 0f);
		}

		// Token: 0x06002A63 RID: 10851 RVA: 0x000B9820 File Offset: 0x000B7A20
		private void ImmediatelyUpdatePermanenceForSolidZoneAtIndex(int zoneIndex, float permanence)
		{
			this._shaderSolidZonePermanenceValues[zoneIndex] = permanence;
		}

		// Token: 0x06002A64 RID: 10852 RVA: 0x000B982C File Offset: 0x000B7A2C
		private void StartPrimaryZonePermanenceAnimations()
		{
			if (this._viewClient.OnFirstFrame)
			{
				return;
			}
			foreach (SolidZone solidZone in this._solidZones)
			{
				if (solidZone.PermanenceSourceUpdateOrder == PermanenceSourceUpdateOrder.Primary)
				{
					this.StartAnimatingTowardsPermanenceValueForZoneIndex(solidZone.shaderIndex, solidZone.SourcePermanence);
				}
			}
		}

		// Token: 0x06002A65 RID: 10853 RVA: 0x000B987A File Offset: 0x000B7A7A
		public void Tick(float deltaTime)
		{
			this.UpdateAnimatingPrimarySolidZones(deltaTime);
			this.UpdateNonAnimatingPrimarySolidZones();
		}

		// Token: 0x06002A66 RID: 10854 RVA: 0x000B988C File Offset: 0x000B7A8C
		public void LateTick(float deltaTime)
		{
			foreach (TileView tileView in this._adjacentTileViewsToUpdate)
			{
				tileView.tileViewPermanenceZoneUpdater.ClearSharedSolidZones();
				tileView.tileViewPermanenceZoneUpdater.UpdateSharedSolidZones(false);
				tileView.tileViewPermanenceZoneUpdater.DecideSolidZoneSourceWinners();
			}
			this._adjacentTileViewsToUpdate.Clear();
			if (this._shouldUpdatePhantomZones)
			{
				this.UpdatePhantomSolidZones();
				this._shouldUpdatePhantomZones = false;
			}
			this.UpdateNonPrimarySolidZones();
		}

		// Token: 0x06002A67 RID: 10855 RVA: 0x000B9920 File Offset: 0x000B7B20
		private void UpdatePhantomSolidZones()
		{
			foreach (TileDirection sectionDirection in this._tileView.ActiveConnectionDirections)
			{
				if (TileUtilities.IsDirectionDiagonal(sectionDirection))
				{
					SolidZone baseSource = this.GetSolidZoneInDirection(sectionDirection, PermanenceTextureMappingDatabase.ZoneSharing.Shared).FindBaseSolidZone();
					if (!(baseSource.SourceTileView == null))
					{
						TileDirection baseSourceDirectionForSharedZone = baseSource.SourceDirection;
						if (TileUtilities.IsDirectionDiagonal(baseSourceDirectionForSharedZone))
						{
							TileDirection oppositeDirection = TileUtilities.GetOppositeDirection(baseSourceDirectionForSharedZone);
							TileDirection phantomADirection = TileUtilities.GetRotatedDirection(oppositeDirection, -1);
							TileDirection phantomBDirection = TileUtilities.GetRotatedDirection(oppositeDirection, 1);
							TileDirection tileSampleDirectionA = TileUtilities.GetOppositeDirection(phantomBDirection);
							TileDirection tileSampleDirectionB = TileUtilities.GetOppositeDirection(phantomADirection);
							SolidZone phantomAZone = this.GetSolidZone(sectionDirection, phantomADirection, PermanenceTextureMappingDatabase.ZoneSharing.Phantom);
							SolidZone solidZone = this.GetSolidZone(sectionDirection, phantomBDirection, PermanenceTextureMappingDatabase.ZoneSharing.Phantom);
							phantomAZone.OfferSolidZoneSource(baseSource.SourceTileView, tileSampleDirectionA, SolidZoneTieBreaker.FirstWins);
							solidZone.OfferSolidZoneSource(baseSource.SourceTileView, tileSampleDirectionB, SolidZoneTieBreaker.FirstWins);
							TileView tileOppositeBaseSourceTileView = baseSource.SourceTileView.GetTileViewInDirection(baseSourceDirectionForSharedZone);
							if (!(tileOppositeBaseSourceTileView == null))
							{
								TileDirection phantomCDirection = TileUtilities.GetOppositeDirection(phantomADirection);
								TileDirection phantomDDirection = TileUtilities.GetOppositeDirection(phantomBDirection);
								SolidZone phantomCZone = this.GetSolidZone(sectionDirection, phantomCDirection, PermanenceTextureMappingDatabase.ZoneSharing.Phantom);
								SolidZone solidZone2 = this.GetSolidZone(sectionDirection, phantomDDirection, PermanenceTextureMappingDatabase.ZoneSharing.Phantom);
								TileDirection tileSampleDirectionC = TileUtilities.GetOppositeDirection(phantomDDirection);
								TileDirection tileSampleDirectionD = TileUtilities.GetOppositeDirection(phantomCDirection);
								phantomCZone.OfferSolidZoneSource(tileOppositeBaseSourceTileView, tileSampleDirectionC, SolidZoneTieBreaker.FirstWins);
								solidZone2.OfferSolidZoneSource(tileOppositeBaseSourceTileView, tileSampleDirectionD, SolidZoneTieBreaker.FirstWins);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002A68 RID: 10856 RVA: 0x000B9A60 File Offset: 0x000B7C60
		private void UpdateAnimatingPrimarySolidZones(float deltaTime)
		{
			foreach (KeyValuePair<int, TweenFloat> animatedPermanenceZoneValue in this._animatedPermanenceZoneValues)
			{
				int zoneIndex = animatedPermanenceZoneValue.Key;
				if (this._solidZones[zoneIndex].PermanenceSourceUpdateOrder == PermanenceSourceUpdateOrder.Primary)
				{
					TweenFloat zoneValue = this._animatedPermanenceZoneValues[zoneIndex];
					if (zoneValue.IsActive)
					{
						zoneValue.Tick(deltaTime);
					}
					else
					{
						this._animationsToRemove.Add(animatedPermanenceZoneValue.Key);
					}
					this.ImmediatelyUpdatePermanenceForSolidZoneAtIndex(zoneIndex, zoneValue.Value);
				}
			}
			foreach (int indexToRemove in this._animationsToRemove)
			{
				this._animatedPermanenceZoneValues.Remove(indexToRemove);
			}
			this._animationsToRemove.Clear();
		}

		// Token: 0x06002A69 RID: 10857 RVA: 0x000B9B58 File Offset: 0x000B7D58
		private void UpdateNonAnimatingPrimarySolidZones()
		{
			foreach (SolidZone solidZone in this._solidZones)
			{
				if (solidZone.PermanenceSourceUpdateOrder == PermanenceSourceUpdateOrder.Primary && !this._animatedPermanenceZoneValues.ContainsKey(solidZone.shaderIndex))
				{
					this.ImmediatelyUpdatePermanenceForSolidZoneAtIndex(solidZone.shaderIndex, solidZone.SourcePermanence);
				}
			}
		}

		// Token: 0x06002A6A RID: 10858 RVA: 0x000B9BAC File Offset: 0x000B7DAC
		private void UpdateNonPrimarySolidZones()
		{
			foreach (SolidZone solidZone in this._solidZones)
			{
				if (solidZone.PermanenceSourceUpdateOrder != PermanenceSourceUpdateOrder.Primary)
				{
					this._shaderSolidZonePermanenceValues[solidZone.shaderIndex] = solidZone.SourcePermanence;
				}
			}
		}

		// Token: 0x06002A6B RID: 10859 RVA: 0x000B9BF0 File Offset: 0x000B7DF0
		private void DecideSolidZoneSourceWinners()
		{
			SolidZone[] solidZones = this._solidZones;
			for (int i = 0; i < solidZones.Length; i++)
			{
				solidZones[i].DecideSolidZoneSourceWinner();
			}
		}

		// Token: 0x06002A6C RID: 10860 RVA: 0x000B9C1C File Offset: 0x000B7E1C
		private void ClearSharedSolidZones()
		{
			foreach (TileDirection direction in TileUtilities.DiagonalDirections)
			{
				this.GetSolidZoneInDirection(direction, PermanenceTextureMappingDatabase.ZoneSharing.Shared).ResetToDefaultSource();
				foreach (TileDirection nonDiagonalDirection in TileUtilities.NonDiagonalDirections)
				{
					this.GetSolidZone(direction, nonDiagonalDirection, PermanenceTextureMappingDatabase.ZoneSharing.Phantom).ResetToDefaultSource();
				}
			}
		}

		// Token: 0x0400246B RID: 9323
		private readonly TileView _tileView;

		// Token: 0x0400246C RID: 9324
		private readonly SolidZone[] _solidZones;

		// Token: 0x0400246D RID: 9325
		private readonly float[] _shaderSolidZonePermanenceValues;

		// Token: 0x0400246E RID: 9326
		private readonly Dictionary<int, TweenFloat> _animatedPermanenceZoneValues;

		// Token: 0x0400246F RID: 9327
		private readonly VisualConstantsData _visualConstants;

		// Token: 0x04002470 RID: 9328
		private readonly PermanenceTextureMappingDatabase _permanenceTextureMappingDatabase;

		// Token: 0x04002471 RID: 9329
		private readonly SolidZone _centerSolidZone;

		// Token: 0x04002472 RID: 9330
		private readonly ViewClient _viewClient;

		// Token: 0x04002473 RID: 9331
		private readonly List<TileView> _adjacentTileViewsToUpdate = new List<TileView>();

		// Token: 0x04002474 RID: 9332
		private bool _shouldUpdatePhantomZones;

		// Token: 0x04002475 RID: 9333
		private readonly List<int> _animationsToRemove = new List<int>(10);
	}
}
