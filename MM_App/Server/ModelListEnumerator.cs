using System;
using System.Collections.Generic;

namespace Server
{
	// Token: 0x0200028D RID: 653
	public struct ModelListEnumerator<T> where T : class, IModel
	{
		// Token: 0x0600100F RID: 4111 RVA: 0x00035F86 File Offset: 0x00034186
		public ModelListEnumerator(IList<IModel> models)
		{
			this._index = -1;
			this._models = models;
		}

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06001010 RID: 4112 RVA: 0x00035F96 File Offset: 0x00034196
		public T Current
		{
			get
			{
				return this._models[this._index] as T;
			}
		}

		// Token: 0x06001011 RID: 4113 RVA: 0x00035FB3 File Offset: 0x000341B3
		public bool MoveNext()
		{
			if (this._models != null && this._index + 1 < this._models.Count)
			{
				this._index++;
				return true;
			}
			return false;
		}

		// Token: 0x04000E43 RID: 3651
		private int _index;

		// Token: 0x04000E44 RID: 3652
		private IList<IModel> _models;
	}
}
