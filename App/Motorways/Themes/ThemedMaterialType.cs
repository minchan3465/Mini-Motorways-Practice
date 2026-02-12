using System;

namespace Motorways.Themes
{
	// Token: 0x0200047A RID: 1146
	public enum ThemedMaterialType
	{
		// Token: 0x0400182B RID: 6187
		Light,
		// Token: 0x0400182C RID: 6188
		Dark,
		// Token: 0x0400182D RID: 6189
		LightSecondary,
		// Token: 0x0400182E RID: 6190
		DarkSecondary,
		// Token: 0x0400182F RID: 6191
		TrafficLightRed,
		// Token: 0x04001830 RID: 6192
		TrafficLightAmber,
		// Token: 0x04001831 RID: 6193
		TrafficLightGreen,
		// Token: 0x04001832 RID: 6194
		TrafficLightInteraction,
		// Token: 0x04001833 RID: 6195
		MotorwayInteraction,
		// Token: 0x04001834 RID: 6196
		RoundaboutInteraction,
		// Token: 0x04001835 RID: 6197
		WorldGrid,
		// Token: 0x04001836 RID: 6198
		RoadInner,
		// Token: 0x04001837 RID: 6199
		RoadOutline,
		// Token: 0x04001838 RID: 6200
		RoadMothballed,
		// Token: 0x04001839 RID: 6201
		RoadCursor,
		// Token: 0x0400183A RID: 6202
		CarparkOutline,
		// Token: 0x0400183B RID: 6203
		CarparkDetail,
		// Token: 0x0400183C RID: 6204
		BridgeOutline,
		// Token: 0x0400183D RID: 6205
		BridgeRamp,
		// Token: 0x0400183E RID: 6206
		TunnelOutline,
		// Token: 0x0400183F RID: 6207
		DryingTunnel,
		// Token: 0x04001840 RID: 6208
		MotorwayHeader,
		// Token: 0x04001841 RID: 6209
		WaterGrid,
		// Token: 0x04001842 RID: 6210
		Wave,
		// Token: 0x04001843 RID: 6211
		TypographyWater,
		// Token: 0x04001844 RID: 6212
		TypographyLand,
		// Token: 0x04001845 RID: 6213
		Beach,
		// Token: 0x04001846 RID: 6214
		Grass,
		// Token: 0x04001847 RID: 6215
		Land,
		// Token: 0x04001848 RID: 6216
		TreeA,
		// Token: 0x04001849 RID: 6217
		TreeB,
		// Token: 0x0400184A RID: 6218
		TreeC,
		// Token: 0x0400184B RID: 6219
		MountainA,
		// Token: 0x0400184C RID: 6220
		MountainB,
		// Token: 0x0400184D RID: 6221
		MountainC,
		// Token: 0x0400184E RID: 6222
		Shadow,
		// Token: 0x0400184F RID: 6223
		VehicleSilhouette,
		// Token: 0x04001850 RID: 6224
		MotorwayOutline,
		// Token: 0x04001851 RID: 6225
		MotorwayInner,
		// Token: 0x04001852 RID: 6226
		MotorwayMothballedInner,
		// Token: 0x04001853 RID: 6227
		MotorwayMothballedOutline,
		// Token: 0x04001854 RID: 6228
		MotorwayShieldOutline,
		// Token: 0x04001855 RID: 6229
		PrimaryText,
		// Token: 0x04001856 RID: 6230
		SecondaryText,
		// Token: 0x04001857 RID: 6231
		LightSubheading,
		// Token: 0x04001858 RID: 6232
		DarkSubheading,
		// Token: 0x04001859 RID: 6233
		PrimaryMenu,
		// Token: 0x0400185A RID: 6234
		SecondaryMenu,
		// Token: 0x0400185B RID: 6235
		TertiaryMenu,
		// Token: 0x0400185C RID: 6236
		LeaderboardPrimary,
		// Token: 0x0400185D RID: 6237
		LeaderboardSecondary,
		// Token: 0x0400185E RID: 6238
		ChallengeIcon,
		// Token: 0x0400185F RID: 6239
		ChallengeAdditional,
		// Token: 0x04001860 RID: 6240
		HighlightedButton,
		// Token: 0x04001861 RID: 6241
		ProfileColor1,
		// Token: 0x04001862 RID: 6242
		ProfileColor2,
		// Token: 0x04001863 RID: 6243
		ProfileColor3,
		// Token: 0x04001864 RID: 6244
		ProfileColor4,
		// Token: 0x04001865 RID: 6245
		ProfileColor5,
		// Token: 0x04001866 RID: 6246
		ProfileColor6,
		// Token: 0x04001867 RID: 6247
		RoundaboutFilling,
		// Token: 0x04001868 RID: 6248
		RoundaboutInnerOutline,
		// Token: 0x04001869 RID: 6249
		Grey,
		// Token: 0x0400186A RID: 6250
		LockedMap,
		// Token: 0x0400186B RID: 6251
		DisabledUpgradeOption,
		// Token: 0x0400186C RID: 6252
		HistogramRulers,
		// Token: 0x0400186D RID: 6253
		HistogramLine,
		// Token: 0x0400186E RID: 6254
		IndicatorGrey,
		// Token: 0x0400186F RID: 6255
		Rail,
		// Token: 0x04001870 RID: 6256
		Train,
		// Token: 0x04001871 RID: 6257
		TrainHeadlights,
		// Token: 0x04001872 RID: 6258
		TrainHeadlightBeams,
		// Token: 0x04001873 RID: 6259
		BoatPath,
		// Token: 0x04001874 RID: 6260
		BoatBody,
		// Token: 0x04001875 RID: 6261
		BoatCabin,
		// Token: 0x04001876 RID: 6262
		BoatLight,
		// Token: 0x04001877 RID: 6263
		BoatRipple,
		// Token: 0x04001878 RID: 6264
		BoatTrail,
		// Token: 0x04001879 RID: 6265
		TenYearAnniversaryButtonNormal,
		// Token: 0x0400187A RID: 6266
		TenYearAnniversaryButtonHighlight,
		// Token: 0x0400187B RID: 6267
		TenYearAnniversaryButtonPressed,
		// Token: 0x0400187C RID: 6268
		TenYearAnniversaryButtonShadow,
		// Token: 0x0400187D RID: 6269
		RailBridge,
		// Token: 0x0400187E RID: 6270
		RailBridgeOutline,
		// Token: 0x0400187F RID: 6271
		Reef,
		// Token: 0x04001880 RID: 6272
		ReefOutline,
		// Token: 0x04001881 RID: 6273
		Count
	}
}
