using System;
using System.Collections.Generic;
using Factory;
using JetBrains.Annotations;
using UnityEngine;

namespace Motorways.Leaderboards
{
	// Token: 0x0200076E RID: 1902
	public class LeaderboardService
	{
		// Token: 0x060034D8 RID: 13528 RVA: 0x000F7838 File Offset: 0x000F5A38
		public void ClearLeaderboardEntryCache(LeaderboardId leaderboardId)
		{
			LeaderboardService.Log.Info("Clearing cache for {0}.", new object[]
			{
				leaderboardId
			});
			this._localLeaderboardEntryCache.Remove(leaderboardId);
			this._topLeaderboardEntryCache.Remove(leaderboardId);
			this._playerCenteredLeaderboardEntryCache.Remove(leaderboardId);
			this._topFriendFilteredLeaderboardEntryCache.Remove(leaderboardId);
			this._histogramCache.Remove(leaderboardId);
		}

		// Token: 0x060034D9 RID: 13529 RVA: 0x000F78A0 File Offset: 0x000F5AA0
		public AsyncRequestHandle RequestLocalEntry(LeaderboardId leaderboardId, [NotNull] LocalEntryRequestCompleted localEntryRequestCompleted)
		{
			LeaderboardService.Log.Info(string.Format("RequestLocalEntry for {0}", leaderboardId), Array.Empty<object>());
			LeaderboardService.CachedLeaderboardRequest cachedRequest;
			if (this._localLeaderboardEntryCache.TryGetValue(leaderboardId, out cachedRequest) && !cachedRequest.HasExpired && cachedRequest.Entries.Count > 0)
			{
				LeaderboardService.Log.Info(string.Format("Found entries for {0} in cache", leaderboardId), Array.Empty<object>());
				localEntryRequestCompleted(cachedRequest.Entries[0], cachedRequest.TotalEntryCount, null);
				return AsyncRequestHandle.CompletedRequestHandle;
			}
			AsyncRequestHandle localRequestHandle = new AsyncRequestHandle();
			this._leaderboardBackend.RequestLocalEntry(leaderboardId, delegate(LeaderboardEntry entry, long count, LeaderboardError error)
			{
				if (error == null)
				{
					if (entry != null)
					{
						DailyLeaderboardId daily = leaderboardId as DailyLeaderboardId;
						if (daily != null && entry.Timestamp != daily.Timestamp)
						{
							entry = null;
						}
					}
					this._localLeaderboardEntryCache.Remove(leaderboardId);
					this._localLeaderboardEntryCache.Add(leaderboardId, new LeaderboardService.CachedLeaderboardRequest(new List<LeaderboardEntry>
					{
						entry
					}, count));
				}
				if (localRequestHandle.IsActive)
				{
					localEntryRequestCompleted(entry, count, error);
				}
			});
			return localRequestHandle;
		}

		// Token: 0x060034DA RID: 13530 RVA: 0x000F7984 File Offset: 0x000F5B84
		public void SubmitScore(LeaderboardId leaderboardId, int score, LeaderboardScoreState scoreState)
		{
			if (scoreState == LeaderboardScoreState.NotSubmitted)
			{
				Diagnostics.FailAssert("Score state should never be set to NotSubmitted.", Array.Empty<object>());
				return;
			}
			if (!this.CanSubmitScoresOffline)
			{
				this._player.MotorwaysExtendedUserProfile.LogUnsubmittedScore(leaderboardId, score, scoreState);
			}
			if (leaderboardId is DailyLeaderboardId)
			{
				this.SubmitScoreWithDailyChallengeValidation(leaderboardId, score, scoreState);
				return;
			}
			this.SubmitScoreWithoutValidation(leaderboardId, score, scoreState);
		}

		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x060034DB RID: 13531 RVA: 0x000F79DA File Offset: 0x000F5BDA
		public bool CanSubmitScoresOffline
		{
			get
			{
				return this._leaderboardBackend.CanSubmitScoresOffline;
			}
		}

		// Token: 0x060034DC RID: 13532 RVA: 0x000F79E8 File Offset: 0x000F5BE8
		private void SubmitScoreWithDailyChallengeValidation(LeaderboardId leaderboardId, int score, LeaderboardScoreState scoreState)
		{
			LeaderboardService.Log.Info(string.Format("Making sure score is not locked before submitting: Leaderboard: {0}, Score: {1}, ScoreState: {2}", leaderboardId, score, scoreState), Array.Empty<object>());
			this.RequestLocalEntry(leaderboardId, delegate(LeaderboardEntry localEntry, long totalLeaderboardEntryCount, LeaderboardError error)
			{
				if (error != null)
				{
					LeaderboardService.Log.Info(string.Format("Not submitting score. Cannot verify that score state is not locked: Leaderboard: {0}, Score: {1}, ScoreState: {2}", leaderboardId, score, scoreState), Array.Empty<object>());
					return;
				}
				bool isScoreLocked = false;
				int currentScore = -1;
				if (localEntry != null)
				{
					isScoreLocked = (localEntry.ScoreState == LeaderboardScoreState.Locked);
					currentScore = localEntry.Score;
				}
				if (MotorwaysScoreValidation.ShouldRecordScore(isScoreLocked, currentScore, score))
				{
					this.SubmitScoreWithoutValidation(leaderboardId, score, scoreState);
					return;
				}
				this._player.MotorwaysExtendedUserProfile.MarkScoreAsSubmitted(leaderboardId);
			});
		}

		// Token: 0x060034DD RID: 13533 RVA: 0x000F7A68 File Offset: 0x000F5C68
		private void SubmitScoreWithoutValidation(LeaderboardId leaderboardId, int score, LeaderboardScoreState scoreState)
		{
			LeaderboardService.Log.Info("Submitting score of {0} to {1} with state {2}.", new object[]
			{
				score,
				leaderboardId,
				scoreState
			});
			this.ClearLeaderboardEntryCache(leaderboardId);
			this._leaderboardBackend.SubmitScore(leaderboardId, score, scoreState, this.GetSubmitRequestAction(leaderboardId, score));
		}

		// Token: 0x060034DE RID: 13534 RVA: 0x000F7ABD File Offset: 0x000F5CBD
		private SubmitScoreRequestCompleted GetSubmitRequestAction(LeaderboardId id, int score)
		{
			LocalEntryRequestCompleted <>9__1;
			return delegate(bool submittedSuccessfully)
			{
				ILeaderboardBackend leaderboardBackend = this._leaderboardBackend;
				LeaderboardId id2 = id;
				LocalEntryRequestCompleted localEntryRequestCompleted;
				if ((localEntryRequestCompleted = <>9__1) == null)
				{
					localEntryRequestCompleted = (<>9__1 = delegate(LeaderboardEntry entry, long count, LeaderboardError error)
					{
						if (error == null && entry != null && entry.Score >= score)
						{
							this._player.MotorwaysExtendedUserProfile.MarkScoreAsSubmitted(id);
						}
					});
				}
				leaderboardBackend.RequestLocalEntry(id2, localEntryRequestCompleted);
			};
		}

		// Token: 0x060034DF RID: 13535 RVA: 0x000F7AE4 File Offset: 0x000F5CE4
		public AsyncRequestHandle RequestTopEntries(LeaderboardId leaderboardId, int entryCount, [NotNull] EntryRequestCompleted entryRequestCompleted)
		{
			LeaderboardService.Log.Info("RequestTopEntries for {0}.", new object[]
			{
				leaderboardId
			});
			LeaderboardService.LeaderboardEntryRequestDelegate requestDelegate = new LeaderboardService.LeaderboardEntryRequestDelegate(this._leaderboardBackend.RequestTopEntries);
			Dictionary<LeaderboardId, LeaderboardService.CachedLeaderboardRequest> cache = this._topLeaderboardEntryCache;
			return this.RequestEntries(leaderboardId, entryCount, entryRequestCompleted, requestDelegate, cache);
		}

		// Token: 0x060034E0 RID: 13536 RVA: 0x000F7B30 File Offset: 0x000F5D30
		public AsyncRequestHandle RequestPlayerCenteredEntries(LeaderboardId leaderboardId, int entryCount, [NotNull] EntryRequestCompleted entryRequestCompleted)
		{
			LeaderboardService.Log.Info("RequestPlayerCenteredEntries for {0}.", new object[]
			{
				leaderboardId
			});
			LeaderboardService.LeaderboardEntryRequestDelegate requestDelegate = new LeaderboardService.LeaderboardEntryRequestDelegate(this._leaderboardBackend.RequestPlayerCenteredEntries);
			Dictionary<LeaderboardId, LeaderboardService.CachedLeaderboardRequest> cache = this._playerCenteredLeaderboardEntryCache;
			return this.RequestEntries(leaderboardId, entryCount, entryRequestCompleted, requestDelegate, cache);
		}

		// Token: 0x060034E1 RID: 13537 RVA: 0x000F7B7C File Offset: 0x000F5D7C
		public AsyncRequestHandle RequestTopFriendFilteredEntries(LeaderboardId leaderboardId, int entryCount, [NotNull] EntryRequestCompleted entryRequestCompleted)
		{
			LeaderboardService.Log.Info("RequestTopFriendFilteredEntries for {0}.", new object[]
			{
				leaderboardId
			});
			LeaderboardService.LeaderboardEntryRequestDelegate requestDelegate = new LeaderboardService.LeaderboardEntryRequestDelegate(this._leaderboardBackend.RequestTopFriendFilteredEntries);
			Dictionary<LeaderboardId, LeaderboardService.CachedLeaderboardRequest> cache = this._topFriendFilteredLeaderboardEntryCache;
			return this.RequestEntries(leaderboardId, entryCount, entryRequestCompleted, requestDelegate, cache);
		}

		// Token: 0x060034E2 RID: 13538 RVA: 0x000F7BC8 File Offset: 0x000F5DC8
		public AsyncRequestHandle RequestHistogram(LeaderboardId leaderboardId, [NotNull] HistogramRequestCompleted histogramRequestCompleted)
		{
			LeaderboardService.Log.Info("RequestHistogram for {0}.", new object[]
			{
				leaderboardId
			});
			LeaderboardService.CachedHistogramRequest cachedHistogram;
			if (this._histogramCache.TryGetValue(leaderboardId, out cachedHistogram) && !cachedHistogram.HasExpired)
			{
				LeaderboardService.Log.Info("Found histogram for {0} in cache.", new object[]
				{
					leaderboardId
				});
				histogramRequestCompleted(cachedHistogram.Buckets, cachedHistogram.BucketSize, null);
				return AsyncRequestHandle.CompletedRequestHandle;
			}
			AsyncRequestHandle requestHandle = new AsyncRequestHandle();
			this._histogramBackend.RequestHistogram(leaderboardId, delegate(List<int> buckets, int size, LeaderboardError error)
			{
				if (error == null)
				{
					this._histogramCache.Remove(leaderboardId);
					this._histogramCache.Add(leaderboardId, new LeaderboardService.CachedHistogramRequest(buckets, size));
				}
				if (requestHandle.IsActive)
				{
					histogramRequestCompleted(buckets, size, error);
				}
			});
			return requestHandle;
		}

		// Token: 0x060034E3 RID: 13539 RVA: 0x000F7C95 File Offset: 0x000F5E95
		public void PresentError([NotNull] LeaderboardError error)
		{
			this._leaderboardBackend.PresentError(error);
		}

		// Token: 0x060034E4 RID: 13540 RVA: 0x000F7CA3 File Offset: 0x000F5EA3
		public bool IsLeaderboardTypeSupported(LeaderboardType type)
		{
			return this._leaderboardBackend.IsLeaderboardTypeSupported(type);
		}

		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x060034E5 RID: 13541 RVA: 0x000F7CB1 File Offset: 0x000F5EB1
		public bool CanAuthenticate
		{
			get
			{
				return this._leaderboardBackend.CanAuthenticate;
			}
		}

		// Token: 0x060034E6 RID: 13542 RVA: 0x000F7CBE File Offset: 0x000F5EBE
		public bool Authenticate(AuthenticationCompleted authenticationCompleted)
		{
			return this._leaderboardBackend.Authenticate(authenticationCompleted);
		}

		// Token: 0x060034E7 RID: 13543 RVA: 0x000F7CCC File Offset: 0x000F5ECC
		private void LogInvalidLocalEntries(LeaderboardId leaderboardId, List<LeaderboardEntry> entries, LeaderboardEntry localEntry)
		{
			if (localEntry == null)
			{
				LeaderboardService.Log.Info(string.Format("No local entry - Leaderboard: {0}", leaderboardId), Array.Empty<object>());
				return;
			}
			if (localEntry.Rank == 0L)
			{
				LeaderboardService.Log.Info(string.Format("Unranked local entry: {0} - Leaderboard: {1}", localEntry, leaderboardId), Array.Empty<object>());
				return;
			}
			foreach (LeaderboardEntry entry in entries)
			{
				if (entry.Type != LeaderboardEntryType.Local)
				{
					if (localEntry.Rank < entry.Rank && localEntry.Score < entry.Score)
					{
						LeaderboardService.Log.Error(string.Format("Invalid local entry detected: Local entry is ranked higher than this entry, but the local entry's score is lower than this entry.\nLocalEntry: {0}\nOtherEntry: {1}\nLeaderboard: {2}", localEntry, entry, leaderboardId), Array.Empty<object>());
						break;
					}
					if (localEntry.Rank > entry.Rank && localEntry.Score > entry.Score)
					{
						LeaderboardService.Log.Error(string.Format("Invalid local entry detected: Local entry is ranked lower than this entry, but local entry's score is higher than this entry.\nLocalEntry: {0}\nOtherEntry: {1}\nLeaderboard: {2}", localEntry, entry, leaderboardId), Array.Empty<object>());
						break;
					}
				}
			}
		}

		// Token: 0x060034E8 RID: 13544 RVA: 0x000F7DD8 File Offset: 0x000F5FD8
		private AsyncRequestHandle RequestEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted, LeaderboardService.LeaderboardEntryRequestDelegate requestDelegate, Dictionary<LeaderboardId, LeaderboardService.CachedLeaderboardRequest> cache)
		{
			LeaderboardService.CachedLeaderboardRequest cachedRequest;
			if (cache.TryGetValue(leaderboardId, out cachedRequest) && !cachedRequest.HasExpired)
			{
				LeaderboardService.Log.Info("Found entries for {0} in cache.", new object[]
				{
					leaderboardId
				});
				entryRequestCompleted(cachedRequest.Entries, cachedRequest.TotalEntryCount, null);
				return AsyncRequestHandle.CompletedRequestHandle;
			}
			LeaderboardService.Log.Info("No cached entries found for {0}.", new object[]
			{
				leaderboardId
			});
			AsyncRequestHandle requestHandle = new AsyncRequestHandle();
			requestDelegate(leaderboardId, entryCount, delegate(List<LeaderboardEntry> entries, long totalLeaderboardEntryCount, LeaderboardError error)
			{
				if (error != null)
				{
					if (requestHandle.IsActive)
					{
						entryRequestCompleted(entries, totalLeaderboardEntryCount, error);
					}
					return;
				}
				if (!Diagnostics.Verify(entries != null, "Invalid state. Having no error implies we have valid entries, even if it's an empty list."))
				{
					if (requestHandle.IsActive)
					{
						entryRequestCompleted(entries, totalLeaderboardEntryCount, error);
					}
					return;
				}
				bool hasLocalScore = false;
				int localEntryIndex = 0;
				for (int entryIndex = 0; entryIndex < entries.Count; entryIndex++)
				{
					if (entries[entryIndex].Type == LeaderboardEntryType.Local)
					{
						hasLocalScore = true;
						localEntryIndex = entryIndex;
						break;
					}
				}
				if (hasLocalScore)
				{
					RecurringLeaderboardId recurringLeaderboardId = leaderboardId as RecurringLeaderboardId;
					if (recurringLeaderboardId != null)
					{
						LeaderboardEntry localEntry3 = entries[localEntryIndex];
						if (localEntry3.Timestamp != recurringLeaderboardId.Timestamp && recurringLeaderboardId.IsLeaderboardOpen())
						{
							LeaderboardService.Log.Info(string.Format("Local entry timestamp {0} does not match expected timestamp {1}. Ignoring local entry.", localEntry3.Timestamp, recurringLeaderboardId.Timestamp), Array.Empty<object>());
							hasLocalScore = false;
							entries.RemoveAt(localEntryIndex);
							localEntryIndex = 0;
						}
					}
				}
				string localEntryPresentString = hasLocalScore ? "present" : "not present";
				LeaderboardService.Log.Info(string.Format("Request received for {0}, local entry is {1}", leaderboardId, localEntryPresentString), Array.Empty<object>());
				if (hasLocalScore)
				{
					LeaderboardEntry localEntry2 = entries[localEntryIndex];
					if (localEntryIndex > entryCount)
					{
						if (entries.Count > entryCount)
						{
							entries.RemoveRange(entryCount, entries.Count - entryCount);
						}
						entries.Add(localEntry2);
					}
					this.LogInvalidLocalEntries(leaderboardId, entries, localEntry2);
					cache.Remove(leaderboardId);
					cache.Add(leaderboardId, new LeaderboardService.CachedLeaderboardRequest(entries, totalLeaderboardEntryCount));
					if (requestHandle.IsActive)
					{
						entryRequestCompleted(entries, totalLeaderboardEntryCount, error);
						return;
					}
				}
				else
				{
					this.RequestLocalEntry(leaderboardId, delegate(LeaderboardEntry localEntry, long localCount, LeaderboardError localRequestError)
					{
						if (localRequestError == null)
						{
							if (entries.Count > entryCount)
							{
								entries.RemoveRange(entryCount, entries.Count - entryCount);
							}
							if (localEntry == null)
							{
								localEntry = new LeaderboardEntry(string.Empty, string.Empty, LeaderboardEntryType.Local, 0, 0L, 0, LeaderboardScoreState.NotSubmitted);
							}
							entries.Add(localEntry);
							this.LogInvalidLocalEntries(leaderboardId, entries, localEntry);
							cache.Remove(leaderboardId);
							cache.Add(leaderboardId, new LeaderboardService.CachedLeaderboardRequest(entries, totalLeaderboardEntryCount));
						}
						else
						{
							LeaderboardService.Log.Warn("Failed to obtain local entry from {0} with error {1}.", new object[]
							{
								leaderboardId,
								localRequestError
							});
						}
						if (requestHandle.IsActive)
						{
							entryRequestCompleted(entries, totalLeaderboardEntryCount, error);
						}
					});
				}
			});
			return requestHandle;
		}

		// Token: 0x060034E9 RID: 13545 RVA: 0x000F7EB8 File Offset: 0x000F60B8
		public static int EncodeScoreContext(LeaderboardId leaderboardId, LeaderboardScoreState scoreState)
		{
			int timeStamp = 0;
			RecurringLeaderboardId recurringLeaderboardId = leaderboardId as RecurringLeaderboardId;
			if (recurringLeaderboardId != null)
			{
				timeStamp = recurringLeaderboardId.Timestamp;
			}
			int timeStampMask = timeStamp / 86400 << 2;
			return (int)(scoreState | (LeaderboardScoreState)timeStampMask);
		}

		// Token: 0x060034EA RID: 13546 RVA: 0x000F7EE5 File Offset: 0x000F60E5
		public static void DecodeScoreContext(int context, out int timeStamp, out LeaderboardScoreState scoreState)
		{
			timeStamp = (context >> 2) * 86400;
			scoreState = (LeaderboardScoreState)(context & 3);
		}

		// Token: 0x04002D0E RID: 11534
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("LeaderboardService");

		// Token: 0x04002D0F RID: 11535
		private const float RequestCacheLifetime = 90f;

		// Token: 0x04002D10 RID: 11536
		public const int NoLeaderboardEntries = 0;

		// Token: 0x04002D11 RID: 11537
		[Dependency]
		private ILeaderboardBackend _leaderboardBackend;

		// Token: 0x04002D12 RID: 11538
		[Dependency]
		private IHistogramBackend _histogramBackend;

		// Token: 0x04002D13 RID: 11539
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04002D14 RID: 11540
		private readonly Dictionary<LeaderboardId, LeaderboardService.CachedLeaderboardRequest> _localLeaderboardEntryCache = new Dictionary<LeaderboardId, LeaderboardService.CachedLeaderboardRequest>();

		// Token: 0x04002D15 RID: 11541
		private readonly Dictionary<LeaderboardId, LeaderboardService.CachedLeaderboardRequest> _topLeaderboardEntryCache = new Dictionary<LeaderboardId, LeaderboardService.CachedLeaderboardRequest>();

		// Token: 0x04002D16 RID: 11542
		private readonly Dictionary<LeaderboardId, LeaderboardService.CachedLeaderboardRequest> _playerCenteredLeaderboardEntryCache = new Dictionary<LeaderboardId, LeaderboardService.CachedLeaderboardRequest>();

		// Token: 0x04002D17 RID: 11543
		private readonly Dictionary<LeaderboardId, LeaderboardService.CachedLeaderboardRequest> _topFriendFilteredLeaderboardEntryCache = new Dictionary<LeaderboardId, LeaderboardService.CachedLeaderboardRequest>();

		// Token: 0x04002D18 RID: 11544
		private readonly Dictionary<LeaderboardId, LeaderboardService.CachedHistogramRequest> _histogramCache = new Dictionary<LeaderboardId, LeaderboardService.CachedHistogramRequest>();

		// Token: 0x04002D19 RID: 11545
		private const int NumBits_ScoreState = 2;

		// Token: 0x04002D1A RID: 11546
		private const int NumBits_Timestamp = 16;

		// Token: 0x04002D1B RID: 11547
		private const int NumBits_Unused = 14;

		// Token: 0x04002D1C RID: 11548
		private const int ScoreStateMask = 3;

		// Token: 0x04002D1D RID: 11549
		private const int TimestampMask = 262140;

		// Token: 0x04002D1E RID: 11550
		private const int UnusedMask = -262144;

		// Token: 0x0200076F RID: 1903
		// (Invoke) Token: 0x060034EE RID: 13550
		private delegate void LeaderboardEntryRequestDelegate(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted);

		// Token: 0x02000770 RID: 1904
		private class CachedLeaderboardRequest
		{
			// Token: 0x170008C3 RID: 2243
			// (get) Token: 0x060034F1 RID: 13553 RVA: 0x000F7F47 File Offset: 0x000F6147
			public bool HasExpired
			{
				get
				{
					return Time.realtimeSinceStartup - this._timestamp > 90f;
				}
			}

			// Token: 0x170008C4 RID: 2244
			// (get) Token: 0x060034F2 RID: 13554 RVA: 0x000F7F5C File Offset: 0x000F615C
			public List<LeaderboardEntry> Entries { get; }

			// Token: 0x170008C5 RID: 2245
			// (get) Token: 0x060034F3 RID: 13555 RVA: 0x000F7F64 File Offset: 0x000F6164
			public long TotalEntryCount { get; }

			// Token: 0x060034F4 RID: 13556 RVA: 0x000F7F6C File Offset: 0x000F616C
			public CachedLeaderboardRequest(List<LeaderboardEntry> entries, long totalEntryCount)
			{
				this.Entries = entries;
				this.TotalEntryCount = totalEntryCount;
				this._timestamp = Time.realtimeSinceStartup;
			}

			// Token: 0x04002D21 RID: 11553
			private readonly float _timestamp;
		}

		// Token: 0x02000771 RID: 1905
		private class CachedHistogramRequest
		{
			// Token: 0x170008C6 RID: 2246
			// (get) Token: 0x060034F5 RID: 13557 RVA: 0x000F7F8D File Offset: 0x000F618D
			public bool HasExpired
			{
				get
				{
					return Time.realtimeSinceStartup - this._timestamp > 90f;
				}
			}

			// Token: 0x170008C7 RID: 2247
			// (get) Token: 0x060034F6 RID: 13558 RVA: 0x000F7FA2 File Offset: 0x000F61A2
			public List<int> Buckets { get; }

			// Token: 0x170008C8 RID: 2248
			// (get) Token: 0x060034F7 RID: 13559 RVA: 0x000F7FAA File Offset: 0x000F61AA
			public int BucketSize { get; }

			// Token: 0x060034F8 RID: 13560 RVA: 0x000F7FB2 File Offset: 0x000F61B2
			public CachedHistogramRequest(List<int> buckets, int bucketSize)
			{
				this.Buckets = buckets;
				this.BucketSize = bucketSize;
				this._timestamp = Time.realtimeSinceStartup;
			}

			// Token: 0x04002D24 RID: 11556
			private readonly float _timestamp;
		}
	}
}
