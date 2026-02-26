using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x020001E3 RID: 483
public class MultiResolutionVideoPlayer : MonoBehaviour
{
	// Token: 0x06000B85 RID: 2949 RVA: 0x000275F8 File Offset: 0x000257F8
	private void Awake()
	{
		this.videoPlayer = base.gameObject.AddComponent<VideoPlayer>();
		this.videoPlayer.playOnAwake = false;
		this.videoPlayer.isLooping = false;
		this.videoPlayer.targetCamera = this.targetCamera;
		this.videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
		this.videoPlayer.aspectRatio = VideoAspectRatio.FitOutside;
		try
		{
			if (!this.TryLoadBestMatchingVideoClip())
			{
				return;
			}
		}
		catch (Exception e)
		{
			MultiResolutionVideoPlayer.Log.Error(e.Message, Array.Empty<object>());
		}
		this.videoPlayer.Prepare();
		if (Application.isEditor)
		{
			int embeddedClipsFound = 0;
			foreach (MultiResolutionVideoPlayer.VideoClipAspectGroup videoAspectGroup in this.videoCandidates)
			{
				foreach (VideoClipData videoClipData in videoAspectGroup.videoClipData)
				{
					if (videoClipData.clip)
					{
						embeddedClipsFound++;
					}
					else
					{
						File.Exists(this.BuildFilePath(videoAspectGroup, videoClipData));
					}
				}
			}
		}
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x0002770C File Offset: 0x0002590C
	private bool TryLoadBestMatchingVideoClip()
	{
		if (this.videoCandidates.Length == 0)
		{
			MultiResolutionVideoPlayer.Log.Error("No video candidates found.", Array.Empty<object>());
			return false;
		}
		float screenAspectRatio = this.videoPlayer.targetCamera.aspect;
		int closestAspectIndex = 0;
		float closestAspectRatio = this.videoCandidates[closestAspectIndex].Aspect;
		for (int candidateIndex = 1; candidateIndex < this.videoCandidates.Length; candidateIndex++)
		{
			float aspect = this.videoCandidates[candidateIndex].Aspect;
			if (Mathf.Abs(aspect - screenAspectRatio) < Math.Abs(closestAspectRatio - screenAspectRatio))
			{
				closestAspectIndex = candidateIndex;
				closestAspectRatio = aspect;
			}
		}
		MultiResolutionVideoPlayer.VideoClipAspectGroup closestAspectGroup = this.videoCandidates[closestAspectIndex];
		MultiResolutionVideoPlayer.Log.Info("Selected aspect ratio: {0}x{1}", new object[]
		{
			closestAspectGroup.size.x,
			closestAspectGroup.size.y
		});
		if (closestAspectGroup.videoClipData.Length == 0 || string.IsNullOrEmpty(closestAspectGroup.videoClipData[0].ClipName))
		{
			MultiResolutionVideoPlayer.Log.Error("No video clips in aspect group {0}x{1}", new object[]
			{
				closestAspectGroup.size.x,
				closestAspectGroup.size.y
			});
			return false;
		}
		int bestResolutionIndex = 0;
		uint bestResolution = closestAspectGroup.videoClipData[bestResolutionIndex].Width * closestAspectGroup.videoClipData[bestResolutionIndex].Height;
		int screenResolution = this.targetCamera.pixelWidth * this.targetCamera.pixelHeight;
		for (int aspectGroupIndex = 1; aspectGroupIndex < closestAspectGroup.videoClipData.Length; aspectGroupIndex++)
		{
			uint resolution = closestAspectGroup.videoClipData[aspectGroupIndex].Width * closestAspectGroup.videoClipData[aspectGroupIndex].Height;
			if ((ulong)resolution < (ulong)((long)screenResolution) && (Mathf.Abs((float)((ulong)resolution - (ulong)((long)screenResolution))) < (float)Math.Abs((long)((ulong)bestResolution - (ulong)((long)screenResolution))) || (ulong)bestResolution > (ulong)((long)screenResolution)))
			{
				bestResolutionIndex = aspectGroupIndex;
				bestResolution = resolution;
			}
		}
		VideoClipData bestClip = closestAspectGroup.videoClipData[bestResolutionIndex];
		MultiResolutionVideoPlayer.Log.Info("Selected resolution: {0}x{1}", new object[]
		{
			bestClip.Width,
			bestClip.Height
		});
		if (bestClip.clip)
		{
			MultiResolutionVideoPlayer.Log.Info("Playing from embedded clip.", new object[]
			{
				bestClip.Width,
				bestClip.Height
			});
			this.videoPlayer.source = VideoSource.VideoClip;
			this.videoPlayer.clip = bestClip.clip;
		}
		else
		{
			string filePath = this.BuildFilePath(closestAspectGroup, bestClip);
			MultiResolutionVideoPlayer.Log.Info("Playing from " + filePath, Array.Empty<object>());
			this.videoPlayer.source = VideoSource.Url;
			this.videoPlayer.url = filePath;
		}
		return true;
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x000279DF File Offset: 0x00025BDF
	private string BuildFilePath(MultiResolutionVideoPlayer.VideoClipAspectGroup aspectGroup, VideoClipData bestClip)
	{
		return string.Concat(new string[]
		{
			Application.streamingAssetsPath,
			"/AppleArcadeSplashVideos/",
			aspectGroup.folderName,
			"/",
			bestClip.ClipName,
			".mp4"
		});
	}

	// Token: 0x040006A5 RID: 1701
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("MultiResolutionVideoPlayer");

	// Token: 0x040006A6 RID: 1702
	public Camera targetCamera;

	// Token: 0x040006A7 RID: 1703
	[HideInInspector]
	public VideoPlayer videoPlayer;

	// Token: 0x040006A8 RID: 1704
	[SerializeField]
	public MultiResolutionVideoPlayer.VideoClipAspectGroup[] videoCandidates;

	// Token: 0x020001E4 RID: 484
	[Serializable]
	public struct VideoClipAspectGroup
	{
		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000B8A RID: 2954 RVA: 0x00027A2F File Offset: 0x00025C2F
		public float Aspect
		{
			get
			{
				return this.size.x / this.size.y;
			}
		}

		// Token: 0x040006A9 RID: 1705
		public Vector2 size;

		// Token: 0x040006AA RID: 1706
		public VideoClipData[] videoClipData;

		// Token: 0x040006AB RID: 1707
		public string folderName;
	}
}
