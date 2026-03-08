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

	public class Vehicle : MonoBehaviour {
		public int Id { get; private set; }
		private static int _nextId = 0;
		
		public VehicleState State = VehicleState.Ready;

		public House HomeObject { get; private set; }
		public Destination TargetDestination { get; private set; }

		public Vector2Int HomeNode { get; private set; }
		public Vector2Int DestNode { get; private set; }

		public Queue<Lane> CurrentPath = new Queue<Lane>();
		public Queue<Lane> ReturnPath = new Queue<Lane>();
		public int CurrentLaneIndex = 0;

		public float DistanceAlongLane = 0f;
		public float CurrentSpeed = 0f;
		
		public float MaxSpeed = 2f * MapSettings.TILE_SIZE;
		public float TargetSpeed = 2f * MapSettings.TILE_SIZE;
		public float Acceleration = 3.5f * MapSettings.TILE_SIZE; 
		public float Braking = 6.0f;     

		public float ParkingTimer = 0f;
		public float ParkingDuration = 3.0f; 

		public bool NeedsPathfind = false;
		public int LatestAttemptedPathfindFrame = 0;
		public bool IsReturning = false;

		public Vehicle() {
			Id = _nextId++;
		}

		public static void ResetId() => _nextId = 0;

		public void SetHome(House home) {
			HomeObject = home;
			HomeNode = home.EntranceCoordinate;
		}

		public Lane GetCurrentLane() {
			if (CurrentPath != null && CurrentPath.Count > 0) return CurrentPath.Peek();
			else return null;
		}

		public void Dispatch(Destination destination) {
			TargetDestination = destination;
			DestNode = destination.EntranceCoordinate;
			IsReturning = false;

			CurrentLaneIndex = 0;
			DistanceAlongLane = 0f;
			State = VehicleState.Ready;

			RequestPathfind();
		}

		public void DispatchHome() {
			IsReturning = true;
			CurrentLaneIndex = 0;
			DistanceAlongLane = 0f;
			State = VehicleState.Ready;

			RequestPathfind();
		}

		public void RequestPathfind() {
			NeedsPathfind = true;
		}

		// [사용자님의 안전한 경로 갱신 로직 복구]
		public void AssignPath(List<Lane> newPathRemaining) {
			if (newPathRemaining == null) return;

			// 1. 현재 달리고 있는 차선(0번)은 유지하여 텔레포트를 방지합니다.
			Lane currentLane = null;
			if (CurrentPath.Count > 0) {
				currentLane = CurrentPath.Dequeue();
			}

			// 2. 나머지 대기 중인 차선들의 예약만 취소합니다.
			while (CurrentPath.Count > 0) {
				CurrentPath.Dequeue().CancelReservation(this.Id);
			}

			// 3. 현재 차선을 다시 넣고, 그 뒤로 새 경로를 잇습니다.
			if (currentLane != null) {
				CurrentPath.Enqueue(currentLane);
			}

			foreach (var lane in newPathRemaining) {
				// 중복 예약 방지
				if (lane != currentLane) {
					lane.Reserve(this.Id);
					CurrentPath.Enqueue(lane);
				}
			}
			NeedsPathfind = false;
		}

		public void AssignReturnPath(List<Lane> newReturnPath) {
			// 귀환 경로 예약 갱신 (고립 방지 핵심)
			ClearReturnPathReservations();
			ReturnPath = new Queue<Lane>(newReturnPath);
			foreach (var lane in ReturnPath) {
				lane.Reserve(this.Id);
			}
		}

		public void UseReturnPath() {
			IsReturning = true;
			CurrentLaneIndex = 0;
			DistanceAlongLane = 0f;
			
			// 이미 예약된 ReturnPath를 CurrentPath로 전환
			CurrentPath = new Queue<Lane>(ReturnPath);
			ReturnPath.Clear();
			
			if (CurrentPath.Count > 0) {
				State = VehicleState.Returning;
				CurrentPath.Peek().Enter(this.Id);
			} else {
				DispatchHome();
			}
		}

		public void ClearCurrentPathReservations() {
			if (CurrentPath != null) {
				foreach (Lane lane in CurrentPath) lane.CancelReservation(this.Id);
				CurrentPath.Clear();
			}
		}

		public void ClearReturnPathReservations() {
			if (ReturnPath != null) {
				foreach (Lane lane in ReturnPath) lane.CancelReservation(this.Id);
				ReturnPath.Clear();
			}
		}

		public void ClearAllReservations() {
			ClearCurrentPathReservations();
			ClearReturnPathReservations();
		}
	}
}
