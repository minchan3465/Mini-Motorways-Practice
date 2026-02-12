using System;
using Factory;
using Motorways.Themes;
using TMPro;
using UnityEngine;

// Token: 0x020001D3 RID: 467
public class LeaderboardPanelEntry : MonoBehaviour
{
	// Token: 0x06000B2A RID: 2858 RVA: 0x00025BF0 File Offset: 0x00023DF0
	public void InitializeWithScope(IScope scope)
	{
		this._scope = scope;
		this._localeDatabase = this._scope.Get<LocaleDatabase>();
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x00025C0C File Offset: 0x00023E0C
	public void SetAsBlankEntry(bool evenRow)
	{
		this._toggler.SetSelectedTheme(evenRow);
		this.rank.TextField.text = "";
		this.player.TextField.text = "";
		this.score.TextField.text = "";
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x00025C64 File Offset: 0x00023E64
	public void UpdateFromLeaderboardEntry(LeaderboardEntry fromEntry, bool evenRow, long totalLeaderboardEntryCount)
	{
		this._toggler.SetSelectedTheme(evenRow);
		string rankString;
		if (fromEntry.Rank == 0L)
		{
			rankString = "-";
			this.score.LocString = StandaloneLocString.CreateNonLocalizedString(this._scope, "-");
		}
		else
		{
			rankString = ((fromEntry.Rank != -1L) ? this._localeDatabase.CurrentLocale.FormatNumber(fromEntry.Rank) : "");
			this.score.LocString = StandaloneLocString.CreateLocalizedNumberString(this._scope, fromEntry.Score);
		}
		StandaloneLocString entryName;
		if (fromEntry.Type == LeaderboardEntryType.Local)
		{
			entryName = fromEntry.FormatLocalUserString(this._scope, totalLeaderboardEntryCount, (LeaderboardEntryFormatOptions)0);
			this.rank.TextField.fontStyle = FontStyles.Bold;
			this.player.TextField.fontStyle = FontStyles.Bold;
			this.score.TextField.fontStyle = FontStyles.Bold;
		}
		else
		{
			entryName = StandaloneLocString.CreateNonLocalizedString(this._scope, fromEntry.Name);
			this.rank.TextField.fontStyle = FontStyles.Normal;
			this.player.TextField.fontStyle = FontStyles.Normal;
			this.score.TextField.fontStyle = FontStyles.Normal;
		}
		this.rank.LocString = StandaloneLocString.CreateNonLocalizedString(this._scope, rankString);
		this.player.LocString = entryName;
	}

	// Token: 0x04000646 RID: 1606
	public const string NoRankSymbol = "-";

	// Token: 0x04000647 RID: 1607
	public LocalizedTextUI rank;

	// Token: 0x04000648 RID: 1608
	public LocalizedTextUI player;

	// Token: 0x04000649 RID: 1609
	public LocalizedTextUI score;

	// Token: 0x0400064A RID: 1610
	[SerializeField]
	private ThemeTypeToggler _toggler;

	// Token: 0x0400064B RID: 1611
	private IScope _scope;

	// Token: 0x0400064C RID: 1612
	private LocaleDatabase _localeDatabase;
}
