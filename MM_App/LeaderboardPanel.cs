using System;
using System.Collections.Generic;
using Factory;
using JetBrains.Annotations;
using Motorways;
using Motorways.Leaderboards;
using Motorways.UI;
using Motorways.Views;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D2 RID: 466
public class LeaderboardPanel : MonoBehaviour
{
	// Token: 0x17000276 RID: 630
	// (get) Token: 0x06000B07 RID: 2823 RVA: 0x00024F76 File Offset: 0x00023176
	public LocalizedTextUI ErrorText
	{
		get
		{
			return this._errorText;
		}
	}

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x06000B08 RID: 2824 RVA: 0x00024F7E File Offset: 0x0002317E
	public TouchToggle SurroundingLeaderboardsButton
	{
		get
		{
			return this._surroundingLeaderboardsButton;
		}
	}

	// Token: 0x17000278 RID: 632
	// (get) Token: 0x06000B09 RID: 2825 RVA: 0x00024F86 File Offset: 0x00023186
	public TouchToggle FriendsLeaderboardsButton
	{
		get
		{
			return this._friendsLeaderboardsButton;
		}
	}

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x06000B0A RID: 2826 RVA: 0x00024F8E File Offset: 0x0002318E
	public TouchToggle GlobalLeaderboardsButton
	{
		get
		{
			return this._globalLeaderboardsButton;
		}
	}

	// Token: 0x1700027A RID: 634
	// (get) Token: 0x06000B0B RID: 2827 RVA: 0x00024F96 File Offset: 0x00023196
	public TouchToggle HistogramLeaderboardsButton
	{
		get
		{
			return this._histogramLeaderboardsButton;
		}
	}

	// Token: 0x1700027B RID: 635
	// (get) Token: 0x06000B0C RID: 2828 RVA: 0x00024F9E File Offset: 0x0002319E
	public TouchButton LeaderboardErrorButton
	{
		get
		{
			return this._leaderboardErrorButton;
		}
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x00024FA8 File Offset: 0x000231A8
	protected void Awake()
	{
		this._surroundingLeaderboardsButton.onValueChanged.AddListener(delegate(bool _)
		{
			this.ChangeTypeForLastRequestedLeaderboard(LeaderboardType.Surrounding);
		});
		this._friendsLeaderboardsButton.onValueChanged.AddListener(delegate(bool _)
		{
			this.ChangeTypeForLastRequestedLeaderboard(LeaderboardType.Friends);
		});
		this._globalLeaderboardsButton.onValueChanged.AddListener(delegate(bool _)
		{
			this.ChangeTypeForLastRequestedLeaderboard(LeaderboardType.Global);
		});
		this._histogramLeaderboardsButton.onValueChanged.AddListener(delegate(bool _)
		{
			this.ChangeTypeForLastRequestedLeaderboard(LeaderboardType.Histogram);
		});
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x00025028 File Offset: 0x00023228
	protected void Update()
	{
		if (this._requestError == null)
		{
			return;
		}
		if (Time.realtimeSinceStartup - this._initializeTime > 0.9f)
		{
			LeaderboardError unprocessedError = this._requestError;
			this._requestError = null;
			this.DisplayError(unprocessedError);
		}
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x00025068 File Offset: 0x00023268
	public void Initialize(IScope scope, TouchOptionButton recurringLeaderboardSelector, MapButton mapButton)
	{
		this._scope = scope;
		this._leaderboardSelector = recurringLeaderboardSelector;
		this._themeDatabase = scope.Get<MotorwaysThemeDatabase>();
		this._leaderboardService = scope.Get<LeaderboardService>();
		this._reachability = scope.Get<IReachability>();
		this._mapSelectScreen = scope.Get<MapSelectScreen>();
		this._lastRequestedLeaderboard = null;
		this._mapButton = mapButton;
		this._mapDefinition = mapButton.MapDefinition;
		this._histogram.Initialize(scope);
		bool canShowGlobal = this._leaderboardService.IsLeaderboardTypeSupported(LeaderboardType.Global);
		bool canShowFriends = this._leaderboardService.IsLeaderboardTypeSupported(LeaderboardType.Friends);
		bool canShowSurrounding = this._leaderboardService.IsLeaderboardTypeSupported(LeaderboardType.Surrounding);
		bool onlySupportsGlobal = canShowGlobal && !canShowFriends && !canShowSurrounding;
		bool isRecurringLeaderboard = mapButton.IsChallengeMapButton();
		this._surroundingLeaderboardsButton.gameObject.SetActive(!onlySupportsGlobal && canShowSurrounding);
		this._friendsLeaderboardsButton.gameObject.SetActive(!onlySupportsGlobal && canShowFriends);
		this._globalLeaderboardsButton.gameObject.SetActive(isRecurringLeaderboard && !onlySupportsGlobal);
		this._histogramLeaderboardsButton.gameObject.SetActive(canShowGlobal && !onlySupportsGlobal);
		if (!isRecurringLeaderboard)
		{
			this._leaderboardSelector.leftButton.gameObject.SetActive(!mapButton.AreChallengesLocked || mapButton.MapDefinition.IsExpertModeUnlocked(this._scope));
			this._leaderboardSelector.rightButton.gameObject.SetActive(!mapButton.AreChallengesLocked || mapButton.MapDefinition.IsExpertModeUnlocked(this._scope));
			if (!mapButton.MapDefinition.IsExpertModeUnlocked(this._scope))
			{
				this._leaderboardSelector.SkipOption(1);
			}
			for (int challengeIndex = 0; challengeIndex < this._mapDefinition.cityChallenges.Length; challengeIndex++)
			{
				CityChallengeData cityChallenge = this._mapDefinition.cityChallenges[challengeIndex];
				this._leaderboardSelector.options[2 + challengeIndex].GetComponent<LocalizedTextUI>().SetStringId(scope, cityChallenge.titleStringId);
			}
			for (int selectorIndex = 2 + this._mapDefinition.cityChallenges.Length; selectorIndex < this._leaderboardSelector.options.Length; selectorIndex++)
			{
				this._leaderboardSelector.SkipOption(selectorIndex);
			}
		}
		this._initializeTime = Time.realtimeSinceStartup;
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x00025290 File Offset: 0x00023490
	[UsedImplicitly]
	public void ChangeTypeForLastRequestedLeaderboard(LeaderboardType type)
	{
		this.ShowLeaderboardFor(type, this._lastRequestedLeaderboard);
		this._mapSelectScreen.PlayerSelectedLeaderboardType = new LeaderboardType?(type);
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x000252B0 File Offset: 0x000234B0
	public void ShowLeaderboardFor(LeaderboardType type, LeaderboardId leaderboardId)
	{
		if (type == LeaderboardType.Surrounding)
		{
			this._histogramParent.SetActive(false);
			this._leaderboardParent.SetActive(true);
			this.ShowSurroundingEntriesFor(leaderboardId);
			this._surroundingLeaderboardsButton.Set(true, false);
			this._friendsLeaderboardsButton.Set(false, false);
			this._globalLeaderboardsButton.Set(false, false);
			this._histogramLeaderboardsButton.Set(false, false);
			this._filterDisplayText.SetStringId(this._scope, StringId.LeaderboardFilter_Surrounding);
		}
		else if (type == LeaderboardType.Friends)
		{
			this._histogramParent.SetActive(false);
			this._leaderboardParent.SetActive(true);
			this.ShowTopFriendEntriesFor(leaderboardId);
			this._friendsLeaderboardsButton.Set(true, false);
			this._surroundingLeaderboardsButton.Set(false, false);
			this._globalLeaderboardsButton.Set(false, false);
			this._histogramLeaderboardsButton.Set(false, false);
			this._filterDisplayText.SetStringId(this._scope, StringId.LeaderboardFilter_Friends);
		}
		else if (type == LeaderboardType.Global)
		{
			this._histogramParent.SetActive(false);
			this._leaderboardParent.SetActive(true);
			this.ShowTopEntriesFor(leaderboardId);
			this._globalLeaderboardsButton.Set(true, false);
			this._histogramLeaderboardsButton.Set(false, false);
			this._surroundingLeaderboardsButton.Set(false, false);
			this._friendsLeaderboardsButton.Set(false, false);
			this._filterDisplayText.SetStringId(this._scope, StringId.LeaderboardFilter_Global);
		}
		else if (type == LeaderboardType.Histogram)
		{
			this.ShowHistogramFor(leaderboardId);
			this._histogramLeaderboardsButton.Set(true, false);
			this._globalLeaderboardsButton.Set(false, false);
			this._surroundingLeaderboardsButton.Set(false, false);
			this._friendsLeaderboardsButton.Set(false, false);
			this._filterDisplayText.SetStringId(this._scope, StringId.LeaderboardFilter_Histogram);
		}
		CityLeaderboardId cityLeaderboardId = leaderboardId as CityLeaderboardId;
		if (cityLeaderboardId != null)
		{
			if (cityLeaderboardId.CityChallengeIndex == -1)
			{
				int gameModeIndex = (cityLeaderboardId.Mode == CityGameMode.Expert) ? 1 : 0;
				this._leaderboardSelector.SetOption(gameModeIndex, false);
				return;
			}
			this._leaderboardSelector.SetOption(2 + cityLeaderboardId.CityChallengeIndex, false);
		}
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x000254AC File Offset: 0x000236AC
	private void ShowHistogramFor(LeaderboardId leaderboardId)
	{
		if (this._lastRequestedLeaderboard != null && this._lastRequestedLeaderboard.Equals(leaderboardId) && this._lastRequestedLeaderboardType == LeaderboardType.Histogram)
		{
			return;
		}
		AsyncRequestHandle requestHandle = this._requestHandle;
		if (requestHandle != null)
		{
			requestHandle.Cancel();
		}
		this.ClearError();
		this._histogramParent.SetActive(false);
		this._leaderboardParent.SetActive(false);
		this._lastRequestedLeaderboard = leaderboardId;
		this._lastRequestedLeaderboardType = LeaderboardType.Histogram;
		this._histogram.ShowHistogram(leaderboardId);
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x00025522 File Offset: 0x00023722
	public void OnHistogramSucceeded()
	{
		this.SetLoadingSpinnerEnabled(false);
		this._histogramParent.SetActive(true);
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x00025538 File Offset: 0x00023738
	public void OnHistogramFailed([CanBeNull] LeaderboardError error)
	{
		if (error != null && error.Code != LeaderboardErrorCode.NoData)
		{
			LeaderboardPanel.Log.Info("Error while requesting leaderboard entries. {0}", new object[]
			{
				error
			});
			this._requestError = error;
			return;
		}
		this._histogramParent.SetActive(false);
		this._leaderboardParent.SetActive(true);
		this.ShowTopEntriesFor(this._lastRequestedLeaderboard);
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x00025598 File Offset: 0x00023798
	private void ShowTopEntriesFor(LeaderboardId leaderboardId)
	{
		if (this._lastRequestedLeaderboard != null && this._lastRequestedLeaderboard.Equals(leaderboardId) && this._lastRequestedLeaderboardType == LeaderboardType.Global)
		{
			return;
		}
		this.ClearError();
		this.SetEntriesEnabled(false);
		this.SetLoadingSpinnerEnabled(true);
		this._lastRequestedLeaderboard = leaderboardId;
		this._lastRequestedLeaderboardType = LeaderboardType.Global;
		AsyncRequestHandle requestHandle = this._requestHandle;
		if (requestHandle != null)
		{
			requestHandle.Cancel();
		}
		this._requestHandle = this._leaderboardService.RequestTopEntries(leaderboardId, 10, new EntryRequestCompleted(this.OnEntryRequestCompleted));
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x00025618 File Offset: 0x00023818
	private void ShowTopFriendEntriesFor(LeaderboardId leaderboardId)
	{
		if (this._lastRequestedLeaderboard != null && this._lastRequestedLeaderboard.Equals(leaderboardId) && this._lastRequestedLeaderboardType == LeaderboardType.Friends)
		{
			return;
		}
		this.ClearError();
		this.SetEntriesEnabled(false);
		this.SetLoadingSpinnerEnabled(true);
		this._lastRequestedLeaderboard = leaderboardId;
		this._lastRequestedLeaderboardType = LeaderboardType.Friends;
		AsyncRequestHandle requestHandle = this._requestHandle;
		if (requestHandle != null)
		{
			requestHandle.Cancel();
		}
		this._requestHandle = this._leaderboardService.RequestTopFriendFilteredEntries(leaderboardId, 10, new EntryRequestCompleted(this.OnEntryRequestCompleted));
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x00025698 File Offset: 0x00023898
	private void ShowSurroundingEntriesFor(LeaderboardId leaderboardId)
	{
		if (this._lastRequestedLeaderboard != null && this._lastRequestedLeaderboard.Equals(leaderboardId) && this._lastRequestedLeaderboardType == LeaderboardType.Surrounding)
		{
			return;
		}
		this.ClearError();
		this.SetEntriesEnabled(false);
		this.SetLoadingSpinnerEnabled(true);
		this._lastRequestedLeaderboard = leaderboardId;
		this._lastRequestedLeaderboardType = LeaderboardType.Surrounding;
		AsyncRequestHandle requestHandle = this._requestHandle;
		if (requestHandle != null)
		{
			requestHandle.Cancel();
		}
		this._requestHandle = this._leaderboardService.RequestPlayerCenteredEntries(leaderboardId, 10, new EntryRequestCompleted(this.OnEntryRequestCompleted));
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x00025718 File Offset: 0x00023918
	private void OnEntryRequestCompleted(List<LeaderboardEntry> entries, long totalLeaderboardEntryCount, LeaderboardError error)
	{
		if (error != null)
		{
			LeaderboardPanel.Log.Info("Error while requesting leaderboard entries. {0}", new object[]
			{
				error
			});
			this._requestError = error;
			return;
		}
		this.ClearError();
		this.SetLoadingSpinnerEnabled(false);
		this.SetEntriesEnabled(true);
		if (entries == null)
		{
			this.EnsureExactNumberOfDisplayedEntries(0);
		}
		else
		{
			this.EnsureExactNumberOfDisplayedEntries(11);
			int entryIndex = 0;
			while (entryIndex < entries.Count && entryIndex < this._displayedEntries.Count)
			{
				LeaderboardEntry entry = entries[entryIndex];
				this._displayedEntries[entryIndex].UpdateFromLeaderboardEntry(entry, entryIndex % 2 == 1, totalLeaderboardEntryCount);
				entryIndex++;
			}
			for (int extraEntryIndex = entries.Count; extraEntryIndex < this._displayedEntries.Count; extraEntryIndex++)
			{
				this._displayedEntries[extraEntryIndex].SetAsBlankEntry(extraEntryIndex % 2 == 1);
			}
			this._displayedEntries[this._displayedEntries.Count - 1].GetComponent<Image>().sprite = this._bottomEntrySprite;
		}
		this._mapSelectScreen.RegisterThemeComponents(this._themeDatabase.GetTheme());
		this._mapSelectScreen.ApplyTheme(this._themeDatabase.GetTheme());
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x0002583C File Offset: 0x00023A3C
	private void DisplayError([NotNull] LeaderboardError error)
	{
		this.SetEntriesEnabled(false);
		this.SetLoadingSpinnerEnabled(false);
		this._lastError = LeaderboardErrorCode.None;
		if (Diagnostics.Verify(error != null && error.Code > LeaderboardErrorCode.None))
		{
			LeaderboardPanel.Log.Info("Leaderboard request resulted in error {0}.", new object[]
			{
				error
			});
			if (error.Code == LeaderboardErrorCode.NotAuthenticated && this._leaderboardService.CanAuthenticate)
			{
				if (!this._leaderboardService.Authenticate(delegate(bool didAuthenticate)
				{
					if (didAuthenticate)
					{
						this.ReloadLeaderboard();
						return;
					}
					this._mapButton.ShowCard(MapButton.Card.Main);
				}))
				{
					this._mapButton.ShowCard(MapButton.Card.Main);
				}
			}
			else
			{
				if (error.Description != StringId.None)
				{
					this._errorText.gameObject.SetActive(true);
					this._errorText.SetStringId(this._scope, error.Description);
				}
				if (error.Code == LeaderboardErrorCode.NoConnection && this._reachability.CanConnectManually)
				{
					this._leaderboardErrorButtonText.LocString = StandaloneLocString.CreateString(this._scope, StringId.Leaderboard_Connect);
					this._leaderboardErrorButton.gameObject.SetActive(true);
					this._lastError = LeaderboardErrorCode.NoConnection;
				}
			}
			this._leaderboardService.PresentError(error);
		}
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x00025956 File Offset: 0x00023B56
	private void ClearError()
	{
		this._errorText.gameObject.SetActive(false);
		this._leaderboardErrorButton.gameObject.SetActive(false);
		this._lastError = LeaderboardErrorCode.None;
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x00025981 File Offset: 0x00023B81
	public void SetLoadingSpinnerEnabled(bool isLoading)
	{
		this._loadingSpinner.SetActive(isLoading);
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x00025990 File Offset: 0x00023B90
	private void SetEntriesEnabled(bool isEnabled)
	{
		foreach (LeaderboardPanelEntry leaderboardPanelEntry in this._displayedEntries)
		{
			leaderboardPanelEntry.gameObject.SetActive(isEnabled);
		}
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x000259E8 File Offset: 0x00023BE8
	private void EnsureExactNumberOfDisplayedEntries(int entryCount)
	{
		if (this._displayedEntries.Count == entryCount || entryCount < 0)
		{
			return;
		}
		for (int entryIndexToRemove = this._displayedEntries.Count - 1; entryIndexToRemove >= entryCount; entryIndexToRemove--)
		{
			this._displayedEntries[entryIndexToRemove].gameObject.transform.SetParent(null, false);
			UnityEngine.Object.Destroy(this._displayedEntries[entryIndexToRemove].gameObject);
			this._displayedEntries.RemoveAt(entryIndexToRemove);
		}
		for (int entryIndexToAdd = this._displayedEntries.Count; entryIndexToAdd < entryCount; entryIndexToAdd++)
		{
			LeaderboardPanelEntry newEntry = UnityEngine.Object.Instantiate<LeaderboardPanelEntry>(this.leaderboardEntryRowPrefab, this.leaderboardEntriesParent.transform);
			if (Diagnostics.Verify(newEntry != null))
			{
				newEntry.InitializeWithScope(this._scope);
				this._displayedEntries.Add(newEntry);
			}
		}
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x00025AB4 File Offset: 0x00023CB4
	public void OnLeaderboardErrorButtonPressed()
	{
		if (this._lastError == LeaderboardErrorCode.NoConnection)
		{
			this.ClearError();
			this.SetEntriesEnabled(false);
			this.SetLoadingSpinnerEnabled(true);
			this._reachability.OpenManualConnection(delegate(InternetConnectionHandle request)
			{
				this.ReloadLeaderboard();
				request.Close();
			});
			return;
		}
		if (this._lastError == LeaderboardErrorCode.NotAuthenticated)
		{
			this._leaderboardService.Authenticate(delegate(bool didAuthenticate)
			{
				if (didAuthenticate)
				{
					this.ReloadLeaderboard();
				}
			});
		}
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x00025B17 File Offset: 0x00023D17
	private void OnDisable()
	{
		AsyncRequestHandle requestHandle = this._requestHandle;
		if (requestHandle == null)
		{
			return;
		}
		requestHandle.Cancel();
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x00025B2C File Offset: 0x00023D2C
	private void ReloadLeaderboard()
	{
		LeaderboardId lastRequestedLeaderboard = this._lastRequestedLeaderboard;
		this._lastRequestedLeaderboard = null;
		if (lastRequestedLeaderboard != null)
		{
			LeaderboardPanel.Log.Info("Forcing a reload of the leaderboard panel for {0}.", new object[]
			{
				lastRequestedLeaderboard
			});
			this.ShowLeaderboardFor(this._lastRequestedLeaderboardType, lastRequestedLeaderboard);
		}
	}

	// Token: 0x04000624 RID: 1572
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("LeaderboardPanel");

	// Token: 0x04000625 RID: 1573
	private const int MaximumTopEntries = 10;

	// Token: 0x04000626 RID: 1574
	private const int DisplayedRows = 11;

	// Token: 0x04000627 RID: 1575
	private const float MinVisibleDurationBeforeErrorDisplay = 0.9f;

	// Token: 0x04000628 RID: 1576
	[SerializeField]
	private Sprite _bottomEntrySprite;

	// Token: 0x04000629 RID: 1577
	[SerializeField]
	private LocalizedTextUI _errorText;

	// Token: 0x0400062A RID: 1578
	[SerializeField]
	private GameObject _loadingSpinner;

	// Token: 0x0400062B RID: 1579
	[SerializeField]
	private GameObject _histogramParent;

	// Token: 0x0400062C RID: 1580
	[SerializeField]
	private GameObject _leaderboardParent;

	// Token: 0x0400062D RID: 1581
	[SerializeField]
	private TouchToggle _surroundingLeaderboardsButton;

	// Token: 0x0400062E RID: 1582
	[SerializeField]
	private TouchToggle _friendsLeaderboardsButton;

	// Token: 0x0400062F RID: 1583
	[SerializeField]
	private TouchToggle _globalLeaderboardsButton;

	// Token: 0x04000630 RID: 1584
	[SerializeField]
	private TouchToggle _histogramLeaderboardsButton;

	// Token: 0x04000631 RID: 1585
	[SerializeField]
	private LocalizedTextUI _filterDisplayText;

	// Token: 0x04000632 RID: 1586
	[SerializeField]
	private TouchButton _leaderboardErrorButton;

	// Token: 0x04000633 RID: 1587
	[SerializeField]
	private LocalizedTextUI _leaderboardErrorButtonText;

	// Token: 0x04000634 RID: 1588
	[SerializeField]
	private Histogram _histogram;

	// Token: 0x04000635 RID: 1589
	public GameObject leaderboardEntriesParent;

	// Token: 0x04000636 RID: 1590
	private readonly List<LeaderboardPanelEntry> _displayedEntries = new List<LeaderboardPanelEntry>();

	// Token: 0x04000637 RID: 1591
	public LeaderboardPanelEntry leaderboardEntryRowPrefab;

	// Token: 0x04000638 RID: 1592
	private IScope _scope;

	// Token: 0x04000639 RID: 1593
	private MapSelectScreen _mapSelectScreen;

	// Token: 0x0400063A RID: 1594
	private LeaderboardService _leaderboardService;

	// Token: 0x0400063B RID: 1595
	private IReachability _reachability;

	// Token: 0x0400063C RID: 1596
	private MotorwaysThemeDatabase _themeDatabase;

	// Token: 0x0400063D RID: 1597
	private LeaderboardId _lastRequestedLeaderboard;

	// Token: 0x0400063E RID: 1598
	private LeaderboardType _lastRequestedLeaderboardType = LeaderboardType.Surrounding;

	// Token: 0x0400063F RID: 1599
	private AsyncRequestHandle _requestHandle;

	// Token: 0x04000640 RID: 1600
	private LeaderboardErrorCode _lastError;

	// Token: 0x04000641 RID: 1601
	private TouchOptionButton _leaderboardSelector;

	// Token: 0x04000642 RID: 1602
	private MapButton _mapButton;

	// Token: 0x04000643 RID: 1603
	private MapDefinition _mapDefinition;

	// Token: 0x04000644 RID: 1604
	private float _initializeTime;

	// Token: 0x04000645 RID: 1605
	private LeaderboardError _requestError;
}
