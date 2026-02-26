using System;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000413 RID: 1043
	public interface ITilemap
	{
		// Token: 0x06001998 RID: 6552
		Tile GetTile(Vector2Int coordinates);

		// Token: 0x06001999 RID: 6553
		Tile GetOrCreateTile(Vector2Int coordinates);

		// Token: 0x0600199A RID: 6554
		Motorway GetMotorway(int id);

		// Token: 0x0600199B RID: 6555
		Motorway CreateMotorway(int id, int motorwayNumber, int replacedMotorwayNumber);
	}
}
