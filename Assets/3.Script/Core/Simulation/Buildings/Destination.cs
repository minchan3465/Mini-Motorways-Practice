using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Simulation.Buildings {
	public class Destination : Building {
		public int CurrentPin { get; private set; }
		public int MaxPins = 6;

		public List<Vector2Int> ParkingSlots = new List<Vector2Int>();
		public HashSet<int> OccupiedSlots = new HashSet<int>();

		public int IncomingVehicles = 0;
		public bool NeedsVehicle => (CurrentPin - IncomingVehicles) > 0;

		public Destination(Vector2Int root, ColorType color) : base(root, color) { }

		public override void InitializeFootprint() {
			OccupiedCoordinates.Add(RootCoordinate);
			OccupiedCoordinates.Add(RootCoordinate + Vector2Int.right);
			OccupiedCoordinates.Add(RootCoordinate + Vector2Int.up);
			OccupiedCoordinates.Add(RootCoordinate + Vector2Int.one);

			ParkingSlots.Add(RootCoordinate);
			ParkingSlots.Add(RootCoordinate + Vector2Int.right);
		}

		public void AddPin() {
			CurrentPin++;
			//TODO : UI 갱신 이벤트 발생
			//TODO : MaxPins 초과 시 타이머 시작 로직.
		}

		public void VehicleArrived(int vehicleId) {
			if (CurrentPin > 0) {
				CurrentPin--;
			}
			OccupiedSlots.Add(vehicleId);
		}

		public void VehicleDeparted(int vehicleId) {
			OccupiedSlots.Remove(vehicleId);
		}

		public bool HasParkingSpace() {
			return OccupiedSlots.Count < ParkingSlots.Count;
		}
	}
}
