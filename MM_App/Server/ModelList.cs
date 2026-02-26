using System;
using System.Collections.Generic;

namespace Server
{
	// Token: 0x0200028C RID: 652
	public struct ModelList<T> where T : class, IModel
	{
		// Token: 0x0600100B RID: 4107 RVA: 0x00035EF0 File Offset: 0x000340F0
		public ModelList(IList<IModel> models)
		{
			this._models = models;
		}

		// Token: 0x17000341 RID: 833
		public T this[int index]
		{
			get
			{
				if (Diagnostics.Verify(this._models != null, "Object not set to a reference.") && Diagnostics.Verify(index < this._models.Count && index >= 0, "Index out of range in ModelList."))
				{
					return this._models[index] as T;
				}
				return default(T);
			}
		}

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x0600100D RID: 4109 RVA: 0x00035F62 File Offset: 0x00034162
		public int Count
		{
			get
			{
				if (this._models != null)
				{
					return this._models.Count;
				}
				return 0;
			}
		}

		// Token: 0x0600100E RID: 4110 RVA: 0x00035F79 File Offset: 0x00034179
		public ModelListEnumerator<T> GetEnumerator()
		{
			return new ModelListEnumerator<T>(this._models);
		}

		// Token: 0x04000E42 RID: 3650
		private IList<IModel> _models;
	}
}
