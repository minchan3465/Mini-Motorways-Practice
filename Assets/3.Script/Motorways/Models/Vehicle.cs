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

	public class Vehicle : MonoBehaviour {
		public int Id { get; private set; }
		private static int _nextId = 0;

		public VehicleState State = VehicleState.Ready;

		public List<Lane> CurrentPath = new List<Lane>();
		public List<Lane> ReturnPath = new List<Lane>();
		public int CurrentLaneIndex = 0;

		public float DistanceAlongLane = 0f;
		public float CurrentSpeed = 0f;
		public float MaxSpeed = 5f; // 기본 속도

		public float ParkingTimer = 0f;
		public float ParkingDuration = 2.0f; // 업무 시간

		public Vehicle() {
			Id = _nextId++;
		}

		public Lane GetCurrentLane() {
			if (CurrentLaneIndex < CurrentPath.Count) return CurrentPath[CurrentLaneIndex];
			else return null;
		}

		public void Dispatch(List<Lane> toDest, List<Lane> toHome) {
			CurrentPath = toDest;
			ReturnPath = toHome;

			CurrentLaneIndex = 0;
			DistanceAlongLane = 0f;
			State = VehicleState.Driving;

			foreach (var lane in CurrentPath) lane.Reserve(this.Id);
			foreach (var lane in ReturnPath) lane.Reserve(this.Id);
		}

		public void ClearPathReservations() {
			if(CurrentPath != null) {
				foreach(var lane in CurrentPath) {
					lane.Exit(this.Id);
				}
			}
		}
	}
}
