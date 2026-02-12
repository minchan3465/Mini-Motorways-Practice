using System;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005E6 RID: 1510
	public struct MotorwayPoint
	{
		// Token: 0x06002A1B RID: 10779 RVA: 0x000B75AD File Offset: 0x000B57AD
		public MotorwayPoint(Vector2 position, MotorwayPointType type)
		{
			this.position = position;
			this.type = type;
		}

		// Token: 0x0400241F RID: 9247
		public Vector2 position;

		// Token: 0x04002420 RID: 9248
		public MotorwayPointType type;
	}
}
