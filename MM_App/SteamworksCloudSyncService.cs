using System;
using System.Collections;
using Factory;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020000D8 RID: 216
public class SteamworksCloudSyncService : ISteamCloudSyncService
{
	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x0600046F RID: 1135 RVA: 0x0000F6A4 File Offset: 0x0000D8A4
	public bool IsSupported
	{
		get
		{
			return FeatureToggle.IsFeatureEnabled(Feature.SteamCrossSave);
		}
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x0000F6B4 File Offset: 0x0000D8B4
	public AsyncRequestHandle Authenticate(SteamCloudAuthenticationCompleted authenticationCompleted)
	{
		Guid authenticationGuid = Guid.NewGuid();
		string url = string.Format("https://steamcommunity.com/oauth/login?response_type=token&client_id={0}&state={1}&mobileminimal=1", "F2FBD1F7", authenticationGuid);
		string callbackUrl = "https://api.dinopoloclub.com/1/minimotorways/steam/authorized/";
		AsyncRequestHandle requestHandle = new AsyncRequestHandle();
		SteamworksCloudSyncService.Log.Info("Opening Steam OAuth page for the player at {0}.", new object[]
		{
			url
		});
		this._oauthClient.RequestAuthorization(url, callbackUrl, delegate(OAuthAuthorizationResult authorizationResult)
		{
			if (!requestHandle.IsActive)
			{
				return;
			}
			switch (authorizationResult)
			{
			case OAuthAuthorizationResult.Unavailable:
				authenticationCompleted(null, SteamCloudSyncError.NotSupported);
				return;
			case OAuthAuthorizationResult.Denied:
				authenticationCompleted(null, SteamCloudSyncError.AuthorizationDenied);
				return;
			case OAuthAuthorizationResult.NoConnection:
				authenticationCompleted(null, SteamCloudSyncError.NoConnection);
				return;
			default:
				this.StartCoroutine(this.FetchAccessTokenCoroutine(requestHandle, authenticationGuid, authenticationCompleted));
				return;
			}
		});
		return requestHandle;
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x0000F744 File Offset: 0x0000D944
	public AsyncRequestHandle DownloadProfiles(string accessToken, SteamCloudProfileDownloadCompleted downloadCompleted)
	{
		AsyncRequestHandle requestHandle = new AsyncRequestHandle();
		this.StartCoroutine(this.DownloadProfilesCoroutine(requestHandle, accessToken, downloadCompleted));
		return requestHandle;
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x0000F767 File Offset: 0x0000D967
	private IEnumerator FetchAccessTokenCoroutine(AsyncRequestHandle requestHandle, Guid guid, SteamCloudAuthenticationCompleted authenticationCompleted)
	{
		while (requestHandle.IsActive)
		{
			string url = string.Format("https://api.dinopoloclub.com/1/minimotorways/steam/token/{0}/", guid);
			SteamworksCloudSyncService.Log.Info("Looking up access token at {0}.", new object[]
			{
				url
			});
			UnityWebRequest accessTokenRequest = UnityWebRequest.Get(url);
			yield return accessTokenRequest.SendWebRequest();
			if (!requestHandle.IsActive)
			{
				yield break;
			}
			if (accessTokenRequest.result == UnityWebRequest.Result.Success)
			{
				SteamworksCloudSyncService.Log.Info("Access token request returned:\n{0}", new object[]
				{
					accessTokenRequest.downloadHandler.text
				});
				JSON.Dictionary jsonResponse = JSON.LoadFromString(accessTokenRequest.downloadHandler.text) as JSON.Dictionary;
				if (jsonResponse != null)
				{
					if (jsonResponse.GetString("result") == "ok")
					{
						string accessToken = jsonResponse.GetString("accessToken");
						string steamId = jsonResponse.GetString("steamId");
						if (!string.IsNullOrEmpty(jsonResponse.GetString("error")))
						{
							authenticationCompleted(null, SteamCloudSyncError.AuthorizationDenied);
							yield break;
						}
						if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(steamId))
						{
							authenticationCompleted(accessToken, SteamCloudSyncError.None);
							yield break;
						}
					}
				}
				else
				{
					SteamworksCloudSyncService.Log.Warn("Failed to parse response as JSON.", Array.Empty<object>());
				}
			}
			else
			{
				SteamworksCloudSyncService.Log.Warn("Access token request error: {0}.", new object[]
				{
					accessTokenRequest.error
				});
			}
			yield return new WaitForSeconds(3f);
			accessTokenRequest = null;
		}
		yield break;
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x0000F784 File Offset: 0x0000D984
	private IEnumerator DownloadProfilesCoroutine(AsyncRequestHandle requestHandle, string accessToken, SteamCloudProfileDownloadCompleted downloadCompleted)
	{
		SteamCloudSyncError error = SteamCloudSyncError.None;
		ILegacyUserProfile steamUserProfile = null;
		IExtendedUserProfile steamExtendedUserProfile = null;
		string url = "https://api.steampowered.com/ICloudService/EnumerateUserFiles/v1/?access_token=" + accessToken + "&appid=1127500&extended_details=1";
		SteamworksCloudSyncService.Log.Info("Querying Steam Cloud files for the player from {0}.", new object[]
		{
			url
		});
		UnityWebRequest cloudFileEnumerationRequest = UnityWebRequest.Get(url);
		yield return cloudFileEnumerationRequest.SendWebRequest();
		if (!requestHandle.IsActive)
		{
			yield break;
		}
		if (cloudFileEnumerationRequest.result == UnityWebRequest.Result.Success)
		{
			SteamworksCloudSyncService.Log.Info("Cloud file query returned:\n{0}", new object[]
			{
				cloudFileEnumerationRequest.downloadHandler.text
			});
			JSON.Dictionary jsonResult = JSON.LoadFromString(cloudFileEnumerationRequest.downloadHandler.text) as JSON.Dictionary;
			if (jsonResult != null)
			{
				JSON.Dictionary jsonResponse = jsonResult.GetDictionary("response");
				if (jsonResponse != null)
				{
					JSON.Array jsonFiles = jsonResponse.GetArray("files");
					if (jsonFiles != null && jsonFiles.Count > 0)
					{
						int num;
						for (int fileIndex = 0; fileIndex < jsonFiles.Count; fileIndex = num)
						{
							JSON.Dictionary jsonFile = jsonFiles.GetDictionary(fileIndex);
							if (jsonFile != null)
							{
								string filename = jsonFile.GetString("filename");
								string fileUrl = jsonFile.GetString("url");
								if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(fileUrl))
								{
									SteamworksCloudSyncService.Log.Warn("Skipping unexpected 'files' entry with no filename or url.", Array.Empty<object>());
								}
								else
								{
									string text;
									string text2;
									IStorableTypeHandler storableTypeHandler = this._storableTypeHandlerRegistry.GetHandlerForFilename(filename, out text, out text2);
									if (storableTypeHandler is UserProfileStorableTypeHandler || storableTypeHandler is ExtendedUserProfileStorableTypeHandler)
									{
										SteamworksCloudSyncService.Log.Info("Attempting to download cloud file {0}.", new object[]
										{
											filename
										});
										UnityWebRequest fileDownloadRequest = UnityWebRequest.Get(fileUrl);
										yield return fileDownloadRequest.SendWebRequest();
										if (!requestHandle.IsActive)
										{
											yield break;
										}
										if (fileDownloadRequest.result == UnityWebRequest.Result.Success)
										{
											IStorable storable = storableTypeHandler.Load(fileDownloadRequest.downloadHandler.data);
											ILegacyUserProfile newUserProfile = storable as ILegacyUserProfile;
											if (newUserProfile == null)
											{
												IExtendedUserProfile newExtendedUserProfile = storable as IExtendedUserProfile;
												if (newExtendedUserProfile == null)
												{
													error = SteamCloudSyncError.InvalidData;
													SteamworksCloudSyncService.Log.Warn("Skipping unknown storable {0}.", new object[]
													{
														storable
													});
													if (storable != null)
													{
														this._scope.Release(storable);
													}
												}
												else
												{
													SteamworksCloudSyncService.Log.Info("Downloaded {0} as an extended user profile.", new object[]
													{
														filename
													});
													if (steamExtendedUserProfile == null)
													{
														steamExtendedUserProfile = newExtendedUserProfile;
													}
													else
													{
														steamExtendedUserProfile.Merge(newExtendedUserProfile, false);
														this._scope.Release(newExtendedUserProfile);
													}
												}
											}
											else
											{
												SteamworksCloudSyncService.Log.Info("Downloaded {0} as a legacy user profile.", new object[]
												{
													filename
												});
												if (steamUserProfile == null)
												{
													steamUserProfile = newUserProfile;
												}
												else
												{
													steamUserProfile.Merge(newUserProfile, false);
													this._scope.Release(newUserProfile);
												}
											}
										}
										else
										{
											error = SteamCloudSyncError.InvalidData;
											SteamworksCloudSyncService.Log.Warn("Failed to download file! {0}.", new object[]
											{
												fileDownloadRequest.error
											});
										}
										fileDownloadRequest = null;
									}
									else
									{
										SteamworksCloudSyncService.Log.Info("Skipping file {0} because it is either unknown or can't be synced.", new object[]
										{
											filename
										});
									}
									filename = null;
									storableTypeHandler = null;
								}
							}
							num = fileIndex + 1;
						}
					}
					else
					{
						SteamworksCloudSyncService.Log.Info("No relevant files were found.", Array.Empty<object>());
					}
					jsonFiles = null;
				}
				else
				{
					SteamworksCloudSyncService.Log.Warn("Didn't find expected response.", Array.Empty<object>());
					error = SteamCloudSyncError.InvalidResponse;
				}
			}
			else
			{
				SteamworksCloudSyncService.Log.Warn("Unable to parse result as JSON.", Array.Empty<object>());
				error = SteamCloudSyncError.InvalidResponse;
			}
		}
		else
		{
			SteamworksCloudSyncService.Log.Warn("File enumeration request error: {0}.", new object[]
			{
				cloudFileEnumerationRequest.error
			});
			error = SteamCloudSyncError.InvalidResponse;
		}
		if (error == SteamCloudSyncError.InvalidData && (steamUserProfile != null || steamExtendedUserProfile != null))
		{
			error = SteamCloudSyncError.None;
		}
		downloadCompleted(steamUserProfile, steamExtendedUserProfile, error);
		yield break;
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x0000F7A8 File Offset: 0x0000D9A8
	private void StartCoroutine(IEnumerator routine)
	{
		if (this._coroutineHost == null)
		{
			GameObject coroutineHostObject = new GameObject();
			this._coroutineHost = coroutineHostObject.AddComponent<SteamworksCloudSyncService.CoroutineHost>();
		}
		this._coroutineHost.StartCoroutine(routine);
	}

	// Token: 0x040001B2 RID: 434
	[Dependency]
	private IOAuthClient _oauthClient;

	// Token: 0x040001B3 RID: 435
	[Dependency]
	private StorableTypeHandlerRegistry _storableTypeHandlerRegistry;

	// Token: 0x040001B4 RID: 436
	[Dependency]
	private IScope _scope;

	// Token: 0x040001B5 RID: 437
	private SteamworksCloudSyncService.CoroutineHost _coroutineHost;

	// Token: 0x040001B6 RID: 438
	private const float AccessTokenFetchPeriod = 3f;

	// Token: 0x040001B7 RID: 439
	private const string ClientId = "F2FBD1F7";

	// Token: 0x040001B8 RID: 440
	private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("SteamworksCloudSync");

	// Token: 0x020000D9 RID: 217
	public class CoroutineHost : MonoBehaviour
	{
	}
}
