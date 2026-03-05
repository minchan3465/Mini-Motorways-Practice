using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models {

	public class Destination : BuildingBase {
		public CarPark _CarPark { get; private set; }

		public int UnassignedPins { get; set; } // 아직 차량이 배정되지 않은 핀 수
		public int IncomingPins { get; set; }   // 차량이 오고 있는 핀 수

		public float PinSpawnTimer;     // 다음 핀 생성까지 남은 시간
		public float OverCrowdingTimer; // 과밀화(게임 오버)까지 남은 시간
		public bool isOverCrowding => (UnassignedPins + IncomingPins) > 6;

		public override void Initialize(int groupIndex, Vector2Int entranceCoord, BuildingLayout layout) {
			base.Initialize(groupIndex, entranceCoord, layout);
			Type = BuildingType.Destination;

			_CarPark = new CarPark();
			_CarPark.Initialize(this, IncomingLane.EndNode, IncomingLane.StartNode);

			UnassignedPins = 0;
			IncomingPins = 0;
			PinSpawnTimer = 10.0f;
			OverCrowdingTimer = 30.0f;
		}

		public override void OnVehicleArrived(int vehicleId) {
			if (_CarPark.TryParkVehicle(vehicleId, 2.0f)) {
				if (IncomingPins > 0) IncomingPins--;
				else if (UnassignedPins > 0) UnassignedPins--;
			}

		}
		public void ReserverPin() {
			if (UnassignedPins > 0) {
				UnassignedPins--;
				IncomingPins++;
			}
		}
	}
}
