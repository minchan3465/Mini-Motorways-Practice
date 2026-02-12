using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using Motorways.Models;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000417 RID: 1047
	[Factory.Serializable(1)]
	public class Passage : IReusable
	{
		// Token: 0x060019BD RID: 6589 RVA: 0x0005C61C File Offset: 0x0005A81C
		public void Initialize(UpgradeType upgradeType, Vector2Int startCoordinates, Vector2Int firstCrossingCoordinates)
		{
			this._upgradeType = upgradeType;
			this._isComplete = false;
			this._startCoordinates = startCoordinates;
			this._crossingCoordinates.Add(firstCrossingCoordinates);
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x0005C63F File Offset: 0x0005A83F
		public void Reset()
		{
			this._upgradeType = UpgradeType.Concrete;
			this._isComplete = false;
			this._startCoordinates = default(Vector2Int);
			this._endCoordinates = default(Vector2Int);
			this._crossingCoordinates.Clear();
		}

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x060019BF RID: 6591 RVA: 0x0005C672 File Offset: 0x0005A872
		public bool IsComplete
		{
			get
			{
				return this._isComplete;
			}
		}

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x060019C0 RID: 6592 RVA: 0x0005C67A File Offset: 0x0005A87A
		public Vector2Int StartCoordinates
		{
			get
			{
				return this._startCoordinates;
			}
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x060019C1 RID: 6593 RVA: 0x0005C682 File Offset: 0x0005A882
		// (set) Token: 0x060019C2 RID: 6594 RVA: 0x0005C68A File Offset: 0x0005A88A
		public Vector2Int EndCoordinates
		{
			get
			{
				return this._endCoordinates;
			}
			set
			{
				this._endCoordinates = value;
				this._isComplete = true;
			}
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x060019C3 RID: 6595 RVA: 0x0005C69A File Offset: 0x0005A89A
		public IList<Vector2Int> CrossingCoordinates
		{
			get
			{
				return this._crossingCoordinates;
			}
		}

		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x060019C4 RID: 6596 RVA: 0x0005C6A4 File Offset: 0x0005A8A4
		public int Length
		{
			get
			{
				int length = this._crossingCoordinates.Count + 1;
				if (this.IsComplete)
				{
					length++;
				}
				return length;
			}
		}

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x060019C5 RID: 6597 RVA: 0x0005C6CC File Offset: 0x0005A8CC
		public UpgradeType UpgradeType
		{
			get
			{
				return this._upgradeType;
			}
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x0005C6D4 File Offset: 0x0005A8D4
		public int GetConcreteCost(ITilemap tilemap)
		{
			int concreteCost = this._behaviour.GetConcreteCostForConnection(tilemap, this._startCoordinates, this._crossingCoordinates[0]);
			int crossingTileCount = this.CrossingCoordinates.Count;
			if (!Diagnostics.Verify(crossingTileCount > 0))
			{
				return concreteCost;
			}
			for (int tileIndex = 1; tileIndex < crossingTileCount; tileIndex++)
			{
				concreteCost += this._behaviour.GetConcreteCostForConnection(tilemap, this._crossingCoordinates[tileIndex - 1], this._crossingCoordinates[tileIndex]);
			}
			if (this._isComplete)
			{
				concreteCost += this._behaviour.GetConcreteCostForConnection(tilemap, this._crossingCoordinates[crossingTileCount - 1], this.EndCoordinates);
			}
			return concreteCost;
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x0005C77C File Offset: 0x0005A97C
		public bool StartsWithConnection(Vector2Int landCoordinates, Vector2Int crossingCoordinate)
		{
			return Diagnostics.Verify(this._crossingCoordinates.Count > 0) && ((landCoordinates == this._startCoordinates && crossingCoordinate == this._crossingCoordinates[0]) || (this._isComplete && landCoordinates == this._endCoordinates && crossingCoordinate == this._crossingCoordinates[this._crossingCoordinates.Count - 1]));
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x0005C800 File Offset: 0x0005AA00
		public bool OverlapsPassage(Passage otherPassage)
		{
			return Diagnostics.Verify(otherPassage._crossingCoordinates.Count > 0) && (this.StartsWithConnection(otherPassage._startCoordinates, otherPassage._crossingCoordinates[0]) || (otherPassage.IsComplete && this.StartsWithConnection(otherPassage._endCoordinates, otherPassage._crossingCoordinates[otherPassage._crossingCoordinates.Count - 1])));
		}

		// Token: 0x060019C9 RID: 6601 RVA: 0x0005C870 File Offset: 0x0005AA70
		public bool CanBeCleared(ITilemap tilemap, Tile.TileChangePermissions permissions)
		{
			if (permissions != Tile.TileChangePermissions.RespectPermanence)
			{
				return true;
			}
			Vector2Int currentPosition = this.StartCoordinates;
			foreach (Vector2Int position in this.CrossingCoordinates)
			{
				TileDirection direction = TileUtilities.GetDirectionBetweenAdjacentCoordinates(currentPosition, position);
				if (!tilemap.GetTile(currentPosition).IsNodePermanent(direction) || !tilemap.GetTile(position).IsNodePermanent(TileUtilities.GetOppositeDirection(direction)))
				{
					return true;
				}
				currentPosition = position;
			}
			if (this.IsComplete)
			{
				TileDirection direction2 = TileUtilities.GetDirectionBetweenAdjacentCoordinates(currentPosition, this.EndCoordinates);
				if (!tilemap.GetTile(currentPosition).IsNodePermanent(direction2) || !tilemap.GetTile(this.EndCoordinates).IsNodePermanent(TileUtilities.GetOppositeDirection(direction2)))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060019CA RID: 6602 RVA: 0x0005C93C File Offset: 0x0005AB3C
		public static bool WillConnectionStartPassage(CityDefinition cityDefinition, ITilemap tilemap, Vector2Int origin, Vector2Int destination, out UpgradeType passageType)
		{
			if (Passage.WillConnectionStartPassage(cityDefinition, UpgradeType.Bridge, tilemap, origin, destination))
			{
				passageType = UpgradeType.Bridge;
				return true;
			}
			if (Passage.WillConnectionStartPassage(cityDefinition, UpgradeType.Tunnel, tilemap, origin, destination))
			{
				passageType = UpgradeType.Tunnel;
				return true;
			}
			passageType = UpgradeType.Bridge;
			return false;
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x0005C967 File Offset: 0x0005AB67
		public static bool WillConnectionJoinPassages(CityDefinition cityDefinition, ITilemap tilemap, Vector2Int origin, Vector2Int destination, out UpgradeType passageType)
		{
			if (Passage.WillConnectionJoinPassages(cityDefinition, UpgradeType.Bridge, tilemap, origin, destination))
			{
				passageType = UpgradeType.Bridge;
				return true;
			}
			if (Passage.WillConnectionJoinPassages(cityDefinition, UpgradeType.Tunnel, tilemap, origin, destination))
			{
				passageType = UpgradeType.Tunnel;
				return true;
			}
			passageType = UpgradeType.Bridge;
			return false;
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x0005C992 File Offset: 0x0005AB92
		public static bool DoesTileHavePassage(CityDefinition cityDefinition, ITilemap tilemap, Vector2Int coordinates, RoadState passageStates)
		{
			return Passage.DoesTileHavePassage(cityDefinition, UpgradeType.Bridge, tilemap, coordinates, passageStates) || Passage.DoesTileHavePassage(cityDefinition, UpgradeType.Tunnel, tilemap, coordinates, passageStates);
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x0005C9AC File Offset: 0x0005ABAC
		public static List<Passage> GetPassagesOnTile(IScope scope, CityDefinition cityDefinition, ITilemap tilemap, Vector2Int coordinates, RoadState passageStates)
		{
			List<Passage> bridges = Passage.GetPassagesOnTile(scope, cityDefinition, UpgradeType.Bridge, tilemap, coordinates, passageStates);
			List<Passage> tunnels = Passage.GetPassagesOnTile(scope, cityDefinition, UpgradeType.Tunnel, tilemap, coordinates, passageStates);
			if (bridges == null && tunnels == null)
			{
				return null;
			}
			List<Passage> passages = bridges;
			if (tunnels != null)
			{
				if (passages == null)
				{
					passages = tunnels;
				}
				else
				{
					passages.AddRange(tunnels);
				}
			}
			return passages;
		}

		// Token: 0x060019CE RID: 6606 RVA: 0x0005C9EF File Offset: 0x0005ABEF
		private void Initialize(UpgradeType upgradeType, Vector2Int startCoordinates, IEnumerable<Vector2Int> crossingCoordinates)
		{
			this._upgradeType = upgradeType;
			this._isComplete = false;
			this._startCoordinates = startCoordinates;
			this._crossingCoordinates.AddRange(crossingCoordinates);
		}

		// Token: 0x060019CF RID: 6607 RVA: 0x0005CA12 File Offset: 0x0005AC12
		private void Initialize(UpgradeType upgradeType, Vector2Int startCoordinates, Vector2Int endCoordinates, IEnumerable<Vector2Int> crossingCoordinates)
		{
			this._upgradeType = upgradeType;
			this._isComplete = true;
			this._startCoordinates = startCoordinates;
			this._endCoordinates = endCoordinates;
			this._crossingCoordinates.AddRange(crossingCoordinates);
		}

		// Token: 0x060019D0 RID: 6608 RVA: 0x0005CA40 File Offset: 0x0005AC40
		private static bool WillConnectionStartPassage(CityDefinition cityDefinition, UpgradeType passageType, ITilemap tilemap, Vector2Int origin, Vector2Int destination)
		{
			Func<Vector2Int, bool> obstructionDelegate = Passage.GetObstructionDelegate(cityDefinition, passageType);
			bool isOriginOverObstruction = obstructionDelegate(origin);
			bool isDestinationOverObstruction = obstructionDelegate(destination);
			Tile destinationTile = tilemap.GetTile(destination);
			return !isOriginOverObstruction && isDestinationOverObstruction && (destinationTile == null || destinationTile.GetTwoLaneRoadCount(RoadState.Active, Tile.MotorwayInclusion.Ignore) == 0);
		}

		// Token: 0x060019D1 RID: 6609 RVA: 0x0005CA88 File Offset: 0x0005AC88
		private static bool WillConnectionJoinPassages(CityDefinition cityDefinition, UpgradeType passageType, ITilemap tilemap, Vector2Int origin, Vector2Int destination)
		{
			Func<Vector2Int, bool> obstructionDelegate = Passage.GetObstructionDelegate(cityDefinition, passageType);
			bool isOriginOverObstruction = obstructionDelegate(origin);
			bool isDestinationOverObstruction = obstructionDelegate(destination);
			if (isOriginOverObstruction && isDestinationOverObstruction)
			{
				Tile originTile = tilemap.GetTile(origin);
				Tile destinationTile = tilemap.GetTile(destination);
				return originTile != null && originTile.GetTwoLaneRoadCount(RoadState.ActiveOrPending, Tile.MotorwayInclusion.Ignore) > 0 && destinationTile != null && destinationTile.GetTwoLaneRoadCount(RoadState.ActiveOrPending, Tile.MotorwayInclusion.Ignore) > 0;
			}
			return false;
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x0005CAE4 File Offset: 0x0005ACE4
		private static bool DoesTileHavePassage(CityDefinition cityDefinition, UpgradeType passageType, ITilemap tilemap, Vector2Int coordinates, RoadState passageStates)
		{
			Tile tile = tilemap.GetTile(coordinates);
			if (tile == null)
			{
				return false;
			}
			Func<Vector2Int, bool> isOverObstruction = Passage.GetObstructionDelegate(cityDefinition, passageType);
			if (isOverObstruction(coordinates))
			{
				return tile.GetTwoLaneRoadCount(passageStates, Tile.MotorwayInclusion.Ignore) > 0;
			}
			foreach (TileDirection roadDirection in tile.GetTwoLaneRoads(passageStates, Tile.MotorwayInclusion.Ignore))
			{
				Vector2Int connectedCoordinates = TileUtilities.GetAdjacentCoordinates(tile.Coordinates, roadDirection);
				if (isOverObstruction(connectedCoordinates))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x0005CB60 File Offset: 0x0005AD60
		private static List<Passage> GetPassagesOnTile(IScope scope, CityDefinition cityDefinition, UpgradeType passageType, ITilemap tilemap, Vector2Int coordinates, RoadState passageStates)
		{
			if (Passage.GetObstructionDelegate(cityDefinition, passageType)(coordinates))
			{
				Passage passage = Passage.GetPassageOnObstructedTile(scope, cityDefinition, passageType, tilemap, coordinates, passageStates);
				if (passage == null)
				{
					return null;
				}
				return new List<Passage>
				{
					passage
				};
			}
			else
			{
				Tile tile = tilemap.GetTile(coordinates);
				if (tile == null)
				{
					return null;
				}
				List<Passage> passages = null;
				TileDirectionBitfield trackedOutboundPassageDirections = default(TileDirectionBitfield);
				foreach (TileDirection roadDirection in tile.GetTwoLaneRoads(passageStates, Tile.MotorwayInclusion.Ignore))
				{
					if (!trackedOutboundPassageDirections[roadDirection])
					{
						Passage passage2 = Passage.GetPassageOnLandTile(scope, cityDefinition, passageType, tilemap, coordinates, roadDirection, passageStates);
						if (passage2 != null)
						{
							if (passages == null)
							{
								passages = new List<Passage>();
							}
							passages.Add(passage2);
							trackedOutboundPassageDirections[roadDirection] = true;
							if (passage2.IsComplete && passage2.EndCoordinates == passage2.StartCoordinates)
							{
								TileDirection terminatingDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(passage2.EndCoordinates, passage2.CrossingCoordinates[passage2.CrossingCoordinates.Count - 1]);
								trackedOutboundPassageDirections[terminatingDirection] = true;
							}
						}
					}
				}
				return passages;
			}
		}

		// Token: 0x060019D4 RID: 6612 RVA: 0x0005CC74 File Offset: 0x0005AE74
		private static Passage GetPassageOnLandTile(IScope scope, CityDefinition cityDefinition, UpgradeType passageType, ITilemap tilemap, Vector2Int startCoordinates, TileDirection startDirection, RoadState passageStates)
		{
			Func<Vector2Int, bool> isOverObstruction = Passage.GetObstructionDelegate(cityDefinition, passageType);
			if (isOverObstruction(startCoordinates))
			{
				return null;
			}
			TileDirection lastDirection;
			Tile passageTile = tilemap.GetTile(startCoordinates).GetAdjacentConnectedTile(out lastDirection, passageStates, new TileDirectionBitfield(startDirection));
			if (passageTile != null && isOverObstruction(passageTile.Coordinates))
			{
				Vector2Int endCoordinates = Vector2Int.zero;
				List<Vector2Int> obstructedCoordinates = new List<Vector2Int>
				{
					passageTile.Coordinates
				};
				for (;;)
				{
					passageTile = passageTile.GetAdjacentConnectedTile(out lastDirection, passageStates, ~new TileDirectionBitfield(TileUtilities.GetOppositeDirection(lastDirection)));
					if (passageTile == null)
					{
						break;
					}
					if (!isOverObstruction(passageTile.Coordinates))
					{
						goto IL_9D;
					}
					obstructedCoordinates.Add(passageTile.Coordinates);
				}
				bool isPassageComplete = false;
				goto IL_A7;
				IL_9D:
				endCoordinates = passageTile.Coordinates;
				isPassageComplete = true;
				IL_A7:
				Passage newPassage = scope.Get<Passage>();
				if (isPassageComplete)
				{
					newPassage.Initialize(passageType, startCoordinates, endCoordinates, obstructedCoordinates);
				}
				else
				{
					newPassage.Initialize(passageType, startCoordinates, obstructedCoordinates);
				}
				return newPassage;
			}
			return null;
		}

		// Token: 0x060019D5 RID: 6613 RVA: 0x0005CD54 File Offset: 0x0005AF54
		private static Passage GetPassageOnObstructedTile(IScope scope, CityDefinition cityDefinition, UpgradeType passageType, ITilemap tilemap, Vector2Int startCoordinates, RoadState passageStates)
		{
			Func<Vector2Int, bool> isOverObstruction = Passage.GetObstructionDelegate(cityDefinition, passageType);
			if (!isOverObstruction(startCoordinates))
			{
				return null;
			}
			Tile middleTile = tilemap.GetTile(startCoordinates);
			if (middleTile == null)
			{
				return null;
			}
			TileDirection firstArmDirection;
			Tile nextTile = middleTile.GetAdjacentConnectedTile(out firstArmDirection, passageStates, TileDirectionBitfield.All);
			if (nextTile != null)
			{
				List<Vector2Int> obstructedCoordinates = new List<Vector2Int>
				{
					middleTile.Coordinates
				};
				Tile firstArmEndTile = null;
				TileDirection lastDirection = firstArmDirection;
				while (nextTile != null)
				{
					if (!isOverObstruction(nextTile.Coordinates))
					{
						firstArmEndTile = nextTile;
						break;
					}
					obstructedCoordinates.Insert(0, nextTile.Coordinates);
					nextTile = nextTile.GetAdjacentConnectedTile(out lastDirection, passageStates, ~new TileDirectionBitfield(TileUtilities.GetOppositeDirection(lastDirection)));
				}
				Tile secondArmEndTile = null;
				nextTile = middleTile.GetAdjacentConnectedTile(out lastDirection, passageStates, ~new TileDirectionBitfield(firstArmDirection));
				if (nextTile != null)
				{
					while (nextTile != null)
					{
						if (!isOverObstruction(nextTile.Coordinates))
						{
							secondArmEndTile = nextTile;
							break;
						}
						obstructedCoordinates.Add(nextTile.Coordinates);
						nextTile = nextTile.GetAdjacentConnectedTile(out lastDirection, passageStates, ~new TileDirectionBitfield(TileUtilities.GetOppositeDirection(lastDirection)));
					}
				}
				Passage newPassage = scope.Get<Passage>();
				if (firstArmEndTile != null && secondArmEndTile != null)
				{
					newPassage.Initialize(passageType, firstArmEndTile.Coordinates, secondArmEndTile.Coordinates, obstructedCoordinates);
				}
				else if (firstArmEndTile != null)
				{
					newPassage.Initialize(passageType, firstArmEndTile.Coordinates, obstructedCoordinates);
				}
				else if (Diagnostics.Verify(secondArmEndTile != null))
				{
					obstructedCoordinates.Reverse();
					newPassage.Initialize(passageType, secondArmEndTile.Coordinates, obstructedCoordinates);
				}
				else
				{
					scope.Release(newPassage);
					newPassage = null;
				}
				return newPassage;
			}
			return null;
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x0005CEC8 File Offset: 0x0005B0C8
		private static Func<Vector2Int, bool> GetObstructionDelegate(CityDefinition cityDefinition, UpgradeType passageType)
		{
			Func<Vector2Int, bool> func = null;
			if (passageType == UpgradeType.Bridge)
			{
				func = new Func<Vector2Int, bool>(cityDefinition.TileIsOverWater);
			}
			else if (passageType == UpgradeType.Tunnel)
			{
				func = new Func<Vector2Int, bool>(cityDefinition.TileIsUnderAMountain);
			}
			return func;
		}

		// Token: 0x040015B3 RID: 5555
		private UpgradeType _upgradeType;

		// Token: 0x040015B4 RID: 5556
		private bool _isComplete;

		// Token: 0x040015B5 RID: 5557
		private Vector2Int _startCoordinates;

		// Token: 0x040015B6 RID: 5558
		private Vector2Int _endCoordinates;

		// Token: 0x040015B7 RID: 5559
		private readonly List<Vector2Int> _crossingCoordinates = new List<Vector2Int>();

		// Token: 0x040015B8 RID: 5560
		[Dependency]
		private GameBehaviourModel _behaviour;
	}
}
