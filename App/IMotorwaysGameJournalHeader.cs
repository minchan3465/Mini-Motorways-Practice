using System;
using FixMath;
using Motorways;

// Token: 0x020001AC RID: 428
internal interface IMotorwaysGameJournalHeader
{
	// Token: 0x17000211 RID: 529
	// (get) Token: 0x0600097D RID: 2429
	GameJournalMotive Motive { get; }

	// Token: 0x17000212 RID: 530
	// (get) Token: 0x0600097E RID: 2430
	string DeviceModel { get; }

	// Token: 0x17000213 RID: 531
	// (get) Token: 0x0600097F RID: 2431
	string DeviceName { get; }

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x06000980 RID: 2432
	DateTime UtcTimestamp { get; }

	// Token: 0x17000215 RID: 533
	// (get) Token: 0x06000981 RID: 2433
	int GameAssemblerSerializerHashCode { get; }

	// Token: 0x17000216 RID: 534
	// (get) Token: 0x06000982 RID: 2434
	string CityId { get; }

	// Token: 0x17000217 RID: 535
	// (get) Token: 0x06000983 RID: 2435
	GameMode Mode { get; }

	// Token: 0x17000218 RID: 536
	// (get) Token: 0x06000984 RID: 2436
	int TripCount { get; }

	// Token: 0x17000219 RID: 537
	// (get) Token: 0x06000985 RID: 2437
	Fix64 TimeElapsed { get; }
}
