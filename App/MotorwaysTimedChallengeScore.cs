using System;
using System.Collections.Generic;
using Factory;
using Motorways;
using Motorways.Leaderboards;
using UnityEngine;

// Token: 0x020001B6 RID: 438
public class MotorwaysTimedChallengeScore
{
	// Token: 0x14000020 RID: 32
	// (add) Token: 0x06000A57 RID: 2647 RVA: 0x00021F94 File Offset: 0x00020194
	// (remove) Token: 0x06000A58 RID: 2648 RVA: 0x00021FCC File Offset: 0x000201CC
	public event Action<MotorwaysTimedChallengeScore> DataChanged;

	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06000A59 RID: 2649 RVA: 0x00022001 File Offset: 0x00020201
	// (set) Token: 0x06000A5A RID: 2650 RVA: 0x00022009 File Offset: 0x00020209
	public int Score { get; private set; } = -1;

	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06000A5B RID: 2651 RVA: 0x00022012 File Offset: 0x00020212
	public int Expiry
	{
		get
		{
			return this._expiry;
		}
	}

	// Token: 0x17000264 RID: 612
	// (get) Token: 0x06000A5C RID: 2652 RVA: 0x0002201A File Offset: 0x0002021A
	public bool HasScoreExpired
	{
		get
		{
			return this._expiry < this._challengeSystem.CurrentTimestamp;
		}
	}

	// Token: 0x17000265 RID: 613
	// (get) Token: 0x06000A5D RID: 2653 RVA: 0x0002202F File Offset: 0x0002022F
	public LeaderboardScoreState ScoreState
	{
		get
		{
			if (this.Score == -1 || this.HasScoreExpired)
			{
				return LeaderboardScoreState.NotSubmitted;
			}
			if (!this._isScoreLocked)
			{
				return LeaderboardScoreState.Editable;
			}
			return LeaderboardScoreState.Locked;
		}
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x00022050 File Offset: 0x00020250
	public void LockScore()
	{
		if (this._challengeType != MapChallenge.ChallengeType.Daily)
		{
			Diagnostics.FailAssert("Tried locking score with type {0}. Only daily challenge score should be locked.", new object[]
			{
				this._challengeType
			});
			return;
		}
		this._isScoreLocked = true;
		this._scoreLockedDateTime = GameDateTime.UtcNow;
		Action<MotorwaysTimedChallengeScore> dataChanged = this.DataChanged;
		if (dataChanged == null)
		{
			return;
		}
		dataChanged(this);
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x000220A8 File Offset: 0x000202A8
	public void Init(MapChallenge.ChallengeType challengeType, int expiry)
	{
		this._challengeType = challengeType;
		this._expiry = expiry;
		this.Score = -1;
		this._isScoreLocked = false;
		this._scoreLockedDateTime = DateTime.MinValue;
		Action<MotorwaysTimedChallengeScore> dataChanged = this.DataChanged;
		if (dataChanged == null)
		{
			return;
		}
		dataChanged(this);
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x000220E4 File Offset: 0x000202E4
	public void InitFromJson(JSON.Dictionary jsonDictionary, MapChallenge.ChallengeType challengeType)
	{
		if (jsonDictionary == null)
		{
			return;
		}
		this._challengeType = challengeType;
		this._expiry = jsonDictionary.GetInt("_expiry", 0);
		this.Score = jsonDictionary.GetInt("Score", -1);
		this._isScoreLocked = jsonDictionary.GetBool("_isScoreLocked", false);
		this._scoreLockedDateTime = jsonDictionary.GetDateTime("_scoreLockedDateTime");
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x00022144 File Offset: 0x00020344
	public object ToJson()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["_expiry"] = this._expiry;
		dictionary["Score"] = this.Score;
		dictionary["_isScoreLocked"] = this._isScoreLocked;
		dictionary["_scoreLockedDateTime"] = this._scoreLockedDateTime;
		return dictionary;
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x000221B0 File Offset: 0x000203B0
	public void UpdateGameScore(int newScore, GameEndReason? gameEndReason)
	{
		if (this.HasScoreExpired)
		{
			Diagnostics.FailAssert("UpdateGameScore should never be called on expired score.", Array.Empty<object>());
			return;
		}
		int currentBestScore = this.Score;
		if (MotorwaysScoreValidation.ShouldRecordScore(this._isScoreLocked, currentBestScore, newScore))
		{
			bool shouldLockScore = gameEndReason != null && MotorwaysScoreValidation.ShouldLockScoreWhenGameEnds(this._challengeType, gameEndReason.Value);
			this.Score = newScore;
			if (!this._isScoreLocked && shouldLockScore)
			{
				this._isScoreLocked = true;
				this._scoreLockedDateTime = GameDateTime.UtcNow;
			}
			Action<MotorwaysTimedChallengeScore> dataChanged = this.DataChanged;
			if (dataChanged == null)
			{
				return;
			}
			dataChanged(this);
		}
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x00022244 File Offset: 0x00020444
	public void Merge(MotorwaysTimedChallengeScore otherScore)
	{
		if (this._expiry == 0 && otherScore._expiry == 0)
		{
			return;
		}
		if (this._expiry < otherScore._expiry)
		{
			this._expiry = otherScore._expiry;
			this._isScoreLocked = otherScore._isScoreLocked;
			this._scoreLockedDateTime = otherScore._scoreLockedDateTime;
			this.Score = otherScore.Score;
			return;
		}
		if (this._expiry == otherScore._expiry)
		{
			if (this._challengeType == MapChallenge.ChallengeType.Weekly)
			{
				this.Score = Mathf.Max(this.Score, otherScore.Score);
				return;
			}
			if (this._challengeType != MapChallenge.ChallengeType.Daily)
			{
				Diagnostics.FailAssert("Unknown challenge type while merging MotorwaysTimedChallengeScore", Array.Empty<object>());
				return;
			}
			if (!this._isScoreLocked && !otherScore._isScoreLocked)
			{
				if (this.Score == -1 && otherScore.Score != -1)
				{
					this.Score = otherScore.Score;
				}
				return;
			}
			if (!this._isScoreLocked && otherScore._isScoreLocked)
			{
				this.Score = otherScore.Score;
				this._isScoreLocked = true;
				this._scoreLockedDateTime = otherScore._scoreLockedDateTime;
				return;
			}
			if (this._isScoreLocked && otherScore._isScoreLocked && otherScore._scoreLockedDateTime < this._scoreLockedDateTime)
			{
				this.Score = otherScore.Score;
				this._scoreLockedDateTime = otherScore._scoreLockedDateTime;
			}
		}
	}

	// Token: 0x04000576 RID: 1398
	private const int NoScoreRecorded = -1;

	// Token: 0x04000577 RID: 1399
	private const int InvalidExpiry = 0;

	// Token: 0x04000578 RID: 1400
	[Dependency]
	private ChallengeSystem _challengeSystem;

	// Token: 0x0400057A RID: 1402
	private int _expiry;

	// Token: 0x0400057B RID: 1403
	private bool _isScoreLocked;

	// Token: 0x0400057C RID: 1404
	private DateTime _scoreLockedDateTime;

	// Token: 0x0400057D RID: 1405
	private MapChallenge.ChallengeType _challengeType;
}
