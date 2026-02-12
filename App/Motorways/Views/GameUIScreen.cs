using System;
using System.Collections.Generic;
using Client;
using Easing;
using Factory;
using Motorways.Audio;
using Motorways.Models;
using Motorways.UI;
using Screens;
using Server;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000540 RID: 1344
	public class GameUIScreen : InGameScalingScreen, IView
	{
		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x0600239B RID: 9115 RVA: 0x00092643 File Offset: 0x00090843
		// (set) Token: 0x0600239C RID: 9116 RVA: 0x0009264B File Offset: 0x0009084B
		public UpgradeBarClient UpgradeBar { get; private set; }

		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x0600239D RID: 9117 RVA: 0x00092654 File Offset: 0x00090854
		public ScoreView ScoreView
		{
			get
			{
				return this._scoreView;
			}
		}

		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x0600239E RID: 9118 RVA: 0x0009265C File Offset: 0x0009085C
		// (set) Token: 0x0600239F RID: 9119 RVA: 0x00092664 File Offset: 0x00090864
		public EditMenuPanel editMenuPanel { get; private set; }

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x060023A0 RID: 9120 RVA: 0x0009266D File Offset: 0x0009086D
		// (set) Token: 0x060023A1 RID: 9121 RVA: 0x00092675 File Offset: 0x00090875
		public ColourWidget ColourWidget { get; private set; }

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x060023A2 RID: 9122 RVA: 0x0009267E File Offset: 0x0009087E
		public TouchButton ScoreButton
		{
			get
			{
				return this._clockView.ScoreButton;
			}
		}

		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x060023A3 RID: 9123 RVA: 0x0009268B File Offset: 0x0009088B
		// (set) Token: 0x060023A4 RID: 9124 RVA: 0x00092693 File Offset: 0x00090893
		public bool TimeButtonsVisible { get; private set; }

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x060023A5 RID: 9125 RVA: 0x0009269C File Offset: 0x0009089C
		public GameObject ScoreTextAnchor
		{
			get
			{
				return this._scoreTextAnchorActive;
			}
		}

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x060023A6 RID: 9126 RVA: 0x000926A4 File Offset: 0x000908A4
		public GameObject ClockAnchor
		{
			get
			{
				return this._clockAnchorActive;
			}
		}

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x060023A7 RID: 9127 RVA: 0x000926AC File Offset: 0x000908AC
		public RectTransform OverlayTransform
		{
			get
			{
				return this._overlayTransform;
			}
		}

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x060023A8 RID: 9128 RVA: 0x000926B4 File Offset: 0x000908B4
		// (set) Token: 0x060023A9 RID: 9129 RVA: 0x000926BC File Offset: 0x000908BC
		public bool FocusPointIsBlocked { get; private set; }

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x060023AA RID: 9130 RVA: 0x000926C5 File Offset: 0x000908C5
		public ISubmitHandler FocussedSelectable
		{
			get
			{
				return this._focussedSelectable;
			}
		}

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x060023AB RID: 9131 RVA: 0x000926CD File Offset: 0x000908CD
		// (set) Token: 0x060023AC RID: 9132 RVA: 0x000926D5 File Offset: 0x000908D5
		public RoadDrawMode CurrentRoadDrawMode
		{
			get
			{
				return this._currentDrawMode;
			}
			set
			{
				this._currentDrawMode = value;
				this.drawModeToggle.SetDrawMode(this._currentDrawMode);
				this._themeDatabase.SetDrawMode(this._currentDrawMode);
			}
		}

		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x060023AD RID: 9133 RVA: 0x00092700 File Offset: 0x00090900
		public bool IsElectiveUpgradeRequested
		{
			get
			{
				return this._electiveUpgradeState == GameUIScreen.ElectiveUpgradeState.RequestedUpgrade;
			}
		}

		// Token: 0x060023AE RID: 9134 RVA: 0x0009270B File Offset: 0x0009090B
		public void ToggleDrawMode()
		{
			this.CurrentRoadDrawMode = ((this.CurrentRoadDrawMode == RoadDrawMode.Add) ? RoadDrawMode.Remove : RoadDrawMode.Add);
		}

		// Token: 0x060023AF RID: 9135 RVA: 0x00092720 File Offset: 0x00090920
		public override void Awake()
		{
			base.Awake();
			Canvas thisCanvas = base.GetComponent<Canvas>();
			this._overlayCanvasObject = new GameObject(base.name + "-OverlayCanvas");
			this._overlayCanvas = this._overlayCanvasObject.AddComponent<Canvas>();
			this._overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
			this._overlayCanvas.planeDistance = thisCanvas.planeDistance;
			this._overlayCanvas.sortingOrder = thisCanvas.sortingOrder;
			CanvasScaler thisCanvasScaler = base.GetComponent<CanvasScaler>();
			CanvasScaler canvasScaler = this._overlayCanvasObject.AddComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = thisCanvasScaler.uiScaleMode;
			canvasScaler.referenceResolution = thisCanvasScaler.referenceResolution;
			canvasScaler.screenMatchMode = thisCanvasScaler.screenMatchMode;
			canvasScaler.matchWidthOrHeight = thisCanvasScaler.matchWidthOrHeight;
			this._overlayCanvas.referencePixelsPerUnit = thisCanvasScaler.referencePixelsPerUnit;
			GameObject safeAreaObject = new GameObject("SafeArea");
			this._overlayTransform = safeAreaObject.AddComponent<RectTransform>();
			safeAreaObject.AddComponent<SafeArea>();
			this._overlayTransform.SetParent(this._overlayCanvasObject.transform, false);
			this._overlayTransform.localPosition = Vector2.zero;
			this._overlayTransform.localScale = Vector2.one;
			this._overlayTransform.sizeDelta = Vector2.zero;
			this._overlayCanvasObject.SetActive(false);
			this._timeButtonFloaters[0] = this.pauseButton.GetComponent<FloatingElement>();
			this._timeButtonFloaters[1] = this.playButton.GetComponent<FloatingElement>();
			this._timeButtonFloaters[2] = this.fastForwardButton.GetComponent<FloatingElement>();
			this._worldGridGameObject = this._worldGrid.gameObject;
		}

		// Token: 0x060023B0 RID: 9136 RVA: 0x000928AC File Offset: 0x00090AAC
		public override void OnCreatedInScope(IScope scope)
		{
			this.UpgradeBar = scope.Get<UpgradeBarClient>();
			this.editMenuPanel = scope.Get<EditMenuPanel>();
			this.ColourWidget = scope.Get<ColourWidget>();
			this._clockView = scope.Get<ClockView>();
			this._clockView.transform.SetParent(this._clockViewParent, false);
			this._clockView.gameObject.SetActive(true);
			this._clockView.OnClockToggled += this.OnClockToggled;
			this._scoreView = this._clockView.ScoreView;
			this._scoreView.OnElectiveUpgradeButtonPressed += this.OnElectiveUpgradeButtonPressed;
			this._scoreView.OnScoreButtonPressed += this.OnScorePressed;
			ViewClient viewClient = scope.Get<ViewClient>();
			viewClient.AddView(this.UpgradeBar);
			viewClient.AddView(this.editMenuPanel);
			viewClient.AddView(this.ColourWidget);
			base.OnCreatedInScope(scope);
			this._canvasGroup.SetInteractable(false);
			this._canvasGroup.SetBlocksRaycasts(false);
			this._canvasGroup.Alpha = 0f;
			this._inputState.Subscribe(this);
			this._scaleToCamera = false;
			this._currentDrawMode = RoadDrawMode.Add;
			this.SetWorldGridActive(false, TransitionStyle.Snap);
			this._tilemapView.viewMode = TilemapView.ViewMode.Normal;
			this.SetRoadCursorActive(false);
			if (this._focusPoint != null)
			{
				this.SetFocusPointActive(false, true);
			}
			if (this._currentSelectedTile != null)
			{
				this._currentSelectedTile.IsHighlighted = false;
				this._currentSelectedTile = null;
			}
			this.backButton.ForceInitializeState();
			this._gameCamera.AttachCameraToCanvas(this._canvas, CameraLayer.UI);
			this.SetUIVisible(false, true, false, false);
			this.fastForwardButton.gameObject.SetActive(true);
			this.extraFastForwardButton.gameObject.SetActive(false);
			this.SetVcrButtonState(false, TimeScale.Single);
		}

		// Token: 0x060023B1 RID: 9137 RVA: 0x00092A80 File Offset: 0x00090C80
		public override void OnReleasedFromScope(IScope scope)
		{
			base.OnReleasedFromScope(scope);
			this.SetUIVisible(false, false, false, false);
			this.SetVcrButtonState(false, TimeScale.Single);
			this._inputState.Unsubscribe(this);
			this.UpgradeBar.OnReleasedFromScope(scope);
			if (this._clockView != null)
			{
				this._clockView.OnClockToggled -= this.OnClockToggled;
				this._clockView.transform.SetParent(null, false);
			}
			if (this._scoreView != null)
			{
				this._scoreView.gameObject.SetActive(true);
				this._scoreView.OnScoreButtonPressed -= this.OnScorePressed;
				this._scoreView.OnElectiveUpgradeButtonPressed -= this.OnElectiveUpgradeButtonPressed;
				scope.Release(this._scoreView);
				this._scoreView = null;
			}
			if (this.editMenuPanel != null)
			{
				scope.Release(this.editMenuPanel);
			}
			if (this.ColourWidget != null)
			{
				scope.Release(this.ColourWidget);
			}
		}

		// Token: 0x060023B2 RID: 9138 RVA: 0x00092B94 File Offset: 0x00090D94
		public override void Reset()
		{
			base.Reset();
			this._currentGraceInteractionTime = 0f;
			this._uiAppearTimer = 0f;
			this._waitToShowClock = false;
			this.TimeButtonsVisible = false;
			this._isTimeButtonVisibilityChangeScheduled = false;
			this._scheduledTimeButtonVisibility = false;
			this._currentGraceInteractionTime = 0f;
			this._clockEnabled = false;
			this._scoreEnabled = false;
			this._upgradeBarEnabled = false;
			this.UpgradeBar.gameObject.SetActive(true);
			this._drawModeVisibleState = DrawModeToggle.VisibleState.AlwaysShowing;
			this._drawButtonsHiddenByTutorial = false;
			this._showDrawButtonsNextTimeInGame = false;
			this._currentSelectedTile = null;
			this._tileHighlightsAllowed = true;
			this.FocusPointIsBlocked = false;
			this._focusPointPosition = null;
			this._focussedSelectable = null;
			this._currentDrawMode = RoadDrawMode.Add;
			this._isForceHidden = false;
			this._isWorldGridForceHidden = false;
			this._hasTransitionedIn = false;
			this._electiveUpgradeState = GameUIScreen.ElectiveUpgradeState.WaitingForNextMilestone;
		}

		// Token: 0x060023B3 RID: 9139 RVA: 0x00090074 File Offset: 0x0008E274
		public override void ScaleToCamera()
		{
			base.ScaleToGameCamera();
		}

		// Token: 0x060023B4 RID: 9140 RVA: 0x00092C6A File Offset: 0x00090E6A
		public virtual RectTransform GetRectTransform()
		{
			return this._rectTransform;
		}

		// Token: 0x060023B5 RID: 9141 RVA: 0x00092C72 File Offset: 0x00090E72
		public Selectable GetFirstUpgradeIconSelectable()
		{
			return this.UpgradeBar.GetFirstUpgradeIconSelectable();
		}

		// Token: 0x060023B6 RID: 9142 RVA: 0x00092C7F File Offset: 0x00090E7F
		public void SetRoadCursorPosition(Vector2 newCursorPosition)
		{
			this._roadCursor.Position = this.NormalizePositionToScaledScreenSize(newCursorPosition);
		}

		// Token: 0x060023B7 RID: 9143 RVA: 0x00092C93 File Offset: 0x00090E93
		public void SetRoadCursorActive(bool active)
		{
			this._roadCursor.IsVisible = active;
		}

		// Token: 0x060023B8 RID: 9144 RVA: 0x00092CA1 File Offset: 0x00090EA1
		public void SetTileHighlightsAllowed(bool allowed)
		{
			if (!allowed && this._currentSelectedTile != null)
			{
				this._currentSelectedTile.IsHighlighted = false;
				this._currentSelectedTile = null;
			}
			this._tileHighlightsAllowed = allowed;
		}

		// Token: 0x060023B9 RID: 9145 RVA: 0x00092CD0 File Offset: 0x00090ED0
		public void SetFocusPointPosition(Vector2 newFocusPointPosition)
		{
			this._focusPointPosition = new Vector2?(this.ClampPositionToScreenSize(newFocusPointPosition));
			this._focusPoint.SetCursorPosition(this.NormalizePositionToScaledScreenSize(this.FocusPointPosition));
			if (this.FocusPointIsBlocked)
			{
				return;
			}
			this.UpdateFocussedSelectable(this.FocusPointPosition);
			if (FeatureToggle.IsFeatureEnabled(Feature.TileHighlights) && (this._inputState.CurrentDeviceInputType == DeviceInputType.Controller || this._inputState.CurrentDeviceInputType == DeviceInputType.Remote) && this._tileHighlightsAllowed)
			{
				if (this._currentSelectedTile == null)
				{
					this._currentSelectedTile = this._tilemapView.GetOrCreateTileView(this._tilemapView.GetTileCoordinatesFromScreenPosition(this.FocusPointPosition));
					if (this._currentSelectedTile != null)
					{
						this._currentSelectedTile.IsHighlighted = true;
						return;
					}
				}
				else
				{
					TileView newlySelectedTile = this._tilemapView.GetOrCreateTileView(this._tilemapView.GetTileCoordinatesFromScreenPosition(this.FocusPointPosition));
					if (newlySelectedTile == null)
					{
						this._currentSelectedTile.IsHighlighted = false;
						this._currentSelectedTile = null;
						return;
					}
					if (newlySelectedTile != this._currentSelectedTile)
					{
						this._currentSelectedTile.IsHighlighted = false;
						this._currentSelectedTile = newlySelectedTile;
						this._currentSelectedTile.IsHighlighted = true;
					}
				}
			}
		}

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x060023BA RID: 9146 RVA: 0x00092E0D File Offset: 0x0009100D
		public Vector2 FocusPointPosition
		{
			get
			{
				if (this._focusPointPosition == null)
				{
					this._focusPointPosition = new Vector2?(this._gameCamera.Dimensions * 0.5f);
				}
				return this._focusPointPosition.Value;
			}
		}

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x060023BB RID: 9147 RVA: 0x00092E47 File Offset: 0x00091047
		public bool IsFocusPointActive
		{
			get
			{
				return this._focusPoint != null && this._focusPoint.IsVisible;
			}
		}

		// Token: 0x060023BC RID: 9148 RVA: 0x00092E64 File Offset: 0x00091064
		public void SetFocusPointActive(bool active, bool instantly = false)
		{
			if (this.FocusPointIsBlocked && active)
			{
				return;
			}
			this._focusPoint.SetFocusPointActive(active, instantly);
			if (!active && this._currentSelectedTile != null)
			{
				this._currentSelectedTile.IsHighlighted = false;
				this._currentSelectedTile = null;
			}
		}

		// Token: 0x060023BD RID: 9149 RVA: 0x00092EA2 File Offset: 0x000910A2
		public void SetFocusPointBlocked(bool blocked)
		{
			this.FocusPointIsBlocked = blocked;
		}

		// Token: 0x060023BE RID: 9150 RVA: 0x00092EAC File Offset: 0x000910AC
		private void UpdateFocussedSelectable(Vector2 position)
		{
			if (EventSystem.current == null)
			{
				return;
			}
			PointerEventData m_PointerEventData = new PointerEventData(EventSystem.current);
			m_PointerEventData.position = position;
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(m_PointerEventData, results);
			ISubmitHandler finalFoundSelectable = null;
			foreach (RaycastResult result in results)
			{
				ISubmitHandler newFocussedSelectable = result.gameObject.GetComponent<ISubmitHandler>();
				if (newFocussedSelectable == null)
				{
					newFocussedSelectable = result.gameObject.GetComponentInParent<ISubmitHandler>();
				}
				if (newFocussedSelectable != null)
				{
					if (this._focussedSelectable != null && newFocussedSelectable != this._focussedSelectable && typeof(IPointerExitHandler).IsAssignableFrom(this._focussedSelectable.GetType()))
					{
						((IPointerExitHandler)this._focussedSelectable).OnPointerExit(m_PointerEventData);
					}
					finalFoundSelectable = newFocussedSelectable;
					if (finalFoundSelectable != null && this._focussedSelectable != finalFoundSelectable && typeof(IPointerEnterHandler).IsAssignableFrom(finalFoundSelectable.GetType()))
					{
						((IPointerEnterHandler)finalFoundSelectable).OnPointerEnter(m_PointerEventData);
						break;
					}
					break;
				}
			}
			if (finalFoundSelectable == null && this._focussedSelectable != null && typeof(IPointerExitHandler).IsAssignableFrom(this._focussedSelectable.GetType()))
			{
				((IPointerExitHandler)this._focussedSelectable).OnPointerExit(m_PointerEventData);
				GameUIScreen.Log.Info("PointerExiting {0}", new object[]
				{
					this._focussedSelectable
				});
			}
			this._focussedSelectable = finalFoundSelectable;
		}

		// Token: 0x060023BF RID: 9151 RVA: 0x00093024 File Offset: 0x00091224
		public void OpenEditMenu(ICreativeModeEditableObject editableObject, bool confirmOrCancelEdit = false)
		{
			if (this.editMenuPanel.EditableObject != null && confirmOrCancelEdit)
			{
				this.editMenuPanel.ConfirmEdit();
			}
			this.editMenuPanel.OpenEditMenu(editableObject);
		}

		// Token: 0x060023C0 RID: 9152 RVA: 0x00093050 File Offset: 0x00091250
		public void ConfirmEditMenuEdit()
		{
			if (this.editMenuPanel != null && this.editMenuPanel.IsOpen)
			{
				this.editMenuPanel.ConfirmEdit();
			}
		}

		// Token: 0x060023C1 RID: 9153 RVA: 0x00093078 File Offset: 0x00091278
		public void SetWorldGridActive(bool active, TransitionStyle transitionStyle = TransitionStyle.Tween)
		{
			if (active && this._isWorldGridForceHidden)
			{
				return;
			}
			GameUIScreen.Log.Info("Setting world grid active: {0}, with transition: {1}", new object[]
			{
				active,
				transitionStyle
			});
			if (transitionStyle == TransitionStyle.Snap)
			{
				this._worldGridThickness.Set(active ? 1f : 0f, 0.01f);
				return;
			}
			if (active)
			{
				this._worldGridThickness.Start(this._worldGridThickness.Value, 1f, this.WorldGridTransitionInTime, Easings.Functions.SineEaseInOut, 0f);
				return;
			}
			this._worldGridThickness.Start(this._worldGridThickness.Value, 0f, this.WorldGridTransitionOutTime, Easings.Functions.SineEaseInOut, 0f);
		}

		// Token: 0x060023C2 RID: 9154 RVA: 0x00093130 File Offset: 0x00091330
		public void SetMotorwayGridActive(bool active, TransitionStyle transitionStyle = TransitionStyle.Tween)
		{
			if (active && this._isForceHidden)
			{
				return;
			}
			if (transitionStyle != TransitionStyle.Tween)
			{
				this._motorwayDotDiagonalTransition.Set(active ? 1f : 0f, 0.01f);
				return;
			}
			if (active)
			{
				this._motorwayDotDiagonalTransition.Start(this._motorwayDotDiagonalTransition.Value, 1f, this.WorldGridTransitionInTime, Easings.Functions.SineEaseInOut, 0f);
				return;
			}
			this._motorwayDotDiagonalTransition.Start(this._motorwayDotDiagonalTransition.Value, 0f, this.WorldGridTransitionInTime, Easings.Functions.SineEaseInOut, 0f);
		}

		// Token: 0x060023C3 RID: 9155 RVA: 0x000931C0 File Offset: 0x000913C0
		public void InitializeUpgradeCursor(UpgradeType upgradeButtonType)
		{
			UpgradeCursor cursor = this._gameScope.Get<UpgradeCursor>();
			Sprite cursorSprite = this.UpgradeBar.GetSpriteForUpgradeType(upgradeButtonType);
			cursor.Initialize(cursorSprite, this._rectTransform);
			this._upgradeCursor = cursor;
		}

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x060023C4 RID: 9156 RVA: 0x000931FA File Offset: 0x000913FA
		public bool HasUpgradeCursor
		{
			get
			{
				return this._upgradeCursor != null;
			}
		}

		// Token: 0x060023C5 RID: 9157 RVA: 0x00093208 File Offset: 0x00091408
		public Vector2Int GetUpgradeCursorTileCoordinates()
		{
			if (Diagnostics.Verify(this.HasUpgradeCursor))
			{
				return this._upgradeCursor.GetTileCoordinates();
			}
			return Vector2Int.zero;
		}

		// Token: 0x060023C6 RID: 9158 RVA: 0x00093228 File Offset: 0x00091428
		public void SetUpgradeCursorPosition(Vector3 position, UpgradeCursor.UpgradeCursorOffsetType offsetType)
		{
			if (Diagnostics.Verify(this.HasUpgradeCursor))
			{
				this._upgradeCursor.SetPosition(this.NormalizePositionToScaledScreenSize(position), offsetType);
			}
		}

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x060023C7 RID: 9159 RVA: 0x0009324F File Offset: 0x0009144F
		// (set) Token: 0x060023C8 RID: 9160 RVA: 0x00093258 File Offset: 0x00091458
		public bool IsUpgradeBarOnOverlay
		{
			get
			{
				return this._isUpgradeBarOnOverlay;
			}
			set
			{
				if (value != this._isUpgradeBarOnOverlay)
				{
					if (value)
					{
						this._gameCamera.AttachCameraToCanvas(this._overlayCanvas, CameraLayer.Overlay);
						this._overlayCanvas.sortingLayerID = this._canvas.sortingLayerID;
						this._overlayCanvasObject.layer = this._gameCamera.OverlayLayerIndex;
						this._overlayCanvasObject.SetActive(true);
						this._upgradeIcons.transform.SetParent(this._overlayTransform.transform, false);
					}
					else
					{
						this._upgradeIcons.transform.SetParent(this.GetUpgradeBarTransform(), false);
					}
					this._isUpgradeBarOnOverlay = value;
				}
			}
		}

		// Token: 0x060023C9 RID: 9161 RVA: 0x000932FA File Offset: 0x000914FA
		protected virtual Transform GetUpgradeBarTransform()
		{
			return this.UpgradeBar.transform;
		}

		// Token: 0x060023CA RID: 9162 RVA: 0x00093307 File Offset: 0x00091507
		public Vector2 NormalizePositionToScaledScreenSize(Vector2 position)
		{
			return position / this._gameCamera.Dimensions * this._rectTransform.sizeDelta;
		}

		// Token: 0x060023CB RID: 9163 RVA: 0x0009332C File Offset: 0x0009152C
		private Vector3 ClampPositionToScreenSize(Vector3 position)
		{
			position.x = Mathf.Clamp(position.x, 0f, this._gameCamera.Width);
			position.y = Mathf.Clamp(position.y, 0f, this._gameCamera.Height);
			return position;
		}

		// Token: 0x060023CC RID: 9164 RVA: 0x0009337E File Offset: 0x0009157E
		public void CancelUpgradeCursor()
		{
			if (Diagnostics.Verify(this.HasUpgradeCursor))
			{
				this._upgradeCursor.CancelUpgradeCursor();
				this._upgradeCursor = null;
			}
		}

		// Token: 0x060023CD RID: 9165 RVA: 0x0009339F File Offset: 0x0009159F
		public void PlaceUpgradeCursorAssetAtPosition(Vector2Int tile)
		{
			if (Diagnostics.Verify(this.HasUpgradeCursor))
			{
				this._upgradeCursor.PlaceAssetAtPosition(tile);
			}
		}

		// Token: 0x060023CE RID: 9166 RVA: 0x000933BA File Offset: 0x000915BA
		public void SetUpgradeCursorVisible(bool visible)
		{
			if (Diagnostics.Verify(this.HasUpgradeCursor))
			{
				this._upgradeCursor.gameObject.SetActive(visible);
			}
		}

		// Token: 0x060023CF RID: 9167 RVA: 0x000933DC File Offset: 0x000915DC
		public virtual TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			base.Tick(timeInterval.Delta);
			this._worldGrid.localScale = this._rectTransform.sizeDelta;
			if (this._showDrawButtonsNextTimeInGame && this._screenStack.GetTopActiveScreenType() == ScreenStack.MotorwaysScreen.InGame)
			{
				this.SetDrawButtonsVisible(true);
				this._showDrawButtonsNextTimeInGame = false;
			}
			if (this._city.Rules.ScoringMode == ScoringMode.EfficiencyMilestones)
			{
				if (this._playerActionController.BlockingPlayerActionCount > 0)
				{
					this._currentGraceInteractionTime = 3.2f;
				}
				if (this._currentGraceInteractionTime > 0f)
				{
					this._currentGraceInteractionTime -= timeInterval.Delta;
				}
				switch (this._electiveUpgradeState)
				{
				case GameUIScreen.ElectiveUpgradeState.WaitingForNextMilestone:
					if (this._upgradeDatabaseModel.IsPendingUpgradeAvailable)
					{
						this._electiveUpgradeState = GameUIScreen.ElectiveUpgradeState.UpgradeAvailable;
					}
					break;
				case GameUIScreen.ElectiveUpgradeState.UpgradeAvailable:
					this.SetElectiveUpgradeAvailable(this._scoreView.IsEfficiencyTickerVisuallyComplete);
					break;
				case GameUIScreen.ElectiveUpgradeState.RequestedUpgrade:
					this.SetElectiveUpgradeAvailable(false);
					if (!this._upgradeDatabaseModel.IsPendingUpgradeAvailable)
					{
						this._electiveUpgradeState = GameUIScreen.ElectiveUpgradeState.WaitingForNextMilestone;
					}
					break;
				}
			}
			if (this._uiAppearTimer > 0f && !this._isForceHidden)
			{
				this._uiAppearTimer -= timeInterval.Delta;
				while (this._uiAppearTimer <= 0f)
				{
					if (!this._upgradeBarEnabled)
					{
						this.SetUpgradeBarVisibility(true, false);
						this._uiAppearTimer = (this._waitToShowClock ? this.clockAppearDelay : 0f);
					}
					else
					{
						if (this._clockEnabled)
						{
							this._uiAppearTimer = -1f;
							break;
						}
						this.SetClockVisibility(true);
						AudioSystem.Instance.ScheduleEvent(AudioEvent.CreateEvent(AudioSystem.Instance.DspTime, AudioEventType.ClockStart, 0.75f, -1f, true, null));
					}
				}
			}
			if (this._isTimeButtonVisibilityChangeScheduled && !this._isForceHidden && !this.AreTimeButtonsAnimating)
			{
				this.SetTimeButtonsVisible(this._scheduledTimeButtonVisibility);
			}
			if (!this._scoreTextAnchorActive.activeSelf && this._scoreModel.Score > 0 && this._hasTransitionedIn)
			{
				this.SetScoreVisible(!this._isForceHidden);
			}
			if (this._worldGridThickness.IsActive)
			{
				float thickness = this._worldGridThickness.Tick(timeInterval.Delta);
				this._themeDatabase.materialCollection.SetWorldGridThickness(thickness);
			}
			if (this.DebugToolsHideWorldGrid)
			{
				this._worldGridGameObject.SetActive(false);
			}
			else
			{
				this._worldGridGameObject.SetActive((double)this._worldGridThickness.Value > 0.0);
			}
			if (this._motorwayDotDiagonalTransition.IsActive)
			{
				float ratio = this._motorwayDotDiagonalTransition.Tick(timeInterval.Delta);
				this._themeDatabase.materialCollection.SetMountainDotDiagonalRatio(ratio);
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x060023D0 RID: 9168 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x060023D1 RID: 9169 RVA: 0x00093688 File Offset: 0x00091888
		protected virtual void SetElectiveUpgradeAvailable(bool available)
		{
			bool isPlayerInterrupted = this._currentGraceInteractionTime >= 0f;
			this._scoreView.electiveUpgradeAnimator.SetBool(ScoreView.UpgradeAvailableId, available);
			this._scoreView.electiveUpgradeAnimator.SetBool(ScoreView.PlayerInterruptedId, isPlayerInterrupted);
		}

		// Token: 0x060023D2 RID: 9170 RVA: 0x000936D4 File Offset: 0x000918D4
		public override void OnTransitionedIn()
		{
			this._hasTransitionedIn = true;
			this._alignToCamera = true;
			this._canvasGroup.Alpha = 1f;
			this._drawModeVisibleState = this.GetDrawModeVisibleStateFromInputType(this._inputState.CurrentDeviceInputType);
			this.ScoreButton.animator.SetBool(GameUIScreen.ScoreChallengeModeAnimatorBool, this._simulation.GetModel<ActiveChallengesModel>().HasChallenges);
			if (this._city.Rules.UIStartVisible() && (!this._upgradeBarEnabled || !this._clockEnabled))
			{
				this._upgradeBarEnabled = false;
				this._clockEnabled = false;
			}
			if (this._city.Rules.UIStartVisible() && (!this._upgradeBarEnabled || !this._clockEnabled))
			{
				this._uiAppearTimer = this.upgradeBarAppearDelay;
				this.SetDrawButtonsVisible(this._drawModeVisibleState == DrawModeToggle.VisibleState.AlwaysShowing);
			}
			if (!this._isForceHidden)
			{
				this.SetClockVisibility(this._clockEnabled);
				this.SetScoreVisible(this._scoreEnabled);
				this.SetUpgradeBarVisibility(this._upgradeBarEnabled, false);
				bool showDrawModeToggle = this._drawModeVisibleState == DrawModeToggle.VisibleState.AlwaysShowing || (this._drawModeVisibleState == DrawModeToggle.VisibleState.ShowWhenFocused && this._cameraView.IsFocussedIn);
				this.SetDrawButtonsVisible(showDrawModeToggle);
			}
			this.SetMenuButtonVisible(true);
			this._canvasGroup.SetInteractable(true);
			this._canvasGroup.SetBlocksRaycasts(true);
			this.backButton.ForceInitializeState();
			BuildingsIndicatorView disconnectedBuildingsView = this._gameScope.Get<BuildingsIndicatorView>();
			if (disconnectedBuildingsView)
			{
				disconnectedBuildingsView.StartPulsing();
			}
			this._scoreView.SetEfficiencyTickerAnimationsPaused(false);
			if (this._city.Rules.ShowColourWidget)
			{
				this.ColourWidget.RefreshColours(true);
				this.ColourWidget.SetGameobjectActive(true);
				return;
			}
			this.ColourWidget.SetGameobjectActive(false);
		}

		// Token: 0x060023D3 RID: 9171 RVA: 0x00093886 File Offset: 0x00091A86
		public override void OnTransitionedOut()
		{
			base.OnTransitionedOut();
			this._menuNavigation.ClearFocus(true);
			this.backButton.ForceInitializeState();
		}

		// Token: 0x060023D4 RID: 9172 RVA: 0x000938A8 File Offset: 0x00091AA8
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			this._hasTransitionedIn = false;
			this.UpgradeBar.DeselectButtons();
			this._canvasGroup.SetInteractable(false);
			this._canvasGroup.SetBlocksRaycasts(false);
			this._transitionDetails = this._screenStack.GetTransitionDetailsFrom(base.ScreenType, inScreen);
			this._focusPoint.SetFocusPointActive(false, true);
			if (this._city.Rules.UIStartVisible() && this._uiAppearTimer > 0f && (!this._upgradeBarEnabled || !this._clockEnabled))
			{
				this._uiAppearTimer = -1f;
			}
			BuildingsIndicatorView disconnectedBuildingsView = this._gameScope.Get<BuildingsIndicatorView>();
			if (disconnectedBuildingsView)
			{
				disconnectedBuildingsView.StopPulsing();
			}
			this._scoreView.SetEfficiencyTickerAnimationsPaused(true);
			if (this._city.GameMode == GameMode.Creative)
			{
				this.ConfirmEditMenuEdit();
			}
			if (this._city.Rules.ShowColourWidget)
			{
				this.ColourWidget.SetGameobjectActive(false);
				this.ColourWidget.Reset();
			}
		}

		// Token: 0x060023D5 RID: 9173 RVA: 0x000939A1 File Offset: 0x00091BA1
		public void OnBack()
		{
			this._screenStack.PushScreen<PauseScreen>(ScreenStack.MotorwaysScreen.Pause, false, this._gameScope, true);
		}

		// Token: 0x060023D6 RID: 9174 RVA: 0x000939B8 File Offset: 0x00091BB8
		public void OnScorePressed()
		{
			ActiveChallengesModel challengeModel = this._game.Scope.Get<ActiveChallengesModel>();
			if (challengeModel.HasChallenges)
			{
				this._screenStack.PushScreen<ChallengeInfoScreen>(ScreenStack.MotorwaysScreen.ChallengeInfo, delegate(ChallengeInfoScreen screen)
				{
					screen.PrepareScreen(challengeModel.challengeType, challengeModel.challenges, challengeModel.timeStart, challengeModel.timeEnd, StringId.Continue, true, true, this._gameScope, true);
				}, false, null, true, null);
			}
		}

		// Token: 0x060023D7 RID: 9175 RVA: 0x00093A13 File Offset: 0x00091C13
		public void OnElectiveUpgradeButtonPressed()
		{
			if (this._city.Rules.ScoringMode == ScoringMode.EfficiencyMilestones && this._upgradeDatabaseModel.IsPendingUpgradeAvailable)
			{
				this._electiveUpgradeState = GameUIScreen.ElectiveUpgradeState.RequestedUpgrade;
				this._player.SetNewContentSeen("EndlessMilestoneFTUXMessage");
			}
		}

		// Token: 0x060023D8 RID: 9176 RVA: 0x00093A4C File Offset: 0x00091C4C
		public void OnClockToggled()
		{
			if (this.AreTimeButtonsAnimating)
			{
				this._isTimeButtonVisibilityChangeScheduled = true;
				this._scheduledTimeButtonVisibility = !this.TimeButtonsVisible;
			}
			else
			{
				this.SetTimeButtonsVisible(!this.TimeButtonsVisible);
			}
			IAudioSystem audioSystem = this._audioSystem;
			if (audioSystem == null)
			{
				return;
			}
			audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.Click, UIAudioProfile.Clock, -1f, this.TimeButtonsVisible, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
		}

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x060023D9 RID: 9177 RVA: 0x00093AB0 File Offset: 0x00091CB0
		private bool AreTimeButtonsAnimating
		{
			get
			{
				foreach (FloatingElement timeButtonFloater in this._timeButtonFloaters)
				{
					if (Diagnostics.Verify(timeButtonFloater != null) && timeButtonFloater.IsAnimating)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x060023DA RID: 9178 RVA: 0x00093AF0 File Offset: 0x00091CF0
		private void SetTimeButtonsVisible(bool visible)
		{
			GameObject[] timeButtonAnchors = this._timeButtonAnchors;
			for (int i = 0; i < timeButtonAnchors.Length; i++)
			{
				timeButtonAnchors[i].SetActive(visible);
			}
			this.TimeButtonsVisible = visible;
			this._isTimeButtonVisibilityChangeScheduled = false;
		}

		// Token: 0x060023DB RID: 9179 RVA: 0x00093B29 File Offset: 0x00091D29
		public void PulseClock()
		{
			this._clockView.Pulse();
		}

		// Token: 0x060023DC RID: 9180 RVA: 0x00093B36 File Offset: 0x00091D36
		public virtual void OnPausePressed()
		{
			this._game.SetPaused(true);
			this.SetVcrButtonState(true, TimeScale.Single);
		}

		// Token: 0x060023DD RID: 9181 RVA: 0x00093B50 File Offset: 0x00091D50
		public virtual void OnPlayPressed()
		{
			this._game.SetPaused(false);
			this._game.SetTimeScale(TimeScale.Single);
			this.SetVcrButtonState(false, TimeScale.Single);
		}

		// Token: 0x060023DE RID: 9182 RVA: 0x00093B7C File Offset: 0x00091D7C
		public virtual void OnFastForwardPressed()
		{
			this._game.SetPaused(false);
			if (FeatureToggle.IsFeatureDisabled(Feature.ExtraFastForward))
			{
				this._game.SetTimeScale(TimeScale.Double);
			}
			else
			{
				this._game.SetTimeScale((this._game.GetTimeScale() == TimeScale.Double) ? TimeScale.ExtraFast : TimeScale.Double);
			}
			this.SetVcrButtonState(false, this._game.GetTimeScale());
		}

		// Token: 0x060023DF RID: 9183 RVA: 0x00093BEB File Offset: 0x00091DEB
		public virtual void OnExtraFastForwardPressed()
		{
			if (FeatureToggle.IsFeatureDisabled(Feature.ExtraFastForward))
			{
				return;
			}
			this._game.SetPaused(false);
			this._game.SetTimeScale(TimeScale.ExtraFast);
			this.SetVcrButtonState(false, TimeScale.ExtraFast);
		}

		// Token: 0x060023E0 RID: 9184 RVA: 0x00093C20 File Offset: 0x00091E20
		public virtual void SetVcrButtonState(bool paused, TimeScale timeScale)
		{
			this.pauseButton.interactable = !paused;
			this.playButton.interactable = (paused || timeScale != TimeScale.Single);
			if (timeScale == TimeScale.ExtraFast || timeScale == TimeScale.Double)
			{
				this.fastForwardButton.gameObject.SetActive(timeScale == TimeScale.Double);
				this.extraFastForwardButton.gameObject.SetActive(timeScale == TimeScale.ExtraFast);
			}
			if (!FeatureToggle.IsFeatureEnabled(Feature.ExtraFastForward))
			{
				this.fastForwardButton.interactable = (paused || timeScale != TimeScale.Double);
			}
			if (this._clockView != null)
			{
				this._clockView.IsVisuallyPaused = paused;
			}
		}

		// Token: 0x060023E1 RID: 9185 RVA: 0x00093CD7 File Offset: 0x00091ED7
		public GameUIScreen.TimeScaleMode GetTimeScaleMode()
		{
			if (this._simulation.IsPaused)
			{
				return GameUIScreen.TimeScaleMode.Paused;
			}
			return this.GetUnpausedTimeScaleMode();
		}

		// Token: 0x060023E2 RID: 9186 RVA: 0x00093CEE File Offset: 0x00091EEE
		public GameUIScreen.TimeScaleMode GetUnpausedTimeScaleMode()
		{
			if (this._game.GetTimeScale() == TimeScale.Single)
			{
				return GameUIScreen.TimeScaleMode.Play;
			}
			if (this._game.GetTimeScale() == TimeScale.Double)
			{
				return GameUIScreen.TimeScaleMode.FastForward;
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.ExtraFastForward))
			{
				return GameUIScreen.TimeScaleMode.ExtraFastForward;
			}
			return GameUIScreen.TimeScaleMode.FastForward;
		}

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x060023E3 RID: 9187 RVA: 0x00093D24 File Offset: 0x00091F24
		public bool IsClockVisible
		{
			get
			{
				return this._clockEnabled;
			}
		}

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x060023E4 RID: 9188 RVA: 0x00093D2C File Offset: 0x00091F2C
		public bool IsScoreVisible
		{
			get
			{
				return this._scoreEnabled;
			}
		}

		// Token: 0x060023E5 RID: 9189 RVA: 0x00093D34 File Offset: 0x00091F34
		public virtual void SetClockVisibility(bool visible)
		{
			this._clockEnabled = (this._clockEnabled || visible);
			this._clockAnchorActive.SetActive(visible);
			this._dayTextAnchorActive.SetActive(visible);
		}

		// Token: 0x060023E6 RID: 9190 RVA: 0x00093D5C File Offset: 0x00091F5C
		public virtual void SetScoreVisible(bool visible)
		{
			this._scoreEnabled = (this._scoreEnabled || visible);
			this._scoreTextAnchorActive.SetActive(visible);
		}

		// Token: 0x060023E7 RID: 9191 RVA: 0x00093D78 File Offset: 0x00091F78
		public virtual void SetMenuButtonVisible(bool visible)
		{
			this.menuButtonAnchor.SetActive(visible);
		}

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x060023E8 RID: 9192 RVA: 0x00093D86 File Offset: 0x00091F86
		// (set) Token: 0x060023E9 RID: 9193 RVA: 0x00093D8E File Offset: 0x00091F8E
		public bool DebugToolsHideUI
		{
			get
			{
				return this._debugToolsHideUI;
			}
			set
			{
				this._debugToolsHideUI = value;
				this._canvasGroup.Alpha = (this._debugToolsHideUI ? 0f : 1f);
			}
		}

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x060023EA RID: 9194 RVA: 0x00093DB6 File Offset: 0x00091FB6
		// (set) Token: 0x060023EB RID: 9195 RVA: 0x00093DBE File Offset: 0x00091FBE
		public bool DebugToolsHideWorldGrid { get; set; }

		// Token: 0x060023EC RID: 9196 RVA: 0x00093DC8 File Offset: 0x00091FC8
		public virtual void SetUIVisible(bool visible, bool instantly = false, bool forceHide = false, bool forceHideWorldGrid = false)
		{
			this._isForceHidden = (forceHide && !visible);
			this._isWorldGridForceHidden = forceHideWorldGrid;
			this._uiVisible = visible;
			this.SetClockVisibility(visible);
			this.SetScoreVisible(visible);
			this.SetUpgradeBarVisibility(visible, instantly || !visible);
			this.UpgradeBar.SetCreativeModeColourWidgetVisible(visible);
			this.SetMenuButtonVisible(visible);
			if (!visible)
			{
				this.SetTimeButtonsVisible(false);
				this.SetDrawButtonsVisible(false);
				if (forceHideWorldGrid)
				{
					this.SetWorldGridActive(false, instantly ? TransitionStyle.Snap : TransitionStyle.Tween);
				}
				this._tilemapView.viewMode = TilemapView.ViewMode.Normal;
			}
			if (instantly)
			{
				List<FloatingElement> floatingElements = new List<FloatingElement>(base.gameObject.GetComponentsInChildren<FloatingElement>());
				foreach (FloatingElement clockFloater in this._clockView.GetComponentsInChildren<FloatingElement>())
				{
					floatingElements.Remove(clockFloater);
					floatingElements.Insert(0, clockFloater);
				}
				foreach (FloatingElement floatingElement in floatingElements)
				{
					floatingElement.Snap();
				}
			}
		}

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x060023ED RID: 9197 RVA: 0x00093ED8 File Offset: 0x000920D8
		public bool IsUiVisible
		{
			get
			{
				return this._uiVisible;
			}
		}

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x060023EE RID: 9198 RVA: 0x00093EE0 File Offset: 0x000920E0
		public bool IsForceHidden
		{
			get
			{
				return this._isForceHidden;
			}
		}

		// Token: 0x060023EF RID: 9199 RVA: 0x00093EE8 File Offset: 0x000920E8
		public void ResetForceHiddenState()
		{
			this._isForceHidden = false;
			this._isWorldGridForceHidden = false;
		}

		// Token: 0x060023F0 RID: 9200 RVA: 0x00093EF8 File Offset: 0x000920F8
		public void SetUpgradeBarVisibility(bool visible, bool instantly = false)
		{
			this._upgradeBarEnabled = (this._upgradeBarEnabled || visible);
			this.UpgradeBar.SetVisibility(visible, instantly);
		}

		// Token: 0x060023F1 RID: 9201 RVA: 0x00093F15 File Offset: 0x00092115
		public void SetDrawButtonsHiddenByTutorial(bool hidden)
		{
			this._drawButtonsHiddenByTutorial = hidden;
		}

		// Token: 0x060023F2 RID: 9202 RVA: 0x00093F1E File Offset: 0x0009211E
		public virtual void SetDrawButtonsVisible(bool visible)
		{
			this.drawButtonAnchors.SetActive(!this._drawButtonsHiddenByTutorial && visible);
			this.drawModeToggle.touchButton.interactable = visible;
		}

		// Token: 0x060023F3 RID: 9203 RVA: 0x00093F48 File Offset: 0x00092148
		public override void InitScreen(IScope gameScope, bool blocksGameInput)
		{
			base.InitScreen(gameScope, blocksGameInput);
			ClockModel clockModel = gameScope.Get<ISimulation>().GetModel<ClockModel>();
			this._clockView.Initialize(clockModel, this._clockAnchorActive, this._clockAnchorInactive, this._dayTextAnchorActive, this._dayTextAnchorInactive, this._scoreTextAnchorActive, this._scoreTextAnchorInactive);
			gameScope.Get<ViewClient>().AddView(this._clockView);
			this.pauseButton.GetComponent<FloatingElement>().SetInactiveAnchor(this._clockView.VcrInactiveAnchor);
			this.playButton.GetComponent<FloatingElement>().SetInactiveAnchor(this._clockView.VcrInactiveAnchor);
			this.fastForwardButton.GetComponent<FloatingElement>().SetInactiveAnchor(this._clockView.VcrInactiveAnchor);
			this.extraFastForwardButton.GetComponent<FloatingElement>().SetInactiveAnchor(this._clockView.VcrInactiveAnchor);
			this._canvasGroup.Alpha = 0f;
			this._canvasGroup.SetInteractable(true);
			this._canvasGroup.SetBlocksRaycasts(true);
			this._worldGrid.localScale = this._rectTransform.sizeDelta;
			this._waitToShowClock = (this._game.StartReason == GameStartReason.New);
		}

		// Token: 0x060023F4 RID: 9204 RVA: 0x00094070 File Offset: 0x00092270
		public override void OnCurrentDeviceInputTypeChanged(DeviceInputType newInputType)
		{
			base.OnCurrentDeviceInputTypeChanged(newInputType);
			if (newInputType != DeviceInputType.Touch)
			{
				this.SetWorldGridActive(false, TransitionStyle.Tween);
				this._tilemapView.viewMode = TilemapView.ViewMode.Normal;
				if (this._cameraView.IsFocussedIn && this.CurrentRoadDrawMode == RoadDrawMode.Remove)
				{
					this.CurrentRoadDrawMode = RoadDrawMode.Add;
				}
			}
			this._drawModeVisibleState = this.GetDrawModeVisibleStateFromInputType(newInputType);
			if (this._drawModeVisibleState == DrawModeToggle.VisibleState.NeverShow)
			{
				this.SetDrawButtonsVisible(false);
			}
			this._showDrawButtonsNextTimeInGame = (this._drawModeVisibleState == DrawModeToggle.VisibleState.AlwaysShowing);
			if (newInputType != DeviceInputType.Controller && this._currentSelectedTile != null)
			{
				this._currentSelectedTile.IsHighlighted = false;
				this._currentSelectedTile = null;
			}
		}

		// Token: 0x060023F5 RID: 9205 RVA: 0x00094109 File Offset: 0x00092309
		private DrawModeToggle.VisibleState GetDrawModeVisibleStateFromInputType(DeviceInputType inputType)
		{
			if (inputType == DeviceInputType.Touch)
			{
				return DrawModeToggle.VisibleState.ShowWhenFocused;
			}
			if (this._player.IsDrawModeToggleEnabled)
			{
				return DrawModeToggle.VisibleState.AlwaysShowing;
			}
			if (inputType == DeviceInputType.Remote)
			{
				return DrawModeToggle.VisibleState.AlwaysShowing;
			}
			return DrawModeToggle.VisibleState.NeverShow;
		}

		// Token: 0x060023F6 RID: 9206 RVA: 0x00094128 File Offset: 0x00092328
		public void ExitEditModeUI()
		{
			this._cameraView.ResetPlayerViewport();
			this.SetWorldGridActive(false, TransitionStyle.Tween);
			this.SetDrawButtonsVisible(false);
			this.SetRoadCursorActive(false);
			if (this.CurrentRoadDrawMode == RoadDrawMode.Remove)
			{
				this.ToggleDrawMode();
			}
			if (this.IsFocusPointActive)
			{
				this.SetFocusPointActive(false, false);
			}
		}

		// Token: 0x04001DA8 RID: 7592
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("View.GameUI");

		// Token: 0x04001DA9 RID: 7593
		[Dependency]
		private City _city;

		// Token: 0x04001DAA RID: 7594
		[Dependency]
		private TilemapView _tilemapView;

		// Token: 0x04001DAB RID: 7595
		[Dependency]
		private ScoreModel _scoreModel;

		// Token: 0x04001DAC RID: 7596
		[Dependency]
		private UpgradeDatabaseModel _upgradeDatabaseModel;

		// Token: 0x04001DAD RID: 7597
		[Dependency]
		private MenuNavigation _menuNavigation;

		// Token: 0x04001DAE RID: 7598
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04001DAF RID: 7599
		[SerializeField]
		private RoadCursor _roadCursor;

		// Token: 0x04001DB0 RID: 7600
		private UpgradeCursor _upgradeCursor;

		// Token: 0x04001DB1 RID: 7601
		[SerializeField]
		private FocusPoint _focusPoint;

		// Token: 0x04001DB2 RID: 7602
		private Vector2? _focusPointPosition;

		// Token: 0x04001DB3 RID: 7603
		[SerializeField]
		protected Transform _worldGrid;

		// Token: 0x04001DB4 RID: 7604
		[SerializeField]
		private Transform _clockViewParent;

		// Token: 0x04001DB5 RID: 7605
		private GameObject _worldGridGameObject;

		// Token: 0x04001DB6 RID: 7606
		private TweenFloat _worldGridThickness = new TweenFloat();

		// Token: 0x04001DB7 RID: 7607
		private TweenFloat _motorwayDotDiagonalTransition = new TweenFloat();

		// Token: 0x04001DB8 RID: 7608
		public SafeArea safeArea;

		// Token: 0x04001DB9 RID: 7609
		[SerializeField]
		public RectTransform playableArea;

		// Token: 0x04001DBB RID: 7611
		private ClockView _clockView;

		// Token: 0x04001DBC RID: 7612
		private ScoreView _scoreView;

		// Token: 0x04001DBD RID: 7613
		[SerializeField]
		private GameObject _clockAnchorActive;

		// Token: 0x04001DBE RID: 7614
		[SerializeField]
		private Transform _clockAnchorInactive;

		// Token: 0x04001DBF RID: 7615
		[SerializeField]
		private GameObject _scoreTextAnchorActive;

		// Token: 0x04001DC0 RID: 7616
		[SerializeField]
		private Transform _scoreTextAnchorInactive;

		// Token: 0x04001DC1 RID: 7617
		[SerializeField]
		private GameObject _dayTextAnchorActive;

		// Token: 0x04001DC2 RID: 7618
		[SerializeField]
		private Transform _dayTextAnchorInactive;

		// Token: 0x04001DC3 RID: 7619
		public GameObject menuButtonAnchor;

		// Token: 0x04001DC4 RID: 7620
		public GameObject drawButtonAnchors;

		// Token: 0x04001DC5 RID: 7621
		public DrawModeToggle drawModeToggle;

		// Token: 0x04001DC6 RID: 7622
		public TouchButton pauseButton;

		// Token: 0x04001DC7 RID: 7623
		public TouchButton playButton;

		// Token: 0x04001DC8 RID: 7624
		public TouchButton fastForwardButton;

		// Token: 0x04001DC9 RID: 7625
		public TouchButton extraFastForwardButton;

		// Token: 0x04001DCA RID: 7626
		[SerializeField]
		private GameObject[] _timeButtonAnchors;

		// Token: 0x04001DCB RID: 7627
		private FloatingElement[] _timeButtonFloaters = new FloatingElement[3];

		// Token: 0x04001DCC RID: 7628
		[SerializeField]
		private GameObject _upgradeIcons;

		// Token: 0x04001DCD RID: 7629
		[Tooltip("The amount of time after a game starts until the upgrade bar transitions in")]
		public float upgradeBarAppearDelay = 1f;

		// Token: 0x04001DCE RID: 7630
		[Tooltip("The amount of time after a game starts until the clock transitions in")]
		public float clockAppearDelay = 5f;

		// Token: 0x04001DCF RID: 7631
		[SerializeField]
		private float WorldGridTransitionInTime = 0.2f;

		// Token: 0x04001DD0 RID: 7632
		[SerializeField]
		private float WorldGridTransitionOutTime = 0.2f;

		// Token: 0x04001DD3 RID: 7635
		private const float GRACE_INTERACTION_TIME = 3.2f;

		// Token: 0x04001DD4 RID: 7636
		private float _currentGraceInteractionTime;

		// Token: 0x04001DD5 RID: 7637
		public static readonly int ScoreChallengeModeAnimatorBool = Animator.StringToHash("ChallengeMode");

		// Token: 0x04001DD6 RID: 7638
		public static readonly int ScorePulseAnimatorTrigger = Animator.StringToHash("Pulse");

		// Token: 0x04001DD7 RID: 7639
		private float _uiAppearTimer;

		// Token: 0x04001DD8 RID: 7640
		private bool _waitToShowClock;

		// Token: 0x04001DDA RID: 7642
		private bool _isTimeButtonVisibilityChangeScheduled;

		// Token: 0x04001DDB RID: 7643
		private bool _scheduledTimeButtonVisibility;

		// Token: 0x04001DDC RID: 7644
		private bool _clockEnabled;

		// Token: 0x04001DDD RID: 7645
		private bool _scoreEnabled;

		// Token: 0x04001DDE RID: 7646
		private bool _upgradeBarEnabled;

		// Token: 0x04001DDF RID: 7647
		private DrawModeToggle.VisibleState _drawModeVisibleState;

		// Token: 0x04001DE0 RID: 7648
		private bool _showDrawButtonsNextTimeInGame;

		// Token: 0x04001DE1 RID: 7649
		private TileView _currentSelectedTile;

		// Token: 0x04001DE2 RID: 7650
		private bool _tileHighlightsAllowed = true;

		// Token: 0x04001DE3 RID: 7651
		private bool _drawButtonsHiddenByTutorial;

		// Token: 0x04001DE4 RID: 7652
		private GameObject _overlayCanvasObject;

		// Token: 0x04001DE5 RID: 7653
		private Canvas _overlayCanvas;

		// Token: 0x04001DE6 RID: 7654
		private RectTransform _overlayTransform;

		// Token: 0x04001DE7 RID: 7655
		private bool _isUpgradeBarOnOverlay;

		// Token: 0x04001DE8 RID: 7656
		private bool _uiVisible;

		// Token: 0x04001DEA RID: 7658
		private ISubmitHandler _focussedSelectable;

		// Token: 0x04001DEB RID: 7659
		private RoadDrawMode _currentDrawMode;

		// Token: 0x04001DEC RID: 7660
		private bool _isForceHidden;

		// Token: 0x04001DED RID: 7661
		private bool _isWorldGridForceHidden;

		// Token: 0x04001DEE RID: 7662
		private bool _hasTransitionedIn;

		// Token: 0x04001DEF RID: 7663
		private GameUIScreen.ElectiveUpgradeState _electiveUpgradeState;

		// Token: 0x04001DF0 RID: 7664
		private bool _debugToolsHideUI;

		// Token: 0x02000541 RID: 1345
		public enum TimeScaleMode
		{
			// Token: 0x04001DF3 RID: 7667
			Paused,
			// Token: 0x04001DF4 RID: 7668
			Play,
			// Token: 0x04001DF5 RID: 7669
			FastForward,
			// Token: 0x04001DF6 RID: 7670
			ExtraFastForward
		}

		// Token: 0x02000542 RID: 1346
		private enum ElectiveUpgradeState
		{
			// Token: 0x04001DF8 RID: 7672
			WaitingForNextMilestone,
			// Token: 0x04001DF9 RID: 7673
			UpgradeAvailable,
			// Token: 0x04001DFA RID: 7674
			RequestedUpgrade
		}
	}
}
