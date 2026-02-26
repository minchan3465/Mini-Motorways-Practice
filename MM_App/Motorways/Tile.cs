using System;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000434 RID: 1076
	[Factory.Serializable(1)]
	public class Tile : IReusable
	{
		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06001A93 RID: 6803 RVA: 0x00060F33 File Offset: 0x0005F133
		// (set) Token: 0x06001A94 RID: 6804 RVA: 0x00060F3B File Offset: 0x0005F13B
		[Serialize(true, null)]
		public ITilemap Tilemap { get; private set; }

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x06001A95 RID: 6805 RVA: 0x00060F44 File Offset: 0x0005F144
		// (set) Token: 0x06001A96 RID: 6806 RVA: 0x00060F4C File Offset: 0x0005F14C
		[Serialize(true, null)]
		public Vector2Int Coordinates { get; private set; }

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x06001A97 RID: 6807 RVA: 0x00060F55 File Offset: 0x0005F155
		// (set) Token: 0x06001A98 RID: 6808 RVA: 0x00060F5D File Offset: 0x0005F15D
		[Serialize(true, null)]
		public TileContentType ContentType { get; private set; }

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x06001A99 RID: 6809 RVA: 0x00060F66 File Offset: 0x0005F166
		// (set) Token: 0x06001A9A RID: 6810 RVA: 0x00060F6E File Offset: 0x0005F16E
		[Serialize(true, null)]
		public IModel ContentModel { get; private set; }

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06001A9B RID: 6811 RVA: 0x00060F77 File Offset: 0x0005F177
		public Fix64 TrafficLightPermanenceProgress
		{
			get
			{
				return this._trafficLightPermanenceProgress;
			}
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06001A9C RID: 6812 RVA: 0x00060F7F File Offset: 0x0005F17F
		// (set) Token: 0x06001A9D RID: 6813 RVA: 0x00060F87 File Offset: 0x0005F187
		public bool HasTrafficLight
		{
			get
			{
				return this._hasTrafficLight;
			}
			set
			{
				if (this._hasTrafficLight != value)
				{
					this._hasTrafficLight = value;
					this.NotifyTileChanged();
				}
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06001A9E RID: 6814 RVA: 0x00060F9F File Offset: 0x0005F19F
		public bool IsTrafficLightPermanent
		{
			get
			{
				return this._trafficLightPermanenceProgress >= Fix64.One;
			}
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06001A9F RID: 6815 RVA: 0x00060FB1 File Offset: 0x0005F1B1
		public bool HasRailConnection
		{
			get
			{
				return this._railConnection != RailTileConnection.InvalidConnection;
			}
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06001AA0 RID: 6816 RVA: 0x00060FC3 File Offset: 0x0005F1C3
		public RailTileConnection RailConnection
		{
			get
			{
				return this._railConnection;
			}
		}

		// Token: 0x06001AA1 RID: 6817 RVA: 0x00060FCB File Offset: 0x0005F1CB
		public void SetRailConnection(RailTileConnection connection)
		{
			if (this._railConnection == connection)
			{
				return;
			}
			this._railConnection = connection;
			this.NotifyTileChanged();
		}

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06001AA2 RID: 6818 RVA: 0x00060FE9 File Offset: 0x0005F1E9
		public bool HasBoatPathConnection
		{
			get
			{
				return this._boatPathConnection != BoatPathTileConnection.InvalidConnection;
			}
		}

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x06001AA3 RID: 6819 RVA: 0x00060FFB File Offset: 0x0005F1FB
		public BoatPathTileConnection BoatPathConnection
		{
			get
			{
				return this._boatPathConnection;
			}
		}

		// Token: 0x06001AA4 RID: 6820 RVA: 0x00061003 File Offset: 0x0005F203
		public void SetBoatPathConnection(BoatPathTileConnection connection)
		{
			if (this._boatPathConnection == connection)
			{
				return;
			}
			this._boatPathConnection = connection;
			this.NotifyTileChanged();
		}

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06001AA5 RID: 6821 RVA: 0x00061021 File Offset: 0x0005F221
		// (set) Token: 0x06001AA6 RID: 6822 RVA: 0x00061029 File Offset: 0x0005F229
		public bool IsCenterOfRoundabout
		{
			get
			{
				return this._isCenterOfRoundabout;
			}
			set
			{
				if (this._isCenterOfRoundabout != value)
				{
					this._isCenterOfRoundabout = value;
					this.NotifyTileChanged();
				}
			}
		}

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06001AA7 RID: 6823 RVA: 0x00061041 File Offset: 0x0005F241
		public Fix64 RoundaboutPermanenceProgress
		{
			get
			{
				return this._roundaboutPermanenceProgress;
			}
		}

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x06001AA8 RID: 6824 RVA: 0x00061049 File Offset: 0x0005F249
		public bool IsRoundaboutPermanent
		{
			get
			{
				return this._roundaboutPermanenceProgress >= Fix64.One;
			}
		}

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x06001AA9 RID: 6825 RVA: 0x0006105B File Offset: 0x0005F25B
		// (set) Token: 0x06001AAA RID: 6826 RVA: 0x00061063 File Offset: 0x0005F263
		public int UnbuiltMotorwayId
		{
			get
			{
				return this._unbuiltMotorwayId;
			}
			set
			{
				if (this._unbuiltMotorwayId == value)
				{
					return;
				}
				this._unbuiltMotorwayId = value;
				this.NotifyTileChanged();
			}
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06001AAB RID: 6827 RVA: 0x0006107C File Offset: 0x0005F27C
		// (set) Token: 0x06001AAC RID: 6828 RVA: 0x00061084 File Offset: 0x0005F284
		public int UnbuiltMotorwayNumber
		{
			get
			{
				return this._unbuiltMotorwayNumber;
			}
			set
			{
				if (this._unbuiltMotorwayNumber == value)
				{
					return;
				}
				this._unbuiltMotorwayNumber = value;
				this.NotifyTileChanged();
			}
		}

		// Token: 0x06001AAD RID: 6829 RVA: 0x0006109D File Offset: 0x0005F29D
		public void Initialize(ITilemap tilemap, Vector2Int coordinates, TileContentType contentType)
		{
			this.Tilemap = tilemap;
			this.Coordinates = coordinates;
			this.ContentType = contentType;
		}

		// Token: 0x06001AAE RID: 6830 RVA: 0x000610B4 File Offset: 0x0005F2B4
		public bool CloneInto(Tile cloneTile)
		{
			bool didTileChange = false;
			didTileChange |= (cloneTile.ContentType != this.ContentType);
			cloneTile.ContentType = this.ContentType;
			didTileChange |= (cloneTile.HasTrafficLight != this.HasTrafficLight);
			cloneTile.HasTrafficLight = this.HasTrafficLight;
			didTileChange |= (cloneTile.IsCenterOfRoundabout != this.IsCenterOfRoundabout);
			cloneTile.IsCenterOfRoundabout = this.IsCenterOfRoundabout;
			didTileChange |= (cloneTile.UnbuiltMotorwayId != this.UnbuiltMotorwayId);
			cloneTile.UnbuiltMotorwayId = this.UnbuiltMotorwayId;
			didTileChange |= (cloneTile.UnbuiltMotorwayNumber != this.UnbuiltMotorwayNumber);
			cloneTile.UnbuiltMotorwayNumber = this.UnbuiltMotorwayNumber;
			didTileChange |= (cloneTile._trafficLightPermanenceProgress != this._trafficLightPermanenceProgress);
			cloneTile._trafficLightPermanenceProgress = this._trafficLightPermanenceProgress;
			didTileChange |= (cloneTile._roundaboutPermanenceProgress != this._roundaboutPermanenceProgress);
			cloneTile._roundaboutPermanenceProgress = this._roundaboutPermanenceProgress;
			didTileChange |= (cloneTile._plannedRoundaboutInput != this._plannedRoundaboutInput || cloneTile._plannedRoundaboutOutput != this._plannedRoundaboutOutput || cloneTile._activeRoundaboutInput != this._activeRoundaboutInput || cloneTile._activeRoundaboutOutput != this._activeRoundaboutOutput || cloneTile._mothballedRoundaboutInput != this._mothballedRoundaboutInput || cloneTile._mothballedRoundaboutOutput != this._mothballedRoundaboutOutput);
			cloneTile._plannedRoundaboutInput = this._plannedRoundaboutInput;
			cloneTile._plannedRoundaboutOutput = this._plannedRoundaboutOutput;
			cloneTile._activeRoundaboutInput = this._activeRoundaboutInput;
			cloneTile._activeRoundaboutOutput = this._activeRoundaboutOutput;
			cloneTile._mothballedRoundaboutInput = this._mothballedRoundaboutInput;
			cloneTile._mothballedRoundaboutOutput = this._mothballedRoundaboutOutput;
			didTileChange |= !cloneTile._isDirectionImmutable.Equals(this._isDirectionImmutable);
			cloneTile._isDirectionImmutable = this._isDirectionImmutable;
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				didTileChange |= (this._twoLaneRoadState[directionIndex] != cloneTile._twoLaneRoadState[directionIndex] || this._plannedMotorways[directionIndex] != cloneTile._plannedMotorways[directionIndex] || this._nodePermanenceProgress[directionIndex] != cloneTile._nodePermanenceProgress[directionIndex] || this._activeMotorways[directionIndex] != cloneTile._activeMotorways[directionIndex] || this._mothballedMotorways[directionIndex] != cloneTile._mothballedMotorways[directionIndex]);
				cloneTile._twoLaneRoadState[directionIndex] = this._twoLaneRoadState[directionIndex];
				cloneTile._plannedMotorways[directionIndex] = this._plannedMotorways[directionIndex];
				cloneTile._nodePermanenceProgress[directionIndex] = this._nodePermanenceProgress[directionIndex];
				cloneTile._activeMotorways[directionIndex] = this._activeMotorways[directionIndex];
				cloneTile._mothballedMotorways[directionIndex] = this._mothballedMotorways[directionIndex];
			}
			didTileChange |= (cloneTile._railConnection != this._railConnection);
			cloneTile._railConnection = this._railConnection;
			if (didTileChange)
			{
				cloneTile.NotifyTileChanged();
			}
			return didTileChange;
		}

		// Token: 0x06001AAF RID: 6831 RVA: 0x00061370 File Offset: 0x0005F570
		public TileDirectionBitfield GetTwoLaneRoads(RoadState states = RoadState.Active, Tile.MotorwayInclusion motorwayInclusion = Tile.MotorwayInclusion.Ignore)
		{
			TileDirectionBitfield twoLaneRoads = default(TileDirectionBitfield);
			for (int twoLaneRoadDirectionIndex = 0; twoLaneRoadDirectionIndex < this._twoLaneRoadState.Length; twoLaneRoadDirectionIndex++)
			{
				TileDirection twoLaneRoadDirection = (TileDirection)twoLaneRoadDirectionIndex;
				twoLaneRoads[twoLaneRoadDirection] = ((this._twoLaneRoadState[twoLaneRoadDirectionIndex] & states) > RoadState.None);
				if (motorwayInclusion == Tile.MotorwayInclusion.Include)
				{
					ref TileDirectionBitfield ptr = ref twoLaneRoads;
					TileDirection direction = twoLaneRoadDirection;
					ptr[direction] |= this.HasMotorwayInDirection(twoLaneRoadDirection, states);
				}
			}
			return twoLaneRoads;
		}

		// Token: 0x06001AB0 RID: 6832 RVA: 0x000613D3 File Offset: 0x0005F5D3
		public RoadState StateOfRoadInDirection(TileDirection direction)
		{
			return this._twoLaneRoadState[(int)direction];
		}

		// Token: 0x06001AB1 RID: 6833 RVA: 0x000613D3 File Offset: 0x0005F5D3
		public RoadState GetTwoLaneRoadStateInDirection(TileDirection direction)
		{
			return this._twoLaneRoadState[(int)direction];
		}

		// Token: 0x06001AB2 RID: 6834 RVA: 0x000613DD File Offset: 0x0005F5DD
		public bool HasTwoLaneRoadInDirection(TileDirection direction, RoadState states = RoadState.Active)
		{
			return direction != TileDirection.None && (this._twoLaneRoadState[(int)direction] & states) > RoadState.None;
		}

		// Token: 0x06001AB3 RID: 6835 RVA: 0x000613F4 File Offset: 0x0005F5F4
		public int GetTwoLaneRoadCount(RoadState states = RoadState.Active, Tile.MotorwayInclusion motorwayInclusion = Tile.MotorwayInclusion.Ignore)
		{
			int count = 0;
			for (int twoLaneRoadDirectionIndex = 0; twoLaneRoadDirectionIndex < this._twoLaneRoadState.Length; twoLaneRoadDirectionIndex++)
			{
				bool hasRoadInDirection = (this._twoLaneRoadState[twoLaneRoadDirectionIndex] & states) > RoadState.None;
				if (motorwayInclusion == Tile.MotorwayInclusion.Include)
				{
					hasRoadInDirection |= this.HasMotorwayInDirection((TileDirection)twoLaneRoadDirectionIndex, states);
				}
				count += (hasRoadInDirection ? 1 : 0);
			}
			return count;
		}

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x06001AB4 RID: 6836 RVA: 0x00061440 File Offset: 0x0005F640
		public TileDirection DrivewayDirection
		{
			get
			{
				if (!Diagnostics.Verify(this.ContentType == TileContentType.House || this.ContentType == TileContentType.Carpark, "It's a bit sketch requesting a driveway direction from something that's not a house."))
				{
					return TileDirection.None;
				}
				for (int twoLaneRoadDirection = 0; twoLaneRoadDirection < this._twoLaneRoadState.Length; twoLaneRoadDirection++)
				{
					if ((this._twoLaneRoadState[twoLaneRoadDirection] & RoadState.ActiveOrPending) != RoadState.None)
					{
						return (TileDirection)twoLaneRoadDirection;
					}
				}
				return TileDirection.None;
			}
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x00061494 File Offset: 0x0005F694
		public RoadTileSignature CreateSignature(RoadState states)
		{
			RoadTileSignature signature = this._scope.Get<RoadTileSignature>();
			TileDirection roundaboutInputDirection = TileDirection.None;
			TileDirection roundaboutOutputDirection = TileDirection.None;
			bool testExitNodes = false;
			if (this.HasRoundabout(states))
			{
				RoadTileConnection roundaboutConnection = this.GetRoundaboutConnection(states);
				signature.AddConnection(roundaboutConnection);
				if ((states & RoadState.Mothballed) != RoadState.None)
				{
					roundaboutInputDirection = roundaboutConnection.input.direction;
					roundaboutOutputDirection = roundaboutConnection.output.direction;
					testExitNodes = true;
				}
			}
			RoadType twoLaneRoadType = RoadType.TwoLane;
			if (this.ContentType == TileContentType.House)
			{
				states &= ~RoadState.Mothballed;
				twoLaneRoadType = RoadType.Driveway;
			}
			foreach (TileDirection direction in this.GetTwoLaneRoads(states, Tile.MotorwayInclusion.Ignore))
			{
				RoadTileNode twoLaneNode = new RoadTileNode(direction, twoLaneRoadType, -1);
				if (!testExitNodes || Roundabout.CanConnectionAddExitNode(roundaboutInputDirection, roundaboutOutputDirection, twoLaneNode))
				{
					signature.AddNode(twoLaneNode);
				}
			}
			foreach (TileDirection direction2 in this.GetMotorwayRamps(states))
			{
				int motorwayId = -1;
				if ((states & RoadState.Active) != RoadState.None && this.HasMotorwayInDirection(direction2, RoadState.Active))
				{
					motorwayId = this._activeMotorways[(int)direction2];
				}
				if (motorwayId == -1 && (states & RoadState.Planned) != RoadState.None && this.HasMotorwayInDirection(direction2, RoadState.Planned))
				{
					motorwayId = this._plannedMotorways[(int)direction2];
				}
				if (motorwayId == -1 && (states & RoadState.Mothballed) != RoadState.None && this.HasMotorwayInDirection(direction2, RoadState.Mothballed))
				{
					motorwayId = this._mothballedMotorways[(int)direction2];
				}
				RoadTileNode motorwayNode = new RoadTileNode(direction2, RoadType.Motorway, motorwayId);
				if (!testExitNodes || Roundabout.CanConnectionAddExitNode(roundaboutInputDirection, roundaboutOutputDirection, motorwayNode))
				{
					signature.AddNode(motorwayNode);
				}
			}
			return signature;
		}

		// Token: 0x06001AB6 RID: 6838 RVA: 0x00061600 File Offset: 0x0005F800
		public bool CanSetNodeState(RoadTileNode node, RoadState newState, Tile.TileChangePermissions changePermissions = Tile.TileChangePermissions.Full)
		{
			if (node.type == RoadType.TwoLane || node.type == RoadType.Driveway)
			{
				if (this.HasMotorwayInDirection(node.direction, RoadState.Planned | RoadState.Active | RoadState.Mothballed))
				{
					return false;
				}
				int nodeIndex = (int)node.direction;
				RoadState currentState = this._twoLaneRoadState[nodeIndex];
				if ((newState & RoadState.VisiblyActive) != RoadState.None && this.HasRoundabout(RoadState.Planned | RoadState.Active) && !Roundabout.CanConnectionAddExitNode(this.GetRoundaboutConnection(RoadState.Planned | RoadState.Active), node))
				{
					return false;
				}
				switch (newState)
				{
				case RoadState.None:
					return currentState == RoadState.Mothballed;
				case (RoadState)1:
				case (RoadState)3:
					break;
				case RoadState.Planned:
					return node.type != RoadType.Driveway && this.HasPermissionToChangeNodeState(node.direction, changePermissions) && (currentState & (RoadState.Pending | RoadState.Active | RoadState.Mothballed)) == RoadState.None;
				case RoadState.Pending:
					return this.HasPermissionToChangeNodeState(node.direction, changePermissions) && (currentState & RoadState.ActiveOrPending) == RoadState.None;
				default:
					if (newState == RoadState.Active)
					{
						return currentState == RoadState.Pending && (!this.HasRoundabout(RoadState.Mothballed) || Roundabout.CanConnectionAddExitNode(this.GetRoundaboutConnection(RoadState.Mothballed), node));
					}
					if (newState == RoadState.Mothballed)
					{
						return this.HasPermissionToChangeNodeState(node.direction, changePermissions) && ((currentState & RoadState.ActiveOrPending) != RoadState.None || (currentState == RoadState.None && TileUtilities.IsDirectionDiagonal(node.direction) && this.IsCenterOfRoundabout));
					}
					break;
				}
			}
			else if (node.type == RoadType.Motorway)
			{
				int directionIndex = (int)node.direction;
				if (newState <= RoadState.Planned)
				{
					if (newState == RoadState.None)
					{
						return this._plannedMotorways[directionIndex] == node.motorwayId || this._mothballedMotorways[directionIndex] == node.motorwayId;
					}
					if (newState == RoadState.Planned)
					{
						return this.HasPermissionToChangeNodeState(node.direction, changePermissions) && !Roundabout.IsTileCenterOfRoundabout(this, RoadState.Planned | RoadState.Pending | RoadState.Active | RoadState.Mothballed) && ((this._activeMotorways[directionIndex] == -1 && this._plannedMotorways[directionIndex] == node.motorwayId) || (this._plannedMotorways[directionIndex] == -1 && this._activeMotorways[directionIndex] == -1 && this.GetTwoLaneRoadStateInDirection(node.direction) == RoadState.None));
					}
				}
				else
				{
					if (newState == RoadState.Active)
					{
						return this.HasPermissionToChangeNodeState(node.direction, changePermissions) && ((this._plannedMotorways[directionIndex] == node.motorwayId && this._mothballedMotorways[directionIndex] == -1) || (this._plannedMotorways[directionIndex] == -1 && this._mothballedMotorways[directionIndex] == node.motorwayId));
					}
					if (newState == RoadState.Mothballed)
					{
						return this._plannedMotorways[directionIndex] == node.motorwayId || this._activeMotorways[directionIndex] == node.motorwayId;
					}
				}
				return false;
			}
			Diagnostics.FailAssert("CanSetNodeState is unable to handle nodes of type {0}.", new object[]
			{
				node.type
			});
			return false;
		}

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x06001AB7 RID: 6839 RVA: 0x00061870 File Offset: 0x0005FA70
		public bool IsDrivewayOnly
		{
			get
			{
				if (this.GetTwoLaneRoadCount(RoadState.Active, Tile.MotorwayInclusion.Ignore) != 1 || this.ContentType == TileContentType.Tree)
				{
					return false;
				}
				TileDirection otherDirection = TileDirection.None;
				Tile otherTile = this.GetAdjacentConnectedTile(out otherDirection, RoadState.ActiveOrPending, this.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Include));
				return otherTile.ContentType != TileContentType.None && TileUtilities.GetOppositeDirection(otherDirection) == otherTile.DrivewayDirection;
			}
		}

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x06001AB8 RID: 6840 RVA: 0x000618C0 File Offset: 0x0005FAC0
		public bool IsDriveway
		{
			get
			{
				if (this.ContentType == TileContentType.Tree)
				{
					return false;
				}
				TileDirection otherDirection = TileDirection.None;
				Tile otherTile = this.GetAdjacentConnectedTile(out otherDirection, RoadState.ActiveOrPending, this.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Include));
				return otherTile.ContentType != TileContentType.None && TileUtilities.GetOppositeDirection(otherDirection) == otherTile.DrivewayDirection;
			}
		}

		// Token: 0x06001AB9 RID: 6841 RVA: 0x00061908 File Offset: 0x0005FB08
		public bool SetNodeState(RoadTileNode node, RoadState newState, Tile.TileChangePermissions permissions = Tile.TileChangePermissions.Full)
		{
			if (!this.CanSetNodeState(node, newState, permissions))
			{
				return false;
			}
			if (node.type == RoadType.TwoLane)
			{
				int nodeIndex = (int)node.direction;
				RoadState oldState = this._twoLaneRoadState[nodeIndex];
				if (newState != oldState)
				{
					if (newState == RoadState.Mothballed && oldState == RoadState.Pending)
					{
						newState = RoadState.None;
					}
					this._twoLaneRoadState[nodeIndex] = newState;
					if (newState == RoadState.Pending && oldState != RoadState.Mothballed)
					{
						this._nodePermanenceProgress[nodeIndex] = Fix64.Zero;
					}
					this.NotifyTileChanged();
					return true;
				}
			}
			if (node.type == RoadType.Driveway)
			{
				int nodeIndex2 = (int)node.direction;
				RoadState oldState2 = this._twoLaneRoadState[nodeIndex2];
				if (newState != oldState2)
				{
					if ((newState & RoadState.ActiveOrPending) != RoadState.None)
					{
						TileDirection oldDrivewayDirection = this.DrivewayDirection;
						if (oldDrivewayDirection != TileDirection.None)
						{
							int oldDrivewayIndex = (int)oldDrivewayDirection;
							if (this._twoLaneRoadState[oldDrivewayIndex] == RoadState.Pending)
							{
								this._twoLaneRoadState[oldDrivewayIndex] = RoadState.None;
							}
							else
							{
								this._twoLaneRoadState[oldDrivewayIndex] = RoadState.Mothballed;
							}
						}
					}
					else if (newState == RoadState.Mothballed && oldState2 == RoadState.Pending)
					{
						newState = RoadState.None;
					}
					this._twoLaneRoadState[nodeIndex2] = newState;
					this.NotifyTileChanged();
					return true;
				}
			}
			if (node.type == RoadType.Motorway)
			{
				int nodeIndex3 = (int)node.direction;
				if (newState <= RoadState.Planned)
				{
					if (newState != RoadState.None)
					{
						if (newState == RoadState.Planned)
						{
							this._plannedMotorways[nodeIndex3] = node.motorwayId;
							goto IL_1DE;
						}
					}
					else
					{
						if (this._plannedMotorways[nodeIndex3] == node.motorwayId)
						{
							this._plannedMotorways[nodeIndex3] = -1;
						}
						if (this._mothballedMotorways[nodeIndex3] == node.motorwayId)
						{
							this._mothballedMotorways[nodeIndex3] = -1;
							goto IL_1DE;
						}
						goto IL_1DE;
					}
				}
				else
				{
					if (newState == RoadState.Active)
					{
						if (this._plannedMotorways[nodeIndex3] == node.motorwayId)
						{
							this._plannedMotorways[nodeIndex3] = -1;
						}
						else
						{
							this._mothballedMotorways[nodeIndex3] = -1;
						}
						this._activeMotorways[nodeIndex3] = node.motorwayId;
						goto IL_1DE;
					}
					if (newState == RoadState.Mothballed)
					{
						if (this._activeMotorways[nodeIndex3] == node.motorwayId)
						{
							this._activeMotorways[nodeIndex3] = -1;
							this._mothballedMotorways[nodeIndex3] = node.motorwayId;
						}
						if (this._plannedMotorways[nodeIndex3] == node.motorwayId)
						{
							this._plannedMotorways[nodeIndex3] = -1;
							goto IL_1DE;
						}
						goto IL_1DE;
					}
				}
				return false;
				IL_1DE:
				this.NotifyTileChanged();
				return true;
			}
			return false;
		}

		// Token: 0x06001ABA RID: 6842 RVA: 0x00061AFC File Offset: 0x0005FCFC
		public bool HasMotorwayInDirection(TileDirection direction, RoadState roadStates)
		{
			return ((roadStates & RoadState.Mothballed) != RoadState.None && this._mothballedMotorways[(int)direction] > -1) || ((roadStates & RoadState.Planned) != RoadState.None && this._plannedMotorways[(int)direction] > -1) || ((roadStates & RoadState.Active) != RoadState.None && this._activeMotorways[(int)direction] > -1);
		}

		// Token: 0x06001ABB RID: 6843 RVA: 0x00061B44 File Offset: 0x0005FD44
		public int GetMotorwayInDirection(TileDirection direction, RoadState roadStates)
		{
			if ((roadStates & RoadState.Active) != RoadState.None && this._activeMotorways[(int)direction] != -1)
			{
				return this._activeMotorways[(int)direction];
			}
			if ((roadStates & RoadState.Planned) != RoadState.None && this._plannedMotorways[(int)direction] != -1)
			{
				return this._plannedMotorways[(int)direction];
			}
			if ((roadStates & RoadState.Mothballed) != RoadState.None && this._mothballedMotorways[(int)direction] != -1)
			{
				return this._mothballedMotorways[(int)direction];
			}
			return -1;
		}

		// Token: 0x06001ABC RID: 6844 RVA: 0x00061BA0 File Offset: 0x0005FDA0
		public TileDirection GetMotorwayRampDirection(int motorwayId)
		{
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				if (this._plannedMotorways[directionIndex] == motorwayId || this._activeMotorways[directionIndex] == motorwayId || this._mothballedMotorways[directionIndex] == motorwayId)
				{
					return (TileDirection)directionIndex;
				}
			}
			return TileDirection.None;
		}

		// Token: 0x06001ABD RID: 6845 RVA: 0x00061BE0 File Offset: 0x0005FDE0
		public TileDirectionBitfield GetMotorwayRamps(RoadState states)
		{
			TileDirectionBitfield motorwayRamps = default(TileDirectionBitfield);
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				motorwayRamps[(TileDirection)directionIndex] = this.HasMotorwayInDirection((TileDirection)directionIndex, states);
			}
			return motorwayRamps;
		}

		// Token: 0x06001ABE RID: 6846 RVA: 0x00061C12 File Offset: 0x0005FE12
		public bool HasRoundabout(RoadState states)
		{
			return ((states & RoadState.Active) == RoadState.Active && this._activeRoundaboutInput != TileDirection.None) || ((states & RoadState.Planned) == RoadState.Planned && this._plannedRoundaboutInput != TileDirection.None) || ((states & RoadState.Mothballed) == RoadState.Mothballed && this._mothballedRoundaboutInput != TileDirection.None);
		}

		// Token: 0x06001ABF RID: 6847 RVA: 0x00061C4C File Offset: 0x0005FE4C
		public RoadTileConnection GetRoundaboutConnection(RoadState states)
		{
			TileDirection roundaboutInput = TileDirection.None;
			TileDirection roundaboutOutput = TileDirection.None;
			if ((states & RoadState.Active) == RoadState.Active && this._activeRoundaboutInput != TileDirection.None)
			{
				roundaboutInput = this._activeRoundaboutInput;
				roundaboutOutput = this._activeRoundaboutOutput;
			}
			if ((states & RoadState.Planned) == RoadState.Planned && this._plannedRoundaboutInput != TileDirection.None)
			{
				roundaboutInput = this._plannedRoundaboutInput;
				roundaboutOutput = this._plannedRoundaboutOutput;
			}
			if ((states & RoadState.Mothballed) == RoadState.Mothballed && this._mothballedRoundaboutInput != TileDirection.None)
			{
				roundaboutInput = this._mothballedRoundaboutInput;
				roundaboutOutput = this._mothballedRoundaboutOutput;
			}
			if (roundaboutInput != TileDirection.None)
			{
				return new RoadTileConnection(new RoadTileNode(roundaboutInput, RoadType.Roundabout, -1), new RoadTileNode(roundaboutOutput, RoadType.Roundabout, -1));
			}
			return RoadTileConnection.InvalidConnection;
		}

		// Token: 0x06001AC0 RID: 6848 RVA: 0x00061CD8 File Offset: 0x0005FED8
		public RoadState GetRoundaboutState(RoadTileConnection roundaboutConnection)
		{
			TileDirection roundaboutInput = roundaboutConnection.input.direction;
			TileDirection roundaboutOutput = roundaboutConnection.output.direction;
			if (this._activeRoundaboutInput == roundaboutInput && this._activeRoundaboutOutput == roundaboutOutput)
			{
				return RoadState.Active;
			}
			if (this._plannedRoundaboutInput == roundaboutInput && this._plannedRoundaboutOutput == roundaboutOutput)
			{
				return RoadState.Planned;
			}
			if (this._mothballedRoundaboutInput == roundaboutInput && this._mothballedRoundaboutOutput == roundaboutOutput)
			{
				return RoadState.Mothballed;
			}
			return RoadState.None;
		}

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06001AC1 RID: 6849 RVA: 0x00061D3C File Offset: 0x0005FF3C
		public bool IsPlannedRoundaboutBlocked
		{
			get
			{
				if (this._mothballedRoundaboutInput != TileDirection.None)
				{
					return true;
				}
				if (this._plannedRoundaboutInput == TileDirection.None || this._plannedRoundaboutOutput == TileDirection.None)
				{
					if (Roundabout.IsTileCenterOfRoundabout(this, RoadState.Planned))
					{
						foreach (TileDirection diagonalDirection in TileUtilities.NonDiagonalDirections)
						{
							if (this.GetTwoLaneRoadStateInDirection(diagonalDirection) != RoadState.None)
							{
								return true;
							}
						}
					}
					return false;
				}
				TileDirectionBitfield invalidExitDirections = Roundabout.GetInvalidExitsForConnection(this._plannedRoundaboutInput, this._plannedRoundaboutOutput);
				TileDirectionBitfield mothballedNodeDirections = this.GetTwoLaneRoads(RoadState.Mothballed, Tile.MotorwayInclusion.Include);
				return (invalidExitDirections.Bits & mothballedNodeDirections.Bits) != 0;
			}
		}

		// Token: 0x06001AC2 RID: 6850 RVA: 0x00061DC4 File Offset: 0x0005FFC4
		public bool IsNodeBlocked(RoadTileNode node)
		{
			return this._mothballedRoundaboutInput != TileDirection.None && !Roundabout.CanConnectionAddExitNode(this.GetRoundaboutConnection(RoadState.Mothballed), node);
		}

		// Token: 0x06001AC3 RID: 6851 RVA: 0x00061DE2 File Offset: 0x0005FFE2
		public bool CanSetRoundaboutState(RoadTileConnection roundaboutConnection, RoadState roundaboutState)
		{
			return this.CanSetRoundaboutState(roundaboutConnection.input.direction, roundaboutConnection.output.direction, roundaboutState);
		}

		// Token: 0x06001AC4 RID: 6852 RVA: 0x00061E04 File Offset: 0x00060004
		public bool CanSetRoundaboutState(TileDirection roundaboutInput, TileDirection roundaboutOutput, RoadState roundaboutState)
		{
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				if (this.HasMotorwayInDirection((TileDirection)directionIndex, RoadState.Planned | RoadState.Active | RoadState.Mothballed))
				{
					return false;
				}
			}
			if (roundaboutState <= RoadState.Planned)
			{
				if (roundaboutState == RoadState.None)
				{
					return (this._mothballedRoundaboutInput == roundaboutInput && this._mothballedRoundaboutOutput == roundaboutOutput) || (this._plannedRoundaboutInput == roundaboutInput && this._plannedRoundaboutOutput == roundaboutOutput);
				}
				if (roundaboutState == RoadState.Planned)
				{
					return !this.HasRoundabout(RoadState.Planned | RoadState.Active);
				}
			}
			else
			{
				if (roundaboutState == RoadState.Active)
				{
					return (this._mothballedRoundaboutInput == roundaboutInput && this._mothballedRoundaboutOutput == roundaboutOutput) || (this._plannedRoundaboutInput == roundaboutInput && this._plannedRoundaboutOutput == roundaboutOutput && !this.IsPlannedRoundaboutBlocked);
				}
				if (roundaboutState == RoadState.Mothballed)
				{
					return (this._activeRoundaboutInput == roundaboutInput && this._activeRoundaboutOutput == roundaboutOutput) || (this._plannedRoundaboutInput == roundaboutInput && this._plannedRoundaboutOutput == roundaboutOutput);
				}
			}
			Diagnostics.FailAssert("Cannot set roundabout state to {0}.", new object[]
			{
				roundaboutState
			});
			return false;
		}

		// Token: 0x06001AC5 RID: 6853 RVA: 0x00061EF2 File Offset: 0x000600F2
		public bool SetRoundaboutState(RoadTileConnection roundaboutConnection, RoadState roundaboutState)
		{
			return this.SetRoundaboutState(roundaboutConnection.input.direction, roundaboutConnection.output.direction, roundaboutState);
		}

		// Token: 0x06001AC6 RID: 6854 RVA: 0x00061F14 File Offset: 0x00060114
		public bool SetRoundaboutState(TileDirection roundaboutInput, TileDirection roundaboutOutput, RoadState roundaboutState)
		{
			if (!this.CanSetRoundaboutState(roundaboutInput, roundaboutOutput, roundaboutState))
			{
				return false;
			}
			Tile.Log.Info("Setting roundabout state of tile {0} for input {1} output {2} to {3}", new object[]
			{
				this,
				roundaboutInput,
				roundaboutOutput,
				roundaboutState
			});
			if (roundaboutState <= RoadState.Planned)
			{
				if (roundaboutState != RoadState.None)
				{
					if (roundaboutState == RoadState.Planned)
					{
						this._plannedRoundaboutInput = roundaboutInput;
						this._plannedRoundaboutOutput = roundaboutOutput;
						if (this._mothballedRoundaboutInput == roundaboutInput && this._mothballedRoundaboutOutput == roundaboutOutput)
						{
							this._mothballedRoundaboutInput = TileDirection.None;
							this._mothballedRoundaboutOutput = TileDirection.None;
						}
						foreach (TileDirection invalidExitDirection in Roundabout.GetInvalidExitsForConnection(roundaboutInput, roundaboutOutput))
						{
							if ((this._twoLaneRoadState[(int)invalidExitDirection] & RoadState.ActiveOrPending) != RoadState.None)
							{
								this.SetNodeState(new RoadTileNode(invalidExitDirection, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
							}
						}
						goto IL_1FE;
					}
				}
				else
				{
					if (this._mothballedRoundaboutInput == roundaboutInput && this._mothballedRoundaboutOutput == roundaboutOutput)
					{
						this._mothballedRoundaboutInput = TileDirection.None;
						this._mothballedRoundaboutOutput = TileDirection.None;
					}
					if (this._plannedRoundaboutInput == roundaboutInput && this._plannedRoundaboutOutput == roundaboutOutput)
					{
						this._plannedRoundaboutInput = TileDirection.None;
						this._plannedRoundaboutOutput = TileDirection.None;
						goto IL_1FE;
					}
					goto IL_1FE;
				}
			}
			else if (roundaboutState != RoadState.Active)
			{
				if (roundaboutState == RoadState.Mothballed)
				{
					if (this._activeRoundaboutInput == roundaboutInput && this._activeRoundaboutOutput == roundaboutOutput)
					{
						this._activeRoundaboutInput = TileDirection.None;
						this._activeRoundaboutOutput = TileDirection.None;
						this._mothballedRoundaboutInput = roundaboutInput;
						this._mothballedRoundaboutOutput = roundaboutOutput;
					}
					if (this._plannedRoundaboutInput == roundaboutInput && this._plannedRoundaboutOutput == roundaboutOutput)
					{
						this._plannedRoundaboutInput = TileDirection.None;
						this._plannedRoundaboutOutput = TileDirection.None;
						goto IL_1FE;
					}
					goto IL_1FE;
				}
			}
			else
			{
				this._plannedRoundaboutInput = TileDirection.None;
				this._plannedRoundaboutOutput = TileDirection.None;
				this._activeRoundaboutInput = roundaboutInput;
				this._activeRoundaboutOutput = roundaboutOutput;
				if (this._mothballedRoundaboutInput == roundaboutInput && this._mothballedRoundaboutOutput == roundaboutOutput)
				{
					this._mothballedRoundaboutInput = TileDirection.None;
					this._mothballedRoundaboutOutput = TileDirection.None;
					foreach (TileDirection invalidExitDirection2 in Roundabout.GetInvalidExitsForConnection(roundaboutInput, roundaboutOutput))
					{
						if (this._twoLaneRoadState[(int)invalidExitDirection2] == RoadState.Pending)
						{
							this.SetNodeState(new RoadTileNode(invalidExitDirection2, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
						}
					}
					goto IL_1FE;
				}
				goto IL_1FE;
			}
			return false;
			IL_1FE:
			this.NotifyTileChanged();
			return true;
		}

		// Token: 0x06001AC7 RID: 6855 RVA: 0x00062126 File Offset: 0x00060326
		public void SetNodeImmutability(TileDirection direction, bool isImmutable)
		{
			if (this._isDirectionImmutable[direction] != isImmutable)
			{
				this._isDirectionImmutable[direction] = isImmutable;
				this.NotifyTileChanged();
			}
		}

		// Token: 0x06001AC8 RID: 6856 RVA: 0x0006214A File Offset: 0x0006034A
		public bool IsNodePermanent(TileDirection direction)
		{
			return this._nodePermanenceProgress[(int)direction] >= Fix64.One || this._isDirectionImmutable[direction];
		}

		// Token: 0x06001AC9 RID: 6857 RVA: 0x00062174 File Offset: 0x00060374
		public TileDirectionBitfield GetPermanentDirections()
		{
			TileDirectionBitfield permanentDirections = TileDirectionBitfield.None;
			foreach (TileDirection direction in TileUtilities.Directions)
			{
				permanentDirections[direction] = (this._nodePermanenceProgress[(int)direction] >= Fix64.One);
			}
			return permanentDirections;
		}

		// Token: 0x06001ACA RID: 6858 RVA: 0x000621BE File Offset: 0x000603BE
		public Fix64 GetNodePermanenceProgress(TileDirection direction)
		{
			if (!this.IsNodePermanent(direction))
			{
				return this._nodePermanenceProgress[(int)direction];
			}
			return Fix64.One;
		}

		// Token: 0x06001ACB RID: 6859 RVA: 0x000621DC File Offset: 0x000603DC
		public bool AnyRoadHasPermanenceBelowValue(Fix64 permanenceToTest, RoadState roadState)
		{
			foreach (TileDirection direction in this.GetTwoLaneRoads(roadState, Tile.MotorwayInclusion.Ignore))
			{
				if (this._nodePermanenceProgress[(int)direction] < permanenceToTest && !this.IsConnectedViaDrivewayInDirection(direction))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001ACC RID: 6860 RVA: 0x0006222D File Offset: 0x0006042D
		private bool HasPermissionToChangeNodeState(TileDirection direction, Tile.TileChangePermissions changePermissions = Tile.TileChangePermissions.Full)
		{
			return !this._isDirectionImmutable[direction] && (changePermissions == Tile.TileChangePermissions.Full || this._nodePermanenceProgress[(int)direction] < Fix64.One);
		}

		// Token: 0x06001ACD RID: 6861 RVA: 0x0006225A File Offset: 0x0006045A
		public void SetNodePermanence(TileDirection direction, bool isPermanent)
		{
			this.SetNodePermanence(direction, isPermanent ? Fix64.One : Fix64.Zero);
		}

		// Token: 0x06001ACE RID: 6862 RVA: 0x00062272 File Offset: 0x00060472
		public void SetNodePermanence(TileDirection direction, Fix64 permanence)
		{
			this._nodePermanenceProgress[(int)direction] = permanence;
			this.NotifyTileChanged();
		}

		// Token: 0x06001ACF RID: 6863 RVA: 0x00062288 File Offset: 0x00060488
		public void IncrementNodePermanenceProgress(Fix64 permanenceProgress, TileDirectionBitfield directions, RoadState state = RoadState.Active)
		{
			bool tileChanged = false;
			TileDirectionBitfield directionsToIncrement = new TileDirectionBitfield(directions.Bits & this.GetTwoLaneRoads(state, Tile.MotorwayInclusion.Include).Bits);
			foreach (TileDirection direction in directionsToIncrement)
			{
				this._nodePermanenceProgress[(int)direction] += permanenceProgress;
				if (this._nodePermanenceProgress[(int)direction] >= Fix64.One)
				{
					this._nodePermanenceProgress[(int)direction] = Fix64.One;
				}
				tileChanged = true;
			}
			if (this.HasTrafficLight && !this.IsTrafficLightPermanent)
			{
				this._trafficLightPermanenceProgress += permanenceProgress;
				if (this._trafficLightPermanenceProgress > Fix64.One)
				{
					this._trafficLightPermanenceProgress = Fix64.One;
				}
				tileChanged = true;
			}
			if (this.IsCenterOfRoundabout && !this.IsRoundaboutPermanent)
			{
				Vector2Int referenceTileOffset = new Vector2Int(-1, 0);
				RoadTileConnection referenceRoundaboutConnection = Roundabout.GetConnectionForCoordinatesOffset(referenceTileOffset);
				Tile referenceTile = this.Tilemap.GetTile(this.Coordinates + referenceTileOffset);
				if (referenceTile != null && referenceTile.GetRoundaboutState(referenceRoundaboutConnection) == RoadState.Active)
				{
					this._roundaboutPermanenceProgress += permanenceProgress;
					if (this._roundaboutPermanenceProgress > Fix64.One)
					{
						this._roundaboutPermanenceProgress = Fix64.One;
					}
					tileChanged = true;
				}
			}
			if (tileChanged)
			{
				this.NotifyTileChanged();
			}
		}

		// Token: 0x06001AD0 RID: 6864 RVA: 0x000623E4 File Offset: 0x000605E4
		public void ResetRoundaboutPermanence()
		{
			if (this._roundaboutPermanenceProgress > Fix64.Zero)
			{
				this._roundaboutPermanenceProgress = Fix64.Zero;
				this.NotifyTileChanged();
			}
		}

		// Token: 0x06001AD1 RID: 6865 RVA: 0x0006240C File Offset: 0x0006060C
		public bool CanSetContentType(TileContentType type)
		{
			if (type == TileContentType.None)
			{
				return true;
			}
			if (this.ContentType == TileContentType.None)
			{
				return this.IsEmpty();
			}
			return (type == TileContentType.Destination || type == TileContentType.Carpark || type == TileContentType.House || type == TileContentType.BoatTerminal) && this.ContentType == TileContentType.Tree && this._city.Rules.ShouldBuildingsBulldozeTrees;
		}

		// Token: 0x06001AD2 RID: 6866 RVA: 0x00062458 File Offset: 0x00060658
		public void SetContentType(TileContentType type, IModel contentModel)
		{
			if (this.ContentType == TileContentType.Tree && (type == TileContentType.Destination || type == TileContentType.Carpark || type == TileContentType.House || type == TileContentType.BoatTerminal))
			{
				TreeModel treeModel = this.ContentModel as TreeModel;
				if (Diagnostics.Verify(treeModel != null, "ContentType at {0} is Tree, but no TreeModel found!"))
				{
					Diagnostics.Verify(this._city.Rules.ShouldBuildingsBulldozeTrees, "Bulldozing tree at {0}, but game rules says we shouldn't be", this.Coordinates);
					treeModel.Bulldoze();
				}
			}
			this.ContentType = type;
			this.ContentModel = contentModel;
			this.NotifyTileChanged();
		}

		// Token: 0x06001AD3 RID: 6867 RVA: 0x000624DA File Offset: 0x000606DA
		public void Subscribe(Tile.IObserver observer)
		{
			this._observers.Subscribe(observer);
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x000624E8 File Offset: 0x000606E8
		public bool Unsubscribe(Tile.IObserver observer)
		{
			return this._observers.Unsubscribe(observer);
		}

		// Token: 0x06001AD5 RID: 6869 RVA: 0x000624F8 File Offset: 0x000606F8
		private void NotifyTileChanged()
		{
			foreach (Tile.IObserver observer in this._observers)
			{
				observer.OnTileChanged(this);
			}
		}

		// Token: 0x06001AD6 RID: 6870 RVA: 0x0006252C File Offset: 0x0006072C
		public void Clear()
		{
			this.ContentType = TileContentType.None;
			this.ContentModel = null;
			this._hasTrafficLight = false;
			this._trafficLightPermanenceProgress = Fix64.Zero;
			this._roundaboutPermanenceProgress = Fix64.Zero;
			this._isCenterOfRoundabout = false;
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				this._twoLaneRoadState[directionIndex] = RoadState.None;
				this._plannedMotorways[directionIndex] = -1;
				this._activeMotorways[directionIndex] = -1;
				this._mothballedMotorways[directionIndex] = -1;
				this._nodePermanenceProgress[directionIndex] = Fix64.Zero;
			}
			this._isDirectionImmutable = TileDirectionBitfield.None;
			this._plannedRoundaboutInput = TileDirection.None;
			this._plannedRoundaboutOutput = TileDirection.None;
			this._activeRoundaboutInput = TileDirection.None;
			this._activeRoundaboutOutput = TileDirection.None;
			this._mothballedRoundaboutInput = TileDirection.None;
			this._mothballedRoundaboutOutput = TileDirection.None;
			this.UnbuiltMotorwayId = -1;
			this.UnbuiltMotorwayNumber = 0;
			this._railConnection = RailTileConnection.InvalidConnection;
			this.NotifyTileChanged();
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x00062600 File Offset: 0x00060800
		public bool CanDrawRoadsOn()
		{
			return this._behaviour.CanDrawRoadOn(this.ContentType);
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x00062614 File Offset: 0x00060814
		public bool IsEmpty()
		{
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				if (this._twoLaneRoadState[directionIndex] != RoadState.None)
				{
					return false;
				}
				if (this._plannedMotorways[directionIndex] != -1 || this._activeMotorways[directionIndex] != -1 || this._mothballedMotorways[directionIndex] != -1)
				{
					return false;
				}
			}
			return !Roundabout.IsTileCenterOfRoundabout(this, RoadState.VisiblyActive) && this.UnbuiltMotorwayId == -1 && this._plannedRoundaboutInput == TileDirection.None && this._activeRoundaboutInput == TileDirection.None && this._mothballedRoundaboutInput == TileDirection.None;
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x00062694 File Offset: 0x00060894
		public bool IsConnectedViaDrivewayInDirection(TileDirection direction)
		{
			Tile adjacentTile = this.Tilemap.GetTile(TileUtilities.GetAdjacentCoordinates(this.Coordinates, direction));
			return adjacentTile != null && (adjacentTile.ContentType == TileContentType.House || adjacentTile.ContentType == TileContentType.Carpark);
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x000626D4 File Offset: 0x000608D4
		public Tile GetAdjacentConnectedTile(out TileDirection connectedTileDirection, RoadState traversableConnectionStates, TileDirectionBitfield traversableDirections)
		{
			connectedTileDirection = TileDirection.None;
			foreach (TileDirection direction in this.GetTwoLaneRoads(traversableConnectionStates, Tile.MotorwayInclusion.Ignore))
			{
				if (traversableDirections[direction])
				{
					connectedTileDirection = direction;
					return this.Tilemap.GetTile(TileUtilities.GetAdjacentCoordinates(this.Coordinates, direction));
				}
			}
			return null;
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x00062730 File Offset: 0x00060930
		public override string ToString()
		{
			string description = string.Format("[Tile Coordinates={0}", this.Coordinates);
			if (this.ContentType != TileContentType.None)
			{
				description += string.Format(", ContentType={0}, ContentModel={1}", this.ContentType, this.ContentModel);
			}
			string activeMotorwayDescription = "";
			string plannedMotorwayDescription = "";
			string mothballedMotorwayDescription = "";
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				if (this._plannedMotorways[directionIndex] != -1)
				{
					plannedMotorwayDescription += string.Format("{0} {1}, ", (TileDirection)directionIndex, this._plannedMotorways[directionIndex]);
				}
				if (this._activeMotorways[directionIndex] != -1)
				{
					activeMotorwayDescription += string.Format("{0} {1}, ", (TileDirection)directionIndex, this._activeMotorways[directionIndex]);
				}
				if (this._mothballedMotorways[directionIndex] != -1)
				{
					mothballedMotorwayDescription += string.Format("{0} {1}, ", (TileDirection)directionIndex, this._mothballedMotorways[directionIndex]);
				}
			}
			if (plannedMotorwayDescription.Length > 0)
			{
				description = description + ", PlannedMotorways=[" + plannedMotorwayDescription.Substring(0, plannedMotorwayDescription.Length - 2) + "]";
			}
			if (activeMotorwayDescription.Length > 0)
			{
				description = description + ", ActiveMotorways=[" + activeMotorwayDescription.Substring(0, activeMotorwayDescription.Length - 2) + "]";
			}
			if (mothballedMotorwayDescription.Length > 0)
			{
				description = description + ", MothballedMotorways=[" + mothballedMotorwayDescription.Substring(0, mothballedMotorwayDescription.Length - 2) + "]";
			}
			return description + "]";
		}

		// Token: 0x06001ADC RID: 6876 RVA: 0x000628C4 File Offset: 0x00060AC4
		public void Reset()
		{
			this.Tilemap = null;
			this.Coordinates = default(Vector2Int);
			this.ContentType = TileContentType.None;
			this.ContentModel = null;
			this._hasTrafficLight = false;
			this._trafficLightPermanenceProgress = Fix64.Zero;
			this._roundaboutPermanenceProgress = Fix64.Zero;
			this._isCenterOfRoundabout = false;
			this._unbuiltMotorwayId = -1;
			this._unbuiltMotorwayNumber = 0;
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				this._twoLaneRoadState[directionIndex] = RoadState.None;
				this._plannedMotorways[directionIndex] = -1;
				this._activeMotorways[directionIndex] = -1;
				this._mothballedMotorways[directionIndex] = -1;
			}
			this._isDirectionImmutable = TileDirectionBitfield.None;
			this._activeRoundaboutInput = TileDirection.None;
			this._activeRoundaboutOutput = TileDirection.None;
			this._mothballedRoundaboutInput = TileDirection.None;
			this._mothballedRoundaboutOutput = TileDirection.None;
			this._plannedRoundaboutInput = TileDirection.None;
			this._plannedRoundaboutOutput = TileDirection.None;
			this._railConnection = RailTileConnection.InvalidConnection;
			this._boatPathConnection = BoatPathTileConnection.InvalidConnection;
			this._observers.UnsubscribeAll();
		}

		// Token: 0x0400162E RID: 5678
		public static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Tile");

		// Token: 0x0400162F RID: 5679
		[Dependency]
		private IScope _scope;

		// Token: 0x04001630 RID: 5680
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x04001631 RID: 5681
		[Dependency]
		protected City _city;

		// Token: 0x04001632 RID: 5682
		[Dependency]
		protected TilemapModel _tilemap;

		// Token: 0x04001637 RID: 5687
		private bool _hasTrafficLight;

		// Token: 0x04001638 RID: 5688
		private Fix64 _trafficLightPermanenceProgress = Fix64.Zero;

		// Token: 0x04001639 RID: 5689
		private RailTileConnection _railConnection = RailTileConnection.InvalidConnection;

		// Token: 0x0400163A RID: 5690
		private BoatPathTileConnection _boatPathConnection = BoatPathTileConnection.InvalidConnection;

		// Token: 0x0400163B RID: 5691
		private bool _isCenterOfRoundabout;

		// Token: 0x0400163C RID: 5692
		private Fix64 _roundaboutPermanenceProgress = Fix64.Zero;

		// Token: 0x0400163D RID: 5693
		private readonly RoadState[] _twoLaneRoadState = new RoadState[8];

		// Token: 0x0400163E RID: 5694
		private int _unbuiltMotorwayId = -1;

		// Token: 0x0400163F RID: 5695
		private int _unbuiltMotorwayNumber;

		// Token: 0x04001640 RID: 5696
		private TileDirection _plannedRoundaboutInput = TileDirection.None;

		// Token: 0x04001641 RID: 5697
		private TileDirection _plannedRoundaboutOutput = TileDirection.None;

		// Token: 0x04001642 RID: 5698
		private TileDirection _activeRoundaboutInput = TileDirection.None;

		// Token: 0x04001643 RID: 5699
		private TileDirection _activeRoundaboutOutput = TileDirection.None;

		// Token: 0x04001644 RID: 5700
		private TileDirection _mothballedRoundaboutInput = TileDirection.None;

		// Token: 0x04001645 RID: 5701
		private TileDirection _mothballedRoundaboutOutput = TileDirection.None;

		// Token: 0x04001646 RID: 5702
		private readonly int[] _plannedMotorways = new int[]
		{
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1
		};

		// Token: 0x04001647 RID: 5703
		private readonly int[] _activeMotorways = new int[]
		{
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1
		};

		// Token: 0x04001648 RID: 5704
		private readonly int[] _mothballedMotorways = new int[]
		{
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1
		};

		// Token: 0x04001649 RID: 5705
		private TileDirectionBitfield _isDirectionImmutable = TileDirectionBitfield.None;

		// Token: 0x0400164A RID: 5706
		private readonly Fix64[] _nodePermanenceProgress = new Fix64[8];

		// Token: 0x0400164B RID: 5707
		[Serialize(false, null)]
		private readonly ObserverList<Tile.IObserver> _observers = new ObserverList<Tile.IObserver>(1);

		// Token: 0x02000435 RID: 1077
		public enum MotorwayInclusion
		{
			// Token: 0x0400164D RID: 5709
			Ignore,
			// Token: 0x0400164E RID: 5710
			Include
		}

		// Token: 0x02000436 RID: 1078
		public struct PassageInfo
		{
			// Token: 0x17000534 RID: 1332
			// (get) Token: 0x06001ADF RID: 6879 RVA: 0x00062AA5 File Offset: 0x00060CA5
			public bool IsStart
			{
				get
				{
					return this._tile.Coordinates == this.passage.StartCoordinates;
				}
			}

			// Token: 0x17000535 RID: 1333
			// (get) Token: 0x06001AE0 RID: 6880 RVA: 0x00062AC2 File Offset: 0x00060CC2
			public bool IsEnd
			{
				get
				{
					return this._tile.Coordinates == this.passage.EndCoordinates;
				}
			}

			// Token: 0x06001AE1 RID: 6881 RVA: 0x00062ADF File Offset: 0x00060CDF
			public PassageInfo(Tile tile, Passage passage)
			{
				this._tile = tile;
				this.passage = passage;
			}

			// Token: 0x0400164F RID: 5711
			private Tile _tile;

			// Token: 0x04001650 RID: 5712
			public Passage passage;
		}

		// Token: 0x02000437 RID: 1079
		public enum TileChangePermissions
		{
			// Token: 0x04001652 RID: 5714
			Full,
			// Token: 0x04001653 RID: 5715
			RespectPermanence
		}

		// Token: 0x02000438 RID: 1080
		public interface IObserver
		{
			// Token: 0x06001AE2 RID: 6882
			void OnTileChanged(Tile changedTile);
		}
	}
}
