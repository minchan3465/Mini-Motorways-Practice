using System;
using System.Collections.Generic;
using System.Text;
using Factory;
using Motorways;
using Motorways.UI;
using UnityEngine;

namespace SoftwareCapabilities
{
	// Token: 0x02000332 RID: 818
	public class SteamSoftwareCapabilities : ISoftwareCapabilities
	{
		// Token: 0x060013BD RID: 5053 RVA: 0x00040DC4 File Offset: 0x0003EFC4
		public void OnAppStart()
		{
			if (SteamworksShared.RestartAppIfNecessary(1127500U))
			{
				SteamSoftwareCapabilities.Log.Warn("The app was not started from Steam, so will restart via Steam", Array.Empty<object>());
				this._hardwareCapabilities.Exit();
				return;
			}
			if (!Diagnostics.Verify(SteamworksShared.Init(1127500U), "Failed to initialise SteamworksShared"))
			{
				return;
			}
			if (this._scope == null)
			{
				AppRuntime appRuntime = UnityEngine.Object.FindObjectOfType<AppRuntime>();
				IScope scope;
				if (appRuntime == null)
				{
					scope = null;
				}
				else
				{
					IApp app = appRuntime.App;
					scope = ((app != null) ? app.Scope : null);
				}
				this._scope = scope;
			}
			this._tickRegistry.AppTicking += delegate(float deltaTime)
			{
				SteamworksShared.RunCallbacks();
				if (!this._hasSyncedAchievements && this._scope.Get<ActivePlayer>().HasActivePlayer)
				{
					this.SyncCompletedAchivements();
					this._hasSyncedAchievements = true;
				}
			};
		}

		// Token: 0x060013BE RID: 5054 RVA: 0x00040E58 File Offset: 0x0003F058
		private void SyncCompletedAchivements()
		{
			foreach (Achievement achievement in this._scope.Get<ActivePlayer>().MotorwaysUserProfile.Achievements)
			{
				if (achievement.IsComplete())
				{
					this._achievementHandler.CompleteAchievement(achievement, false);
				}
			}
		}

		// Token: 0x060013BF RID: 5055 RVA: 0x00040ECC File Offset: 0x0003F0CC
		public void OnAppShutdown()
		{
			SteamworksShared.Shutdown();
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x060013C0 RID: 5056 RVA: 0x00040ED4 File Offset: 0x0003F0D4
		public LocaleDatabase.LocaleId PreferredLocaleId
		{
			get
			{
				LocaleDatabase.LocaleId localeId = SteamworksShared.GetLocaleId();
				if (localeId == LocaleDatabase.LocaleId.Unknown || !this._localeDatabase.IsLocaleSelectable(localeId))
				{
					localeId = UnityLocaleQuery.GetLocaleId(this._localeDatabase);
				}
				return localeId;
			}
		}

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x060013C1 RID: 5057 RVA: 0x00040F05 File Offset: 0x0003F105
		public bool SupportsCloudSaves { get; }

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x060013C2 RID: 5058 RVA: 0x000020AA File Offset: 0x000002AA
		public bool CanShareImage
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x060013C3 RID: 5059 RVA: 0x00015E46 File Offset: 0x00014046
		public Vector2Int ScreenshotDimensions
		{
			get
			{
				return new Vector2Int(Screen.width, Screen.height);
			}
		}

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x060013C4 RID: 5060 RVA: 0x00040F0D File Offset: 0x0003F10D
		public bool SupportsHighDPI { get; }

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x060013C5 RID: 5061 RVA: 0x0000222C File Offset: 0x0000042C
		public bool SupportsMultipleProfiles
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x060013C6 RID: 5062 RVA: 0x000020AA File Offset: 0x000002AA
		public bool SupportsMovieScreen
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x060013C7 RID: 5063 RVA: 0x000020AA File Offset: 0x000002AA
		public bool SupportsDisplayOptions
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x00040F18 File Offset: 0x0003F118
		public bool SaveGif(byte[] data, string tag, string parentFolder, out StringId messageId, out StringId messageHeaderId)
		{
			bool success = ImageSharingUtility.SaveGIF(data, tag + ImageSharingUtility.GIF, parentFolder);
			messageId = (success ? StringId.Gif_Save_Directory_Steam : StringId.Moviemode_Failure);
			messageHeaderId = (success ? StringId.Moviemode_Popup_Header : StringId.Moviemode_Popup_Header_Failure);
			return success;
		}

		// Token: 0x060013C9 RID: 5065 RVA: 0x00040F60 File Offset: 0x0003F160
		public bool SaveScreenshot(Texture2D screenshot, string tag, string parentFolder, out StringId messageId)
		{
			if (Application.isEditor)
			{
				messageId = StringId.PhotoGif_Save_Directory_Steam;
				return ImageSharingUtility.SaveScreenshotToPictures(screenshot, tag + ".png", parentFolder);
			}
			Color32[] pixels = screenshot.GetPixels32();
			if (pixels == null)
			{
				messageId = StringId.Photomode_Failure;
				return false;
			}
			byte[] rgb = new byte[pixels.Length * 3];
			for (int byteIndex = 0; byteIndex < pixels.Length; byteIndex++)
			{
				int y = byteIndex / screenshot.width;
				int x = byteIndex - y * screenshot.width;
				y = screenshot.height - 1 - y;
				int pixelIndex = y * screenshot.width + x;
				rgb[byteIndex * 3] = pixels[pixelIndex].r;
				rgb[byteIndex * 3 + 1] = pixels[pixelIndex].g;
				rgb[byteIndex * 3 + 2] = pixels[pixelIndex].b;
			}
			bool success = SteamworksShared.SaveScreenshot(rgb, screenshot.width, screenshot.height);
			messageId = (success ? StringId.PhotoGif_Save_Directory_Steam : StringId.Photomode_Failure);
			return success;
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x000022F5 File Offset: 0x000004F5
		public void SetIsInMainMenuScreen(bool isInMainMenuScreen)
		{
		}

		// Token: 0x060013CB RID: 5067 RVA: 0x000022F5 File Offset: 0x000004F5
		public void SetIsInGame(bool isInGame)
		{
		}

		// Token: 0x060013CC RID: 5068 RVA: 0x0004104E File Offset: 0x0003F24E
		public void SetRichPresence(Dictionary<string, string> tokens)
		{
			SteamworksShared.SetRichPresence(tokens);
		}

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x060013CD RID: 5069 RVA: 0x00041056 File Offset: 0x0003F256
		public StringId DeleteCloudGameStringId
		{
			get
			{
				return StringId.DeleteSpecificJournalPrompt_Steam;
			}
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x0000222C File Offset: 0x0000042C
		public bool AllowsTimedChallengeMessages()
		{
			return false;
		}

		// Token: 0x060013CF RID: 5071 RVA: 0x0004105D File Offset: 0x0003F25D
		public static Dictionary<string, string> GetRichPresenceTokens(string cityName, string displayKey)
		{
			return new Dictionary<string, string>
			{
				{
					"steam_display",
					displayKey
				},
				{
					"city",
					SteamSoftwareCapabilities.ConvertCityNameToSnakeCase(cityName)
				}
			};
		}

		// Token: 0x060013D0 RID: 5072 RVA: 0x00041084 File Offset: 0x0003F284
		public static string ConvertCityNameToSnakeCase(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			if (text.Length < 2)
			{
				return text;
			}
			StringBuilder sb = new StringBuilder();
			sb.Append(char.ToLowerInvariant(text[0]));
			for (int charIndex = 1; charIndex < text.Length; charIndex++)
			{
				char character = text[charIndex];
				if (char.IsUpper(character))
				{
					sb.Append('_');
					sb.Append(char.ToLowerInvariant(character));
				}
				else
				{
					sb.Append(character);
				}
			}
			return sb.ToString();
		}

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x060013D1 RID: 5073 RVA: 0x000020AA File Offset: 0x000002AA
		public bool SupportsEvergreenButton
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x060013D2 RID: 5074 RVA: 0x00015F79 File Offset: 0x00014179
		public StringId TenYearCelebrationPopupBody
		{
			get
			{
				return StringId.Popup_Body_CrossPromo_AuroraBorealis;
			}
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x060013D3 RID: 5075 RVA: 0x0004110A File Offset: 0x0003F30A
		public string TenYearCelebrationMiniMetroStoreLink
		{
			get
			{
				return TenYearCelebrationMiniMetroStoreLinks.SteamStoreLink;
			}
		}

		// Token: 0x0400107D RID: 4221
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("SteamSoftwareCapabilities");

		// Token: 0x0400107E RID: 4222
		[Dependency]
		private IHardwareCapabilities _hardwareCapabilities;

		// Token: 0x0400107F RID: 4223
		[Dependency]
		private LocaleDatabase _localeDatabase;

		// Token: 0x04001080 RID: 4224
		[Dependency]
		private TickRegistry _tickRegistry;

		// Token: 0x04001081 RID: 4225
		[Dependency]
		private IAchievementHandler _achievementHandler;

		// Token: 0x04001082 RID: 4226
		[Dependency]
		private IScope _scope;

		// Token: 0x04001083 RID: 4227
		private bool _hasSyncedAchievements;

		// Token: 0x04001086 RID: 4230
		public const string CityModeKey = "#ModeCity";

		// Token: 0x04001087 RID: 4231
		public const string DailyChallengeModeKey = "#ModeDailyChallenge";

		// Token: 0x04001088 RID: 4232
		public const string WeeklyChallengeModeKey = "#ModeWeeklyChallenge";

		// Token: 0x04001089 RID: 4233
		public const string CityKey = "city";

		// Token: 0x0400108A RID: 4234
		public const string SteamDisplayKey = "steam_display";
	}
}
