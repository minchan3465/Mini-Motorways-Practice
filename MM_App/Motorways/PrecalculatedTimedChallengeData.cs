using System;

namespace Motorways
{
	// Token: 0x02000356 RID: 854
	[Serializable]
	public class PrecalculatedTimedChallengeData
	{
		// Token: 0x04001171 RID: 4465
		public string name;

		// Token: 0x04001172 RID: 4466
		public ChallengeData[] challenges;

		// Token: 0x04001173 RID: 4467
		public MapDefinition.CityNames city;

		// Token: 0x04001174 RID: 4468
		public bool overriden;

		// Token: 0x04001175 RID: 4469
		public bool serverOverride;
	}
}
