using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Factory;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class iOSSoftwareCapabilities : ISoftwareCapabilities
{
	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06000645 RID: 1605 RVA: 0x00015EF6 File Offset: 0x000140F6
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return this._hardwareCapabilities.PreferredLocaleId;
		}
	}

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000646 RID: 1606 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsCloudSaves
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000647 RID: 1607 RVA: 0x00015F03 File Offset: 0x00014103
	public bool CanShareImage
	{
		get
		{
			return iOSSoftwareCapabilities.iOSShareAPI.CanShareImage();
		}
	}

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000648 RID: 1608 RVA: 0x00015E46 File Offset: 0x00014046
	public Vector2Int ScreenshotDimensions
	{
		get
		{
			return new Vector2Int(Screen.width, Screen.height);
		}
	}

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000649 RID: 1609 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsHighDPI
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x0600064A RID: 1610 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsMultipleProfiles
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x0600064B RID: 1611 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsMovieScreen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x0600064C RID: 1612 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsDisplayOptions
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInMainMenuScreen(bool isInMainMenuScreen)
	{
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetIsInGame(bool isInGame)
	{
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void OnAppStart()
	{
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnAppShutdown()
	{
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x00015F0C File Offset: 0x0001410C
	public bool SaveScreenshot(Texture2D screenshot, string tag, string parentFolder, out StringId messageId)
	{
		byte[] pngData = screenshot.EncodeToPNG();
		GCHandle pinnedPngData = GCHandle.Alloc(pngData, GCHandleType.Pinned);
		iOSSoftwareCapabilities.iOSShareAPI.ShareImage(pinnedPngData.AddrOfPinnedObject(), pngData.Length);
		pinnedPngData.Free();
		messageId = StringId.None;
		return true;
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x00015F44 File Offset: 0x00014144
	public bool SaveGif(byte[] data, string tag, string parentFolder, out StringId messageId, out StringId messageHeaderId)
	{
		GCHandle pinnedGifData = GCHandle.Alloc(data, GCHandleType.Pinned);
		iOSSoftwareCapabilities.iOSShareAPI.ShareImage(pinnedGifData.AddrOfPinnedObject(), data.Length);
		pinnedGifData.Free();
		messageId = StringId.None;
		messageHeaderId = StringId.None;
		return true;
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x000022F5 File Offset: 0x000004F5
	public void SetRichPresence(Dictionary<string, string> tokens)
	{
	}

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000654 RID: 1620 RVA: 0x00015EF2 File Offset: 0x000140F2
	public StringId DeleteCloudGameStringId
	{
		get
		{
			return StringId.DeleteSpecificJournalPrompt_iCloud;
		}
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x000020AA File Offset: 0x000002AA
	public bool AllowsTimedChallengeMessages()
	{
		return true;
	}

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000656 RID: 1622 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsEvergreenButton
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x06000657 RID: 1623 RVA: 0x00015F79 File Offset: 0x00014179
	public StringId TenYearCelebrationPopupBody
	{
		get
		{
			return StringId.Popup_Body_CrossPromo_AuroraBorealis;
		}
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000658 RID: 1624 RVA: 0x00015F80 File Offset: 0x00014180
	public string TenYearCelebrationMiniMetroStoreLink
	{
		get
		{
			return "https://apple.co/-MiniMetro";
		}
	}

	// Token: 0x04000295 RID: 661
	[Dependency]
	protected IHardwareCapabilities _hardwareCapabilities;

	// Token: 0x0200011F RID: 287
	private static class iOSShareAPI
	{
		// Token: 0x0600065A RID: 1626 RVA: 0x000020AA File Offset: 0x000002AA
		public static bool ShareImage(IntPtr imageData, int imageDataLength)
		{
			return true;
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x000020AA File Offset: 0x000002AA
		public static bool CanShareImage()
		{
			return true;
		}
	}
}
