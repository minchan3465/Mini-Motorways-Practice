using System;
using System.Collections.Generic;
using Motorways;
using Motorways.Processes;

// Token: 0x020001AD RID: 429
public class LegacyMotorwaysUserProfile : LegacyBaseUserProfile
{
	// Token: 0x1700021A RID: 538
	// (get) Token: 0x06000986 RID: 2438 RVA: 0x0001F5EA File Offset: 0x0001D7EA
	// (set) Token: 0x06000987 RID: 2439 RVA: 0x0001F5F2 File Offset: 0x0001D7F2
	public bool IsColorblindModeEnabled
	{
		get
		{
			return this._isColorblindModeEnabled;
		}
		set
		{
			if (this._isColorblindModeEnabled != value)
			{
				this._isColorblindModeEnabled = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x1700021B RID: 539
	// (get) Token: 0x06000988 RID: 2440 RVA: 0x0001F60A File Offset: 0x0001D80A
	// (set) Token: 0x06000989 RID: 2441 RVA: 0x0001F612 File Offset: 0x0001D812
	public bool IsSkipTransitionsEnabled
	{
		get
		{
			return this._isSkipTransitionsEnabled;
		}
		set
		{
			if (this._isSkipTransitionsEnabled != value)
			{
				this._isSkipTransitionsEnabled = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x0001F62C File Offset: 0x0001D82C
	protected override void LoadFromJson(JSON.Dictionary jsonDictionary)
	{
		base.LoadFromJson(jsonDictionary);
		JSON.Array cityStatisticsJson = jsonDictionary.GetArray(LegacyMotorwaysUserProfile.CityStatisticsKey);
		if (cityStatisticsJson != null)
		{
			for (int currentCityIndex = 0; currentCityIndex < cityStatisticsJson.Count; currentCityIndex++)
			{
				MotorwaysCityStatistics newCityStatistics = this._scope.Get<MotorwaysCityStatistics>();
				newCityStatistics.DataChanged += this.OnCityStatisticsChanged;
				newCityStatistics.InitFromJson(cityStatisticsJson[currentCityIndex] as JSON.Dictionary);
				this._allCityStatistics.Add(newCityStatistics);
			}
		}
		this._completedTutorials = (TutorialProgressionProcess.TutorialType)jsonDictionary.GetInt(LegacyMotorwaysUserProfile.CompletedTutorialsKey, 0);
		this._isColorblindModeEnabled = jsonDictionary.GetBool(LegacyMotorwaysUserProfile.ColorblindModeKey, false);
		this._isSkipTransitionsEnabled = jsonDictionary.GetBool(LegacyMotorwaysUserProfile.SkipTransitionsKey, false);
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x0001F6D4 File Offset: 0x0001D8D4
	protected override void SaveToJson(Dictionary<string, object> jsonDictionary)
	{
		base.SaveToJson(jsonDictionary);
		List<object> cityStatisticsJson = new List<object>();
		foreach (MotorwaysCityStatistics eachCity in this._allCityStatistics)
		{
			cityStatisticsJson.Add(eachCity.ToJson());
		}
		jsonDictionary[LegacyMotorwaysUserProfile.CityStatisticsKey] = cityStatisticsJson;
		jsonDictionary[LegacyMotorwaysUserProfile.CompletedTutorialsKey] = (int)this._completedTutorials;
		jsonDictionary[LegacyMotorwaysUserProfile.ColorblindModeKey] = this._isColorblindModeEnabled;
		jsonDictionary[LegacyMotorwaysUserProfile.SkipTransitionsKey] = this._isSkipTransitionsEnabled;
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x0001F788 File Offset: 0x0001D988
	protected override void MergeValues(ForwardCompatibleJsonSaveData otherSaveData)
	{
		base.MergeValues(otherSaveData);
		LegacyMotorwaysUserProfile otherUserProfile = otherSaveData as LegacyMotorwaysUserProfile;
		if (otherUserProfile != null)
		{
			this.IsColorblindModeEnabled = base.ChooseLatest<bool>(this._isColorblindModeEnabled, otherUserProfile._isColorblindModeEnabled, otherUserProfile.UtcTimestamp);
			this.IsSkipTransitionsEnabled = base.ChooseLatest<bool>(this._isSkipTransitionsEnabled, otherUserProfile._isSkipTransitionsEnabled, otherUserProfile.UtcTimestamp);
			TutorialProgressionProcess.TutorialType oldValue = this._completedTutorials;
			this._completedTutorials |= otherUserProfile._completedTutorials;
			if (this._completedTutorials != oldValue)
			{
				base.OnValueChanged();
			}
			foreach (MotorwaysCityStatistics theirCityStatistics in otherUserProfile._allCityStatistics)
			{
				this.GetCityStatisticsForCity(theirCityStatistics.CityId, theirCityStatistics.Mode, true).Merge(theirCityStatistics);
			}
		}
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x0001F868 File Offset: 0x0001DA68
	public void SetTutorialTypeComplete(TutorialProgressionProcess.TutorialType completedType)
	{
		if ((this._completedTutorials & completedType) == TutorialProgressionProcess.TutorialType.None)
		{
			this._completedTutorials |= completedType;
			base.OnValueChanged();
		}
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x0001F888 File Offset: 0x0001DA88
	public bool IsTutorialTypeCompleted(TutorialProgressionProcess.TutorialType completedType)
	{
		return (this._completedTutorials & completedType) == completedType;
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x0001F895 File Offset: 0x0001DA95
	public bool IsAnyTutorialCompleted()
	{
		return this._completedTutorials > TutorialProgressionProcess.TutorialType.None;
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x0001F8A0 File Offset: 0x0001DAA0
	public void ClearTutorialCompletion()
	{
		this._completedTutorials = TutorialProgressionProcess.TutorialType.None;
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x0001F8AC File Offset: 0x0001DAAC
	public MotorwaysCityStatistics GetCityStatisticsForCity(string cityId, GameMode mode, bool createIfNecessary = false)
	{
		for (int currentCity = 0; currentCity < this._allCityStatistics.Count; currentCity++)
		{
			if (this._allCityStatistics[currentCity].CityId == cityId && this._allCityStatistics[currentCity].Mode == mode)
			{
				return this._allCityStatistics[currentCity];
			}
		}
		MotorwaysCityStatistics newCityStatistics = null;
		if (createIfNecessary)
		{
			newCityStatistics = this._scope.Get<MotorwaysCityStatistics>();
			newCityStatistics.DataChanged += this.OnCityStatisticsChanged;
			newCityStatistics.InitWithCityIdAndMode(cityId, mode);
			this._allCityStatistics.Add(newCityStatistics);
		}
		return newCityStatistics;
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x0001F944 File Offset: 0x0001DB44
	public override void RecordGameStatistics(IGameStatistics gameStatistics)
	{
		MotorwaysGameStatistics motorwaysGameStatistics = gameStatistics as MotorwaysGameStatistics;
		if (motorwaysGameStatistics != null)
		{
			MotorwaysCityStatistics cityStatistics = this.GetCityStatisticsForCity(motorwaysGameStatistics.CityId, motorwaysGameStatistics.Mode, true);
			if (!motorwaysGameStatistics.Challenge.HasChallenges)
			{
				cityStatistics.RecordGameStatistics(motorwaysGameStatistics);
				return;
			}
			cityStatistics.RecordCumulativeGameStatistics(motorwaysGameStatistics);
		}
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x0001F98B File Offset: 0x0001DB8B
	public void ClearCityStatistics()
	{
		this._allCityStatistics.Clear();
	}

	// Token: 0x1700021C RID: 540
	// (get) Token: 0x06000994 RID: 2452 RVA: 0x0001F998 File Offset: 0x0001DB98
	public int TotalPlayTime
	{
		get
		{
			int totalPlayTime = 0;
			foreach (MotorwaysCityStatistics cityStatistics in this._allCityStatistics)
			{
				totalPlayTime += cityStatistics.TotalPlayTime;
			}
			return totalPlayTime;
		}
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x0001F9F0 File Offset: 0x0001DBF0
	private void OnCityStatisticsChanged(MotorwaysCityStatistics changedCityStatistics)
	{
		base.OnValueChanged();
	}

	// Token: 0x04000504 RID: 1284
	private readonly List<MotorwaysCityStatistics> _allCityStatistics = new List<MotorwaysCityStatistics>();

	// Token: 0x04000505 RID: 1285
	private TutorialProgressionProcess.TutorialType _completedTutorials;

	// Token: 0x04000506 RID: 1286
	private bool _isColorblindModeEnabled;

	// Token: 0x04000507 RID: 1287
	private bool _isSkipTransitionsEnabled;

	// Token: 0x04000508 RID: 1288
	private static string ColorblindModeKey = "ColorBlindMode";

	// Token: 0x04000509 RID: 1289
	private static string SkipTransitionsKey = "SkipTransitions";

	// Token: 0x0400050A RID: 1290
	private static string CityStatisticsKey = "allCityStatistics";

	// Token: 0x0400050B RID: 1291
	private static string CompletedTutorialsKey = "completedTutorials";
}
