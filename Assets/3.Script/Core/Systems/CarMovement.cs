using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	//using Core.Utils;
	using Core.Systems.Structure;

	public enum BehaviorState {
		WaitingForDestination,  // 집에서 대기 중
		DrivingToDestination,   // 집 -> 목적지로 이동 중 (도로 주행 포함)
		ParkingAtDestination,   // 목적지 입구 -> 주차장 진입 중
		ParkedAtDestination,    // 목적지 도착 (업무 수행 중)
		DrivingHome,            // 목적지 -> 집으로 복귀 중
		RealigningDriveway      // (미구현) 도로가 끊기거나 입구가 바뀌었을 때 경로 재탐색
	}
	//TODO : 상태 머신 추가로 인하여, 새롭게 갈아엎어야함. checked. end.

	public class CarMovement : MonoBehaviour {
		[Header("Movement Settings")]
		[SerializeField] private float _speed = 2.0f;
		[SerializeField] private float _rotationSpeed = 15.0f;
		[SerializeField] private float _arrivalThreshold = 0.05f;
		//[SerializeField] private float _laneOffset = 0.15f; // 우측 통행 오프셋 값

		[Header("State Info")]
		[SerializeField] private BehaviorState _currentState = BehaviorState.WaitingForDestination;

		public House OwnerHouse;
		private StructureBase _startStructure;
		private StructureBase _targetStructure;
		private List<Vector2Int> _gridPath;

		private int _gridPathIndex = 0;

		private Vector3 _currentTargetPos;
		private bool _isExitingBuilding = false;
		private bool _isEnteringHouse = false;

		// 초기화 -> 집 좌표를 기억함
		public void Initialize(House house, Destination dest, List<Vector2Int> roadPath) {
			OwnerHouse = house;
			_startStructure = house;
			_targetStructure = dest;
			_gridPath = roadPath;

			transform.position = _startStructure.transform.position;

			SetState(BehaviorState.DrivingToDestination);
		}
		private void Update() {
			if (_currentState.Equals(BehaviorState.WaitingForDestination) ||
				_currentState.Equals(BehaviorState.ParkedAtDestination)) return;

			MoveAndRotate();
		}

		public void SetState(BehaviorState newState) {
			_currentState = newState;

			switch (_currentState) {
				case BehaviorState.WaitingForDestination:   // 집에서 대기중... (집 도착)
					OnReturnedHome();
					break;
				case BehaviorState.DrivingToDestination:    //목적지까지 운전~
					_gridPathIndex = 0;
					_isExitingBuilding = true;  //건물에서 나오는것부터 시작
					SetupExitingPath(_startStructure);
					break;
				case BehaviorState.ParkingAtDestination:    //목적지에 입장,주차
					SetupEnteringPath(_targetStructure);
					break;
				case BehaviorState.ParkedAtDestination:     //목적지에 도착 처리
					OnParkedAtDestination();
					break;
				case BehaviorState.DrivingHome:             //집으로 복귀~
					SwapStructures();
					if (CalculateReturnPath()) {
						_gridPathIndex = 0;
						_isExitingBuilding = true;
						SetupExitingPath(_startStructure);
					} else {
						Debug.Log("아잇 씻팔! 돌아갈 길이 없잖아~ 죽을게.");
						Destroy(gameObject);
					}
					break;
				case BehaviorState.RealigningDriveway:
					//현재 미구현.
					//나중에 차가 막혔을때 대기하는 상태일수도 있음.
					break;
			}
		}

		//---------------------- 차량 움직이는 코어 로직...
		private void MoveAndRotate() {
			//이동
			float step = _speed * Time.deltaTime;   //속도
			transform.position = Vector3.MoveTowards(transform.position, _currentTargetPos, step);  //이동

			//방향
			Vector3 dir = (_currentTargetPos - transform.position).normalized;
			if (dir != Vector3.zero) {
				Quaternion lookRot = Quaternion.LookRotation(dir);
				transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * _rotationSpeed); //부드럽게 회전
			}

			//차량의 위치가 목표 지점까지 도달했는가?
			//_arrivalThreshold = 거리
			//여기서 Distance가 아닌, sqrMagnitute를 쓰는 이유 : 연산이 더 빠름!
			//이유는 제곱근 하는 연산이 생각보다 연산이 무겁다고 하네요.
			if (Vector3.SqrMagnitude(_currentTargetPos - transform.position) < _arrivalThreshold) {
				OnTargetReached();
			}
		}

		private void OnTargetReached() {
			OnSegmentComplete();
		}

		//구간 완료 로직이 상태 기반으로 결정됩니다.
		private void OnSegmentComplete() {
			//만약 건물에서 도로 입구까지 나가는 단계였을 경우.
			if (_isExitingBuilding) {
				_isExitingBuilding = false; //상태 교환에서 true였으니, 이제 false로...
				SetupNextRoadSegment();         //다음 도로로 이동합시다~~
				return;
			}

			switch (_currentState) {
				case BehaviorState.DrivingToDestination:
					_gridPathIndex += 1;
					if (_gridPathIndex >= _gridPath.Count - 1) {
						// 도로 끝 -> 목적지 진입
						SetState(BehaviorState.ParkingAtDestination);
					} else {
						SetupNextRoadSegment();
					}
					break;
				case BehaviorState.DrivingHome:
					// [Fix] 집 내부로 들어가는 단계였는가?
					if (_isEnteringHouse) {
						// 집 중앙 도착 완료 -> 대기 상태로 전환
						_isEnteringHouse = false;
						SetState(BehaviorState.WaitingForDestination);
						return;
					}

					// 도로 주행 중
					_gridPathIndex += 1;

					if (_gridPathIndex >= _gridPath.Count - 1) {
						// 집 입구(도로 끝) 도착 -> 집 내부로 진입 명령
						_isEnteringHouse = true; // 플래그 ON
						SetupEnteringPath(_targetStructure);
					} else {
						SetupNextRoadSegment();
					}
					break;
				case BehaviorState.ParkingAtDestination:
					//주차중이였는데, 끝난거니 주차 완료 상태로 가십쇼
					SetState(BehaviorState.ParkedAtDestination);
					break;
			}
		}

		//-----------------경로 관련 메서드들 (상태)

		//건물에서 나가는 메서드
		private void SetupExitingPath(StructureBase from) {
			//건물 중심 -> 입구
			Vector2Int entrance = from.EntranceCoordinate;
			//Vector3 entranceWorld = new Vector3(entrance.x + 0.5f, 0, entrance.y + 0.5f);
			_currentTargetPos = new Vector3(entrance.x + 0.5f, 0, entrance.y + 0.5f);
		}

		//다음 도로의 위치를 받고(찾고) 이동합니다.
		private void SetupNextRoadSegment() {
			// 다음 타일 좌표 가져오기
			Vector2Int nextTile = _gridPath[_gridPathIndex + 1];
			_currentTargetPos = new Vector3(nextTile.x + 0.5f, 0, nextTile.y + 0.5f);

			//하단은 우측으로 이동하는걸 표현한건데, 나중에 사용할 예정.
			/*
			Vector2Int currentTile = _gridPath[_gridPathIndex];
			Vector2Int nextTile = _gridPath[_gridPathIndex + 1];

			Vector3 p0 = new Vector3(currentTile.x + 0.5f, 0, currentTile.y + 0.5f);
			Vector3 p1 = new Vector3(nextTile.x + 0.5f, 0, nextTile.y + 0.5f);

			Vector3 dir = (p1 - p0).normalized;
			// 오른쪽 벡터 (외적) 
			Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;

			// 최종 목표: 다음 타일 중앙 + 오른쪽으로 살짝 비켜난 곳
			_currentTargetPos = p1 + (right * _laneOffset);
			*/
		}

		//건물에 들어갑니다잇~
		private void SetupEnteringPath(StructureBase to) {
			if (to is Destination dest) {
				_currentTargetPos = dest.GetParkingPosition();
			} else {
				_currentTargetPos = to.transform.position;
			}
			//Vector3 targetCenter = to.transform.position;
			//_currentWaypoints = new List<Vector3> { targetCenter };
			//_waypointIndex = 0;
		}

		

		//----------------- 로직 처리...

		//목적지에 도착(주차 완료) -> 이제 집으로 돌아가기
		private void OnParkedAtDestination() {
			if (_targetStructure is Destination dest) {
				dest.CarArrived();
			}

			SetState(BehaviorState.DrivingHome);
		}

		//집에 돌아왓으니 삭제~ (추후 오브젝트 풀링으로 변경해야함).
		private void OnReturnedHome() {
			if (OwnerHouse != null) OwnerHouse.CarReturned(this);
			else Destroy(gameObject);
		}

		private void SwapStructures() {
			//var로 받는 이유. -> 그냥!
			var temp = _startStructure;
			_startStructure = _targetStructure;
			_targetStructure = temp;
		}

		//돌아가는 길을 계산해봅시다.
		//추후 나중에는 return false는 없고, 무조건 차량이 지나가고 도로가 사라지게 해야함.
		private bool CalculateReturnPath() {
			List<Vector2Int> path = Pathfinder.FindPath(_startStructure.EntranceCoordinate, _targetStructure.EntranceCoordinate);
			if (path != null) {
				_gridPath = path;
				return true;
			}
			return false;
		}
	}
}