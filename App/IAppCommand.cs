using System;
using Factory;
using Factory.Pools;

// Token: 0x02000159 RID: 345
[Factory.Serializable(1)]
public interface IAppCommand : IReusable
{
	// Token: 0x170001AB RID: 427
	// (get) Token: 0x0600078E RID: 1934
	float Timestamp { get; }

	// Token: 0x0600078F RID: 1935
	bool Execute(IApp receiver);
}
