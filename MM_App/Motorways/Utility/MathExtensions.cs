using System;

namespace Motorways.Utility
{
	// Token: 0x02000466 RID: 1126
	public static class MathExtensions
	{
		// Token: 0x06001C33 RID: 7219 RVA: 0x0006850C File Offset: 0x0006670C
		public static int Mod(int a, int n)
		{
			int r = a % n;
			if (r >= 0)
			{
				return r;
			}
			return r + n;
		}
	}
}
