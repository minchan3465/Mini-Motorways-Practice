using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Client;
using Easing;
using Factory;
using FixMath;
using Gif.Components;
using Motorways.Audio;
using Motorways.Models;
using Motorways.Views.Trains;
using Popups;
using Screens;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000559 RID: 1369
	public class MovieScreen : BaseScalingScreen, IGameStartScreen
	{
		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x060024C1 RID: 9409 RVA: 0x00099827 File Offset: 0x00097A27
		// (set) Token: 0x060024C2 RID: 9410 RVA: 0x00099830 File Offset: 0x00097A30
		private MovieScreen.ScreenState State
		{
			get
			{
				return this._state;
			}
			set
			{
				if (this._state != value)
				{
					this._state = value;
					if (this._state == MovieScreen.ScreenState.Recording)
					{
						this._playImage.sprite = this._loadingSprite;
						this._playButtonAnimator.SetBool(MovieScreen.LoadingBool, true);
						return;
					}
					if (this._state == MovieScreen.ScreenState.Playing)
					{
						this._playImage.sprite = this._pauseSprite;
						this._playButtonAnimator.SetBool(MovieScreen.LoadingBool, false);
						return;
					}
					this._playImage.sprite = this._playSprite;
					this._playButtonAnimator.SetBool(MovieScreen.LoadingBool, false);
				}
			}
		}

		// Token: 0x060024C3 RID: 9411 RVA: 0x000998CA File Offset: 0x00097ACA
		public void OnBackPressed()
		{
			if (this._canvasGroup.CanvasGroup.blocksRaycasts)
			{
				this._screenStack.PopOneScreen();
			}
		}

		// Token: 0x060024C4 RID: 9412 RVA: 0x000998E9 File Offset: 0x00097AE9
		public void OnGifCaptureButtonPressed()
		{
			if (this.State == MovieScreen.ScreenState.Idle)
			{
				this.RecordGif();
				return;
			}
			if (this.State == MovieScreen.ScreenState.Playing || this.State == MovieScreen.ScreenState.Paused)
			{
				this.SaveGif();
			}
		}

		// Token: 0x060024C5 RID: 9413 RVA: 0x00099912 File Offset: 0x00097B12
		public void OnPlayButtonPressed()
		{
			if (this.State == MovieScreen.ScreenState.Idle)
			{
				this.RecordGif();
				return;
			}
			if (this.State == MovieScreen.ScreenState.Playing)
			{
				this.State = MovieScreen.ScreenState.Paused;
				return;
			}
			if (this.State == MovieScreen.ScreenState.Paused)
			{
				this.State = MovieScreen.ScreenState.Playing;
			}
		}

		// Token: 0x060024C6 RID: 9414 RVA: 0x00099944 File Offset: 0x00097B44
		private void SaveGif()
		{
			if (!Diagnostics.Verify(!string.IsNullOrEmpty(this._folderString), "Parent folder string isn't set!"))
			{
				this._folderString = "Mini Motorways";
			}
			StringKey key = this._appScope.Get<StringKey>();
			key.InitWithString(this._game.MapDefinition.mapName);
			StandaloneLocString localizedCityName = StandaloneLocString.CreateString(this._appScope, key);
			StringId messageId;
			StringId messageHeaderId;
			this._softwareCapabilities.SaveGif(this._gifStream.ToArray(), localizedCityName.ToString(), this._folderString, out messageId, out messageHeaderId);
			if (messageId != StringId.None)
			{
				this.popupStack.PushPopup<ChallengeInfoPopup>(0f, false).Initialise(this._appScope, messageHeaderId, messageId, null);
			}
		}

		// Token: 0x060024C7 RID: 9415 RVA: 0x000999F0 File Offset: 0x00097BF0
		public override void OnCreatedInScope(IScope scope)
		{
			base.OnCreatedInScope(scope);
			StandaloneLocString localizedFolderString = StandaloneLocString.CreateString(this._appScope, StringId.MiniMotorways);
			this._folderString = localizedFolderString.ToString();
			this._appScope.Release(localizedFolderString);
			this._frames = new Texture2D[this.TotalFrames];
			this._gifImage.enabled = false;
			this._canvas.worldCamera = this._gameCamera.UICamera;
		}

		// Token: 0x060024C8 RID: 9416 RVA: 0x00099A60 File Offset: 0x00097C60
		private void Update()
		{
			if (this.State == MovieScreen.ScreenState.Playing)
			{
				this._currentFrameTime += Time.deltaTime;
				if (this._currentFrameTime >= 0.08f)
				{
					this._currentFrameTime -= 0.08f;
					this._gifImage.texture = this._frames[this._currentFrame];
					this._currentFrame++;
					if (this._currentFrame >= this._frames.Length)
					{
						this._currentFrame = 0;
					}
				}
			}
		}

		// Token: 0x060024C9 RID: 9417 RVA: 0x00099AE4 File Offset: 0x00097CE4
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			if (this._gifCaptureCoroutine != null)
			{
				this._gameCamera.PostProcessingEnabled = true;
				this._canvasGroup.SetBlocksRaycasts(true);
				base.StopCoroutine(this._gifCaptureCoroutine);
				this._gifCaptureCoroutine = null;
			}
			this._canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			base.TransitionOut(inScreen);
			this.ReleaseGame();
			GameContainerScreen activeScreen = this._screenStack.GetActiveScreen<GameContainerScreen>();
			activeScreen.SetGameSuspended(false);
			Game baseGame = activeScreen.GetActiveGame();
			ViewClient viewClient = baseGame.Scope.Get<ViewClient>();
			viewClient.SetAllGameObjectsEnabled(true);
			foreach (DestinationView destinationView in viewClient.GetViews<DestinationView>())
			{
				destinationView.SetPinViewVisible(true);
			}
			foreach (VehicleView vehicleView in viewClient.GetViews<VehicleView>())
			{
				vehicleView.SkipHeadlightResponseTime = false;
			}
			(baseGame as MotorwaysGame).StartAudio();
			this._saveButtonAnchor.SetActive(false);
			AudioSystem.Instance.UpdateVolume(this._player.VolumeSetting);
		}

		// Token: 0x060024CA RID: 9418 RVA: 0x00099C18 File Offset: 0x00097E18
		public override void TransitionOutTick()
		{
			base.TransitionOutTick();
			GameContainerScreen gameContainer = this._screenStack.GetActiveScreen<GameContainerScreen>();
			this.ResizeFrame(gameContainer.GetActiveGame());
			this._canvasGroup.Alpha = Mathf.Clamp01(1f - this.TransitionOutPercentage() * 2f);
			float movementLerp = Easings.CubicEaseInOut(this.TransitionOutPercentage());
			Vector3 position = Vector3.Lerp(this._desiredPosition, this._oldCameraPosition, movementLerp);
			this._gameCamera.SetPosition(position);
		}

		// Token: 0x060024CB RID: 9419 RVA: 0x00099C9A File Offset: 0x00097E9A
		public override void OnTransitionedOut()
		{
			base.OnTransitionedOut();
			MotorwaysGame motorwaysGame = this._screenStack.GetActiveScreen<GameContainerScreen>().GetActiveGame() as MotorwaysGame;
			if (motorwaysGame == null)
			{
				return;
			}
			motorwaysGame.StartAudio();
		}

		// Token: 0x060024CC RID: 9420 RVA: 0x00099CC1 File Offset: 0x00097EC1
		public override void OnTransitionedIn()
		{
			base.OnTransitionedIn();
			this.ResizeFrame(this._game);
			this._appScope.Get<InputState>().BlockGameInput = true;
			this._playerActionController.CancelAllActions();
			this._canvas.renderMode = RenderMode.ScreenSpaceCamera;
		}

		// Token: 0x060024CD RID: 9421 RVA: 0x00099CFD File Offset: 0x00097EFD
		public virtual void PrepareForNewGame(CityDefinition newCity, MapDefinition newMapDefinition, MotorwaysGame game, MapChallenge newMapChallenge = null, bool startPaused = false)
		{
			this._game = game;
			this._newCity = newCity;
			this._newMapDefinition = newMapDefinition;
			this._newMapChallenge = newMapChallenge;
			this.RegisterThemeComponents(this._themeDatabase.GetTheme());
		}

		// Token: 0x060024CE RID: 9422 RVA: 0x00099D30 File Offset: 0x00097F30
		public override void RegisterThemeComponents(ITheme theme)
		{
			base.RegisterThemeComponents(theme);
			if (this._newCity != null)
			{
				List<IThemeComponent> mapAssets = new List<IThemeComponent>();
				this._newCity.GetComponentsInChildren<IThemeComponent>(mapAssets);
				if (mapAssets != null)
				{
					foreach (IThemeComponent themeComponent in mapAssets)
					{
						themeComponent.InitializeTheme(this._themeDatabase);
					}
				}
				if (this.themeComponents == null)
				{
					this.themeComponents = mapAssets;
					return;
				}
				this.themeComponents.AddRange(mapAssets);
			}
		}

		// Token: 0x060024CF RID: 9423 RVA: 0x00099DC8 File Offset: 0x00097FC8
		public override void TransitionInTick()
		{
			base.TransitionInTick();
			float movementLerp = Easings.CubicEaseInOut(this.TransitionInPercentage());
			this._canvasGroup.Alpha = Mathf.Clamp01(this.TransitionInPercentage() * 2f);
			this._gameCamera.OrthographicSize = Mathf.Lerp(this._previousCameraZoom, this._desiredZoom, movementLerp);
			Vector3 position = Vector3.Lerp(this._transitionDetails.spline.inPoint, this._desiredPosition, movementLerp);
			this._gameCamera.SetPosition(position);
			this.ResizeFrame(this._game);
		}

		// Token: 0x060024D0 RID: 9424 RVA: 0x00099E60 File Offset: 0x00098060
		private void ResizeFrame(Game game)
		{
			Rect playableScreenRect = this.GetAreaToCapture(game);
			this._frameRect.sizeDelta = playableScreenRect.size / this._frameRect.lossyScale;
			float width = (this._rectTransform.sizeDelta.x - this._frameRectBorder.rect.width) / 2f + 1f;
			float sideHeight = this._frameRectBorder.rect.height;
			this._frameRectLeft.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			this._frameRectLeft.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sideHeight);
			this._frameRectRight.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			this._frameRectRight.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sideHeight);
			float topHeight = this._rectTransform.sizeDelta.y / 2f - this._frameRectBorder.rect.height / 2f + this._frameRect.localPosition.y;
			this._frameRectTop.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, topHeight);
			float bottomHeight = this._rectTransform.sizeDelta.y / 2f - this._frameRectBorder.rect.height / 2f - this._frameRect.localPosition.y;
			this._frameRectBottom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bottomHeight);
		}

		// Token: 0x060024D1 RID: 9425 RVA: 0x00099FBC File Offset: 0x000981BC
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			this._saveButtonAnchor.SetActive(false);
			this.State = MovieScreen.ScreenState.Idle;
			this.SetProgressBarFill(0f);
			GameContainerScreen baseGameScreen = this._appScope.Get<GameContainerScreen>();
			if (baseGameScreen != null)
			{
				baseGameScreen.SetGameSuspended(true);
				MotorwaysGame motorwaysGame = baseGameScreen.GetActiveGame() as MotorwaysGame;
				motorwaysGame.StopAudio();
				motorwaysGame.Scope.Get<ViewClient>().SetAllGameObjectsEnabled(false);
			}
			if (this._game == null)
			{
				this._game = this._appScope.Get<MotorwaysGame>();
			}
			this._game.SetMapDefinition(this._newMapDefinition);
			this._game.Start(this._newCity, GameMode.Movie, this._newMapChallenge, true);
			AudioSystem.Instance.UpdateVolume(0);
			this._game.SetPaused(true);
			this._game.Tick(0f);
			this._game.Scope.Get<GameBehaviourModel>().CanGameOver = false;
			this._game.Scope.Get<CityPlanModel>().SpawningMode = CityPlanModel.BuildingSpawningMode.None;
			foreach (VehicleView vehicleView in this._game.Scope.Get<ViewClient>().GetViews<VehicleView>())
			{
				vehicleView.SkipHeadlightResponseTime = true;
			}
			this.PrepareForMovieCapture();
			this.SetDesiredCameraParameters();
			base.TransitionIn(outScreen);
			this._oldCameraPosition = this._transitionDetails.spline.inPoint;
		}

		// Token: 0x060024D2 RID: 9426 RVA: 0x0009A138 File Offset: 0x00098338
		public void PrepareForMovieCapture()
		{
			ViewClient _viewClient = this._game.Scope.Get<ViewClient>();
			foreach (DestinationView destinationView in _viewClient.GetViews<DestinationView>())
			{
				destinationView.SetPinViewVisible(false);
			}
			foreach (VehicleView vehicleView in _viewClient.GetViews<VehicleView>())
			{
				vehicleView.IsTrailActive = true;
			}
			foreach (TrainView trainView in _viewClient.GetViews<TrainView>())
			{
				trainView.IsTrailActive = true;
			}
		}

		// Token: 0x060024D3 RID: 9427 RVA: 0x0009A21C File Offset: 0x0009841C
		private void ReleaseGame()
		{
			if (Diagnostics.Verify(this._game != null, "Trying to release a game when we don't have one!"))
			{
				this.UnregisterThemeComponents();
				this._game.StopAudio();
				this._game.ClearPathfinder();
				this._game.Scope.ParentScope.Release(this._game);
				this._game = null;
				UnityEngine.Object.Destroy(this._newCity.gameObject);
			}
		}

		// Token: 0x060024D4 RID: 9428 RVA: 0x0009A290 File Offset: 0x00098490
		private void SetDesiredCameraParameters()
		{
			Rect playableCaptureArea = this.GetAreaToCapture(this._game);
			playableCaptureArea.min = this._gameCamera.DefaultCamera.ScreenToWorldPoint(playableCaptureArea.min);
			playableCaptureArea.max = this._gameCamera.DefaultCamera.ScreenToWorldPoint(playableCaptureArea.max);
			float desiredOrthagraphicHeight = playableCaptureArea.height / 2f * this.CameraScale;
			float desiredOrthagraphicWidth = playableCaptureArea.width / this._gameCamera.DefaultCamera.aspect / 2f * this.CameraScale;
			this._desiredZoom = Mathf.Max(desiredOrthagraphicHeight, desiredOrthagraphicWidth);
			ClockModel gameClock = this._game.Scope.Get<ClockModel>();
			Vector3Fixed gamePosition = this._game.Scope.Get<City>().GetPlayableAreaPositionAtTime(gameClock.ExpansionTime);
			this._desiredPosition = new Vector2((float)gamePosition.x, (float)gamePosition.y);
		}

		// Token: 0x060024D5 RID: 9429 RVA: 0x0009A398 File Offset: 0x00098598
		private void OnDrawGizmosSelected()
		{
			if (this._game != null)
			{
				Rect playableCaptureArea = this.GetAreaToCapture(this._game);
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(playableCaptureArea.center, playableCaptureArea.size);
				playableCaptureArea.min = this._gameCamera.DefaultCamera.ScreenToWorldPoint(playableCaptureArea.min);
				playableCaptureArea.max = this._gameCamera.DefaultCamera.ScreenToWorldPoint(playableCaptureArea.max);
				Gizmos.color = Color.blue;
				Gizmos.DrawWireCube(playableCaptureArea.center, playableCaptureArea.size);
			}
		}

		// Token: 0x060024D6 RID: 9430 RVA: 0x0009A45C File Offset: 0x0009865C
		private Rect GetAreaToCapture(Game game)
		{
			ClockModel gameClock = game.Scope.Get<ClockModel>();
			RectFixed playableAreaRect = game.Scope.Get<City>().GetSimulationPlayableAreaAtTime(gameClock.ExpansionTime, City.PlayableAreaRoundingType.AllowPartialTiles);
			Vector3 bottomLeft = (Vector3)(playableAreaRect.Min * TilemapModel.TileWidth);
			Vector3 topRight = (Vector3)(playableAreaRect.Max * TilemapModel.TileWidth);
			bottomLeft = this._gameCamera.DefaultCamera.WorldToScreenPoint(bottomLeft);
			topRight = this._gameCamera.DefaultCamera.WorldToScreenPoint(topRight);
			return new Rect
			{
				min = bottomLeft + new Vector3(-this.Padding, -this.Padding),
				max = topRight + new Vector3(this.Padding, this.Padding)
			};
		}

		// Token: 0x060024D7 RID: 9431 RVA: 0x0009A534 File Offset: 0x00098734
		public void RecordGif()
		{
			this.State = MovieScreen.ScreenState.Recording;
			this._game.SetPaused(false);
			this._canvasGroup.SetBlocksRaycasts(false);
			MovieScreen.Log.Info("Preparing Gif Capture", Array.Empty<object>());
			this._gifStream = new MemoryStream();
			this._gifEncoder = new AnimatedGifEncoder();
			this._gifEncoder.Start(this._gifStream);
			this._gifEncoder.SetFrameRate((float)this.FrameRate);
			this._gifEncoder.SetRepeat(0);
			Rect captureAreaRect = this.GetAreaToCapture(this._game);
			this._gifRenderTarget = RenderTexture.GetTemporary((int)captureAreaRect.width, (int)captureAreaRect.height);
			this._gifCaptureCoroutine = this.RecordFrames(captureAreaRect, this.TotalFrames);
			base.StartCoroutine(this._gifCaptureCoroutine);
		}

		// Token: 0x060024D8 RID: 9432 RVA: 0x0009A604 File Offset: 0x00098804
		private void CaptureFrame(Rect rect)
		{
			if (Diagnostics.Verify(this._currentFrame < this._frames.Length, "Capturing more frames than expected! Have {0} but currently trying to get frame {1}", this._frames.Length, this._currentFrame))
			{
				this._frames[this._currentFrame] = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
				this._frames[this._currentFrame].ReadPixels(rect, 0, 0);
				this._frames[this._currentFrame].Apply();
				this._gifEncoder.AddFrame(this._frames[this._currentFrame]);
			}
			this._currentFrame++;
		}

		// Token: 0x060024D9 RID: 9433 RVA: 0x0009A6B7 File Offset: 0x000988B7
		private IEnumerator RecordFrames(Rect rect, int totalFrames)
		{
			this._gameCamera.PostProcessingEnabled = false;
			for (float accumulatedTime = 0f; accumulatedTime < this.WarmUpDuration; accumulatedTime += 0.05f)
			{
				yield return new WaitForSecondsRealtime(0.05f);
				this._game.Tick(0.05f * this.SimulationSpeed);
				this.SetProgressBarFill(0.2f * (accumulatedTime / this.WarmUpDuration));
			}
			int num;
			for (int frameIndex = 0; frameIndex < totalFrames; frameIndex = num + 1)
			{
				MovieScreen.Log.Info("Capturing gif frame {0} of {1}", new object[]
				{
					frameIndex,
					totalFrames
				});
				this._game.Tick(0.05f * this.SimulationSpeed);
				yield return new WaitForEndOfFrame();
				this.CaptureFrame(rect);
				this.SetProgressBarFill(0.2f + 0.8f * ((float)frameIndex / (float)totalFrames));
				num = frameIndex;
			}
			this.SetProgressBarFill(1f);
			this._gifEncoder.Finish();
			this._gifEncoder = null;
			MovieScreen.Log.Info("Gif Capture complete!", Array.Empty<object>());
			RenderTexture.ReleaseTemporary(this._gifRenderTarget);
			this._gameCamera.PostProcessingEnabled = true;
			this._canvasGroup.SetBlocksRaycasts(true);
			this.State = MovieScreen.ScreenState.Playing;
			this._currentFrame = 0;
			this._gifImage.enabled = true;
			this._gifImage.texture = this._frames[this._currentFrame];
			this._saveButtonAnchor.SetActive(true);
			this._gifCaptureCoroutine = null;
			yield break;
		}

		// Token: 0x060024DA RID: 9434 RVA: 0x0009A6D4 File Offset: 0x000988D4
		private void SetProgressBarFill(float progress)
		{
			float parentWidth = this._progressbarFill.parent.GetComponent<RectTransform>().sizeDelta.x;
			this._progressbarFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentWidth * progress);
		}

		// Token: 0x060024DB RID: 9435 RVA: 0x0009A70C File Offset: 0x0009890C
		private void DestroyFrames()
		{
			for (int frameIndex = 0; frameIndex < this.TotalFrames; frameIndex++)
			{
				if (this._frames[frameIndex] != null)
				{
					UnityEngine.Object.Destroy(this._frames[frameIndex]);
					this._frames[frameIndex] = null;
				}
			}
		}

		// Token: 0x060024DC RID: 9436 RVA: 0x0009A750 File Offset: 0x00098950
		public override void Reset()
		{
			base.Reset();
			this._desiredZoom = 0f;
			this._desiredPosition = default(Vector2);
			this._oldCameraPosition = default(Vector2);
			this.DestroyFrames();
			this.State = MovieScreen.ScreenState.Idle;
			this._gifImage.enabled = false;
			this._currentFrame = 0;
			this._currentFrameTime = 0f;
			this._saveButtonAnchor.SetActive(false);
		}

		// Token: 0x04001EBD RID: 7869
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("MovieScreen");

		// Token: 0x04001EBE RID: 7870
		[Dependency]
		private ISoftwareCapabilities _softwareCapabilities;

		// Token: 0x04001EBF RID: 7871
		[Dependency]
		private PlayerActionController _playerActionController;

		// Token: 0x04001EC0 RID: 7872
		[SerializeField]
		private RectTransform _frameRect;

		// Token: 0x04001EC1 RID: 7873
		[SerializeField]
		private RectTransform _frameRectBorder;

		// Token: 0x04001EC2 RID: 7874
		[SerializeField]
		private RectTransform _frameRectLeft;

		// Token: 0x04001EC3 RID: 7875
		[SerializeField]
		private RectTransform _frameRectRight;

		// Token: 0x04001EC4 RID: 7876
		[SerializeField]
		private RectTransform _frameRectTop;

		// Token: 0x04001EC5 RID: 7877
		[SerializeField]
		private RectTransform _frameRectBottom;

		// Token: 0x04001EC6 RID: 7878
		[SerializeField]
		private RectTransform _progressbarFill;

		// Token: 0x04001EC7 RID: 7879
		[SerializeField]
		private RawImage _gifImage;

		// Token: 0x04001EC8 RID: 7880
		[SerializeField]
		private Image _playImage;

		// Token: 0x04001EC9 RID: 7881
		[SerializeField]
		private GameObject _saveButtonAnchor;

		// Token: 0x04001ECA RID: 7882
		[SerializeField]
		private Animator _playButtonAnimator;

		// Token: 0x04001ECB RID: 7883
		[SerializeField]
		private Sprite _playSprite;

		// Token: 0x04001ECC RID: 7884
		[SerializeField]
		private Sprite _pauseSprite;

		// Token: 0x04001ECD RID: 7885
		[SerializeField]
		private Sprite _loadingSprite;

		// Token: 0x04001ECE RID: 7886
		[Tooltip("How much faster to run the simulation.")]
		[SerializeField]
		[Header("GIF Capture")]
		private float SimulationSpeed = 5f;

		// Token: 0x04001ECF RID: 7887
		[SerializeField]
		[Tooltip("Amount of frames to capture.")]
		private int TotalFrames = 100;

		// Token: 0x04001ED0 RID: 7888
		[SerializeField]
		[Tooltip("Gif resulting framerate.")]
		private int FrameRate = 12;

		// Token: 0x04001ED1 RID: 7889
		[SerializeField]
		[Tooltip("Multiplied by the game's camera scale to get the screen's camera scale.")]
		private float CameraScale = 2f;

		// Token: 0x04001ED2 RID: 7890
		[SerializeField]
		[Tooltip("The padding between the frame and game playable area.")]
		private float Padding = 40f;

		// Token: 0x04001ED3 RID: 7891
		[SerializeField]
		[Tooltip("How long (in seconds) to run the simulation before capturing frames? Used to spin up the trails.")]
		private float WarmUpDuration = 1f;

		// Token: 0x04001ED4 RID: 7892
		private static readonly int LoadingBool = Animator.StringToHash("Loading");

		// Token: 0x04001ED5 RID: 7893
		protected MotorwaysGame _game;

		// Token: 0x04001ED6 RID: 7894
		protected CityDefinition _newCity;

		// Token: 0x04001ED7 RID: 7895
		protected MapDefinition _newMapDefinition;

		// Token: 0x04001ED8 RID: 7896
		protected MapChallenge _newMapChallenge;

		// Token: 0x04001ED9 RID: 7897
		private string _folderString = "";

		// Token: 0x04001EDA RID: 7898
		private float _desiredZoom;

		// Token: 0x04001EDB RID: 7899
		private Vector2 _desiredPosition;

		// Token: 0x04001EDC RID: 7900
		private Vector2 _oldCameraPosition;

		// Token: 0x04001EDD RID: 7901
		private Texture2D[] _frames;

		// Token: 0x04001EDE RID: 7902
		private int _currentFrame;

		// Token: 0x04001EDF RID: 7903
		private float _currentFrameTime;

		// Token: 0x04001EE0 RID: 7904
		private MovieScreen.ScreenState _state;

		// Token: 0x04001EE1 RID: 7905
		private IEnumerator _gifCaptureCoroutine;

		// Token: 0x04001EE2 RID: 7906
		private const float PlaybackFrameTime = 0.08f;

		// Token: 0x04001EE3 RID: 7907
		private MemoryStream _gifStream;

		// Token: 0x04001EE4 RID: 7908
		private AnimatedGifEncoder _gifEncoder;

		// Token: 0x04001EE5 RID: 7909
		private RenderTexture _gifRenderTarget;

		// Token: 0x04001EE6 RID: 7910
		private const float DeltaTime = 0.05f;

		// Token: 0x04001EE7 RID: 7911
		private const float WarmUpProgressAmount = 0.2f;

		// Token: 0x0200055A RID: 1370
		private enum ScreenState
		{
			// Token: 0x04001EE9 RID: 7913
			Idle,
			// Token: 0x04001EEA RID: 7914
			Recording,
			// Token: 0x04001EEB RID: 7915
			Playing,
			// Token: 0x04001EEC RID: 7916
			Paused
		}
	}
}
