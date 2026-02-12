using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class NullSoftwareCapabilities : ISoftwareCapabilities
{
	// Token: 0x17000155 RID: 341
	// (get) Token: 0x06000672 RID: 1650 RVA: 0x000020AA File Offset: 0x000002AA
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return LocaleDatabase.LocaleId.en_US;
		}
	}

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000673 RID: 1651 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsCloudSaves
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000674 RID: 1652 RVA: 0x000020AA File Offset: 0x000002AA
	public bool CanShareImage
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000675 RID: 1653 RVA: 0x00015E46 File Offset: 0x00014046
	public Vector2Int ScreenshotDimensions
	{
		get
		{
			return new Vector2Int(Screen.width, Screen.height);
		}
	}

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x06000676 RID: 1654 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsHighDPI
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700015A RID: 346
	// (get) Token: 0x06000677 RID: 1655 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsMultipleProfiles
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06000678 RID: 1656 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsMovieScreen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06000679 RID: 1657 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsDisplayOptions
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInMainMenuScreen(bool isInMainMenuScreen)
	{
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInGame(bool isInGame)
	{
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void OnAppStart()
	{
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnAppShutdown()
	{
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00016014 File Offset: 0x00014214
	public bool SaveGif(byte[] data, string tag, string parentFolder, out StringId messageId, out StringId messageHeaderId)
	{
		bool success = ImageSharingUtility.SaveGIF(data, tag + ImageSharingUtility.GIF, parentFolder);
		messageId = (success ? StringId.Gif_Save_Directory_Steam : StringId.Moviemode_Failure);
		messageHeaderId = (success ? StringId.Moviemode_Popup_Header : StringId.Moviemode_Popup_Header_Failure);
		return success;
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x0001605C File Offset: 0x0001425C
	public bool SaveScreenshot(Texture2D screenshot, string tag, string parentFolder, out StringId messageId)
	{
		bool success = ImageSharingUtility.SaveScreenshotToPictures(screenshot, tag + ".gif", parentFolder);
		messageId = (success ? StringId.PhotoGif_Save_Directory_Steam : StringId.Photomode_Failure);
		return success;
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetRichPresence(Dictionary<string, string> tokens)
	{
	}

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x06000681 RID: 1665 RVA: 0x0000222C File Offset: 0x0000042C
	public StringId DeleteCloudGameStringId
	{
		get
		{
			return StringId.None;
		}
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x0000222C File Offset: 0x0000042C
	public bool AllowsTimedChallengeMessages()
	{
		return false;
	}

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000683 RID: 1667 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsEvergreenButton
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000684 RID: 1668 RVA: 0x0000222C File Offset: 0x0000042C
	public StringId TenYearCelebrationPopupBody
	{
		get
		{
			return StringId.None;
		}
	}

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000685 RID: 1669 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public string TenYearCelebrationMiniMetroStoreLink
	{
		get
		{
			return null;
		}
	}
}
