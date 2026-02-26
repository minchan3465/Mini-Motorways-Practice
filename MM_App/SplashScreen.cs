using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

// Token: 0x02000246 RID: 582
public class SplashScreen : MonoBehaviour
{
	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x06000DEE RID: 3566 RVA: 0x0002ED96 File Offset: 0x0002CF96
	private static bool IsFirstRun
	{
		get
		{
			return PlayerPrefs.GetInt("IsFirstRun", 0) != 1;
		}
	}

	// Token: 0x170002E9 RID: 745
	// (set) Token: 0x06000DEF RID: 3567 RVA: 0x0002EDA9 File Offset: 0x0002CFA9
	private Color FadeImageColor
	{
		set
		{
			if (this._fadeImage != null)
			{
				this._fadeImage.color = value;
			}
		}
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x0002EDC5 File Offset: 0x0002CFC5
	private static void UpdateFirstRunFlag()
	{
		if (SplashScreen.IsFirstRun)
		{
			PlayerPrefs.SetInt("IsFirstRun", 1);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x0002EDE0 File Offset: 0x0002CFE0
	private void Awake()
	{
		string localizedString;
		TMP_FontAsset fontAsset;
		if (this._showDemoDisclaimer && this._localizer.GetLocalization(StringId.AppleDemo_SplashScreenNotice, out localizedString, out fontAsset))
		{
			localizedString = localizedString.Replace("{Name}", "Mini Motorways");
			this._displayText.font = fontAsset;
			this._displayText.text = localizedString;
		}
		if (this._textCanvasGroup != null)
		{
			this._textCanvasGroup.alpha = 0f;
		}
		if (Screen.fullScreen)
		{
			Vector2Int targetResolution = Vector2Int.zero;
			if (!DesktopHardwareCapabilities.HasHighPowerGpu && !PlayerPrefs.HasKey("HasEnforcedMaxResolution"))
			{
				targetResolution = new Vector2Int(1920, 1080);
			}
			if (DesktopHardwareCapabilities.SafeAreaHeight > 0 && !PlayerPrefs.HasKey("HasAdjustedResolutionForSafeArea"))
			{
				targetResolution = DesktopHardwareCapabilities.SafeAreaDimensions;
			}
			if (targetResolution.x > 0 && targetResolution.y > 0)
			{
				Vector2Int bestResolution = DesktopHardwareCapabilities.GetClosestResolution(targetResolution);
				if (bestResolution.x > 0 && bestResolution.y > 0)
				{
					Debug.LogFormat("Changing resolution to {0}x{1}.", new object[]
					{
						bestResolution.x,
						bestResolution.y
					});
					Screen.SetResolution(bestResolution.x, bestResolution.y, Screen.fullScreen);
				}
			}
			PlayerPrefs.SetInt("HasAdjustedResolutionForSafeArea", 1);
		}
		string missingLibraryFilename;
		if (!DllUtilities.AreLibrariesLoaded(out missingLibraryFilename))
		{
			this.ShowLibraryNotLoadedPopup(this._canvas, missingLibraryFilename);
			base.enabled = false;
			return;
		}
		this.FadeImageColor = Color.clear;
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x0002EF4C File Offset: 0x0002D14C
	private void ShowLibraryNotLoadedPopup(Canvas canvas, string missingLibraryFilename)
	{
		Debug.LogFormat("Mini Motorways cannot launch because it cannot load the file: {0}.", new object[]
		{
			missingLibraryFilename
		});
		if (canvas == null)
		{
			return;
		}
		canvas.gameObject.AddComponent<GraphicRaycaster>();
		GameObject gameObject = new GameObject("EventSystem");
		gameObject.transform.parent = null;
		gameObject.AddComponent<EventSystem>();
		gameObject.AddComponent<StandaloneInputModule>();
		GameObject loadFailurePopup = UnityEngine.Object.Instantiate<GameObject>(AssetBundleUtility.LoadAsset<GameObject>("core", "CouldNotLoadLibrariesPopup"), canvas.transform);
		if (loadFailurePopup == null)
		{
			return;
		}
		loadFailurePopup.GetComponent<CouldNotLoadLibrariesPopup>().SetMissingLibraryFilename(missingLibraryFilename);
		RectTransform component = loadFailurePopup.GetComponent<RectTransform>();
		component.offsetMin = new Vector2(0f, 0f);
		component.offsetMax = new Vector2(0f, 0f);
		LayoutRebuilder.ForceRebuildLayoutImmediate(canvas.GetComponent<RectTransform>());
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x0002F014 File Offset: 0x0002D214
	private void Start()
	{
		if (this.splashScreenVideo != null && this.splashScreenVideo.videoPlayer != null && (this.splashScreenVideo.videoPlayer.clip || !string.IsNullOrEmpty(this.splashScreenVideo.videoPlayer.url)))
		{
			this.splashScreenVideo.videoPlayer.errorReceived += this.OnVideoError;
			if (!this.splashScreenVideo.videoPlayer.isPrepared)
			{
				this.splashScreenVideo.videoPlayer.prepareCompleted += this.OnVideoPrepared;
			}
			else
			{
				this.OnVideoPrepared(this.splashScreenVideo.videoPlayer);
			}
		}
		else
		{
			SplashScreen.Log.Error("Splash screen failed to play video ", Array.Empty<object>());
			this.OnSplashVideoComplete((this.splashScreenVideo != null) ? this.splashScreenVideo.videoPlayer : null);
		}
		this._sceneLoadOperation = SceneManager.LoadSceneAsync(this.sceneStringToLoad, LoadSceneMode.Additive);
		this._sceneLoadOperation.allowSceneActivation = false;
		if (this._showDemoDisclaimer)
		{
			this._splashScreenStage = SplashScreen.SplashScreenStage.WaitForVideoComplete;
			return;
		}
		this._splashScreenStage = SplashScreen.SplashScreenStage.WaitForSceneLoad;
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x0002F140 File Offset: 0x0002D340
	private void Update()
	{
		if (this._splashScreenStage == SplashScreen.SplashScreenStage.WaitForVideoComplete)
		{
			if (!this._hasFinishedSplashVideo)
			{
				return;
			}
			this._fadeTimer = 0f;
			this._splashScreenStage = SplashScreen.SplashScreenStage.TextFadeIn;
		}
		else if (this._splashScreenStage == SplashScreen.SplashScreenStage.TextFadeIn)
		{
			this._fadeTimer += Time.deltaTime;
			this._textCanvasGroup.alpha = this.FadeAnimationFunction(this._fadeTimer / this._textFadeInDurationSeconds);
			if (this._fadeTimer >= this._textFadeInDurationSeconds)
			{
				this._splashScreenStage = SplashScreen.SplashScreenStage.TextHold;
				this._fadeTimer = 0f;
			}
		}
		else if (this._splashScreenStage == SplashScreen.SplashScreenStage.TextHold)
		{
			this._fadeTimer += Time.deltaTime;
			if (this._fadeTimer >= this._textHoldDurationSeconds)
			{
				this._splashScreenStage = SplashScreen.SplashScreenStage.TextFadeOut;
				this._fadeTimer = 0f;
			}
		}
		else if (this._splashScreenStage == SplashScreen.SplashScreenStage.TextFadeOut)
		{
			this._fadeTimer += Time.deltaTime;
			this._textCanvasGroup.alpha = 1f - this.FadeAnimationFunction(this._fadeTimer / this._textFadeOutDurationSeconds);
			if (this._fadeTimer >= this._textFadeOutDurationSeconds)
			{
				this._splashScreenStage = SplashScreen.SplashScreenStage.WaitForSceneLoad;
				this._fadeTimer = 0f;
			}
		}
		else if (this._splashScreenStage == SplashScreen.SplashScreenStage.WaitForSceneLoad)
		{
			if (!SplashScreen.IsFirstRun && Input.anyKey)
			{
				this._fadeTimer = 0f;
				this._splashScreenStage = SplashScreen.SplashScreenStage.SkippedVideoFadeOut;
			}
			else
			{
				if (this._sceneLoadOperation.progress < 0.9f || !this._hasFinishedSplashVideo)
				{
					return;
				}
				this._sceneLoadOperation.allowSceneActivation = true;
				this._splashScreenStage = SplashScreen.SplashScreenStage.HoldOnBlackScreen;
			}
		}
		if (this._splashScreenStage == SplashScreen.SplashScreenStage.SkippedVideoFadeOut)
		{
			if (this._fadeTimer <= this.videoFadeOutDurationSeconds)
			{
				this.FadeImageColor = Color.Lerp(Color.clear, Color.black, this.FadeAnimationFunction(this._fadeTimer / this.videoFadeOutDurationSeconds));
				this._fadeTimer += Time.deltaTime;
				return;
			}
			if (this._sceneLoadOperation.progress < 0.9f)
			{
				return;
			}
			this.OnSplashVideoComplete((this.splashScreenVideo != null) ? this.splashScreenVideo.videoPlayer : null);
			this._sceneLoadOperation.allowSceneActivation = true;
			this._splashScreenStage = SplashScreen.SplashScreenStage.HoldOnBlackScreen;
		}
		if (this._splashScreenStage == SplashScreen.SplashScreenStage.HoldOnBlackScreen)
		{
			if (!this._canStartFade)
			{
				return;
			}
			this._fadeTimer = 0f;
			this._splashScreenStage = ((this.waitFramesBeforeFade <= 0f) ? SplashScreen.SplashScreenStage.Fade : SplashScreen.SplashScreenStage.WaitForFrames);
		}
		if (this._splashScreenStage == SplashScreen.SplashScreenStage.WaitForFrames)
		{
			if ((float)this._waitFrames < this.waitFramesBeforeFade)
			{
				this._waitFrames++;
				return;
			}
			this._splashScreenStage = SplashScreen.SplashScreenStage.Fade;
		}
		if (this._splashScreenStage == SplashScreen.SplashScreenStage.Fade)
		{
			if (this._fadeTimer <= this.gameFadeInDurationSeconds)
			{
				this.FadeImageColor = Color.Lerp(Color.black, Color.clear, this.FadeAnimationFunction(this._fadeTimer / this.gameFadeInDurationSeconds));
				this._fadeTimer += Time.deltaTime;
				return;
			}
			this.FadeImageColor = Color.clear;
			this._isFadeComplete = true;
			this._splashScreenStage = SplashScreen.SplashScreenStage.DestroyGameObject;
		}
		if (this._splashScreenStage == SplashScreen.SplashScreenStage.DestroyGameObject)
		{
			SplashScreen.UpdateFirstRunFlag();
			UnityEngine.Object.Destroy(base.gameObject);
			this._splashScreenStage = SplashScreen.SplashScreenStage.Finished;
		}
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x0002F466 File Offset: 0x0002D666
	private float FadeAnimationFunction(float x)
	{
		return SplashScreen.AppleArcadeFadeInAnimationFunction.Solve(x, UnitBezier.SolveEpsilon(this.gameFadeInDurationSeconds));
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x0002F47E File Offset: 0x0002D67E
	public void StartFade()
	{
		this._canStartFade = true;
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x0002F487 File Offset: 0x0002D687
	public bool IsFadeComplete()
	{
		return this._isFadeComplete;
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x0002F48F File Offset: 0x0002D68F
	private void OnVideoPrepared(VideoPlayer videoPlayer)
	{
		this.splashScreenVideo.videoPlayer.Play();
		this.splashScreenVideo.videoPlayer.loopPointReached += this.OnSplashVideoComplete;
		this.VideoTimeout();
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x0002F4C4 File Offset: 0x0002D6C4
	private Task VideoTimeout()
	{
		SplashScreen.<VideoTimeout>d__43 <VideoTimeout>d__;
		<VideoTimeout>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<VideoTimeout>d__.<>4__this = this;
		<VideoTimeout>d__.<>1__state = -1;
		<VideoTimeout>d__.<>t__builder.Start<SplashScreen.<VideoTimeout>d__43>(ref <VideoTimeout>d__);
		return <VideoTimeout>d__.<>t__builder.Task;
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x0002F507 File Offset: 0x0002D707
	private void OnVideoError(VideoPlayer source, string message)
	{
		SplashScreen.Log.Error("Splash video error " + message, Array.Empty<object>());
		this.OnSplashVideoComplete(source);
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x0002F52C File Offset: 0x0002D72C
	private void OnSplashVideoComplete(VideoPlayer source)
	{
		if (source != null)
		{
			source.enabled = false;
		}
		if (this._hasFinishedSplashVideo)
		{
			return;
		}
		this._hasFinishedSplashVideo = true;
		this.FadeImageColor = Color.black;
		if (this.splashScreenVideo == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.splashScreenVideo);
	}

	// Token: 0x04000802 RID: 2050
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("SplashScreen");

	// Token: 0x04000803 RID: 2051
	private const string UILayerName = "UI";

	// Token: 0x04000804 RID: 2052
	private const string HasAdjustedResolutionForSafeAreaKey = "HasAdjustedResolutionForSafeArea";

	// Token: 0x04000805 RID: 2053
	public MultiResolutionVideoPlayer splashScreenVideo;

	// Token: 0x04000806 RID: 2054
	[SerializeField]
	[CanBeNull]
	private Image _fadeImage;

	// Token: 0x04000807 RID: 2055
	public string sceneStringToLoad = "main";

	// Token: 0x04000808 RID: 2056
	public float videoFadeOutDurationSeconds = 0.35f;

	// Token: 0x04000809 RID: 2057
	public float gameFadeInDurationSeconds = 0.35f;

	// Token: 0x0400080A RID: 2058
	private float _fadeTimer;

	// Token: 0x0400080B RID: 2059
	[Tooltip("The number of frames to wait before fading.")]
	public float waitFramesBeforeFade = 1f;

	// Token: 0x0400080C RID: 2060
	private int _waitFrames;

	// Token: 0x0400080D RID: 2061
	[SerializeField]
	private bool _showDemoDisclaimer;

	// Token: 0x0400080E RID: 2062
	[SerializeField]
	private BakedLocalizer _localizer;

	// Token: 0x0400080F RID: 2063
	[SerializeField]
	private Canvas _canvas;

	// Token: 0x04000810 RID: 2064
	[SerializeField]
	private CanvasGroup _textCanvasGroup;

	// Token: 0x04000811 RID: 2065
	[SerializeField]
	private TMP_Text _displayText;

	// Token: 0x04000812 RID: 2066
	[SerializeField]
	private float _textFadeInDurationSeconds = 1.5f;

	// Token: 0x04000813 RID: 2067
	[SerializeField]
	private float _textHoldDurationSeconds = 3f;

	// Token: 0x04000814 RID: 2068
	[SerializeField]
	private float _textFadeOutDurationSeconds = 1.5f;

	// Token: 0x04000815 RID: 2069
	private AsyncOperation _sceneLoadOperation;

	// Token: 0x04000816 RID: 2070
	private SplashScreen.SplashScreenStage _splashScreenStage;

	// Token: 0x04000817 RID: 2071
	private bool _hasFinishedSplashVideo;

	// Token: 0x04000818 RID: 2072
	private bool _canStartFade;

	// Token: 0x04000819 RID: 2073
	private bool _isFadeComplete;

	// Token: 0x0400081A RID: 2074
	private const string IsFirstRunPlayerPrefsKey = "IsFirstRun";

	// Token: 0x0400081B RID: 2075
	private const int OpenedBefore = 1;

	// Token: 0x0400081C RID: 2076
	private const int NotOpenedBefore = 0;

	// Token: 0x0400081D RID: 2077
	private static readonly UnitBezier AppleArcadeFadeInAnimationFunction = new UnitBezier(0f, 0f, 0.6f, 1f);

	// Token: 0x0400081E RID: 2078
	private GameObject _splashScreenCanvas;

	// Token: 0x02000247 RID: 583
	private enum SplashScreenStage
	{
		// Token: 0x04000820 RID: 2080
		LoadScene,
		// Token: 0x04000821 RID: 2081
		WaitForVideoComplete,
		// Token: 0x04000822 RID: 2082
		TextFadeIn,
		// Token: 0x04000823 RID: 2083
		TextHold,
		// Token: 0x04000824 RID: 2084
		TextFadeOut,
		// Token: 0x04000825 RID: 2085
		WaitForSceneLoad,
		// Token: 0x04000826 RID: 2086
		WaitForFrames,
		// Token: 0x04000827 RID: 2087
		SkippedVideoFadeOut,
		// Token: 0x04000828 RID: 2088
		HoldOnBlackScreen,
		// Token: 0x04000829 RID: 2089
		Fade,
		// Token: 0x0400082A RID: 2090
		DestroyGameObject,
		// Token: 0x0400082B RID: 2091
		Finished
	}
}
