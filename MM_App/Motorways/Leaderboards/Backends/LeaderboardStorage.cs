using System;
using System.Collections.Generic;

namespace Motorways.Leaderboards.Backends
{
	// Token: 0x0200078A RID: 1930
	internal class LeaderboardStorage
	{
		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x06003565 RID: 13669 RVA: 0x000F982A File Offset: 0x000F7A2A
		public LeaderboardEntry LocalEntry
		{
			get
			{
				return this.entries[this.localEntryIndex];
			}
		}

		// Token: 0x06003566 RID: 13670 RVA: 0x000F9840 File Offset: 0x000F7A40
		public void InsertOrUpdateEntry(string name, LeaderboardEntryType entryType, int score, int context)
		{
			score = Math.Max(score, this.LocalEntry.Score);
			int timeStamp;
			LeaderboardScoreState scoreState;
			LeaderboardService.DecodeScoreContext(context, out timeStamp, out scoreState);
			LeaderboardEntry newLeaderboardEntry = LeaderboardEntry.TestEntry(name, entryType, score, -1L, timeStamp, scoreState);
			int existingEntryIndex = this.entries.IndexOf(newLeaderboardEntry);
			if (existingEntryIndex != -1)
			{
				if (this.entries[existingEntryIndex].ScoreState == LeaderboardScoreState.Locked)
				{
					return;
				}
				this.entries.RemoveAt(existingEntryIndex);
			}
			int insertionIndex = this.entries.FindIndex((LeaderboardEntry entry) => newLeaderboardEntry.Score >= entry.Score);
			if (insertionIndex == -1)
			{
				this.entries.Add(newLeaderboardEntry);
			}
			else
			{
				this.entries.Insert(insertionIndex, newLeaderboardEntry);
			}
			for (int entryIndex = 0; entryIndex < this.entries.Count; entryIndex++)
			{
				LeaderboardEntry entry2 = this.entries[entryIndex];
				entry2.Rank = (long)(entryIndex + 1);
				this.entries[entryIndex] = entry2;
			}
			if (entryType == LeaderboardEntryType.Local)
			{
				this.localEntryIndex = this.entries.IndexOf(newLeaderboardEntry);
			}
		}

		// Token: 0x04002D6D RID: 11629
		private const int InvalidEntryIndex = -1;

		// Token: 0x04002D6E RID: 11630
		private static readonly List<LeaderboardEntry> DefaultTopEntries = new List<LeaderboardEntry>
		{
			LeaderboardEntry.TestEntry("Amazing Anton", LeaderboardEntryType.Global, 1000, 1L, 0, LeaderboardScoreState.Editable),
			LeaderboardEntry.TestEntry("Better Betty", LeaderboardEntryType.Global, 800, 2L, 0, LeaderboardScoreState.Editable),
			LeaderboardEntry.TestEntry("Counted Casey", LeaderboardEntryType.Global, 700, 3L, 0, LeaderboardScoreState.Editable),
			LeaderboardEntry.TestEntry("Delectable Darren", LeaderboardEntryType.Global, 600, 4L, 0, LeaderboardScoreState.Editable),
			LeaderboardEntry.TestEntry("Terrible Tom", LeaderboardEntryType.Global, 400, 5L, 0, LeaderboardScoreState.Editable),
			LeaderboardEntry.TestEntry("Medicore Misty", LeaderboardEntryType.Global, 334, 6L, 0, LeaderboardScoreState.Editable),
			LeaderboardEntry.TestEntry("Tried-hard Tane", LeaderboardEntryType.Global, 333, 7L, 0, LeaderboardScoreState.Editable),
			LeaderboardEntry.TestEntry("Lacklustre Lurk", LeaderboardEntryType.Global, 332, 8L, 0, LeaderboardScoreState.Editable),
			LeaderboardEntry.TestEntry("Racing Ramona", LeaderboardEntryType.Global, 331, 9L, 0, LeaderboardScoreState.Editable),
			LeaderboardEntry.TestEntry("Lets-Go Lucy", LeaderboardEntryType.Global, 20, 10L, 0, LeaderboardScoreState.Editable),
			LeaderboardEntry.TestEntry("Keep-it-up Kim", LeaderboardEntryType.Global, 15, 11L, 0, LeaderboardScoreState.Editable),
			LeaderboardEntry.TestEntry("Participation Patrick", LeaderboardEntryType.Global, 10, 12L, 0, LeaderboardScoreState.Editable),
			LeaderboardEntry.TestEntry("Test User", LeaderboardEntryType.Local, 1, 13L, 0, LeaderboardScoreState.NotSubmitted)
		};

		// Token: 0x04002D6F RID: 11631
		public readonly List<LeaderboardEntry> entries = new List<LeaderboardEntry>(LeaderboardStorage.DefaultTopEntries);

		// Token: 0x04002D70 RID: 11632
		public int localEntryIndex = LeaderboardStorage.DefaultTopEntries.Count - 1;
	}
}
