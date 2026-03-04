using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Motorways.Actions {
	using Views;

	public class InteractionController : MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private Camera _mainCamera;

		//클릭 후 드래그로 인정받기 위한 최소 거리 (픽셀 단위 아님, 월드 단위) = 클릭 노이즈를 방지하는 Deadzone
		[SerializeField, Range(0.1f, 1.0f)] private float _initialDragDeadzone = 0.3f;

		//타일 중앙에서 마우스가 얼마나 떨어져야 건설이 결정되는가?
		[SerializeField, Range(0.5f, 2.0f)] private float _connectionDistanceThreshold = 0.8f;

		private PlayerInput _input;
		private Plane _groundPlane;
		private Vector2 _mouseScreenPos;

		//마우스가 가리키고 있는 그리드 좌표
		public Vector2Int CurrentGridPointer { get; private set; }
		public bool IsPointerValid { get; private set; }

		private MotorwaysPlayerAction _currentAction = null;

		#region Getters
		public float InitialDragDeadzone => _initialDragDeadzone;
		public float ConnectionDistanceThreshold => _connectionDistanceThreshold;
		public bool IsDragging => _currentAction != null;
		#endregion

		private void Awake() {
			_input = new PlayerInput();
			_groundPlane = new Plane(Vector3.up, Vector3.zero);
			if (_mainCamera == null) _mainCamera = Camera.main;
		}

		private void OnEnable() {
			_input.Enable();
			_input.Player.CursorPosition.performed += OnCursorMove;
			_input.Player.Build.started += OnBuildStarted;   //건설 시작
			_input.Player.Build.canceled += OnBuildCanceled; //건설 끝
			_input.Player.Remove.started += OnRemoveStarted;
			_input.Player.Remove.canceled += OnRemoveCanceled;
		}

		private void OnDisable() {
			_input.Disable();
			_input.Player.CursorPosition.performed -= OnCursorMove;
			_input.Player.Build.started -= OnBuildStarted;
			_input.Player.Build.canceled -= OnBuildCanceled;
			_input.Player.Remove.started -= OnRemoveStarted;
			_input.Player.Remove.canceled -= OnRemoveCanceled;
		}

		private void Update() {
			if (_currentAction != null) {
				_currentAction.Tick(Time.deltaTime);
				if (_currentAction.IsComplete) _currentAction = null;
			}
		}

		//---------- 건설(좌클릭)
		private void OnBuildStarted(InputAction.CallbackContext context) {
			if (_currentAction != null || !IsPointerValid) return;
			
			_currentAction = new DrawRoadAction();
			_currentAction.Initialize(this);
			_currentAction.OnActionBegin(Time.time);

			//그리드 표시
			if (GridView.Instance != null) GridView.Instance.SetVisible(true, false);
		}

		private void OnBuildCanceled(InputAction.CallbackContext context) {
			if (_currentAction is DrawRoadAction) {
				_currentAction.OnActionComplete();
				//그리드 숨김
				if (GridView.Instance != null) GridView.Instance.SetVisible(false);
			}
		}

		//---------- 삭제(우클릭)
		private void OnRemoveStarted(InputAction.CallbackContext context) {
			if (_currentAction != null || !IsPointerValid) return;

			_currentAction = new RemoveRoadAction();
			_currentAction.Initialize(this);
			_currentAction.OnActionBegin(Time.time);

			//그리드 표시
			if (GridView.Instance != null) GridView.Instance.SetVisible(true, true);	//삭제라서 뒤 매개변수 true
		}

		private void OnRemoveCanceled(InputAction.CallbackContext context) {
			if (_currentAction is RemoveRoadAction) {
				_currentAction.OnActionComplete();
				//그리드 숨김
				if (GridView.Instance != null) GridView.Instance.SetVisible(false);
			}
		}

		//---------- 이동(마우스 이동)
		private void OnCursorMove(InputAction.CallbackContext context) {
			_mouseScreenPos = context.ReadValue<Vector2>();

			//그리드 좌표 갱신
			UpdateGridPointer();
		}

		private void UpdateGridPointer() {
			//마우스 좌표(그리드상) 계산
			Vector3 hitPoint = GetWorldPositionFromMouse();
			if (IsPointerValid) {
				int x = Mathf.FloorToInt(hitPoint.x);
				int y = Mathf.FloorToInt(hitPoint.z);
				Vector2Int coord = new Vector2Int(x, y);

				//맵 범위 내에 있는지 체크
				if (MapManager.Instance.IsInPlayableArea(coord)) {
					CurrentGridPointer = coord;
				} else {
					IsPointerValid = false;
				}
			}
		}

		public Vector3 GetWorldPositionFromMouse() {
			Ray ray = _mainCamera.ScreenPointToRay(_mouseScreenPos);
			if (_groundPlane.Raycast(ray, out float enter)) {
				//바닥면 히트!! (인식된 지점 반환)
				IsPointerValid = true;
				return ray.GetPoint(enter); //좌표 저장하고 반환.
			}
			//바닥없으면 그냥 0반환 (불가)
			IsPointerValid = false;
			return Vector3.zero;
		}

		//마우스가 이동한 좌표 (클릭->드래그) 기반으로 방향을 구하는것. 정규화 후 8방향으로 스냅핑합니다.
		public Vector2Int CalculateSnappedDirection(Vector3 diff) {
			//각도 계산 (Atan2로 각도 변환)
			//z를 y축으로 사용해서 계산
			float angle = Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg;

			//양수 각도 고정 (0 ~ 360)
			if (angle < 0) angle += 360f;

			//8방향 섹터 (45도씩 분할 -> 22.5도 오프셋으로 반올림 처리)
			//0:East, 1:NE, 2:North, 3:NW, 4:West, 5:SW, 6:South, 7:SE
			int sector = Mathf.RoundToInt(angle / 45f) % 8;

			switch (sector) {
				case 0: return new Vector2Int(1, 0);   //East
				case 1: return new Vector2Int(1, 1);   //NE
				case 2: return new Vector2Int(0, 1);   //North
				case 3: return new Vector2Int(-1, 1);  //NW
				case 4: return new Vector2Int(-1, 0);  //West
				case 5: return new Vector2Int(-1, -1); //SW
				case 6: return new Vector2Int(0, -1);  //South
				case 7: return new Vector2Int(1, -1);  //SE
			}
			return Vector2Int.zero;
		}
	}
}
