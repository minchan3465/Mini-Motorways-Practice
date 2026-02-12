using System;

namespace Motorways.Leaderboards
{
	// Token: 0x0200076A RID: 1898
	public class CityLeaderboardId : LeaderboardId
	{
		// Token: 0x170008B9 RID: 2233
		// (get) Token: 0x060034C0 RID: 13504 RVA: 0x000F72DB File Offset: 0x000F54DB
		public MapDefinition.CityNames City { get; }

		// Token: 0x170008BA RID: 2234
		// (get) Token: 0x060034C1 RID: 13505 RVA: 0x000F72E3 File Offset: 0x000F54E3
		public CityGameMode Mode { get; }

		// Token: 0x170008BB RID: 2235
		// (get) Token: 0x060034C2 RID: 13506 RVA: 0x000F72EB File Offset: 0x000F54EB
		public int CityChallengeIndex { get; }

		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x060034C3 RID: 13507 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool IsRecurringLeaderboard
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060034C4 RID: 13508 RVA: 0x000F72F3 File Offset: 0x000F54F3
		public CityLeaderboardId(MapDefinition.CityNames city, CityGameMode mode, int cityChallengeIndex)
		{
			this.City = city;
			this.Mode = mode;
			this.CityChallengeIndex = cityChallengeIndex;
			this._serializedString = this.Serialize();
		}

		// Token: 0x060034C5 RID: 13509 RVA: 0x000F731C File Offset: 0x000F551C
		public static bool IsCityLeaderboardId(string leaderboardIdString)
		{
			return leaderboardIdString.StartsWith("map");
		}

		// Token: 0x060034C6 RID: 13510 RVA: 0x000F732C File Offset: 0x000F552C
		public new static CityLeaderboardId Deserialize(string leaderboardIdString)
		{
			if (!CityLeaderboardId.IsCityLeaderboardId(leaderboardIdString))
			{
				LeaderboardId.Log.Error("Invalid CityLeaderboardId string prefix: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			int prefixLength = "map".Length + 1;
			if (leaderboardIdString.Length < prefixLength)
			{
				LeaderboardId.Log.Error("Too few characters for CityLeaderboardId string: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			string[] idComponents = leaderboardIdString.Substring(prefixLength).Split('_', StringSplitOptions.None);
			int componentCount = idComponents.Length;
			if (componentCount < 2 || componentCount > 3)
			{
				LeaderboardId.Log.Error("Invalid component count for CityLeaderboardId: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			MapDefinition.CityNames city;
			if (!Enum.TryParse<MapDefinition.CityNames>(idComponents[0], true, out city))
			{
				LeaderboardId.Log.Error("Failed to parse city string from CityLeaderboardId: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			CityGameMode mode;
			if (!Enum.TryParse<CityGameMode>(idComponents[1], true, out mode))
			{
				LeaderboardId.Log.Error("Failed to parse game mode string from CityLeaderboardId: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			int cityChallengeIndex = -1;
			if (componentCount == 3)
			{
				string cityChallengeStr = idComponents[2];
				if (cityChallengeStr.Length != 10)
				{
					LeaderboardId.Log.Error("Failed to parse city challenge string from CityLeaderboardId: " + leaderboardIdString, Array.Empty<object>());
					return null;
				}
				if (!int.TryParse(cityChallengeStr.Substring(cityChallengeStr.Length - 1, 1), out cityChallengeIndex))
				{
					LeaderboardId.Log.Error("Failed to parse city challenge index from CityLeaderboardId: " + leaderboardIdString, Array.Empty<object>());
					return null;
				}
			}
			return new CityLeaderboardId(city, mode, cityChallengeIndex);
		}

		// Token: 0x060034C7 RID: 13511 RVA: 0x000F748C File Offset: 0x000F568C
		private string Serialize()
		{
			string city = this.City.ToString().ToLower();
			string mode = this.Mode.ToString().ToLower();
			if (this.Mode == CityGameMode.CityChallenge)
			{
				return string.Format("{0}_{1}_{2}_challenge{3}", new object[]
				{
					"map",
					city,
					mode,
					this.CityChallengeIndex
				});
			}
			return "map_" + city + "_" + mode;
		}

		// Token: 0x04002D05 RID: 11525
		public const string CityIdPrefix = "map";
	}
}
