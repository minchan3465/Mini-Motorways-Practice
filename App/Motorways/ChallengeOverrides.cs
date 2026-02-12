using System;
using System.Collections.Generic;
using System.Text;
using Factory;
using UnityEngine;
using UnityEngine.Networking;

namespace Motorways
{
	// Token: 0x02000342 RID: 834
	public class ChallengeOverrides
	{
		// Token: 0x0600149D RID: 5277 RVA: 0x00043EE8 File Offset: 0x000420E8
		public void Initialize(ChallengeSystem challengeSystem, ChallengeDatabase challengeDatabase, MapDatabase mapDatabase)
		{
			this._challengeSystem = challengeSystem;
			this._challengeDatabase = challengeDatabase;
			this._mapDatabase = mapDatabase;
			this.LoadLocalOverrides();
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x00043F08 File Offset: 0x00042108
		private void LoadLocalOverrides()
		{
			byte[] overridesBytes = this._fileSystem.ReadFile("challengeOverrides.json");
			if (overridesBytes == null)
			{
				return;
			}
			string overridesJson = Encoding.UTF8.GetString(overridesBytes);
			if (this.Deserialize(overridesJson))
			{
				byte[] versionBytes = this._fileSystem.ReadFile("challengeOverrides_version.json");
				if (versionBytes != null)
				{
					string versionJson = Encoding.UTF8.GetString(versionBytes);
					if (this._overrideVersion.Deserialize(versionJson))
					{
						ChallengeOverrides.Log.Info("Loaded challenge overrides version {0}.", new object[]
						{
							this._overrideVersion.Timestamp
						});
						return;
					}
				}
			}
			else
			{
				ChallengeOverrides.Log.Info("Failed to import challenge overrides from {0}. They will be fetched again from the server.", new object[]
				{
					"challengeOverrides.json"
				});
			}
		}

		// Token: 0x0600149F RID: 5279 RVA: 0x00043FB4 File Offset: 0x000421B4
		public void RefreshOverridesFromServer(Action<ChallengeOverrides.RefreshResult> callback = null)
		{
			DateTime timeNow = GameDateTime.UtcNow;
			if (!(timeNow - this._timeLastRefreshedOverrides < ChallengeOverrides.MinRefreshTimespan) && !this._isOpeningConnection)
			{
				this._isOpeningConnection = true;
				this._reachability.OpenSilentConnection(delegate(InternetConnectionHandle handle)
				{
					this._isOpeningConnection = false;
					if (this._reachability.Connectivity == InternetConnectivity.Disconnected)
					{
						ChallengeOverrides.Log.Info("Not refreshing challenges because the internet connection is not available.", Array.Empty<object>());
						Action<ChallengeOverrides.RefreshResult> callback3 = callback;
						if (callback3 != null)
						{
							callback3(ChallengeOverrides.RefreshResult.Error);
						}
						handle.Close();
						return;
					}
					this._timeLastRefreshedOverrides = timeNow;
					string versionUri = "https://api.dinopoloclub.com/1/minimotorways/challenges/version/";
					UnityWebRequest wwwVersion = UnityWebRequest.Get(versionUri);
					wwwVersion.SendWebRequest().completed += delegate(AsyncOperation <p0>)
					{
						if (!Diagnostics.Verify(wwwVersion.result == UnityWebRequest.Result.Success, "Failed to download the new Versions file"))
						{
							Action<ChallengeOverrides.RefreshResult> callback4 = callback;
							if (callback4 != null)
							{
								callback4(ChallengeOverrides.RefreshResult.Error);
							}
							handle.Close();
							return;
						}
						string versionJson = wwwVersion.downloadHandler.text;
						ChallengeOverrideVersion serverVersion = new ChallengeOverrideVersion();
						if (!Diagnostics.Verify(serverVersion.Deserialize(versionJson), "Failed to deserialize ServerVersion. The json may be in an unexpected format."))
						{
							Action<ChallengeOverrides.RefreshResult> callback5 = callback;
							if (callback5 != null)
							{
								callback5(ChallengeOverrides.RefreshResult.Error);
							}
							handle.Close();
							return;
						}
						if (serverVersion.Timestamp <= this._overrideVersion.Timestamp)
						{
							Action<ChallengeOverrides.RefreshResult> callback6 = callback;
							if (callback6 != null)
							{
								callback6(ChallengeOverrides.RefreshResult.NoChange);
							}
							handle.Close();
							return;
						}
						ChallengeOverrides.Log.Info("The server's challenge override version ({0}) is NEWER than the local version ({1}).", new object[]
						{
							serverVersion.Timestamp,
							this._overrideVersion.Timestamp
						});
						string overridesUri = string.Format("https://api.dinopoloclub.com/1/minimotorways/challenges/{0}/", serverVersion.Timestamp);
						UnityWebRequest www = UnityWebRequest.Get(overridesUri);
						www.SendWebRequest().completed += delegate(AsyncOperation <p0>)
						{
							if (!Diagnostics.Verify(www.result == UnityWebRequest.Result.Success, "Failed to download the new Overrides file"))
							{
								Action<ChallengeOverrides.RefreshResult> callback7 = callback;
								if (callback7 != null)
								{
									callback7(ChallengeOverrides.RefreshResult.Error);
								}
								handle.Close();
								return;
							}
							string overridesJson = www.downloadHandler.text;
							if (!Diagnostics.Verify(this.Deserialize(overridesJson), "Failed to deserialize Overrides. The json may be in an unexpected format."))
							{
								Action<ChallengeOverrides.RefreshResult> callback8 = callback;
								if (callback8 != null)
								{
									callback8(ChallengeOverrides.RefreshResult.Error);
								}
								handle.Close();
							}
							this._overrideVersion = serverVersion;
							this._fileSystem.WriteFile("challengeOverrides.json", Encoding.UTF8.GetBytes(overridesJson));
							this._fileSystem.WriteFile("challengeOverrides_version.json", Encoding.UTF8.GetBytes(versionJson));
							ChallengeOverrides.Log.Info("Local challenge overrides have been updated to version {0}.", new object[]
							{
								this._overrideVersion.Timestamp
							});
							Action<ChallengeOverrides.RefreshResult> callback9 = callback;
							if (callback9 != null)
							{
								callback9(ChallengeOverrides.RefreshResult.Success);
							}
							handle.Close();
						};
					};
				});
				return;
			}
			Action<ChallengeOverrides.RefreshResult> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(ChallengeOverrides.RefreshResult.NoChange);
		}

		// Token: 0x060014A0 RID: 5280 RVA: 0x00044035 File Offset: 0x00042235
		public bool TryGetDailyChallenge(int startTime, int endTime, out MapChallenge result)
		{
			return this.TryGetChallenge(this._dailyChallenges, startTime, endTime, new ChallengeOverrides.ChallengeFactory(MapChallenge.CreateDailyChallenge), out result);
		}

		// Token: 0x060014A1 RID: 5281 RVA: 0x00044052 File Offset: 0x00042252
		public bool TryGetWeeklyChallenge(int startTime, int endTime, out MapChallenge result)
		{
			return this.TryGetChallenge(this._weeklyChallenges, startTime, endTime, new ChallengeOverrides.ChallengeFactory(MapChallenge.CreateWeeklyChallenge), out result);
		}

		// Token: 0x060014A2 RID: 5282 RVA: 0x00044070 File Offset: 0x00042270
		private bool TryGetChallenge(List<ChallengeOverride> overrides, int startTime, int endTime, ChallengeOverrides.ChallengeFactory createChallenge, out MapChallenge result)
		{
			result = null;
			ChallengeOverride challengeOverride = null;
			foreach (ChallengeOverride challenge in overrides)
			{
				if (challenge.timestamp == startTime)
				{
					challengeOverride = challenge;
					break;
				}
			}
			if (challengeOverride == null)
			{
				return false;
			}
			MapDefinition mapDefinition = this._mapDatabase.MapLibrary.GetMapByName(challengeOverride.cityName);
			if (mapDefinition == null)
			{
				ChallengeOverrides.Log.Error("Failed to map CityName: " + challengeOverride.cityName + " - Defaulting to excluding it from the Overrides", Array.Empty<object>());
				return false;
			}
			List<ChallengeData> challengeData = new List<ChallengeData>(challengeOverride.challengeNames.Length);
			foreach (string challengeName in challengeOverride.challengeNames)
			{
				ChallengeData match;
				if (this._challengeDatabase.TryGetChallenge(challengeName, out match))
				{
					challengeData.Add(match);
				}
				else
				{
					ChallengeOverrides.Log.Error("Failed to map ChallengeData: " + challengeName + " - Defaulting to excluding it from the ChallengeData container", Array.Empty<object>());
				}
			}
			if (challengeData.Count == 0)
			{
				ChallengeOverrides.Log.Error(string.Format("MapChallenge ({0} - {1}) has no ChallengeData - Defaulting to excluding it from the Overrides", startTime, endTime), Array.Empty<object>());
				return false;
			}
			result = createChallenge(this._challengeSystem, mapDefinition, challengeData.ToArray(), startTime, endTime, (ulong)((long)startTime));
			return result != null;
		}

		// Token: 0x060014A3 RID: 5283 RVA: 0x000441D4 File Offset: 0x000423D4
		public string Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			List<object> days = this.SerializeChallenges(this._dailyChallenges);
			dictionary.Add("Days", days);
			List<object> weeks = this.SerializeChallenges(this._weeklyChallenges);
			dictionary.Add("Weeks", weeks);
			return Json.Serialize(dictionary, false);
		}

		// Token: 0x060014A4 RID: 5284 RVA: 0x00044220 File Offset: 0x00042420
		private List<object> SerializeChallenges(List<ChallengeOverride> challenges)
		{
			List<object> results = new List<object>(challenges.Count);
			foreach (ChallengeOverride challenge in challenges)
			{
				results.Add(new Dictionary<string, object>
				{
					{
						"Timestamp",
						challenge.timestamp
					},
					{
						"CityName",
						challenge.cityName
					},
					{
						"ChallengeNames",
						challenge.challengeNames
					}
				});
			}
			return results;
		}

		// Token: 0x060014A5 RID: 5285 RVA: 0x000442BC File Offset: 0x000424BC
		private bool Deserialize(string json)
		{
			JSON.Dictionary dictionary = JSON.ToDictionary(JSON.LoadFromString(json));
			if (dictionary == null)
			{
				ChallengeOverrides.Log.Error("Failed to parse JSON string to Dictionary.\n" + json, Array.Empty<object>());
				return false;
			}
			JSON.Array daysNode = dictionary.GetArray("Days");
			JSON.Array weeksNode = dictionary.GetArray("Weeks");
			if (daysNode == null || weeksNode == null)
			{
				ChallengeOverrides.Log.Error(string.Concat(new string[]
				{
					"Failed to extract both Days and Weeks Arrays from Dictionary.\nDays Node:\n",
					Json.Serialize(daysNode, false),
					"\n\nWeeks Node:\n",
					Json.Serialize(weeksNode, false),
					"\n",
					json,
					"\n\nSource:\n",
					json
				}), Array.Empty<object>());
				return false;
			}
			List<ChallengeOverride> dailyChallenges = this.DeserializeChallenges(daysNode);
			List<ChallengeOverride> weeklyChallenges = this.DeserializeChallenges(weeksNode);
			if (dailyChallenges == null || weeklyChallenges == null)
			{
				ChallengeOverrides.Log.Error(string.Format("Failed to Deserialize Daily or Weekly Challenges.\nDailyChallenges: {0}\nWeekly Challenges: {1}\n\nSource:\n{2}", dailyChallenges, weeklyChallenges, json), Array.Empty<object>());
				return false;
			}
			this._dailyChallenges.Clear();
			this._dailyChallenges.AddRange(dailyChallenges);
			this._weeklyChallenges.Clear();
			this._weeklyChallenges.AddRange(weeklyChallenges);
			return true;
		}

		// Token: 0x060014A6 RID: 5286 RVA: 0x000443D4 File Offset: 0x000425D4
		private List<ChallengeOverride> DeserializeChallenges(JSON.Array challenges)
		{
			List<ChallengeOverride> results = new List<ChallengeOverride>(challenges.Count);
			for (int challengeIndex = 0; challengeIndex < challenges.Count; challengeIndex++)
			{
				JSON.Dictionary dictionary = JSON.ToDictionary(challenges[challengeIndex]);
				if (dictionary == null)
				{
					ChallengeOverrides.Log.Error("Failed to convert to Dictionary.\n" + Json.Serialize(challenges[challengeIndex], false), Array.Empty<object>());
					return null;
				}
				int timestamp = dictionary.GetInt("Timestamp", -1);
				string cityName = dictionary.GetString("CityName");
				JSON.Array jsonChallengeNames = dictionary.GetArray("ChallengeNames");
				if (timestamp == -1 || cityName == null || jsonChallengeNames == null)
				{
					ChallengeOverrides.Log.Error(string.Format("Failed to Deserialize ChallengeOverride.\nIndex: {0}\nTimestamp: {1}\nCityName: {2}\nChallengeNames:\n{3}\n\nSource:\n{4}", new object[]
					{
						challengeIndex,
						timestamp,
						cityName,
						Json.Serialize(jsonChallengeNames, false),
						Json.Serialize(challenges, false)
					}), Array.Empty<object>());
					return null;
				}
				string[] challengeNames = new string[jsonChallengeNames.Count];
				for (int nameIndex = 0; nameIndex < challengeNames.Length; nameIndex++)
				{
					string challengeName = jsonChallengeNames.GetString(nameIndex);
					if (challengeName == null)
					{
						ChallengeOverrides.Log.Error(string.Format("Invalid string entry in ChallengesArray.\nIndex: {0}\nElement: {1}\n\nSource:\n{2}", nameIndex, jsonChallengeNames[0], Json.Serialize(jsonChallengeNames, false)), Array.Empty<object>());
						return null;
					}
					challengeNames[nameIndex] = challengeName;
				}
				ChallengeOverride challengeOverride = new ChallengeOverride(timestamp, cityName, challengeNames);
				results.Add(challengeOverride);
			}
			return results;
		}

		// Token: 0x060014A7 RID: 5287 RVA: 0x0004453A File Offset: 0x0004273A
		public static ChallengeOverrides EDITOR_CreateFromOverrides(List<ChallengeOverride> dailyChallenges, List<ChallengeOverride> weeklyChallenges)
		{
			ChallengeOverrides challengeOverrides = new ChallengeOverrides();
			challengeOverrides._dailyChallenges.AddRange(dailyChallenges);
			challengeOverrides._weeklyChallenges.AddRange(weeklyChallenges);
			return challengeOverrides;
		}

		// Token: 0x04001115 RID: 4373
		public static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ChallengeOverrides");

		// Token: 0x04001116 RID: 4374
		public const string OverridesFilepath = "challengeOverrides.json";

		// Token: 0x04001117 RID: 4375
		public const string OverridesVersionFilepath = "challengeOverrides_version.json";

		// Token: 0x04001118 RID: 4376
		[Dependency]
		private IReachability _reachability;

		// Token: 0x04001119 RID: 4377
		[Dependency]
		private IFileSystem _fileSystem;

		// Token: 0x0400111A RID: 4378
		private ChallengeSystem _challengeSystem;

		// Token: 0x0400111B RID: 4379
		private ChallengeDatabase _challengeDatabase;

		// Token: 0x0400111C RID: 4380
		private MapDatabase _mapDatabase;

		// Token: 0x0400111D RID: 4381
		private readonly List<ChallengeOverride> _dailyChallenges = new List<ChallengeOverride>();

		// Token: 0x0400111E RID: 4382
		private readonly List<ChallengeOverride> _weeklyChallenges = new List<ChallengeOverride>();

		// Token: 0x0400111F RID: 4383
		private ChallengeOverrideVersion _overrideVersion = new ChallengeOverrideVersion();

		// Token: 0x04001120 RID: 4384
		private bool _isOpeningConnection;

		// Token: 0x04001121 RID: 4385
		private DateTime _timeLastRefreshedOverrides;

		// Token: 0x04001122 RID: 4386
		private static readonly TimeSpan MinRefreshTimespan = TimeSpan.FromMinutes(10.0);

		// Token: 0x02000343 RID: 835
		// (Invoke) Token: 0x060014AB RID: 5291
		private delegate MapChallenge ChallengeFactory(ChallengeSystem challengeSystem, MapDefinition mapDefinition, ChallengeData[] challenges, int timeStart, int timeEnd, ulong seed);

		// Token: 0x02000344 RID: 836
		public enum RefreshResult
		{
			// Token: 0x04001124 RID: 4388
			Error,
			// Token: 0x04001125 RID: 4389
			NoChange,
			// Token: 0x04001126 RID: 4390
			Success
		}
	}
}
