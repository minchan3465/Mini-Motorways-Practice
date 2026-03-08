using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Models;
	using Motorways.Managers;

	public class ParkVehicleProcess : MonoBehaviour, ISimulationProcess {
		public static ParkVehicleProcess Instance;

		private List<CarPark> _activeCarparks = new List<CarPark>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		public void RegisterCarpark(CarPark carpark) {
			if (!_activeCarparks.Contains(carpark)) {
				_activeCarparks.Add(carpark);
			}
		}

		private void Start() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RegisterProcess(this);
			}
		}

		public void Tick(float dt) {
			foreach (var carpark in _activeCarparks) {
				ProcessParkingLogic(carpark, dt);
			}
		}

		private void ProcessParkingLogic(CarPark carpark, float dt) {
			List<int> currentVehicles = carpark.ParkingTimers.Keys.ToList();
			List<int> finishedVehicles = new List<int>();

			foreach(int vehicleId in currentVehicles) {
				float currentTime = carpark.ParkingTimers[vehicleId];
				float nextTime = currentTime - dt;
				carpark.ParkingTimers[vehicleId] = nextTime;
				if(nextTime <= 0) {
					finishedVehicles.Add(vehicleId);
				}
			}

			foreach(int vehicleId in finishedVehicles) {
				carpark.ReleaseVehicle(vehicleId);
				ReturnVehicleToHome(carpark, vehicleId);
			}
		}

		private void ReturnVehicleToHome(CarPark carpark, int vehicleId) {
			var vehicle = VehicleMovementProcess.Instance.GetVehicle(vehicleId);

			if (vehicle != null) {
				// [수정] 집으로 돌아가는 전용 메서드 호출
				vehicle.DispatchHome();
			}
		}
	}
}
