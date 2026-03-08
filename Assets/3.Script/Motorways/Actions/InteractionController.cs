using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Motorways.Actions {
	using Managers;
	using Views;

	public class InteractionController : MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private Camera _mainCamera;
		public Camera MainCamera => _mainCamera;

		//클릭 후 드래그로 인정받기 위한 최소 거리 (픽셀 단위 아님, 월드 단위) = 클릭 노이즈를 방지하는 Deadzone
		[SerializeField, Range(0.1f, 1.0f)] private float _initialDragDeadzone = 0.3f;

		//타일 중앙에서 마우스가 얼마나 떨어져야 건설이 결정되는가?
		[SerializeField, Range(0.5f, 2.0f)] private float _connectionDistanceThreshold = 0.8f;

		private PlayerInput _input;
		private Plane _groundPlane;
		private Vector2 _mouseScreenPos;
		public Vector2 MouseScreenPos => _mouseScreenPos;

		//마우스가 가리키고 있는 그리드 좌표
		public Vector2Int CurrentGridPointer { get; private set; }
		public bool IsPointerValid { get; private set; }

		private MotorwaysPlayerAction _currentAction = null;

		#region Getters
		public float InitialDragDeadzone => _initialDragDeadzone * MapSettings.TILE_SIZE;
		public float ConnectionDistanceThreshold => _connectionDistanceThreshold * MapSettings.TILE_SIZE;
		public bool IsDragging => _currentAction != null;
		public bool IsBuildingRoad => _currentAction is DrawRoadAction;
		#endregion

		public static InteractionController Instance { get; private set; }

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);

			_input = new PlayerInput();
			_groundPlane = new Plane(Vector3.up, Vector3.zero);
			if (_mainCamera == null) _mainCamera = Camera.main;
		}

		private void OnEnable() {
			_input.Enable();
			_input.Player.CursorPosition.performed += OnCursorMove;
			_input.Player.Build.started += OnBuildStarted;
			_input.Player.Build.canceled += OnBuildCanceled;
			_input.Player.Remove.started += OnRemoveStarted;
			_input.Player.Remove.canceled += OnRemoveCanceled;

			_input.Player.Pause.started += OnTimePause;
			_input.Player.ESC.started += OnMenuPause;

			_input.Player.Zoom.performed += OnZoom;
			_input.Player.ZoomMove.started += OnPanStarted;
			_input.Player.ZoomMove.canceled += OnPanCanceled;
		}

		private void OnDisable() {
			_input.Disable();
			_input.Player.CursorPosition.performed -= OnCursorMove;
			_input.Player.Build.started -= OnBuildStarted;
			_input.Player.Build.canceled -= OnBuildCanceled;
			_input.Player.Remove.started -= OnRemoveStarted;
			_input.Player.Remove.canceled -= OnRemoveCanceled;

			_input.Player.Pause.started -= OnTimePause;
			_input.Player.ESC.started -= OnMenuPause;

			_input.Player.Zoom.performed -= OnZoom;
			_input.Player.ZoomMove.started -= OnPanStarted;
			_input.Player.ZoomMove.canceled -= OnPanCanceled;
		}

		private void Update() {
			if (_currentAction != null) {
				_currentAction.Tick(Time.deltaTime);
				if (_currentAction.IsComplete) _currentAction = null;
			}
		}

		public Vector3 GetMapCenter() {
			if (MapManager.Instance == null) return Vector3.zero;
			RectInt area = MapManager.Instance.PlayableArea;
			float centerX = (area.xMin + area.xMax) * MapSettings.TILE_SIZE * 0.5f;
			float centerZ = (area.yMin + area.yMax) * MapSettings.TILE_SIZE * 0.5f;
			float currentY = (_mainCamera != null) ? _mainCamera.transform.position.y : 10f;
			return new Vector3(centerX, currentY, centerZ);
		}

		//---------- 시간 일시정지 (스페이스바)
		private void OnTimePause(InputAction.CallbackContext context) {
			// [사용자 요청] 메뉴가 열려있을 때는 스페이스바 무시
			if (GameMenuManager.Instance != null && GameMenuManager.Instance.IsMenuOpen) return;

			TimePauseAction pauseAction = new TimePauseAction();
			pauseAction.Initialize(this);
			pauseAction.OnActionBegin(Time.time);
		}

		//---------- 게임 메뉴 일시정지 (ESC)
		private void OnMenuPause(InputAction.CallbackContext context) {
			GamePauseAction menuAction = new GamePauseAction();
			menuAction.Initialize(this);
			menuAction.OnActionBegin(Time.time);
		}

		private void OnZoom(InputAction.CallbackContext context) {
			CameraZoomAction zoomAction = new CameraZoomAction();
			zoomAction.Initialize(this);
			zoomAction.SetScrollDelta(context.ReadValue<Vector2>().y);
			zoomAction.OnActionBegin(Time.time);
		}

		private void OnPanStarted(InputAction.CallbackContext context) {
			if (_currentAction != null) return;
			_currentAction = new CameraPanAction();
			_currentAction.Initialize(this);
			_currentAction.OnActionBegin(Time.time);
		}

		private void OnPanCanceled(InputAction.CallbackContext context) {
			if (_currentAction is CameraPanAction) _currentAction.OnActionComplete();
		}

		private void OnBuildStarted(InputAction.CallbackContext context) {
			if (IsPointerOverUI()) return;
			if (GridView.Instance != null) GridView.Instance.SetVisible(true, false);
			if (_currentAction != null || !IsPointerValid) return;
			_currentAction = new DrawRoadAction();
			_currentAction.Initialize(this);
			_currentAction.OnActionBegin(Time.time);
		}

		private void OnBuildCanceled(InputAction.CallbackContext context) {
			if (_currentAction is DrawRoadAction) _currentAction.OnActionComplete();
			if (GridView.Instance != null && _mainCamera.orthographicSize >= 14.9f) GridView.Instance.SetVisible(false);
			if (RoadNetworkManager.Instance != null) RoadNetworkManager.Instance.ValidateAllBridges();
		}

		private void OnRemoveStarted(InputAction.CallbackContext context) {
			if (IsPointerOverUI()) return;
			if (GridView.Instance != null) GridView.Instance.SetVisible(true, true);
			if (_currentAction != null || !IsPointerValid) return;
			_currentAction = new RemoveRoadAction();
			_currentAction.Initialize(this);
			_currentAction.OnActionBegin(Time.time);
		}

		private void OnRemoveCanceled(InputAction.CallbackContext context) {
			if (_currentAction is RemoveRoadAction) _currentAction.OnActionComplete();
			if (GridView.Instance != null) GridView.Instance.SetVisible(_mainCamera.orthographicSize < 14.9f, false);
			if (RoadNetworkManager.Instance != null) RoadNetworkManager.Instance.ValidateAllBridges();
		}

		private void OnCursorMove(InputAction.CallbackContext context) {
			_mouseScreenPos = context.ReadValue<Vector2>();
			UpdateGridPointer();
		}

		private void UpdateGridPointer() {
			Vector3 hitPoint = GetWorldPositionFromMouse();
			if (IsPointerValid) {
				int x = Mathf.FloorToInt(hitPoint.x / MapSettings.TILE_SIZE);
				int y = Mathf.FloorToInt(hitPoint.z / MapSettings.TILE_SIZE);
				Vector2Int coord = new Vector2Int(x, y);
				if (MapManager.Instance.IsInPlayableArea(coord)) CurrentGridPointer = coord;
				else IsPointerValid = false;
			}
		}

		public Vector3 GetWorldPositionFromMouse() {
			Ray ray = _mainCamera.ScreenPointToRay(_mouseScreenPos);
			if (_groundPlane.Raycast(ray, out float enter)) {
				IsPointerValid = true;
				return ray.GetPoint(enter);
			}
			IsPointerValid = false;
			return Vector3.zero;
		}

		public Vector2Int CalculateSnappedDirection(Vector3 diff) {
			float max = Mathf.Max(Mathf.Abs(diff.x), Mathf.Abs(diff.z));
			if (max < 0.05f) return Vector2Int.zero;
			return new Vector2Int(Mathf.RoundToInt(diff.x / max), Mathf.RoundToInt(diff.z / max));
		}

		private bool IsPointerOverUI() {
			if (EventSystem.current == null) return false;
			PointerEventData eventData = new PointerEventData(EventSystem.current) { position = _mouseScreenPos };
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventData, results);
			return results.Count > 0;
		}
	}
}
