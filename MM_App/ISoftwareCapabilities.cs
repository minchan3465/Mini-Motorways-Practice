using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000EF RID: 239
public interface ISoftwareCapabilities
{
	// Token: 0x060004EA RID: 1258
	void OnAppStart();

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x060004EB RID: 1259
	LocaleDatabase.LocaleId PreferredLocaleId { get; }

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x060004EC RID: 1260
	bool SupportsCloudSaves { get; }

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x060004ED RID: 1261
	bool CanShareImage { get; }

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x060004EE RID: 1262
	Vector2Int ScreenshotDimensions { get; }

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x060004EF RID: 1263
	bool SupportsHighDPI { get; }

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x060004F0 RID: 1264
	bool SupportsMultipleProfiles { get; }

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x060004F1 RID: 1265
	bool SupportsMovieScreen { get; }

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x060004F2 RID: 1266
	bool SupportsDisplayOptions { get; }

	// Token: 0x060004F3 RID: 1267
	bool SaveGif(byte[] gifData, string tag, string parentFolder, out StringId messageId, out StringId messageHeaderId);

	// Token: 0x060004F4 RID: 1268
	bool SaveScreenshot(Texture2D screenshot, string tag, string parentFolder, out StringId messageId);

	// Token: 0x060004F5 RID: 1269
	void SetIsInMainMenuScreen(bool isInMainMenuScreen);

	// Token: 0x060004F6 RID: 1270
	void SetIsInGame(bool isInGame);

	// Token: 0x060004F7 RID: 1271
	void OnAppShutdown();

	// Token: 0x060004F8 RID: 1272
	void SetRichPresence(Dictionary<string, string> tokens);

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x060004F9 RID: 1273
	StringId DeleteCloudGameStringId { get; }

	// Token: 0x060004FA RID: 1274
	bool AllowsTimedChallengeMessages();

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x060004FB RID: 1275
	bool SupportsEvergreenButton { get; }

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x060004FC RID: 1276
	StringId TenYearCelebrationPopupBody { get; }

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x060004FD RID: 1277
	string TenYearCelebrationMiniMetroStoreLink { get; }
}
