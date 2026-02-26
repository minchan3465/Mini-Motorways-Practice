using System;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000353 RID: 851
	[CreateAssetMenu(fileName = "New Challenge", menuName = "Motorways/Challenges/CityChallenge", order = 3)]
	public class CityChallengeData : ScriptableObject
	{
		// Token: 0x04001166 RID: 4454
		public ChallengeData[] challenges;

		// Token: 0x04001167 RID: 4455
		[EnumSearch(typeof(StringId), true)]
		public string titleStringId;

		// Token: 0x04001168 RID: 4456
		[EnumSearch(typeof(StringId), true)]
		public string descriptionStringId;

		// Token: 0x04001169 RID: 4457
		public int targetScore;
	}
}
