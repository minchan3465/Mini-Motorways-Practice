using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class MacAppStoreSoftwareCapabilities : ISoftwareCapabilities
{
	// Token: 0x17000149 RID: 329
	// (get) Token: 0x0600065C RID: 1628 RVA: 0x00015F87 File Offset: 0x00014187
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return this._hardwareCapabilities.PreferredLocaleId;
		}
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x0600065D RID: 1629 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsCloudSaves
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x0600065E RID: 1630 RVA: 0x000020AA File Offset: 0x000002AA
	public bool CanShareImage
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x0600065F RID: 1631 RVA: 0x00015E46 File Offset: 0x00014046
	public Vector2Int ScreenshotDimensions
	{
		get
		{
			return new Vector2Int(Screen.width, Screen.height);
		}
	}

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x06000660 RID: 1632 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsHighDPI
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000661 RID: 1633 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsMultipleProfiles
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06000662 RID: 1634 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsMovieScreen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000663 RID: 1635 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsDisplayOptions
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x00015F94 File Offset: 0x00014194
	public bool SaveScreenshot(Texture2D screenshot, string tag, string parentFolder, out StringId messageId)
	{
		bool success = ImageSharingUtility.SaveScreenshotToPictures(screenshot, tag + ImageSharingUtility.PNG, parentFolder);
		messageId = (success ? StringId.PhotoGif_Save_Directory_Mac : StringId.Photomode_Failure);
		return success;
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x00015FC8 File Offset: 0x000141C8
	public bool SaveGif(byte[] data, string tag, string parentFolder, out StringId messageId, out StringId messageHeaderId)
	{
		bool success = ImageSharingUtility.SaveGIF(data, tag + ImageSharingUtility.GIF, parentFolder);
		messageId = (success ? StringId.PhotoGif_Save_Directory_Mac : StringId.Moviemode_Failure);
		messageHeaderId = (success ? StringId.Moviemode_Popup_Header : StringId.Moviemode_Popup_Header_Failure);
		return success;
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInMainMenuScreen(bool isInMainMenuScreen)
	{
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInGame(bool isInGame)
	{
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x0001600D File Offset: 0x0001420D
	public virtual void OnAppStart()
	{
		MacAppStoreSoftwareCapabilities.InitializeFairPlay();
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnAppShutdown()
	{
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetRichPresence(Dictionary<string, string> tokens)
	{
	}

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x0600066B RID: 1643 RVA: 0x00015EF2 File Offset: 0x000140F2
	public StringId DeleteCloudGameStringId
	{
		get
		{
			return StringId.DeleteSpecificJournalPrompt_iCloud;
		}
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x000020AA File Offset: 0x000002AA
	public bool AllowsTimedChallengeMessages()
	{
		return true;
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void InitializeFairPlay()
	{
	}

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x0600066E RID: 1646 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsEvergreenButton
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x0600066F RID: 1647 RVA: 0x00015F79 File Offset: 0x00014179
	public StringId TenYearCelebrationPopupBody
	{
		get
		{
			return StringId.Popup_Body_CrossPromo_AuroraBorealis;
		}
	}

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000670 RID: 1648 RVA: 0x00015F80 File Offset: 0x00014180
	public string TenYearCelebrationMiniMetroStoreLink
	{
		get
		{
			return "https://apple.co/-MiniMetro";
		}
	}

	// Token: 0x04000296 RID: 662
	[Dependency]
	protected IHardwareCapabilities _hardwareCapabilities;
}
