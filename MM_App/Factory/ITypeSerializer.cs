using System;

namespace Factory
{
	// Token: 0x0200031F RID: 799
	public interface ITypeSerializer : ISerializer
	{
		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06001360 RID: 4960
		Type Type { get; }

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06001361 RID: 4961
		int TypeId { get; }

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06001362 RID: 4962
		int Version { get; }
	}
}
