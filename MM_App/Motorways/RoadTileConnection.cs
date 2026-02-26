using System;
using System.Collections.Generic;
using Factory;

namespace Motorways
{
	// Token: 0x02000423 RID: 1059
	public readonly struct RoadTileConnection : IComparable
	{
		// Token: 0x06001A0C RID: 6668 RVA: 0x0005EC16 File Offset: 0x0005CE16
		public RoadTileConnection(TileDirection inputDirection, TileDirection outputDirection)
		{
			this.input = new RoadTileNode(inputDirection, RoadType.TwoLane, -1);
			this.output = new RoadTileNode(outputDirection, RoadType.TwoLane, -1);
		}

		// Token: 0x06001A0D RID: 6669 RVA: 0x0005EC34 File Offset: 0x0005CE34
		public RoadTileConnection(RoadTileNode inputNode, RoadTileNode outputNode)
		{
			this.input = inputNode;
			this.output = outputNode;
		}

		// Token: 0x06001A0E RID: 6670 RVA: 0x0005EC44 File Offset: 0x0005CE44
		public RoadTileConnection GetRotatedConnection(RoadTileRotation rotation)
		{
			return new RoadTileConnection(this.input.GetRotatedNode(rotation), this.output.GetRotatedNode(rotation));
		}

		// Token: 0x06001A0F RID: 6671 RVA: 0x0005EC63 File Offset: 0x0005CE63
		public RoadTileConnection GetReflectedConnection()
		{
			return new RoadTileConnection(this.output, this.input);
		}

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06001A10 RID: 6672 RVA: 0x0005EC76 File Offset: 0x0005CE76
		public bool IsUTurn
		{
			get
			{
				return this.input.direction == this.output.direction;
			}
		}

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x06001A11 RID: 6673 RVA: 0x0005EC90 File Offset: 0x0005CE90
		public bool IsRoundabout
		{
			get
			{
				return this.input.type == RoadType.Roundabout && this.output.type == RoadType.Roundabout;
			}
		}

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x06001A12 RID: 6674 RVA: 0x0005ECB0 File Offset: 0x0005CEB0
		public bool IsMotorway
		{
			get
			{
				return this.input.type == RoadType.Motorway && this.output.type == RoadType.Motorway;
			}
		}

		// Token: 0x06001A13 RID: 6675 RVA: 0x0005ECD0 File Offset: 0x0005CED0
		public RoadTileNode GetOtherNode(TileDirection direction)
		{
			if (this.input.direction == direction)
			{
				return this.output;
			}
			if (this.output.direction == direction)
			{
				return this.input;
			}
			return new RoadTileNode(TileDirection.None, RoadType.TwoLane, -1);
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x0005ED04 File Offset: 0x0005CF04
		public bool IntersectsOtherConnection(RoadTileConnection other, bool leftSideTraffic = false, bool smallIntersection = false, bool allowCrossingInFrontOfOther = false)
		{
			if (this.output.direction == other.output.direction && (!allowCrossingInFrontOfOther || this.input.direction == other.input.direction))
			{
				return true;
			}
			if (this.input.direction == other.input.direction)
			{
				return false;
			}
			if (this.CompareTo(other) == 0)
			{
				return true;
			}
			if (this.input.direction == other.output.direction && this.output.direction == other.input.direction)
			{
				return false;
			}
			if (smallIntersection && other.input.type != RoadType.Motorway && other.output.type != RoadType.Motorway)
			{
				RoadTileConnection intersectingPath = other.GetRotatedConnection(RoadTileRotation.HalfTurn);
				if (this.CompareTo(intersectingPath) == 0)
				{
					return true;
				}
			}
			if (allowCrossingInFrontOfOther)
			{
				return false;
			}
			int count = 0;
			int num = (int)((this.input.direction + (leftSideTraffic ? 1 : 0)) % (TileDirection)8);
			int endingIndex = (int)((this.output.direction + (leftSideTraffic ? 0 : 1)) % (TileDirection)8);
			for (int i = num; i != endingIndex; i = (i + 1) % 8)
			{
				if (other.input.direction == (TileDirection)i || other.output.direction == (TileDirection)i)
				{
					count++;
				}
			}
			return count == 1;
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x0005EE35 File Offset: 0x0005D035
		public static bool operator ==(RoadTileConnection lhs, RoadTileConnection rhs)
		{
			return lhs.Equals(rhs, TreatMotorwaysAs.Motorways);
		}

		// Token: 0x06001A16 RID: 6678 RVA: 0x0005EE40 File Offset: 0x0005D040
		public static bool operator !=(RoadTileConnection lhs, RoadTileConnection rhs)
		{
			return !lhs.Equals(rhs, TreatMotorwaysAs.Motorways);
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x0005EE50 File Offset: 0x0005D050
		public override bool Equals(object obj)
		{
			if (obj is RoadTileConnection)
			{
				RoadTileConnection roadTileConnection = (RoadTileConnection)obj;
				return this.Equals(roadTileConnection, TreatMotorwaysAs.Motorways);
			}
			return false;
		}

		// Token: 0x06001A18 RID: 6680 RVA: 0x0005EE76 File Offset: 0x0005D076
		public bool Equals(RoadTileConnection otherConnection)
		{
			return this.Equals(otherConnection, TreatMotorwaysAs.Motorways);
		}

		// Token: 0x06001A19 RID: 6681 RVA: 0x0005EE80 File Offset: 0x0005D080
		public bool Equals(RoadTileConnection otherConnection, TreatMotorwaysAs motorwayNodeTreatment)
		{
			return this.input.Equals(otherConnection.input, motorwayNodeTreatment) && this.output.Equals(otherConnection.output, motorwayNodeTreatment);
		}

		// Token: 0x06001A1A RID: 6682 RVA: 0x0005EEAC File Offset: 0x0005D0AC
		public int CompareTo(object obj)
		{
			if (obj is RoadTileConnection)
			{
				RoadTileConnection connection = (RoadTileConnection)obj;
				return this.CompareTo(connection);
			}
			return 1;
		}

		// Token: 0x06001A1B RID: 6683 RVA: 0x0005EED4 File Offset: 0x0005D0D4
		public int CompareTo(RoadTileConnection otherConnection)
		{
			int comparison = this.input.CompareTo(otherConnection.input);
			if (comparison != 0)
			{
				return comparison;
			}
			comparison = this.output.CompareTo(otherConnection.output);
			if (comparison != 0)
			{
				return comparison;
			}
			return 0;
		}

		// Token: 0x06001A1C RID: 6684 RVA: 0x0005EF10 File Offset: 0x0005D110
		public override int GetHashCode()
		{
			return this.GetHashCode(TreatMotorwaysAs.Motorways);
		}

		// Token: 0x06001A1D RID: 6685 RVA: 0x0005EF1C File Offset: 0x0005D11C
		public int GetHashCode(TreatMotorwaysAs motorwayNodeTreatment)
		{
			int hashCode = this.input.GetHashCode(motorwayNodeTreatment);
			int outputHash = this.output.GetHashCode(motorwayNodeTreatment);
			return hashCode << 16 | outputHash;
		}

		// Token: 0x06001A1E RID: 6686 RVA: 0x0005EF47 File Offset: 0x0005D147
		public override string ToString()
		{
			return string.Format("{0} to {1}", this.input, this.output);
		}

		// Token: 0x040015EC RID: 5612
		public readonly RoadTileNode input;

		// Token: 0x040015ED RID: 5613
		public readonly RoadTileNode output;

		// Token: 0x040015EE RID: 5614
		public static readonly RoadTileConnection InvalidConnection = new RoadTileConnection(new RoadTileNode(TileDirection.None, RoadType.TwoLane, -1), new RoadTileNode(TileDirection.None, RoadType.TwoLane, -1));

		// Token: 0x02000424 RID: 1060
		public class MotorwayAgnosticEqualityComparer : IEqualityComparer<RoadTileConnection>
		{
			// Token: 0x06001A20 RID: 6688 RVA: 0x0005EF85 File Offset: 0x0005D185
			public bool Equals(RoadTileConnection x, RoadTileConnection y)
			{
				return x.input.Equals(y.input, TreatMotorwaysAs.TwoLaneRoads) && x.output.Equals(y.output, TreatMotorwaysAs.TwoLaneRoads);
			}

			// Token: 0x06001A21 RID: 6689 RVA: 0x0005EFB1 File Offset: 0x0005D1B1
			public int GetHashCode(RoadTileConnection obj)
			{
				return obj.input.GetHashCode(TreatMotorwaysAs.TwoLaneRoads) * 397 ^ obj.output.GetHashCode(TreatMotorwaysAs.TwoLaneRoads);
			}
		}

		// Token: 0x02000425 RID: 1061
		public class Serializer : PrimitiveSerializer
		{
			// Token: 0x06001A23 RID: 6691 RVA: 0x0005EFD4 File Offset: 0x0005D1D4
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is RoadTileConnection)
				{
					RoadTileConnection connection = (RoadTileConnection)obj;
					bool hasMotorwayNodes = connection.input.motorwayId != -1 || connection.output.motorwayId != -1;
					context.Writer.Write(hasMotorwayNodes);
					context.Writer.Write((byte)connection.input.direction);
					context.Writer.Write((byte)connection.input.type);
					if (hasMotorwayNodes)
					{
						context.Writer.Write(connection.input.motorwayId);
					}
					context.Writer.Write((byte)connection.output.direction);
					context.Writer.Write((byte)connection.output.type);
					if (hasMotorwayNodes)
					{
						context.Writer.Write(connection.output.motorwayId);
					}
					return true;
				}
				return false;
			}

			// Token: 0x06001A24 RID: 6692 RVA: 0x0005F0B4 File Offset: 0x0005D2B4
			public override object Deserialize(object existingObj, ImportContext context)
			{
				bool hasMotorwayNodes = context.Reader.ReadBoolean();
				RoadTileNode inputNode = new RoadTileNode(TileUtilities.DeserializeDirection(context.Reader.ReadByte()), (RoadType)context.Reader.ReadByte(), hasMotorwayNodes ? context.Reader.ReadInt32() : -1);
				RoadTileNode output = new RoadTileNode(TileUtilities.DeserializeDirection(context.Reader.ReadByte()), (RoadType)context.Reader.ReadByte(), hasMotorwayNodes ? context.Reader.ReadInt32() : -1);
				return new RoadTileConnection(inputNode, output);
			}
		}
	}
}
