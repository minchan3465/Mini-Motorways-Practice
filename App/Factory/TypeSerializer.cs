using System;

namespace Factory
{
	// Token: 0x02000320 RID: 800
	public class TypeSerializer<T> : CompositeSerializer, ITypeSerializer, ISerializer where T : class
	{
		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06001363 RID: 4963 RVA: 0x0004037F File Offset: 0x0003E57F
		// (set) Token: 0x06001364 RID: 4964 RVA: 0x00040387 File Offset: 0x0003E587
		public Type Type { get; private set; }

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06001365 RID: 4965 RVA: 0x00040390 File Offset: 0x0003E590
		// (set) Token: 0x06001366 RID: 4966 RVA: 0x00040398 File Offset: 0x0003E598
		public int TypeId { get; private set; }

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06001367 RID: 4967 RVA: 0x000403A1 File Offset: 0x0003E5A1
		// (set) Token: 0x06001368 RID: 4968 RVA: 0x000403A9 File Offset: 0x0003E5A9
		public int Version { get; private set; }

		// Token: 0x06001369 RID: 4969 RVA: 0x000403B4 File Offset: 0x0003E5B4
		public TypeSerializer() : base(typeof(T))
		{
			this.Type = typeof(T);
			this.TypeId = TypeUtilities.GetTypeId(this.Type);
			SerializableAttribute serializableAttribute = TypeUtilities.GetCustomAttribute<SerializableAttribute>(this.Type);
			this.Version = serializableAttribute.Version;
		}
	}
}
