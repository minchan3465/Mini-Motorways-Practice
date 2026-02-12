using System;

namespace Factory
{
	// Token: 0x02000302 RID: 770
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class SerializeAttribute : Attribute
	{
		// Token: 0x170003BC RID: 956
		// (get) Token: 0x060012E2 RID: 4834 RVA: 0x0003EDA1 File Offset: 0x0003CFA1
		// (set) Token: 0x060012E3 RID: 4835 RVA: 0x0003EDA9 File Offset: 0x0003CFA9
		public bool IsSerialized { get; private set; }

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x060012E4 RID: 4836 RVA: 0x0003EDB2 File Offset: 0x0003CFB2
		// (set) Token: 0x060012E5 RID: 4837 RVA: 0x0003EDBA File Offset: 0x0003CFBA
		public ISerializer CustomSerializer { get; private set; }

		// Token: 0x060012E6 RID: 4838 RVA: 0x0003EDC3 File Offset: 0x0003CFC3
		public SerializeAttribute(bool serialize = true, Type serializer = null)
		{
			this.IsSerialized = serialize;
			if (serializer != null)
			{
				this.CustomSerializer = (Activator.CreateInstance(serializer) as ISerializer);
			}
		}
	}
}
