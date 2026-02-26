using System;
using System.Collections.Generic;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000582 RID: 1410
	public class BuildingIndicatorEventView : IView, IReusable
	{
		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x060026C9 RID: 9929 RVA: 0x000A5158 File Offset: 0x000A3358
		private Color IndicatorTargetColour
		{
			get
			{
				Color color = Color.white;
				if (this._destinationView != null)
				{
					color = this._destinationView.GetBuildingColor(ThemeComponentGroupTarget.BuildingBase);
				}
				else if (this._houseView != null)
				{
					color = this._houseView.GetBuildingColor(ThemeComponentGroupTarget.BuildingBase);
				}
				return color;
			}
		}

		// Token: 0x060026CA RID: 9930 RVA: 0x000A51A4 File Offset: 0x000A33A4
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			this.TickEchoSpawns(timeInterval.Delta);
			this.TickDarkEchoSpawns(timeInterval.Delta);
			this.TickArrowSpawn(timeInterval.Delta);
			this.RemoveCompletedSpawns();
			this.UpdateSpawnPositions();
			if (this.IsComplete())
			{
				return TickResult.Destroy;
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x060026CB RID: 9931 RVA: 0x000022F5 File Offset: 0x000004F5
		public void SetGameobjectActive(bool isActive)
		{
		}

		// Token: 0x060026CC RID: 9932 RVA: 0x000A51E4 File Offset: 0x000A33E4
		private void TickEchoSpawns(float tickTime)
		{
			if (this._config.echoDelayInSeconds >= 0f)
			{
				this._config.echoDelayInSeconds = this._config.echoDelayInSeconds - tickTime;
				if (this._config.echoDelayInSeconds < 0f)
				{
					IndicatorEchoView echoView = IndicatorEchoView.Create(this._viewClient, this._config.position, this.IndicatorTargetColour, this._config.echoRingWidthCurve, this._config.echoScaleMin, this._config.echoScaleMax, this._config.echoDurationInSeconds);
					this._children.Add(echoView.transform);
					this._config.echoCount = this._config.echoCount - 1;
					if (this._config.echoCount > 0)
					{
						this._config.echoDelayInSeconds = this._config.echoDelayInSeconds + this._config.echoCircleRate;
					}
				}
			}
		}

		// Token: 0x060026CD RID: 9933 RVA: 0x000A52C4 File Offset: 0x000A34C4
		private void TickDarkEchoSpawns(float tickTime)
		{
			if (this._config.darkEchoDelayInSeconds >= 0f)
			{
				this._config.darkEchoDelayInSeconds = this._config.darkEchoDelayInSeconds - tickTime;
				if (this._config.darkEchoDelayInSeconds < 0f)
				{
					AlertView darkEchoView = AlertView.Create(this._viewClient, this._config.position, new Color?(this._theme.GetGlobalColor(this._constants.BuildingEchoAlertColor)), new float?(this._config.darkEchoScaleMax), null, null);
					this._children.Add(darkEchoView.transform);
				}
			}
		}

		// Token: 0x060026CE RID: 9934 RVA: 0x000A5374 File Offset: 0x000A3574
		private void TickArrowSpawn(float tickTime)
		{
			if (this._config.arrowDelayInSeconds >= 0f)
			{
				this._config.arrowDelayInSeconds = this._config.arrowDelayInSeconds - tickTime;
				if (this._config.arrowDelayInSeconds < 0f && this.ShouldCreateIndicatorArrow())
				{
					IndicatorArrowView.Create(this._viewClient, this._destinationView, this._config.arrowType, this._safeAreaRect, this._config.arrowKnockNumber, this._config.arrowKnockDelay, this._config.arrowExitDelay);
				}
			}
		}

		// Token: 0x060026CF RID: 9935 RVA: 0x000A5404 File Offset: 0x000A3604
		private bool ShouldCreateIndicatorArrow()
		{
			if (this._destinationView == null)
			{
				return false;
			}
			if (!this._cameraView.IsFocussedIn)
			{
				return false;
			}
			Camera defaultCamera = this._gameCamera.DefaultCamera;
			this._safeAreaRect.GetWorldCorners(this._safeAreaWorldCorners);
			Vector3 minV = defaultCamera.WorldToScreenPoint(this._safeAreaWorldCorners[0]);
			Vector3 maxV = defaultCamera.WorldToScreenPoint(this._safeAreaWorldCorners[2]);
			Rect screenRect = Rect.MinMaxRect(minV.x, minV.y, maxV.x, maxV.y);
			Bounds destinationBounds = this._destinationView.GetBounds();
			Vector3 screenTargetMin = defaultCamera.WorldToScreenPoint(destinationBounds.min);
			Vector3 screenTargetMax = defaultCamera.WorldToScreenPoint(destinationBounds.max);
			return !Rect.MinMaxRect(screenTargetMin.x, screenTargetMin.y, screenTargetMax.x, screenTargetMax.y).Overlaps(screenRect);
		}

		// Token: 0x060026D0 RID: 9936 RVA: 0x000A54EC File Offset: 0x000A36EC
		private void RemoveCompletedSpawns()
		{
			int childIndex = 0;
			while (childIndex < this._children.Count)
			{
				if (!this._children[childIndex].gameObject.activeSelf)
				{
					this._children.RemoveAt(childIndex);
				}
				else
				{
					childIndex++;
				}
			}
		}

		// Token: 0x060026D1 RID: 9937 RVA: 0x000A5535 File Offset: 0x000A3735
		private void UpdateSpawnPositions()
		{
			if (this._config.clampToScreen)
			{
				this.ClampToScreen();
			}
		}

		// Token: 0x060026D2 RID: 9938 RVA: 0x000A554C File Offset: 0x000A374C
		private void ClampToScreen()
		{
			Camera owningCamera = this._gameCamera.DefaultCamera;
			Vector3 worldPosition = this._config.position;
			Vector3 screenPosition = owningCamera.WorldToScreenPoint(worldPosition);
			Vector3 clampedScreenPosition = new Vector3(Mathf.Clamp(screenPosition.x, 0f, (float)owningCamera.pixelWidth), Mathf.Clamp(screenPosition.y, 0f, (float)owningCamera.pixelHeight), screenPosition.z);
			Vector3 clampedWorldPos = owningCamera.ScreenToWorldPoint(clampedScreenPosition);
			foreach (Transform transform in this._children)
			{
				transform.position = clampedWorldPos;
			}
		}

		// Token: 0x060026D3 RID: 9939 RVA: 0x000A5608 File Offset: 0x000A3808
		private bool IsComplete()
		{
			return this._config.echoDelayInSeconds < 0f && this._config.darkEchoDelayInSeconds < 0f && this._children.Count <= 0;
		}

		// Token: 0x060026D4 RID: 9940 RVA: 0x000A5641 File Offset: 0x000A3841
		public void Reset()
		{
			this._destinationView = null;
			this._houseView = null;
			this._config = default(BuildingIndicatorEventView.Config);
			this._children.Clear();
		}

		// Token: 0x060026D5 RID: 9941 RVA: 0x000A5668 File Offset: 0x000A3868
		private static BuildingIndicatorEventView Create(ViewClient viewClient, ref BuildingIndicatorEventView.Config config)
		{
			BuildingIndicatorEventView newEvent = viewClient.Scope.Get<BuildingIndicatorEventView>();
			newEvent._config = config;
			viewClient.AddView(newEvent);
			return newEvent;
		}

		// Token: 0x060026D6 RID: 9942 RVA: 0x000A5695 File Offset: 0x000A3895
		public static BuildingIndicatorEventView CreateHouseIndicator(ViewClient viewClient, HouseView houseView, ref BuildingIndicatorEventView.Config config)
		{
			BuildingIndicatorEventView buildingIndicatorEventView = BuildingIndicatorEventView.Create(viewClient, ref config);
			buildingIndicatorEventView._houseView = houseView;
			return buildingIndicatorEventView;
		}

		// Token: 0x060026D7 RID: 9943 RVA: 0x000A56A5 File Offset: 0x000A38A5
		public static BuildingIndicatorEventView CreateDestinationIndicator(ViewClient viewClient, DestinationView destinationView, RectTransform safeAreaRect, ref BuildingIndicatorEventView.Config config)
		{
			BuildingIndicatorEventView buildingIndicatorEventView = BuildingIndicatorEventView.Create(viewClient, ref config);
			buildingIndicatorEventView._destinationView = destinationView;
			buildingIndicatorEventView._safeAreaRect = safeAreaRect;
			return buildingIndicatorEventView;
		}

		// Token: 0x040020B9 RID: 8377
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x040020BA RID: 8378
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x040020BB RID: 8379
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x040020BC RID: 8380
		[Dependency]
		private MotorwaysThemeDatabase _theme;

		// Token: 0x040020BD RID: 8381
		[Dependency]
		private VisualConstantsData _constants;

		// Token: 0x040020BE RID: 8382
		private DestinationView _destinationView;

		// Token: 0x040020BF RID: 8383
		private HouseView _houseView;

		// Token: 0x040020C0 RID: 8384
		private BuildingIndicatorEventView.Config _config;

		// Token: 0x040020C1 RID: 8385
		private RectTransform _safeAreaRect;

		// Token: 0x040020C2 RID: 8386
		private Vector3[] _safeAreaWorldCorners = new Vector3[4];

		// Token: 0x040020C3 RID: 8387
		private List<Transform> _children = new List<Transform>();

		// Token: 0x02000583 RID: 1411
		[System.Serializable]
		public struct Config
		{
			// Token: 0x040020C4 RID: 8388
			public float echoDelayInSeconds;

			// Token: 0x040020C5 RID: 8389
			public AnimationCurve echoRingWidthCurve;

			// Token: 0x040020C6 RID: 8390
			public float echoScaleMin;

			// Token: 0x040020C7 RID: 8391
			public float echoScaleMax;

			// Token: 0x040020C8 RID: 8392
			public float echoDurationInSeconds;

			// Token: 0x040020C9 RID: 8393
			public bool clampToScreen;

			// Token: 0x040020CA RID: 8394
			public int echoCount;

			// Token: 0x040020CB RID: 8395
			public float echoCircleRate;

			// Token: 0x040020CC RID: 8396
			public float darkEchoDelayInSeconds;

			// Token: 0x040020CD RID: 8397
			public float darkEchoScaleMax;

			// Token: 0x040020CE RID: 8398
			public float arrowDelayInSeconds;

			// Token: 0x040020CF RID: 8399
			public IndicatorArrowView.IndicatorType arrowType;

			// Token: 0x040020D0 RID: 8400
			public int arrowKnockNumber;

			// Token: 0x040020D1 RID: 8401
			public float arrowKnockDelay;

			// Token: 0x040020D2 RID: 8402
			public float arrowExitDelay;

			// Token: 0x040020D3 RID: 8403
			public Vector2 position;
		}
	}
}
