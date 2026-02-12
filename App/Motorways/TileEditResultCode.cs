using System;

namespace Motorways
{
	// Token: 0x02000410 RID: 1040
	public enum TileEditResultCode
	{
		// Token: 0x0400157D RID: 5501
		Success,
		// Token: 0x0400157E RID: 5502
		NotInitialized,
		// Token: 0x0400157F RID: 5503
		InvalidTileCoordinate,
		// Token: 0x04001580 RID: 5504
		CannotConnectToCarpark,
		// Token: 0x04001581 RID: 5505
		CannotConnectHouseToBridge,
		// Token: 0x04001582 RID: 5506
		NotEnoughUpgrades,
		// Token: 0x04001583 RID: 5507
		NotEnoughConcrete,
		// Token: 0x04001584 RID: 5508
		NotEnoughConcreteForMotorway,
		// Token: 0x04001585 RID: 5509
		CannotClearTile,
		// Token: 0x04001586 RID: 5510
		MotorwayTooShort,
		// Token: 0x04001587 RID: 5511
		MotorwayBlockedByMountain,
		// Token: 0x04001588 RID: 5512
		CannotConnectHouseToTunnel,
		// Token: 0x04001589 RID: 5513
		ClearForSpecificTypeNotNeeded,
		// Token: 0x0400158A RID: 5514
		EditAlreadyExists,
		// Token: 0x0400158B RID: 5515
		CannotCreateBridge,
		// Token: 0x0400158C RID: 5516
		CannotCreateTunnel,
		// Token: 0x0400158D RID: 5517
		NoDeletableRoads,
		// Token: 0x0400158E RID: 5518
		NoDeletableUpgrade,
		// Token: 0x0400158F RID: 5519
		CannotConnectHouseToRail,
		// Token: 0x04001590 RID: 5520
		CannotCreateCrossing
	}
}
