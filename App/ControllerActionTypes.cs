using System;

// Token: 0x0200019F RID: 415
public static class ControllerActionTypes
{
	// Token: 0x020001A0 RID: 416
	public enum MotorwaysControllerActions
	{
		// Token: 0x040004D1 RID: 1233
		NavigateUp,
		// Token: 0x040004D2 RID: 1234
		NavigateRight,
		// Token: 0x040004D3 RID: 1235
		NavigateDown,
		// Token: 0x040004D4 RID: 1236
		NavigateLeft,
		// Token: 0x040004D5 RID: 1237
		NavigateInDirection,
		// Token: 0x040004D6 RID: 1238
		AccumulateNavigateInDirection,
		// Token: 0x040004D7 RID: 1239
		ResetAccumulatedNavigation,
		// Token: 0x040004D8 RID: 1240
		ActivateSelected,
		// Token: 0x040004D9 RID: 1241
		ActivateBack,
		// Token: 0x040004DA RID: 1242
		ActivateMenu,
		// Token: 0x040004DB RID: 1243
		BeginMoveInGameFocus,
		// Token: 0x040004DC RID: 1244
		MoveInGameFocus,
		// Token: 0x040004DD RID: 1245
		EndMoveInGameFocus,
		// Token: 0x040004DE RID: 1246
		DrawRoad,
		// Token: 0x040004DF RID: 1247
		CancelDrawRoad,
		// Token: 0x040004E0 RID: 1248
		FocusUpgradeBar,
		// Token: 0x040004E1 RID: 1249
		SelectUpgrade,
		// Token: 0x040004E2 RID: 1250
		PlaceUpgrade,
		// Token: 0x040004E3 RID: 1251
		MoveMotorway,
		// Token: 0x040004E4 RID: 1252
		MoveMotorwayHandle,
		// Token: 0x040004E5 RID: 1253
		ToggleDrawMode,
		// Token: 0x040004E6 RID: 1254
		ToggleGameSpeed,
		// Token: 0x040004E7 RID: 1255
		DecreaseGameSpeed,
		// Token: 0x040004E8 RID: 1256
		IncreaseGameSpeed,
		// Token: 0x040004E9 RID: 1257
		ActivateControllerSelect,
		// Token: 0x040004EA RID: 1258
		Zoom
	}
}
