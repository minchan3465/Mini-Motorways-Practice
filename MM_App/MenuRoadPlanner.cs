using System;
using System.Collections.Generic;
using Motorways;
using UnityEngine;

// Token: 0x02000263 RID: 611
public class MenuRoadPlanner : MonoBehaviour
{
	// Token: 0x04000893 RID: 2195
	public List<MenuRoadPlanner.RoadNode> roads;

	// Token: 0x04000894 RID: 2196
	public Vector3 offset;

	// Token: 0x04000895 RID: 2197
	[TextArea]
	[Header("In Order: x, y, length, direction")]
	public string outputFormat = "ScheduleLineOfRoads(new Vector2Int({0}, {1}), {2}, TileDirection.{3})\n";

	// Token: 0x02000264 RID: 612
	[Serializable]
	public struct RoadNode
	{
		// Token: 0x1700030C RID: 780
		// (get) Token: 0x06000E88 RID: 3720 RVA: 0x000315F1 File Offset: 0x0002F7F1
		public Vector2Int StartPoint
		{
			get
			{
				return this.startPosition;
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x06000E89 RID: 3721 RVA: 0x000315FC File Offset: 0x0002F7FC
		public Vector2Int EndPoint
		{
			get
			{
				Vector2Int directionVector = TileUtilities.GetAdjacencyOffsetForDirection(this.direction);
				directionVector.x *= this.length;
				directionVector.y *= this.length;
				return this.startPosition + directionVector;
			}
		}

		// Token: 0x04000896 RID: 2198
		public Vector2Int startPosition;

		// Token: 0x04000897 RID: 2199
		public TileDirection direction;

		// Token: 0x04000898 RID: 2200
		public int length;
	}
}
