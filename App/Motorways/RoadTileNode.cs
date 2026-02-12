using System;
using Factory;

namespace Motorways
{
	// Token: 0x02000428 RID: 1064
	public readonly struct RoadTileNode : IComparable
	{
		// Token: 0x06001A2F RID: 6703 RVA: 0x0005F3AC File Offset: 0x0005D5AC
		public RoadTileNode(TileDirection direction, RoadType type = RoadType.TwoLane, int motorwayId = -1)
		{
			this.direction = direction;
			this.type = type;
			this.motorwayId = motorwayId;
		}

		// Token: 0x06001A30 RID: 6704 RVA: 0x0005F3C3 File Offset: 0x0005D5C3
		public RoadTileNode GetRotatedNode(RoadTileRotation rotation)
		{
			return new RoadTileNode(TileUtilities.GetRotatedDirection(this.direction, rotation), this.type, -1);
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x0005F3DD File Offset: 0x0005D5DD
		public static bool operator ==(RoadTileNode lhs, RoadTileNode rhs)
		{
			return lhs.Equals(rhs, TreatMotorwaysAs.Motorways);
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x0005F3E8 File Offset: 0x0005D5E8
		public static bool operator !=(RoadTileNode lhs, RoadTileNode rhs)
		{
			return !lhs.Equals(rhs, TreatMotorwaysAs.Motorways);
		}

		// Token: 0x06001A33 RID: 6707 RVA: 0x0005F3F8 File Offset: 0x0005D5F8
		public override bool Equals(object obj)
		{
			if (obj is RoadTileNode)
			{
				RoadTileNode otherNode = (RoadTileNode)obj;
				return this.CompareTo(otherNode) == 0;
			}
			return false;
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x0005F420 File Offset: 0x0005D620
		public bool Equals(RoadTileNode otherNode)
		{
			return this.CompareTo(otherNode) == 0;
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x0005F42C File Offset: 0x0005D62C
		public bool Equals(RoadTileNode otherNode, TreatMotorwaysAs motorwayTreatment)
		{
			if (this.direction != otherNode.direction)
			{
				return false;
			}
			if (motorwayTreatment == TreatMotorwaysAs.Motorways)
			{
				return this.type == otherNode.type && this.motorwayId == otherNode.motorwayId;
			}
			if (this.type == RoadType.TwoLane || this.type == RoadType.Motorway)
			{
				return otherNode.type == RoadType.TwoLane || otherNode.type == RoadType.Motorway;
			}
			return this.type == otherNode.type;
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x0005F4A0 File Offset: 0x0005D6A0
		public int CompareTo(object obj)
		{
			if (obj is RoadTileNode)
			{
				RoadTileNode otherNode = (RoadTileNode)obj;
				return this.CompareTo(otherNode);
			}
			return 1;
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x0005F4C8 File Offset: 0x0005D6C8
		public int CompareTo(RoadTileNode otherNode)
		{
			if (this.direction != otherNode.direction)
			{
				return this.direction - otherNode.direction;
			}
			if (this.type != otherNode.type)
			{
				return this.type - otherNode.type;
			}
			if (this.motorwayId != otherNode.motorwayId)
			{
				return this.motorwayId - otherNode.motorwayId;
			}
			return 0;
		}

		// Token: 0x06001A38 RID: 6712 RVA: 0x0005F52A File Offset: 0x0005D72A
		public override int GetHashCode()
		{
			return RoadTileNode.GetHashCode(this.type, this.direction, this.motorwayId);
		}

		// Token: 0x06001A39 RID: 6713 RVA: 0x0005F543 File Offset: 0x0005D743
		public int GetHashCode(TreatMotorwaysAs motorwayNodeTreatment)
		{
			if (motorwayNodeTreatment == TreatMotorwaysAs.TwoLaneRoads && this.type == RoadType.Motorway)
			{
				return RoadTileNode.GetHashCode(RoadType.TwoLane, this.direction, -1);
			}
			return RoadTileNode.GetHashCode(this.type, this.direction, this.motorwayId);
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x0005F577 File Offset: 0x0005D777
		private static int GetHashCode(RoadType type, TileDirection direction, int motorwayId)
		{
			return (int)((int)type << 16 | (RoadType)((int)direction << 8) | motorwayId + RoadType.Roundabout);
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x0005F588 File Offset: 0x0005D788
		public override string ToString()
		{
			string nodeString = this.direction.ToShortString();
			if (this.type != RoadType.TwoLane)
			{
				nodeString = nodeString + " " + this.type.ToString();
			}
			if (this.motorwayId != -1)
			{
				nodeString = nodeString + " " + this.motorwayId.ToString();
			}
			return nodeString;
		}

		// Token: 0x040015F7 RID: 5623
		public readonly TileDirection direction;

		// Token: 0x040015F8 RID: 5624
		public readonly RoadType type;

		// Token: 0x040015F9 RID: 5625
		public readonly int motorwayId;

		// Token: 0x02000429 RID: 1065
		public class Serializer : PrimitiveSerializer
		{
			// Token: 0x06001A3C RID: 6716 RVA: 0x0005F5EC File Offset: 0x0005D7EC
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is RoadTileNode)
				{
					RoadTileNode node = (RoadTileNode)obj;
					bool hasMotorwayNode = node.motorwayId != -1;
					context.Writer.Write(hasMotorwayNode);
					context.Writer.Write((byte)node.direction);
					context.Writer.Write((byte)node.type);
					if (hasMotorwayNode)
					{
						context.Writer.Write(node.motorwayId);
					}
					return true;
				}
				return false;
			}

			// Token: 0x06001A3D RID: 6717 RVA: 0x0005F65C File Offset: 0x0005D85C
			public override object Deserialize(object existingObj, ImportContext context)
			{
				bool hasMotorwayNode = context.Reader.ReadBoolean();
				return new RoadTileNode((TileDirection)context.Reader.ReadByte(), (RoadType)context.Reader.ReadByte(), hasMotorwayNode ? context.Reader.ReadInt32() : -1);
			}
		}
	}
}
