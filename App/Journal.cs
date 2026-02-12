using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;

// Token: 0x0200017A RID: 378
[Factory.Serializable(1)]
public class Journal<T> : IReleasedFromScopeHandler, IReusable
{
	// Token: 0x0600087E RID: 2174 RVA: 0x0001A6AA File Offset: 0x000188AA
	public void Record(T entry)
	{
		this._entries.Add(entry);
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0001A6B8 File Offset: 0x000188B8
	public T GetEntry(int entryIndex)
	{
		return this._entries[entryIndex];
	}

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x06000880 RID: 2176 RVA: 0x0001A6C6 File Offset: 0x000188C6
	public int EntryCount
	{
		get
		{
			return this._entries.Count;
		}
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x0001A6D3 File Offset: 0x000188D3
	public void Clear()
	{
		this._entries.Clear();
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0001A6E0 File Offset: 0x000188E0
	public void OnReleasedFromScope(IScope scope)
	{
		foreach (T entry in this._entries)
		{
			scope.Release(entry);
		}
		this._entries.Clear();
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x0001A744 File Offset: 0x00018944
	public void Reset()
	{
		this.Clear();
	}

	// Token: 0x040003EF RID: 1007
	private List<T> _entries = new List<T>();
}
