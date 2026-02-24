using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models {

	public class Destination : BuildingBase {
		public CarPark _CarPark { get; private set; }

		public int UnassignedPins { get; set; } //아직 대기중인 핀
		public int IncomingPins { get; set; }   //배차된 핀

		public float PinSpawnTimer;     //핀 생성 남은 시간
		public float OverCrowdingTimer; //과부하까지 남은 시간
		public bool isOverCrowding => (UnassignedPins + IncomingPins) > 6;

		public override void Initialize(int groupIndex, Vector2Int originCoord, BuildingLayout layout) {
			base.Initialize(groupIndex, originCoord, layout);
			Type = BuildingType.Destination;

			OccupiedCoordinates = new List<Vector2Int>();
			OccupiedCoordinates = new List<Vector2Int>();
			for (int x = 0; x < layout.Footprint.x; x++) {
				for (int y = 0; y < layout.Footprint.y; y++) {
					OccupiedCoordinates.Add(originCoord + new Vector2Int(x, y));
				}
			}

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

				//점수 추가.
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

