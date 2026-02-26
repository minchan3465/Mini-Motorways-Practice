using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000250 RID: 592
public static class AssetBundleUtility
{
	// Token: 0x06000E21 RID: 3617 RVA: 0x0002FB38 File Offset: 0x0002DD38
	public static GameObject LoadPrefab(string bundleName, string prefabName)
	{
		return AssetBundleUtility.LoadAsset<GameObject>(bundleName, prefabName);
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x0002FB44 File Offset: 0x0002DD44
	public static AssetType LoadAsset<AssetType>(string bundleName, string assetName) where AssetType : UnityEngine.Object
	{
		AssetBundle bundle = AssetBundleUtility.LoadAssetBundle(bundleName);
		if (bundle != null)
		{
			AssetType asset = bundle.LoadAsset<AssetType>(assetName);
			if (asset != null)
			{
				return asset;
			}
			AssetBundleUtility.DebugMissingAsset(bundle, assetName);
		}
		return default(AssetType);
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x0002FB8C File Offset: 0x0002DD8C
	public static T LoadSubAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object
	{
		AssetBundle bundle = AssetBundleUtility.LoadAssetBundle(bundleName);
		if (bundle != null)
		{
			T[] assets = bundle.LoadAssetWithSubAssets<T>(assetName);
			if (assets != null && assets.Length != 0)
			{
				return assets[0];
			}
			AssetBundleUtility.DebugMissingAsset(bundle, assetName);
		}
		return default(T);
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x0002FBD0 File Offset: 0x0002DDD0
	public static AssetBundleUtility.AsyncLoadResult LoadPrefabAsync(string bundleName, string prefabName, MonoBehaviour owner)
	{
		if (Application.isEditor)
		{
			return new AssetBundleUtility.AsyncLoadResult(bundleName, prefabName)
			{
				asset = AssetBundleUtility.LoadPrefab(bundleName, prefabName)
			};
		}
		AssetBundleUtility.AsyncLoadResult result = new AssetBundleUtility.AsyncLoadResult(bundleName, prefabName);
		owner.StartCoroutine(result.AsyncLoadAsset());
		return result;
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x0002FC10 File Offset: 0x0002DE10
	public static AssetBundleUtility.AsyncLoadResult LoadAssetAsync(string bundleName, string assetName, MonoBehaviour owner)
	{
		AssetBundleUtility.AsyncLoadResult result = new AssetBundleUtility.AsyncLoadResult(bundleName, assetName);
		owner.StartCoroutine(result.AsyncLoadAsset());
		return result;
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x0002FC34 File Offset: 0x0002DE34
	private static AssetBundle LoadAssetBundle(string bundleName)
	{
		AssetBundle bundle;
		if (AssetBundleUtility._assetBundles.TryGetValue(bundleName, out bundle))
		{
			return bundle;
		}
		if (!Application.isEditor && FeatureToggle.IsFeatureEnabled(Feature.LoadRemotePrefabs))
		{
			string url = "https://build.dinopoloclub.com/asset-bundle?name=" + bundleName;
			AssetBundleUtility.Log.Info("Attempting to load bundle '{0}' from {1}.", new object[]
			{
				bundleName,
				url
			});
			using (UnityWebRequest assetBundleRequest = UnityWebRequest.Get(url))
			{
				assetBundleRequest.downloadHandler = new DownloadHandlerAssetBundle(url, 0U);
				assetBundleRequest.SendWebRequest();
				while (!assetBundleRequest.isDone)
				{
					Thread.Sleep(100);
				}
				AssetBundleUtility.Log.Info("Request completed with response code {0}.", new object[]
				{
					assetBundleRequest.responseCode
				});
				if (assetBundleRequest.result == UnityWebRequest.Result.Success)
				{
					bundle = DownloadHandlerAssetBundle.GetContent(assetBundleRequest);
					if (bundle != null)
					{
						AssetBundleUtility.Log.Info("Fetched remote bundle successfully.", Array.Empty<object>());
					}
					else
					{
						AssetBundleUtility.Log.Info("Downloaded completed, but the remote bundle could not be loaded.", Array.Empty<object>());
					}
				}
				else
				{
					AssetBundleUtility.Log.Info("Failed to download remote bundle.\n{0}", new object[]
					{
						assetBundleRequest.error
					});
				}
			}
		}
		if (bundle == null)
		{
			bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "AssetBundles", bundleName));
		}
		AssetBundleUtility._assetBundles[bundleName] = bundle;
		return bundle;
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x0002FD8C File Offset: 0x0002DF8C
	private static void DebugMissingAsset(AssetBundle bundle, string missingAssetName)
	{
		AssetBundleUtility.Log.Error("Unable to find asset named '{0}' in asset bundle '{1}'.", new object[]
		{
			missingAssetName,
			bundle.name
		});
		AssetBundleUtility.Log.Info("The asset bundle contains these assets:", Array.Empty<object>());
		foreach (string assetName in bundle.GetAllAssetNames())
		{
			AssetBundleUtility.Log.Info(" - {0}", new object[]
			{
				assetName
			});
		}
	}

	// Token: 0x04000849 RID: 2121
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("AssetBundleUtility");

	// Token: 0x0400084A RID: 2122
	private static Dictionary<string, AssetBundle> _assetBundles = new Dictionary<string, AssetBundle>();

	// Token: 0x02000251 RID: 593
	public class AsyncLoadResult
	{
		// Token: 0x06000E29 RID: 3625 RVA: 0x0002FE1C File Offset: 0x0002E01C
		public AsyncLoadResult(string bundleName, string assetName)
		{
			this._bundleName = bundleName;
			this._assetName = assetName;
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x06000E2A RID: 3626 RVA: 0x0002FE32 File Offset: 0x0002E032
		public bool HasValue
		{
			get
			{
				return this.asset != null;
			}
		}

		// Token: 0x06000E2B RID: 3627 RVA: 0x0002FE40 File Offset: 0x0002E040
		public IEnumerator AsyncLoadAsset()
		{
			string loadName = Path.Combine(Application.streamingAssetsPath, "AssetBundles", this._bundleName);
			AssetBundleCreateRequest bundleLoadRequest = AssetBundle.LoadFromFileAsync(loadName);
			yield return bundleLoadRequest;
			AssetBundle myLoadedAssetBundle = bundleLoadRequest.assetBundle;
			if (myLoadedAssetBundle == null)
			{
				AssetBundleUtility.Log.Warn("Failed to load AssetBundle {0}/{1}!", new object[]
				{
					this._bundleName,
					this._assetName
				});
				yield break;
			}
			AssetBundleRequest assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync(this._assetName);
			yield return assetLoadRequest;
			this.asset = assetLoadRequest.asset;
			if (this.asset == null)
			{
				AssetBundleUtility.DebugMissingAsset(myLoadedAssetBundle, this._assetName);
			}
			myLoadedAssetBundle.Unload(false);
			yield break;
		}

		// Token: 0x0400084B RID: 2123
		public UnityEngine.Object asset;

		// Token: 0x0400084C RID: 2124
		private string _bundleName;

		// Token: 0x0400084D RID: 2125
		private string _assetName;
	}
}
