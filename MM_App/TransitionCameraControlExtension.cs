using System;

// Token: 0x02000196 RID: 406
public static class TransitionCameraControlExtension
{
	// Token: 0x0600091C RID: 2332 RVA: 0x0001DABF File Offset: 0x0001BCBF
	public static bool Contains(this TransitionCameraControl superset, TransitionCameraControl subset)
	{
		return (superset & subset) == subset;
	}
}
