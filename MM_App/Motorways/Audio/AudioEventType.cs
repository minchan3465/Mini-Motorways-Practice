using System;

namespace Motorways.Audio
{
	// Token: 0x02000639 RID: 1593
	[Flags]
	public enum AudioEventType : long
	{
		// Token: 0x040026AE RID: 9902
		None = 0L,
		// Token: 0x040026AF RID: 9903
		BuildRoad = 1L,
		// Token: 0x040026B0 RID: 9904
		MothballRoad = 2L,
		// Token: 0x040026B1 RID: 9905
		VehicleArrivedAtDestination = 4L,
		// Token: 0x040026B2 RID: 9906
		VehicleArrivedAtHouse = 8L,
		// Token: 0x040026B3 RID: 9907
		VehicleDepartedDestination = 16L,
		// Token: 0x040026B4 RID: 9908
		VehicleDepartedHouse = 32L,
		// Token: 0x040026B5 RID: 9909
		VehicleEnteredMotorway = 64L,
		// Token: 0x040026B6 RID: 9910
		VehicleLeftMotorway = 128L,
		// Token: 0x040026B7 RID: 9911
		TrafficLightGreen = 512L,
		// Token: 0x040026B8 RID: 9912
		TrafficLightAmber = 1024L,
		// Token: 0x040026B9 RID: 9913
		TreeBulldozed = 2048L,
		// Token: 0x040026BA RID: 9914
		CityStart = 4096L,
		// Token: 0x040026BB RID: 9915
		ClockStart = 8192L,
		// Token: 0x040026BC RID: 9916
		WeekStart = 16384L,
		// Token: 0x040026BD RID: 9917
		HouseSpawned = 32768L,
		// Token: 0x040026BE RID: 9918
		DestinationSpawned = 65536L,
		// Token: 0x040026BF RID: 9919
		DestinationActivated = 131072L,
		// Token: 0x040026C0 RID: 9920
		VehicleEnteredCarpark = 262144L,
		// Token: 0x040026C1 RID: 9921
		DestinationDemanded = 524288L,
		// Token: 0x040026C2 RID: 9922
		VehicleReceivesPin = 1048576L,
		// Token: 0x040026C3 RID: 9923
		VehicleFulfillsDemand = 2097152L,
		// Token: 0x040026C4 RID: 9924
		UserInterface = 4194304L,
		// Token: 0x040026C5 RID: 9925
		TextMessageShown = 8388608L,
		// Token: 0x040026C6 RID: 9926
		GameOver = 16777216L,
		// Token: 0x040026C7 RID: 9927
		LogoPinAppear = 33554432L,
		// Token: 0x040026C8 RID: 9928
		LogoPinDisappear = 67108864L,
		// Token: 0x040026C9 RID: 9929
		UpgradeDragged = 134217728L,
		// Token: 0x040026CA RID: 9930
		UpgradeReleased = 268435456L,
		// Token: 0x040026CB RID: 9931
		UpgradeOver = 536870912L,
		// Token: 0x040026CC RID: 9932
		UpgradeOut = 1073741824L,
		// Token: 0x040026CD RID: 9933
		VehicleSpawned = 2147483648L,
		// Token: 0x040026CE RID: 9934
		Pulse = 4294967296L,
		// Token: 0x040026CF RID: 9935
		CreativeModeEditPanelButtonAppears = 8589934592L,
		// Token: 0x040026D0 RID: 9936
		UpgradePlaced = 274877906944L,
		// Token: 0x040026D1 RID: 9937
		MotorwayHandlePulled = 549755813888L,
		// Token: 0x040026D2 RID: 9938
		MotorwayHandleReleased = 1099511627776L,
		// Token: 0x040026D3 RID: 9939
		NightMode = 2199023255552L,
		// Token: 0x040026D4 RID: 9940
		DayStart = 4398046511104L,
		// Token: 0x040026D5 RID: 9941
		DrawMode = 8796093022208L,
		// Token: 0x040026D6 RID: 9942
		DestinationConnectedToNetwork = 17592186044416L,
		// Token: 0x040026D7 RID: 9943
		HouseConnectedToNetwork = 35184372088832L,
		// Token: 0x040026D8 RID: 9944
		BuildBridge = 562949953421312L,
		// Token: 0x040026D9 RID: 9945
		DestinationMutated = 1125899906842624L,
		// Token: 0x040026DA RID: 9946
		DestinationOvercrowding = 2251799813685248L,
		// Token: 0x040026DB RID: 9947
		RippleAlert = 4503599627370496L,
		// Token: 0x040026DC RID: 9948
		MenuExit = 9007199254740992L,
		// Token: 0x040026DD RID: 9949
		UpgradeDragSnap = 18014398509481984L,
		// Token: 0x040026DE RID: 9950
		LateGame = 36028797018963968L,
		// Token: 0x040026DF RID: 9951
		BuildTunnel = 72057594037927936L,
		// Token: 0x040026E0 RID: 9952
		UnlockMap = 144115188075855872L,
		// Token: 0x040026E1 RID: 9953
		AudioMinimized = 288230376151711744L,
		// Token: 0x040026E2 RID: 9954
		ElectiveUpgradeAvailable = 576460752303423488L,
		// Token: 0x040026E3 RID: 9955
		ElectiveUpgradePulse = 1152921504606846976L,
		// Token: 0x040026E4 RID: 9956
		TrainArrives = 2305843009213693952L,
		// Token: 0x040026E5 RID: 9957
		TrainDeparts = 4611686018427387904L
	}
}
