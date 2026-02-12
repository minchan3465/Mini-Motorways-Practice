using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class tvOSDemoSoftwareCapabilities : ISoftwareCapabilities
{
	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000687 RID: 1671 RVA: 0x0001608F File Offset: 0x0001428F
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return this._hardwareCapabilities.PreferredLocaleId;
		}
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000688 RID: 1672 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsCloudSaves
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000689 RID: 1673 RVA: 0x0000222C File Offset: 0x0000042C
	public bool CanShareImage
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x0600068A RID: 1674 RVA: 0x00015E46 File Offset: 0x00014046
	public Vector2Int ScreenshotDimensions
	{
		get
		{
			return new Vector2Int(Screen.width, Screen.height);
		}
	}

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x0600068B RID: 1675 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsHighDPI
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x0600068C RID: 1676 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsMultipleProfiles
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x0600068D RID: 1677 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsMovieScreen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x0600068E RID: 1678 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsDisplayOptions
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInMainMenuScreen(bool isInMainMenuScreen)
	{
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInGame(bool isInGame)
	{
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void OnAppStart()
	{
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnAppShutdown()
	{
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x00015EE0 File Offset: 0x000140E0
	public bool SaveScreenshot(Texture2D screenshot, string name, string parentFolder, out StringId messageId)
	{
		messageId = StringId.None;
		return false;
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x00015EE7 File Offset: 0x000140E7
	public bool SaveGif(byte[] data, string tag, string parentFolder, out StringId messageId, out StringId messageHeaderId)
	{
		messageId = StringId.None;
		messageHeaderId = StringId.None;
		return false;
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetRichPresence(Dictionary<string, string> tokens)
	{
	}

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06000696 RID: 1686 RVA: 0x00015EF2 File Offset: 0x000140F2
	public StringId DeleteCloudGameStringId
	{
		get
		{
			return StringId.DeleteSpecificJournalPrompt_iCloud;
		}
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x000020AA File Offset: 0x000002AA
	public bool AllowsTimedChallengeMessages()
	{
		return true;
	}

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06000698 RID: 1688 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsEvergreenButton
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06000699 RID: 1689 RVA: 0x0000222C File Offset: 0x0000042C
	public StringId TenYearCelebrationPopupBody
	{
		get
		{
			return StringId.None;
		}
	}

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x0600069A RID: 1690 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public string TenYearCelebrationMiniMetroStoreLink
	{
		get
		{
			return null;
		}
	}

	// Token: 0x04000297 RID: 663
	[Dependency]
	protected IHardwareCapabilities _hardwareCapabilities;
}
