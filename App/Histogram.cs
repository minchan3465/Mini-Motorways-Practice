using System;
using System.Collections.Generic;
using Easing;
using Factory;
using JetBrains.Annotations;
using Motorways;
using Motorways.Leaderboards;
using Motorways.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001CE RID: 462
public class Histogram : MonoBehaviour
{
	// Token: 0x06000AEA RID: 2794 RVA: 0x00024318 File Offset: 0x00022518
	public void Initialize(IScope scope)
	{
		this._scope = scope;
		this._player = this._scope.Get<ActivePlayer>();
		this._themeDatabase = scope.Get<MotorwaysThemeDatabase>();
		this._leaderboardService = scope.Get<LeaderboardService>();
		this._mapSelectScreen = scope.Get<MapSelectScreen>();
		this._mapDatabase = scope.Get<MapDatabase>();
		this.Clear();
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x00024374 File Offset: 0x00022574
	private void Clear()
	{
		for (int notchIndex = 0; notchIndex < this._indicatorNotches.Count; notchIndex++)
		{
			this._indicatorNotches[notchIndex].gameObject.SetActive(false);
		}
		for (int columnIndex = 0; columnIndex < this._columns.Count; columnIndex++)
		{
			this._columns[columnIndex].RectTransform.sizeDelta = new Vector2(0f, 0f);
			this._columns[columnIndex].SubRectTransform.sizeDelta = new Vector2(0f, 0f);
		}
		this._youText.gameObject.SetActive(false);
		Vector3 anchoredPosition = this._youBar.anchoredPosition;
		anchoredPosition.x = 0f;
		this._youBar.anchoredPosition = anchoredPosition;
		this._youBar.gameObject.SetActive(false);
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0002445E File Offset: 0x0002265E
	public void ShowHistogram(LeaderboardId leaderboardId)
	{
		this.Clear();
		this.BuildAHistogram(leaderboardId);
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x00024470 File Offset: 0x00022670
	private void BuildAHistogram(LeaderboardId leaderboardId)
	{
		AsyncRequestHandle requestHandle = this._requestHandle;
		if (requestHandle != null)
		{
			requestHandle.Cancel();
		}
		this._leaderboardPanel.ErrorText.gameObject.SetActive(false);
		this._leaderboardPanel.SetLoadingSpinnerEnabled(true);
		CityLeaderboardId cityLeaderboardId = leaderboardId as CityLeaderboardId;
		if (cityLeaderboardId != null)
		{
			MapDefinition mapDefinition = this._mapDatabase.MapLibrary.GetMapByName(cityLeaderboardId.City.ToString());
			if (Diagnostics.Verify(mapDefinition != null))
			{
				this._cityText.SetStringId(this._scope, mapDefinition.mapName);
			}
		}
		else if (leaderboardId is DailyLeaderboardId)
		{
			this._cityText.SetStringId(this._scope, StringId.DailyChallenge);
		}
		else if (leaderboardId is WeeklyLeaderboardId)
		{
			this._cityText.SetStringId(this._scope, StringId.WeeklyChallenge);
		}
		this._highScoreText.LocString = null;
		this._requestHandle = this._leaderboardService.RequestHistogram(leaderboardId, delegate(List<int> buckets, int size, LeaderboardError error)
		{
			if (error == null && buckets != null && buckets.Count > 0 && size > 0)
			{
				this._bucketRange = size;
				this.GenerateBuckets(buckets);
				this.OnHistogramRetrieved(leaderboardId);
				return;
			}
			Histogram.Log.Warn("Failed to get histogram data.\n{0}", new object[]
			{
				error
			});
			this._leaderboardPanel.OnHistogramFailed(error);
		});
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0002459C File Offset: 0x0002279C
	private void LocalEntryRequestCompleted(LeaderboardEntry localEntry, long totalLeaderboardEntryCount, LeaderboardError error)
	{
		if (error != null || localEntry == null)
		{
			this.OnEntryRequestComplete(null, totalLeaderboardEntryCount);
		}
		else
		{
			this.OnEntryRequestComplete(localEntry, totalLeaderboardEntryCount);
		}
		this._mapSelectScreen.RegisterThemeComponents(this._themeDatabase.GetTheme());
		this._mapSelectScreen.ApplyTheme(this._themeDatabase.GetTheme());
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x000245F0 File Offset: 0x000227F0
	private void OnEntryRequestComplete(LeaderboardEntry localEntry, long totalLeaderboardEntryCount)
	{
		if (localEntry == null || localEntry.Rank <= 0L || localEntry.Score < 0)
		{
			this._youText.gameObject.SetActive(false);
			this._youBar.gameObject.SetActive(false);
			this._highScoreText.LocString = StandaloneLocString.CreateNonLocalizedString(this._scope, "-");
		}
		else
		{
			this._youText.LocString = localEntry.FormatLocalUserString(this._scope, totalLeaderboardEntryCount, LeaderboardEntryFormatOptions.BoldYou | LeaderboardEntryFormatOptions.MultiLine | LeaderboardEntryFormatOptions.IncludePercentileInTopTen);
			int score = localEntry.Score;
			this._highScoreText.LocString = StandaloneLocString.CreateLocalizedNumberString(this._scope, score);
			float horizontalScoreLocation = 1f;
			float maxHistogramScore = this._columns[this._columns.Count - 1].EndRange;
			if ((float)score < maxHistogramScore)
			{
				horizontalScoreLocation = (float)score / maxHistogramScore;
			}
			float halfBarWidth = this._youBar.rect.width * 0.5f;
			float barXPos = Mathf.Lerp(halfBarWidth, this._columnParent.rect.width - halfBarWidth, horizontalScoreLocation);
			Histogram.ConstrainCenteredText(this._youText.TextField, barXPos, this._columnParent.rect.width);
			Vector3 anchoredPosition = this._youBar.anchoredPosition;
			anchoredPosition.x = barXPos;
			this._youBar.anchoredPosition = anchoredPosition;
			this._youText.gameObject.SetActive(true);
			this._youBar.gameObject.SetActive(true);
		}
		this._leaderboardPanel.OnHistogramSucceeded();
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x00024778 File Offset: 0x00022978
	private void GenerateBuckets([NotNull] List<int> rawBuckets)
	{
		this._buckets.Clear();
		this._buckets.Capacity = rawBuckets.Count;
		int maxBucketSize = 0;
		int rawBucketCount = rawBuckets.Count;
		for (int rawBucketIndex = 0; rawBucketIndex < rawBucketCount; rawBucketIndex++)
		{
			int bucketSize = rawBuckets[rawBucketIndex];
			this._buckets.Add(bucketSize);
			maxBucketSize = Mathf.Max(maxBucketSize, bucketSize);
		}
		int minBucketSize = Mathf.CeilToInt((float)maxBucketSize * this._minBucketSizeRelativeToMax);
		int longTailLength = 0;
		while (longTailLength < this._buckets.Count && this._buckets[this._buckets.Count - 1 - longTailLength] < minBucketSize)
		{
			longTailLength++;
		}
		if (longTailLength > 0)
		{
			this._buckets.RemoveRange(this._buckets.Count - 1 - longTailLength, longTailLength);
		}
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x0002483C File Offset: 0x00022A3C
	private void OnHistogramRetrieved(LeaderboardId leaderboardId)
	{
		if (this._buckets.Count < this._columns.Count)
		{
			this._leaderboardPanel.OnHistogramFailed(null);
			return;
		}
		float maxHeight = this._columnParent.rect.height;
		int bucketCount = this._buckets.Count;
		float bucketWidth = (float)this._bucketRange;
		float maxScore = bucketWidth * (float)bucketCount;
		int columnCount = this._columns.Count;
		float columnWidth = maxScore / (float)columnCount;
		float maxColumnHeight = 0f;
		for (int columnIndex = 0; columnIndex < this._columns.Count; columnIndex++)
		{
			float columnMinScore = (float)columnIndex * columnWidth;
			float columnMaxScore = (float)(columnIndex + 1) * columnWidth;
			int bucketStartIndex = Mathf.FloorToInt(columnMinScore / bucketWidth);
			float bucketStartFactor = columnMinScore / bucketWidth - (float)bucketStartIndex;
			int bucketEndIndex = Mathf.FloorToInt(columnMaxScore / bucketWidth);
			float bucketEndFactor = columnMaxScore / bucketWidth - (float)bucketEndIndex;
			if (bucketEndIndex >= bucketCount)
			{
				bucketEndIndex = bucketCount - 1;
				bucketEndFactor = 1f;
			}
			float entryCount = (float)this._buckets[bucketStartIndex] * (1f - bucketStartFactor);
			for (int rawBarCursor = bucketStartIndex + 1; rawBarCursor <= bucketEndIndex; rawBarCursor++)
			{
				entryCount += (float)this._buckets[rawBarCursor];
			}
			entryCount -= (float)this._buckets[bucketEndIndex] * (1f - bucketEndFactor);
			this._columns[columnIndex].Initialise(columnMinScore, columnMaxScore, entryCount, columnIndex % 2 == 1);
			this._columns[columnIndex].RectTransform.sizeDelta = new Vector2(0f, maxHeight);
			maxColumnHeight = Mathf.Max(entryCount, maxColumnHeight);
		}
		for (int columnIndex2 = 0; columnIndex2 < this._columns.Count; columnIndex2++)
		{
			HistogramColumn histogramColumn = this._columns[columnIndex2];
			float columnHeightFactor = histogramColumn.NumberOfEntries / maxColumnHeight;
			histogramColumn.SetHeight(columnHeightFactor * maxHeight, Mathf.Lerp(this._minColumnTweenDuration, this._maxColumnTweenDuration, columnHeightFactor), this._columnTweenDelay * (float)columnIndex2, this._columnTweenEasingType);
		}
		this._columnParent.GetComponent<HorizontalLayoutGroup>().enabled = false;
		this._columnParent.GetComponent<HorizontalLayoutGroup>().enabled = true;
		int scoreRangePerNotch = this.CalculateNotchIncrement(maxScore);
		for (int notchIndex = 0; notchIndex < this._indicatorNotchTexts.Count; notchIndex++)
		{
			this._indicatorNotchTexts[notchIndex].text = string.Format("{0}", notchIndex * scoreRangePerNotch);
		}
		float axisScale = (float)(scoreRangePerNotch * (this._indicatorNotches.Count - 1)) / maxScore;
		float notchWidth = this._indicatorNotches[0].rect.width;
		float totalAxisWidth = this._indicatorNotchRect.rect.width * axisScale;
		float availableWidth = totalAxisWidth - notchWidth * (float)this._indicatorNotches.Count;
		this._indicatorNotchLayoutGroup.spacing = availableWidth / (float)(this._indicatorNotches.Count - 1);
		foreach (RectTransform rectTransform in this._indicatorNotches)
		{
			rectTransform.gameObject.SetActive(true);
		}
		float halfZeroWidth = this._indicatorNotchTexts[0].GetPreferredValues().x * 0.5f;
		float finalNotchCentre = totalAxisWidth - notchWidth * 0.5f;
		Histogram.ConstrainCenteredText(this._indicatorNotchTexts[this._indicatorNotchTexts.Count - 1], finalNotchCentre, this._columnParent.rect.width + halfZeroWidth - notchWidth * 0.5f);
		this._requestHandle = this._leaderboardService.RequestLocalEntry(leaderboardId, new LocalEntryRequestCompleted(this.LocalEntryRequestCompleted));
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x00024BE4 File Offset: 0x00022DE4
	private int CalculateNotchIncrement(float maxScore)
	{
		int notchIncrement = 0;
		foreach (int maxNotchMultiple in Histogram.MaxNotchMultiples)
		{
			int maxNotchScore = Mathf.FloorToInt(maxScore / (float)maxNotchMultiple) * maxNotchMultiple;
			notchIncrement = maxNotchScore / (this._indicatorNotches.Count - 1);
			if ((float)notchIncrement > maxScore - (float)maxNotchScore)
			{
				break;
			}
		}
		return notchIncrement;
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x00024C34 File Offset: 0x00022E34
	private static void ConstrainCenteredText(TMP_Text text, float center, float maxHorizontalConstraint)
	{
		float constraint = 0f;
		float halfTextWidth = text.GetPreferredValues().x * 0.5f;
		if (halfTextWidth > center)
		{
			constraint = halfTextWidth - center;
		}
		else if (halfTextWidth > maxHorizontalConstraint - center)
		{
			constraint = maxHorizontalConstraint - center - halfTextWidth;
		}
		RectTransform textTransform = text.gameObject.GetComponent<RectTransform>();
		textTransform.anchoredPosition = new Vector2(constraint, textTransform.anchoredPosition.y);
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x00024C92 File Offset: 0x00022E92
	private void OnDisable()
	{
		AsyncRequestHandle requestHandle = this._requestHandle;
		if (requestHandle == null)
		{
			return;
		}
		requestHandle.Cancel();
	}

	// Token: 0x040005F8 RID: 1528
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("HistogramPanel");

	// Token: 0x040005F9 RID: 1529
	[SerializeField]
	private RectTransform _columnParent;

	// Token: 0x040005FA RID: 1530
	[SerializeField]
	private List<HistogramColumn> _columns = new List<HistogramColumn>();

	// Token: 0x040005FB RID: 1531
	[SerializeField]
	private RectTransform _indicatorNotchRect;

	// Token: 0x040005FC RID: 1532
	[SerializeField]
	private HorizontalLayoutGroup _indicatorNotchLayoutGroup;

	// Token: 0x040005FD RID: 1533
	[SerializeField]
	private LocalizedTextUI _cityText;

	// Token: 0x040005FE RID: 1534
	[SerializeField]
	private LocalizedTextUI _highScoreText;

	// Token: 0x040005FF RID: 1535
	[SerializeField]
	private LocalizedTextUI _youText;

	// Token: 0x04000600 RID: 1536
	[SerializeField]
	private RectTransform _youBar;

	// Token: 0x04000601 RID: 1537
	[SerializeField]
	private List<RectTransform> _indicatorNotches = new List<RectTransform>();

	// Token: 0x04000602 RID: 1538
	[SerializeField]
	private List<TextMeshProUGUI> _indicatorNotchTexts = new List<TextMeshProUGUI>();

	// Token: 0x04000603 RID: 1539
	[SerializeField]
	[Tooltip("How large a bucket at the end of the histogram has to be to avoid being pruned, relative to the histogram's biggest bucket.")]
	private float _minBucketSizeRelativeToMax = 0.008f;

	// Token: 0x04000604 RID: 1540
	[SerializeField]
	[Tooltip("How long a column takes to tween in if it has a value of 0.")]
	private float _minColumnTweenDuration = 0.1f;

	// Token: 0x04000605 RID: 1541
	[Tooltip("How long a column takes to tween in if it has the highest value across the graph.")]
	[SerializeField]
	private float _maxColumnTweenDuration = 0.9f;

	// Token: 0x04000606 RID: 1542
	[SerializeField]
	[Tooltip("How long to wait between start each column's animation.")]
	private float _columnTweenDelay = 0.02f;

	// Token: 0x04000607 RID: 1543
	[SerializeField]
	private Easings.Functions _columnTweenEasingType;

	// Token: 0x04000608 RID: 1544
	private IScope _scope;

	// Token: 0x04000609 RID: 1545
	private MapSelectScreen _mapSelectScreen;

	// Token: 0x0400060A RID: 1546
	private LeaderboardService _leaderboardService;

	// Token: 0x0400060B RID: 1547
	private MotorwaysThemeDatabase _themeDatabase;

	// Token: 0x0400060C RID: 1548
	private AsyncRequestHandle _requestHandle;

	// Token: 0x0400060D RID: 1549
	private readonly List<int> _buckets = new List<int>();

	// Token: 0x0400060E RID: 1550
	private int _bucketRange;

	// Token: 0x0400060F RID: 1551
	[SerializeField]
	private LeaderboardPanel _leaderboardPanel;

	// Token: 0x04000610 RID: 1552
	[Dependency]
	private ActivePlayer _player;

	// Token: 0x04000611 RID: 1553
	private MapDatabase _mapDatabase;

	// Token: 0x04000612 RID: 1554
	private static readonly int[] MaxNotchMultiples = new int[]
	{
		500,
		250,
		150,
		50
	};
}
