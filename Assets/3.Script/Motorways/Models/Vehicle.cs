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

		public Queue<Lane> CurrentPath = new Queue<Lane>();
		public Queue<Lane> ReturnPath = new Queue<Lane>();
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
			if (CurrentLaneIndex < CurrentPath.Count) return CurrentPath.Peek();
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

				if (remainingLanes > 1) { return CurrentPath.ElementAt(1); }
				return CurrentPath.Peek();
			}
		}

		//기존 경로의 일부분을 교체 및 병합하는 메서드.
		public void AssignPath(List<Lane> newPathRemaining) {
			if(CurrentPath == null || CurrentPath.Count == 0) return; 

			int keepCount = Mathf.Min(2, CurrentPath.Count);
			List<Lane> keptLanes = new List<Lane>();
			
			//확정된 차선 미리 빼두기.
			for(int i = 0; i < keepCount; i++) {
				keptLanes.Add(CurrentPath.Dequeue());
			}

			//나머지 경로 예약 취소
			while(CurrentPath.Count > 0) {
				Lane discarded = CurrentPath.Dequeue();
				discarded.CancelReservation(this.Id);
			}

			//확정 차선 다시 넣기
			foreach (Lane lane in keptLanes) {
				CurrentPath.Enqueue(lane);
			}

			//새로 찾은 경로 넣고 예약
			foreach (Lane newLane in newPathRemaining) {
				newLane.Reserve(this.Id);
				CurrentPath.Enqueue(newLane);
			}
		}

		public void AssignReturnPath(List<Lane> newReturnPath) {
			if(ReturnPath != null) {
				foreach (Lane lane in ReturnPath) {
					lane.CancelReservation(this.Id);
				}
			}

			ReturnPath = new Queue<Lane>(newReturnPath);

			foreach (Lane lane in ReturnPath) {
				lane.Reserve(this.Id);
			}
		}
	}
}
