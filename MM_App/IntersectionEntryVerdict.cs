using System;

// Token: 0x0200019C RID: 412
public enum IntersectionEntryVerdict
{
	// Token: 0x040004C1 RID: 1217
	Unknown,
	// Token: 0x040004C2 RID: 1218
	NoIntersectingLanes,
	// Token: 0x040004C3 RID: 1219
	NoBlockingVehicles,
	// Token: 0x040004C4 RID: 1220
	ExceededMaximumWaitTime,
	// Token: 0x040004C5 RID: 1221
	NoReservedLane,
	// Token: 0x040004C6 RID: 1222
	BlockedByTrafficLight,
	// Token: 0x040004C7 RID: 1223
	BlockedByTraversingVehicle,
	// Token: 0x040004C8 RID: 1224
	BlockedByInboundVehicle,
	// Token: 0x040004C9 RID: 1225
	Shoved,
	// Token: 0x040004CA RID: 1226
	BlockedByUnsafeCrossing,
	// Token: 0x040004CB RID: 1227
	BlockedByCongestedCrossing
}
