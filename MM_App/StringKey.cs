using System;
using System.Collections.Generic;
using Factory.Pools;

// Token: 0x0200018A RID: 394
public abstract class StringKey : IReusable
{
	// Token: 0x060008EB RID: 2283 RVA: 0x0001D6AE File Offset: 0x0001B8AE
	public override bool Equals(object obj)
	{
		return obj is StringKey && this == (StringKey)obj;
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x0001D6C6 File Offset: 0x0001B8C6
	public static bool operator ==(StringKey x, StringKey y)
	{
		if (x == null)
		{
			return y == null;
		}
		return x.Equals(y);
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x0001D6D7 File Offset: 0x0001B8D7
	public static bool operator !=(StringKey x, StringKey y)
	{
		return !(x == y);
	}

	// Token: 0x060008EE RID: 2286
	public abstract bool Equals(StringKey other);

	// Token: 0x060008EF RID: 2287
	public abstract override int GetHashCode();

	// Token: 0x060008F0 RID: 2288
	public abstract string GetStringId();

	// Token: 0x060008F1 RID: 2289
	public abstract Dictionary<string, string> GetParameters();

	// Token: 0x060008F2 RID: 2290
	public abstract int GetCount();

	// Token: 0x060008F3 RID: 2291
	public abstract bool IsPlural();

	// Token: 0x060008F4 RID: 2292
	public abstract void InitWithStringId(StringId stringId);

	// Token: 0x060008F5 RID: 2293
	public abstract void InitWithStringId(StringId stringId, int newCount, Dictionary<string, string> newParameters = null);

	// Token: 0x060008F6 RID: 2294
	public abstract void InitWithStringId(StringId stringId, float newCount, Dictionary<string, string> newParameters = null);

	// Token: 0x060008F7 RID: 2295
	public abstract void InitWithString(string stringKey);

	// Token: 0x060008F8 RID: 2296
	public abstract void InitWithString(string stringKey, int newCount, Dictionary<string, string> newParameters = null);

	// Token: 0x060008F9 RID: 2297
	public abstract void InitWithString(string stringKey, float newCount, Dictionary<string, string> newParameters = null);

	// Token: 0x060008FA RID: 2298
	public abstract void InitWithNonLocalizedString(string nonLocalizedString);

	// Token: 0x060008FB RID: 2299
	public abstract void Reset();
}
