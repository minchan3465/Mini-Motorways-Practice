using System;
using System.Collections.Generic;
using System.Linq;
using Client;
using Factory;
using Factory.Pools;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000584 RID: 1412
	public class BuildingsIndicatorView : MonoBehaviour, IView, IViewClientObserver, IReusable, IReleasedFromScopeHandler, DestinationView.IObserver
	{
		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x060026D9 RID: 9945 RVA: 0x000A56DB File Offset: 0x000A38DB
		// (set) Token: 0x060026DA RID: 9946 RVA: 0x000A56E3 File Offset: 0x000A38E3
		public bool AlertsEnabled { get; set; } = true;

		// Token: 0x060026DB RID: 9947 RVA: 0x000A56EC File Offset: 0x000A38EC
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._viewClient.OnFirstFrame)
			{
				this._viewClient.Subscribe(this);
				if (!this._city.Rules.ShowDisconnectedBuildingsUI())
				{
					return TickResult.Destroy;
				}
				foreach (DestinationView destinationView in this._city.Scope.Get<ViewIndex>().DestinationViews)
				{
					destinationView.Subscribe(this);
				}
				SafeArea safeArea = this._gameUiScreen.safeArea;
				if (Diagnostics.Verify(safeArea != null, this._gameUiScreen, "Safe area hasn't been set on the GameUIScreen component. We need to update the prefab"))
				{
					this._safeAreaRect = safeArea.GetComponent<RectTransform>();
				}
				this._timeUntilPulseInSeconds = this._pulseDelayInSeconds;
			}
			if (!this._game.HasGameEnded)
			{
				this.TickPendingIndicators(timeInterval.Delta);
				this.TickPulses(timeInterval.Delta);
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x060026DC RID: 9948 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x060026DD RID: 9949 RVA: 0x000A57DC File Offset: 0x000A39DC
		private void TickPendingIndicators(float tickTime)
		{
			if (this._timeUntilPendingIndicator > 0f && !this._cameraView.IsFocussedIn)
			{
				this._timeUntilPendingIndicator -= tickTime;
				if (this._timeUntilPendingIndicator <= 0f && this._pendingIndicators.Count > 0)
				{
					HouseView houseView = this._pendingIndicators[0];
					this._pendingIndicators.RemoveAt(0);
					this.CreateHouseAddedIndicator(houseView);
					this._timeUntilPendingIndicator += this._echoRate;
				}
			}
		}

		// Token: 0x060026DE RID: 9950 RVA: 0x000A585F File Offset: 0x000A3A5F
		private void TickPulses(float tickTime)
		{
			if (this._pulsingEnabled)
			{
				this._timeUntilPulseInSeconds -= tickTime;
				if (this._timeUntilPulseInSeconds < 0f)
				{
					this._timeUntilPulseInSeconds += this._pulseRateInSeconds;
					this.DoPulse();
				}
			}
		}

		// Token: 0x060026DF RID: 9951 RVA: 0x000A58A0 File Offset: 0x000A3AA0
		private void DoPulse()
		{
			foreach (DestinationView destinationView in this._city.Scope.Get<ViewIndex>().DestinationViews)
			{
				if (this.IsValidForPulse(destinationView))
				{
					destinationView.DoDisconnectedPulse();
				}
			}
		}

		// Token: 0x060026E0 RID: 9952 RVA: 0x000A5904 File Offset: 0x000A3B04
		private bool IsValidForPulse(DestinationView destinationView)
		{
			return destinationView.NetworkConnectivity == NetworkConnectivity.Disconnected && Time.time - destinationView.SpawnTime >= this._pulseDelayInSeconds;
		}

		// Token: 0x060026E1 RID: 9953 RVA: 0x000A5928 File Offset: 0x000A3B28
		public void OnReleasedFromScope(IScope scope)
		{
			this._viewClient.Unsubscribe(this);
		}

		// Token: 0x060026E2 RID: 9954 RVA: 0x000A5938 File Offset: 0x000A3B38
		public void OnViewAdded(IClient client, IView view)
		{
			HouseView houseView = view as HouseView;
			if (houseView != null)
			{
				this.OnHouseAdded(houseView);
				return;
			}
			DestinationView destinationView = view as DestinationView;
			if (destinationView != null)
			{
				this.CreateDestinationAddedIndicator(destinationView);
				destinationView.Subscribe(this);
			}
		}

		// Token: 0x060026E3 RID: 9955 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnViewRemoved(IClient client, IView view)
		{
		}

		// Token: 0x060026E4 RID: 9956 RVA: 0x000A596F File Offset: 0x000A3B6F
		public void OnDemandLevelChanged(DestinationView owner)
		{
			if (owner.Model.IsUpgraded)
			{
				this.CreateDestinationUpgradedEcho(owner);
			}
		}

		// Token: 0x060026E5 RID: 9957 RVA: 0x000A5985 File Offset: 0x000A3B85
		public void OnImminentFailAlert(DestinationView destinationView, bool isInitialAlert)
		{
			if (isInitialAlert)
			{
				this.CreateDestinationImminentFailEcho(destinationView);
				return;
			}
			this.CreateDestinationAlert(destinationView);
		}

		// Token: 0x060026E6 RID: 9958 RVA: 0x000A5999 File Offset: 0x000A3B99
		public void OnBigPinAppeared(DestinationView destinationView)
		{
			this.CreateDestinationBigPinEcho(destinationView);
		}

		// Token: 0x060026E7 RID: 9959 RVA: 0x000A59A4 File Offset: 0x000A3BA4
		private BuildingIndicatorEventView.Config BuildIndicatorConfig(BuildingsIndicatorView.IndicatorSharedSettings sharedSettings, BuildingsIndicatorView.IndicatorSettings settings)
		{
			BuildingIndicatorEventView.Config config = new BuildingIndicatorEventView.Config
			{
				echoDelayInSeconds = sharedSettings.echoDelayInSeconds,
				echoRingWidthCurve = sharedSettings.echoRingWidthCurve,
				echoScaleMin = sharedSettings.echoScaleMin,
				echoScaleMax = sharedSettings.echoScaleMax,
				echoDurationInSeconds = sharedSettings.echoDurationInSeconds,
				clampToScreen = settings.clampToScreen,
				echoCount = settings.echoCount,
				echoCircleRate = sharedSettings.echoRepeatDelayInSeconds,
				darkEchoDelayInSeconds = sharedSettings.darkEchoDelayInSeconds,
				darkEchoScaleMax = BuildingsIndicatorView.NormaliseDarkEchoScale(sharedSettings.darkEchoScaleMax),
				arrowDelayInSeconds = sharedSettings.arrowDelayInSeconds,
				arrowType = settings.arrowType,
				arrowKnockNumber = this._arrowKnockNumber,
				arrowKnockDelay = this._arrowKnockDelayInSeconds,
				arrowExitDelay = this._arrowExitDelay
			};
			if (!settings.hasDarkEcho)
			{
				config.darkEchoDelayInSeconds = -1f;
			}
			return config;
		}

		// Token: 0x060026E8 RID: 9960 RVA: 0x000A5A98 File Offset: 0x000A3C98
		private static float NormaliseDarkEchoScale(float scale)
		{
			return scale * 0.8f - 1f;
		}

		// Token: 0x060026E9 RID: 9961 RVA: 0x000A5AA8 File Offset: 0x000A3CA8
		private void OnHouseAdded(HouseView houseView)
		{
			Bounds bounds = houseView.GetBounds();
			bool isBuildingOnScreen = this.IsBoundsIntersectingScreen(bounds);
			if (!this._cameraView.IsFocussedIn || isBuildingOnScreen)
			{
				this.CreateHouseAddedIndicator(houseView);
				return;
			}
			this._pendingIndicators.Add(houseView);
			if (this._timeUntilPendingIndicator <= 0f)
			{
				this._timeUntilPendingIndicator = this._echoRate;
			}
		}

		// Token: 0x060026EA RID: 9962 RVA: 0x000A5B04 File Offset: 0x000A3D04
		private bool IsBoundsIntersectingScreen(Bounds bounds)
		{
			Camera owningCamera = this._gameCamera.DefaultCamera;
			Rect screenRect = new Rect
			{
				max = new Vector2((float)owningCamera.pixelWidth, (float)owningCamera.pixelHeight)
			};
			return new Rect
			{
				min = owningCamera.WorldToScreenPoint(bounds.min),
				max = owningCamera.WorldToScreenPoint(bounds.max)
			}.Overlaps(screenRect);
		}

		// Token: 0x060026EB RID: 9963 RVA: 0x000A5B88 File Offset: 0x000A3D88
		private bool IsDestinationIntersectionScreen(DestinationView destinationView)
		{
			Bounds bounds = destinationView.GetBounds();
			return this.IsBoundsIntersectingScreen(bounds);
		}

		// Token: 0x060026EC RID: 9964 RVA: 0x000A5BA4 File Offset: 0x000A3DA4
		private void CreateHouseAddedIndicator(HouseView houseView)
		{
			if (this.AlertsEnabled)
			{
				BuildingIndicatorEventView.Config config = this.BuildIndicatorConfig(this._houseSharedSettings, this._houseAppearIndicatorSettings);
				config.position = houseView.transform.position;
				BuildingIndicatorEventView.CreateHouseIndicator(this._viewClient, houseView, ref config);
			}
		}

		// Token: 0x060026ED RID: 9965 RVA: 0x000A5BF4 File Offset: 0x000A3DF4
		private void CreateDestinationAddedIndicator(DestinationView destinationView)
		{
			if (this.AlertsEnabled)
			{
				if (this.IsDestinationIntersectionScreen(destinationView))
				{
					this.CreateDestinationAlert(destinationView);
				}
				BuildingIndicatorEventView.Config config = this.BuildIndicatorConfig(this._destinationSharedSettings, this._destinationAppearIndicatorSettings);
				config.position = destinationView.transform.position;
				BuildingIndicatorEventView.CreateDestinationIndicator(this._viewClient, destinationView, this._safeAreaRect, ref config);
			}
		}

		// Token: 0x060026EE RID: 9966 RVA: 0x000A5C58 File Offset: 0x000A3E58
		private void CreateDestinationBigPinEcho(DestinationView destinationView)
		{
			if (this.AlertsEnabled)
			{
				BuildingIndicatorEventView.Config config = this.BuildIndicatorConfig(this._destinationSharedSettings, this._destinationBigPinIndicatorSettings);
				config.position = destinationView.BigPinAlertPosition;
				BuildingIndicatorEventView.CreateDestinationIndicator(this._viewClient, destinationView, this._safeAreaRect, ref config);
			}
		}

		// Token: 0x060026EF RID: 9967 RVA: 0x000A5CA4 File Offset: 0x000A3EA4
		private void CreateDestinationUpgradedEcho(DestinationView destinationView)
		{
			if (this.AlertsEnabled)
			{
				BuildingIndicatorEventView.Config config = this.BuildIndicatorConfig(this._destinationSharedSettings, this._destinationDemandUpgradedIndicatorSettings);
				config.position = destinationView.transform.position;
				BuildingIndicatorEventView.CreateDestinationIndicator(this._viewClient, destinationView, this._safeAreaRect, ref config);
			}
		}

		// Token: 0x060026F0 RID: 9968 RVA: 0x000A5CF8 File Offset: 0x000A3EF8
		private void CreateDestinationImminentFailEcho(DestinationView destinationView)
		{
			if (this.AlertsEnabled)
			{
				BuildingIndicatorEventView.Config config = this.BuildIndicatorConfig(this._destinationSharedSettings, this._destinationImminentFailIndicatorSettings);
				config.position = destinationView.BigPinAlertPosition;
				BuildingIndicatorEventView.CreateDestinationIndicator(this._viewClient, destinationView, this._safeAreaRect, ref config);
			}
		}

		// Token: 0x060026F1 RID: 9969 RVA: 0x000A5D44 File Offset: 0x000A3F44
		private void CreateDestinationAlert(DestinationView destinationView)
		{
			if (this.AlertsEnabled && this._city.Rules.ShowsUI())
			{
				AlertView.Create(this._viewClient, destinationView.transform.position, new Color?(this._themeDatabase.GetGlobalColor(this._constants.BuildingEchoAlertColor)), null, null, null);
			}
		}

		// Token: 0x060026F2 RID: 9970 RVA: 0x000A5DB8 File Offset: 0x000A3FB8
		public void Reset()
		{
			this._pulsingEnabled = false;
			this.AlertsEnabled = true;
			this._timeUntilPulseInSeconds = 0f;
			this._timeUntilPendingIndicator = 0f;
			this._pendingIndicators.Clear();
		}

		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x060026F3 RID: 9971 RVA: 0x000A5DE9 File Offset: 0x000A3FE9
		public bool PulsingEnabled
		{
			get
			{
				return this._pulsingEnabled;
			}
		}

		// Token: 0x060026F4 RID: 9972 RVA: 0x000A5DF1 File Offset: 0x000A3FF1
		public void StartPulsing()
		{
			this._pulsingEnabled = true;
		}

		// Token: 0x060026F5 RID: 9973 RVA: 0x000A5DFA File Offset: 0x000A3FFA
		public void StopPulsing()
		{
			this._pulsingEnabled = false;
			this._timeUntilPulseInSeconds = this._pulseRateInSeconds;
		}

		// Token: 0x060026F6 RID: 9974 RVA: 0x000A5E0F File Offset: 0x000A400F
		public IEnumerable<HouseView> EDITOR_GetHouseViews()
		{
			if (this._city == null)
			{
				return Enumerable.Empty<HouseView>();
			}
			return this._city.Scope.Get<ViewIndex>().HouseViews;
		}

		// Token: 0x060026F7 RID: 9975 RVA: 0x000A5E34 File Offset: 0x000A4034
		public IEnumerable<DestinationView> EDITOR_GetDestinationViews()
		{
			if (this._city == null)
			{
				return Enumerable.Empty<DestinationView>();
			}
			return this._city.Scope.Get<ViewIndex>().DestinationViews;
		}

		// Token: 0x060026F8 RID: 9976 RVA: 0x000A5E59 File Offset: 0x000A4059
		public void EDITOR_CreateHouseAddedIndicator(HouseView houseView)
		{
			this.CreateHouseAddedIndicator(houseView);
		}

		// Token: 0x060026F9 RID: 9977 RVA: 0x000A5E62 File Offset: 0x000A4062
		public void EDITOR_CreateDestinationAddedIndicator(DestinationView destinationView)
		{
			this.CreateDestinationAddedIndicator(destinationView);
		}

		// Token: 0x060026FA RID: 9978 RVA: 0x000A5999 File Offset: 0x000A3B99
		public void EDITOR_CreateDestinationBigPinEcho(DestinationView destinationView)
		{
			this.CreateDestinationBigPinEcho(destinationView);
		}

		// Token: 0x060026FB RID: 9979 RVA: 0x000A5E6B File Offset: 0x000A406B
		public void EDITOR_CreateDestinationUpgradedEcho(DestinationView destinationView)
		{
			this.CreateDestinationUpgradedEcho(destinationView);
		}

		// Token: 0x060026FC RID: 9980 RVA: 0x000A5E74 File Offset: 0x000A4074
		public void EDITOR_CreateDestinationImminentFailEcho(DestinationView destinationView)
		{
			this.CreateDestinationImminentFailEcho(destinationView);
		}

		// Token: 0x040020D4 RID: 8404
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x040020D5 RID: 8405
		[Dependency]
		private MotorwaysGame _game;

		// Token: 0x040020D6 RID: 8406
		[Dependency]
		private City _city;

		// Token: 0x040020D7 RID: 8407
		[Dependency]
		private MotorwaysThemeDatabase _themeDatabase;

		// Token: 0x040020D8 RID: 8408
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x040020D9 RID: 8409
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x040020DA RID: 8410
		[Dependency]
		private GameUIScreen _gameUiScreen;

		// Token: 0x040020DB RID: 8411
		[Dependency]
		private VisualConstantsData _constants;

		// Token: 0x040020DC RID: 8412
		[MinValue(0f)]
		[SerializeField]
		[Header("Indicator Event Shared Settings")]
		[Tooltip("When buildings spawn offscreen in edit mode, this is the time between echos when the player exits edit mode.")]
		private float _echoRate;

		// Token: 0x040020DD RID: 8413
		[SerializeField]
		[MinValue(0f)]
		private float _arrowExitDelay;

		// Token: 0x040020DE RID: 8414
		[SerializeField]
		[MinValue(0)]
		private int _arrowKnockNumber;

		// Token: 0x040020DF RID: 8415
		[MinValue(0f)]
		[SerializeField]
		private float _arrowKnockDelayInSeconds;

		// Token: 0x040020E0 RID: 8416
		[SerializeField]
		private BuildingsIndicatorView.IndicatorSharedSettings _houseSharedSettings;

		// Token: 0x040020E1 RID: 8417
		[SerializeField]
		private BuildingsIndicatorView.IndicatorSharedSettings _destinationSharedSettings;

		// Token: 0x040020E2 RID: 8418
		[Header("Indicator Event Settings")]
		[SerializeField]
		private BuildingsIndicatorView.IndicatorSettings _houseAppearIndicatorSettings;

		// Token: 0x040020E3 RID: 8419
		[SerializeField]
		private BuildingsIndicatorView.IndicatorSettings _destinationAppearIndicatorSettings;

		// Token: 0x040020E4 RID: 8420
		[SerializeField]
		private BuildingsIndicatorView.IndicatorSettings _destinationDemandUpgradedIndicatorSettings;

		// Token: 0x040020E5 RID: 8421
		[SerializeField]
		private BuildingsIndicatorView.IndicatorSettings _destinationBigPinIndicatorSettings;

		// Token: 0x040020E6 RID: 8422
		[SerializeField]
		private BuildingsIndicatorView.IndicatorSettings _destinationImminentFailIndicatorSettings;

		// Token: 0x040020E7 RID: 8423
		[Tooltip("The time it takes a building to begin pulsing since it was spawned.")]
		[MinValue(0f)]
		[SerializeField]
		[Header("Pulse Settings")]
		private float _pulseDelayInSeconds;

		// Token: 0x040020E8 RID: 8424
		[Tooltip("The time interval between the start of each pulse. Timing does not wait until the pulse has finished.")]
		[SerializeField]
		[MinValue(0f)]
		private float _pulseRateInSeconds;

		// Token: 0x040020EA RID: 8426
		private RectTransform _safeAreaRect;

		// Token: 0x040020EB RID: 8427
		private bool _pulsingEnabled;

		// Token: 0x040020EC RID: 8428
		private float _timeUntilPulseInSeconds;

		// Token: 0x040020ED RID: 8429
		private float _timeUntilPendingIndicator;

		// Token: 0x040020EE RID: 8430
		private readonly List<HouseView> _pendingIndicators = new List<HouseView>();

		// Token: 0x02000585 RID: 1413
		[System.Serializable]
		private struct IndicatorSharedSettings
		{
			// Token: 0x060026FE RID: 9982 RVA: 0x000A5E98 File Offset: 0x000A4098
			public override int GetHashCode()
			{
				return this.echoDelayInSeconds.GetHashCode() ^ this.echoDurationInSeconds.GetHashCode() ^ this.echoScaleMin.GetHashCode() ^ this.echoScaleMax.GetHashCode() ^ this.echoRepeatDelayInSeconds.GetHashCode() ^ this.darkEchoDelayInSeconds.GetHashCode() ^ this.darkEchoScaleMax.GetHashCode() ^ this.arrowDelayInSeconds.GetHashCode();
			}

			// Token: 0x040020EF RID: 8431
			[MinValue(0f)]
			[Tooltip("The time to wait once the building spawns before the echo starts playing.")]
			public float echoDelayInSeconds;

			// Token: 0x040020F0 RID: 8432
			[MinValue(0f)]
			[Tooltip("The time it takes the echo to go from the scale Min to Max.")]
			public float echoDurationInSeconds;

			// Token: 0x040020F1 RID: 8433
			[Tooltip("The width of the echo ring on the screen as progress goes from 0 to 1")]
			public AnimationCurve echoRingWidthCurve;

			// Token: 0x040020F2 RID: 8434
			[MinValue(0f)]
			[Tooltip("The initial scale of the echo when it spawns.")]
			public float echoScaleMin;

			// Token: 0x040020F3 RID: 8435
			[Tooltip("The final scale of the echo before it is destroyed.")]
			[MinValue(0f)]
			public float echoScaleMax;

			// Token: 0x040020F4 RID: 8436
			[Tooltip("The time between echos for an event which has multiple echos.")]
			[MinValue(0f)]
			public float echoRepeatDelayInSeconds;

			// Token: 0x040020F5 RID: 8437
			[Space]
			[MinValue(0f)]
			[Tooltip("The time to wait once the building spawns before the dark echo starts playing.")]
			public float darkEchoDelayInSeconds;

			// Token: 0x040020F6 RID: 8438
			[MinValue(0f)]
			[Tooltip("The final scale of the dark echo before it is destroyed.")]
			public float darkEchoScaleMax;

			// Token: 0x040020F7 RID: 8439
			[Space]
			[Tooltip("The time to wait once the building spawns before the arrow appears.")]
			[MinValue(0f)]
			public float arrowDelayInSeconds;
		}

		// Token: 0x02000586 RID: 1414
		[System.Serializable]
		private struct IndicatorSettings
		{
			// Token: 0x040020F8 RID: 8440
			[Tooltip("The number of echos to display with a timing interval of echoRepeatDelayInSeconds.")]
			[MinValue(1)]
			public int echoCount;

			// Token: 0x040020F9 RID: 8441
			[Tooltip("If the building this indicator relates to is offscreen, clampToScreen will keep the indicator half on screen.")]
			public bool clampToScreen;

			// Token: 0x040020FA RID: 8442
			[Tooltip("If a dark echo should be played for this indicator.")]
			public bool hasDarkEcho;

			// Token: 0x040020FB RID: 8443
			[Tooltip("The arrow type changes the arrow icon and color.")]
			public IndicatorArrowView.IndicatorType arrowType;
		}
	}
}
