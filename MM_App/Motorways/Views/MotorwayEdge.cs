using System;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005E4 RID: 1508
	public struct MotorwayEdge
	{
		// Token: 0x06002A1A RID: 10778 RVA: 0x000B758E File Offset: 0x000B578E
		public MotorwayEdge(MotorwayPoint from, MotorwayPoint to, Vector2 normal, MotorwayEdgeType type)
		{
			this.from = from;
			this.to = to;
			this.normal = normal;
			this.type = type;
		}

		// Token: 0x04002416 RID: 9238
		public MotorwayPoint from;

		// Token: 0x04002417 RID: 9239
		public MotorwayPoint to;

		// Token: 0x04002418 RID: 9240
		public Vector2 normal;

		// Token: 0x04002419 RID: 9241
		public MotorwayEdgeType type;
	}
}
