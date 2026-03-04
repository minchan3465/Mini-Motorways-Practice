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
		//public Lane HomeIncomingLane { get; private set; }

		public Queue<Lane> CurrentPath = new Queue<Lane>();
		public Queue<Lane> ReturnPath = new Queue<Lane>();
		public int CurrentLaneIndex = 0;

		public float DistanceAlongLane = 0f;
		public float CurrentSpeed = 0f;
		public float MaxSpeed = 2f;
		public float TargetSpeed = 5f;
		public float Acceleration = 3.5f; // 초당 속도 증가량
		public float Braking = 6.0f;     // 초당 속도 감소량 (제동)

		public float ParkingTimer = 0f;
		public float ParkingDuration = 2.0f; //

		public PathfindUrgency RepathUrgency = PathfindUrgency.NotRequired;
		public int LatestAttemptedPathfindFrame = 0;

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

			//HomeIncomingLane = homeInLane;
			//if (HomeIncomingLane != null) {
			//	HomeIncomingLane.Reserve(this.Id);
			//}

			RequestPathfind();
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
					lane.CancelReservation(this.Id);
				}
			}

			if (State == VehicleState.Returning && CurrentPath != null) {
				foreach (Lane lane in CurrentPath) {
					lane.CancelReservation(this.Id);
				}
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
