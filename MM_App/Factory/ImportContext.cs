using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Factory
{
	// Token: 0x020002F8 RID: 760
	public class ImportContext
	{
		// Token: 0x060012A0 RID: 4768 RVA: 0x0003E3F4 File Offset: 0x0003C5F4
		public ImportContext(BinaryReader reader, IScope scope)
		{
			this._reader = reader;
			this._scope = scope;
			this._objectLibrary.Add(null);
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x060012A1 RID: 4769 RVA: 0x0003E42C File Offset: 0x0003C62C
		public BinaryReader Reader
		{
			get
			{
				return this._reader;
			}
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x060012A2 RID: 4770 RVA: 0x0003E434 File Offset: 0x0003C634
		public IScope Scope
		{
			get
			{
				return this._scope;
			}
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x0003E43C File Offset: 0x0003C63C
		public void AddObject(object obj)
		{
			this._objectLibrary.Add(obj);
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x0003E44A File Offset: 0x0003C64A
		public object GetObject(int objectIndex)
		{
			if (Diagnostics.Verify(objectIndex < this._objectLibrary.Count, "Cannot find object with index {0}, as the library contains only {1}.", objectIndex, this._objectLibrary.Count))
			{
				return this._objectLibrary[objectIndex];
			}
			return null;
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x0003E48A File Offset: 0x0003C68A
		public void AddUnmappedDictionary(IDictionary dictionary, List<object> keys, List<object> values)
		{
			this._unmappedDictionaries.Add(new ImportContext.UnmappedDictionary(dictionary, keys, values));
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x0003E4A0 File Offset: 0x0003C6A0
		public void MapDictionaries()
		{
			foreach (ImportContext.UnmappedDictionary unmappedDictionary in this._unmappedDictionaries)
			{
				unmappedDictionary.Map();
			}
			this._unmappedDictionaries.Clear();
		}

		// Token: 0x04001023 RID: 4131
		private readonly BinaryReader _reader;

		// Token: 0x04001024 RID: 4132
		private readonly IScope _scope;

		// Token: 0x04001025 RID: 4133
		private readonly List<object> _objectLibrary = new List<object>();

		// Token: 0x04001026 RID: 4134
		private readonly List<ImportContext.UnmappedDictionary> _unmappedDictionaries = new List<ImportContext.UnmappedDictionary>();

		// Token: 0x020002F9 RID: 761
		private class UnmappedDictionary
		{
			// Token: 0x060012A7 RID: 4775 RVA: 0x0003E4FC File Offset: 0x0003C6FC
			public UnmappedDictionary(IDictionary dictionary, List<object> keys, List<object> values)
			{
				this._dictionary = dictionary;
				this._keys = keys;
				this._values = values;
			}

			// Token: 0x060012A8 RID: 4776 RVA: 0x0003E51C File Offset: 0x0003C71C
			public void Map()
			{
				int entryCount = this._keys.Count;
				for (int entryIndex = 0; entryIndex < entryCount; entryIndex++)
				{
					this._dictionary.Add(this._keys[entryIndex], this._values[entryIndex]);
				}
			}

			// Token: 0x04001027 RID: 4135
			private readonly IDictionary _dictionary;

			// Token: 0x04001028 RID: 4136
			private readonly List<object> _keys;

			// Token: 0x04001029 RID: 4137
			private readonly List<object> _values;
		}
	}
}
