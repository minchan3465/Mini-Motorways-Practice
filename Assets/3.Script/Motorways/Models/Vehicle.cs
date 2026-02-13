using System.Collections;
using System.Collections.Generic;
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

		public List<Lane> CurrentPath = new List<Lane>();
		public List<Lane> ReturnPath = new List<Lane>();
		public int CurrentLaneIndex = 0;

		public float DistanceAlongLane = 0f;
		public float CurrentSpeed = 0f;
		public float MaxSpeed = 5f; // 기본 속도

		public float ParkingTimer = 0f;
		public float ParkingDuration = 2.0f; // 업무 시간

		//재탐색 요청 상태
		public PathfindUrgency RepathUrgency = PathfindUrgency.NotRequired;

		public Vehicle() {
			Id = _nextId++;
		}

		public Lane GetCurrentLane() {
			if (CurrentLaneIndex < CurrentPath.Count) return CurrentPath[CurrentLaneIndex];
			else return null;
		}

		public void Dispatch(Vector2Int homeNode, Vector2Int destNode) {
			HomeNode = homeNode;
			DestNode = destNode;

			CurrentLaneIndex = 0;
			DistanceAlongLane = 0f;

			State = VehicleState.Ready;
			RequestPathfind();
		}

		public void ClearPathReservations() {
			if(CurrentPath != null) {
				foreach(var lane in CurrentPath) {
					lane.Exit(this.Id);
				}
			}
		}

		public void RequestPathfind() {
			RepathUrgency = PathfindUrgency.WhenPossible;
		}

		public Lane LastCommittedLane {
			get {
				if (CurrentPath == null || CurrentPath.Count == 0) return null;
				int remainingLanes = CurrentPath.Count - CurrentLaneIndex;

				if (remainingLanes > 1) { return CurrentPath[CurrentLaneIndex + 1]; }
				return CurrentPath[CurrentLaneIndex];
			}
		}

		//기존 경로의 일부분을 교체 및 병합하는 메서드.
		public void AssignPath(List<Lane> newPathRemaining) {
			Lane committed = LastCommittedLane;
			if (committed == null) return;

			int committedIndex = CurrentPath.IndexOf(committed);	//이 Lane이 있는 번호를 찾습니다.
			if (committedIndex == -1) committedIndex = CurrentLaneIndex; // 예외 처리 (현재 차선 기준)
			
			for(int i = committedIndex +1; i < CurrentPath.Count; i++) {
				CurrentPath[i].InboundVehicles.Remove(this.Id);
			}

			int removeCount = CurrentPath.Count - (committedIndex + 1);
			if(removeCount > 0) {
				CurrentPath.RemoveRange(committedIndex + 1, removeCount);
			}

			//새 경로 이어 붙히기 + 새 도로 예약
			foreach (Lane newLane in newPathRemaining) {
				newLane.Reserve(this.Id);
				CurrentPath.Add(newLane);
			}
		}

		public void AssignReturnPath(List<Lane> newReturnPath) {
			if(ReturnPath != null) {
				foreach (Lane lane in ReturnPath) {
					lane.InboundVehicles.Remove(this.Id);
				}
			}

			ReturnPath = newReturnPath;

			foreach (Lane lane in ReturnPath) {
				lane.Reserve(this.Id);
			}
		}
	}
}
