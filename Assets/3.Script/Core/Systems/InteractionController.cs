using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Systems {
	//using Core.Data;

	public class InteractionController : MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private Camera _mainCamera;

		// [New] 클릭 후 드래그로 인정받기 위한 최소 거리 (픽셀 단위 아님, 월드 단위)
		// 클릭 미스를 방지하는 Deadzone
		[SerializeField, Range(0.1f, 1.0f)] private float _initialDragDeadzone = 0.3f;

		// [New] 타일 중앙에서 마우스가 얼마나 멀어져야 도로가 연결되는가?
		// 1.0f = 타일 한 칸 거리. 1.25f 정도가 쫀득함.
		[SerializeField, Range(0.5f, 2.0f)] private float _connectionDistanceThreshold = 0.8f;

		private PlayerInput _input;
		private Vector2 _mouseScreenPos;
		private Plane _groundPlane;

		//마우스가 가리키고 있는 그리드 좌표
		public Vector2Int CurrentGridPointer { get; private set; }
		public bool IsPointerValid { get; private set; }

		private bool _isDraggingBuild = false;
		private bool _isDraggingRemove = false;

		private Vector2Int _lastGridPointer;    // 마지막으로 도로가 깔린 타일 좌표
		private Vector2Int _startDragPointer;   // 드래그 시작 타일 좌표
		private Vector3 _clickOriginWorldPos;   // [New] 최초 클릭한 월드 좌표 (Deadzone 체크용)
		private bool _hasPassedDeadzone = false; // [New] 데드존을 넘었는지 여부

		//-----------------------------------------------
		private void Awake() {
			_input = new PlayerInput();
			_groundPlane = new Plane(Vector3.up, Vector3.zero);
			if (_mainCamera == null) _mainCamera = Camera.main;
		}

		private void OnEnable() {
			_input.Enable();
			_input.Player.CursorPosition.performed += OnCursorMove;
			_input.Player.Build.started += OnBuildStarted;   // 눌렀을 때
			_input.Player.Build.canceled += OnBuildCanceled; // 뗐을 때
			_input.Player.Remove.started += OnRemoveStarted;
			_input.Player.Remove.canceled += OnRemoveCanceled;
		}
		private void OnDisable() {
			_input.Disable();
			_input.Player.CursorPosition.performed -= OnCursorMove;
			_input.Player.Build.started += OnBuildStarted;   // 눌렀을 때
			_input.Player.Build.canceled += OnBuildCanceled; // 뗐을 때
			_input.Player.Remove.started -= OnRemoveStarted;
			_input.Player.Remove.canceled -= OnRemoveCanceled;
		}


		//----------건설(좌클릭)
		private void OnBuildStarted(InputAction.CallbackContext context) {
			if (_isDraggingRemove) return;
			if (!IsPointerValid) return;

			if (!RoadSystem.Instance.IsRoadBuildable(CurrentGridPointer)) return;

			_isDraggingBuild = true;
			_hasPassedDeadzone = false;

			_lastGridPointer = CurrentGridPointer; // 현재 위치 기억
			_startDragPointer = CurrentGridPointer; // 시작점 기억
			_clickOriginWorldPos = GetWorldPositionFromMouse();

			RoadSystem.Instance.CreateRoadNode(CurrentGridPointer);
		}
		private void OnBuildCanceled(InputAction.CallbackContext context) {
			if(_isDraggingBuild) {
				_isDraggingBuild = false;

				//도로 한개면... 삭제해야지?
				RoadSystem.Instance.CleanupifIsolated(_startDragPointer);
				//만약에 말야... 우리.. 에러날수도 있으니 점검용
				if(_startDragPointer != _lastGridPointer) {
					RoadSystem.Instance.CleanupifIsolated(_lastGridPointer);
				}
			}
		}

		//----------삭제(우클릭)
		private void OnRemoveStarted(InputAction.CallbackContext context) {
			if (_isDraggingBuild) return;
			_isDraggingRemove = true;

			if(IsPointerValid) {
				RoadSystem.Instance.RemoveRoad(CurrentGridPointer);
				_lastGridPointer = CurrentGridPointer;
			}
		}
		private void OnRemoveCanceled(InputAction.CallbackContext context) {
			_isDraggingRemove = false;
		}

		//---------- 조작(마우스 이동)
		private void OnCursorMove(InputAction.CallbackContext context) {
			_mouseScreenPos = context.ReadValue<Vector2>();

			//기존 포인터 갱신
			UpdateSimpleGridPointer();

			if (!IsPointerValid) return;

			if(_isDraggingBuild) {
				Vector3 currentMouseWolrdPos = GetWorldPositionFromMouse();

				//조건1. 데드존 체크.
				if(!_hasPassedDeadzone) {
					//안넘어갔다면
					float distFromClick = Vector3.Distance(_clickOriginWorldPos, currentMouseWolrdPos);
					if(distFromClick > _initialDragDeadzone) {
						//만약 클릭 범위가 데드존 범위 밖으로 넘어갔으면,
						//조건2로 넘어감. (타일 중심으로 기준 판정).
						_hasPassedDeadzone = true;
					} else {
						//아직 데드존 안이면 클릭판정 유지 (조건0 = 클릭판정)
						return;
					}
				} else {
					//만약 데드존 바깥의 범위라면
					//타일 중심으로 판정내기 위해 다음 메서드로 넘어감!
					ProcessDirectionalBuild(currentMouseWolrdPos);
				}
			} else if (_isDraggingRemove) { //삭제
				if (CurrentGridPointer != _lastGridPointer) {
					RoadSystem.Instance.RemoveRoad(CurrentGridPointer);
					_lastGridPointer = CurrentGridPointer;
				}
			}
		}


		//---------- 건설(판정)
		private void ProcessDirectionalBuild(Vector3 mousePos) {
			Vector3 lastTileCenter = new Vector3(_lastGridPointer.x + 0.5f, 0, _lastGridPointer.y + 0.5f);
			Vector3 diff = mousePos - lastTileCenter;
			//float distance = diff.magnitude;
			float squareDist = Mathf.Max(Mathf.Abs(diff.x), Mathf.Abs(diff.z));

			//미리보기 Mesh 만드는거 나중에 추가하기.

			//조건4. 일정 거리 이상 드래그 했을 경우.
			//여기서 원으로 했는데, 이러니까 대각선의 길이는 적당하나, 가로 세로의 길이는 적당하지 않음.
			//따라서 원이 아닌, 네모로 변경.
			if (squareDist >= _connectionDistanceThreshold) {
				//여기까지 들어왔으면 사실상 건설하면 됩니다.
				Vector2Int offset = CalculateSnappedDirection(diff);
				Vector2Int candidatePos = _lastGridPointer + offset;

				if(RoadSystem.Instance.IsRoadBuildable(candidatePos)) {
					RoadSystem.Instance.ConnectRoads(_lastGridPointer, candidatePos);

					_lastGridPointer = candidatePos;
					//hasPassedDeadzon은 false로 안합니다. 클릭했을때만 판정함.
				}
			}
		}

		//---------- 유틸
		private void UpdateSimpleGridPointer() {
			//마우스 좌표(그리드) 갱신
			Vector3 hitPoint = GetWorldPositionFromMouse();
			if(IsPointerValid) {
				int x = Mathf.FloorToInt(hitPoint.x);
				int y = Mathf.FloorToInt(hitPoint.z);
				CurrentGridPointer = new Vector2Int(x, y);
			}
		}

		private Vector3 GetWorldPositionFromMouse() {
			Ray ray = _mainCamera.ScreenPointToRay(_mouseScreenPos);
			if(_groundPlane.Raycast(ray, out float enter)) {
				//땅에 닿았음!! (인식된 범위 내)
				IsPointerValid = true;
				return ray.GetPoint(enter); //좌표 계산하고 반환.
			}
			//못닿았으면 그냥 0반환 (실패)
			IsPointerValid = false;
			return Vector3.zero;
		}

		//마우스가 이동한 좌표 (클릭->드래그) 벡터의 각도를 구하는거. 정규화 및 8방향으로 스냅핑합니다.
		private Vector2Int CalculateSnappedDirection(Vector3 diff) {
			// 각도 계산 (Atan2는 라디안 반환)
			// z를 y축으로 생각하여 계산
			float angle = Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg;

			// 음수 각도 보정 (0 ~ 360)
			if (angle < 0) angle += 360f;

			// 8방향 스냅 (45도씩 분할 -> 22.5도 오프셋으로 반올림 처리)
			// 0:East, 1:NE, 2:North, 3:NW, 4:West, 5:SW, 6:South, 7:SE
			int sector = Mathf.RoundToInt(angle / 45f) % 8;

			switch (sector) {
				case 0: return new Vector2Int(1, 0);   // East
				case 1: return new Vector2Int(1, 1);   // NE
				case 2: return new Vector2Int(0, 1);   // North
				case 3: return new Vector2Int(-1, 1);  // NW
				case 4: return new Vector2Int(-1, 0);  // West
				case 5: return new Vector2Int(-1, -1); // SW
				case 6: return new Vector2Int(0, -1);  // South
				case 7: return new Vector2Int(1, -1);  // SE
			}
			return Vector2Int.zero;
		}

		private void OnDrawGizmos() {
			if (Application.isPlaying && IsPointerValid) {
				Gizmos.color = Color.cyan;
				Vector3 drawPos = new Vector3(CurrentGridPointer.x + 0.5f, 0, CurrentGridPointer.y + 0.5f);
				Gizmos.DrawWireCube(drawPos, Vector3.one * 0.95f);

				// [Debug] 드래그 중일 때 기준점과 현재 마우스 위치 연결선 표시
				if (_isDraggingBuild) {
					Gizmos.color = Color.yellow;
					Vector3 lastCenter = new Vector3(_lastGridPointer.x + 0.5f, 0, _lastGridPointer.y + 0.5f);

					// 데드존 표시
					if (!_hasPassedDeadzone) {
						Gizmos.DrawWireSphere(_clickOriginWorldPos, _initialDragDeadzone);
					} else {
						Gizmos.color = Color.yellow;
						// 기준 타일에서 연결 임계값(Threshold) 범위 표시 (사각형)
						float size = _connectionDistanceThreshold * 2;
						Gizmos.DrawWireCube(lastCenter, new Vector3(size, 0.1f, size));

						// 현재 마우스 위치까지 선
						Gizmos.DrawLine(lastCenter, GetWorldPositionFromMouse());
					}
				}
			}
		}
	}
}
