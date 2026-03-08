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

		public Vector2Int HomeNode { get; private set; }
		public Vector2Int DestNode { get; private set; }

		public Queue<Lane> CurrentPath = new Queue<Lane>();
		public Queue<Lane> ReturnPath = new Queue<Lane>();
		public int CurrentLaneIndex = 0;

		public float DistanceAlongLane = 0f;
		public float CurrentSpeed = 0f;
		
		// [사용자님의 원본 수치 복구]
		public float MaxSpeed = 2f * MapSettings.TILE_SIZE;
		public float TargetSpeed = 2f * MapSettings.TILE_SIZE;
		public float Acceleration = 3.5f * MapSettings.TILE_SIZE; 
		public float Braking = 6.0f;     

		public float ParkingTimer = 0f;
		public float ParkingDuration = 3.0f; 

		public bool NeedsPathfind = false;
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

			RequestPathfind();
		}

		public void RequestPathfind() {
			NeedsPathfind = true;
		}

		public void AssignPath(List<Lane> newPath) {
			CurrentPath = new Queue<Lane>(newPath);
			foreach (var lane in CurrentPath) lane.Reserve(this.Id);
			NeedsPathfind = false;
		}

		public void AssignReturnPath(List<Lane> newReturnPath) {
			ReturnPath = new Queue<Lane>(newReturnPath);
			foreach (var lane in ReturnPath) lane.Reserve(this.Id);
		}

		public void ClearAllReservations() {
			if (CurrentPath != null) {
				foreach (Lane lane in CurrentPath) lane.CancelReservation(this.Id);
				CurrentPath.Clear();
			}
			if (ReturnPath != null) {
				foreach (Lane lane in ReturnPath) lane.CancelReservation(this.Id);
				ReturnPath.Clear();
			}
		}
	}
}
