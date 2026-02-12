using System;

namespace Motorways.Audio
{
	// Token: 0x02000640 RID: 1600
	public static class Dbug
	{
		// Token: 0x06002CD0 RID: 11472 RVA: 0x000CF764 File Offset: 0x000CD964
		public static bool Assert(bool condition)
		{
			return Diagnostics.Verify(condition);
		}

		// Token: 0x06002CD1 RID: 11473 RVA: 0x000CF76C File Offset: 0x000CD96C
		public static bool Assert(bool condition, string message, object[] args = null)
		{
			return Diagnostics.Verify(condition, message, args);
		}

		// Token: 0x04002730 RID: 10032
		public static readonly Diagnostics.Log.Channel Log = AudioSystem.Log;
	}
}
