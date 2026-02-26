using System;
using System.Collections.Generic;
using Factory;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x020006ED RID: 1773
	public abstract class AddMotorwayAction : MotorwaysPlayerAction
	{
		// Token: 0x06003089 RID: 12425 RVA: 0x000E3ED3 File Offset: 0x000E20D3
		public override void Reset()
		{
			base.Reset();
			this._newMotorwayId = -1;
			this._newMotorwayNumber = 0;
			this._anchorCoordinates = default(Vector2Int);
			this._anchorDirection = TileDirection.North;
			this._danglingCoordinates = default(Vector2Int);
			this._danglingDirection = TileDirection.North;
		}

		// Token: 0x0600308A RID: 12426 RVA: 0x000E3F0F File Offset: 0x000E210F
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this.SetColourWidgetRadialVisible(false);
			this._newMotorwayId = -1;
			this._newMotorwayNumber = 0;
		}

		// Token: 0x0600308B RID: 12427 RVA: 0x000E3F2D File Offset: 0x000E212D
		public override void OnActionComplete()
		{
			if (this.MotorwayBeingEdited != null)
			{
				this.MotorwayBeingEdited.IsBeingEdited = false;
			}
			base.OnActionComplete();
		}

		// Token: 0x0600308C RID: 12428 RVA: 0x000E3F4F File Offset: 0x000E214F
		public override void OnActionCancel()
		{
			if (this.MotorwayBeingEdited != null)
			{
				this.MotorwayBeingEdited.IsBeingEdited = false;
			}
			base.OnActionCancel();
		}

		// Token: 0x0600308D RID: 12429 RVA: 0x000E3F74 File Offset: 0x000E2174
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.Source != InputEventSource.Mouse)
			{
				this.OnActionComplete();
				return;
			}
			if (inputEvent.InputAction == 19 && inputEvent.ButtonState == InputEventButtonState.JustUp)
			{
				this.OnActionComplete();
				return;
			}
			if (inputEvent.InputAction == 20 && inputEvent.ButtonState == InputEventButtonState.JustDown)
			{
				this.OnActionCancel();
				return;
			}
			PlayerAction.Log.Error(string.Format("Unexpected mouse button index {0} with state {1} from input {2}!", inputEvent.InputAction, inputEvent.ButtonState, inputEvent), Array.Empty<object>());
			this.OnActionCancel();
		}

		// Token: 0x0600308E RID: 12430 RVA: 0x000E3FFC File Offset: 0x000E21FC
		protected AddMotorwayAction.MotorwayActionResult SetAnchorTile(Vector2Int anchorCoordinates, TileDirection anchorDirection)
		{
			if (!this.DoesTileSupportMotorway(anchorCoordinates))
			{
				PlayerAction.Log.Info("AddMotorwayAction cannot anchor at tile {0} (over water or not buildable).", new object[]
				{
					anchorCoordinates
				});
				return AddMotorwayAction.MotorwayActionResult.TileDoesNotSupportMotorway;
			}
			if (anchorDirection == TileDirection.None && !this.DoesTileHaveAvailableDirection(anchorCoordinates))
			{
				PlayerAction.Log.Info("AddMotorwayAction cannot find a valid direction on anchor tile {0}.", new object[]
				{
					anchorCoordinates
				});
				return AddMotorwayAction.MotorwayActionResult.NoAvailableRampDirection;
			}
			if (this._newMotorwayId == -1)
			{
				this._newMotorwayId = this._city.GetNextMotorwayIdAndIncrement();
				PlayerAction.Log.Info("AddMotorwayAction creating motorway {0}, beginning from anchor coordinates {1} in direction {2}.", new object[]
				{
					this._newMotorwayId,
					this._anchorCoordinates,
					this._anchorDirection
				});
			}
			this._anchorCoordinates = anchorCoordinates;
			this._anchorDirection = anchorDirection;
			return AddMotorwayAction.MotorwayActionResult.Success;
		}

		// Token: 0x0600308F RID: 12431 RVA: 0x000E40C8 File Offset: 0x000E22C8
		private bool CrossesRailDiagonal(Vector2Int start, TileDirection direction)
		{
			Vector2Int vectorDirection = TileUtilities.GetAdjacencyOffsetForDirection(direction);
			Vector2Int horizontal = new Vector2Int(vectorDirection.x, 0);
			Vector2Int vertical = new Vector2Int(0, vectorDirection.y);
			Tile horizontalTile = this._tilemapView.GetTile(start + horizontal);
			Tile verticalTile = this._tilemapView.GetTile(start + vertical);
			return horizontalTile != null && horizontalTile.HasRailConnection && verticalTile != null && verticalTile.HasRailConnection;
		}

		// Token: 0x06003090 RID: 12432 RVA: 0x000E413C File Offset: 0x000E233C
		protected AddMotorwayAction.MotorwayActionResult SetDanglingTile(Vector2Int danglingCoordinates)
		{
			if (!this.DoesTileSupportMotorway(danglingCoordinates))
			{
				return AddMotorwayAction.MotorwayActionResult.TileDoesNotSupportMotorway;
			}
			if (this.HasMotorwayOnTile(danglingCoordinates, -1))
			{
				return AddMotorwayAction.MotorwayActionResult.TileDoesNotSupportMotorway;
			}
			if (Mathf.Abs(danglingCoordinates.x - this._anchorCoordinates.x) <= 1 && Mathf.Abs(danglingCoordinates.y - this._anchorCoordinates.y) <= 1)
			{
				return AddMotorwayAction.MotorwayActionResult.TooShort;
			}
			TileDirectionBitfield availableAnchorDirections = this.GetAvailableMotorwayDirections(this._anchorCoordinates);
			TileDirectionBitfield availableDanglingDirections = this.GetAvailableMotorwayDirections(danglingCoordinates);
			if (availableAnchorDirections.Count * availableDanglingDirections.Count == 0)
			{
				return AddMotorwayAction.MotorwayActionResult.NoAvailableRampDirection;
			}
			Vector2 anchorToDanglingDirection = (danglingCoordinates - this._anchorCoordinates).normalized;
			TileDirection directAnchorDirection = TileUtilities.GetClosestDirection(anchorToDanglingDirection);
			if (Mathf.Abs(TileUtilities.GetDistanceBetweenDirections(this._anchorDirection, directAnchorDirection)) <= 1)
			{
				TileDirection preferredDirection;
				if (Vector2.Dot(TileUtilities.GetVectorForDirection(this._anchorDirection), anchorToDanglingDirection) <= Mathf.Cos(0.7853982f) && !this.CrossesRailDiagonal(this._anchorCoordinates, directAnchorDirection))
				{
					preferredDirection = directAnchorDirection;
				}
				else
				{
					preferredDirection = this._anchorDirection;
				}
				if (preferredDirection != TileDirection.None && availableAnchorDirections[preferredDirection])
				{
					availableAnchorDirections = new TileDirectionBitfield(preferredDirection);
				}
			}
			Vector2 anchorToDanglingTangent = anchorToDanglingDirection.GetTangent();
			List<Tuple<TileDirection, TileDirection, float>> matches = new List<Tuple<TileDirection, TileDirection, float>>();
			foreach (TileDirection anchorDirection in availableAnchorDirections)
			{
				Vector2 vectorForDirection = TileUtilities.GetVectorForDirection(anchorDirection);
				float anchorDirectionDirectness = Vector2.Dot(vectorForDirection, anchorToDanglingDirection) * 0.5f + 0.5f;
				float anchorDirectionSide = Vector2.Dot(vectorForDirection, anchorToDanglingTangent);
				bool anchorCrossesDiagonalRail = this.CrossesRailDiagonal(this._anchorCoordinates, anchorDirection);
				foreach (TileDirection danglingDirection in availableDanglingDirections)
				{
					Vector2 vectorForDirection2 = TileUtilities.GetVectorForDirection(danglingDirection);
					float danglingDirectionDirectness = Vector2.Dot(vectorForDirection2, -anchorToDanglingDirection) * 0.5f + 0.5f;
					float danglingDirectionSide = Vector2.Dot(vectorForDirection2, anchorToDanglingTangent);
					float pairFitness = anchorDirectionDirectness * danglingDirectionDirectness;
					if (anchorDirectionSide * danglingDirectionSide > 0f)
					{
						pairFitness += 0.1f;
					}
					if (anchorCrossesDiagonalRail)
					{
						pairFitness -= 0.5f;
					}
					if (this.CrossesRailDiagonal(danglingCoordinates, danglingDirection))
					{
						pairFitness -= 0.5f;
					}
					int matchAddIndex = 0;
					foreach (Tuple<TileDirection, TileDirection, float> currentMatch in matches)
					{
						if (pairFitness > currentMatch.Item3)
						{
							break;
						}
						matchAddIndex++;
					}
					matches.Insert(matchAddIndex, new Tuple<TileDirection, TileDirection, float>(anchorDirection, danglingDirection, pairFitness));
				}
			}
			if (matches.Count == 0 || (matches[0].Item1 == TileDirection.None && matches[0].Item2 == TileDirection.None))
			{
				return AddMotorwayAction.MotorwayActionResult.NoAvailableRampPairing;
			}
			this._anchorDirection = matches[0].Item1;
			this._danglingDirection = matches[0].Item2;
			this._danglingCoordinates = danglingCoordinates;
			return AddMotorwayAction.MotorwayActionResult.Success;
		}

		// Token: 0x06003091 RID: 12433 RVA: 0x000E4400 File Offset: 0x000E2600
		protected bool HasMotorwayOnTile(Vector2Int position, int editedMotorwayId = -1)
		{
			Tile tile = this._tilemapView.GetTile(position);
			if (tile != null)
			{
				if (tile.UnbuiltMotorwayId != -1 && tile.UnbuiltMotorwayId != editedMotorwayId)
				{
					return true;
				}
				foreach (TileDirection direction in tile.GetMotorwayRamps(RoadState.VisiblyActive))
				{
					if (tile.GetMotorwayInDirection(direction, RoadState.VisiblyActive) != editedMotorwayId)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003092 RID: 12434 RVA: 0x000E4464 File Offset: 0x000E2664
		protected void DisplayError(AddMotorwayAction.MotorwayActionResult errorCode, bool errorPertainsToAnchor)
		{
			StringId errorStringId = StringId.None;
			float delay = 0f;
			switch (errorCode)
			{
			case AddMotorwayAction.MotorwayActionResult.TileDoesNotSupportMotorway:
				errorStringId = StringId.Error_TileDoesntSupportMotorway;
				delay = 0.5f;
				break;
			case AddMotorwayAction.MotorwayActionResult.TooShort:
				errorStringId = StringId.Error_MotorwayTooShort;
				delay = 1f;
				break;
			case AddMotorwayAction.MotorwayActionResult.NoAvailableRampDirection:
				errorStringId = StringId.Error_MotorwayNoAvailableRampDirection;
				delay = 0.5f;
				break;
			case AddMotorwayAction.MotorwayActionResult.NoAvailableRampPairing:
				errorStringId = StringId.Error_MotorwayNoAvailableRampDirection;
				delay = 0.5f;
				break;
			case AddMotorwayAction.MotorwayActionResult.CollidesWithMountain:
				errorStringId = StringId.Error_MotorwayCollidesWithMountain;
				delay = 0.5f;
				break;
			}
			if (errorStringId != StringId.None)
			{
				this._notificationView.AddNotification(errorStringId, delay, null);
			}
		}

		// Token: 0x06003093 RID: 12435 RVA: 0x000E44F4 File Offset: 0x000E26F4
		protected bool UpdateTileEdit()
		{
			TileEditResult editResult = this.CreateTileEdit(this._newMotorwayId, this._newMotorwayNumber, this._anchorCoordinates, this._anchorDirection, this._danglingCoordinates, this._danglingDirection);
			if (editResult.IsSuccessful)
			{
				base.ClearDraftClientEdits();
				base.AddTileEdit(editResult.edit, MotorwaysPlayerAction.EditExecuteTiming.Draft);
				if (this.MotorwayBeingEdited != null)
				{
					this.MotorwayBeingEdited.IsBeingEdited = true;
				}
				this._feedbackGenerator.GenerateFeedback(HapticFeedbackType.LightImpact);
			}
			return editResult.IsSuccessful;
		}

		// Token: 0x06003094 RID: 12436
		protected abstract TileEditResult CreateTileEdit(int newMotorwayId, int motorwayNumber, Vector2Int anchorCoordinates, TileDirection anchorDirection, Vector2Int danglingCoordinates, TileDirection danglingDirection);

		// Token: 0x06003095 RID: 12437 RVA: 0x000E4578 File Offset: 0x000E2778
		protected bool DoesTileSupportMotorway(Vector2Int coordinates)
		{
			if (!this._city.Definition.TileIsBuildable(coordinates) || this._city.Definition.TileIsOverWater(coordinates) || this._city.Definition.TileIsUnderAMountain(coordinates) || this._city.Definition.TileIsOverRail(coordinates))
			{
				return false;
			}
			if (!this._city.IsTileInPlayableArea(coordinates, this._clockModel.ExpansionTime))
			{
				return false;
			}
			Tile targetTile = this._tilemapView.GetTile(coordinates);
			return targetTile == null || targetTile.ContentType == TileContentType.None;
		}

		// Token: 0x06003096 RID: 12438 RVA: 0x000E460C File Offset: 0x000E280C
		private bool DoesTileHaveAvailableDirection(Vector2Int coordinates)
		{
			Tile tile = this._tilemapView.GetTile(coordinates);
			if (tile == null)
			{
				return true;
			}
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				if (TileEditor.TileSupportsMotorwayInDirection(tile, (TileDirection)directionIndex, this._city.NextMotorwayId))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003097 RID: 12439 RVA: 0x000E4650 File Offset: 0x000E2850
		private TileDirectionBitfield GetAvailableMotorwayDirections(Vector2Int coordinates)
		{
			Tile tile = this._tilemapView.GetTile(coordinates);
			if (tile == null)
			{
				return TileDirectionBitfield.All;
			}
			TileDirectionBitfield availableDirections = default(TileDirectionBitfield);
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				TileDirection direction = (TileDirection)directionIndex;
				availableDirections[direction] = tile.CanSetNodeState(new RoadTileNode(direction, RoadType.Motorway, (this._newMotorwayId != -1) ? this._newMotorwayId : this._city.NextMotorwayId), RoadState.Planned, Tile.TileChangePermissions.Full);
			}
			return availableDirections;
		}

		// Token: 0x06003098 RID: 12440 RVA: 0x000E46C0 File Offset: 0x000E28C0
		protected TileDirection ValidTileDirectionFor(Vector2Int start, Vector2Int end)
		{
			Vector2 startToEnd = (end - start).normalized;
			TileDirection preferredDirection = TileUtilities.GetClosestDirection(startToEnd);
			bool favourClockwise = Vector2.Dot(TileUtilities.DirectionToTileAdjacencyOffset[(int)TileUtilities.GetRotatedDirection(preferredDirection, 2)], startToEnd) > 0f;
			Tile tile = this._tilemapView.GetTile(start);
			if (tile == null)
			{
				return preferredDirection;
			}
			foreach (TileDirection directionOption in TileUtilities.GetRadiatedDirections(preferredDirection, favourClockwise))
			{
				if (TileEditor.TileSupportsMotorwayInDirection(tile, directionOption, (this._newMotorwayId != -1) ? this._newMotorwayId : this._city.NextMotorwayId))
				{
					return directionOption;
				}
			}
			return TileDirection.None;
		}

		// Token: 0x17000819 RID: 2073
		// (get) Token: 0x06003099 RID: 12441 RVA: 0x000E4790 File Offset: 0x000E2990
		protected MotorwayView MotorwayBeingEdited
		{
			get
			{
				if (this._newMotorwayId != -1)
				{
					return this._tilemapView.GetMotorwayView(this._newMotorwayId);
				}
				return null;
			}
		}

		// Token: 0x040029CC RID: 10700
		[Dependency]
		protected IAudioSystem _audioSystem;

		// Token: 0x040029CD RID: 10701
		[Dependency]
		private NotificationView _notificationView;

		// Token: 0x040029CE RID: 10702
		protected int _newMotorwayId = -1;

		// Token: 0x040029CF RID: 10703
		protected int _newMotorwayNumber;

		// Token: 0x040029D0 RID: 10704
		protected Vector2Int _anchorCoordinates;

		// Token: 0x040029D1 RID: 10705
		protected TileDirection _anchorDirection;

		// Token: 0x040029D2 RID: 10706
		protected Vector2Int _danglingCoordinates;

		// Token: 0x040029D3 RID: 10707
		protected TileDirection _danglingDirection;

		// Token: 0x020006EE RID: 1774
		protected enum MotorwayActionResult
		{
			// Token: 0x040029D5 RID: 10709
			Success,
			// Token: 0x040029D6 RID: 10710
			TileDoesNotSupportMotorway,
			// Token: 0x040029D7 RID: 10711
			TooShort,
			// Token: 0x040029D8 RID: 10712
			NoAvailableRampDirection,
			// Token: 0x040029D9 RID: 10713
			NoAvailableRampPairing,
			// Token: 0x040029DA RID: 10714
			CollidesWithMountain
		}
	}
}
