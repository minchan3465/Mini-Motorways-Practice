using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;

namespace Motorways
{
	// Token: 0x0200042D RID: 1069
	[Factory.Serializable(1)]
	public class RoadTileSignature : IComparable<RoadTileSignature>, IReusable, IDisposable
	{
		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06001A4E RID: 6734 RVA: 0x0005FCFC File Offset: 0x0005DEFC
		public bool IsEmpty
		{
			get
			{
				return this._connections.Count == 0;
			}
		}

		// Token: 0x06001A4F RID: 6735 RVA: 0x0005FD0C File Offset: 0x0005DF0C
		public bool AddNode(RoadTileNode newNode)
		{
			if (!Diagnostics.Verify(newNode.type == RoadType.TwoLane || newNode.type == RoadType.Driveway || newNode.type == RoadType.Motorway, "Unable to add non-TwoLane roads."))
			{
				return false;
			}
			if (this._inputNodes.Contains(newNode) || this._outputNodes.Contains(newNode))
			{
				return false;
			}
			if (this._inputNodes.Count == 0 && this._outputNodes.Count == 0)
			{
				this.AddConnection(new RoadTileConnection(newNode, newNode));
				return true;
			}
			this._inputNodes.Add(newNode);
			this._outputNodes.Add(newNode);
			if (this.IsDeadEnd)
			{
				this._connections.RemoveAt(0);
			}
			foreach (RoadTileNode inputNode in this._inputNodes)
			{
				if (!inputNode.Equals(newNode))
				{
					this.AddConnection(new RoadTileConnection(inputNode, newNode));
				}
			}
			foreach (RoadTileNode outputNode in this._outputNodes)
			{
				if (!outputNode.Equals(newNode))
				{
					this.AddConnection(new RoadTileConnection(newNode, outputNode));
				}
			}
			return true;
		}

		// Token: 0x06001A50 RID: 6736 RVA: 0x0005FE60 File Offset: 0x0005E060
		public bool HasInputNode(RoadTileNode node)
		{
			foreach (RoadTileNode existingNode in this._inputNodes)
			{
				if (existingNode.Equals(node))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001A51 RID: 6737 RVA: 0x0005FEC0 File Offset: 0x0005E0C0
		public bool HasOutputNode(RoadTileNode node)
		{
			foreach (RoadTileNode existingNode in this._outputNodes)
			{
				if (existingNode.Equals(node))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001A52 RID: 6738 RVA: 0x0005FF20 File Offset: 0x0005E120
		public bool HasNode(RoadTileNode node)
		{
			return this.HasInputNode(node) || this.HasOutputNode(node);
		}

		// Token: 0x06001A53 RID: 6739 RVA: 0x0005FF34 File Offset: 0x0005E134
		public void AddConnection(RoadTileConnection connection)
		{
			int insertIndex = 0;
			while (insertIndex < this._connections.Count && this._connections[insertIndex].CompareTo(connection) < 0)
			{
				insertIndex++;
			}
			this._connections.Insert(insertIndex, connection);
			if (!this.HasInputNode(connection.input))
			{
				this._inputNodes.Add(connection.input);
			}
			if (!this.HasOutputNode(connection.output))
			{
				this._outputNodes.Add(connection.output);
			}
			this._connectionDirections[connection.input.direction] = true;
			this._connectionDirections[connection.output.direction] = true;
		}

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06001A54 RID: 6740 RVA: 0x0005FFE8 File Offset: 0x0005E1E8
		public IEnumerable<RoadTileConnection> Connections
		{
			get
			{
				return this._connections;
			}
		}

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06001A55 RID: 6741 RVA: 0x0005FFF0 File Offset: 0x0005E1F0
		public TileDirectionBitfield ConnectionDirections
		{
			get
			{
				return this._connectionDirections;
			}
		}

		// Token: 0x06001A56 RID: 6742 RVA: 0x0005FFF8 File Offset: 0x0005E1F8
		public bool HasConnection(RoadTileConnection connection)
		{
			return this._connections.Contains(connection);
		}

		// Token: 0x06001A57 RID: 6743 RVA: 0x00060006 File Offset: 0x0005E206
		public IEnumerable<RoadTileConnection> GetConnectionsToDirection(TileDirection direction)
		{
			foreach (RoadTileConnection connection in this._connections)
			{
				if (connection.input.direction == direction || connection.output.direction == direction)
				{
					yield return connection;
				}
			}
			List<RoadTileConnection>.Enumerator enumerator = default(List<RoadTileConnection>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06001A58 RID: 6744 RVA: 0x0006001D File Offset: 0x0005E21D
		public void Reset()
		{
			this._connections.Clear();
			this._inputNodes.Clear();
			this._outputNodes.Clear();
			this._connectionDirections = TileDirectionBitfield.None;
		}

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x06001A59 RID: 6745 RVA: 0x0006004C File Offset: 0x0005E24C
		public bool IsDeadEnd
		{
			get
			{
				return this._connections.Count == 1 && this._connections[0].IsUTurn;
			}
		}

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x06001A5A RID: 6746 RVA: 0x00060080 File Offset: 0x0005E280
		public bool IsRoundaboutCorner
		{
			get
			{
				foreach (RoadTileConnection connection in this._connections)
				{
					if (connection.input.type == RoadType.Roundabout && connection.output.type == RoadType.Roundabout && connection.input.direction != TileUtilities.GetOppositeDirection(connection.output.direction))
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x06001A5B RID: 6747 RVA: 0x0006010C File Offset: 0x0005E30C
		public RoadTileSignature CreateRotatedSignature(RoadTileRotation rotation, IScope context)
		{
			if (rotation == RoadTileRotation.None)
			{
				return this;
			}
			RoadTileSignature rotatedSignature = context.Get<RoadTileSignature>();
			foreach (RoadTileConnection connection in this._connections)
			{
				rotatedSignature.AddConnection(connection.GetRotatedConnection(rotation));
			}
			return rotatedSignature;
		}

		// Token: 0x06001A5C RID: 6748 RVA: 0x00060174 File Offset: 0x0005E374
		public int CompareTo(RoadTileSignature otherSignature)
		{
			if (this._connections.Count != otherSignature._connections.Count)
			{
				return this._connections.Count - otherSignature._connections.Count;
			}
			for (int connectionIndex = 0; connectionIndex < this._connections.Count; connectionIndex++)
			{
				int compareValue = this._connections[connectionIndex].CompareTo(otherSignature._connections[connectionIndex]);
				if (compareValue != 0)
				{
					return compareValue;
				}
			}
			return 0;
		}

		// Token: 0x06001A5D RID: 6749 RVA: 0x000601F0 File Offset: 0x0005E3F0
		public override bool Equals(object obj)
		{
			RoadTileSignature signature = obj as RoadTileSignature;
			return signature != null && this.CompareTo(signature) == 0;
		}

		// Token: 0x06001A5E RID: 6750 RVA: 0x00060214 File Offset: 0x0005E414
		public override int GetHashCode()
		{
			int hashCode = 0;
			foreach (RoadTileConnection connection in this._connections)
			{
				hashCode ^= connection.GetHashCode();
			}
			return hashCode;
		}

		// Token: 0x06001A5F RID: 6751 RVA: 0x00060274 File Offset: 0x0005E474
		public override string ToString()
		{
			if (this._connections.Count == 0)
			{
				return "RoadTileSignature";
			}
			List<string> connectionStrings = new List<string>();
			foreach (RoadTileConnection connection in this.Connections)
			{
				connectionStrings.Add(connection.ToString());
			}
			return string.Format("RoadTileSignature[Connections={0}]", string.Join(", ", connectionStrings));
		}

		// Token: 0x06001A60 RID: 6752 RVA: 0x000602FC File Offset: 0x0005E4FC
		public void Dispose()
		{
			this._scope.Release(this);
		}

		// Token: 0x04001604 RID: 5636
		[Dependency]
		private IScope _scope;

		// Token: 0x04001605 RID: 5637
		private readonly List<RoadTileConnection> _connections = new List<RoadTileConnection>();

		// Token: 0x04001606 RID: 5638
		private readonly List<RoadTileNode> _inputNodes = new List<RoadTileNode>();

		// Token: 0x04001607 RID: 5639
		private readonly List<RoadTileNode> _outputNodes = new List<RoadTileNode>();

		// Token: 0x04001608 RID: 5640
		private TileDirectionBitfield _connectionDirections = TileDirectionBitfield.None;

		// Token: 0x0200042E RID: 1070
		public class MotorwayAgnosticEqualityComparer : EqualityComparer<RoadTileSignature>
		{
			// Token: 0x06001A62 RID: 6754 RVA: 0x00060340 File Offset: 0x0005E540
			public override bool Equals(RoadTileSignature signature1, RoadTileSignature signature2)
			{
				if (signature1 == null && signature2 == null)
				{
					return true;
				}
				if (signature1 == null || signature2 == null)
				{
					return false;
				}
				int connectionCount = signature1._connections.Count;
				if (connectionCount != signature2._connections.Count)
				{
					return false;
				}
				for (int connectionIndex = 0; connectionIndex < connectionCount; connectionIndex++)
				{
					if (!signature1._connections[connectionIndex].Equals(signature2._connections[connectionIndex], TreatMotorwaysAs.TwoLaneRoads))
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x06001A63 RID: 6755 RVA: 0x000603AC File Offset: 0x0005E5AC
			public override int GetHashCode(RoadTileSignature signature)
			{
				int hashCode = 0;
				foreach (RoadTileConnection connection in signature._connections)
				{
					hashCode ^= connection.GetHashCode(TreatMotorwaysAs.TwoLaneRoads);
				}
				return hashCode;
			}
		}
	}
}
