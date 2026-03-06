using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models {

	public class Destination : BuildingBase {
		public CarPark _CarPark { get; private set; }

		public int UnassignedPins { get; set; } // 아직 차량이 배정되지 않은 핀 수
		public int IncomingPins { get; set; }   // 차량이 오고 있는 핀 수

		public const int MAX_PINS = 10;
		public const int GAUGE_START_PINS = 6;

		public float PinSpawnTimer;     // 다음 핀 생성까지 남은 시간
		public float OverCrowdingTimer; // 과밀화(게임 오버)까지 남은 시간
		public int TotalDemand => UnassignedPins + IncomingPins;
		public bool isOverCrowding => TotalDemand >= GAUGE_START_PINS;
		public bool isActive = false;

		public override void Initialize(int groupIndex, Vector2Int entranceCoord, BuildingLayout layout) {
			base.Initialize(groupIndex, entranceCoord, layout);
			Type = BuildingType.Destination;

			_CarPark = new CarPark();
			_CarPark.Initialize(this, IncomingLane.EndNode, IncomingLane.StartNode);

			UnassignedPins = 0;
			IncomingPins = 0;
			PinSpawnTimer = 10.0f;
			OverCrowdingTimer = 30.0f;
			isActive = true;

			// DemandProcess에서 관리하도록 변경
			Process.DemandProcess.Instance.RegisterDestination(this);
		}

		public override void OnVehicleArrived(int vehicleId) {
			// 차량이 도착하면 핀 하나를 해결
			if (IncomingPins > 0) {
				IncomingPins--;
			} else if (UnassignedPins > 0) {
				UnassignedPins--;
			}
			_CarPark.TryParkVehicle(vehicleId, 2.0f);
		}

		public void ReserverPin() {
			if (UnassignedPins > 0) {
				UnassignedPins--;
				IncomingPins++;
			}
		}
	}
}
