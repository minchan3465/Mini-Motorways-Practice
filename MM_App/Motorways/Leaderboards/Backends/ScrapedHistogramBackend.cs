using System;
using System.Collections;
using System.Collections.Generic;
using Factory;
using UnityEngine;
using UnityEngine.Networking;

namespace Motorways.Leaderboards.Backends
{
	// Token: 0x02000783 RID: 1923
	public abstract class ScrapedHistogramBackend : IHistogramBackend, IReleasedFromScopeHandler
	{
		// Token: 0x0600353C RID: 13628 RVA: 0x000F9098 File Offset: 0x000F7298
		public virtual void RequestHistogram(LeaderboardId leaderboardId, HistogramRequestCompleted histogramRequestCompleted)
		{
			if (this._coroutineHost == null)
			{
				GameObject coroutineHostObject = new GameObject();
				this._coroutineHost = coroutineHostObject.AddComponent<ScrapedHistogramBackend.CoroutineHost>();
			}
			this._coroutineHost.StartCoroutine(this.DownloadHistogram(leaderboardId, histogramRequestCompleted));
		}

		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x0600353D RID: 13629
		protected abstract string ServiceId { get; }

		// Token: 0x0600353E RID: 13630 RVA: 0x000F90D9 File Offset: 0x000F72D9
		private IEnumerator DownloadHistogram(LeaderboardId leaderboardId, HistogramRequestCompleted histogramRequestCompleted)
		{
			string histogramId = SteamworksLeaderboardBackend.GetBackendLeaderboardId(leaderboardId);
			string histogramUrl = string.Concat(new string[]
			{
				"https://api.dinopoloclub.com/1/minimotorways/leaderboards/",
				this.ServiceId,
				"/",
				histogramId,
				"/"
			});
			UnityWebRequest headRequest = UnityWebRequest.Head(histogramUrl);
			yield return headRequest.SendWebRequest();
			if (headRequest.result != UnityWebRequest.Result.Success)
			{
				ScrapedHistogramBackend.Log.Warn("Failed to download header data for histogram file at {0}! Aborting!", new object[]
				{
					histogramUrl
				});
				histogramRequestCompleted(null, 0, ScrapedHistogramBackend.UnknownError);
				yield break;
			}
			int contentLength;
			if (!int.TryParse(headRequest.GetResponseHeader("Content-Length"), out contentLength) || contentLength > 20000)
			{
				ScrapedHistogramBackend.Log.Error("Histogram data at {0} too large or header malformed! Is {1} characters. Allowed {2} characters. Aborting!", new object[]
				{
					histogramUrl,
					contentLength,
					20000
				});
				histogramRequestCompleted(null, 0, ScrapedHistogramBackend.NoDataError);
				yield break;
			}
			UnityWebRequest www = UnityWebRequest.Get(histogramUrl);
			yield return www.SendWebRequest();
			if (www.result != UnityWebRequest.Result.Success)
			{
				ScrapedHistogramBackend.Log.Warn("Failed to get histogram data.\n{0}", new object[]
				{
					www.error
				});
				histogramRequestCompleted(null, 0, ScrapedHistogramBackend.UnknownError);
				yield break;
			}
			if (www.downloadHandler.text.Length > 20000)
			{
				ScrapedHistogramBackend.Log.Error("Even though header said data would be {0} long, we've got {1} characters! Aborting!", new object[]
				{
					contentLength,
					www.downloadHandler.text.Length
				});
				histogramRequestCompleted(null, 0, ScrapedHistogramBackend.NoDataError);
				yield break;
			}
			List<int> buckets;
			int bucketSize;
			this.LoadHistogramDataFromJson(www.downloadHandler.text, out buckets, out bucketSize);
			if (buckets != null && bucketSize > 0)
			{
				histogramRequestCompleted(buckets, bucketSize, null);
			}
			else
			{
				histogramRequestCompleted(null, 0, ScrapedHistogramBackend.NoDataError);
			}
			yield break;
		}

		// Token: 0x0600353F RID: 13631 RVA: 0x000F90F8 File Offset: 0x000F72F8
		protected void LoadHistogramDataFromJson(string dictionaryString, out List<int> buckets, out int bucketSize)
		{
			buckets = null;
			bucketSize = 0;
			JSON.Dictionary dictionary = JSON.ToDictionary(JSON.LoadFromString(dictionaryString));
			JSON.Dictionary histogramJson = (JSON.Dictionary)((dictionary != null) ? dictionary["histogram"] : null);
			if (histogramJson != null && histogramJson.GetBool("can_be_graphed", true))
			{
				JSON.Array jsonBuckets = histogramJson.GetArray("buckets");
				if (jsonBuckets != null)
				{
					buckets = new List<int>(jsonBuckets.Count);
					for (int bucketIndex = 0; bucketIndex < jsonBuckets.Count; bucketIndex++)
					{
						buckets.Add(jsonBuckets.GetInt(bucketIndex));
					}
				}
				bucketSize = histogramJson.GetInt("bucket_size", 0);
			}
		}

		// Token: 0x06003540 RID: 13632 RVA: 0x000F9187 File Offset: 0x000F7387
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._coroutineHost != null)
			{
				UnityEngine.Object.Destroy(this._coroutineHost);
			}
		}

		// Token: 0x04002D59 RID: 11609
		private ScrapedHistogramBackend.CoroutineHost _coroutineHost;

		// Token: 0x04002D5A RID: 11610
		private static readonly LeaderboardError UnknownError = new LeaderboardError(LeaderboardErrorCode.Unknown, StringId.LeaderboardError_Generic);

		// Token: 0x04002D5B RID: 11611
		private static readonly LeaderboardError NoDataError = new LeaderboardError(LeaderboardErrorCode.NoData, StringId.None);

		// Token: 0x04002D5C RID: 11612
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ScrapedHistogramBackend");

		// Token: 0x02000784 RID: 1924
		public class CoroutineHost : MonoBehaviour
		{
		}
	}
}
