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
				// [롤백] 객체 참조 방식인 DispatchHome을 제거하고, 태초의 좌표 기반 Dispatch로 복구합니다.
				// 목적지(carpark)에서 집(homeNode)으로 가도록 설정합니다.
				vehicle.Dispatch(carpark.CarparkCoordinate, vehicle.HomeNode);
			}
		}
	}
}
