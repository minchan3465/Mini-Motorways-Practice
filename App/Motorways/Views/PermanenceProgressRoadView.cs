using System;
using Motorways.Models;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005ED RID: 1517
	public class PermanenceProgressRoadView
	{
		// Token: 0x06002A31 RID: 10801 RVA: 0x000B84B0 File Offset: 0x000B66B0
		public PermanenceProgressRoadView(MaterialPropertyBlock materialPropertyBlock, Renderer renderer, TileView tileView, PermanenceZoneTextureLibrary permanenceZoneTextureLibrary, VisualConstantsData visualConstants, bool shouldShowPermanenceVisuals)
		{
			this._materialPropertyBlock = materialPropertyBlock;
			this._renderer = renderer;
			this._tileView = tileView;
			this._permanenceZoneTextureLibrary = permanenceZoneTextureLibrary;
			this._visualConstants = visualConstants;
			this.SetPermanenceVisibility(shouldShowPermanenceVisuals);
			Vector2 tilePosition = (Vector3)TilemapModel.GetWorldPositionForCoordinates(this._tileView.Coordinates);
			this._renderer.GetPropertyBlock(this._materialPropertyBlock);
			this._materialPropertyBlock.SetVector(PermanenceProgressRoadView.TileCoordinatesWorldspace, tilePosition);
			this._renderer.SetPropertyBlock(this._materialPropertyBlock);
			this.UpdatePermanenceTexturesOnRenderer();
			permanenceZoneTextureLibrary.OnTexturesRecreated += this.UpdatePermanenceTexturesOnRenderer;
			this.UpdateDebugZoneIndex();
			tileView._visualConstants.OnExpertPermanenceDebugZoneIndexChanged += this.UpdateDebugZoneIndex;
			this.UpdateDebugViewOpacity(this._tileView._visualConstants.PermanenceDebugViewOpacity);
			tileView._visualConstants.OnExpertPermanenceDebugViewOpacityChanged += delegate()
			{
				this.UpdateDebugViewOpacity(this._tileView._visualConstants.PermanenceDebugViewOpacity);
			};
		}

		// Token: 0x06002A32 RID: 10802 RVA: 0x000B85A8 File Offset: 0x000B67A8
		private void UpdatePermanenceTexturesOnRenderer()
		{
			this._renderer.GetPropertyBlock(this._materialPropertyBlock);
			this._materialPropertyBlock.SetTexture(PermanenceProgressRoadView.PermanenceIndexTexture, this._permanenceZoneTextureLibrary.PermanenceIndexTexture);
			this._materialPropertyBlock.SetTexture(PermanenceProgressRoadView.PermanenceFadeTexture, this._permanenceZoneTextureLibrary.PermanenceFadeTexture);
			this._renderer.SetPropertyBlock(this._materialPropertyBlock);
		}

		// Token: 0x06002A33 RID: 10803 RVA: 0x000B8610 File Offset: 0x000B6810
		public void SetPermanenceVisibility(bool shouldShowPermanenceVisuals)
		{
			this._shouldShowPermanenceVisuals = shouldShowPermanenceVisuals;
			this._renderer.GetPropertyBlock(this._materialPropertyBlock);
			this._materialPropertyBlock.SetFloat(PermanenceProgressRoadView.ShouldShowPermanenceVisuals, shouldShowPermanenceVisuals ? 1f : 0f);
			this._renderer.SetPropertyBlock(this._materialPropertyBlock);
		}

		// Token: 0x06002A34 RID: 10804 RVA: 0x000B8665 File Offset: 0x000B6865
		private float GetVisualPermanence(float permanence)
		{
			return this._visualConstants.DryingRoadFalloff.Evaluate(permanence);
		}

		// Token: 0x06002A35 RID: 10805 RVA: 0x000B8678 File Offset: 0x000B6878
		public void UpdatePermanenceValues()
		{
			if (!this._shouldShowPermanenceVisuals || this._tileView.tileViewPermanenceZoneUpdater == null)
			{
				return;
			}
			int roundaboutCount = 0;
			Vector2 roundaboutPermanences = new Vector2(0f, 0f);
			Vector4 roundaboutPositions = new Vector4(0f, 0f, 0f, 0f);
			Tile tile = this._tileView.Tile;
			ITilemap tilemap = tile.Tilemap;
			RoadTileConnection connection = tile.GetRoundaboutConnection(RoadState.VisiblyActive);
			if (connection != RoadTileConnection.InvalidConnection)
			{
				Vector2Int coordinatesOffset = Roundabout.GetCoordinatesOffsetForConnection(connection);
				Tile roundaboutCentreTile = tilemap.GetTile(this._tileView.Tile.Coordinates - coordinatesOffset);
				if (roundaboutCentreTile != null)
				{
					roundaboutCount = 1;
					roundaboutPermanences.x = this.GetVisualPermanence((float)roundaboutCentreTile.RoundaboutPermanenceProgress);
					roundaboutPositions = (Vector2)TilemapModel.GetWorldPositionForCoordinates(tile.Coordinates - coordinatesOffset);
				}
			}
			foreach (TileDirection diagonalDirection in TileUtilities.DiagonalDirections)
			{
				Vector2Int diagonallyAdjacentCoordinates = TileUtilities.GetAdjacentCoordinates(tile.Coordinates, diagonalDirection);
				Tile diagonallyAdjacentTile = tilemap.GetTile(diagonallyAdjacentCoordinates);
				if (diagonallyAdjacentTile != null && diagonallyAdjacentTile.IsCenterOfRoundabout)
				{
					float roundaboutPermanence = this.GetVisualPermanence((float)diagonallyAdjacentTile.RoundaboutPermanenceProgress);
					Vector2 roundaboutPosition = (Vector2)TilemapModel.GetWorldPositionForCoordinates(diagonallyAdjacentCoordinates);
					if (roundaboutCount != 0)
					{
						roundaboutPermanences.y = roundaboutPermanence;
						roundaboutPositions.z = roundaboutPosition.x;
						roundaboutPositions.w = roundaboutPosition.y;
						roundaboutCount = 2;
						break;
					}
					roundaboutPermanences.x = roundaboutPermanence;
					roundaboutPositions.x = roundaboutPosition.x;
					roundaboutPositions.y = roundaboutPosition.y;
					roundaboutCount = 1;
				}
			}
			this._renderer.GetPropertyBlock(this._materialPropertyBlock);
			this._materialPropertyBlock.SetInt(PermanenceProgressRoadView.RoundaboutCount, roundaboutCount);
			if (roundaboutCount > 0)
			{
				this._materialPropertyBlock.SetVector(PermanenceProgressRoadView.RoundaboutCenterWorldspace, roundaboutPositions);
				this._materialPropertyBlock.SetVector(PermanenceProgressRoadView.RoundaboutPermanence, roundaboutPermanences);
			}
			this._materialPropertyBlock.SetFloatArray(PermanenceProgressRoadView.PermanenceValues, this._tileView.tileViewPermanenceZoneUpdater.ShaderSolidZonePermanenceValues);
			this._renderer.SetPropertyBlock(this._materialPropertyBlock);
		}

		// Token: 0x06002A36 RID: 10806 RVA: 0x000B88A4 File Offset: 0x000B6AA4
		private void UpdateDebugZoneIndex()
		{
			if (!this._shouldShowPermanenceVisuals)
			{
				return;
			}
			this._renderer.GetPropertyBlock(this._materialPropertyBlock);
			Diagnostics.Log.Info("PermanenceProgressRoadView", "Updating debug index to {0}", new object[]
			{
				this._tileView._visualConstants.PermanenceDebugViewZoneIndex
			});
			this._materialPropertyBlock.SetInt(PermanenceProgressRoadView.DebugZoneIndex, this._tileView._visualConstants.PermanenceDebugViewZoneIndex);
			this._renderer.SetPropertyBlock(this._materialPropertyBlock);
		}

		// Token: 0x06002A37 RID: 10807 RVA: 0x000B8929 File Offset: 0x000B6B29
		private void UpdateDebugViewOpacity(float debugViewOpacity)
		{
			if (!this._shouldShowPermanenceVisuals)
			{
				return;
			}
			this._renderer.GetPropertyBlock(this._materialPropertyBlock);
			this._materialPropertyBlock.SetFloat(PermanenceProgressRoadView.ShowDebugView, debugViewOpacity);
			this._renderer.SetPropertyBlock(this._materialPropertyBlock);
		}

		// Token: 0x04002439 RID: 9273
		private const int NoDebugZoneIndex = -1;

		// Token: 0x0400243A RID: 9274
		private readonly MaterialPropertyBlock _materialPropertyBlock;

		// Token: 0x0400243B RID: 9275
		private readonly Renderer _renderer;

		// Token: 0x0400243C RID: 9276
		private readonly TileView _tileView;

		// Token: 0x0400243D RID: 9277
		private bool _shouldShowPermanenceVisuals;

		// Token: 0x0400243E RID: 9278
		private static readonly int TileCoordinatesWorldspace = Shader.PropertyToID("_TileCoordinatesWorldspace");

		// Token: 0x0400243F RID: 9279
		private static readonly int PermanenceIndexTexture = Shader.PropertyToID("_PermanenceIndexTexture");

		// Token: 0x04002440 RID: 9280
		private static readonly int PermanenceFadeTexture = Shader.PropertyToID("_PermanenceFadeTexture");

		// Token: 0x04002441 RID: 9281
		private static readonly int PermanenceValues = Shader.PropertyToID("_PermanenceValues");

		// Token: 0x04002442 RID: 9282
		private static readonly int DebugZoneIndex = Shader.PropertyToID("_DebugZoneIndex");

		// Token: 0x04002443 RID: 9283
		private static readonly int ShouldShowPermanenceVisuals = Shader.PropertyToID("_ShouldShowPermanenceZones");

		// Token: 0x04002444 RID: 9284
		private static readonly int RoundaboutCount = Shader.PropertyToID("_RoundaboutCount");

		// Token: 0x04002445 RID: 9285
		private static readonly int RoundaboutCenterWorldspace = Shader.PropertyToID("_RoundaboutCenterWorldspace");

		// Token: 0x04002446 RID: 9286
		private static readonly int RoundaboutPermanence = Shader.PropertyToID("_RoundaboutPermanence");

		// Token: 0x04002447 RID: 9287
		private readonly PermanenceZoneTextureLibrary _permanenceZoneTextureLibrary;

		// Token: 0x04002448 RID: 9288
		private static readonly int ShowDebugView = Shader.PropertyToID("_ShowDebugView");

		// Token: 0x04002449 RID: 9289
		private readonly VisualConstantsData _visualConstants;
	}
}
