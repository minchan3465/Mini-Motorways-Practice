using System;
using Factory;
using Factory.Pools;

// Token: 0x02000075 RID: 117
[Factory.Serializable(1)]
public abstract class AppCommand : IAppCommand, IReusable
{
	// Token: 0x1700002E RID: 46
	// (get) Token: 0x0600011D RID: 285 RVA: 0x00004C61 File Offset: 0x00002E61
	// (set) Token: 0x0600011E RID: 286 RVA: 0x00004C69 File Offset: 0x00002E69
	[Serialize(true, null)]
	public float Timestamp { get; protected set; }

	// Token: 0x0600011F RID: 287
	public abstract bool Execute(IApp receiver);

	// Token: 0x06000120 RID: 288
	public abstract void Reset();
}
