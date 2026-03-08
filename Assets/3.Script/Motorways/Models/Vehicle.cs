using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Models {

	public enum VehicleState {
		Ready,
		Driving,
		Arrived,
		Returning
	}

	public enum PathfindUrgency {
		NotRequired,
		WhenPossible
	}

	public class Vehicle : MonoBehaviour {
		public int Id { get; private set; }
		private static int _nextId = 0;
		
		public VehicleState State = VehicleState.Ready;

		public Vector2Int HomeNode { get; private set; }
		public Vector2Int DestNode { get; private set; }
		
		// [추가] 현재 목표가 목적지인지, 집인지 구분하기 위한 플래그
		public bool IsReturning { get; private set; } = false;

		public Queue<Lane> CurrentPath = new Queue<Lane>();
		public Queue<Lane> ReturnPath = new Queue<Lane>();
		public int CurrentLaneIndex = 0;

		public float DistanceAlongLane = 0f;
		public float CurrentSpeed = 0f;
		public float MaxSpeed = 2f * MapSettings.TILE_SIZE;
		public float TargetSpeed = 5f * MapSettings.TILE_SIZE;
		public float Acceleration = 3.5f * MapSettings.TILE_SIZE; //초당 속도 증가량
		public float Braking = 6.0f;     //초당 속도 감소량 (제동)

		public float ParkingTimer = 0f;
		public float ParkingDuration = 3.0f; //

		public PathfindUrgency RepathUrgency = PathfindUrgency.NotRequired;
		public int LatestAttemptedPathfindFrame = 0;

		public int BlockingVehicleId = -1; // 현재 이 차량을 막고 있는 차량 ID
		public bool IsShoving = false;      // 교차로 억지 진입 여부 (교착 상태 해소용)
		public float StuckTimer = 0f;       // 정지 상태 유지 시간

		public Vehicle() {
			Id = _nextId++;
		}

		// [추가] 차량 스폰 시 한 번만 호출되어 집 위치를 고정합니다.
		public void SetHome(Vector2Int homeNode) {
			HomeNode = homeNode;
		}

		public Lane GetCurrentLane() {
			if (CurrentLaneIndex < CurrentPath.Count) return CurrentPath.Peek();
			else return null;
		}

		// [수정] 목적지로 출발할 때 호출
		public void Dispatch(Vector2Int destNode) {
			DestNode = destNode;
			IsReturning = false;

			CurrentLaneIndex = 0;
			DistanceAlongLane = 0f;
			State = VehicleState.Ready;

			RequestPathfind();
		}

		// [추가] 집으로 돌아갈 때 호출
		public void DispatchHome() {
			IsReturning = true;

			CurrentLaneIndex = 0;
			DistanceAlongLane = 0f;
			State = VehicleState.Ready;

			RequestPathfind();
		}

		public Lane LastCommittedLane {
			get {
				if (CurrentPath == null || CurrentPath.Count == 0) return null;
				int remainingLanes = CurrentPath.Count - CurrentLaneIndex;

				if (remainingLanes > 1) { return CurrentPath.ElementAt(1); }
				return CurrentPath.Peek();
			}
		}

		public void AssignPath(List<Lane> newPathRemaining) {
			if(CurrentPath == null || CurrentPath.Count == 0) return; 

			int keepCount = Mathf.Min(2, CurrentPath.Count);
			List<Lane> keptLanes = new List<Lane>();
			
			for(int i = 0; i < keepCount; i++) {
				keptLanes.Add(CurrentPath.Dequeue());
			}

			while(CurrentPath.Count > 0) {
				Lane discarded = CurrentPath.Dequeue();
				discarded.CancelReservation(this.Id);
			}

			foreach (Lane lane in keptLanes) {
				CurrentPath.Enqueue(lane);
			}
			foreach (Lane newLane in newPathRemaining) {
				newLane.Reserve(this.Id);
				CurrentPath.Enqueue(newLane);
			}
		}

		public void AssignReturnPath(List<Lane> newReturnPath) {
			if(ReturnPath != null) {
				foreach (Lane lane in ReturnPath) {
					// 현재 진행 중인 경로에 포함되어 있다면 예약을 취소하지 않음 (U턴 등에서 예약 증발 방지)
					if (CurrentPath == null || !CurrentPath.Contains(lane)) {
						lane.CancelReservation(this.Id);
					}
				}
			}

			if (State == VehicleState.Returning && CurrentPath != null) {
				// Returning 상태일 때는 CurrentPath가 곧 귀환 경로이므로 여기선 건드리지 않습니다.
				// (원래 여기서 취소하는 로직이 있었으나, 중복/버그 방지를 위해 제거 또는 조건 변경)
			}

			ReturnPath = new Queue<Lane>(newReturnPath);
			foreach (Lane lane in ReturnPath) {
				lane.Reserve(this.Id);
			}
		}

		public void ClearAllReservations() {
			if (CurrentPath != null) {
				foreach (Lane lane in CurrentPath) {
					lane.CancelReservation(this.Id);
				}
				CurrentPath.Clear();
			}

			if (ReturnPath != null) {
				foreach (Lane lane in ReturnPath) {
					lane.CancelReservation(this.Id);
				}
				ReturnPath.Clear();
			}
		}
	}
}
