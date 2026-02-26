using System;
using System.Collections.Generic;
using Motorways;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class MotorwaysCityStatistics
{
	// Token: 0x1400001F RID: 31
	// (add) Token: 0x06000998 RID: 2456 RVA: 0x0001FA38 File Offset: 0x0001DC38
	// (remove) Token: 0x06000999 RID: 2457 RVA: 0x0001FA70 File Offset: 0x0001DC70
	public event Action<MotorwaysCityStatistics> DataChanged;

	// Token: 0x1700021D RID: 541
	// (get) Token: 0x0600099A RID: 2458 RVA: 0x0001FAA5 File Offset: 0x0001DCA5
	// (set) Token: 0x0600099B RID: 2459 RVA: 0x0001FAAD File Offset: 0x0001DCAD
	public string CityId { get; private set; }

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x0600099C RID: 2460 RVA: 0x0001FAB6 File Offset: 0x0001DCB6
	// (set) Token: 0x0600099D RID: 2461 RVA: 0x0001FABE File Offset: 0x0001DCBE
	public GameMode Mode { get; private set; }

	// Token: 0x1700021F RID: 543
	// (get) Token: 0x0600099E RID: 2462 RVA: 0x0001FAC7 File Offset: 0x0001DCC7
	// (set) Token: 0x0600099F RID: 2463 RVA: 0x0001FACF File Offset: 0x0001DCCF
	public int MaxTrips
	{
		get
		{
			return this._maxTrips;
		}
		set
		{
			this._maxTrips = value;
			Action<MotorwaysCityStatistics> dataChanged = this.DataChanged;
			if (dataChanged == null)
			{
				return;
			}
			dataChanged(this);
		}
	}

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x060009A0 RID: 2464 RVA: 0x0001FAE9 File Offset: 0x0001DCE9
	// (set) Token: 0x060009A1 RID: 2465 RVA: 0x0001FAF1 File Offset: 0x0001DCF1
	public int MaxTripsDayCount
	{
		get
		{
			return this._maxTripsDayCount;
		}
		set
		{
			this._maxTripsDayCount = value;
			Action<MotorwaysCityStatistics> dataChanged = this.DataChanged;
			if (dataChanged == null)
			{
				return;
			}
			dataChanged(this);
		}
	}

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x060009A2 RID: 2466 RVA: 0x0001FB0B File Offset: 0x0001DD0B
	// (set) Token: 0x060009A3 RID: 2467 RVA: 0x0001FB13 File Offset: 0x0001DD13
	public int MaxAverageTrips
	{
		get
		{
			return this._maxAverageTrips;
		}
		set
		{
			this._maxAverageTrips = value;
			Action<MotorwaysCityStatistics> dataChanged = this.DataChanged;
			if (dataChanged == null)
			{
				return;
			}
			dataChanged(this);
		}
	}

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x060009A4 RID: 2468 RVA: 0x0001FB2D File Offset: 0x0001DD2D
	// (set) Token: 0x060009A5 RID: 2469 RVA: 0x0001FB35 File Offset: 0x0001DD35
	public int TotalTrips
	{
		get
		{
			return this._totalTrips;
		}
		set
		{
			this._totalTrips = value;
			Action<MotorwaysCityStatistics> dataChanged = this.DataChanged;
			if (dataChanged == null)
			{
				return;
			}
			dataChanged(this);
		}
	}

	// Token: 0x17000223 RID: 547
	// (get) Token: 0x060009A6 RID: 2470 RVA: 0x0001FB4F File Offset: 0x0001DD4F
	// (set) Token: 0x060009A7 RID: 2471 RVA: 0x0001FB57 File Offset: 0x0001DD57
	public int MaxDuration
	{
		get
		{
			return this._maxDuration;
		}
		set
		{
			this._maxDuration = value;
			Action<MotorwaysCityStatistics> dataChanged = this.DataChanged;
			if (dataChanged == null)
			{
				return;
			}
			dataChanged(this);
		}
	}

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x060009A8 RID: 2472 RVA: 0x0001FB71 File Offset: 0x0001DD71
	// (set) Token: 0x060009A9 RID: 2473 RVA: 0x0001FB79 File Offset: 0x0001DD79
	public int TotalDuration
	{
		get
		{
			return this._totalDuration;
		}
		set
		{
			this._totalDuration = value;
			Action<MotorwaysCityStatistics> dataChanged = this.DataChanged;
			if (dataChanged == null)
			{
				return;
			}
			dataChanged(this);
		}
	}

	// Token: 0x17000225 RID: 549
	// (get) Token: 0x060009AA RID: 2474 RVA: 0x0001FB93 File Offset: 0x0001DD93
	// (set) Token: 0x060009AB RID: 2475 RVA: 0x0001FB9B File Offset: 0x0001DD9B
	public int TotalPlayTime
	{
		get
		{
			return this._totalPlayTime;
		}
		set
		{
			this._totalPlayTime = value;
			Action<MotorwaysCityStatistics> dataChanged = this.DataChanged;
			if (dataChanged == null)
			{
				return;
			}
			dataChanged(this);
		}
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x0001FBB5 File Offset: 0x0001DDB5
	public void InitWithCityIdAndMode(string cityId, GameMode mode)
	{
		this.CityId = cityId;
		this.Mode = mode;
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x0001FBC8 File Offset: 0x0001DDC8
	public void InitFromJson(JSON.Dictionary jsonDictionary)
	{
		if (jsonDictionary == null)
		{
			return;
		}
		this.CityId = (jsonDictionary.GetString("CityId") ?? "");
		string modeString = jsonDictionary.GetString("Mode");
		if (!string.IsNullOrEmpty(modeString))
		{
			GameMode jsonMode;
			if (Diagnostics.Verify(Enum.TryParse<GameMode>(modeString, out jsonMode), "{0} is not a valid game mode! Setting to Normal.", modeString))
			{
				this.Mode = jsonMode;
			}
			else
			{
				this.Mode = GameMode.Normal;
			}
		}
		this._maxTrips = jsonDictionary.GetInt("MaxTrips", 0);
		this._maxTripsDayCount = jsonDictionary.GetInt("MaxTripsDayCount", 0);
		this._maxAverageTrips = jsonDictionary.GetInt("MaxAverageTrips", 0);
		this._totalTrips = jsonDictionary.GetInt("TotalTrips", 0);
		this._maxDuration = jsonDictionary.GetInt("MaxDuration", 0);
		this._totalDuration = jsonDictionary.GetInt("TotalDuration", 0);
		this._totalPlayTime = jsonDictionary.GetInt("TotalPlayTime", 0);
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x0001FCAC File Offset: 0x0001DEAC
	public void RecordGameStatistics(MotorwaysGameStatistics motorwaysGameStatistics)
	{
		this.RecordCumulativeGameStatistics(motorwaysGameStatistics);
		if (motorwaysGameStatistics.TotalTrips > this.MaxTrips)
		{
			this.MaxTrips = motorwaysGameStatistics.TotalTrips;
			this.MaxTripsDayCount = motorwaysGameStatistics.TotalDuration;
		}
		this.MaxAverageTrips = Mathf.Max(this.MaxAverageTrips, motorwaysGameStatistics.PeakAverageTrips);
		this.MaxDuration = Mathf.Max(this.MaxDuration, motorwaysGameStatistics.TotalDuration);
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x0001FD14 File Offset: 0x0001DF14
	public void RecordCumulativeGameStatistics(MotorwaysGameStatistics motorwaysGameStatistics)
	{
		this.TotalTrips += motorwaysGameStatistics.NewTrips;
		this.TotalDuration += motorwaysGameStatistics.NewDuration;
		this.TotalPlayTime += motorwaysGameStatistics.NewPlayTime;
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x0001FD50 File Offset: 0x0001DF50
	public void Merge(MotorwaysCityStatistics otherStatistics)
	{
		this.MaxTrips = Mathf.Max(this.MaxTrips, otherStatistics.MaxTrips);
		this.MaxTripsDayCount = Mathf.Max(this.MaxTripsDayCount, otherStatistics.MaxTripsDayCount);
		this.MaxAverageTrips = Mathf.Max(this.MaxAverageTrips, otherStatistics.MaxAverageTrips);
		this.TotalTrips = Mathf.Max(this.TotalTrips, otherStatistics.TotalTrips);
		this.MaxDuration = Mathf.Max(this.MaxDuration, otherStatistics.MaxDuration);
		this.TotalDuration = Mathf.Max(this.TotalDuration, otherStatistics.TotalDuration);
		this.TotalPlayTime = Mathf.Max(this.TotalPlayTime, otherStatistics.TotalPlayTime);
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x0001FE00 File Offset: 0x0001E000
	public object ToJson()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["CityId"] = this.CityId;
		dictionary["Mode"] = this.Mode.ToString();
		dictionary["MaxTrips"] = this.MaxTrips;
		dictionary["MaxTripsDayCount"] = this.MaxTripsDayCount;
		dictionary["MaxAverageTrips"] = this.MaxAverageTrips;
		dictionary["TotalTrips"] = this.TotalTrips;
		dictionary["MaxDuration"] = this.MaxDuration;
		dictionary["TotalDuration"] = this.TotalDuration;
		dictionary["TotalPlayTime"] = this.TotalPlayTime;
		return dictionary;
	}

	// Token: 0x0400050F RID: 1295
	private int _maxTrips;

	// Token: 0x04000510 RID: 1296
	private int _maxTripsDayCount;

	// Token: 0x04000511 RID: 1297
	private int _maxAverageTrips;

	// Token: 0x04000512 RID: 1298
	private int _totalTrips;

	// Token: 0x04000513 RID: 1299
	private int _maxDuration;

	// Token: 0x04000514 RID: 1300
	private int _totalDuration;

	// Token: 0x04000515 RID: 1301
	private int _totalPlayTime;
}
