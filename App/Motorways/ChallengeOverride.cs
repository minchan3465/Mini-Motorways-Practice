using System;

namespace Motorways
{
	// Token: 0x02000349 RID: 841
	public class ChallengeOverride
	{
		// Token: 0x060014B7 RID: 5303 RVA: 0x00044A0F File Offset: 0x00042C0F
		public ChallengeOverride(int timestamp, string cityName, string[] challengeNames)
		{
			this.timestamp = timestamp;
			this.cityName = cityName;
			this.challengeNames = challengeNames;
		}

		// Token: 0x04001132 RID: 4402
		public readonly int timestamp;

		// Token: 0x04001133 RID: 4403
		public readonly string cityName;

		// Token: 0x04001134 RID: 4404
		public readonly string[] challengeNames;
	}
}
