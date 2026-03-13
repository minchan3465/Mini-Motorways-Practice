using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models {

	public class Destination : BuildingBase {
		public CarPark _CarPark { get; private set; }

		public int UnassignedPins { get; set; } //아직 차량이 배정되지 않은 핀 수
		public int IncomingPins { get; set; }   //차량이 오고 있는 핀 수

		public const int MAX_PINS = 10;
		public const int GAUGE_START_PINS = 7;

		public float PinSpawnTimer;     //다음 핀 생성까지 남은 시간
		public float OverCrowdingTimer; //과밀화(게임 오버)까지 남은 시간
		public int TotalDemand => UnassignedPins + IncomingPins;
		public bool isOverCrowding => TotalDemand >= GAUGE_START_PINS;
		public bool isActive = false;

		public override void Initialize(int groupIndex, Vector2Int entranceCoord, BuildingLayout layout) {
			base.Initialize(groupIndex, entranceCoord, layout);
			Type = BuildingType.Destination;

			_CarPark = new CarPark();
			_CarPark.Initialize(this, IncomingLane.StartNode, IncomingLane.EndNode);

			UnassignedPins = 0;
			IncomingPins = 0;
			PinSpawnTimer = 10.0f;
			OverCrowdingTimer = 60.0f;
			isActive = true;

			//DemandProcess에서 관리하도록 변경
			Process.DemandProcess.Instance.RegisterDestination(this);
		}

		public override void OnVehicleArrived(int vehicleId) {
			//차량이 도착하면 핀 하나를 해결
			bool pinResolved = false;
			if (IncomingPins > 0) {
				IncomingPins--;
				pinResolved = true;
			} else if (UnassignedPins > 0) {
				UnassignedPins--;
				pinResolved = true;
			}

			if (pinResolved) {
				//핀을 해결했을 때만 점수 추가
				if (Managers.ScoreManager.Instance != null) {
					Managers.ScoreManager.Instance.AddScore(1);
				}
			}

			//차량이 도착하면 과밀화 타이머를 즉시 일정량(예: 20%) 회복시킵니다.
			//30초의 20%인 6초를 즉시 더해줍니다.
			OverCrowdingTimer = Mathf.Min(30.0f, OverCrowdingTimer + 6.0f);

			_CarPark.TryParkVehicle(vehicleId, 3.0f);
		}

		public void ReserverPin() {
			if (UnassignedPins > 0) {
				UnassignedPins--;
				IncomingPins++;
			}
		}
	}
}
