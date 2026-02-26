using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

// Token: 0x02000123 RID: 291
public class tvOSSoftwareCapabilities : ISoftwareCapabilities
{
	// Token: 0x1700016D RID: 365
	// (get) Token: 0x0600069C RID: 1692 RVA: 0x0001609C File Offset: 0x0001429C
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return this._hardwareCapabilities.PreferredLocaleId;
		}
	}

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x0600069D RID: 1693 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsCloudSaves
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700016F RID: 367
	// (get) Token: 0x0600069E RID: 1694 RVA: 0x0000222C File Offset: 0x0000042C
	public bool CanShareImage
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x0600069F RID: 1695 RVA: 0x00015E46 File Offset: 0x00014046
	public Vector2Int ScreenshotDimensions
	{
		get
		{
			return new Vector2Int(Screen.width, Screen.height);
		}
	}

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x060006A0 RID: 1696 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsHighDPI
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x060006A1 RID: 1697 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsMultipleProfiles
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x060006A2 RID: 1698 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsMovieScreen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x060006A3 RID: 1699 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsDisplayOptions
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInMainMenuScreen(bool isInMainMenuScreen)
	{
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInGame(bool isInGame)
	{
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void OnAppStart()
	{
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnAppShutdown()
	{
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x00015EE0 File Offset: 0x000140E0
	public bool SaveScreenshot(Texture2D screenshot, string name, string parentFolder, out StringId messageId)
	{
		messageId = StringId.None;
		return false;
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x00015EE7 File Offset: 0x000140E7
	public bool SaveGif(byte[] data, string tag, string parentFolder, out StringId messageId, out StringId messageHeaderId)
	{
		messageId = StringId.None;
		messageHeaderId = StringId.None;
		return false;
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetRichPresence(Dictionary<string, string> tokens)
	{
	}

	// Token: 0x17000175 RID: 373
	// (get) Token: 0x060006AB RID: 1707 RVA: 0x00015EF2 File Offset: 0x000140F2
	public StringId DeleteCloudGameStringId
	{
		get
		{
			return StringId.DeleteSpecificJournalPrompt_iCloud;
		}
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x000020AA File Offset: 0x000002AA
	public bool AllowsTimedChallengeMessages()
	{
		return true;
	}

	// Token: 0x17000176 RID: 374
	// (get) Token: 0x060006AD RID: 1709 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsEvergreenButton
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000177 RID: 375
	// (get) Token: 0x060006AE RID: 1710 RVA: 0x0000222C File Offset: 0x0000042C
	public StringId TenYearCelebrationPopupBody
	{
		get
		{
			return StringId.None;
		}
	}

	// Token: 0x17000178 RID: 376
	// (get) Token: 0x060006AF RID: 1711 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public string TenYearCelebrationMiniMetroStoreLink
	{
		get
		{
			return null;
		}
	}

	// Token: 0x04000298 RID: 664
	[Dependency]
	protected IHardwareCapabilities _hardwareCapabilities;
}
