using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003CA RID: 970
	[CreateAssetMenu(fileName = "New PlayTogetherChallengeDatabase", menuName = "Motorways/Play Together/Play Together Challenge Database", order = 3)]
	public class PlayTogetherChallengeDatabase : ScriptableObject, IEnumerable<PlayTogetherChallengeDatabase.Challenge>, IEnumerable
	{
		// Token: 0x06001707 RID: 5895 RVA: 0x00053D68 File Offset: 0x00051F68
		public bool TryGetChallenge(string activityName, out PlayTogetherChallengeDatabase.Challenge challenge)
		{
			if (this.challengeLookup == null)
			{
				this.challengeLookup = new Dictionary<string, PlayTogetherChallengeDatabase.Challenge>();
				foreach (PlayTogetherChallengeDatabase.Challenge c in this.challenges)
				{
					this.challengeLookup.Add(c.ChallengeId, c);
				}
			}
			return this.challengeLookup.TryGetValue(activityName, out challenge);
		}

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06001708 RID: 5896 RVA: 0x00053DE8 File Offset: 0x00051FE8
		public int Count
		{
			get
			{
				return this.challenges.Count;
			}
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x00053DF5 File Offset: 0x00051FF5
		public IEnumerator<PlayTogetherChallengeDatabase.Challenge> GetEnumerator()
		{
			return this.challenges.GetEnumerator();
		}

		// Token: 0x0600170A RID: 5898 RVA: 0x00053DF5 File Offset: 0x00051FF5
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.challenges.GetEnumerator();
		}

		// Token: 0x040013BB RID: 5051
		[SerializeField]
		private List<PlayTogetherChallengeDatabase.Challenge> challenges;

		// Token: 0x040013BC RID: 5052
		private Dictionary<string, PlayTogetherChallengeDatabase.Challenge> challengeLookup;

		// Token: 0x020003CB RID: 971
		[Serializable]
		public class Challenge
		{
			// Token: 0x1700045A RID: 1114
			// (get) Token: 0x0600170C RID: 5900 RVA: 0x00053E07 File Offset: 0x00052007
			public string MapName
			{
				get
				{
					return this.mapName;
				}
			}

			// Token: 0x1700045B RID: 1115
			// (get) Token: 0x0600170D RID: 5901 RVA: 0x00053E0F File Offset: 0x0005200F
			public string ChallengeId
			{
				get
				{
					return this.challengeId;
				}
			}

			// Token: 0x1700045C RID: 1116
			// (get) Token: 0x0600170E RID: 5902 RVA: 0x00053E17 File Offset: 0x00052017
			public GameMode GameMode
			{
				get
				{
					return this.gameMode;
				}
			}

			// Token: 0x040013BD RID: 5053
			[SerializeField]
			private string challengeId;

			// Token: 0x040013BE RID: 5054
			[SerializeField]
			private string mapName;

			// Token: 0x040013BF RID: 5055
			[SerializeField]
			private GameMode gameMode;
		}
	}
}
