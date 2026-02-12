using System;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x0200067C RID: 1660
	public static class Settings
	{
		// Token: 0x040027F3 RID: 10227
		public const float PAN_WIDTH = 4f;

		// Token: 0x040027F4 RID: 10228
		public const float PAN_CLOCK = 0.75f;

		// Token: 0x040027F5 RID: 10229
		public const int CHORD_STACK_CEILING = 0;

		// Token: 0x040027F6 RID: 10230
		public static float PITCH_PAUSE = 0.9375f;

		// Token: 0x040027F7 RID: 10231
		public static float PITCH_NIGHT = 1.6875f;

		// Token: 0x040027F8 RID: 10232
		public static float PITCH_ANCHOR = 1f;

		// Token: 0x040027F9 RID: 10233
		public static float PITCH_MIXBUS_ATTENUATION = -3f;

		// Token: 0x040027FA RID: 10234
		public static readonly Vector2 PITCH_BOING_IN_PLACE = new Vector2(0f, 0.04f);

		// Token: 0x040027FB RID: 10235
		public static readonly Vector2 PITCH_TREE_BULLDOZED = new Vector2(0f, 0.1f);

		// Token: 0x040027FC RID: 10236
		public static readonly Vector2 ECHO_DECAY_RANGE = new Vector2(0.25f, 0.45f);

		// Token: 0x040027FD RID: 10237
		public static readonly Vector2 ECHO_WET_RANGE = new Vector2(0.1f, 0.2f);

		// Token: 0x040027FE RID: 10238
		public const float ECHO_OFF_DECAY = 0.75f;

		// Token: 0x040027FF RID: 10239
		public const double IDLE_LOOP_FADE_IN = 2.0;

		// Token: 0x04002800 RID: 10240
		public const double IDLE_LOOP_FADE_OUT = 3.5;

		// Token: 0x04002801 RID: 10241
		public const double BASS_FADE_IN = 0.5;

		// Token: 0x04002802 RID: 10242
		public const double BASS_FADE_OUT = 0.5;

		// Token: 0x04002803 RID: 10243
		public static readonly Param.Group UPGRADE_GRAB = Param.Gain(1f, -1f).Pitch(0.75f, -1f);

		// Token: 0x04002804 RID: 10244
		public static readonly Param.Group UPGRADE_RELEASE = Param.Gain(0.75f, -1f).Pitch(0.33f, -1f);

		// Token: 0x04002805 RID: 10245
		public static readonly Param.Group BUILD_BRIDGE = Param.Gain(0.33f, 0.6f);

		// Token: 0x04002806 RID: 10246
		public static readonly Param.Group BUILD_TUNNEL = Param.Gain(0.3f, 0.55f);

		// Token: 0x04002807 RID: 10247
		public static readonly Param.Group BUILD_ROAD = Param.Gain(0.33f, 0.6f).Pitch(0.75f, 1.25f);

		// Token: 0x04002808 RID: 10248
		public static readonly Param.Group DELETE_ROAD = Param.Gain(0.25f, 0.375f);

		// Token: 0x04002809 RID: 10249
		public static readonly Param.Group MOTHBALL_ROAD = Param.Gain(1f, 0.5f).Pitch(1f, 1.5f);

		// Token: 0x0400280A RID: 10250
		public static readonly Param.Group BULLDOZE_TREE = Param.Gain(0.5f, 0.75f).Pitch(0.75f, 1.25f);

		// Token: 0x0200067D RID: 1661
		public static class Attenuation
		{
			// Token: 0x0400280B RID: 10251
			public const float FALLOFF = 5f;

			// Token: 0x0400280C RID: 10252
			public const float FALLOFF_HOCKETS_MENU = 33f;

			// Token: 0x0400280D RID: 10253
			public const float FALLOFF_IDLE_LOOPS_MENU = 500f;

			// Token: 0x0400280E RID: 10254
			public const float FALLOFF_SPAWNS = 25f;

			// Token: 0x0200067E RID: 1662
			public static class Zoom
			{
				// Token: 0x0400280F RID: 10255
				public static readonly Vector2 DYNAMIC_RANGE = new Vector2(0.33f, 1f);

				// Token: 0x04002810 RID: 10256
				public static readonly float MENU = Settings.Attenuation.Zoom.DYNAMIC_RANGE.x + 0.5f * (Settings.Attenuation.Zoom.DYNAMIC_RANGE.y - Settings.Attenuation.Zoom.DYNAMIC_RANGE.x);

				// Token: 0x04002811 RID: 10257
				public const bool HOUSE_SPAWNED = false;

				// Token: 0x04002812 RID: 10258
				public const bool DESTINATION_ACTIVATED = false;

				// Token: 0x04002813 RID: 10259
				public const bool IDLE_LOOPS = false;

				// Token: 0x04002814 RID: 10260
				public const bool GROUP_LOOPS_MENU = false;
			}
		}

		// Token: 0x0200067F RID: 1663
		public static class Gain
		{
			// Token: 0x04002815 RID: 10261
			public static readonly Vector2 KEYBOARD = new Vector2(1f, 0.3f);

			// Token: 0x04002816 RID: 10262
			public const float BASS_STATIC = 0.5f;

			// Token: 0x04002817 RID: 10263
			public const float BASS_AMBIENT = 0.4f;

			// Token: 0x04002818 RID: 10264
			public const float CLOCK = 0.5f;

			// Token: 0x04002819 RID: 10265
			public const float CHORD_STARTUP = 0.55f;

			// Token: 0x0400281A RID: 10266
			public const float CHORD_INGAME = 0.275f;

			// Token: 0x0400281B RID: 10267
			public static readonly Vector2 CHORD_WEEKOVER = new Vector2(0.25f, 0.55f);

			// Token: 0x0400281C RID: 10268
			public const float CHORD_DESTINATION_IN_GROUP_Y = 0.33f;

			// Token: 0x0400281D RID: 10269
			public const float CHORD_DESTINATION_IN_GROUP_N = 0.15f;

			// Token: 0x0400281E RID: 10270
			public const float GROUP_LOOP_HOME = 0.17f;

			// Token: 0x0400281F RID: 10271
			public const float GROUP_LOOP_DEST_MAX = 0.4f;

			// Token: 0x04002820 RID: 10272
			public const float IDLE_LOOP = 0.125f;

			// Token: 0x04002821 RID: 10273
			public const float IDLE_LOOP_MENU = 0.09375f;

			// Token: 0x04002822 RID: 10274
			public const float VEHICLE_MOTOR = 0.2f;

			// Token: 0x04002823 RID: 10275
			public const float VEHICLE_RECEIVES_PIN = 0.18f;

			// Token: 0x04002824 RID: 10276
			public const float VEHICLE_RECEIVES_PIN_REVERSE = 0.01f;

			// Token: 0x04002825 RID: 10277
			public const float VEHICLE_HORN = 0.11f;

			// Token: 0x04002826 RID: 10278
			public const float SFX_WHOOSH = 0.075f;

			// Token: 0x04002827 RID: 10279
			public static readonly Vector2 UI_CHECKBOX_HOVER = new Vector2(0.1f, 0.35f);

			// Token: 0x04002828 RID: 10280
			public const float HOUSE_SPAWNED = 1f;

			// Token: 0x04002829 RID: 10281
			public static readonly float[] HOUSE_SPAWNED_CHORD = new float[]
			{
				0.05f,
				0.1f
			};

			// Token: 0x0400282A RID: 10282
			public const float DESTINATION_ACTIVATED = 1f;

			// Token: 0x0400282B RID: 10283
			public static readonly Vector2 DESTINATION_DEMANDED = new Vector2(0.2f, 0.4f);

			// Token: 0x0400282C RID: 10284
			public const float MOTORWAY_HANDLE_RELEASED = 1f;

			// Token: 0x0400282D RID: 10285
			public const float MOTORWAY_HANDLE_PULLED = 0.75f;
		}
	}
}
