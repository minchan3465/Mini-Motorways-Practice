using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	using Core.Structure;

	//public enum BehaviorState {
	//	WaitingForDestination,  //집에서 대기 중
	//	DrivingToDestination,   //집 -> 목적지로 이동 중 (도로 주행 포함)
	//	ParkingAtDestination,   //목적지 입구 -> 주차장 진입 중
	//	ParkedAtDestination,    //목적지 도착 (업무 수행 중)
	//	DrivingHome,            //목적지 -> 집으로 복귀 중
	//	RealigningDriveway      //차량 위치 재정렬. (현재 제작 X)
	//}

	//조금 더 이름을 가볍게 만들기.
	public enum BehaviorState {
		Idle,                   // 대기
		DrivingToDestination,   // 출근 중
		ParkingAtDestination,   // 주차 진입
		Parked,                 // 업무 중
		DrivingHome             // 퇴근 중
	}


	//TODO : 상태 머신 추가로 인하여, 새롭게 갈아엎어야함. 
	//		ㄴ end.
	//TODO : 상태머신은 제끼고, 일단 경로 찾기에 관련한 시스템을 대규모 개편을 해야함. (Vector2Int -> Lane으로)
	//		ㄴ 길을 인식하는 과정을 Vector2Int에서 Lane으로 바꿈. (좌표와 비트마스크 -> 인접 그래프)
	public class CarMovement : MonoBehaviour {
		[Header("Movement Settings")]
		[SerializeField] private float _maxSpeed = 2.0f;
		[SerializeField] private float _rotationSpeed = 15.0f;

		[Header("State")]
		[SerializeField] private BehaviorState _currentState = BehaviorState.Idle;

		//그리드 좌표가 아닌 Lane으로 변경.
		private List<Lane> _currentPath = new List<Lane>(); // 현재 이동 중인 경로
		private List<Lane> _returnPath = new List<Lane>();  // 미리 확보한 퇴근 경로 (왕복 티켓)

		private int _pathIndex = 0;
		private Lane _currentLane;
		private float _distanceAlongLane = 0f;

		private House _home;
		private Destination _destination;

		private int _lastMapUpdateFrame = 0;

		private int _assignedSlotIndex = -1;
		private WaitForSeconds _workDuration = new WaitForSeconds(3.0f);

		//----------------------------------------

		//--- 초기화 ---
		public void Initialize(House house, Destination dest, List<Lane> toDest, List<Lane> toHome) {
			_home = house;
			_destination = dest;
			transform.position = _home.transform.position;

			_currentPath = toDest;
			_returnPath = toHome;

			RegisterPath(_currentPath);
			RegisterPath(_returnPath);

			_pathIndex = 0;
			if (_currentPath.Count > 0) {
				EnterLane(_currentPath[0], 0f);
			}
			SetState(BehaviorState.DrivingToDestination);

			_lastMapUpdateFrame = Time.frameCount;

			//SetPath(initialPath);
			//SetState(BehaviorState.DrivingToDestination);
		}
		//private void SetPath(List<Lane> newPath) {
		//	if (newPath == null || newPath.Count == 0) return;

		//	_path = newPath;
		//	_pathIndex = 0;

		//	// 첫 번째 도로에 차를 올림
		//	EnterLane(_path[0], 0f);

		//	_lastMapUpdateFrame = Time.frameCount;
		//}

		//--- 루프(이동) ---
		private void Update() {
			if (_currentState == BehaviorState.DrivingToDestination ||
				_currentState == BehaviorState.DrivingHome) {

				ProcessDriving();	
				UpdateRotation();
			}
		}
		
		//--- 주행 로직 ---
		private void ProcessDriving() {
			if (_currentLane == null) return;
			float moveStep = _maxSpeed * Time.deltaTime;
			// if (IsVehicleAhead(moveStep)) return;	//앞차 감지용
			_distanceAlongLane += moveStep;

			if (_distanceAlongLane >= _currentLane.Length) {
				// 초과한 거리만큼 다음 레인으로 넘김 (부드러운 연결)
				float overflow = _distanceAlongLane - _currentLane.Length;
				TryHotswapPath();

				// 다음 도로로 넘어가기
				AdvanceToNextLane(overflow);
			} else {
				//좌표 갱신인데, 보간하는것.
				float t = _distanceAlongLane / _currentLane.Length;
				transform.position = Vector3.Lerp(_currentLane.StartWorldPos, _currentLane.EndWorldPos, t);
			}
		}

		//--- Lane 환승 및 도착 처리.
		private void AdvanceToNextLane(float startDistance) {
			if (_currentLane != null) { 
				_currentLane.UnregisterVehicle(this);
				RoadSystem.Instance.CheckAndProcessMothballedLane(_currentLane);
			}
			_pathIndex++;

			//도착시.
			if(_pathIndex >= _currentPath.Count) {
				HandleArrival();
				return;
			}
			//다음 도로로.
			EnterLane(_currentPath[_pathIndex], startDistance);
		}
		private	void EnterLane(Lane lane, float startDist) {
			_currentLane = lane;
			_distanceAlongLane = startDist; //넘어온 거리만큼 보정합니다.

			//Initialize에서 이미 Register를 했으므로 여기서 중복 호출할 필요는 없지만,
			//Hotswap 등으로 경로가 바뀌었을 수 있으므로 안전하게 한번 더 호출해도 됨 (HashSet이라 중복 X)
			//근데 개인적으로는 안해보는게 더 나을듯...
			//_currentLane.RegisterVehicle(this);
		}

		private void HandleArrival() {
			_currentLane = null;

			if(_currentState == BehaviorState.DrivingToDestination) {
				SetState(BehaviorState.ParkingAtDestination);
			} else if (_currentState == BehaviorState.DrivingHome) {
				//ClearAllReservations();	//혹시 모를 예약 전체 해제.
				SetState(BehaviorState.Idle);
			}
		}
		
		//--- 도로 변경 (핫 스왑 : 주행 중 경로 변경한다는 뜻) --- ~> 원작의 방식을 따라가게끔 만들었다. 엄청 어렵당.
		private void TryHotswapPath() {
			if (RoadSystem.Instance.LatestMapUpdateFrame <= _lastMapUpdateFrame) return;

			//끝난 도로의 끝점에서, 목적지까지 다시 검색합니다.	(현재 도로가 시작이 되는건 아닙니다!)
			Vector2Int startNode = _currentLane.EndNode;
			Vector2Int targetNode = (_currentState == BehaviorState.DrivingToDestination)
									? _destination.EntranceCoordinate
									: _home.EntranceCoordinate;

			//드디어 PathFinder에게 Lane 리스트 요청합니다.
			List<Lane> betterPath = Pathfinder.FindLanePath(startNode, targetNode);
			if(betterPath != null) {
				//경로를 발견했다면!
				// 기존 경로의 남은 부분 예약 취소
				for (int i = _pathIndex + 1; i < _currentPath.Count; i++) {
					UnregisterLane(_currentPath[i]);
				}

				// 새 경로 예약 걸기
				RegisterPath(betterPath);

				// 경로 교체 (현재까지 온 길 + 새 길)
				List<Lane> newFullPath = new List<Lane>();
				for (int i = 0; i <= _pathIndex; i++) newFullPath.Add(_currentPath[i]);
				newFullPath.AddRange(betterPath);

				_currentPath = newFullPath;
				_lastMapUpdateFrame = Time.frameCount;
			}
		}

		//--- 상태 머신 ---
		public void SetState(BehaviorState newState) {
			_currentState = newState;

			switch (newState) {
				case BehaviorState.Idle:
					if (_home != null) _home.CarReturned(this);
					break;
				case BehaviorState.DrivingToDestination:
					//집에서 경로 목적지 지정해줄 때, 초기화 해주면서 경로 등록 하니 넘어갑시다.
					break;
				case BehaviorState.ParkingAtDestination:
					if(_destination != null) {
						_moveTargetPos = _destination.GetParkingPosition(out _assignedSlotIndex); // 구색 맞추기용
						SetState(BehaviorState.Parked); // 바로 업무 시작 (또는 이동 연출 추가 가능)
					}
					break;
				case BehaviorState.Parked:
					StartCoroutine(DoWork());
					break;
				case BehaviorState.DrivingHome:
					if (_returnPath != null && _returnPath.Count > 0) {
						_currentPath = _returnPath; // 경로 교체
						_returnPath = null;         // 사용했으니 비움
						_pathIndex = 0;

						EnterLane(_currentPath[0], 0f);
					} else {
						// 이론상 발생 불가 (Initialize에서 체크했으므로)
						Debug.LogError("CarMovement: 귀환 경로가 없습니다! (심각한 오류)");
					}
					break;
			}
		}

		private IEnumerator DoWork() {
			if (_destination != null) _destination.CarArrived();
			yield return _workDuration;
			if (_destination != null) _destination.ReleaseParkingSlot(_assignedSlotIndex);
			SetState(BehaviorState.DrivingHome);
		}

		//--- 유틸 ---
		private void RegisterPath(List<Lane> path) {
			if (path == null) return;
			foreach (var lane in path) {
				lane.RegisterVehicle(this); // Lane.VehiclesOnLane.Add
			}
		}
		private void UnregisterLane(Lane lane) {
			if (lane != null) {
				lane.UnregisterVehicle(this); // Lane.VehiclesOnLane.Remove
											  // 예약자가 0명이 되면 RoadSystem에게 "나 지워도 돼"라고 알림
				RoadSystem.Instance.CheckAndProcessMothballedLane(lane);
			}
		}
		private void ClearAllReservations() {
			if (_currentPath != null) {
				foreach (var l in _currentPath) UnregisterLane(l);
			}
			if (_returnPath != null) {
				foreach (var l in _returnPath) UnregisterLane(l);
			}
		}
		private void UpdateRotation() {
			if (_currentLane == null) return;

			Vector3 direction = (_currentLane.EndWorldPos - _currentLane.StartWorldPos).normalized;
			if (direction != Vector3.zero) {
				Quaternion targetRot = Quaternion.LookRotation(direction);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _rotationSpeed);
			}
		}

		// 이동용 임시 변수 (주차장 등 Lane이 아닌 곳에서의 이동을 위해 남겨둠)
		private Vector3 _moveTargetPos;
	}
}