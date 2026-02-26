using System;

// Token: 0x02000134 RID: 308
public enum Feature
{
	// Token: 0x0400029F RID: 671
	Group_Hidden,
	// Token: 0x040002A0 RID: 672
	NotSelected,
	// Token: 0x040002A1 RID: 673
	OptionsDebugMenu,
	// Token: 0x040002A2 RID: 674
	DiagnosticReports,
	// Token: 0x040002A3 RID: 675
	DiagnosticReportsButton,
	// Token: 0x040002A4 RID: 676
	SubmitDiagnosticReportOnGameOver,
	// Token: 0x040002A5 RID: 677
	SubmitDiagnosticReportOnException,
	// Token: 0x040002A6 RID: 678
	ElevateErrorsForCloudDiagnostics,
	// Token: 0x040002A7 RID: 679
	SubmitOnlyOneDiagnosticReportOnExceptionPerGame,
	// Token: 0x040002A8 RID: 680
	ToggleDiagnosticReportButtonWithKeyCode,
	// Token: 0x040002A9 RID: 681
	RecordAppJournal,
	// Token: 0x040002AA RID: 682
	RecordSimulationJournal,
	// Token: 0x040002AB RID: 683
	RecordLogs,
	// Token: 0x040002AC RID: 684
	TrackAnalyticsInDiagnosticReports,
	// Token: 0x040002AD RID: 685
	LoadRemotePrefabs,
	// Token: 0x040002AE RID: 686
	BetaWatermark,
	// Token: 0x040002AF RID: 687
	AlwaysEnterTutorial,
	// Token: 0x040002B0 RID: 688
	SoakTest,
	// Token: 0x040002B1 RID: 689
	ValidateSimulationDeterminism,
	// Token: 0x040002B2 RID: 690
	AppleStoreDemo,
	// Token: 0x040002B3 RID: 691
	RecordStorageAuditTrail,
	// Token: 0x040002B4 RID: 692
	Analytics,
	// Token: 0x040002B5 RID: 693
	MockControllerAsRemote,
	// Token: 0x040002B6 RID: 694
	IdleVehicleCheckerDiagnosticReport,
	// Token: 0x040002B7 RID: 695
	LargeScoreDiagnosticReport,
	// Token: 0x040002B8 RID: 696
	OnScreenDebugTools,
	// Token: 0x040002B9 RID: 697
	TelemetryToggle,
	// Token: 0x040002BA RID: 698
	CycleLanguages,
	// Token: 0x040002BB RID: 699
	ValidatePooledObjectScrubbing,
	// Token: 0x040002BC RID: 700
	TrackScopedAllocations,
	// Token: 0x040002BD RID: 701
	RecordStackTracesForScopedAllocations,
	// Token: 0x040002BE RID: 702
	SteamBetaLeaderboards,
	// Token: 0x040002BF RID: 703
	Group_Maps,
	// Token: 0x040002C0 RID: 704
	MapUnlocks,
	// Token: 0x040002C1 RID: 705
	DebugMapUnlockButton,
	// Token: 0x040002C2 RID: 706
	Group_Challenges,
	// Token: 0x040002C3 RID: 707
	InjectDebugChallenges,
	// Token: 0x040002C4 RID: 708
	RandomChallengesMapButton,
	// Token: 0x040002C5 RID: 709
	RandomChallengesAreExpert,
	// Token: 0x040002C6 RID: 710
	ChallengeTimeControl,
	// Token: 0x040002C7 RID: 711
	CityChallenges,
	// Token: 0x040002C8 RID: 712
	Group_Notifications,
	// Token: 0x040002C9 RID: 713
	SkipGameCountAndViewedCheckForPermissionPopup,
	// Token: 0x040002CA RID: 714
	MessageDebugButtons,
	// Token: 0x040002CB RID: 715
	Group_InGame,
	// Token: 0x040002CC RID: 716
	BringMotorwaysToTopWhenEdited,
	// Token: 0x040002CD RID: 717
	InGameDevTools,
	// Token: 0x040002CE RID: 718
	MaximumWaitTimeAtIntersections,
	// Token: 0x040002CF RID: 719
	CheckForVehicleCollisionsWhenMerging,
	// Token: 0x040002D0 RID: 720
	IdleVehicleCheckerGUI,
	// Token: 0x040002D1 RID: 721
	CinematicMode,
	// Token: 0x040002D2 RID: 722
	Group_Menu,
	// Token: 0x040002D3 RID: 723
	ProfileSelectScreen,
	// Token: 0x040002D4 RID: 724
	AlwaysEnterResumeScreen,
	// Token: 0x040002D5 RID: 725
	FTUX_Accessibility,
	// Token: 0x040002D6 RID: 726
	ControllerSensitivityOption,
	// Token: 0x040002D7 RID: 727
	SteamCrossSave,
	// Token: 0x040002D8 RID: 728
	UpgradeScreenViewMap,
	// Token: 0x040002D9 RID: 729
	ExpertLock,
	// Token: 0x040002DA RID: 730
	Group_Art,
	// Token: 0x040002DB RID: 731
	TileHighlights,
	// Token: 0x040002DC RID: 732
	RoadDrawingAnimations,
	// Token: 0x040002DD RID: 733
	VehicleTrails,
	// Token: 0x040002DE RID: 734
	RoadDrawingEndTileCommit,
	// Token: 0x040002DF RID: 735
	Group_DebugDisplays,
	// Token: 0x040002E0 RID: 736
	DemandCounters,
	// Token: 0x040002E1 RID: 737
	ScheduleView,
	// Token: 0x040002E2 RID: 738
	TutorialView,
	// Token: 0x040002E3 RID: 739
	PlayerActionView,
	// Token: 0x040002E4 RID: 740
	RecordIntersectionDecisions,
	// Token: 0x040002E5 RID: 741
	EndlessEfficiencyText,
	// Token: 0x040002E6 RID: 742
	Group_DebugCheats,
	// Token: 0x040002E7 RID: 743
	StartWithTenMotorways,
	// Token: 0x040002E8 RID: 744
	ResetAchievementsButton,
	// Token: 0x040002E9 RID: 745
	UnlimitedUpgrades,
	// Token: 0x040002EA RID: 746
	DisplaySelection,
	// Token: 0x040002EB RID: 747
	ClockPauseColor,
	// Token: 0x040002EC RID: 748
	SmallPinSFXWithMinimalSoundscape,
	// Token: 0x040002ED RID: 749
	CursorFade,
	// Token: 0x040002EE RID: 750
	EndlessWithWeeklyMilestones,
	// Token: 0x040002EF RID: 751
	WrapperGameUI,
	// Token: 0x040002F0 RID: 752
	AutoZoomEnabledOption,
	// Token: 0x040002F1 RID: 753
	ToggleGameUIWithController,
	// Token: 0x040002F2 RID: 754
	ExpertNoDemolish,
	// Token: 0x040002F3 RID: 755
	MockPhone,
	// Token: 0x040002F4 RID: 756
	ExtraFastForward,
	// Token: 0x040002F5 RID: 757
	WhatTheCarEasterEgg
}
