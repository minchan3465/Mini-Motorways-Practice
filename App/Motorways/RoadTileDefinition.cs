using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000427 RID: 1063
	[Factory.Serializable(1)]
	public class RoadTileDefinition : IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x06001A28 RID: 6696 RVA: 0x0005F163 File Offset: 0x0005D363
		public bool CanExport
		{
			get
			{
				return this.connectionToPath != null && this.rotation == RoadTileRotation.None && this.connectionToPath.Count > 0 && this.connectionToPath.Count <= 2;
			}
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x0005F198 File Offset: 0x0005D398
		public RoadTilePath GetPath(RoadTileConnection connection)
		{
			RoadTilePath path;
			if (this.connectionToPath.TryGetValue(connection, out path))
			{
				return path;
			}
			return null;
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x0005F1B8 File Offset: 0x0005D3B8
		public RoadTileDefinition CreateRotatedDefinition(IScope scope, RoadTileRotation newRotation)
		{
			RoadTileDefinition rotatedDefinition = scope.Get<RoadTileDefinition>();
			rotatedDefinition.mesh = this.mesh;
			rotatedDefinition.rotation = newRotation;
			RoadTileRotation amountToRotateConnections = TileUtilities.SubtractRotation(newRotation, this.rotation);
			foreach (KeyValuePair<RoadTileConnection, RoadTilePath> pair in this.connectionToPath)
			{
				RoadTileConnection connection = pair.Key.GetRotatedConnection(amountToRotateConnections);
				RoadTilePath path = pair.Value.CreateRotatedPath(amountToRotateConnections);
				rotatedDefinition.connectionToPath.Add(connection, path);
			}
			return rotatedDefinition;
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x0005F260 File Offset: 0x0005D460
		public override string ToString()
		{
			if (this.connectionToPath.Count == 0)
			{
				return "RoadTileDefinition[]";
			}
			List<string> connectionStrings = new List<string>();
			foreach (KeyValuePair<RoadTileConnection, RoadTilePath> connection in this.connectionToPath)
			{
				connectionStrings.Add(connection.Key.ToString());
			}
			return "RoadTileDefinition[" + string.Join(", ", connectionStrings) + "]";
		}

		// Token: 0x06001A2C RID: 6700 RVA: 0x0005F2FC File Offset: 0x0005D4FC
		public void OnReleasedFromScope(IScope scope)
		{
			foreach (RoadTilePath path in this.connectionToPath.Values)
			{
				scope.Release(path);
			}
			this.connectionToPath.Clear();
		}

		// Token: 0x06001A2D RID: 6701 RVA: 0x0005F360 File Offset: 0x0005D560
		public void Reset()
		{
			this.index = -1;
			this.mesh = null;
			this.rotation = RoadTileRotation.None;
			this.connectionToPath.Clear();
		}

		// Token: 0x040015F1 RID: 5617
		public int index = -1;

		// Token: 0x040015F2 RID: 5618
		public RoadTileMesh mesh;

		// Token: 0x040015F3 RID: 5619
		public RoadTileRotation rotation;

		// Token: 0x040015F4 RID: 5620
		public readonly Dictionary<RoadTileConnection, RoadTilePath> connectionToPath = new Dictionary<RoadTileConnection, RoadTilePath>(new RoadTileConnection.MotorwayAgnosticEqualityComparer());

		// Token: 0x040015F5 RID: 5621
		public Vector2 interactionCircleOffset = Vector2.zero;

		// Token: 0x040015F6 RID: 5622
		public Vector2[] trafficLightOffsets;
	}
}
