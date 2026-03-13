using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models {
	public class CarPark {
		public Destination Owner { get; private set; }

		public int Capacity { get; private set; }

		public Lane EntranceLane { get; private set; }
		public Lane ExitLane { get; private set; }

		public List<int> ParkedVehicles { get; private set; } = new List<int>();
		public Dictionary<int, float> ParkingTimers { get; private set; } = new Dictionary<int, float>();

		public Vector2Int CarparkCoordinate { get; private set; }
		public Vector2Int RoadCoordinate { get; private set; } //도로와 연결된 좌표

		//--- 초기화 ---
		public void Initialize(Destination owner, Vector2Int roadCoord, Vector2Int carparkCoord) {
			Owner = owner;
			RoadCoordinate = roadCoord;
			CarparkCoordinate = carparkCoord;
			Capacity = 3; //원작 일반 기준

			//1. 들어오는 길 (도로 -> 주차장)
			EntranceLane = new Lane(roadCoord, carparkCoord);

			//2. 나가는 길 (주차장 -> 도로)
			ExitLane = new Lane(carparkCoord, roadCoord);

			ParkedVehicles.Clear();
		}

		public bool TryParkVehicle(int vehicleId, float duration) {
			if (ParkedVehicles.Count >= Capacity) return false; //주차 자리 없음.

			//차량을 주차장에 추가
			if (!ParkedVehicles.Contains(vehicleId)) {
				ParkedVehicles.Add(vehicleId);
				ParkingTimers[vehicleId] = duration;
			}
			return true;
		}

		public void ReleaseVehicle(int vehicleId) {
			if (ParkedVehicles.Contains(vehicleId)) {
				ParkedVehicles.Remove(vehicleId);
				ParkingTimers.Remove(vehicleId);
			}
		}
	}
}
