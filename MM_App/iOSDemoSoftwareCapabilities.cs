using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

// Token: 0x0200011D RID: 285
public class iOSDemoSoftwareCapabilities : ISoftwareCapabilities
{
	// Token: 0x06000630 RID: 1584 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnAppStart()
	{
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnAppShutdown()
	{
	}

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000632 RID: 1586 RVA: 0x00015ED3 File Offset: 0x000140D3
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return this._hardwareCapabilities.PreferredLocaleId;
		}
	}

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000633 RID: 1587 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsCloudSaves
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000634 RID: 1588 RVA: 0x0000222C File Offset: 0x0000042C
	public bool CanShareImage
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000635 RID: 1589 RVA: 0x00015E46 File Offset: 0x00014046
	public Vector2Int ScreenshotDimensions
	{
		get
		{
			return new Vector2Int(Screen.width, Screen.height);
		}
	}

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000636 RID: 1590 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsHighDPI
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000637 RID: 1591 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsMultipleProfiles
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000638 RID: 1592 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsMovieScreen
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000639 RID: 1593 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsDisplayOptions
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00015EE0 File Offset: 0x000140E0
	public bool SaveScreenshot(Texture2D screenshot, string tag, string parentFolder, out StringId messageId)
	{
		messageId = StringId.None;
		return false;
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x00015EE7 File Offset: 0x000140E7
	public bool SaveGif(byte[] data, string tag, string parentFolder, out StringId messageId, out StringId messageHeaderId)
	{
		messageId = StringId.None;
		messageHeaderId = StringId.None;
		return false;
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInMainMenuScreen(bool isInMainMenuScreen)
	{
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInGame(bool isInGame)
	{
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetRichPresence(Dictionary<string, string> tokens)
	{
	}

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x0600063F RID: 1599 RVA: 0x00015EF2 File Offset: 0x000140F2
	public StringId DeleteCloudGameStringId
	{
		get
		{
			return StringId.DeleteSpecificJournalPrompt_iCloud;
		}
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x000020AA File Offset: 0x000002AA
	public bool AllowsTimedChallengeMessages()
	{
		return true;
	}

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000641 RID: 1601 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsEvergreenButton
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x06000642 RID: 1602 RVA: 0x0000222C File Offset: 0x0000042C
	public StringId TenYearCelebrationPopupBody
	{
		get
		{
			return StringId.None;
		}
	}

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000643 RID: 1603 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public string TenYearCelebrationMiniMetroStoreLink
	{
		get
		{
			return null;
		}
	}

	// Token: 0x04000294 RID: 660
	[Dependency]
	protected IHardwareCapabilities _hardwareCapabilities;
}
