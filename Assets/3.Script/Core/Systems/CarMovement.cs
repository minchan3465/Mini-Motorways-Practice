using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	//using Core.Utils;
	using Core.Data;
	using Core.Systems.Structure;

	public enum BehaviorState {
		WaitingForDestination,  // 집에서 대기 중
		DrivingToDestination,   // 집 -> 목적지로 이동 중 (도로 주행 포함)
		ParkingAtDestination,   // 목적지 입구 -> 주차장 진입 중
		ParkedAtDestination,    // 목적지 도착 (업무 수행 중)
		DrivingHome,            // 목적지 -> 집으로 복귀 중
		RealigningDriveway      // (미구현) 도로가 끊기거나 입구가 바뀌었을 때 경로 재탐색. 고립된 상태가 절대 아님.
	}
	//TODO : 상태 머신 추가로 인하여, 새롭게 갈아엎어야함. checked. end.

	public class CarMovement : MonoBehaviour {
		[Header("Movement Settings")]
		[SerializeField] private float _speed = 2.0f;
		[SerializeField] private float _rotationSpeed = 15.0f;
		[SerializeField] private float _arrivalThreshold = 0.05f;

		[Header("State Info")]
		[SerializeField] private BehaviorState _currentState = BehaviorState.WaitingForDestination;

		public House OwnerHouse;
		private StructureBase _startStructure;
		private StructureBase _targetStructure;

		//경로 관련...
		private List<Vector2Int> _currentPath;
		private List<Vector2Int> _returnPath; //복귀 경로 (미리 확보)
		private int _pathIndex = 0;
		private int _lastPathfindFrame = 0;

		private Vector3 _moveTargetPos; //이동 목표

		private int _assignedSlotIndex = -1;
		private WaitForSeconds _workDuration = new WaitForSeconds(3.0f);

		//------------------------------------------------------------------------------------------

		//--- 초기화 ---
		private void Start() {
			// 매니저에 등록 (최적화)
			if (StructureManager.Instance != null) StructureManager.Instance.RegisterCar(this);
		}
		private void OnDestroy() {
			if (StructureManager.Instance != null) StructureManager.Instance.UnregisterCar(this);
		}
		public void Initialize(House house, Destination dest, List<Vector2Int> initialPath) {
			OwnerHouse = house;
			_startStructure = house;
			_targetStructure = dest;

			SetPath(initialPath);

			transform.position = _startStructure.transform.position;
			SetState(BehaviorState.DrivingToDestination);
		}


		//--- 이동 ---
		private void Update() {
			if (_currentState == BehaviorState.DrivingToDestination ||
				_currentState == BehaviorState.DrivingHome ||
				_currentState == BehaviorState.ParkingAtDestination) {

				MoveAndRotate();
			}
		}
		private void MoveAndRotate() {
			float step = _speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, _moveTargetPos, step);

			Vector3 dir = (_moveTargetPos - transform.position).normalized;
			if (dir != Vector3.zero) {
				Quaternion targetRot = Quaternion.LookRotation(dir);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _rotationSpeed);
			}

			if (Vector3.SqrMagnitude(_moveTargetPos - transform.position) < _arrivalThreshold) {
				OnWaypointReached();
			}
		}


		//--- 차량의 상태 관리들 ---
		private void OnWaypointReached() {
			switch (_currentState) {
				case BehaviorState.DrivingToDestination:
					HandleDriving(isReturnTrip: false);
					break;

				case BehaviorState.DrivingHome:
					HandleDriving(isReturnTrip: true);
					break;

				case BehaviorState.ParkingAtDestination:
					HandleParking();
					break;
			}
		}

		private void HandleDriving(bool isReturnTrip) { 
			//방금 지나온 타일을 반납합니다.
			//건물 인덱스가 아니며, 인덱스가 유효할때 (= 도로 위)
			if (_currentPath != null && _pathIndex > 0 && _pathIndex < _currentPath.Count) {
				RoadSystem.Instance.NotifyRelease(_currentPath[_pathIndex - 1]);
			}

			if (!isReturnTrip && _returnPath == null) {
				if (_pathIndex >= _currentPath.Count - 2) {
					SecureReturnPath(); // 미리 계산 및 예약
				}
			}

			//입구로 들어가는 그런게 아닌, 도로 위라면.
			_pathIndex++;

			if (_pathIndex >= _currentPath.Count) {
				//도로 끝 -> 건물 진입
				if (!isReturnTrip) SetState(BehaviorState.ParkingAtDestination);
				else SetState(BehaviorState.WaitingForDestination);
			} else {
				//안전장치. 이동하기 전에 맵이 바뀐지 확인합시다.
				if (!isReturnTrip && _returnPath == null) TryRepathIfMapChanged();
				SetupMoveTargetToNextTile();
			}
		}
		private void HandleParking() {
			// 주차 슬롯에 도착했으므로 업무 시작
			SetState(BehaviorState.ParkedAtDestination);
		}
		private IEnumerator HandleWorking() {
			if (_targetStructure is Destination dest) {
				dest.CarArrived();
				//차량은 계속해서 목적지로 왕복을 할 테니, 매번 new로 만들 필요 없이 캐싱합시다.
				yield return _workDuration;
				dest.ReleaseParkingSlot(_assignedSlotIndex);
				SetState(BehaviorState.DrivingHome);
			}
		}

		public void SetState(BehaviorState newState) {
			_currentState = newState;

			switch (_currentState) {
				case BehaviorState.WaitingForDestination:   // 집에서 대기중... (집 도착)
					OnReturnedHome();
					break;
				case BehaviorState.DrivingToDestination:
					_pathIndex = 0;
					SetupMoveTargetToNextTile();
					break;
				case BehaviorState.ParkingAtDestination:    //목적지에 입장,주차
					if (_targetStructure is Destination dest) {
						_moveTargetPos = dest.GetParkingPosition(out _assignedSlotIndex);
					}
					break;
				case BehaviorState.ParkedAtDestination:     //목적지에 도착 처리
					StartCoroutine(HandleWorking());
					break;
				case BehaviorState.DrivingHome:             //집으로 복귀~
					SwapStructures();
					if (_returnPath != null) {
						_currentPath = _returnPath;
						_returnPath = null; //돌아가는건 이제 지워줍니다.
						_pathIndex = 0;
						_lastPathfindFrame = Time.frameCount;
						// 예약(NotifyReservation)은 이미 SecureReturnPath에서 했으므로 다시 안 합니다.
						SetupMoveTargetToNextTile();
					} else {
						//만약 예약된 경로가 없다면 (긴급 재탐색 필요...)
						Debug.LogWarning("[치명적인 오류] 누락! 긴급 탐색 시도중.");
						List<Vector2Int> emergencyPath = Pathfinder.FindPath(_startStructure.EntranceCoordinate, _targetStructure.EntranceCoordinate, true);
						if (emergencyPath != null) {
							SetPath(emergencyPath);
							SetupMoveTargetToNextTile();
						} else {
							// 진짜 고립 (이동 불가)
							Debug.LogWarning("[치명적인 오류] ㅋㅋ ㅈ됨. 길 없음.");
							SetState(BehaviorState.RealigningDriveway);
						}
					}
					break;
			}
		}

		//--- 경로, 예약 관련. (유틸)
		private void SetupMoveTargetToNextTile() {
			if (_currentPath != null && _pathIndex < _currentPath.Count) {
				Vector2Int gridPos = _currentPath[_pathIndex];
				_moveTargetPos = new Vector3(gridPos.x + 0.5f, 0, gridPos.y + 0.5f);
			}
		}
		private void SecureReturnPath() {
			if (_returnPath != null) return;    //이미 돌아갈 길이 있으면.

			//Active 우선
			_returnPath = Pathfinder.FindPath(_targetStructure.EntranceCoordinate, _startStructure.EntranceCoordinate, false);

			//없으면 삭제 대기 도로 포함 (고립 방지)
			if (_returnPath == null) {
				_returnPath = Pathfinder.FindPath(_targetStructure.EntranceCoordinate, _startStructure.EntranceCoordinate, true);
			}

			//[핵심] 찾은 즉시 예약 (Overlap)...
			//현재 타고 있는 도로의 예약이 풀리기 전에, 돌아갈 길의 예약을 먼저 건다.
			if (_returnPath != null) {
				foreach (var tile in _returnPath) {
					RoadSystem.Instance.NotifyReservation(tile);
				}
			}
		}
		private void SetPath(List<Vector2Int> newPath) {
			_currentPath = newPath;
			_pathIndex = 0;
			_lastPathfindFrame = Time.frameCount;

			if (_currentPath != null) {
				foreach (var tile in _currentPath) {
					RoadSystem.Instance.NotifyReservation(tile);
				}
			}

			SetupMoveTargetToNextTile();
		}
		private void TryRepathIfMapChanged() {
			if (RoadSystem.Instance.LatestMapUpdateFrame <= _lastPathfindFrame) return;

			Vector2Int currentPos = _currentPath[_pathIndex];
			Vector2Int targetPos = (_currentState == BehaviorState.DrivingToDestination)
								   ? _targetStructure.EntranceCoordinate
								   : _startStructure.EntranceCoordinate;

			List<Vector2Int> newSuffixPath = Pathfinder.FindPath(currentPos, targetPos, allowMothballed: false);
			if (newSuffixPath != null) {
				ReleaseFutureReservations();
				_currentPath = newSuffixPath;
				_pathIndex = 0;
				_lastPathfindFrame = Time.frameCount;
				for (int i = 1; i < _currentPath.Count; i++) {
					RoadSystem.Instance.NotifyReservation(_currentPath[i]);
				}
			} else {
				_lastPathfindFrame = Time.frameCount;
			}
		}

		private void ReleaseAllReservations() {
			if (_currentPath != null) {
				for (int i = _pathIndex; i < _currentPath.Count; i++) {
					RoadSystem.Instance.NotifyRelease(_currentPath[i]);
				}
			}
			if (_returnPath != null) {
				foreach (var tile in _returnPath) {
					RoadSystem.Instance.NotifyRelease(tile);
				}
			}
		}
		private void ReleaseFutureReservations() {
			if (_currentPath != null) {
				for (int i = _pathIndex + 1; i < _currentPath.Count; i++) {
					RoadSystem.Instance.NotifyRelease(_currentPath[i]);
				}
			}
		}

		//--- 상태 관련 (유틸) ---
		private void OnReturnedHome() {
			ReleaseAllReservations();
			if (OwnerHouse != null) OwnerHouse.CarReturned(this);
			else Destroy(gameObject);
		}
		private void SwapStructures() {
			//var로 받는 이유. -> 그냥!
			var temp = _startStructure;
			_startStructure = _targetStructure;
			_targetStructure = temp;
		}
	}
}