using System;
using UnityEngine;

// Token: 0x0200026C RID: 620
public enum StringId
{
	// Token: 0x040008B2 RID: 2226
	None,
	// Token: 0x040008B3 RID: 2227
	MiniMotorways,
	// Token: 0x040008B4 RID: 2228
	Minutes,
	// Token: 0x040008B5 RID: 2229
	Hours,
	// Token: 0x040008B6 RID: 2230
	Monday,
	// Token: 0x040008B7 RID: 2231
	Tuesday,
	// Token: 0x040008B8 RID: 2232
	Wednesday,
	// Token: 0x040008B9 RID: 2233
	Thursday,
	// Token: 0x040008BA RID: 2234
	Friday,
	// Token: 0x040008BB RID: 2235
	Saturday,
	// Token: 0x040008BC RID: 2236
	Sunday,
	// Token: 0x040008BD RID: 2237
	MondayLong,
	// Token: 0x040008BE RID: 2238
	TuesdayLong,
	// Token: 0x040008BF RID: 2239
	WednesdayLong,
	// Token: 0x040008C0 RID: 2240
	ThursdayLong,
	// Token: 0x040008C1 RID: 2241
	FridayLong,
	// Token: 0x040008C2 RID: 2242
	SaturdayLong,
	// Token: 0x040008C3 RID: 2243
	SundayLong,
	// Token: 0x040008C4 RID: 2244
	Restart,
	// Token: 0x040008C5 RID: 2245
	TryAgain,
	// Token: 0x040008C6 RID: 2246
	Options,
	// Token: 0x040008C7 RID: 2247
	Exit,
	// Token: 0x040008C8 RID: 2248
	GameOver,
	// Token: 0x040008C9 RID: 2249
	Concrete_Title,
	// Token: 0x040008CA RID: 2250
	Bridge_Title,
	// Token: 0x040008CB RID: 2251
	Tunnel_Title,
	// Token: 0x040008CC RID: 2252
	TrafficLight_Title,
	// Token: 0x040008CD RID: 2253
	Motorway_Title,
	// Token: 0x040008CE RID: 2254
	Roundabout_Title,
	// Token: 0x040008CF RID: 2255
	Concrete,
	// Token: 0x040008D0 RID: 2256
	Bridge,
	// Token: 0x040008D1 RID: 2257
	Tunnel,
	// Token: 0x040008D2 RID: 2258
	TrafficLight,
	// Token: 0x040008D3 RID: 2259
	Motorway,
	// Token: 0x040008D4 RID: 2260
	Roundabout,
	// Token: 0x040008D5 RID: 2261
	Play,
	// Token: 0x040008D6 RID: 2262
	Replay,
	// Token: 0x040008D7 RID: 2263
	Begin,
	// Token: 0x040008D8 RID: 2264
	Continue,
	// Token: 0x040008D9 RID: 2265
	Leaderboard_Player,
	// Token: 0x040008DA RID: 2266
	Audio_Off,
	// Token: 0x040008DB RID: 2267
	Audio_FirstLevel,
	// Token: 0x040008DC RID: 2268
	Audio_SecondLevel,
	// Token: 0x040008DD RID: 2269
	Audio_ThirdLevel,
	// Token: 0x040008DE RID: 2270
	Soundscape_Off,
	// Token: 0x040008DF RID: 2271
	Soundscape_Minimal,
	// Token: 0x040008E0 RID: 2272
	Soundscape_Full,
	// Token: 0x040008E1 RID: 2273
	Options_Game,
	// Token: 0x040008E2 RID: 2274
	Options_Video,
	// Token: 0x040008E3 RID: 2275
	Options_Audio,
	// Token: 0x040008E4 RID: 2276
	Options_Controls,
	// Token: 0x040008E5 RID: 2277
	Options_iCloud,
	// Token: 0x040008E6 RID: 2278
	Options_Messages,
	// Token: 0x040008E7 RID: 2279
	Options_Debug,
	// Token: 0x040008E8 RID: 2280
	Options_Controls_SiriRemote,
	// Token: 0x040008E9 RID: 2281
	Options_Controls_Gamepad,
	// Token: 0x040008EA RID: 2282
	Options_Controls_Touch,
	// Token: 0x040008EB RID: 2283
	Options_Controls_Keyboard,
	// Token: 0x040008EC RID: 2284
	Options_Controls_Mouse,
	// Token: 0x040008ED RID: 2285
	Options_Controls_Input_Title_Action,
	// Token: 0x040008EE RID: 2286
	Options_Controls_Input_Title_Key,
	// Token: 0x040008EF RID: 2287
	Options_Controls_Input_Title_InputMethod,
	// Token: 0x040008F0 RID: 2288
	Options_Controls_Input_Move,
	// Token: 0x040008F1 RID: 2289
	Options_Controls_Input_Draw,
	// Token: 0x040008F2 RID: 2290
	Options_Controls_Input_Back,
	// Token: 0x040008F3 RID: 2291
	Options_Controls_Input_Delete,
	// Token: 0x040008F4 RID: 2292
	Options_Controls_Input_ToggleDelete,
	// Token: 0x040008F5 RID: 2293
	Options_Controls_Input_Touch_BuildRoad,
	// Token: 0x040008F6 RID: 2294
	Language,
	// Token: 0x040008F7 RID: 2295
	Fullscreen,
	// Token: 0x040008F8 RID: 2296
	Retina,
	// Token: 0x040008F9 RID: 2297
	NightMode,
	// Token: 0x040008FA RID: 2298
	SkipTransitions,
	// Token: 0x040008FB RID: 2299
	FrameRate,
	// Token: 0x040008FC RID: 2300
	Smooth,
	// Token: 0x040008FD RID: 2301
	LowPower,
	// Token: 0x040008FE RID: 2302
	VeryLowPower,
	// Token: 0x040008FF RID: 2303
	ColorblindMode,
	// Token: 0x04000900 RID: 2304
	Volume,
	// Token: 0x04000901 RID: 2305
	Soundscape,
	// Token: 0x04000902 RID: 2306
	Resolution,
	// Token: 0x04000903 RID: 2307
	Vibration,
	// Token: 0x04000904 RID: 2308
	ShowAchievements,
	// Token: 0x04000905 RID: 2309
	Back,
	// Token: 0x04000906 RID: 2310
	InProgress,
	// Token: 0x04000907 RID: 2311
	Tutorial,
	// Token: 0x04000908 RID: 2312
	New,
	// Token: 0x04000909 RID: 2313
	DeleteAllDataButton,
	// Token: 0x0400090A RID: 2314
	DeleteAllDataPrompt_iCloud,
	// Token: 0x0400090B RID: 2315
	ConfirmDeleteAllData,
	// Token: 0x0400090C RID: 2316
	CancelDeleteAllData,
	// Token: 0x0400090D RID: 2317
	DeleteSpecificJournalPrompt_iCloud,
	// Token: 0x0400090E RID: 2318
	ConfirmDeleteSpecificJournal,
	// Token: 0x0400090F RID: 2319
	CancelDeleteSpecificJournal,
	// Token: 0x04000910 RID: 2320
	DeleteProfile,
	// Token: 0x04000911 RID: 2321
	DeleteProfileDescription,
	// Token: 0x04000912 RID: 2322
	DeleteProfileDescription2,
	// Token: 0x04000913 RID: 2323
	DeleteProfileDescription3,
	// Token: 0x04000914 RID: 2324
	ProfileIcon,
	// Token: 0x04000915 RID: 2325
	BackgroundColor,
	// Token: 0x04000916 RID: 2326
	TotalTrips,
	// Token: 0x04000917 RID: 2327
	LastDatePlayed,
	// Token: 0x04000918 RID: 2328
	ControllerConfigureButton,
	// Token: 0x04000919 RID: 2329
	Select,
	// Token: 0x0400091A RID: 2330
	Selected,
	// Token: 0x0400091B RID: 2331
	Create,
	// Token: 0x0400091C RID: 2332
	UpgradeConcreteDescription,
	// Token: 0x0400091D RID: 2333
	UpgradeBridgeDescription,
	// Token: 0x0400091E RID: 2334
	UpgradeTunnelDescription,
	// Token: 0x0400091F RID: 2335
	UpgradeMotorwayDescription,
	// Token: 0x04000920 RID: 2336
	UpgradeRoundaboutDescription,
	// Token: 0x04000921 RID: 2337
	UpgradeTrafficLightDescription,
	// Token: 0x04000922 RID: 2338
	BestScore,
	// Token: 0x04000923 RID: 2339
	TargetScore,
	// Token: 0x04000924 RID: 2340
	Score,
	// Token: 0x04000925 RID: 2341
	You,
	// Token: 0x04000926 RID: 2342
	TopPercentile,
	// Token: 0x04000927 RID: 2343
	BottomPercentile,
	// Token: 0x04000928 RID: 2344
	iCloudNotConnectedToInternet,
	// Token: 0x04000929 RID: 2345
	iCloudNotLoggedIn,
	// Token: 0x0400092A RID: 2346
	iCloudNetworkProblem,
	// Token: 0x0400092B RID: 2347
	iCloudQuotaExceeded,
	// Token: 0x0400092C RID: 2348
	iCloudAuthenticationIssue,
	// Token: 0x0400092D RID: 2349
	iCloudUpdateRequired,
	// Token: 0x0400092E RID: 2350
	iCloudRestrictedAccount,
	// Token: 0x0400092F RID: 2351
	SignedInToiCloud,
	// Token: 0x04000930 RID: 2352
	Online,
	// Token: 0x04000931 RID: 2353
	SyncedWithiCloud,
	// Token: 0x04000932 RID: 2354
	iCloudServiceName,
	// Token: 0x04000933 RID: 2355
	SyncToiCloud,
	// Token: 0x04000934 RID: 2356
	SyncingWithiCloud,
	// Token: 0x04000935 RID: 2357
	OptionsMenuMessages,
	// Token: 0x04000936 RID: 2358
	OptionsMenuMessagesDescription,
	// Token: 0x04000937 RID: 2359
	OptionsNotifications,
	// Token: 0x04000938 RID: 2360
	OptionsNotificationsAreDisabled,
	// Token: 0x04000939 RID: 2361
	OptionsNotificationsAreEnabled,
	// Token: 0x0400093A RID: 2362
	OptionsChallengeReminders,
	// Token: 0x0400093B RID: 2363
	OptionsContentReminders,
	// Token: 0x0400093C RID: 2364
	OptionsEnableNotificationsButton,
	// Token: 0x0400093D RID: 2365
	LosAngeles,
	// Token: 0x0400093E RID: 2366
	LosAngelesDescription,
	// Token: 0x0400093F RID: 2367
	Beijing,
	// Token: 0x04000940 RID: 2368
	BeijingDescription,
	// Token: 0x04000941 RID: 2369
	MexicoCity,
	// Token: 0x04000942 RID: 2370
	MexicoCityDescription,
	// Token: 0x04000943 RID: 2371
	Tokyo,
	// Token: 0x04000944 RID: 2372
	TokyoDescription,
	// Token: 0x04000945 RID: 2373
	DarEsSalaam,
	// Token: 0x04000946 RID: 2374
	DarEsSalaamDescription,
	// Token: 0x04000947 RID: 2375
	Moscow,
	// Token: 0x04000948 RID: 2376
	MoscowDescription,
	// Token: 0x04000949 RID: 2377
	Munich,
	// Token: 0x0400094A RID: 2378
	MunichDescription,
	// Token: 0x0400094B RID: 2379
	Manila,
	// Token: 0x0400094C RID: 2380
	ManilaDescription,
	// Token: 0x0400094D RID: 2381
	Zurich,
	// Token: 0x0400094E RID: 2382
	ZurichDescription,
	// Token: 0x0400094F RID: 2383
	RioDeJaneiro,
	// Token: 0x04000950 RID: 2384
	RioDeJaneiroDescription,
	// Token: 0x04000951 RID: 2385
	SubmitDiagnosticsReport,
	// Token: 0x04000952 RID: 2386
	PassThroughString,
	// Token: 0x04000953 RID: 2387
	ControllerRequired,
	// Token: 0x04000954 RID: 2388
	Tutorial_PromptToDrawRoads_Mouse,
	// Token: 0x04000955 RID: 2389
	Tutorial_DeleteAllRoads,
	// Token: 0x04000956 RID: 2390
	Tutorial_Welcome,
	// Token: 0x04000957 RID: 2391
	Tutorial_HouseIntroduction,
	// Token: 0x04000958 RID: 2392
	Tutorial_DestinationIntroduction,
	// Token: 0x04000959 RID: 2393
	Tutorial_DemandIntroduction,
	// Token: 0x0400095A RID: 2394
	Tutorial_FreeDragToBuildRoads,
	// Token: 0x0400095B RID: 2395
	Tutorial_TapEnterBuildMode,
	// Token: 0x0400095C RID: 2396
	Tutorial_DragToBuildRoads,
	// Token: 0x0400095D RID: 2397
	Tutorial_ExitDrawMode,
	// Token: 0x0400095E RID: 2398
	Tutorial_ControllerStartDrawingRoads,
	// Token: 0x0400095F RID: 2399
	Tutorial_RemoteStartDrawingRoads,
	// Token: 0x04000960 RID: 2400
	Tutorial_ControllerDrawRoad,
	// Token: 0x04000961 RID: 2401
	Tutorial_RemoteDrawRoad,
	// Token: 0x04000962 RID: 2402
	Tutorial_RemoteEnterBuildMode,
	// Token: 0x04000963 RID: 2403
	Tutorial_TouchDeleteMode,
	// Token: 0x04000964 RID: 2404
	Tutorial_MouseDeleteMode,
	// Token: 0x04000965 RID: 2405
	Tutorial_RemoteDeleteMode,
	// Token: 0x04000966 RID: 2406
	Tutorial_ControllerDeleteMode,
	// Token: 0x04000967 RID: 2407
	Tutorial_TouchDeleteMode_UnnecessaryRoadsTip,
	// Token: 0x04000968 RID: 2408
	Tutorial_MouseDeleteMode_UnnecessaryRoadsTip,
	// Token: 0x04000969 RID: 2409
	Tutorial_ControllerDeleteMode_UnnecessaryRoadsTip,
	// Token: 0x0400096A RID: 2410
	Tutorial_RemoteDeleteMode_UnnecessaryRoadsTip,
	// Token: 0x0400096B RID: 2411
	Tutorial_EarlyDeleteMode,
	// Token: 0x0400096C RID: 2412
	Tutorial_TouchEarlyDeleteMode,
	// Token: 0x0400096D RID: 2413
	Tutorial_ControllerEarlyDeleteMode,
	// Token: 0x0400096E RID: 2414
	Tutorial_RemoteEarlyDeleteMode,
	// Token: 0x0400096F RID: 2415
	Tutorial_ScoreIntroduction,
	// Token: 0x04000970 RID: 2416
	Tutorial_ClockIntroduction,
	// Token: 0x04000971 RID: 2417
	Tutorial_SecondHouseConnection,
	// Token: 0x04000972 RID: 2418
	Tutorial_ThirdHouseConnection,
	// Token: 0x04000973 RID: 2419
	Tutorial_RiverHouseConnection,
	// Token: 0x04000974 RID: 2420
	Tutorial_RiverHouseConnectionInstruction,
	// Token: 0x04000975 RID: 2421
	Tutorial_NewColour,
	// Token: 0x04000976 RID: 2422
	Tutorial_OvercrowdingOne,
	// Token: 0x04000977 RID: 2423
	Tutorial_OvercrowdingTwo,
	// Token: 0x04000978 RID: 2424
	Tutorial_OvercrowdingThree,
	// Token: 0x04000979 RID: 2425
	Tutorial_BridgeHouseConnection,
	// Token: 0x0400097A RID: 2426
	Tutorial_TrafficLight,
	// Token: 0x0400097B RID: 2427
	Tutorial_TrafficLightUpgradeScreen,
	// Token: 0x0400097C RID: 2428
	Tutorial_MotorwayUpgradeScreen,
	// Token: 0x0400097D RID: 2429
	Tutorial_MotorwayZero,
	// Token: 0x0400097E RID: 2430
	Tutorial_MotorwayOne,
	// Token: 0x0400097F RID: 2431
	Tutorial_MotorwayTwo,
	// Token: 0x04000980 RID: 2432
	Tutorial_MotorwayThree,
	// Token: 0x04000981 RID: 2433
	Tutorial_MotorwayFour,
	// Token: 0x04000982 RID: 2434
	Tutorial_End,
	// Token: 0x04000983 RID: 2435
	Tutorial_EndTwo,
	// Token: 0x04000984 RID: 2436
	Tutorial_FirstUpgradeConcrete,
	// Token: 0x04000985 RID: 2437
	Tutorial_FirstUpgradeBridge,
	// Token: 0x04000986 RID: 2438
	Tutorial_SecondUpgrade,
	// Token: 0x04000987 RID: 2439
	Tutorial_TouchTwoFingerPan,
	// Token: 0x04000988 RID: 2440
	Tutorial_ChooseTheBridge,
	// Token: 0x04000989 RID: 2441
	Tutorial_DrawRoadAcrossWater,
	// Token: 0x0400098A RID: 2442
	Tutorial_ChooseTheTrafficLight,
	// Token: 0x0400098B RID: 2443
	Error_NotEnoughConcrete,
	// Token: 0x0400098C RID: 2444
	Error_Tutorial_NotEnoughConcrete,
	// Token: 0x0400098D RID: 2445
	Error_Tutorial_NotEnoughConcrete_Touch,
	// Token: 0x0400098E RID: 2446
	Error_Tutorial_NotEnoughConcrete_Mouse,
	// Token: 0x0400098F RID: 2447
	Error_Tutorial_NotEnoughConcrete_Controller,
	// Token: 0x04000990 RID: 2448
	Error_Tutorial_NotEnoughConcrete_Remote,
	// Token: 0x04000991 RID: 2449
	Error_NotEnoughConcreteMotorway,
	// Token: 0x04000992 RID: 2450
	Error_CannotConnectToCarpark,
	// Token: 0x04000993 RID: 2451
	Error_CannotConnectHouseToBridge,
	// Token: 0x04000994 RID: 2452
	Error_CannotConnectHouseToTunnel,
	// Token: 0x04000995 RID: 2453
	Error_MotorwayCollidesWithMountain,
	// Token: 0x04000996 RID: 2454
	Error_TileDoesntSupportMotorway,
	// Token: 0x04000997 RID: 2455
	Error_MotorwayTooShort,
	// Token: 0x04000998 RID: 2456
	Error_MotorwayNoAvailableRampDirection,
	// Token: 0x04000999 RID: 2457
	Error_MotorwayNoAvailableRampPairing,
	// Token: 0x0400099A RID: 2458
	Error_TileDoesntSupportRoundabout,
	// Token: 0x0400099B RID: 2459
	Error_CannotConnectHouseToRail,
	// Token: 0x0400099C RID: 2460
	MiniMotorways_Menu,
	// Token: 0x0400099D RID: 2461
	WeekCount,
	// Token: 0x0400099E RID: 2462
	WeekTagline_ChooseUpgrade,
	// Token: 0x0400099F RID: 2463
	WeekTagline_Concrete,
	// Token: 0x040009A0 RID: 2464
	SkipTutorial,
	// Token: 0x040009A1 RID: 2465
	GameOver_LineOne,
	// Token: 0x040009A2 RID: 2466
	GameOver_LineTwo,
	// Token: 0x040009A3 RID: 2467
	GameOver_TutorialEarly_LineOne,
	// Token: 0x040009A4 RID: 2468
	GameOver_TutorialEarly_LineTwo,
	// Token: 0x040009A5 RID: 2469
	GameOver_TutorialLate_LineOne,
	// Token: 0x040009A6 RID: 2470
	GameOver_TutorialLate_LineTwo,
	// Token: 0x040009A7 RID: 2471
	ExitToMenu,
	// Token: 0x040009A8 RID: 2472
	MainMenu,
	// Token: 0x040009A9 RID: 2473
	ResumeGame,
	// Token: 0x040009AA RID: 2474
	NewProfile,
	// Token: 0x040009AB RID: 2475
	AchievementsHeader,
	// Token: 0x040009AC RID: 2476
	AchievementDescription_LosAngeles_Score_1,
	// Token: 0x040009AD RID: 2477
	AchievementDescription_Beijing_Score_1,
	// Token: 0x040009AE RID: 2478
	AchievementDescription_MexicoCity_Score_1,
	// Token: 0x040009AF RID: 2479
	AchievementDescription_DarEsSalaam_Score_1,
	// Token: 0x040009B0 RID: 2480
	AchievementDescription_Moscow_Score_1,
	// Token: 0x040009B1 RID: 2481
	AchievementDescription_Tokyo_Score_1,
	// Token: 0x040009B2 RID: 2482
	AchievementDescription_Munich_Score_1,
	// Token: 0x040009B3 RID: 2483
	AchievementDescription_Manila_Score_1,
	// Token: 0x040009B4 RID: 2484
	AchievementDescription_Zurich_Score_1,
	// Token: 0x040009B5 RID: 2485
	AchievementDescription_RioDeJaneiro_Score_1,
	// Token: 0x040009B6 RID: 2486
	AchievementDescription_LosAngeles_Score_1_Title,
	// Token: 0x040009B7 RID: 2487
	AchievementDescription_Beijing_Score_1_Title,
	// Token: 0x040009B8 RID: 2488
	AchievementDescription_MexicoCity_Score_1_Title,
	// Token: 0x040009B9 RID: 2489
	AchievementDescription_DarEsSalaam_Score_1_Title,
	// Token: 0x040009BA RID: 2490
	AchievementDescription_Moscow_Score_1_Title,
	// Token: 0x040009BB RID: 2491
	AchievementDescription_Tokyo_Score_1_Title,
	// Token: 0x040009BC RID: 2492
	AchievementDescription_Munich_Score_1_Title,
	// Token: 0x040009BD RID: 2493
	AchievementDescription_Manila_Score_1_Title,
	// Token: 0x040009BE RID: 2494
	AchievementDescription_Zurich_Score_1_Title,
	// Token: 0x040009BF RID: 2495
	AchievementDescription_RioDeJaneiro_Score_1_Title,
	// Token: 0x040009C0 RID: 2496
	AchievementDescription_DailyChallenge_1_Title,
	// Token: 0x040009C1 RID: 2497
	AchievementDescription_DailyChallenge_1,
	// Token: 0x040009C2 RID: 2498
	AchievementDescription_DailyChallenge_1_Achieved,
	// Token: 0x040009C3 RID: 2499
	AchievementDescription_WeeklyChallenge_1_Title,
	// Token: 0x040009C4 RID: 2500
	AchievementDescription_WeeklyChallenge_1,
	// Token: 0x040009C5 RID: 2501
	AchievementDescription_WeeklyChallenge_1_Achieved,
	// Token: 0x040009C6 RID: 2502
	AchievementDescription_Tutorial,
	// Token: 0x040009C7 RID: 2503
	AchievementDescription_Tutorial_Title,
	// Token: 0x040009C8 RID: 2504
	AchievementDescription_Tutorial_Achieved,
	// Token: 0x040009C9 RID: 2505
	Credits_AdditionalThanks,
	// Token: 0x040009CA RID: 2506
	Credits_Accounting,
	// Token: 0x040009CB RID: 2507
	Credits_Art,
	// Token: 0x040009CC RID: 2508
	Credits_Audio,
	// Token: 0x040009CD RID: 2509
	Credits_Design,
	// Token: 0x040009CE RID: 2510
	Credits_Legal,
	// Token: 0x040009CF RID: 2511
	Credits_LevelDesign,
	// Token: 0x040009D0 RID: 2512
	Credits_Licenses,
	// Token: 0x040009D1 RID: 2513
	Credits_Localization,
	// Token: 0x040009D2 RID: 2514
	Credits_Production,
	// Token: 0x040009D3 RID: 2515
	Credits_Programming,
	// Token: 0x040009D4 RID: 2516
	Credits_StudioAndBusinessManagement,
	// Token: 0x040009D5 RID: 2517
	Credits_StudioAssistant,
	// Token: 0x040009D6 RID: 2518
	Credits_Testing,
	// Token: 0x040009D7 RID: 2519
	Credits_Video,
	// Token: 0x040009D8 RID: 2520
	Credits_ProducedByApple,
	// Token: 0x040009D9 RID: 2521
	Credits_CommunityManagement,
	// Token: 0x040009DA RID: 2522
	Credits_QualityAssurance,
	// Token: 0x040009DB RID: 2523
	AppleDemo_SplashScreenNotice,
	// Token: 0x040009DC RID: 2524
	AppleDemo_FeatureNotEnabled,
	// Token: 0x040009DD RID: 2525
	DailyChallenge,
	// Token: 0x040009DE RID: 2526
	WeeklyChallenge,
	// Token: 0x040009DF RID: 2527
	WeeklyChallenge_ThisWeek,
	// Token: 0x040009E0 RID: 2528
	WeeklyChallenge_LastWeek,
	// Token: 0x040009E1 RID: 2529
	DailyChallenge_Tutorial,
	// Token: 0x040009E2 RID: 2530
	WeeklyChallenge_Tutorial,
	// Token: 0x040009E3 RID: 2531
	DailyChallenge_LockedConfirmation,
	// Token: 0x040009E4 RID: 2532
	DailyChallenge_SaveGameConfirmation,
	// Token: 0x040009E5 RID: 2533
	DailyChallenge_SaveGameConfirmationNewMap,
	// Token: 0x040009E6 RID: 2534
	DailyChallenge_SaveGameConfirmationResumeGame,
	// Token: 0x040009E7 RID: 2535
	DailyChallenge_RestartConfirmation,
	// Token: 0x040009E8 RID: 2536
	Challenge_TimeLeft_Days,
	// Token: 0x040009E9 RID: 2537
	Challenge_TimeLeft_Hours,
	// Token: 0x040009EA RID: 2538
	Challenge_TimeLeft_Minutes,
	// Token: 0x040009EB RID: 2539
	Challenge_TimeLeft_Seconds,
	// Token: 0x040009EC RID: 2540
	Challenge_RandomChallengesMapTitle,
	// Token: 0x040009ED RID: 2541
	Challenge_RandomChallengesMapDescription,
	// Token: 0x040009EE RID: 2542
	Challenge_Upgrades_StartWithMotorwayTitle,
	// Token: 0x040009EF RID: 2543
	Challenge_Upgrades_StartWithMotorwayDescription,
	// Token: 0x040009F0 RID: 2544
	Challenge_Upgrades_StartWithMotorwayNotBridgeDescription,
	// Token: 0x040009F1 RID: 2545
	Challenge_Upgrades_StartWithBridgesTitle,
	// Token: 0x040009F2 RID: 2546
	Challenge_Upgrades_StartWithBridgesDescription,
	// Token: 0x040009F3 RID: 2547
	Challenge_Upgrades_StartWithTrafficLightsTitle,
	// Token: 0x040009F4 RID: 2548
	Challenge_Upgrades_StartWithTrafficLightsDescription,
	// Token: 0x040009F5 RID: 2549
	Challenge_Upgrades_StartWithTunnelsTitle,
	// Token: 0x040009F6 RID: 2550
	Challenge_Upgrades_StartWithTunnelsDescription,
	// Token: 0x040009F7 RID: 2551
	Challenge_Upgrades_OneLimitedMotorwayTitle,
	// Token: 0x040009F8 RID: 2552
	Challenge_Upgrades_SomeLimitedMotorwaysTitle,
	// Token: 0x040009F9 RID: 2553
	Challenge_Upgrades_LimitedMotorwaysDescription,
	// Token: 0x040009FA RID: 2554
	Challenge_Upgrades_LimitedTunnelsTitle,
	// Token: 0x040009FB RID: 2555
	Challenge_Upgrades_LimitedTunnelsDescription,
	// Token: 0x040009FC RID: 2556
	Challenge_Upgrades_LimitedTrafficLightsTitle,
	// Token: 0x040009FD RID: 2557
	Challenge_Upgrades_LimitedTrafficLightsDescription,
	// Token: 0x040009FE RID: 2558
	Challenge_Upgrades_StartWithNoBridgesTitle,
	// Token: 0x040009FF RID: 2559
	Challenge_Upgrades_StartWithNoBridgesDescription,
	// Token: 0x04000A00 RID: 2560
	Challenge_Upgrades_StartWithNoTunnelsTitle,
	// Token: 0x04000A01 RID: 2561
	Challenge_Upgrades_StartWithNoTunnelsDescription,
	// Token: 0x04000A02 RID: 2562
	Challenge_Upgrades_StartWith3MotorwaysTitle,
	// Token: 0x04000A03 RID: 2563
	Challenge_Upgrades_StartWithMaxMotorwaysTitle,
	// Token: 0x04000A04 RID: 2564
	Challenge_Upgrades_StartWithMaxMotorwaysDescription,
	// Token: 0x04000A05 RID: 2565
	Challenge_Upgrades_StartWithLimitedBridgesTitle,
	// Token: 0x04000A06 RID: 2566
	Challenge_Upgrades_StartWithLimitedBridgesDescription,
	// Token: 0x04000A07 RID: 2567
	Challenge_Upgrades_StartWithExtraRoadTilesTitle,
	// Token: 0x04000A08 RID: 2568
	Challenge_Upgrades_StartWithExtraRoadTilesDescription,
	// Token: 0x04000A09 RID: 2569
	Challenge_Upgrades_StartWithBridgeLimitedRoadsTitle,
	// Token: 0x04000A0A RID: 2570
	Challenge_Upgrades_StartWithBridgeLimitedRoadsDescription,
	// Token: 0x04000A0B RID: 2571
	Challenge_Upgrades_NoBridgesTitle,
	// Token: 0x04000A0C RID: 2572
	Challenge_Upgrades_NoBridgesDescription,
	// Token: 0x04000A0D RID: 2573
	Challenge_Upgrades_NoMotorwaysTitle,
	// Token: 0x04000A0E RID: 2574
	Challenge_Upgrades_NoMotorwaysDescription,
	// Token: 0x04000A0F RID: 2575
	Challenge_Upgrades_NoTrafficLightsTitle,
	// Token: 0x04000A10 RID: 2576
	Challenge_Upgrades_NoTrafficLightsDescription,
	// Token: 0x04000A11 RID: 2577
	Challenge_Upgrades_NoTunnelsTitle,
	// Token: 0x04000A12 RID: 2578
	Challenge_Upgrades_NoTunnelsDescription,
	// Token: 0x04000A13 RID: 2579
	Challenge_Upgrades_NoExtraRoadTilesTitle,
	// Token: 0x04000A14 RID: 2580
	Challenge_Upgrades_NoExtraRoadTilesDescription,
	// Token: 0x04000A15 RID: 2581
	Challenge_Upgrades_DoubleUpgradesTitle,
	// Token: 0x04000A16 RID: 2582
	Challenge_Upgrades_DoubleUpgradesDescription,
	// Token: 0x04000A17 RID: 2583
	Challenge_Upgrades_DoubleTrafficLightsTitle,
	// Token: 0x04000A18 RID: 2584
	Challenge_Upgrades_DoubleTrafficLightsDescription,
	// Token: 0x04000A19 RID: 2585
	Challenge_Upgrades_DoubleBridgesTitle,
	// Token: 0x04000A1A RID: 2586
	Challenge_Upgrades_DoubleBridgesDescription,
	// Token: 0x04000A1B RID: 2587
	Challenge_Upgrades_DoubleMotorwaysTitle,
	// Token: 0x04000A1C RID: 2588
	Challenge_Upgrades_DoubleMotorwaysDescription,
	// Token: 0x04000A1D RID: 2589
	Challenge_Upgrades_DoubleRoadTilesTitle,
	// Token: 0x04000A1E RID: 2590
	Challenge_Upgrades_DoubleRoadTilesDescription,
	// Token: 0x04000A1F RID: 2591
	Challenge_Upgrades_DoubleTunnelsTitle,
	// Token: 0x04000A20 RID: 2592
	Challenge_Upgrades_DoubleTunnelsDescription,
	// Token: 0x04000A21 RID: 2593
	Challenge_Upgrades_StartWithFullSetUpgradesTitle,
	// Token: 0x04000A22 RID: 2594
	Challenge_Upgrades_StartWithFullSetUpgradesDescription,
	// Token: 0x04000A23 RID: 2595
	Challenge_Upgrades_OnlyFullSetUpgradesTitle,
	// Token: 0x04000A24 RID: 2596
	Challenge_Upgrades_OnlyFullSetUpgradesDescription,
	// Token: 0x04000A25 RID: 2597
	Challenge_Upgrades_FewerRoadTilesUpgradesTitle,
	// Token: 0x04000A26 RID: 2598
	Challenge_Upgrades_FewerRoadTilesUpgradesDescription,
	// Token: 0x04000A27 RID: 2599
	Challenge_Upgrades_NoWeeklyChoiceTitle,
	// Token: 0x04000A28 RID: 2600
	Challenge_Upgrades_NoWeeklyChoiceDescription,
	// Token: 0x04000A29 RID: 2601
	Challenge_Upgrades_OnlyOneWeeklyChoiceTitle,
	// Token: 0x04000A2A RID: 2602
	Challenge_Upgrades_OnlyOneWeeklyChoiceDescription,
	// Token: 0x04000A2B RID: 2603
	Challenge_Upgrades_ThreeWeeklyChoicesTitle,
	// Token: 0x04000A2C RID: 2604
	Challenge_Upgrades_ThreeWeeklyChoicesDescription,
	// Token: 0x04000A2D RID: 2605
	Challenge_Destinations_GroupCircleDestinationsTitle,
	// Token: 0x04000A2E RID: 2606
	Challenge_Destinations_GroupCircleDestinationsDescription,
	// Token: 0x04000A2F RID: 2607
	Challenge_Destinations_AllCircleDestinationsTitle,
	// Token: 0x04000A30 RID: 2608
	Challenge_Destinations_AllCircleDestinationsDescription,
	// Token: 0x04000A31 RID: 2609
	Challenge_Destinations_NoCircleDestinationsTitle,
	// Token: 0x04000A32 RID: 2610
	Challenge_Destinations_NoCircleDestinationsDescription,
	// Token: 0x04000A33 RID: 2611
	Challenge_Destinations_NoDeadzonesTitle,
	// Token: 0x04000A34 RID: 2612
	Challenge_Destinations_NoDeadzonesDescription,
	// Token: 0x04000A35 RID: 2613
	Challenge_Destinations_AnySpawnDestinationsTitle,
	// Token: 0x04000A36 RID: 2614
	Challenge_Destinations_AnySpawnDestinationsDescription,
	// Token: 0x04000A37 RID: 2615
	Challenge_Destinations_DoubleDestinationsTitle,
	// Token: 0x04000A38 RID: 2616
	Challenge_Destinations_DoubleDestinationsDescription,
	// Token: 0x04000A39 RID: 2617
	Leaderboard_ChallengeTimeRunningOut,
	// Token: 0x04000A3A RID: 2618
	LeaderboardError_RecurringLeaderboardUnsupported,
	// Token: 0x04000A3B RID: 2619
	LeaderboardError_NotAuthenticatedGameCenter,
	// Token: 0x04000A3C RID: 2620
	LeaderboardError_Generic,
	// Token: 0x04000A3D RID: 2621
	LeaderboardError_NotAuthenticatedNsa,
	// Token: 0x04000A3E RID: 2622
	LeaderboardError_NoConnection,
	// Token: 0x04000A3F RID: 2623
	Local_Notifications_PermissionsRequest_Title,
	// Token: 0x04000A40 RID: 2624
	Local_Notifications_PermissionsRequest_Description,
	// Token: 0x04000A41 RID: 2625
	Local_Notifications_PermissionsRequest_Confirmation,
	// Token: 0x04000A42 RID: 2626
	Local_Notifications_PermissionsRequest_DeniedConfirmation,
	// Token: 0x04000A43 RID: 2627
	Local_Notifications_RecurringDC5Days_Title,
	// Token: 0x04000A44 RID: 2628
	Local_Notifications_RecurringDC5Days_Text,
	// Token: 0x04000A45 RID: 2629
	Local_Notifications_RecurringDC14Days_Title,
	// Token: 0x04000A46 RID: 2630
	Local_Notifications_RecurringDC14Days_Text,
	// Token: 0x04000A47 RID: 2631
	Local_Notifications_RecurringWC1Day_Title,
	// Token: 0x04000A48 RID: 2632
	Local_Notifications_RecurringWC1Day_Text,
	// Token: 0x04000A49 RID: 2633
	Local_Notifications_RecurringWC5Days_Title,
	// Token: 0x04000A4A RID: 2634
	Local_Notifications_RecurringWC5Days_Text,
	// Token: 0x04000A4B RID: 2635
	Local_Notifications_RecurringWC21Days_Title,
	// Token: 0x04000A4C RID: 2636
	Local_Notifications_RecurringWC21Days_Text,
	// Token: 0x04000A4D RID: 2637
	Local_Notifications_RecurringGame7Days_Title,
	// Token: 0x04000A4E RID: 2638
	Local_Notifications_RecurringGame7Days_Text,
	// Token: 0x04000A4F RID: 2639
	Local_Notifications_RecurringGame28Days_Title,
	// Token: 0x04000A50 RID: 2640
	Local_Notifications_RecurringGame28Days_Text,
	// Token: 0x04000A51 RID: 2641
	Local_Notifications_RecurringNewMap3Days_Title,
	// Token: 0x04000A52 RID: 2642
	Local_Notifications_RecurringNewMap3Days_Text,
	// Token: 0x04000A53 RID: 2643
	Local_Notifications_RecurringNewMap10Days_Title,
	// Token: 0x04000A54 RID: 2644
	Local_Notifications_RecurringNewMap10Days_Text,
	// Token: 0x04000A55 RID: 2645
	Local_Notifications_1OffDCWC7Days_Title,
	// Token: 0x04000A56 RID: 2646
	Local_Notifications_1OffDCWC7Days_Text,
	// Token: 0x04000A57 RID: 2647
	Local_Notifications_1OffDCWC21Days_Title,
	// Token: 0x04000A58 RID: 2648
	Local_Notifications_1OffDCWC21Days_Text,
	// Token: 0x04000A59 RID: 2649
	Local_Notifications_1OffNewMap7Days_Title,
	// Token: 0x04000A5A RID: 2650
	Local_Notifications_1OffNewMap7Days_Text,
	// Token: 0x04000A5B RID: 2651
	InGame_Messages_RecurringDC3Hours_Text,
	// Token: 0x04000A5C RID: 2652
	InGame_Messages_RecurringDC20Hours_Text,
	// Token: 0x04000A5D RID: 2653
	InGame_Messages_RecurringWCAvailable_Text,
	// Token: 0x04000A5E RID: 2654
	InGame_Messages_RecurringWC6Days_Text,
	// Token: 0x04000A5F RID: 2655
	InGame_Messages_RecurringNewUpdate_Text,
	// Token: 0x04000A60 RID: 2656
	InGame_Messages_RecurringResumeSavedGame_Text,
	// Token: 0x04000A61 RID: 2657
	InGame_Messages_1OffWelcome_Text,
	// Token: 0x04000A62 RID: 2658
	InGame_Messages_1OffTutorial1Day_Text,
	// Token: 0x04000A63 RID: 2659
	InGame_Messages_1OffDCWCUpdate_Text,
	// Token: 0x04000A64 RID: 2660
	InGame_Messages_1OffDCWC7Days_Text,
	// Token: 0x04000A65 RID: 2661
	InGame_Messages_1OffDCWC21Days_Text,
	// Token: 0x04000A66 RID: 2662
	InGame_Messages_1OffNewMap7Days_Text,
	// Token: 0x04000A67 RID: 2663
	FTUX_Accessibility_EnableColorblindModeDescription,
	// Token: 0x04000A68 RID: 2664
	FTUX_Accessibility_ReplayTutorialDescription,
	// Token: 0x04000A69 RID: 2665
	FTUX_Accessibility_ReplayTutorialPrompt,
	// Token: 0x04000A6A RID: 2666
	FTUX_Accessibility_SkipTransitionDescription,
	// Token: 0x04000A6B RID: 2667
	Dubai,
	// Token: 0x04000A6C RID: 2668
	DubaiDescription,
	// Token: 0x04000A6D RID: 2669
	AchievementDescription_Dubai_Score_1_Achieved,
	// Token: 0x04000A6E RID: 2670
	MysteryUpgradeDescription,
	// Token: 0x04000A6F RID: 2671
	MysteryUpgradeName,
	// Token: 0x04000A70 RID: 2672
	DrawModeToggle,
	// Token: 0x04000A71 RID: 2673
	FTUX_Accessibility_DrawModeToggleDescription,
	// Token: 0x04000A72 RID: 2674
	Challenge_Upgrades_UnlimitedBridge,
	// Token: 0x04000A73 RID: 2675
	Challenge_Upgrades_UnlimitedBridgeDescription,
	// Token: 0x04000A74 RID: 2676
	Challenge_Upgrades_UnlimitedConcreteDescription,
	// Token: 0x04000A75 RID: 2677
	Challenge_Upgrades_UnlimitedConcrete,
	// Token: 0x04000A76 RID: 2678
	Challenge_Upgrades_UnlimitedTunnelDescription,
	// Token: 0x04000A77 RID: 2679
	Challenge_Upgrades_UnlimitedTunnel,
	// Token: 0x04000A78 RID: 2680
	Challenge_Upgrades_UnlimitedTrafficLightDescription,
	// Token: 0x04000A79 RID: 2681
	Challenge_Upgrades_UnlimitedTrafficLight,
	// Token: 0x04000A7A RID: 2682
	Challenge_Upgrades_UnlimitedRoundaboutDescription,
	// Token: 0x04000A7B RID: 2683
	Challenge_Upgrades_UnlimitedRoundabout,
	// Token: 0x04000A7C RID: 2684
	Challenge_Upgrades_OneLimitedRoundaboutDescription,
	// Token: 0x04000A7D RID: 2685
	Challenge_Upgrades_OneLimitedRoundabout,
	// Token: 0x04000A7E RID: 2686
	Challenge_Upgrades_StartWithFourRoundaboutDescription,
	// Token: 0x04000A7F RID: 2687
	Challenge_Upgrades_StartWithFourRoundabout,
	// Token: 0x04000A80 RID: 2688
	Challenge_Upgrades_RoundaboutOnlyDescription,
	// Token: 0x04000A81 RID: 2689
	Challenge_Upgrades_RoundaboutOnly,
	// Token: 0x04000A82 RID: 2690
	Challenge_Upgrades_DoubleRoundaboutDescription,
	// Token: 0x04000A83 RID: 2691
	Challenge_Upgrades_DoubleRoundabout,
	// Token: 0x04000A84 RID: 2692
	Challenge_Upgrades_RandomUpgradeDescription,
	// Token: 0x04000A85 RID: 2693
	Challenge_Upgrades_RandomUpgrade,
	// Token: 0x04000A86 RID: 2694
	Challenge_Environment_IndestructableTreesDescription,
	// Token: 0x04000A87 RID: 2695
	Challenge_Environment_IndestructableTrees,
	// Token: 0x04000A88 RID: 2696
	Options_Game_DrawDeleteToggle,
	// Token: 0x04000A89 RID: 2697
	Options_Controls_Input_DecreaseGameSpeed,
	// Token: 0x04000A8A RID: 2698
	Options_Controls_Input_IncreaseGameSpeed,
	// Token: 0x04000A8B RID: 2699
	Options_Controls_Input_TogglePause,
	// Token: 0x04000A8C RID: 2700
	Options_Controls_Input_PauseGame,
	// Token: 0x04000A8D RID: 2701
	Options_Controls_Input_FastForward,
	// Token: 0x04000A8E RID: 2702
	Options_Video_Display,
	// Token: 0x04000A8F RID: 2703
	Options_Video_AntiAliasing,
	// Token: 0x04000A90 RID: 2704
	Options_Video_AntiAliasing_Off,
	// Token: 0x04000A91 RID: 2705
	Options_Video_AntiAliasing_2x,
	// Token: 0x04000A92 RID: 2706
	Options_Video_AntiAliasing_4x,
	// Token: 0x04000A93 RID: 2707
	Options_Video_AntiAliasing_8x,
	// Token: 0x04000A94 RID: 2708
	Options_Controls_Input_LockUI,
	// Token: 0x04000A95 RID: 2709
	Options_Controls_Input_HideUI,
	// Token: 0x04000A96 RID: 2710
	Options_Controls_Input_ShowUI,
	// Token: 0x04000A97 RID: 2711
	Options_Controls_Input_ShowLockUI,
	// Token: 0x04000A98 RID: 2712
	Options_Controls_Keyboard_Spacebar,
	// Token: 0x04000A99 RID: 2713
	Options_Controls_Keyboard_RightArrow,
	// Token: 0x04000A9A RID: 2714
	Options_Controls_Keyboard_LeftArrow,
	// Token: 0x04000A9B RID: 2715
	Options_Controls_Keyboard_LeftArrowRightArrow,
	// Token: 0x04000A9C RID: 2716
	Options_Controls_Keyboard_DownArrow,
	// Token: 0x04000A9D RID: 2717
	Options_Controls_Keyboard_UpArrow,
	// Token: 0x04000A9E RID: 2718
	Credits,
	// Token: 0x04000A9F RID: 2719
	MapUnlock,
	// Token: 0x04000AA0 RID: 2720
	MapUnlock_ToUnlock,
	// Token: 0x04000AA1 RID: 2721
	PhotoGif_Save_Directory_Mac,
	// Token: 0x04000AA2 RID: 2722
	PhotoGif_Save_Directory_Steam,
	// Token: 0x04000AA3 RID: 2723
	AchievementDescription_Dubai_Score_1,
	// Token: 0x04000AA4 RID: 2724
	AchievementDescription_Dubai_Score_1_Title,
	// Token: 0x04000AA5 RID: 2725
	AchievementDescription_Dubai_Score_2_Title,
	// Token: 0x04000AA6 RID: 2726
	AchievementDescription_MexicoCity_Score_2_Title,
	// Token: 0x04000AA7 RID: 2727
	AchievementDescription_RioDeJaneiro_Score_2_Title,
	// Token: 0x04000AA8 RID: 2728
	AchievementDescription_Manila_Score_2_Title,
	// Token: 0x04000AA9 RID: 2729
	AchievementDescription_Zurich_Score_2_Title,
	// Token: 0x04000AAA RID: 2730
	AchievementDescription_Munich_Score_2_Title,
	// Token: 0x04000AAB RID: 2731
	AchievementDescription_Moscow_Score_2_Title,
	// Token: 0x04000AAC RID: 2732
	AchievementDescription_DarEsSalaam_Score_2_Title,
	// Token: 0x04000AAD RID: 2733
	AchievementDescription_Tokyo_Score_2_Title,
	// Token: 0x04000AAE RID: 2734
	AchievementDescription_Beijing_Score_2_Title,
	// Token: 0x04000AAF RID: 2735
	AchievementDescription_LosAngeles_Score_2_Title,
	// Token: 0x04000AB0 RID: 2736
	AchievementDescription_Tokyo_Score_2,
	// Token: 0x04000AB1 RID: 2737
	AchievementDescription_Dubai_Score_2,
	// Token: 0x04000AB2 RID: 2738
	AchievementDescription_MexicoCity_Score_2,
	// Token: 0x04000AB3 RID: 2739
	AchievementDescription_RioDeJaneiro_Score_2,
	// Token: 0x04000AB4 RID: 2740
	AchievementDescription_Manila_Score_2,
	// Token: 0x04000AB5 RID: 2741
	AchievementDescription_Zurich_Score_2,
	// Token: 0x04000AB6 RID: 2742
	AchievementDescription_Munich_Score_2,
	// Token: 0x04000AB7 RID: 2743
	AchievementDescription_Moscow_Score_2,
	// Token: 0x04000AB8 RID: 2744
	AchievementDescription_DarEsSalaam_Score_2,
	// Token: 0x04000AB9 RID: 2745
	AchievementDescription_Beijing_Score_2,
	// Token: 0x04000ABA RID: 2746
	AchievementDescription_LosAngeles_Score_2,
	// Token: 0x04000ABB RID: 2747
	AchievementDescription_Dubai_Score_3,
	// Token: 0x04000ABC RID: 2748
	AchievementDescription_MexicoCity_Score_3,
	// Token: 0x04000ABD RID: 2749
	AchievementDescription_RioDeJaneiro_Score_3,
	// Token: 0x04000ABE RID: 2750
	AchievementDescription_Manila_Score_3,
	// Token: 0x04000ABF RID: 2751
	AchievementDescription_Zurich_Score_3,
	// Token: 0x04000AC0 RID: 2752
	AchievementDescription_Munich_Score_3,
	// Token: 0x04000AC1 RID: 2753
	AchievementDescription_Moscow_Score_3,
	// Token: 0x04000AC2 RID: 2754
	AchievementDescription_DarEsSalaam_Score_3,
	// Token: 0x04000AC3 RID: 2755
	AchievementDescription_Tokyo_Score_3,
	// Token: 0x04000AC4 RID: 2756
	AchievementDescription_Beijing_Score_3,
	// Token: 0x04000AC5 RID: 2757
	AchievementDescription_LosAngeles_Score_3,
	// Token: 0x04000AC6 RID: 2758
	AchievementDescription_Dubai_Score_3_Title,
	// Token: 0x04000AC7 RID: 2759
	AchievementDescription_MexicoCity_Score_3_Title,
	// Token: 0x04000AC8 RID: 2760
	AchievementDescription_RioDeJaneiro_Score_3_Title,
	// Token: 0x04000AC9 RID: 2761
	AchievementDescription_Manila_Score_3_Title,
	// Token: 0x04000ACA RID: 2762
	AchievementDescription_Zurich_Score_3_Title,
	// Token: 0x04000ACB RID: 2763
	AchievementDescription_Munich_Score_3_Title,
	// Token: 0x04000ACC RID: 2764
	AchievementDescription_Moscow_Score_3_Title,
	// Token: 0x04000ACD RID: 2765
	AchievementDescription_DarEsSalaam_Score_3_Title,
	// Token: 0x04000ACE RID: 2766
	AchievementDescription_Tokyo_Score_3_Title,
	// Token: 0x04000ACF RID: 2767
	AchievementDescription_Beijing_Score_3_Title,
	// Token: 0x04000AD0 RID: 2768
	AchievementDescription_LosAngeles_Score_3_Title,
	// Token: 0x04000AD1 RID: 2769
	AchievementDescription_WinThereDoneThat_Description,
	// Token: 0x04000AD2 RID: 2770
	AchievementDescription_WinThereDoneThat_Title,
	// Token: 0x04000AD3 RID: 2771
	AchievementDescription_WheelyNiceTime_Description,
	// Token: 0x04000AD4 RID: 2772
	AchievementDescription_WheelyNiceTime_Title,
	// Token: 0x04000AD5 RID: 2773
	AchievementDescription_MassTransit_Description,
	// Token: 0x04000AD6 RID: 2774
	AchievementDescription_MassTransit_Title,
	// Token: 0x04000AD7 RID: 2775
	AchievementDescription_StairwayToSeven_Description,
	// Token: 0x04000AD8 RID: 2776
	AchievementDescription_StairwayToSeven_Title,
	// Token: 0x04000AD9 RID: 2777
	AchievementDescription_SkylineFive_Description,
	// Token: 0x04000ADA RID: 2778
	AchievementDescription_SkylineFive_Title,
	// Token: 0x04000ADB RID: 2779
	AchievementDescription_TheseAreTheDays_Description,
	// Token: 0x04000ADC RID: 2780
	AchievementDescription_TheseAreTheDays_Title,
	// Token: 0x04000ADD RID: 2781
	AchievementDescription_TheLongHaul_Description,
	// Token: 0x04000ADE RID: 2782
	AchievementDescription_TheLongHaul_Title,
	// Token: 0x04000ADF RID: 2783
	AchievementDescription_VroomVroom_Description,
	// Token: 0x04000AE0 RID: 2784
	AchievementDescription_VroomVroom_Title,
	// Token: 0x04000AE1 RID: 2785
	AchievementDescription_NeedForSpeed_Description,
	// Token: 0x04000AE2 RID: 2786
	AchievementDescription_NeedForSpeed_Title,
	// Token: 0x04000AE3 RID: 2787
	AchievementDescription_WoodRiddance_Description,
	// Token: 0x04000AE4 RID: 2788
	AchievementDescription_WoodRiddance_Title,
	// Token: 0x04000AE5 RID: 2789
	AchievementDescription_OneToOneHundred_Description,
	// Token: 0x04000AE6 RID: 2790
	AchievementDescription_OneToOneHundred_Title,
	// Token: 0x04000AE7 RID: 2791
	AchievementDescription_TrollTown_Description,
	// Token: 0x04000AE8 RID: 2792
	AchievementDescription_TrollTown_Title,
	// Token: 0x04000AE9 RID: 2793
	AchievementDescription_TheDarkSideOfTheRoad_Description,
	// Token: 0x04000AEA RID: 2794
	AchievementDescription_TheDarkSideOfTheRoad_Title,
	// Token: 0x04000AEB RID: 2795
	AchievementDescription_OneOfEverything_Description,
	// Token: 0x04000AEC RID: 2796
	AchievementDescription_OneOfEverything_Title,
	// Token: 0x04000AED RID: 2797
	AchievementDescription_DriversLicense_Description,
	// Token: 0x04000AEE RID: 2798
	AchievementDescription_DriversLicense_Title,
	// Token: 0x04000AEF RID: 2799
	AchievementDescription_TryTryAgain_Description,
	// Token: 0x04000AF0 RID: 2800
	AchievementDescription_TryTryAgain_Title,
	// Token: 0x04000AF1 RID: 2801
	AchievementDescription_RoadIncarnation_Description,
	// Token: 0x04000AF2 RID: 2802
	AchievementDescription_RoadIncarnation_Title,
	// Token: 0x04000AF3 RID: 2803
	AchievementDescription_TheLongWayHome_Description,
	// Token: 0x04000AF4 RID: 2804
	AchievementDescription_TheLongWayHome_Title,
	// Token: 0x04000AF5 RID: 2805
	AchievementDescription_HighwayToHell_Description,
	// Token: 0x04000AF6 RID: 2806
	AchievementDescription_HighwayToHell_Title,
	// Token: 0x04000AF7 RID: 2807
	AchievementDescription_ATileAMinute_Description,
	// Token: 0x04000AF8 RID: 2808
	AchievementDescription_ATileAMinute_Title,
	// Token: 0x04000AF9 RID: 2809
	AchievementDescription_TensOfTunnels_Description,
	// Token: 0x04000AFA RID: 2810
	AchievementDescription_TensOfTunnels_Title,
	// Token: 0x04000AFB RID: 2811
	AchievementDescription_HittingAllTheLights_Description,
	// Token: 0x04000AFC RID: 2812
	AchievementDescription_HittingAllTheLights_Title,
	// Token: 0x04000AFD RID: 2813
	AchievementDescription_ISeetheLights_Description,
	// Token: 0x04000AFE RID: 2814
	AchievementDescription_ISeetheLights_Title,
	// Token: 0x04000AFF RID: 2815
	AchievementDescription_BoogieLights_Description,
	// Token: 0x04000B00 RID: 2816
	AchievementDescription_BoogieLights_Title,
	// Token: 0x04000B01 RID: 2817
	AchievementDescription_RapidTransit_Description,
	// Token: 0x04000B02 RID: 2818
	AchievementDescription_RapidTransit_Title,
	// Token: 0x04000B03 RID: 2819
	AchievementDescription_FiftyRich_Description,
	// Token: 0x04000B04 RID: 2820
	AchievementDescription_FiftyRich_Title,
	// Token: 0x04000B05 RID: 2821
	AchievementDescription_ManyManyMotorways_Description,
	// Token: 0x04000B06 RID: 2822
	AchievementDescription_ManyManyMotorways_Title,
	// Token: 0x04000B07 RID: 2823
	AchievementDescription_MoveIsInTheHeart_Description,
	// Token: 0x04000B08 RID: 2824
	AchievementDescription_MoveIsInTheHeart_Title,
	// Token: 0x04000B09 RID: 2825
	AchievementDescription_GoTheExtraMile_Description,
	// Token: 0x04000B0A RID: 2826
	AchievementDescription_GoTheExtraMile_Title,
	// Token: 0x04000B0B RID: 2827
	AchievementDescription_VanishIntoPinAir_Description,
	// Token: 0x04000B0C RID: 2828
	AchievementDescription_VanishIntoPinAir_Title,
	// Token: 0x04000B0D RID: 2829
	AchievementDescription_RoundaboutCity_Description,
	// Token: 0x04000B0E RID: 2830
	AchievementDescription_RoundaboutCity_Title,
	// Token: 0x04000B0F RID: 2831
	AchievementDescription_GoinginCircles_Description,
	// Token: 0x04000B10 RID: 2832
	AchievementDescription_GoinginCircles_Title,
	// Token: 0x04000B11 RID: 2833
	AchievementDescription_TheRoundaboutWay_Description,
	// Token: 0x04000B12 RID: 2834
	AchievementDescription_TheRoundaboutWay_Title,
	// Token: 0x04000B13 RID: 2835
	AchievementDescription_TunnelThrough_Description,
	// Token: 0x04000B14 RID: 2836
	AchievementDescription_TunnelThrough_Title,
	// Token: 0x04000B15 RID: 2837
	AchievementDescription_TunnelVision_Description,
	// Token: 0x04000B16 RID: 2838
	AchievementDescription_TunnelVision_Title,
	// Token: 0x04000B17 RID: 2839
	AchievementDescription_ATonofTunnels_Description,
	// Token: 0x04000B18 RID: 2840
	AchievementDescription_ATonofTunnels_Title,
	// Token: 0x04000B19 RID: 2841
	AchievementDescription_ABridgetoEverywhere_Description,
	// Token: 0x04000B1A RID: 2842
	AchievementDescription_ABridgetoEverywhere_Title,
	// Token: 0x04000B1B RID: 2843
	AchievementDescription_LetsBuildBridges_Description,
	// Token: 0x04000B1C RID: 2844
	AchievementDescription_LetsBuildBridges_Title,
	// Token: 0x04000B1D RID: 2845
	AchievementDescription_BridgetheGap_Description,
	// Token: 0x04000B1E RID: 2846
	AchievementDescription_BridgetheGap_Title,
	// Token: 0x04000B1F RID: 2847
	AchievementDescription_MomsSpaghetti_Description,
	// Token: 0x04000B20 RID: 2848
	AchievementDescription_MomsSpaghetti_Title,
	// Token: 0x04000B21 RID: 2849
	AchievementDescription_PinlessCity_Description,
	// Token: 0x04000B22 RID: 2850
	AchievementDescription_PinlessCity_Title,
	// Token: 0x04000B23 RID: 2851
	AchievementDescription_DailyChallenge_3_Achieved,
	// Token: 0x04000B24 RID: 2852
	AchievementDescription_DailyChallenge_2_Achieved,
	// Token: 0x04000B25 RID: 2853
	AchievementDescription_DailyChallenge_3,
	// Token: 0x04000B26 RID: 2854
	AchievementDescription_DailyChallenge_3_Title,
	// Token: 0x04000B27 RID: 2855
	AchievementDescription_DailyChallenge_2,
	// Token: 0x04000B28 RID: 2856
	AchievementDescription_DailyChallenge_2_Title,
	// Token: 0x04000B29 RID: 2857
	AchievementDescription_WeeklyChallenge_2_Achieved,
	// Token: 0x04000B2A RID: 2858
	AchievementDescription_WeeklyChallenge_2,
	// Token: 0x04000B2B RID: 2859
	AchievementDescription_WeeklyChallenge_2_Title,
	// Token: 0x04000B2C RID: 2860
	AchievementDescription_LosAngeles_Score_1_Achieved,
	// Token: 0x04000B2D RID: 2861
	AchievementDescription_Beijing_Score_1_Achieved,
	// Token: 0x04000B2E RID: 2862
	AchievementDescription_MexicoCity_Score_1_Achieved,
	// Token: 0x04000B2F RID: 2863
	AchievementDescription_DarEsSalaam_Score_1_Achieved,
	// Token: 0x04000B30 RID: 2864
	AchievementDescription_Moscow_Score_1_Achieved,
	// Token: 0x04000B31 RID: 2865
	AchievementDescription_Tokyo_Score_1_Achieved,
	// Token: 0x04000B32 RID: 2866
	AchievementDescription_Munich_Score_1_Achieved,
	// Token: 0x04000B33 RID: 2867
	AchievementDescription_Manila_Score_1_Achieved,
	// Token: 0x04000B34 RID: 2868
	AchievementDescription_Zurich_Score_1_Achieved,
	// Token: 0x04000B35 RID: 2869
	AchievementDescription_RioDeJaneiro_Score_1_Achieved,
	// Token: 0x04000B36 RID: 2870
	AchievementDescription_Dubai_Score_2_Achieved,
	// Token: 0x04000B37 RID: 2871
	AchievementDescription_MexicoCity_Score_2_Achieved,
	// Token: 0x04000B38 RID: 2872
	AchievementDescription_RioDeJaneiro_Score_2_Achieved,
	// Token: 0x04000B39 RID: 2873
	AchievementDescription_Manila_Score_2_Achieved,
	// Token: 0x04000B3A RID: 2874
	AchievementDescription_Zurich_Score_2_Achieved,
	// Token: 0x04000B3B RID: 2875
	AchievementDescription_Munich_Score_2_Achieved,
	// Token: 0x04000B3C RID: 2876
	AchievementDescription_Moscow_Score_2_Achieved,
	// Token: 0x04000B3D RID: 2877
	AchievementDescription_DarEsSalaam_Score_2_Achieved,
	// Token: 0x04000B3E RID: 2878
	AchievementDescription_Tokyo_Score_2_Achieved,
	// Token: 0x04000B3F RID: 2879
	AchievementDescription_Beijing_Score_2_Achieved,
	// Token: 0x04000B40 RID: 2880
	AchievementDescription_LosAngeles_Score_2_Achieved,
	// Token: 0x04000B41 RID: 2881
	Gif_Save_Directory_Steam,
	// Token: 0x04000B42 RID: 2882
	Moviemode_Failure,
	// Token: 0x04000B43 RID: 2883
	Photomode_Failure,
	// Token: 0x04000B44 RID: 2884
	Moviemode_Popup_Header,
	// Token: 0x04000B45 RID: 2885
	Moviemode_Popup_Header_Failure,
	// Token: 0x04000B46 RID: 2886
	Tutorial_ChooseTheMotorway,
	// Token: 0x04000B47 RID: 2887
	Tutorial_RoundaboutNoTripsHint,
	// Token: 0x04000B48 RID: 2888
	Tutorial_DragRoundabout,
	// Token: 0x04000B49 RID: 2889
	Tutorial_ChooseTheRoundabout,
	// Token: 0x04000B4A RID: 2890
	Options_Controls_Input_Menu,
	// Token: 0x04000B4B RID: 2891
	Options_Controls_Input_SelectController,
	// Token: 0x04000B4C RID: 2892
	Tutorial_PromptToDrawRoads_Touch,
	// Token: 0x04000B4D RID: 2893
	SteamRichPresence_City,
	// Token: 0x04000B4E RID: 2894
	SteamRichPresence_DailyChallenge,
	// Token: 0x04000B4F RID: 2895
	SteamRichPresence_WeeklyChallenge,
	// Token: 0x04000B50 RID: 2896
	PhotoGif_Save_Directory_Switch,
	// Token: 0x04000B51 RID: 2897
	Tutorial_Welcome_02,
	// Token: 0x04000B52 RID: 2898
	Tutorial_TapEnterBuildMode_Touch,
	// Token: 0x04000B53 RID: 2899
	Tutorial_PromptToDrawRoad_Touch,
	// Token: 0x04000B54 RID: 2900
	Tutorial_PromptToDrawRoad_Mouse,
	// Token: 0x04000B55 RID: 2901
	Tutorial_PromptToFinishDrawRoad_Controller,
	// Token: 0x04000B56 RID: 2902
	Tutorial_PromptToStartDrawRoad_Remote,
	// Token: 0x04000B57 RID: 2903
	Tutorial_PromptToFinishDrawRoad_Remote,
	// Token: 0x04000B58 RID: 2904
	Tutorial_PromptToDeleteRoad_Touch,
	// Token: 0x04000B59 RID: 2905
	Tutorial_PromptToDeleteRoad_Mouse,
	// Token: 0x04000B5A RID: 2906
	Tutorial_PromptToDeleteRoad_MouseToggle,
	// Token: 0x04000B5B RID: 2907
	Tutorial_PromptToDeleteRoad_Controller,
	// Token: 0x04000B5C RID: 2908
	Tutorial_PromptToDeleteRoad_Remote,
	// Token: 0x04000B5D RID: 2909
	Tutorial_TapExitBuildMode_Touch,
	// Token: 0x04000B5E RID: 2910
	Tutorial_TapExitBuildMode_MouseToggle,
	// Token: 0x04000B5F RID: 2911
	Tutorial_TapExitBuildMode_Controller,
	// Token: 0x04000B60 RID: 2912
	Tutorial_TapExitBuildMode_Remote,
	// Token: 0x04000B61 RID: 2913
	Tutorial_HouseLabel,
	// Token: 0x04000B62 RID: 2914
	Tutorial_DestinationLabel,
	// Token: 0x04000B63 RID: 2915
	Tutorial_ConnectRoad_Touch,
	// Token: 0x04000B64 RID: 2916
	Tutorial_ConnectRoad_Mouse,
	// Token: 0x04000B65 RID: 2917
	Tutorial_ConnectRoad_Controller,
	// Token: 0x04000B66 RID: 2918
	Tutorial_ConnectRoad_Remote,
	// Token: 0x04000B67 RID: 2919
	Tutorial_DemandIntroduction_02,
	// Token: 0x04000B68 RID: 2920
	Tutorial_Error_EarlyDeleteMode_Touch_MouseToggle,
	// Token: 0x04000B69 RID: 2921
	Tutorial_Error_EarlyDeleteMode_Mouse,
	// Token: 0x04000B6A RID: 2922
	Tutorial_Error_EarlyDeleteMode_Controller,
	// Token: 0x04000B6B RID: 2923
	Tutorial_Error_EarlyDeleteMode_Remote,
	// Token: 0x04000B6C RID: 2924
	Tutorial_ReorientHouse,
	// Token: 0x04000B6D RID: 2925
	Tutorial_ExplainEndOfWeek,
	// Token: 0x04000B6E RID: 2926
	Tutorial_TrafficLight_02,
	// Token: 0x04000B6F RID: 2927
	Tutorial_TouchTwoFingerPan_02,
	// Token: 0x04000B70 RID: 2928
	Tutorial_Motorway_PlaceStart,
	// Token: 0x04000B71 RID: 2929
	Tutorial_Motorway_PlaceEnd,
	// Token: 0x04000B72 RID: 2930
	Tutorial_Motorway_Roads,
	// Token: 0x04000B73 RID: 2931
	Tutorial_OvercrowdingTwo_02,
	// Token: 0x04000B74 RID: 2932
	Tutorial_OvercrowdingFour,
	// Token: 0x04000B75 RID: 2933
	Tutorial_OvercrowdingThree_02,
	// Token: 0x04000B76 RID: 2934
	Tutorial_ClockIntroduction_Mouse,
	// Token: 0x04000B77 RID: 2935
	Tutorial_ClockIntroduction_Controller,
	// Token: 0x04000B78 RID: 2936
	Tutorial_ClockIntroduction_Remote,
	// Token: 0x04000B79 RID: 2937
	Tutorial_Completed,
	// Token: 0x04000B7A RID: 2938
	GameOver_TutorialLate_LineThree,
	// Token: 0x04000B7B RID: 2939
	GameOver_Tutorial_MenuButton,
	// Token: 0x04000B7C RID: 2940
	Tutorial_Error_DeleteRoads_Touch_MouseToggle,
	// Token: 0x04000B7D RID: 2941
	Tutorial_Error_DeleteRoads_Mouse,
	// Token: 0x04000B7E RID: 2942
	Tutorial_Error_DeleteRoads_Controller,
	// Token: 0x04000B7F RID: 2943
	Tutorial_Error_DeleteRoads_Remote,
	// Token: 0x04000B80 RID: 2944
	Tutorial_Error_UnconnectedHouses,
	// Token: 0x04000B81 RID: 2945
	AchievementDescription_DarEsSalaam_Score1_02,
	// Token: 0x04000B82 RID: 2946
	AchievementDescription_DarEsSalaam_Score1_02_Achieved,
	// Token: 0x04000B83 RID: 2947
	Tutorial_PromptToStartDrawRoad_Controller,
	// Token: 0x04000B84 RID: 2948
	Tutorial_ScoretoComplete,
	// Token: 0x04000B85 RID: 2949
	WeeklyChallengeDateDuration,
	// Token: 0x04000B86 RID: 2950
	SaveGameOverwriteConfirmation,
	// Token: 0x04000B87 RID: 2951
	StartNewGameHeader,
	// Token: 0x04000B88 RID: 2952
	Error_MotorwaysDLL_Title,
	// Token: 0x04000B89 RID: 2953
	Error_MotorwaysDLL_Description,
	// Token: 0x04000B8A RID: 2954
	Options_Controls_Input_Ctrl,
	// Token: 0x04000B8B RID: 2955
	DeleteSpecificJournalPrompt_Steam,
	// Token: 0x04000B8C RID: 2956
	WellingtonDescriptionID,
	// Token: 0x04000B8D RID: 2957
	Wellington,
	// Token: 0x04000B8E RID: 2958
	AchievementDescription_Wellington_Score_2_Achieved,
	// Token: 0x04000B8F RID: 2959
	AchievementDescription_Wellington_Score_1_Achieved,
	// Token: 0x04000B90 RID: 2960
	AchievementDescription_Wellington_Score_3,
	// Token: 0x04000B91 RID: 2961
	AchievementDescription_Wellington_Score_2,
	// Token: 0x04000B92 RID: 2962
	AchievementDescription_Wellington_Score_1,
	// Token: 0x04000B93 RID: 2963
	AchievementDescription_Wellington_Score_3_Title,
	// Token: 0x04000B94 RID: 2964
	AchievementDescription_Wellington_Score_2_Title,
	// Token: 0x04000B95 RID: 2965
	AchievementDescription_Wellington_Score_1_Title,
	// Token: 0x04000B96 RID: 2966
	Customise,
	// Token: 0x04000B97 RID: 2967
	Save_Colors,
	// Token: 0x04000B98 RID: 2968
	Colorblind_Popup_Description,
	// Token: 0x04000B99 RID: 2969
	NewControllerScheme_Description,
	// Token: 0x04000B9A RID: 2970
	NewControllerScheme_Title,
	// Token: 0x04000B9B RID: 2971
	Challenge_Upgrades_NoRoundaboutsDescription,
	// Token: 0x04000B9C RID: 2972
	Challenge_Upgrades_NoRoundaboutsTitle,
	// Token: 0x04000B9D RID: 2973
	Challenge_Upgrades_CostDoubleBridgeDescription,
	// Token: 0x04000B9E RID: 2974
	Challenge_Upgrades_CostDoubleBridgeTitle,
	// Token: 0x04000B9F RID: 2975
	Challenge_Upgrades_CostDoubleTunnelDescription,
	// Token: 0x04000BA0 RID: 2976
	Challenge_Upgrades_CostDoubleTunnelTitle,
	// Token: 0x04000BA1 RID: 2977
	Challenge_Upgrades_CostFreeTunnelDescription,
	// Token: 0x04000BA2 RID: 2978
	Challenge_Upgrades_CostFreeTunnelTitle,
	// Token: 0x04000BA3 RID: 2979
	Challenge_Upgrades_CostFreeBridgeTunnelDescription,
	// Token: 0x04000BA4 RID: 2980
	Challenge_Upgrades_CostFreeBridgeTunnelTitle,
	// Token: 0x04000BA5 RID: 2981
	Challenge_Upgrades_UnlimitedBridgeTunnelDescription,
	// Token: 0x04000BA6 RID: 2982
	Challenge_Upgrades_UnlimitedBridgeTunnelTitle,
	// Token: 0x04000BA7 RID: 2983
	Challenge_Upgrades_CostMotorwaysDescription,
	// Token: 0x04000BA8 RID: 2984
	Challenge_Upgrades_CostMotorwaysTitle,
	// Token: 0x04000BA9 RID: 2985
	Challenge_Upgrades_CostDoubleDiagonalRoadsTitle,
	// Token: 0x04000BAA RID: 2986
	Challenge_Upgrades_CostDoubleStraightRoadsDescription,
	// Token: 0x04000BAB RID: 2987
	Challenge_Upgrades_CostDoubleStraightRoadsTitle,
	// Token: 0x04000BAC RID: 2988
	Challenge_Upgrades_CostDoubleDiagonalRoadsDescription,
	// Token: 0x04000BAD RID: 2989
	Challenge_Destinations_DemandIncreaseAllDescription,
	// Token: 0x04000BAE RID: 2990
	Challenge_Destinations_DemandIncreaseAllTitle,
	// Token: 0x04000BAF RID: 2991
	Challenge_Upgrades_CostFreeBridgeDescription,
	// Token: 0x04000BB0 RID: 2992
	Challenge_Upgrades_CostFreeBridgeTitle,
	// Token: 0x04000BB1 RID: 2993
	CityChallenge_CardTitle,
	// Token: 0x04000BB2 RID: 2994
	CityChallenge_SelectChallenge,
	// Token: 0x04000BB3 RID: 2995
	CityChallenge_UnlockChallenge,
	// Token: 0x04000BB4 RID: 2996
	CityChallenge_InfoPopup_Title,
	// Token: 0x04000BB5 RID: 2997
	CityChallenge_InfoPopup_Body,
	// Token: 0x04000BB6 RID: 2998
	CityChallenge_LosAngeles_1,
	// Token: 0x04000BB7 RID: 2999
	CityChallenge_LosAngeles_2,
	// Token: 0x04000BB8 RID: 3000
	CityChallenge_LosAngeles_3,
	// Token: 0x04000BB9 RID: 3001
	CityChallenge_Beijing_1,
	// Token: 0x04000BBA RID: 3002
	CityChallenge_Beijing_2,
	// Token: 0x04000BBB RID: 3003
	CityChallenge_Tokyo_1,
	// Token: 0x04000BBC RID: 3004
	CityChallenge_Tokyo_2,
	// Token: 0x04000BBD RID: 3005
	CityChallenge_DarEsSalaam_1,
	// Token: 0x04000BBE RID: 3006
	CityChallenge_Moscow_1,
	// Token: 0x04000BBF RID: 3007
	CityChallenge_Moscow_2,
	// Token: 0x04000BC0 RID: 3008
	CityChallenge_Munich_1,
	// Token: 0x04000BC1 RID: 3009
	CityChallenge_Munich_2,
	// Token: 0x04000BC2 RID: 3010
	CityChallenge_Zurich_1,
	// Token: 0x04000BC3 RID: 3011
	CityChallenge_Zurich_2,
	// Token: 0x04000BC4 RID: 3012
	CityChallenge_Manila_1,
	// Token: 0x04000BC5 RID: 3013
	CityChallenge_Manila_2,
	// Token: 0x04000BC6 RID: 3014
	CityChallenge_RioDeJaneiro_1,
	// Token: 0x04000BC7 RID: 3015
	CityChallenge_RioDeJaneiro_2,
	// Token: 0x04000BC8 RID: 3016
	CityChallenge_Dubai_1,
	// Token: 0x04000BC9 RID: 3017
	CityChallenge_Dubai_2,
	// Token: 0x04000BCA RID: 3018
	CityChallenge_MexicoCity_1,
	// Token: 0x04000BCB RID: 3019
	CityChallenge_Wellington_1,
	// Token: 0x04000BCC RID: 3020
	AchievementDescription_Zurich_Challenge_Score_Title,
	// Token: 0x04000BCD RID: 3021
	AchievementDescription_Wellington_Challenge_Score_Title,
	// Token: 0x04000BCE RID: 3022
	AchievementDescription_Tokyo_Challenge_Score_Title,
	// Token: 0x04000BCF RID: 3023
	AchievementDescription_RioDeJaneiro_Challenge_Score_Title,
	// Token: 0x04000BD0 RID: 3024
	AchievementDescription_Munich_Challenge_Score_Title,
	// Token: 0x04000BD1 RID: 3025
	AchievementDescription_Moscow_Challenge_Score_Title,
	// Token: 0x04000BD2 RID: 3026
	AchievementDescription_Mexico_Challenge_Score_Title,
	// Token: 0x04000BD3 RID: 3027
	AchievementDescription_Manila_Challenge_Score_Title,
	// Token: 0x04000BD4 RID: 3028
	AchievementDescription_LosAngeles_Challenge_Score_Title,
	// Token: 0x04000BD5 RID: 3029
	AchievementDescription_Dubai_Challenge_Score_Title,
	// Token: 0x04000BD6 RID: 3030
	AchievementDescription_DarEsSalaam_Challenge_Score_Title,
	// Token: 0x04000BD7 RID: 3031
	AchievementDescription_Beijing_Challenge_Score_Title,
	// Token: 0x04000BD8 RID: 3032
	AchievementDescription_Zurich_Challenge_Score,
	// Token: 0x04000BD9 RID: 3033
	AchievementDescription_Wellington_Challenge_Score,
	// Token: 0x04000BDA RID: 3034
	AchievementDescription_Tokyo_Challenge_Score,
	// Token: 0x04000BDB RID: 3035
	AchievementDescription_RioDeJaneiro_Challenge_Score,
	// Token: 0x04000BDC RID: 3036
	AchievementDescription_Munich_Challenge_Score,
	// Token: 0x04000BDD RID: 3037
	AchievementDescription_Moscow_Challenge_Score,
	// Token: 0x04000BDE RID: 3038
	AchievementDescription_Mexico_Challenge_Score,
	// Token: 0x04000BDF RID: 3039
	AchievementDescription_Manila_Challenge_Score,
	// Token: 0x04000BE0 RID: 3040
	AchievementDescription_LosAngeles_Challenge_Score,
	// Token: 0x04000BE1 RID: 3041
	AchievementDescription_Dubai_Challenge_Score,
	// Token: 0x04000BE2 RID: 3042
	AchievementDescription_DarEsSalaam_Challenge_Score,
	// Token: 0x04000BE3 RID: 3043
	AchievementDescription_Beijing_Challenge_Score,
	// Token: 0x04000BE4 RID: 3044
	AchievementDescription_Zurich_Challenge_Score_Achieved,
	// Token: 0x04000BE5 RID: 3045
	AchievementDescription_Wellington_Challenge_Score_Achieved,
	// Token: 0x04000BE6 RID: 3046
	AchievementDescription_Tokyo_Challenge_Score_Achieved,
	// Token: 0x04000BE7 RID: 3047
	AchievementDescription_RioDeJaneiro_Challenge_Score_Achieved,
	// Token: 0x04000BE8 RID: 3048
	AchievementDescription_Munich_Challenge_Score_Achieved,
	// Token: 0x04000BE9 RID: 3049
	AchievementDescription_Moscow_Challenge_Score_Achieved,
	// Token: 0x04000BEA RID: 3050
	AchievementDescription_Mexico_Challenge_Score_Achieved,
	// Token: 0x04000BEB RID: 3051
	AchievementDescription_Manila_Challenge_Score_Achieved,
	// Token: 0x04000BEC RID: 3052
	AchievementDescription_LosAngeles_Challenge_Score_Achieved,
	// Token: 0x04000BED RID: 3053
	AchievementDescription_Dubai_Challenge_Score_Achieved,
	// Token: 0x04000BEE RID: 3054
	AchievementDescription_DarEsSalaam_Challenge_Score_Achieved,
	// Token: 0x04000BEF RID: 3055
	AchievementDescription_Beijing_Challenge_Score_Achieved,
	// Token: 0x04000BF0 RID: 3056
	Telemetry,
	// Token: 0x04000BF1 RID: 3057
	Options_Controls_Input_SelectDraw,
	// Token: 0x04000BF2 RID: 3058
	Options_Controls_Input_Gamepad_BackDelete,
	// Token: 0x04000BF3 RID: 3059
	Options_Controls_Input_PauseMenu,
	// Token: 0x04000BF4 RID: 3060
	Options_Controls_Input_ToggleZoom,
	// Token: 0x04000BF5 RID: 3061
	Options_Controls_Input_MoveCamera,
	// Token: 0x04000BF6 RID: 3062
	Options_Controls_Input_QuickAccessUpgrades,
	// Token: 0x04000BF7 RID: 3063
	FTUX_Accessibility_DrawDeleteHoldOrTapDescription,
	// Token: 0x04000BF8 RID: 3064
	TapDrawToggle,
	// Token: 0x04000BF9 RID: 3065
	Tutorial_Error_DeleteRoads_ControllerTap,
	// Token: 0x04000BFA RID: 3066
	Tutorial_Error_EarlyDeleteMode_ControllerTap,
	// Token: 0x04000BFB RID: 3067
	Normal,
	// Token: 0x04000BFC RID: 3068
	NewColorblindPicker_Title,
	// Token: 0x04000BFD RID: 3069
	NewColorblindPicker_Description,
	// Token: 0x04000BFE RID: 3070
	Tutorial_PromptToStartDrawRoad_ControllerTap,
	// Token: 0x04000BFF RID: 3071
	Tutorial_PromptToDeleteRoad_ControllerTap,
	// Token: 0x04000C00 RID: 3072
	Tutorial_ConnectRoad_ControllerTap,
	// Token: 0x04000C01 RID: 3073
	LeaderboardFilter_Surrounding,
	// Token: 0x04000C02 RID: 3074
	LeaderboardFilter_Friends,
	// Token: 0x04000C03 RID: 3075
	LeaderboardFilter_Global,
	// Token: 0x04000C04 RID: 3076
	LeaderboardFilter_Histogram,
	// Token: 0x04000C05 RID: 3077
	Leaderboard_SignIn,
	// Token: 0x04000C06 RID: 3078
	Options_CrossSave,
	// Token: 0x04000C07 RID: 3079
	CrossSave_Importer_Loading,
	// Token: 0x04000C08 RID: 3080
	CrossSave_Importer_Header,
	// Token: 0x04000C09 RID: 3081
	CrossSave_ImportSuccessful,
	// Token: 0x04000C0A RID: 3082
	CrossSave_ImportSteamData,
	// Token: 0x04000C0B RID: 3083
	CrossSave_Explanation_1,
	// Token: 0x04000C0C RID: 3084
	CrossSave_Explanation_2,
	// Token: 0x04000C0D RID: 3085
	CrossSave_Error_DataImportFail,
	// Token: 0x04000C0E RID: 3086
	CrossSave_Error_DataDownloadFail,
	// Token: 0x04000C0F RID: 3087
	CrossSave_Error_NoSteamData,
	// Token: 0x04000C10 RID: 3088
	CrossSave_Error_SteamLinkCancel,
	// Token: 0x04000C11 RID: 3089
	CrossSave_Error_SteamLinkFail,
	// Token: 0x04000C12 RID: 3090
	CrossSave_Error_NoConnection,
	// Token: 0x04000C13 RID: 3091
	ChiangMai,
	// Token: 0x04000C14 RID: 3092
	ChiangMaiDescriptionID,
	// Token: 0x04000C15 RID: 3093
	CityChallenge_ChiangMai_1,
	// Token: 0x04000C16 RID: 3094
	CityChallenge_ChiangMai_2,
	// Token: 0x04000C17 RID: 3095
	AchievementDescription_ChiangMai_Challenge_Score_Title,
	// Token: 0x04000C18 RID: 3096
	AchievementDescription_ChiangMai_Challenge_Score,
	// Token: 0x04000C19 RID: 3097
	AchievementDescription_ChiangMai_Challenge_Score_Achieved,
	// Token: 0x04000C1A RID: 3098
	AchievementDescription_ChiangMai_Score_3_Achieved,
	// Token: 0x04000C1B RID: 3099
	AchievementDescription_ChiangMai_Score_3,
	// Token: 0x04000C1C RID: 3100
	AchievementDescription_ChiangMai_Score_3_Title,
	// Token: 0x04000C1D RID: 3101
	AchievementDescription_ChiangMai_Score_2_Achieved,
	// Token: 0x04000C1E RID: 3102
	AchievementDescription_ChiangMai_Score_2,
	// Token: 0x04000C1F RID: 3103
	AchievementDescription_ChiangMai_Score_2_Title,
	// Token: 0x04000C20 RID: 3104
	AchievementDescription_ChiangMai_Score_1_Achieved,
	// Token: 0x04000C21 RID: 3105
	AchievementDescription_ChiangMai_Score_1,
	// Token: 0x04000C22 RID: 3106
	AchievementDescription_ChiangMai_Score_1_Title,
	// Token: 0x04000C23 RID: 3107
	WarsawDescription,
	// Token: 0x04000C24 RID: 3108
	Warsaw,
	// Token: 0x04000C25 RID: 3109
	AchievementDescription_Warsaw_Challenge_Score_Achieved,
	// Token: 0x04000C26 RID: 3110
	AchievementDescription_Warsaw_Score_2_Achieved,
	// Token: 0x04000C27 RID: 3111
	AchievementDescription_Warsaw_Score_1_Achieved,
	// Token: 0x04000C28 RID: 3112
	AchievementDescription_Warsaw_Challenge_Score,
	// Token: 0x04000C29 RID: 3113
	AchievementDescription_Warsaw_Score_3,
	// Token: 0x04000C2A RID: 3114
	AchievementDescription_Warsaw_Score_2,
	// Token: 0x04000C2B RID: 3115
	AchievementDescription_Warsaw_Score_1,
	// Token: 0x04000C2C RID: 3116
	AchievementDescription_Warsaw_Challenge_Score_Title,
	// Token: 0x04000C2D RID: 3117
	AchievementDescription_Warsaw_Score_3_Title,
	// Token: 0x04000C2E RID: 3118
	AchievementDescription_Warsaw_Score_2_Title,
	// Token: 0x04000C2F RID: 3119
	AchievementDescription_Warsaw_Score_1_Title,
	// Token: 0x04000C30 RID: 3120
	CityChallenge_Warsaw_1,
	// Token: 0x04000C31 RID: 3121
	CityChallenge_Warsaw_2,
	// Token: 0x04000C32 RID: 3122
	ZoomLevel4,
	// Token: 0x04000C33 RID: 3123
	ZoomLevel3,
	// Token: 0x04000C34 RID: 3124
	ZoomLevel2,
	// Token: 0x04000C35 RID: 3125
	ZoomLevel1,
	// Token: 0x04000C36 RID: 3126
	ZoomLevel,
	// Token: 0x04000C37 RID: 3127
	ZoomEnabled,
	// Token: 0x04000C38 RID: 3128
	Options_ControllerCursorSpeed_Fastest,
	// Token: 0x04000C39 RID: 3129
	Options_ControllerCursorSpeed_Faster,
	// Token: 0x04000C3A RID: 3130
	Options_ControllerCursorSpeed_Default,
	// Token: 0x04000C3B RID: 3131
	Options_ControllerCursorSpeed_Slower,
	// Token: 0x04000C3C RID: 3132
	Options_ControllerCursorSpeed_Slowest,
	// Token: 0x04000C3D RID: 3133
	Options_ControllerCursorSpeed_Title,
	// Token: 0x04000C3E RID: 3134
	Options_Help,
	// Token: 0x04000C3F RID: 3135
	Credits_OperationsCoordinator,
	// Token: 0x04000C40 RID: 3136
	TapDrawToggleNoIcon,
	// Token: 0x04000C41 RID: 3137
	Leaderboard_Connect,
	// Token: 0x04000C42 RID: 3138
	InGame_Messages_1OffNightlightsUpdate,
	// Token: 0x04000C43 RID: 3139
	Credits_ArtDirection,
	// Token: 0x04000C44 RID: 3140
	Mode,
	// Token: 0x04000C45 RID: 3141
	Endless_Mode_Name,
	// Token: 0x04000C46 RID: 3142
	Expert_Mode_Name,
	// Token: 0x04000C47 RID: 3143
	Creative_Mode_Name,
	// Token: 0x04000C48 RID: 3144
	ExpertMode_Unlock_Info,
	// Token: 0x04000C49 RID: 3145
	To_Unlock,
	// Token: 0x04000C4A RID: 3146
	Endless,
	// Token: 0x04000C4B RID: 3147
	Expert,
	// Token: 0x04000C4C RID: 3148
	Creative,
	// Token: 0x04000C4D RID: 3149
	Menu,
	// Token: 0x04000C4E RID: 3150
	ModeInfoPopup_Expert2_Body,
	// Token: 0x04000C4F RID: 3151
	ModeInfoPopup_Expert2_Title,
	// Token: 0x04000C50 RID: 3152
	ModeInfoPopup_Expert1_Body,
	// Token: 0x04000C51 RID: 3153
	ModeInfoPopup_Expert1_Title,
	// Token: 0x04000C52 RID: 3154
	ModeInfoPopup_Endless2_Body,
	// Token: 0x04000C53 RID: 3155
	ModeInfoPopup_Endless2_Title,
	// Token: 0x04000C54 RID: 3156
	ModeInfoPopup_Endless1_Body,
	// Token: 0x04000C55 RID: 3157
	ModeInfoPopup_Endless1_Title,
	// Token: 0x04000C56 RID: 3158
	ContinueInEndless,
	// Token: 0x04000C57 RID: 3159
	ZoomLevel5,
	// Token: 0x04000C58 RID: 3160
	Credits_ConceptArt,
	// Token: 0x04000C59 RID: 3161
	Options_AudioVideo,
	// Token: 0x04000C5A RID: 3162
	Options_Display,
	// Token: 0x04000C5B RID: 3163
	Options_Privacy,
	// Token: 0x04000C5C RID: 3164
	Options_Controls_Input_Pan,
	// Token: 0x04000C5D RID: 3165
	LisbonDescriptionID,
	// Token: 0x04000C5E RID: 3166
	LisbonNameID,
	// Token: 0x04000C5F RID: 3167
	AchievementDescription_Lisbon_Challenge_Score_Title,
	// Token: 0x04000C60 RID: 3168
	AchievementDescription_Lisbon_Score_3_Title,
	// Token: 0x04000C61 RID: 3169
	AchievementDescription_Lisbon_Score_2_Title,
	// Token: 0x04000C62 RID: 3170
	AchievementDescription_Lisbon_Score_1_Title,
	// Token: 0x04000C63 RID: 3171
	AchievementDescription_Lisbon_Challenge_Score,
	// Token: 0x04000C64 RID: 3172
	AchievementDescription_Lisbon_Score_3,
	// Token: 0x04000C65 RID: 3173
	AchievementDescription_Lisbon_Score_2,
	// Token: 0x04000C66 RID: 3174
	AchievementDescription_Lisbon_Score_1,
	// Token: 0x04000C67 RID: 3175
	AchievementDescription_Lisbon_Challenge_Score_Achieved,
	// Token: 0x04000C68 RID: 3176
	AchievementDescription_Lisbon_Score_2_Achieved,
	// Token: 0x04000C69 RID: 3177
	AchievementDescription_Lisbon_Score_1_Achieved,
	// Token: 0x04000C6A RID: 3178
	CityChallenge_Lisbon_1,
	// Token: 0x04000C6B RID: 3179
	Challenge_ExpertDescription,
	// Token: 0x04000C6C RID: 3180
	Challenge_ExpertTitle,
	// Token: 0x04000C6D RID: 3181
	AchievementDescription_MoscowOrMunich_Score_1,
	// Token: 0x04000C6E RID: 3182
	AchievementDescription_Endless_Milestones1,
	// Token: 0x04000C6F RID: 3183
	Credits_Marketing,
	// Token: 0x04000C70 RID: 3184
	MilestoneCount,
	// Token: 0x04000C71 RID: 3185
	Restart_Endless,
	// Token: 0x04000C72 RID: 3186
	Restart_Classic,
	// Token: 0x04000C73 RID: 3187
	Restart_Expert,
	// Token: 0x04000C74 RID: 3188
	Error_NoDeletableRoads,
	// Token: 0x04000C75 RID: 3189
	AchievementDescription_Endless_Milestones1_Achieved,
	// Token: 0x04000C76 RID: 3190
	AchievementDescription_Endless_Milestones1_Title,
	// Token: 0x04000C77 RID: 3191
	AchievementDescription_Expert_Score100_Achieved,
	// Token: 0x04000C78 RID: 3192
	AchievementDescription_Expert_Score100,
	// Token: 0x04000C79 RID: 3193
	AchievementDescription_Expert_Score100_Title,
	// Token: 0x04000C7A RID: 3194
	AchievementDescription_LosAngeles_Expert_Score,
	// Token: 0x04000C7B RID: 3195
	AchievementDescription_LosAngeles_Expert_Score_Title,
	// Token: 0x04000C7C RID: 3196
	AchievementDescription_Beijing_Expert_Score,
	// Token: 0x04000C7D RID: 3197
	AchievementDescription_Beijing_Expert_Score_Title,
	// Token: 0x04000C7E RID: 3198
	AchievementDescription_Tokyo_Expert_Score,
	// Token: 0x04000C7F RID: 3199
	AchievementDescription_Tokyo_Expert_Score_Title,
	// Token: 0x04000C80 RID: 3200
	AchievementDescription_DarEsSalaam_Expert_Score,
	// Token: 0x04000C81 RID: 3201
	AchievementDescription_DarEsSalaam_Expert_Score_Title,
	// Token: 0x04000C82 RID: 3202
	AchievementDescription_Moscow_Expert_Score,
	// Token: 0x04000C83 RID: 3203
	AchievementDescription_Moscow_Expert_Score_Title,
	// Token: 0x04000C84 RID: 3204
	AchievementDescription_Munich_Expert_Score,
	// Token: 0x04000C85 RID: 3205
	AchievementDescription_Munich_Expert_Score_Title,
	// Token: 0x04000C86 RID: 3206
	AchievementDescription_Zurich_Expert_Score,
	// Token: 0x04000C87 RID: 3207
	AchievementDescription_Zurich_Expert_Score_Title,
	// Token: 0x04000C88 RID: 3208
	AchievementDescription_Manila_Expert_Score,
	// Token: 0x04000C89 RID: 3209
	AchievementDescription_Manila_Expert_Score_Title,
	// Token: 0x04000C8A RID: 3210
	AchievementDescription_RioDeJaneiro_Expert_Score,
	// Token: 0x04000C8B RID: 3211
	AchievementDescription_RioDeJaneiro_Expert_Score_Title,
	// Token: 0x04000C8C RID: 3212
	AchievementDescription_Dubai_Expert_Score,
	// Token: 0x04000C8D RID: 3213
	AchievementDescription_Dubai_Expert_Score_Title,
	// Token: 0x04000C8E RID: 3214
	AchievementDescription_MexicoCity_Expert_Score,
	// Token: 0x04000C8F RID: 3215
	AchievementDescription_MexicoCity_Expert_Score_Title,
	// Token: 0x04000C90 RID: 3216
	AchievementDescription_Wellington_Expert_Score,
	// Token: 0x04000C91 RID: 3217
	AchievementDescription_Wellington_Expert_Score_Title,
	// Token: 0x04000C92 RID: 3218
	AchievementDescription_Warsaw_Expert_Score,
	// Token: 0x04000C93 RID: 3219
	AchievementDescription_Warsaw_Expert_Score_Title,
	// Token: 0x04000C94 RID: 3220
	AchievementDescription_ChiangMai_Expert_Score,
	// Token: 0x04000C95 RID: 3221
	AchievementDescription_ChiangMai_Expert_Score_Title,
	// Token: 0x04000C96 RID: 3222
	AchievementDescription_Lisbon_Expert_Score,
	// Token: 0x04000C97 RID: 3223
	AchievementDescription_Lisbon_Expert_Score_Title,
	// Token: 0x04000C98 RID: 3224
	WeeksRemaining,
	// Token: 0x04000C99 RID: 3225
	Mode_Change,
	// Token: 0x04000C9A RID: 3226
	WeeksRemainingNone_Body,
	// Token: 0x04000C9B RID: 3227
	WeeksRemainingNone,
	// Token: 0x04000C9C RID: 3228
	FTUX_Endless,
	// Token: 0x04000C9D RID: 3229
	InGame_Messages_EndlessExpert,
	// Token: 0x04000C9E RID: 3230
	Error_NoDeletableUpgrades,
	// Token: 0x04000C9F RID: 3231
	Replay_Challenge,
	// Token: 0x04000CA0 RID: 3232
	Restart_Challenge,
	// Token: 0x04000CA1 RID: 3233
	Error_PermanentObject,
	// Token: 0x04000CA2 RID: 3234
	Challenge_Upgrades_SpeedDecreaseBridgeDescription,
	// Token: 0x04000CA3 RID: 3235
	Challenge_Upgrades_SpeedDecreaseBridgeTitle,
	// Token: 0x04000CA4 RID: 3236
	Challenge_Upgrades_SpeedDecreaseTunnelDescription,
	// Token: 0x04000CA5 RID: 3237
	Challenge_Upgrades_SpeedDecreaseTunnelTitle,
	// Token: 0x04000CA6 RID: 3238
	Challenge_Upgrades_SpeedIncreaseBridgeDescription,
	// Token: 0x04000CA7 RID: 3239
	Challenge_Upgrades_SpeedIncreaseBridgeTitle,
	// Token: 0x04000CA8 RID: 3240
	Challenge_Upgrades_SpeedIncreaseTunnelDescription,
	// Token: 0x04000CA9 RID: 3241
	Challenge_Upgrades_SpeedIncreaseTunnelTitle,
	// Token: 0x04000CAA RID: 3242
	BusanDescription,
	// Token: 0x04000CAB RID: 3243
	Busan,
	// Token: 0x04000CAC RID: 3244
	AchievementDescription_Busan_Score_1,
	// Token: 0x04000CAD RID: 3245
	AchievementDescription_Busan_Score_2,
	// Token: 0x04000CAE RID: 3246
	AchievementDescription_Busan_Score_3,
	// Token: 0x04000CAF RID: 3247
	CityChallenge_Busan_1,
	// Token: 0x04000CB0 RID: 3248
	CityChallenge_Busan_2,
	// Token: 0x04000CB1 RID: 3249
	AchievementDescription_Busan_Challenge_Score,
	// Token: 0x04000CB2 RID: 3250
	AchievementDescription_Busan_Challenge_Score_Achieved,
	// Token: 0x04000CB3 RID: 3251
	AchievementDescription_Busan_Score_2_Achieved,
	// Token: 0x04000CB4 RID: 3252
	AchievementDescription_Busan_Score_1_Achieved,
	// Token: 0x04000CB5 RID: 3253
	AchievementDescription_Busan_Challenge_Score_Title,
	// Token: 0x04000CB6 RID: 3254
	AchievementDescription_Busan_Score_3_Title,
	// Token: 0x04000CB7 RID: 3255
	AchievementDescription_Busan_Score_2_Title,
	// Token: 0x04000CB8 RID: 3256
	AchievementDescription_Busan_Score_1_Title,
	// Token: 0x04000CB9 RID: 3257
	AchievementDescription_Busan_Expert_Score,
	// Token: 0x04000CBA RID: 3258
	AchievementDescription_Busan_Expert_Score_Title,
	// Token: 0x04000CBB RID: 3259
	London,
	// Token: 0x04000CBC RID: 3260
	LondonDescription,
	// Token: 0x04000CBD RID: 3261
	AchievementDescription_London_Score_1,
	// Token: 0x04000CBE RID: 3262
	AchievementDescription_London_Score_2,
	// Token: 0x04000CBF RID: 3263
	AchievementDescription_London_Score_3,
	// Token: 0x04000CC0 RID: 3264
	CityChallenge_London_1,
	// Token: 0x04000CC1 RID: 3265
	AchievementDescription_London_Challenge_Score,
	// Token: 0x04000CC2 RID: 3266
	AchievementDescription_London_Score_1_Achieved,
	// Token: 0x04000CC3 RID: 3267
	AchievementDescription_London_Score_2_Achieved,
	// Token: 0x04000CC4 RID: 3268
	AchievementDescription_London_Challenge_Score_Achieved,
	// Token: 0x04000CC5 RID: 3269
	AchievementDescription_London_Score_1_Title,
	// Token: 0x04000CC6 RID: 3270
	AchievementDescription_London_Score_2_Title,
	// Token: 0x04000CC7 RID: 3271
	AchievementDescription_London_Score_3_Title,
	// Token: 0x04000CC8 RID: 3272
	AchievementDescription_London_Challenge_Score_Title,
	// Token: 0x04000CC9 RID: 3273
	AchievementDescription_London_Expert_Score_Title,
	// Token: 0x04000CCA RID: 3274
	AchievementDescription_London_Expert_Score,
	// Token: 0x04000CCB RID: 3275
	Mumbai,
	// Token: 0x04000CCC RID: 3276
	MumbaiDescription,
	// Token: 0x04000CCD RID: 3277
	AchievementDescription_Mumbai_Score_1,
	// Token: 0x04000CCE RID: 3278
	AchievementDescription_Mumbai_Score_2,
	// Token: 0x04000CCF RID: 3279
	AchievementDescription_Mumbai_Score_3,
	// Token: 0x04000CD0 RID: 3280
	CityChallenge_Mumbai_1,
	// Token: 0x04000CD1 RID: 3281
	AchievementDescription_Mumbai_Challenge_Score,
	// Token: 0x04000CD2 RID: 3282
	AchievementDescription_Mumbai_Score_1_Achieved,
	// Token: 0x04000CD3 RID: 3283
	AchievementDescription_Mumbai_Score_2_Achieved,
	// Token: 0x04000CD4 RID: 3284
	AchievementDescription_Mumbai_Challenge_Score_Achieved,
	// Token: 0x04000CD5 RID: 3285
	AchievementDescription_Mumbai_Score_1_Title,
	// Token: 0x04000CD6 RID: 3286
	AchievementDescription_Mumbai_Score_2_Title,
	// Token: 0x04000CD7 RID: 3287
	AchievementDescription_Mumbai_Score_3_Title,
	// Token: 0x04000CD8 RID: 3288
	AchievementDescription_Mumbai_Challenge_Score_Title,
	// Token: 0x04000CD9 RID: 3289
	AchievementDescription_Mumbai_Expert_Score_Title,
	// Token: 0x04000CDA RID: 3290
	AchievementDescription_Mumbai_Expert_Score,
	// Token: 0x04000CDB RID: 3291
	InGame_Messages_ExpertDailyChallenge,
	// Token: 0x04000CDC RID: 3292
	NewYorkCityDescID,
	// Token: 0x04000CDD RID: 3293
	NewYorkCityNameID,
	// Token: 0x04000CDE RID: 3294
	AchievementDescription_NewYorkCity_Score_400_Achieved,
	// Token: 0x04000CDF RID: 3295
	AchievementDescription_NewYorkCity_Score_400,
	// Token: 0x04000CE0 RID: 3296
	AchievementDescription_NewYorkCity_Score_400_Title,
	// Token: 0x04000CE1 RID: 3297
	AchievementDescription_NewYorkCity_Score_1000_Achieved,
	// Token: 0x04000CE2 RID: 3298
	AchievementDescription_NewYorkCity_Score_1000,
	// Token: 0x04000CE3 RID: 3299
	AchievementDescription_NewYorkCity_Score_1000_Title,
	// Token: 0x04000CE4 RID: 3300
	AchievementDescription_NewYorkCity_Score_2000,
	// Token: 0x04000CE5 RID: 3301
	AchievementDescription_NewYorkCity_Score_2000_Title,
	// Token: 0x04000CE6 RID: 3302
	CityChallenge_NewYorkCity_1,
	// Token: 0x04000CE7 RID: 3303
	AchievementDescription_NewYorkCity_Challenge_Score_600_Achieved,
	// Token: 0x04000CE8 RID: 3304
	AchievementDescription_NewYorkCity_Challenge_Score_600,
	// Token: 0x04000CE9 RID: 3305
	AchievementDescription_NewYorkCity_Challenge_Score_Title,
	// Token: 0x04000CEA RID: 3306
	AchievementDescription_NewYorkCity_Expert_Score_500,
	// Token: 0x04000CEB RID: 3307
	AchievementDescription_NewYorkCity_Expert_Score_Title,
	// Token: 0x04000CEC RID: 3308
	CityChallenge_Mumbai_2,
	// Token: 0x04000CED RID: 3309
	InGame_Messages_10YearCelebration,
	// Token: 0x04000CEE RID: 3310
	Popup_Body_10YearCelebration_MetroCrossPromo,
	// Token: 0x04000CEF RID: 3311
	Popup_Header_10YearCelebration_MetroCrossPromo,
	// Token: 0x04000CF0 RID: 3312
	Popup_Body_10YearCelebration_MetroCrossPromo_AppleArcade,
	// Token: 0x04000CF1 RID: 3313
	Popup_Body_10YearCelebration_NoCrossPromoUpdateDescription,
	// Token: 0x04000CF2 RID: 3314
	Popup_Body_10YearCelebration_NoCrossPromoUpdateDescription_ShortVersion,
	// Token: 0x04000CF3 RID: 3315
	AchievementDescription_Reykjavik_Score_400,
	// Token: 0x04000CF4 RID: 3316
	AchievementDescription_Reykjavik_Score_400_Achieved,
	// Token: 0x04000CF5 RID: 3317
	AchievementDescription_Reykjavik_Score_400_Title,
	// Token: 0x04000CF6 RID: 3318
	CityChallenge_Reykjavik_1,
	// Token: 0x04000CF7 RID: 3319
	ReykjavikDescription,
	// Token: 0x04000CF8 RID: 3320
	Reykjavik,
	// Token: 0x04000CF9 RID: 3321
	AchievementDescription_NewMapName_Expert_Score,
	// Token: 0x04000CFA RID: 3322
	AchievementDescription_NewMapName_Expert_Score_Title,
	// Token: 0x04000CFB RID: 3323
	AchievementDescription_NewMapName_Score_2000,
	// Token: 0x04000CFC RID: 3324
	AchievementDescription_NewMapName_Score_2000_Title,
	// Token: 0x04000CFD RID: 3325
	AchievementDescription_NewMapName_Score_1000,
	// Token: 0x04000CFE RID: 3326
	AchievementDescription_NewMapName_Score_1000_Achieved,
	// Token: 0x04000CFF RID: 3327
	AchievementDescription_NewMapName_Score_1000_Title,
	// Token: 0x04000D00 RID: 3328
	AchievementDescription_Reykjavik_Challenge_Score,
	// Token: 0x04000D01 RID: 3329
	AchievementDescription_Reykjavik_Challenge_Score_Achieved,
	// Token: 0x04000D02 RID: 3330
	AchievementDescription_Reykjavik_Challenge_Score_Title,
	// Token: 0x04000D03 RID: 3331
	CityChallenge_Reykjavik_2,
	// Token: 0x04000D04 RID: 3332
	Popup_Body_CrossPromo_AuroraBorealis,
	// Token: 0x04000D05 RID: 3333
	iCloud_FailedSync_Generic_Website,
	// Token: 0x04000D06 RID: 3334
	iCloud_FailedSync_Generic_OptionsMenu,
	// Token: 0x04000D07 RID: 3335
	iCloud_FailedSync_Generic,
	// Token: 0x04000D08 RID: 3336
	Options_iCloud_CacheIssue_IncorrectAccount,
	// Token: 0x04000D09 RID: 3337
	Options_iCloud_CacheIssue_NotSignedIn,
	// Token: 0x04000D0A RID: 3338
	[InspectorName("Popups/CopenhagenSteam")]
	Popup_Body_Evergreen_Copenhagen_Steam,
	// Token: 0x04000D0B RID: 3339
	[InspectorName("Popups/CopenhagenApple")]
	Popup_Body_Evergreen_Copenhagen_Apple,
	// Token: 0x04000D0C RID: 3340
	[InspectorName("Popups/CairnsSteam")]
	Popup_Body_Evergreen_Cairns_Steam,
	// Token: 0x04000D0D RID: 3341
	[InspectorName("Popups/CairnsApple")]
	Popup_Body_Evergreen_Cairns_Apple,
	// Token: 0x04000D0E RID: 3342
	VancouverCityDescription,
	// Token: 0x04000D0F RID: 3343
	Vancouver,
	// Token: 0x04000D10 RID: 3344
	AchievementDescription_Vancouver_Score_1000_Title,
	// Token: 0x04000D11 RID: 3345
	AchievementDescription_Vancouver_Score_400_Title,
	// Token: 0x04000D12 RID: 3346
	AchievementDescription_Vancouver_Score_2000_Title,
	// Token: 0x04000D13 RID: 3347
	AchievementDescription_Vancouver_Expert_Score_Title,
	// Token: 0x04000D14 RID: 3348
	AchievementDescription_Vancouver_Challenge_Score_Title,
	// Token: 0x04000D15 RID: 3349
	AchievementDescription_Vancouver_Expert_Score,
	// Token: 0x04000D16 RID: 3350
	AchievementDescription_Vancouver_Score_2000,
	// Token: 0x04000D17 RID: 3351
	AchievementDescription_Vancouver_Score_1000,
	// Token: 0x04000D18 RID: 3352
	AchievementDescription_Vancouver_Score_400,
	// Token: 0x04000D19 RID: 3353
	AchievementDescription_Vancouver_Challenge_Score_Achieved,
	// Token: 0x04000D1A RID: 3354
	AchievementDescription_Vancouver_Score_1000_Achieved,
	// Token: 0x04000D1B RID: 3355
	AchievementDescription_Vancouver_Score_400_Achieved,
	// Token: 0x04000D1C RID: 3356
	AchievementDescription_Vancouver_Challenge_Score,
	// Token: 0x04000D1D RID: 3357
	CityChallenge_Vancouver_1,
	// Token: 0x04000D1E RID: 3358
	CityChallenge_Vancouver_2,
	// Token: 0x04000D1F RID: 3359
	GameCenterLoginRetryRequiredTitle,
	// Token: 0x04000D20 RID: 3360
	GameCenterLoginRetryRequiredDescription,
	// Token: 0x04000D21 RID: 3361
	Options_iCloud_visit_faq,
	// Token: 0x04000D22 RID: 3362
	CityChallenge_Copenhagen_1,
	// Token: 0x04000D23 RID: 3363
	CopenhagenDescription,
	// Token: 0x04000D24 RID: 3364
	Copenhagen,
	// Token: 0x04000D25 RID: 3365
	AchievementDescription_Copenhagen_Challenge_Score_Achieved,
	// Token: 0x04000D26 RID: 3366
	AchievementDescription_Copenhagen_Score_1000_Achieved,
	// Token: 0x04000D27 RID: 3367
	AchievementDescription_Copenhagen_Score_400_Achieved,
	// Token: 0x04000D28 RID: 3368
	AchievementDescription_Copenhagen_Challenge_Score,
	// Token: 0x04000D29 RID: 3369
	AchievementDescription_Copenhagen_Expert_Score,
	// Token: 0x04000D2A RID: 3370
	AchievementDescription_Copenhagen_Score_2000,
	// Token: 0x04000D2B RID: 3371
	AchievementDescription_Copenhagen_Score_1000,
	// Token: 0x04000D2C RID: 3372
	AchievementDescription_Copenhagen_Score_400,
	// Token: 0x04000D2D RID: 3373
	AchievementDescription_Copenhagen_Challenge_Score_Title,
	// Token: 0x04000D2E RID: 3374
	AchievementDescription_Copenhagen_Expert_Score_Title,
	// Token: 0x04000D2F RID: 3375
	AchievementDescription_Copenhagen_Score_2000_Title,
	// Token: 0x04000D30 RID: 3376
	AchievementDescription_Copenhagen_Score_1000_Title,
	// Token: 0x04000D31 RID: 3377
	AchievementDescription_Copenhagen_Score_400_Title,
	// Token: 0x04000D32 RID: 3378
	CairnsDescription,
	// Token: 0x04000D33 RID: 3379
	Cairns,
	// Token: 0x04000D34 RID: 3380
	CityChallenge_Cairns_1,
	// Token: 0x04000D35 RID: 3381
	AchievementDescription_Cairns_Challenge_Score_Title,
	// Token: 0x04000D36 RID: 3382
	AchievementDescription_Cairns_Expert_Score_Title,
	// Token: 0x04000D37 RID: 3383
	AchievementDescription_Cairns_Score_2000_Title,
	// Token: 0x04000D38 RID: 3384
	AchievementDescription_Cairns_Score_1000_Title,
	// Token: 0x04000D39 RID: 3385
	AchievementDescription_Cairns_Score_400_Title,
	// Token: 0x04000D3A RID: 3386
	AchievementDescription_Cairns_Challenge_Score_Achieved,
	// Token: 0x04000D3B RID: 3387
	AchievementDescription_Cairns_Score_1000_Achieved,
	// Token: 0x04000D3C RID: 3388
	AchievementDescription_Cairns_Score_400_Achieved,
	// Token: 0x04000D3D RID: 3389
	AchievementDescription_Cairns_Challenge_Score,
	// Token: 0x04000D3E RID: 3390
	AchievementDescription_Cairns_Expert_Score,
	// Token: 0x04000D3F RID: 3391
	AchievementDescription_Cairns_Score_2000,
	// Token: 0x04000D40 RID: 3392
	AchievementDescription_Cairns_Score_1000,
	// Token: 0x04000D41 RID: 3393
	AchievementDescription_Cairns_Score_400,
	// Token: 0x04000D42 RID: 3394
	Credits_PeopleExperienceCoach,
	// Token: 0x04000D43 RID: 3395
	Credits_StudioTeam,
	// Token: 0x04000D44 RID: 3396
	Credits_DevelopmentTeam,
	// Token: 0x04000D45 RID: 3397
	Credits_OfficeManager,
	// Token: 0x04000D46 RID: 3398
	Credits_HeadsOfDepartment,
	// Token: 0x04000D47 RID: 3399
	Credits_FinancialStrategy,
	// Token: 0x04000D48 RID: 3400
	Credits_FinancialAdministration,
	// Token: 0x04000D49 RID: 3401
	AchievementDescription_MapUnlock_Score350,
	// Token: 0x04000D4A RID: 3402
	AchievementDescription_MapUnlock_Score300,
	// Token: 0x04000D4B RID: 3403
	AchievementDescription_MapUnlock_Score250,
	// Token: 0x04000D4C RID: 3404
	AchievementDescription_MapUnlock_Score200,
	// Token: 0x04000D4D RID: 3405
	CinematicMode_ErrorMessage_NoCarsToFollow,
	// Token: 0x04000D4E RID: 3406
	Credits_ProgrammingLead,
	// Token: 0x04000D4F RID: 3407
	Credits_DesignLead,
	// Token: 0x04000D50 RID: 3408
	Credits_ArtLead,
	// Token: 0x04000D51 RID: 3409
	Credits_MarketingLead,
	// Token: 0x04000D52 RID: 3410
	Credits_CommunityManagementLead,
	// Token: 0x04000D53 RID: 3411
	Credits_ProductionLead,
	// Token: 0x04000D54 RID: 3412
	Credits_QualityAssuranceLead,
	// Token: 0x04000D55 RID: 3413
	Credits_DigitalProduction,
	// Token: 0x04000D56 RID: 3414
	Tutorial_CreativeMode_Info2_Body,
	// Token: 0x04000D57 RID: 3415
	Tutorial_CreativeMode_Info1_Body_TouchOrController,
	// Token: 0x04000D58 RID: 3416
	Tutorial_CreativeMode_Info2_Header,
	// Token: 0x04000D59 RID: 3417
	Tutorial_CreativeMode_Info1_Header,
	// Token: 0x04000D5A RID: 3418
	Tutorial_CreativeMode_Info1_Body_Mouse,
	// Token: 0x04000D5B RID: 3419
	Popup_Body_Evergreen_CreativeMode_Steam,
	// Token: 0x04000D5C RID: 3420
	Popup_Body_Evergreen_CreativeMode_Apple,
	// Token: 0x04000D5D RID: 3421
	InGame_Messages_CreativeMode,
	// Token: 0x04000D5E RID: 3422
	HongKongDescription,
	// Token: 0x04000D5F RID: 3423
	CityChallenge_HongKong_1,
	// Token: 0x04000D60 RID: 3424
	HongKongNameId,
	// Token: 0x04000D61 RID: 3425
	CityChallenge_HongKong_2,
	// Token: 0x04000D62 RID: 3426
	Challenge_Upgrades_NoBridgesDescription1,
	// Token: 0x04000D63 RID: 3427
	Challenge_Upgrades_NoBridges_startwith1_MotorwayTitle,
	// Token: 0x04000D64 RID: 3428
	Challenge_Upgrades_NoBridges_startwith1_MotorwayDescription,
	// Token: 0x04000D65 RID: 3429
	[InspectorName("Popups/HongKongSteam")]
	Popup_Body_Evergreen_HongKong_Steam,
	// Token: 0x04000D66 RID: 3430
	[InspectorName("Popups/HongKongApple")]
	Popup_Body_Evergreen_HongKong_Apple,
	// Token: 0x04000D67 RID: 3431
	AchievementDescription_HongKong_Score_400,
	// Token: 0x04000D68 RID: 3432
	AchievementDescription_HongKong_Score_400_Achieved,
	// Token: 0x04000D69 RID: 3433
	AchievementDescription_HongKong_Score_400_Title,
	// Token: 0x04000D6A RID: 3434
	AchievementDescription_HongKong_Score_1000,
	// Token: 0x04000D6B RID: 3435
	AchievementDescription_HongKong_Score_1000_Achieved,
	// Token: 0x04000D6C RID: 3436
	AchievementDescription_HongKong_Score_1000_Title,
	// Token: 0x04000D6D RID: 3437
	AchievementDescription_HongKong_Score_2000,
	// Token: 0x04000D6E RID: 3438
	AchievementDescription_HongKong_Score_2000_Title,
	// Token: 0x04000D6F RID: 3439
	AchievementDescription_HongKong_Expert_Score_500,
	// Token: 0x04000D70 RID: 3440
	AchievementDescription_HongKong_Expert_Score_Title,
	// Token: 0x04000D71 RID: 3441
	AchievementDescription_HongKong_Challenge_Score_600,
	// Token: 0x04000D72 RID: 3442
	AchievementDescription_HongKong_Challenge_Score_600_Achieved,
	// Token: 0x04000D73 RID: 3443
	AchievementDescription_HongKong_Challenge_Score_Title,
	// Token: 0x04000D74 RID: 3444
	PT_Challenge_HongKong,
	// Token: 0x04000D75 RID: 3445
	CityChallenge_CapeTown_1,
	// Token: 0x04000D76 RID: 3446
	CapeTownDescription,
	// Token: 0x04000D77 RID: 3447
	CapeTown,
	// Token: 0x04000D78 RID: 3448
	AchievementDescription_CapeTown_Score_400,
	// Token: 0x04000D79 RID: 3449
	AchievementDescription_CapeTown_Score_400_Achieved,
	// Token: 0x04000D7A RID: 3450
	AchievementDescription_CapeTown_Score_400_Title,
	// Token: 0x04000D7B RID: 3451
	AchievementDescription_CapeTown_Score_1000,
	// Token: 0x04000D7C RID: 3452
	AchievementDescription_CapeTown_Score_1000_Achieved,
	// Token: 0x04000D7D RID: 3453
	AchievementDescription_CapeTown_Score_1000_Title,
	// Token: 0x04000D7E RID: 3454
	AchievementDescription_CapeTown_Score_2000,
	// Token: 0x04000D7F RID: 3455
	AchievementDescription_CapeTown_Score_2000_Title,
	// Token: 0x04000D80 RID: 3456
	AchievementDescription_CapeTown_Expert_Score_500,
	// Token: 0x04000D81 RID: 3457
	AchievementDescription_CapeTown_Expert_Score_Title,
	// Token: 0x04000D82 RID: 3458
	AchievementDescription_CapeTown_Challenge_Score_600,
	// Token: 0x04000D83 RID: 3459
	AchievementDescription_CapeTown_Challenge_Score_600_Achieved,
	// Token: 0x04000D84 RID: 3460
	AchievementDescription_CapeTown_Challenge_Score_Title
}
