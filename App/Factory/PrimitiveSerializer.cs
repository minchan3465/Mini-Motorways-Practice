using System;
using System.Collections.Generic;

namespace Factory
{
	// Token: 0x020002FD RID: 765
	public abstract class PrimitiveSerializer : ISerializer
	{
		// Token: 0x060012BE RID: 4798
		public abstract object Deserialize(object existingObj, ImportContext context);

		// Token: 0x060012BF RID: 4799
		public abstract bool Serialize(object obj, ExportContext context);

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x060012C0 RID: 4800 RVA: 0x0000222C File Offset: 0x0000042C
		public bool CanNestObjects
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x0003E564 File Offset: 0x0003C764
		public IEnumerable<object> GetNestedObjects(object obj)
		{
			yield break;
		}
	}
}
