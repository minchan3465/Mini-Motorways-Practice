using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Motorways.Leaderboards;
using Steamworks;
using Steamworks.Data;

// Token: 0x020000DD RID: 221
public static class SteamworksShared
{
	// Token: 0x06000486 RID: 1158 RVA: 0x0000FF4C File Offset: 0x0000E14C
	public static bool RestartAppIfNecessary(uint appId)
	{
		bool restartRequired = false;
		try
		{
			restartRequired = SteamClient.RestartAppIfNecessary(appId);
		}
		catch (Exception e)
		{
			SteamworksShared.Log.Error(string.Format("Caught Exception : {0}", e), Array.Empty<object>());
		}
		return restartRequired;
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0000FF94 File Offset: 0x0000E194
	public static bool Init(uint appId)
	{
		try
		{
			SteamClient.Init(appId, true);
			SteamUserStats.RequestCurrentStats();
		}
		catch (Exception e)
		{
			SteamworksShared.Log.Error(string.Format("Caught Exception : {0}", e), Array.Empty<object>());
		}
		return SteamworksShared.IsValid;
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x0000FFE4 File Offset: 0x0000E1E4
	public static void Shutdown()
	{
		SteamClient.Shutdown();
	}

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06000489 RID: 1161 RVA: 0x0000FFEB File Offset: 0x0000E1EB
	public static bool IsValid
	{
		get
		{
			return SteamClient.IsValid;
		}
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x0000FFF2 File Offset: 0x0000E1F2
	public static void RunCallbacks()
	{
		SteamClient.RunCallbacks();
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0000FFFC File Offset: 0x0000E1FC
	public static void RequestLocalLeaderboardEntry(string leaderboardName, LocalEntryRequestCompleted localEntryRequestCompleted)
	{
		SteamworksShared.<RequestLocalLeaderboardEntry>d__9 <RequestLocalLeaderboardEntry>d__;
		<RequestLocalLeaderboardEntry>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RequestLocalLeaderboardEntry>d__.leaderboardName = leaderboardName;
		<RequestLocalLeaderboardEntry>d__.localEntryRequestCompleted = localEntryRequestCompleted;
		<RequestLocalLeaderboardEntry>d__.<>1__state = -1;
		<RequestLocalLeaderboardEntry>d__.<>t__builder.Start<SteamworksShared.<RequestLocalLeaderboardEntry>d__9>(ref <RequestLocalLeaderboardEntry>d__);
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x0001003C File Offset: 0x0000E23C
	public static void SubmitScore(LeaderboardId leaderboardId, string leaderboardName, int score, LeaderboardScoreState scoreState, SubmitScoreRequestCompleted onCompleted)
	{
		SteamworksShared.<SubmitScore>d__10 <SubmitScore>d__;
		<SubmitScore>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SubmitScore>d__.leaderboardId = leaderboardId;
		<SubmitScore>d__.leaderboardName = leaderboardName;
		<SubmitScore>d__.score = score;
		<SubmitScore>d__.scoreState = scoreState;
		<SubmitScore>d__.onCompleted = onCompleted;
		<SubmitScore>d__.<>1__state = -1;
		<SubmitScore>d__.<>t__builder.Start<SteamworksShared.<SubmitScore>d__10>(ref <SubmitScore>d__);
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00010094 File Offset: 0x0000E294
	public static void RequestTopLeaderboardEntries(string leaderboardName, int entryCount, EntryRequestCompleted entryRequestCompleted)
	{
		SteamworksShared.<RequestTopLeaderboardEntries>d__14 <RequestTopLeaderboardEntries>d__;
		<RequestTopLeaderboardEntries>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RequestTopLeaderboardEntries>d__.leaderboardName = leaderboardName;
		<RequestTopLeaderboardEntries>d__.entryCount = entryCount;
		<RequestTopLeaderboardEntries>d__.entryRequestCompleted = entryRequestCompleted;
		<RequestTopLeaderboardEntries>d__.<>1__state = -1;
		<RequestTopLeaderboardEntries>d__.<>t__builder.Start<SteamworksShared.<RequestTopLeaderboardEntries>d__14>(ref <RequestTopLeaderboardEntries>d__);
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x000100DC File Offset: 0x0000E2DC
	public static void RequestPlayerCenteredLeaderboardEntries(string leaderboardName, int entryCount, EntryRequestCompleted entryRequestCompleted)
	{
		SteamworksShared.<RequestPlayerCenteredLeaderboardEntries>d__15 <RequestPlayerCenteredLeaderboardEntries>d__;
		<RequestPlayerCenteredLeaderboardEntries>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RequestPlayerCenteredLeaderboardEntries>d__.leaderboardName = leaderboardName;
		<RequestPlayerCenteredLeaderboardEntries>d__.entryCount = entryCount;
		<RequestPlayerCenteredLeaderboardEntries>d__.entryRequestCompleted = entryRequestCompleted;
		<RequestPlayerCenteredLeaderboardEntries>d__.<>1__state = -1;
		<RequestPlayerCenteredLeaderboardEntries>d__.<>t__builder.Start<SteamworksShared.<RequestPlayerCenteredLeaderboardEntries>d__15>(ref <RequestPlayerCenteredLeaderboardEntries>d__);
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x00010124 File Offset: 0x0000E324
	public static void RequestTopFriendLeaderboardEntries(string leaderboardName, int entryCount, EntryRequestCompleted entryRequestCompleted)
	{
		SteamworksShared.<RequestTopFriendLeaderboardEntries>d__16 <RequestTopFriendLeaderboardEntries>d__;
		<RequestTopFriendLeaderboardEntries>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RequestTopFriendLeaderboardEntries>d__.leaderboardName = leaderboardName;
		<RequestTopFriendLeaderboardEntries>d__.entryRequestCompleted = entryRequestCompleted;
		<RequestTopFriendLeaderboardEntries>d__.<>1__state = -1;
		<RequestTopFriendLeaderboardEntries>d__.<>t__builder.Start<SteamworksShared.<RequestTopFriendLeaderboardEntries>d__16>(ref <RequestTopFriendLeaderboardEntries>d__);
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x00010164 File Offset: 0x0000E364
	private static global::LeaderboardEntry ToLeaderboardEntry(Steamworks.Data.LeaderboardEntry steamworksEntry, long? rankOverride = null)
	{
		LeaderboardEntryType entryType = LeaderboardEntryType.Global;
		if (steamworksEntry.User.IsMe)
		{
			entryType = LeaderboardEntryType.Local;
		}
		else if (steamworksEntry.User.IsFriend)
		{
			entryType = LeaderboardEntryType.Friend;
		}
		LeaderboardScoreState scoreState = LeaderboardScoreState.Editable;
		int timeStamp = 0;
		if (steamworksEntry.Details != null)
		{
			LeaderboardService.DecodeScoreContext(steamworksEntry.Details[0], out timeStamp, out scoreState);
		}
		return new global::LeaderboardEntry(steamworksEntry.User.Id.ToString(), steamworksEntry.User.Name, entryType, steamworksEntry.Score, rankOverride ?? ((long)steamworksEntry.GlobalRank), timeStamp, scoreState);
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x000101FE File Offset: 0x0000E3FE
	private static Task<Leaderboard?> GetLeaderboard(string leaderboardName)
	{
		return SteamUserStats.FindOrCreateLeaderboardAsync(leaderboardName, LeaderboardSort.Descending, LeaderboardDisplay.Numeric);
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x00010208 File Offset: 0x0000E408
	public static bool SaveScreenshot(byte[] bytes, int width, int height)
	{
		return SteamworksShared.IsValid && SteamScreenshots.WriteScreenshot(bytes, width, height) != null;
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x00010230 File Offset: 0x0000E430
	public static void SetRichPresence(Dictionary<string, string> tokens)
	{
		if (!SteamworksShared.IsValid)
		{
			return;
		}
		if (tokens == null || tokens.Count == 0)
		{
			SteamFriends.ClearRichPresence();
			return;
		}
		foreach (KeyValuePair<string, string> pair in tokens)
		{
			SteamFriends.SetRichPresence(pair.Key, pair.Value);
		}
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x000102A4 File Offset: 0x0000E4A4
	public static bool CompleteAchievement(string name)
	{
		Steamworks.Data.Achievement achievement;
		return SteamworksShared.IsValid && SteamworksShared.TryFindAchievement(name, out achievement) && (achievement.State || achievement.Trigger(true));
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x000102DC File Offset: 0x0000E4DC
	public static bool ClearAchievement(string name)
	{
		Steamworks.Data.Achievement achievement;
		return SteamworksShared.IsValid && (SteamworksShared.TryFindAchievement(name, out achievement) && achievement.State) && achievement.Clear();
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x00010310 File Offset: 0x0000E510
	public static bool IsAchievementCompleted(string name)
	{
		Steamworks.Data.Achievement achievement;
		return SteamworksShared.IsValid && SteamworksShared.TryFindAchievement(name, out achievement) && achievement.State;
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x00010339 File Offset: 0x0000E539
	public static bool IncrementStatistic(string statisticId, int amount)
	{
		if (!SteamworksShared.IsValid)
		{
			return false;
		}
		SteamUserStats.AddStat(statisticId, amount);
		return false;
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x0001034D File Offset: 0x0000E54D
	public static byte[] ReadCloudFile(string filename)
	{
		if (!SteamworksShared.IsValid)
		{
			return null;
		}
		return SteamRemoteStorage.FileRead(filename);
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x0001035E File Offset: 0x0000E55E
	public static bool WriteCloudFile(string filename, byte[] data)
	{
		return SteamworksShared.IsValid && SteamRemoteStorage.FileWrite(filename, data);
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x00010370 File Offset: 0x0000E570
	public static bool DeleteCloudFile(string filename)
	{
		return SteamworksShared.IsValid && SteamRemoteStorage.FileDelete(filename);
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x00010381 File Offset: 0x0000E581
	public static IEnumerable<string> GetCloudFiles()
	{
		if (!SteamworksShared.IsValid)
		{
			yield break;
		}
		foreach (string filename in SteamRemoteStorage.Files)
		{
			yield return filename;
		}
		IEnumerator<string> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0001038C File Offset: 0x0000E58C
	public static LocaleDatabase.LocaleId GetLocaleId()
	{
		if (!SteamworksShared.IsValid)
		{
			return LocaleDatabase.LocaleId.Unknown;
		}
		string gameLanguage = SteamApps.GameLanguage;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(gameLanguage);
		if (num <= 1901528810U)
		{
			if (num <= 599131013U)
			{
				if (num <= 319214730U)
				{
					if (num != 308944030U)
					{
						if (num != 316123288U)
						{
							if (num == 319214730U)
							{
								if (gameLanguage == "romanian")
								{
									return LocaleDatabase.LocaleId.en_US;
								}
							}
						}
						else if (gameLanguage == "danish")
						{
							return LocaleDatabase.LocaleId.da;
						}
					}
					else if (gameLanguage == "swedish")
					{
						return LocaleDatabase.LocaleId.sv_SE;
					}
				}
				else if (num <= 497316822U)
				{
					if (num != 380651494U)
					{
						if (num == 497316822U)
						{
							if (gameLanguage == "bulgarian")
							{
								return LocaleDatabase.LocaleId.bg;
							}
						}
					}
					else if (gameLanguage == "russian")
					{
						return LocaleDatabase.LocaleId.ru;
					}
				}
				else if (num != 505713757U)
				{
					if (num == 599131013U)
					{
						if (gameLanguage == "french")
						{
							return LocaleDatabase.LocaleId.fr;
						}
					}
				}
				else if (gameLanguage == "brazilian")
				{
					return LocaleDatabase.LocaleId.pt_BR;
				}
			}
			else if (num <= 1262725376U)
			{
				if (num != 683056061U)
				{
					if (num != 693158059U)
					{
						if (num == 1262725376U)
						{
							if (gameLanguage == "latam")
							{
								return LocaleDatabase.LocaleId.es_MX;
							}
						}
					}
					else if (gameLanguage == "norwegian")
					{
						return LocaleDatabase.LocaleId.no;
					}
				}
				else if (gameLanguage == "ukrainian")
				{
					return LocaleDatabase.LocaleId.uk;
				}
			}
			else if (num <= 1580935484U)
			{
				if (num != 1544226106U)
				{
					if (num == 1580935484U)
					{
						if (gameLanguage == "portuguese")
						{
							return LocaleDatabase.LocaleId.pt_PT;
						}
					}
				}
				else if (gameLanguage == "hungarian")
				{
					return LocaleDatabase.LocaleId.hu;
				}
			}
			else if (num != 1703858441U)
			{
				if (num == 1901528810U)
				{
					if (gameLanguage == "japanese")
					{
						return LocaleDatabase.LocaleId.ja;
					}
				}
			}
			else if (gameLanguage == "arabic")
			{
				return LocaleDatabase.LocaleId.ar;
			}
		}
		else if (num <= 3229236340U)
		{
			if (num <= 2798875500U)
			{
				if (num != 2471602315U)
				{
					if (num != 2499415067U)
					{
						if (num == 2798875500U)
						{
							if (gameLanguage == "czech")
							{
								return LocaleDatabase.LocaleId.cs;
							}
						}
					}
					else if (gameLanguage == "english")
					{
						return LocaleDatabase.LocaleId.en_US;
					}
				}
				else if (gameLanguage == "italian")
				{
					return LocaleDatabase.LocaleId.it;
				}
			}
			else if (num <= 3180870988U)
			{
				if (num != 2805355685U)
				{
					if (num == 3180870988U)
					{
						if (gameLanguage == "polish")
						{
							return LocaleDatabase.LocaleId.pl;
						}
					}
				}
				else if (gameLanguage == "schinese")
				{
					return LocaleDatabase.LocaleId.zh_CN;
				}
			}
			else if (num != 3210859552U)
			{
				if (num == 3229236340U)
				{
					if (gameLanguage == "finnish")
					{
						return LocaleDatabase.LocaleId.fi;
					}
				}
			}
			else if (gameLanguage == "koreana")
			{
				return LocaleDatabase.LocaleId.ko;
			}
		}
		else if (num <= 3719199419U)
		{
			if (num <= 3405445907U)
			{
				if (num != 3264533134U)
				{
					if (num == 3405445907U)
					{
						if (gameLanguage == "german")
						{
							return LocaleDatabase.LocaleId.de;
						}
					}
				}
				else if (gameLanguage == "tchinese")
				{
					return LocaleDatabase.LocaleId.zh_TW;
				}
			}
			else if (num != 3426057626U)
			{
				if (num == 3719199419U)
				{
					if (gameLanguage == "spanish")
					{
						return LocaleDatabase.LocaleId.es_ES;
					}
				}
			}
			else if (gameLanguage == "vietnamese")
			{
				return LocaleDatabase.LocaleId.en_US;
			}
		}
		else if (num <= 3759690811U)
		{
			if (num != 3739448251U)
			{
				if (num == 3759690811U)
				{
					if (gameLanguage == "thai")
					{
						return LocaleDatabase.LocaleId.th;
					}
				}
			}
			else if (gameLanguage == "turkish")
			{
				return LocaleDatabase.LocaleId.tr;
			}
		}
		else if (num != 4151292721U)
		{
			if (num == 4263372803U)
			{
				if (gameLanguage == "greek")
				{
					return LocaleDatabase.LocaleId.el;
				}
			}
		}
		else if (gameLanguage == "dutch")
		{
			return LocaleDatabase.LocaleId.nl;
		}
		SteamworksShared.Log.Warn("Encountered unrecognised language code '{0}'.", new object[]
		{
			SteamApps.GameLanguage
		});
		return LocaleDatabase.LocaleId.Unknown;
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x00010848 File Offset: 0x0000EA48
	private static bool TryFindAchievement(string name, out Steamworks.Data.Achievement result)
	{
		foreach (Steamworks.Data.Achievement achievement in SteamUserStats.Achievements)
		{
			if (achievement.Identifier == name)
			{
				result = achievement;
				return true;
			}
		}
		return false;
	}

	// Token: 0x040001D2 RID: 466
	private static readonly LeaderboardError UnknownError = new LeaderboardError(LeaderboardErrorCode.Unknown, StringId.LeaderboardError_Generic);

	// Token: 0x040001D3 RID: 467
	private static readonly LeaderboardError NotAuthenticatedError = new LeaderboardError(LeaderboardErrorCode.Unknown, StringId.LeaderboardError_Generic);

	// Token: 0x040001D4 RID: 468
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("SteamworksApi");

	// Token: 0x040001D5 RID: 469
	private const int PerAttemptQueryCount = 20;

	// Token: 0x040001D6 RID: 470
	private const int MaxAttemptsToFilterScores = 10;

	// Token: 0x040001D7 RID: 471
	private const int MaximumScore = 200000;
}
