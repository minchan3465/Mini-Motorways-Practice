using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Factory;
using Helpers.GameCenter;

namespace Motorways.Leaderboards.Backends
{
	// Token: 0x0200077B RID: 1915
	public class GameCenterLeaderboardBackend : ILeaderboardBackend
	{
		// Token: 0x06003514 RID: 13588 RVA: 0x000F85C4 File Offset: 0x000F67C4
		public void RequestLocalEntry(LeaderboardId leaderboardId, LocalEntryRequestCompleted localEntryRequestCompleted)
		{
			if (!this._gameCenterAuthentication.IsAuthenticated)
			{
				GameCenterLeaderboardBackend.Log.Error(string.Format("Local entry request fail - Not authenticated with GameCenter - Leaderboard: {0}", leaderboardId), Array.Empty<object>());
				localEntryRequestCompleted(null, 0L, GameCenterLeaderboardBackend.NotAuthenticatedError);
				return;
			}
			if (leaderboardId.IsRecurringLeaderboard && !GameCenterShared.GCSupportsRecurringLeaderboards())
			{
				GameCenterLeaderboardBackend.Log.Error(string.Format("Local entry request fail - Recurring Leaderboards are not supported - Leaderboard: {0}", leaderboardId), Array.Empty<object>());
				localEntryRequestCompleted(null, 0L, GameCenterLeaderboardBackend.RecurringLeaderboardUnsupportedError);
				return;
			}
			this._entryRequests.Enqueue(new GameCenterLeaderboardBackend.LocalEntryRequest(leaderboardId, localEntryRequestCompleted));
			if (this._entryRequests.Count == 1)
			{
				this._tickRegistry.AppTicking += this.Tick;
			}
		}

		// Token: 0x06003515 RID: 13589 RVA: 0x000F867C File Offset: 0x000F687C
		public void SubmitScore(LeaderboardId leaderboardId, int score, LeaderboardScoreState scoreState, SubmitScoreRequestCompleted submitScoreRequestCompleted)
		{
			if (!this._gameCenterAuthentication.IsAuthenticated)
			{
				GameCenterLeaderboardBackend.Log.Error(string.Format("Submit score fail - Not authenticated with GameCenter - Leaderboard: {0}, Score: {1}, ScoreState: {2}", leaderboardId, score, scoreState), Array.Empty<object>());
				submitScoreRequestCompleted(false);
				return;
			}
			string backendLeaderboardId = this.GetBackendLeaderboardId(leaderboardId);
			int context = LeaderboardService.EncodeScoreContext(leaderboardId, scoreState);
			bool isSubmitted = GameCenterShared.GCSetLeaderboardScore(backendLeaderboardId, score, context);
			submitScoreRequestCompleted(isSubmitted);
		}

		// Token: 0x170008CB RID: 2251
		// (get) Token: 0x06003516 RID: 13590 RVA: 0x000020AA File Offset: 0x000002AA
		public bool CanSubmitScoresOffline
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003517 RID: 13591 RVA: 0x000F86E4 File Offset: 0x000F68E4
		public void RequestTopEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
			if (!this._gameCenterAuthentication.IsAuthenticated)
			{
				entryRequestCompleted(null, 0L, GameCenterLeaderboardBackend.NotAuthenticatedError);
				return;
			}
			if (leaderboardId.IsRecurringLeaderboard && !GameCenterShared.GCSupportsRecurringLeaderboards())
			{
				entryRequestCompleted(null, 0L, GameCenterLeaderboardBackend.RecurringLeaderboardUnsupportedError);
				return;
			}
			this._entryRequests.Enqueue(new GameCenterLeaderboardBackend.TopEntryRequest(leaderboardId, entryCount, entryRequestCompleted));
			if (this._entryRequests.Count == 1)
			{
				this._tickRegistry.AppTicking += this.Tick;
			}
		}

		// Token: 0x06003518 RID: 13592 RVA: 0x000F8768 File Offset: 0x000F6968
		public void RequestPlayerCenteredEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
			if (!this._gameCenterAuthentication.IsAuthenticated)
			{
				entryRequestCompleted(null, 0L, GameCenterLeaderboardBackend.NotAuthenticatedError);
				return;
			}
			if (leaderboardId.IsRecurringLeaderboard && !GameCenterShared.GCSupportsRecurringLeaderboards())
			{
				entryRequestCompleted(null, 0L, GameCenterLeaderboardBackend.RecurringLeaderboardUnsupportedError);
				return;
			}
			this._entryRequests.Enqueue(new GameCenterLeaderboardBackend.PlayerCenteredEntryRequest(leaderboardId, entryCount, entryRequestCompleted));
			if (this._entryRequests.Count == 1)
			{
				this._tickRegistry.AppTicking += this.Tick;
			}
		}

		// Token: 0x06003519 RID: 13593 RVA: 0x000F87EC File Offset: 0x000F69EC
		public void RequestTopFriendFilteredEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
			if (!this._gameCenterAuthentication.IsAuthenticated)
			{
				entryRequestCompleted(null, 0L, GameCenterLeaderboardBackend.NotAuthenticatedError);
				return;
			}
			if (leaderboardId.IsRecurringLeaderboard && !GameCenterShared.GCSupportsRecurringLeaderboards())
			{
				entryRequestCompleted(null, 0L, GameCenterLeaderboardBackend.RecurringLeaderboardUnsupportedError);
				return;
			}
			this._entryRequests.Enqueue(new GameCenterLeaderboardBackend.FriendEntryRequest(leaderboardId, entryCount, entryRequestCompleted));
			if (this._entryRequests.Count == 1)
			{
				this._tickRegistry.AppTicking += this.Tick;
			}
		}

		// Token: 0x0600351A RID: 13594 RVA: 0x000022F5 File Offset: 0x000004F5
		public void PresentError(LeaderboardError error)
		{
		}

		// Token: 0x0600351B RID: 13595 RVA: 0x000F8870 File Offset: 0x000F6A70
		private bool HaveRequestsToProcess()
		{
			return this.entryRequestInProgress != null || this._entryRequests.Count > 0;
		}

		// Token: 0x0600351C RID: 13596 RVA: 0x000F888A File Offset: 0x000F6A8A
		private void Tick(float deltaTime)
		{
			if (!this.HaveRequestsToProcess())
			{
				this._tickRegistry.AppTicking -= this.Tick;
				return;
			}
			this.ProcessEntryRequests();
		}

		// Token: 0x0600351D RID: 13597 RVA: 0x000F88B4 File Offset: 0x000F6AB4
		private void ProcessEntryRequests()
		{
			if (this.entryRequestInProgress != null)
			{
				if (GameCenterShared.GCIsLeaderboardRequestFinished())
				{
					List<LeaderboardEntry> entries;
					LeaderboardError requestError = GameCenterLeaderboardBackend.GetResults(out entries);
					GameCenterLeaderboardBackend.Log.Info("Request finished! Leaderboard: {0}, RequestType: {1}, Received entry count: {2}, Error: {3}", new object[]
					{
						this.entryRequestInProgress.LeaderboardId,
						this.entryRequestInProgress.GetType().FullName,
						entries.Count,
						requestError
					});
					GameCenterLeaderboardBackend.IEntryRequest entryRequest = this.entryRequestInProgress;
					if (entryRequest is GameCenterLeaderboardBackend.LocalEntryRequest)
					{
						GameCenterLeaderboardBackend.LocalEntryRequest localEntryRequest = (GameCenterLeaderboardBackend.LocalEntryRequest)entryRequest;
						LeaderboardEntry localEntry;
						if (entries.Count > 0)
						{
							localEntry = entries[0];
						}
						else
						{
							localEntry = null;
						}
						LocalEntryRequestCompleted entryRequestCompleted = localEntryRequest.entryRequestCompleted;
						if (entryRequestCompleted != null)
						{
							entryRequestCompleted(localEntry, GameCenterShared.GCGetTotalLeaderboardEntryCount(), requestError);
						}
					}
					else if (entryRequest is GameCenterLeaderboardBackend.TopEntryRequest)
					{
						GameCenterLeaderboardBackend.TopEntryRequest topEntryRequest = (GameCenterLeaderboardBackend.TopEntryRequest)entryRequest;
						EntryRequestCompleted entryRequestCompleted2 = topEntryRequest.entryRequestCompleted;
						if (entryRequestCompleted2 != null)
						{
							entryRequestCompleted2(entries, GameCenterShared.GCGetTotalLeaderboardEntryCount(), requestError);
						}
					}
					else if (entryRequest is GameCenterLeaderboardBackend.PlayerCenteredEntryRequest)
					{
						GameCenterLeaderboardBackend.PlayerCenteredEntryRequest playerCenteredEntryRequest = (GameCenterLeaderboardBackend.PlayerCenteredEntryRequest)entryRequest;
						EntryRequestCompleted entryRequestCompleted3 = playerCenteredEntryRequest.entryRequestCompleted;
						if (entryRequestCompleted3 != null)
						{
							entryRequestCompleted3(entries, GameCenterShared.GCGetTotalLeaderboardEntryCount(), requestError);
						}
					}
					else if (entryRequest is GameCenterLeaderboardBackend.FriendEntryRequest)
					{
						GameCenterLeaderboardBackend.FriendEntryRequest friendsEntryRequest = (GameCenterLeaderboardBackend.FriendEntryRequest)entryRequest;
						EntryRequestCompleted entryRequestCompleted4 = friendsEntryRequest.entryRequestCompleted;
						if (entryRequestCompleted4 != null)
						{
							entryRequestCompleted4(entries, GameCenterShared.GCGetTotalLeaderboardEntryCount(), requestError);
						}
					}
					this.entryRequestInProgress = null;
				}
				return;
			}
			if (this._entryRequests.Count <= 0)
			{
				return;
			}
			GameCenterLeaderboardBackend.IEntryRequest request = this._entryRequests.Dequeue();
			this.entryRequestInProgress = request;
			string gameCenterLeaderboardId = this.GetBackendLeaderboardId(request.LeaderboardId);
			if (request is GameCenterLeaderboardBackend.LocalEntryRequest)
			{
				GameCenterShared.GCRequestLocalLeaderboardEntry(gameCenterLeaderboardId);
				return;
			}
			if (request is GameCenterLeaderboardBackend.TopEntryRequest)
			{
				GameCenterShared.GCRequestTopLeaderboardEntries(gameCenterLeaderboardId);
				return;
			}
			if (request is GameCenterLeaderboardBackend.FriendEntryRequest)
			{
				GameCenterShared.GCRequestFriendLeaderboardEntries(gameCenterLeaderboardId);
				return;
			}
			if (!(request is GameCenterLeaderboardBackend.PlayerCenteredEntryRequest))
			{
				return;
			}
			GameCenterShared.GCRequestPlayerCenteredLeaderboardEntries(gameCenterLeaderboardId);
		}

		// Token: 0x0600351E RID: 13598 RVA: 0x000F8A78 File Offset: 0x000F6C78
		private static LeaderboardError GetResults(out List<LeaderboardEntry> topEntries)
		{
			topEntries = new List<LeaderboardEntry>();
			int entryCount = GameCenterShared.GCGetDownloadedLeaderboardEntryCount();
			if (entryCount < 0)
			{
				return GameCenterLeaderboardBackend.UnknownError;
			}
			for (int entryIndex = 0; entryIndex < entryCount; entryIndex++)
			{
				LeaderboardEntry leaderboardEntry = GameCenterLeaderboardBackend.GetLeaderboardEntryAtIndex(entryIndex);
				if (leaderboardEntry != null)
				{
					topEntries.Add(leaderboardEntry);
				}
			}
			return null;
		}

		// Token: 0x0600351F RID: 13599 RVA: 0x000F8ABC File Offset: 0x000F6CBC
		private static LeaderboardEntry GetLeaderboardEntryAtIndex(int entryIndex)
		{
			IntPtr cId = IntPtr.Zero;
			IntPtr cName = IntPtr.Zero;
			int context = 0;
			int entryScore = 0;
			long rank = 0L;
			bool isLocal = false;
			bool isFriend = false;
			if (!GameCenterShared.GCGetRetrievedLeaderboardEntry(entryIndex, ref cId, ref cName, ref entryScore, ref rank, ref context, ref isLocal, ref isFriend))
			{
				return null;
			}
			string id = Marshal.PtrToStringAuto(cId);
			string name = Marshal.PtrToStringAuto(cName);
			string formattedName = "";
			foreach (int charCode in name)
			{
				if (charCode != 8236 && charCode != 8234 && charCode != 8235 && charCode != 8206 && charCode != 8207)
				{
					formattedName += ((char)charCode).ToString();
				}
			}
			name = formattedName;
			LeaderboardEntryType entryType = LeaderboardEntryType.Global;
			if (isLocal)
			{
				entryType = LeaderboardEntryType.Local;
			}
			else if (isFriend)
			{
				entryType = LeaderboardEntryType.Friend;
			}
			int timestamp;
			LeaderboardScoreState scoreState;
			LeaderboardService.DecodeScoreContext(context, out timestamp, out scoreState);
			GameCenterLeaderboardBackend.Log.Info("Entry retrieved from backend - Name: {0}, Rank: {1}, Score: {2}, Context: {3}, Timestamp: {4}, Score State: {5}", new object[]
			{
				name,
				rank,
				entryScore,
				context,
				timestamp,
				scoreState
			});
			return new LeaderboardEntry(id, name, entryType, entryScore, rank, timestamp, scoreState);
		}

		// Token: 0x06003520 RID: 13600 RVA: 0x000020AA File Offset: 0x000002AA
		public bool IsLeaderboardTypeSupported(LeaderboardType type)
		{
			return true;
		}

		// Token: 0x170008CC RID: 2252
		// (get) Token: 0x06003521 RID: 13601 RVA: 0x0000222C File Offset: 0x0000042C
		public bool CanAuthenticate
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003522 RID: 13602 RVA: 0x0000222C File Offset: 0x0000042C
		public bool Authenticate(AuthenticationCompleted authenticationCompleted)
		{
			return false;
		}

		// Token: 0x06003523 RID: 13603 RVA: 0x000F8BF4 File Offset: 0x000F6DF4
		private string GetBackendLeaderboardId(LeaderboardId leaderboardId)
		{
			CityLeaderboardId cityLeaderboardId = leaderboardId as CityLeaderboardId;
			if (cityLeaderboardId == null)
			{
				DailyLeaderboardId dailyLeaderboardId = leaderboardId as DailyLeaderboardId;
				if (dailyLeaderboardId != null)
				{
					return "grp." + dailyLeaderboardId.Day.ToString().ToLower();
				}
				WeeklyLeaderboardId weeklyLeaderboardId = leaderboardId as WeeklyLeaderboardId;
				if (weeklyLeaderboardId == null)
				{
					Diagnostics.FailAssert("Invalid ILeaderboard derived type: {0}", new object[]
					{
						leaderboardId
					});
					return null;
				}
				char week = (weeklyLeaderboardId.Week == ChallengeSystem.LeaderboardWeek.WeekA) ? 'a' : 'b';
				return string.Format("grp.week_{0}", week);
			}
			else
			{
				if (cityLeaderboardId.Mode == CityGameMode.CityChallenge)
				{
					return string.Format("grp.{0}_{1}_challenge{2}", cityLeaderboardId.City.ToString().ToLower(), cityLeaderboardId.Mode.ToString().ToLower(), cityLeaderboardId.CityChallengeIndex);
				}
				CityLeaderboardId cityLeaderboardId2 = cityLeaderboardId;
				return "grp." + cityLeaderboardId2.City.ToString().ToLower() + "_" + cityLeaderboardId2.Mode.ToString().ToLower();
			}
		}

		// Token: 0x04002D3F RID: 11583
		private static readonly LeaderboardError UnknownError = new LeaderboardError(LeaderboardErrorCode.Unknown, StringId.LeaderboardError_Generic);

		// Token: 0x04002D40 RID: 11584
		private static readonly LeaderboardError NotAuthenticatedError = new LeaderboardError(LeaderboardErrorCode.NotAuthenticated, StringId.LeaderboardError_NotAuthenticatedGameCenter);

		// Token: 0x04002D41 RID: 11585
		private static readonly LeaderboardError RecurringLeaderboardUnsupportedError = new LeaderboardError(LeaderboardErrorCode.RecurringLeaderboardUnsupported, StringId.LeaderboardError_RecurringLeaderboardUnsupported);

		// Token: 0x04002D42 RID: 11586
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("GameCenterBackend");

		// Token: 0x04002D43 RID: 11587
		[Dependency]
		private IGameCenterAuthentication _gameCenterAuthentication;

		// Token: 0x04002D44 RID: 11588
		[Dependency]
		private TickRegistry _tickRegistry;

		// Token: 0x04002D45 RID: 11589
		private Queue<GameCenterLeaderboardBackend.IEntryRequest> _entryRequests = new Queue<GameCenterLeaderboardBackend.IEntryRequest>();

		// Token: 0x04002D46 RID: 11590
		private GameCenterLeaderboardBackend.IEntryRequest entryRequestInProgress;

		// Token: 0x0200077C RID: 1916
		private interface IEntryRequest
		{
			// Token: 0x170008CD RID: 2253
			// (get) Token: 0x06003526 RID: 13606
			LeaderboardId LeaderboardId { get; }
		}

		// Token: 0x0200077D RID: 1917
		private struct LocalEntryRequest : GameCenterLeaderboardBackend.IEntryRequest
		{
			// Token: 0x170008CE RID: 2254
			// (get) Token: 0x06003527 RID: 13607 RVA: 0x000F8D88 File Offset: 0x000F6F88
			public readonly LeaderboardId LeaderboardId { get; }

			// Token: 0x06003528 RID: 13608 RVA: 0x000F8D90 File Offset: 0x000F6F90
			public LocalEntryRequest(LeaderboardId leaderboardId, LocalEntryRequestCompleted entryRequestCompleted)
			{
				this.LeaderboardId = leaderboardId;
				this.entryRequestCompleted = entryRequestCompleted;
			}

			// Token: 0x04002D48 RID: 11592
			public LocalEntryRequestCompleted entryRequestCompleted;
		}

		// Token: 0x0200077E RID: 1918
		private struct TopEntryRequest : GameCenterLeaderboardBackend.IEntryRequest
		{
			// Token: 0x170008CF RID: 2255
			// (get) Token: 0x06003529 RID: 13609 RVA: 0x000F8DA0 File Offset: 0x000F6FA0
			public readonly LeaderboardId LeaderboardId { get; }

			// Token: 0x0600352A RID: 13610 RVA: 0x000F8DA8 File Offset: 0x000F6FA8
			public TopEntryRequest(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
			{
				this.LeaderboardId = leaderboardId;
				this.entryCount = entryCount;
				this.entryRequestCompleted = entryRequestCompleted;
			}

			// Token: 0x04002D4A RID: 11594
			public int entryCount;

			// Token: 0x04002D4B RID: 11595
			public EntryRequestCompleted entryRequestCompleted;
		}

		// Token: 0x0200077F RID: 1919
		private struct PlayerCenteredEntryRequest : GameCenterLeaderboardBackend.IEntryRequest
		{
			// Token: 0x170008D0 RID: 2256
			// (get) Token: 0x0600352B RID: 13611 RVA: 0x000F8DBF File Offset: 0x000F6FBF
			public readonly LeaderboardId LeaderboardId { get; }

			// Token: 0x0600352C RID: 13612 RVA: 0x000F8DC7 File Offset: 0x000F6FC7
			public PlayerCenteredEntryRequest(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
			{
				this.LeaderboardId = leaderboardId;
				this.entryCount = entryCount;
				this.entryRequestCompleted = entryRequestCompleted;
			}

			// Token: 0x04002D4D RID: 11597
			public int entryCount;

			// Token: 0x04002D4E RID: 11598
			public EntryRequestCompleted entryRequestCompleted;
		}

		// Token: 0x02000780 RID: 1920
		private struct FriendEntryRequest : GameCenterLeaderboardBackend.IEntryRequest
		{
			// Token: 0x170008D1 RID: 2257
			// (get) Token: 0x0600352D RID: 13613 RVA: 0x000F8DDE File Offset: 0x000F6FDE
			public readonly LeaderboardId LeaderboardId { get; }

			// Token: 0x0600352E RID: 13614 RVA: 0x000F8DE6 File Offset: 0x000F6FE6
			public FriendEntryRequest(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
			{
				this.LeaderboardId = leaderboardId;
				this.entryCount = entryCount;
				this.entryRequestCompleted = entryRequestCompleted;
			}

			// Token: 0x04002D50 RID: 11600
			public int entryCount;

			// Token: 0x04002D51 RID: 11601
			public EntryRequestCompleted entryRequestCompleted;
		}
	}
}
