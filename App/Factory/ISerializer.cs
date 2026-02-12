using System;
using System.Collections.Generic;

namespace Factory
{
	// Token: 0x020002FC RID: 764
	public interface ISerializer
	{
		// Token: 0x060012BA RID: 4794
		bool Serialize(object obj, ExportContext context);

		// Token: 0x060012BB RID: 4795
		object Deserialize(object existingObj, ImportContext context);

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x060012BC RID: 4796
		bool CanNestObjects { get; }

		// Token: 0x060012BD RID: 4797
		IEnumerable<object> GetNestedObjects(object obj);
	}
}
