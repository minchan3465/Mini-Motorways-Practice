using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	using Core.Utils;

	public class CarMovement : MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private float _maxSpeed = 3.0f;
		[SerializeField] private float _acceleration = 5.0f;
		[SerializeField] private float _deceleration = 5.0f;
		[SerializeField] private float _turnSmoothness = 15.0f;

		private List<Vector2Int> _gridPath;
		private int _currentIndex = 0;
		private bool _isMoving = false;

		private float _currentSpeed = 0f;

		// 왕복 시스템 변수
		private Vector2Int _homeLocation; // 집 좌표
		private bool _isReturning = false; // 현재 귀가 중인가?

		private Vector3 _startPos;
		private Vector3 _endPos;
		private Vector3 _controlPos;
		private bool _isCurving = false;
		private float _progressT = 0f;

		// 초기화 -> 집 좌표를 기억함
		public void Initialize(Vector2Int homePos) {
			_homeLocation = homePos;
		}

		public void SetPath(List<Vector2Int> path) {
			if (path == null || path.Count < 2) return;

			_gridPath = path;
			_currentIndex = 0;
			_currentSpeed = 0f;

			transform.position = GridToWorld(_gridPath[0]);

			SetupNextSegment();
			_isMoving = true;
		}

		private void Update() {
			if (!_isMoving) return;

			HandleSpeed();

			float segmentDistance = Vector3.Distance(_startPos, _endPos);
			//float segmentDistance = Vector3.SqrMagnitude(_endPos- _startPos);
			if (_isCurving) segmentDistance *= 1.2f;
			if (segmentDistance < 0.001f) segmentDistance = 1f;

			_progressT += (Time.deltaTime * _currentSpeed) / segmentDistance;

			Vector3 nextPos;
			Vector3 direction;

			//Vector3 nextPos = Vector3.Lerp(_startPos, _endPos, _progressT);
			//Vector3 direction = (_endPos - _startPos).normalized;

			if (_isCurving) {
				//곡선 이동 (Bezier) -> 적용
				nextPos = BezierUtils.GetPoint(_startPos, _controlPos, _endPos, _progressT);
				direction = BezierUtils.GetTangent(_startPos, _controlPos, _endPos, _progressT);
			} else {
				// 직선 이동
				nextPos = Vector3.Lerp(_startPos, _endPos, _progressT);
				direction = (_endPos - _startPos).normalized;
			}

			transform.position = nextPos;

			if (direction != Vector3.zero) {
				Quaternion targetRot = Quaternion.LookRotation(direction);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _turnSmoothness);
			}

			if (_progressT >= 1.0f) {
				_currentIndex++;

				if (_currentIndex >= _gridPath.Count - 1) {
					OnPathCompleted();
				} else {
					SetupNextSegment();
				}
			}
		}

		private void HandleSpeed() {
			int tilesRemaining = (_gridPath.Count - 1) - _currentIndex;

			if (tilesRemaining <= 1) _currentSpeed = Mathf.MoveTowards(_currentSpeed, 1.0f, _deceleration * _deceleration * Time.deltaTime);
			else _currentSpeed = Mathf.MoveTowards(_currentSpeed, _maxSpeed, _acceleration * Time.deltaTime);
		}

		private void SetupNextSegment() {
			_progressT = 0f;
			_startPos = transform.position;

			//혹시 모를 오류 ㅔㅊ크
			if (_currentIndex + 1 >= _gridPath.Count) return;

			//이거 하는 이유
			//곡선의 경우 중앙에 도착할 때 환벽한 1자 형태가 아닐 가능성이 높음.
			//또한 움직임도 부자연스럽게 꺾일바에, 차라리 베지어 곡선을 써서 A->B->C를 A->C처럼 자연스럽게 꺾이는걸 묘사.
			Vector2Int currentGrid = _gridPath[_currentIndex];
			Vector2Int nextGrid = _gridPath[_currentIndex + 1];
			Vector3 nextWorld = GridToWorld(nextGrid);

			if (_currentIndex + 2 < _gridPath.Count) {
				Vector2Int futureGrid = _gridPath[_currentIndex + 2];

				// 방향 계산
				Vector2Int dirToNext = nextGrid - currentGrid;
				Vector2Int dirToFuture = futureGrid - nextGrid;

				if (dirToNext != dirToFuture) {
					_isCurving = true;
					_controlPos = nextWorld; // 코너의 꼭짓점
					_endPos = (GridToWorld(nextGrid) + GridToWorld(futureGrid)) * 0.5f; // 다음 직선 도로의 진입점(중간)
					return;
				}
			}

			//직선 구간이거나 마지막 구간일 때
			_isCurving = false;
			//다음 타일의 중앙이 아니라, 다음 타일과 그 다음 타일 사이의 중간점까지 가야 부드러울 수 있음.
			//하지만 마지막 구간 등 예외가 많으므로, 직선일 땐 정직하게 타일 중앙(혹은 끝)으로 이동

			//만약 이전에 곡선 이동을 했다면, 현재 위치는 타일의 경계선(중간) 쯤일 것임.
			//거기서 다음 타일 중앙까지 직선으로 연결.
			_endPos = nextWorld;
		}

		private void OnPathCompleted() {
			_isMoving = false;

			if (!_isReturning) {
				// 목적지 도착 -> 집으로 복귀 명령
				Debug.Log("목적지 도착. 집으로 복귀합니다.");
				_isReturning = true;

				// 현재 위치(목적지)에서 집으로 가는 경로 계산
				Vector2Int currentGridPos = _gridPath[_gridPath.Count - 1];
				List<Vector2Int> returnPath = Pathfinder.FindPath(currentGridPos, _homeLocation);

				if (returnPath != null) {
					// 잠시 대기 후 출발하거나 바로 출발 (여기선 바로 출발)
					SetPath(returnPath);
				} else {
					Debug.LogError("집으로 돌아가는 길이 끊겼습니다!");
					//근데 이러면 안됨. 추후 수정 예정...
				}
			} else {
				// 집 도착 -> 주차 완료
				Debug.Log("집에 도착했습니다. (대기 모드)");
				// 실제 게임에서는 여기서 집에 '차량 가용 수'를 +1 해줍니다.
				// 지금은 테스트이므로 삭제하지 않고 멈춰둡니다.
			}
		}



		private Vector3 GridToWorld(Vector2Int gridPos) {
			return new Vector3(gridPos.x + 0.5f, 0, gridPos.y + 0.5f);
		}
	}
}