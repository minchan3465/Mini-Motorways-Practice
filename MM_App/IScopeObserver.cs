using System;
using Factory;

// Token: 0x02000132 RID: 306
public interface IScopeObserver
{
	// Token: 0x060006FF RID: 1791
	void OnScopeReleased(IScope scopeBeingReleased);
}
