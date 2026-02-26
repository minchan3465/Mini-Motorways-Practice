using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

// Token: 0x02000124 RID: 292
public class WeGameSoftwareCapabilities : ISoftwareCapabilities
{
	// Token: 0x17000179 RID: 377
	// (get) Token: 0x060006B1 RID: 1713 RVA: 0x000160A9 File Offset: 0x000142A9
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return this._hardwareCapabilities.PreferredLocaleId;
		}
	}

	// Token: 0x1700017A RID: 378
	// (get) Token: 0x060006B2 RID: 1714 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsCloudSaves
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700017B RID: 379
	// (get) Token: 0x060006B3 RID: 1715 RVA: 0x0000222C File Offset: 0x0000042C
	public bool CanShareImage
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700017C RID: 380
	// (get) Token: 0x060006B4 RID: 1716 RVA: 0x00015E46 File Offset: 0x00014046
	public Vector2Int ScreenshotDimensions
	{
		get
		{
			return new Vector2Int(Screen.width, Screen.height);
		}
	}

	// Token: 0x1700017D RID: 381
	// (get) Token: 0x060006B5 RID: 1717 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsHighDPI
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700017E RID: 382
	// (get) Token: 0x060006B6 RID: 1718 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsMultipleProfiles
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x060006B7 RID: 1719 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsMovieScreen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000180 RID: 384
	// (get) Token: 0x060006B8 RID: 1720 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsDisplayOptions
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInMainMenuScreen(bool isInMainMenuScreen)
	{
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInGame(bool isInGame)
	{
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void OnAppStart()
	{
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnAppShutdown()
	{
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x000160B6 File Offset: 0x000142B6
	public bool SaveGif(byte[] data, string tag, string parentFolder, out StringId messageId, out StringId messageHeaderId)
	{
		messageId = StringId.None;
		messageHeaderId = StringId.None;
		throw new NotImplementedException();
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x000160C5 File Offset: 0x000142C5
	public bool SaveScreenshot(Texture2D screenshot, string tag, string parentFolder, out StringId messageId)
	{
		messageId = StringId.None;
		throw new NotImplementedException();
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetRichPresence(Dictionary<string, string> tokens)
	{
	}

	// Token: 0x17000181 RID: 385
	// (get) Token: 0x060006C0 RID: 1728 RVA: 0x0000222C File Offset: 0x0000042C
	public StringId DeleteCloudGameStringId
	{
		get
		{
			return StringId.None;
		}
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x0000222C File Offset: 0x0000042C
	public bool AllowsTimedChallengeMessages()
	{
		return false;
	}

	// Token: 0x17000182 RID: 386
	// (get) Token: 0x060006C2 RID: 1730 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsEvergreenButton
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000183 RID: 387
	// (get) Token: 0x060006C3 RID: 1731 RVA: 0x0000222C File Offset: 0x0000042C
	public StringId TenYearCelebrationPopupBody
	{
		get
		{
			return StringId.None;
		}
	}

	// Token: 0x17000184 RID: 388
	// (get) Token: 0x060006C4 RID: 1732 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public string TenYearCelebrationMiniMetroStoreLink
	{
		get
		{
			return null;
		}
	}

	// Token: 0x04000299 RID: 665
	[Dependency]
	protected IHardwareCapabilities _hardwareCapabilities;
}
