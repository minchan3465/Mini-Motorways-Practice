using System;

namespace Factory
{
	// Token: 0x02000301 RID: 769
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class SerializableAttribute : Attribute
	{
		// Token: 0x170003BB RID: 955
		// (get) Token: 0x060012DF RID: 4831 RVA: 0x0003ED81 File Offset: 0x0003CF81
		// (set) Token: 0x060012E0 RID: 4832 RVA: 0x0003ED89 File Offset: 0x0003CF89
		public int Version { get; private set; }

		// Token: 0x060012E1 RID: 4833 RVA: 0x0003ED92 File Offset: 0x0003CF92
		public SerializableAttribute(int version = 1)
		{
			this.Version = version;
		}
	}
}
