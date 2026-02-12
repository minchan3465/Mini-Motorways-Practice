using System;

// Token: 0x02000188 RID: 392
public class LocalizedString
{
	// Token: 0x060008D3 RID: 2259 RVA: 0x0001D130 File Offset: 0x0001B330
	public LocalizedString(Locale newLocale, string newLocalString)
	{
		this.locale = newLocale;
		this.localString = newLocalString;
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x0001D146 File Offset: 0x0001B346
	public override bool Equals(object obj)
	{
		return obj is LocalizedString && this.Equals((LocalizedString)obj);
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x0001D15E File Offset: 0x0001B35E
	public bool Equals(LocalizedString obj)
	{
		return this.locale == obj.locale && this.localString == obj.localString;
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x0001D181 File Offset: 0x0001B381
	public override int GetHashCode()
	{
		return this.locale.GetHashCode() ^ this.localString.GetHashCode();
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x0001D19A File Offset: 0x0001B39A
	public static bool operator ==(LocalizedString x, LocalizedString y)
	{
		if (x == null || y == null)
		{
			return x == null && y == null;
		}
		return x.Equals(y);
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x0001D1B3 File Offset: 0x0001B3B3
	public static bool operator !=(LocalizedString x, LocalizedString y)
	{
		return !(x == y);
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x0001D1BF File Offset: 0x0001B3BF
	public override string ToString()
	{
		return this.localString;
	}

	// Token: 0x04000470 RID: 1136
	public Locale locale;

	// Token: 0x04000471 RID: 1137
	public string localString;
}
