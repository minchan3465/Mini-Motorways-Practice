using System;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public interface IHardwareCapabilities
{
	// Token: 0x060004C5 RID: 1221
	void OnAppStart();

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x060004C6 RID: 1222
	RuntimePlatform Platform { get; }

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x060004C7 RID: 1223
	LocaleDatabase.LocaleId PreferredLocaleId { get; }

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x060004C8 RID: 1224
	string PersistentStoragePath { get; }

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x060004C9 RID: 1225
	string UniqueDeviceId { get; }

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x060004CA RID: 1226
	DeviceInputType DefaultDeviceInputType { get; }

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x060004CB RID: 1227
	// (remove) Token: 0x060004CC RID: 1228
	event Action<DeviceInputGamepadStyle> OnGamepadStyleChanged;

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x060004CD RID: 1229
	DeviceInputGamepadStyle CurrentGamepadStyle { get; }

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x060004CE RID: 1230
	bool SupportsHapticFeedback { get; }

	// Token: 0x060004CF RID: 1231
	void GenerateHapticFeedback(HapticFeedbackType feedback);

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x060004D0 RID: 1232
	// (set) Token: 0x060004D1 RID: 1233
	bool IsPreventingSleep { get; set; }

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x060004D2 RID: 1234
	bool SupportsManualExit { get; }

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x060004D3 RID: 1235
	bool SupportsChangingResolution { get; }

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x060004D4 RID: 1236
	Vector2Int DefaultMaximumResolution { get; }

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x060004D5 RID: 1237
	bool SupportsAntiAliasingOptions { get; }

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x060004D6 RID: 1238
	int DefaultAntiAliasingLevel { get; }

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x060004D7 RID: 1239
	bool SupportsMultipleDisplays { get; }

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x060004D8 RID: 1240
	int DisplayCount { get; }

	// Token: 0x060004D9 RID: 1241
	void Exit();
}
