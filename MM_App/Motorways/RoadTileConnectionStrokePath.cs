using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using Motorways.Utility;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000426 RID: 1062
	[Factory.Serializable(1)]
	public class RoadTileConnectionStrokePath : IReusable
	{
		// Token: 0x06001A26 RID: 6694 RVA: 0x0005F13C File Offset: 0x0005D33C
		public void Reset()
		{
			this.pathPoints.Clear();
			this.pathSpline = null;
		}

		// Token: 0x040015EF RID: 5615
		public readonly List<Vector2> pathPoints = new List<Vector2>();

		// Token: 0x040015F0 RID: 5616
		public Spline.BezierSpline pathSpline;
	}
}
