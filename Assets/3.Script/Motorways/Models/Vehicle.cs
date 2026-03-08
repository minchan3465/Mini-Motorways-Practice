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

		// [수정] 기존 예약을 확실히 지우고 새 경로 할당 (데이터 누수 방지)
		public void AssignPath(List<Lane> newPath) {
			ClearCurrentPathReservations();
			CurrentPath = new Queue<Lane>(newPath);
			foreach (var lane in CurrentPath) lane.Reserve(this.Id);
			NeedsPathfind = false;
		}

		// [수정] 기존 귀환 예약을 확실히 지우고 새 귀환 경로 할당
		public void AssignReturnPath(List<Lane> newReturnPath) {
			ClearReturnPathReservations();
			ReturnPath = new Queue<Lane>(newReturnPath);
			foreach (var lane in ReturnPath) lane.Reserve(this.Id);
		}

		// [추가] 미리 예약된 귀환 경로를 현재 주행 경로로 전환 (고립 방지 핵심)
		public void UseReturnPath() {
			IsReturning = true;
			CurrentLaneIndex = 0;
			DistanceAlongLane = 0f;
			
			// ReturnPath를 CurrentPath로 이동 (이미 예약되어 있으므로 추가 Reserve 불필요)
			CurrentPath = new Queue<Lane>(ReturnPath);
			ReturnPath.Clear();
			
			if (CurrentPath.Count > 0) {
				State = VehicleState.Returning;
				CurrentPath.Peek().Enter(this.Id);
			} else {
				// 경로가 정말 없다면 어쩔 수 없이 재탐색 요청
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
