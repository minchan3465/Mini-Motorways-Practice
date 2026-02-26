using System;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Processes
{
	// Token: 0x0200049C RID: 1180
	public class TilePermanenceUpdatingProcess : IProcess, IReusable
	{
		// Token: 0x06001D30 RID: 7472 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x000732C4 File Offset: 0x000714C4
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			if (!this._city.Rules.RoadsBecomePermanentOverTime || timestep <= Fix64.Zero)
			{
				return;
			}
			Fix64 permanenceProgressDelta = timestep / this._constants.DurationTillRoadPermanence;
			foreach (TileModel tileModel in simulation.GetModels<TileModel>())
			{
				TileDirectionBitfield directionsToIncrement = this.GetDirectionsToIncrement(tileModel, simulation);
				if (directionsToIncrement.Count > 0 || tileModel.Tile.HasTrafficLight || tileModel.Tile.IsCenterOfRoundabout)
				{
					tileModel.Tile.IncrementNodePermanenceProgress(permanenceProgressDelta, directionsToIncrement, RoadState.Active);
				}
			}
			foreach (MotorwayModel motorwayModel in simulation.GetModels<MotorwayModel>())
			{
				motorwayModel.IncrementPermanence(permanenceProgressDelta, RoadState.Active);
			}
		}

		// Token: 0x06001D32 RID: 7474 RVA: 0x0007338C File Offset: 0x0007158C
		private TileDirectionBitfield GetDirectionsToIncrement(TileModel tileModel, ISimulation simulation)
		{
			if (tileModel.Tile.ContentType == TileContentType.House || tileModel.Tile.ContentType == TileContentType.Carpark)
			{
				return TileDirectionBitfield.None;
			}
			if (!this._city.Definition.TileIsOverWater(tileModel.Coordinates) && !this._city.Definition.TileIsUnderAMountain(tileModel.Coordinates))
			{
				TileDirectionBitfield directionsToIncrement = TileDirectionBitfield.None;
				foreach (TileDirection direction in tileModel.Tile.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Ignore))
				{
					if (!tileModel.Tile.IsNodePermanent(direction) && !tileModel.Tile.IsConnectedViaDrivewayInDirection(direction))
					{
						Vector2Int neighbourCoordinate = TileUtilities.GetAdjacentCoordinates(tileModel.Coordinates, direction);
						if ((!this._city.Definition.TileIsOverWater(neighbourCoordinate) && !this._city.Definition.TileIsUnderAMountain(neighbourCoordinate)) || this._tilemap.GetTile(neighbourCoordinate) == null || !TilePermanenceUpdatingProcess.IsCoordinatePartOfIncompletePassage(neighbourCoordinate, simulation))
						{
							directionsToIncrement[direction] = true;
						}
					}
				}
				foreach (TileDirection motorwayDirection in tileModel.Tile.GetMotorwayRamps(RoadState.Active))
				{
					if (!tileModel.Tile.IsNodePermanent(motorwayDirection))
					{
						directionsToIncrement[motorwayDirection] = true;
					}
				}
				return directionsToIncrement;
			}
			if (TilePermanenceUpdatingProcess.IsCoordinatePartOfIncompletePassage(tileModel.Coordinates, simulation))
			{
				return TileDirectionBitfield.None;
			}
			return tileModel.Tile.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Ignore);
		}

		// Token: 0x06001D33 RID: 7475 RVA: 0x000734F8 File Offset: 0x000716F8
		private static bool IsCoordinatePartOfIncompletePassage(Vector2Int coordinates, ISimulation simulation)
		{
			foreach (PassageModel passage in simulation.GetModels<PassageModel>())
			{
				if (passage.Passage.CrossingCoordinates.Contains(coordinates) && !passage.Passage.IsComplete)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04001916 RID: 6422
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001917 RID: 6423
		[Dependency]
		private City _city;

		// Token: 0x04001918 RID: 6424
		[Dependency]
		private TilemapModel _tilemap;
	}
}
