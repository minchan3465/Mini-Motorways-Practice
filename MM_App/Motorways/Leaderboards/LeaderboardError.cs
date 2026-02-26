using System;

namespace Motorways.Leaderboards
{
	// Token: 0x02000766 RID: 1894
	public class LeaderboardError
	{
		// Token: 0x170008B5 RID: 2229
		// (get) Token: 0x060034B2 RID: 13490 RVA: 0x000F71E3 File Offset: 0x000F53E3
		// (set) Token: 0x060034B3 RID: 13491 RVA: 0x000F71EB File Offset: 0x000F53EB
		public LeaderboardErrorCode Code { get; private set; }

		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x060034B4 RID: 13492 RVA: 0x000F71F4 File Offset: 0x000F53F4
		// (set) Token: 0x060034B5 RID: 13493 RVA: 0x000F71FC File Offset: 0x000F53FC
		public StringId Description { get; private set; }

		// Token: 0x060034B6 RID: 13494 RVA: 0x000F7205 File Offset: 0x000F5405
		public LeaderboardError(LeaderboardErrorCode code, StringId description = StringId.None)
		{
			this.Code = code;
			this.Description = description;
		}

		// Token: 0x060034B7 RID: 13495 RVA: 0x000F721B File Offset: 0x000F541B
		public override string ToString()
		{
			return string.Format("[LeaderboardError code={0} Description={1}]", this.Code, this.Description);
		}
	}
}
