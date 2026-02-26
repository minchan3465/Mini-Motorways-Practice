using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Factory;
using JetBrains.Annotations;
using Motorways;
using Motorways.Leaderboards;
using Motorways.Models;
using NotificationService.Events;
using UnityEngine;

// Token: 0x020001B0 RID: 432
public class MotorwaysExtendedUserProfile : BaseExtendedUserProfile, ICreatedInScopeHandler
{
	// Token: 0x060009CC RID: 2508 RVA: 0x00020430 File Offset: 0x0001E630
	protected override void LoadFromJson(JSON.Dictionary jsonDictionary)
	{
		base.LoadFromJson(jsonDictionary);
		JSON.Dictionary challengeScoreJson = jsonDictionary.GetDictionary("AllChallengeScores");
		this.LoadChallengeOfType(challengeScoreJson, MapChallenge.ChallengeType.Daily);
		this.LoadChallengeOfType(challengeScoreJson, MapChallenge.ChallengeType.Weekly);
		this._areMenuMessagesEnabled = jsonDictionary.GetBool("AreMenuMessagesEnabled", false);
		this._isTapDrawEnabled = jsonDictionary.GetBool("IsTapDraw", false);
		this._controllerSensitivity = jsonDictionary.GetInt("ControllerSensitivity", 2);
		this._isDrawModeToggleEnabled = jsonDictionary.GetBool("IsDrawModeToggleEnabled", false);
		this._isTelemetryEnabled = jsonDictionary.GetBool("IsTelemetryEnabled", true);
		this._doesHudStartLocked = jsonDictionary.GetBool("DoesHudStartLockedKey", false);
		this.LoadColorblindPaletteIndexFromJson(jsonDictionary.GetArray("PlayerColorblindPaletteIndexes"));
		this.LoadCityChallengeScoresFromJson(jsonDictionary.GetArray("AllCityChallengeScores"));
		this.LoadUnsubmittedScoresFromJson(jsonDictionary.GetDictionary(this.UnsubmittedScoresKey));
		JSON.Array gameNotificationEventsJson = jsonDictionary.GetArray("_notificationEvents");
		this.LoadGameNotificationEvents(gameNotificationEventsJson);
		JSON.Dictionary achievementStatsJson = jsonDictionary.GetDictionary("AchievementStats");
		this._achievementStatistics.LoadFromJson(achievementStatsJson);
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x0002052C File Offset: 0x0001E72C
	protected override void SaveToJson(Dictionary<string, object> jsonDictionary)
	{
		base.SaveToJson(jsonDictionary);
		jsonDictionary["AllChallengeScores"] = this.GenerateChallengeScoreJson();
		jsonDictionary["_notificationEvents"] = this.GenerateGameNotificationJson();
		jsonDictionary["AreMenuMessagesEnabled"] = this._areMenuMessagesEnabled;
		jsonDictionary["IsTapDraw"] = this._isTapDrawEnabled;
		jsonDictionary["ControllerSensitivity"] = this._controllerSensitivity;
		jsonDictionary["IsDrawModeToggleEnabled"] = this._isDrawModeToggleEnabled;
		jsonDictionary["IsTelemetryEnabled"] = this._isTelemetryEnabled;
		jsonDictionary["DoesHudStartLockedKey"] = this._doesHudStartLocked;
		jsonDictionary["PlayerColorblindPaletteIndexes"] = this._playerColorblindPaletteIndexes;
		jsonDictionary["AllCityChallengeScores"] = this.GenerateCityChallengeScoresJson();
		jsonDictionary["AchievementStats"] = this._achievementStatistics.Save();
		jsonDictionary[this.UnsubmittedScoresKey] = this.GenerateUnsubmittedScoresJson();
	}

	// Token: 0x17000230 RID: 560
	// (get) Token: 0x060009CE RID: 2510 RVA: 0x00020630 File Offset: 0x0001E830
	// (set) Token: 0x060009CF RID: 2511 RVA: 0x00020638 File Offset: 0x0001E838
	public bool IsTapDrawEnabled
	{
		get
		{
			return this._isTapDrawEnabled;
		}
		set
		{
			if (this._isTapDrawEnabled != value)
			{
				this._isTapDrawEnabled = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000231 RID: 561
	// (get) Token: 0x060009D0 RID: 2512 RVA: 0x00020650 File Offset: 0x0001E850
	// (set) Token: 0x060009D1 RID: 2513 RVA: 0x00020658 File Offset: 0x0001E858
	public int ControllerSensitivity
	{
		get
		{
			return this._controllerSensitivity;
		}
		set
		{
			if (this._controllerSensitivity != value)
			{
				this._controllerSensitivity = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000232 RID: 562
	// (get) Token: 0x060009D2 RID: 2514 RVA: 0x00020670 File Offset: 0x0001E870
	// (set) Token: 0x060009D3 RID: 2515 RVA: 0x00020678 File Offset: 0x0001E878
	public bool IsDrawModeToggleEnabled
	{
		get
		{
			return this._isDrawModeToggleEnabled;
		}
		set
		{
			if (this._isDrawModeToggleEnabled != value)
			{
				this._isDrawModeToggleEnabled = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x060009D4 RID: 2516 RVA: 0x00020690 File Offset: 0x0001E890
	// (set) Token: 0x060009D5 RID: 2517 RVA: 0x00020698 File Offset: 0x0001E898
	public bool IsTelemetryEnabled
	{
		get
		{
			return this._isTelemetryEnabled;
		}
		set
		{
			if (this._isTelemetryEnabled != value)
			{
				this._isTelemetryEnabled = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000234 RID: 564
	// (get) Token: 0x060009D6 RID: 2518 RVA: 0x000206B0 File Offset: 0x0001E8B0
	// (set) Token: 0x060009D7 RID: 2519 RVA: 0x000206B8 File Offset: 0x0001E8B8
	public bool DoesHudStartLocked
	{
		get
		{
			return this._doesHudStartLocked;
		}
		set
		{
			if (this._doesHudStartLocked != value)
			{
				this._doesHudStartLocked = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000235 RID: 565
	// (get) Token: 0x060009D8 RID: 2520 RVA: 0x000206D0 File Offset: 0x0001E8D0
	// (set) Token: 0x060009D9 RID: 2521 RVA: 0x000206D8 File Offset: 0x0001E8D8
	public AchievementStatistics AchievementStatistics
	{
		get
		{
			return this._achievementStatistics;
		}
		set
		{
			this._achievementStatistics = value;
		}
	}

	// Token: 0x17000236 RID: 566
	// (get) Token: 0x060009DA RID: 2522 RVA: 0x000206E1 File Offset: 0x0001E8E1
	// (set) Token: 0x060009DB RID: 2523 RVA: 0x000206E9 File Offset: 0x0001E8E9
	public List<int> PlayerColorblindPaletteIndexes
	{
		get
		{
			return this._playerColorblindPaletteIndexes;
		}
		set
		{
			this._playerColorblindPaletteIndexes = value;
			base.OnValueChanged();
		}
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x000206F8 File Offset: 0x0001E8F8
	public void LoadColorblindPaletteIndexFromJson(JSON.Array indexArray)
	{
		if (indexArray != null && indexArray.Count > 0)
		{
			this._playerColorblindPaletteIndexes.Clear();
			for (int paletteIndex = 0; paletteIndex < indexArray.Count; paletteIndex++)
			{
				int entry = indexArray.GetInt(paletteIndex);
				this._playerColorblindPaletteIndexes.Add(entry);
			}
		}
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x00020744 File Offset: 0x0001E944
	private void LoadChallengeOfType(JSON.Dictionary statsJson, MapChallenge.ChallengeType challengeType)
	{
		MotorwaysTimedChallengeScore timedChallengeScore = this._scope.Get<MotorwaysTimedChallengeScore>();
		timedChallengeScore.DataChanged += this.OnChallengeScoreChanged;
		JSON.Dictionary challengeScore = (statsJson != null) ? statsJson.GetDictionary(challengeType.ToString()) : null;
		if (challengeScore != null)
		{
			timedChallengeScore.InitFromJson(challengeScore, challengeType);
		}
		this._allChallengeScores[challengeType] = timedChallengeScore;
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x0001F9F0 File Offset: 0x0001DBF0
	private void OnChallengeScoreChanged(MotorwaysTimedChallengeScore _)
	{
		base.OnValueChanged();
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x000207A4 File Offset: 0x0001E9A4
	[NotNull]
	public MotorwaysTimedChallengeScore GetChallengeScore(MapChallenge.ChallengeType challengeType, int expiry)
	{
		MotorwaysTimedChallengeScore challengeScore;
		if (this._allChallengeScores.TryGetValue(challengeType, out challengeScore))
		{
			if (challengeScore.Expiry < expiry)
			{
				challengeScore.Init(challengeType, expiry);
			}
			return challengeScore;
		}
		MotorwaysTimedChallengeScore newChallengeScore = this._scope.Get<MotorwaysTimedChallengeScore>();
		newChallengeScore.DataChanged += this.OnChallengeScoreChanged;
		newChallengeScore.Init(challengeType, expiry);
		this._allChallengeScores[challengeType] = newChallengeScore;
		return newChallengeScore;
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x00020808 File Offset: 0x0001EA08
	private Dictionary<string, object> GenerateChallengeScoreJson()
	{
		Dictionary<string, object> challengeScoreJson = new Dictionary<string, object>();
		foreach (KeyValuePair<MapChallenge.ChallengeType, MotorwaysTimedChallengeScore> challengeScore in this._allChallengeScores)
		{
			if (!challengeScore.Value.HasScoreExpired)
			{
				challengeScoreJson[challengeScore.Key.ToString()] = challengeScore.Value.ToJson();
			}
		}
		return challengeScoreJson;
	}

	// Token: 0x17000237 RID: 567
	// (get) Token: 0x060009E1 RID: 2529 RVA: 0x00020890 File Offset: 0x0001EA90
	public IEnumerable<CityChallengeStatistics> CityChallengeStatisticsEnumerator
	{
		get
		{
			return this._cityChallengeStatistics;
		}
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x00020898 File Offset: 0x0001EA98
	public CityChallengeStatistics GetCityChallengeScore(string cityId, GameMode mode, int challengeIndex, bool createIfEmpty = true)
	{
		foreach (CityChallengeStatistics stats in this._cityChallengeStatistics)
		{
			if (stats.CityId == cityId && stats.Mode == mode && stats.ChallengeIndex == challengeIndex)
			{
				return stats;
			}
		}
		if (createIfEmpty)
		{
			CityChallengeStatistics newStats = new CityChallengeStatistics(cityId, mode, challengeIndex, 0);
			newStats.DataChanged += base.OnValueChanged;
			this._cityChallengeStatistics.Add(newStats);
			return newStats;
		}
		return null;
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x0002093C File Offset: 0x0001EB3C
	public IEnumerable<CityChallengeStatistics> GetCityChallengeScores(string cityId, GameMode mode)
	{
		foreach (CityChallengeStatistics stats in this._cityChallengeStatistics)
		{
			if (stats.CityId == cityId && stats.Mode == mode)
			{
				yield return stats;
			}
		}
		List<CityChallengeStatistics>.Enumerator enumerator = default(List<CityChallengeStatistics>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x0002095C File Offset: 0x0001EB5C
	public void LoadCityChallengeScoresFromJson(JSON.Array challengeScores)
	{
		if (challengeScores != null)
		{
			for (int challengeIndex = 0; challengeIndex < challengeScores.Count; challengeIndex++)
			{
				CityChallengeStatistics stats = CityChallengeStatistics.InitFromJson(challengeScores[challengeIndex] as JSON.Dictionary);
				stats.DataChanged += base.OnValueChanged;
				this._cityChallengeStatistics.Add(stats);
			}
		}
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x000209B0 File Offset: 0x0001EBB0
	public List<object> GenerateCityChallengeScoresJson()
	{
		List<object> challenges = new List<object>();
		foreach (CityChallengeStatistics challenge in this._cityChallengeStatistics)
		{
			challenges.Add(challenge.ToJson());
		}
		return challenges;
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x00020A10 File Offset: 0x0001EC10
	public void LogUnsubmittedScore(LeaderboardId leaderboardId, int scoreToSubmitLater, LeaderboardScoreState state)
	{
		ValueTuple<int, LeaderboardScoreState> existingScoreAndState;
		if (this._unsubmittedScores.TryGetValue(leaderboardId, out existingScoreAndState))
		{
			if (existingScoreAndState.Item2 != LeaderboardScoreState.Locked)
			{
				this._unsubmittedScores[leaderboardId] = new ValueTuple<int, LeaderboardScoreState>(Mathf.Max(scoreToSubmitLater, existingScoreAndState.Item1), state);
			}
		}
		else
		{
			this._unsubmittedScores.Add(leaderboardId, new ValueTuple<int, LeaderboardScoreState>(scoreToSubmitLater, state));
		}
		base.OnValueChanged();
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x00020A70 File Offset: 0x0001EC70
	public IEnumerable<ValueTuple<LeaderboardId, int, LeaderboardScoreState>> GetAndClearUnsubmittedScores()
	{
		List<ValueTuple<LeaderboardId, int, LeaderboardScoreState>> unsubmittedScoreList = new List<ValueTuple<LeaderboardId, int, LeaderboardScoreState>>();
		foreach (KeyValuePair<LeaderboardId, ValueTuple<int, LeaderboardScoreState>> unsubmittedScore in this._unsubmittedScores)
		{
			LeaderboardId leaderboardId = unsubmittedScore.Key;
			bool isLeaderboardOpen = true;
			RecurringLeaderboardId recurringLeaderboardId = leaderboardId as RecurringLeaderboardId;
			if (recurringLeaderboardId != null)
			{
				isLeaderboardOpen = recurringLeaderboardId.IsLeaderboardOpen();
			}
			if (isLeaderboardOpen)
			{
				unsubmittedScoreList.Add(new ValueTuple<LeaderboardId, int, LeaderboardScoreState>(leaderboardId, unsubmittedScore.Value.Item1, unsubmittedScore.Value.Item2));
			}
		}
		this._unsubmittedScores.Clear();
		base.OnValueChanged();
		return unsubmittedScoreList;
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x00020B1C File Offset: 0x0001ED1C
	public IEnumerable<ValueTuple<LeaderboardId, int, LeaderboardScoreState>> GetUnsubmittedScores()
	{
		foreach (KeyValuePair<LeaderboardId, ValueTuple<int, LeaderboardScoreState>> unsubmittedScore in this._unsubmittedScores)
		{
			yield return new ValueTuple<LeaderboardId, int, LeaderboardScoreState>(unsubmittedScore.Key, unsubmittedScore.Value.Item1, unsubmittedScore.Value.Item2);
		}
		Dictionary<LeaderboardId, ValueTuple<int, LeaderboardScoreState>>.Enumerator enumerator = default(Dictionary<LeaderboardId, ValueTuple<int, LeaderboardScoreState>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x00020B2C File Offset: 0x0001ED2C
	public void MarkScoreAsSubmitted(LeaderboardId leaderboardId)
	{
		if (this._unsubmittedScores.ContainsKey(leaderboardId))
		{
			this._unsubmittedScores.Remove(leaderboardId);
			base.OnValueChanged();
		}
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x00020B50 File Offset: 0x0001ED50
	private Dictionary<string, object> GenerateUnsubmittedScoresJson()
	{
		Dictionary<string, object> json = new Dictionary<string, object>();
		foreach (KeyValuePair<LeaderboardId, ValueTuple<int, LeaderboardScoreState>> entry in this._unsubmittedScores)
		{
			string encodedScore = MotorwaysExtendedUserProfile.Caesar(entry.Value.Item1.ToString(), 17);
			json.Add(MotorwaysExtendedUserProfile.Caesar(entry.Key.SerializedString, 22), new object[]
			{
				encodedScore,
				(int)entry.Value.Item2
			});
		}
		return json;
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x00020BF8 File Offset: 0x0001EDF8
	private void LoadUnsubmittedScoresFromJson(JSON.Dictionary unsubmittedScoresJson)
	{
		if (unsubmittedScoresJson != null)
		{
			this._unsubmittedScores.Clear();
			foreach (string encodedLeaderboardId in unsubmittedScoresJson.Keys)
			{
				JSON.Array scoreAndState = unsubmittedScoresJson.GetArray(encodedLeaderboardId);
				if (scoreAndState != null && scoreAndState.Count == 2)
				{
					string encodedScore = scoreAndState.GetString(0);
					if (encodedScore != null)
					{
						encodedScore = MotorwaysExtendedUserProfile.Caesar(encodedScore, -17);
						LeaderboardScoreState state = (LeaderboardScoreState)scoreAndState.GetInt(1);
						int score;
						if (int.TryParse(encodedScore, out score) && score > 0)
						{
							LeaderboardId leaderboardId = LeaderboardId.Deserialize(MotorwaysExtendedUserProfile.Caesar(encodedLeaderboardId, -22));
							if (leaderboardId != null)
							{
								this._unsubmittedScores.Add(leaderboardId, new ValueTuple<int, LeaderboardScoreState>(score, state));
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x17000238 RID: 568
	// (get) Token: 0x060009EC RID: 2540 RVA: 0x00020CC0 File Offset: 0x0001EEC0
	public NotificationEvent? LatestNotificationEvent
	{
		get
		{
			return this._latestNotificationEvent;
		}
	}

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x060009ED RID: 2541 RVA: 0x00020CC8 File Offset: 0x0001EEC8
	public List<NotificationEvent> NotificationEvents
	{
		get
		{
			return this._notificationEvents;
		}
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x000022F5 File Offset: 0x000004F5
	public void ClearOldNotificationEvents()
	{
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x00020CD0 File Offset: 0x0001EED0
	public void AddGameNotificationEvent(NotificationEvent notificationEvent)
	{
		notificationEvent.Id = this._notificationEvents.Count;
		this._notificationEvents.Add(notificationEvent);
		this.UpdateLatestEvent(notificationEvent);
		base.OnValueChanged();
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x00020CFD File Offset: 0x0001EEFD
	public void UpdateGameNotificationEventWithId(int id, NotificationEvent updatedNotificationEvent)
	{
		updatedNotificationEvent.Id = id;
		this._notificationEvents[id] = updatedNotificationEvent;
		this.UpdateLatestEvent(updatedNotificationEvent);
		base.OnValueChanged();
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00020D21 File Offset: 0x0001EF21
	public void RemoveAllGameNotificationsEvents()
	{
		this._latestNotificationEvent = null;
		this._notificationEvents.Clear();
		base.OnValueChanged();
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x00020D40 File Offset: 0x0001EF40
	private void UpdateLatestEvent(NotificationEvent newNotificationEvent)
	{
		if (this._latestNotificationEvent == null)
		{
			this._latestNotificationEvent = new NotificationEvent?(newNotificationEvent);
			return;
		}
		if (newNotificationEvent.OccuredAt > this._latestNotificationEvent.Value.OccuredAt)
		{
			this._latestNotificationEvent = new NotificationEvent?(newNotificationEvent);
		}
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x00020D94 File Offset: 0x0001EF94
	private void LoadGameNotificationEvents(JSON.Array jsonArray)
	{
		if (jsonArray == null)
		{
			return;
		}
		for (int eventIndex = 0; eventIndex < jsonArray.Count; eventIndex++)
		{
			JSON.Dictionary dictionary = jsonArray[eventIndex] as JSON.Dictionary;
			if (dictionary != null)
			{
				NotificationEvent? possibleNotificationEvent = this.LoadGameNotificationEvent(dictionary);
				if (possibleNotificationEvent != null)
				{
					NotificationEvent notificationEvent = possibleNotificationEvent.Value;
					notificationEvent.Id = this._notificationEvents.Count;
					this._notificationEvents.Add(notificationEvent);
					this.UpdateLatestEvent(notificationEvent);
				}
			}
		}
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x00020E04 File Offset: 0x0001F004
	private NotificationEvent? LoadGameNotificationEvent(JSON.Dictionary jsonDictionary)
	{
		if (!jsonDictionary.ContainsKey("OccuredAt") || !jsonDictionary.ContainsKey("EventType"))
		{
			MotorwaysExtendedUserProfile.Log.Warn("OccuredAt or EventType not saved with notification event. Skipping...", Array.Empty<object>());
			return null;
		}
		DateTime occuredAt = jsonDictionary.GetDateTime("OccuredAt");
		string eventTypeString = jsonDictionary.GetString("EventType");
		Type eventTypeType = Type.GetType(eventTypeString);
		if (eventTypeType == null)
		{
			MotorwaysExtendedUserProfile.Log.Warn("Unknown eventType {0} when loading game notification event. Skipping...", new object[]
			{
				eventTypeString
			});
			return null;
		}
		INotificationEventType eventType = Activator.CreateInstance(eventTypeType) as INotificationEventType;
		INotificationEventTypeWithData eventTypeWithData = eventType as INotificationEventTypeWithData;
		if (eventTypeWithData != null && !eventTypeWithData.InitFromJson(jsonDictionary))
		{
			MotorwaysExtendedUserProfile.Log.Warn("Error while loading data for notification event {0}. Skipping...", new object[]
			{
				eventTypeString
			});
			return null;
		}
		return new NotificationEvent?(new NotificationEvent(occuredAt, eventType)
		{
			Id = this._notificationEvents.Count
		});
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00020F00 File Offset: 0x0001F100
	private List<object> GenerateGameNotificationJson()
	{
		List<object> gameNotificationJson = new List<object>();
		foreach (NotificationEvent gameNotificationEvent in this._notificationEvents)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["EventType"] = gameNotificationEvent.EventType.GetType().FullName;
			dictionary["OccuredAt"] = gameNotificationEvent.OccuredAt;
			Dictionary<string, object> gameNotificationDictionary = dictionary;
			INotificationEventTypeWithData eventTypeWithData = gameNotificationEvent.EventType as INotificationEventTypeWithData;
			if (eventTypeWithData != null)
			{
				eventTypeWithData.ToJson(ref gameNotificationDictionary);
			}
			gameNotificationJson.Add(gameNotificationDictionary);
		}
		return gameNotificationJson;
	}

	// Token: 0x1700023A RID: 570
	// (get) Token: 0x060009F6 RID: 2550 RVA: 0x00020FB0 File Offset: 0x0001F1B0
	// (set) Token: 0x060009F7 RID: 2551 RVA: 0x00020FB8 File Offset: 0x0001F1B8
	public bool AreMenuMessagesEnabled
	{
		get
		{
			return this._areMenuMessagesEnabled;
		}
		set
		{
			if (this._areMenuMessagesEnabled != value)
			{
				this._areMenuMessagesEnabled = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00020FD0 File Offset: 0x0001F1D0
	public override void RecordGameStatistics(IGameStatistics gameStatistics)
	{
		MotorwaysGameStatistics motorwaysGameStatistics = gameStatistics as MotorwaysGameStatistics;
		if (motorwaysGameStatistics != null && motorwaysGameStatistics.Challenge != null && motorwaysGameStatistics.Challenge.HasChallenges && (motorwaysGameStatistics.Challenge.challengeType == MapChallenge.ChallengeType.Daily || motorwaysGameStatistics.Challenge.challengeType == MapChallenge.ChallengeType.Weekly))
		{
			ActiveChallengesModel challenge = motorwaysGameStatistics.Challenge;
			if (challenge.IsActive)
			{
				this.GetChallengeScore(challenge.challengeType, challenge.timeEnd).UpdateGameScore(motorwaysGameStatistics.TotalTrips, motorwaysGameStatistics.GameEndReason);
			}
		}
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x0002104C File Offset: 0x0001F24C
	protected override void MergeValues(ForwardCompatibleJsonSaveData otherSaveData)
	{
		base.MergeValues(otherSaveData);
		MotorwaysExtendedUserProfile otherUserProfile = otherSaveData as MotorwaysExtendedUserProfile;
		if (otherUserProfile != null)
		{
			foreach (KeyValuePair<MapChallenge.ChallengeType, MotorwaysTimedChallengeScore> kvp in otherUserProfile._allChallengeScores)
			{
				this.GetChallengeScore(kvp.Key, kvp.Value.Expiry).Merge(kvp.Value);
			}
			foreach (CityChallengeStatistics otherStats in otherUserProfile._cityChallengeStatistics)
			{
				this.GetCityChallengeScore(otherStats.CityId, otherStats.Mode, otherStats.ChallengeIndex, true).Merge(otherStats);
			}
			foreach (KeyValuePair<LeaderboardId, ValueTuple<int, LeaderboardScoreState>> otherEntry in otherUserProfile._unsubmittedScores)
			{
				ValueTuple<int, LeaderboardScoreState> ourScore;
				if (this._unsubmittedScores.TryGetValue(otherEntry.Key, out ourScore))
				{
					if ((ourScore.Item2 != LeaderboardScoreState.Locked || otherEntry.Value.Item2 == LeaderboardScoreState.Locked) && (ourScore.Item1 < otherEntry.Value.Item1 || otherEntry.Value.Item2 == LeaderboardScoreState.Locked))
					{
						this._unsubmittedScores[otherEntry.Key] = new ValueTuple<int, LeaderboardScoreState>(otherEntry.Value.Item1, otherEntry.Value.Item2);
					}
				}
				else
				{
					this._unsubmittedScores.Add(otherEntry.Key, new ValueTuple<int, LeaderboardScoreState>(otherEntry.Value.Item1, otherEntry.Value.Item2));
				}
			}
			int ourEventCount = this._notificationEvents.Count;
			foreach (NotificationEvent otherEvent in otherUserProfile._notificationEvents)
			{
				bool addEvent = true;
				for (int ourEventIndex = 0; ourEventIndex < ourEventCount; ourEventIndex++)
				{
					NotificationEvent ourEvent = this._notificationEvents[ourEventIndex];
					if (otherEvent.OccuredAt == ourEvent.OccuredAt && otherEvent.EventType.Matches(ourEvent.EventType))
					{
						addEvent = false;
						break;
					}
				}
				if (addEvent)
				{
					this._notificationEvents.Add(otherEvent);
				}
			}
			this.AreMenuMessagesEnabled = base.ChooseLatest<bool>(this._areMenuMessagesEnabled, otherUserProfile._areMenuMessagesEnabled, otherUserProfile.UtcTimestamp);
			this.DoesHudStartLocked = base.ChooseLatest<bool>(this._doesHudStartLocked, otherUserProfile._doesHudStartLocked, otherUserProfile.UtcTimestamp);
			this.IsTapDrawEnabled = base.ChooseLatest<bool>(this._isTapDrawEnabled, otherUserProfile._isTapDrawEnabled, otherUserProfile.UtcTimestamp);
			this.ControllerSensitivity = base.ChooseLatest<int>(this._controllerSensitivity, otherUserProfile._controllerSensitivity, otherUserProfile.UtcTimestamp);
			this.IsDrawModeToggleEnabled = base.ChooseLatest<bool>(this._isDrawModeToggleEnabled, otherUserProfile._isDrawModeToggleEnabled, otherUserProfile.UtcTimestamp);
			this.IsTelemetryEnabled = base.ChooseLatest<bool>(this._isTelemetryEnabled, otherUserProfile._isTelemetryEnabled, otherUserProfile.UtcTimestamp);
			this.PlayerColorblindPaletteIndexes = base.ChooseLatest<List<int>>(this._playerColorblindPaletteIndexes, otherUserProfile._playerColorblindPaletteIndexes, otherUserProfile.UtcTimestamp);
			this._achievementStatistics.Merge(otherUserProfile.AchievementStatistics, base.UtcTimestamp, otherUserProfile.UtcTimestamp);
		}
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x000213C4 File Offset: 0x0001F5C4
	public static string Caesar(string source, short shift)
	{
		int maxChar = Convert.ToInt32(char.MaxValue);
		int minChar = Convert.ToInt32('\0');
		char[] buffer = source.ToCharArray();
		for (int charIndex = 0; charIndex < buffer.Length; charIndex++)
		{
			int shifted = Convert.ToInt32(buffer[charIndex]) + (int)shift;
			if (shifted > maxChar)
			{
				shifted -= maxChar;
			}
			else if (shifted < minChar)
			{
				shifted += maxChar;
			}
			buffer[charIndex] = Convert.ToChar(shifted);
		}
		return new string(buffer);
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x0002142C File Offset: 0x0001F62C
	public void OnCreatedInScope(IScope scope)
	{
		this._achievementStatistics.DataChanged += base.OnValueChanged;
	}

	// Token: 0x04000531 RID: 1329
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("MotorwaysExtendedUserProfile");

	// Token: 0x04000532 RID: 1330
	private bool _isTapDrawEnabled;

	// Token: 0x04000533 RID: 1331
	private const string IsTapDrawEnabledKey = "IsTapDraw";

	// Token: 0x04000534 RID: 1332
	private const int DefaultControllerSensitivity = 2;

	// Token: 0x04000535 RID: 1333
	private int _controllerSensitivity = 2;

	// Token: 0x04000536 RID: 1334
	private const string ControllerSensitivityKey = "ControllerSensitivity";

	// Token: 0x04000537 RID: 1335
	private bool _isDrawModeToggleEnabled;

	// Token: 0x04000538 RID: 1336
	private const string IsDrawModeToggleEnabledKey = "IsDrawModeToggleEnabled";

	// Token: 0x04000539 RID: 1337
	private bool _isTelemetryEnabled = true;

	// Token: 0x0400053A RID: 1338
	private const string IsTelemetryEnabledKey = "IsTelemetryEnabled";

	// Token: 0x0400053B RID: 1339
	private bool _doesHudStartLocked;

	// Token: 0x0400053C RID: 1340
	private const string DoesHudStartLockedKey = "DoesHudStartLockedKey";

	// Token: 0x0400053D RID: 1341
	private AchievementStatistics _achievementStatistics = new AchievementStatistics();

	// Token: 0x0400053E RID: 1342
	private const string AchievementStatsKey = "AchievementStats";

	// Token: 0x0400053F RID: 1343
	private const string PlayerColorblindPaletteIndexesKey = "PlayerColorblindPaletteIndexes";

	// Token: 0x04000540 RID: 1344
	private List<int> _playerColorblindPaletteIndexes = new List<int>
	{
		0,
		1,
		2,
		3,
		4,
		5
	};

	// Token: 0x04000541 RID: 1345
	private readonly Dictionary<MapChallenge.ChallengeType, MotorwaysTimedChallengeScore> _allChallengeScores = new Dictionary<MapChallenge.ChallengeType, MotorwaysTimedChallengeScore>();

	// Token: 0x04000542 RID: 1346
	private const string ChallengeScoreKey = "AllChallengeScores";

	// Token: 0x04000543 RID: 1347
	private readonly List<CityChallengeStatistics> _cityChallengeStatistics = new List<CityChallengeStatistics>();

	// Token: 0x04000544 RID: 1348
	private const string CityChallengeScoreKey = "AllCityChallengeScores";

	// Token: 0x04000545 RID: 1349
	[TupleElementNames(new string[]
	{
		"score",
		"state"
	})]
	private readonly Dictionary<LeaderboardId, ValueTuple<int, LeaderboardScoreState>> _unsubmittedScores = new Dictionary<LeaderboardId, ValueTuple<int, LeaderboardScoreState>>();

	// Token: 0x04000546 RID: 1350
	private readonly string UnsubmittedScoresKey = MotorwaysExtendedUserProfile.Caesar("UnsubmittedScores", 22);

	// Token: 0x04000547 RID: 1351
	private const int StringDecodeKey = 22;

	// Token: 0x04000548 RID: 1352
	private const int ScoreDecodeKey = 17;

	// Token: 0x04000549 RID: 1353
	private NotificationEvent? _latestNotificationEvent;

	// Token: 0x0400054A RID: 1354
	private List<NotificationEvent> _notificationEvents = new List<NotificationEvent>();

	// Token: 0x0400054B RID: 1355
	private bool _areMenuMessagesEnabled = true;

	// Token: 0x0400054C RID: 1356
	private const string AreMenuMessagesEnabledKey = "AreMenuMessagesEnabled";
}
