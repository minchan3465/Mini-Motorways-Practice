using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

// Token: 0x0200011C RID: 284
public class HumbleSoftwareCapabilities : ISoftwareCapabilities
{
	// Token: 0x17000125 RID: 293
	// (get) Token: 0x0600061B RID: 1563 RVA: 0x00015E32 File Offset: 0x00014032
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return this._hardwareCapabilities.PreferredLocaleId;
		}
	}

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x0600061C RID: 1564 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsCloudSaves
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000127 RID: 295
	// (get) Token: 0x0600061D RID: 1565 RVA: 0x00015E3F File Offset: 0x0001403F
	public bool CanShareImage
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x0600061E RID: 1566 RVA: 0x00015E46 File Offset: 0x00014046
	public Vector2Int ScreenshotDimensions
	{
		get
		{
			return new Vector2Int(Screen.width, Screen.height);
		}
	}

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x0600061F RID: 1567 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsHighDPI
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000620 RID: 1568 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsMultipleProfiles
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06000621 RID: 1569 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsMovieScreen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000622 RID: 1570 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsDisplayOptions
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInMainMenuScreen(bool isInMainMenuScreen)
	{
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInGame(bool isInGame)
	{
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void OnAppStart()
	{
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnAppShutdown()
	{
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x00015E58 File Offset: 0x00014058
	public bool SaveGif(byte[] data, string tag, string parentFolder, out StringId messageId, out StringId messageHeaderId)
	{
		bool success = ImageSharingUtility.SaveGIF(data, tag + ImageSharingUtility.GIF, parentFolder);
		messageId = (success ? StringId.PhotoGif_Save_Directory_Mac : StringId.Moviemode_Failure);
		messageHeaderId = (success ? StringId.Moviemode_Popup_Header : StringId.Moviemode_Popup_Header_Failure);
		return success;
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x00015EA0 File Offset: 0x000140A0
	public bool SaveScreenshot(Texture2D screenshot, string tag, string parentFolder, out StringId messageId)
	{
		bool success = ImageSharingUtility.SaveScreenshotToPictures(screenshot, tag + ImageSharingUtility.PNG, parentFolder);
		messageId = (success ? StringId.PhotoGif_Save_Directory_Mac : StringId.Photomode_Failure);
		return success;
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetRichPresence(Dictionary<string, string> tokens)
	{
	}

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x0600062A RID: 1578 RVA: 0x0000222C File Offset: 0x0000042C
	public StringId DeleteCloudGameStringId
	{
		get
		{
			return StringId.None;
		}
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x0000222C File Offset: 0x0000042C
	public bool AllowsTimedChallengeMessages()
	{
		return false;
	}

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x0600062C RID: 1580 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsEvergreenButton
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x0600062D RID: 1581 RVA: 0x0000222C File Offset: 0x0000042C
	public StringId TenYearCelebrationPopupBody
	{
		get
		{
			return StringId.None;
		}
	}

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x0600062E RID: 1582 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public string TenYearCelebrationMiniMetroStoreLink
	{
		get
		{
			return null;
		}
	}

	// Token: 0x04000293 RID: 659
	[Dependency]
	protected IHardwareCapabilities _hardwareCapabilities;
}
