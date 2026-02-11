using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Simulation.Buildings {
	using Core.Simulation.Vehicles;

	public class House : Building {
		public List<int> OwnedVehicleIds = new List<int>();				//보유한 차량 (2대)
		private HashSet<int> _dispatchedVehicles = new HashSet<int>();	//현재 출동한 차량
		public int AvailableVehicles => OwnedVehicleIds.Count - _dispatchedVehicles.Count;  //출동 가능한 차량 수

		public House(Vector2Int root, ColorType color) : base(root, color) { }
		public override void InitializeFootprint() {
			OccupiedCoordinates.Add(RootCoordinate);
		}

		public bool TryDispatchVehicle(out int vehicleId) {
			vehicleId = -1;
			foreach(int id in OwnedVehicleIds) {
				if(!_dispatchedVehicles.Contains(id)) {
					//출동한 차량이 아닐경우
					_dispatchedVehicles.Add(id);
					vehicleId = id;
					return true;
				}
			}
			return false;
		}

		public void VehicleReturned(int vehicleId) {
			if (_dispatchedVehicles.Contains(vehicleId)) {
				//출동한 차량이 돌아온거라면
				_dispatchedVehicles.Remove(vehicleId);
			}
		}

	}
}

